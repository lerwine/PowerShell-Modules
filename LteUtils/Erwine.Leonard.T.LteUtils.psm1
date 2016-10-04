('System.Windows.Forms', 'System.Web', 'System.Security') | ForEach-Object {
	if ((Add-Type -AssemblyName $_ -PassThru -ErrorAction Stop) -eq $null) { throw ('Cannot load assembly "{0}".' -f $_) }
}

Function Get-WebResponse {
	[CmdletBinding(DefaultParameterSetName = "GET")]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = "GET")]
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = "Form")]
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = "Xml")]
		[string]$Uri,
		
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = "Form")]
		[HashTable]$FormData,
		
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = "Xml")]
		[Xml]$XmlData,
		
		[Parameter(Mandatory = $false, ParameterSetName = "GET")]
		[switch]$Get,
		
		[switch]$Anonymous
	)
	
	Process {
		[System.Net.WebRequest]$WebRequest = [System.Net.WebRequest]::Create($Uri);
		if ($WebRequest -ne $null) {
			if (-not $Anonymous) { $WebRequest.Credentials = [System.Net.CredentialCache]::DefaultCredentials }
            switch ($PSCmdlet.ParameterSetName) {
                'Form' {
			        $WebRequest.Method = 'POST';
				    $WebRequest.ContentType = 'application/x-www-form-urlencoded';
                    $StreamWriter = $null;
                    [System.IO.Stream]$Stream = $WebRequest.GetRequestStream();
                    try {
                        $StreamWriter = New-Object -TypeName 'System.IO.StreamWriter' -ArgumentLisdt $Stream, ([System.Text.Encoding]::ASCII);
                        try {
                            $KeysWritten = $false;
            				if ($FormData.Count -gt 0) {
            					foreach ($key in $FormData.Keys) {
            						if ($KeysWritten) {
                                        $StreamWriter.Write('&');
                                    } else {
                                        $KeysWritten = $true;
                                    }
            						$StreamWriter.Write([System.Uri]::EscapeDataString($key))
            						if ($FormData[$key] -ne $null) {
                                        $StreamWriter.Write('=')
                						if ($FormData[$key] -is [string]) {
                							$StreamWriter.Write([System.Uri]::EscapeDataString($Post[$key]));
                						} else {
                							$StreamWriter.Write([System.Uri]::EscapeDataString(($Post[$key] | Out-String -Stream)));
                						}
                                    }
            					}
            				}
                        } catch {
                            throw
                        } finally {
    						$StreamWriter.Flush();
    						$Stream.Flush();
                            $WebRequest.ContentLength = [int]$Stream.Position;
    						$StreamWriter.Close();
                            $StreamWriter = $null;
                            $Stream = $null;
                        }
                    } catch {
                        throw
                    } finally {
                        if ($Stream -ne $null) {
    						$Stream.Flush();
    						$Stream.Close();
                            $Stream = $null;
                        }
                    }
                    break;
                }
                'Xml' {
			        $WebRequest.Method = 'POST';
				    $WebRequest.ContentType = 'text/xml;charset=utf-8';
                    $XmlWriterSettings = New-Object -Typename 'System.Xml.XmlWriterSettings';
                    $XmlWriterSettings.Indent = $true;
                    $XmlWriterSettings.CloseOutput = $true;
                    $XmlWriterSettings.Encoding = [System.Text.Encoding]::UTF8;
                    $XmlWriterSettings.OmitXmlDeclaration = $true;
                    $XmlWriter = $null;
                    [System.IO.Stream]$Stream = $WebRequest.GetRequestStream();
                    try {
                        $XmlWriter = [System.Xml.XmlWriter]::Create($Stream, $XmlWriterSettings);
                        try {
                            XmlData.WriteTo($XmlWriter);
                        } catch {
                            throw
                        } finally {
    						$XmlWriter.Flush();
    						$Stream.Flush();
                            $WebRequest.ContentLength = [int]$Stream.Position;
    						$XmlWriter.Close();
                            $XmlWriter = $null;
                            $Stream = $null;
                        }
                    } catch {
                        throw
                    } finally {
                        if ($Stream -ne $null) {
    						$Stream.Flush();
    						$Stream.Close();
                            $Stream = $null;
                        }
                    }
                    break;
                }
                default {
			        $WebRequest.Method = $PSCmdlet.ParameterSetName;
                    break;
                }
            }
			$WebRequest.Method = $PSCmdlet.ParameterSetName;
			if ($PSCmdlet.ParameterSetName -eq 'POST') {
				$WebRequest.ContentType = "application/x-www-form-urlencoded";
				$StringBuilder = [System.Text.StringBuilder];
				if ($Post.Count -gt 0) {
					foreach ($key in $Post.Keys) {
						if ($StringBuilder.Length -gt 0) { $StringBuilder.Append('&') }
						$StringBuilder.Append([System.Uri]::EscapeDataString($key))
						if ($Post[$key] -ne $null) { $StringBuilder.Append('=') }
						if ($Post[$key] -is [string]) {
							$StringBuilder.Append([System.Uri]::EscapeDataString($Post[$key]));
						} else {
							$StringBuilder.Append([System.Uri]::EscapeDataString(($Post[$key] | Out-String -Stream)));
						}
					}
				}
				$ReqData = $StringBuilder.ToString();
				$Encoding = [System.Text.Encoding]::ASCII;
				[System.IO.Stream]$Stream = $WebRequest.GetRequestStream();
				if ($Stream -ne $null) {
					try {
						[System.IO.StreamWriter]$StreamWriter = New-Object -TypeName 'System.IO.StreamWriter' -ArgumentList $Stream, $Encoding;
						if ($StreamWriter -ne $null) {
							try {
								StreamWriter.Write('{0}', $ReqData);
								$StreamWriter.Flush();
								$WebRequest.ContentLength = ($Encoding.GetBytes($ReqData)).Length;
							} catch {
								throw;
							} finally {
								$StreamWriter.Close();
								$StreamWriter.Dispose();
							}
						}
					} catch {
						throw;
					} finally {
						$Stream.Close();
						$Stream.Dispose();
					}
				}
			}
			$WebRequest.GetResponse();
		}
	}
}

Function Get-WebResponseContent {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[System.Net.WebResponse]$WebResponse
	)
	
	Process {
		$ResponseStream = $WebResponse.GetResponseStream();
		if ($ResponseStream -ne $null) {
			try {
				[System.IO.StreamReader]$StreamReader = New-Object -TypeName 'System.IO.StreamReader' -ArgumentList $ResponseStream;
				if ($StreamReader -ne $null) {
					try {
						$StreamReader.ReadToEnd();
					} catch {
						throw;
					} finally {
						$StreamReader.Close();
						$StreamReader.Dispose();
					}
				}
			} catch {
				throw;
			} finally {
				$ResponseStream.Close();
				$ResponseStream.Dispose();
			}
		}
	}
}

Function Format-WebResponse {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[System.Net.WebResponse]$WebResponse
	)
	
	Process {
		@'
ResponseUri: {0}
	CharacterSet: {1}
	ContentEncoding: {2}
	ContentLength: {3}
	ContentType: {4}
	IsFromCache: {5}
	IsMutuallyAuthenticated: {6}
	LastModified: {7}
	Method: {8}
	ProtocolVersion: {9}
	Server: {10}
	StatusCode: {11}
	StatusDescription: {12}
'@ -f $WebResponse.ResponseUri, $WebResponse.CharacterSet, $WebResponse.ContentEncoding, $WebResponse.ContentLength, $WebResponse.ContentType, $WebResponse.IsFromCache, $WebResponse.IsMutuallyAuthenticated, $WebResponse.LastModified, $WebResponse.Method, $WebResponse.ProtocolVersion, $WebResponse.Server, $WebResponse.StatusCode, $WebResponse.StatusDescription;
			# $WebResponse.Cookies
			# $WebResponse.Headers
	}
}

Function Show-DnsHostByName {
	[CmdletBinding()]
	Param(
		[Parameter(Position = 0, ValueFromPipeline = $true)]
		[string]$HostName
	)
	
	Process {
		$IPHostEntry = [System.Net.Dns]::GetHostbyName($HostName);
		
		if ($IPHostEntry -ne $null) {
			'HostName: {0}' -f $IPHostEntry.HostName
			if ($IPHostEntry.AddressList -ne $null -and $IPHostEntry.AddressList.Length -gt 0) {
				foreach ($IPAddress in $IPHostEntry.AddressList) {
					$AddressBytes = $IPAddress.GetAddressBytes();
					@'
		IP Address: {0:d3}.{1:d3}.{2:d3}.{3:d3}
					 Family: {4}
			IsIPv6LinkLocal: {5}
			IsIPv6Multicast: {6}
			IsIPv6SiteLocal: {7}
					ScopeId{8}
'@ -f ([int]($AddressBytes[0])), ([int]($AddressBytes[1])), ([int]($AddressBytes[2])), ([int]($AddressBytes[3])), $IPAddress.AddressFamily.ToString('F'), $IPAddress.IsIPv6LinkLocal, $IPAddress.IsIPv6Multicast, $IPAddress.IsIPv6SiteLocal, (&{ if ($IPAddress.ScopeId -ne $null) { ': ' + $IPAddress.ScopeId } else { ' is null'} })
				}
			}
			if ($IPHostEntry.Aliases -ne $null -and $IPHostEntry.Aliases.Length -gt 0) {
				foreach ($Alias in $IPHostEntry.Aliases) {
					"`tAlias: {0}" -f $Alias
				}
			}
		}
	}
}

Set-Alias nslookup Show-DnsHostByName -Description 'Show DNS Host By Name' -Scope Global;

Function Get-IndentedLines {
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [AllowEmptyString()]
		[string]$InputString,
        
        [Parameter(Mandatory = $false)]
        [int]$Level = 1
    )
    
    Begin {
        $indent = "`t";
        if ($Level -gt 1) { $indent = New-Object -TypeName 'System.String' -ArgumentList $indent, $Level }
    }
    
    Process {
        $split = $InputString.Split("`n") | ForEach-Object { if ($_ -eq "`r") { '' } else { if ($_.EndsWith("`r")) { $_.SubString(0, $_.Length - 1) } else { $_ } } };
        foreach ($s in $split) {
            $trimmed = $s.TrimEnd();
            if ($trimmed.Length -gt 0) { $indent + $trimmed } else { '' }
        }
    }
}

Function Write-XmlToStream {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		[System.Xml.XmlDocument]$XmlDocument,
		[Parameter(Mandatory = $true, Position = 1)]
		[System.IO.Stream]$Stream,
		[Parameter(Mandatory = $false, Position = 2)]
		[System.Xml.XmlWriterSettings]$XmlWriterSettings
	)
    $settings = $XmlWriterSettings;
    if (-not $PSBoundParameters.ContainsKey('XmlWriterSettings')) {
        $settings = New-Object -TypeName 'System.Xml.XmlWriterSettings';
        $settings.Encoding = [System.Text.Encoding]::UTF8;
        $settings.Indent = $true;
    } else {
        $settings = $settings.Clone();
    }
    $settings.CloseOutput = $false;
    
    $XmlWriter = [System.Xml.XmlTextWriter]::Create($Stream, $settings);
    try {
        $MamlDocument.WriteTo($XmlWriter);
        $XmlWriter.Flush();
        $XmlWriterSettings.Encoding.GetString($MemoryStream.ToArray());
    } catch {
        throw;
    } finally {
        $XmlWriter.Close();
    }
}

Function Get-XmlText {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		[System.Xml.XmlDocument]$XmlDocument,
		[Parameter(Mandatory = $false, Position = 1)]
		[System.Xml.XmlWriterSettings]$XmlWriterSettings
	)
    $settings = $XmlWriterSettings;
    if (-not $PSBoundParameters.ContainsKey('XmlWriterSettings')) {
        $settings = New-Object -TypeName 'System.Xml.XmlWriterSettings';
        $settings.Encoding = [System.Text.Encoding]::UTF8;
        $settings.Indent = $true;
    }
    
    $MemoryStream = New-Object -TypeName 'System.IO.MemoryStream';
    try {
        Write-XmlToStream -XmlDocument $XmlDocument -Stream $MemoryStream -XmlWriterSettings $settings;
        $settings.Encoding.GetString($MemoryStream.ToArray());
    } catch {
        throw;
    } finally {
        $MemoryStream.Close();
    }
}