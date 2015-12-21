
Namespace zxweb.xtag 

'*******************************************************************************
'	Process multiple tags all at once. 
'
'!!	Don't use Converter as the name, since collides with System.Converter.
'
'	Todos:
'		- extract tag class.
'		2011.11.21 - optimize passon()
'
'	History:
'		2011-11-20 changed defaulthost
'

Public Class XTagConverter
	Inherits ConverterBase
	
Private _isNewWay As Boolean = False
Protected Function isNewWay() As Boolean
	Return _isNewWay	
End Function
Protected Sub setNewWay(Optional bv As Boolean = True)
	_isNewWay = bv	
End Sub
	
	
'*****************
'	Life cycle
'
'*****************

'XX
Public Sub New()
End Sub
	
Public Sub New( _
	tkey As String, _
	Optional hasValues As Boolean = False _
)
	newHelper(tkey, hasValues)
End Sub

Public Sub New( _
	tkey As String, _
	argType As EnumArgType _
)
	newHelper(tkey, argType)
	
	'setNewWay()
End Sub


'*************
'	Events
'
'*************

'!! The 'middle' should not be modified. 
'XX merge 
'!! obsolete
Protected Overridable Function transform( _
	ByRef body As String _
) As String
	XTrace.showRun("XTagConverter.transform")
	
	If _isNewWay Then 
		Return ""	
	End If
	
	Dim akey As String = getKey()	
	
	If body = "" Then
		body = getParam() 'XX last
	End If
	
	If body = "" Or akey = "" Then 
		XTrace.warn("XTagConverter.transform", "body or key empty, return")	
		XTrace.show("XTagConverter.transform", "key", akey, _
			"body", body)
		Return "" ' no change
	End If

	Dim rslt As String = "<" & akey & ">" & body & "</" & akey & ">"		

	XTrace.showEnd("XTagConverter.transform")
	Return rslt
End Function


Protected Overrides Function transform2( _
	ByRef ioFullText As String _
) As Integer 
	Dim rslt As Integer
	
	If String.IsNullOrEmpty(ioFullText) Then
		Return 0 '?? ex
	End If
	
	XTrace.show("XTagConverter.transform2", _
		"CurrentTag.getWhole()", CurrentTag.getWhole())

	If Not isNewWay() Then
		Dim news As String = transform(CurrentTag.Body)	
		XTrace.show("ConverterBase.transform", "Transformed")				
		
		'XX allow "" replacement
		If news <> "" Then	
			'!! don't replace with "" 
			ioFullText = ioFullText.Replace(CurrentTag.getWhole(), news)					
		End If	
	Else
		Dim isok As Boolean = True '!! default
		Dim news2 As String = transformTag(isok)
		
		If isok Then	
			ioFullText = ioFullText.Replace(CurrentTag.getWhole(), news2)					
		End If		
	End If
	
	rslt = ioFullText.Length 
	'XTrace.show("XTagConverter.transform2", "ioFullText", ioFullText)
		'"rslt", rslt)
	Return rslt
End Function

End Class 'XTagConverter

End Namespace