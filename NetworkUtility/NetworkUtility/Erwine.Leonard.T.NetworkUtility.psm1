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

Function ConvertFrom-UrlEncoded {
    [CmdletBinding(DefaultParameterSetName = 'Value')]
    [OutputType([Hashtable])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Hashtable')]
		[AllowEmptyString()]
		[AllowNull()]
        [string]$Data
	)

	Process {
		$Hastable = @{};
		if ($Data -ne $null) {
			$FirstQueryChar = $Data.IndexOf('?');
			if ($FirstQueryChar -ge 0) {
				$FirstSeparatorChar = $Data.IndexOf('=');
				if ($FirstSeparatorChar -lt 0) { $FirstSeparatorChar = $Data.IndexOf('&') }
				if ($FirstSeparatorChar -ge 0 -and $FirstQueryChar -gt $FirstSeparatorChar) {
					$Data = $Data.Substring($FirstSeparatorChar);
				} else {
					$Data = $Data.Substring($FirstQueryChar);
				}
			}
			if ($Data.Contains('&')) {
				$Pairs = $Data -split '&';
			} else {
				$Pairs = @($Data);
			}
			$Pairs | ForEach-Object {
				$Index = $_.Split('=', 2);
				if ($Index -lt 0) {
					if ($HashTable.ContainsKey($_)) {
						$Hastable[$_] = @($Hastable[$_]) + @($null);
					} else {
						$Hastable.Add($_, $null);
					}
				} else {
					$k = $_.Substring(0, $Index);
					if ($HashTable.ContainsKey($k)) {
						$Hastable[$k] = @($Hastable[$k]) + @($_.Substring($Index + 1));
					} else {
						$Hastable.Add($k, $_.Substring($Index + 1));
					}
				}
			}
		}
	}
}

Function ConvertTo-UrlEncoded {
    [CmdletBinding(DefaultParameterSetName = 'Value')]
    [OutputType([string])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Hashtable')]
        [Hashtable]$Data,
		
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipelineByProperptyName = $true, ParameterSetName = 'KeyValue')]
		[AllowEmptyString()]
        [string]$Key,
		
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Value')]
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipelineByProperptyName = $true, ParameterSetName = 'KeyValue')]
		[AllowEmptyString()]
		[AllowNull()]
        [object]$Value
	)

	Process {
		switch ($PSCmdlet.ParameterSetName) {
			'Value' {
				if ($Value -ne $null) {
					if ($Value -is [string]) {
						[System.Uri]::EscapeDataString($Value);
					} else {
						[System.Uri]::EscapeDataString((($Value | Out-String) -replace '(\r\n?\n)$', ''));
					}
				}
			}
			'Hashtable' {
				$StringBuilder = New-Object -TypeName 'System.Text.StringBuilder';

				foreach ($key in $Data.Keys) {
					if ($key -is [string]) {
						$KeyValue = ConvertTo-UrlEncoded -Key $key -Value $Data[$key];
					} else {
						$KeyValue = ConvertTo-UrlEncoded -Key (($key | Out-String) -replace '(\r\n?\n)$', '') -Value $Data[$key];
					}
					
					if ($KeyValue.Length -gt 0) {
						if ($StringBuilder.Length -gt 0) { $StringBuilder.Append('&') }
						$StringBuilder.Append($KeyValue);
					}
				}
				$StringBuilder.ToString();
			}
			default {
				$ValueEnc = ConvertTo-UrlEncoded -Value $Value
				if ($Key.Length -eq 0) {
					if ($ValueEnc -eq $null) { '' } else { $ValueEnc }
				} else {
					if ($ValueEnc -eq $null) {
						ConvertTo-UrlEncoded -Value $Key;
					} else {
						'{0}={1}' -f $ValueEnc, (ConvertTo-UrlEncoded -Value $Key);
					}
				}
			}
		}
	}
}

Function Initialze-WebRequestPostForm {
    [CmdletBinding(DefaultParameterSetName = 'GetRequest')]
    [OutputType([System.Net.WebRequest], ParameterSetName = 'GetRequest')]
    [OutputType([System.Management.Automation.PSObject], ParameterSetName = 'GetResponse')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Net.WebRequest]$WebRequest,
        
        [Parameter(Mandatory = $true)]
        [Hashtable]$FormData,
        
        [Parameter(ParameterSetName = 'GetResponse')]
        [bool]$AllowRedirect,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'GetResponse')]
        [switch]$GetResponse,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'GetRequest')]
        [switch]$PassThru
    )
    
    Process {
        $WebRequest.Method = 'POST';
        
        $WebRequest.ContentType = 'application/x-www-form-urlencoded';
        $Stream = $WebRequest.GetRequestStream();
        try {
            $StreamWriter = New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream ([System.Text.Encoding]::ASCII);
            try {
                $keyWritten = $false;
                foreach ($key in $Formdata.Keys) {
                    if ($keyWritten) { $StreamWriter.Write('&') } else { $keyWritten = $true }
                    $StreamWriter.Write([System.Uri]::EscapeDataString($key));
                    $StreamWriter.Write('=');
                    if ($FormData[$key] -ne $null) {
                        if ($FormData[$key] -is [string]) {
                            $StreamWriter.Write([System.Uri]::EscapeDataString($Formdata[$key]));
                        } else {
                            $StreamWriter.Write([System.Uri]::EscapeDataString((($Formdata[$key] | Out-String) -replace '(\r\n?|\n)$', ''));
                        }
                    }
                }
            } catch {
                throw;
            } finally {
                $StreamWriter.Flush();
                $Stream.Flush();
                $WebRequest.ContentLength = [int]$Stream.Position;
                $StreamWriter.Close();
                $StreamWriter.Dispose();
                $Stream = $null;
            }
        } catch {
            throw;
        } finally {
            if ($Stream -ne $null) { $Stream.Dispose() }
        }
        if ($GetResponse) {
            if ($PSBoundParameters.ContainsKey('AllowRedirect')) { $WebRequest.AllowRedirect = $AllowRedirect }
            $WebRequest.GetResponse() | Write-Output;
        } else {
            if ($PassThru) { $WebRequest | Write-Output };
        }
    }
}

Function Initialize-WebRequestPostXml {
    [CmdletBinding(DefaultParameterSetName = 'GetRequest')]
    [OutputType([System.Net.WebRequest], ParameterSetName = 'GetRequest')]
    [OutputType([System.Management.Automation.PSObject], ParameterSetName = 'GetResponse')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Net.WebRequest]$WebRequest,
        
        [Parameter(Mandatory = $true)]
        [System.Xml.XmlNode]$XmlData,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'GetRequest')]
        [switch]$PassThru,
        
        [Parameter(ParameterSetName = 'GetResponse')]
        [bool]$AllowRedirect,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'GetResponse')]
        [switch]$GetResponse
    )
    
    Process {
        $WebRequest.Method = 'POST';
        
        $WebRequest.ContentType = 'text/xml;charset=utf-8';
        $Stream = $WebRequest.GetRequestStream();
        try {
            $XmlWriterSettings = New-Object -TypeName 'System.Xml.XmlWriterSettings' -Property @{
                Indent = $true;
                CloseOutput = $true;
                Encoding = [System.Text.Encoding]::UTF8;
                OmitXmlDeclaration = $true;
            };
            $XmlWriter = [System.Xml.XmlWriter]::Create($Stream, $XmlWriterSettings);
            try {
                $XmlData.WriteTo($XmlWriter);
            } catch {
                throw;
            } finally {
                $XmlWriter.Flush();
                $Stream.Flush();
                $WebRequest.ContentLength = [int]$Stream.Position;
                $XmlWriter.Close();
                $XmlWriter = $null;
                $Stream = $null;
            }
        } catch {
            throw;
        } finally {
            if ($Stream -ne $null) { $Stream.Dispose() }
        }
        if ($GetResponse) {
            if ($PSBoundParameters.ContainsKey('AllowRedirect')) { $WebRequest.AllowRedirect = $AllowRedirect }
            $WebRequest.GetResponse() | Write-Output;
        } else {
            if ($PassThru) { $WebRequest | Write-Output };
        }
    }
}

Function Get-WebResponse {
    [CmdletBinding()]
    [OutputType([System.Management.Automation.PSObject])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [System.Net.WebRequest]$WebRequest,
        
        [bool]$AllowRedirect
    )
    
    Process {
        if ($PSBoundParameters.ContainsKey('AllowRedirect')) { $WebRequest.AllowRedirect = $AllowRedirect }
        $Response = @{
            Request = $WebRequest;
            ErrorStatus = [System.Net.WebExceptionStatus]::UnknownError;
            StatusCode = [System.Management.Automation.PSInvocationState]::NotStarted;
            Success = $false;
            StatusDescription = 'Request not sent.';
        };
        try {
            $Response['Response'] = $WebRequest.GetResponse();
            $Response['ErrorStatus'] = [System.Net.WebExceptionStatus]::Success;
            if ($Response['Response'].StatusCode -ne $null) {
                $Response['StatusCode'] = $Response['Response'].StatusCode;
                $Response['Success'] = $SuccessCodes -contains $Response['StatusCode'];
            } else {
                $Response['StatusCode'] = [System.Management.Automation.PSInvocationState]::Completed;
                $Response['Success'] = $true;
            }
            if ($Response['Response'].StatusDescription -eq $null -or $Response['Response'].StatusDescription.Trim() -eq '') {
                $Response['StatusDescription'] = $Response['StatusCode'].ToString('F');
            } else {
                $Response['StatusDescription'] = $Response['Response'].StatusDescription;
            }
            
        } catch [System.Net.WebException] {
            $Response['ErrorStatus'] = $_.Exception.Status;
            $Response['Response'] = $_.Exception.Response;
            $Response['Error'] = $_;
            if ($Response['Response'] -ne $null -and $Response['Response'].StatusCode -ne $null) {
                $Response['StatusCode'] = $Response['Response'].StatusCode;
            } else {
                $Response['StatusCode'] = [System.Management.Automation.PSInvocationState]::Failed;
            }
            if ($Response['Response'] -eq $null -or $Response['Response'].StatusDescription -eq $null -or $Response['Response'].StatusDescription.Trim() -eq '') {
                if ($_.ErrorDetails -eq $null -or $_.ErrorDetails.Message -eq $null -or $_.ErrorDetails.Message.Trim() -eq '') {
                    if ($_.Exception -eq $null -or $_.Exception.Message -eq $null -or $_.Exception.Message.Trim() -eq '') {
                        $Response['StatusDescription'] = $Response['StatusCode'].ToString('F');
                    } else {
                        $Response['StatusDescription'] = $_.Exception.Message;
                    }
                } else {
                    $Response['StatusDescription'] = $_.ErrorDetails.Message;
                }
            } else {
                $Response['StatusDescription'] = $Response['Response'].StatusDescription;
            }
        } catch [System.Net.Sockets.SocketException] {
            $Response['StatusCode'] = $_.SocketErrorCode;
            $Response['Error'] = $_;
            if ($_.ErrorDetails -eq $null -or $_.ErrorDetails.Message -eq $null -or $_.ErrorDetails.Message.Trim() -eq '') {
                if ($_.Exception -eq $null -or $_.Exception.Message -eq $null -or $_.Exception.Message.Trim() -eq '') {
                    $Response['StatusDescription'] = $Response['StatusCode'].ToString('F');
                } else {
                    $Response['StatusDescription'] = $_.Exception.Message;
                }
            } else {
                $Response['StatusDescription'] = $_.ErrorDetails.Message;
            }
        } catch {
            $Response['StatusCode'] = [System.Management.Automation.PSInvocationState]::Failed;
            if ($_.Exception -ne $null -and $_.Exception -is [System.Management.Automation.MethodException]) {
                if ($_.Exception.ErrorRecord -ne $null) {
                    $Response['Error'] = $_.Exception.ErrorRecord;
                } else {
                    $Response['Error'] = $_;
                }
                if ($_.Exception.InnerException -ne $null) {
                    $e = $_.Exception.InnerException;
                } else {
                    $e = $null;
                }
            } else {
                $e = $null;
            }
            if ($e -ne $null -and $e.Message -ne $null -and $e.Message.Trim() -ne '') {
                $Response['StatusDescription'] = $e.Message;
            } else {
                if ($Response['Error'].ErrorDetails -eq $null -or $Response['Error'].ErrorDetails.Message -eq $null -or $Response['Error'].ErrorDetails.Message.Trim() -eq '') {
                    if ($Response['Error'].Exception -eq $null -or $Response['Error'].Exception.Message -eq $null -or $Response['Error'].Exception.Message.Trim() -eq '') {
                        $Response['StatusDescription'] = $Response['StatusCode'].ToString('F');
                    } else {
                        $Response['StatusDescription'] = $Response['Error'].Exception.Message;
                    }
                } else {
                    $Response['StatusDescription'] = $Response['Error'].ErrorDetails.Message;
                }
            }
        }
        New-Object -TypeName 'System.Management.Automation.PSObject' -Parameter $Response;
        if ($PSBoundParameters.ContainsKey('AllowRedirect')) { $WebRequest.AllowRedirect = $AllowRedirect }
        $WebRequest.GetResponse() | Write-Output;
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