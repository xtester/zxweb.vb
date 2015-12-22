' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/5/19

Namespace zxweb.trace

'*******************************************************************************
'

Public MustInherit Class TraceWriter
	
Protected _swr As StreamWriter
Public Sub setStream(ByRef ins As StreamWriter)	
	_swr = ins	
End Sub

'XX priv
Protected _logger As XTrace
Public Sub setTracer(ByRef int As XTrace)
	_logger = int	
End Sub
Public Function getTracer() As XTrace
	Return _logger	
End Function


'XX priv?
Shared Protected s_lineCount As Integer = 0

Shared Public Sub clearStates()
	s_lineCount	= 0
End Sub


'*****************
'	Life cycle
'
'*****************

Public Sub New(
	ByRef logger As XTrace, _	
	ByRef inwr As StreamWriter _
)
	_logger = logger	
	_swr = inwr	
End Sub


'***************
'	Services
'
'***************

Public Overridable Function GetWriterType() As String
	Return "Generic"
End Function


Public Overridable Sub writeHeader()	
End Sub

Public Overridable Sub write(ByRef tmsg As TraceMessage)	
End Sub

Public Sub close()
	If _swr Is Nothing Then Return
	
	_swr.flush()
	_swr.Close()		
End Sub

End Class 'TraceWriter

End Namespace