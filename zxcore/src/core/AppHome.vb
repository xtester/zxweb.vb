
NameSpace zx 'XX zxweb.core

'*******************************************************************************
'	TODO:
'		- remove from .aspx files, and rm checkin()
'		- 2011.1108 remove Comments stuff
'

Public Class DoorKeeper
	Implements IAppCenter
	
'**************
'	Helpers
'
'**************
	
Public Function getMetaProvider() As IMetaProvider _
		Implements IAppCenter.getMetaProvider
	Return ResourceMetaManager.getInstance()
End Function


'XX merge. in CommentFile
Public Shared Function getXResourceMeta() As IMetaProvider
	Return ResourceMetaManager.getInstance()
End Function


Private Shared _server As HttpServerUtility
Public Shared Function getHttpServer() As HttpServerUtility
	Return _server	
End Function


'*************
'	Fields
'
'*************

'*****************
' 	Properties
'
'*****************

' Databases
'

' each session should have its own db obj -> IPersistent
' Shared by all clients ? No.
' Lived longer than _usr
' 

Private Shared _membdb As DB '!! don't use it, could be null.
Public Readonly Property MEMBDB As DB _
		Implements IAppCenter.MEMBDB			
	Get 
		If _membdb Is Nothing Then
			_membdb = New DB("usrdb")
		End If
		
		return _membdb
	End Get
End Property


Private Shared _syspdb As DB 
Public Readonly Property SYSPDB As DB _
		Implements IAppCenter.SYSPDB			
	Get 
		If _syspdb Is Nothing Then 
			_syspdb = New DB("syspdb")
		End If
		
		return _syspdb
	End Get
End Property

'XX merge
Private Shared _locdb As DatabaseSystemProfile
Public Shared Function getSYSPDB() As DatabaseSystemProfile
	If _locdb Is Nothing Then 
		_locdb = New DatabaseSystemProfile("syspdb")
	End If
	Return _locdb	
End Function


Private Shared _resdb As DB
Public Property REFSDB As DB _
Implements IAppCenter.REFSDB		
	Get 
		If _resdb Is Nothing Then
			_resdb = New DB("resdb")
		End If 
		
		return _resdb
	End Get
	Set
		' readonly
	End Set
End Property


Private Shared _patdb As DB
Public Property PATNDB As DB _
Implements IAppCenter.PATNDB	
	Get 
		If _patdb Is Nothing Then
			_patdb = New DB("patdb")
		End If 
		
		return _patdb
	End Get
	Set		
		' readonly
	End Set
End Property


Private Shared _bookdb As DB
Public Property BOOKDB As DB _
Implements IAppCenter.BOOKDB
	Get 
		If _bookdb Is Nothing Then
			_bookdb = New DB("bookdb") 'XX
		End If 		
		return _bookdb
	End Get
	Set		
		' readonly
	End Set
End Property


'***************** 
'	Life cycle 
'
'*****************

'XX
Protected Sub New()
End Sub


Private Shared _singleton As IAppCenter

Public Shared Function getInstance() As IAppCenter		
	If _singleton Is Nothing Then
		_singleton = New DoorKeeper	
	End If
	
	Return _singleton 
End Function


' included by page entries, count page by content file path
'
' XX- called by many .aspx files, don't remove
'
' Used by:
'		LabelCounter
'
Public Shared Sub checkin(ByRef context As HttpContext)	
Try	
	Dim path As String = context.Request.Url.AbsolutePath
	
	init(context)	
	getSYSPDB().countLoc(path) ' instance for thread safe ?	
Catch ex As Exception
	XTrace.show("DoorKeeper.checkin", ex)	
End Try
End Sub 'checkin


' it should be called by every page usr requests before other methods
'XX
'
Shared Private s_isInited As Boolean = False

'!!
Public Shared Sub init(ByRef inContext As HttpContext)
Try
	XTrace.showRun("Doorkeeper.init")			
	If s_isInited Then Return
	
	_server = inContext.Server
	
	'!! Set IAppCenter
	Dim app As IAppCenter = getInstance() 	
		
	XTrace.init(app)
	DB.init(inContext) 'XX					
		
	Dim lib2 As ILibCore = ObjectFactory.getObject("zxweb.core.ZxwebHome") 'XX
	If lib2 IsNot Nothing Then
		lib2.init(app)
	End If
	
	s_isInited = True
Catch ex As Exception
	XTrace.show("Doorkeeper.init", ex)	
End Try

	XTrace.showEnd("DoorKeeper.init")
End Sub 'init


'***************
'	Services
'
'***************

' read from LDB, old use.
'
'XX mv
'
'Public Shared Function readLoc(sname As String) As String
'	Dim rslt, loc As String
'	Dim ldb As DB
'	
'Try	
'	If sname = "" Then Return ""
'	
'	ldb = getLocationDB()
'	'XX wrap the query behind SDB class
'	loc = ldb.readTableField("Locations", "SNAME", sname, "LOC")
'	If loc <> "" Then 	
'		Dim i As Integer = loc.LastIndexOf(".")
'		Dim lpos As Integer = loc.Length() - 1
'		Dim j As Integer = loc.LastIndexOf(".xml")
'		
'		If j < 0 Then 
'		' .aspx or no .xml, replace .* with .xml 
'			Dim ns As String = loc.Substring(i, lpos-i+1)
'			loc = loc.Replace(ns, ".xml")
'		End If
'	End If
'	
'	rslt = loc
'Catch ex As Exception
'	rslt = ""
'	XTrace.show("DoorKeeper.readLoc", ex)
'End Try
'
'	'Util.trace("DoorKeeper.readLoc", "loc: " & rslt)
'	Return rslt
'End Function 'readLoc


'**************
'	Helpers
'

Public Shared Function mapPath(ins As String) As String
	Dim rslt As String = ""
	
Try
	If ins = "" Then Return "" 'XX throw ex ?
	
	rslt = _server.mapPath(ins)	
Catch ex As Exception
	XTrace.show("AppHome.mapPath", ex)
End Try

	XTrace.show("AppHome.mapPath", "rslt", rslt)
	Return rslt
End Function


Public Function mapPath2(path As String) As String _
		Implements IAppCenter.mapPath2
	Return mapPath(path)
End Function


'XX mv
Public Shared Function readCategoryPath( _
	category As String _
) As String
	Dim rslt, search As String
	rslt = ""
	
	If category = "" Then Return rslt
	
	Try
		search = "categories/item[@name='" & category & "']/@vpath"
		rslt = SysConfig.getValue(search) 
		'XTrace.show("DoorKeeper.readCategoryPath", "category", 
		'	category, "search path", search, "vpath", rslt)
	Catch ex As Exception
		XTrace.show("DoorKeeper.readCategoryPath", ex)		
	End Try
	
	Return rslt
End Function 'readCategoryPath


' tested
'XX use SysConfig
Public Shared Function allowLogin() As Boolean
	Dim rslt As Boolean = False
	
	Dim bval As String = _
		Util.config_readXmlElementAttribute("/system/login", "enabled")
	rslt = Util.isEnabled(bval)

	Return rslt
End Function


' tested
'XX 
Public Shared Function isAllowRegister() As Boolean
	Dim bval As String = SysConfig.readNodeAttribute("/system/register/@enabled")
	'XTrace.show("Doorkeeper.isAllowRegister", "bval", bval)
	Return Util.isEnabled(bval, True)	
End Function


'??
Public Function getSite() As String _
		Implements IAppCenter.getSite
	Dim rslt As String = SysConfig.getSite()	
	Return rslt	
End Function

End Class 'DoorKeeper

End Namespace


'*******************************************************************************
'

NameSpace zxweb.core
	
'		
Public Class AppHome
	Inherits zx.DoorKeeper
	
' Used by:
'	DocHeader
'
Public Shared Function isTraceMode() As Boolean	
	Dim rslt As Boolean = False
	
	Try
		rslt = SysConfig.getTracer().isTraceEnabled() 
	Catch ex As Exception		
		'Throw
	End Try
	
	Return rslt
End Function


Private Shared s_FactoryResource As IFactoryResource

Public Shared Sub setFactoryResource(ByRef factory As IFactoryResource) 
	s_FactoryResource = factory	
End Sub

Public Shared Function getFactoryResource() As IFactoryResource
	Return s_FactoryResource
End Function


'****************
'	Constants
'
'****************

ReadOnly Public Shared InvalidResourcePart As String = "__invalid_resource_part" 

End Class 'AppHome

End Namespace 