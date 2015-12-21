
NameSpace zxweb.xtag

'*******************************************************************************
'

Public Class TextTransformer
	Implements ITextTransformer	
	
Private _context As Object
Public Sub setContext(ByRef ctxt As Object)
	_context = ctxt	
End Sub
Public Function getContext() As Object
	Return _context	
End Function


Private _chain As ArrayList
' For extension
Public Function getChain() As ArrayList _
		Implements ITextTransformer.getChain	
	Return _chain
End Function
'XX interface
Private Sub setChain(ByRef ch As ArrayList)
	_chain = ch
End Sub


'XX Filter
Private _excludes As String
Public Function getExcludes() As String _
		Implements ITextTransformer.getExcludes		
	Return _excludes
End Function
Public Sub setExcludes(list As String) _
		Implements ITextTransformer.setExcludes	
	_excludes = list
End Sub


Private _escapes As New ArrayList

Public Function getEscapes() As ArrayList _
		Implements ITextTransformer.getEscapedTags
	Return _escapes
End Function

Sub addEscapedTag(ByRef map As EscapedTagInfo) _
		Implements ITextTransformer.addEscapedTag	
	If map Is Nothing Then Return	
	_escapes.Add(map)
End Sub


'***************** 
'	Life cycle 
'
'*****************

Public Sub New()
	init()	
End Sub

Public Sub New(ByRef ctxt As Object)
	setContext(ctxt)
	
	init()	
End Sub


Protected Overridable Sub init()
	If _chain Is Nothing Then
		setChain(createChain())
	End If
	
	buildChain(Me) 
End Sub


'**************
'	Service
'
'**************

Private Sub buildChain( _
	Optional ByRef master As ITextTransformer = Nothing _
) 
	Dim converter As ConverterBase
	
Try	
	Dim cvts As ArrayList = getChain()
	
	Dim sz As Integer = cvts.Count	
	If sz < 2 Then 
		If sz > 0 Then
			converter = cvts(0)
			converter.setMaster(master)
		End If
		
		Return '!!
	End If
	'XTrace.show("TextTransformer.init", "size", sz)
	
	For i As Integer = 0 To sz - 2
		converter = cvts(i)
		converter.setNextConverter(cvts(i + 1))
		converter.setMaster(master) '!!
	Next
	
	converter = cvts(sz - 1)
	converter.setMaster(master)
Catch ex As Exception
	XTrace.show("TextTransformer.buildChain", ex)
End Try
End Sub 'buildChain


Private Function getFirstConverter() As XTagConverter
	Dim rslt As ArrayList = getChain()
	Return rslt(0)
End Function


'!! keep src intact
Public Overridable Function run(ByRef src As String) As String _
		Implements ITextTransformer.run	
	Dim rslt As String
	
	If src = "" Then Return ""
			
	Dim cvt As XTagConverter = getFirstConverter()	
	cvt.feed(String.Copy(src))	' copy and feed
	rslt = cvt.run()		
	
	Return rslt
End Function


'!! create new one
'XX conf
'
Private Function createChain() As ArrayList
	Dim cvts As New ArrayList

Try
	'XX bad for svg and xml	
	cvts.Add(New BreakLineConverter()) ' <br />
	
	' Links
	'	
	'!! put the longer ahead
	cvts.Add(New LinkMarkConverter("url"))

	'! without new blank win
	cvts.Add(New LinkMarkConverter("local2", True)) ' True, use default host
	'!! with the _blank
	cvts.Add(New LinkMarkConverter("local", True)) 
	
	' HTML
	'
	'XX
	cvts.Add(New HtmlConverter("h1"))
	cvts.Add(New CssClassConverter("h2new"))
	cvts.Add(New HtmlConverter("h2"))
	cvts.Add(New HtmlConverter("h3"))
	
	' Text
	cvts.Add(New HtmlConverter("i"))	
	cvts.Add(New HtmlConverter("strong"))	
	cvts.Add(New HtmlConverter("del", "s"))	
	cvts.Add(New XTagConverter("u", EnumArgType.IsOptional)) 	
	
	cvts.Add(New LineListConverter("ol"))
	cvts.Add(New LineListConverter("ul"))
	cvts.Add(New DotLineConverter()) ' dotline		
	
	' Images
	cvts.Add(New ImgConverter(getContext())) ' img
	
	' Blanks
	cvts.Add(New BlankConverter()) 'blank
	cvts.Add(New IndentConverter()) 'indent
	
	' Hr
	cvts.Add(New HrConverter()) 		
	
	' Box
	cvts.Add(New RefConverter()) ' ref	
	cvts.Add(New NoteConverter()) ' note	
	
	cvts.Add(New XResourceConverter()) ' [[...]]			
	
	' Sections
	'
	cvts.Add(New Subtitle3Converter()) 'sub3
	
	cvts.Add(New Sub2iConverter()) '!! sub2i, place before sub2
	cvts.Add(New Subtitle2Converter()) 'sub2
	
	cvts.Add(New SublinkConverter()) ' sublink, for sidebar menus
	
	' Additional
	'
	cvts.Add(New DownloadConverter()) 'downl	
	
	cvts.Add(New WikipediaConverter()) 'wkpd
Catch ex As Exception
	XTrace.show("TextTransformer.createChain", ex)
End Try
	
	Return cvts
End Function 'createChain


Public Sub append(ByRef cvtrlist As ArrayList) _
		Implements ITextTransformer.append	
	If cvtrlist Is Nothing Then Return
	
	Dim isChanged As Boolean = False
	Dim cl As ArrayList = getChain()			
	
	For Each cvtr As ConverterBase In cvtrlist
		If cvtr IsNot Nothing Then 
			cl.Add(cvtr)
			isChanged = True
		End If
	Next
	
	If isChanged Then init() '!! reset 
End Sub

End Class 'TextTransformer

End Namespace