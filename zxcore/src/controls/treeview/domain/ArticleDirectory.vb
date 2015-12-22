
NameSpace zxweb.treeview
	
'*******************************************************************************
' 	The content tree.
'	
'	Todos:
'
	
Public Class ArticleDirectory
	
Private _sname As String
Private _xfile As XmlFile

Private _contentTreeNode As XmlElement
Private _contentTreeRootNode As XmlElement

Private _dirTreeSrcRoot As XmlSrcNode = Nothing
Private _dirSytle As String = "plain"

Private _pch As IContentProvider 'XX


'*****************
'	Life cycle
'
'*****************

Public Sub New(ByRef pc As IContentProvider)
	Try	
		_pch = pc
		_sname = pc.getSName()
		_xfile = pc.getXmlFile(_sname)		
		_contentTreeNode = _xfile.getElement("/root/ContentTree")			
		_contentTreeRootNode = _xfile.getElement("/root/ContentTree/root") 'XX
	Catch ex As Exception
		XTrace.show("DirTree.initialize", ex)	
	End Try	
End Sub


'XX support ordering tag?
Public Function isUsePageTabs() As Boolean	
	Dim rslt As Boolean	
	Dim strReadTab As String = ""
	
	If Not _contentTreeRootNode Is Nothing Then ' root tag
		strReadTab = _contentTreeRootNode.getAttribute("usePageTab") 'all
	End If
	rslt = Util.isEnabled(strReadTab)	
	Return rslt
End Function


' section name as title
'XX support style2
Public Function isNameAsTitle() As Boolean
	Dim rslt As Boolean = True
	Dim strNameAsTitle As String = ""
	
	If Not _contentTreeRootNode Is Nothing Then 
		strNameAsTitle = _contentTreeRootNode.getAttribute("secNameAsTitle") 'all
	End If
	
	rslt = Util.isEnabled(strNameAsTitle, True) 'default	
	Return rslt
End Function


Protected Function isUseDefaultTitles() As Boolean
	If _contentTreeNode Is Nothing Then Return False 'error	
	Dim rslt As Boolean = Util.isEnabled(_contentTreeNode.getAttribute("useDefault")) ' read pages		
	Return rslt
End Function


'?? why xf
'
Public Function getroot(ByRef xf As XmlFile) As XmlSrcNode
	Dim rslt As XmlSrcNode	= Nothing
	
	If _dirTreeSrcRoot IsNot Nothing Then Return _dirTreeSrcRoot
	
	Dim orderingNode As XmlElement = xf.getElement("/root/ordering") 
	'old version
	If orderingNode IsNot Nothing Then	
		'XX rm
		Dim handler As IContentProvider = _
			AppHome.getFactoryResource().createInstance(_sname).getContentHandler()
			
		rslt = New SrcNodeCustom(orderingNode, orderingNode.ChildNodes, _
					handler) 'XX
		'Me)			
		'!! error: startSrcNode = New SrcNodeCustom(orderingNode, _
		' orderingNode.ChildNodes, _mypage.getContentHandler())
		'childs = orderingNode.ChildNodes.Count
		XTrace.show("DirTree.getroot", "Return ordering")
		_dirTreeSrcRoot = rslt
		_dirSytle = "ordering"
		
		Return rslt
	End If
	
	XTrace.show("DirTree.getroot", "Not ordering")
	Dim useDefault As Boolean = isUseDefaultTitles()		
	If (_contentTreeRootNode Is Nothing) And (Not useDefault) Then Return Nothing
	
	If useDefault Then
		' no tree	
		Dim nodes As XmlNodeList = _xfile.readNodes("//page") 'XX
		
		'childs = nodes.Count
		rslt = New SrcNodeEx(_xfile.getElement(_pch.getBodyXmlPath()), nodes) 'XX	 
	Else
		' normal
		rslt = New XmlSrcNode(_contentTreeRootNode)
		'childs = rootNode.ChildNodes.Count	'XX only with 'node'
	End If
	
	_dirTreeSrcRoot = rslt
	Return rslt 
End Function 

End Class 'ArticleDirectory

End NameSpace