Function Get-X509Store {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.X509Certificates.StoreName]$StoreName = [System.Security.Cryptography.X509Certificates.StoreName]::My,
        
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.X509Certificates.StoreLocation]$StoreLocation = [System.Security.Cryptography.X509Certificates.StoreLocation]::CurrentUser
    )
    
    New-Object -TypeName 'System.Security.Cryptography.X509Certificates.X509Store' -ArgumentList ([System.Security.Cryptography.X509Certificates.StoreName]::My, `
        [System.Security.Cryptography.X509Certificates.StoreLocation]::CurrentUser);
}


Function Select-X509Certificate {
    [CmdletBinding(DefaultParameterSetName = 'ByDate')]
    [OutputType([System.Security.Cryptography.X509Certificates.X509Certificate2Collection])]
    Param(
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.X509Certificates.X509Store]$Store,
        
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]$UsageFlags,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'ByDate')]
        [System.DateTime]$Date = [System.DateTime]::Now,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'ByDate')]
        [switch]$Invert,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'All')]
        [switch]$All
    )
    $X509Store = $Store;
    if (-not $PSBoundParameters.ContainsKey('Store')) { $X509Store = Get-X509Store }
    $Certificates = New-Object -TypeName 'System.Security.Cryptography.X509Certificates.X509Certificate2Collection';
    $X509Store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::OpenExistingOnly -bor [System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly);
    try {
        foreach ($X509Certificate2 in $X509Store.Certificates) {
            if (-not $All) {
                if ($X509Certificate2.NotAfter -lt $Date -or $X509Certificate2.NotBefore -gt $Date) {
                    if (-not $Invert) { continue; }
                } else {
                    if ($Invert) { continue; }
                }
            }
            if ($PSBoundParameters.ContainsKey('UsageFlags')) {
                [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]$X509KeyUsageFlags = [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]::None;
                foreach ($ext in $X509Certificate2.Extensions) {
                    if ($ext -is [System.Security.Cryptography.X509Certificates.X509KeyUsageExtension]) {
                        [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]$X509KeyUsageFlags = $X509KeyUsageFlags -bor $ext.KeyUsages;
                    }
                }
                if ($UsageFlags -eq [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]::None) {
                    if ($X509KeyUsageFlags -ne $UsageFlags) { continue }
                } else {
                    $Flags = $X509KeyUsageFlags -band $UsageFlags;
                    if ([System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]($X509KeyUsageFlags -band $UsageFlags) -eq `
                        [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]::None) { continue }
                }
            }
            $Certificates.Add($X509Certificate2) | Out-Null;
        }
        
        return ,$Certificates;
    } catch {
        throw;
    } finally {
        $X509Store.Close();
    }
}

Function Show-X509Certificate {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate
    )
    
    [System.Security.Cryptography.X509Certificates.X509Certificate2UI]::DisplayCertificate($Certificate);
}

Function Read-X509Certificate {
    [CmdletBinding(DefaultParameterSetName = 'Single')]
    [OutputType([System.Security.Cryptography.X509Certificates.X509Certificate2], ParameterSetName = 'Single')]
    [OutputType([System.Security.Cryptography.X509Certificates.X509Certificate2Collection], ParameterSetName = 'Multi')]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$Message,
        
        [Parameter(Mandatory = $false)]
        [string]$Title = 'Select Certificate',
        
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.X509Certificates.X509Certificate2Collection]$CertificateCollection,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'Single')]
        [switch]$SingleSelection,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Multi')]
        [switch]$MultiSelection
    )
    
    [System.Security.Cryptography.X509Certificates.X509Certificate2Collection]$Certificates = $CertificateCollection;
    if (-not $PSBoundParameters.ContainsKey('CertificateCollection')) {
        [System.Security.Cryptography.X509Certificates.X509Certificate2Collection]$Certificates = Select-X509Certificate `
            -UsageFlags ([System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]::KeyEncipherment);
    }
    
    if ($MultiSelection) {
        [System.Security.Cryptography.X509Certificates.X509Certificate2UI]::SelectFromCollection($Certificates, $Title, $Message, `
            [System.Security.Cryptography.X509Certificates.X509SelectionFlag]::MultiSelection);
    } else {
        $Certificates = [System.Security.Cryptography.X509Certificates.X509Certificate2UI]::SelectFromCollection($Certificates, $Title, $Message, `
            [System.Security.Cryptography.X509Certificates.X509SelectionFlag]::SingleSelection);
        if ($Certificates.Count -gt 0) { $Certificates[0] }
    }
}

Function New-RSACryptoServiceProvider {
    [CmdletBinding()]
    [OutputType([System.Security.Cryptography.AesManaged])]
    Param(
        [Parameter(Mandatory = $false)]
        [int]$KeySize,
        
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.CspParameters]$Parameters
    )
    
    if ($PSBoundParameters.ContainsKey('Parameters')) {
        if ($PSBoundParameters.ContainsKey('KeySize')) {
            New-Object -TypeName 'System.Security.Cryptography.RSACryptoServiceProvider' -ArgumentList $KeySize, $Parameters;
        } else {
            New-Object -TypeName 'System.Security.Cryptography.RSACryptoServiceProvider' -ArgumentList $Parameters;
        }
    } else {
        if ($PSBoundParameters.ContainsKey('KeySize')) {
            New-Object -TypeName 'System.Security.Cryptography.RSACryptoServiceProvider' -ArgumentList $KeySize;
        } else {
            New-Object -TypeName 'System.Security.Cryptography.RSACryptoServiceProvider';
        }
    }
}

Function New-AesManaged {
    [CmdletBinding()]
    [OutputType([System.Security.Cryptography.AesManaged])]
    Param(
        [Parameter(Mandatory = $false)]
        [int]$KeySize = 256,
        
        [Parameter(Mandatory = $false)]
        [int]$BlockSize = 128,
        
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.CipherMode]$Mode = [System.Security.Cryptography.CipherMode]::CBC,
        
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.PaddingMode]$PaddingMode = [System.Security.Cryptography.PaddingMode]::PKCS7
        #[System.Security.Cryptography.CipherMode]$PaddingMode = [System.Security.Cryptography.PaddingMode]::Zeros
    )
    
    $AesManaged = New-Object -TypeName 'System.Security.Cryptography.AesManaged';
    $AesManaged.KeySize = $KeySize;
    $AesManaged.BlockSize = $BlockSize;
    $AesManaged.Mode = $Mode;
    $AesManaged.Padding = $PaddingMode;
    $AesManaged | Write-Output;
}

Function Protect-WithRSA {
    [CmdletBinding()]
    [OutputType([byte[]])]
    Param(
        [Parameter(Mandatory = $true)]
        [System.Security.Cryptography.RSACryptoServiceProvider]$RSA,
        
        [Parameter(Mandatory = $true)]
        [byte[]]$Bytes,
        
        [Parameter(Mandatory = $false)]
        [switch]$OAEP
    )
    
    $encrypted = $RSA.Encrypt($Bytes, $OAEP.IsPresent);
    return ,$encrypted;
}

Function Protect-WithX509Certificate {
    [CmdletBinding()]
    [OutputType([byte[]])]
    Param(
        [Parameter(Mandatory = $true)]
        [ValidateScript({ $_.PublicKey -ne $null -and $_.PublicKey.Key -ne $null -and $_.PublicKey.Key -is [System.Security.Cryptography.RSACryptoServiceProvider]})]
        [System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate,
        
        [Parameter(Mandatory = $true)]
        [byte[]]$Bytes,
        
        [Parameter(Mandatory = $false)]
        [switch]$OAEP
    )
    
    if ($OAEP) {
        Protect-WithRSA -RSA $Certificate.PublicKey.Key -Bytes $Bytes -OAEP;
    } else {
        Protect-WithRSA -RSA $Certificate.PublicKey.Key -Bytes $Bytes;
    }
}

Function Protect-WithSymmetricAlgorithm {
    [CmdletBinding(DefaultParameterSetName = 'Certificate')]
    Param(
        [Parameter(Mandatory = $true, ParameterSetName = 'Certificate')]
        [ValidateScript({ $_.PublicKey -ne $null -and $_.PublicKey.Key -ne $null -and $_.PublicKey.Key -is [System.Security.Cryptography.RSACryptoServiceProvider]})]
        [System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'RSA')]
        [System.Security.Cryptography.RSACryptoServiceProvider]$RSA,
        
        [Parameter(Mandatory = $true)]
        [System.IO.Stream]$InputStream,
        
        [Parameter(Mandatory = $true)]
        [System.IO.Stream]$OutputStream,
        
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.SymmetricAlgorithm]$SymmetricAlgorithm,
        
        [Parameter(Mandatory = $false)]
        [int]$ProgressId,

        [Parameter(Mandatory = $false)]
        [int]$ParentProgressId,

        [Parameter(Mandatory = $false)]
        [switch]$OAEP
    )

    try {
        $Algorithm = $SymmetricAlgorithm;
        if (-not $PSBoundParameters.ContainsKey('SymmetricAlgorithm')) {
            if ($PSBoundParameters.ContainsKey('ProgressId')) {
                if ($PSBoundParameters.ContainsKey('ParentProgressId')) {
                    Write-Progress -Activity 'Encrypting Data' -Status 'Initializing' -Id $ProgressId -PercentComplete 0 -CurrentOperation 'Initializing encryption provider' -ParentId $ParentProgressId;
                } else {
                    Write-Progress -Activity 'Encrypting Data' -Status 'Initializing' -Id $ProgressId -PercentComplete 0 -CurrentOperation 'Initializing encryption provider';
                }
            }
            $Algorithm = New-AesManaged -ErrorAction Stop;
        }

        if ($PSBoundParameters.ContainsKey('ProgressId')) {
            if ($PSBoundParameters.ContainsKey('ParentProgressId')) {
                Write-Progress -Activity 'Encrypting Data' -Status 'Initializing' -Id $ProgressId -PercentComplete 0 -CurrentOperation 'Writing encryption data headers' -ParentId $ParentProgressId;
            } else {
                Write-Progress -Activity 'Encrypting Data' -Status 'Initializing' -Id $ProgressId -PercentComplete 0 -CurrentOperation 'Writing encryption data headers';
            }
        }

        Write-LengthEncodedBytes -Stream $OutputStream -Bytes $Algorithm.IV -ErrorAction Stop;
    
        if ($PSBoundParameters.ContainsKey('Certificate')) {
            if ($OAEP) {
                Write-LengthEncodedBytes -Stream $OutputStream -Bytes (Protect-WithX509Certificate -Certificate $Certificate -Bytes $Algorithm.Key -OAEP) -ErrorAction Stop;
            } else {
                Write-LengthEncodedBytes -Stream $OutputStream -Bytes (Protect-WithX509Certificate -Certificate $Certificate -Bytes $Algorithm.Key) -ErrorAction Stop;
            }
        } else {
            if ($OAEP) {
                Write-LengthEncodedBytes -Stream $OutputStream -Bytes (Protect-WithRSA -RSA $RSA -Bytes $Algorithm.Key -OAEP) -ErrorAction Stop;
            } else {
                Write-LengthEncodedBytes -Stream $OutputStream -Bytes (Protect-WithRSA -RSA $RSA -Bytes $Algorithm.Key) -ErrorAction Stop;
            }
        }
    
        [long]$totalBytes = $InputStream.Length - $InputStream.Position;
        Write-LongIntegerToStream -Stream $OutputStream -Value $totalBytes;
    
        $ICryptoTransform = $Algorithm.CreateEncryptor();
	    $Path = [System.IO.Path]::GetTempFileName();
	    $FileStream = $null;
	    try {
		    $FileStream = New-Object -TypeName 'System.IO.FileStream' -ArgumentList ($Path, [System.IO.FileMode]::OpenOrCreate, [System.IO.FileAccess]::ReadWrite) `
                -ErrorAction Stop;
	    } catch {
		    if ([System.IO.File]::Exists($Path)) { [System.IO.File]::Delete($Path) }
		    throw;
	    }
	
        $CryptoStream = $null;
        try {
            if ($PSBoundParameters.ContainsKey('ProgressId')) {
                if ($PSBoundParameters.ContainsKey('ParentProgressId')) {
                    Write-Progress -Activity 'Encrypting Data' -Status 'Initializing' -Id $ProgressId -PercentComplete 0 -CurrentOperation 'Opening cryptographic stream' -ParentId $ParentProgressId;
                } else {
                    Write-Progress -Activity 'Encrypting Data' -Status 'Initializing' -Id $ProgressId -PercentComplete 0 -CurrentOperation 'Opening cryptographic stream';
                }
            }
            $CryptoStream = New-Object -TypeName 'System.Security.Cryptography.CryptoStream' -ArgumentList ($FileStream, $ICryptoTransform, `
                [System.Security.Cryptography.CryptoStreamMode]::Write) -ErrorAction Stop;
        } catch {
            $FileStream.Dispose();
            throw;
        }
    
        try {
		    $DataBuffer = New-DataBuffer -Capacity ($Algorithm.BlockSize / 8);
		    [long]$bytesRead = 0;
            $percentComplete = 0;
		    for ($count = (Read-DataBuffer -Buffer $DataBuffer -Stream $InputStream); $count -eq $DataBuffer.Capacity; $count = (Read-DataBuffer -Buffer $DataBuffer `
                -Stream $InputStream)) {
			    Write-DataBuffer -Buffer $DataBuffer -Stream $CryptoStream;
			    [long]$bytesRead = $bytesRead + [long]$count;
                if ($bytesRead -lt $totalBytes) {
                    [int]$pct = (($bytesRead * 100) / $totalBytes -shr 1);
                    if ($pct -ne $percentComplete) {
                        $percentComplete = $pct;
                        if ($PSBoundParameters.ContainsKey('ProgressId')) {
                            if ($PSBoundParameters.ContainsKey('ParentProgressId')) {
                                Write-Progress -Activity 'Encrypting Data' -Status 'Working' -Id $ProgressId -PercentComplete $percentComplete -CurrentOperation ('Encrypting {0} bytes of {1}' -f $bytesRead, $totalBytes) -ParentId $ParentProgressId;
                            } else {
                                Write-Progress -Activity 'Encrypting Data' -Status 'Working' -Id $ProgressId -PercentComplete $percentComplete -CurrentOperation ('Encrypting {0} bytes of {1}' -f $bytesRead, $totalBytes);
                            }
                        }
                    }
                }
		    }
            $CryptoStream.FlushFinalBlock();
            $FileStream.Seek(0L, [System.IO.SeekOrigin]::Begin);
		    [long]$bytesRead = 0;
            $percentComplete = 0;
		    for ($count = (Read-DataBuffer -Buffer $DataBuffer -Stream $FileStream); $count -gt 0; $count = (Read-DataBuffer -Buffer $DataBuffer -Stream $FileStream)) {
			    [long]$bytesRead = $bytesRead + [long]$count;
                [int]$pct = (($bytesRead * 100) / $FileStream.Length -shr 1) + 50;
			    Write-DataBuffer -Buffer $DataBuffer -Stream $OutputStream -Count $count;
                if ($pct -ne $percentComplete) {
                    $percentComplete = $pct;
                    if ($PSBoundParameters.ContainsKey('ProgressId')) {
                        if ($PSBoundParameters.ContainsKey('ParentProgressId')) {
                            Write-Progress -Activity 'Saving Data' -Status 'Working' -Id $ProgressId -PercentComplete $percentComplete -CurrentOperation ('Saving {0} bytes of {1}' -f $bytesRead, $FileStream.Length) -ParentId $ParentProgressId;
                        } else {
                            Write-Progress -Activity 'Saving Data' -Status 'Working' -Id $ProgressId -PercentComplete $percentComplete -CurrentOperation ('Saving {0} bytes of {1}' -f $bytesRead, $FileStream.Length);
                        }
                    }
                }
		    }
        } catch { throw; }
        finally {
            $CryptoStream.Close();
            $CryptoStream.Dispose();
		    if ([System.IO.File]::Exists($Path)) { [System.IO.File]::Delete($Path) }
        }
        if ($PSBoundParameters.ContainsKey('ProgressId')) {
            if ($PSBoundParameters.ContainsKey('ParentProgressId')) {
                Write-Progress -Activity 'Encrypting Data' -Status 'Success' -Id $ProgressId -PercentComplete 0 -ParentId $ParentProgressId;
            } else {
                Write-Progress -Activity 'Encrypting Data' -Status 'Success' -Id $ProgressId -PercentComplete 0;
            }
        }
    } catch {
        if ($PSBoundParameters.ContainsKey('ProgressId')) {
            if ($PSBoundParameters.ContainsKey('ParentProgressId')) {
                Write-Progress -Activity 'Encrypting Data' -Status 'Failed' -Id $ProgressId -PercentComplete 0 -ParentId $ParentProgressId;
            } else {
                Write-Progress -Activity 'Encrypting Data' -Status 'Failed' -Id $ProgressId -PercentComplete 0;
            }
        }
        throw;
    }
}

Function Unprotect-WithRSA {
    [CmdletBinding()]
    [OutputType([byte[]])]
    Param(
        [Parameter(Mandatory = $true)]
        [System.Security.Cryptography.RSACryptoServiceProvider]$RSA,
        
        [Parameter(Mandatory = $true)]
        [byte[]]$Bytes,
        
        [Parameter(Mandatory = $false)]
        [switch]$OAEP
    )
    
    $decrypted = $RSA.Decrypt($Bytes, $OAEP.IsPresent);
    return ,$decrypted;
}

Function Unprotect-WithX509Certificate {
    [CmdletBinding()]
    [OutputType([byte[]])]
    Param(
        [Parameter(Mandatory = $true)]
        [ValidateScript({ $_.PrivateKey -ne $null -and $_.PrivateKey -is [System.Security.Cryptography.RSACryptoServiceProvider]})]
        [System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate,
        
        [Parameter(Mandatory = $true)]
        [byte[]]$Bytes,
        
        [Parameter(Mandatory = $false)]
        [switch]$OAEP
    )
    
    if ($OaepPadding) {
        Unprotect-WithRSA -RSA $Certificate.PrivateKey -Bytes $Bytes -OAEP;
    } else {
        Unprotect-WithRSA -RSA $Certificate.PrivateKey -Bytes $Bytes;
    }
}

Function Unprotect-WithSymmetricAlgorithm {
    [CmdletBinding(DefaultParameterSetName = 'Certificate')]
    [OutputType([long])]
    Param(
        [Parameter(Mandatory = $true, ParameterSetName = 'Certificate')]
        [ValidateScript({ $_.PrivateKey -ne $null -and $_.PrivateKey -is [System.Security.Cryptography.RSACryptoServiceProvider]})]
        [System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'RSA')]
        [System.Security.Cryptography.RSACryptoServiceProvider]$RSA,
        
        [Parameter(Mandatory = $true)]
        [System.IO.Stream]$InputStream,
        
        [Parameter(Mandatory = $true)]
        [System.IO.Stream]$OutputStream,
        
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.SymmetricAlgorithm]$SymmetricAlgorithm,
        
        [Parameter(Mandatory = $false)]
        [int]$ProgressId,

        [Parameter(Mandatory = $false)]
        [int]$ParentProgressId,

        [Parameter(Mandatory = $false)]
        [switch]$OAEP
    )
    
    try {
        $Algorithm = $SymmetricAlgorithm;
        if (-not $PSBoundParameters.ContainsKey('SymmetricAlgorithm')) {
            if ($PSBoundParameters.ContainsKey('ProgressId')) {
                if ($PSBoundParameters.ContainsKey('ParentProgressId')) {
                    Write-Progress -Activity 'Decrypting Data' -Status 'Initializing' -Id $ProgressId -PercentComplete 0 -CurrentOperation 'Initializing encryption provider' -ParentId $ParentProgressId;
                } else {
                    Write-Progress -Activity 'Decrypting Data' -Status 'Initializing' -Id $ProgressId -PercentComplete 0 -CurrentOperation 'Initializing encryption provider';
                }
            }
            $Algorithm = New-AesManaged;
        }
    
        if ($PSBoundParameters.ContainsKey('ProgressId')) {
            if ($PSBoundParameters.ContainsKey('ParentProgressId')) {
                Write-Progress -Activity 'Decrypting Data' -Status 'Initializing' -Id $ProgressId -PercentComplete 0 -CurrentOperation 'Reading encryption data headers' -ParentId $ParentProgressId;
            } else {
                Write-Progress -Activity 'Decrypting Data' -Status 'Initializing' -Id $ProgressId -PercentComplete 0 -CurrentOperation 'Reading encryption data headers';
            }
        }

        $Algorithm.IV = Read-LengthEncodedBytes -Stream $InputStream -ErrorAction Stop;
    
        if ($PSBoundParameters.ContainsKey('Certificate')) {
            if ($OAEP) {
                $Algorithm.Key = Unprotect-WithX509Certificate -Certificate $Certificate -Bytes (Read-LengthEncodedBytes -Stream $InputStream -ErrorAction Stop) -OAEP;
            } else {
                $Algorithm.Key = Unprotect-WithX509Certificate -Certificate $Certificate -Bytes (Read-LengthEncodedBytes -Stream $InputStream -ErrorAction Stop);
            }
        } else {
            if ($OAEP) {
                $Algorithm.Key = Unprotect-WithRSA -RSA $RSA -Bytes (Read-LengthEncodedBytes -Stream $InputStream -ErrorAction Stop) -OAEP;
            } else {
                $Algorithm.Key = Unprotect-WithRSA -RSA $RSA -Bytes (Read-LengthEncodedBytes -Stream $InputStream -ErrorAction Stop);
            }
        }
    
        [long]$totalBytes = Read-LongIntegerFromStream -Stream $InputStream;
        if ($totalBytes -lt 0) { throw 'Invalid data length' }
    
        if ($totalBytes -gt 0) {
            $ICryptoTransform = $Algorithm.CreateDecryptor();
            $CryptoStream = $null;
            try {
                if ($PSBoundParameters.ContainsKey('ProgressId')) {
                    if ($PSBoundParameters.ContainsKey('ParentProgressId')) {
                        Write-Progress -Activity 'Decrypting Data' -Status 'Initializing' -Id $ProgressId -PercentComplete 0 -CurrentOperation 'Opening cryptographic stream' -ParentId $ParentProgressId;
                    } else {
                        Write-Progress -Activity 'Decrypting Data' -Status 'Initializing' -Id $ProgressId -PercentComplete 0 -CurrentOperation 'Opening cryptographic stream';
                    }
                }
                $CryptoStream = New-Object -TypeName 'System.Security.Cryptography.CryptoStream' -ArgumentList ($InputStream, $ICryptoTransform, `
                    [System.Security.Cryptography.CryptoStreamMode]::Read);
            } catch {
                $MemoryStream.Dispose();
                throw;
            }
        
            try {
			    $DataBuffer = New-DataBuffer -Capacity ($Algorithm.BlockSize / 8);
                $remainingBytes = $totalBytes;
                $percentComplete = 0;
			    for ($count = (Read-DataBuffer -Buffer $DataBuffer -Stream $CryptoStream); $count -gt 0; $count = (Read-DataBuffer -Buffer $DataBuffer -Stream $CryptoStream)) {
                    if ($remainingBytes -lt 1) { continue }
					if ($totalBytes -lt [long]$count) { [int]$count = $totalBytes; }
                    if ($count -eq 0) { continue }
					Write-DataBuffer -Buffer $DataBuffer -Stream $OutputStream -Count $count;
			        [long]$remainingBytes = $remainingBytes - [long]$count;
                    $bytesRead = $totalBytes - $remainingBytes;
                    [int]$pct = ($bytesRead * 100) / $totalBytes;
                    if ($pct -ne $percentComplete) {
                        $percentComplete = $pct;
                        if ($PSBoundParameters.ContainsKey('ProgressId')) {
                            if ($PSBoundParameters.ContainsKey('ParentProgressId')) {
                                Write-Progress -Activity 'Decrypting Data' -Status 'Working' -Id $ProgressId -PercentComplete $percentComplete -CurrentOperation ('Decrypting {0} bytes of {1}' -f $bytesRead, $totalBytes) -ParentId $ParentProgressId;
                            } else {
                                Write-Progress -Activity 'Decrypting Data' -Status 'Working' -Id $ProgressId -PercentComplete $percentComplete -CurrentOperation ('Decrypting {0} bytes of {1}' -f $bytesRead, $totalBytes);
                            }
                        }
                    }
			    }
                #$CryptoStream.FlushFinalBlock();
            } catch { throw; }
            finally {
                $CryptoStream.Close();
                $CryptoStream.Dispose();
            }
        }
    } catch {
        if ($PSBoundParameters.ContainsKey('ProgressId')) {
            if ($PSBoundParameters.ContainsKey('ParentProgressId')) {
                Write-Progress -Activity 'Decrypting Data' -Status 'Failed' -Id $ProgressId -PercentComplete 0 -ParentId $ParentProgressId;
            } else {
                Write-Progress -Activity 'Decrypting Data' -Status 'Failed' -Id $ProgressId -PercentComplete 0;
            }
        }
        throw;
    }
}
