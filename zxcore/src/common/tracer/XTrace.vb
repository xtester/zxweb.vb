
Imports System.Threading

Imports zxweb.trace

Namespace zxweb

'*******************************************************************************
'
'!! TraceFilter may conflict with System.Diagnostics.TraceFilter
'

Public Class XTrace

'!! 
Private Shared s_filter As TraceFilter = Nothing
Shared Public Function getFilter() As TraceFilter
	Return s_filter	
End Function

' Used by:
'	checkFilter()
Shared Public Sub setFilter(ByRef inf As TraceFilter)
	s_filter = inf	
End Sub


Private Shared s_logPPath As String

Private _worker As Thread


'*****************
'	Properties
'
'*****************

Private Shared s_destFile As String 

'XX
Public Shared Property PhysicalFilePath As String
	Get
		Return s_destFile
	End Get
	Set (fp As String)
		s_destFile = fp
	End Set
End Property


'XX
Public Shared Property SysLogPhyPath As String
	Get
		Return s_logPPath
	End Get
	Set (fp As String)
		s_logPPath = fp
	End Set
End Property


'*****************
'	Life cycle
'
'*****************

Private Shared s_singleton As XTrace

Public Shared Function getInstance() As XTrace
	If s_singleton Is Nothing Then
		s_singleton = New XTrace
	End If
	
	Return s_singleton
End Function


'XX 
Private Shared s_traceFileRelPath As String 
Public Shared Function getTraceFileRelPath() As String
	Return s_traceFileRelPath	
End Function


Public Shared Sub init(ByRef iapp As IAppCenter)
	Try				
		'XX what if config_file error ?		
		s_traceFileRelPath = SysConfig.readNodeAttribute("system/trace/@file")		
		PhysicalFilePath = AppHome.mapPath(s_traceFileRelPath)
		
		SysLogPhyPath = AppHome.mapPath( _
			SysConfig.readNodeAttribute("system/errors/@file"))
		
'		XTrace.show("XTrace.init", "PhysicalFilePath", PhysicalFilePath, _
'			"SysLogPhyPath", SysLogPhyPath)
	Catch ex As Exception
		'trace("Util.init", ex)
	End Try			
End Sub


'**************
'	Helpers
'
'**************

Public Shared Function existFile() As Boolean
	Return File.Exists(PhysicalFilePath)
End Function


'*************** 
'	Services 
'
'***************

' basic
Public Shared Sub show( _
	ByRef where As String, _
	str As String _
)	
	process(where, str, "XTrace") 'XX
End Sub 

' for cs
Public Shared Sub show2( _
	where As String, _
	str As String _
)	
	show(where, str)
End Sub 


'XX
Public Shared Sub warn( _
	ByRef where As String, _
	ins As String, _
	Optional what As String = "" _
)	
	'Dim len As Integer = UBound(params)
	Dim msg As String = "Warning: "
		
	If what = "" Then
		msg &= ins & "."
	Else
		msg &= ins & ": " & what 'XX
	End If
	
	process(where, msg, "XTrace")
End Sub 


'??
Public Shared Sub show( _
	ByRef obj As Object, _
	str As String _
)	
	Dim ss, where As String
	Dim fi, ei As Integer
	
Try	
	If obj Is Nothing Then Return
	
	ss = Environment.StackTrace
	fi = ss.IndexOf(obj.GetType().BaseType().Name)
	'fi = ss.IndexOf(obj.DeclaringType.Name)
	ei = ss.IndexOf("(", fi + 1)
	If fi > 0 And ei > fi Then
		where = ss.SubString(fi, ei - fi)
		process(where, str, "XTrace")
	End If
Catch ex As Exception
End Try
End Sub 'show


Public Shared Sub showRun( _
	ByRef where As String, _
	str As String _
)	
	process(where, str & " ...", "XTrace")
End Sub


Public Shared Sub showRun( _
	ByRef where As String _
)	
	process(where, "Run...", "XTrace")
End Sub 

Public Shared Sub showRun2( _
	where As String _
)	
	showRun(where)
End Sub 


'!! end is keyword
Public Shared Sub showEnd( _
	ByRef where As String _
)	
	process(where, "End.", "XTrace")
End Sub 

Public Shared Sub showEnd2( _
	where As String _
)	
	showEnd(where)
End Sub 


Public Shared Sub show( _
	ByRef place As String, _
	ByRef ex As Exception _
)
	If ex Is Nothing Then Return 'XX Warning
	getInstance().output(New TMsgException(place, ex))
End Sub

' for c#
Public Shared Sub show2( _
	place As String, _
	ByRef ex As Exception _
)
	show(place, ex)
End Sub


Public Shared Sub checkNull( _
	ByRef where As String, _
	ByRef objname As String, _
	ByRef obj As Object _
)
	show(where, "'" & objname & "' is null", obj Is Nothing)
End Sub

Public Shared Sub checkNull2( _
	where As String, _
	objname As String, _
	ByRef obj As Object _
)
	checkNull(where, objname, obj Is Nothing)
End Sub


' maps
Public Shared Sub show( _
	ByRef where As String, _
	ParamArray params() As String _
)
	Dim leng As Integer

Try	
	leng = UBound(params) ' 1st dimension
	If leng < 0 Then '!!
		Return
	End If
	
	For i As Integer = 0 To leng Step 2
      	show(where, params(i) & ": " & params(i+1))  
    Next i
Catch ex As Exception
End Try
End Sub 'show


' for cs
Public Shared Sub show2( _
	where As String, _
	ParamArray params() As String _
)
	show(where, params)
End Sub	


'XX len; change
Public Shared Sub checkString( _
	ByRef place As String, _
	ByRef ins As String, _
	Optional strName As String = "The string" _
)
	If ins <> "" Then
		show(place, strName & " (len:" & ins.Length & ")")  	
	Else
		warn(place, strName & " is empty")
	End If
End Sub


' tricky
Private Shared s_isCheckSwitch As Boolean = False

Public Shared Sub beginCheckSwitch()
	s_isCheckSwitch = True	
End Sub

Public Shared Sub endCheckSwitch()
	s_isCheckSwitch = False	
End Sub


'!! interface implementer can't be 'shared'
'
' Called by: 
'	
'XX performance
'
Private Shared Sub process( _
	ByRef place As String, _
	ByRef msg As String, _
	Optional ns As String = "" _
) 'Implements ICommentEventHandler.process
Try
	'XX mv
	If s_destFile = "" Or msg = "" Then 
		Return 'XX
	End If
	
	If s_isCheckSwitch Then
		Return '!!
	End If
	
	'XX dead loop; use AppHome
	'!! performance
	If Not getTracer().isTraceEnabled() Then 
		Return
	End If
	
	Dim logger As XTrace = getInstance() 	
	Dim tmsg As New TraceMessage(ns, place, msg)	 'XX

'	'logger._worker = New Thread(AddressOf logger.writeTrace)
'	'If logger._worker Is Nothing Then
'	
	logger.output(tmsg)
	
	'Else
		'ms._worker.Start()
	'End If
	
	'ms._worker.Join()
Catch ex As Exception
	'logEx("XTrace.process", ex)
End Try
End Sub 'process
	
	
Private Shared Function getTracer() As ITraceManager
	Return SysConfig.getTracer()
End Function

	
'*************
'	States
'

Private Shared s_isPurged As Boolean = False

Private Shared Sub clearStates()
	s_isPurged = False
	
	HtmlTraceWriter.clearStates()
End Sub

Private Shared s_mode As XTraceMode 
Public Shared Function getMode() As String
	Return s_mode.ToString()	
End Function

'XX mvout
' name collision - .Net TraceMode
Private Enum XTraceMode
	Open ' append 
	Purge ' create a new empty file
	Silent ' closed
End Enum

Private Shared s_isConfigSource As Boolean = True

' For string to enum
'
'XX
Private Shared Sub checkMode() 	
Try		
	Dim modestr As String = SysConfig.getTraceMode() 
	
	Select Case modestr
		Case "open"
			s_mode = XTraceMode.Open
			
		Case "purge" 'old: new
			s_mode = XTraceMode.Purge		
			
		Case Else
			s_mode = XTraceMode.Silent 	
	End Select	
Catch ex As Exception
'!!
End Try	
End Sub 'checkMode


'XX
Public Shared Function isModePurge(Optional refresh As Boolean = False) As Boolean
	If refresh Then checkMode()
	Return (s_mode = XTraceMode.Purge)
End Function


Public Shared Function isModeSilent(Optional refresh As Boolean = False) As Boolean
	If refresh Then checkMode()
	Return (s_mode = XTraceMode.Silent)
End Function


Public Shared Function isModeOpen(Optional refresh As Boolean = False) As Boolean
	If refresh Then checkMode()
	Return (s_mode = XTraceMode.Open)
End Function


Private Shared Sub setModeOpen() 
	s_mode = XTraceMode.Open
End Sub


Private Shared Sub setModePurge() 
	s_mode = XTraceMode.Purge
End Sub


' Still keep the content of the log file.
'
' Used by: 
'	output()
'
Public Shared Sub closeTrace(Optional ByRef msg As String = "")	
Try
	Dim sw As StreamWriter = File.CreateText(s_destFile)	

	sw.WriteLine(DateTime.Now & " Server tracing is closed.<br />")
'	sw.WriteLine("Reason: " & msg)

	sw.flush()
	sw.Close()	
	
	clearStates()	
Catch ex As Exception	
	'Throw
End Try	
	
'	'!! clear
'	Dim logs As StreamWriter = File.CreateText(s_logPPath)		
'	logs.WriteLine(s_header)	
'	logs.WriteLine(DateTime.Now & " System log closed (zxweb).")
'	logs.WriteLine("Reason: " & msg)
'
'	logs.flush()
'	logs.Close()				
End Sub 'closeTrace


'XX
Shared Private s_filterTip As String
Shared Public Function getFilterTip() As String
	Return s_filterTip	
End Function


Shared Private s_isFilterChanged As Boolean = False
Public Shared Function isFilterChanged() As Boolean
	Return s_isFilterChanged
End Function


' sync
Private _lockCheckFilter As New Object

Private Sub checkFilter() 
	Dim old As String = ""
	
SyncLock(_lockCheckFilter)
	_isCheckingFilter = True '!! before any statements
	
	Dim newFilter As TraceFilter = SysConfig.readTraceFilter() 'XX
		
	If newFilter IsNot Nothing AndAlso _
		Not newFilter.isIdentical(getFilter()) _
	Then
		s_isFilterChanged = True		
		
		If getFilter() IsNot Nothing Then 
			old = getFilter().ToString()
		End If
		
		old = " (Last: " & old & ")"
		
		' Reset 
		setFilter(newFilter)
	Else
		s_isFilterChanged = False
	End If	
	'If s_filter IsNot Nothing Then logSysError("XTrace.writeTrace", 
	'"old: " & s_filter.ToString())
		
	s_filterTip = getFilter().ToString() & old	
	'log("XTrace.checkFilter", "Filter: " & rslt)
	
	_isCheckingFilter = False
End SyncLock
End Sub 'checkFilter


'XX merge
Public Function isExcluded( _
	ByRef tmsg As TraceMessage _	
) As Boolean
	Dim filter As TraceFilter = getFilter()
	
	If tmsg Is Nothing OrElse filter Is Nothing Then 
		'log("XTrace.isExcluded", "Msg or filter is nothing, return")
		Return True
	End If
	
	Return filter.isExcluded(tmsg)
End Function


Shared Private s_writer As TraceWriter

'XX
Private Shared Function getWriter( _
	ByRef logger As XTrace, _	
	ByRef inwr As StreamWriter _
) As TraceWriter
	If s_writer Is Nothing Then
		s_writer = New HtmlTraceWriter(logger, inwr)
			'New PlainTextTraceWriter(logger, inwr)
	Else
		s_writer.setStream(inwr)
		s_writer.setTracer(logger)
	End If
	
	Return s_writer
End Function


'XX 
Shared Public Function createNewFile() As StreamWriter
	log("XTrace.createNewFile", "Create a new log file...")	
	Dim rslt As StreamWriter
	
Try
	SyncLock(s_lockOutput)
		If existFile() Then
			'File.Delete(s_destFile)
		End If
		
		Dim tracer As ITraceManager = getTracer()
		If tracer IsNot Nothing Then
			tracer.setLogFileAbsPath(s_destFile)
		End If
		
		rslt = File.CreateText(s_destFile)					
		Dim twr As TraceWriter = getWriter(getInstance(), rslt)						
		
		'log("XTrace.createNewFile", "Write header")
		twr.writeHeader()	
		twr.close() '!!
		
		s_isPurged = True 'XX			
	End SyncLock
Catch ex As Exception	
	'Throw
End Try	
	
	Return rslt
End Function 'createNewFile


Private _isCheckingFilter As Boolean = False

'!! sync
Private Shared s_lockOutput As New Object

' Write to the trace file.
'
'XX not Active thread yet
'XX output: for testing
'
Private Sub output( _
	ByRef tmsg As TraceMessage, _	
	Optional ByRef output As String = "" _
)			
	Dim stwriter As StreamWriter = Nothing '!! keep here for ex

Try		
	If _isCheckingFilter Then
		Return '!! Avoid reentry, then dead loop.
	End If
	
	checkFilter() 'XX mv
	'log("XTrace.output", "Filter changed: " & isFilterChanged())
	'log("XTrace.output", "new: " & newFilter.ToString())		

	If isExcluded(tmsg) Then 
		'log("XTrace.output", "Message excluded, return.")
		Return
	End If
	
	'log("XTrace.output", "Run...")
	
	checkMode()	'XX	mv
	
	SyncLock(s_lockOutput)
'		If isModeSilent(True) Then
'			'Util.appendString(output, "Silent;")	
'			closeTrace("Silent mode") ' Write to the file sth.
'			Return
'		End If

		If (Not existFile()) OrElse _
			(isModePurge() And Not s_isPurged) OrElse _
			isFilterChanged() _
		Then
			stwriter = createNewFile()
			
			stwriter.WriteLine("Created on: " & DateTime.Now)
			stwriter.WriteLine("Trace begins ...")
'			stwriter.Flush()
'			stwriter.Close()				
		Else
			log("XTrace.output", "Open file to append...")
			stwriter = File.AppendText(s_destFile)
		End If
		
		' To write content
		'
		Dim twr As TraceWriter = getWriter(Me, stwriter)			
		'log("XTrace.output", twr.GetWriterType())				
		log("XTrace.output", "Write msg " & tmsg.Source)
		
		twr.write(tmsg)		
		twr.close() '!!
	End SyncLock	
Catch ex As Exception
	logEx("XTrace.output", ex)		
Finally
	'?? Close the stream
	If stwriter IsNot Nothing Then stwriter = Nothing
End Try

	If Not _worker Is Nothing Then _worker.Abort()
	'log("XTrace.output", "End")
End Sub 'output


'*********************************************
'	Self logger, useful when XTrace breaks 
'
'XX performance

Public Shared Sub logEx(ByRef src As String, ByRef ex As Exception)
	log(src, ex.ToString())
End Sub


Private Shared Sub log(ByRef src As String, ByRef msg As String)
	If True Then Return	
	
	Try
		SyncLock GetType(XTrace) 'XX
			Dim logWriter As StreamWriter = File.AppendText(s_logPPath)
			
			logWriter.WriteLine(DateTime.Now & " [" & src & "] " & msg)
			logWriter.flush()
			logWriter.close()				
		End SyncLock
	Catch ex2 As Exception
	'??
	End Try
End Sub

End Class 'XTrace

End Namespace 