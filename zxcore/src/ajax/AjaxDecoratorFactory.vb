
NameSpace zxweb.ajax

'*******************************************************************************
'	Todos:
'		- assembly name
'

Public Class AjaxDecoratorFactory
	Inherits ControlBase

Private _context As AjaxContext
Private _subject As ControlBase 


'*************
'	Events
'
'*************

'XX
Public Overrides Sub loadControl(Optional eas As EventArgs = Nothing)
	XTrace.showRun("AjaxDecoratorFactory.loadControl")
	
Try	
	'XX use context obj	
	_subject.setParams(_context.getParams()) 
	_subject.setPageContainer(_context.getContainer()) 	
	_subject.setOutputId(_context.getOutputId()) '!! for ajax	 
	_subject.CSSControl = "" '??
	 
	_subject.loadControl(eas)
Catch ex As Exception
	XTrace.show("AjaxDecoratorFactory.loadControl", ex)	
	XTrace.checkNull("AjaxDecoratorFactory.loadControl>ex", "_subject", _subject)
End Try	
End Sub	


' Called by: 
'		AjaxGateway
'
Public Shared Function createInstance( _
	clsn As String, _
	ByRef context As AjaxContext, _
	Optional nspace As String = "zxweb", _
	Optional assembly As String = "" _
) As ControlBase	
	XTrace.showRun("AjaxDecoratorFactory.createInstance")
	
	If clsn = "" Or context Is Nothing Then 
		Return Nothing
	End If
	
	If nspace = "" Then nspace = "zxweb" '!!
	
	Dim fullname As String = nspace & ".AjaxAdapter" & clsn 'XX conf, change prefix	
	XTrace.show("AjaxDecoratorFactory.createInstance", "To create", fullname)
	XTrace.show("AjaxDecoratorFactory.createInstance", "In assembly", assembly)
	
	Dim rslt As New AjaxDecoratorFactory
	rslt._subject = ObjectFactory.getObject(fullname, assembly) 
	rslt._context = context
	
	XTrace.show("AjaxDecoratorFactory.createInstance"£¬"Created", fullname)
	XTrace.showEnd("AjaxDecoratorFactory.createInstance")
	Return rslt
End Function


Protected Overrides Sub OnPreRender(eas As EventArgs)
	XTrace.showRun("AjaxDecoratorFactory.OnPreRender", "Run subject prerender")
	
	Try
		_subject.PreRenderDelegate(eas)				
	Catch ex As Exception	
		XTrace.show("AjaxDecoratorFactory.OnPreRender", ex)
		XTrace.checkNull("AjaxDecoratorFactory.OnPreRender>ex", _
			"_subject",_subject)
	End Try
End Sub


Public Overrides Sub RenderControl(writer As HtmlTextWriter)
	XTrace.showRun("AjaxDecoratorFactory.RenderControl")
	_subject.RenderControl(writer)
End Sub

End Class 'AjaxDecoratorFactory

End Namespace