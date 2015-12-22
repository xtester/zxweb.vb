' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/5/10

Namespace zxweb.trace

'*******************************************************************************
'

Public Interface ITraceManager
	
	Function isTraceEnabled() As Boolean
	
	Function getTraceMode() As String	
	
	'XX
	Function getFilter( _
		ftype As String _
	) As String
	
	Function isExcludeIrrelevantExceptions() As Boolean
	
	' srcObj: to prevent dead loop	
	Function isIncludeAll( _
		Optional ByRef srcObj As IObjectQueryIncludeAll = Nothing) As Boolean
	
	' In
	' 
	
	Sub setLogFileAbsPath(ins As String)
	
End Interface

End Namespace