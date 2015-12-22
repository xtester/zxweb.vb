Namespace zxweb.data

'*******************************************************************************
'	A wrapper for database fields.
'
'	Todos:
'		- merge sname and path?
'

Public Class LocationRecord 
	Inherits DataRecord

Private _domain As String = "Internal" ' Outer, Remote


'*****************
'	Properties
'
'*****************

Public Property SNAME As String
	Get
		Return getVal("SNAME")	
	End Get
	Set (ps As String)
		add("SNAME", ps)
	End Set
End Property


' for loc search other than sname, like comment sname/id
'
Public Property PATH As String
	Get
		Return getVal("LOC")
	End Get
	Set (ps As String)
		add("LOC", ps)
	End Set
End Property


Public Property COUNT As String
	Get
		Return getVal("COUNTNUM")
	End Get
	Set (ps As String)
		setValue("COUNTNUM", ps) '!! don't use Add
		setType("COUNTNUM", "Integer")
	End Set
End Property


'*****************
'	Life cycle 
'
'*****************

' internal
Protected Sub New()
	MyBase.New()
End Sub


' Called by: LocationDB
'XX
Public Sub New(p As String)
	MyBase.New()	

	PATH = p '!! not use sname
	_domain = "Internal"
	
	add("TYPE", "File") ' ?? type is always file ?	
End Sub


' Called by: 
'		Comment, XResource
'
Public Shared Function createInstance( _
	sn As String, _
	Optional isRemote As Boolean = False _
) As LocationRecord
	If sn = "" Then Return Nothing
	
	Dim rslt As New LocationRecord
	
	rslt.SNAME = sn
	rslt._domain = IIf(isRemote, "Outer", "Internal") '!!
		
	Return rslt	
End Function


'*************
'	Helper 
'
'*************

Public Function isLocal() As Boolean
	Return _domain = "Internal"
End Function

End Class 'LocationRecord

End Namespace