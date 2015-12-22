
NameSpace zxweb.treeview

'*******************************************************************************
'

Public Class AjaxAdapterTreeView
	Inherits TreeBrowser
	
Private Shared _handlerSpace As String = "zxj.treeview.nodeHandlers." 'XX
	
Protected _section As String
Public Sub setSection(s As String)
	_section = s
End Sub


' load the xresource by sname
'
'XX merge
Protected Overrides Function getXmlFile() As XmlFile
	Dim xf As XmlFile = Nothing 
	
Try
	Dim sname As String = getSName()	
	xf = AppHome.getFactoryResource().createInstance(sname).getXmlFile()
	XTrace.show("AjaxAdapterTreeView.getXmlFile", "XmlFile loaded for", sname)
Catch ex As Exception
	XTrace.show("AjaxAdapterTreeView.getXmlFile", ex)
End Try

	Return xf
End Function


Protected Overrides Function getEntrySection() As String
	Return _section
End Function


'!!
Protected Overrides Function getNodeHandler( _
	nid As String, _
	Optional isTrail As Boolean = True, _
	Optional onlyName As Boolean = False _	
) As String
	Dim rslt As String = _handlerSpace & MyBase.getNodeHandler(nid)			
	'XTrace.show("AjaxAdapterTreeView.getNodeHandler", "rslt", rslt)
	Return rslt
End Function


Protected Overrides Function genFunctionHeader(
	ByRef node As TreeNode _
) As String
	If node Is Nothing Then Return ""

	Dim funcname As String = MyBase.getNodeHandler(node.getId(), False, True)
	Dim rslt As String = _handlerSpace & funcname & " = function () {"	
	'XTrace.show("AjaxAdapterTreeView.genFunctionHeader", "rslt", rslt)
	Return rslt
End Function


Protected Overrides Sub process(ByRef start As XmlSrcNode)
	processTree(start, getNodeIdBase(), _lastSectionId)
	
	genNodeFunctions()
End Sub


'XX
Protected Overrides Sub output()		
	Dim postActions As String = ""	
	XTrace.showRun("AjaxAdapterTreeView.output")

	write(XPage.genOutputTag(getClientOutputId())) 'XX helper
	MyBase.output() 'no entry

Try	
	postActions = getSavedNodeScripts() 
	
	Dim entryNode As TreeNode = getEntryNode()
	If entryNode Is Nothing Then
		entryNode = _nodes.Item(_firstSectionId)
	End If
	
	postActions &= getNodeHandler(entryNode.getId()) 	
'	XTrace.show("AjaxAdapterTreeView.output", _
'		"postActions (" & postActions.Length & _
'		")", postActions)
	writePostActions(postActions) 
Catch ex As Exception
	XTrace.show("AjaxAdapterTreeView.output", ex)
End Try
	
	XTrace.showEnd("AjaxAdapterTreeView.output")
End Sub 'output

End Class 'AjaxAdapterTreeView


'*******************************************************************************
' 	tricky
'

Public Class AjaxAdapterTreeBrowser
	Inherits AjaxAdapterTreeView
	
'XX	
Public Overrides Sub loadControl(Optional eas As EventArgs = Nothing)
	setSName(_params("sname"))		
	setSection(_params("section"))
	 
	MyBase.loadControl(eas)
End Sub	

End Class

End Namespace