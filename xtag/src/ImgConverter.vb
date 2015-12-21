
Imports zxweb.data

Namespace zxweb.xtag

'*******************************************************************************
'	body for img args
'	Param host without http:// 
'
'	Todos:
'		- img title
'		- safety, foreign pics
'

Public Class ImgConverter
	Inherits XTagConverter
	
Protected _isUseDefaultWidth As Boolean = True


'*****************
'	Life cycle
'
'*****************

'!! For derived
Protected Sub New()
End Sub
	
Public Sub New( _
	ByRef ctxt As Object _
)
	MyBase.New("img", True) ' have value
	
	setContext(ctxt)
	'specifyHost(isUseDefaultHost)
End Sub


Protected Function getHost() As String
	Dim existInlineHost As Boolean = Util.exist(getParam(), "http://")	
	Dim rslt As String = getParam("host")
	
	If Not existInlineHost Then
		If rslt = "" Then
			rslt = getDefaultHost()	'XX	
			
			If rslt = "" Then
				Dim handler As IContentProvider 
				
				Try
					handler = CType(getContext(), IContentProvider)
					XTrace.show("ImgConverter.getHost", _
						"Is CORS", handler.isCorsRequest())
					
					If handler.isCorsRequest() Then
						rslt = AppHome.getInstance().getSite()
					End If
				Catch ex As Exception		
					XTrace.show("ImgConverter.getHost~Type change error", ex)
				End Try			
			End If 
		End If
		
		If rslt <> "" Then
			rslt = "http://" & rslt
		End If
	Else
		rslt = ""
	End If
	
	Return rslt
End Function 'getHost

' body: for img properties
'
Protected Overrides Function transform( _
	ByRef body As String _
) As String
	Dim rslt As String = ""
	Dim imgparams As String = body
	
	'XTrace.show("ImgConverter.transform", "attribute", body)
	If getArgString() = "" Then Return ""
	XTrace.show("ImgConverter.transform", "args", getArgString())
	
	If _isUseDefaultWidth Then
		Dim width As String = getParam("w", 2) 
		If width <> "" Then 
			If Not Util.exist(width, "px") Then width &= "px"
			imgparams = "width='" & width & "'"
		End If
		XTrace.show("ImgConverter.transform", "width", imgparams)
	End If
	
	Dim height As String = getParam("h", 3)
	If height <> "" Then 
		If Not Util.exist(height, "px") Then height &= "px"
		imgparams &= " height='" & height & "'"
	End If
	
	If imgparams = "" Then
		' default size 
		imgparams = "width='50%' height='50%'" 'XX conf
	End If
		
	' Assemble rslt
	'	
	Dim url As String = getHost() & getParam()
	rslt = "<img src='" & url & "' " & imgparams & "></img>"	 'XX					
	
	'XTrace.show("ImgConverter.transform", "rslt", rslt)
	Return rslt		
End Function 'transform
	
End Class 'ImgConverter

End Namespace