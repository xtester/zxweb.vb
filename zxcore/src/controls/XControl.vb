' Creation Date: 2013/10/5
' Copyright (c) Xun Zhang
' www.zhangxun.com


Namespace zxweb

'********************************************************************
'

Public Class XControl
	Inherits ControlBase	
	
Protected Overrides Sub initControl()
	MyBase.initControl()	
	
	setUseWriteBuffer(True) 
End Sub	


'XX merge with initControl
Protected Overrides Sub OnInit(eargs As EventArgs)
Try	
	XTrace.showRun("XControl.OnInit")
	
	MyBase.OnInit(eargs) '??	
	MyBase.initControl()			
	
	XTrace.showEnd("XControl.OnInit")
Catch ex As Exception
	XTrace.show("XControl.OnInit", ex)			
End Try
End Sub	'OnInit


Protected Overrides Sub RenderContents(Writer As HtmlTextWriter)	
	If Util.isame(runtimeMode, "custom") Then
		' Content control
		XTrace.showRun("XControl.RenderContents" & getIdTag(), _
			"Xmode, call genOutput()")
		genOutput()
	Else
		' Also for ajax controls, and calling output()
		XTrace.showRun("XControl.RenderContents" & getIdTag(), _
			"Normal mode, call RenderContents")
		MyBase.RenderContents(Writer)
	End If	
	
	XTrace.showEnd("XControl.RenderContents" & getIdTag())
End Sub	

End Class 'XControl

End Namespace