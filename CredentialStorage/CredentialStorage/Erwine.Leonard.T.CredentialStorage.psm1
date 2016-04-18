if ((Get-Module -Name 'Erwine.Leonard.T.IOUtility') -eq $null) { Import-Module -Name 'Erwine.Leonard.T.IOUtility' -ErrorAction Stop }
if ((Get-Module -Name 'Erwine.Leonard.T.XmlUtility') -eq $null) { Import-Module -Name 'Erwine.Leonard.T.XmlUtility' -ErrorAction Stop }
Add-Type -Path 'CredentialComponentCollection.cs', 'CredentialFolder.cs', 'CredentialItem.cs', 'CredentialStorageComponent.cs', 'CredentialStorageContainer.cs',
	'CredentialStorageDocument.cs', 'ICredentialComponent.cs', 'ICredentialContainer.cs', 'ICredentialContent.cs', 'ICredentialSite.cs', 'INestedCredentialContainer.cs' `
	-ReferencedAssemblies 'System', 'System.Management.Automation', 'System.Xml';

$Script:CredentialsNamespace = 'urn:Erwine.Leonard.T:PowerShell:CredentialStorage';
$Script:RootElementName = 'CredentialStorage';
$Script:FolderElementName = 'Folder';
$Script:CredentialElementName = 'Credential';
$Script:CurrentCredentialDocument = New-CredentialStorageDocument;
$Script:ContainerStack = @($Script:CurrentCredentialDocument);

Function New-CredentialStorageDocument {
    [CmdletBinding()]
    [OutputType([System.Xml.XmlDocument])]
    Param()
    
	New-Object -TypeName 'CredentialStorageCLR.CredentialStorageDocument';
}

Function Open-CredentialStorageDocument {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$Path,

		[switch]$AsCurrent
    )
    
    $XmlSerializer = New-Object -TypeName 'System.Xml.Serialization.XmlSerializer' -ArgumentList ([CredentialStorageCLR.CredentialStorageDocument]);
	$XmlReader = [System.Xml.XmlReader]::Create($Path);
	try {
		[CredentialStorageCLR.CredentialStorageDocument]$CredentialStorageDocument = $XmlSerializer.Deserialize($XmlReader);
		if ($AsCurrent) {
			$Script:CurrentCredentialDocument = $CredentialStorageDocument;
			$Script:ContainerStack = @($Script:CurrentCredentialDocument);
		}
	} catch {
		throw;
	} finally {
		$XmlReader.Dispose();
	}

	$Script:ContainerStack[0] | Write-Output;
}

Function Save-CredentialStorageDocument {
    [CmdletBinding(DefaultParameterSetName = 'Path')]
    [OutputType([System.Xml.XmlDocument], ParameterSetName = 'Path')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Path')]
        [string]$Path,

		[CredentialStorageCLR.CredentialStorageDocument]$Document
    )

	if ($PSBoundParameters.ContainsKey('CredentialStorageDocument')) {
		$CredentialStorageDocument = $Document;
	} else {
		$CredentialStorageDocument = $Script:CurrentCredentialDocument;
	}
	
	$Settings = New-Object -TypeName 'System.Xml.XmlWriterSettings' -Property @{
		CloseOutput = $true;
		Indent = $true;
	};
    $XmlSerializer = New-Object -TypeName 'System.Xml.Serialization.XmlSerializer' -ArgumentList ([CredentialStorageCLR.CredentialStorageDocument]);
	$XmlWriter = [System.Xml.XmlWriter]::Create($Path, $Settings);
	try {
		$XmlSerializer.Serialize($XmlWriter, $CredentialStorageDocument);
	} catch {
		throw;
	} finally {
		$XmlReader.Dispose();
	}
}

Function Enter-CredentialFolder {
    [CmdletBinding(DefaultParameterSetName = 'Path')]
    Param(
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Name')]
        [string]$Name
    )
    
    Process {
		$Folder = @($Script:ContainerStack[0].Contents | Where-Object { $_.DisplayText -ieq $Name });
		if ($Folder.Count -eq 0) {
			throw 'Name not found.';
		} else {
			$Script:ContainerStack += @($Folder[0]);
			$Script:ContainerStack[0] | Write-Output;
		}
    }
}

Function Exit-CredentialFolder {
    [CmdletBinding()]
    Param()

	if ($Script:ContainerStack.Count -gt 1) { $Script:ContainerStack = @($Script:ContainerStack[1..($Script.ContainerStack.Count)]) }

	$Script:ContainerStack[0] | Write-Output;
}

Function New-CredentialFolder {
    [CmdletBinding()]
    [OutputType([CredentialStorageCLR.CredentialFolder])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$Name,
		
        [Parameter(Position = 1)]
        [System.ICredentialContainer]$Container
    )

	if ($PSBoundParameters.ContainsKey('Container')) {
		$CredentialContainer = $Container;
	} else {
		$CredentialContainer = $Script:ContainerStack[0];
	}
	
	$CredentialFolder = New-Object -TypeName 'CredentialStorageCLR.CredentialStorageDocument';
	$CredentialFolder.DisplayText = $Name;
	$Container.Contents.Add($CredentialFolder);
	$CredentialFolder | Write-Output;
}

Function New-CredentialDomain {
    [CmdletBinding()]
    [OutputType([CredentialStorageCLR.CredentialDomain])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$Name,
		
        [Parameter(Position = 1)]
        [System.ICredentialContainer]$Container
    )

	if ($PSBoundParameters.ContainsKey('Container')) {
		$CredentialContainer = $Container;
	} else {
		$CredentialContainer = $Script:ContainerStack[0];
	}
	
	$CredentialDomain = New-Object -TypeName 'CredentialStorageCLR.CredentialDomain';
	$CredentialDomain.DisplayText = $Name;
	$Container.Contents.Add($CredentialDomain);
	$CredentialDomain | Write-Output;
}

Function New-StoredCredential {
    [CmdletBinding(DefaultParameterSetName = 'Lookup')]
    [OutputType([CredentialStorageCLR.StoredCredential])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$CredentialName,
		
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Lookup')]
        [string]$DomainName,
		
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Object')]
        [System.CredentialDomain]$Domain
    )
	
	if ($PSCmdlet.ParameterSetName -eq 'Lookup') {
		$Domain = @($Script:ContainerStack[0].Contents | Where-Object { $_.DisplayText -ieq $DomainName });
		if ($Domain.Count -eq 0) {
			throw 'Name not found.';
		} else {
			$Domain = $Domain[0];
		}
	}

	$StoredCredential = New-Object -TypeName 'CredentialStorageCLR.CredentialDomain';
	$StoredCredential.DisplayText = $Name;
	$Domain.Credentials.Add($StoredCredential);
	$StoredCredential | Write-Output;
}