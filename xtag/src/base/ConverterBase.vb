' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/4/1

Namespace zxweb.xtag

'*******************************************************************************
'	Todos:
'		- Multiple aliases
'

Public MustInherit Class ConverterBase
	
'XX	
Public Shared _appCenter As IAppCenter
Public Shared Sub setAppCenter(ByRef ac As IAppCenter)
	_appCenter = ac
End Sub


' The context could be anything.
'!! 
'XX mv
Private _context As Object
Public Sub setContext(ByRef obj As Object)
	_context = obj
End Sub
Protected Function getContext() As Object
	Return _context	
End Function


'XX merge?
Private _key As String
Public Function getKey() As String
	Return _key	
End Function
Protected Sub setKey(k As String)
	_key = k
End Sub


Private _aliasKey As String
Public Sub setAlias(ins As String)
	_aliasKey = ins	
End Sub


' For HTML tags
'XX mvin
Private _isAllowEmptyBody As Boolean 
Public Property IsAllowEmptyBody() As Boolean
	Get
		Return _isAllowEmptyBody
	End Get
	Set
		_isAllowEmptyBody = value
	End Set
End Property


' When body is empty
Private _isBodyFromArg As Boolean = True
Public Property IsBodyFromArg() As Boolean
	Get
		Return _isBodyFromArg
	End Get
	Set
		_isBodyFromArg = value
	End Set
End Property


'XX rm
Public Function getArgString() As String
	Return CurrentTag().Argstr
End Function


Private _isUseDefaultHost As Boolean = False
Public Sub specifyHost(bv As Boolean)
	_isUseDefaultHost = bv
End Sub

Private _defaultHost As String ' usually a domain name

' For imgs
Public Function getDefaultHost() As String
	If _isUseDefaultHost Then
		'If _defaultHost = "" Then _defaultHost = _appCenter.getSite()
	Else
		_defaultHost = ""
	End If
	
	Return _defaultHost
End Function


' The original text
Private _original As String
Public Function getTextInput() As String
	Return _original	
End Function

'!! no copy
Public Sub feed(ByRef src As String) 
	_original = src
End Sub


Private _nextConverter As XTagConverter
Public Sub setNextConverter(nextc As XTagConverter)
	_nextConverter = nextc
End Sub


Private _transformer As ITextTransformer
'!! keep public, or exception
Public Sub setMaster(ByRef master As ITextTransformer)
	_transformer = master
End Sub
Public Function getMaster() As ITextTransformer
	Return _transformer
End Function


'***************** 
'	Life cycle 
'
'*****************

'!! for raw
Public Sub New()
End Sub

Public Sub New(ByRef tag As AbstractTag)
	_currentTag = tag	
End Sub

'!! keep arg order 
'XX
'
' Params:
'	useDefaultHost - the running server
'
Public Sub New( _
	tkey As String, _
	Optional isRequired As Boolean = False, _
	Optional useDefaultHost As Boolean = False _
)
	_isHaveParams = isRequired
	newHelper(tkey, isRequired, useDefaultHost)
End Sub


'XX rm
Protected Sub setTags( _
	inkey As String, _
	Optional haveValue As Boolean = False _
)
	If inkey = "" Then Return
	
	'XX 
	setKey(inkey	)	
	setCurrentTag(New XTag(inkey, haveValue))
End Sub


Private _currentTag As AbstractTag
Public ReadOnly Property CurrentTag() As AbstractTag
	Get
		Return _currentTag
	End Get
End Property
Public Function setCurrentTag(ByRef tag As AbstractTag) As AbstractTag
	_currentTag = tag
	Return _currentTag
End Function


' Called by: 
'	ImgConverterEx...
'
'XX merge
Protected Sub newHelper( _
	akey As String, _
	Optional haveValue As Boolean = False, _
	Optional useDefaultHost As Boolean = False _
)
	setTags(akey, haveValue)			
	specifyHost(useDefaultHost) 'XX
End Sub

Protected Sub newHelper( _
	inkey As String, _
	argType As EnumArgType _
)	
	setKey(inkey	)		
	setCurrentTag(New XTag(inkey, argType))		
End Sub


'XX mvin
Private _isHaveParams As Boolean = True ' default
Private Function isArgsRequired() As Boolean
	Return _isHaveParams	
End Function


'XX 
Public Sub setArgOptional()	
	If CurrentTag IsNot Nothing Then
		CurrentTag.setArgOptional()				
	End If
End Sub


'***************
'	Services
'
'***************

' Return: true, no tag or alias exist
'
Private Function isTagInvalid(ByRef src As String) As Boolean	
	Dim rslt As Boolean = False
	
	If _currentTag Is Nothing Or src Is Nothing Or src = "" Then Return True
	
	If _currentTag.HeaderBeginMark = "" OrElse _currentTag.Footer = "" OrElse _
		((Not Util.exist(src, _currentTag.HeaderBeginMark)) AndAlso _
		(_aliasKey <> "" And Not Util.exist(src, _aliasKey))) _
			Then
'		XTrace.show("ConverterBase.isTagInvalid", _
'			"_beginTag", _beginTag, _
'			"_endTag", _endTag)
		rslt = True
	End If
	
	Return rslt
End Function


' Preconditions: run feed()
'
Public Overridable Function run() As String
	Dim rslt As String = ""
	
Try				
	XTrace.showRun("ConverterBase.run", "Processing <strong>'" & _
		_currentTag.Name & _
		"'</strong> tags")
	
	rslt = getTextInput() 	
	If rslt = "" Or isTagInvalid(rslt) Then 
		'XX exist
		XTrace.warn("ConverterBase.run", "rslt empty or tag invalid, passon")		
		If rslt <> "" Then
			passon(rslt)
		End If	
			
		'XTrace.showEnd("ConverterBase.run")
		Return rslt 
	End If
	
	XTrace.show("ConverterBase.run", "Try 1"	)	
	rslt = makeChanges(rslt, _currentTag) 
	
	'XX
	If CurrentTag.isArgOptional() Then 
		XTrace.show("ConverterBase.run", "Try no param"	)	
		rslt = makeChanges(rslt, New XTag(getKey(), False))
	End If
	
	'XX extend to alias list
	If _aliasKey <> "" Then
		XTrace.show("ConverterBase.run", "Try alias", _aliasKey)	
		rslt = makeChanges(rslt, New XTag(_aliasKey, CurrentTag.getArgType())) 
		'XX support more params		
		
	Else
		XTrace.show("ConverterBase.run", "No alias")	
	End If	
Catch ex As Exception
	XTrace.show("ConverterBase.run", ex)
	' keep change made, don't clear rslt
End Try

	passon(rslt)	

	'XTrace.showEnd("ConverterBase.run")
	Return rslt	
End Function 'run


' ins:
' intag: the current tag
'
Private Function makeChanges( 
	ByRef ioText As String, _	
	Optional ByRef intag As AbstractTag = Nothing _
) As String
	XTrace.showRun("ConverterBase.makeChanges")
	If ioText = "" Then 
		XTrace.warn("ConverterBase.makeChanges", "Invalid input, return")
		Return ""
	End If
	
	If intag Is Nothing Then
		intag = CurrentTag
	Else
		setCurrentTag(intag)
	End If
	'!! intag is not used hereafter
	
	Dim fp, posEndMark, slen, posSearchStart As Integer '!! keep here
	posSearchStart = 0	
	
	Dim rslt As String = ioText	
	slen = rslt.Length
		
	If Not Util.exist(ioText, CurrentTag.HeaderBeginMark) Then
		XTrace.warn("ConverterBase.makeChanges", "'" & _
			CurrentTag.HeaderBeginMark & "' " & _
			"not found, return")
		Return ioText
	End If
	
	XTrace.show("ConverterBase.makeChanges", _
		"HeaderBeginMark", CurrentTag.HeaderBeginMark, _
		"Footer", CurrentTag.Footer, _
		"Arg type", CurrentTag.getArgType().ToString(), _
		"isArgsRequired", isArgsRequired(), _
		"isArgOptional", CurrentTag.isArgOptional())
	
	XTrace.showRun("ConverterBase.makeChanges", "Start loop")
	
	Do	
		' change next tag ...
		Dim isCompact As Boolean = False
		_currentTag.setCompact(False) '!! init
		
		fp = rslt.IndexOf(CurrentTag.HeaderBeginMark, posSearchStart) 
		' the front pointer
				
		' Set ep
		'
		If fp >= 0 Then 
			'~~ use func
'			If Util.exist(CurrentTag.HeaderBeginMark, "[code") Then
'				Dim size As Integer = IIf(rslt.Length - fp < 15, _
'					rslt.Length - fp, 15)
'				XTrace.show("ConverterBase.makeChanges", _
'					"Lead", rslt.Substring(fp, size))
'			End If 
			
			Dim posCHCM As Integer = _
				rslt.IndexOf(CurrentTag.CompactHeaderClosingMark, fp) 
			' Compact Head Closing Mark
			Dim posFooter As Integer = rslt.IndexOf(CurrentTag.Footer, fp) 
			' normal
			Dim posNextTag As Integer = rslt.IndexOf( _
				CurrentTag.HeaderBeginMark, fp + 1)

			XTrace.show("ConverterBase.makeChanges", "posCHCM", posCHCM, _
				"posFooter", posFooter, "posNextTag", posNextTag)			
			
			'XX simplify
			If posCHCM < 0 Or _
					((posNextTag < 0 Or posNextTag > posFooter) _
						AndAlso posFooter > posCHCM) Or _
					(posFooter > 0 And posFooter < posCHCM) Or _
					(posCHCM > 0 AndAlso rslt.Chars(posCHCM - 1) <> " "c _
						AndAlso rslt.Chars(posCHCM - 1) <> "/"c) _
			Then
				XTrace.show("ConverterBase.makeChanges", _
					"Normal end mark detected")
				posEndMark = posFooter ' [/...] normal end pointer
			Else
				' Check for [code in [code2 the like.
				'				
				Dim isFormatOk As Boolean = False
				Dim posEqual As Integer = fp + CurrentTag.HeaderBeginMark.Length
				If isArgsRequired() AndAlso _
						Util.exist(CurrentTag.HeaderBeginMark, "=") Then
					'XX
					posEqual -= 1
				End If
				
				Dim equalChar As String = rslt.Chars(posEqual)
				XTrace.show("ConverterBase.makeChanges", "equalChar", equalChar)
				
				If equalChar <> "/" Then
					If equalChar = "=" OrElse equalChar = " " Then
						isFormatOk = True
					End If
				Else
					'XX It should be '/]'
					If posCHCM = posEqual Then
						isFormatOk = True
					End If
				End If
				
				If isFormatOk Then
					posEndMark = posCHCM ' compact end
					isCompact = True
					_currentTag.setCompact(True)
				Else
					XTrace.warn("ConverterBase.makeChanges", _
						"Bad compact format, pass and continue searching...")
					
					posSearchStart = fp + 1 '!! don't use posFooter
					Continue Do
				End If
			End If
		End If
		XTrace.show("ConverterBase.makeChanges", "isCompact", isCompact)
			
		If fp >= 0 And fp < posEndMark Then
			Dim posArgs, posHCM As Integer
			
			posArgs = fp + CurrentTag.HeaderBeginMark.Length ' might have '='
			XTrace.show("ConverterBase.makeChanges", "fp", fp, _
				"HeaderBeginMark.Length", CurrentTag.HeaderBeginMark.Length)
			If rslt.Chars(posArgs) = "="c Then
				posArgs += 1 ' first arg after =
			Else
				If rslt.Chars(posArgs) = " "c Then
					posArgs = -1 ' no arg
				End If
			End If			
			
			If isCompact Then
				posHCM = posEndMark - 1 '!! exclude the empty char
			Else
				posHCM = rslt.IndexOf(CurrentTag.HeaderClosingMark, posArgs) 
				' normally the ]
			End If
			XTrace.show("ConverterBase.makeChanges", _
				"posEndMark", posEndMark, _
				"posArgs", posArgs, "posHCM", posHCM)
			
			If posHCM <= posEndMark And posHCM > 0 Then			
				Dim args As String = ""
				If posArgs > 0 Then
					XTrace.show("ConverterBase.makeChanges", _
						"Char at pos_" & posArgs, rslt.Chars(posArgs))
					args = rslt.Substring(posArgs, posHCM - posArgs)
				End If
				'XX use method? not include ]
				XTrace.show("ConverterBase.makeChanges", "args", args)
				
				_currentTag.parseParams(args)								
				
				Dim body As String = ""
				If Not isCompact Then 
					Dim middleStart As Integer = posHCM + CurrentTag.HeaderClosingMark.Length
					body = rslt.SubString(middleStart, posEndMark - middleStart) 
					' not include the closing [/
				End If
				
				'~~ 
'				If Util.exist(CurrentTag.HeaderBeginMark, "[sub") Then
'					XTrace.show("ConverterBase.makeChanges", "Body", body)
'				End If 
				
				_currentTag.setBody(body)
				
				Dim lenEndMark As Integer = IIf(isCompact, _
					CurrentTag.CompactHeaderClosingMark.Length, _
					CurrentTag.Footer.Length)
				
				_currentTag.setWhole(rslt.Substring(fp, _
					posEndMark + lenEndMark - fp))
				
				' Make change
				'
				If posArgs > 0 AndAlso rslt.Chars(posArgs) = "^"c Then					
					' Escape					
					XTrace.show("ConverterBase.makeChanges", "It's escaped.")
					getMaster().addEscapedTag(New EscapedTagInfo(CurrentTag))
				Else
					XTrace.show("ConverterBase.makeChanges", "Normal operation")
					slen = transform2(rslt) '!! reset str len					
				End If				
				
				' go ahead
				posSearchStart = posEndMark 	
			Else
				Throw New Exception("Parse error: bad ']' ")								
			End If
		Else			
			XTrace.show("ConverterBase.makeChanges", "fp", fp, "ep", posEndMark)
			XTrace.warn("ConverterBase.makeChanges", "Parse error, exit loop")			
			posSearchStart = slen 
		End If
		
		XTrace.showRun("ConverterBase.makeChanges", "Loop continues")
		'XTrace.show("ConverterBase.makeChanges", "startp", startp)
	Loop While posSearchStart < slen '- _endTag.Length '??	
	
	XTrace.showEnd("ConverterBase.makeChanges")
	Return rslt
End Function 'makeChanges


Protected Sub passon(ByRef ioText As String)	
	XTrace.showRun("ConverterBase.passon", "To next worker")
	
	If Not _nextConverter Is Nothing Then
		_nextConverter.feed(ioText)
		ioText = _nextConverter.run()
	Else
		' To the end
		Dim pworker As New PostWorker(ioText)
		pworker.setMaster(getMaster())
		ioText = pworker.run()		
	End If
End Sub
	
	
' Do nothing	
Protected Overridable Function transformTag( _
	Optional ByRef outIsRsltOk As Boolean = True _
) As String
	Return ""	
End Function


' Do nothing
Protected Overridable Function transform2( _
	ByRef ioFullText As String _
) As Integer
	If ioFullText Is Nothing OrElse ioFullText = "" Then Return 0
	Return ioFullText.Length
End Function


'*********************
'	Tag properties
'

' delegates
'
Public Function getParam( _
	pnm As String, _
	Optional defidx As Integer = -1 _
) As String
	Return CurrentTag().getParam(pnm, defidx)
End Function 	
	
Public Function getParam(Optional index As Integer = 1) As String
	Return CurrentTag().getParam(index)	
End Function


' Underline text
'
Protected Function decorate(ByRef title As String) As String
'	XTrace.show("LinkMarkConverter.transform", "param2", button)
	Dim rslt As String = title
	If title = "" Then Return "" 'XX may need process

	If CurrentTag().getLastParam() = "u" OrElse _
			CurrentTag().hasParam("u") Then
		rslt = "<u>" & rslt & "</u>"		
	End If	
	
	Return rslt
End Function

End Class 'ConverterBase

End Namespace