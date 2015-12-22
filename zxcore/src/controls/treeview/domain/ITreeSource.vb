
Namespace zxweb.treeview

'*******************************************************************************
'	The interface of the tree content provider
'
'	Todos:
'

Public Interface ITreeSource
	
	Function getDirTree() As ArticleDirectory	
	
	Function getNodeTitle(section As String) As String
	
	'XX rename
	Function isExistNode(name As String) As Boolean
	
	Function getRootNode() As XmlSrcNode
	
	Function getRemoteSite() As String
	
	Function getRemoteView() As String
	
End Interface

End Namespace