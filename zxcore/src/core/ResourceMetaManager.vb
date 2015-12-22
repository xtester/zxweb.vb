
Namespace zxweb.core
	
'*******************************************************************************	
'

Public Class ResourceProviderFile
	
Private _xmlRoot As String
Public ReadOnly Property XmlRoot() As String
	Get
		Return _xmlRoot
	End Get
End Property


Private _xfile As XmlFile
Public ReadOnly Property Xfile() As XmlFile
	Get
		Return _xfile
	End Get
End Property


Private _tag As String
Public ReadOnly Property Tag() As String
	Get
		Return _tag
	End Get
End Property


'XX
Public Sub New( _
	ByRef xmf As XmlFile	, _
	Optional ins As String = "/root", _
	Optional intag As String = "target" _
)	
	_xfile = xmf
	_xmlRoot = ins
	_tag = intag
End Sub	

End Class 'ResourceProviderFile

	
'*******************************************************************************	
'	Provides SName mapping and meta-data of xresources.
'
'	Todos:
'		- rename?
'

Public Class ResourceMetaManager
	Implements IMetaProvider
	
'XX rm	
Private Shared s_mainFile As XmlFile
	
Private Shared s_providers As New ArrayList	
	
Private Shared _singleton As ResourceMetaManager


'*****************
'	Life cycle
'
'*****************

'XX null exception?
Private Sub New()
	s_mainFile = XmlFile.load2(SysConfig.getResourcesFilePath()) 
	s_providers.Add(New ResourceProviderFile(s_mainFile))
	
	init()
End Sub


Private Sub init()
Try	
	XTrace.showRun("ResourceMetaManager.init")
	If s_mainFile Is Nothing Then 
		Return 'Error
	End If
		
	' Load more files
	Dim fileNodeList As XmlNodeList = s_mainFile.readNodes("/root/more/file")
	If fileNodeList Is Nothing OrElse fileNodeList.Count < 1 Then
		Return	
	End If
	
	For i As Integer = 0 To fileNodeList.Count - 1
		Try
			Dim fpath As String = fileNodeList(i).Attributes("path").Value
			Dim xmlPath As String = fileNodeList(i).Attributes("xmlPath").Value
			Dim tag As String = 	fileNodeList(i).Attributes("tag").Value
			XTrace.show("ResourceMetaManager.init", "fpath", fpath, _
				"xmlPath", xmlPath)
			
			Dim rfile As XmlFile = XmlFile.load2(fpath)
			If rfile IsNot Nothing Then
				s_providers.Add(New ResourceProviderFile(rfile, xmlPath, tag))				
		 	End If
		Catch iex As Exception
		End Try
	Next
Catch ex As Exception
	XTrace.show("ResourceMetaManager.init", ex)	
End Try

	XTrace.show("ResourceMetaManager.init", "s_moreFiles", s_providers.Count)	
	XTrace.showEnd("ResourceMetaManager.init")
End Sub


' thread safe?
Public Shared Function getInstance() As ResourceMetaManager		
	If _singleton Is Nothing OrElse s_mainFile Is Nothing _
			OrElse s_mainFile.isUpdated() Then
		_singleton = New ResourceMetaManager	
	End If
	
	'XX check null xfile
	
	Return _singleton 
End Function


'***************
'	Services
'
'***************

'@@ tested
'
' Whether the sname is defined.
'
' Used by:
'		CommentFile XX
'
'XX support alias
'
Public Function existSName( _
	sname As String, _
	Optional isCheckAlias As Boolean = False _
) As Boolean _
		Implements IMetaProvider.existSName
	Dim rslt As Boolean = False
	
Try				
	If sname = "" OrElse s_providers.Count < 1 Then Return False
	
	For i As Integer = 0 To s_providers.Count - 1
		Try
			Dim provider As ResourceProviderFile = s_providers(i)
			Dim pbase As String = provider.XmlRoot
			Dim ss As String = pbase & "/" & provider.Tag & _
				"[@sname='" & sname & "']"
			
			rslt = provider.Xfile.existNode(ss)
			
			If Not rslt Then
				rslt = provider.Xfile.existNode(ss.ToLower())	
			End If		
			
			If isCheckAlias And Not rslt Then
				Dim newSName As String = ""
				rslt = existProperty(newSName, "alias", sname) 
			End If
			
			If rslt Then Exit For
		Catch ex As Exception
		End Try
	Next	
Catch ex As Exception
	XTrace.show("ResourceMetaManager.existSName", ex)		
End Try

	Return rslt
End Function 'existSName


'XX
Public Function findSecuredResources() As String _
		Implements IMetaProvider.findSecuredResources
	Dim rslt As String = s_mainFile.readXml("/root/secured")
	Return rslt	
End Function


'@@ tested
'
'!! sname: may be alias, changed back to the original sname when return'
'	inkey: can be leveled, parent/child1/child2
'
Public Function find( _
	ByRef sname As String, _
	inkey As String _
) As String _
		Implements IMetaProvider.find
   	Dim rslt As String = ""	
	If sname = "" Or inkey = "" Then Return "" 
	
Try		
	For i As Integer = 0 To s_providers.Count - 1
		Try
			XTrace.showRun("ResourceMetaManager.find", "Try provider" & (i + 1))	
			
			Dim provider As ResourceProviderFile = s_providers(i)
			Dim xfile As XmlFile = provider.Xfile
			Dim pbase As String = provider.XmlRoot & "/" & provider.Tag
			
			Dim spath As String = pbase & "[@sname='" & sname & "']"
			Dim spath2 As String = spath.ToLower()
			
			If xfile.existNode(spath) Then
				rslt = xfile.readInnerText(spath & "/" & inkey)
				
			ElseIf xfile.existNode(spath2) Then
				rslt = xfile.readInnerText(spath2 & "/" & inkey)
				sname = sname.ToLower() '??
				
			Else			
				' try alias
				Dim apath As String = pbase & "[alias='" & sname & "']" 'XX
				
				If xfile.existNode(apath) Then
					'!! correct sname
					sname = xfile.readNodeAttribute(apath & "/@sname") 'XX sname = "" ?
					rslt = xfile.readInnerText(apath & "/" & inkey)			
				Else
					' try lower case
					Dim apath2 As String = pbase & _
						"[alias='" & sname.ToLower() & "']" 'XX
					
					If xfile.existNode(apath2) Then
						'!! correct sname
						sname = xfile.readNodeAttribute(apath2 & "/@sname") 'XX sname = "" ?
						rslt = xfile.readInnerText(apath2 & "/" & inkey)			
					End If
				End If
			End If
			
			If rslt <> "" Then 
				Exit For
			Else
				XTrace.warn("ResourceMetaManager.find", "rslt empty")
			End If
		Catch ex As Exception
			XTrace.show("ResourceMetaManager.find>loop", ex)		
		End Try
	Next
	
	If rslt = "" Then
		XTrace.warn("ResourceMetaManager.find", _
			"Warning", sname & "/" & inkey & " not found")
	Else
		XTrace.warn("ResourceMetaManager.find", "Found value", rslt)
	End If
Catch ex As Exception
	XTrace.show("ResourceMetaManager.find", ex)		
End Try	

	Return rslt
End Function 'find


Public Function getNode( _
	sname As String, _
	key As String _
) As XmlElement _
		Implements IMetaProvider.getNode
	Dim rslt As XmlElement = Nothing
	
	Dim spath As String = "/root/target[@sname='" & sname & "']/" & key
	rslt = s_mainFile.getElement(spath)
	
	Return rslt
End Function


'@@ tested
'
' Return: relative path
'
' Used by: 
'	XResource
'
Public Function getViewPath(sname As String) As String _
		Implements IMetaProvider.getViewPath
	Dim defaultv, rslt, viewName As String
	rslt = "" : defaultv = "" : viewName = ""
	
Try
	defaultv = ViewManager.getDefaultViewPath()
	If sname = "" Then Return defaultv
	
	' location file overrides	
	viewName = ViewManager.adjustView(find(sname, "template"), True) 	
	
	If viewName = "" Then
		' path read from DB, old style
		rslt = AppHome.getInstance().SYSPDB.readTableField( _
			"Locations", "SNAME", sname, "SHOWGATE") 'XX
		
		If rslt = "" Then 
			rslt = defaultv
		End If
	Else
		rslt = "/_templates/tmpl_" & viewName & ".aspx" '!!XX
		
		If Not File.Exists(AppHome.mapPath(rslt)) Then
			Dim viewHome As String = SysConfig.getViewHome()			
			rslt = viewHome & "/" & SysConfig.getViewPrefix() & viewName & _
				".aspx"
		End If
	End If
Catch ex As Exception
	XTrace.show("ResourceMetaManager.getViewPath", ex)
	rslt = defaultv 'error
End Try

	XTrace.show("ResourceMetaManager.getViewPath", "sname", sname, "rslt", rslt)
	Return rslt
End Function 'getViewPath	


' No change
'
' Return: block name
'
Public Function getViewTemplate(sname As String) As String _
		Implements IMetaProvider.getViewTemplate
	Dim rslt As String = "" 
	
Try
'	defaultv = ViewManager.getDefaultViewPath()
	If sname = "" Then Return ""
	
	rslt = find(sname, "template")
Catch ex As Exception
	XTrace.show("ResourceMetaManager.getViewTemplate", ex)
End Try

	XTrace.show("ResourceMetaManager.getViewTemplate", _
		"sname", sname, "rslt", rslt)
	Return rslt	
End Function	


'@@ tested by utIMetaProvider
'
'XX more files?
'
Public Function getAllResources() As ArrayList _
		Implements IMetaProvider.getAllResources
	Dim rslt As New ArrayList '= Nothing
	
	Dim nodeList As XmlNodeList = s_mainFile.readNodes("/root/target")
	'If nodeList.Count > 0 Then rslt = New ArrayList
	For Each xnode As XmlElement In nodeList
		'Dim nn As String = xnode.LocalName		
		Dim snm As String = xnode.GetAttribute("sname")			
		rslt.Add(snm)	
	Next
	
	Return rslt
End Function


'@@ tested
'
' inoutSName: can be ""
'
Public Function existProperty( _
	ByRef inoutSName As String, _
	ppty As String, _
	value As String _
) As Boolean _
		Implements IMetaProvider.existProperty
	Dim rslt As Boolean = False
	If ppty = "" Then Return False
	
	For i As Integer = 0 To s_providers.Count - 1
		Try
			'XTrace.showRun("ResourceMetaManager.find", "Try provider" & (i + 1))	
			
			Dim provider As ResourceProviderFile = s_providers(i)
			Dim pbase As String = provider.XmlRoot & "/" & provider.Tag
			
			Dim condition As String = ppty & "='" & value & "'"
			condition = IIf(inoutSName = "", condition, "@sname='" & _
				inoutSName & "' and " & condition)
			Dim searchPath As String = pbase & "[" & condition & "]"
			'XTrace.show("ResourceMetaManager.existProperty", "To search", searchPath)
			
			rslt = provider.Xfile.existNode(searchPath.ToLower()) '!!
			'XTrace.show("ResourceMetaManager.existProperty", "rslt", rslt)
			
			If rslt And inoutSName = "" Then 
				' Correct the sname
				inoutSName = provider.Xfile.readNodeAttribute( _
					searchPath.ToLower(), "sname")
			End If
			
			If rslt Then Exit For
		Catch ex As Exception		
			XTrace.show("ResourceMetaManager.existProperty>for", ex)
		End Try	
	Next
	
	Return rslt
End Function 'existProperty

End Class 'ResourceMeta

End Namespace