
NameSpace zxweb.xtag

'*******************************************************************************
'	[comment=tt-comment,159
'

Public Class CommentMarkConverter
	Inherits XTagConverter

Protected _type As String = "comment"


Public Sub New()
	MyBase.New("comment", True)
End Sub


Protected Overrides Function transform( _
	ByRef middle As String _
) As String
	Dim rslt, title As String
	Dim sname, cid, query, template As String		
	
	rslt = ""	
	If getArgString() = "" Then Return ""
		
	sname = getParam()
	cid = getParam(2)
	'XTrace.show("Util.replaceUrl", "sname", sname, "id", cid)						
		
	If sname <> "" And cid <> "" Then		
		query = "?sname=" & sname & "&id=" & cid
		'XX 
		template = Util.readXmlElementContent("/templates/virtualPath") 'XX
		template &= Util.readXmlElementContent("/templates/" & _type)										
		'XTrace.show("Util.replaceUrl", "template", template)
		
		If template <> "" Then 'XX		
			Dim url As String = template & query		
			
			title = IIf(middle = "", sname & "/" & cid, middle)			
			rslt = Util.genAnchor(url, title, url) 'XX new win
		End If
	End If	
			
	Return rslt		
End Function 'transform
	
End Class 'CommentMarkConverter


'*******************************************************************************
'

Public Class QnasConverter
	Inherits CommentMarkConverter
	
Public Sub New()
	setTags("qnas", True)
	
	_type = "qnas"
End Sub
	
End Class 'QnasConverter

End Namespace