if ((Get-Module -Name 'Erwine.Leonard.T.IOUtility') -eq $null) { Import-Module -Name 'Erwine.Leonard.T.IOUtility' -ErrorAction Stop }
if ((Get-Module -Name 'Erwine.Leonard.T.XmlUtility') -eq $null) { Import-Module -Name 'Erwine.Leonard.T.XmlUtility' -ErrorAction Stop }

Function Initialize-CurrentModule {
    [CmdletBinding()]
    Param()
	
	$Local:BaseName = $PSScriptRoot | Join-Path -ChildPath $MyInvocation.MyCommand.Module.Name;
	
    $Local:ModuleManifest = Test-ModuleManifest -Path ($PSScriptRoot | Join-Path -ChildPath ('{0}.psd1' -f $MyInvocation.MyCommand.Module.Name));
    $Local:Assemblies = @($Local:ModuleManifest.PrivateData.CompilerOptions.AssemblyReferences | ForEach-Object {
        (Add-Type -AssemblyName $_ -PassThru)[0].Assembly.Location
    });
    $Local:Splat = @{
        TypeName = 'System.CodeDom.Compiler.CompilerParameters';
        ArgumentList = (,$Local:Assemblies);
        Property = @{
            IncludeDebugInformation = $Local:ModuleManifest.PrivateData.CompilerOptions.IncludeDebugInformation;
        }
    };
    if ($Local:ModuleManifest.PrivateData.CompilerOptions.ConditionalCompilationSymbols -ne '') {
        $Local:Splat.Property.CompilerOptions = '/define:' + $Local:ModuleManifest.PrivateData.CompilerOptions.ConditionalCompilationSymbols;
    }

    $Script:AssemblyPath = @(Add-Type -Path ($Local:ModuleManifest.PrivateData.CompilerOptions.CustomTypeSourceFiles | ForEach-Object { $PSScriptRoot | Join-Path -ChildPath $_ }) -CompilerParameters (New-Object @Local:Splat) -PassThru)[0].Assembly.Location;
}

Initialize-CurrentModule;

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

Function Resolve-CredentialLocation {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [string]$Path
    )
    
    Process {
		$List = New-Object -TypeName 'System.Collections.Generic.List[string]';
		$List.AddRange(@(Split-CredentialLocation -Elements));
		for ($i=0; $i -lt $List.Count; $i++) {
			if ($List[$i] -eq '.') {
				$List.RemoveAt($i);
				$i--;
			} else {
				if ($List[$i] -eq '..') {
					$List.RemoveAt($i);
					if ($i -gt 0) {
						$List.RemoveAt($i - 1);
						$i--;
					}
					$i--;
				}
			}
		}

		$List.ToArray() -join [System.IO.Path]::AltDirectorySeparatorChar;
	}
}

Function Split-CredentialLocation {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Parent')]
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Leaf')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Elements')]
        [string]$Path,
		
        [Parameter(Mandatory = $true, ParameterSetName = 'Parent')]
		[switch]$Parent,
		
        [Parameter(Mandatory = $true, ParameterSetName = 'Leaf')]
		[switch]$Leaf,
		
        [Parameter(Mandatory = $true, ParameterSetName = 'Elements')]
		[switch]$Elements,
		
        [Parameter(ParameterSetName = 'Parent')]
        [Parameter(ParameterSetName = 'Leaf')]
		[switch]$Resolve
    )
    
    Process {
		switch ($PSCmdlet.ParameterSetName) {
			'Parent' {
				if ($Resolve) { $Path = $Path | Resolve-CredentialLocation }
				if ($Path -eq '') {
					'' | Write-Output;
				} else {
					$e = @(Split-CredentialLocation -Path $Path -Elements);
					if ($e.Count -lt 2) {
						'' | Write-Output;
					} else {
						$Path = $e[1..($e.Count)] -join [System.IO.Path]::AltDirectorySeparatorChar;
					}
				}
				break;
			}
			'Leaf' {
				if ($Resolve) { $Path = $Path | Resolve-CredentialLocation }
				if ($Path -eq '') {
					'' | Write-Output;
				} else {
					$e = @(Split-CredentialLocation -Path $Path -Elements);
					if ($e.Count -eq  0) {
						'' | Write-Output;
					} else {
						$e[($e.Count) - 1];
					}
				}
				break;
			}
			'Elements' {
				@($Path.Split([System.IO.Path]::DirectorySeparatorChar) | ForEach-Object { $_.Split([System.IO.Path]::AltDirectorySeparatorChar) }) | Where-Object { $_ -ne '' }
				break;
			}
		}
	}
}

Function Join-CredentialLocation {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 1)]
        [string]$Path,
		
        [Parameter(Mandatory = $true, Position = 1)]
        [string]$ChildPath
    )
    
    Process {
	}
}

Function Test-CredentialLocation {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [string]$Path,

		[Microsoft.PowerShell.Commands.TestPathType]$PathType = [Microsoft.PowerShell.Commands.TestPathType]::Any
    )
    
    Process {
	}
}

Function Set-CredentialLocation {
    [CmdletBinding(DefaultParameterSetName = 'Path')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Name')]
        [string]$Path
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