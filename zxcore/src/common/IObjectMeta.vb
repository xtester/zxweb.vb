
Namespace zx.common
	
'*******************************************************************************
'	Interface: IObjectMeta
'

Public Interface IObjectMeta
	
	Function getClassName(keyword As String) As String
	
	Function getAssembly(keyword As String) As String
	
End Interface

End Namespace