
Namespace zxweb.data

'*******************************************************************************
'

Public Class CacheFile
	Inherits XmlFile

'******
' 	Constructors 
'
'*******

Public Sub New(fname As String) 
'Note: can't put Try clause here, first statement must be New()
	MyBase.New(fname)
End Sub


Public Shared Shadows Function loadReady(fullpath As String) As CacheFile
	Dim rslt As CacheFile

Try	
	If fullpath = "" Then Return Nothing
	
	'XTrace.show("XmlFile.loadReady", "fullpath", fullpath)
	rslt = New CacheFile(fullpath)	
	rslt.load()
	
Catch ex As Exception
	XTrace.show("CacheFile.loadReady", ex)
	rslt = Nothing '!! tricky
	If Not createEmptyFile(fullpath) Then
		Throw ex 'XX 2007-9-8 ??
	End If
End Try

	Return rslt
End Function 'loadReady


'!! no save 
'
Protected Function fetchBlock(block As String) As XmlElement
	Dim root, blockElem As XmlElement

	If block = "" Then Return Nothing
	
	root = _doc.DocumentElement
	blockElem = _doc.SelectSingleNode("/root/" & block)
	If blockElem Is Nothing Then
		blockElem = _doc.CreateElement(block)  
		root.insertAfter(blockElem, Nothing)	
	End If
	
	Return blockElem
End Function


Public Function insertItem( _
	block As String, _
	id As String _
) As Boolean
	Dim rslt As Boolean = False
	Dim itemElem, blockElem As XmlElement
	Dim idAttrib As XmlAttribute
	
	If block = "" Or id = "" Then Return False
	
Try	
	SyncLock GetType(CacheFile)		
		blockElem = fetchBlock(block)
		
		itemElem = _doc.SelectSingleNode("/root/" & block & "/item[@id='" & id & "']")
		If itemElem Is Nothing Then
			itemElem = _doc.CreateElement("item")  		
			idAttrib = _doc.CreateAttribute("id")
			idAttrib.Value = id 
			itemElem.SetAttributeNode(idAttrib)			
			
			blockElem.insertAfter(itemElem, Nothing)	
		End If
				
		save()
		rslt = True	
	End SyncLock
Catch ex As Exception
	XTrace.show("CacheFile.insertItem", ex)
End Try
	
	Return rslt
End Function


Public Function insertItemProperty( _
	block As String, _
	id As String, _
	tag As String, _
	val As String _
) As Boolean
	Dim rslt As Boolean = False
	Dim itemElem, propertyElem As XmlElement
	itemElem = Nothing : propertyElem = Nothing
	Dim spath As String

	If block = "" Or id = "" Or tag = "" Then Return False
	
Try	
	spath = "/root/" & block & "/item[@id='" & id & "']"
	SyncLock GetType(CacheFile)		
		itemElem = _doc.SelectSingleNode(spath)
		If itemElem Is Nothing Then
			'XTrace.show("CacheFile.insertItemProperty", "")
			insertItem(block, id)
			'!! read again
			itemElem = _doc.SelectSingleNode(spath)
		End If
		
		propertyElem = _doc.SelectSingleNode(spath & "/" & tag)
		If propertyElem Is Nothing Then
			propertyElem = _doc.CreateElement(tag) 'create new element
			itemElem.insertAfter(propertyElem, Nothing)	
		End If
				
		propertyElem.InnerText = val
		save()
		rslt = True	
	End SyncLock
Catch ex As Exception
	'XTrace.show("CacheFile.insertItemProperty", "block", block, "item", id, "property", tag, "value", val)
	XTrace.show("CacheFile.insertItemProperty", "itemElem empty", itemElem Is Nothing, "propertyElem", propertyElem Is Nothing)
	XTrace.show("CacheFile.insertItemProperty", ex)
End Try	
	
	Return rslt 
End Function 'insertItemProperty


Public Sub setBlockCount( _
	block As String, _
	count As Integer _
) 		
	setBlockAttribute(block, "count", count)	
End Sub


'XX
Public Function readBlockCount(block As String) As Integer
	Dim rslt As Integer = -1
	
Try
	If block = "" Then Return -1
	rslt = CInt(readNodeAttribute("/root/" & block & "/@count"))
Catch ex As Exception
	rslt = -1	
End Try

	Return rslt
End Function


Public Function isObsolete(block As String) As Boolean
	Dim rslt, isExist As Boolean
	Dim state As String
	
	If block = "" Then Return False '!! no change
	
	state = readNodeAttribute("/root/" & block & "/@state", isExist)	
	If Not isExist OrElse String.Compare(state, "obsolete", True) = 0 Then
		rslt = True
	Else
		rslt = False
	End If
	
	Return rslt
End Function 'isObsolete


Public Sub setFresh(block As String)
	setBlockAttribute(block, "state", "fresh")
	setBlockAttribute(block, "createTime", DateTime.Now.ToString())
		
End Sub


Public Sub setObsolete(block As String)
	XTrace.show("CacheFile.setObsolete", "target", block)
	setBlockAttribute(block, "state", "obsolete")
End Sub


Protected Sub setBlockAttribute( _
	block As String, _
	attrib As String, _
	value As String _
)
	Dim blockElem As XmlElement
	Dim attribNode As XmlAttribute

Try	
	SyncLock GetType(CacheFile)		
		blockElem = fetchBlock(block)
		
		attribNode = _doc.CreateAttribute(attrib)
		attribNode.Value = value
		blockElem.SetAttributeNode(attribNode)			
		
		save()
	End SyncLock
Catch ex As Exception
	XTrace.show("CacheFile.setBlockAttribute", ex)
End Try	

End Sub 'setBlockAttribute

End Class 'CacheFile

End NameSpace