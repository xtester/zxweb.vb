' Created on 2006-1-27
'

Namespace ZX 'XX!! caution error, used by all template .aspx

'*******************************************************************************
'	Base controller class for all pages.
'
'   To do:
' 		- remove _xfile, PageContent (contentHandler).
'

Public Class ZXPage 
	Inherits PageBase
	Implements IMultiplePages
	
'*******	******
'	Fields
'
'*************
	
Private _xresrc As IWebResource
Public Overridable Function getXResource() As IWebResource
	Return _xresrc
End Function


'XX move out
Protected _xfile As XmlFile 
Public Function getXmlFile() As XmlFile
	return _xfile
End Function


'XX move out
Private _contentHandler As IContentProvider


Protected _sname As String 

Private _totalPage As Integer
Private _curPageNo As Integer


'XX outer location ??

' relative-inner, absolute-outer resources
' usually xml file path
Private _contentPath As String
Public Function getContentPath() As String
	Return _contentPath
End Function


' Multiple part?
'
'XX merge into xresource
Private Function isMultiple(Optional isSet As Boolean = False) As Boolean	
	XTrace.showRun("ZXPage.isMultiple")	
	Dim rslt As Boolean = False
	
	If _contentHandler IsNot Nothing Then
		rslt = _contentHandler.isMultiple(isSet)		
	End If
	
	Return rslt
End Function 


'XX section id
Private _names() As String ' all names


Protected _showEndNavigator As Boolean = False
Public Sub setShowEndNavigatorTrue() ' by ContentLoader
	_showEndNavigator = True
End Sub
Public Sub setAllowEndNavigator(bv As Boolean)
	_showEndNavigator = bv
End Sub


Public Function getTotalPage() As Integer _
		Implements IMultiplePages.getTotalPage		
	Return _totalPage
End Function


'!!
Public Function getSrcUrl() As String _
		Implements IMultiplePages.getSrcUrl	
	
	' what if _sname null	
	Return "/?sname=" & _sname
End Function


' Called by: getTitle()
'		
'XX merge with pagecontent
'?? why need page container
Private Function process( _
	ByRef tn As XmlElement, _
	ByRef inp As String _
) As Boolean
	If tn Is Nothing Then Return False
	
	Dim temp As String = tn.InnerXml
	
	'XX
	If ControlLoader.process(Me, tn, temp) Then 
		inp = temp
		Return True
	End If
	
	Return False
End Function


'XX mvin; merge
Public Overrides Function getTitle() As String
	Dim tn As XmlElement = Nothing
	Dim rslt As String = _xresrc.readTitleAndXmlNode(tn) 'XX 	
	
	process(tn, rslt)		
	
	Return rslt
End Function


'XX rename, for xresource
Public Overrides Function getVirtualPath() As String
	Dim	rslt As String = _xresrc.getProperty("logicalPath")
	Return rslt
End Function


'XX
Public Function getSName() As String
	If _sname = "" Then _sname = getParam("sname") '?? try again 
	return _sname
End Function


'??
Public Property SName As String
	Get
		return getSName()
	End Get
	Set (ps As String)
		_sname = ps
	End Set	
End Property


'***************
' 	Services 
'
'***************

' unused
Public Function computeDisplayRange( _
	ByRef starti As Integer, _
	ByRef endi As Integer _	
) As Boolean _
		Implements IMultiplePages.computeDisplayRange
	'totalcomments As Integer _
	Return False
End Function


'XX rm
'
' loc: in/out, relative path (content file) 
'
Private Function readContentPath( _
	ByRef inloc As String _
) As Boolean
	Dim rslt As Boolean = False
	
Try 	
	' XX wrap the query behind SDB class
	Dim sdb As DB = AppHome.getInstance().SYSPDB
	Dim loc2 As String = sdb.readTableField("Locations", "SNAME", _sname, "LOC")
	If loc2 = "" Then Return False ' query error
	
	' process the location
	Dim i As Integer = loc2.LastIndexOf(".")
	Dim lpos As Integer = loc2.Length() - 1
	Dim j As Integer = loc2.LastIndexOf(".xml")
	
	If j < 0 Then 
	' the postfix is .aspx or no .xml, replace .* with .xml
		Dim ns As String = loc2.Substring(i, lpos-i+1)
		' Util.trace("ZXPage.readContentPath", "ns: " & ns)
		loc2 = loc2.Replace(ns, ".xml")
	End If
	
	' sname is correct and exist in the db
	inloc = loc2
	' Util.trace("ZXPage.readContentPath", "loc: " & loc)	
	rslt = True
Catch ex As Exception
	rslt = False
	inloc = "" '!!
	XTrace.show("ZXPage.readContentPath", ex)
End Try

	Return rslt	
End Function 'readContentPath


'XX
'!! template name is not case-sensitive, must use '/', not '\'
'
Protected Function getViewTemplate() As String
	Dim rslt As String = _xresrc.getViewPath() 
	XTrace.show("ZXPage.getViewTemplate", "got template", rslt)
	
	'XX
	If _xresrc.isCommentResource() And getParam("id") <> "" Then
		rslt = "/_templates/tmpl_commenteditor.aspx" 'XX
		
	ElseIf _xresrc.isQnasResource() And getParam("id") <> "" Then
		rslt = "/_templates/tmpl_qnaseditor.aspx" 'XX
		
	End If
	
	Return rslt
End Function 


' Initialize common attributes, after load
'
' Postcondition:
'	- set current page num
' 	- set title prefix
'   - set total pages, _totalPage
' 
Private Sub initializePage()
	Dim st, pt, apt As String

	If _xfile Is Nothing Then Return ' error	
	
	Try
		If isMultiple(True) Then 
			_names = _xresrc.getPartNames()
			_totalPage = _names.Length
		Else
		'XX old way, read counts specified
			st = _xresrc.getProperty("pages")
			' Util.trace("ZXPage.initializePage", "st: " & st)
			_totalPage = Util.convertToPages(st)
		End If
	Catch ex As Exception
		_totalPage = 1		
		' Util.trace("ZXPage.initializePage", "exp: " & ex.tostring())
	End Try
	XTrace.show("ZXPage.initializePage", "isMultiple", isMultiple())
	
	' Set current page num
	If getParams() Is Nothing Then Return	
	
	Try	
		pt = getParam("pg")
		If pt <> "" Then
			_curPageNo = Util.convertToPages(pt)
		Else 
		' XX Navigator through Aspect attribute
			apt = getParam("aspect") '? 'aspect' used where ? resource list
			' Util.trace("ZXPage.initializePage", "apt: " & apt)
			If apt <> "" Then
				Dim nodeList as XmlNodeList 
				
				' locate the enclosing tag
				nodeList = _xfile.getDoc().SelectNodes( _
						"/descendant::*[attribute::aspect='" & apt & "']") '!!
				
				Dim pgnode as XmlNode = nodeList.Item(0)
				Dim ns As String = pgnode.LocalName
					' Util.trace("ZXPage.initializePage", "node name: " & ns )
				_curPageNo = CInt(ns.Remove(0, 4)) ' Remove prefix 'page'
					' Util.trace("ZXPage.initializePage", "pno: " & _curPageNo )
			Else
				If Not isMultiple() Then _curPageNo = 1 'XX error ?
			End If
		End If
	Catch ex As Exception
		XTrace.show("ZXPage.initializePage", ex)
		_curPageNo = 1
	End Try			
End Sub 'initializePage


' Get part name, and set _curPageNo.
' HTTP param 'pg' is also used in some doc views.
'
'XX support param 'page' 
'XX mv?
'
Public Function getCurrentPartName() As String
	Dim rslt As String = "index" 'XX default?
	Dim pgn As String = ""

Try	
	If isMultiple() Then ' Do not use pageX
		rslt = getParam("section") ' HTTP
		
		If rslt = "" Then 
			XTrace.warn("ZXPage.getCurrentPartName", "input section is empty")
			
			' try pg
			pgn = getParam("pg")	 'XX verify int range			
						
			If pgn = "" Then 
				' Both section and pg are empty.
				
				If Util.isDisabled(getParam("useDefaultPart")) Then
					Return AppHome.InvalidResourcePart '!!			
				End If
					
				pgn = 1 ' default
			End If
		
			rslt = _names(pgn - 1) '!! starts from 0
			If rslt = "" Then 
				rslt = "index" 'XX warning; config
			End If
		Else
		' XX useful ?
			_curPageNo = getCurrentPageIndex() '??
		End If	
	Else
		' Single		
		Dim pg As String = getParam("pg") 'XX
		
		If pg <> "" Then 
			rslt = "page" & Util.convertToPages(pg) ' may not exist
			' Util.trace("ZXPage.getPart", "pgn: " & pgn)
		else 
			If _curPageNo > 1 Then 'XX ?
				rslt = "page" & _curPageNo
			Else
				' Navi to Default Page1 		
				If _xfile.existNode("/root/main_body/page1") Then 
					rslt = "page1" 'XX
				End If
			End If
		End If
	End If
Catch ex As Exception
	XTrace.show("ZXPage.getCurrentPartName", ex)	
End Try

	XTrace.show("ZXPage.getCurrentPartName", "rslt", rslt)
	Return rslt
End Function 'getCurrentPart


Public Function getCurrentPageIndex() As Integer _
		Implements IMultiplePages.readCurPageNo		
	Dim pn As Integer = 1 ' default index section
	Dim i As Integer
	Dim sec As String 

Try	
	If isMultiple() Then
		sec = getParam("section")
		If sec <> "" Then
			For i = 0 To getTotalPage() - 1
				If _names(i) = sec Then 
					pn = i + 1 
				End If
			Next 			
			'Util.trace("ZXPage.getCurrentPageIndex", "pn: " & pn)
		Else
		' section null
			sec = getParam("pg")
			pn = Util.convertToPages(sec)		
		End If
	Else 
	' use pg
		pn = _curPageNo		
	End If
Catch ex As Exception	
End Try
	
	Return pn
End Function 'getCurrentPageIndex


'XX rename
Protected Function readPageTabs() As String() _
		Implements IMultiplePages.readPageTabs		
	Return _xresrc.getPartTitles()	
End Function


'XX current page/section title
Public Function getSubtitle() As String	
	Return _xresrc.getPartTitle(getCurrentPageIndex()) 
End Function 


'XX move out; simplify
Public Function notShowTabInHeader() As Boolean
	Dim val, attribs, attribs2 As String
	Dim rslt As Boolean = False ' default, that means display it.
	
Try		
	'XX
	attribs = "tabNoShownInHeader"
	attribs2 = "TabNoShownInHeader"
	
	val = readPartAttribute(attribs2)
	If Util.isEnabled(val, False) Then rslt = True
	'?? val empty, exception, val.ToLower().Equals("yes")	
	If Not rslt Then
		val = readPartAttribute(attribs)
		If Util.isEnabled(val, False) Then rslt = True	
	End If
	
	If Not rslt Then
		If _xresrc.getProperty("isNotShowPartInPageTitle") Then rslt = True
	End If
Catch ex As Exception
	XTrace.show("ZXPage.notShowTabInHeader", ex)
End Try

	Return rslt
End Function 'notShowTabInHeader


'********************* 
'	Error Handling 
'

'XX
Protected Sub handleError( _
	ByRef what As String _
)	
	'XTrace.show("ZXPage.handleError", what)		
	'XTrace.checkNull("ZXPage.handleError", "_xsession", _xsession)
	_xsession.handleError(what, Context) 	
End Sub


'XX
Private Sub handleXResourceError( _
	ByRef what As String _
)	
	Dim link As String = ""
	XTrace.show("ZXPage.handleXResourceError", _sname, what)
	
	If _sname <> "" Then link = "/?" & _sname
	Dim msgsname As String = "抱歉，您要访问的目标 " & _
		Util.genAnchor(link, _sname, link)
	
	handleError(msgsname & " " & what) 	
End Sub


'**************
'	Helpers
'

Protected Shared Function parseTarget(ins As String) As String
	XTrace.show("ZXPage.parseTarget"	, "Original target", ins)
	If ins = "" Then Return ""	
	Dim rslt As String = ins
	
	If Util.exist(ins, "/") Then
		Try
			Dim keys As ArrayList = Util.parseValues(ins, "/")
			
			rslt = "/?sname=" & keys(0) 
			rslt &= "&section=" & keys(1)
		Catch ex As Exception
			'
		End Try
	Else
		rslt = "/?sname=" & ins
	End If
	
	Return rslt
End Function


'*************
'	Events
'
'*************

Private Shared _resourceProcessor As IResourceProcessor
Public Shared Sub setResourceProcessor(ByRef inrp As IResourceProcessor)
	_resourceProcessor = inrp
End Sub


'XX _xfile load exception
'
Public Overrides Sub Page_Load(sender AS Object, eargs AS EventArgs) 
Try
	XTrace.showRun("ZXPage.Page_Load")
	MyBase.Page_Load(sender, eargs)
	
	setUserAgent(Context.Request.UserAgent) 'XX mv
	
	_sname = getParam("sname")
	XTrace.show("ZXPage.Page_Load", "Read sname", _sname)	
	If _sname = "" And getParam("ctrlid") <> "" Then Return 'XX ajax control
	
	'!! for sname search in a form post.
	If Util.exist(_sname, "/") Then
		redirect(parseTarget(_sname))  
	End If	
	
	' Create the resource
	_xresrc = AppHome.getFactoryResource().createInstance(_sname)
	
	' check path
	Dim isPathOk As Boolean = False '!!
	Dim path As String = _xresrc.getProperty("path") 
	If path <> "" And _sname <> "" Then
	' ok, founded in conf file
		_contentPath = path 
		isPathOk = True 'XX ?
	Else
	'!! path can be empty, defined in db as the old way.
		isPathOk = readContentPath(_contentPath) 'XX
	End If
	
	XTrace.show("ZXPage.Page_Load>XResource created", _
		"sname", _sname, "path", _contentPath)
				
	checkForward() '!!
	
	Dim usePath As Boolean = _xresrc.isUsePath()	 'XX
	If (Not isPathOk) Or _contentPath = "" Or _sname = "" Then	
		If usePath Then
		' forward to error page		
			handleXResourceError("不存在。")
		End If
	End If
	
	If _xresrc.isLocked() Then
		handleXResourceError("已被锁定。")
	End If
	
	If _xresrc.isSecured() AndAlso Not _xsession.isLogined() Then _
		handleXResourceError("需要<a href='/?action=login'>登录</a>才能访问。")
		
	If _xresrc.isLocal() And usePath And _contentPath <> "" Then
		' if template not match, auto redirect.
		Dim vtemplate As New ViewManager(Me) 'XX
		
		vtemplate.checkView(getViewTemplate(), getViewPath(), _
			getParam("forced"))
		
		_xfile = XmlFile.loadReady(Server.MapPath(_contentPath)) 'XX 	
		
		'XX merge or rm
		_contentHandler = _xresrc.getContentHandler()
		_contentHandler.setXPage(Me) 'XX
		
		' Process the content/body file
		initializePage()
		
		' CORS
		Response.AppendHeader("Access-Control-Allow-Origin", "*") 'XX config	
		If Util.isame(getParam("reqType"), "cors") Then
			_xresrc.setIsCorsRequest(True)
		End If
		
		XTrace.show("ZXPage.Page_Load", "Page data/file loaded.")
		
		_xresrc.setRequestedPartName(getCurrentPartName()) 'XX needed?
		
		If _resourceProcessor IsNot Nothing Then
			_resourceProcessor.run(_sname)
		End If
	End If
	
	XTrace.showEnd("ZXPage.Page_Load")
	Return '!!
Catch ex As Exception
	' XX redirect to errpage
	XTrace.show("ZXPage.Page_Load", ex)
End Try

	handleXResourceError("正在维护中 ...") 
End Sub	'Page_Load


'*****************
'	Forwarding
'

Private _isAllowForward As Boolean = True
Public Sub setAllowForward(bv As Boolean)
	_isAllowForward = bv
End Sub


'!! forward to another xresource sname/section
'
Private Sub checkForward()
	Dim target, sn, sec As String
	target = "" : sec = ""
	Dim vals As ArrayList
	XTrace.showRun("ZXPage.checkForward")
	
Try	
	XTrace.show("ZXPage.checkForward", "_rawUrl", _rawUrl)
	
	If Not _isAllowForward Then 
		XTrace.show("ZXPage.checkForward", "Disabled")
		Return
	End If
	
	' For register.aspx
	Dim isRaw As Boolean = Util.isEnabled(_xresrc.getProperty("forward", "raw"))
	'XTrace.show("ZXPage.checkForward", "israw", isRaw)
	target = _xresrc.getProperty("forward")
	
	'!! For testing, removed original sname
	Dim params As String = PageBase.getChangedQuery(getHttpContext())

	If isRaw Then 
		redirect(target & params)
	End If
	
	'XX
	vals = Util.parseValues(target, "/")
	If Not vals Is Nothing AndAlso vals.Count > 0 Then
		sn = vals(0)
		
		If vals.Count > 1 Then sec = vals(1)
		
		If sn <> "" Then
			If sec = "" Then
				redirect("/?sname=" & sn & params)
			Else
				redirect("/?sname=" & sn & "&section=" & sec & params)
			End If
		End If
	End If	
Catch ex As Exception
	XTrace.show("ZXPage.checkForward", ex)
End Try
End Sub 'checkForward 


Public Sub redirect(path As String)
	XTrace.show("ZXPage.redirect", "Forward to", path)	
	Response.Redirect(path)
End Sub


'XX section
Public Sub redirect( _
	tmplname As String, _
	sname As String _
)
	If tmplname = "" Or sname = "" Then Return 'XX Error
	
	'XX use ResourceMetaManager
	Dim path As String = "/_templates/tmpl_" & tmplname & ".aspx?sname=" & _
		sname
	redirect(path) 
End Sub


' viewn: *.aspx - view template
Public Sub forwardToView( _
	template As String _
)	
	If template = "" Then Return '!! err
	
	Dim target As String = Util.getQueryString(_rawUrl, template & "?")	
	XTrace.show("ZXPage.forwardToView", "_rawUrl", _rawUrl)
	
	redirect(target) '!! keep Context for shared use ??
End Sub 


'***************
'	Counting
'

' Return: used?
Public Function countMe() As Integer
	Dim rslt As Integer = -1
	If _sname = "" Then
		Return rslt 'error
	End If
	
	rslt = _xresrc.countVisit()
	Return rslt
End Function 'countMe


'***************
'	Part
'

' default: current part
'XX move in, merge with PagePart
'
Private Function getPartRoot(Optional partnm As String = "") As String
	Dim rslt As String
	
	If partnm = "" Then partnm = getCurrentPartName() 
	rslt = "//page[@name='" & partnm & "']" 'XX
	
	Return rslt	
End Function


'XX move out
Public Function readPartAttribute( _
	attrib As String, _
	Optional partnm As String = "" _
) As String
	Dim rslt As String	
	If attrib = "" Then Return ""
	
	Dim searchstr As String = getPartRoot(partnm) & "/@" & attrib
	'XTrace.show("ZXPage.readPartAttribute", "search", searchstr)
	rslt = _xfile.readNodeAttribute(searchstr)
	
	Return rslt
End Function


'XX
Public Overridable Function renderNavigator(name As String) As Boolean _
		Implements IMultiplePages.renderNavigator
	'Util.trace("ZxPage.renderNavigator", "name", name)
	If name = "end" Then 
		If _xfile Is Nothing Then Return False 'XX

		Dim sAllowEndNavi As String = readPartAttribute("showBottomNavigator")	
		setAllowEndNavigator(Util.isEnabled(sAllowEndNavi, True))
		
		Return _showEndNavigator
	Else
		Return True
	End If	
End Function 'renderNavigator


Public Overridable Function allowBottomControl(name As String) As Boolean 
	If name = "end" Or name = "bottom" Then 
		If _xfile Is Nothing Then Return False 'XX

		Dim sAllowEndNavi As String = readPartAttribute("showBottomNavigator")	
		Dim flag As Boolean = Util.isEnabled(sAllowEndNavi, True)
		
		Return flag
	Else
		Return True
	End If	
End Function 'allowBottomControl


'XX mvin
Public Function getDirectoryLink() As String
	Return _xresrc.getProperty("dirPage")
End Function


Protected Overrides Sub Render(Writer As HtmlTextWriter)	
Try	
	XTrace.showRun("ZXPage.Render")	
	Dim mode As String = getParam("mode")
	Dim outputId As String = getParam("output") 'XX name
	
	setWriter(Writer) '!! allow embedded controls to show.
	XTrace.show("ZXPage.Render", "SName", getSName(), "mode", mode)
	
	If Util.isame(mode, "ajax") Then  
		' only section content
		XTrace.show("ZXPage.Render", "mode is ajax")		
		
		countMe() '??
		Writer.write(XPage.genOutputTag(outputId)) 
		
		'!! body countPages='yes' must be set
		Dim rslt As String = _contentHandler.readContent()
		Writer.write(rslt)
		
		'XX extract func
		If rslt <> "" AndAlso rslt IsNot Nothing Then
			XTrace.show("ZXPage.Render", "Chars written", rslt.Length)	
			
			If rslt.Length >= 50 Then
				'XX
				XTrace.show("ZXPage.Render", "Sample", rslt.Substring(0, 50)	 & _
					"<br />")
			End If
		Else	
			XTrace.warn("ZXPage.Render", "rslt null/empty")
		End If
		
		XTrace.show("ZXPage.Render", "Return of ajax")	
		Return
		
	Else If Util.isame(mode, "test") Then
		XTrace.show("ZXPage.Render", "mode is test")
		
		Writer.write(XPage.genOutputTag(outputId)) 
		
		Dim tcid As String = getParam("tcid")
		Writer.write(ControlBase.genPostActions( _
			"zxtester.pagetest.verify(zxtester_all." & tcid & ");")) 'XX
	End If	
	
	MyBase.Render(Writer)	
	XTrace.showEnd("ZXPage.Render")		
Catch ex As Exception
	XTrace.show("ZXPage.Render", ex)
End Try	
End Sub 'Render

End Class 'ZXPage

End Namespace


Namespace zxweb

'*******************************************************************************
'XX formats not unified, some with blanks, some not.
'

Public Class XPage
	Inherits ZXPage
	
' ins: the div id of the output
'
Public Shared Function genOutputTag(ins As String) As String
	Dim prefix As String = "zxweb_outputdiv"	 
	' To avoid conflicts with other XML notes
	Dim rslt As String = "<!--" & prefix & " " & ins & " " & prefix & "-->"
	
	XTrace.show("XPage.genOutputTag", "rslt", rslt, _
		"output id", ins)
	Return rslt
End Function
	
End Class 'XPage

End NameSpace