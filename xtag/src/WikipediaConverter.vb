
Namespace zxweb.xtag

'*******************************************************************************
'

Public Class WikipediaConverter
	Inherits XTagConverter

Public Sub New()
	MyBase.New("wkpd", True) 
	setNewWay(True)
End Sub

	
Protected Overrides Function transformTag( _
	Optional ByRef outIsRsltOk As Boolean = True _
) As String
	Dim rslt As String = ""	
	If CurrentTag.Argstr = "" Then Return ""
		
	Dim param0 As String = getParam()
	Dim url As String = "http://en.wikipedia.org/wiki/" & param0 'XX
	Dim title As String = IIf(CurrentTag.Body = "", _
		"Wikipedia:" & param0, CurrentTag.Body)
		
	title = decorate(title)			
	rslt = Util.genAnchor2(url, title)				
	'XTrace.show("WikipediaConverter.transform", "result", news)
	
	Return rslt		
End Function
	
End Class 'WikipediaConverter

End Namespace