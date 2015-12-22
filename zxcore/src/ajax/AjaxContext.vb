' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com

NameSpace zxweb.ajax

'*******************************************************************************
' 	Todos:
'

Public Class AjaxContext

Private _container As ZXPage
Public Function getContainer() As ZXPage 'XX
	Return _container
End Function


Private _params As NameValueCollection
Public Function getParams() As NameValueCollection
	Return _params
End Function


' client id 
Private _outputId As String
Public Function getOutputId() As String
	Return _outputId
End Function


'XX need parent?
' oid: output
Public Sub New( _
	ByRef parent As ZXPage, _
	ByRef nvs As NameValueCollection, _
	oid As String _
)
	_container = parent
	_params = nvs
	_outputId = oid
End Sub

End Class 'AjaxContext

End Namespace