
Imports zx.common ' Keep for TraceFilter
Imports zxweb.trace

NameSpace zxweb.core

'*******************************************************************************
'	The system configuration. 
'

Public Class SysConfig
	
Private Shared _bootFile As XmlFile	
Private Shared Function getBootFile() As XmlFile
	If _bootFile Is Nothing OrElse _bootFile.isUpdated() Then	
		Dim path As String = "/bin/zxboot.xml" '!!
		_bootFile = XmlFile.load2(path) 'XX
	End If 
	
	Return _bootFile
End Function


Private Shared _xfile As XmlFile
'XX ref locations
Private Shared Function getConfigFile() As XmlFile
	If _xfile Is Nothing OrElse _xfile.isUpdated() Then		
		_xfile = XmlFile.loadReady(getFullConfigFilePath()) 
	End If 
	
	Return _xfile
End Function


'!!
Private Shared _rootTag As String = "/conf_root/"


'XX
Public Shared Function getConfigPath() As String
	Return getBootFile().readInnerText("/root/SysConfig/path")
End Function

'XX ex
Public Shared Function getFullConfigFilePath() As String
	Dim confile As String = getConfigPath() & "/" & _
		getBootFile().readInnerText("/root/SysConfig/mainfile")
	
	Return AppHome.mapPath(confile) 
End Function

Public Shared Function getResourcesFilePath() As String
	Return getConfigPath() & "/" & _
		getBootFile().readInnerText("/root/SysConfig/resources")
End Function

Public Shared Function getControlsFilePath() As String
	Return getConfigPath() & "/" & _
		getBootFile().readInnerText("/root/SysConfig/controls")
End Function

Public Shared Function getActionsFilePath() As String
	Return getConfigPath() & "/" & _
		getBootFile().readInnerText("/root/SysConfig/actions")
End Function

Public Shared Function getMembersFilePath() As String
	Return getConfigPath() & "/" & _
		getBootFile().readInnerText("/root/SysConfig/members")
End Function


'XX
Public Readonly Shared NameSpaceTabs As String = "zxweb.tabs"

Public ReadOnly Shared ZxwebAssembly As String = "zxcontrols"


'***************
'	Services 
'
'***************

Public Shared Function isDirty() As Boolean
	If _xfile Is Nothing Then Return True
	
	Return _xfile.isUpdated()
End Function


Public Shared Function getImgHome() As String
	Return readNode("etc/img/home")	
End Function


'!! path: without leading '/'
'
Public Shared Function readNode( _
	relpath As String, _
	Optional isText As Boolean = False, _
	Optional isReturnEx As Boolean = False _
) As String
	Dim rslt As String = ""	
	
	'XTrace.showRun("SysConfig.readNode")
	If relpath = "" Then Return ""	
	
	Try
		Dim fullpath As String = _rootTag & relpath
		'XTrace.show("SysConfig.readNode", "fullpath", fullpath, _
		'	"isText", isText)
		
		If isText Then
			rslt = getConfigFile().readInnerText(fullpath)	
		Else
			rslt = getConfigFile().readXml(fullpath)	
		End If		
	Catch ex As Exception		
		XTrace.show("SysConfig.readNode", ex)
		
		If isReturnEx Then
			rslt = "#ex#"
		End If
	End Try	
	
	'XTrace.show("SysConfig.readNode", "rslt", rslt)	
	Return rslt
End Function 'readNode


'XX attribute@
Public Shared Function readNodeAttribute(path As String) As String
	Dim rslt As String	
	If path = "" Then Return ""
	
	rslt = getConfigFile().readNodeAttribute(_rootTag & path)	
	Return rslt
End Function


Public Shared Function getValue(path As String) As String
	Dim rslt As String = readNodeAttribute(path)	
	Return rslt
End Function


'XX useful?
Public Shared Function getSite() As String
	Dim rslt As String = readNode("site")
	Return rslt
End Function


'**************
'	Tracing
'

'XX mvto AppHome

Private Shared s_traceManager As ITraceManager

Public Shared Function getTracer() As ITraceManager
	Return s_traceManager	
End Function

Public Shared Sub setTraceManager(ByRef tman As ITraceManager)
	s_traceManager = tman
End Sub

' Used by:
'	XTrace
'
'XX empty
Public Shared Function readTraceFilter() As TraceFilter
	Dim rslt As TraceFilter	
	Dim includes As String = ""
	Dim excludes As String = ""
	
	Dim useConfigedFilter As Boolean = _
		Util.isEnabled(readNodeAttribute("system/trace/@useFilterHere"))
	
	If s_traceManager Is Nothing OrElse useConfigedFilter Then
		includes = readNodeAttribute("system/trace/@includes")
		excludes = readNodeAttribute("system/trace/@excludes")
	Else
		includes = s_traceManager.getFilter("includes")	
		excludes = s_traceManager.getFilter("excludes")	
	End If
	
	rslt = New TraceFilter(includes, excludes)
	Return rslt
End Function


Public Shared Function getFilterString() As String
	Dim filter As TraceFilter = readTraceFilter()
	Dim rslt As String = filter.ToString()	
	
	Return rslt
End Function


'XX Obsolete
'
'?? need to read every time
Public Shared Function readTraceStateFromFile() As String
	Dim rslt As String = readNodeAttribute("system/trace/@state")	
	Return rslt
End Function

Private Shared s_traceMode As String = "disabled"

' unused
Public Shared Sub setTraceMode(ins As String)	
	s_traceMode = ins
End Sub

Public Shared Function getTraceMode() As String
	Dim rslt As String = s_traceMode
	
	Dim isConfigSource As Boolean = _
		Util.isEnabled(readNodeAttribute("system/trace/@useFilterHere"))

	If s_traceManager IsNot Nothing AndAlso Not isConfigSource Then
		rslt = s_traceManager.getTraceMode()
	Else
		' when wt corrupted
		rslt = readNodeAttribute("system/trace/@mode")
	End If
		
	Return rslt
End Function


'**************
'	Comment
'

Public Shared Function getCommentFilePostfix() As String
	Dim rslt As String = readNode("comments/repository/postfix")
	Return rslt
End Function


'!! don't leak the path
Public Shared Function getCommentRepositoryPath() As String
	Dim rslt As String = _
		Util.config_readXmlElementAttribute("/comments/repository", "dir")
	Return rslt ' "/_reserved/comments/"
End Function

Public Shared Function getCommentRepositoryPathPhysical() As String
	Return DoorKeeper.mapPath(getCommentRepositoryPath())
End Function


'XX always gen new empty file
'
Public Shared Function readCommentFilePath(sname As String) As String
	Dim rslt, fname As String	
	If sname = "" Then Return ""
	rslt = ""
	
Try	
	Dim xresrc As IWebResource = _
		AppHome.getFactoryResource().createInstance(sname)
	
	fname = xresrc.getProperty("path_comments")

	'Util.trace("DoorKeeper.readCommentFilePath", "sname: " & sname)
	If fname = "" Then 
	' Default use sname
	' XX 
		Dim templ As String = xresrc.getProperty("template")		
		'XX
		If templ <> "qnas" Then
			rslt = getCommentRepositoryPath() & sname & ".comments.xml"
		End If
	Else
	' designated by config
		rslt = getCommentRepositoryPath() & fname & ".comments.xml"	
	End If	
Catch ex As Exception
	rslt = ""
	XTrace.show("DoorKeeper.readCommentFilePath", ex)
End Try

	'Util.trace("DoorKeeper.readCommentFilePath", "loc: " & rslt)
	Return rslt
End Function 'readCommentFilePath

'XX
Public Shared Function readCommentFilePathPhysical(sname As String) As String	
	Return DoorKeeper.mapPath(readCommentFilePath(sname))
End Function


'***********
'	Qnas
'

Public Shared Function getQnasRepositoryPath() As String
	Dim rslt As String = readNodeAttribute("qnas/repository/@dir")
	
	'!! identical to comments: 
	'XX r = Util.config_readXmlElementAttribute("/qnas/repository", "dir") 	
	'XTrace.show("DoorKeeper.getQnasRepositoryPath", "qnas path", rslt)
	Return rslt ' "/_reserved/qnas/"
End Function


Public Shared Function getQnasRepositoryPathPhysical() As String
	Return DoorKeeper.mapPath(getQnasRepositoryPath())
End Function


Public Shared Function getQnasFilePostfix() As String
	Dim rslt As String = readNode("qnas/repository/postfix") 
	Return rslt
End Function


'XX move
Public Shared Function readQnasFilePathPhysical(sname As String) As String
	Dim rslt, fname As String 	
	If sname = "" Then Return ""
	rslt = ""
	
Try	
	Dim xresrc As IWebResource = _
		AppHome.getFactoryResource().createInstance(sname)

	fname = xresrc.getProperty("filename")

	'XX search in LDB ?
	If fname = "" Then 
	' Default use sname
		rslt = DoorKeeper.mapPath("/_reserved/qnas/" & sname & ".qnas.xml")
	Else
		rslt = DoorKeeper.mapPath("/_reserved/qnas/" & fname & ".qnas.xml")
	End If	
Catch ex As Exception
	rslt = ""
End Try
	'Util.trace("DoorKeeper.readCommentFilePath", "loc: " & rslt)
	Return rslt
End Function 'readQnasFilePathPhysical


Public Shared Function getJsHome() As String
	Dim rslt As String = readNode("js/home", True) 			
	Return rslt	
End Function


Public Shared Function getTesterHome() As String
	Dim rslt As String = readNode("tester/home")		
	Return rslt
End Function


'XX
Public Shared Function getZxwebHome() As String
	XTrace.showRun("SysConfig.getZxwebHome")	
	Dim rslt As String = "/"	
	Dim bootfile As XmlFile = getBootFile()	
	
	If bootfile IsNot Nothing Then 
		XTrace.show("SysConfig.getZxwebHome", _
			"bootfile", bootfile.getRelFilePath())
		Dim tmp As String = bootfile.readInnerText("/root/zxweb/home") 
		'XX conf_root
		
		If tmp <> "" Then 
			rslt = tmp
		End If
	Else
		XTrace.warn("SysConfig.getZxwebHome", "bootfile is null")
	End If
	
	XTrace.show("SysConfig.getZxwebHome", "rslt", rslt)
	Return rslt
End Function


Public Shared Function getViewHome() As String
	Dim rslt As String = readNode("views/home")		
	Return rslt		
End Function

Public Shared Function getViewPrefix() As String
	Dim rslt As String = readNode("views/prefix")		
	Return rslt		
End Function


Public Shared Function isXTagEnabled() As Boolean
	Dim rslt As Boolean = Util.isEnabled(readNode("system/xtag/enabled"))
	Return rslt
End Function

End Class 'SysConfig

End Namespace