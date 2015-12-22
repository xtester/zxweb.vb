' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com
' Creation Date: 2014/8/9

Namespace zxweb.core

'*******************************************************************************
'	Controls configuration
'

Public Class ControlsConfig
	
Private Shared _xfile As XmlFile


'XX
Private Shared Function getXmlRoot() As String
	Return "/controls"
End Function


Private Shared Function getPropertyPath(name As String) As String
	Return getXmlRoot() & "/" & name
End Function


Public Shared Function getFile() As XmlFile	
	Dim rslt As XmlFile = _xfile
	
	Try		
		'XX
		If rslt IsNot Nothing AndAlso Not rslt.isUpdated() Then 
			Return rslt
		End If	
		
		rslt = XmlFile.load2(SysConfig.getControlsFilePath()) 					
	Catch ex As Exception
		XTrace.show("ControlsConfig.getFile", ex)
		'XX throw?
	End Try	
	
	Return rslt
End Function 'getFile


Public Shared Function readProperty( _
	propName As String, _
	Optional defv As String = "" _
) As String
	Dim rslt As String = ""
	
	If propName = "" Then 
		Return ""	
	End If
	
	rslt = getFile().readXml(getPropertyPath(propName)) 
	If rslt = "" Then rslt = defv
	
	Return rslt
End Function

End Class 'ControlsConfig

End NameSpace