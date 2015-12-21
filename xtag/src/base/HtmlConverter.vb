' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/4/12

Namespace zxweb.xtag

'*******************************************************************************
'

Public Class HtmlConverter
	Inherits XTagConverter
	
Private _htmlTag As String 	
Public Property HtmlTag() As String
	Get
		Return _htmlTag
	End Get
	Set
		_htmlTag = value
	End Set
End Property


'*****************
'	Life cycle
'
'*****************

Protected Sub New	
End Sub

Public Sub New( _
	tkey As String, _
	Optional correspondent As String = "", _
	Optional bvEmptyBody As Boolean = False _
)
	newHelper(tkey)
	
	setNewWay()
	setArgOptional()
	IsAllowEmptyBody = bvEmptyBody
	
	HtmlTag = IIf(correspondent = "", tkey, correspondent)
End Sub


Protected Overrides Function transformTag( _
	Optional ByRef outIsRsltOk As Boolean = True _
) As String
	Dim rslt As String	
	Dim akey As String = getKey()
	Dim body As String = CurrentTag.Body
	
	If akey = "" Then 
		outIsRsltOk = False
		XTrace.warn("HtmlConverter.transformTag", "Key empty, return")
		Return "" 
	Else
		If _htmlTag <> "" Then akey = _htmlTag
	End If
	
	If body = "" Then 
		If IsBodyFromArg Then
			body = getParam() 'arg1
		End If
		
		If body = "" AndAlso Not IsAllowEmptyBody Then 
			outIsRsltOk = False			
			XTrace.warn("HtmlConverter.transformTag", "Body empty, return")
			Return "" 
		End If
	End If
	
	Dim attributes As String = getAttributes()

	rslt = "<" & akey & attributes & ">" & body & "</" & akey & ">"		
	
	XTrace.show("HtmlConverter.transformTag", "rslt", rslt)
	Return rslt
End Function 'transformTag


Protected Overridable Function getAttributes() As String
	Return ""	
End Function

End Class 'HtmlConverter

End Namespace