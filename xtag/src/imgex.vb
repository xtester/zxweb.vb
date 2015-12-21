
NameSpace zxweb.xtag 

'*******************************************************************************
'	Todos:
'		- Merge with img
'

Public Class ImgConverterEx
	Inherits ImgConverter

Public Sub New(ByRef ctxt As Object)
	newHelper("imgex", True)
	setNewWay(True)
	setContext(ctxt)
End Sub


'XX page container
Protected Overrides Function transformTag( _
	Optional ByRef outIsRsltOk As Boolean = True _
) As String
	Dim bsty As String = CType(CurrentTag(), XTag).getBoxStyle(True)
	If bsty <> "" Then 
		bsty = " " & bsty
	End If
	
	'XX
	Dim rslt As String = "<div class='imgex'" & bsty & ">"
	
	_isUseDefaultWidth = False
	
	rslt &= MyBase.transform(CurrentTag.Body) 'XX
	rslt &= "</div>"
	
	XTrace.show("ImgConverterEx.transform", "rslt", rslt)	
	Return rslt
End Function 'transformTag

End Class 'ImgConverterEx

End Namespace