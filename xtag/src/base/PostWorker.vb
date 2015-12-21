' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/4/27

Namespace zxweb.xtag

'*******************************************************************************
'	The last default worker in the converter chain.
'

Public Class PostWorker
	Inherits ConverterBase
	
Public Sub New(ByRef inText As String)		
	feed(inText)			
End Sub


Public Overrides Function run() As String
	Dim rslt As String = ""
	XTrace.showRun("PostWorker.run")
	
Try
	rslt = getTextInput()
	
	If Not String.IsNullOrEmpty(rslt) Then
		rslt = rslt.Replace("\/]", "/]") ' footer escape
	End If
	
	Dim escapedTags As ArrayList = getMaster().getEscapedTags()	
	If escapedTags IsNot Nothing Then
		For i As Integer = 0 To escapedTags.Count - 1
			Try
				Dim map As EscapedTagInfo = escapedTags(i)				
				
				XTrace.show("PostWorker.run", "Old", map.Old, _
					"Corrected", map.Corrected)
				rslt = rslt.Replace(map.Old, map.Corrected)				
			Catch ex As Exception
				XTrace.show("PostWorker.run>For", ex)
			End Try
		Next
	End If
Catch ex As Exception	
	XTrace.show("PostWorker.run", ex)
	XTrace.checkNull("PostWorker.run>ex", "master", getMaster())
End Try		
	
	XTrace.showEnd("PostWorker.run")
	Return rslt
End Function

End Class 'PostWorker

End Namespace