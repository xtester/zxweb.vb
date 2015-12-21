' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/3/17

Namespace zxweb.xtag

'*******************************************************************************
'

Public MustInherit Class AbstractTag
	
Private _beginTagHeader As String = ""
Public Property HeaderBeginMark() As String
	Get
		Return _beginTagHeader
	End Get
	Set
		_beginTagHeader = value
	End Set
End Property
	
Private _endTag As String = ""
Public Property Footer() As String
	Get
		Return _endTag
	End Get
	Set
		_endTag = value
	End Set
End Property

Private _key As String = ""
Public Property Keyword() As String
	Get
		Return _key
	End Get
	Set
		_key = value
	End Set
End Property

Private _name As String = ""
Public Property Name() As String
	Get
		Return _name
	End Get
	Set
		_name = value
	End Set
End Property


Private _headClosingMark As String = ""

Public Property HeaderClosingMark() As String
	Get
		Return _headClosingMark
	End Get
	Set
		_headClosingMark = value
	End Set
End Property

Private _compactHeadClosingMark As String = "" 

Public Property CompactHeaderClosingMark() As String
	Get
		Return _compactHeadClosingMark
	End Get
	Set
		_compactHeadClosingMark = value
	End Set
End Property


' Arg types
'

Private _argType As EnumArgType
Public Sub setArgType(argt As EnumArgType)
	_argType = argt	
End Sub
Public Function getArgType() As EnumArgType
	Return _argType	
End Function

Public Function isMustHaveArgs() As Boolean
	Return (_argType = EnumArgType.MustHave)	
End Function

Public Sub setMustHaveArgs()
	_argType = EnumArgType.MustHave
End Sub

Public Sub setNoArg()
	_argType = EnumArgType.None
End Sub

Public Sub setArgOptional()
	_argType = EnumArgType.IsOptional
End Sub

Public Function isArgOptional() As Boolean
	Return (	_argType = EnumArgType.IsOptional)
End Function


' Has no body
Private _isCompact As Boolean = False
Public Function isCompact() As Boolean
	Return _isCompact	
End Function
Public Sub setCompact(bv As Boolean)
	_isCompact = bv	
End Sub


Private _argstr As String
Public ReadOnly Property Argstr() As String
	Get
		Return _argstr
	End Get
End Property


Private _paramNames As Hashtable
Public Sub setParamNames(ByRef params As Hashtable) 
	_paramNames = params
End Sub

Private _paramNameList As ArrayList
Public Sub setParamNameList(ByRef list As ArrayList) 
	_paramNameList = list	
End Sub

Private _paramList As ArrayList
Public Sub setParams(ByRef list As ArrayList) 
	_paramList = list	
End Sub
Public Function getParamList() As ArrayList
	Return _paramList	
End Function


Private _whole As String
Public Function getWhole() As String
	Return _whole	
End Function


Private _body As String
Public ReadOnly Property Body() As String
	Get
		Return _body
	End Get
End Property

'
Public Sub setBody(ByRef ins As String)	
'	Dim equalChar As String = ""	
'	If (_argstr <> " " AndAlso (Not _argstr.StartsWith("^")) _
'			AndAlso (Not String.IsNullOrEmpty(_argstr))) _
'			AndAlso Not Util.exist(HeaderBeginMark, "=") Then
'		XTrace.show("AbstractTag.setBody", "Set equalChar")
'		equalChar = "="
'	End If	
'	If _isCompact Then 
''		'_whole = HeaderBeginMark & equalChar & _argstr & " " & _
''			CompactHeaderClosingMark
'	Else
''		_whole = HeaderBeginMark & equalChar & _argstr & HeaderClosingMark & _
''			_body & Footer					
'	End If
	
	_body = ins	
	XTrace.show("AbstractTag.setBody", _
		"_argstr", "[" & _argstr & "]" & _argstr.Length, _
		"_isCompact", _isCompact, _
		"HeaderBeginMark", HeaderBeginMark)
End Sub


Public Sub setWhole(ByRef ins As String)
	_whole = ins	
	XTrace.show("AbstractTag.setWhole", _
		"_whole", _whole)	
End Sub


'***************
'	Services
'
'***************

'!! _arglist may have trailing blank char.
'
Public Function parseParams(args As String) As ArrayList
	Dim paramNames As Hashtable = Nothing
	Dim paramNameList As ArrayList = Nothing	
	Dim paramList As ArrayList = Nothing
	
	_argstr = args	
	XTrace.showRun("AbstractTag.processParams")
	If _argstr Is Nothing Or _argstr = "" Then 
		XTrace.warn("AbstractTag.processParams", _
			"_args empty, return Nothing")
		Return Nothing '!!
	End If
	
	'XTrace.show("AbstractTag.getParamList", "arglist(" & _arglist.Length & ")", _arglist)	
	paramList = Util.parseValues(_argstr, ",") 	
	
	Try
		If paramList.Count > 1 Then '!! url= may have = in value
			paramNames = New Hashtable
			paramNameList = New ArrayList
			paramNameList.Add("") '!! arg1
			
			For i As Integer = 1 To paramList.Count-1 '!! start from 2nd key
				Dim param As String = paramList(i)
				
				If Util.exist(param, "=") Then
					Dim paramnm As String = _
						param.Substring(0, param.IndexOf("="))
					
					paramNames.Add(paramnm, i+1) ' start from 1
					paramList(i) = param.Replace(paramnm & "=", "") '!!
					paramNameList.Add(paramnm)
				Else
					paramNameList.Add("")
				End If
			Next			
		End If
		
		XTrace.show("AbstractTag.processParams", _
			"Param count", paramList.Count)
	Catch ex As Exception		
		XTrace.show("AbstractTag.processParams", ex)
		XTrace.show("AbstractTag.processParams>ex", "_argstr", _argstr)
	End Try
	
	setParams(paramList)
	setParamNameList(paramNameList)
	setParamNames(paramNames)
	
	Return paramList
End Function 'processParams


Public Function getParam(Optional index As Integer = 1) As String
	Dim rslt As String = ""
	If index < 1 Then Return "" '!!
	
	If index = 1 Then rslt = _argstr
	
	If _paramList IsNot Nothing AndAlso _paramList.Count >= index Then 
		rslt = _paramList(index - 1)
	End If
	
	If rslt <> "" Then
		Dim escapes As Char() = New Char(1) {" "c, ","c} '?? ,
		rslt = rslt.TrimEnd(escapes) 
		rslt = rslt.TrimStart(escapes)
	End If
	
	Return rslt
End Function


Public Function getLastParam() As String
	Dim rslt As String = ""
	
	If _paramList IsNot Nothing Then 
		rslt = getParam(_paramList.Count)	
	End If	
	
	Return rslt
End Function


Public Function hasParam(input As String) As Boolean
	Dim rslt As Boolean = False	
	If input = "" Then Return False
		
	If getParamList() Is Nothing Then Return False
	
	For i As Integer = 1 To _paramList.Count
		Dim param As String = getParam(i)
		'XTrace.show("AbstractTag.hasParam", i & "(" & param.Length & ")", param)
		If Util.isame(param, input) Then
			rslt = True
			Exit For
		End If
	Next
	
	Return rslt
End Function


' pnm: should not be ""
' defidx: default index (start from 1)
'
Public Function getParam( _
	pname As String, _
	Optional defidx As Integer = -1 _
) As String
	Dim rslt As String = ""
	Dim pidx As Integer = -1
	If pname = "" Then Return ""
	
	If _paramNames IsNot Nothing Then
		'XTrace.show("AbstractTag.getParam~str", "Get idx", pnm)
		pidx = _paramNames.Item(pname)
	End If	
	
	If pidx > 0 Then 'allow 1
		rslt = getParam(pidx)
	Else
		' no arg name
		If defidx > 1 Then
			' check if arg name	matches
			Dim isRight As Boolean = True
			
			If _paramNameList IsNot Nothing AndAlso _
					_paramNameList.Count >= defidx Then
				Dim argn As String = _paramNameList(defidx-1)
				If argn <> pname And argn <> "" Then isRight = False
			End If
			'XTrace.show("AbstractTag.getParam~str", "pidx", pidx)

			If isRight Then 	rslt = getParam（defidx）
		End If
	End If
	
	'XTrace.show("AbstractTag.getParam~str", "pidx", pidx)
	Return rslt
End Function 'getParam

End Class 'AbstractTag

End Namespace