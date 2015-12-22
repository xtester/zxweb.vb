
NameSpace zxweb.core

Public Interface IMetaProvider
	
	'!! sname may change 	
	Function find( _
		ByRef sname As String, _
		key As String _
	) As String
	
	
	' Used by:
	'	CommentFile
	'
	Function existSName( _
		sname As String, _
		Optional isCheckAlias As Boolean = False _
	) As Boolean   	
	
	
	Function findSecuredResources() As String
	
	' Return relative path
	Function getViewPath(sname As String) As String 
	
	' No change from config
	Function getViewTemplate(sname As String) As String 
	
	
	'XX
	Function getNode(sname As String, pty As String) As XmlElement
	
	
	Function getAllResources() As ArrayList
	
	
	Function existProperty(ByRef sname As String, ppty As String, _
		value As String) As Boolean

End Interface 'IMetaProvider

End Namespace