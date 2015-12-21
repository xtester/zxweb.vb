
Namespace zxweb.xtag

'*******************************************************************************
'

Public Class CssClassConverter
	Inherits XTagConverter

Public Sub New(key As String)
	MyBase.New(key)
End Sub

	
Protected Overrides Function transform( _
	ByRef middle As String _
) As String
	Dim news As String = ""
	
	XTrace.show("CssClassConverter.transform", "attribute", middle)
	If middle <> "" Then
		news = "<div class='" & getKey() & "'>" & middle & "</div>"						
	End If
	
	XTrace.show("CssClassConverter.transform", "result", news)
	Return news		
End Function
	
End Class 'CssClassConverter

End Namespace