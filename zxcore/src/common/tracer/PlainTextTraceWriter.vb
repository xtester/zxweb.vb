' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2014/12/24

Namespace zxweb.trace

'*******************************************************************************
'

Public Class PlainTextTraceWriter
	Inherits TraceWriter
	
Public Sub New( _
	ByRef logger As XTrace, _	
	ByRef inwr As StreamWriter _
)
	MyBase.New(logger, inwr)
End Sub


Public Overrides Function GetWriterType() As String
	Return "PlainTextTraceWriter"
End Function


Public Overrides Sub writeHeader()	
'	_swr.WriteLine(_logger.getHeader())
		
	_swr.WriteLine("Filter: " & _logger.getFilterTip())
	_swr.WriteLine("Date: " & DateTime.Now)
	_swr.WriteLine("")
	_swr.WriteLine("Trace begins ...")
	_swr.WriteLine("")	
End Sub


Public Overrides Sub write(ByRef tmsg As TraceMessage)	
	If tmsg Is Nothing Then Return
	
	If Not _logger.isExcluded(tmsg) Then
		s_lineCount += 1
		_swr.WriteLine("L" & s_lineCount & " " & tmsg.Tag & _
			" " & DateTime.Now & _
			" [" & tmsg.Source & "] " & tmsg.Message)		
	End If
End Sub

End Class 'PlainTextTraceWriter

End NameSpace