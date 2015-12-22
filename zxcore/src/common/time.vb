' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com

NameSpace zx.common

'*******************************************************************************
'	Profiling
'

Public Class Time

Shared Public Sub setTime(ByRef dt As DateTime)	
	dt = DateTime.Now
End Sub


Shared Public Sub show( _
	ByRef where As String, _
	ByRef t2 As DateTime, _
	ByRef t1 As DateTime _
)
Try
	XTrace.show(where, "Time used", DateTime.op_Subtraction(t2, t1).ToString())	
Catch ex As Exception
	XTrace.show("Time.show", ex)
End Try
End Sub

End Class 'Time

End Namespace