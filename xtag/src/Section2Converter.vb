' Creation Date: 2013/9/12
' Copyright (c) Xun Zhang
' www.zhangxun.com

Namespace zxweb.xtag

'*******************************************************************************
'
	
Public Class Section2Converter
	Inherits SectionConverter
	
Public Sub New()
	MyBase.New("section2")
End Sub

' Called by: ArticleHandler
Public Sub New(ByRef ctx As Object)
	MyClass.New()
	setContext(ctx)
End Sub


Protected Overrides Function prepareData() As Boolean
	Dim rslt As Boolean = False
	
	Try
		setSName(AppHome.getFactoryResource().adjustSname( _
			CType(getContext(), String)))
		'XTrace.checkNull("Section2Converter.prepareData", "context", _context)	
		createXResource() '!!
	Catch ex As Exception
		XTrace.show("Section2Converter.prepareData", ex)		
	End Try
	
	setSection(getParam())
	XTrace.show("Section2Converter.prepareData", _
		"sname", getSName(), "section", getSection())
	
	If getSection() <> "" And getSName() <> "" Then 
		rslt = True
	End If
	
	Return rslt
End Function

End Class 'Section2Converter

End Namespace