
Imports zxweb.treeview

NameSpace zxweb.data
	
'*******************************************************************************
' 	Metadata from two parts:
'		ArticleHandler
'		ResourceMeta
'

Public Interface IContentProvider
	
	Function getSName() As String
	
	Function readMeta2( _
		sname As String, _
		ptyname As String, _
		Optional isXml As Boolean = False _
	) As String
	
	
	'loadready
	'XX rm
	Function getXmlFile(Optional sname As String = "") As XmlFile
	
	
	' Titles
	'
	
	Function getSectionTitle( _
		sname As String, _
		section As String _
	) As String
	
	Function getTitleByPartName(name As String) As String
	
	'XX merge
	Function getQualifiedTitle( _
		which As Integer _
	) As String
	
	Function getQualifiedPartTitle( _
		section As String, _
		Optional isUseBlanks As Boolean = True _
	) As String	
	
	
	Function transformText( _
		input As String _
	) As String
	
	
	Function existSection(name As String) As Boolean
	
	Function getPartNames() As String()
	
	'XX merge
	Function getPartAttribute( _
		attrib As String, _
		partnm As String _
	) As String
	
	Function getPartAttribute2( _
		partn As String, _
		attrib As String, _
		Optional defaulv As String = "", _
		Optional isRecursive As Boolean = False _
	) As String
	
	Function getTabTitle(idx As Integer) As String
	
	Function readPageTitles() As String() 
	
	
	Function process(ByRef xnode As XmlElement) As String
	
	Function processControls(target As String) As String 'XX
	
	
	Function getDirTree() As ArticleDirectory
	
	Function getDirectoryHtml() As String
	
	'XX
	Function getNode(pty As String) As XmlElement
	
	
	Function isMultiple(Optional isSet As Boolean = False) As Boolean
	
	
	'XX
	Function readContent() As String
	
	
	'XX merge
	Function read( _
		target As String, _
		ByRef isReadOk As Boolean, _	
		Optional cfile As XmlFile = Nothing, _
		Optional partnm As String = ""
	) As String
	
	
	Function getBodyXmlPath() As String
	
	' the html body tag
	Function readBodyAttribute(name As String) As String
	
	Function checkBodyFlag( _
		name As String, _
		Optional isDef As Boolean = False _
	) As Boolean
	
	'XX rm 
	Sub setXPage(ByRef xpg As ZXPage)
	
	'XX rm
	Function isCorsRequest() As Boolean
	
	Sub setIsCorsRequest(bv As Boolean)
	
End Interface 'IContentProvider

End NameSpace