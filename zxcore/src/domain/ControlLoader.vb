
Imports System.Reflection

NameSpace zxweb.domain

'*******************************************************************************
'	Todos:
'		2012.8.11 remove shared
'

Public Class ControlLoader
	
'XX rm static 	
Private Shared _controls As Hashtable

Private Shared _controlListString As String 'raw


'*****************
'	Life cycle
'
'*****************

Private Shared s_loader As ControlLoader

Public Shared Function getInstance() As ControlLoader
	If s_loader Is Nothing Then
		s_loader = New ControlLoader
	End If
	
	Return s_loader
End Function


'***************
'	Services
'
'***************

'!! src maybe changed
'XX merge
'
Public Shared Function getPackage(Byref src As String) As String
	Dim rslt As String = ""
	
	If Util.exist(src, ".") Then
		Dim lasti As Integer = src.LastIndexOf(".")		
		Dim strlen As Integer = src.Length
		
		rslt = src.Substring(0, lasti)
		src = src.Substring(lasti+1£¬ strlen-lasti-1)  
	End If
	
	Return rslt
End Function


'XX 
' /controls/CustomContentControls
'
Private Shared Function readControlList() As String
Try
	If _controlListString <> "" And _controls IsNot Nothing Then 
		XTrace.warn("ControlLoader.readControlList", "Already set, return")
		Return _controlListString
	End If
	
	Dim confile As XmlFile = ControlsConfig.getFile()
	_controlListString = confile.readXml("/controls/CustomContentControls")		
	
	Dim ctrlList As ArrayList = Util.parseValues(_controlListString, ",")	
	
	Dim listCount As Integer = ctrlList.Count
	_controls = New Hashtable(listCount)
	XTrace.show("ControlLoader.readControlList", "To read controls", listCount)
	Dim actualRead As Integer = 0
	
	For i As Integer = 0 To listCount-1
		Try 
			Dim val As String = ctrlList(i)
			Dim iidx As Integer = val.IndexOf(":")
			Dim pridx As Integer = val.IndexOf(".")
			
			If iidx >= 0 Or pridx >= 0 Then
				Dim assem As String = ObjectFactory.getdefaultAssembly()	 
				Dim cls As String = val 
				
				If iidx >= 0 Then 
					If iidx > 0 Then assem = val.Substring(0, iidx) ':
					cls = val.Substring(iidx+1, val.Length-iidx-1)
				End If
				
				Dim package As String = getPackage(cls)				
				Dim ctx As New ControlContext(assem, package)
				
				_controls.Add(cls, ctx) 
				actualRead += 1
				XTrace.show("ControlLoader.readControlList>For", "Added class" & _
					actualRead, cls, "assembly", assem, "package", package)
			Else
				_controls.Add(val, Nothing)
				actualRead += 1
				XTrace.show("ControlLoader.readControlList>For", "Added class" & _
					actualRead, val)
			End If		
		Catch ex As Exception
			XTrace.show("ControlLoader.readControlList>For", ex)
		End Try
	Next	
	
	XTrace.show("ControlLoader.readControlList", "Read " & _
		IIf(listCount = actualRead, "ok", "error"), _
		listCount & "|" & actualRead)
Catch ex As Exception
	XTrace.show("ControlLoader.readControlList", ex)
End Try
	
	Return _controlListString
End Function 'readControlList


'XX exclude <top>, <a>...
' cn: control class 
'
Private Shared Function isControlEnabled(cn As String) As Boolean
	Dim rslt As Boolean = False
	If cn = "" Then Return False 
	
	readControlList()
	If _controls Is Nothing Then 
		Return False ' check here, system error!
	End If
	
	'!! don't use string search, use array search instead.
	If _controls.ContainsKey(cn) Then 
		rslt = True
	End If
	
	XTrace.show("ControlLoader.isControlEnabled", cn, rslt)
'	XTrace.show("ControlLoader.isControlEnabled", "Contains zxweb.PatternList", _
'		_controls.ContainsKey("zxweb.PatternList"))
	Return rslt
End Function

	
Protected Shared Sub addControls( _
	ByRef parent As XmlNode, _
	ByRef nodes As ArrayList _
) 
	If parent Is Nothing Or nodes Is Nothing Then Return
	
	Dim childs As XmlNodeList = parent.ChildNodes	
	
	XTrace.show("ControlLoader.addControls", "Current node", parent.Name)	
	If parent.NodeType = XmlNodeType.Element AndAlso isControlEnabled(parent.Name) Then 
		nodes.Add(parent)
		'XTrace.show("ControlLoader.addControls", "Added")
	Else
		XTrace.warn("ControlLoader.addControls", "Not added")		
	End If
	
	If childs.Count < 1 Then Return
	
	'XTrace.show("ControlLoader.addControls", "Processing childs", childs.Count)		
	For Each element As XmlNode In childs
		If element.NodeType = XmlNodeType.Element AndAlso isControlEnabled(element.Name) Then 
			nodes.Add(element)
			'XTrace.show("ControlLoader.addControls", "Added", element.Name)
		Else
			addControls(element, nodes)
		End If
	Next
End Sub 'addControls


Private Sub initContext( _
	ctrlName As String, _	
	ByRef inContext As ControlContext, _	
	ByRef assembly As String, _
	ByRef package As String _
)
	Dim cctx As ControlContext = inContext		
	If ctrlName = "" Then
		Return
	End If
	
	If cctx Is Nothing Then
		XTrace.warn("ControlLoader.initContext", _
			"No contronl context from parameter, read from own list")
		cctx = _controls.Item(ctrlName) 'XX
	Else
		If cctx.Assembly = "" And cctx.Package = "" Then
			XTrace.warn("ControlLoader.initContext", _
				"Both assembly And package Of the inControlContext empty, " & _
				"read from own list")
			
			Dim cctx2 As ControlContext = _controls.Item(ctrlName) 'XX					
			'					cctx.Assembly = cctx2.Assembly
			'					cctx.Package = cctx2.Package
			cctx = cctx2
		End If
	End If
	
	If cctx IsNot Nothing Then
		XTrace.show("ControlLoader.initContext", _
			"Setting control context")
		assembly = cctx.Assembly
		package = cctx.Package
	Else
		XTrace.warn("ControlLoader.initContext", "Control context null")
	End If
	
	If package = "" Then 
		XTrace.warn("ControlLoader.initContext", _
			"Package empty, set to sys default")
		package = "zx" 'XX
	End If
	
	If assembly = "" Then 
		XTrace.warn("ControlLoader.initContext", _
			"Assembly empty, set to sys default")
		assembly = SysConfig.ZxwebAssembly
	End If	
End Sub 'initContext


'!! might run recursively, controls embedded, e.g. in the homepage.
'
'XX default namespace zx. for controls
'XX pdata: return val
'XX remove containerpage
'
' tn: 
' container: page container
' pdata: must not be null !! tn.innerxml
'
' Used by:
'	TabCard, PlaceHolder, XPage, DataDrivenControlTest, ArticleHandler	
'
Public Shared Function process( _
	ByRef containerPage As ZXPage, _
	ByRef tn As XmlElement, _
	ByRef pdata As String, _
	Optional ByRef parentCtrl As ControlBase = Nothing, _
	Optional ByRef returnedCtrls As ArrayList = Nothing, _ 
	Optional ByRef ctrlctxt As ControlContext = Nothing _
) As Boolean
	Dim cnt As Integer
	Dim nodes As ArrayList
	Dim rslt As Boolean = False
	
	If tn Is Nothing Then
		XTrace.show("ControlLoader.process", "Warning", "Node empty!")
		Return False
	End If

	cnt = tn.ChildNodes.Count
	XTrace.show("ControlLoader.process", "Child nodes found", cnt)
	If cnt > 0 Then 
		nodes = New ArrayList
	Else
		Return rslt	
	End If
	
	Try
		addControls(tn, nodes)
	Catch ex As Exception
		XTrace.show("ControlLoader.process>add", ex)
	End Try
	XTrace.show("ControlLoader.process", "Control nodes added", nodes.Count)
			
	'XTrace.show("ControlLoader.process", "child(" & i & ") Name", tn.ChildNodes(i).Name)
	'XTrace.show("ControlLoader.process", "child(" & i & ") Prefix", tn.ChildNodes(i).Prefix)
	'XTrace.show("ControlLoader.process", "child(" & i & ") Type", tn.ChildNodes(i).NodeType)
	'XTrace.show("ControlLoader.process", "child(" & i & ") isElement", tn.ChildNodes(i).NodeType = XmlNodeType.Element)
	'XTrace.show("ControlLoader.process", "child(" & i & ") IText", tn.ChildNodes(i).InnerText)				

	For i As Integer = 0 To nodes.Count - 1
		Dim ctrl As ControlBase = Nothing
		Dim clsn As String = ""
		Dim assemb As String = ""
		Dim cpackage As String = "" 
		
		Try
			Dim element As XmlElement = nodes(i)						
			Dim elenm As String = element.Name
			XTrace.showRun("ControlLoader.process", "Processing " & elenm)
			
			' Set the control context
			'
			getInstance().initContext(elenm, ctrlctxt, assemb, cpackage)			
			
			'!! case-sensitive?
			clsn = cpackage & "." & elenm 'XX namespace, zxweb or others						
			'assem = [Assembly].GetExecutingAssembly()			
				
			XTrace.show("ControlLoader.process", _ 
					"To create " & clsn & " in assembly '" & assemb & "'")
			ctrl = ObjectFactory.getObject(clsn, assemb) 
			
			If ctrl Is Nothing Then
				XTrace.warn("ControlLoader.process", "Failed to create the control")
			Else
				XTrace.show("ControlLoader.process", "Control created ok")
				
				' set properties		
				Dim attribs As XmlAttributeCollection = element.Attributes			
				XTrace.show("ControlLoader.process", "Found properties", attribs.Count)
				
				' Get the Type object
				Dim typestr As String = IIf(assemb = "", clsn, clsn & "," & assemb)
				Dim mytype As Type = Type.GetType(typestr)			
				XTrace.show("ControlLoader.process", "Typestr", typestr)
				XTrace.checkNull("ControlLoader.process", "Returned type object", mytype)
				
				For Each attr As XmlAttribute In attribs		
					Dim pio As PropertyInfo = Nothing
					
					Try	
						'XX case-sensitive!
						pio = mytype.GetProperty(attr.Name)			
						
						If pio Is Nothing Then
							pio = mytype.GetProperty(attr.Name, _
								BindingFlags.Instance Or BindingFlags.Public _
								Or BindingFlags.IgnoreCase)
						End If
						' pio.GetValue(Myproperty, Nothing).ToString())		
						'XTrace.show("ControlLoader.process", "Process Property", attr.Name)
						
						Dim attribval As String = element.GetAttribute(attr.Name)
						XTrace.show("ControlLoader.process>property", _
							"Property value", attribval)
						
						pio.SetValue(ctrl, attribval, Nothing)
					Catch ex As Exception
						'XX AmbiguousMatchException, ArgumentNullException						
						XTrace.show("ControlLoader.process>property", ex)
						XTrace.show("ControlLoader.process>property>ex", _
							"Type", typestr, _
							"Attribute", attr.Name)
						
						XTrace.checkNull("ControlLoader.process>property>ex", _
							"PropertyInfo", pio)
					End Try
				Next
			End If 'ctrl

			If containerPage IsNot Nothing Then
				ctrl.setPageContainer(containerPage)	
				ctrl.setXSession(containerPage.getXSession()) 'added 2012-9-11
			End If
			
			ctrl.setContainer(parentCtrl)
			ctrl.setEmbeddedInXml(True)		
			ctrl.setData(element.InnerXml)	
			ctrl.runtimeMode = "custom" '!!
			
			If ctrlctxt IsNot Nothing Then ' in page mode
				ctrl.setContext(ctrlctxt)
			End If
				
			' run
			XTrace.showRun("ControlLoader.process", "Load")
			ctrl.loadControl() 
			
			XTrace.showRun("ControlLoader.process", "Render")
			ctrl.invokeRenderDelegate()
			
			Dim eox As String = element.OuterXml
			
			If Not ctrl.isUseDefaultRender() Then
				XTrace.showRun("ControlLoader.process", "Get the outputs")
				'!! might run recursively when xcontrol embedded.
				Dim rtn As String = ctrl.genOutput(True, False) '!!	run just once, flush		
				
				If rtn = "" AndAlso Not ctrl.isHideEmptyTip() Then 
					rtn = "#" & clsn & "#" '"#control#" 'XX conf
				End If
				XTrace.show("ControlLoader.process", "Run result of the control", rtn)
				
				pdata = pdata.Replace(eox, rtn) '!!
				'XTrace.show("ControlLoader.process", "eox", eox, "after", pdata)
			Else
				pdata = pdata.Replace(eox, "")
			End If
			'XTrace.show("ControlLoader.process", "Final", pdata)
			
			If returnedCtrls IsNot Nothing Then returnedCtrls.Add(ctrl)
		Catch ex As Exception
			XTrace.show("ControlLoader.process>control", ex)
			
			'Failed to load
			If ctrl Is Nothing Then pdata = "#" & clsn & "#"
		End Try
	Next	
	
	rslt = True 'XX
	XTrace.showEnd("ControlLoader.process")
	Return rslt
End Function 'process	


'?? is parentCtrl must
Public Shared Function process( _
	ByRef srcNode As XmlNode, _	
	Optional ByRef parentCtrl As ControlBase = Nothing, _
	Optional ByRef returnedCtrls As ArrayList = Nothing, _ 	
	Optional ByRef containingPage As ZXPage = Nothing, _
	Optional ByRef ctrlctxt As ControlContext = Nothing _
) As String
	If srcNode Is Nothing Then Return ""
	
	Dim rslt As String = srcNode.InnerXml 'original
	process(containingPage, srcNode, rslt, parentCtrl, returnedCtrls, ctrlctxt)
	
	Return rslt
End Function


'!!
'XX
Public Shared Function render( _
	ByRef ctrl As ControlBase, _
	Optional ByRef container As ControlBase = Nothing, _
	Optional isEnableWrite As Boolean = True _
) As String
	Dim rslt As String = ""		
	If ctrl Is Nothing Then Return "" 'XX excep?
	
	ctrl.enableWriterBuffer = "true"
	ctrl.setContainer(container)
	ctrl.setEnableWrite(isEnableWrite)
	
	XTrace.showRun("ControlLoader.render", "load")
	ctrl.loadControl()
	
	XTrace.showRun("ControlLoader.render", "render")
	ctrl.invokeRenderDelegate()
	
	XTrace.showRun("ControlLoader.render", "gen output")
	rslt = ctrl.genOutput(True, False) 'Flush, not run again
	Return rslt
End Function 'render

End Class 'ControlLoader

End Namespace