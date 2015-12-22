
NameSpace zxweb

'XX split

Public Interface IMultiplePages

	Function getTotalPage() As Integer
	
	Function readPageTabs() As String()
	
	Function getSrcUrl() As String
	
	Function readCurPageNo() As Integer
	
	Function renderNavigator(name As String) As Boolean
	
	Function computeDisplayRange( _
		ByRef starti As Integer, _
		ByRef endi As Integer _	
	) As Boolean

	'Function getDirectoryLink() As String
	
End Interface 


Public Interface IPagedControl

	Function getItemTotal() As Integer
	
	Sub initForPaging(ByRef adapter As IMultiplePages)
	
	Function getBase() As ControlBase
	
	Function getItemPerPage() As Integer
	
End Interface

End Namespace 