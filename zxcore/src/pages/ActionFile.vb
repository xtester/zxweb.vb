Namespace zxweb.controllers
	
'*********************************************************
'	Class: ActionFile
'	

Public Class ActionFileAdapter
	Implements IObjectMeta	
	
Private _xfile As XmlFile
		
Public Sub New(ByRef xf As XmlFile)	
	_xfile = xf	
End Sub


Protected Overridable Function getPath(keyword As String) As String
	Return "/root/action[@name='" & keyword & "']/controller"	
End Function

	
Public Function getClassName(keyword As String) As String _
		Implements IObjectMeta.getClassName			
	Return _xfile.readXml(getPath(keyword))
End Function
	
	
Public Function getAssembly(keyword As String) As String _
		Implements IObjectMeta.getAssembly			
	Return _xfile.readNodeAttribute(getPath(keyword), "assembly") 
End Function
	
End Class 'ActionFile

End Namespace