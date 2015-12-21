' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2014/12/23

Namespace zxweb.xtag

'*******************************************************************************
' 	Todos:
'

Public Interface ITextTransformer
	
	' list: Converter names to be excluded from running
	Sub setExcludes(list As String) 
	
	Function getExcludes() As String 
	
	' src: intact
	Function run(ByRef src As String) As String	
	
	Function getChain() As ArrayList	
	
	Sub append(ByRef cvtrlist As ArrayList)
	
	Function getEscapedTags() As ArrayList
	
	Sub addEscapedTag(ByRef map As EscapedTagInfo)
	
End Interface 'ITextTransformer

End Namespace