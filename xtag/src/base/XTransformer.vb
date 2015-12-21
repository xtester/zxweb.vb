' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/3/15

Namespace zxweb.xtag

'*******************************************************************************
'	The facade of the xtag engine.
'
' 	Used by:
'		FloatBarLoader
'

Public Class XTransformer
	
' Use a new transformer object every call.	
'
'XX choose transformer type
'
Public Shared Function process( _
	ByRef src As String, _
	Optional excludes As String = "" _
) As String
	Dim transformer As ITextTransformer = _
		TextTransformerFactory.getInstance().createTransformer() 
	
	transformer.setExcludes(excludes)
	
	Return transformer.run(src)
End Function		
	
End Class 

End Namespace