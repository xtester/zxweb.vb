
NameSpace zxweb.controllers

'*******************************************************************************
'

Public Class ControllerBase
	Implements IController
	
Private Shared _actionFile As XmlFile	

Private _context As HttpContext
Public Function getContext() As HttpContext	
	Return _context
End Function
Public Sub setContext(ByRef contxt As HttpContext)
	_context = contxt
End Sub


Private _xsess As XSession
Protected Function getXSession() As XSession
	Return _xsess
End Function


Private _action As String
Public Function getAction() As String
	Return _action 
End Function
Public Sub setAction(act As String)
	_action = act
End Sub


'************
'	Life cycles
'
'

Protected Sub New()
End Sub	

Protected Sub init()
	_xsess = XSession.getObject(_context)
End Sub


Protected Function getRequest() As HttpRequest
	Return _context.Request
End Function

Protected Function getResponse() As HttpResponse
	Return _context.Response
End Function


Protected Function getHttpParameters() As NameValueCollection
	Dim rslt As NameValueCollection = getRequest().Params
	Return rslt
End Function

Protected Function getHttpParam(key As String) As String
	If key = "" Then Return ""
	Dim rslt As String = ""
	Dim nvc As NameValueCollection = getHttpParameters()
	
	If nvc IsNot Nothing Then rslt = nvc(key)
	Return rslt
End Function



'XX
Protected Sub redirect(view As String)
	getResponse().Redirect(view)
End Sub


'
Private Shared Function getActionFile() As XmlFile
	If _actionFile Is Nothing OrElse _actionFile.isUpdated() Then 
		_actionFile = XmlFile.load2(SysConfig.getActionsFilePath())
	End If
	
	Return _actionFile
End Function


Private Shared _controllerFactory As ObjectFactory

Public Shared Function getFactory() As ObjectFactory
	If _controllerFactory Is Nothing Then
		_controllerFactory = New ObjectFactory(New ActionFileAdapter(getActionFile()))
	End If
	
	Return _controllerFactory
End Function


Public Shared Function getInstance(ByRef contxt As HttpContext, command As String) _
		As IController
	Dim rslt As ControllerBase = Nothing	
	
	'XTrace.show("ControllerBase.getInstance", "action", command, "assembly", assembly, "class", classname)	
	rslt = CType(getFactory().getInstance(command), IController)	
	If rslt IsNot Nothing Then
		rslt.setContext(contxt)
		rslt.setAction(command)			
		rslt.init()
	Else
		'XX remove last contxt
		XSession.getObject(contxt).handleError("抱歉！控制器 " & command & _
			" 不存在。", contxt)
	End If
	
	Return rslt
End Function 'getInstance	


' Services

Protected Overridable Sub forwardToView()
	Dim view As String
	Dim action As String = getAction()
	
	view = getActionFile().readXml("/root/action[@name='" & action & "']/view")
	XTrace.show("ControllerBase.forwardToView", "view", view, "action", action)

	If view = "" Then view = "/" 'XX
	
	redirect(view)
End Sub


Protected Overridable Sub handleAction()
End Sub


Public Overridable Sub run() _
		Implements IController.run
	Try
		handleAction()
		forwardToView()
	Catch ex As ActionHandlingException
		redirect(ex.getView())
	End Try
End Sub


Protected Sub addCookie(ByRef cookie As HttpCookie) 
	getResponse().Cookies.Add(cookie)
End Sub


Protected Sub handleSystemError(ByRef msg As String) 
	getXSession().handleError(msg, getContext())	
End Sub

End Class 'ControllerBase

End NameSpace