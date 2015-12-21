' Creation Date: 2013/10/30
' Copyright (c) Xun Zhang
' www.zhangxun.com

Namespace zxweb.xtag
	
'*******************************************************************************
'

Public Class Subtitle2Converter
	Inherits XTagConverter
	
Private _stylecls As String = "h2new"
Public Sub setStyleClass(ins As String) 'XX
	_stylecls = ins	
End Sub


Public Sub New()
	MyBase.New("sub2", True) 
	setArgOptional()
End Sub


Protected Overrides Function transform( _
	ByRef middle As String _
) As String
	Dim rslt As String = ""
	Dim tag As String = getParam()		
	Dim title As String 
	
	If tag = "" Then tag = middle
	If tag = "" Then 
		XTrace.warn("Subtitle2Converter", "tag empty, return")
		Return ""
	End If
	
	title = "title=""" & tag & """"
	If middle = "" Then middle = tag
		
	'XTrace.show("SubtitleConverter.transform", "attribute", middle)
	rslt = "<div class='" & _stylecls & "' id='" & tag & "' " & title & ">" _
		& middle & "</div>" 'XX
	
	XTrace.show("Subtitle2Converter", "rslt", rslt)
	Return rslt		
End Function	

End Class 'Subtitle2Converter


'*******************************************************************************
'

Public Class Sub2iConverter
	Inherits Subtitle2Converter
	
Public Sub New()
	newHelper("sub2i", True) 
	setArgOptional()
	
	setStyleClass("sub2i")
End Sub

End Class


'*******************************************************************************
'

Public Class Subtitle3Converter
	Inherits Subtitle2Converter
	
Public Sub New()
	newHelper("sub3", True) 
	setArgOptional() 
	
	setStyleClass("sub3")
End Sub
	
End Class 'Subtitle3Converter

End Namespace