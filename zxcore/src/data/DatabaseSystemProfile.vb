
NameSpace zxweb.data

'*******************************************************************************
' 	Count user access.
'

Public Class DatabaseSystemProfile 
	Inherits DB

' db name
Public Sub New(name As String)
	MyBase.New(name)
End Sub


'***************
'	Services
' 
'***************

'!! Called by DoorKeeper, use content file path
'XX
Public Function countLoc(loc As String) As Boolean	
	Return countVisit(New LocationRecord(loc), "LOC") '??
End Function


' tarfield: SNAME or LOC - use path directly
'
Public Function countVisit( _
	ByRef dr As LocationRecord, _
	tarfield As String  _
) As Boolean
	Dim i As Integer 
	Dim s, tablename As String
	
Try	
	XTrace.showRun("LocationDB.countVisit")		
	
	If dr.isLocal() Then
		tablename = "Locations"
	Else
		tablename = "LocationsRemote"
	End If
	
	SyncLock GetType(DatabaseSystemProfile)	
		' If the field does not exist, create it.
		If tarfield = "LOC" Then
			s = dr.PATH	' use path		
		Else
			s = dr.SNAME
		End If
		
		If Not existRecord(tablename, tarfield, s) Then
			XTrace.showRun("LocationDB.countVisit", "To create new record")	
			dr.COUNT = "0"
			' dr.Add("NAME", name)		
			' Util.trace("LocationDB.countThisLocation", "Path: " & path)
			insertRecord(tablename, dr)
		Else
			XTrace.show("LocationDB.countVisit", "record exists")	
		End If
		
		If readVisitCount(tablename, s, i, tarfield) Then
			XTrace.show("LocationDB.countVisit", "Read field okay, to update")	
			i += 1
			' Count is db keyword, cannot be used as table name
			updateTableField(tablename, tarfield, s, "COUNTNUM", i)
			dr.COUNT = i		
		Else
			XTrace.show("LocationDB.countVisit", "Warning", "Failed to read count")	
			XTrace.show("LocationDB.countVisit", "table", tablename, "s", s, "target field", tarfield)	
		End If
	End SyncLock 
	
	Return TRUE	
Catch ex As Exception
	XTrace.show("LocationDB.countVisit", ex)
	Return FALSE
End Try
End Function 'countVisit


Public Function countComment( _
	ByRef dr As LocationRecord _
) As Boolean

	Dim i As Integer 
	Dim s, tablename, tarfield As String
	
TRY	
	tablename = "Comments"
	tarfield = "SNAME"
	
	SyncLock GetType(DatabaseSystemProfile)	
		s = dr.SNAME
		
		If Not existRecord(tablename, tarfield, s) Then
			'XX conf
			dr.COUNT = "1" '!! at least once
			insertRecord(tablename, dr)
		End If
		
		If readVisitCount(tablename, s, i, tarfield) Then
			i += 1
			'!! Count is db keyword, cannot be used as table name
			updateTableField(tablename, tarfield, s, "COUNTNUM", i)
			'XTrace.show("LocationDB.countComment", "Location.Count", dr.COUNT, "i", i)
			dr.COUNT = i		
		End If
	End SyncLock 
	
	Return TRUE	
Catch ex As Exception
	XTrace.show("LocationDB.countComment", ex)
	Return FALSE
End TRY
End Function 'countComment


'@@ tested
' Return: False if error
' count: 
'
Public Function readVisitCount( _
	table As String, _
	pkey As String, _
	ByRef count As Integer, _
	Optional tarfield As String = "LOC" _
) As Boolean	
	Dim i As String = ""
	Dim rslt As Boolean = False
	If table = "" Or pkey = "" Then Return False
	
Try 	
	i = readTableField(table, tarfield, pkey, "COUNTNUM")	
	'If i <> "" Then 
	count = CInt(i)
	'End If
	If count >= 0 Then rslt = True 'XX
Catch ex As Exception
	XTrace.show("LocationDB.readVisitCount", ex)
	XTrace.show("LocationDB.readVisitCount>ex", "Table", table, _
		"field", tarfield, "key", pkey, "count", count)
End Try

	Return rslt
End Function 'readVisitCount


'@@ tested
' rs: list of DataRecord
'
'XX
Public Function readTopList( _
	ByRef size As Integer, _
	ByRef rs As ArrayList _
) As Boolean
	If size < 1 Then Return False
	
	Dim rflag As Boolean = FALSE
	Dim i, j As Integer	
	Dim inr As New DataRecord
	
Try 
	inr.add("NAME")
	inr.add("LOC")
	inr.add("COUNTNUM")
	inr.add("SNAME")
	inr.add("SHOWGATE")
	inr.add("CATEGORY")
	inr.setType("COUNTNUM", "Integer")
	
	If readRecordSet("Locations", "", "", inr, rs, _ 
				"", "", "COUNTNUM DESC") Then
		' read ok
		Dim loc, sg, sn As String
		
		j = rs.Count 
		If size > j Then size = j	
		
		For i = 0 To size - 1 
			sg = rs(i).getVal("SHOWGATE")
			sn = rs(i).getVal("SNAME")
			If sn <> "" Then
				If sg <> "" Then
				' designated template
					rs(i).setValue("LOC", sg & "?sname=" & sn) '!! complier not check method
				Else 
				'XX default gateway
				'?? read outer conf ?
					loc = rs(i).getVal("LOC")
					Dim pi As Integer = loc.LastIndexOf(".xml")
					If pi > 0 Or loc = "" Then 
					' it's a xml file, use sname						
					' XXX default gateway set outside
						rs(i).setValue("LOC", "/entry.aspx?sname=" & sn) 'XX conf
					End If
				End If
			End If
		Next	
		If i > 0 Then rflag = True
	End If	
Catch ex As Exception
	XTrace.show("LocationDB.readTopList", ex)
End Try

	return rflag	
End Function 'readTopList


'@@ tested
'XX
Public Function readHitMost( _
	size As Integer, _
	table As String, _
	Optional cmtype As String = "comments", _
	Optional ByRef excludedSNames As ArrayList = Nothing _
) As ArrayList
	Dim rslt, tempa As ArrayList
	tempa = Nothing : rslt = Nothing
	Dim i, j As Integer
	Dim sname, snameid As String
	sname = "" 
	
	If size < 1 Then Return Nothing
	
	Dim inr As New DataRecord			
	inr.add("SNAME") ' sname/id
	inr.add("COUNTNUM")
	inr.setType("COUNTNUM", "Integer")	
	XTrace.show("LocationDB.readHitMost", "table", table)
	readRecordSet(table, "", "", inr, tempa) 	
	XTrace.checkNull("LocationDB.readHitMost", "tempa", tempa)

	If Not tempa Is Nothing Then
		Dim snum As Integer = size
		Dim tmpc As Integer = tempa.Count 
		
		If size > tmpc Then snum = tmpc		
		rslt = New ArrayList(size)
		j = -1
		
		For i = 0 To snum - 1
			' comment should be valid
			Dim found As Boolean = False
			
			Do
				j += 1 	
				'XTrace.show("CommentsDB.readMostRecentComments", "tempc", tmpc, "j", j)
				If (j < tmpc) Then		
					Dim excluded As Boolean = False
					
					'counti = tempa(j).getVal("COUNTNUM") 
					snameid = tempa(j).getVal("SNAME")
					XTrace.show("LocationDB.readHitMost", "snameid", snameid)
					
					If Not excludedSNames Is Nothing Then
						excluded = (excludedSNames.IndexOf(sname) >= 0)
					End If
					
					If Not excluded Then				
						found = True
					End If
				End If
			Loop Until found Or j >= tmpc - 1
			If found Then 
				rslt.Add(tempa(j))
			ElseIf j >= tmpc - 1 Then
				i = snum 'break	
			End If
			'XTrace.show("CommentsDB.readMostRecentComments", "i", i, "j", j)
			'XTrace.show("CommentsDB.readMostRecentComments", "sname", sname)
		Next
		' keep trails empty
		'rslt.RemoveRange(closei, rslt.Count - closei)
	End If	
					
	Return rslt
End Function 'readHitMost

End Class 'LocationDB

End NameSpace