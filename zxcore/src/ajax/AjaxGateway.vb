
NameSpace zxweb.ajax

'*******************************************************************************
'	For loading ajax controls
'		MemberComments, MostRecentComments, MostRecentQnas, TreeBrowser
'
'	Todos:
'		- inherits pagebase directly ??
'

Public Class AjaxGateway
	Inherits ZXPage 'XX why need sname processing ??

Protected _ctrlname As String
Protected _ctrltype As String ' config or program
Protected _ctrlId As String ' the container

Private _nspace As String
	
Protected _outputId As String

Private _target As ControlBase


'*************
'	Events
'
'*************

'XX init, combine
Protected Overrides Sub initParams()
	MyBase.initParams()	
	
	_ctrlname = getParam("ctrl")	'!! ctrl config name? class name
	_ctrlId = getParam("ctrlid")
	_nspace = getParam("namespace")
	
	_outputId = getParam("output")		
	
	XTrace.show("AjaxGateway.initParams", "ns", _nspace, _
		"ctrlname", _ctrlname£¬ "ctrlid", _ctrlId, "outputid", _outputId)
End Sub


'XX duplicate?
'!! don't use mybase
Public Overrides Sub Page_Load(sender As Object, events As EventArgs)
Try	
	XTrace.showRun("AjaxGateway.Page_Load")
	loadWork()
	
	' CORS
	Response.AppendHeader("Access-Control-Allow-Origin", "*") 'XX config	
	
	_target = getControl()			
	
	XTrace.showRun("AjaxGateway.Page_Load", "Loading _target " & _
		_ctrlname & ":" & _ctrlId)
	_target.loadControl() 		
	XTrace.show("AjaxGateway.Page_Load", "Target control loaded.")
Catch ex As Exception
	XTrace.show("AjaxGateway.Page_Load", ex)	
	XTrace.show("AjaxGateway.Page_Load>ex", "_ctrlname", _ctrlname, "_ctrlId", _
		_ctrlId, "_nspace", _nspace)
	XTrace.checkNull("AjaxGateway.Page_Load>ex", "_target", _target)	
End Try	
End Sub


Protected Overridable Function getControl() As ControlBase
	Dim rslt As ControlBase = Nothing
	
	XTrace.show("AjaxGateway.getControl", "ctrl", _ctrlname, "nsapce", _nspace)	
	
	If _ctrlname = "" Then 
		XTrace.show("AjaxGateway.getControl", "Error", "_ctrlname empty")		
		Return Nothing
	End If
	
	Dim ajc As New AjaxContext(Me, getParams(), _outputId)
	
	Dim assembly As String = getParam("assembly")
	XTrace.show("AjaxGateway.getControl", "Assembly", assembly)
	
	'default package zxweb
	rslt = AjaxDecoratorFactory.createInstance(_ctrlname, ajc, _nspace, _
		assembly)
	Return rslt
End Function


Public Overridable Sub Page_PreRender(sender As Object, eargs As EventArgs)			
	XTrace.showRun("AjaxGateway.Page_PreRender")
	
Try	
	_target.preRenderDelegate(eargs) 		
Catch ex As Exception
	XTrace.show("AjaxGateway.Page_PreRender", ex)	
	XTrace.checkNull("AjaxGateway.Page_PreRender>ex", "_target", _target)			
End Try	
End Sub


Protected Overrides Sub Render(writer As HtmlTextWriter)	
Try				
	XTrace.showRun("AjaxGateway.Render")	
	
	_target.RenderControl(writer)
	
	setFinished(True) '!! useful?
	'Dim t1, t2 As DateTime
	'Time.setTime(t1)
	
	'Time.setTime(t2)
	'Time.show("AjaxGateway.Render>AjaxMostRecentComments", t2, t1)			
Catch ex As Exception
	XTrace.show("AjaxGateway.Render", ex)
End Try	
End Sub 'Render


'**************
'	Helpers
'
'**************

Public Shared Function getPath() As String
	Dim rslt As String = ControlsConfig.readProperty("ajax/gateway")
	Return rslt
End Function


' ctrln: class name of the target server control, without prefix
' ctrlid: ??
'
Public Shared Function getLink( _
	ctrln As String, _
	ctrlid As String, _
	Optional nspace As String = "", _
	Optional inAssembly As String = "" _
) As String
	If ctrln = "" Or ctrlid = "" Then Return ""
	
	Dim rslt As String = getPath() & "?forced=yes" 'XX
	
	rslt &= "&ctrl=" & ctrln
	rslt &= "&ctrlid=" & ctrlid
	
	If nspace <> "" Then rslt &= "&namespace=" & nspace
	If inAssembly <> "" Then rslt &= "&assembly=" & inAssembly
		
	Return rslt
End Function

End Class 'AjaxGateway

End Namespace