' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2014/11/12

Namespace zxweb.trace

'*******************************************************************************
'

Public Class HtmlTraceWriter
	Inherits TraceWriter
	
Public Sub New( _
	ByRef logger As XTrace, _	
	ByRef inwr As StreamWriter _
)
	MyBase.New(logger, inwr)
End Sub
	
	
Public Overrides Function GetWriterType() As String
	Return "HtmlTraceWriter"
End Function


'*************
'	Events
'
'*************

Public Overrides Sub writeHeader()	
	'_swr.WriteLine("<strong>Platform</strong>: " & _logger.getHeader())		
	_swr.WriteLine("<strong>Created on</strong>: " & DateTime.Now)
	
	'XX For txt display
	_swr.WriteLine("<br /><strong>Mode</strong>: " & getTracer().getMode()) 
	_swr.WriteLine("<br /><strong>Filter</strong>: " & _
		getTracer().getFilterTip())
	
	_swr.WriteLine("<br /><strong>Trace begins ...</strong>")
	_swr.WriteLine("<p />")	
End Sub


Public Overrides Sub write(ByRef tmsg As TraceMessage)	
	If tmsg Is Nothing OrElse getTracer().isExcluded(tmsg) Then
		'XTrace.log("HtmlTraceWriter", "excluded, return")
		Return
	End If	
	
	tmsg.Tag = "HWR"
	
	'XTrace.log("HtmlTraceWriter", "To write")
	
	s_lineCount += 1
	_swr.WriteLine("L" & s_lineCount & " " & tmsg.Tag & _
		" " & DateTime.Now & _
		" [" & tmsg.Source & "]<br />") 
		
	Dim msg As String = tmsg.Message
	'XX String.Split
	If Util.exist(msg, ":") Then
		Dim pos As Integer = msg.IndexOf(":")
		Dim akey As String = msg.Substring(0, pos)		
		Dim newKey As String 
		
		' Change colors
		If Util.exist(akey, "exception") Then
			newKey = "<span style=""color: red;"">" & akey & "</span>"			
		Else	 If Util.isame(akey, "warning") Then
			newKey = "<span style=""color: #FDAB02;""><strong>" & akey & _
				"</strong></span>"
		Else
			newKey = "<strong>" & akey & "</strong>"
		End If
		
		msg = newKey & msg.Substring(pos, msg.Length - pos)
	End If
	
	_swr.WriteLine("&nbsp;&nbsp;&nbsp;&nbsp;" & msg & "<br />")
	'XTrace.log("HtmlTraceWriter", "End")
End Sub
	
End Class 'HtmlTraceWriter

End Namespace