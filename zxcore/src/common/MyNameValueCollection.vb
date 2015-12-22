' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2014/7/6

Namespace zx.common

'*******************************************************************************
'	For HTTP parameters.
'

Public Class MyNameValueCollection
	Inherits NameValueCollection
	
Public Shared Function copy( _
	ByRef nvc As NameValueCollection _
) As NameValueCollection
	If nvc Is Nothing Then Return Nothing
	
	Dim rslt As NameValueCollection = New MyNameValueCollection
	
	For i As Integer = 0 To nvc.Count-1
		rslt.Add(nvc.Keys(i), nvc.Item(i))
	Next
	
	Return rslt	
End Function


Protected Function getString(Optional sign As String = "&") As String
	Dim rslt As String = ""
	
	For i As Integer = 0 To Count-1
		rslt &= Keys(i) & "=" & Item(i) 
		
		If i < Count-1 Then
			rslt &= sign
		End If
	Next
	
	Return rslt
End Function


Public Overrides Function ToString() As String
	Return getString()
End Function

End Class 'MyNameValueCollection

End Namespace