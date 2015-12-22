
Namespace zxweb.treeview

'*******************************************************************************
' 	use old tag 'ordering'
'

Public Class SrcNodeCustom
	Inherits XmlSrcNode
	
Dim _pc As IContentProvider


Public Sub New( _
	ByRef nd As XmlElement, _
	ByRef nodes As XmlNodeList, _
	ByRef pct As IContentProvider _
)
	MyBase.New(nd)	
	_nodeList = nodes
	_pc = pct
End Sub
	

Public Overrides Function getTitle() As String
	Dim name, rslt, title As String 
	
	name = getName(True) 'forced to read
	'XTrace.show("SrcNodeCustom.getTitle", "section", name)
	rslt = _pc.getQualifiedPartTitle(name, False) 'no head blanks, with prefix
	'XTrace.show("SrcNodeCustom.getTitle", "qualified", rslt)
	
	title = MyBase.getTitle()
	'XTrace.show("SrcNodeCustom.getTitle", "title", title)
	If title = "" Then
		Dim t2 As String = _pc.getTitleByPartName(name) 
		rslt &= IIf(t2 = "", name, t2)
	Else
	' in meta
		rslt &= title
	End If
	
	'XTrace.show("SrcNodeCustom.getTitle", "rslt", rslt)
	Return rslt
End Function


' section name
Public Overrides Function getName(Optional isForced As Boolean = False) As String
	Dim rslt As String = _node.Name
	
	If rslt = "ordering" Then 
		rslt = "" 'XX
	Else
		If Not isForced Then
			Dim flag As String = _node.GetAttribute("link")
			If Util.isDisabled(flag) Then rslt = ""
		End If
	End If
	
	Return rslt
End Function


Public Overrides Function getChildCount() As Integer
	If _nodeList Is Nothing Then Return -1
	Return _nodeList.Count
End Function

Public Overrides Function getChild(i As Integer) As XmlSrcNode
	Dim child As XmlElement = _nodeList(i)
	Return New SrcNodeCustom(child, child.ChildNodes, _pc) '!! tricky
End Function


Public Overrides Function getEntry() As String
	Dim rslt As String = ""
	
	For Each node As XmlElement In _nodeList		
		If rslt = "" Then rslt = node.Name							
	Next
	
	Return rslt
End Function

End Class 'SrcNodeCustom

End Namespace