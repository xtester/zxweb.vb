' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/12/14

Namespace zxweb.domain

'*******************************************************************************
'

Public Interface IWebResource
	
	Function isLocal() As Boolean
	
	Function isLocked() As Boolean
	
	Function isSecured() As Boolean
	
	Function isCommentResource() As Boolean 
	
	Function isQnasResource() As Boolean
	
	
	'XX
	Function getPartNames() As String()
	
	Function getPartTitles() As String()
	
	Function getPartTitle(i As Integer) As String
	
	Function getTitle(
		Optional section As String = "" _
	) As String
	
	Function getQualifiedSubtitle(which As Integer) As String
	
	
	Function getProperty( _
	 	akey As String, _
		Optional attrib As String = "" _
	) As String
	
	
	Function getViewPath() As String
	
	Function getViewTemplate() As String
	
	
	Function existPart(part As String) As Boolean
	
	
	Function getContent(part As String) As String
	
	' of page 
	Function getSpecificContent( _
		ByRef target As String, _
		ByRef outs As String _
	) As Boolean
	
	Function genAnchor( _
		part As String, _
		Optional middle As String = "", _
		Optional tip As String = "", _
		Optional isNewTab As Boolean = False _	
	) As String

	
	Function countVisit() As Integer
	
	
	Sub setContentHandler(ByRef handler As IContentProvider)
	Function getContentHandler() As IContentProvider
	
	Sub setRequestedPartName(ins As String)
	
	'XX
	Function getXmlFile() As XmlFile	
	
	'XX 
	Sub init()
	
	Function isUsePath() As Boolean
	
	Sub setIsCorsRequest(bv As Boolean)
	
	'XX
	Function readTitleAndXmlNode( _
		Optional ByRef outXnode As XmlElement = Nothing _
	) As String
	
	
	Function getHideList() As MyArrayList	
	
	
	Function isCorsProxy() As Boolean
	
	Function isControlEnabled( _
		control As String, _
		Optional defval As Boolean = True _
	) As Boolean
	
	Function isShowSubtitle(part As String) As Boolean
	
	Function isCommentAllowed() As Boolean
	
	Function isUseAdvancedSideBar() As Boolean	
	
	
	Function getPrimaryKey() As String
	
End Interface

End Namespace