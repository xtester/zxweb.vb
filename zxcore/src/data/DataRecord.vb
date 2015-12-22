NameSpace zxweb.domain 'XX

'*******************************************************************************
'

' Can it contain raw object ?
'
Public Class DataRecord
	
' the data	
Private _record As NameValueCollection

Private _types As NameValueCollection

'!!
Private _indexFlag As NameValueCollection


Private _tableName As String
Public Sub setTable(name As String)
	_tableName = name
End Sub
Public Function getTable() As String
	Return _tableName
End Function


'*****************
'	Life cycle
'
'*****************

Public Sub New()
	_record = New NameValueCollection
	_types = New NameValueCollection
	_indexFlag = New NameValueCollection
End Sub


Public Sub New(ByRef nvc As NameValueCollection)
	_record = nvc
	_types = New NameValueCollection
	_indexFlag = New NameValueCollection
End Sub


'***************
' 	Services
'
'***************

Public Sub setIndex(f As String)
	_indexFlag.Add(f, "Yes")
End Sub


Public Function hasIndex(f As String) As Boolean
	hasIndex = FALSE
	If _indexFlag(f)="Yes" Then hasIndex = True
End Function


'!! Add is append ?
Public Sub add( _
	column As String, _
	Optional val As Object = "" _
)
	If val Is Nothing Then 
		_record.Add(column, "") 
	Else
		_record.Add(column, val.toString()) '?
	End If
End Sub


Public Sub add( _
	column As String, _
	val As Object, _
	ty As String, _
	Optional ByVal idx As Boolean = FALSE _
)
	_record.Add(column, val.toString()) '?
	If ty<>"" And ty<>"String" Then setType(column, ty)
	If idx Then setIndex(column)
End Sub


Public Function getVal(column As String) As Object
	If _record Is Nothing Then Return Nothing
	
	Return _record(column)
End Function


'XX auto add new key?
Public Function setValue(column As String, val As Object) As Boolean
	If _record Is Nothing Then Return False
	
	_record(column) = val.toString()
	Return TRUE
End Function


'************
' 	types
'

Public Function hasTypes() As Boolean
	If _types Is Nothing Or _types.Count = 0 Then 
		Return FALSE
	End If
	
	Return TRUE
End Function


Public Function getValType(i As Integer) As String
	If _types Is Nothing Then Return ""
	
	Return _types(i)
End Function


Public Function getValType(column As String) As String
	If _types Is Nothing Then Return ""
	
	Return _types(column)
End Function


Public Sub setType(column As String, ty As String)
	If _types Is Nothing Then Return 
	
	_types(column) = ty
End Sub 


Public Function getCount() As Integer
	If _record Is Nothing Then Return 0
	
	Return _record.Count
End Function


Public Function getAllkeys() As String()
	If _record Is Nothing Then Return Nothing
	
	Return _record.Allkeys
End Function


Public Function getKey(i As Integer) As String
'	If i < 0 Then		 
'	End If
	Dim rslt As String = _record.AllKeys(i)
	Return rslt
End Function


'**************
'	Helpers
'
'**************

'XX
Public Function toNVC() As NameValueCollection
	If _record Is Nothing Then Return Nothing
	
	Dim tmp As New NameValueCollection
	Dim i As Integer
	Dim ks As String() = _record.AllKeys
	
	For i = 0 To _record.Count - 1
		tmp.Add(ks(i), _record(i).toString())
	Next
	
	Return tmp
End Function


' format
Public Overrides Function ToString() As String
	Dim ks() As String	
	If _record Is Nothing Then Return ""
	
	Dim sb As New StringBuilder
	
	ks = _record.AllKeys
	Dim cnt As Integer = _record.Count
	For i As Integer=0 To cnt-1
		sb.Append(ks(i) & ":" & _record(i).toString())
		If i < cnt-1 Then sb.Append(" ") 'XX
	Next

	Return sb.ToString()
End Function

End Class 'DataRecord

End NameSpace