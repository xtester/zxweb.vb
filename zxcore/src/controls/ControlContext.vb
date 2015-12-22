' Creation Date: 2013/9/16
' Copyright (c) Xun Zhang
' www.zhangxun.com

Namespace zxweb

'*******************************************************************************
'	Used by: 
'		ControlLoader, ArticleHandler
'
'XX merge
'

Public Class ControlContext
	
Private _assembly As String
Public Property Assembly() As String
	Get
		Return _assembly
	End Get
	Set
		_assembly = value
	End Set
End Property


Private _package As String
Public Property Package() As String
	Get
		Return _package
	End Get
	Set
		_package = value
	End Set
End Property


' Where the control is declared.
Private _xmlfile As XmlFile
Public Function getXmlFile() As XmlFile
	Return _xmlfile
End Function


'*****************
'	Life cycle
'
'*****************
	
Public Sub New()
End Sub

Public Sub New(assemb As String, packg As String)
	Assembly = assemb
	Package = packg	
End Sub

Public Sub New(ByRef inf As XmlFile)
	_xmlfile = inf
End Sub

End Class 'ControlContext

End Namespace