
Namespace zxweb.xtag

'*******************************************************************************
'

Public Class NoteConverter
	Inherits XTagConverter

Public Sub New()
	MyBase.New("note")
End Sub


' middle: text, const
'
Protected Overrides Function transform( _
	ByRef middle As String _
) As String
	Dim rslt As String
	
	rslt = "<span class='red_text'>" & middle & "</span>"			
	Return rslt		
End Function
	
End Class 'NoteConverter

End Namespace