
NameSpace zxweb

'*******************************************************************************
'

Public MustInherit Class PostControlBase
	Inherits ControlBase
		
Protected Overrides Sub OnInit(ea As EventArgs)
Try	
	MyBase.OnInit(ea)	
	MyBase.initControl()		
	setUseWriteBuffer(True) 
Catch ex As Exception
	XTrace.show("PostControlBase.OnInit", ex)			
End Try
End Sub	

End Class

End Namespace