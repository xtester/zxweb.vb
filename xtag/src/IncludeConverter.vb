
Imports zxweb.domain

NameSpace zxweb.xtag

'*******************************************************************************
'	Examples:
'		[include=customercomments,intro,no,n /]
'	
'	n - new tab
'

Public Class IncludeConverter
	Inherits SectionConverter
	
Public Sub New()
	MyBase.New("include")
	setNewWay(True) 'XX
End Sub
	

' default: show tag tip
'
Protected Overrides Function transformTag( _
	Optional ByRef outIsRsltOk As Boolean = True _
) As String
	Dim rslt, query As String
	rslt = "" : query = ""
	
	XTrace.showRun("IncludeConverter.transformTag")
	
	If Not prepareData() Then
		outIsRsltOk = False
		XTrace.warn("IncludeConverter.transformTag", _
			"prepareData failed, return")
		Return ""
	End If	
	
	XTrace.show("IncludeConverter.transformTag", "Target sname", getSName(), _
		"section", getSection())
	
	Dim xresrc As IWebResource = getXResource()
	
	Dim tagStyle As String = "style='color: grey; font-size: 8px;'"
	Dim isShowEnclosingTag As Boolean = True
	Dim tip As String = xresrc.getTitle(getSection())
	'IIf(_section = "", _sname, _sname & "/" & _section)
	
	isShowEnclosingTag = Not Util.isDisabled(getParam(3)) '!! tricky
	XTrace.show("IncludeConverter.transformTag", "isShowTag", isShowEnclosingTag)
	
	Dim isNewTab As Boolean = (CurrentTag().getLastParam() = "n") 'XX
	XTrace.show("IncludeConverter.transformTag", "Last param", _
		CurrentTag().getLastParam())
	Util.decorateTip(tip, isNewTab)
	
	If isShowEnclosingTag Then
		query = getUrl("", tip, isNewTab)
		rslt = "<span " + tagStyle + " title='include start'>I:>" & query _
			& ":</span><br />" 'XX format
	End If
	XTrace.show("IncludeConverter.transformTag", "tip", tip, _
		"Query", query)
	
	Dim content As String = xresrc.getContent(getSection())		
	If content <> "" Then
		'XX len
		'XTrace.show("IncludeConverter.transformTag", "Content read", content)
	Else
		XTrace.warn("IncludeConverter.transformTag", "Content empty")
	End If
	rslt &= content
	
	If isShowEnclosingTag Then
		rslt &= "<br /><span " & tagStyle & " title='include end'>" _
			& query & "<:I" & "</span><br />"
	End If		
	
	XTrace.checkString("IncludeConverter.transformTag", rslt, "rslt")
	Return rslt		
End Function 'transformTag

End Class 'IncludeConverter

End Namespace