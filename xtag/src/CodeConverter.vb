
NameSpace zxweb.xtag

'*******************************************************************************
'	Usage:
'
'	[program=,boxh=
'

Public Class ProgramConverter
	Inherits XTagConverter

Public Sub New()
	MyBase.New("program", EnumArgType.IsOptional)
	
	setAlias("code")
	setNewWay()
End Sub


'
Protected Overrides Function transformTag( _
	Optional ByRef outIsRsltOk As Boolean = True _
) As String
	Dim rslt, codes, fname As String	
	rslt = ""
	
	fname = getParam()	
	If fname <> "" Then
		codes = Util.readFile("/archive/program/" & fname) 'XX
		codes = XTransformer.process(codes) '?? recursive
	Else
		codes = CurrentTag.Body
	End If
	
	If codes <> "" Then
		Dim sts As String = CType(CurrentTag(), XTag).getBoxStyle(False, 2) 
		'no auto calc
		
		rslt = "<div class='source_code' " & sts & ">"
		rslt &= "<pre>" & codes & "</pre></div>"
	End If
	
	'XTrace.show("ProgramConverter.transform", "rslt", rslt)
	Return rslt		
End Function 'transform
	
End Class 'ProgramConverter


'*******************************************************************************
'

Public Class Code2Converter
	Inherits XTagConverter

Public Sub New()
	MyBase.New("code2", True)
	setArgOptional()
	setAlias("c")
	setNewWay(True)
End Sub


Protected Overrides Function transformTag( _
	Optional ByRef outIsRsltOk As Boolean = True _
) As String
	Dim rslt, codes As String	
	rslt = ""
	
	If Not CurrentTag.isCompact() Then
		codes = CurrentTag.Body
	Else 
		codes = CurrentTag.getParam()
	End If 
	
	If codes <> "" Then
		Dim style As String = "style='font-size: 12px; " & _
		"font-family: ""Courier New"", Courier, mono; background-color: white; " & _
		"border-style: solid; border-width: 1px; border-color: #E9E9E9;	" & _
		"padding-left: 5px; padding-right: 5px;'"
		
		rslt = "<span " & style & ">" & codes & "</span>"
	End If
	
	'XTrace.show("Code2Converter.transformTag", "rslt", rslt)
	Return rslt		
End Function 'transformTag
	
End Class 'Code2Converter

End Namespace