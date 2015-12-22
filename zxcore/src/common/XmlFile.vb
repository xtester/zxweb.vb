
Namespace zx.common.xml

'*******************************************************************************
'	Todos:
'		- remove or delete node
'

Public Class XmlFile
	
'XX	priv
Protected _filepath As String ' physcial/global, with name?

Public Function getAbsFilePath() As String
	XTrace.show("XmlFile.getAbsFilePath", "_filepath", _filepath)	
	Return _filepath
End Function

' Set by load2()
Private _relFilePath As String 
Public Function getRelFilePath() As String
	Return _relFilePath	
End Function


Private _loadTime As DateTime

Private _rootTag As String = "root"
Public Function getRootPath() As String
	Return "/" & _rootTag & "/" 'XX
End Function


'***************** 
'	Life cycle 
'
'*****************

Protected Sub New(fname As String) 	
	_filepath = fname ' Absolute path
	_doc = Nothing
End Sub


' By default, a new file will not be created when the target does not exist.
'
'?? create directory if not existed
'XX ex
'
Public Shared Function load2( _
	relpath As String, _
	Optional isDefaultCreat As Boolean = False _
) As XmlFile
	If relpath = "" Then Return Nothing	
	
	Dim absPath As String = AppHome.mapPath(relpath)
	If (Not File.Exists(absPath)) AndAlso Not isDefaultCreat Then
		Return Nothing
	End If
	
	Dim rslt As XmlFile = loadReady(absPath) 'XX
	rslt._relFilePath = relpath
	
	Return rslt
End Function

'XX merge, avoid use
Public Shared Function loadReady(fullpath As String) As XmlFile
	Dim rslt As XmlFile

Try	
	If fullpath = "" Then	
		XTrace.warn("XmlFile.loadReady", "path empty") 'XX 2014-7-2
		Return Nothing
	End If
	
	rslt = New XmlFile(fullpath)	
	rslt.load()	
Catch ex As Exception
	XTrace.show("XmlFile.loadReady", ex)
	XTrace.show("XmlFile.loadReady>Exception", "path", fullpath)
	
	rslt = Nothing	
	Throw ex 'XX 2007-9-8
End Try

	Return rslt
End Function 'loadReady


Protected Shared Function createEmptyFile(fpath As String) As Boolean
	Dim rslt As Boolean = True

Try	
	If fpath = "" Then Return False
	
	XTrace.show("XmlFile.createEmptyFile", "create empty xmlfile", fpath)
	
	SyncLock GetType(XmlFile)		
		Dim fs As StreamWriter
		
		fs = File.createText(fpath)
		'XX conf
		fs.writeLine("<?xml version='1.0' encoding='utf-8'?>")
		'XX other fields
		fs.writeLine("<root>") 'XX conf
		fs.WriteLine("</root>")
		
		fs.Flush()
		fs.Close()
	End	SyncLock 	
Catch ex As Exception
	XTrace.show("XmlFile.createEmptyFile", ex)
	rslt = False
End Try

	Return rslt
End Function 'createEmptyFile


'?? What if load error
'XX make priv and merge, avoid use
'
Public Overridable Function load() As Boolean
Try	
	'Util.trace("XmlFile.load", "run ...")
	
	'XX why always create ?
	If Not File.exists(_filepath) Then
		createEmptyFile(_filepath)
	End If
	
	If File.exists(_filepath) Then		
		'!! trace here may cause server crash.
		'XTrace.show("XmlFile.load", "File " & _filepath & " exists, loading...")
		_doc = New XmlDocument
		_doc.Load(_filepath)		
	Else
		'XX
	End If
	
	_loadTime = File.GetLastWriteTime(_filepath)
	
	If _doc Is Nothing Then
		Return False
	Else
		Return True
	End If	
Catch ex As Exception
	XTrace.show("XmlFile.load", "File", _filepath)
	XTrace.show("XmlFile.load", ex)
	Throw ex 'XX 2007-9-8
End Try

	Return False	
End Function 'load


'**************
'	Helpers
'
'**************

'XX priv
Protected _doc As XmlDocument

'XX
Public Function getDoc() As XmlDocument
	Return _doc
End Function


'@@ tested
Public Function getFileName() As String
	If _filePath = "" Then Return ""

	Dim fn As String = ""

Try	
	Dim fp As String = _filepath	
	Dim slen As Integer = fp.Length()
	Dim si As Integer 
	
	si = fp.LastIndexOf("/")	
	If si < 0 Then
		si = fp.LastIndexOf("\")
	End If
	If si >= 0 Then
		fn = fp.Substring(si+1, slen-1-si) 
	End If
Catch ex As Exception
	fn = ""
	'Util.trace("XmlFile.getFileName", ex)
End Try
	
	Return fn
End Function


'@@ tested
'FIXME rename
Public Function isValid() As Boolean
	If Not File.exists(_filepath) Then
		Return False
	Else
		Return True
	End If
End Function


'@@ tested
'XX sync
Public Sub setInnerText(path As String, val As String)
	Dim targetnode As XmlElement = getElement(path)
	
	If targetnode Is Nothing Then 
		XTrace.warn("XmlFile.update", "Node null " & path)
		Return
	End If
	
	targetnode.InnerText = val 'XX
	save()
End Sub 'update


'XX Sync?
Public Sub save()
	_doc.Save(_filepath)
End Sub


'*************** 
'	Services 
'
'***************

'@@ tested
'XX
Public Function readInnerText(path As String) As String
   	Dim node As XmlNode = Nothing
	Dim rslt As String = ""
	If path = "" Then Return ""
	
Try	
	node = _doc.SelectSingleNode(path)
	
	If node IsNot Nothing Then
		'!! A node may contain one or more childnodes.
		rslt = node.ChildNodes(0).InnerText 'XX #text
	End If
Catch ex As Exception
	XTrace.show("XmlFile.readInnerText", ex)
	XTrace.show("XmlFile.readInnerText>ex", "path", path)
	XTrace.checkNull("XmlFile.readInnerText>ex", "node", node)
End Try

	Return rslt
End Function


'@@ tested
Public Function existNode( _
	path As String _
) As Boolean
	Dim node As XmlElement
	Dim rslt As Boolean = False
	If path = "" Then Return rslt
	
Try
	node = _doc.SelectSingleNode(path)
	
	If node IsNot Nothing Then
		rslt = True
	End If
Catch ex As Exception
	XTrace.show("XmlFile.existNode", ex)	
End Try

	Return rslt
End Function 'existNode


' Return: 
'	False - not created.
'
Public Function createNode( _
	nodenm As String, _
	Optional loc As String = "/root", _
	Optional condition As String = "" _
) As XmlElement
	Dim rslt As XmlElement = Nothing
	If nodenm = "" Then Return rslt
	
	rslt = getElement(loc & "/" & nodenm & condition)
	If rslt IsNot Nothing Then Return rslt 
	
	SyncLock GetType(XmlFile) 'FIXME don't use GetType
		Try
			Dim whereElement As XmlElement = _doc.SelectSingleNode(loc)
			Dim newElement As XmlElement = _doc.CreateElement(nodenm)  
			
			whereElement.AppendChild(newElement)
			
			save()
			
			rslt = newElement
		Catch ex As Exception
			XTrace.show("XmlFile.createNode", ex)
			XTrace.show("XmlFile.createNode", "Where", loc, "To add node", nodenm)
			rslt = Nothing
		End Try		
	End SyncLock
	
	Return rslt
End Function 'createNode


'@@ tested 
'
Public Function createChildNode( _
	ByRef parent As XmlElement, _
	nodeName As String, _
	Optional val As String = "", _
	Optional isAllowMultiple As Boolean = False _
) As XmlElement
	Dim rslt As XmlElement = Nothing
	If nodeName = "" Or parent Is Nothing Then Return rslt
	
	rslt = parent.SelectSingleNode(nodeName)	
	If rslt Is Nothing Or isAllowMultiple Then
		rslt = _doc.CreateElement(nodeName)		
	End If	
	
	If rslt IsNot Nothing Then
		parent.AppendChild(rslt)
		
		If val <> "" Then
			rslt.InnerText = val	
		End If		
	End If
	
	Return rslt
End Function 'createChildNode
	
	
'@@ tested
'
' Read the XML content of a node, returns inner or outer XML.
'
' XX Return "" if node invalid or containment is nothing/""
'
Public Function readXml( _
	path As String, _
	Optional rmode As String = "Inner" _
) As String
	Dim rslt As String = ""
	Dim node As XmlElement
	If path = "" Then
		Return ""	
	End If
	
Try
	' Util.trace("XmlFile.readNode", "path: " & path)
	node = _doc.SelectSingleNode(path)
	
	If Not node Is Nothing Then
		If rmode = "Outer" Then
			rslt = node.OuterXml
		Else
			rslt = node.InnerXml
		End If
	End If
Catch ex As Exception
	XTrace.show("XmlFile.readNodeXml", ex)	
End Try
	
	Return rslt
End Function


'@@ tested
Public Function readNodes(path As String) As XmlNodeList
	Dim rslt As XmlNodeList = Nothing

Try
	rslt = getDoc().SelectNodes(path)
Catch ex As Exception
End Try

	Return rslt
End Function


'****************
'	Attribute
'

'@@ tested
'
' nodePath: relative, without lead /
'
Public Function readAttribute(nodePath As String, inkey As String) As String
	Dim rslt As String = ""
	If inkey = "" Then Return ""
	
	rslt = readNodeAttribute(getRootPath() & nodePath & "/@" & inkey)
	Return rslt
End Function


'@@ tested
'
' Used by: PlainHeadline
'
'XX merge
Public Shared Function readAttribute( _
	ByRef node As XmlElement, _
	inkey As String _
) As String
	Dim rslt As String = ""
	Dim attrib As XmlAttribute
	
	If node Is Nothing OrElse inkey = "" Then Return ""
	
Try
	attrib = node.Attributes.ItemOf(inkey)
	rslt = attrib.Value
Catch ex As Exception	
	XTrace.show("XmlFile.readAttribute~node", ex)	
End Try
	
	Return rslt
End Function


' path: should contain @...
'
'XX	merge
'
Public Function readNodeAttribute( _
	path As String, _
	Optional ByRef isExist As Boolean = False _
) As String
	Dim rslt As String = ""

Try
	Dim node As XmlAttribute = _doc.SelectSingleNode(path)
	
	If Not node Is Nothing Then
		rslt = node.Value
		isExist = True
	End If
Catch ex As Exception	
	XTrace.show("XmlFile.readNodeAttribute", ex)	
End Try
	
	Return rslt
End Function


' Helper
'
' attrib: without @
'
Public Function readNodeAttribute( _
	path As String, _
	attrib As String, _
	Optional ByRef isExist As Boolean = False _
) As String
	If attrib = "" Then Return ""

	Return readNodeAttribute(path & "/@" & attrib, isExist)
End Function


' rslt: might be also empty when not exist
'
'XX
Public Function readNodeAttribute( _
	ByRef node As XmlNode, _
	attrib As String, _
	Optional ByRef isExist As Boolean = False _
) As String
	Dim rslt As String = ""
	Dim nodeAttrib As XmlAttribute

Try
	nodeAttrib = node.SelectSingleNode("@" & attrib)
	
	If Not nodeAttrib Is Nothing Then
		rslt = nodeAttrib.Value
		isExist = True
	End If
Catch ex As Exception	
End Try
	
	Return rslt
End Function


'@@ tested
'
' fresh?
'!! xtrace may cause infinite looping
'
Public Function isUpdated() As Boolean
'	XTrace.showRun("XmlFile.isUpdated")	
	Dim newtime As DateTime

Try	
	newtime = File.GetLastWriteTime(_filepath)
'	XTrace.show("XmlFile.isUpdated", "_filepath", _filepath, "_loadTime", _
'		_loadTime.ToString(), "newtime", newtime.ToString())
	
	If DateTime.Compare(_loadTime, newtime) < 0 Then
		Return True
	End If
Catch ex As Exception
	XTrace.show("XmlFile.isUpdated", ex)
End Try
	
	Return False
End Function 'isUpdated


'@@ tested
Public Function getElement(path As String) As XmlElement
	Dim rslt As XmlElement
	
Try	
	If path = "" OrElse Not load() Then 
		Return Nothing
	End If
	
	rslt = _doc.SelectSingleNode(path)	
Catch ex As Exception
	XTrace.show("XmlFile.getElement", ex)
	rslt = Nothing
End Try	

	Return rslt
End Function 'getElement

End Class 'XmlFile

End Namespace