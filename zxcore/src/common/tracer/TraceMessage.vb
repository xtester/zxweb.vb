' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2014/11/12

Namespace zxweb.trace

'*******************************************************************************
'

Public Class TraceMessage
	
'	
Private _tag As String
Public Property Tag() As String
	Get
		Return _tag
	End Get
	Set
		_tag = value
	End Set
End Property


Private _source As String
Public Property Source() As String
	Get
		Return _source
	End Get
	Set
		_source = value
	End Set
End Property


Private _message As String
Public Overridable Property Message() As String
	Get
		Return _message
	End Get
	Set
		_message = value
	End Set
End Property


'*****************
'	Life cycle
'
'*****************

Public Sub New( _
	name As String, _
	place As String, _
	msg As String _
)
	_tag = name
	_source = place 
	_message = msg
End Sub


Public Overridable Function isException() As Boolean
	Return False	
End Function

End Class 'TraceMessage

End Namespace