('System.Core', 'mscorlib', 'System.Security') | ForEach-Object {
	if ($null -eq (Add-Type -AssemblyName $_ -PassThru -ErrorAction Stop)) { throw ('Cannot load assembly "{0}".' -f $_) }
}

<#
if ($null -eq (Get-Module -Name 'Erwine.Leonard.T.IOUtility')) { Import-Module -Name 'Erwine.Leonard.T.IOUtility' -ErrorAction Stop }

Function New-CryptographyOid {
    < #
        .SYNOPSIS
            Create new cryptographic object identifier.

        .DESCRIPTION
            Initializes a new instance of an object which represents a cryptographic object identifier.
        
        .INPUTS
            System.String. A dotted number of the identifier.
            System.Security.Cryptography.Oid. An object representing identifier information be duplicated.
         
        .OUTPUTS
            System.Security.Cryptography.Oid. Represents a cryptographic object identifier.

        .EXAMPLE
            # Initialize a new instance of the AsnEncodedData class.
            $Oid = New-CryptographyOid;

        .EXAMPLE
            # Initialize a new instance of the Oid class using a string value of an Oid object.
            $Oid = New-CryptographyOid -Value '1.2.840.113549.1.1.1';

        .EXAMPLE
            # Initialize new instances of the Oid class using string values.
            $OidStrings = @('1.2.840.113549.1.1.1', '1.3.6.1.4.1.311.20.2');
            $OidArray = $OidStrings | New-CryptographyOid;

        .EXAMPLE
            # Initialize a new instance of the Oid class using the specified value and friendly name.
            $Oid = New-CryptographyOid -Value '1.2.840.113549.1.1.1' -FriendlyName '3DES';

        .EXAMPLE
            # Duplicate an instance of the Oid class.
            $DuplicateOid = New-CryptographyOid -Oid $OriginalOid;

        .EXAMPLE
            # Duplicate instances of the Oid class.
            $DuplicateOidArray = $OriginalOidArray | New-CryptographyOid;
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.oid.aspx
    # >
    [CmdletBinding(DefaultParameterSetName = 'String')]
    [OutputType([System.Security.Cryptography.Oid])]
    Param(
        # The dotted number of the identifier.
        [Parameter(Mandatory = $false, ValueFromPipeline = $true, ParameterSetName = 'String')]
        [Parameter(Mandatory = $true, ParameterSetName = 'Friendly')]
        [ValidatePattern('^[\d]+(\.\d+)*$')]
        [string]$Value,
        
        # The friendly name of the identifier.
        [Parameter(Mandatory = $true, ParameterSetName = 'Friendly')]
        [string]$FriendlyName,
        
        # An object which represents identifier information be duplicated.
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'Oid')]
        [System.Security.Cryptography.Oid]$Oid
    )
    
    switch ($PSCmdlet.ParameterSetName) {
        'Friendly' {
            New-Object -TypeName 'System.Security.Cryptography.Oid' -ArgumentList $Value, $FriendlyName;
            break;
        }
        'Oid' {
            New-Object -TypeName 'System.Security.Cryptography.Oid' -ArgumentList $Oid;
            break;
        }
        default {
            if ($PSBoundParameters.ContainsKey('Value')) {
                New-Object -TypeName 'System.Security.Cryptography.Oid' -ArgumentList $Value;
            } else {
                New-Object -TypeName 'System.Security.Cryptography.Oid'
            }
            break;
        }
    }
}
New-Alias -Name 'New-CryptoObjectIdentifier' -Value 'New-CryptographyOid' -Scope Global -Force;

#>

Function New-AsnEncodedData {
    <#
        .SYNOPSIS
            Create new Abstract Syntax Notation One (ASN.1)-encoded data.

        .DESCRIPTION
            Initializes a new instance of an object which represents Abstract Syntax Notation One (ASN.1)-encoded data.
         
        .INPUTS
            System.Security.Cryptography.AsnEncodedData. Abstract Syntax Notation One (ASN.1)-encoded data to be duplicated.

        .OUTPUTS
            System.Security.Cryptography.AsnEncodedData. Represents Abstract Syntax Notation One (ASN.1)-encoded data.

        .EXAMPLE
            # Initialize a new instance of the AsnEncodedData class
            $AsnEncodedData = New-AsnEncodedData;

        .EXAMPLE
            # Initialize a new instance of the AsnEncodedData class using a byte array
            $AsnEncodedData = New-AsnEncodedData -Oid $OidObject -RawData $ByteArray;

        .EXAMPLE
            # Initialize a new instance of the AsnEncodedData class using an Oid object and a byte array
            $AsnEncodedData = New-AsnEncodedData -RawData $ByteArray -Oid $OidObject;

        .EXAMPLE
            # Duplicate an instance of the AsnEncodedData class.
            $DuplicateAsnEncodedData = New-AsnEncodedData -AsnEncodedData $OriginalAsnEncodedData;

        .EXAMPLE
            # Duplicate instances of the AsnEncodedData class.
            $DuplicateAsnEncodedData = $AsnEncodedDataArray | New-AsnEncodedData;

        .EXAMPLE
            # Initializes a new instance of the AsnEncodedData class using a byte array and a string Oid value
            $OidString = '1.2.840.113549.1.1.1';
            $OidObject = New-CryptographyOid -Oid ($OidString | New-CryptographyOid);
        
        .LINK
            New-CryptographyOid
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.asnencodeddata.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.oid.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'RawData')]
    [OutputType([System.Security.Cryptography.AsnEncodedData])]
    Param(
        # A byte array that contains Abstract Syntax Notation One (ASN.1)-encoded data.
        [Parameter(Mandatory = $false, ParameterSetName = 'RawData')]
        [Parameter(Mandatory = $true, ParameterSetName = 'Oid')]
        [byte[]]$RawData,
        
        # An object which represents a cryptographic object identifier.
        [Parameter(Mandatory = $true, ParameterSetName = 'Oid')]
        [System.Security.Cryptography.Oid]$Oid,
        
        # An instance of the AsnEncodedData class to be duplicated.
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'AsnEncodedData')]
        [System.Security.Cryptography.AsnEncodedData]$AsnEncodedData
    )
    
    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'AsnEncodedData' {
                New-Object -TypeName 'System.Security.Cryptography.AsnEncodedData' -ArgumentList $AsnEncodedData;
                break;
            }
            'Oid' {
                New-Object -TypeName 'System.Security.Cryptography.AsnEncodedData' -ArgumentList ($Oid, $RawData);
                break;
            }
            default {
                if ($PSBoundParameters.ContainsKey('RawData')) {
                    New-Object -TypeName 'System.Security.Cryptography.AsnEncodedData' -ArgumentList $RawData;
                } else {
                    New-Object -TypeName 'System.Security.Cryptography.AsnEncodedData'
                }
                break;
            }
        }
    }
}

Function New-X509StoreOpenFlags {
    <#
        .SYNOPSIS
            Create X.509 certificate store open flags.

        .DESCRIPTION
            Creates a value which specifies the way to open an X.509 certificate store.
         
        .OUTPUTS
            System.Security.Cryptography.X509Certificates.OpenFlags. Specifies the way to open the X.509 certificate store.

        .EXAMPLE
            # Create flags value to open a store in read-only mode, and only if it already exists.
            $OpenFlags = New-X509StoreOpenFlag;
            
        .EXAMPLE
            # Create flags value to open a store in read-write mode, creating it if it does not already exist.
            $OpenFlags = New-X509StoreOpenFlag -Write -CreateOrOpen;

        .EXAMPLE
            # Create flags value to open a store in with max permissions mode, creating it if it does not already exist.
            $OpenFlags = New-X509StoreOpenFlag -MaxAllowed -CreateOrOpen;
            
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.openflags.aspx
    #>
    [CmdletBinding()]
    Param(
        # If store does not exist, it will be created
        [Parameter(Mandatory = $false)]
        [switch]$CreateOrOpen,
        
        # Include archived certificates when opening the store
        [Parameter(Mandatory = $false)]
        [switch]$IncludeArchived,
        
        # Open the X.509 certificate store for the highest access allowed
        [Parameter(Mandatory = $false)]
        [switch]$MaxAllowed,
        
        # Open the X.509 certificate store for both reading and writing
        [Parameter(Mandatory = $false)]
        [switch]$Write
    )
    
    $OpenFlags = [System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly;
    if ($Write) { $OpenFlags = [System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite }
    if (-not $CreateOrOpen) {
        [System.Security.Cryptography.X509Certificates.OpenFlags]$OpenFlags = $OpenFlags -bor [System.Security.Cryptography.X509Certificates.OpenFlags]::OpenExistingOnly;
    }
    if ($MaxAllowed) {
        [System.Security.Cryptography.X509Certificates.OpenFlags]$OpenFlags = $OpenFlags -bor [System.Security.Cryptography.X509Certificates.OpenFlags]::MaxAllowed;
    }
    if ($IncludeArchived) {
        [System.Security.Cryptography.X509Certificates.OpenFlags]$OpenFlags = $OpenFlags -bor [System.Security.Cryptography.X509Certificates.OpenFlags]::IncludeArchived;
    }
    
    $OpenFlags | Write-Output;
}

Function Get-X509Store {
    <#
        .SYNOPSIS
            Create an X.509 store object.

        .DESCRIPTION
            Initializes a new instance of an object representing an X.509 store, which is a physical store where certificates are persisted and managed.
         
        .OUTPUTS
            System.Security.Cryptography.X509Certificates.X509Store. Represents an X.509 store, which is a physical store where certificates are persisted and managed.

        .EXAMPLE
            # Initialize a new instance of the X509Store class using the personal certificates of the current user store.
            # Since this is a disposble object, it is good to ensure it is closed and disposed properly when finished, which this also demonstrates.
            $MyCertsX509Store = Get-X509Store;
            try {
                # Do work
            } finally {
                $X509Store = $null;
            }
        .EXAMPLE
            # Initialize a new instance of the X509Store class using the specified StoreLocation value.
            $LocalMachineX509Store = Get-X509Store -StoreLocation LocalMachine;

        .EXAMPLE
            # Initialize a new instance of the X509Store class using the specified StoreName value for the current user store, and open it for read/write.
            $CurrentUserAddressBookX509Store = Get-X509Store -StoreName AddressBook -Open (New-X509StoreOpenFlags -Write);

        .EXAMPLE
            # Initialize a new instance of the X509Store class using the specified StoreLocation and StoreName values.
            $DisallowedLocalMachineCertsX509Store = Get-X509Store -StoreLocation LocalMachine -StoreName Disallowed;

        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.x509store.aspx
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.storename.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.storelocation.aspx
            
        .Notes
            StoreName Parameter descriptions:
                AddressBook = The X.509 certificate store for other users.
                AuthRoot = The X.509 certificate store for third-party certificate authorities (CAs).
                CertificateAuthority = The X.509 certificate store for intermediate certificate authorities (CAs). 
                Disallowed = The X.509 certificate store for revoked certificates.
                My = The X.509 certificate store for personal certificates.
                Root = The X.509 certificate store for trusted root certificate authorities (CAs).
                TrustedPeople = The X.509 certificate store for directly trusted people and resources.
                TrustedPublisher = The X.509 certificate store for directly trusted publishers.
            StoreLocation Parameter descriptions:
                CurrentUser = The X.509 certificate store used by the current user.
                LocalMachine = The X.509 certificate store assigned to the local machine.
    #>
    [CmdletBinding()]
    Param(
        # Specifies the name of the X.509 certificate store to open.
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.X509Certificates.StoreName]$Name,
        
        # Specifies the location of the X.509 certificate store.
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.X509Certificates.StoreLocation]$Location
    )
    
    $X509Store = $null;
    if ($PSBoundParameters.ContainsKey('Name')) {
        if ($PSBoundParameters.ContainsKey('Location')) {
            New-Object -TypeName 'System.Security.Cryptography.X509Certificates.X509Store' -ArgumentList $Name, $Location;
        } else {
            New-Object -TypeName 'System.Security.Cryptography.X509Certificates.X509Store' -ArgumentList $Location;
        }
    } else {
        if ($PSBoundParameters.ContainsKey('Location')) {
            New-Object -TypeName 'System.Security.Cryptography.X509Certificates.X509Store' -ArgumentList $Name;
        } else {
            New-Object -TypeName 'System.Security.Cryptography.X509Certificates.X509Store';
        }
    }
}

Function New-X509KeyUsageFlags {
    <#
        .SYNOPSIS
            Create X.509 certificate key usage flags.

        .DESCRIPTION
            Creates a value which specifies how a certificate key can be used.
         
        .OUTPUTS
            System.Security.Cryptography.X509Certificates.X509KeyUsageFlags. Defines how a certificate key can be used.

        .EXAMPLE
            # Create flags value with no key usage parameters.
            $UsageFlags = New-X509KeyUsageFlags;
            
        .EXAMPLE
            # Create flags value for key encryption and digital signatures.
            $UsageFlags = New-X509KeyUsageFlags -KeyEncipherment -DigitalSignature;
 
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.x509keyusageflags.aspx
    #>
    [CmdletBinding()]
    Param(
        # Can sign Certificate Revocation Lists.
        [Parameter(Mandatory = $false)]
        [switch]$CrlSign,
        
        # The key can be used for data encryption.
        [Parameter(Mandatory = $false)]
        [switch]$DataEncipherment,
        
        # The key can be used for decryption only.
        [Parameter(Mandatory = $false)]
        [switch]$DecipherOnly,
        
        # The key can be used as a digital signature.
        [Parameter(Mandatory = $false)]
        [switch]$DigitalSignature,
        
        # The key can be used for encryption only.
        [Parameter(Mandatory = $false)]
        [switch]$EncipherOnly,
        
        # The key can be used to determine key agreement, such as a key created using the Diffie-Hellman key agreement algorithm.
        [Parameter(Mandatory = $false)]
        [switch]$KeyAgreement,
        
        # The key can be used to sign certificates.
        [Parameter(Mandatory = $false)]
        [switch]$KeyCertSign,
        
        # The key can be used for key encryption.
        [Parameter(Mandatory = $false)]
        [switch]$KeyEncipherment,

        # The key can be used for authentication.
        [Parameter(Mandatory = $false)]
        [switch]$NonRepudiation 

    )
    $X509KeyUsageFlags = [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]::None;
    if ($CrlSign) {
        [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]$X509KeyUsageFlags = $X509KeyUsageFlags -bor [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]::CrlSign;
    }
    if ($DataEncipherment) {
        [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]$X509KeyUsageFlags = $X509KeyUsageFlags -bor [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]::DataEncipherment;
    }
    if ($DecipherOnly) {
        [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]$X509KeyUsageFlags = $X509KeyUsageFlags -bor [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]::DecipherOnly;
    }
    if ($DigitalSignature) {
        [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]$X509KeyUsageFlags = $X509KeyUsageFlags -bor [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]::DigitalSignature;
    }
    if ($EncipherOnly) {
        [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]$X509KeyUsageFlags = $X509KeyUsageFlags -bor [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]::EncipherOnly;
    }
    if ($KeyAgreement) {
        [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]$X509KeyUsageFlags = $X509KeyUsageFlags -bor [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]::KeyAgreement;
    }
    if ($KeyCertSign) {
        [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]$X509KeyUsageFlags = $X509KeyUsageFlags -bor [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]::KeyCertSign;
    }
    if ($KeyEncipherment) {
        [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]$X509KeyUsageFlags = $X509KeyUsageFlags -bor [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]::KeyEncipherment;
    }
    if ($NonRepudiation) {
        [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]$X509KeyUsageFlags = $X509KeyUsageFlags -bor [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]::NonRepudiation;
    }
    
    $X509KeyUsageFlags | Write-Output;
}

Function Select-X509Certificate {
    <#
        .SYNOPSIS
            Select X.509 certificates from a store.

        .DESCRIPTION
            Selects a collection of certificates from a certificate store with optional search parameters.
         
        .OUTPUTS
            System.Security.Cryptography.X509Certificates.X509Certificate2Collection. Represents a collection of X509Certificate2 objects.

        .EXAMPLE
            # Select all certificates which are valid at the current date and time
            $X509Certificate2Collection = Select-X509Certificate;
        
        .EXAMPLE
            # Select all certificates which are NOT valid at the current date and time
            $X509Certificate2Collection = Select-X509Certificate -Invalid;
        
        .EXAMPLE
            # Select all valid certificates which support key encryption
            $X509Certificate2Collection = Select-X509Certificate -UsageFlags (New-X509KeyUsageFlags -KeyEncipherment);
        
        .EXAMPLE
            # Select all certificates, regardless of their effective dates
            $X509Certificate2Collection = Select-X509Certificate -AllDates;
        
        .EXAMPLE
            # Select all valid certificates which where valid before today, and support digital signatures
            $X509Certificate2Collection = Select-X509Certificate -To ([DateTime]::Now) -UsageFlags (New-X509KeyUsageFlags -DigitalSignature);
        
        .LINK
            New-X509KeyUsageFlags
           
        .LINK
            New-X509Store
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.x509store.aspx
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.x509keyusageflags.aspx
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.x509certificate2collection.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'ByDate')]
    [OutputType([System.Security.Cryptography.X509Certificates.X509Certificate2Collection])]
    Param(
        # The X.509 certificate store from which to retreive certificates
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.X509Certificates.X509Store]$Store,
        
        # Selects only certificates which have usage indications that match this value. You can use 'New-X509KeyUsageFlags' to create this value.
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.X509Certificates.X509KeyUsageFlags]$UsageFlags,
        
        # Select certificates whose valid dates occur on or after this date. If no date range is specified, then only currently valid certificates are selected.
        [Parameter(Mandatory = $false, ParameterSetName = 'ByDate')]
        [DateTime]$From,
        
        # Select certificates whose valid dates occur before this date. If no date range is specified, then only currently valid certificates are selected.
        [Parameter(Mandatory = $false, ParameterSetName = 'ByDate')]
        [DateTime]$To,
        
        # Inverts the 'Date' match - selects certificates which are NOT valid durring the specified date range.
        [Parameter(Mandatory = $false, ParameterSetName = 'ByDate')]
        [switch]$Invalid,
        
        # Selects certificates, regardless of whether their effective dates are valid.
        [Parameter(Mandatory = $true, ParameterSetName = 'All')]
        [switch]$AllDates
    )
    
    $X509Store = $Store;
    if (-not $PSBoundParameters.ContainsKey('Store')) { $X509Store = Get-X509Store }
    $Certificates = New-Object -TypeName 'System.Security.Cryptography.X509Certificates.X509Certificate2Collection';
    $X509Store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::OpenExistingOnly -bor [System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly);
    $FromRange = $null;
    if ($PSBoundParameters.ContainsKey('From')) { $FromRange = $From }
    $ToRange = $null;
    if ($PSBoundParameters.ContainsKey('To')) {
        $FromRange = $To
    } else {
        if ($null -eq $ToRange) {
            $FromRange = [System.DateTime]::Now;
            $ToRange = $FromRange.AddSeconds(1.0);
        }
    }
    
    try {
        foreach ($X509Certificate2 in $X509Store.Certificates) {
            if (-not $AllDates) {
                if ($Invalid) {
                    if ($FromRange -le $X509Certificate2.NotAfter) { continue }
                    if ($ToRange -ge $X509Certificate2.NotBefore) { continue }
                } else {
                    if ($FromRange -gt $X509Certificate2.NotAfter) { continue }
                    if ($ToRange -lt $X509Certificate2.NotBefore) { continue }
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
    <#
        .SYNOPSIS
            Display certificate information.

        .DESCRIPTION
            Displays a dialog box that contains the properties of an X.509 certificate and its associated certificate chain.
         
        .LINK
            Select-X509Certificate
           
        .LINK
            Read-X509Certificate
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.x509certificate2.aspx
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.x509certificate2ui.aspx
    #>
    [CmdletBinding()]
    Param(
        # The X.509 certificate to display.
        [Parameter(Mandatory = $true)]
        [System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate
    )
    
    [System.Security.Cryptography.X509Certificates.X509Certificate2UI]::DisplayCertificate($Certificate);
}

Function Read-X509Certificate {
    <#
        .SYNOPSIS
            Prompt user for certificate selection.

        .DESCRIPTION
            Displays a dialog box for selecting an X.509 certificate from a certificate collection.
         
        .OUTPUTS
            System.Security.Cryptography.X509Certificates.X509Certificate2. Represents a certificate the user has selected.
            System.Security.Cryptography.X509Certificates.X509Certificate2Collection. Represents the certificates the user has selected.

        .EXAMPLE
            # Prompt user to select certificate to use for encryption.
            $X509Certificate2Collection = Select-X509Certificate -UsageFlags (New-X509KeyUsageFlags -KeyEncipherment);
            $X509Certificate2 = Read-X509Certificate -Message 'Select certificate for encryption' -CertificateCollection $X509Certificate2Collection;
            if ($null -eq $X509Certificate2.Count) { 'No certificate was selected.' | Write-Warning }
            
        .EXAMPLE
            # Select certificates which the user might want to export.
            $CertificateCollection = Read-X509Certificate -Message 'Select certificates to export' -MultiSelection;
            if ($null -eq $CertificateCollection) {
                'Certificate selection was canceled' | Write-Warning;
                return;
            }
            if ($CertificateCollection.Count -eq 0) {
                'No certificate was selected.' | Write-Warning;
                return;
            }
            
        .EXAMPLE
            # Select all valid certificates which support key encryption
            $X509Certificate2Collection = Select-X509Certificate -UsageFlags (New-X509KeyUsageFlags -KeyEncipherment);
        
        .EXAMPLE
            # Select all certificates, regardless of their effective dates
            $X509Certificate2Collection = Select-X509Certificate -AllDates;
        
        .EXAMPLE
            # Select all valid certificates which where valid before today, and support digital signatures
            $X509Certificate2Collection = Select-X509Certificate -To ([DateTime]::Now) -UsageFlags (New-X509KeyUsageFlags -DigitalSignature);
        
        .LINK
            Select-X509Certificate
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.x509certificate2.aspx
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.x509certificate2collection.aspx
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.x509certificate2ui.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'Single')]
    [OutputType([System.Security.Cryptography.X509Certificates.X509Certificate2], ParameterSetName = 'Single')]
    [OutputType([System.Security.Cryptography.X509Certificates.X509Certificate2Collection], ParameterSetName = 'Multi')]
    Param(
        # A descriptive message to guide the user. The message is displayed in the dialog box.
        [Parameter(Mandatory = $true)]
        [string]$Message,
        
        # The title of the dialog box.
        [Parameter(Mandatory = $false)]
        [string]$Title = 'Select Certificate',
        
        # A collection of X.509 certificates to select from.
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.X509Certificates.X509Certificate2Collection]$CertificateCollection,
        
        # Select only a single certificate
        [Parameter(Mandatory = $false, ParameterSetName = 'Single')]
        [switch]$SingleSelection,
        
        # Allow user to select one or more certificates
        [Parameter(Mandatory = $true, ParameterSetName = 'Multi')]
        [switch]$MultiSelection
    )
    
    [System.Security.Cryptography.X509Certificates.X509Certificate2Collection]$Certificates = $CertificateCollection;
    if (-not $PSBoundParameters.ContainsKey('CertificateCollection')) {
        [System.Security.Cryptography.X509Certificates.X509Certificate2Collection]$Certificates = Select-X509Certificate;
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
    <#
        .SYNOPSIS
            Create new RSA crypto service provider.

        .DESCRIPTION
            Initializes a new instance of the RSACryptoServiceProvider class.
         
        .OUTPUTS
            System.Security.Cryptography.RSACryptoServiceProvider. Performs asymmetric encryption and decryption using the implementation of the RSA algorithm provided by the cryptographic service provider (CSP). 

        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.cspparameters.aspx
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsacryptoserviceprovider.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Security.Cryptography.RSACryptoServiceProvider])]
    Param(
        # The size of the key to use in bits.
        [Parameter(Mandatory = $false)]
        [int]$KeySize,
        
        # The parameters to be passed to the cryptographic service provider (CSP).
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
    <#
        .SYNOPSIS
            Create AES encryption provider.

        .DESCRIPTION
            Initializes a new instance of the AesManaged class.
         
        .OUTPUTS
            System.Security.Cryptography.AesManaged. Provides a managed implementation of the Advanced Encryption Standard (AES) symmetric algorithm. 

        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode.aspx
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.paddingmode.aspx
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.aesmanaged.aspx
    #>
    [CmdletBinding()]
    [OutputType([System.Security.Cryptography.AesManaged])]
    Param(
        # Size, in bits, of the secret key used for the symmetric algorithm.
        [Parameter(Mandatory = $false)]
        [int]$KeySize = 256,
        
        # Block size, in bits, of the cryptographic operation.
        [Parameter(Mandatory = $false)]
        [int]$BlockSize = 128,
        
        # Initialization vector (IV) to use for the symmetric algorithm.
        [Parameter(Mandatory = $false)]
        [byte[]]$InitializationVector,
        
        # The secret key used for the symmetric algorithm.
        [Parameter(Mandatory = $false)]
        [byte[]]$Key,
        
        # Mode for operation of the symmetric algorithm.
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.CipherMode]$Mode = [System.Security.Cryptography.CipherMode]::CBC,
        
        # Padding mode used in the symmetric algorithm.
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.PaddingMode]$PaddingMode = [System.Security.Cryptography.PaddingMode]::PKCS7
    )
    
    $AesManaged = New-Object -TypeName 'System.Security.Cryptography.AesManaged';
    $AesManaged.BlockSize = $BlockSize;
    $AesManaged.Mode = $Mode;
    $AesManaged.Padding = $PaddingMode;
    if ($PSBoundParameters.ContainsKey('InitializationVector')) { $AesManaged.IV = $InitializationVector; }
    if ($PSBoundParameters.ContainsKey('Key')) {
        if ($PSBoundParameters.ContainsKey('Key')) {
            $AesManaged.KeySize = $KeySize;
        } else {
            $AesManaged.KeySize = $Key.Length;
        }
        $AesManaged.Key = $Key;
    } else {
        $AesManaged.KeySize = $KeySize;
    }
    $AesManaged | Write-Output;
}

Function Protect-WithRSA {
    <#
        .SYNOPSIS
            Encrypt data with RSA.

        .DESCRIPTION
            Encrypts data with the RSA algorithm.
         
        .OUTPUTS
            System.Byte[]. The encrypted data. 

        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsacryptoserviceprovider.aspx
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsaencryptionpadding.aspx
    #>
    [CmdletBinding()]
    [OutputType([byte[]])]
    Param(
        # Cryptographic service provider which will perform the encryption.
        [Parameter(Mandatory = $true)]
        [System.Security.Cryptography.RSACryptoServiceProvider]$RSA,
        
        # The data to be encrypted. 
        [Parameter(Mandatory = $true)]
        [byte[]]$Bytes,
        
        # $true to perform direct RSA encryption using OAEP padding (only available on a computer running Microsoft Windows XP or later); otherwise, $false to use PKCS#1 v1.5 padding. 
        [Parameter(Mandatory = $false)]
        [switch]$OAEP
    )
    
    $encrypted = $encrypted = $RSA.Encrypt($Bytes, $OAEP.IsPresent);
    return ,$encrypted;
}

Function Protect-WithX509Certificate {
    <#
        .SYNOPSIS
            Encrypt data with PKI certificate.

        .DESCRIPTION
            Encrypts data with the PKI certificate's public key.
         
        .OUTPUTS
            System.Byte[]. The encrypted data. 

        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificate2.aspx
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsaencryptionpadding.aspx
    #>
    [CmdletBinding()]
    [OutputType([byte[]])]
    Param(
        # The PKI certificate containing a public key to use for encryption.
        [Parameter(Mandatory = $true)]
        [ValidateScript({ $null -ne $_.PublicKey -and $null -ne $_.PublicKey.Key -and $_.PublicKey.Key -is [System.Security.Cryptography.RSACryptoServiceProvider]})]
        [System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate,
        
        # The data to be encrypted. 
        [Parameter(Mandatory = $true)]
        [byte[]]$Bytes,
        
        # $true to perform direct RSA encryption using OAEP padding (only available on a computer running Microsoft Windows XP or later); otherwise, $false to use PKCS#1 v1.5 padding. 
        [Parameter(Mandatory = $false)]
        [switch]$OAEP
    )
    
    if ($OAEP) {
        Protect-WithRSA -RSA $Certificate.PublicKey.Key -Bytes $Bytes -OAEP;
    } else {
        Protect-WithRSA -RSA $Certificate.PublicKey.Key -Bytes $Bytes;
    }
}

Function New-ProgressWriterObject {
    Param(
        [Parameter(Mandatory = $true)]
        [string]$Activity,
        
        [Parameter(Mandatory = $false)]
        [string]$Status = 'Initializing',
        
        [Parameter(Mandatory = $false)]
        [string]$CurrentOperation,
        
        [Parameter(Mandatory = $false)]
        [int]$StageCount = 1,
        
        [Parameter(Mandatory = $false)]
        [long]$TotalBytes = 0,
        
        [Parameter(Mandatory = $false)]
        [int]$ProgressId,

        [Parameter(Mandatory = $false)]
        [int]$ParentProgressId
    )

    $ProgressObject = @{
        Activity = $Activity;
        Status = $Status;
        StageCount = $StageCount;
        CurrentStage = 1;
        PercentComplete = $null;
        CurrentOperation = $CurrentOperation;
        TotalBytes = $TotalBytes;
        RemainingBytes = $TotalBytes;
        BytesRead = 0;
        HasChanges = $true
    };
    if ($PSBoundParameters.ContainsKey('ProgressId')) {
        $ProgressObject.Add('ProgressId', $ProgressId);
        if ($PSBoundParameters.ContainsKey('ParentProgressId')) { $ProgressObject.Add('ParentProgressId', $ParentProgressId) }
    }
    $ProgressObject = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $ProgressObject;
    $ProgressObject | Add-Member -MemberType ScriptMethod -Name 'WriteProgress' -Value {
        Param(
            [Parameter(Mandatory = $true, Position = 0)]
            [bool]$Completed
        )
        if ($null -ne $this.ProgressId -and $this.HasChanges) {
            $splat = @{ Activity = $this.Activity; Status = $this.Status; Id = $this.ProgressId }
            if ($null -ne $this.PercentComplete) { $splat.Add('PercentComplete', $this.PercentComplete) }
            if ($null -ne $this.ParentProgressId) { $splat.Add('ParentId', $this.ParentProgressId) }
            if ($this.CurrentOperation -ne '') { $splat.Add('CurrentOperation', $this.CurrentOperation) }
            if ($Completed) { $splat.Add('Completed', [switch]$true) }
            Write-Progress @splat;
            $this.HasChanges = $false;
        }
    };
    $ProgressObject | Add-Member -MemberType ScriptMethod -Name 'WriteOperationProgress' -Value {
        Param(
            [Parameter(Mandatory = $true, Position = 0)]
            [string]$CurrentOperation
        )
        if ($this.CurrentOperation -ne $CurrentOperation) { $this.HasChanges = $true }
        $this.CurrentOperation = $CurrentOperation;
        $this.WriteProgress($false);
    };
    $ProgressObject | Add-Member -MemberType ScriptMethod -Name 'SetTotalBytes' -Value {
        Param(
            [Parameter(Mandatory = $true, Position = 0)]
            [long]$TotalBytes
        )
        if ($this.TotalBytes -ne $TotalBytes) { $this.HasChanges = $true }
        $this.BytesRead = [long]0;
        $this.RemainingBytes = $TotalBytes;
        $this.TotalBytes = $TotalBytes;
    };
    $ProgressObject | Add-Member -MemberType ScriptMethod -Name 'IncrementStage' -Value {
        Param(
            [Parameter(Mandatory = $true, Position = 0)]
            [long]$TotalBytes
        )
        if ($this.StageCount -gt $this.CurrentStage) {
            $this.CurrentStage++;
            $this.HasChanges = $true;
            $this.PercentComplete = (($this.CurrentStage - 1) * 100) / $this.StageCount;
        } else {
            if ($this.TotalBytes -ne $TotalBytes) { $this.HasChanges = $true }
        }

        $this.BytesRead = [long]0;
        $this.RemainingBytes = $TotalBytes;
        $this.TotalBytes = $TotalBytes;
    };
    $ProgressObject | Add-Member -MemberType ScriptMethod -Name 'WriteStatusProgress' -Value {
        Param(
            [Parameter(Mandatory = $true, Position = 0)]
            [string]$Activity,
            [Parameter(Mandatory = $true, Position = 1)]
            [string]$Status,
            [Parameter(Mandatory = $false, Position = 2)]
            [AllowEmptyString()]
            [string]$CurrentOperation,
            [Parameter(Mandatory = $false, Position = 3)]
            [int]$PercentComplete
        )
        if ($this.Activity -ne $Activity -or $this.Status -ne $Status -or $this.CurrentOperation -ne $CurrentOperation) { $this.HasChanges = $true }
        $this.Activity = $Activity;
        $this.Status = $Status;
        $this.CurrentOperation = $CurrentOperation;
        if ($PSBoundParameters.ContainsKey('PercentComplete')) {
            if ($this.PercentComplete -ne $PercentComplete) { $this.HasChanges = $true }
            $this.PercentComplete = $PercentComplete;
        }
        $this.WriteProgress($false);
    };
    $ProgressObject | Add-Member -MemberType ScriptMethod -Name 'UpdateBytesRead' -Value {
        Param(
            [Parameter(Mandatory = $true, Position = 0)]
            [long]$Count,
            [Parameter(Mandatory = $false, Position = 1)]
            [string]$Format = 'Processed {0} bytes of {1}',
            [Parameter(Mandatory = $false, Position = 2)]
            [double]$Delta = 1.0
        )
        if ($Count -ne 0) { $this.HasChanges = $true }
        $this.BytesRead += $Count;
        $this.RemainingBytes -= $Count;
        if ($this.TotalBytes -lt 1 -or $this.BytesRead -lt 0) {
            [int]$pct = 0;
        } else {
            if ($this.BytesRead -ge $this.TotalBytes) {
                [int]$pct = ($this.CurrentStage * 100) / $this.StageCount;
            } else {
                [int]$pct = ($this.BytesRead * 100) / $this.TotalBytes;
                if ($this.StageCount -gt 1) {
                    [int]$pct = ($pct / $this.StageCount) + ((($this.CurrentStage - 1) * 100) / $this.StageCount);
                }
            }
        }
        if ($pct -ne $this.PercentComplete) { $this.HasChanges = $true }
        $this.PercentComplete = $pct;
        $this.WriteOperationProgress(($Format -f $this.BytesRead, $this.TotalBytes));
    };
    $ProgressObject | Add-Member -MemberType ScriptMethod -Name 'WriteCompleted' -Value {
        Param(
            [bool]$Success
        )
        $this.CurrentOperation = '';
        if ($Success) {
            $this.Status = 'Success';
        } else {
            $this.Status = 'Fail';
        }
        $this.WriteProgress($true);
    };
    return $ProgressObject;
}

Function Protect-WithSymmetricAlgorithm {
    <#
        .SYNOPSIS
            Encrypt data with symmetric algorithm.

        .DESCRIPTION
            Encrypts data with symmetric encryption, using the PKI certificate's public key to encrypt the symmetric encryption key.
         
        .OUTPUTS
            System.Int64. The number of bytes encrypted. Due to header information being written to the output stream, this will be less than the total number of bytes written. 

        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificate2.aspx
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsaencryptionpadding.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'Implicit')]
    [OutputType([long])]
    Param(
        # The PKI certificate containing a public key to use for symmetric key encryption.
        [Parameter(Mandatory = $true, ParameterSetName = 'Implicit')]
        [Parameter(Mandatory = $true, ParameterSetName = 'CertificateExplicit')]
        [ValidateScript({ $null -ne $_.PublicKey -and $null -ne $_.PublicKey.Key -and $_.PublicKey.Key -is [System.Security.Cryptography.RSACryptoServiceProvider]})]
        [System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate,
        
        # Encryption provider to use for symmetric key encryption.
        [Parameter(Mandatory = $true, ParameterSetName = 'RSAExplicit')]
        [System.Security.Cryptography.RSACryptoServiceProvider]$RSA,
        
        # Stream containing data to be encrypted.
        [Parameter(Mandatory = $true)]
        [System.IO.Stream]$InputStream,
        
        # Destination stream for encrypted data.
        [Parameter(Mandatory = $true)]
        [System.IO.Stream]$OutputStream,
        
        # Symmetric algorithm to use for encryption.
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.SymmetricAlgorithm]$SymmetricAlgorithm,
        
        # Identifier to use for progress indicator. If this is not specified, then no progess indicator will be used.
        [Parameter(Mandatory = $false)]
        [int]$ProgressId,

        # Id of parent progress indicator.
        [Parameter(Mandatory = $false)]
        [int]$ParentProgressId,
        
        # $true to perform direct RSA encryption using OAEP padding (only available on a computer running Microsoft Windows XP or later); otherwise, $false to use PKCS#1 v1.5 padding. 
        [Parameter(Mandatory = $false, ParameterSetName = "Implicit")]
        [switch]$OAEP
    )
    
    $TotalSourceLength = $InputStream.Length - $InputStream.Position;
    if ($TotalSourceLength -eq 0) { return $TotalSourceLength }
    if ($PSBoundParameters.ContainsKey('ProgressId')) {
        if ($PSBoundParameters.ContainsKey('ParentProgressId')) {
            $ProgressObject = New-ProgressWriterObject -Activity 'Encrypting Data' -TotalBytes $TotalSourceLength -StageCount 2 -ProgressId $ProgressId -ParentProgressId $ParentProgressId;
        } else {
            $ProgressObject = New-ProgressWriterObject -Activity 'Encrypting Data' -TotalBytes $TotalSourceLength -StageCount 2 -ProgressId $ProgressId;
        }
    } else {
        $ProgressObject = New-ProgressWriterObject -Activity 'Encrypting Data' -TotalBytes $TotalSourceLength -StageCount 2;
    }

    try {
        $Algorithm = $SymmetricAlgorithm;
        if (-not $PSBoundParameters.ContainsKey('SymmetricAlgorithm')) {
            $ProgressObject.WriteOperationProgress('Initializing data encryption provider');
            $Algorithm = New-AesManaged -ErrorAction Stop;
        }

        $ProgressObject.WriteOperationProgress('Writing encryption data headers');
		[Erwine.Leonard.T.CertificateCryptography.StreamHelper]::WriteLengthEncodedBytes($OutputStream, $Algorithm.IV, 0, $Algorithm.IV.Length);

        $splat = @{ Bytes = $Algorithm.Key; ErrorAction = ([System.Management.Automation.ActionPreference]::Stop) };
        if ($OAEP) { $splat.Add('OAEP', [switch]$true) }
        if ($PSBoundParameters.ContainsKey('Certificate')) {
            $splat.Add('Certificate', $Certificate);
            $EncryptedKey = Protect-WithX509Certificate @splat;
        } else {
            $splat.Add('RSA', $RSA);
            $EncryptedKey = Protect-WithRSA @splat;
        }
		[Erwine.Leonard.T.CertificateCryptography.StreamHelper]::WriteLengthEncodedBytes($OutputStream, $EncryptedKey, 0, $EncryptedKey.Length);
		[Erwine.Leonard.T.CertificateCryptography.StreamHelper]::WriteLongInteger($OutputStream, $ProgressObject.TotalBytes);
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
            $ProgressObject.WriteOperationProgress('Opening cryptographic stream');
            $CryptoStream = New-Object -TypeName 'System.Security.Cryptography.CryptoStream' -ArgumentList ($FileStream, $ICryptoTransform, `
                [System.Security.Cryptography.CryptoStreamMode]::Write) -ErrorAction Stop;
        } catch {
            $FileStream.Dispose();
            throw;
        }
    
        try {
            $ProgressObject.WriteStatusProgress('Encrypting Data', 'Working');
			[int]$BlockSizeBytes = $Algorithm.BlockSize / 8;
		    $Buffer = New-Object -TypeName 'System.Byte[]' -ArgumentList $BlockSizeBytes;
		    for ($count = $InputStream.Read($Buffer, 0, $BlockSizeBytes); $count -gt 0 -and $ProgressObject.BytesRead -lt $ProgressObject.TotalBytes; `
					$count = $InputStream.Read($Buffer, 0, $BlockSizeBytes)) {
				$CryptoStream.Write($Buffer, 0, $BlockSizeBytes);
                $ProgressObject.UpdateBytesRead($count, 'Encrypted {0} bytes of {1}');
		    }
            $CryptoStream.FlushFinalBlock();
            $FileStream.Seek(0L, [System.IO.SeekOrigin]::Begin) | Out-Null;
            $ProgressObject.IncrementStage($FileStream.Length);
            $ProgressObject.WriteStatusProgress('Saving Data', 'Working');
			$BlockSizeBytes = 32768;
		    $Buffer = New-Object -TypeName 'System.Byte[]' -ArgumentList $BlockSizeBytes;
		    for ($count = $FileStream.Read($Buffer, 0, $BlockSizeBytes); $count -gt 0; $count = $FileStream.Read($Buffer, 0, $BlockSizeBytes)) {
				$OutputStream.Write($Buffer, 0, $count);
                $ProgressObject.UpdateBytesRead($count, 'Saved {0} bytes of {1}');
		    }
            $TotalSourceLength | Write-Output;
        } catch { throw; }
        finally {
            $CryptoStream.Close();
            $CryptoStream.Dispose();
		    if ([System.IO.File]::Exists($Path)) { [System.IO.File]::Delete($Path) }
        }
        $ProgressObject.WriteCompleted($true);
    } catch {
        $ProgressObject.WriteCompleted($false);
        throw;
    }
}

Function Unprotect-WithRSA {
    <#
        .SYNOPSIS
            Decrypt data with RSA.

        .DESCRIPTION
            Decrypts data with the RSA algorithm.
         
        .OUTPUTS
            System.Byte[]. The decrypted data. 

        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsacryptoserviceprovider.aspx
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsaencryptionpadding.aspx
    #>
    [CmdletBinding()]
    [OutputType([byte[]])]
    Param(
        # Cryptographic service provider which will perform the decryption.
        [Parameter(Mandatory = $true)]
        [System.Security.Cryptography.RSACryptoServiceProvider]$RSA,
        
        # The encryptd data.
        [Parameter(Mandatory = $true)]
        [byte[]]$Bytes,
        
        # $true to perform direct RSA decryption using OAEP padding (only available on a computer running Microsoft Windows XP or later); otherwise, $false to use PKCS#1 v1.5 padding. 
        [Parameter(Mandatory = $false)]
        [switch]$OAEP
    )
    
    $decrypted = $RSA.Decrypt($Bytes, $OAEP.IsPresent);
    return ,$decrypted;
}

Function Unprotect-WithX509Certificate {
    <#
        .SYNOPSIS
            Decrypt data with PKI certificate.

        .DESCRIPTION
            Decrypts data with the PKI certificate's private key.
         
        .OUTPUTS
            System.Byte[]. The decrypted data. 

        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificate2.aspx
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsaencryptionpadding.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = "Implicit")]
    [OutputType([byte[]])]
    Param(
        # The PKI certificate containing a private key to use for encryption.
        [Parameter(Mandatory = $true)]
        [ValidateScript({ $null -ne $_.PrivateKey -and $_.PrivateKey -is [System.Security.Cryptography.RSACryptoServiceProvider]})]
        [System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate,
        
        # The encryptd data.
        [Parameter(Mandatory = $true)]
        [byte[]]$Bytes,
        
        # $true to perform direct RSA encryption using OAEP padding (only available on a computer running Microsoft Windows XP or later); otherwise, $false to use PKCS#1 v1.5 padding. 
        [Parameter(Mandatory = $false, ParameterSetName = "Implicit")]
        [switch]$OAEP
    )
    
    if ($OAEP) {
        Unprotect-WithRSA -RSA $Certificate.PrivateKey -Bytes $Bytes -OAEP;
    } else {
        Unprotect-WithRSA -RSA $Certificate.PrivateKey -Bytes $Bytes;
    }
}

Function Unprotect-WithSymmetricAlgorithm {
    <#
        .SYNOPSIS
            Decrypt data with symmetric algorithm.

        .DESCRIPTION
            Decrypts data with symmetric encryption, using the PKI certificate's private key to decrypt the symmetric encryption key.
         
        .OUTPUTS
            System.Int64. The number of bytes decrypted. 

        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificate2.aspx
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsaencryptionpadding.aspx
    #>
    [CmdletBinding(DefaultParameterSetName = 'Implicit')]
    [OutputType([long])]
    Param(
        # The PKI certificate containing a private key to use for symmetric key encryption.
        [Parameter(Mandatory = $true, ParameterSetName = 'Implicit')]
        [Parameter(Mandatory = $true, ParameterSetName = 'CertificateExplicit')]
        [ValidateScript({ $null -ne $_.PrivateKey -and $_.PrivateKey -is [System.Security.Cryptography.RSACryptoServiceProvider]})]
        [System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate,
        
        # Encryption provider to use for symmetric key encryption.
        [Parameter(Mandatory = $true, ParameterSetName = 'RSAExplicit')]
        [System.Security.Cryptography.RSACryptoServiceProvider]$RSA,
        
        # Stream containing encrypted data.
        [Parameter(Mandatory = $true)]
        [System.IO.Stream]$InputStream,
        
        # Destination stream for decrypted data.
        [Parameter(Mandatory = $true)]
        [System.IO.Stream]$OutputStream,
        
        # Symmetric algorithm to use for decryption.
        [Parameter(Mandatory = $false)]
        [System.Security.Cryptography.SymmetricAlgorithm]$SymmetricAlgorithm,
        
        # Identifier to use for progress indicator. If this is not specified, then no progess indicator will be used.
        [Parameter(Mandatory = $false)]
        [int]$ProgressId,

        # Id of parent progress indicator.
        [Parameter(Mandatory = $false)]
        [int]$ParentProgressId,
        
        # $true to perform direct RSA decryption using OAEP padding (only available on a computer running Microsoft Windows XP or later); otherwise, $false to use PKCS#1 v1.5 padding. 
        [Parameter(Mandatory = $false, ParameterSetName = "Implicit")]
        [switch]$OAEP
    )

    if ($InputStream.Length -eq $InputStream.Position) { return [long]0 }

    if ($PSBoundParameters.ContainsKey('ProgressId')) {
        if ($PSBoundParameters.ContainsKey('ParentProgressId')) {
            $ProgressObject = New-ProgressWriterObject -Activity 'Decrypting Data' -ProgressId $ProgressId -ParentProgressId $ParentProgressId;
        } else {
            $ProgressObject = New-ProgressWriterObject -Activity 'Decrypting Data' -ProgressId $ProgressId;
        }
    } else {
        $ProgressObject = New-ProgressWriterObject -Activity 'Decrypting Data';
    }
    
    try {
        $Algorithm = $SymmetricAlgorithm;
        if (-not $PSBoundParameters.ContainsKey('SymmetricAlgorithm')) {
            $ProgressObject.WriteOperationProgress('Initializing data encryption provider');
            $Algorithm = New-AesManaged;
        }
        $ProgressObject.WriteOperationProgress('Reading encryption data headers');

        $Algorithm.IV = Read-LengthEncodedBytes -Stream $InputStream -ErrorAction Stop;
        [byte[]]$EncryptedKey = Read-LengthEncodedBytes -Stream $InputStream -ErrorAction Stop;
    
        if ($PSBoundParameters.ContainsKey('Certificate')) {
            if ($OAEP) {
                $Algorithm.Key = Unprotect-WithX509Certificate -Certificate $Certificate -Bytes $EncryptedKey -OAEP;
            } else {
                $Algorithm.Key = Unprotect-WithX509Certificate -Certificate $Certificate -Bytes $EncryptedKey;
            }
        } else {
            if ($OAEP) {
                $Algorithm.Key = Unprotect-WithRSA -RSA $RSA -Bytes $EncryptedKey -OAEP;
            } else {
                $Algorithm.Key = Unprotect-WithRSA -RSA $RSA -Bytes $EncryptedKey;
            }
        }
    
        [long]$TotalSourceLength = Read-LongIntegerFromStream -Stream $InputStream;
        if ($TotalSourceLength -lt 0) { throw 'Invalid data length' }

        $ProgressObject.SetTotalBytes($TotalSourceLength);
    
        if ($TotalSourceLength -gt 0) {
            $ICryptoTransform = $Algorithm.CreateDecryptor();
            $ProgressObject.WriteOperationProgress('Opening cryptographic stream');
            $CryptoStream = New-Object -TypeName 'System.Security.Cryptography.CryptoStream' -ArgumentList ($InputStream, $ICryptoTransform, `
                [System.Security.Cryptography.CryptoStreamMode]::Read);
        
            try {
                $ProgressObject.WriteStatusProgress('Decrypting Data', 'Working');
				[int]$BlockSizeBytes = $Algorithm.BlockSize / 8;
				$Buffer = New-Object -TypeName 'System.Byte[]' -ArgumentList $BlockSizeBytes;
			    for ($count =  $CryptoStream.Read($Buffer, 0, $BlockSizeBytes); $count -gt 0; $count = $CryptoStream.Read($Buffer, 0, $BlockSizeBytes)) {
                    if ($ProgressObject.RemainingBytes -lt 1) { continue }
					if ($ProgressObject.RemainingBytes -lt [long]$count) { [int]$count = $ProgressObject.RemainingBytes; }
					$OutputStream.Write($Buffer, 0, $count);
                    $ProgressObject.UpdateBytesRead($count, 'Decrypted {0} bytes of {1}');
			    }
            } catch { throw; }
            finally {
                $CryptoStream.Close();
                $CryptoStream.Dispose();
            }
        }
        $TotalSourceLength | Write-Output;
        $ProgressObject.WriteCompleted($true);
    } catch {
        $ProgressObject.WriteCompleted($false);
        throw;
    }
}

<#
Function New-X500DistinguishedName {
    < #
        .SYNOPSIS
            Create new X500 Distinguished Name.

        .DESCRIPTION
            Create new object which represents the distinguished name of an X509 certificate.
         
        .OUTPUTS
            System.Security.Cryptography.X509Certificates.X500DistinguishedName. Represents the distinguished name of an X509 certificate.

        .LINK
            New-AsnEncodedData

        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.asnencodeddata.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.x500distinguishednameflags.aspx

        .LINK
            https://msdn.microsoft.com/en-us/library/system.security.cryptography.x500distinguishedname.aspx
    # >
    [CmdletBinding(DefaultParameterSetName = 'String')]
    [OutputType([System.Security.Cryptography.X509Certificates.X500DistinguishedName])]
    Param(
        # An AsnEncodedData object that represents the distinguished name.
        [Parameter(Mandatory = $true, ParameterSetName = 'AsnEncoded')]
        [System.Security.Cryptography.AsnEncodedData]$AsnEncodedData,
        
        # A byte array that contains distinguished name information.
        [Parameter(Mandatory = $true, ParameterSetName = 'Bytes')]
        [byte[]]$Bytes,
        
        # A string that represents the distinguished name.
        [Parameter(Mandatory = $true, ParameterSetName = 'String')]
        [string]$DistinguishedName,
        
        # A bitwise combination of the enumeration values that specify the characteristics of the distinguished name.
        [Parameter(Mandatory = $false, ParameterSetName = 'String')]
        [System.Security.Cryptography.X509Certificates.X500DistinguishedNameFlags]$Flags
    )
}

Function New-SelfSignedCertificateExample {
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true, ValueFromPipelineByPropertyName=$true)]
        [string]$CN
    )

    $CX500DistinguishedName = New-Object -COM 'X509Enrollment.CX500DistinguishedName.1';
    $CX500DistinguishedName.Encode(('CN={0}' -f $CN), 0);

    $CX509PrivateKey = New-Object -COM 'X509Enrollment.CX509PrivateKey.1';
    $CX509PrivateKey.ProviderName = 'Microsoft RSA SChannel Cryptographic Provider';
    $CX509PrivateKey.KeySpec = 1;
    $CX509PrivateKey.Length = 1024;
    $CX509PrivateKey.SecurityDescriptor = 'D:PAI(A;;0xd01f01ff;;;SY)(A;;0xd01f01ff;;;BA)(A;;0x80120089;;;NS)';
    $CX509PrivateKey.MachineContext = 1;
    $CX509PrivateKey.Create();

    $ServerUuthOid = New-Object -COM 'X509Enrollment.CObjectId.1';
    $ServerUuthOid.InitializeFromValue('1.3.6.1.5.5.7.3.1');
    $EkuOids = New-Object -COM 'X509Enrollment.CObjectIds.1';
    $EkuOids.Add($ServerUuthOid);
    $EkuExt = New-Object -COM 'X509Enrollment.CX509ExtensionEnhancedKeyUsage.1';
    $EkuExt.InitializeEncode($EkuOids);

    $CX509CertificateRequestCertificate = New-Object -COM 'X509Enrollment.CX509CertificateRequestCertificate.1';
    $CX509CertificateRequestCertificate.InitializeFromPrivateKey(2, $CX509PrivateKey, '');
    $CX509CertificateRequestCertificate.Subject = $CX500DistinguishedName;
    $CX509CertificateRequestCertificate.Issuer = $CX500DistinguishedName.Subject;
    $CX509CertificateRequestCertificate.NotBefore = Get-Date;
    $CX509CertificateRequestCertificate.NotAfter = $CX509CertificateRequestCertificate.NotBefore.AddDays($ExpireAfterDays);
    $CX509CertificateRequestCertificate.X509Extensions.Add($EkuExt);
    $CX509CertificateRequestCertificate.Encode();

    $CX509Enrollment = new-object -com 'X509Enrollment.CX509Enrollment.1';
    $CX509Enrollment.InitializeFromRequest($CX509CertificateRequestCertificate);
    $CertData = $CX509Enrollment.CreateRequest(0);
    $CX509Enrollment.InstallResponse(2, $CertData, 0, '');
}

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

Function New-SelfSignedCertificate {
    [CmdletBinding()]
    [OutputType([System.Security.Cryptography.X509Certificates.X509Certificate2])]
    Param (
        [Parameter(Mandatory=$true, ValueFromPipelineByPropertyName=$true)]
        [string]$CN
    )
    
    @'
    // create DN for subject and issuer
    var dn = new CX500DistinguishedName();
    dn.Encode("CN=" + subjectName, X500NameFlags.XCN_CERT_NAME_STR_NONE);

    // create a new private key for the certificate
    CX509PrivateKey privateKey = new CX509PrivateKey();
    privateKey.ProviderName = "Microsoft Base Cryptographic Provider v1.0";
    privateKey.MachineContext = true;
    privateKey.Length = 2048;
    privateKey.KeySpec = X509KeySpec.XCN_AT_SIGNATURE; // use is not limited
    privateKey.ExportPolicy = X509PrivateKeyExportFlags.XCN_NCRYPT_ALLOW_PLAINTEXT_EXPORT_FLAG;
    privateKey.Create();

    // Use the stronger SHA512 hashing algorithm
    var hashobj = new CObjectId();
    hashobj.InitializeFromAlgorithmName(ObjectIdGroupId.XCN_CRYPT_HASH_ALG_OID_GROUP_ID,
        ObjectIdPublicKeyFlags.XCN_CRYPT_OID_INFO_PUBKEY_ANY, 
        AlgorithmFlags.AlgorithmFlagsNone, "SHA512");

    // add extended key usage if you want - look at MSDN for a list of possible OIDs
    var oid = new CObjectId();
    oid.InitializeFromValue("1.3.6.1.5.5.7.3.1"); // SSL server
    var oidlist = new CObjectIds();
    oidlist.Add(oid);
    var eku = new CX509ExtensionEnhancedKeyUsage();
    eku.InitializeEncode(oidlist); 

    // Create the self signing request
    var cert = new CX509CertificateRequestCertificate();
    cert.InitializeFromPrivateKey(X509CertificateEnrollmentContext.ContextMachine, privateKey, "");
    cert.Subject = dn;
    cert.Issuer = dn; // the issuer and the subject are the same
    cert.NotBefore = DateTime.Now;
    // this cert expires immediately. Change to whatever makes sense for you
    cert.NotAfter = DateTime.Now; 
    cert.X509Extensions.Add((CX509Extension)eku); // add the EKU
    cert.HashAlgorithm = hashobj; // Specify the hashing algorithm
    cert.Encode(); // encode the certificate

    // Do the final enrollment process
    var enroll = new CX509Enrollment();
    enroll.InitializeFromRequest(cert); // load the certificate
    enroll.CertificateFriendlyName = subjectName; // Optional: add a friendly name
    string csr = enroll.CreateRequest(); // Output the request in base64
    // and install it back as the response
    enroll.InstallResponse(InstallResponseRestrictionFlags.AllowUntrustedCertificate,
        csr, EncodingType.XCN_CRYPT_STRING_BASE64, ""); // no password
    // output a base64 encoded PKCS#12 so we can import it back to the .Net security classes
    var base64encoded = enroll.CreatePFX("", // no password, this is for internal consumption
        PFXExportOptions.PFXExportChainWithRoot);

    // instantiate the target class with the PKCS#12 data (and the empty password)
    return new System.Security.Cryptography.X509Certificates.X509Certificate2(
        System.Convert.FromBase64String(base64encoded), "", 
        // mark the private key as exportable (this is usually what you want to do)
        System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable
    );
'@
}
#>