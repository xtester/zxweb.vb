' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2014/12/23

Namespace zxweb

'*******************************************************************************
'

Public Class XSession
	
' Shared by each usr session
Protected Shared _tagMe As String = "xsobj" '?
Protected Shared _tagActiveUsrList As String = "active_usrs"
Protected Shared _tagTotalSessions As String = "total_sessions"


Protected Shared Application As HttpApplicationState
Private Shared _server As HttpServerUtility

' to save objs
Private _session As HttpSessionState 
Public Function getSession() As HttpSessionState
	Return _session
End Function


Private _cookieId As String
Protected Sub setCookieId(ins As String)
	_cookieId = ins	
End Sub
Protected Function getCookieId() As String
	Return _cookieId
End Function


'*****************
'	Life cycle
'
'*****************

Protected Sub New()	
End Sub

' session init
' initialize the db, utils
'
'?? throw ex
Protected Sub New(ByRef context As HttpContext)
	init(context)
End Sub 'New


Protected Overridable Sub init(ByRef context As HttpContext)
Try
	_session = context.Session
	Application = context.Application 'XX
	_server = context.Server

	' save the entry in memory
	_session.Add(_tagMe, Me) ' initialize when first call
	_session.Add("Autoback", "")
	_session.Add("ErrorId", "")
		
	addSession() 'online access
Catch ex As Exception
	XTrace.show("XSession.init", ex)
End Try	
End Sub 'init


Private Shared _isUseExtension As Boolean = False
Public Shared Sub setUseExtension(bv As Boolean)
	_isUseExtension = bv	
End Sub


' get the per-session entry object
'
' called by:
'		PageBase.loadWork()
'
Public Shared Function getObject( _
	ByRef context As HttpContext _
) As XSession
	Dim rslt As XSession = Nothing
	
Try
	'XTrace.show("XSession.getObject", "Timeout", sess.TimeOut)	
	rslt = context.Session(_tagMe) ' 
	'?? rslt.setContext(context) 2011-5-2
	
	If rslt Is Nothing Then 
		If _isUseExtension Then
			rslt = ObjectFactory.getObject("zxweb.XSessionEx") 'XX
			rslt.init(context)
		Else
			rslt = New XSession(context) 'add xsobj in session		
		End If		
		
		'XTrace.showRun("XSession.getObject_1133", "XSession created.")
	End If			
	
	rslt.checkCookie(context) ' try login
Catch ex As Exception
	XTrace.show("XSession.getObject", ex)
End Try

	Return rslt
End Function 'getObject


' what if Client close browser ?
' ?? when run
' XX delegate
'
Protected Overrides Sub Finalize()
Try
	removeSession()
	_session.Remove(_tagMe)
	'_session = Nothing
Catch ex As Exception
	'...
End Try

	MyBase.Finalize()
End Sub


Protected Overridable Sub checkCookie( _
	ByRef context As HttpContext _
)
	setCookieId(LoginCookie.getCookie(context)) 
End Sub


Private Shared Sub addSession()
	Dim ss as String
	Dim iss As Integer

Try	
	Application.Lock()
	ss = Application.Get(_tagTotalSessions)		
	Try	
		iss = CInt(ss)
	Catch ex As Exception
		iss = 0
	End Try
	
	If iss < 0 Then iss = 0		
	iss += 1
	
	Application.Set(_tagTotalSessions, CStr(iss))
	Application.UnLock()	
Catch ex As Exception
End Try
End Sub 'addSession
	
	
Private Shared Sub removeSession()
	Dim ss as String
	Dim iss As Integer

Try	
	Application.Lock()
	ss = Application.Get(_tagTotalSessions)		
	Try	
		iss = CInt(ss)
	Catch ex As Exception
		iss = 2
	End Try
	
	If iss < 1 Then iss = 2
		
	iss -= 1
	Application.Set(_tagTotalSessions, CStr(iss))
	Application.UnLock()	
Catch ex As Exception
End Try
End Sub	


'***************
'	Services
'
'***************

'XX merge to zxpage?
Public Sub handleError( _
	msg As String, _
	ByRef ctx As HttpContext _
)
	Dim eid As String = ""
	'XTrace.show("XSession.handleError", "msg", msg)
	
	createError(msg, eid)	
	
	' For testing
	'
	Dim newurl As String = "/?sname=Error" & PageBase.getChangedQuery(ctx)		
	XTrace.warn("XSession.handleError", "Redirect to page " & newurl)	

	ctx.Response.Redirect(newurl) 
End Sub


' For every page sending response
'
Public Function setResponseExpires( _
	ByRef response As HttpResponse, _
	ByVal ts As Integer, _
	Optional ByVal cab As HttpCacheability = HttpCacheability.NoCache _
) As Boolean
	Dim myCachePol As HttpCachePolicy
	
	myCachePol = response.Cache
	myCachePol.SetExpires(DateTime.Now.AddSeconds(ts))
	myCachePol.SetCacheability(cab)
	
	Return True
End Function


'****************
'	Last Page
'

Public Sub saveLastPage(path As String)
Try
	If path = "" Then Return
	_session.Add("LastPage", path)
Catch ex As Exception
End Try	
End Sub


'!! default: clear
Public Function getLastPage() As String
	Dim rslt As String
	
	If _session Is Nothing Then Return ""
	
	rslt = _session("LastPage")
	_session("LastPage") = ""
	
	Return rslt
End Function


'********************* 
'	Error Handling 
'

'XX
Public Function createError( _
	emsg As String, _
	ByRef eid As String _
) As Boolean
	Dim rslt As Boolean = False
	
Try
	If eid = "" Then
		eid = Guid.NewGuid().ToString()
	End If
	_session.Add(eid, emsg)
	_session("ErrorId") = eid
	
	rslt = True
	XTrace.show("XSession.createError", "Created ok", eid)
Catch ex As Exception
	rslt = False
	XTrace.show("XSession.createError", ex)
End Try
	
	Return rslt
End Function 'createError


'!!
Public Function getError( _
	Optional eid As String = "" _
) As String
	Dim rslt, token As String 
    rslt = ""
	
Try	
	If eid = "" Then
		token = _session("ErrorId") 'XX conf
	Else
		token = eid
	End If
	
	'XTrace.show("XSession.getError", "token", token)
	rslt = _session(token)
	
	'default
	_session.Remove(token) 'XX conditional
Catch ex As Exception
End Try
	
	XTrace.show("XSession.getError", "msg", rslt)		
	Return rslt
End Function 'getError


'************
'	Cache
'

Public Function getCache(tag As String) As Object
	Dim rslt As Object = Application.Get(tag)
	Return rslt
End Function


' obj empty, remove it
'
'?? use set
Public Sub setCache( _
	tag As String, _
	Optional ByRef obj As Object = Nothing _
)
	If tag = "" Then Return 

Try	
	XTrace.showRun("XSession.setCache")
	Application.Lock()
	
	Application.Remove(tag)
	If obj IsNot Nothing Then 
		Application.Add(tag, obj)
	End If
	
	Application.UnLock()
Catch ex As Exception
	'!! System error
	XTrace.show("XSession.setCache", ex)
End Try
End Sub


Public Overridable Function isLogined() As Boolean
	Return False
End Function


public Function getActiveSessions() As Integer
	Dim rslt As Integer
	Dim tmp As String
	
	tmp = Application.Get(_tagTotalSessions)		
	Try 
		rslt = CInt(tmp)
	Catch ex As Exception
		rslt = 1
	End Try

	Return rslt
End Function

End Class 'XSession

End Namespace