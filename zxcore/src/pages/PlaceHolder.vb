' Creation Date: 2013/11/1
' Copyright (c) Xun Zhang
' www.zhangxun.com

Namespace zxweb

'*******************************************************************************
'	Used together with ViewTemplate.
'

Public Class PlaceHolder
	Inherits XControl
	
Protected Overrides Sub initControl()
	MyBase.initControl()
	
	If _name = "" Then _name = "PlaceHolder/" 'For trace
End Sub	


'XX
Protected Overrides Sub output()
	XTrace.showRun("PlaceHolder.output")
	XTrace.show("PlaceHolder.output", "SName", SName)
		
	Dim tag As String = ViewManager.getBlockName( _
		getXResource().getViewTemplate())
	XTrace.show("PlaceHolder.output", "tag", tag)		
	
	Dim node As XmlElement = TemplateFactory.getTemplateNode("views/" & tag)		
	XTrace.checkNull("PlaceHolder.output", "Template node", node)	
	'XX when node null
	
	'XX
	XTrace.showRun("PlaceHolder.output", "Processing the node")
	Dim rslt As String = ControlLoader.process(node, Me, Nothing, getPage())	
	write(rslt)
	
'	If rslt <> "" Then
'		XTrace.show("PlaceHolder.output", "rslt (" & rslt.Length & ")", rslt)
'	Else
'		XTrace.show("PlaceHolder.output", "rslt empty")
'	End If
	
	XTrace.showEnd("PlaceHolder.output")
End Sub	'output
	
End Class 'PlaceHolder

End Namespace