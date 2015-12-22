
NameSpace zxweb.core
	
'*******************************************************************************
'	

Public Interface IAppCenter
		
	ReadOnly Property MEMBDB As DB
	
	ReadOnly Property SYSPDB As DB

	Property BOOKDB As DB
	
	Property PATNDB As DB
	
	Property REFSDB As DB

	
	Function getMetaProvider() As IMetaProvider
	
	'!! Show the result publicly may disclose the absolute server path, 
	'	security risk.
	Function mapPath2(path As String) As String    	

	'??
	Function getSite() As String
	
End Interface 'IAppCenter

End NameSpace