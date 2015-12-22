
Imports System.Web
'Imports System.Web.SessionState
'Imports System.Web.UI
'Imports System.Web.UI.WebControls

NameSpace zxweb.core

'*******************************************************************************

Public Class LoginCookie
	'Inherits 

'XX default expire
Public Shared Function CreateInstance( _
	Optional seed As String = "", _
	Optional expireDays As Integer = 15 _
) As HttpCookie
	Dim rslt As HttpCookie = New HttpCookie("MemberInfo")
	
	rslt("LoginId") = seed
	If seed = "" Then expireDays = -1D
	rslt.Expires = Now.AddDays(expireDays)

	'XTrace.show("LoginCookie.handleAction", "Cookie", cookieId, "usrn", usrn)	
	Return rslt
End Function
	
	
Public Shared Function getCookie(ByRef context As HttpContext) As String
	Dim rslt As String = ""

Try	
	rslt = context.Request.Cookies("MemberInfo")("LoginId")
Catch ex As Exception
End Try	

	Return rslt
End Function

End Class 'LoginCookie

End Namespace