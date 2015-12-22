' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2015/3/9

Namespace zxweb.data

'*******************************************************************************
'

Public Class DataFile
	
Private _xfile As XmlFile

' The relative file path
Private _path As String
Public Sub setPath(ins As String)
	_path = ins	
End Sub
Public Function getPath() As String
	Return _path 'XX use _xfile?
End Function


Shared ReadOnly Private s_root As String = "/root" 'XX 


'*************
'	Events
'
'*************

Protected Overridable Sub init()
End Sub


'***************
'	Services
'
'***************

Protected Function getFile() As XmlFile
	Dim rslt As XmlFile = _xfile
	
	If rslt Is Nothing OrElse rslt.isUpdated() Then
		_xfile = XmlFile.load2(getPath()) 
		rslt = _xfile
	End If
	
	Return rslt
End Function


'XX
Public Function readValue( _
	path As String, _
	Optional ByRef outVar As String = Nothing, _
	Optional isOverride As Boolean = False _
) As String
	Dim rslt As String = ""
	
	If path = "" Then Return ""
	If outVar IsNot Nothing AndAlso outVar <> "" AndAlso Not isOverride Then 
		Return outVar
	End If
	
	rslt = getFile().readInnerText(s_root & "/" & path)	
	outVar = rslt
	
	Return rslt
End Function

End Class 'DataFile

End Namespace