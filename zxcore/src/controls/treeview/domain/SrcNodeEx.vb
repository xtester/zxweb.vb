Namespace zxweb.treeview

'*********************************************************
' Class SrcNodeEx
'
' default read in body, no content root.  
'

Public Class SrcNodeEx
	Inherits XmlSrcNode
	
Public Sub New(ByRef nd As XmlElement, ByRef nodes As XmlNodeList)
	MyBase.New(nd)	
	_nodeList = nodes
End Sub
	

Public Overrides Function getTitle() As String
	Dim rslt As String = _node.getAttribute("tab")
	If rslt = "" Then rslt = _node.getAttribute("Tab")
	Return rslt
End Function

Public Overrides Function getName(Optional isForced As Boolean = False) As String
	Return _node.getAttribute("name")
End Function


Public Overrides Function getChildCount() As Integer
	If _nodeList Is Nothing Then Return -1
	Return _nodeList.Count
End Function

Public Overrides Function getChild(i As Integer) As XmlSrcNode
	Dim child As XmlElement = _nodeList(i)
	Return New SrcNodeEx(child, Nothing) '!! tricky
End Function


Public Overrides Function getEntry() As String
	Dim rslt As String = ""
	
	For Each node As XmlElement In _nodeList		
		If rslt = "" Then rslt = node.getAttribute("name")							
	Next
	
	'XTrace.show("SrcNodeEx.getEntry", "rslt", rslt)
	Return rslt
End Function

End Class 'SrcNodeEx

End Namespace