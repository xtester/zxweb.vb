
' Created on 2006-Aug-8

Namespace zxweb

'*******************************************************************************
'	The base class for all zxweb controls.
'
'	TODO:
'		- move _confile out (Controls Config)
'		20120207 - handle initControl(), twice run?
'
'	History:
'		20111103 - added field _xession, 
'					     method redirect
'

Public Class ControlBase
	Inherits WebControl
	
'*************
'	Fields
'
'*************

' Values: Cached
Private _state As String
Public Sub setState(ins As String)
	_state = ins	
End Sub
Public Function getState() As String
	Return _state
End Function


' value: 'custom' for XML controls	
Private _runtimeMode As String = "normal" ' in .aspx
Public Property runtimeMode As String
	Get
		return _runtimeMode
	End Get
	Set (ps As String)
		_runtimeMode = ps
	End Set
End Property	


' False: no write to Writer, only in buffer.
Private _isEnableWrite As Boolean = True
Public Sub setEnableWrite(bv As Boolean)
	_isEnableWrite = bv	
End Sub
Public Function isEnableWrite() As Boolean
	Return _isEnableWrite	
End Function

	
Private _initialized As Boolean = False
Protected Sub setInitialized()
	_initialized = True
End Sub


' Set by:
'	initControl()
'XX needed?
Private _xsession As XSession
Protected Function getXSession() As XSession
	Return _xsession
End Function
Public Sub setXSession(ByRef xs As XSession) 
	_xsession = xs	
End Sub


' For config 
Protected _ctrlClass As String
Protected Sub setControlClass(ccs As String)
	_ctrlClass = ccs
End Sub


' contained xml
Private _data As String
Public Sub setData(ins As String) 
	_data = ins	
End Sub
Public Function getXmlData() As String
	Return _data	
End Function
	

Private _allowedToRun As Boolean = True 'XX move
Protected Function allowedToRun() As Boolean
	Return _allowedToRun
End Function
Protected Sub setAllowedToRun(bv As Boolean)
	_allowedToRun = bv
End Sub


' custom control in content file
Private _isEmbeddedInXml As Boolean = False
Protected Function isEmbeddedInXml() As Boolean
	Return _isEmbeddedInXml
End Function
Public Sub setEmbeddedInXml(bv As Boolean)
	_isEmbeddedInXml = bv
End Sub


'XX merge with allowedToRun
'XX conflicts with WebControl.isEnabled
'
Protected Shadows Function isEnabled() As Boolean
	Dim rslt As Boolean
	
	Dim flag As String = readPropertyAttribute(Name, "enabled")	
	rslt = Util.isEnabled(flag) 
	
	Return rslt
End Function


'XX name change
Protected _container As ControlBase ' parent control
Public Sub setContainer(ByRef parent As ControlBase)
	_container = parent
End Sub


Private _context As ControlContext
Public Sub setContext(ByRef inc As ControlContext)
	_context = inc
End Sub
Public Function getContext() As ControlContext
	Return _context 
End Function

Public Function getContainingXmlFile() As XmlFile
	Dim targcon As ControlContext = _context
	Dim rslt As XmlFile = Nothing
	
	If targcon Is Nothing AndAlso _container IsNot Nothing Then
		targcon = _container.getContext()
	End If
	
	If targcon IsNot Nothing Then 
		rslt = targcon.getXmlFile()
	End If
	
	Return rslt
End Function


Private _isClientTrace As Boolean = False ' not open
Public Function isClientTrace() As Boolean
	Return _isClientTrace
End Function


Private _writeBuffer As StringBuilder

Private _useWriteBuffer As Boolean = False 'default
Protected Function isUseWriteBuffer() As Boolean
	Return _useWriteBuffer
End Function
'XX
Public Sub setUseWriteBuffer(bv As Boolean)
	_useWriteBuffer = bv
End Sub


'XX
Public Function getClass() As String	
	Return Me.GetType().FullName
End Function


'***********
'	Page
'

'XX private
Protected _mypage As ZXPage = Nothing

Public Function getPage() As ZXPage
	Return _mypage
End Function

'XX rename
Public Sub setPageContainer(ByRef p As ZXPage)
	_mypage = p
	
	Page = p 'XX for PatternList, 2009-1-23
End Sub


'****************
'	XResource
'

Private _xresrc As IWebResource ' 2012-8-15

Public Sub setXResource(ByRef xres As IWebResource)
	_xresrc = xres	
End Sub

' Get from page first.
'
Public Overridable Function getXResource() As IWebResource
	Dim rslt As IWebResource = Nothing
	XTrace.showRun("ControlBase.getXResource")
	
	If _xresrc IsNot Nothing Then 
		Return _xresrc
	End If
	
	If _mypage IsNot Nothing Then 
		rslt = _mypage.getXResource()
	End If
	
	If rslt Is Nothing Then
		Dim snm As String = getSName() '!!
		XTrace.show("ControlBase.getXResource", "To create xresource", snm)
		
		rslt = AppHome.getFactoryResource().createInstance(snm) 
		'!! for ajax 2012-8-15
	End If
	
	_xresrc = rslt
	XTrace.checkNull("ControlBase.getXResource", "_xresrc", _xresrc)
	Return rslt
End Function


' for ajax
Private _embeddedSName As String
Public Function getEmbeddedSName() As String
	Return _embeddedSName
End Function
Public Sub setEmbeddedSName(val As String)
	_embeddedSName = val
End Sub


'XX priv
Protected _sname As String ' moved from DocHeader, and other derived controls

'XX merge or not?
Public Overridable Sub setSName(val As String)
	_sname = val
End Sub

' for control tags
Public Property SName As String
	Get
		If _sname = "" And _container IsNot Nothing Then
			_sname = _container.SName
		End If
		
		If _sname = "" Then _sname = getSName()
		
		return _sname 'XX
	End Get
	Set (t As String)
		_sname = t
	End Set
End Property


Public Function isSName(sn As String) As Boolean
	Return Util.isame(getSName(), sn)	
End Function


'XX merge with property
'?? when called
'
Public Overridable Function getSName( _
	Optional readFromParams As Boolean = False _
) As String
	Dim rslt As String = ""
	
Try
	XTrace.showRun("ControlBase.getSName")
	
	If readFromParams And Not (Context.Request Is Nothing) Then
		'Dim params As NameValueCollection ' Request params					
		Return Context.Request.Params("sname")				
	End If
	
	If _sname <> "" Then 
		Return _sname 'XX?? 2013-4-12
	Else
		XTrace.show("ControlBase.getSName", "_sname null")
	End If
	
	'XX get from xpage
	If _mypage Is Nothing Then 
		initPage()
	End If
Catch ex As Exception
	XTrace.show("ControlBase.getSName", ex)
End Try

	'XX
	If _mypage IsNot Nothing Then 
		setSName(_mypage.SName)	
		rslt = _sname
	End If
	
	XTrace.show("ControlBase.getSName", "_sname", _sname, "rslt", rslt)
	Return rslt
End Function 'getSName


'XX private
Protected _params As NameValueCollection

'XX byref?
'!! keep public
'
' Used by:
'		Ajax	DecoratorFactory
'
Public Sub setParams(p As NameValueCollection) 
	_params = p
End Sub

Public Function getHttpParam(name As String) As String
	If _params Is Nothing Or name = "" Then 
		XTrace.checkNull("ControlBase.getHttpParam", "_params", _params)
		Return ""
	End If
	
	Return _params(name)
End Function


' Config
'

'XX priv and use get
Protected Shared _configFile As XmlFile 'for all controls 

Protected Function getConfigFile() As XmlFile	
	If _configFile Is Nothing OrElse _configFile.isUpdated() Then
		XTrace.show("ControlBase.getControlsConfigFile", "Warning", _
			"Null or updated")
		_configFile = ControlsConfig.getFile()
	Else
		XTrace.show("ControlBase.getControlsConfigFile", "Warning", _
			"Fresh, use cached")
	End If
	
'	XTrace.checkNull("ControlBase.setConfigureFile", _
'		"Controls Config", _confile)
	Return _configFile
End Function


Protected _allowSpan As Boolean = False

Protected _br As String = "<br />"
Protected _brln As String = "<br /><br />"

'XX priv, merge with getClass()
Protected _name As String 'ctrl name, for addressing in controls.xml


'XX use UniqueId?
'XX change format
'
Private _ctrlId As Integer '!! set by initControl, use serial
Public Function getCtrlId() As String 'XX global?
	Return "zxctrl" & _ctrlId
End Function

' count
Private Shared _serial As Integer = 0 'trick for ctrls in one page
'?? Sync
Private Function getNewSerial() As Integer
	_serial += 1
	Return _serial
End Function


'XX rm
Private _ctrlConfigRoot As String
Protected Overridable Function getCtrlConfigRoot() As String
	Return _ctrlConfigRoot
End Function


'XX use .net CssClass property?
'XX priv
Protected _cssClass As String 
Public Sub setCssClass(cc As String)
	_cssClass = cc	
End Sub
Protected _cssClassProperty As String = "#default" 'XX


Protected Function isDefaultCssId() As Boolean
	If _cssClassProperty = "#default" Or _cssClassProperty = "" Then 
		Return True
	Else
		Return False
	End If
End Function


'*************
'	Paging
'

'XX
Private _pagingAdapter As IMultiplePages
Public Sub setPagingAdapter(ByRef pa As IMultiplePages)
	_pagingAdapter = pa
End Sub
Public Function getPagingAdapter() As IMultiplePages
	Return _pagingAdapter
End Function


' For Ajax output
Protected _outputId As String 'XX priv
Public Sub setOutputId(s As String)
	_outputId = s
End Sub
Public Function getClientOutputId() As String 'XX name
	Return _outputId	
End Function


'XX 
Private _jscripts As String
Protected Function getJScripts() As String
	Return _jscripts	
End Function

Protected Sub appendScripts(ByRef scr As String)
	_jscripts &= "<script>" & scr & "</script>"
End Sub


'***** 
'	Properties 
'
'******

'!! ctrl name, for addressing in controls.xml
' not ctrl class
'
Public Property Name As String 
	Get
		return _name
	End Get
	Set (ps As String)
		_name = ps
	End Set
End Property


'XX
Public Property CSSControl As String
	Get
		return _cssClassProperty
	End Get
	Set (ps As String)
		_cssClassProperty = ps
	End Set
End Property


'XX
Public Property enableWriterBuffer As String
	Get
		Return IIf(isUseWriteBuffer(), "Yes", "No")
	End Get
	Set(bv As String)
		If Util.isEnabled(bv, False) Then
			setUseWriteBuffer(True)
		Else
			setUseWriteBuffer(False)
		End If
	End Set
End Property


' For XControl
Private _useDefaultRender As Boolean = False
Public Function isUseDefaultRender() As Boolean
	Return _useDefaultRender
End Function

Public Property useDefaultRender As String
	Get
		Return IIf(isUseDefaultRender(), "Yes", "No")
	End Get
	Set(strv As String)
		_useDefaultRender = Util.isEnabled(strv)
	End Set
End Property


Public Property clientTrace As String
	Get
		Return IIf(isClientTrace(), "Yes", "No")
	End Get
	Set(bv As String)
		If Util.isEnabled(bv, False) Then
			_isClientTrace = True
		Else
			_isClientTrace = False
		End If
	End Set
End Property


' hide #control# when output ""
Private _isHideEmptyTip As Boolean = False
Public Function isHideEmptyTip() As Boolean
	Return _isHideEmptyTip
End Function
Public Property hideEmptyTip As String
	Get
		Return IIf(_isHideEmptyTip, "Yes", "No")
	End Get
	Set(bv As String)
		_isHideEmptyTip = Util.isEnabled(bv) 
	End Set
End Property


'*****************
'	Life cycle 
'
'*****************

' Load all needed data before rendering.
'

'XX needed?
Private _isLoaded As Boolean = False
' For ajax adapters
Protected Sub setLoaded()
	_isLoaded = True	
End Sub

'!! ensure to run, even in test suite
'?? event useful?
Protected Overrides Sub OnLoad(e AS EventArgs) 
	XTrace.showRun("ControlBase.OnLoad")		
	
	If _isLoaded Then 
		XTrace.show("ControlBase.OnLoad", "Already loaded, return")		
		Return
	End If
	
	Try
		initControl() 'XX needed?	
		setInitialized() 'XX
		
		setParams(Context.Request.Params)	
		
		'!!
		CreateChildControls()
	Catch ex As Exception
		XTrace.show("ControlBase.OnLoad", ex)
	End Try	
	
	_isLoaded = True
	XTrace.showEnd("ControlBase.OnLoad")		
End Sub 'OnLoad


' A simple OnLoad() wrapper for ajax controls. For normal controls, use OnLoad() 
' instead.
'
'!! Since .net Control has already an event-handler called Load(), changed the name.
Public Overridable Sub loadControl(Optional eargs As EventArgs = Nothing)
	XTrace.showRun("ControlBase.loadControl" & getIdTag())	
	OnLoad(eargs)	
	XTrace.showEnd("ControlBase.loadControl" & getIdTag())	
End Sub


' Called by OnLoad and RenderContents, ajax delegate
Protected Overridable Sub initControl()
Try
	XTrace.showRun("ControlBase.initControl")
	If _initialized Then 
		XTrace.show("ControlBase.initControl", "Init'ed, pass")
		Return
	End If
	
	AppHome.init(Context) 'XX 
	
	_ctrlId = getNewSerial()
	setXSession(XSession.getObject(Context))	 'XX
	
	_configFile = ControlsConfig.getFile() 'XX 
	
	setUserAgent()
Catch ex As Exception
	XTrace.show("ControlBase.initControl", ex)	
End Try	
End Sub 'initControl


' For specific control, may be somewhere else.
'
Protected Function loadConfigFile(target As String) As XmlFile
	Dim src As String = ""
	Dim rslt As XmlFile = Nothing
	
	src = readPropertyAttribute(target, "src")
	XTrace.show("ControlBase.loadConfigFile", "src", src)
	If src <> "" Then
		rslt = XmlFile.loadReady(mapPath(src)) 'XX 
	End If
	
	If rslt Is Nothing Then 
		rslt = getConfigFile() '!! use shared default
	End If

	Return rslt
End Function


'**************
'	Servies 
'
'**************

Public Sub handleError( _
	Optional msg As String = "" _
)	
	getXSession().handleError(msg, Context)
End Sub


'*****************
'	Properties
'
'*****************

' path: without leading '/'
'XX mv
Protected Overridable Function readProperty( _ 
	path As String, _
	Optional defaultv As String = "" _
) As String
	Dim rslt As String 
	
	rslt = getConfigFile().readXml("/controls/" & path) 'XX
	If rslt = "" Then rslt = defaultv
	
	Return rslt
End Function


'XX mv
Public Function readProperty2( _ 
	name As String, _
	Optional defv As String = "" _
) As String
	Return readProperty(getPropertyPath(name), defv)
End Function


'XX rm
Private Function getPropertyPath(name As String) As String
	Return getCtrlConfigRoot() & "/" & name
End Function


Protected Function readPropertyAttribute( _ 
	path As String, _
	attr As String, _
	Optional defaultv As String = "" _
) As String
	
	Return readPropertyAttribute(getConfigFile(), path, attr, defaultv)
End Function


'XX xfile
Protected Function readPropertyAttribute( _ 
	ByRef xfile As XmlFile, _
	path As String, _
	attr As String, _
	Optional defaultv As String = "" _
) As String
	Dim rslt As String 
	
	If xfile Is Nothing Then Return ""
	If path = "" Or attr = "" Then Return ""
	
	rslt = xfile.readNodeAttribute("/controls/" & path & "/@" & attr)
	If rslt = "" Then rslt = defaultv
	
	Return rslt
End Function


'**************
'	Helpers
'
'**************

'!! when redirect, cause thread abortion
Protected Sub redirect(path As String)
	Context.Response.Redirect(path)
End Sub


'************ 
'	Write 
'

' basic one
Public Sub write(ByRef outs As String)
	If outs = "" Then Return
	Dim wr As HtmlTextWriter = Nothing
	
Try	
	If Not isUseWriteBuffer() Then
		wr = getWriter(True)
		wr.Write(outs)
	Else
		If _writeBuffer Is Nothing Then
			_writeBuffer = New StringBuilder("", 2000) 'XX conf
		End If
		
		_writeBuffer.Append(outs)
	End If
Catch ex As Exception
	XTrace.show("ControlBase.write", "outs", outs)	
	XTrace.show("ControlBase.write", "useWriteBuffer", isUseWriteBuffer())	
	XTrace.checkNull("ControlBase.write", "writer", wr)	
	XTrace.show("ControlBase.write", ex)	
End Try
End Sub 'write


Protected Sub write(ByRef sb As StringBuilder)
	If sb Is Nothing Then Return
	write(sb.ToString())
End Sub


Protected Sub writeBreak(ByRef outs As String)
	write(outs & _br)
End Sub

Protected Sub writeBreak()
	write(_br)
End Sub

Protected Sub writeBreakLn(ByRef outs As String)
	write(outs & _brln)
End Sub

Protected Sub writeBreakLine(ByRef outs As String)
	writeBreakLn(outs)
End Sub

Protected Sub writeBreakLn()
	write(_brln)
End Sub

Protected Sub writeBlanks(n As Integer)
	For i As Integer = 1 To n
		write("&nbsp;")
	Next
End Sub


'XX use Util.genDiv
Public Sub writeDiv( _
	ByRef attrib As String, _
	Optional ByRef text As String = "" _
)
	write("<div " & attrib & ">")
	write(text)
	write("</div>")
End Sub


'XX param optional?
Protected Sub writeSpan( _
	ByRef text As String, _
	Optional ByRef attrib As String = "" _
)
	write("<span " & attrib & ">")
	write(text)
	write("</span>")
End Sub

'XX writeSpanWithId


'XX use <span title="">
'
Public Sub writeSpanWithTitle( _
	ByRef text As String, _
	ByRef tip As String, _
	Optional lfDecorator As String = "", _
	Optional rtDecorator As String = "" _
)
	Dim outs As String = ""

Try	
	XTrace.showRun("ControlBase.writeSpanWithTitle")
	If text = "" Then Return

	outs = lfDecorator & text & rtDecorator

	If tip = "" Then
		XTrace.show("ControlBase.writeSpanWithTitle", "Warning", "tip null")
	Else
		outs = "<span title='" & tip & "'>" & outs & "</span>"	
	End If

	'XX tiny, too many
	'Dim outl As New Label		
	'outl.ToolTip = tip
	'outl.Text = outs
	'outl.RenderControl(_writer) 'XX can't control
Catch ex As Exception
	'XTrace.show("ControlBase.printLabelWithTip", "writer is null", (_writer Is Nothing))
	XTrace.show("ControlBase.writeSpanWithTitle", ex)
End Try

	XTrace.show("ControlBase.writeSpanWithTitle", "outs", outs)
	write(outs)	
End Sub


'XX Is Page always okay? Maybe not (e.g. custom), so use DoorKeeper instead.
'
Protected Function mapPath(ByRef p As String) As String
	Dim rslt As String
	
	rslt = DoorKeeper.mapPath(p) 'XX
 	Return rslt 
End Function


'********
'	Rendering 
'
'************

Public Overrides Sub RenderBeginTag( _
   writer As HtmlTextWriter _
)
	If _allowSpan Then
		XTrace.showRun("ControlBase.RenderBeginTag")
		MyBase.RenderBeginTag(writer)
	End If
	'!! Remove the <span> tag
End Sub


Public Overrides Sub RenderEndTag( _
   writer As HtmlTextWriter _
)
	If _allowSpan Then
		XTrace.showRun("ControlBase.RenderEndTag")
		MyBase.RenderEndTag(writer)
	End If
	'!! Remove the </span> tag
End Sub


'XX enhance
Public Function isPageSetOk() As Boolean
	Dim rslt As Boolean = False
	
	If _mypage IsNot Nothing AndAlso _params IsNot Nothing AndAlso _
			_writer IsNot Nothing Then
		rslt = True
	End If
	
	Return rslt
End Function


' Called by: 
'		OnPreRender
'XX 
Protected Sub initPage()
Try	
	XTrace.showRun("ControlBase.initPage")	
	
	If isPageSetOk() Then 
		XTrace.warn("ControlBase.initPage", "Already init'ed, return")	
		Return 
	End If
	
	'!! invalid for normal apsx		
	If Page IsNot Nothing AndAlso _
			Page.GetType().isSubclassOf(Type.GetType("ZX.ZXPage")) Then 'XX
		_mypage = CType(Page, ZXPage)	
		XTrace.show("ControlBase.initPage", "Converted to ZXPage")			
		
		setWriter(_mypage.getWriter())	
	Else	
		XTrace.warn("ControlBase.initPage", "Page is not ZXPage, failed to convert")		
	End If	
Catch ex As Exception
	XTrace.show("ControlBase.initPage", ex)
End Try

	XTrace.showEnd("ControlBase.initPage")	
End Sub 'initPage


'XX
Protected Overrides Sub OnPreRender(eargs As EventArgs)
Try	
	XTrace.showRun("ControlBase.OnPreRender" & getIdTag())		
	
	If Not _isLoaded Then
		loadControl() 'XX!! keep 
	End If
		
	XTrace.showRun("ControlBase.OnPreRender" & getIdTag(), "Calling base")	
	MyBase.OnPreRender(eargs)
	
	XTrace.showRun("ControlBase.OnPreRender" & getIdTag(), "Setting page object")	
	initPage()	
		
	'!!
	XTrace.showRun("ControlBase.OnPreRender" & getIdTag(), "Calling subcontrols")
	For Each subControl As ControlBase In Controls
		subControl.OnPreRender(eargs)
	Next	
	
	XTrace.show("ControlBase.OnPreRender" & getIdTag(), "SName", _sname)
	XTrace.checkNull("ControlBase.OnPreRender" & getIdTag(), "_params", _params)
Catch ex As Exception
	XTrace.show("ControlBase.OnPreRender" & getIdTag(), ex)
'	XTrace.checkNull("ControlBase.OnPreRender>ex", "Page", Page)
'	XTrace.checkNull("ControlBase.OnPreRender>ex", "_mypage", _mypage)
End Try

	XTrace.showEnd("ControlBase.OnPreRender" & getIdTag())		
End Sub 'OnPreRender


' for ajax deco
'
' Used by:
'	AjaxDecoratorFactory, AjaxGateway
'
Public Overridable Sub PreRenderDelegate(eas As EventArgs)
	OnPreRender(eas)	
End Sub


'**********************
'	The HTML Writer
'

Private _writer As HtmlTextWriter
Public Sub setWriter(Writer As HtmlTextWriter)
	_writer = Writer
End Sub

Public Function getWriter( _
	Optional isUpdate As Boolean = False _
) As HtmlTextWriter
	If (Not _mypage Is Nothing) AndAlso isUpdate Then 
		XTrace.warn("ControlBase.getWriter", "Set to page's write")
		setWriter(_mypage.getWriter())
	End If
	
	Return _writer
End Function


' Gather all strings before render
' Note: if not flush, strings will heap up in ajax mode
'
' isFlush: true, by DocHeader
'
Public Overridable Function genOutput( _
	Optional isFlush As Boolean = False, _
	Optional isRun As Boolean = True _
) As String	
	Dim rslt As String
	
	If isRun Then 
		XTrace.showRun("ControlBase.genOutput" & " {" & _name & "}", "Run output")
		output() ' actual rendering
	End If

	If _writeBuffer Is Nothing Then 
		XTrace.show("ControlBase.genOutput", "Warning", "_writeBuffer null")
		Return ""
	End If
	
	rslt = _writeBuffer.ToString()
	
	If isFlush Then _writeBuffer = New StringBuilder 'XX
	Return rslt	
End Function 'genOutput


' works only with writer buffer 
Protected Overridable Sub output()
	XTrace.showRun("ControlBase.output")	
	XTrace.showEnd("ControlBase.output")	
End Sub


'!! May not work for custom mode (recursively process by the ControlLoader), 
' 	causing <div> or other tags broken. Be careful to use.
'
Public Sub flushToWriter()
	Dim txt As String = ""	
	If _writer Is Nothing Then Return	
	
	txt = genOutput() 'XX
	If txt <> "" Then
		_writer.Write(txt) 'XX 
		_writeBuffer = Nothing
	End If
End Sub


Public Sub clearWriteBuffer()	
	_writeBuffer = Nothing
End Sub


' Render xcontrols
'
'!!XX mv into XControl
'
' Called by:
'	ControlLoader, ...
'
Public Sub invokeRenderDelegate(Optional isLoad As Boolean = True) 'XX change to false
	XTrace.showRun("ControlBase.invokeRenderDelegate" & getIdTag())	
	
	setUseWriteBuffer(True) '!! Keep for ajax comments and qnas tabs

	'XX rm
	If isLoad Then 
		XTrace.showRun("ControlBase.invokeRenderDelegate" & getIdTag(), "loadControl")
		loadControl() 
	End If
	
	XTrace.showRun("ControlBase.invokeRenderDelegate" & getIdTag(), "OnPreRender")
	OnPreRender(Nothing) 'XX EventArgs

	If isUseDefaultRender() Then '!!
		XTrace.showRun("ControlBase.invokeRenderDelegate", "Use default render")				
		
		'??
		AddAttributesToRender(getWriter())
				
		XTrace.showRun("ControlBase.invokeRenderDelegate" & getIdTag(), _
			"RenderBeginTag")		
		RenderBeginTag(getWriter())
	End If
	
	XTrace.showRun("ControlBase.invokeRenderDelegate" & getIdTag(), _
		"RenderContents")
	RenderContents(getWriter())
	
	If isUseDefaultRender() Then
		XTrace.showRun("ControlBase.invokeRenderDelegate" & getIdTag(), _
			"RenderEndTag")
		RenderEndTag(getWriter())
	End If
	
	XTrace.showEnd("ControlBase.invokeRenderDelegate" & getIdTag())	
End Sub 'invokeRenderDelegate


'******************
'	JavaScripts
'
'******************

Protected Sub includeJavaScripts(path As String)
	If path = "" Then Return
	
	write("<script type='text/javascript' src='" & path & "'></script> ")
End Sub


Public Sub writejs( _
	ByRef content As String, _
	Optional src As String = "" _
)
	If content = "" And src = "" Then Return
	
	Dim attrib As String = IIf(src = "", "", " src=""" & src & """")
	
	write("<script" & attrib & ">")		
	write(content)	
	write("</script>")	
End Sub


Protected Sub writeJsLoader(src As String)
	writejs("", src)	
End Sub


Protected Sub writejsfunc(ByRef sig As String, ByRef body As String)
	If sig = "" Then Return
	write("<script>function " & sig & " {")		
	write(body)	
	write("}</script>")	
End Sub


' For tracing
Public Function getIdTag() As String
	Dim clsn As String = _name
	
	If clsn = "" Then clsn = getClass()
	Return " {" & getCtrlId() & ":" & clsn & "}"	
End Function


'!!error: Public Overrides Sub RenderControl(Writer As HtmlTextWriter)
'
Protected Overrides Sub RenderContents(Writer As HtmlTextWriter)
Try
	XTrace.showRun("ControlBase.RenderContents" & " {" & _name & "}")	
	setWriter(Writer) '?? useful	
	
	initControl() '!! why here	
	XTrace.show("ControlBase.RenderContents", "Init ok")
	
	XTrace.show("ControlBase.RenderContents", "Buffer enabled", isUseWriteBuffer())
	XTrace.checkNull("ControlBase.RenderContents", "hwriter", Writer)
	XTrace.show("ControlBase.RenderContents", "SName property", SName)
'	If Writer Is Nothing Then
'		Writer = getWriter()	
'	End If
	
	If isUseWriteBuffer() Then '??
		XTrace.show("ControlBase.RenderContents" & " {" & _name & "}", "Using write buffer")		
		Dim outs As String = genOutput() '!! run
		
		If outs <> "" Then 
			XTrace.show("ControlBase.RenderContents", "Output len", outs.Length)
			Dim tracelen As Integer = IIf(outs.Length > 50, 50, outs.Length)
			XTrace.show("ControlBase.RenderContents", "Output", outs.Substring(0, tracelen)) 'XX
		Else
			XTrace.warn("ControlBase.RenderContents", "Output empty")
		End If
		
		If Writer IsNot Nothing AndAlso isEnableWrite() Then
			Writer.Write(outs) '!! don't use _writer here.
		End If
	Else
		XTrace.show("ControlBase.RenderContents" & " {" & _name & "}", "Warning", "write nothing")	
	End If

	'!! must keep here, to render children.
	XTrace.showRun("ControlBase.RenderContents", "Call base render")
	MyBase.RenderContents(Writer)
	
	XTrace.showEnd("ControlBase.RenderContents" & " {" & _name & "}")
Catch ex As Exception
	XTrace.show("ControlBase.RenderContents", ex)
End Try
End Sub 'RenderContents


'XX var array
Public Function isTemplate(tmpName As String) As Boolean
	Dim rslt As Boolean = False
	
	XTrace.show("ControlBase.isTemplate", "Check name", tmpName)
	If _mypage IsNot Nothing AndAlso _
			Util.exist(_mypage.getViewPath(), tmpName) Then
		rslt = True
	End If
	
	Return rslt
End Function


'**********
' postactions, js
'
'**********

' For ajax
'XX move?
Public Shared Function genPostActions(ByRef actions As String) As String
	Return "<!--postactions " & actions & " postactions-->"
End Function


Private _postactions As String = ""
Public Overridable Function getPostActions() As String
	Return _postActions	
End Function

Public Sub writePostActions(ByRef actions As String)
	_postactions = actions	
	write(genPostActions(actions))	
End Sub

'XX appendPostactions


'**************
'	Browser 
'

Private _userAgent As String

Private Sub setUserAgent()
	Dim mypg As ZXPage = getPage()
	
	If mypg IsNot Nothing Then
		_userAgent = mypg.getUserAgent()
		'XTrace.show("ControlBase.setUserAgent", "Set _userAgent", _userAgent)
	Else
		'XTrace.warn("ControlBase.setUserAgent", "mypg null")
	End If			
End Sub

Public Function getUserAgent() As String
	If _userAgent = "" Then
		setUserAgent()
	End If
	
	Return _userAgent	
End Function


'XX merge with page
Public Function isFireFox() As Boolean
	Dim rslt As Boolean = False	
	
	If Util.exist(getUserAgent(), "Firefox") Then 
		rslt = True
	End If
	
	Return rslt	
End Function

End Class 'ControlBase

End NameSpace