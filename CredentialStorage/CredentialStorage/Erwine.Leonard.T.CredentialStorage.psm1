if ((Get-Module -Name 'Erwine.Leonard.T.IOUtility') -eq $null) { Import-Module -Name 'Erwine.Leonard.T.IOUtility' -ErrorAction Stop }
if ((Get-Module -Name 'Erwine.Leonard.T.XmlUtility') -eq $null) { Import-Module -Name 'Erwine.Leonard.T.XmlUtility' -ErrorAction Stop }
Add-Type -Path 'CredentialComponentCollection.cs', 'CredentialFolder.cs', 'CredentialItem.cs', 'CredentialStorageComponent.cs', 'CredentialStorageContainer.cs',
	'CredentialStorageDocument.cs', 'ICredentialComponent.cs', 'ICredentialContainer.cs', 'ICredentialContent.cs', 'ICredentialSite.cs', 'INestedCredentialContainer.cs' `
	-ReferencedAssemblies 'System', 'System.Management.Automation', 'System.Xml';

$Script:CredentialsNamespace = 'urn:Erwine.Leonard.T:PowerShell:CredentialStorage';
$Script:RootElementName = 'CredentialStorage';
$Script:FolderElementName = 'Folder';
$Script:CredentialElementName = 'Credential';
$Script:CurrentCredentialDocument = New-CredentialDocument;
$Script:CurrentCredentialContainer = $Script:CurrentCredentialDocument.DocumentElement;

Function New-CredentialDocument {
    [CmdletBinding()]
    [OutputType([System.Xml.XmlDocument])]
    Param()
    
    $CredentialDocument = New-Object -TypeName 'System.Xml.XmlDocument';
    $CredentialDocument.AppendChild($CredentialDocument.CreateElement($Script:RootElementName, $Script:CredentialsNamespace)) | Out-Null;
    $CredentialDocument | Write-Output;
}

Function Test-CredentialDocument {
    [CmdletBinding()]
    [OutputType([bool])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Element')]
        [Alias('XmlDocument', 'CredentialStorage', 'CredentialStorageDocument')]
        [System.Xml.XmlDocument]$Document
    )
    
    Process {
        $Document.DocumentElement -ne $null -and $Document.DocumentElement | Test-CredentialParent;
    }
}

Function Test-CredentialParent {
    [CmdletBinding()]
    [OutputType([bool])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Element')]
        [System.Xml.XmlElement]$Element,
        
        [switch]$ValidateProperties
    )
    
    Process {
        if ($XmlElement.NamespaceURI -ne $Script:CredentialsNamespace) {
            $false | Write-Output;
        } else {
			if ($XmlElement.LocalName -ne $Script:RootElementName) {
				$true | Write-Output;
			} else {
				if ($XmlElement.LocalName -ne $Script:FolderElementName) {
					if ($ValidateProperties) {
						if ($XmlElement.SelectSingleNode('@Name') -eq $null) {
							$false | Write-Output;
						} else {
							$true | Write-Output;
						}
					} else {
						$true | Write-Output;
					}
				} else {
					$false | Write-Output;
				}
			}
			
        }
	}
}

Function Test-CredentialElement {
    [CmdletBinding()]
    [OutputType([bool])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [Alias('XmlElement', 'Credential', 'CredentialElement')]
        [System.Xml.XmlElement]$Element,
        
        [switch]$ValidateProperties
    )
    
    Process {
        if ($XmlElement.NamespaceURI -ne $Script:CredentialsNamespace -or $XmlElement.LocalName -ne $Script:CredentialElementName) {
            $false | Write-Output;
        } else {
            if ($ValidateProperties) {
                if ($XmlElement.SelectSingleNode('@Name') -eq $null -or $XmlElement.SelectSingleNode('@UserName') -eq $null -or $XmlElement.SelectSingleNode('@Location') -eq $null -or $XmlElement.SelectSingleNode('@Password') -eq $null) {
                    $false | Write-Output;
                } else {
                    $true | Write-Output;
                }
            } else {
                $true | Write-Output;
            }
        }
    }
}

Function Load-CredentialDocument {
    [CmdletBinding(DefaultParameterSetName = 'Path')]
    [OutputType([System.Xml.XmlDocument], ParameterSetName = 'Path')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Path')]
        [string]$Path,
		
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'XmlDocument')]
        [ValidateScript({ Test-CredentialDocument -Document $_ })]
        [System.Xml.XmlDocument]$Document,
		
        [Parameter(ParameterSetName = 'Path')]
		[switch]$AsCurrent
    )
    
    Process {
		if ($PSCmdlet.ParameterSetName -eq 'XmlDocument') {
			$Script:CurrentCredentialDocument = $Document;
			$Script:CurrentCredentialContainer = $Document.DocumentElement;
		} else {
			$XmlDocument = Read-XmlDocument -InputUri $Path;
			if ($XmlDocument -ne $null) {
				if ((Test-CredentialDocument -Document $XmlDocument)) {
					If ($AsCurrent) {
						$Script:CurrentCredentialDocument = $XmlDocument;
						$Script:CurrentCredentialContainer = $XmlDocument.DocumentElement;
					}
					$XmlDocument | Write-Output;
				} else {
					throw 'Document was not a valid credentials document.';
				}
			}
		}
    }
}

Function Save-CredentialDocument {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$Path,

        [Parameter(Position = 1)]
        [ValidateScript({ Test-CredentialDocument -Document $_ })]
        [System.Xml.XmlDocument]$Document
	)
    
    Process {
		$XmlWriterSettings = New-XmlWriterSettings -Indent $true;

    }
}

Function Open-CredentialFolder {
    [CmdletBinding(DefaultParameterSetName = 'Path')]
    Param(
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Name')]
        [string]$Name,
		
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'XmlElement')]
        [ValidateScript({ Test-CredentialParent -Document $_ })]
        [System.Xml.XmlElement]$Element
    )
    
    Process {
		if ($PSCmdlet.ParameterSetName -eq 'XmlElement') {
			$Script:CurrentCredentialContainer = $Element;
			$Script:CurrentCredentialDocument = $Element.OwnerDocument;
		} else {
			throw 'Not Implemented.';
		}
    }
}

Function Close-CredentialFolder {
    [CmdletBinding()]
    Param()

	if ($Script:CurrentCredentialContainer.ParentNode -eq $null) {
		$Script:CurrentCredentialDocument = New-CredentialDocument;
		$Script:CurrentCredentialContainer = $Script:CurrentCredentialDocument.DocumentElement;
	} else {
		$Script:CurrentCredentialContainer = $Script:CurrentCredentialContainer.ParentNode;
	}
}

Function Get-CredentialLookupXPath {
    [CmdletBinding(DefaultParameterSetName = 'Default')]
    [OutputType([string])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Name')]
        [string]$Name,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'UserName')]
        [string]$UserName,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Location')]
        [string]$Location,
        
        [Parameter(Mandatory = $true, Position = 1)]
        [string]$Prefix
    )
    
    Process {
        $XPath = '{0}:{1}' -f $Prefix, $Script:CredentialElementName;
        if ($PSBoundParameters.Count -eq 1) {
            $XPath | Write-Output;
        } else {
            if ($PSBoundParameters.ContainsKey('UserName')) {
                $PredicateNode = '{0}:UserName' -f $Prefix;
                $PredicateValue = $UserName;
            } else {
                if ($PSBoundParameters.ContainsKey('Location')) {
                    $PredicateNode = '{0}:Location' -f $Prefix;
                    $PredicateValue = $Location;
                } else {
                    $PredicateNode = '@Name';
                    $PredicateValue = $Name;
                }
            }
            ('{0}[normalize-whitespace(translate("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "abcdefghijklmnopqrstuvwxyz", {1}))={2}]' -f $XPath, $PredicateNode, `
                    ($PredicateValue.ToLower() | ConvertTo-WhitespaceNormalizedString | ConvertTo-XPathQuotedString)) | Write-Output;
        }
    }
}

Function New-CredentialFolder {
    [CmdletBinding()]
    [OutputType([System.Xml.XmlElement])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$Name,
		
        [Parameter(Position = 1, ParameterSetName = 'XmlElement')]
        [ValidateScript({ Test-CredentialParent -Document $_ })]
        [System.Xml.XmlElement]$ParentElement,
		
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'XmlDocument')]
        [ValidateScript({ Test-CredentialDocument -Document $_ })]
        [System.Xml.XmlDocument]$Document
    )

}

Function New-CredentialElement {
    [CmdletBinding()]
    [OutputType([System.Xml.XmlElement])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$Name,
		
        [Parameter(Position = 1, ParameterSetName = 'XmlElement')]
        [ValidateScript({ Test-CredentialParent -Document $Document })]
        [System.Xml.XmlElement]$ParentElement,
		
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'XmlDocument')]
        [ValidateScript({ Test-CredentialDocument -Document $Document })]
        [System.Xml.XmlDocument]$Document
    )

}