
Namespace zx.common

'*******************************************************************************
'	Todos:
'

Public Class MyArrayList 
	Inherits ArrayList
	
'@tested
Public Overrides Function ToString() As String
	Dim sb As New StringBuilder 'XX size
	
	For i As Integer = 0 To Count-1 
		Try
			If i < Count - 1 Then
				sb.Append(Item(i).ToString() & ",")
			Else
				sb.Append(Item(i).ToString())				
			End If
		Catch ex As Exception
			XTrace.show("MyArrayList.ToString", ex)
		End Try
	Next
	
	Return sb.ToString()
End Function


'@tested
Public Shared Function dump(ByRef inSrc As ArrayList) As String	
	If inSrc Is Nothing Then
		Return "{}"
	End If
	
	Dim rslt As New StringBuilder 'XX size
	rslt.Append("{")
	
	Dim cnt As Integer = inSrc.Count
	For i As Integer = 0 To cnt - 1 
		Try
			Dim val As String = inSrc.Item(i).ToString()
				
			If i < cnt - 1 Then
				rslt.Append(val & ",")
			Else
				rslt.Append(val)				
			End If
		Catch ex As Exception
			'XTrace.show("MyArrayList.ToString", ex)
		End Try
	Next	
	
	rslt.Append("}")
	Return rslt.ToString()	
End Function

'@tested
Public Shared Function dump(ByRef inSrc As Array) As String	
	If inSrc Is Nothing Then
		Return "{}"
	End If
	
	Dim rslt As New StringBuilder 'XX size
	rslt.Append("{")
	
	Dim cnt As Integer = inSrc.Length
	For i As Integer = 0 To cnt - 1 
		Try
			Dim val As String = inSrc(i).ToString()
				
			If i < cnt - 1 Then
				rslt.Append(val & ",")
			Else
				rslt.Append(val)				
			End If
		Catch ex As Exception
			'XTrace.show("MyArrayList.ToString", ex)
		End Try
	Next	
	
	rslt.Append("}")
	Return rslt.ToString()	
End Function

End Class 'MyArrayList

End Namespace