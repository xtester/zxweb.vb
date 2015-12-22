Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Collections
Imports System.Collections.Specialized
Imports System.Text
Imports System.Xml
Imports Microsoft.VisualBasic

Imports zx
Imports ZX.common


NameSpace zxweb.controllers


'*********************************************************
'	Class: ActionHandlingException
'

Public Class ActionHandlingException
	Inherits Exception
	
Protected _nextView As String
Public Function getView() As String
	Return _nextView
End Function
	
Public Sub New()
End Sub 

Public Sub New( _
	view As String, _
	Optional message As String = "" _
)
	MyBase.New(message)
	_nextView = view
End Sub 

End Class 'ActionHandlingException

End NameSpace