' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/5/23

Namespace zxweb.xtag

'*******************************************************************************
'

Public Class LineListConverter
	Inherits HtmlConverter
	
Public Sub New( _
	tkey As String _
)
	MyBase.New(tkey, tkey, True)	
End Sub


Protected Overrides Function transformTag( _
	Optional ByRef outIsRsltOk As Boolean = True _
) As String
	Dim rslt As String = ""
	Dim akey As String = getKey()
	Dim body As String = CurrentTag.Body	
	
	If body <> "" Then			
		'XX
		Dim deli As String = "@|@~&" 'XX
		Dim temp As String = body.Replace("<br />", deli) 'XX
		temp = temp.Replace(vbCrLf, 	deli) 	
		
		Dim linesList As ArrayList = Util.parseValues(temp, deli)
		XTrace.show("LineListConverter.transformTag", _
			"Line count", linesList.Count)
		
		For i As Integer = 0 To linesList.Count - 1
			If i = 0 OrElse i = linesList.Count - 1 Then Continue For
			
			Dim lstr As String = linesList(i)
			
			rslt &= "<li>" & lstr & "</li>"
		Next
		
		Dim style As String = _
			" style=""margin-top: -10px; margin-bottom: -28px;"""
		rslt = "<" & akey & style & ">" & rslt & "</" & akey & ">"	
	Else
		XTrace.warn("LineListConverter.transformTag", "body empty")
		outIsRsltOk = False		
	End If
	
	Return rslt
End Function 'transformTag
	
End Class 'LineListConverter

End Namespace