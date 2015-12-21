' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/3/17

Namespace zxweb.xtag

'*******************************************************************************
'

Public Class XTag
	Inherits AbstractTag	
	
'*****************
'	Life cycle
'
'*****************

'XX obsolete
Public Sub New( _
	inkey As String, _
	Optional isHaveParams As Boolean = True _
)
	Keyword = inkey
	Name = inkey
	
	If isHaveParams Then
		setArgOptional()
	Else
		setNoArg()
	End If
	
	init()
End Sub

Public Sub New( _
	inkey As String, _
	argType As EnumArgType _
)
	Keyword = inkey
	Name = inkey
	
	setArgType(argType)
	
	init()
End Sub

Private Sub init()
	If Keyword = "" Then Return 'XX 
	
	HeaderBeginMark = "[" & Keyword	
	Footer = "[/" & Keyword & "]"
	
	If isMustHaveArgs() Then 
		HeaderBeginMark &= "=" 
	End If
	
	HeaderClosingMark = "]"
	CompactHeaderClosingMark = "/]" '!! not have the empty char
End Sub


'***************
'	Services
'
'***************

' Used by: program, imgex
'
'XX support param name like 'boxHeight='
Public Function getBoxStyle( _
	Optional isAutoCalc As Boolean = False, _	
	Optional defidx As Integer = -1, _
	Optional more As String = "" _	
) As String
	Dim rsltbh As String = ""
	Dim params As ArrayList = getParamList()
	
	Dim boxheight As String = getParam("boxh", defidx)
	If boxheight = "" And isAutoCalc Then
		Try
			Dim ctenth As Integer = CInt(getParam("h")) 'XX
			If ctenth > 0 Then boxheight = ctenth + 30			
		Catch ex As Exception
		End Try
	End If
	
	If boxheight <> "" Then 
		rsltbh = "style='height: " & boxheight & "px;" & more & "'" 'XX
	End If
	
	Return rsltbh
End Function

End Class 'XTag

End Namespace