
Namespace zxweb.xtag

'*******************************************************************************
'	Syntax:
'		[sname=_sname_{, n, u} /]
'		[sname=_sname_{, n, u}]{_title_}[/sname]
'
'		n: new tab; u: underlined
'

Public Class SNameConverter
	Inherits XTagConverter

Public Sub New()
	newHelper("sname", EnumArgType.MustHave)		
	
	setNewWay(True)
End Sub


Protected Overrides Function transformTag( _
	Optional ByRef outIsRsltOk As Boolean = True _
) As String
	Dim rslt As String = ""	
	If CurrentTag.Argstr = "" Then 
		outIsRsltOk = False
		Return ""
	End If
		
	Dim query, title, sname As String
		
	sname = getParam()				
	title = decorate(IIf(CurrentTag.Body = "", sname, CurrentTag.Body))		
	query = "/?" & sname
	
	Dim isNewWin As Boolean = IIf(CurrentTag.hasParam("n"), True, False)
	'XTrace.show("SNameConverter.transform", "isNewWin", isNewWin)	
	rslt = Util.genAnchor(query, title, query, isNewWin)
		
	Return rslt		
End Function

End Class 'SNameConverter

End Namespace