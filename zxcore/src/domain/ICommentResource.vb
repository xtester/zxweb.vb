' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2014/12/23

Namespace zxweb.domain

'*******************************************************************************
'

Public Interface ICommentResource
	
	Function getComment( _
		sname As String, _
		cid As String _
	) As IComment				
	
	
	Function getCommentContent( _
		sname As String, _
		cid As String _
	) As String
	
	
	Function getLink( _
		ByRef cmnt As IComment, _
		Optional txt As String = "", _
		Optional tip As String = "", _
		Optional isNewTab As Boolean = False _
	) As String
	
End Interface

End Namespace