Function New-SelfSignedX509Certificate2 {
    [CmdletBinding()]
	[OutputType([System.Security.Cryptography.X509Certificates.X509Certificate2])]
    Param(
		[string]$Subject,
		
		[Parameter(Mandatory = $false)]
		[ValidateSet('KeyExchange', 'Signature')]
		[string]$X509KeySpec = 'KeyExchange',
		
		[Parameter(Mandatory = $false)]
		[ValidateSet('User', 'Computer')]
		[string]$MachineContext = 'User',
		
		[Parameter(Mandatory = $false)]
		[ValidateSet('DnsName', 'IpAddress')]
		[string]$AltNameType = 'DnsName',
		
		[Parameter(Mandatory = $false)]
		[ValidateSet('User', 'Computer', 'AdminForceMachine')]
		[string]$X509CertificateEnrollmentContext = 'User',
		
		[Parameter(Mandatory = $false)]
		[ValidateSet('AllUsages', 'None', 'Decrypt', 'Signing')]
		[string]$X509PrivateKeyUsageFlags = 'AllUsages',
		
		[Parameter(Mandatory = $false)]
		[switch]$DataEncipherment,
		
		[Parameter(Mandatory = $false)]
		[switch]$DigitalSignature,
		
		[Parameter(Mandatory = $false)]
		[switch]$KeyEncipherment,
		
		[Parameter(Mandatory = $false)]
		[int]$KeyLength = 2048,
		
		[Parameter(Mandatory = $false)]
		[switch]$DenyExport,
		
		[Parameter(Mandatory = $false)]
		[string[]]$AltDnsNames,
		
		[Parameter(Mandatory = $false)]
		[string[]]$IPAltNames,
		
		[Parameter(Mandatory = $false)]
		[switch]$ServerAuth,
		[switch]$ClientAuth,
		[switch]$SmartCardAuth,
		[switch]$EFS,
		[switch]$CodeSigning
	)
	
	#region Create private key.

	$CX509PrivateKey = New-Object -COM "X509Enrollment.CX509PrivateKey.1";
	# Provider name from original example: 'Microsoft RSA SChannel Cryptographic Provider';
	$CX509PrivateKey.ProviderName = 'Microsoft Enhanced Cryptographic Provider v1.0';
	$CX509PrivateKey.KeySpec = &{ if ($X509KeySpec -eq 'Signature') { 2 } else { 1 } };
	$CX509PrivateKey.KeyUsage = &{
		switch ($X509PrivateKeyUsageFlags) {
			'None' { 0; break; }
			'Decrypt' { 1; break; }
			'Signing' { 2; break; }
			default { 0xffffff; break; }
		}
	};
	# Assigning algorithm did not exist in first example
	$OID = New-Object -COM 'X509Enrollment.CObjectId.1';
	$OID.InitializeFromValue('1.2.840.113549.1.1.5') ;
	$CX509PrivateKey.Algorithm = $OID;
	$CX509PrivateKey.Length = $KeyLength;
	$CX509PrivateKey.MachineContext = &{ if ($MachineContext -eq 'Computer') { 1 } else { 0 } }; 
	$CX509PrivateKey.ExportPolicy = &{ if ($DenyExport) { 0 } else { 1 } };
	
	# the next line does not exist in an alternate example I found
	#$CX509PrivateKey.SecurityDescriptor = 'D:PAI(A;;0xd01f01ff;;;SY)(A;;0xd01f01ff;;;BA)(A;;0x80120089;;;NS)'
	
	$CX509PrivateKey.Create();
	
	#endregion
	
	#region Create certificate request template
	
	$CX509CertificateRequestCertificate = New-Object -com 'X509Enrollment.CX509CertificateRequestCertificate.1';
	$CX509CertificateRequestCertificate.InitializeFromPrivateKey((&{
		switch ($X509CertificateEnrollmentContext) {
			'Computer' { 2; break; }
			'AdminForceMachine' { 3; break; }
			default { 1; break; }
		}
	}), $CX509PrivateKey, "");

	#region Add alternate names
	
	if ($AltDnsNames.Count -gt 0 -or $IPAltNames.Count -gt 0) {
		$CAlternativeNames = New-Object -ComObject 'X509Enrollment.CAlternativeNames';
		$CX509ExtensionAlternativeNames = New-Object -ComObject 'X509Enrollment.CX509ExtensionAlternativeNames';
		foreach ($dnsName in $AltDnsNames) {
			$CAlternativeName = New-Object -ComObject X509Enrollment.CAlternativeName;
			$CAlternativeName.InitializeFromString(3, $dnsName);
			$CAlternativeNames.Add($CAlternativeName);
		 }
		 foreach ($ip in $IPAltNames) {
			$base64EncodedIp = [Convert]::ToBase64String($ip.GetAddressBytes());
			$CAlternativeName = New-Object -ComObject X509Enrollment.CAlternativeName;
			$CAlternativeName.InitializeFromRawData(8, 1, $base64EncodedIp) ;
			$CAlternativeNames.Add($CAlternativeName)
		 }
		 $CX509ExtensionAlternativeNames.InitializeEncode($CAlternativeNames)
		 $CX509CertificateRequestCertificate.X509Extensions.Add($CX509ExtensionAlternativeNames)
	}
	
	#endregion
	
	#region Certificate Extensions.
	
	$KeyUsageOids = New-Object -COM 'X509Enrollment.CObjectIds.1';
	$KeyUsageOids.Add($OID) 
	if ($ServerAuth) {
		$OID = New-Object -COM 'X509Enrollment.CObjectId.1';
		$OID.InitializeFromValue('1.3.6.1.5.5.7.3.1') ;
		$KeyUsageOids.Add($OID) 
	}
	if ($ClientAuth) {
		$OID = New-Object -COM 'X509Enrollment.CObjectId.1';
		$OID.InitializeFromValue('1.3.6.1.5.5.7.3.2') ;
		$KeyUsageOids.Add($OID) 
	}
	if ($SmartCardAuth) {
		$OID = New-Object -COM 'X509Enrollment.CObjectId.1';
		$OID.InitializeFromValue('1.3.6.1.4.1.311.20.2.2') ;
		$KeyUsageOids.Add($OID) 
	}
	if ($EFS) {
		$OID = New-Object -COM 'X509Enrollment.CObjectId.1';
		$OID.InitializeFromValue('1.3.6.1.4.1.311.10.3.4') ;
		$KeyUsageOids.Add($OID) 
	}
	if ($CodeSigning) {
		$OID = New-Object -COM 'X509Enrollment.CObjectId.1';
		$OID.InitializeFromValue('1.3.6.1.5.5.7.3.3') ;
		$KeyUsageOids.Add($OID) 
	}
	
	$CX509ExtensionEnhancedKeyUsage = New-Object -com 'X509Enrollment.CX509ExtensionEnhancedKeyUsage.1';
	$CX509ExtensionEnhancedKeyUsage.InitializeEncode($KeyUsageOids);
	$CX509CertificateRequestCertificate.X509Extensions.Add($CX509ExtensionEnhancedKeyUsage);
	
	$KeyUsages = 0;
	if ($DataEncipherment) { $KeyUsages = 0x10 }
	if ($DigitalSignature) { $KeyUsages = $KeyUsages -bor 0x80 }
	if ($KeyEncipherment) { $KeyUsages = $KeyUsages -bor 0x20 }
	if ($KeyUsages -eq 0) { $KeyUsages = 0xA0 }
	$CX509ExtensionKeyUsage = New-Object -ComObject 'X509Enrollment.CX509ExtensionKeyUsage';
	$CX509ExtensionKeyUsage.InitializeEncode($KeyUsages);
	$CX509CertificateRequestCertificate.X509Extensions.Add($CX509ExtensionKeyUsage);
	
	#endregion
	
	#region Create Subject field in X.500 format
	
	$CX500DistinguishedName = New-Object -COM "X509Enrollment.CX500DistinguishedName.1";
	$CX500DistinguishedName.Encode(('CN={0}' -f $Subject), 3);
	$CX509CertificateRequestCertificate.Subject = $CX500DistinguishedName;
	$CX509CertificateRequestCertificate.Issuer = $CX509CertificateRequestCertificate.Subject;
	$CX509CertificateRequestCertificate.NotBefore = Get-Date;
	$CX509CertificateRequestCertificate.NotAfter = $CX509CertificateRequestCertificate.NotBefore.AddDays(1825);
	$CX509CertificateRequestCertificate.Encode();
	
	#endregion
	
	#endregion
	
	#region Process request and build end certificate.
	
	$CX509Enrollment = New-Object -com "X509Enrollment.CX509Enrollment.1";
	$CX509Enrollment.InitializeFromRequest($CX509CertificateRequestCertificate);
	# Original example had value of 0
	$certdata = $CX509Enrollment.CreateRequest(1);
	$CX509Enrollment.CertificateFriendlyName = $FriendlyName;
	# Original example had values of 2 and 0
	$CX509Enrollment.InstallResponse(4, $certdata, 1, "");
	
	#endregion
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