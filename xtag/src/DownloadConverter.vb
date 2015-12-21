

Namespace zxweb.xtag

'********************************************************************
' 	Class: DownloadConverter
'
'

Public Class DownloadConverter
	Inherits XTagConverter

Public Sub New()
	MyBase.New("downl", True) 'have values
End Sub

	
Protected Overrides Function transform( _
	ByRef middle As String _
) As String
	Dim rslt As String = ""
	
	If getArgString() <> "" Then
		Dim fnbase As String = getParam() 
		Dim filename As String = fnbase
		Dim ftype As String = "." & getParam(2)
		Dim ver As String = getParam(3) 'XX
		
		If ver <> "" Then
			filename &= ver & ftype
		Else
			filename &= ftype
		End If
		
		Dim url As String = "/download/" & filename 
		Dim icon As String = IIf(ftype = ".pdf", _
			"<img src='/img/pdf_icon.gif' border='0' height='16px' /> ", "")
		
		Dim isCompact As Boolean = Util.exist(getArgString(), "compact")
		
		Dim title As String = icon & "下载" & _
			IIf(isCompact, "", """" & fnbase & """") '?? don't use chn “, error display
		
		If Not isCompact Then
			Dim pages As String = getParam(4)
			If pages <> "" Then
				title &= "（" & ver & "，" & pages & "页）"
			End If			
		End If
		
		rslt = Util.genAnchor2(url, title)			
	End If			
	
	'XTrace.show("DownloadConverter.transform", "result", news)
	Return rslt		
End Function
	
End Class 'DownloadConverter

End Namespace