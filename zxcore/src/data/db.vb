
Imports System.Data.OleDb

NameSpace zxweb 'XX

'*******************************************************************************
'	Database operations
'

Public Class DB

Private _name As String
Private _dbstr As String

'XX rm
Private Shared _server As HttpServerUtility


Private _conn As OleDbConnection 
Private Function getConnection() As OleDbConnection
	Return _conn
End Function


Protected Sub closeConnection()
	If Not _conn Is Nothing Then
		_conn.Close()
	End If	
End Sub


'*****************
'	Life cycle
'
'*****************

Public Sub New(n As String) 
	_name = n 
	_conn = Nothing
	
	' create conn
	initConnection()
End Sub


Public Shared Sub init(ByRef context As HttpContext) 'ByRef context As HTTPContext
	_server = context.Server
End Sub


' It can be called seperately.
'
Private Function initConnection( _
	Optional boolReset As Boolean = FALSE _
) As Boolean
	Dim ps, dbp, dbstr As String
	dbstr = ""
	Dim rslt As Boolean = False
	
Try	
	If Not _conn Is Nothing Then
		If boolReset Then
		' close it first, than create a new one
			_conn.Close() 
			_conn = Nothing
		Else 
			Return True 'XX already created. 2009-1-17
		End If
	End If

	If _name = "" Then Return False 'XX	
	
	ps = Util.readXmlElementContent("/db/provider_str")
	dbp = Util.config_readXmlElementAttribute("/db/" & _name, "path")
	dbstr = ps & " Data Source=" & _server.MapPath(dbp) & ";"	
		
	_conn = New OleDbConnection(dbstr) 
	'Return - put here useless, still run finally ?	
	If _conn Is Nothing Then
		' Util.trace("db.initConnection", "db conn error: " & dbstr)	
	Else
		' Util.trace("db.initConnection", "db conn established: " & dbstr)		
		rslt = True
	End If		
Catch ex As Exception
	_conn = Nothing	
	XTrace.show("db.initConnection", "dbstr", dbstr)
	XTrace.show("db.initConnection", ex)
End Try

	Return rslt
End Function 'initConnection


'XX pre-check [] in tablen
Private Function genSQLString( _
	tablen As String, _
	Optional key As String = "" _
) As String	
	Dim rslt As String
	
	If tablen = "" Then Return ""
	
	If key = "" Then
		rslt = "SELECT * FROM [" & tablen & "]"
	Else
		rslt = "SELECT * FROM [" & tablen & "] where " & key & "=?"
	End If
	
	Return rslt
End Function


'***************
' 	Services
'
'***************

' 
'
Public Function existRecord( _
	table As String, _
	field1 As String, _
	val As String _
) As Boolean	
	Dim rslt As Boolean = False
	Dim sql As String 
	Dim usrReader As OleDbDataReader = Nothing			
	Dim objCommand As OleDbCommand

Try
	XTrace.show("db.existRecord", "table", table, "target field", field1, "val", val)
	
	If table = "" Or field1 = "" Then Return False	
	If Not initConnection() Then Return False 
	
	sql = genSQLString(table, field1)
	objCommand = New OleDbCommand(sql, _conn)
	objCommand.Parameters.Add("@p1", OleDbType.BSTR).Value = val ' types other than bstr
	
	_conn.Close() : _conn.Open()		

	XTrace.show("db.existRecord", "sql", sql)
	
	usrReader = objCommand.ExecuteReader()
	If usrReader.Read() Then rslt = TRUE	
Catch ex As Exception
	XTrace.show("db.existRecord", ex)
Finally
	If Not usrReader Is Nothing Then usrReader.Close()
	objCommand = Nothing
	_conn.Close()			
End Try	

	'Util.trace("db.existRecord", "field:" & field1 & ",val:" & val)
	Return rslt
End Function 'existRecord


' read usr record 'pfield' in 'table'
'  	where 'field1' = val
'
Public Function readTableField( _
	table As String, _
	field1 As String, _
	val As String, _
	pfield As String _
) As String	
	Dim result, sql As String 
	result = ""
	Dim usrReader As OleDbDataReader = Nothing			
	Dim objCommand As OleDbCommand

Try
	XTrace.show("DB.readTableField", "table", table, "key", field1, "val", val, "pfield", pfield)	
	If table = "" Or field1 = "" Or pfield = "" Then Return ""	
	If Not initConnection() Then Return "" 'XX
	
	sql = genSQLString(table, field1)
	objCommand = New OleDbCommand(sql, _conn)
	objCommand.Parameters.Add("@p1", OleDbType.BSTR).Value = val ' types other than bstr
	
	_conn.Close() : _conn.Open()		
	' Catch ex As InvalidOperationException ' prevent conn already open after exception	
	'XTrace.show("db.readTableField", "sql", sql)
	
	usrReader = objCommand.ExecuteReader()
	If usrReader.Read() Then
		Select Case pfield ' not flexible
			Case "ID", "COUNTNUM"
				result = usrReader.GetInt32(usrReader.GetOrdinal(pfield))
			Case "REGTIME","EDITTIME","LOGNTIME"
				result = usrReader.GetDateTime(usrReader.GetOrdinal(pfield))
			Case "Register"
				result = usrReader.GetInt32(usrReader.GetOrdinal(pfield))		
			Case Else
				result = usrReader.GetString(usrReader.GetOrdinal(pfield))
		End Select
	End If		
Catch ex As Exception ' 2006-01-31
	XTrace.show("db.readTableField", "Read table", table, "Read Field", pfield)
	XTrace.show("db.readTableField", "where Field " & field1, val)
	XTrace.show("db.readTableField", ex)
	
	result = ""
Finally
	If Not usrReader Is Nothing Then usrReader.Close()
	objCommand = Nothing
	_conn.Close()		
End Try	

	' Util.trace("db:readTableField", "name:" & val & ",result:" & result)
	Return result
End Function 'readTableField	


' Update one field
'
Public Function updateTableField( _
	table As String, _
	key As String, _
	val As String, _
	pfield As String, _
	data As Object _
) As Boolean
	
	updateTableField = FALSE		
	
	'Util.trace("db.updateTableField", table & "," & key & "," & val & "," & pfield)
	
	If table="" Or key="" Or val="" Or pfield="" Then Return FALSE
	
	Dim SQL As String = "SELECT * FROM " & table & " WHERE " & key & " = '" & val & "'" 
	
	Dim Profile As New DataSet
	Dim ProfileAdpt As New OleDbDataAdapter
	Dim objCommand As OleDbCommand
	Dim ProfileCB As OleDbCommandBuilder
	
Try	
	initConnection()
	If _conn Is Nothing Then Return FALSE ' failed

	objCommand = New OleDbCommand(SQL, _conn)
	ProfileAdpt.SelectCommand = objCommand
	ProfileCB = New OleDbCommandBuilder(ProfileAdpt)

	_conn.Close()
	_conn.Open()		 
	ProfileAdpt.Fill(Profile)
	
	' what if target not found ?
	Profile.Tables(0).Rows(0)(pfield) = data ' type should match

	ProfileAdpt.Update(Profile) 		
	_conn.Close()
	updateTableField = TRUE
	
	'Util.trace("db.updateTableField", "data: " & data.ToString())
Catch ex As Exception
	XTrace.show("db.updateTableField", ex)
End Try											

	Profile = Nothing
	ProfileAdpt = Nothing
	objCommand = Nothing
	ProfileCB = Nothing
	'return 
End Function 'updateTableField


' Read single record
'
' !! table: should not inlucde [,]
'
Public Function readRecord( _
	table As String, _
	key As String, _
	val As String, _
	ByRef inoutRecord As DataRecord _
) As Boolean
		
	Dim strSQLQuery As String = "SELECT * FROM [" & table & "] WHERE " & key & " = ?" 
	Dim usrReader As OleDbDataReader 
	
Try
	' Util.trace("db.readRecord", "run ...")
	initConnection()
	If _conn Is Nothing Then
		 ' Util.trace("db.readRecord", "db conn null")
		 Return FALSE ' failed
	ElseIf inoutRecord Is Nothing Then
		 ' Util.trace("db.readRecord", "iorecord null")
		 Return FALSE ' failed
	ElseIf inoutRecord.getCount() = 0  Then 
		 ' Util.trace("db.readRecord", "iorecord count 0")
		 Return FALSE ' failed
	End If 
		
	Dim objCommand As OleDbCommand = New OleDbCommand(strSQLQuery, _conn)
	objCommand.Parameters.Add("@p1", OleDbType.BSTR).Value = val
	
	_conn.Close() ' avoiding exception
	_conn.Open()		
	' Util.trace("db.readRecord", "sql: " + objCommand.CommandText + " " + val)
	usrReader = objCommand.ExecuteReader()
	readRecord = False
	If usrReader.Read() Then 
		Dim i, li As Integer
		Dim ty As String
		Dim rval As Object = Nothing
		
		Dim fn As String() = inoutRecord.getAllkeys()
		
		' Util.trace("db.readRecord", "run loop")
		For i = 0 To inoutRecord.getCount() - 1
			Try 
				li = usrReader.GetOrdinal(fn(i))	
				rval = ""
				If Not usrReader.IsDBNull(li) Then
					If Not inoutRecord.hasTypes() Then
						rval = usrReader.GetString(li)
					Else
						ty = inoutRecord.getValType(fn(i))
						'Util.trace("db.readRecord", "type:" & ty & ",key:" & fn(i))
						Select Case ty ' what if usrreader not find this field exist, or bad field name
							Case "DateTime"
								rval = usrReader.GetDateTime(li)
							Case "Integer"
								rval = usrReader.GetInt32(li)
							Case Else
								rval = usrReader.GetString(li)
						End Select
					End If
				End If	
'				Util.trace("db.readRecord", "key: " & fn(i) & ",val: " & inoutRecord.getVal(fn(i)))
			Catch ex As IndexOutOfRangeException
				rval = ""
			Finally
				inoutRecord.setValue(fn(i), rval)
			End Try
		Next 
		readRecord = TRUE
	End If
Catch ex As Exception
	readRecord = FALSE
	XTrace.show("db.readRecord", ex)
Finally
	If Not _conn Is Nothing Then _conn.Close()													
End Try
End Function 'readRecord


Public Function readRecords( _
	table As String, _
	ByRef returnedRecords As ArrayList, _
	ByRef selectedFields As DataRecord, _ 
	Optional condition As Array = Nothing, _
) As Boolean
	Dim rslt As Boolean = False
	If table = "" Then Return rslt

	Dim akey As String = ""
	Dim value As String = ""
	If condition IsNot Nothing AndAlso condition.Length > 1 Then
		akey = condition(0)
		value = condition(1)
	End If
	
	rslt = readRecordSet(table, akey, value, selectedFields, returnedRecords)
		
	Return rslt
End Function


' table: better with [...], or may fail
' key/val: where condition match the key/val pair
' inoutRecord: store query fields and their types
'
'XX
Public Function readRecordSet( _
	table As String, _
	key As String, _ 
	val As String, _
	ByRef inoutRecord As DataRecord, _ 
	ByRef rs As ArrayList, _
	Optional sel As String = "*", _
	Optional whc As String = "", _
	Optional ord As String = "" _
) As Boolean		
	Dim usrReader As OleDbDataReader
	Dim scnt, sql As String 		
	scnt = ""
	Dim objCommand As OleDbCommand
	Dim rslt As Boolean = False
	
Try
	XTrace.showRun("db.readRecordSet")
	
	initConnection()
	If _conn Is Nothing OrElse table = "" Then Return FALSE 
	
	If sel = "" Then sel = "*" ' for all, in case param error
	
	If inoutRecord Is Nothing OrElse inoutRecord.getCount() = 0 Then
		scnt = "COUNT"
		sel = "*"
	End If
	
	' Preparing the SQL string
	If key <> "" And Val <> "" And whc = "" Then
		' Order ?
	 	sql = "SELECT " & sel & " FROM " & table & " WHERE " & key & " = ?" 
		'XTrace.show("db.readRecordSet", "SQL", sql)
		objCommand = New OleDbCommand(sql, _conn)
		objCommand.Parameters.Add("@p1", OleDbType.BSTR).Value = val
	Else
		If key = "" And Val = "" And whc <> "" Then
			sql = "SELECT " & sel & " FROM " & table & " WHERE " & whc
		ElseIf key = "" And Val = "" And whc = "" Then
			sql = "SELECT " & sel & " FROM " & table
		Else
			Return False
		End If
		
		If ord <> "" Then 
			sql &= " Order by " & ord ' Keep heading blank
		End If
		
		objCommand = New OleDbCommand(sql, _conn)			
	End If
	XTrace.show("DB.readRecordSet", "SQL prepared", sql)	
	
	_conn.Close() : _conn.Open()		
	usrReader = objCommand.ExecuteReader()
	
	Dim i, li As Integer
	Dim ty As String
	Dim rval As Object
	Dim nr As DataRecord
	
	XTrace.show("DB.readRecordSet", "After reader executed")	
	'XTrace.show("DB.readRecordSet", "scnt", scnt)	
	
	While usrReader.Read() 
		nr = New DataRecord()
		
		If scnt <> "COUNT" Then
		' Read each field
			Dim fn As String() = inoutRecord.getAllkeys()
			Dim j As Integer = inoutRecord.getCount() - 1
			'XTrace.show("DB.readRecordSet", "count-j", j)
			
			For i = 0 To j
				li = usrReader.GetOrdinal(fn(i))	
				'XTrace.show("db.readRecordSet", "li", li, "field", fn(i))
				If Not usrReader.IsDBNull(li) Then
					If Not inoutRecord.hasTypes() Then ' Default 
						rval = usrReader.GetString(li)
					Else
						ty = inoutRecord.getValType(fn(i))
						'Util.trace("db.readRecord", "type:" & ty & ",key:" & fn(i))
						Select Case ty ' what if usrreader not find this field exist, or bad field name
							Case "DateTime"
								rval = usrReader.GetDateTime(li)
							Case "Integer"
								rval = usrReader.GetInt32(li)
							Case Else
								rval = usrReader.GetString(li)
						End Select
					End If
				Else 
					rval = ""
				End If
				'XTrace.show("DB.readRecordSet", "[" & i & "]", fn(i), "Value", rval)
				nr.add(fn(i), rval)
			Next 
		End If
		
		If rs Is Nothing Then rs = New ArrayList
		rs.Add(nr)
	End While
	
	rslt = TRUE
Catch ex As Exception	
	XTrace.show("db.readRecordSet", ex)
	XTrace.show("db.readRecordSet>ex", inoutRecord.ToString())
Finally
	If _conn IsNot Nothing Then _conn.Close()													
End Try

	XTrace.showEnd("db.readRecordSet")
	Return rslt
End Function 'readRecordSet


' Write to DB, update
'
Public Function writeRecord( _
	table As String, _
	key As String, _
	val As String, _
	ByRef inRecord As DataRecord _
) As Boolean
	Dim sql As String = "SELECT * FROM " & table & " WHERE " & key & " = '" & val & "'"
	
Try
	'Util.trace("db.writeRecord", "before write table1")

	initConnection()
	If _conn Is Nothing Or inRecord Is Nothing OrElse inRecord.getCount() = 0 _
		 Then Return FALSE ' failed
	
	Dim Profile As New DataSet()
	Dim ProfileAdpt As New OleDbDataAdapter()

	Dim objCommand As New OleDbCommand(sql, _conn)
	ProfileAdpt.SelectCommand = objCommand
	Dim ProfileCB As New OleDbCommandBuilder(ProfileAdpt)

	_conn.Close() ' in case already open exception
	_conn.Open()		 
	ProfileAdpt.Fill(Profile)

	' Util.trace("db.writeRecord", "before write table2")
	Dim fn As String() = inRecord.getAllkeys()
	Dim ty As String
	Dim i As Integer
	For i = 0 To inRecord.getCount() - 1
		If Not inRecord.hasTypes() Then
			Profile.Tables(0).Rows(0)(fn(i)) = inRecord.getVal(fn(i))
		Else
			ty = inRecord.getValType(fn(i))
			Dim inval As Object = inRecord.getVal(fn(i))
			' Util.trace("db.writeRecord", "type:" & ty & ",key:" & fn(i))
			Select Case ty 
			' what if usrreader not find this field exist, or bad field name
				Case "DateTime"
					Profile.Tables(0).Rows(0)(fn(i)) = DateTime.Parse(inval)
				Case "Integer"
					Profile.Tables(0).Rows(0)(fn(i)) = CInt(inval)
				Case Else
					Profile.Tables(0).Rows(0)(fn(i)) = inval
			End Select
		End If
		' Util.trace("db.writeRecord", ",key:" & fn(i) & ",val:" & inRecord(fn(i)))
	Next 	

	ProfileAdpt.Update(Profile) 		
	
	writeRecord = TRUE
Catch ex As Exception
	writeRecord = FALSE
	'XTrace.show("db.insertRecord", ex)
Finally
	If Not _conn Is Nothing Then _conn.Close()													
End Try
End Function 'writeRecord


' Insert a new record into the table
' Usage:
' 	  inr.Add("RANK", pc("Rank"))
'  	  inr.setType("REGTIME","DateTime")
'
Public Function insertRecord( _
	table As String, _
	ByRef pc As DataRecord _
) As Boolean
	Dim sql As String
	Dim i, j As Integer
	
	insertRecord = TRUE	
Try		
	initConnection()
	If _conn Is Nothing Or table Is Nothing Or _
		pc Is Nothing OrElse pc.getCount() = 0 _
	Then Return FALSE ' failed
	' Util.trace("db.insertRecord", "begin")

	sql = "INSERT INTO " & table & " ("	
	j = pc.getCount()
	Dim fn As String() = pc.getAllkeys()
	
	For i = 1 To j-1
		If pc.hasIndex(fn(i-1)) Then sql &= "["
		sql &= fn(i-1)
		If pc.hasIndex(fn(i-1)) Then sql &= "]"
		sql &= ","
	Next 
	If pc.hasIndex(fn(j-1)) Then sql &= "["
	sql &= fn(j-1) 
	If pc.hasIndex(fn(j-1)) Then sql &= "]"
	sql &= ") VALUES ("
	
	For i = 1 To j-1
		sql &= "@cp" & i & ", "
	Next 
	sql &= "@cp" & j & ");"

	'Util.trace("db.insertRecord", "db: " & _name & ", command: " & sql)
	Dim objCommand As New OleDbCommand(sql, _conn)	
	Dim odp As OleDbParameter
	Dim ty As String
	
	For i = 1 To j
		If Not pc.hasTypes() Then
			'Util.trace("db.insertRecord", "no types")

			odp = New OleDbParameter("@cp" & i, OleDbType.BSTR)
			odp.Value = pc.getVal(fn(i-1))
		Else
			ty = pc.getValType(fn(i-1))
			Select Case ty
				Case "DateTime"
					'Util.trace("db.insertRecord", "type is DateTime")				
					odp = New OleDbParameter("@cp" & i, OleDbType.Variant)
					odp.Value = DateTime.Parse(pc.getVal(fn(i-1)))
				Case "Integer"
					odp = New OleDbParameter("@cp" & i, OleDbType.Integer)
					odp.Value = CInt(pc.getVal(fn(i-1)))
					'Util.trace("db.insertRecord", "type is Integer, value: " & odp.Value)
				Case Else
					odp = New OleDbParameter("@cp" & i, OleDbType.BSTR)
					odp.Value = pc.getVal(fn(i-1))
					'Util.trace("db.insertRecord", "type is String, value: " & odp.Value)
		  	End Select
		End If		
		objCommand.Parameters.Add(odp) 
	Next

	_conn.Close() : _conn.Open()
	objCommand.ExecuteNonQuery()
	
	' Util.trace("db.insertRecord", "sucess")
Catch ex As Exception
	If ex.ToString().IndexOf("重复的值") >= 0 Then
		XTrace.showRun("db.insertRecord", "记录已存在")
	Else
		XTrace.show("db.insertRecord", ex)
	End If
	'XTrace.show("db.insertRecord", "SQL statements", sql)	
	insertRecord = FALSE
Finally
	If Not _conn Is Nothing Then _conn.Close()	
End Try
End Function 'insertRecord

End Class 'DB

End NameSpace