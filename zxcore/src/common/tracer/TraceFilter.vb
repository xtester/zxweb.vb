Imports zxweb.trace

NameSpace zxweb.trace

'*******************************************************************************	
'!! name - System.Diagnostics.TraceFilter
'

Public Class TraceFilter 
	Implements IObjectQueryIncludeAll	
	 
' allowed keywords	 
Private _includes As String
Public Function getIncludes() As String
	Return _includes
End Function

Private _excludes As String
Public Function getExcludes() As String
	Return _excludes
End Function


'*****************
'	Life cycle
'
'*****************

Public Sub New( _
	Optional fs As String = "", _
	Optional ex As String = "" _
)	
	_includes = fs
	_excludes = ex
End Sub	


'***************
'	Services
'
'***************

Public Function isIdentical(ByRef inobj As TraceFilter) As Boolean
	Dim rslt As Boolean = False
	
	If inobj IsNot Nothing Then
		rslt = (inobj._includes = _includes And inobj._excludes = _excludes)		
	Else
		'XTrace.log("TraceFilter.isIdentical", "inobj null")
	End If
	
	'XTrace.log("TraceFilter.isIdentical", rslt)
	Return rslt
End Function


' Test whether tmsg is included (has keywords listed in the keylist).
'
'XX
Private Function isExist( _
	ByRef tmsg As TraceMessage, _	
	ByRef keylist As String _
) As Boolean
	Dim rslt As Boolean = False
	
	If keylist = "" Or tmsg Is Nothing Then 
		Return rslt
	End If
	
	If Util.exist(keylist, tmsg.Source) Then
		Return True
	End If
	
	'logSysError("XTrace.isExist", "To parse...")
	
	keylist = keylist.TrimStart("*"c)
	keylist = keylist.TrimStart("^"c)

	Dim keys As MyArrayList = Util.parseValues(keylist, ",") 'XX
	
	For Each akey As String In keys
		If Util.exist(tmsg.Source, akey) OrElse _
			Util.exist(tmsg.Message, akey) _
		Then 
			rslt = True
			Exit For
		End If
	Next
	
	Return rslt
End Function 'isExist


Shared Private s_isExcludedDefault As Boolean

Public Sub setIsExcludedDefault(bv As Boolean) _
		Implements IObjectQueryIncludeAll.setIsExcludedDefault	
	s_isExcludedDefault = bv
End Sub


'XX 
Public Function isExcluded( _
	ByRef tmsg As TraceMessage _	
) As Boolean
	Dim rslt As Boolean = False 
	
	If tmsg Is Nothing Then 
		Return rslt 'XX ex
	End If
	
	'!!
	If s_isPreventDeadLoop Then
		Return s_isExcludedDefault
	End If
	
Try	
	Dim includestr As String = getIncludes()
	Dim excludestr As String = getExcludes()	
	
	Dim tman As ITraceManager = SysConfig.getTracer()

	' Check ex
	'
	'!! place here, or cause performance problems
	'
	If tmsg.isException() Then		
		If tman IsNot Nothing AndAlso tman.isExcludeIrrelevantExceptions() _
		Then
			'XX more conditions			
			If isExist(tmsg, includestr) Then
				Return False
			Else
				Return True
			End If
		Else
			Return False
		End If		
	End If 
	
	If includestr = "" And excludestr = "" Then 
		Return False 'XX
	End If
	
	'XX mv
	Dim isIncludeAll As Boolean = False
	
	Dim tmpb As Boolean = False
	If tman IsNot Nothing Then
		tmpb = tman.isIncludeAll(Me)		
	End If
	
	isIncludeAll = tmpb OrElse _
		(includestr <> "" AndAlso includestr.Substring(0, 1) = "*")
	
	'!! System.ArgumentOutOfRangeException
	If isIncludeAll And Not Util.exist(excludestr, "^") Then 'XX lead ^ in ex
		Return False
	End If
	
	If excludestr <> "" AndAlso excludestr.Substring(0, 1) = "*" Then
		' Exclude all
		Return True 
	End If
		
	'XX 
	Dim isIncluded As Boolean = isExist(tmsg, includestr)
	Dim isExcluded2 As Boolean = isExist(tmsg, excludestr)
	
	If isExcluded2 OrElse ((Not isIncluded) AndAlso (Not isIncludeAll)) Then 
		rslt = True
	End If
Catch ex As Exception
	rslt = True 'XX
End Try	
		
	Return rslt
End Function 'isExcluded


Public Overrides Function ToString() As String
	Dim rslt As String = ""
	
	If _excludes <> "" Or _includes <> "" Then
		rslt = _includes & "/" & _excludes
	End If
	
	Return rslt
End Function


Shared Private s_isPreventDeadLoop As Boolean 

'XX synclock
Public Sub entryCritical( _
	Optional isExcluded As Boolean = True _
) Implements IObjectQueryIncludeAll.entryCritical
	s_isPreventDeadLoop = True
	s_isExcludedDefault = isExcluded
End Sub	

Public Sub exitCritical() _	
		Implements IObjectQueryIncludeAll.exitCritical
	s_isPreventDeadLoop = False
End Sub	

End Class 'TraceFilter


'*******************************************************************************	
' To prevent dead loop.

Public Interface IObjectQueryIncludeAll
	
	' unused
	Sub setIsExcludedDefault(bv As Boolean)
	
	' isExcluded: False may get very slow
	Sub entryCritical(Optional isExcluded As Boolean = True)
	
	Sub exitCritical()
	
End Interface

End Namespace