
Imports System.Reflection
Imports zxweb

NameSpace zx.common

'*******************************************************************************
'	Reflective Object Factory
'
' 	Todos:
'		- Better support of creation attributes (version, culture, etc.).
'

Public Class ObjectFactory
	
'***********
'	Data
'
'***********
	
Private Shared _defaultAssembly As String = "zxcontrols" 'XX
Public Shared Function getDefaultAssembly() As String
	Return _defaultAssembly	
End Function


Private _objMeta As IObjectMeta


'*****************
'	Life cycle
'
'*****************

Public Sub New(ojm As IObjectMeta)
	_objMeta = ojm
End Sub


'***************
'	Services
'
'***************

'@@ tested
Public Function getInstance(keyword As String) As Object
	If keyword = "" Then Return Nothing	
	
	Return getObject(_objMeta.getClassName(keyword), _
		_objMeta.getAssembly(keyword))
End Function


'@@ tested
'
' clsn:  
Public Shared Function getObject( _
	clsn As String, _
	Optional asblyin As String = "" _
) As Object	
	'XTrace.showRun("ObjectFactory.getObject")
	Dim asbly As String = IIf(asblyin = "", _defaultAssembly, asblyin)
	Dim rslt As Object = createInstance(asbly, clsn)
	
	Return rslt
End Function


'@@ tested
'
' classname: qualified, and package name is case-sensitive!!
'
Public Shared Function createInstance( _
	assembly As String, _
	classname As String, _
	Optional constructor As String = "", _
	Optional ver As String = "0.0.0.0", _
	Optional culture As String = "neutral", _
	Optional token As String = "null" _
) As Object
	Dim rslt As Object 	
	If assembly = "" Or classname = "" Then Return Nothing
	
Try
	Dim attrib As String = "Version=" & ver & ", Culture=" & culture & _
		", PublicKeyToken=" & token
	Dim mtype As Type = System.Type.GetType(classname & ", " & assembly & ", " _
		& attrib)
	'!! case_sensitive
		
	If constructor = "" Then
		'XX exception, missing constructor
		'XTrace.warn("ObjectFactory.createInstance", _
		'	"no constructor for " & classname & ", use default")
		
		' calls new() by default
		rslt = mtype.InvokeMember(Nothing, BindingFlags.CreateInstance, _
			Nothing, Nothing, Nothing)
	Else
		' invoke the constructor
		'XTrace.show("ObjectFactory.createInstance", _
		'	"Use constructor", constructor)
		rslt = invoke(Nothing, constructor, Nothing, mtype)
	End If 	
Catch ex As Exception
'	XTrace.show("ObjectFactory.createInstance", ex)
'	XTrace.show("ObjectFactory.createInstance>ex", "assembly", assembly, _
'		"class", classname)
	
	rslt = Nothing
End Try
	
	Return rslt	
End Function 'createInstance


'@@ tested
'
' Invoke the method of a object.
'
'XX excep?; binding flags
'
Public Shared Function invoke( _
	ByRef obj As Object, _
	method As String, _
	Optional ByRef args As Object = Nothing, _
	Optional ByRef inType As Type = Nothing _
£© As Object
	'XTrace.showRun("ObjectFactory.invoke")
	If method = "" Then Return Nothing
	
	Dim objType As Type = Nothing
	
	If obj IsNot Nothing Then 
		objType = obj.GetType()
	Else
		objType = inType
	End If
	
	'XTrace.show("ObjectFactory.invoke", "Method", mnm)
	'XTrace.checkNull("ObjectFactory.invoke", "objType", objType)
	
	Dim rslt As Object = objType.InvokeMember(method, _
		BindingFlags.IgnoreCase Or BindingFlags.Public Or _
		BindingFlags.FlattenHierarchy _
		Or BindingFlags.NonPublic Or BindingFlags.Instance Or _
		BindingFlags.Static Or _
		BindingFlags.InvokeMethod Or BindingFlags.OptionalParamBinding, _
		Nothing, obj, args) 
	' Or BindingFlags.DeclaredOnly 
	' FlattenHierarchy: public and protected static members up the hierarchy 
	' should be returned.
	' http://msdn.microsoft.com/zh-cn/library/system.reflection.bindingflags(v=vs.80).aspx
	
'	If rslt Is Nothing Then
'		XTrace.show("ObjectFactory.invoke", "rslt", "null")
'	Else
'		XTrace.show("ObjectFactory.invoke", "rslt", rslt.ToString())
'	End If
	
	Return rslt
End Function 'invoke


'**************
'	Helpers
'

'@@ tested
'
' Test whether a class exists in the (default or) specified assembly.
'
' clsfn: with ns
' Return: False, when no new (default?)
'
'XX shared
Public Shared Function isOk( _
	clsfn As String, _
	Optional assembly As String = "" _
) As Boolean
	Return getObject(clsfn, assembly) IsNot Nothing
End Function


'@@ tested

' Return a .NET Type object of the specified class.
'
' fullClass: with ns
'
Public Shared Function getSysType( _
	fullClass As String, _
	assembl As String, _
	Optional attrib As String = "" _
) As Type
	Dim rslt As Type = Nothing
	
	If fullClass = "" Then Return rslt
	
	If attrib = "" Then
		'XX default
		attrib = "Version=" & "0.0.0.0" & ", Culture=" & "neutral" & _
			", PublicKeyToken=" & "null"
	End If
	
	rslt = System.Type.GetType(fullClass & ", " & assembl & ", " & attrib) 	
	'XTrace.checkNull("ObjectFactory.getSysType", "rslt", rslt)
	
	Return rslt
End Function

End Class 'ObjectFactory

End Namespace