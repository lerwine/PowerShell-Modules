$Script:SiteUrl = 'http://{0}/_vti_bin/SiteData.asmx?WSDL' -f $env:COMPUTERNAME;
$WebRequest = [System.Net.WebRequest]::Create($Script:SiteUrl);
$WebRequest.UseDefaultCredentials = $true;
$WebResponse = $WebRequest.GetResponse();
$Stream = $WebResponse.GetResponseStream();
$Script:Namespaces = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
    soap = 'http://schemas.xmlsoap.org/wsdl/soap/';
    tm = 'http://microsoft.com/wsdl/mime/textMatching/';
    soapenc = 'http://schemas.xmlsoap.org/soap/encoding/';
    mime = 'http://schemas.xmlsoap.org/wsdl/mime/';
    tns = 'http://schemas.microsoft.com/sharepoint/soap/';
    xsd = 'http://www.w3.org/2001/XMLSchema';
    xsi = 'http://www.w3.org/2001/XMLSchema-instance';
    soap12 = 'http://schemas.xmlsoap.org/wsdl/soap12/';
    http = 'http://schemas.xmlsoap.org/wsdl/http/';
    wsdl = 'http://schemas.xmlsoap.org/wsdl/';
};
$XmlReader = [System.Xml.XmlReader]::Create($Stream);
$XmlDocument = New-Object -TypeName 'System.Xml.XmlDocument';
$XmlDocument.Load($Stream);
$XmlReader.Close();
$Stream.Dispose();
$WebResponse.Dispose();
$XmlNamespaceManager = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $XmlDocument.NameTable;
$Script:Namespaces | Get-Member -MemberType NoteProperty | ForEach-Object {
    $XmlNamespaceManager.AddNamespace($_.Name, $Script:Namespaces.($_.Name));
}

$XmlSchemaSet = New-Object -TypeName 'System.Xml.Schema.XmlSchemaSet';
$XmlDocument.CreateNavigator().
$XmlDocument.DocumentElement