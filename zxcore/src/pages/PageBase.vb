Imports zxweb.core

Namespace zxweb

'*******************************************************************************
'	Todos:
'

Public Class PageBase
	Inherits Page	
	
Private _params As NameValueCollection 

Public Function getParams() As NameValueCollection
	Return _params	
End Function

Public Function getParam(inkey As String) As String
	Dim rslt As String = ""
	
	If Not _params Is Nothing Then 
		rslt = _params(inkey)
	End If
	
	Return rslt
End Function


Protected _rawUrl As String


Private _writer As HtmlTextWriter
Public Function getWriter() As HtmlTextWriter
	Return _writer
End Function
Protected Sub setWriter(ByRef wr As HtmlTextWriter)
	_writer = wr
End Sub


Private _finished As Boolean = False
Protected Function isFinished() As Boolean
	Return _finished
End Function
Protected Sub setFinished(bv As Boolean)
	_finished = bv	
End Sub


Protected Overridable Sub initParams()
	XTrace.showRun("PageBase.initParams")	
	_params = Request.Params	
	_rawUrl = Request.RawUrl
End Sub	


Public Overridable Function getTitle() As String
	Return ""	
End Function


Public Overridable Function getVirtualPath() As String
	Return ""	
End Function


'XX useful?
Public Function getHttpContext() As HttpContext
	Return Context
End Function


'XX move to xresource
Private _titlePrefix As String
Public Function getTitlePrefix() As String
	Return _titlePrefix
End Function


'XX priv
Protected _xsession As XSession
Public Function getXSession() As XSession
	If _xsession Is Nothing Then
		_xsession = XSession.getObject(Context)
		'XX what if null
	End If
	Return _xsession	
End Function


' relative 
'
Public Function getViewPath() As String
	Dim rslt As String
	Dim slen, si As Integer 	
	
	XTrace.show("PageBase.getViewPath", "rawUrl", _rawUrl)
	If _rawUrl = "" Then Return "" '!! Error
	
	slen = _rawUrl.Length()
	si = _rawUrl.LastIndexOf("?")		
	If si < 0 Then si = slen	
	rslt = _rawUrl.Substring(0, si)  
	
	XTrace.show("PageBase.getViewPath", "rslt", rslt)	
	Return rslt	
End Function

'Ref: Util.getQueryString()


Public Shared Function getChangedQuery( _
	ByRef ctx As HttpContext, _	
	Optional excluded As String = "", _
	Optional withPrefix As Boolean = True _
) As String
	If ctx Is Nothing Then Return ""
	
	Dim isChanged As Boolean = False
	Dim oldUrl As String = ctx.Request.RawUrl
	Dim rslt As String = ""
	XTrace.show("PageBase.getChangedQuery", "Original url", oldUrl)
	
	If Util.exist(oldUrl, "mode=test") Then
		Dim mnv As MyNameValueCollection = _
			MyNameValueCollection.copy(ctx.Request.QueryString)
		
		If Not Util.exist(excluded, "sname") Then
			mnv.Remove("sname")
		End If
		
		mnv.Remove("dummy")
		
		rslt = mnv.ToString()
		If withPrefix Then rslt = "&" & rslt
		
		isChanged = True
	End If
	
	Dim note As String = IIf(isChanged, "changed", "unchanged")
	XTrace.show("PageBase.getChangedQuery", "rslt (" & note & ")", rslt)
	Return rslt
End Function 'getChangedQuery


Private _userAgent As String
Public Function getUserAgent() As String
	Return _userAgent	
End Function
Protected Sub setUserAgent(ins As String)
	_userAgent = ins	
	'XTrace.show("PageBase.setUserAgent", "rslt", _userAgent)	
End Sub


Public Function isFireFox() As Boolean
	Dim rslt As Boolean = Util.exist(getUserAgent(), "Firefox") 	
	Return rslt	
End Function

Public Function isIE() As Boolean
	Dim rslt As Boolean = Util.exist(getUserAgent(), "MSIE") 	
	Return rslt	
End Function


Public Function getBrowserName() As String
	Dim 	rslt As String = ""
	
	If isIE() Then
		rslt = "MSIE"
	ElseIf isFireFox() Then
		rslt = "Firefox"
	End If
	
	Return rslt
End Function


'*************
'	Events
'
'*************

'!! Keep, _writer is used by all controls.
Protected Overrides Function CreateHtmlTextWriter( _
	writer As TextWriter _
) As HtmlTextWriter
 	'XTrace.showRun("ZXPage.CreateHtmlTextWriter")   
	SetWriter(MyBase.CreateHtmlTextWriter(writer))
	Return getWriter()
End Function


Protected Overridable Sub loadWork() 
	XTrace.showRun("PageBase.loadWork")		
	
Try
	initParams()	
	setUserAgent(Context.Request.UserAgent)	
	AppHome.init(Context)
		
	getXSession().setResponseExpires(Response, 1) 'XX ??	
	_titlePrefix = Util.config_readXmlElementAttribute("/page/title", "prefix")
	'XX	change func	
Catch ex As Exception	
	'Throw
End Try	
	
	XTrace.showEnd("PageBase.loadWork")	
End Sub 'loadWork


'!! don't use shadows
Public Overridable Sub Page_Load(sender As Object, events As EventArgs)	
	XTrace.showRun("PageBase.Page_Load")	
	loadWork()	
	XTrace.showEnd("PageBase.Page_Load")	
End Sub

End Class 'PageBase

End Namespace