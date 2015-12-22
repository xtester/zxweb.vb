
Imports System.Xml 'XX

Imports zxweb.ajax

NameSpace zxweb.treeview

'*******************************************************************************
'	Usage:	<ContentTree useDefault>
'
'	Todos:
'		- remove treeview2.aspx
'

Public Class TreeBrowser
	Inherits XControl

Private _idPrefix As String

' Nid format
' ...


Private _savedNodeScripts As String
Public Function getSavedNodeScripts() As String
	Return _savedNodeScripts	
End Function


' The content provider of the tree
Private _source As ITreeSource
Public Sub setSource(tr As ITreeSource)
	_source = tr
End Sub

'XX
Private _srcNode As XmlSrcNode
Public Sub setSrcNode(ByRef srcnd As XmlSrcNode) 
	_srcNode = srcnd	
End Sub


'XX
Protected Function getNodeIdBase() As String
	Return _idPrefix & "treenode"
End Function


'XX
Private Function getOutputId() As String 
	Return "ContentView" 'XX
End Function


'XX no ex; cached
'
Private Function getUrlOfContentProvider() As String
	Dim rslt As String = ""
	Dim view As String = RemoteView
	
	XTrace.showRun("TreeBrowser.getUrlOfContentProvider")
	
	If view = "" Then
		Dim xresrc As IWebResource = getXResource()
		
		Try
			XTrace.checkNull("TreeBrowser.getUrlOfContentProvider", _
				"xresrc", xresrc)
			If xresrc IsNot Nothing Then
				'		XTrace.show("TreeBrowser.getUrlOfContentProvider", _
				'			"View of xresrc", xresrc.getViewPath())
				view = xresrc.getViewPath()
			Else
				' Use xpage as an alternative, which might be a gateway.
				view = getPage().getViewPath() 
			End If						
		Catch ex As Exception			
			XTrace.show("TreeBrowser.getUrlOfContentProvider", ex)
			
			view = "/" 'XX for tester?
		End Try
	End If		
		
	'XX other providers
'	XTrace.show("TreeBrowser.getUrlOfContentProvider", _
'		"Content provider", provider, _

	Dim site As String = RemoteSite 

	rslt = site & view 'XX	
	XTrace.show("TreeBrowser.getUrlOfContentProvider", "rslt", rslt)	
	
	Return rslt
End Function 'getUrlOfContentProvider


Private _urlAjaxGateway As String ' for controls


Private _entryNode As TreeNode 
Protected Function getEntryNode() As TreeNode
	Return _entryNode
End Function

Private _bufInit As New StringBuilder(300)

Private _bufVars As New StringBuilder(300)
Private _bufCreate As New StringBuilder(300)
Protected Function getCreateNodesScripts() As String
	Return _bufCreate.ToString()
End Function

Private _bufAdd As New StringBuilder(300)
Private _bufAddHandlers As New StringBuilder(300)

Protected _nodes As New HashTable
Private _sections As New ArrayList

Private _usePageTab As Boolean
Private _nameAsTitle As Boolean

Protected _firstSectionId, _lastSectionId As String

Private _refreshTreeCmd As String

Private _nodeMenuTopOffsetIE As String 
Public Sub setNodeMenuTopOffsetIE(ins As String) 
	_nodeMenuTopOffsetIE = ins	
End Sub
Public Function getNodeMenuTopOffsetIE() As String
	Return _nodeMenuTopOffsetIE	
End Function


'*****************
'	Properties
'
'*****************

Private _rowWidth As String = "5000" 'default
Public Property rowWidth As String
	Get
		return _rowWidth
	End Get
	Set (ids As String)
		_rowWidth = ids
	End Set
End Property


'?? unused
Private _isHideEmptyNode As Boolean = False
Public Sub setHideEmptyNode(bv As Boolean)
	_isHideEmptyNode = bv	
End Sub


Private _remoteView As String
Public Property RemoteView() As String
	Get
		If _remoteView = "" Then
			_remoteView = _source.getRemoteView()
		End If
		
		Return _remoteView
	End Get
	Set
		_remoteView = value
	End Set
End Property

Private _remoteSite As String = ""
Public Property RemoteSite() As String
	Get
		Dim rslt As String = _remoteSite
		
		If rslt = "" Then
			_remoteSite = _source.getRemoteSite()
			rslt = _remoteSite
		End If
		
		' Add the prefix
		If rslt <> "" Then
			rslt = "http://"	 & rslt
		End If
		
		Return rslt
	End Get
	Set
		_remoteSite = value
	End Set
End Property


'*************
'	Events
'

'!! mypage is set at OnPreRender

Protected Overrides Sub initControl()
	MyBase.initControl()

	_idPrefix = "TreeBrowser" & getCtrlId() & "_"
		
	'XX 
	_urlAjaxGateway = AjaxGateway.getLink( _
		"TreeBrowser", "1", "zxweb.treeview", _
		ControlsConfig.readProperty("treeview/assembly"))
End Sub


'XX
Protected Overridable Function getNodeHandler( _
	nid As String, _
	Optional isTrail As Boolean = True, _
	Optional onlyName As Boolean = False _
) As String
	Dim rslt, trail As String
	Dim parts As String = "()"
	rslt = "" : trail = ""
	
	If onlyName Then
		parts = ""
	End If
	
	If isTrail Then trail = ";" '!!
	
	rslt &= nid & "_handler" & parts & trail 
	
	Return rslt
End Function


Private Sub writeLineHeading( _
	children As Integer, _
	nodeId As String, _
	blanks As Integer _
)
	Dim padding As Integer	
	
	If children > 0 Then
		' Node having children		
		padding = blanks * Util.getInt( _
			readProperty("treeview/treenode/containerNode/leftPadding"), 1)
		writeSpan("", "style='padding-left: " & padding & "px;'")
		
		Dim iconId As String = nodeId & "_icon"
		
		' Write the +/- icon
		writeSpan(Icons.Minus, "id='" & iconId & "'" & _
			" style='padding-top: 0px;'" & _
			" onMouseDown=""zxj.treeview.onNodePressed('" & nodeId & "');""" & _
			" onMouseOver=""zxj.treeview.onMouseOverNode('" & nodeId & "');""" & _
			" onMouseOut=""zxj.treeview.onMouseOutNode('" & iconId & "');""")
	Else
		' The leaf node				
		padding = Util.getInt( _
			readProperty("treeview/treenode/leafNode/leftPadding"), 1)
				
		Dim lstep As Integer = blanks * 5 + (padding + (blanks - 1) * 2)
		
		writeSpan("&nbsp;", "style='padding-left: " & lstep & "px;'")
	End If	
End Sub	'writeLineHeading
	

' To generate the tree.
' 
' nodeId:
'
Protected Sub processTree( _
	ByRef node As XmlSrcNode, _
	nodeId As String, _
	ByRef deepestId As String, _
	Optional blanks As Integer = 0 _
)
Try
	XTrace.showRun("TreeBrowser.processTree")
	
	Dim isRoot As Boolean = False ' For setting the client root
	Dim nodeTitle As String = node.getTitle() 
	Dim link As String = node.getName() ' The internal section name, not title.
	
	XTrace.show("TreeBrowser.processTree", "nodeTitle", nodeTitle)
	
	write("<div id='" & nodeId & "' style='width: " & rowWidth & "px;'>") '!! tricky length
	
	'XX conditions
	If (blanks = 0 And nodeTitle = "") OrElse node.isRoot() Then 
		nodeTitle = IIf(nodeTitle = "", "目录", nodeTitle) 'XX default, config		
		isRoot = True
		
		XTrace.show("TreeBrowser.processTree", "Root", nodeTitle)
	End If
	
	Dim ccount As Integer = node.getChildCount() 'XX name node
	XTrace.show("TreeBrowser.processTree", "Child count", ccount)
	
	' Write line heading
	'
	writeLineHeading(ccount, nodeId, blanks)
	
	' Set node title...
	'	
	If _usePageTab Or nodeTitle = "" Then
		Dim ptab As String = _source.getNodeTitle(link) 'XX from page tag
		' override
		If ptab <> "" Then nodeTitle = ptab
	End If
	
	If nodeTitle = "" Then 
		If _nameAsTitle Then 
			nodeTitle = link
		Else
			nodeTitle = "..." '!! to show the link
		End If
	End If
	
	XTrace.showRun("TreeBrowser.processTree", "Create tree node")
	Dim nobj As New TreeNode(nodeId, nodeTitle, link) 
	_nodes.Add(nodeId, nobj)
	_sections.Add(nodeId) 'XX the root node
	nobj.setIndex(_sections.Count - 1)
	
	' Set node id
	'
	Dim idText As String = getIdNodeTitle(node, nodeId)
	Dim attrib As String = "title='" & getTitleTip(node, nodeTitle) & _
		"' id='" & idText & "'"
	node.setViewNidOfTitlePart(idText) ' with _text	
	node.ViewNid = nodeId
	
	'!! after setTreeItemId
	writeTitlePrefix(node)
	
	' Write the node title...
	'	
	XTrace.show("TreeBrowser.processTree", "Write span with id", idText)
	If (RemoteSite = "" AndAlso Not _source.isExistNode(link)) OrElse _
		isRoot Then 'XX
		XTrace.warn("TreeBrowser.processTree", _
			"Part /" & link & "/ does not exist, or it's root")
		writeSpan(nodeTitle, attrib)
	Else
		' Write a link
		writeSpan("<a href='#' style='text-decoration: none;' " & _
			"onMouseDown='" & _
			getTitlePressedAction(link, nodeId) & "'>" & nodeTitle & "</a>", _
			attrib)
'		& " onMouseOver=""zxj.treeview.onMouseOverNode('" & idText & "');""" & _
'			" onMouseOut=""zxj.treeview.onMouseOutNode('" & idText & "');""")
	End If 
	
	' total pages
	If nodeTitle = "目录" Then writeSpan("", "id='TreeBrowser_total'")
	
	' Js
	'
	_bufInit.Append(nodeId & ".init();") 'XX
	_bufVars.Append("var " & nodeId & ";")
	_bufCreate.Append(nodeId & " = new zxj.treeview.Node(true, " & ccount & _
		", '" & nodeId & "');")
	
	'XX multi trees
	If isRoot Then
		_bufCreate.Append("zxj.treeview.root = " & nodeId & ";")
	End If
	
'	XTrace.show("TreeBrowser.processTree", "Node added", nobj.ToString())
'	XTrace.show("TreeBrowser.processTree", "nodes", _nodes.Count)
		
	Dim index As Integer = 1	
	Dim did As String = ""
	
	For i As Integer = 0 To ccount-1
	'For Each childnode As XmlElement In node.ChildNodes
		Dim childnode As XmlSrcNode = node.getChild(i)	
		Dim childName As String = nodeId & "_" & index
		
'		If _isHideEmptyNode And childnode.getChildCount() < 1 Then 
'			XTrace.warn("TreeBrowser.processTree>For", "No child, exit:" & childName)
'			Exit For
'		End If

		Dim blankinc As Integer = IIf(isFireFox(), 2, 4)
		processTree(childnode, childName, did, blanks + blankinc)
				
		_bufAdd.Append(nodeId & ".addChild(" & index & "," & childName & ");")
	
		index += 1				
	Next
	
	' The end tag
	write("</div>")
		
	If ccount < 1 Then 
		deepestId = nodeId	
	Else
		deepestId = did
	End If
Catch ex As Exception
	XTrace.checkNull("TreeBrowser.processTree", "node", node)
	XTrace.show("TreeBrowser.processTree", ex)
End Try	

	XTrace.showEnd("TreeBrowser.processTree")
End Sub 'processTree


Protected Overridable Function getTitleTip( _
	ByRef node As XmlSrcNode, _
	actualTitle As String _
) As String
	Dim rslt As String = "(" & actualTitle & "," & node.getName() & ")"
	Return rslt
End Function


Protected Overridable Sub writeTitlePrefix(ByRef node As XmlSrcNode)	
End Sub


Protected Overridable Function getTitlePressedAction( _
	link As String, _
	nodeId As String _
) As String
	Dim rslt As String = _
		"zxj.treeview.onNodeTitlePressed(""" & getNodeHandler(nodeId) & """);"
	Return rslt	
End Function


' node: target
'
'XX
Private Function genShowContentCmd( _
	ByRef node As TreeNode _
) As String
	Dim rslt, url, title, section, nid As String	
	rslt = ""
	XTrace.showRun("TreeBrowser.genShowContentCmd")
	If node Is Nothing Then Return ""	
		
	section = node.getSection()
	nid = node.getId()
	'XTrace.show("TreeBrowser.genShowContentCmd", "section", section, "nid", nid)	
		
	'!! may have chinese as part name
	'XX
	Dim params As String = "forced=yes&mode=ajax&ctrl=TreeBrowser&ctrlid=1" & _
		"&sname=" & getSName() & "&section=" & _
		AppHome.getHttpServer().UrlEncode(section) _
		& "&output=" & getOutputId() & "&useDefaultPart=false"
	
	If RemoteSite <> "" Then
		params &= "&reqType=cors"
	End If
	
	url = getUrlOfContentProvider() & "?" & params
	XTrace.show("TreeBrowser.genShowContentCmd", "url", url)	
	
	' meta first
	title = node.getTitle()
	If title = "" Then
		title = _source.getNodeTitle(section)			
	End If
	
	'XTrace.showRun("TreeBrowser.genShowContentCmd", "gen rslt")	
	'rslt = "traceRun('showContent');"
	'rslt &= "trace('showContent', 'sname', '" & getSName() & "');"
	'rslt &= "trace('showContent', 'title', '" & title & "');"
	rslt &= getPrintButtonsCmd(node)
	rslt &= "zxj.set('pageSubtitle', '" & title & "'); "	
	rslt &= "showControl('breakLine'); "
	rslt &= "zxj.set('" & getOutputId() & "', ""<div class='blanks' />"");" 'XX
	
	'XX
	If _source.isExistNode(section) OrElse RemoteSite <> "" Then
		rslt &= "zxj.ajaxSend('" & url &  "', '" & getOutputId() & _
			"', 'GET', '', 'zxj.treeview.onContentUpdated();');"
	End If	
	
	rslt &= "zxj.treeview.setFocus('" & nid & "');"	
	rslt &= "zxg_isPageContentChanged = true;" ' floatbar.js
	rslt &= "zxg_currentSectionId = '#" & nid & "';" ' treebrowser.js
	
	Return rslt
End Function 'genShowContentCmd	


'XX
Protected Overridable Function genFunctionHeader( _
	ByRef node As TreeNode _
) As String
	If node Is Nothing Then Return ""
	
	Dim funcname As String = getNodeHandler(node.getId(), False)
	Dim rslt As String = "function " & funcname & " {"
	
	Return rslt
End Function


Protected Function genNodeFunctions() As String
	XTrace.showRun("TreeBrowser.genNodeFunctions")	
	Dim rslt As New StringBuilder(500)
	Dim enumerator As IDictionaryEnumerator
	
	rslt.Append(_bufVars) '!! put here; all global vars
	
	Try
		enumerator = _nodes.GetEnumerator()		
		'myEnumerator.moveNext() 
		'!! don't bypass root
		'!! the first may not be the root.
		'!! hashtable get order may not be FIFO; longest name first
		While enumerator.moveNext()
			Dim node As TreeNode
			
			Try
				node = enumerator.Value
				
				'XX should no ex
				rslt.Append(genFunctionHeader(node))
				rslt.Append("restoreItem('TreeBrowser_Content');")
				rslt.Append(genShowContentCmd(node))
				rslt.Append("};")											
			Catch ex As Exception				
				'XTrace.checkNull()
			End Try
		End While		
	Catch ex As Exception		
		'
	End Try

	rslt.Append(genSetupFunc())			
	_savedNodeScripts = rslt.ToString()	
	
	XTrace.showEnd("TreeBrowser.genNodeFunctions")	
	Return _savedNodeScripts	
End Function 'genNodeFunctions

' Setup the tree
'XX
Private Function genSetupFunc() As String
	Dim funcname As String
	Dim rslt As New StringBuilder(300)
	
	funcname = _idPrefix & "setup()"
	
	rslt.Append("(function " & funcname & " {")
		'rslt.Append("traceRun('" & funcname & " setup');")
		rslt.Append(_bufCreate) 
		rslt.Append(_bufInit)
		rslt.Append(_bufAdd)
		
		rslt.Append("gTreeBrowser_Root.set(" & getNodeIdBase() & ");") 'XX	
		
		'XX 
		Dim pages As Integer = getTotalPages()
		If pages > 0 Then 
			rslt.Append("zxj.set('TreeBrowser_total', " & _
				"'（/" & pages & "）');")
		End If			
	rslt.Append("}());")
	
	Return rslt.ToString()
End Function


Protected Function getTotalPages() As Integer
	Dim rslt As Integer = _nodes.Count - 1	
	Return IIf(rslt >= 0, rslt, 0)
End Function


'XX just text 
Private Function getIdNodeTitle( _
	ByRef srcNode As XmlSrcNode, _	
	original As String _
) As String
	Dim rslt As String = original
	
	'AndAlso Not srcNode.isRoot()
	If original <> "" Then 
		rslt = original & "_text" 'XX
	End If
	
	Return rslt
End Function


Protected Function getNode(section As String) As TreeNode 
	Dim rslt, node As TreeNode
	rslt = Nothing
	
	If section = "" Then Return Nothing
	
	Dim myEnumerator As IDictionaryEnumerator = _nodes.GetEnumerator()	
	Dim found As Boolean = False	
	While myEnumerator.moveNext() And Not found 'XX
		node = myEnumerator.Value
		If node.getSection() = section Then
			rslt = node
			found = true
		End If	
	End While	
	
	Return rslt
End Function


' section: name
Protected Function showEntrySection( _
	section As String _
) As Boolean	
	_entryNode = getNode(section)	
	Return showEntrySection(_entryNode)
End Function 


Private Function showEntrySection( _
	ByRef node As TreeNode _
) As Boolean	
	If node Is Nothing Then Return False
	
	'XX postrun move up to base
	writejs("common_postRun = '" & getNodeHandler(node.getId()) & _
		"; zxj.treeview.onContentUpdated();'") 'XX
	Return True	
End Function 


Protected Overridable Function getPrintButtonsCmd( _
	ByRef current As TreeNode _
) As String
	Dim blanks, firstButton, lastButton, previousButton, nextButton, _
		topButton, bottomButton, currentButton, rslt As String
	previousButton = "" : nextButton = ""
	rslt = ""
	Dim firstNode, lastNode, previousNode, nextNode As TreeNode	
	previousNode = Nothing : nextNode = Nothing
	Dim size, loc As Integer 
	
	If _firstSectionId = "" Or _firstSectionId = _lastSectionId Then Return ""
		
	blanks = "&nbsp;&nbsp;&nbsp;&nbsp;" 'XX
	
	Try
		'XTrace.show("TreeBrowser.getPrintButtonsCmd", "section", section)		
		firstNode = _nodes.Item(_firstSectionId)
		lastNode = _nodes.Item(_lastSectionId)
		
		firstButton = "<span onMouseDown='showFirstPage();' onMouseUp=''>" & _
			"<img id='fpbtn'  src='" & Icons.FirstButton & "' height='20px' title='首页-" & _
			firstNode.getTitle() & "'></img></span>" 'XX id
		lastButton = "<img src='" & Icons.LastButton & "' height='20px' " & _
			"onMouseDown='showLastPage();' title='末页 " & lastNode.getIndex() & _
			"-" & lastNode.getTitle() & "'></img>"

		size = _sections.Count
						
		loc = current.getIndex()		
		If loc > 1 Then
			previousNode = _nodes.Item(_sections(loc - 1))
		End If
		If loc < (size - 1) Then
			nextNode = _nodes.Item(_sections(loc + 1))
		End If				

		If loc > 1 And Not previousNode Is Nothing Then
			previousButton = "<img src='" & Icons.PreviousButton & "' height='20px' " & _
				"title='上页 " & previousNode.getIndex() & "-" & _
				previousNode.getTitle() & "' onMouseDown='" & _
				getNodeHandler(previousNode.getId()) & "zxButtonPostScript();'></img>"
		End If
					
		If loc < (size - 1) And Not nextNode Is Nothing Then
			nextButton = "<img src='" & Icons.NextButton & "' height='20px' title='下页 " & _
				nextNode.getIndex() & "-" & nextNode.getTitle() & "' onMouseDown='" _
				& getNodeHandler(nextNode.getId()) & "zxButtonPostScript();'></img>"
		End If
						
		currentButton = "<a class='CurrentPageButton' href='#' title='刷新-" & _
			current.getTitle() & " (" & current.getSection() & ")' " & _
			"onMouseDown='" & getNodeHandler(current.getId()) _
			& "'>第&nbsp;" & loc & "/" & getTotalPages() & "&nbsp;页</a>" 'XX
									
		topButton = "<img height='20px' src='" & Icons.TopButton & "' title='页首' " & _
			"border='0' onMouseDown='zx_scrollToTop();' />"
		bottomButton = "<img height='20px' src='" & Icons.BottomButton & "' " & _
			"title='页脚' border='0' onMouseDown='zx_scrollToBottom();' />"
											
		rslt &= "firstPageHandler = """ & getNodeHandler(firstNode.getId()) & """;"
		rslt &= "lastPageHandler = """ & getNodeHandler(lastNode.getId()) & """;"
		
		'XTrace.show("TreeBrowser.getPrintButtonsCmd", "_refreshTreeCmd", _refreshTreeCmd)
		Dim refreshButtons As String = "<div id='zxWindowButtons'>" & _
			"<a href='/?" & getSName() & "'><img id='zxButtonRefreshWin' " & _
			"onMouseDown='' class='zxFloatbarRefreshButton' " & _
			"src='/img/refresh_1.gif' title='刷新窗口' border='0' /></a> " 'XX 
		
		refreshButtons &= "<img onMouseDown='zxButtonPostScript();" & _
			getNodeHandler(current.getId()) & "'  Class='zxFloatbarRefreshButton' " & _
			"src='/img/refresh_2.gif' title='刷新当前页' border='0' /> "
		
		refreshButtons &= "<img id='zxButtonRefreshDir' "
		
		refreshButtons &= "onMouseDown='zxButtonPostScript();zxRefreshContentTree();' " & _
			"Class='zxFloatbarRefreshButton' src='/img/refresh_3.gif' " & _
			"title='刷新目录' border='0' /></div>"
		
		Dim style2 As String = refreshButtons 
		
		style2 &= firstButton & IIf(previousButton = "", "", blanks & _
			previousButton) & blanks & topButton & "<br />"
		
		style2 &= currentButton & "<br />"
		
		style2 &= lastButton & blanks & IIf(nextButton = "", "", nextButton & _
			blanks) & bottomButton & "<br />"
		
		style2 &= "<a href='#' onMouseDown='zx_scrollToDirTree();' " & _
			"title='目录定位'>" & _
			"<img src='/img/location.png' height='19px' border='0' " & _
			"style='margin: 5px;' /></a>"
			
		rslt &= "zxj.set(""HiddenFloatbarSectionButtons"", """ & style2 & """);"				
	Catch ex As Exception
		XTrace.show("TreeBrowser.getPrintButtonsCmd", ex)
	End Try
	
	Return rslt
End Function 'getPrintButtonsCmd


' Refresh the tree
'
' !! Need ie's compatible view to function
'
Protected Overridable Sub writeTreeButtons()
	Dim url, target As String
	
	target = "TreeBrowser_Tree" 'XX
	url = _urlAjaxGateway & "&sname=" & getSName() & "&section=" & _
		getHttpParam("section") & "&output=" & target

	_refreshTreeCmd = "removeItem('TreeBrowser_Content');"	
	_refreshTreeCmd &= "zxj.ajaxSend('" & url &  "', '" & target & "');"
	'XTrace.show("TreeBrowser.writeTreeButtons", "_refreshTreeCmd", _refreshTreeCmd)
	
	Dim margin As String = Util.getInt( _
		readProperty("treeview/treeButtons/marginLeft"), 1)
	
	'XX use write function helper
	write("<script>function zxRefreshContentTree() {")
	write(_refreshTreeCmd)
	write("}</script>")
	
	write("<img id='TreeBrowser_RefreshArrow' width='15px' " & _
		"style='margin-left: " & margin & "px;' " & _
		"src='/img/icon-refresh.png' title='刷新' " & _
		"onMouseDown='zxRefreshContentTree();' />") 'XX
End Sub 'writeTreeButtons


'XX mv out 
Protected Overridable Function getXmlFile() As XmlFile
	Dim xf As XmlFile = getXResource().getXmlFile()
	Return xf
End Function


Protected Overridable Function getEntrySection() As String
	Dim section As String = getHttpParam("section")	
	Return section
End Function


'!! Not for ajax
Protected Overridable Sub process(ByRef startSrcNode As XmlSrcNode)
	writeTreeButtons()
	
	write("<div id='TreeBrowser_Tree'>")
	processTree(startSrcNode, getNodeIdBase(), _lastSectionId) 'XX lastid
	write("</div>")
	
	XTrace.showRun("TreeBrowser.process", "Generate node scripts")
	writejs(genNodeFunctions())	
End Sub


Protected Overrides Sub OnLoad(e As EventArgs) 
Try	
	MyBase.OnLoad(e)
	
	initSource()
	initTreeProperties()
Catch ex As Exception
	XTrace.show("TreeBrowser.OnLoad", ex)	
End Try	
End Sub 'OnLoad


' In OnLoad
Protected Overridable Sub initSource()
	_source = getXResource()	
	
	Dim dt As ArticleDirectory = _source.getDirTree() 	
	_srcNode = dt.getroot(getXmlFile()) 'XX xml	
End Sub


Protected Overridable Sub initTreeProperties()
	Dim dt As ArticleDirectory = _source.getDirTree() 	
	
	_usePageTab = dt.isUsePageTabs()
	_nameAsTitle = dt.isNameAsTitle() ' default yes			
End Sub


'XX	
'
Protected Overrides Sub output()		
Try	
	If _srcNode Is Nothing Then 
		XTrace.show("TreeBrowser.output", "Warning", "No source node, return")
		Return
	End If		
	
	_firstSectionId = getNodeIdBase() & "_" & 1 'XX

	XTrace.showRun("TreeBrowser.output", "Process")
	process(_srcNode) 
	
	' Handling entry section	
	XTrace.showRun("TreeBrowser.output", "Handling entry section")
	Dim entrySection As String = getEntrySection()
	' HTTP param first
	If Not showEntrySection(entrySection) Then
		entrySection = _srcNode.getEntry() 'XX
		If Not showEntrySection(entrySection) Then 
			' show first node automatically
			Try 
				Dim firstNode As TreeNode = _nodes.Item(_firstSectionId)
				showEntrySection(firstNode)
			Catch ex As Exception
			' not found node
			End Try			
		End If
	End If	
	
	' Node menu hidden
	writeDiv("id=""zxTreeView_nodeMenu"" " & _
		"onMouseOver=""zxj.restore('zxTreeView_nodeMenu');"" " & _
		"onMouseOut=""zxj.remove('zxTreeView_nodeMenu');""", _
		"<span onMouseDown='zxj.treeview.onOpenAll();'>Open all</span><br />" & _
		"---------------<br />" & _
		"<span onMouseDown='zxj.treeview.onCloseOthers();'>Close others</span>")
	
	XTrace.show("TreeBrowser.output", "End")
Catch ex As Exception
	XTrace.checkNull("TreeBrowser.output", "_source", _source)
	XTrace.show("TreeBrowser.output", ex)
End Try	
End Sub	'output

End Class 'TreeBrowser

End Namespace