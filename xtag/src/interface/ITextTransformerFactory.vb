' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/4/2

Namespace zxweb.xtag

'*******************************************************************************
'

Public Interface ITransformerFactory
	
	'XX
	Function createTransformer( _
		Optional ttype As String = "default", _
		Optional ByRef ctxt As Object = Nothing _
	) As ITextTransformer
	
End Interface

End Namespace