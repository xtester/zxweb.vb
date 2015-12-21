' Creation Date: 2013/9/11
' Copyright (c) Xun Zhang
' www.zhangxun.com

Namespace zxweb.xtag

'*******************************************************************************
'	Default is level 0 (no blanks)
'

Public Class IndentConverter
 	Inherits BlankConverter
 	
Public Sub New()
	newHelper("indent") 
	setArgOptional()	
End Sub


Protected Overrides Function getBlanks(level As Integer) As String
	Dim rslt As String = ""	
	
	If level >= 0 Then
		rslt = MyBase.getBlanks(level * 2)
	End If
	
	Return rslt
End Function
 
End Class 'IndentConverter

End Namespace