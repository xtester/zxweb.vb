
Imports zx.common.xml

NameSpace zxweb.core

'*******************************************************************************
'	Create and manage template blocks for views and controls.
'

Public Class TemplateFactory
	
Private Shared _tfile As XmlFile
Private Shared Function getFile() As XmlFile
	Dim rslt As XmlFile = _tfile
	
	If rslt Is Nothing Then 
		_tfile = XmlFile.loadReady( _
			AppHome.mapPath("/_templates/templates.xml")) 'XX should create?
		rslt = _tfile
	End If
	
	Return rslt
End Function


'***************
'	Services
'
'***************

' Support layered	
'
Shared Public Function getTemplate( _
	Optional name As String = "default" _
) As String
	Dim rslt As String = ""	

Try	
	'XX	
	Dim tfile As XmlFile = XmlFile.loadReady( _
		AppHome.mapPath("/_templates/templates.xml"))
	
	rslt = tfile.readXml("/root/" & name)
Catch ex As Exception
End Try

	Return rslt
End Function


Shared Public Function getTemplateNode( _
	name As String _
) As XmlElement
	Dim rslt As XmlElement = Nothing
	
	If name = "" Then Return Nothing
	
	Try		
		rslt = getFile().getElement("/root/" & name) 'XX root tag
	Catch ex As Exception		
	End Try	
		
	Return rslt 
End Function 


Shared Public Function existTemplate(
	name As String _
) As Boolean
	Dim rslt As Boolean = False
	
	If getTemplateNode(name) IsNot Nothing Then rslt = True
	
	Return rslt 	
End Function

End Class 'TemplateFactory

End Namespace