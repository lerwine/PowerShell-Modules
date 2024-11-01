if ($null -eq $Script:VersionCoreSeparators) {
    New-Variable -Name 'VersionCoreSeparators' -Option ReadOnly -Value ([System.Collections.ObjectModel.ReadOnlyCollection[char]]::new(([char[]]('+', '-', '#', '?', '\', '/', '&', '=', ':', '|', '`', '^', '>', '<', '~'))));
    New-Variable -Name 'WhiteSpaceRegex' -Option ReadOnly -Value ([regex]::new('[\P{Z}\P{C}]'));
    New-Variable -Name 'NonWhiteSpacesRegex' -Option ReadOnly -Value ([regex]::new('[\P{Z}\P{C}]+'));
    New-Variable -Name 'SymbolRegex' -Option ReadOnly -Value ([regex]::new('\p{S}'));
    New-Variable -Name 'VersionStartRegex' -Option ReadOnly -Value ([regex]::new('\d+(\.\d+)*(-[^+]*)?(\+|$)'));
    New-Variable -Name 'Pep440Regex' -Option ReadOnly -Value ([regex]::new('^(?:(?<epoch>\d+)!)?(?<release>\d+(?:\.\d+)*)(?:[_.-]?(?:(?<a>a(?:lpha)?)|(?<b>b(?:eta)?)|(?<c>r?c)|(?<pre>pre(?:view)?))[_.-]?(?<pre_n>\d*))?(?:-(?<post_n1>\d+)|[_.-]?(?:(?<post>post)|(?<rev>rev)|(?<r>r))[_.-]?(?<post_n2>\d*))?(?:[_.-]?dev[_.-]?(?<dev>\d*))?(?:\+(?<local>[a-z\d]+(?:[_.-][a-z\d]+)*))?$', $IgnoreCaseRegexOptions));
    # $SeparatorIndex -lt $Script:DotSeparatorIndex: $Script:VersionComponentSeparators[$SeparatorIndex]
    # $SeparatorIndex -eq $Script:DotSeparatorIndex: '.'
    # $SeparatorIndex -lt $Script:WsSeparatorIndex: $Script:VersionSubSeparators[$SeparatorIndex - ($Script:DotSeparatorIndex + 1)]
    # $SeparatorIndex -eq $Script:WsSeparatorIndex: WhiteSpace
    # $Script:VersionCoreSeparators.Length + 1: [char]::IsPunctuation()
    # $Script:VersionCoreSeparators.Length + 2: \s+
}

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

if ($null -eq $Script:VS_GALLERY_VERSION_EXP) {
    New-Variable -Name 'VS_GALLERY_VERSION_EXP' -Option ReadOnly -Scope 'Script' -Value ([regex]::new('^(\^|>=)?(?:(\d+)|x)(?:\.(?:(\d+)|x)(?:\.(?:(\d+)|x))?)?(?:-(.+))?$', [System.Text.RegularExpressions.RegexOptions]::Compiled));
    New-Variable -Name 'VS_GALLERY_NOT_BEFORE_EXP' -Option ReadOnly -Scope 'Script' -Value ([regex]::new('^-(\d{4})(\d{2})(\d{2})$', [System.Text.RegularExpressions.RegexOptions]::Compiled));
    New-Variable -Name 'VS_GALLERY_TARGET_PLATFORM_EXP' -Option ReadOnly -Scope 'Script' -Value ([regex]::new('^(linux-(x64|arm(64|hf))|(win32|alpine|darwin)-((x|arm)64)|web|un(iversal|known|defined))$', [System.Text.RegularExpressions.RegexOptions]::Compiled));
    New-Variable -Name 'VS_GALLERY_INSTALL_SOURCE_EXP' -Option ReadOnly -Scope 'Script' -Value ([regex]::new('^(gallery|vsix|resource)$', [System.Text.RegularExpressions.RegexOptions]::Compiled));
    New-Variable -Name 'VS_GALLERY_EXTENSION_KIND_EXP' -Option ReadOnly -Scope 'Script' -Value ([regex]::new('^(ui|workspace|web)$', [System.Text.RegularExpressions.RegexOptions]::Compiled));
    New-Variable -Name 'VS_GALLERY_TARGET_PLATFORM_WIN32_X64' -Option Constant -Scope 'Script' -Value 'win32-x64';
	New-Variable -Name 'VS_GALLERY_TARGET_PLATFORM_WIN32_ARM64' -Option Constant -Scope 'Script' -Value 'win32-arm64';
	New-Variable -Name 'VS_GALLERY_TARGET_PLATFORM_LINUX_X64' -Option Constant -Scope 'Script' -Value 'linux-x64';
	New-Variable -Name 'VS_GALLERY_TARGET_PLATFORM_LINUX_ARM64' -Option Constant -Scope 'Script' -Value 'linux-arm64';
	New-Variable -Name 'VS_GALLERY_TARGET_PLATFORM_LINUX_ARMHF' -Option Constant -Scope 'Script' -Value 'linux-armhf';
	New-Variable -Name 'VS_GALLERY_TARGET_PLATFORM_ALPINE_X64' -Option Constant -Scope 'Script' -Value 'alpine-x64';
	New-Variable -Name 'VS_GALLERY_TARGET_PLATFORM_ALPINE_ARM64' -Option Constant -Scope 'Script' -Value 'alpine-arm64';
	New-Variable -Name 'VS_GALLERY_TARGET_PLATFORM_DARWIN_X64' -Option Constant -Scope 'Script' -Value 'darwin-x64';
	New-Variable -Name 'VS_GALLERY_TARGET_PLATFORM_DARWIN_ARM64' -Option Constant -Scope 'Script' -Value 'darwin-arm64';
	New-Variable -Name 'VS_GALLERY_TARGET_PLATFORM_WEB' -Option Constant -Scope 'Script' -Value 'web';
	New-Variable -Name 'VS_GALLERY_TARGET_PLATFORM_UNIVERSAL' -Option Constant -Scope 'Script' -Value 'universal';
	New-Variable -Name 'VS_GALLERY_TARGET_PLATFORM_UNKNOWN' -Option Constant -Scope 'Script' -Value 'unknown';
	New-Variable -Name 'VS_GALLERY_TARGET_PLATFORM_UNDEFINED' -Option Constant -Scope 'Script' -Value 'undefined';
}


Function Test-VsExtensionVersionString {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowNull()]
        [AllowEmptyString()]
        [string[]]$InputString
    )

    Process {
        foreach ($Value in $InputString) {
            if ($Value -eq '*') {
                $true | Write-Output;
            } else {
                [System.Text.RegularExpressions.Match]$M = $Script:VS_GALLERY_VERSION_EXP.Match($Value);
                if ($M.Success) {
                    $i = 0;
                    $G = $M.Groups[2];
                    (((-not $G.Success) -or [int]::TryParse($G.Value, [ref]$i)) -and ((-not ($G = $M.Groups[3]).Success) -or [int]::TryParse($G.Value, [ref]$i)) -and ((-not ($G = $M.Groups[4]).Success) -or [int]::TryParse($G.Value, [ref]$i))) | Write-Output;
                } else {
                    $false | Write-Output;
                }
            }
        }
    }
}

class VsExtensionVersion {
    [ValidateRange(0, [int]::MaxValue)]
    [int]$MajorBase = 0;
	
    [bool]$MajorMustEqual = $false;
	
    [ValidateRange(0, [int]::MaxValue)]
    [int]$MinorBase = 0;
	
    [bool]$MinorMustEqual = $false;
	
    [ValidateRange(0, [int]::MaxValue)]
    [int]$PatchBase = 0;
	
    [bool]$PatchMustEqual = $false;

    [AllowNull()]
    [Nullable[DateTime]]$NotBefore = $null;
	
    [bool]$IsMinimum = $false;
}

Function New-VsExtensionVersion {
    [CmdletBinding(DefaultParameterSetName = 'Values')]
    [OutputType([ParsedVsExtensionVersion])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'Parse')]
        [ValidateScript({ $_ | Test-VsExtensionVersionString })]
        [string[]]$InputString,

        [Parameter(ParameterSetName = 'Parse')]
        [switch]$DoNotNormalize,
        
        [Parameter(ParameterSetName = 'Values')]
        [int]$MajorBase = 0,
        
        [Parameter(ParameterSetName = 'Values')]
        [int]$MinorBase = 0,
        
        [Parameter(ParameterSetName = 'Values')]
        [int]$PatchBase = 0,
        
        [Parameter(ParameterSetName = 'Values')]
        [DateTime]$NotBefore
    )

    Process {
        if ($PSCmdlet.ParameterSetName -eq 'Parse') {
            foreach ($Value in $InputString) {
                $VsExtensionVersion = [VsExtensionVersion]::new();
                if ($Value -ne '*') {
                    [System.Text.RegularExpressions.Match]$M = $Script:VS_GALLERY_VERSION_EXP.Match($Value);
                    $G = $M.Groups[2];
                    if ($G.Success) {
                        $VsExtensionVersion.MajorBase = [int]::Parse($G.Value);
                        $VsExtensionVersion.MajorMustEqual = $true;
                    } else {
                        $VsExtensionVersion.MajorBase = '0';
                        $VsExtensionVersion.MajorMustEqual = $false;
                    }
                    $G = $M.Groups[3];
                    if ($G.Success) {
                        $VsExtensionVersion.MinorBase = [int]::Parse($G.Value);
                        $VsExtensionVersion.MinorMustEqual = $true;
                    } else {
                        $VsExtensionVersion.MinorBase = '0';
                        $VsExtensionVersion.MinorMustEqual = $false;
                    }
                    $G = $M.Groups[4];
                    if ($G.Success) {
                        $VsExtensionVersion.PatchBase = [int]::Parse($G.Value);
                        $VsExtensionVersion.PatchMustEqual = $true;
                    } else {
                        $VsExtensionVersion.PatchBase = '0';
                        $VsExtensionVersion.PatchMustEqual = $false;
                    }
                    $G = $M.Groups[1];
                    if ($G.Success) {
                        if ($G.Value -eq '^') {
                            $VsExtensionVersion.PatchMustEqual = $false;
                            if ($VsExtensionVersion.MajorBase -ne 0) { $VsExtensionVersion.MinorMustEqual = $false }
                        } else {
                            $VsExtensionVersion.IsMinimum = $true;
                        }
                    }
                    $G = $M.Groups[5];
                    if ($G.Success) {
                        $M = $Script:VS_GALLERY_NOT_BEFORE_EXP.Match($G.Value);
                        if ($M.Success) {
                            $VsExtensionVersion.NotBefore = [DateTime]::new([int]::parse($M.Groups[1].Value), [int]::parse($M.Groups[2].Value), [int]::parse($M.Groups[3].Value), 0, 0, 0, [System.DateTimeKind]::Utc);
                        }
                    }
                }
                $ParsedVsExtensionVersion | Write-Output;
            }
        } else {
            $VsExtensionVersion = [VsExtensionVersion]@{
                MajorBase = $MajorBase;
                MajorMustEqual = $PSBoundParameters.ContainsKey('MajorBase');
                MinorBase = $MinorBase;
                MinorMustEqual = $PSBoundParameters.ContainsKey('MinorBase');
                PatchBase = $PatchBase;
                PatchMustEqual = $PSBoundParameters.ContainsKey('PatchBase');
            };
            if ($PSBoundParameters.ContainsKey('NotBefore')) {
                $VsExtensionVersion.NotBefore = $NotBefore;
            }
            $VsExtensionVersion | Write-Output;
        }
    }
}

Function Compare-SemanticVersion {
    <#
    .SYNOPSIS
        Compare semantic versions.
    .DESCRIPTION
        Compares two semantic version values, returning a number indicating whether one version is less than, greater than, or equal to the other.
    .OUTPUTS
        System.Int32. 0 if Version and Other are equal; greater than 1 if Version is greather than Other; otherwise, less than 1 if Version is less than Other.
    #>
    [CmdletBinding(DefaultParameterSetName = 'Type')]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        # The version string to be compared.
        [string]$Version,

        [Parameter(Mandatory = $true, Position = 1)]
        [AllowEmptyString()]
        # The version string to compare to.
        [string]$Other,

        # Don't assume that corresponding version number elements are zero when one version string has more version number elements than the other.
        [switch]$DontAssumeZeroElements,

        # Do not normalize whitespace
        [switch]$WsNotNormalized,

        [Parameter(Mandatory = $true, ParameterSetName = 'Type')]
        # The version string comparison type. The default is OrdinalIgnoreCase.
        [System.StringComparison]$ComparisonType = [System.StringComparison]::OrdinalIgnoreCase
    )
    
    Begin {
        $Comparer = $ComparisonType | Get-StringComparer;
        $IsOrdinal = ($ComparisonType -eq [System.StringComparison]::OrdinalIgnoreCase -or $ComparisonType -eq [System.StringComparison]::Ordinal);
    }

}

Function Select-SemanticVersion {
    <#
    .SYNOPSIS
        Filter by Semver versions.
    .DESCRIPTION
        Filters by SemanticVersion values.
    #>
    [CmdletBinding(DefaultParameterSetName = 'EqualTo')]
    [OutputType([System.Management.Automation.SemanticVersion])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [System.Management.Automation.SemanticVersion[]]$Version,

        [Parameter(Mandatory = $true, ParameterSetName = 'EqualTo')]
        [System.Management.Automation.SemanticVersion]$EqualTo,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotEqualTo')]
        [System.Management.Automation.SemanticVersion]$NotEqualTo,

        [Parameter(Mandatory = $true, ParameterSetName = 'GreaterThan')]
        [System.Management.Automation.SemanticVersion]$GreaterThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotGreaterThan')]
        [System.Management.Automation.SemanticVersion]$NotGreaterThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'LessThan')]
        [System.Management.Automation.SemanticVersion]$LessThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotLessThan')]
        [System.Management.Automation.SemanticVersion]$NotLessThan
    )

}

Function Select-BySemanticVersion {
    <#
    .SYNOPSIS
        Filter by Semver versions.
    .DESCRIPTION
        Filters by SemanticVersion values.
    #>
    [CmdletBinding(DefaultParameterSetName = 'EqualTo')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [object[]]$InputObject,

        [Parameter(Mandatory = $true)]
        [string]$PropertyName,

        [Parameter(Mandatory = $true, ParameterSetName = 'EqualTo')]
        [System.Management.Automation.SemanticVersion]$EqualTo,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotEqualTo')]
        [System.Management.Automation.SemanticVersion]$NotEqualTo,

        [Parameter(Mandatory = $true, ParameterSetName = 'GreaterThan')]
        [System.Management.Automation.SemanticVersion]$GreaterThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotGreaterThan')]
        [System.Management.Automation.SemanticVersion]$NotGreaterThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'LessThan')]
        [System.Management.Automation.SemanticVersion]$LessThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotLessThan')]
        [System.Management.Automation.SemanticVersion]$NotLessThan,

        [switch]$NullSameAsZero,

        [switch]$IgnoreNonVersionValues
    )
    
    Begin {
        if ($NullSameAsZero.IsPresent) { $Local:Zero = [System.Management.Automation.SemanticVersion]::new(0) }
        [System.Management.Automation.SemanticVersion]$Local:VersionValue = $null;
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
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
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
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
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
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
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
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
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
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
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
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
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
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Obj.($PropertyName), [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
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
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
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
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
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
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
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
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
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
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Obj.($PropertyName), [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
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
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [Alias('Version')]
        [System.Management.Automation.SemanticVersion[]]$ReferenceVersion,

        [Parameter(Mandatory = $true)]
        [Alias('To')]
        [System.Management.Automation.SemanticVersion]$DifferenceVersion,

        [Parameter(Mandatory = $true, ParameterSetName = 'PassThru')]
        [switch]$PassThru
    )

    Process {
        if ($PassThru.IsPresent) {
            foreach ($Version in $ReferenceVersion) {
                ($Version | Add-Member -MemberType NoteProperty -Name 'ComparisonIndicator' -Value $Version.CompareTo($ReferenceVersion) -PassThru) | Write-Output;
            }
        } else {
            foreach ($Version in $ReferenceVersion) {
                $Version.CompareTo($ReferenceVersion) | Write-Output;
            }
        }
    }
}

Function Compare-IdentifierNameString {
    [CmdletBinding()]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value,

        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        # The version string to compare to.
        [string]$Other,

        # Do not normalize whitespace
        [switch]$WsNotNormalized,

        [Parameter(Mandatory = $true)]
        # The string comparar to use
        [System.StringComparer]$Comparer,

        [Parameter(Mandatory = $true)]
        [bool]$IsOrdinal
    )

    if ($WsNotNormalized.IsPresent) {
        $Private:ValueString = $Value;
        $Private:OtherString = $Other;
    } else {
        $Private:ValueString = $Value | Optimize-WhiteSpace;
        $Private:OtherString = $Other | Optimize-WhiteSpace;
    }

    if ($ValueString.Length -eq 0) {
        if ($OtherString.Length -eq 0) { 0 | Write-Output } else { -1 | Write-Output }
    } else {
        if ($OtherString.Length -eq 0) {
            1 | Write-Output;
        } else {
            $ValueArr = $ValueString.Split('-');
            $OtherArr = $OtherString.Split('-');
            if ($WsNotNormalized) {
                $Idx = 0;
                $Diff = 0;
                do {
                    $Diff = Compare-CharSeparatedAlphaNum -Value $Value -Other $Other -SeparatorIndex 2 -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                    if ($Diff -ne 0) { break }
                    $Idx++;
                } while ($Idx -lt $ValueArr.Length -and $Idx -lt $OtherArr.Length);
                if ($Diff -eq 0) { $Diff = $ValueArr.Length = $OtherArr.Length }
            } else {
                $Idx = 0;
                $Diff = 0;
                do {
                    $Diff = Compare-CharSeparatedAlphaNum -Value $Value -Other $Other -SeparatorIndex 2 -Comparer $Comparer -IsOrdinal $IsOrdinal;
                    if ($Diff -ne 0) { break }
                    $Idx++;
                } while ($Idx -lt $ValueArr.Length -and $Idx -lt $OtherArr.Length);
                if ($Diff -eq 0) { $Diff = $ValueArr.Length = $OtherArr.Length }
            }
        }
    }
}

Function Compare-IdentityString {
    [CmdletBinding()]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value,

        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Other,

        [switch]$WsNotNormalized,

        [Parameter(Mandatory = $true)]
        [System.StringComparer]$Comparer,

        [Parameter(Mandatory = $true)]
        [bool]$IsOrdinal
    )

    if ($WsNotNormalized.IsPresent) {
        $Private:ValueString = $Value;
        $Private:OtherString = $Other;
    } else {
        $Private:ValueString = $Value | Optimize-WhiteSpace;
        $Private:OtherString = $Other | Optimize-WhiteSpace;
    }

    if ($ValueString.Length -eq 0) {
        if ($OtherString.Length -eq 0) { 0 | Write-Output } else { -1 | Write-Output }
    } else {
        if ($OtherString.Length -eq 0) {
            1 | Write-Output;
        } else {
            $ValueArr = $ValueString.Split('.');
            $OtherArr = $OtherString.Split('.');
            if ($WsNotNormalized) {
                $Idx = 0;
                $Diff = 0;
                do {
                    $Diff = Compare-IdentifierNameString -Value $ValueArr[$Idx] -Other $OtherArr[$Idx] -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                    if ($Diff -ne 0) { break }
                    $Idx++;
                } while ($Idx -lt $ValueArr.Length -and $Idx -lt $OtherArr.Length);
                if ($Diff -eq 0) { $Diff = $ValueArr.Length = $OtherArr.Length }
            } else {
                $Idx = 0;
                $Diff = 0;
                do {
                    $Diff = Compare-IdentifierNameString -Value $ValueArr[$Idx] -Other $OtherArr[$Idx] -Comparer $Comparer -IsOrdinal $IsOrdinal;
                    if ($Diff -ne 0) { break }
                    $Idx++;
                } while ($Idx -lt $ValueArr.Length -and $Idx -lt $OtherArr.Length);
                if ($Diff -eq 0) { $Diff = $ValueArr.Length = $OtherArr.Length }
            }
        }
    }
}

Function Compare-PackageVersion {
    <#
    .SYNOPSIS
        Compare version strings.
    .DESCRIPTION
        Compares two version strings where the assumed format is Publisher.ID@SemanticVersion or ID@SemanticVersion.
    .OUTPUTS
        System.Int32. 0 if Version and Other are equal; greater than 1 if Version is greather than Other; otherwise, less than 1 if Version is less than Other.
    #>
    [CmdletBinding(DefaultParameterSetName = 'Type')]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        # The version string to be compared.
        [string]$Version,

        [Parameter(Mandatory = $true, Position = 1)]
        [AllowEmptyString()]
        # The version string to compare to.
        [string]$Other,

        # Don't assume that corresponding version number elements are zero when one version string has more version number elements than the other.
        [switch]$DontAssumeZeroElements,

        # Do not normalize whitespace
        [switch]$WsNotNormalized,

        # The version string comparison type. The default is OrdinalIgnoreCase.
        [System.StringComparison]$ComparisonType = [System.StringComparison]::OrdinalIgnoreCase
    )

    Begin {
        $Comparer = $ComparisonType | Get-StringComparer;
        $IsOrdinal = ($ComparisonType -eq [System.StringComparison]::OrdinalIgnoreCase -or $ComparisonType -eq [System.StringComparison]::Ordinal);
    }

    Process {
        if ($WsNotNormalized.IsPresent) {
            $Private:VersionString = $Version;
            $Private:OtherString = $Other;
        } else {
            $Private:VersionString = $Version | Optimize-WhiteSpace;
            $Private:OtherString = $Other | Optimize-WhiteSpace;
        }

        if ($VersionString.Length -eq 0) {
            if ($OtherString.Length -eq 0) { 0 | Write-Output } else { -1 | Write-Output }
        } else {
            if ($OtherString.Length -eq 0) {
                1 | Write-Output;
            } else {
                $VersionIndex = $VersionString.IndexOf('@');
                $OtherIndex = $OtherString.IndexOf('@');
                if ($VersionIndex -lt 0) {
                    if ($OtherIndex -lt 0) {
                        $VersionIndex = $VersionString.IndexOf('.');
                        $OtherIndex = $OtherString.IndexOf('.');
                        if ($VersionIndex -lt 0) {
                            if ($OtherIndex -lt 0) {
                                if ($Script:VersionStartRegex.IsMatch($VersionString)) {
                                    if ($Script:VersionStartRegex.IsMatch($OtherString)) {
                                        if ($WsNotNormalized.IsPresent) {
                                            if ($DontAssumeZeroElements.IsPresent) {
                                                $Diff = Compare-SemanticVersionCore -Version $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized -DontAssumeZeroElements;
                                            } else {
                                                $Diff = Compare-SemanticVersionCore -Version $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                            }
                                        } else {
                                            if ($DontAssumeZeroElements.IsPresent) {
                                                $Diff = Compare-SemanticVersionCore -Version $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -DontAssumeZeroElements;
                                            } else {
                                                $Diff = Compare-SemanticVersionCore -Version $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                            }
                                        }
                                    } else {
                                        if ($DontAssumeZeroElements.IsPresent) { $Diff = -1 } else { $Diff = 1 }
                                    }
                                } else {
                                    if ($Script:VersionStartRegex.IsMatch($OtherString)) {
                                        if ($DontAssumeZeroElements.IsPresent) { $Diff = 1 } else { $Diff = -1 }
                                    } else {
                                        if ($WsNotNormalized.IsPresent) {
                                            $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                        } else {
                                            $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                        }
                                    }
                                }
                            } else {
                                if ($Script:VersionStartRegex.IsMatch($VersionString)) {
                                    if ($DontAssumeZeroElements.IsPresent) { $Diff = -1 } else { $Diff = 1 }
                                } else {
                                    if ($WsNotNormalized.IsPresent) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    } else {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                }
                            }
                        } else {
                            if ($OtherIndex -lt 0) {
                                if ($Script:VersionStartRegex.IsMatch($OtherString)) {
                                    if ($DontAssumeZeroElements.IsPresent) { $Diff = 1 } else { $Diff = -1 }
                                } else {
                                    if ($WsNotNormalized.IsPresent) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    } else {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                }
                            } else {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    if ($Diff -eq 0) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    }
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    if ($Diff -eq 0) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                }
                            }
                        }
                    } else {
                        $VersionIndex = $VersionString.IndexOf('.');
                        $OtherString = $OtherString.Substring(0, $OtherIndex + 1);
                        $OtherIndex = $OtherString.IndexOf('.');
                        if ($VersionIndex -lt 0) {
                            if ($Script:VersionStartRegex.IsMatch($VersionString)) {
                                if ($DontAssumeZeroElements.IsPresent) { $Diff = -1 } else { $Diff = 1 }
                            } else {
                                if ($OtherIndex -lt 0) {
                                    if ($WsNotNormalized.IsPresent) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    } else {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                } else {
                                    if ($WsNotNormalized.IsPresent) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    } else {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                }
                            }
                        } else {
                            if ($OtherIndex -lt 0) {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                }
                            } else {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    if ($Diff -eq 0) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    }
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    if ($Diff -eq 0) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                }
                            }
                        }
                    }
                } else {
                    if ($OtherIndex -lt 0) {
                        $VersionString = $VersionString.Substring(0, $VersionIndex);
                        $VersionIndex = $VersionString.IndexOf('.');
                        $OtherIndex = $OtherString.IndexOf('.');
                        if ($VersionIndex -lt 0) {
                            if ($OtherIndex -lt 0) {
                                if ($Script:VersionStartRegex.IsMatch($OtherString)) {
                                    if ($DontAssumeZeroElements.IsPresent) { $Diff = 1 } else { $Diff = -1 }
                                } else {
                                    if ($WsNotNormalized.IsPresent) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    } else {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                }
                            } else {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                }
                            }
                        } else {
                            if ($OtherIndex -lt 0) {
                                if ($Script:VersionStartRegex.IsMatch($OtherString)) {
                                    if ($DontAssumeZeroElements.IsPresent) { $Diff = 1 } else { $Diff = -1 }
                                } else {
                                    if ($WsNotNormalized.IsPresent) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    } else {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                }
                            } else {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    if ($Diff -eq 0) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    }
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    if ($Diff -eq 0) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                }
                            }
                        }
                    } else {
                        $VersionID = $VersionString.Substring(0, $VersionIndex);
                        $OtherID = $OtherString.Substring(0, $OtherIndex);
                        $VIdx = $VersionID.IndexOf('.');
                        $OIdx = $OtherID.IndexOf('.');
                        if ($VIdx -lt 0) {
                            if ($OIdx -lt 0) {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionID -Other $OtherID -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    if ($Diff -eq 0) {
                                        if ($DontAssumeZeroElements.IsPresent) {
                                            $Diff = Compare-SemanticVersionCore -Version $VersionString.Substring($VersionIndex + 1)  -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized -DontAssumeZeroElements;
                                        } else {
                                            $Diff = Compare-SemanticVersionCore -Version $VersionString.Substring($VersionIndex + 1)  -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                        }
                                    }
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionID -Other $OtherID -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    if ($Diff -eq 0) {
                                        if ($DontAssumeZeroElements.IsPresent) {
                                            $Diff = Compare-SemanticVersionCore -Version $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -DontAssumeZeroElements;
                                        } else {
                                            $Diff = Compare-SemanticVersionCore -Version $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                        }
                                    }
                                }
                            } else {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionID -Other $OtherID.Substring(0, $OIdx) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionID -Other $OtherID.Substring(0, $OIdx) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                }
                                if ($Diff -eq 0) { $Diff = -1 }
                            }
                        } else {
                            if ($OIdx -lt 0) {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionID.Substring(0, $VIdx) -Other $OtherID -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionID.Substring(0, $VIdx) -Other $OtherID -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                }
                                if ($Diff -eq 0) { $Diff = 1 }
                            } else {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionID.Substring(0, $VIdx) -Other $OtherID.Substring(0, $OIdx) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    if ($Diff -eq 0) {
                                        if ($DontAssumeZeroElements.IsPresent) {
                                            $Diff = Compare-SemanticVersionCore -Version $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized -DontAssumeZeroElements;
                                        } else {
                                            $Diff = Compare-SemanticVersionCore -Version $VersVersionString.Substring($VersionIndex + 1)ionString -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                        }
                                    }
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionID.Substring(0, $VIdx) -Other $OtherID.Substring(0, $OIdx) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    if ($Diff -eq 0) {
                                        if ($DontAssumeZeroElements.IsPresent) {
                                            $Diff = Compare-SemanticVersionCore -Version $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -DontAssumeZeroElements;
                                        } else {
                                            $Diff = Compare-SemanticVersionCore -Version $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

Function Compare-PackageIdentity {
    <#
    .SYNOPSIS
        Compare version strings.
    .DESCRIPTION
        Compares two version strings where the assumed format is Publisher.ID-SemanticVersion@TargetPlatform, ID-SemanticVersion@TargetPlatform, Publisher.ID-SemanticVersion, or ID-SemanticVersion.
    .OUTPUTS
        System.Int32. 0 if Version and Other are equal; greater than 1 if Version is greather than Other; otherwise, less than 1 if Version is less than Other.
    #>
    [CmdletBinding()]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        # The version string to be compared.
        [string]$Version,

        [Parameter(Mandatory = $true, Position = 1)]
        [AllowEmptyString()]
        # The version string to compare to.
        [string]$Other,

        # Don't assume that corresponding version number elements are zero when one version string has more version number elements than the other.
        [switch]$DontAssumeZeroElements,

        # Do not normalize whitespace
        [switch]$WsNotNormalized,

        # The version string comparison type. The default is OrdinalIgnoreCase.
        [System.StringComparison]$ComparisonType = [System.StringComparison]::OrdinalIgnoreCase
    )

    Begin {
        $Comparer = $ComparisonType | Get-StringComparer;
        $IsOrdinal = ($ComparisonType -eq [System.StringComparison]::OrdinalIgnoreCase -or $ComparisonType -eq [System.StringComparison]::Ordinal);
    }

    Process {
        # Pub-Pub-Pub
        # Pub--Pub
        # Pub-Pub
        # Pub
        # ID-ID-ID
        # ID--ID
        # ID-ID
        # ID
        # Mjr.Mnr.Rev
        # Mjr.Mnr
        # Mjr
        
        # Pub.ID-Ver-Pre+Bld@Pfm
        # Pub.ID-Ver-Pre+Bld
        # Pub.ID-Ver-Pre@Pfm
        # Pub.ID-Ver+Bld@Pfm
        # ID-Ver-Pre+Bld@Pfm
        # Pub.ID-Ver-Pre
        # Pub.ID-Ver+Bld
        # Pub.ID-Ver@Pfm
        # Pub.ID+Bld@Pfm
        # ID-Ver-Pre+Bld
        # ID-Ver-Pre@Pfm
        # ID-Ver+Bld@Pfm
        # Pub.ID-Ver
        # Pub.ID+Bld
        # Pub.ID@Pfm
        # ID-Ver-Pre
        # ID-Ver+Bld
        # ID-Ver@Pfm
        # ID+Bld@Pfm
        # Pub.ID
        # ID-Ver
        # ID+Bld
        # ID@Pfm
        # ID
    }

}

Function Test-MyFunc {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [int]$Value,

        [Parameter(Mandatory = $true)]
        [int]$Other,
        
        [switch]$Alt
    )

    Begin {
        if ($Alt.IsPresent) {
            $Private:OtherValue = $Other - 1;
        } else {
            $Private:OtherValue = $Other;
        }
    }

    Process {
        "OtherValue: $($OtherValue | ConvertTo-Json)";
    }
}

Test-MyFunc -Value 1 -Other 0 -Alt;