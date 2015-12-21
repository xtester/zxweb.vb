' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2014/10/12

Namespace zxweb.xtag

'*******************************************************************************
'

Public Class SvgConverter
	Inherits ImgConverter
	
'*****************
'	Life cycle
'
'*****************

Public Sub New()
	newHelper("svg", True)
End Sub


Public Sub New( _
	Optional ctxt As Object = Nothing _
)
	newHelper("svg", True)
	setContext(ctxt)
End Sub


'***************
'	Services
'
'***************

Private Function parseTop(ins As String) As String
	Dim rslt As String = ""
	Dim valueMap As New Hashtable
	
	If ins = "" Then Return ""
	
	Dim strLen As Integer = ins.Length
	Dim raw As String = ins.Substring(1, strLen - 2) 'XX strip	
	XTrace.show("SvgConverter.parseTop", "raw", raw)
	
	Dim valueList As MyArrayList = Util.parseValues(raw, ";")
	If valueList IsNot Nothing Then
		For i As Integer = 0 To valueList.Count - 1
			Try
				Dim valuestr As String = valueList(i) 	
				Dim strlist As ArrayList = Util.split(valuestr, ":")
				
				XTrace.show("SvgConverter.parseTop>for", _
					"key" & i, strlist(0), "value" & i, strlist(1))
				
				Dim hashkey As String = ""
				Select Case strlist(0)
					Case "ie"
						hashkey = "MSIE"
					Case "ff"
						hashkey = "Firefox"
				End Select
				
				valueMap.Add(hashkey, strlist(1))				
			Catch ex As Exception				
			End Try
		Next	
	End If 
	
	Dim browser As String = ""
	Try
		browser = CType(getContext(), String)
	Catch ex As Exception	
		'XX
	End Try

	rslt = valueMap(browser)
	XTrace.show("SvgConverter.parseTop", "browser", browser, _
		"rslt", rslt)

	Return rslt
End Function 'parseTop

	
Protected Overrides Function transform( _
	ByRef middle As String _
) As String
	XTrace.showRun("SvgConverter")
	Dim rslt As String = ""
	
	Dim fpath As String = getParam()	
	If fpath = "" Then Return ""
	
	rslt = Util.readFile(fpath)	
		
	Dim isCentered As Boolean = True
	If isCentered Then
		rslt = "<center>" & rslt & "</center>"
	End If
		
	Dim width As String = getParam("w")
	Dim height As String = getParam("h")
	Dim topValue As String = getParam("top")
	Dim left As String = getParam("left")
	
	If width <> "" Or height <> "" Or topValue <> "" Or left <> "" Then
		Dim style As String = "position: relative;"
		
		If width <> "" Then
			style &= " width: " & width & "px;"
		End If
		
		If height <> "" Then
			style &= " height: " & height & "px;"
		End If
				
		If topValue <> "" Then 
			style &= " top: " & parseTop(topValue) & "px;"			
		End If
		
		If left <> "" Then
			style &= " left: " & left & "px;"
		End If
		
		rslt = "<div style=""" & style & """>" & rslt & "</div>" 
	End If

	'XTrace.show("SvgConverter", "rslt", rslt)
	Return rslt
End Function	
	
End Class 'SvgConverter

End Namespace