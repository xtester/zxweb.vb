' Creation Date: 2013/9/11
' Copyright (c) Xun Zhang
' www.zhangxun.com
'

Namespace zxweb.xtag

'*******************************************************************************
' 	Default is 0 blanks.
'

Public Class BlankConverter
	Inherits XTagConverter

	
Public Sub New()
	MyBase.New("blank") 
	setNewWay(True)
	setArgOptional()	
End Sub


Protected Overridable Function getBlanks(total As Integer) As String
	Dim rslt As String = ""	
	
	For i As Integer = 1 To total
		rslt &= "&nbsp;&nbsp;"
	Next
	
	Return rslt
End Function


Protected Overrides Function transformTag( _
	Optional ByRef outIsRsltOk As Boolean = True _
) As String
	Dim rslt As String = CurrentTag.Body
	
	If CurrentTag.Argstr = "" Then 
		If CurrentTag.Body = "" Then
			rslt = "" ' Remove the tag
		End If
		Return rslt
	End If
	
	Dim total As Integer = Util.getInt(getParam(), 0)	
	Dim blanks As String = getBlanks(total)
	'XTrace.show("BlankConverter.transform", "blanks", total & "[" & blanks & "]")
	
	If CurrentTag.Body = "" Then
		rslt = blanks & CurrentTag.getParam(2) 'XX
	Else
		'?? why
		rslt = blanks & CurrentTag.Body.Replace("<br />"， "<br />" & blanks)
	End If
	
	'XTrace.show("BlankConverter.transform", "result", news)	
	Return rslt		
End Function
	
End Class 'BlankConverter

End Namespace