
Imports zxweb.data

NameSpace zxweb.xtag

'*******************************************************************************
'

Public Class AutodirConverter
	Inherits XTagConverter
	
Private _pc As IContentProvider

'*****************
'	Life cycle
'
'*****************

Public Sub New()
	MyBase.New("autodir")
End Sub

' Called by: 
Public Sub New(ByRef ctx As Object)
	MyClass.New()
	setContext(ctx)
End Sub


Protected Function prepareData() As Boolean
	Dim rslt As Boolean = False
	
	Try
		_pc = CType(getContext(), IContentProvider)
		If Not _pc Is Nothing Then rslt = True
	Catch ex As Exception
		XTrace.show("AutodirConverter.prepareData", ex)		
	End Try
	
	Return rslt
End Function


Protected Overrides Function transform( _
	ByRef middle As String _
) As String
	Dim rslt As String = ""
	
	If prepareData() Then 
		rslt  = _pc.getDirectoryHtml()		
	End If
	
	Return rslt
End Function

End Class 'AutodirConverter

End Namespace