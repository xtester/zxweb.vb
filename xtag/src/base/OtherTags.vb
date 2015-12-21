' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/3/17

Namespace zxweb.xtag

'*******************************************************************************
'

Public Class XResourceTag
	Inherits AbstractTag
	
Public Sub New()
	Name = "[["
	setNoArg() 
	
	HeaderBeginMark = "[["	
	Footer = "]]"
End Sub	
	
End Class 'XResourceTag

End Namespace