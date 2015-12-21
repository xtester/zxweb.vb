
Namespace zxweb.xtag

'*******************************************************************************
'

Public Class RefConverter
	Inherits XTagConverter

Public Sub New()
	MyBase.New("ref") 
End Sub

	
Protected Overrides Function transform( _
	ByRef middle As String _
) As String
	Dim news As String = ""
	
	'XTrace.show("RefConverter.transform", "attribute", middle)
	If middle <> "" Then
		news = "<div class='ref'>" & middle & "</div>"						
	End If
	
	'XTrace.show("RefConverter.transform", "result", news)
	Return news		
End Function
	
End Class 'RefConverter

End Namespace