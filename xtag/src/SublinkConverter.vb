' Creation Date: 2013/11/2
' Copyright (c) Xun Zhang
' www.zhangxun.com

Namespace zxweb.xtag

'*******************************************************************************
'

Public Class SublinkConverter	
	Inherits XTagConverter

Public Sub New()
	MyBase.New("sublink", True)
End Sub


Protected Overrides Function transform( _
	ByRef middle As String _
) As String
	Dim rslt, title, anchor As String
	rslt = ""
	
	anchor = getParam()
	If anchor = "" Then 
		Return "" 'error
	End If
	
	Dim param2 As String = getParam(2)
	Dim div As String = IIf(param2 = "", "", ", ""#" & param2 & """")
	
	title = IIf(middle = "", anchor, middle) 		
	
	Dim cmd As String = "$(""" & anchor & """).scrollIntoView();"
		'"zx_scrollTo(""#" & anchor & """" & div & ");"
	rslt = "<div onClick='" & cmd & "' title='转到" & _
		anchor & "'>" & title & "</div>"
	
	Return rslt		
End Function	
	
End Class 'SublinkConverter

End Namespace