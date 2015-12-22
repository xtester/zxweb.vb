
Namespace zxweb.treeview

'*******************************************************************************
'	Todos:
'

Public Class SrcNodeBase
	
' _text	
Private _treeid As String
Public Function getViewNidOfTitlePart() As String
	Return _treeid
End Function	
Public Sub setViewNidOfTitlePart(ins As String)
	_treeid = ins	
End Sub

' The base node id in a treeview
Private _viewNid As String
Public Property ViewNid() As String
	Get
		Return _viewNid
	End Get
	Set
		_viewNid = value
	End Set
End Property

	
' Displayed html
Private _title As String
Public Function getBaseTitle() As String
	Return _title
End Function	
'!!
Public Overridable Function getTitle() As String
	Return _title
End Function	
Public Sub setTitle(nm As String)
	_title = nm
End Sub

'XX
Public Property Title As String 
	Get
		return getTitle() 
	End Get
	Set (ps As String)
		setTitle(ps)
	End Set
End Property

Public Overridable Function getTitleTip() As String
	Return ""
End Function	


' node name or nid, not the title
Private _name As String
'XX
Protected Function base_getName() As String
	Return _name 
End Function
Public Overridable Function getName( _
	Optional isForced As Boolean = False _
) As String
	Return _name 
End Function
Public Sub setName(nm As String)
	_name = nm
End Sub


Private _isRoot As Boolean = False
Public Function isRoot(Optional bv As Boolean = False) As Boolean
	If bv = True Then
		_isRoot = bv
	End If
	
	Return _isRoot		
End Function


'*****************
'	Life cycle
'
'*****************

Protected Sub New()
End Sub


Public Sub New(tt As String)	
	_title = tt
End Sub	
	
End Class 'TreeNodeBase

End Namespace