
Imports System.Xml

NameSpace zxweb.treeview

'*******************************************************************************
'	For XML providers.
'

Public Class XmlSrcNode
	Inherits SrcNodeBase	

Protected _node As XmlElement
Protected _nodeList As XmlNodeList


'****************
'	Lifecycle
'
'****************

Protected Sub New()
End Sub

Public Sub New(tt As String)	
	setTitle(tt)
End Sub	

Public Sub New(ByRef nd As XmlElement)
	_node = nd
	If Not _node Is Nothing Then
		_nodeList = _node.SelectNodes("node")
	End If
End Sub


'***************
'	Services
'
'***************

'XX
Public Overrides Function getTitle() As String
	Dim rslt As String = _node.getAttribute("text")
	
	If rslt = "" Then rslt =  _node.getAttribute("tab")
	
	Return rslt
End Function


' section name
Public Overrides Function getName( _
	Optional isForced As Boolean = False _
) As String
	Dim rslt As String = _node.getAttribute("link")	
	
	If rslt = "" Then
		'XX
		Dim qnas As String = _node.getAttribute("qnas")	
		If qnas <> "" Then
			rslt = _node.getAttribute("text")	
		End If
	End If
	
	Return rslt
End Function


Public Overridable Function getChildCount() As Integer
	Return _nodeList.Count
End Function

Public Overridable Function getChild(i As Integer) As XmlSrcNode
	Dim child As XmlElement = _nodeList(i)
	Return New XmlSrcNode(child)
End Function


Public Overridable Function getEntry() As String
	Return _node.getAttribute("entry")
End Function


' Find attribute value
Public Function find( _
	partn As String, _
	attrib As String, _
	Optional recursive As Boolean = False _
) As String
	Dim rslt As String = ""	
	Dim target As XmlElement = Nothing
	
	'XTrace.showRun("XmlSrcNode.find")
	'XTrace.show("XmlSrcNode.find", "Part", partn, "Attribute", attrib)
	
	Try		
		target = _node.SelectSingleNode("//node[@link='" & partn & "']")
		
		If target Is Nothing Then
			' For qnas node
			target = _node.SelectSingleNode("//node[@text='" & partn & "']")
		End If
			
		If target Is Nothing Then
			target = _node.SelectSingleNode("//" & partn) 'XX error for chinese name
		End If
		
		If target IsNot Nothing Then
			rslt = target.Attributes.ItemOf(attrib).Value
		End If
	Catch ex As Exception		
		'XTrace.show("XmlSrcNode.find", ex)
	End Try
	
	If target IsNot Nothing And rslt = "" And recursive Then 
		rslt = findUp(target.ParentNode, attrib)
	End If
	
	Return rslt
End Function 'find


'XX rename 'ordering'
'
Protected Function findUp(ByRef curnode As XmlElement, attrib As String) As String
	Dim rslt As String = ""	
	'XTrace.show("XmlSrcNode.find2", "attrib", attrib)
	
	Try
		rslt = curnode.Attributes.ItemOf(attrib).Value
	Catch ex As Exception		
		'XTrace.show("XmlSrcNode.find2", ex)
	End Try
	
	If curnode Is Nothing OrElse curnode.LocalName = "root" OrElse _
			curnode.LocalName = "ordering" Then 		
		Return rslt
	End If	
	
	If rslt = "" Then rslt = findUp(curnode.ParentNode, attrib)

	Return rslt
End Function


Public Overridable Function getIndex() As String	
	Return ""	
End Function

End Class 'XmlSrcNode

End Namespace