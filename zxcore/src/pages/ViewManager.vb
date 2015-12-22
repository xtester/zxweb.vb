' Creation Date: 2013/11/5
' Copyright (c) Xun Zhang
' www.zhangxun.com

Namespace zxweb

'*******************************************************************************
'	Todos:
'		- Refacotr check()
'

Public Class ViewManager
	
Private _page As ZXPage	'XX 


Private Shared s_defaultView As String 

' Return: with '.aspx'
'XX 
Public Shared Function getDefaultViewFileName() As String
	Dim rslt As String = s_defaultView
	
	If rslt = "" OrElse SysConfig.isDirty() Then
		rslt = SysConfig.readNode("templates/default") 'XX
		setDefaultView(rslt)
	End If
	
	'XTrace.show("ViewManager.getDefaultFileName", "rslt", rslt)
	Return rslt
End Function

' ins: just filename
Public Shared Sub setDefaultView(ins As String)
	s_defaultView = ins	
End Sub


'XX config
' Return: path
'
Public Shared Function getDefaultViewPath() As String
	Return "/_templates/" & getDefaultViewFileName()
End Function


'*****************
'	Life cycle
'
'*****************

'XX
Public Sub New(inpg As ZXPage)	
	_page = inpg
End Sub


'***************
'	Services
'
'***************

' auto transfer, may call forward...
'
'XX refactor, use config
'
Public Sub checkView( _
	viewPathByConfig As String, _
	viewPathNow As String, _
	forced As String _
)
	XTrace.showRun("ViewManager.checkView")
	
	If Util.isEnabled(forced) Then
		XTrace.warn("ViewManager.checkView", "Forced use, return")
		Return
	End If
	
	XTrace.show("ViewManager.checkView", _
		"viewPathNow", viewPathNow, _
		"viewPathByConfig", viewPathByConfig, _
		"forced", forced, _
		"Default view", getDefaultViewFileName())
	
	' To select out comments
	'XX
	If Util.exist(viewPathByConfig, "tmpl_basic.aspx", _
			"tmpl_book.aspx", _
			"tmpl_refresource.aspx", _
			"twocols", _
			"tmpl_onecol.aspx", _
			"tmpl_course.aspx") _
		AndAlso _
			Not Util.exist(viewPathNow, getDefaultViewFileName(), _
				"tmpl_CommentEditor.aspx", "tmpl_comments.aspx") _
	Then
		' All use the default
		_page.forwardToView(getDefaultViewPath()) 
	End If
	
	Dim viewNameByConfig As String = extractFileNameBase(viewPathByConfig)
	
	'XX base <> "/entry.aspx" And, error logic 2011-11-13
	If viewPathNow <> viewPathByConfig Then 'XX conf
		' Comment
		If (Util.exist(viewPathNow, "tmpl_CommentEditor.aspx")) _
			AndAlso _
			(Util.exist(viewPathByConfig, "tmpl_comments.aspx") _
			OrElse Util.exist(viewPathByConfig, "treeview2") _
			OrElse Util.exist(viewPathByConfig, getDefaultViewFileName()) _
			OrElse Util.exist(viewPathByConfig, "tmpl_book.aspx") _			
			OrElse Util.exist(viewPathByConfig, "tmpl_refresource.aspx") _
			OrElse Util.exist(viewPathByConfig, "tmpl_test.aspx")) Then
			' ok, don't forward
						
		ElseIf (Util.exist(viewPathNow, "tmpl_comments.aspx")) _
			AndAlso _
			(Util.exist(viewPathByConfig, "treeview2", getDefaultViewFileName(), _
				"tmpl_book.aspx")) Then 
			' ok
			
		' Qnas	
		ElseIf (Util.exist(viewPathNow, "qnaseditor")) _
			AndAlso _
			Util.exist(viewPathByConfig, "qnasbrowser") Then
			' ok
			
		ElseIf isTemplateCorrect(extractFileNameBase(viewPathNow), _
					viewNameByConfig) Then 
			' ok
			
		'!! new version, in templates.xml
		'XX search in the view templates file
		ElseIf Util.exist(viewPathNow, getDefaultViewFileName()) _
			AndAlso _
				(Util.exist(viewPathByConfig, _
				"tmpl_basic.aspx", "tmpl_book.aspx", _
				"tmpl_refresource.aspx", "tmpl_twocols", _
				"tmpl_onecol.aspx", "tmpl_course.aspx") OrElse _
				TemplateFactory.existTemplate("views/" & viewNameByConfig)) _
			Then
			' ok
			
		Else
			' Correct the template by forwarding
			XTrace.warn("ViewManager.checkView", "Views mismatch")
			
			Dim destin As String = viewPathByConfig
			
			If Not File.Exists(AppHome.mapPath(destin)) Then
				XTrace.warn("ViewManager.checkView", _
					"View by config not exist, use default")
				destin = getDefaultViewPath()
			End If

			XTrace.warn("ViewManager.checkView", "Forward to " & destin)	
			_page.forwardToView(destin)
		End If
	End If
	
	XTrace.show("ViewManager.checkView", "Correct", viewPathNow)
	XTrace.showEnd("ViewManager.checkView")
End Sub 'check


'
Private Shared Function isTemplateCorrect( _
	baseView As String, _	
	tmplName As String _
) As Boolean
	Dim rslt As Boolean = False
	
	If baseView = "" Then Return False ' Error
	
	If tmplName = "" OrElse Util.isame(tmplName, s_defaultTemplate) Then 
		Return True '!!
	End If
	
	Dim str As String = SysConfig.readNode("views/" & baseView)
	If Util.exist(str, tmplName) Then
		rslt = True
	End If
	
	XTrace.show("ViewManager.isTemplateCorrect", "baseView", baseView, _
		"Template defined", str, "Template current", tmplName)
	Return rslt
End Function 'isTemplateCorrect


' onecol, course are for craig
'
'XX
Public Shared Function adjustView( _
	ins As String, _
	Optional isReturnEmptyString As Boolean = False _
) As String
	Dim rslt As String = ins	
	
	If ins = "" Then
		rslt = IIf(isReturnEmptyString, "", getDefaultViewFileName())
	Else		
		If Util.exist(ins, "basic", "book", _
			"refresource", "twocols", "onecol", "course") Then 
			rslt = getDefaultViewFileName()
		End If
	End If
	
	Return extractFileNameBase(rslt)
End Function


'**************
'	Helpers
'

' Remove the prefix and postfix of the filename.
'
'XX mv to Util; use LTrim, RTrim
'
Private Shared Function extractFileNameBase(ins As String) As String
	Dim rslt As String = ins
	If ins = "" Then Return ""
	
Try	
	'!! Get only the filename
	Dim posSlash As Integer = rslt.LastIndexOf("/")
	If posSlash >= 0 Then
		rslt = rslt.Substring(posSlash + 1)
	End If
	
	' Some may not have prefixes.
	Dim pos1 As Integer = rslt.IndexOf("tmpl_")	'XX	
	If pos1 >= 0 Then
		rslt = rslt.Substring(pos1 + 5)	
	End If
	
	Dim pos2 As Integer = rslt.IndexOf(".aspx")
	If pos2 >= 0 Then
		rslt = rslt.Substring(0, pos2)
	End If
Catch ex As Exception
'	
End Try	
	
	Return rslt
End Function 'extractFileNameBase


Private Shared s_defaultTemplate As String = "default_doc" 'XX conf

' return: 
'XX
Public Shared Function getBlockName(viewPath As String) As String
	Dim rslt As String = s_defaultTemplate 
	Dim tmps As String = extractFileNameBase(viewPath)	
	
	XTrace.show("ViewManager.getBlockName", "ins", viewPath)
	
	If tmps <> "" AndAlso (Not ViewManager.isDefaultView(tmps)) Then 
		' Not use the default block
		rslt = tmps
	End If	
	
	XTrace.show("ViewManager.getBlockName", "rslt", rslt)	
	Return rslt
End Function


' Is it the default view (.aspx)?
'
' ins: 
'
Public Shared Function isDefaultView(ins As String) As Boolean
	Dim rslt As Boolean = False
	If ins = "" Then Return rslt
	
	rslt = Util.isame(ins, extractFileNameBase(getDefaultViewFileName()))
	Return rslt
End Function

End Class 'ViewManager

End Namespace