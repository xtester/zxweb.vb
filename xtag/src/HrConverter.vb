' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/4/30

Namespace zxweb.xtag

'*******************************************************************************
'

Public Class HrConverter
	Inherits HtmlConverter
	
Public Sub New()
	MyBase.New
	newHelper("hr", EnumArgType.IsOptional)
	
	setNewWay()
	IsAllowEmptyBody = True 'XX
	IsBodyFromArg = False
	
	HtmlTag = "hr"
End Sub


Protected Overrides Function getAttributes() As String
	Dim rslt As String = ""
		
	Dim size As String = getParam()
	If size <> "" Then
		rslt &= " size=""" & size & """"
	End If
	
	Dim width As String = getParam("w")
	If width <> "" Then
		rslt &= " width=""" & width & "%"""
	End If	
	
	Return rslt
End Function

End Class 'HrConverter

End Namespace