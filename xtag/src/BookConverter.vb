
Namespace zxweb.xtag

'*******************************************************************************
'	Display the book image or icon.	
'
'	Todos:
'

Public Class BookConverter
	Inherits ImgConverter
	
Public Sub New(
	ByRef ctxt As Object _
)
	newHelper("book", True)
	setContext(ctxt)
End Sub


Protected Overrides Function transform( _
	ByRef middle As String _
) As String
	Dim rslt, title, sname, imgnm, imgtitle, width, link, htarget As String
	rslt = "" : imgtitle = " " : htarget = ""
		
	sname = getParam()
	
	title = middle
	If middle = "" Then 
		title = getParam("title", 4)
		If title = "" Then title = sname '!! don't change middle
	End If
		
	imgnm = getParam(2)
	'XTrace.show("LinkMarkConverter.transform", "link", sname, "button", button)	
	If sname = "" And title <> "" Then imgtitle = " title='" & title & "' "
	
	width = getParam("w", 3) 
	If width = "" Then 
		width = "80" 'XX
	End If
	If Not Util.exist(width, "px") Then width &= "px"
	
	' Assemble rslt
	'	
	Dim url As String = getHost() & "/archive/books/" & imgnm 'XX conf
	
	rslt = "<img src='" & url & "' border='0' width='" & _
		width & "'" & imgtitle & "/>"
	
	If sname <> "" Then
		link = "/?" & sname
		If Util.exist(link, "http://") Then 
			link = sname
			htarget = " target='_blank'"
		End If
		
		rslt = "<a href='" & link & "' title='" & title & "'" & htarget & ">" _
			& rslt & "</a>"
	End If	
		
	Return rslt		
End Function 'transform
	
End Class 'BookConverter

End Namespace