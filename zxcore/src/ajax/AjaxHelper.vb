' Copyright (c) Xun Zhang
' http://www.zhangxun.com
' mailto:zhangxun_service@hotmail.com

Namespace zxweb.ajax

'*******************************************************************************
'	TargetName: for client id
'
'	Todos:
'

Public Class AjaxHelper
	
Public Shared Sub process( _
	ByRef ctrl As ControlBase, _		
	ByRef ctxt As Hashtable _
)
Try
	If ctrl Is Nothing Or ctxt Is Nothing Then Return
	
	' To write div...
	
	Dim target As String = "zx" & ctxt("TargetName") & "_" & ctrl.getCtrlId() 'XX conf
	XTrace.show("AjaxHelper.process", "targetDiv", target)
	ctrl.writeDiv("id='" & target & "'", "载入数据...") 
	
	' To write action...
	
	Dim host As String = "" 
	'XX ctrlid=1 not used
	
	Dim param As IDictionaryEnumerator = ctxt("Params").GetEnumerator()
	Dim pstr As String = ""
	While param.MoveNext()
		pstr &= "&" & param.Key & "=" & param.Value
	End While
	
	'!!
	Dim url As String = host & ctxt("BaseLink") & pstr & "&output=" & target
	'XTrace.show("AjaxCommentDisplay.writeContent", "url", url)
	
	Dim action As String = "ajaxShow('" & url & "', '" & target & "');" 'XX
	ctrl.writejs(action) '!! use postactions?			
	
	XTrace.show("AjaxHelper.process", "Action", action)
Catch ex As Exception
	XTrace.show("AjaxHelper.process", ex)	
End Try	
End Sub 'process


Public Shared Sub writeResponseHeader(ByRef ctrl As ControlBase)
	If ctrl Is Nothing Then Return	
	ctrl.write(XPage.genOutputTag(ctrl.getClientOutputId()))	
End Sub


' etc: !! must have an enclosing "'"
'
'XX
Shared Public Function genCommand( _
	clsnm As String, _
	divid As String, _
	sname As String, _
	Optional nmspc As String = "zxweb", _
	Optional assemb As String = "", _
	Optional etc As String = "" _
) As String
	Dim addition As String = IIf(etc = "", "'", "&" & etc) 'XX
	
	Dim rslt As String = "zxj.ajaxSend('" & _
		AjaxGateway.getLink(clsnm, "1", nmspc, assemb) & _
		"&sname=" & sname & addition & _
		", '" + divid + "');" 
	'!! addition
	
	Return rslt
End Function
	
End Class 'AjaxHelper

End Namespace