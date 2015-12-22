' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2014/12/31

Namespace zxweb

'*******************************************************************************
'

Public Class Icons
	
Public Shared Readonly Property FirstButton As String
	Get
		Return SysConfig.getImgHome() & "/first.jpg"
	End Get
End Property

Public Shared Readonly Property NextButton As String
	Get
		Return SysConfig.getImgHome() & "/next.jpg"
	End Get
End Property

Public Shared Readonly Property PreviousButton As String
	Get
		Return SysConfig.getImgHome() & "/previous.jpg"
	End Get
End Property

Public Shared Readonly Property LastButton As String
	Get
		Return SysConfig.getImgHome() & "/last.jpg"
	End Get
End Property

Public Shared Readonly Property TopButton As String
	Get
		Return SysConfig.getImgHome() & "/top.jpg"
	End Get
End Property

Public Shared Readonly Property BottomButton As String
	Get
		Return SysConfig.getImgHome() & "/bottom.jpg"
	End Get
End Property


Public Shared Readonly Property Minus As String
	Get
		Return "<img src='" & SysConfig.getImgHome() & "/minus.gif" & "' />" 		
	End Get
End Property

Public Shared Readonly Property Plus As String
	Get
		Return SysConfig.getImgHome() & "/plus.gif"
	End Get
End Property


End Class 'Icons

End Namespace