
Namespace zxweb.xtag

'*******************************************************************************
'	Add HTML breaks.
'
'	Todos: 
'		- merge with PostWorker?
'		- test
'

Public Class BreakLineConverter
	Inherits XTagConverter

Public Overrides Function run() As String
	Dim rslt As String = getTextInput() 

Try	
	'XX refactor:move up
	If Not Util.exist(getMaster().getExcludes(), "breakline") Then 	
		rslt = rslt.Replace(vbCrLf, "<br />") '!! copy
	End If
	
	passon(rslt)
Catch ex As Exception
	XTrace.show("BreakLineConverter.run", ex)
End Try

	Return rslt	
End Function

End Class 'BreakLineConverter

End Namespace