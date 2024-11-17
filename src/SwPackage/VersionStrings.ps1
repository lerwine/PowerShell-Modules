<#
export const EXTENSION_CATEGORIES = [
	'AI',
	'Azure',
	'Chat',
	'Data Science',
	'Debuggers',
	'Extension Packs',
	'Education',
	'Formatters',
	'Keymaps',
	'Language Packs',
	'Linters',
	'Machine Learning',
	'Notebooks',
	'Programming Languages',
	'SCM Providers',
	'Snippets',
	'Testing',
	'Themes',
	'Visualization',
	'Other',
];

export interface IRelaxedExtensionManifest {
	name: string;
	displayName?: string;
	publisher: string;
	version: string;
	engines: { readonly vscode: string };
	description?: string;
	main?: string;
	browser?: string;
	preview?: boolean;
	// For now this only supports pointing to l10n bundle files
	// but it will be used for package.l10n.json files in the future
	l10n?: string;
	icon?: string;
	categories?: string[];
	keywords?: string[];
	activationEvents?: string[];
	extensionDependencies?: string[];
	extensionPack?: string[];
	extensionKind?: ExtensionKind | ExtensionKind[];
	contributes?: IExtensionContributions;
	repository?: { url: string };
	bugs?: { url: string };
	originalEnabledApiProposals?: readonly string[];
	enabledApiProposals?: readonly string[];
	api?: string;
	scripts?: { [key: string]: string };
	capabilities?: IExtensionCapabilities;
}

export type IExtensionManifest = Readonly<IRelaxedExtensionManifest>;

export const enum ExtensionType {
	System,
	User
}

export interface IExtension {
	readonly type: ExtensionType;
	readonly isBuiltin: boolean;
	readonly identifier: IExtensionIdentifier;
	readonly manifest: IExtensionManifest;
	readonly location: URI;
	readonly targetPlatform: TargetPlatform;
	readonly publisherDisplayName?: string;
	readonly readmeUrl?: URI;
	readonly changelogUrl?: URI;
	readonly isValid: boolean;
	readonly validations: readonly [Severity, string][];
}

export interface IRelaxedExtensionDescription extends IRelaxedExtensionManifest {
	id?: string;
	identifier: ExtensionIdentifier;
	uuid?: string;
	publisherDisplayName?: string;
	targetPlatform: TargetPlatform;
	isBuiltin: boolean;
	isUserBuiltin: boolean;
	isUnderDevelopment: boolean;
	extensionLocation: URI;
}
#>

if ($null -eq $Script:SEMVER_ZERO) {
    New-Variable -Name 'SEMVER_ZERO' -Option ReadOnly -Scope 'Script' -Value ([semver]::new(0));
}

Function Select-SemanticVersion {
    <#
    .SYNOPSIS
        Get specific Semver versions.
    .DESCRIPTION
        Filters SemanticVersion values.
    #>
    [CmdletBinding(DefaultParameterSetName = 'EqualTo')]
    [OutputType([semver])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [semver[]]$Version,

        [Parameter(Mandatory = $true, ParameterSetName = 'EqualTo')]
        [semver]$EqualTo,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotEqualTo')]
        [semver]$NotEqualTo,

        [Parameter(Mandatory = $true, ParameterSetName = 'GreaterThan')]
        [semver]$GreaterThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotGreaterThan')]
        [semver]$NotGreaterThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'LessThan')]
        [semver]$LessThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotLessThan')]
        [semver]$NotLessThan
    )

    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'NotEqualTo' {
                $Version | Where-Object { -not $_.Equals($NotEqualTo) }
                break;
            }
            'GreaterThan' {
                $Version | Where-Object { -not $_.CompareTo($GreaterThan) -gt 0 }
                break;
            }
            'NotGreaterThan' {
                $Version | Where-Object { -not $_.CompareTo($NotGreaterThan) -le 0 }
                break;
            }
            'LessThan' {
                $Version | Where-Object { -not $_.CompareTo($LessThan) -lt 0 }
                break;
            }
            'NotLessThan' {
                $Version | Where-Object { -not $_.CompareTo($NotLessThan) -ge 0 }
                break;
            }
            default { # EqualTo
                $Version | Where-Object { $_.Equals($EqualTo) }
                break;
            }
        }
    }

}

Function Select-BySemanticVersion {
    <#
    .SYNOPSIS
        Filter by Semver version property values.
    .DESCRIPTION
        Filters by SemanticVersion property values.
    #>
    [CmdletBinding(DefaultParameterSetName = 'EqualTo')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [object[]]$InputObject,

        [Parameter(Mandatory = $true)]
        [string]$PropertyName,

        [Parameter(Mandatory = $true, ParameterSetName = 'EqualTo')]
        [semver]$EqualTo,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotEqualTo')]
        [semver]$NotEqualTo,

        [Parameter(Mandatory = $true, ParameterSetName = 'GreaterThan')]
        [semver]$GreaterThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotGreaterThan')]
        [semver]$NotGreaterThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'LessThan')]
        [semver]$LessThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotLessThan')]
        [semver]$NotLessThan,

        [switch]$NullSameAsZero,

        [switch]$IgnoreNonVersionValues
    )
    
    Begin {
        if ($NullSameAsZero.IsPresent) { $Local:Zero = [semver]::new(0) }
        [semver]$Local:VersionValue = $null;
    }
    Process {
        if ($NullSameAsZero.IsPresent) {
            switch ($PSCmdlet.ParameterSetName) {
                'NotEqualTo' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ($null -eq $Value) {
                            if (-not $Local:Zero.Equals($NotEqualTo)) { $Obj | Write-Output }
                        } else {
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [semver], [ref]$Local:VersionValue)) {
                                if (-not $Local:VersionValue.Equals($NotEqualTo)) { $Obj | Write-Output }
                            } else {
                                if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $NotEqualTo) -ne 0) {
                                    $Obj | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
                'GreaterThan' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ($null -eq $Value) {
                            if ($Local:Zero.CompareTo($GreaterThan) -gt 0) { $Obj | Write-Output }
                        } else {
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [semver], [ref]$Local:VersionValue)) {
                                if ($Local:VersionValue.CompareTo($GreaterThan) -gt 0) { $Obj | Write-Output }
                            } else {
                                if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $GreaterThan) -gt 0) {
                                    $Obj | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
                'NotGreaterThan' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ($null -eq $Value) {
                            if ($Local:Zero.CompareTo($NotGreaterThan) -le 0) { $Obj | Write-Output }
                        } else {
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [semver], [ref]$Local:VersionValue)) {
                                if ($Local:VersionValue.CompareTo($NotGreaterThan) -le 0) { $Obj | Write-Output }
                            } else {
                                if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $NotGreaterThan) -le 0) {
                                    $Obj | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
                'LessThan' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ($null -eq $Value) {
                            if ($Local:Zero.CompareTo($LessThan) -lt 0) { $Obj | Write-Output }
                        } else {
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [semver], [ref]$Local:VersionValue)) {
                                if ($Local:VersionValue.CompareTo($LessThan) -lt 0) { $Obj | Write-Output }
                            } else {
                                if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $LessThan) -lt 0) {
                                    $Obj | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
                'NotLessThan' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ($null -eq $Value) {
                            if ($Local:Zero.CompareTo($NotLessThan) -ge 0) { $Obj | Write-Output }
                        } else {
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [semver], [ref]$Local:VersionValue)) {
                                if ($Local:VersionValue.CompareTo($NotLessThan) -ge 0) { $Obj | Write-Output }
                            } else {
                                if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $NotLessThan) -ge 0) {
                                    $Obj | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
                default {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ($null -eq $Value) {
                            if ($Local:Zero.Equals($EqualTo)) { $Obj | Write-Output }
                        } else {
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [semver], [ref]$Local:VersionValue)) {
                                if ($Local:VersionValue.Equals($EqualTo)) { $Obj | Write-Output }
                            } else {
                                if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $EqualTo) -eq 0) {
                                    $Obj | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
            }
        } else {
            switch ($PSCmdlet.ParameterSetName) {
                'NotEqualTo' {
                    foreach ($Obj in $InputObject) {
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Obj.($PropertyName), [semver], [ref]$Local:VersionValue)) {
                            if ($null -eq $Local:VersionValue -or -not $Local:VersionValue.Equals($NotEqualTo)) { $Obj | Write-Output }
                        } else {
                            if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $NotEqualTo) -ne 0) {
                                $Obj | Write-Output;
                            }
                        }
                    }
                    break;
                }
                'GreaterThan' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [semver], [ref]$Local:VersionValue)) {
                            if ($null -ne $Local:VersionValue -and $Local:VersionValue.CompareTo($GreaterThan) -gt 0) { $Obj | Write-Output }
                        } else {
                            if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $GreaterThan) -gt 0) {
                                $Obj | Write-Output;
                            }
                        }
                    }
                    break;
                }
                'NotGreaterThan' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [semver], [ref]$Local:VersionValue)) {
                            if ($null -eq $Local:VersionValue -or $Local:VersionValue.CompareTo($NotGreaterThan) -le 0) { $Obj | Write-Output }
                        } else {
                            if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $NotGreaterThan) -le 0) {
                                $Obj | Write-Output;
                            }
                        }
                    }
                    break;
                }
                'LessThan' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [semver], [ref]$Local:VersionValue)) {
                            if ($null -eq $Local:VersionValue -or $Local:VersionValue.CompareTo($NotGreaterThan) -lt 0) { $Obj | Write-Output }
                        } else {
                            if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $NotGreaterThan) -lt 0) {
                                $Obj | Write-Output;
                            }
                        }
                    }
                    break;
                }
                'NotLessThan' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [semver], [ref]$Local:VersionValue)) {
                            if ($null -ne $Local:VersionValue -and $Local:VersionValue.CompareTo($GreaterThan) -ge 0) { $Obj | Write-Output }
                        } else {
                            if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $GreaterThan) -ge 0) {
                                $Obj | Write-Output;
                            }
                        }
                    }
                    break;
                }
                default {
                    foreach ($Obj in $InputObject) {
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Obj.($PropertyName), [semver], [ref]$Local:VersionValue)) {
                            if ($null -ne $Local:VersionValue -and $Local:VersionValue.Equals($NotEqualTo)) { $Obj | Write-Output }
                        } else {
                            if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $NotEqualTo) -eq 0) {
                                $Obj | Write-Output;
                            }
                        }
                    }
                    break;
                }
            }
        }
    }
}

Function Compare-SemanticVersion {
    <#
    .SYNOPSIS
        Compare Semver versions.
    .DESCRIPTION
        Compares two SemanticVersion values, returning a number indicating whether one version is less than, greater than, or equal to the other.
    #>
    [CmdletBinding(DefaultParameterSetName = 'ReturnsInt')]
    [OutputType([int], ParameterSetName = 'ReturnsInt')]
    [OutputType([semver], ParameterSetName = 'PassThru')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [Alias('Version', 'ReferenceVersion')]
        [semver[]]$InputValue,

        [Parameter(Mandatory = $true)]
        [Alias('DifferenceVersion')]
        [semver]$To,

        [Parameter(Mandatory = $true, ParameterSetName = 'PassThru')]
        [switch]$PassThru
    )

    Process {
        if ($PassThru.IsPresent) {
            foreach ($Version in $InputValue) {
                ($Version | Add-Member -MemberType NoteProperty -Name 'ComparisonIndicator' -Value $Version.CompareTo($To) -PassThru) | Write-Output;
            }
        } else {
            foreach ($Version in $InputValue) {
                $Version.CompareTo($To) | Write-Output;
            }
        }
    }
}

Function Compare-SemverString {
    <#
    .SYNOPSIS
        Compare Semver version strings.
    .DESCRIPTION
        Compares two Semver strings, returning a number indicating whether one version is less than, greater than, or equal to the other.
    #>
    [CmdletBinding(DefaultParameterSetName = 'ReturnsInt')]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [Alias('Version', 'ReferenceVersion')]
        [AllowEmptyString()]
        [string[]]$InputString,

        [Parameter(Mandatory = $true)]
        [Alias('DifferenceVersion')]
        [semver]$To,

        [Parameter(Mandatory = $true, ParameterSetName = 'PassThru')]
        [switch]$EmptyIsNotZero
    )

    Begin { $Ts = $To.ToString() }

    Process {
        [semver]$Version = $null;
        if ($EmptyIsNotZero.IsPresent) {
            foreach ($vs in ($InputString | ForEach-Object { $_.Trim() })) {
                if ([semver]::TryParse($vs, [ref]$Version)) {
                    $Version.CompareTo($To) | Write-Output;
                } else {
                    [string]::Compare($vs, $Ts, $true) | Write-Output;
                }
            }
        } else {
            foreach ($vs in ($InputString | ForEach-Object { $_.Trim() })) {
                if ($vs -eq '') {
                    $Script:SEMVER_ZERO.CompareTo($To) | Write-Output;
                } else {
                    if ([semver]::TryParse($vs.Trim(), [ref]$Version)) {
                        $Version.CompareTo($To) | Write-Output;
                    } else {
                        [string]::Compare($vs, $Ts, $true) | Write-Output;
                    }
                }
            }
        }
    }
}
