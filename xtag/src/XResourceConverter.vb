' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/3/18

Namespace zxweb.xtag

'*******************************************************************************
'	[[sname|title|u|n]] wiki-like
'

Public Class XResourceConverter
	Inherits XTagConverter
	
Public Sub New()
	MyBase.New()
	setCurrentTag(New XResourceTag())
End Sub


Protected Overrides Function transform( _
	ByRef body As String _
) As String
	XTrace.showRun("XResourceConverter.transform")
	Dim rslt As String = body	
	Dim title As String = body
	Dim link As String = body
	Dim isNewWin As Boolean = False
	
	If Util.exist(body, "|") Then
		Try
			Dim values As ArrayList = Util.parseValues(body, "|")
			
			link = values(0)
			title = values(1)		
			
			If Util.exist(values, "u") Then
				title = "<u>" & title & "</u>"
			End If			
			
			isNewWin = Util.exist(values, "n")
		Catch ex As Exception			
			'Throw
		End Try
	End If
	
	Dim tip As String = "/?" & link
	tip = IIf(isNewWin, tip & " 打开新窗口", tip)
	
	rslt = Util.genAnchor("/?" & link, title, tip, isNewWin)	
	XTrace.show("XResourceConverter.transform", "rslt", rslt)
	
	Return rslt
End Function
	
End Class 'XResourceConverter

End Namespace