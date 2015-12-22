' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/2/26

Namespace zxweb.trace

'*******************************************************************************
'

Public Class TMsgException	
	Inherits TraceMessage
	
'Private _exception As Exception	


Public Sub New( _
	place As String, _
	ByRef ex As Exception, _
	Optional id As String = "" _
)
	MyBase.New(id, place, "^^Caught " & ex.ToString())
End Sub


Public Overrides Function isException() As Boolean
	Return True	
End Function

End Class 'TMsgException

End Namespace