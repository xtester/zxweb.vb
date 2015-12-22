
Imports zxweb.controllers

NameSpace zxweb

'*******************************************************************************
'	The gateway for all pages. And /entry.aspx?... also makes it work for those
'	sites which don't allow /?... in links.
'

Public Class EntryPage
	Inherits ZXPage

' throw exception
Protected Sub handleAction(action As String) 
	'XX handle 'Action' captial write
	If action = "" Then Return '!! Home
	
	Dim controller As IController = ControllerBase.getInstance(Context, action)
	controller.run()
End Sub


'!! use shadow
Public Shadows Sub Page_Load(sender As Object, events As EventArgs) 
	Dim target As String	 = "" 'ex
	
Try	
	XTrace.showRun("EntryPage.Page_Load")	
	Dim params As NameValueCollection = Request.Params 'XX
	XTrace.show("EntryPage.Page_Load", "RawUrl", Request.RawUrl)	
	
	handleAction(params("action")) '! might forward
	
	If params("sname") = "" Then
	' For default format /?xxx 
		'XTrace.show("/default.aspx", "size", qs.AllKeys.GetUpperBound(0), "key0", qs.AllKeys(0))		
		target = Util.getQueryString(Request.RawUrl) 
		
		If target <> "" Then		
			Dim nurl As String = parseTarget(target)
			XTrace.show("EntryPage.Page_Load", "Redirect to page"£¬ nurl)
			
			redirect(nurl)  
		Else 
		'!! it's Home
			Return		
		End If
	End If
	
	' continue processing...
	XTrace.show("EntryPage.Page_Load", "Call base page load")
	MyBase.Page_Load(sender, events)

	' got sname
	XTrace.show("EntryPage.Page_Load", "sname", _sname)
	If _sname <> "" Then 	
		If getXResource().isLocal() Then
		' Local
			Dim ts As String = getViewTemplate() 			
			forwardToView(ts) 'XX detect template non-existing exception
		Else 
		' Outer web			
			Dim url As String = getContentPath() 'XX
			XTrace.show("EntryPage.Page_Load", "Outer url", url)
			
			countMe()
			redirect(url)
		End If
	End If	
Catch ex As System.Threading.ThreadAbortException
	XTrace.show("EntryPage.Page_Load", "ThreadAbortException!") '
Catch ex As Exception
	XTrace.show("EntryPage.Page_Load", ex)
Finally
	XTrace.show("EntryPage.Page_Load", "Target", target)	
End Try

	XTrace.show("EntryPage.Page_Load", "Return to home.") 
	' default error
	redirect("/") 'XX
End Sub 'Page_Load

End Class 'EntryPage

End NameSpace