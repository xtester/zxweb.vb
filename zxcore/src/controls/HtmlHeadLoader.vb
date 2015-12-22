' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/1/26

Namespace zxweb

'*******************************************************************************
'

Public Class HtmlHeadLoader
	Inherits XControl
	
Private _jsHome As String
Public ReadOnly Property JsHome() As String
	Get
		Return _jsHome
	End Get
End Property

	
Protected Overrides Sub initControl()
	MyBase.initControl()
	
	_jsHome = SysConfig.getJsHome() & "/"
	'XTrace.show("HtmlHeadLoader.initControl", "_jshome", _jsHome)	
End Sub	


Protected Sub writeStyleSheetLink( _
	name As String, _
	Optional path As String = "" _
)
	If name = "" Then Return
	
	'XX
	write("<link rel=""stylesheet"" href=""" & path & "/" & name & ".css"" " & _
		"type=""text/css"" />")
End Sub


Protected Overridable Sub loadMoreJs()	
End Sub

Protected Overridable Sub loadCss()		
End Sub


Protected Overrides Sub output()
	loadCss()
	
	writeJsLoader(JsHome & "zx/zxweb.js")
	writeJsLoader(JsHome & "zx/treeview.js")
	
	loadMoreJs()
	
	' jQuery
	'
	writeStyleSheetLink("jquery-ui-1.8.20.custom", JsHome & "css/ui-lightness")
	
	writeJsLoader(JsHome & "jquery-1.7.2.min.js")
	writeJsLoader(JsHome & "jquery-ui-1.8.20.custom.min.js")
	
	writejs("jQuery.noConflict();")
End Sub	
	
End Class 'HtmlHeadLoader

End Namespace