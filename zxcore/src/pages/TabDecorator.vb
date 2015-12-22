
NameSpace zxweb

'*******************************************************************************
'	decorate page part names (tabs)
'

Public Class TabDecorator
	
Private _xfile As XmlFile
Private _sname As String

Private _baseName As String
Public Function getBaseName() As String
	Return _baseName
End Function

Private _style As String
Public Sub setStyle(s As String) 
	_style = s
End Sub


'*****************
'	Life cycle
'
'*****************

Public Sub New(ByRef f As XmlFile, n As String, sn As String)	
	_xfile = f
	_baseName = n
	_sname = sn
	
	init()
End Sub


Protected Sub init()
	Dim ts As String
	
	ts = _xfile.readAttribute(getBaseName(), "style")
	setStyle(ts)
End Sub


'***************
'	Services
'
'***************

' Return: prefix for Tab title
'
Public Function genPrefix( _
	ctx As String, _
	Optional isUseBlanks As Boolean = True _
) As String
	Dim rslt As String
	Dim xnode As XmlNode 

Try	 
	xnode = _xfile.readNodes("//" & getBaseName() & "//" & ctx).Item(0)	
	If isBypassed(xnode) Then Return ""	

	Dim level As Integer
	rslt = getIndex(xnode, level) & " "		
	If isMetaRoot(xnode.ParentNode) Then 
		If _style <> "compact" Then
			rslt = "第 " & rslt & " 章 "	
		End If
	End If
	
	Dim blanks As String = ""
	If isUseBlanks Then
		For i As Integer = 1 To level
			blanks &= "&nbsp;&nbsp;&nbsp;&nbsp;"
		Next	
	End If
	
	rslt = blanks & rslt
Catch ex As Exception
	rslt = ""
End Try

	Return rslt
End Function 'genPrefix


Protected Function isBypassed(ByRef nd As XmlNode) As Boolean
	Dim flag As String = XmlFile.readAttribute(nd, "prefix") 'XX useful?
	Return Util.isDisabled(flag)
End Function


Private Function isMetaRoot(ByRef nd As XmlNode) As Boolean
	Dim rslt As Boolean
	
	rslt = (String.Compare(nd.LocalName, getBaseName(), True) = 0)
	Return rslt
End Function


Private Function getIndex(ByRef xnode As XmlNode, ByRef level As Integer) As String
	Dim rslt, name As String 
	rslt = ""
	Dim pointer As Integer = -1
	Dim loc As Integer = 1
	
Try	
	If isMetaRoot(xnode) Then Return ""
	
	name = xnode.LocalName
	
	Dim container As XmlNode = xnode.ParentNode
	Dim nl As XmlNodelist = container.ChildNodes
	For Each peer As XmlNode In nl
		If peer.LocalName = name Then
			pointer = loc
		End If 'XX Break
		
		If Not isBypassed(peer) Then 
			loc += 1
		End If
	Next	
	
	If pointer > 0 Then	
	' found		
		If Not isMetaRoot(container) Then
			level += 1
			rslt = getIndex(container, level) & "." & pointer
		Else
			rslt = pointer
		End If		
	End If
Catch ex As Exception
End Try
	
	Return rslt
End Function 'getIndex	


Public Function getTabPath( _
	ByRef tn As XmlNode, _
	ByRef parent As IContentProvider _
) As String
	Dim rslt, url, prefix As String
	rslt = ""
	Dim level As Integer

Try	
	If IsMetaRoot(tn) Then Return ""

	prefix = getIndex(tn, level)
	url = "<a href='/?sname=" & _sname & "&section=" & tn.LocalName & _
		"' title='" & prefix & " " & parent.getTitleByPartName(tn.LocalName) & _
		" (" & tn.LocalName & ")'>" & prefix & "</a>" 'XX
	rslt = getTabPath(tn.ParentNode, parent) & " / " & url
Catch ex As Exception
	XTrace.show("TabDecorator.getTabPath", ex)
End Try

	Return rslt
End Function 'getTabPath


Public Function getTabNode(tag As String) As XmlNode
	Dim rslt As XmlNode
	Dim path As String = "//" & getBaseName() & "//" & tag

	'XTrace.show("TabDecorator.getTabPath", "to read tag", path)
	rslt = _xfile.readNodes(path).Item(0)	
	Return rslt
End Function

End Class 'TabDecorator

End Namespace