
Namespace zxweb.xtag

'*******************************************************************************
'	url, local2, local
'
'	[url=..., button | u /]
'

Public Class LinkMarkConverter
	Inherits XTagConverter

Public Sub New( _
	inkey As String, _
	Optional isUseDefaultHost As Boolean = False _
)
	newHelper(inkey, EnumArgType.MustHave) 
	
	specifyHost(isUseDefaultHost) 'XX
End Sub

	
Protected Overrides Function transform( _
	ByRef middle As String _
) As String
	Dim rslt, title, href, button As String
	rslt = "" : button = ""
	
	href = getParam()
	If href = "" Then Return rslt
		
	button = getParam(2) 'XX
	'XTrace.show("LinkMarkConverter.transform", "link", href, "button", button)
		
	title = middle
	If middle = "" Then 
		If Util.isame(button, "default") Then
			title = "&lt; 链接 &gt;"
		ElseIf button <> "" AndAlso button <> "u" Then 'XX
			title = "< " & button & " >" 
		Else
			title = href		
		End If
	End If
	
	title = decorate(title)	
	
	'XX
	If CurrentTag.Footer = "[/local2]" Then
		rslt = Util.genAnchor(href, title, href) ' not new tab for show							
	Else
		' local or url, open new win/tab
		rslt = Util.genAnchor(href, title, href, True) 
	End If
	
	'XTrace.show("LinkMarkConverter", "rslt", rslt)
	Return rslt		
End Function
	
End Class 'LinkMarkConverter

End Namespace