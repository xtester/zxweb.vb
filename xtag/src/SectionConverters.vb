
Imports zxweb.domain

NameSpace zxweb.xtag

'*******************************************************************************
' 	[section=sname, sec]...
'
'	Todos:
'		- merge with sname tag?
'

Public Class SectionConverter
	Inherits XTagConverter
	
Private _sname As String
Public Function getSName() As String
	Return _sname	
End Function
Protected Sub setSName(ins As String)
	_sname = ins
End Sub
	
Private _section As String
Public Function getSection() As String
	Return _section	
End Function
Protected Sub setSection(ins As String)
	_section = ins	
End Sub


Private _xresrc As IWebResource
Public Function getXResource() As IWebResource
	Return _xresrc	
End Function


'*****************
'	Life cycle
'
'*****************

Public Sub New()
	MyBase.New("section", True)
End Sub

'!! for subclasses
Protected Sub New(key As String)
	MyBase.New(key, True)
End Sub


'***************
'	Services
'
'***************

Protected Sub createXResource()
	_xresrc = AppHome.getFactoryResource().createInstance(_sname) 		
End Sub


' Return: False - failed
Protected Overridable Function prepareData() As Boolean
	Dim rslt As Boolean = False	
	If getArgString() = "" Then Return rslt	
	
Try
	Dim factory As IFactoryResource = AppHome.getFactoryResource()
	_sname = factory.adjustSname(getParam()) 		
	createXResource() '!! after setting _sname
	
	_section = getParam(2)	
	If _section = "n" Then _section = "" '!! for new tab XX
	
	XTrace.show("SectionConverter.prepareData", _
		"_sname", _sname, "_section", _section)
		
	If _section <> "" Then '!!
		rslt = _xresrc.existPart(_section)
		If Not rslt Then
			XTrace.warn("SectionConverter.prepareData", _
				"Resource <i>" & _sname & "</i> has no section named '" & _
				_section & "'")
		End If
	Else
		rslt = _xresrc IsNot Nothing
	End If
Catch ex As Exception	
	XTrace.show("SectionConverter.prepareData", ex)	
	XTrace.checkNull("SectionConverter.prepareData>ex", "_xresrc", _xresrc)	
End Try

	XTrace.show("SectionConverter.prepareData", "Return", rslt)	
	Return rslt
End Function 'prepareData


' Used by:
'	IncludeConverter
'
Protected Function getUrl( _
	middle As String, _
	Optional tip As String = "", _
	Optional isNewTab As Boolean = False _
) As String
	Dim rslt As String = _xresrc.genAnchor(_section, middle, tip, isNewTab)
	return rslt
End Function


Protected Overrides Function transform( _
	ByRef body As String _
) As String
	XTrace.showRun("SectionConverter.transform")
	If isNewWay() Then Return ""	

	Dim rslt, name2 As String
	rslt = ""
	
	If prepareData() Then
		name2 = body
		
		If body = "" Then 
			If _sname <> "" AndAlso _section <> "" Then
				name2 = _sname & "/" & _section
			Else
				name2 = "<链接>" 'XX conf				
			End If
		End If
		
		name2 = decorate(name2)
		rslt = getUrl(name2)
	Else 
		XTrace.warn("SectionConverter.transform", "input error")
	End If				
	
	XTrace.show("SectionConverter.transform", "Return", rslt)
	Return rslt		
End Function
	
End Class 'SectionConverter

End Namespace