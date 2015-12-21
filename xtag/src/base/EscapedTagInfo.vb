' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/4/27

Namespace zxweb.xtag

'*******************************************************************************
'

Public Class EscapedTagInfo
	
Private _old As String
Public Property Old() As String
	Get
		Return _old
	End Get
	Set
		_old = value
	End Set
End Property


Private _corrected As String	
Public Property Corrected() As String
	Get
		Return _corrected
	End Get
	Set
		_corrected = value
	End Set
End Property


Public Sub New(ByRef intag As AbstractTag)
	Old = intag.getWhole()
	
	init()
End Sub


Private Sub init()
	If String.IsNullOrEmpty(Old) Then Return
	
	Dim posEscapeSign As Integer = Old.IndexOf("^") 'XX or =^ 
	If posEscapeSign > 0 Then
		Corrected = Old.Remove(posEscapeSign, 1)
	End If
End Sub

End Class 'EscapedTagInfo

End Namespace