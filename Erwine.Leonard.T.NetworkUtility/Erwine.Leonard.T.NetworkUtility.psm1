Function New-WebRequest {
    [CmdletBinding(DefaultParameterSetName = 'PSCredential')]
    [OutputType([System.Net.WebRequest])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Uri]$Uri,

        [System.Net.Cache.RequestCacheLevel]$CachePolicy,

        [string]$Method,

        [string]$ConnectionGroupName,

        [Hashtable]$Headers,

        [bool]$PreAuthenticate,

        [ValidateRange(0, 2147483647)]
        [int]$Timeout,

        [System.Net.Security.AuthenticationLevel]$AuthenticationLevel,

        [System.Security.Principal.TokenImpersonationLevel]$ImpersonationLevel,

        [Parameter(ParameterSetName = 'PSCredential')]
        [System.Management.Automation.PSCredential]$PSCredential,

        [Parameter(Mandatory = $true, ParameterSetName = 'ICredentials')]
        [System.Net.ICredentials]$Credentials,

        [Parameter(Mandatory = $true, ParameterSetName = 'UseDefaultCredentials')]
        [switch]$UseDefaultCredentials
    )

    Process {
        $WebRequest = [System.Net.WebRequest]::Create($Uri);
        if ($WebRequest -ne $null) {
            $WebRequest.UseDefaultCredentials = $UseDefaultCredentials;
            if ($PSBoundParameters.ContainsKey('CachePolicy')) { $WebRequest.CachePolicy = New-Object -TypeName 'System.Net.Cache.RequestCachePolicy' -ArgumentList $CachePolicy }
            if ($PSBoundParameters.ContainsKey('Method')) { $WebRequest.Method = $Method }
            if ($PSBoundParameters.ContainsKey('ConnectionGroupName')) { $WebRequest.ConnectionGroupName = $ConnectionGroupName }
            if ($PSBoundParameters.ContainsKey('PreAuthenticate')) { $WebRequest.PreAuthenticate = $PreAuthenticate }
            if ($PSBoundParameters.ContainsKey('Timeout')) { $WebRequest.Timeout = $Timeout }
            if ($PSBoundParameters.ContainsKey('AuthenticationLevel')) { $WebRequest.AuthenticationLevel = $AuthenticationLevel }
            if ($PSBoundParameters.ContainsKey('ImpersonationLevel')) { $WebRequest.ImpersonationLevel = $ImpersonationLevel }
            if ($PSBoundParameters.ContainsKey('PSCredential')) { $WebRequest.Credentials = $PSCredential.GetNetworkCredential() }
            if ($PSBoundParameters.ContainsKey('Credentials')) { $WebRequest.Credentials = $Credentials }

            if ($PSBoundParameters.ContainsKey('Headers') -and $Headers.Count -gt 0) {
                $Headers.Keys | ForEach-Object {
                    if ($_ -is [string]) { $Key = $_ } else { $Key = ($_ | Out-String) -replace '(\r\n?|\n)$', '' }
                    if ($Headers[$_] -eq $null) {
                        $WebRequest.Headers.Add($Key, '');
                    } else {
                        if ($Headers[$_] -is [string]) {
                            $WebRequest.Headers.Add($Key, $Headers[$_]);
                        } else {
                            $WebRequest.Headers.Add($Key, (($Headers[$_] | Out-String) -replace '(\r\n?|\n)$', ''));
                        }
                    }
                }
            }
        }
    }
}

Function Test-SoapEnvelope {
    [CmdletBinding(DefaultParameterSetName = 'XmlNamespaceManager')]
    [OutputType([bool])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObject')]
        [System.Management.Automation.PSObject]$SoapEnvelope,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [System.Xml.XmlDocument]$XmlDocument,
        
        [Parameter(ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [Parameter(Mandatory = $true, ParameterSetName = 'XmlNamespaceManager')]
        [System.Xml.XmlNamespaceManager]$XmlNamespaceManager
    )
    
    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'PSObject' {
                if ($SoapEnvelope.XmlDocument -eq $null) { $false } else { $SoapEnvelope | Test-SoapEnvelope }
                break;
            }
            'Properties' {
                if ($PSBoundParameters.ContainsKey('XmlNamespaceManager')) {
                    $XmlNamespaceManager = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $XmlDocument.NameTable;
                    $XmlNamespaceManager.AddNamespace('xsd', 'http://www.w3.org/2001/XMLSchema');
                    $XmlNamespaceManager.AddNamespace('xsi', 'http://www.w3.org/2001/XMLSchema-instance');
                    $XmlNamespaceManager.AddNamespace('soap12', 'http://www.w3.org/2003/05/soap-envelope');
                }
                if (Test-SoapEnvelope -XmlNamespaceManager $XmlNamespaceManager) {
                    $XPath = '/{0}:Envelope/{0}:Body' -f $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2003/05/soap-envelope');
                    if ($XmlDocument.SelectNodes($XPath, $XmlNamespaceManager).Count -eq 1) { $true } else { $false }
                } else {
                    $false;
                }
            }
            default {
                if ($XmlNamespaceManager.HasNamespace('http://www.w3.org/2001/XMLSchema') -and $XmlNamespaceManager.HasNamespace('http://www.w3.org/2001/XMLSchema-instance') -and `
                        $XmlNamespaceManager.HasNamespace('http://www.w3.org/2003/05/soap-envelope')) {
                    $true
                } else {
                    $false;
                }
            }
        }
    }
}

Function Get-SoapXmlNamespacePrefix {
    [CmdletBinding(DefaultParameterSetname = 'PropertiesSoap')]
    [OutputType([System.Xml.XmlDocument])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObjectSoap')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObjectSchema')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObjectInstance')]
        [ValidateScript({ $_ | Test-SoapEnvelope })]
        [System.Management.Automation.PSObject]$SoapEnvelope,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'PropertiesSoap')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'PropertiesSchema')]
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'PropertiesInstance')]
        [ValidateScript({ Test-SoapEnvelope -XmlDocument $_ })]
        [System.Xml.XmlDocument]$XmlDocument,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [ValidateScript({ Test-SoapEnvelope -XmlNamespaceManager $_ })]
        [System.Xml.XmlNamespaceManager]$XmlNamespaceManager,
        
        [Parameter(ParameterSetname = 'PropertiesSoap')]
        [Parameter(Mandatory = $true, ParameterSetname = 'PSObjectSoap')]
        [switch]$Soap,
        
        [Parameter(Mandatory = $true, ParameterSetname = 'PSObjectSchema')]
        [Parameter(Mandatory = $true, ParameterSetname = 'PropertiesSchema')]
        [switch]$Schema,
        
        [Parameter(Mandatory = $true, ParameterSetname = 'PSObjectInstance')]
        [Parameter(Mandatory = $true, ParameterSetname = 'PropertiesInstance')]
        [switch]$SchemaInstance
    )
    
    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'PSObjectSoap' { $SoapEnvelope | Get-SoapBodyElement -Soap; break; }
            'PSObjectSchema' { $SoapEnvelope | Get-SoapBodyElement -Schema; break; }
            'PSObjectInstance' { $SoapEnvelope | Get-SoapBodyElement -SchemaInstance; break; }
            'PropertiesSoap' { $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2003/05/soap-envelope'); break; }
            'PropertiesSchema' { $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2001/XMLSchema'); break; }
            default { $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2001/XMLSchema-instance'); break; }
        }
    }
}

Function Add-SoapBodyElement {
    [CmdletBinding(DefaultParameterSetName = 'Properties')]
    [OutputType([System.Xml.XmlElement[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObject')]
        [ValidateScript({ $_ | Test-SoapEnvelope })]
        [System.Management.Automation.PSObject]$SoapEnvelope,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [ValidateScript({ Test-SoapEnvelope -XmlDocument $_ })]
        [System.Xml.XmlDocument]$XmlDocument,
        
        [Parameter(ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [ValidateScript({ Test-SoapEnvelope -XmlNamespaceManager $_ })]
        [System.Xml.XmlNamespaceManager]$XmlNamespaceManager,
        
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetname = 'PSObject')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Properties')]
        [System.Xml.XmlElement[]]$Body
    )
    
    Begin { if ($PSCmdlet.ParameterSetName -eq 'PSObject') { $BodyElements = @() } }
    
    Process {
        if ($PSCmdlet.ParameterSetName -eq 'PSObject') {
            $BodyElements = $BodyElements + $Body;
        } else {
            $XPath = '/{0}:Envelope/{0}:Body' -f $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2003/05/soap-envelope');
            $BodyElement = $XmlDocument.SelectSingleNode($XPath, $XmlNamespaceManager);
            foreach ($XmlElement in $Body) { $BodyElement.AppendChild($XmlDocument.ImportNode($XmlElement)) }
        }
    }
    
    End { if ($PSCmdlet.ParameterSetName -eq 'PSObject') { $SoapEnvelope | Add-SoapBodyElement -Body $BodyElements } }
}

Function Get-SoapBodyElement {
    [CmdletBinding(DefaultParameterSetName = 'Properties')]
    [OutputType([System.Xml.XmlElement[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetname = 'PSObject')]
        [ValidateScript({ $_ | Test-SoapEnvelope })]
        [System.Management.Automation.PSObject]$SoapEnvelope,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [ValidateScript({ Test-SoapEnvelope -XmlDocument $_ })]
        [System.Xml.XmlDocument]$XmlDocument,
        
        [Parameter(ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Properties')]
        [ValidateScript({ Test-SoapEnvelope -XmlNamespaceManager $_ })]
        [System.Xml.XmlNamespaceManager]$XmlNamespaceManager
    )
    
    Process {
        if ($PSCmdlet.ParameterSetName -eq 'PSObject') {
            $SoapEnvelope | Get-SoapBodyElement
        } else {
            $XPath = '/{0}:Envelope/{0}:Body/*' -f $XmlNamespaceManager.LookupNamespace('http://www.w3.org/2003/05/soap-envelope');
            $XmlNodeList = $XmlDocument.SelectNodes($XPath, $XmlNamespaceManager);
            for ($n = 0; $n -lt $XmlNodeList.Count; $n++) { $XmlNodeList.Item($n) }
        }
    }
}

Function New-SoapEnvelope {
    [CmdletBinding()]
    [OutputType([System.Xml.XmlDocument])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Xml.XmlElement[]]$Body,
    )
    
    [Xml]$XmlDocument = @'
<?xml version="1.0" encoding="utf-8"?>
<soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
  <soap12:Body />
</soap12:Envelope>
'@;
    $XmlNamespaceManager = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $SoapEnvelope.NameTable;
    $XmlNamespaceManager.AddNamespace('xsd', 'http://www.w3.org/2001/XMLSchema');
    $XmlNamespaceManager.AddNamespace('xsi', 'http://www.w3.org/2001/XMLSchema-instance');
    $XmlNamespaceManager.AddNamespace('soap12', 'http://www.w3.org/2003/05/soap-envelope');
    
    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ XmlDocument = $XmlDocument; XmlNamespaceManager = $XmlNamespaceManager; }
}