
NameSpace zxweb.domain

'
Public Interface IComment

	Property Content As String
	Property ContentLength As Integer
	Property Subject As String
	Property Id As String
	Property Time As String
	Property LastEditTime As String
	Property UsrName As String
	Property SName As String
	Property VisitCount As String
	Property Category As String
	
	Function isUpdated() As Boolean
	Function isInserted() As Boolean
	
	Function genHtmlHeader(Optional isPrintSubject As Boolean = False) As String
	Function getTransformedContent() As String
		
End Interface 'IComment

End Namespace 