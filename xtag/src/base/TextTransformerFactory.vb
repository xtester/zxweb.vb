
NameSpace zxweb.xtag

'*******************************************************************************
'	Todos:
'		- Save transformer in session to speed up?
'

Public Class TextTransformerFactory
	Implements ITransformerFactory	
	
Private Shared _singleton As ITransformerFactory	

Public Shared Function getInstance() As ITransformerFactory
	If _singleton Is Nothing Then
		_singleton = New TextTransformerFactory
	End If
	
	Return _singleton
End Function
	
	
'XX every time new object?
'?? raw for plain
Public Function createTransformer( _
	Optional ttype As String = "default", _
	Optional ByRef ctxt As Object = Nothing _	
) As ITextTransformer _
		Implements ITransformerFactory.createTransformer
	Dim rslt As ITextTransformer = New TextTransformer(ctxt) 
	
	Select Case ttype
	    Case "short"
	    Case Else
	    		rslt.append(getMore(ctxt))
	End Select
	
	Return rslt	
End Function


'XX
Private Function getMore(ByRef ctxt As Object) As ArrayList
	Dim rslt As New ArrayList 
	
	' Images
	rslt.Add(New ImgConverterEx(ctxt)) 	
	'rslt.Add(New SvgConverter()) 'XX	
	
	' Code
	rslt.Add(New ProgramConverter()) ' program or code
	rslt.Add(New Code2Converter()) ' code2
	
	' Snames
	rslt.Add(New SectionConverter()) ' section
	rslt.Add(New SNameConverter()) ' sname
	
	' Inclusion
	rslt.Add(New IncludeConverter()) 'include
	
	' Comments
	rslt.Add(New CommentMarkConverter()) 'comment
	rslt.Add(New QnasConverter()) 	
	
	' Additional
	rslt.Add(New BookConverter(ctxt))
	
	Return rslt
End Function

End Class 'TransformerFactory

End Namespace