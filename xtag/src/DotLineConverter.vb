
Namespace zxweb.xtag

'*******************************************************************************
'	A text line decorated with a little colored (default red) block at the 
'	front.
'
'	Todos:
'

Public Class DotLineConverter
	Inherits XTagConverter

Public Sub New()
	MyBase.New("dotline", True) 
	setArgOptional()
End Sub

	
Protected Overrides Function transform( _
	ByRef middle As String _
) As String
	Dim rslt As String = ""
	
	Dim color As String = getParam()
	If color = "" Then 
		'XX
		color = "red"
	End If
	
	rslt = "<div style=""width: auto; margin: 0 0 -40px 0; " & _
		"padding: 0 0 0 0;"">" & _
		"<div style=""background-color: " & color & "; " & _
		"width: 10px; height: 10px; ""></div>" & _
		"<div style=""position: relative; left: 15px; top: -14px;"">" & middle _
		& "</div></div>"
	
	'XTrace.show("DotLineConverter.transform", "result", news)
	Return rslt	
End Function
	
End Class 'DotLineConverter

End Namespace