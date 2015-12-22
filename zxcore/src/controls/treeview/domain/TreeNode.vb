
NameSpace zxweb.treeview

'*******************************************************************************
'   A node in the tree.
'

Public Class TreeNode
	
' nid	
Private _id As String
Public Function getId() As String
	Return _id
End Function

Private _title As String
Public Function getTitle() As String
	Return _title
End Function

' section name
Private _section As String 'page tab name
Public Function getSection() As String
	Return _section
End Function

Private _index As Integer
Public Function getIndex() As Integer
	Return _index
End Function
Public Sub setIndex(i As Integer) 
	_index = i	
End Sub


Public Sub New(i As String, t As String, s As String)
	_id = i
	_title = t
	_section = s
End Sub

Public Overrides Function ToString() As String
	Dim rslt As String = "index:" & _index & " id:" & _id & " section:" _
		& _section & " title:" & _title
	Return rslt
End Function

End Class 'TreeNode

End Namespace