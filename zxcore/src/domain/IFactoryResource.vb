' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/12/14

Namespace zxweb.domain

'*******************************************************************************
'

Public Interface IFactoryResource
	
	Function createInstance( _
		sname As String _
	) As IWebResource
	
	Function adjustSname( _
		sname As String, _
		Optional isTestExistance As Boolean = False _
	) As String	
	
End Interface

End Namespace