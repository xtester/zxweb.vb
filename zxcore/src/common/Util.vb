
Imports System.Globalization
Imports System.Reflection

Namespace zx.common

'*******************************************************************************
'	Todos:
'		- rm config stuff
'

Public Class Util

'*************
'	Config
'

' Config Module
' parse strings like "/db/path"
'XX
Private Shared Function locate( _
	ByRef reader As XmlTextReader, _
	ByRef target As String _
) As Boolean
	Dim fs, es As Integer ' Slash pos
	Dim slen As Integer = target.Length()
	Dim bLoop As Boolean = TRUE
	Dim kind As String = [String].Copy(target)
	Dim result As Boolean = FALSE ' Error 
	
	If reader Is Nothing OrElse target="" Then Return result
	Try
		fs = kind.IndexOf("/")
		If fs = -1 OrElse fs = slen - 1 Then Return result ' format error
		Do 
			es = kind.IndexOf("/", fs+1)
			If es = fs + 1 Then Return result ' no target, format error
			If es = -1 Then ' not found, to the end
				bLoop = FALSE
				target = kind.Substring(fs+1, slen-fs-1)			
			Else
				target = kind.Substring(fs+1, es-fs-1)			
			End If
			If bLoop Then
				Dim bNotFound As Boolean = TRUE 
				While reader.Read() And bNotFound
					If reader.Name = target Then bNotFound = FALSE
				End While
				If bNotFound Then Return result ' not found			
			End If
			fs = es ' search next 
		Loop Until Not bLoop
		If target<>"" Then result = TRUE	
	Finally
	End Try
	Return result
End Function 'locate


'XX mv
Public Shared Function readXmlElementContent( _
	target As String _
) As String	
	Dim val As String = ""
	Dim reader As XmlTextReader = Nothing
	
	If target = ""  Then return val			
	'Dim reader As XmlValidatingReader = Nothing
      
	Try
		' Load the reader with the data file and 
		' ignore all whitespace nodes. 
		
		reader = New XmlTextReader(SysConfig.getFullConfigFilePath())
		'reader.WhitespaceHandling = WhitespaceHandling.None
		
		' Implement the validating reader over the text reader. 
		' reader = New XmlValidatingReader(txtreader)
		' reader.ValidationType = ValidationType.None
		' Dim loopFlag As Boolean = TRUE
		
		' Util.trace("util.readXmlElementContent", "begin read:" & target)
		If Not locate(reader, target) Then Return ""

		While reader.Read() 
			If reader.Name = target Then 
				' read attributes
				val = reader.ReadString()
				Return val
			End If
		End While
		' Util.trace("util.readXmlElementContent", "val: " & val)
	Catch ex As Exception
		XTrace.show("Util.readXmlElementContent", ex)
	Finally
		If Not (reader Is Nothing) Then
			reader.Close()
		End If
	End Try
	
	Return val
End Function 'readXmlElementContent


'XX mv
Public Shared Function config_readXmlElementAttribute( _
	kind As String, _
	att As String _
) As String
	Dim result As String = ""
	Dim target As String = kind ' extract from kind string
	Dim reader As XmlTextReader = Nothing
	'Dim reader As XmlValidatingReader = Nothing
	
	If att="" Or kind="" Then Return ""			
   	
   	Try
		' Load the reader with the data file and ignore all whitespace nodes. 
		reader = New XmlTextReader(SysConfig.getFullConfigFilePath())
		'reader.WhitespaceHandling = WhitespaceHandling.None
		
		If Not locate(reader, target) Then Return ""
		While reader.Read() 
			If reader.Name = target Then 
				' read attributes
				result = reader.GetAttribute(att)
				Return result
			End If
		End While
	Catch ex As Exception
		' Util.trace("util.readXmlElementContent", "exp: " & ex.ToString())
	Finally
		If Not (reader Is Nothing) Then
			reader.Close()
			reader = Nothing
		End If
	End Try
	
	Return result
End Function


'***********
'	File
'
'***********

'
Public Shared Function readFile( _
	relpath As String, _
	Optional isThrowEx As Boolean = False _
) As String
	Dim rslt As StringBuilder = Nothing
	Dim sr As StreamReader = Nothing
	
	Try
		sr = New StreamReader(AppHome.mapPath(relpath))
		rslt = New StringBuilder(1000) 'XX conf
		
		Dim line As String
		
		Do
			line = sr.ReadLine()
			rslt.Append(line & vbCrLf) 'XX 
		Loop Until line Is Nothing
		
		sr.Close()
	Catch ex As Exception
		XTrace.show("Util.readFile", ex)
		
		If sr IsNot Nothing Then
			sr.Close()
		End If
		
		If isThrowEx Then Throw ex
	End Try
	
	Dim rslt2 As String
	If rslt IsNot Nothing Then
		rslt2 = rslt.ToString()
	End If
	
	Return rslt2
End Function 'readFile


'XX merge
'Public Shared Function readFile( _
'	ByRef cxt As HttpContext, _
'	fname As String _
') As String
'	Dim sb As New StringBuilder()
'	
'	Try
'		' Create an instance of StreamReader to read from a file.
'		Dim sr As StreamReader = New StreamReader(cxt.Server.MapPath(fname))
'		Dim line As String
'		' Read and display the lines from the file until the end 
'		' of the file is reached.
'		Do
'			line = sr.ReadLine()
'			sb.append(line)
'		Loop Until line Is Nothing
'		sr.Close()
'		
'		Return sb.toString()
'	Catch E As Exception
'		sb = Nothing
'		Return ""
'	End Try	
'End Function


'*********************
'	Switch Checker
'

'@@ tested
'
' ignoreCase: rarely used.
'
Public Shared Function isEnabled( _
	ins As String, _
	Optional emptyDefault As Boolean = False, _
	Optional ignoreCase As Boolean = True _	
) As Boolean
	Dim rslt As Boolean = IIf(ins = "", emptyDefault, _
		Util.isame(ins, "yes", ignoreCase) OrElse _
		Util.isame(ins, "true", ignoreCase))
	Return rslt
End Function 


'@@ tested
'
' ignoreCase: rarely used.
'
Public Shared Function isDisabled( _
	ins As String, _
	Optional emptyDefault As Boolean = False, _
	Optional ignoreCase As Boolean = True _	
) As Boolean
	Dim rslt As Boolean = IIf(ins = "", emptyDefault, _	
		Util.isame(ins, "false", ignoreCase) OrElse _
		Util.isame(ins, "no", ignoreCase))
	Return rslt
End Function


'*******************
'	Anchors/Urls
'

' Get string after the last '?'
'XX first or last ?
'
Public Shared Function getQueryString(	_
	raw As String, _
	Optional prefix As String = "" _
) As String
	Dim rslt As String = ""
	Dim slen, si As Integer

Try		
	If raw = "" Then Return ""
	
	slen = raw.Length()
	si = raw.LastIndexOf("?") 'XX first?
	If si >= 0 And slen-1 > si  Then 
		rslt = raw.SubString(si + 1, slen - si - 1)
	End If
	
	If prefix <> "" Then 
		rslt = prefix & rslt
	End If
Catch ex As Exception
End Try
	
	Return rslt
End Function 'getQueryString


Public Shared Function genAnchor( _
	href As String, _
	body As String, _
	Optional tip As String = "", _
	Optional isNewWin As Boolean = False _
) As String
	Dim rslt As String = ""
	Dim param As String = ""
	
	If href = "" And body = "" Then Return ""
	
	If isNewWin Then param = " target='_blank'"
	
	Dim title As String = IIf(tip <> "", " title='" & tip & "'", "")
	
	If href = "" Then
		rslt = "<span" & title & ">" & body & "</span>"
	Else		
		rslt = "<a href='" & href & "'" & title & param & ">" & body & "</a>" 	
	End If
	
	Return rslt
End Function

' Always new win or tab	
Public Shared Function genAnchor2( _
	href As String, _
	body As String _
) As String
	Return genAnchor(href, body, href, True) 
End Function	


Shared Public Function getLinkList(ByRef nvc As NameValueCollection) As String
	Dim rslt As String = ""
	If nvc Is Nothing Then Return ""
	
	Dim isEmpty As Boolean = True
	For i As Integer=0 To nvc.Count-1
		Dim keystr As String = nvc.GetKey(i)
		Dim url As String = nvc.Item(i)
		
		'XX
		If url <> "" Then ' only show valid keys
			If Not isEmpty Then rslt += " | "
			
			rslt += genAnchor(url, keystr, url, True) ' new win	
			isEmpty = False
		End If
	Next
	
	Return rslt
End Function 'getLinkList


'**************
'	Numbers
'
'**************

' Return: 1 at least
'
Public Shared Function convertToPages( _
	ins As String, _
	Optional min As Integer = 1, _
	Optional max As Integer = 1000, _
	Optional defv As Integer = 1
) As Integer
	Dim rslt As Integer = defv	
	If ins = "" Then Return rslt
	
	rslt = getInt(ins)	
	If rslt < min Or rslt > max Then 
		rslt = defv
	End If

	Return rslt
End Function 


' Wrap type change exceptions.
'
' return: -1, error
'
'XX ins="", throw ex?
'
Shared Public Function getInt( _
	ins As String, _	
	Optional deft As Integer = -1 _	
) As Integer
	Dim rslt As Integer = deft
	
	If ins <> "" Then 
		Try
			rslt = CInt(ins)	 
		Catch ex As Exception
			XTrace.show("Util.getInt", ex)
			XTrace.show("Util.getInt>ex", "ins", ins)
		End Try
	End If
	
	Return rslt
End Function


'*************
'	String
'
'*************

' Used in: xtrace
'
' useful?
' var args
Public Shared Sub append(ByRef output As String, ByRef val As String)
	If output Is Nothing Then output = ""
	
	If output IsNot Nothing AndAlso val <> "" Then 		
		output &= val
	End If
End Sub


'XX case-sensitive support
'
' params: returns true if anyone exists
'
Public Shared Function exist( _
	ByRef src As String, _
	ByRef akey As String, _
	ParamArray params() As String _
) As Boolean
	Dim rslt As Boolean = False
	
	If Util.isame(src, akey) Then Return True
	If akey = "" Then Return False 
	
	If src <> "" Then
		Dim loc As Integer = src.IndexOf(akey, _
			StringComparison.OrdinalIgnoreCase)
		rslt = (loc >= 0)
		'XTrace.show("Util.exist", "src", src, "key", key)
	End If
	
	If (Not rslt) AndAlso params IsNot Nothing Then
		For i As Integer = 0 To params.Length - 1
			Dim nextkey As String = params(i)
			
			If src.IndexOf(nextkey, StringComparison.OrdinalIgnoreCase) >= 0 Then
				rslt = True
				Exit For
			End If
		Next
	End If
	
	Return rslt
End Function 'exist

Public Shared Function exist( _
	ByRef src As ArrayList, _
	ByRef value As String, _
	Optional isIgnoreCase As Boolean = True _
) As Boolean
	Dim rslt As Boolean = False
	
	If src Is Nothing OrElse value = "" Then Return False
	
	For i As Integer = 0 To src.Count-1
		If Util.isame(src(i), value, isIgnoreCase) Then
			rslt = True
			Exit For
		End If
	Next
	
	Return rslt
End Function


'XX more target values
Public Shared Function isame( _
	ByRef src As String, _
	ByRef target As String, _
	Optional isIgnoreCase As Boolean = True _
) As Boolean
	Return (String.Compare(src, target, isIgnoreCase) = 0)
End Function


' trim spaces
'
Public Shared Function trim(src As String) As String
	Dim rslt As String = ""
	If src = "" Then Return ""
	
	Dim escapes As Char() = New Char() {" "c} 
	rslt = src.TrimEnd(escapes) 
	rslt = rslt.TrimStart(escapes)	
		
	Return rslt
End Function


' For headlines
' assume no [url] ... within
'
Public Shared Function fixText( _
	ts As String, _
	Optional defaultSLen As Integer = 25, _
	Optional defaultShowLen As Integer = 20 _
) As String	
	Dim rslt As String = ts

Try	
	'textElemIndex = StringInfo.ParseCombiningCharacters(ts)
	'XTrace.show("HeadLine.fixTitle", "len", CStr(teil), "str", ts, "slen", ts.Length)
	If ts.Length() >= defaultSLen Then		
		If False Then
		'XX Trial, incomplete
			Dim sb As New StringBuilder
			Dim charEnum As TextElementEnumerator = StringInfo.GetTextElementEnumerator(ts)
			Dim i As Integer						
			'XTrace.show("HeadLine.fixTitle", "cur", cur, "char", CStr(ts.Chars(10)), "isSurrogate", Char.IsSurrogate(ts.Chars(10)))
			'XTrace.show("HeadLine.fixTitle", "isLetterOrDigit", Char.IsLetterOrDigit(ts.Chars(10)))
			'XTrace.show("HeadLine.fixTitle", "bytes", uni.GetByteCount(ts.Chars(10)))
						
			While charEnum.moveNext() 
				sb.Append(charEnum.GetTextElement())
				i += 1
			End While 
			rslt = sb.Append(" ...").ToString()
		Else
		' real execution
			rslt = ts.Substring(0, defaultShowLen) + " ..."
		End If
	End If
Catch ex As Exception
	XTrace.show("Util.fixText", ex)
End Try

	Return rslt	
End Function 'fixText


' ?? support blank 
' /val1/val2: 3 elements, "", val1, val2
'
' !! / returns Nothing.
'
' delimiter: any length
'
'!! XTrace may cause dead loop
'
Public Shared Function parseValues( _
	ByRef instr As String, _
	Optional delimiter As String = "," _
) As MyArrayList
	Dim posPrevious, posNext As Integer ' opening pos, the nearest closing pos
	Dim rslt As MyArrayList = Nothing
	
'	XTrace.showRun("Util.parseValues")	
	'XTrace.show("Util.parseValues", "input", str)
	If instr = "" Or instr Is Nothing Or delimiter = "" Then 
		Return Nothing
	End If
	
	Dim slen As Integer = instr.Length
	If slen < 1 Then Return Nothing	
	
	Dim delen As Integer = delimiter.Length
	
	Try			
		posPrevious = instr.IndexOf(delimiter)
		
		rslt = New MyArrayList		
		
		If posPrevious < 0 Then 
			rslt.Add(trim(instr)) ' only one element
			Return rslt 
		Else
			rslt.Add(trim(instr.Substring(0, posPrevious))) ' add the first
		End If
				
		Do 
			posNext = instr.IndexOf(delimiter, posPrevious + delen) 	
			
			If posNext < 0 Then ' not found, to the end
				rslt.Add(trim(instr.Substring(posPrevious + delen, _
					slen - posPrevious - delen)))
			Else
				If posNext = posPrevious + delen Then 
					rslt.Add("") '!! series, add an empty value.
				Else
					rslt.Add(trim(instr.Substring(posPrevious + delen, _
						posNext - posPrevious - delen)))
				End If				
			End If
			
			posPrevious = posNext 
		Loop Until posNext < 0
	Catch ex As Exception
		XTrace.show("Util.parseValues", ex)
	End Try	

	'XTrace.show("Util.parseValues", "rslt", rslt.ToString())
	Return rslt
End Function 'parseValues


Public Shared Function isException( _
	ByRef ex As Exception, _	
	match As String _
) As Boolean
	Dim rslt As Boolean = False
	If ex Is Nothing Or match = "" Then
		 Return rslt
	End If
	
	rslt = isame(ex.GetBaseException().GetType().Name, match) 
	Return rslt
End Function	


'***********
'	HTML
'

'XX unused
Public Shared Function genSpan( _
	ByRef text As String, _
	Optional ByRef attrib As String = "" _
) As String
	Dim rslt As String = "<span " & attrib & ">"
	
	rslt &= text
	rslt &= "</span>"
	
	Return rslt
End Function


Shared Public Function genDiv( _
	ByRef attribs As String, _
	Optional ByRef content As String = "" _
) As String
	Dim rslt As String = ""
	If attribs = "" AndAlso content = "" Then Return ""
	
	Dim attstr As String = IIf(attribs = "", "", " " & attribs)
	
	rslt = "<div" & attstr & ">" & content & "</div>"
	Return rslt
End Function


Public Shared Function genHtmlBlanks(n As Integer) As String
	Dim rslt As String = ""
	
	For i As Integer = 1 To n
		rslt &= "&nbsp;"
	Next
	
	Return rslt
End Function


'@tested
'
Public Shared Function split(ins As String, delimeter As String) As ArrayList
	Dim rslt As ArrayList = Nothing
	
	If ins = "" Or delimeter = "" Then Return Nothing
	
	Try
		rslt = New ArrayList
		
		Dim vslen As Integer = ins.Length	
		Dim posDeli As Integer = ins.IndexOf(delimeter)
		
		If posDeli >= 0 Then
			Dim part1 As String = ins.Substring(0, posDeli)
			Dim part2 As String = _
				ins.Substring(posDeli + 1, vslen - posDeli - 1)
			
			rslt.Add(part1)
			rslt.Add(part2)	
		Else
			rslt.Add(ins)			
		End If
	Catch ex As Exception	
	End Try	
	
	Return rslt
End Function 'split


Public Shared Sub decorateTip( _
	ByRef tip As String, _
	isNewWin As Boolean _
) 
	If isNewWin Then
		tip = tip & " *打开新窗口"
	End If
End Sub

End Class 'Util

End Namespace