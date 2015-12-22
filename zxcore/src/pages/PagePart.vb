
NameSpace zxweb.domain

'*******************************************************************************
'

Public Class PagePart

Private _sname As String
Private _part As String

Private _xfile As XmlFile ' the main content
Private _handler As IContentProvider


Public Sub New( _
	sn As String, _
	pt As String _
)
	_sname = sn
	_part = pt
	
	'XX as field
	Dim xresrc As IWebResource = _
		AppHome.getFactoryResource().createInstance(_sname)
	_handler = xresrc.getContentHandler()	
	_xfile = xresrc.getXmlFile() 			
End Sub	
	
	
Public Shared Function getPath( _
	part As String, _
	Optional isAbsolute As Boolean = False, _
	Optional attrib As String = "" _
) As String
	Dim prefix As String = IIf(isAbsolute, "/root/", "")	
	Dim rslt As String = IIf(part = "", prefix & "main_body", _
		prefix & "main_body/page[@name='" & part & "']")
	
	If attrib <> "" Then
		rslt &= "/@" & attrib
	End If
	
	Return rslt
End Function


'XX 
Public Function readAttribute( _
	attrib As String, _
	Optional defv As String = "", _
	Optional isReadUp As Boolean = True _
) As String
	XTrace.showRun("PagePart.readAttribute", "'" & attrib & "' for part " & _part)
	Dim rslt As String	
	If attrib = "" Then 
		Return ""
	End If
	
	rslt = _xfile.readNodeAttribute(getPath(_part, True, attrib)) 'absolute 'XX rm
	If rslt = "" Then
		XTrace.warn("PagePart.readAttribute"£¬"First read rslt empty")
		rslt = _handler.getPartAttribute2(_part, attrib, defv, isReadUp) 'recursive
	End If
	
	XTrace.show("PagePart.readAttribute", "rslt", rslt)
	Return rslt
End Function


Public Function getTitle() As String
	Dim rslt As String = readAttribute("text", "", False) ' in dirtree
	
	If rslt = "" Then 
		rslt = readAttribute("Tab", "", False)
		If rslt = "" Then 
			rslt = readAttribute("tab", "", False)
		End If
	End If
			
	Return rslt
End Function


' 
Public Function readContent() As String
	Dim rslt, fn, bstrReadSection As String
	rslt = ""
	XTrace.showRun("PagePart.readContent")
	
	'!! before fn and setting _xfile
	bstrReadSection = readAttribute("importSection", "yes")
	
	fn = readAttribute("filename")	
	If fn <> "" Then
		_xfile = XmlFile.loadReady(DoorKeeper.MapPath(fn))				
	End If	
	XTrace.show("PagePart.readContent", "Use content file", _xfile.getRelFilePath())
	
	Dim inpart As String = _part
	If Not Util.isEnabled(bstrReadSection) Then
		'XTrace.show("PagePart.readContent", "Read section", _part)		
		inpart = ""
	End If	
	Dim fullp As String = getPath(inpart, True)
	
	XTrace.showRun("PagePart.readContent", "To read node: " & fullp)		
	Dim xnode As XmlElement = _xfile.getElement(fullp)
	rslt = _handler.process(xnode)
	
	Return rslt
End Function 'readContent

End Class 'PagePart

End Namespace