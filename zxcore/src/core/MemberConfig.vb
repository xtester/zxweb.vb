
NameSpace zxweb.core

'*******************************************************************************
'	Meta-data of registered members.
'
' 	Todos:
'		- 
'

Public Class MemberConfig
	
'XX	
Public Shared Function getColumnOwner(sname As String) As String	
	Dim rslt As String = ""

Try	
	Dim conf As XmlFile = XmlFile.load2(SysConfig.getMembersFilePath())
	
	rslt = conf.readNodeAttribute("/root/usr[column=""" & sname & """]/@name")	
Catch ex As Exception
	XTrace.show("MemberConfig.getColumnOwner", ex)
End Try
	
	Return rslt
End Function
	
End Class 'MemberConfig

End Namespace