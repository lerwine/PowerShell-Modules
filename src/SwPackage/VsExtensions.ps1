<#
https://github.com/microsoft/vscode/blob/b6648af1eac7021cc646d1a9f59e93026a7c1689/src/vs/platform/extensionManagement/common/extensionGalleryService.ts#L39
interface IRawGalleryExtensionProperty {
	readonly key: string;
	readonly value: string;
}
#>
class VsGalleryExtensionProperty {
    [string]$Key;
    
    [string]$Value;

    static [VsGalleryExtensionProperty] FromVsExtensionObject([object]$JsonObj) {
        if ($null -eq $JsonObj) { return $null }
        return [VsGalleryExtensionProperty]@{
            Key = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.key, [string]);
            Value = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.value, [string]);
        };
    }
}
<#
https://github.com/microsoft/vscode/blob/b6648af1eac7021cc646d1a9f59e93026a7c1689/src/vs/platform/extensionManagement/common/extensionGalleryService.ts#L34
interface IRawGalleryExtensionFile {
	readonly assetType: string;
	readonly source: string;
}
#>
class VsGalleryExtensionFile {
    [string]$AssetType;
    
    [string]$Source;

    static [VsGalleryExtensionFile] FromVsExtensionObject([object]$JsonObj) {
        if ($null -eq $JsonObj) { return $null }
        return [VsGalleryExtensionFile]@{
            AssetType = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.assetType, [string]);
            Source = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.source, [string]);
        };
    }
}
<#
https://github.com/microsoft/vscode/blob/b6648af1eac7021cc646d1a9f59e93026a7c1689/src/vs/platform/extensionManagement/common/extensionGalleryService.ts#L44
export interface IRawGalleryExtensionVersion {
	readonly version: string;
	readonly lastUpdated: string;
	readonly assetUri: string;
	readonly fallbackAssetUri: string;
	readonly files: IRawGalleryExtensionFile[];
	readonly properties?: IRawGalleryExtensionProperty[];
	readonly targetPlatform?: string;
}
#>
class VsGalleryExtensionVersion {
    [System.Management.Automation.SemanticVersion]$Version;
    
    [DateTime]$LastUpdated;

    [Uri]$AssetUri;

    [Uri]$FallbackAssetUri;

    [VsGalleryExtensionFile[]]$Files;

    [AllowNull()]
    [VsGalleryExtensionProperty[]]$Properties = $null;

    [AllowNull()]
    [string]$TargetPlatform = $null;

    static [VsGalleryExtensionVersion] FromVsExtensionObject([object]$JsonObj) {
        if ($null -eq $JsonObj) { return $null }
        $VsGalleryExtensionVersion = [VsGalleryExtensionVersion]@{
            Version = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.version, [System.Management.Automation.SemanticVersion]);
            LastUpdated = [DateTime]::SpecifyKind([System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.version, [DateTime]), [DateTimeKind]::Utc);
            Files = ([VsGalleryExtensionFile[]]@($JsonObj.files) | ForEach-Object { [VsGalleryExtensionFile]::FromVsExtensionObject($_) } | Where-Object { $null -ne $_ });
            TargetPlatform = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.targetPlatform, [string]);;
        };
        [Uri]$Uri = $null;
        $Value = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.assetUri, [string]);
        if ($null -eq $Value) { $Value = '' }
        if ([Uri]::TryCreate($Value, [UriKind]::Absolute, [ref]$Uri)) {
            $VsGalleryExtensionVersion.AssetUri = $Uri;
        } else {
            $VsGalleryExtensionVersion.AssetUri = [Uri]::new($Value, [UriKind]::Relative);
        }
        $Value = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.fallbackAssetUri, [string]);
        if ($null -eq $Value) { $Value = '' }
        if ([Uri]::TryCreate($Value, [UriKind]::Absolute, [ref]$Uri)) {
            $VsGalleryExtensionVersion.FallbackAssetUri = $Uri;
        } else {
            $VsGalleryExtensionVersion.FallbackAssetUri = [Uri]::new($Value, [UriKind]::Relative);
        }
        if ($null -ne $JsonObj.properties) {
            [VsGalleryExtensionProperty[]]$Arr = @(@($JsonObj.properties) | ForEach-Object { [VsGalleryExtensionProperty]::FromVsExtensionObject($_) } | Where-Object { $null -ne $_ });
            if ($Arr.Length -gt 0) { $VsGalleryExtensionVersion.Properties = $Arr }
        }
        return $VsGalleryExtensionVersion;
    }
}

<#
interface IRawGalleryExtensionStatistics {
	readonly statisticName: string;
	readonly value: number;
}
#>
class VSGalleryExtensionStatistics {
    [string]$StatisticName;
    
    [double]$Value = 0.0;

    static [VSGalleryExtensionStatistics] FromVsExtensionObject([object]$JsonObj) {
        if ($null -eq $JsonObj) { return $null }
        $VSGalleryExtensionStatistics = [VSGalleryExtensionStatistics]@{
            StatisticName = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.statisticName, [string]);
        };
        $d = 0.0;
        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($JsonObj.value, [double], [ref]$d)) { $VSGalleryExtensionStatistics.Value = $d }
        return $VSGalleryExtensionStatistics;
    }
}

<#
https://github.com/microsoft/vscode/blob/b6648af1eac7021cc646d1a9f59e93026a7c1689/src/vs/platform/extensionManagement/common/extensionGalleryService.ts#L59
interface IRawGalleryExtensionPublisher {
	readonly displayName: string;
	readonly publisherId: string;
	readonly publisherName: string;
	readonly domain?: string | null;
	readonly isDomainVerified?: boolean;
}
#>
class GalleryExtensionPublisher {
    [string]$DisplayName;
    
    [string]$PublisherId;
    
    [string]$PublisherName;
    
    [AllowNull()]
    [string]$Domain = $null;

    [AllowNull()]
    [Nullable[bool]]$IsDomainVerified = $null;

    static [VsGalleryExtension] FromVsExtensionObject([object]$JsonObj) {
        if ($null -eq $JsonObj) { return $null }
        $GalleryExtensionPublisher = [GalleryExtensionPublisher]@{
            DisplayName = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.displayName, [string]);
            PublisherId = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.extensionId, [string]);
            PublisherName = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.extensionName, [string]);
            Domain = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.shortDescription, [string]);
        };
        [Nullable[bool]]$nb = $null;
        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($JsonObj.isDomainVerified, [Nullable[bool]], [ref]$nb)) {
            $GalleryExtensionPublisher.IsDomainVerified = $nb;
        }
        return $GalleryExtensionPublisher;
    }
}

<#
https://github.com/microsoft/vscode/blob/b6648af1eac7021cc646d1a9f59e93026a7c1689/src/vs/platform/extensionManagement/common/extensionGalleryService.ts#L67
interface IRawGalleryExtension {
	readonly extensionId: string;
	readonly extensionName: string;
	readonly displayName: string;
	readonly shortDescription?: string;
	readonly publisher: IRawGalleryExtensionPublisher;
	readonly versions: IRawGalleryExtensionVersion[];
	readonly statistics: IRawGalleryExtensionStatistics[];
	readonly tags: string[] | undefined;
	readonly releaseDate: string;
	readonly publishedDate: string;
	readonly lastUpdated: string;
	readonly categories: string[] | undefined;
	readonly flags: string;
}
#>
class VsGalleryExtension {
    [string]$ExtensionId;

    [string]$ExtensionName;
    
    [string]$DisplayName;

    [AllowNull()]
    [string]$ShortDescription;

    [GalleryExtensionPublisher]$Publisher;

    [AllowNull()]
    [VsGalleryExtensionVersion[]]$Versions = $null;

    [VSGalleryExtensionStatistics[]]$Statistics;

    [AllowNull()]
    [string[]]$Tags = $null;

    [DateTime]$ReleaseDate;

    [DateTime]$PublishedDate;

    [DateTime]$LastUpdated;

    [AllowNull()]
    [string[]]$Categories = $null;

    [string]$Flags;
    
    # [Parameter(ParameterSetName = 'ExplicitAttributes')]
    # [switch]$IncludeFiles,

    # [switch]$IncludeVersionProperties,

    # [switch]$IncludeInstallationTargets,

    # [switch]$IncludeAssetUri,

    # [switch]$IncludeStatistics,

    # [switch]$IncludeLatestVersionOnly,

    # [switch]$Unpublished,

    # [switch]$IncludeNameConflictInfo,

    # [Parameter(Mandatory = $true, ParameterSetName = 'AllAttributes')]
    # [switch]$AllAttributes
    static [VsGalleryExtension] FromVsExtensionObject([object]$JsonObj) {
        if ($null -eq $JsonObj) { return $null }
        $VsGalleryExtension = [VsGalleryExtension]@{
            ExtensionId = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.extensionId, [string]);
            ExtensionName = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.extensionName, [string]);
            DisplayName = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.displayName, [string]);
            ShortDescription = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.shortDescription, [string]);
            Publisher = [GalleryExtensionPublisher]::FromVsExtensionObject($JsonObj.publisher);
            Versions = ([VsGalleryExtensionVersion[]]@($JsonObj.versions) | ForEach-Object { [VsGalleryExtensionVersion]::FromVsExtensionObject($_) } | Where-Object { $null -ne $_ });
            Statistics = ([VSGalleryExtensionStatistics[]]@($JsonObj.versions) | ForEach-Object { [VSGalleryExtensionStatistics]::FromVsExtensionObject($_) } | Where-Object { $null -ne $_ });
            ReleaseDate = [DateTime]::SpecifyKind([System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.releaseDate, [DateTime]), [DateTimeKind]::Utc);
            PublishedDate = [DateTime]::SpecifyKind([System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.publishedDate, [DateTime]), [DateTimeKind]::Utc);
            LastUpdated = [DateTime]::SpecifyKind([System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.lastUpdated, [DateTime]), [DateTimeKind]::Utc);
            Flags = [System.Management.Automation.LanguagePrimitives]::ConvertTo($JsonObj.flags, [string]);
        };
        if ($null -ne $JsonObj.versions) {
            [VsGalleryExtensionVersion[]]$Arr = @(@($JsonObj.versions) | ForEach-Object { [VsGalleryExtensionVersion]::FromVsExtensionObject($_) } | Where-Object { $null -ne $_ });
            if ($Arr.Length -gt 0) { $VsGalleryExtension.Versions = $Arr }
        }
        if ($null -ne $JsonObj.tags) {
            [string[]]$Arr = @(@($JsonObj.tags) | ForEach-Object { [System.Management.Automation.LanguagePrimitives]::ConvertTo($_, [string]) } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) });
            if ($Arr.Length -gt 0) { $VsGalleryExtension.Tags = $Arr }
        }
        if ($null -ne $JsonObj.categories) {
            [string[]]$Arr = @(@($JsonObj.categories) | ForEach-Object { [System.Management.Automation.LanguagePrimitives]::ConvertTo($_, [string]) } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) });
            if ($Arr.Length -gt 0) { $VsGalleryExtension.Categories = $Arr }
        }

        return $VsGalleryExtension;
    }
}

class ParsedVsExtensionVersion {
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

    static [ParsedVsExtensionVersion] Parse([string]$Value) {
        if ([string]::IsNullOrWhiteSpace($Value)) { return $null }
        $ParsedVersion = [ParsedVsExtensionVersion]::new();
        if ($Value -eq '*') { return $ParsedVersion }
        [System.Text.RegularExpressions.Match]$M = $Script:VS_GALLERY_VERSION_EXP.Match($Value);
        if (-not $M.Success) { throw 'Invalid version string' }
        $G = $M.Groups[2];
        if ($G.Success) {
            $ParsedVersion.MajorBase = [int]::Parse($G.Value);
            $ParsedVersion.MajorMustEqual = $true;
        } else {
            $ParsedVersion.MajorBase = '0';
            $ParsedVersion.MajorMustEqual = $false;
        }
        $G = $M.Groups[3];
        if ($G.Success) {
            $ParsedVersion.MinorBase = [int]::Parse($G.Value);
            $ParsedVersion.MinorMustEqual = $true;
        } else {
            $ParsedVersion.MinorBase = '0';
            $ParsedVersion.MinorMustEqual = $false;
        }
        $G = $M.Groups[4];
        if ($G.Success) {
            $ParsedVersion.PatchBase = [int]::Parse($G.Value);
            $ParsedVersion.PatchMustEqual = $true;
        } else {
            $ParsedVersion.PatchBase = '0';
            $ParsedVersion.PatchMustEqual = $false;
        }
        $G = $M.Groups[1];
        if ($G.Success) {
            if ($G.Value -eq '^') {
                $ParsedVersion.PatchMustEqual = $false;
                if ($ParsedVersion.MajorBase -ne 0) { $ParsedVersion.MinorMustEqual = $false }
            } else {
                $ParsedVersion.IsMinimum = $true;
            }
        }
        $G = $M.Groups[5];
        if ($G.Success) {
            $M = $Script:VS_GALLERY_NOT_BEFORE_EXP.Match($G.Value);
            if ($M.Success) {
                $ParsedVersion.NotBefore = [DateTime]::new([int]::parse($M.Groups[1].Value), [int]::parse($M.Groups[2].Value), [int]::parse($M.Groups[3].Value), 0, 0, 0, [System.DateTimeKind]::Utc);
            }
        }
        return $ParsedVersion;
    }
}

class VsExtensionFile {
    [string]$PublisherId;

    [string]$PackageId;

    [System.Management.Automation.SemanticVersion]$Version;

    [AllowNull()]
    [string]$TargetPlatform;

    [string]$FullName;
    
    [DateTime]$CreationTime;
    
    [DateTime]$LastWriteTime;
    
    [long]$Length;
}

class ScannedVsExtensionFile : VsExtensionFile {
    [string]$DisplayName;
    [AllowNull()]
    [string]$Description = $null;
    [AllowNull()]
    [string]$IconUri = $null;
    [AllowNull()]
    [object[]]$IconData = $null;
}


if ($null -eq $Script:VSIX_NAME_EXP) {
    New-Variable -Name 'VSIX_NAME_EXP' -Option ReadOnly -Scope 'Script' -Value ([regex]::new('^([^.]+)\.((?:[^-]+|-(?!\d+(?:[.@]|$))+)-(\d+(?:\.\d+)*(?:-.*)?)(?:@(.+)?)?$', [System.Text.RegularExpressions.RegexOptions]::Compiled));
    # ([^.]+)         \.((?:[^-]+|-(?!\d+(?:[.@]|$))+) -(\d+(?:\.\d+)*)(?:@(.+)?)?
    # redhat          .vscode-xml                      -0.27.2024052908@win32-x64
    # ms-dotnettools  .csharp                          -2.43.16@win32-x64
    # ms-vscode       .vscode-typescript-next          -5.7.20240829
    # arnoudkooicom   .sn-scriptsync                   -3.3.4
    # firefox-devtools.vscode-firefox-debug            -2.9.8
    # firefox-devtools.vscode-firefox-debug            -2.9.10
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

Function Find-VsExtensionInMarketPlace {
    [CmdletBinding()]
    [OutputType([VsGalleryExtension])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$Publisher,
        
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$ID,
        
        [ValidatePattern('^(linux-(x64|arm(64|hf))|(win32|alpine|darwin)-((x|arm)64)|web|un(iversal|known|defined))$')]
        [string]$TargetPlatform,

        [int]$MaxPage = 10000,

        [int]$PageSize = 100,

        [Parameter(ParameterSetName = 'ExplicitAttributes')]
        [switch]$IncludeVersions,

        [Parameter(ParameterSetName = 'ExplicitAttributes')]
        [switch]$IncludeFiles,

        [Parameter(ParameterSetName = 'ExplicitAttributes')]
        [switch]$IncludeCategoryAndTags,

        [Parameter(ParameterSetName = 'ExplicitAttributes')]
        [switch]$IncludeSharedAccounts,

        [switch]$ExcludeNonValidated,

        [switch]$IncludeVersionProperties,

        [switch]$IncludeInstallationTargets,

        [switch]$IncludeAssetUri,

        [switch]$IncludeStatistics,

        [switch]$IncludeLatestVersionOnly,

        [switch]$Unpublished,

        [switch]$IncludeNameConflictInfo,

        [Parameter(Mandatory = $true, ParameterSetName = 'AllAttributes')]
        [switch]$AllAttributes
    )

    $Flags = 0;
    if ($AllAttributes.IsPresent) {
        $Flags = 0x1f;
    } else {
        if ($IncludeVersions.IsPresent) { $Flags = 0x1 }
        if ($IncludeFiles.IsPresent) { $Flags = $Flags -bor 0x2 }
        if ($IncludeCategoryAndTags.IsPresent) { $Flags = $Flags -bor 0x4 }
        if ($IncludeSharedAccounts.IsPresent) { $Flags = $Flags -bor 0x8 }
        if ($IncludeVersionProperties.IsPresent) { $Flags = $Flags -bor 0x10 }
    }
    if ($ExcludeNonValidated.IsPresent) { $Flags = $Flags -bor 0x20 }
    if ($IncludeInstallationTargets.IsPresent) { $Flags = $Flags -bor 0x40 }
    if ($IncludeAssetUri.IsPresent) { $Flags = $Flags -bor 0x80 }
    if ($IncludeStatistics.IsPresent) { $Flags = $Flags -bor 0x100 }
    if ($IncludeLatestVersionOnly.IsPresent) { $Flags = $Flags -bor 0x200 }
    if ($Unpublished.IsPresent) { $Flags = $Flags -bor 0x1000 }
    if ($IncludeNameConflictInfo.IsPresent) { $Flags = $Flags -bor 0x8000 }
    
    $criteria = @([PSCustomObject]@{
        filterType = 7;
        value = "$Publisher.$ID";
    }, [PSCustomObject]@{
        filterType = 8;
        value = "Microsoft.VisualStudio.Code";
    });
    if ($PSBoundParameters.ContainsKey('TargetPlatform')) {
        $criteria += [PSCustomObject]@{
            filterType = 23;
            value = $TargetPlatform;
        }
    }
    $requestBody = [PSCustomObject]@{
        filters = ([object[]]@([PSCustomObject]@{
            criteria = ([object[]]$criteria);
            pageNumber = 1;
            pageSize = $PageSize;
            sortBy = 0;
            sortOrder = 0;
        }));
        assetTypes = (New-Object -TypeName 'System.Object[]' -ArgumentList 0);
        flags = $Flags;
    } | ConvertTo-Json -Depth 4;
    $requestHeaders = [System.Collections.Generic.Dictionary[string,string]]::new();
    $requestHeaders.Add('Accept','application/json; charset=utf-8; api-version=3.2-preview.1');
    $requestHeaders.Add('Content-Type','application/json; charset=utf-8');
    $UriBuilder = [System.UriBuilder]::new($MyInvocation.MyCommand.Module.PrivateData.BaseMarketPlaceUri);
    $UriBuilder.Path = "/_apis/public/gallery/extensionquery";
    $Response = Invoke-WebRequest -Uri $UriBuilder.Uri -Method POST -Headers $requestHeaders -Body $requestBody -UseBasicParsing;
    if ($null -ne $Response) {
        $Response.Content | Out-File -LiteralPath ($PSScriptRoot | Join-Path -ChildPath 'Example.json');
        ($Response.Content | ConvertFrom-Json).results | ForEach-Object {
            $_.extensions | ForEach-Object { [VsGalleryExtension]::FromVsExtensionObject($_) } | Where-Object { $null -ne $_ }
        };
    }
}

Function New-VsExtensionFile {
    [CmdletBinding(DefaultParameterSetName = "WcPath")]
    [OutputType([VsExtensionFile])]
    Param(
        # Specifies a path to one or more locations. Wildcards are permitted.
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName= "WcPath", ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true, HelpMessage = "Path to one or more VSIX files.")]
        [ValidateNotNullOrEmpty()]
        [SupportsWildcards()]
        [ValidateScript({ $_ | Test-Path -PathType Leaf })]
        [string[]]$Path,

        # Specifies a path to one or more locations. Unlike the Path parameter, the value of the LiteralPath parameter is
        # used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters,
        # enclose it in single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any
        # characters as escape sequences.
        [Parameter(Mandatory = $true,  ParameterSetName = "LiteralPath", ValueFromPipelineByPropertyName = $true, HelpMessage = "Literal path to one or more VSIX files.")]
        [Alias("PSPath", "FullName")]
        [ValidateNotNullOrEmpty()]
        [ValidateScript({ Test-Path -LiteralPath $_ -PathType Leaf })]
        [string[]]$LiteralPath
    )

    Process {
        if ($PSCmdlet.ParameterSetName -eq 'Path') {
            ($Path | Get-Item -Force) | Where-Object { -not $_.PSIsContainer } | ForEach-Object {
                $M = $Script:VSIX_NAME_EXP.Match($_.BaseName);
                [System.Management.Automation.SemanticVersion]$Version = $null;
                if ($M.Success -and [System.Management.Automation.SemanticVersion]::TryParse($M.Groups[3].Value, [ref]$Version)) {
                    $VsExtensionFile = [VsExtensionFile]@{
                        PublisherId = $M.Groups[1].Value;
                        PackageId = $M.Groups[2].Value;
                        Version = $Version;
                        FullName = $_.FullName;
                        CreationTime = $_.CreationTimeUtc;
                        LastWriteTime = $_.LastWriteTimeUtc;
                        Length = $_.Length;
                    };
                    $G = $M.Groups[4];
                    if ($G.Success) { $VsExtensionFile.TargetPlatform = $G.Value }
                    $VsExtensionFile | Write-Output;
                } else {
                    Write-Error -Message "File name format is invalid for $($_.Name) in $($_.DirectoryName)" -Category InvalidArgument -ErrorId 'InvalidPackageName' -TargetObject $_;
                }
            }
        } else {
            (Get-Item -LiteralPath $LiteralPath -Force) | ForEach-Object {
                $M = $Script:VSIX_NAME_EXP.Match($_.BaseName);
                [System.Management.Automation.SemanticVersion]$Version = $null;
                if ($M.Success -and [System.Management.Automation.SemanticVersion]::TryParse($M.Groups[3].Value, [ref]$Version)) {
                    $VsExtensionFile = [VsExtensionFile]@{
                        PublisherId = $M.Groups[1].Value;
                        PackageId = $M.Groups[2].Value;
                        Version = $Version;
                        FullName = $_.FullName;
                        CreationTime = $_.CreationTimeUtc;
                        LastWriteTime = $_.LastWriteTimeUtc;
                        Length = $_.Length;
                    };
                    $G = $M.Groups[4];
                    if ($G.Success) { $VsExtensionFile.TargetPlatform = $G.Value }
                    $VsExtensionFile | Write-Output;
                } else {
                    Write-Error -Message "File name format is invalid for $($_.Name) in $($_.DirectoryName)" -Category InvalidArgument -ErrorId 'InvalidPackageName' -TargetObject $_;
                }
            }
        }
    }
}

Function Read-VsixManifest {
    [CmdletBinding()]
    [OutputType([ScannedVsExtensionFile])]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$ExtractedPath,

        [string]$SourcePath,
        
        [DateTime]$CreationTime,
        
        [DateTime]$LastWriteTime,
        
        [long]$FileLength
    )

    $Path = $ExtractedPath | Join-Path -ChildPath 'extension.vsixmanifest';
    if ($Path | Test-Path -PathType Leaf) {
        [Xml]$Xml = Get-Content -LiteralPath $Path;
        if ($null -ne $Xml) {
            if ($null -ne $Xml.DocumentElement) {
                $ScannedVsExtensionFile = [ScannedVsExtensionFile]@{
                    FullName = $SourcePath;
                    CreationTime = $CreationTime;
                    LastWriteTime = $LastWriteTime;
                    Length = $FileLength;
                };
                $nsmgr = [System.Xml.XmlNamespaceManager]::new($Xml.NameTable);
                $nsmgr.AddNamespace('vsx', $Xml.DocumentElement.PSBase.NamespaceURI);
                $Element = $Xml.SelectSingleNode('/vsx:PackageManifest/vsx:Metadata/vsx:Identity', $nsmgr);
                if ($null -ne $Element) {
                    $a = $Element.PSBase.SelectSingleNode('@Id');
                    if ($null -ne $a) {
                        $ScannedVsExtensionFile.PackageId = $a.Value;
                    } else {
                        Write-Error -Message "/vsx:PackageManifest/vsx:Metadata/vsx:Identity/@Id not found in $SourcePath" -Category ObjectNotFound -ErrorId 'NoManifestXmlIdentityId' -TargetObject ($SourcePath | Join-Path -ChildPath 'extension.vsixmanifest');
                    }
                    $a = $Element.PSBase.SelectSingleNode('@Version');
                    if ($null -ne $a) {
                        [System.Management.Automation.SemanticVersion]$Version = $null;
                        if ([System.Management.Automation.SemanticVersion]::TryParse($a.Value, [ref]$Version)) {
                            $ScannedVsExtensionFile.Version = $Version;
                        } else {
                            Write-Error -Message "Error parsing /vsx:PackageManifest/vsx:Metadata/vsx:Identity/@Version in $SourcePath" -Category ObjectNotFound -ErrorId 'ManifestXmlIdentityVersionParseError' -TargetObject ($SourcePath | Join-Path -ChildPath 'extension.vsixmanifest');
                        }
                    } else {
                        Write-Error -Message "/vsx:PackageManifest/vsx:Metadata/vsx:Identity/@Version not found in $SourcePath" -Category ObjectNotFound -ErrorId 'NoManifestXmlIdentityVersion' -TargetObject ($SourcePath | Join-Path -ChildPath 'extension.vsixmanifest');
                    }
                    $a = $Element.PSBase.SelectSingleNode('@Publisher');
                    if ($null -ne $a) {
                        $cc.PublisherId = $a.Value;
                    } else {
                        Write-Error -Message "/vsx:PackageManifest/vsx:Metadata/vsx:Identity/@Publisher not found in $SourcePath" -Category ObjectNotFound -ErrorId 'NoManifestXmlIdentityPublisher' -TargetObject ($SourcePath | Join-Path -ChildPath 'extension.vsixmanifest');
                    }
                    $a = $Element.PSBase.SelectSingleNode('@TargetPlatform');
                    if ($null -ne $a) { $ScannedVsExtensionFile.TargetPlatform = $a.Value }
                    $ScannedVsExtensionFile | Write-Output;
                } else {
                    Write-Error -Message "/vsx:PackageManifest/vsx:Metadata/vsx:Identity not found in $SourcePath" -Category ObjectNotFound -ErrorId 'NoManifestXmlIdentity' -TargetObject ($SourcePath | Join-Path -ChildPath 'extension.vsixmanifest');
                }
                $Element = $Xml.SelectSingleNode('/vsx:PackageManifest/vsx:Metadata/vsx:DisplayName', $nsmgr);
                if ($null -ne $Element -and -not $Element.IsEmpty) {
                    $ScannedVsExtensionFile.DisplayName = $Element.InnerText;
                } else {
                    Write-Error -Message "/vsx:PackageManifest/vsx:Metadata/vsx:DisplayName not found in $SourcePath" -Category ObjectNotFound -ErrorId 'NoManifestXml' -TargetObject ($SourcePath | Join-Path -ChildPath 'extension.vsixmanifest');
                }
                $Element = $Xml.SelectSingleNode('/vsx:PackageManifest/vsx:Metadata/vsx:Description', $nsmgr);
                if ($null -ne $Element -and -not $Element.IsEmpty) { $ScannedVsExtensionFile.Description = $Element.InnerText }
                $Element = $Xml.SelectSingleNode('/vsx:PackageManifest/vsx:Metadata/vsx:Icon', $nsmgr);
                if ($null -ne $Element -and -not $Element.IsEmpty) {
                    $Item.IconUri = $Element.InnerText;
                    $p = $ExtractedPath | Join-Path -ChildPath $Element.InnerText;
                    if ($p | Test-Path -PathType Leaf) {
                        $Item.IconData = Get-Content -LiteralPath $p -Raw;
                    }
                }
                if ([string]::IsNullOrWhiteSpace($ScannedVsExtensionFile.PackageId) -or [string]::IsNullOrWhiteSpace($ScannedVsExtensionFile.DisplayName) -or [string]::IsNullOrWhiteSpace($ScannedVsExtensionFile.Description) -or `
                    $null -eq $ScannedVsExtensionFile.Version -or [string]::IsNullOrWhiteSpace($ScannedVsExtensionFile.PublisherName) -or [string]::IsNullOrWhiteSpace($ScannedVsExtensionFile.IconUri)) {
                    $p = $ExtractedPath | Join-Path -ChildPath 'extension\package.json';
                    if ($p | Test-Path -PathType Leaf) {
                        $JsonObj = (Get-Content -LiteralPath $p) | ConvertFrom-Json;
                        if ($null -ne $JsonObj) {
                            if ([string]::IsNullOrWhiteSpace($ScannedVsExtensionFile.PackageId)) { $Item.PackageId = $JsonObj.name }
                            if ([string]::IsNullOrWhiteSpace($ScannedVsExtensionFile.DisplayName)) { $Item.DisplayName = $JsonObj.displayName }
                            if ([string]::IsNullOrWhiteSpace($ScannedVsExtensionFile.Description)) { $Item.Description = $JsonObj.description }
                            if ([string]::IsNullOrWhiteSpace($ScannedVsExtensionFile.Version)) { $Item.Version = $JsonObj.version }
                            if ([string]::IsNullOrWhiteSpace($ScannedVsExtensionFile.PublisherId)) { $Item.PublisherId = $JsonObj.publisher }
                            if ([string]::IsNullOrWhiteSpace($ScannedVsExtensionFile.IconUri) -and -not [string]::IsNullOrWhiteSpace($JsonObj.icon)) {
                                $ScannedVsExtensionFile.IconUri = $JsonObj.icon;
                                $p = $ExtractedPath | Join-Path -ChildPath $JsonObj.icon;
                                if ($p | Test-Path -PathType Leaf) {
                                    $Item.IconData = Get-Content -LiteralPath $p -Raw;
                                }
                            }
                        }
                    }
                }
                $ScannedVsExtensionFile | Write-Output;
            } else {
                Write-Error -Message "extension.vsixmanifest is empty in $SourcePath" -Category ObjectNotFound -ErrorId 'ManifestXmlEmpty' -TargetObject ($SourcePath | Join-Path -ChildPath 'extension.vsixmanifest');
            }
        }
    } else {
        Write-Error -Message "extension.vsixmanifest not found in $SourcePath" -Category ObjectNotFound -ErrorId 'ManifestXmlNotFound' -TargetObject ($SourcePath | Join-Path -ChildPath 'extension.vsixmanifest');
    }

}

Function Read-VsExtensionFile {
    [CmdletBinding(DefaultParameterSetName = "WcPath")]
    [OutputType([ScannedVsExtensionFile])]
    Param(
        # Specifies a path to one or more locations. Wildcards are permitted.
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName= "WcPath", ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true, HelpMessage = "Path to one or more VSIX files.")]
        [ValidateNotNullOrEmpty()]
        [SupportsWildcards()]
        [ValidateScript({ $_ | Test-Path -PathType Leaf })]
        [string[]]$Path,

        # Specifies a path to one or more locations. Unlike the Path parameter, the value of the LiteralPath parameter is
        # used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters,
        # enclose it in single quotation marks. Single quotation marks tell Windows PowerShell not to interpret any
        # characters as escape sequences.
        [Parameter(Mandatory = $true,  ParameterSetName = "LiteralPath", ValueFromPipelineByPropertyName = $true, HelpMessage = "Literal path to one or more VSIX files.")]
        [Alias("PSPath", "FullName")]
        [ValidateNotNullOrEmpty()]
        [ValidateScript({ Test-Path -LiteralPath $_ -PathType Leaf })]
        [string[]]$LiteralPath,

        [switch]$IgnoreFileNameMismatch
    )

    Begin {
        $TempPath = [System.IO.Path]::GetTempPath();
    }

    Process {
        $TempName = [Guid]::NewGuid().ToString('n');
        $TempFolder = $TempPath | Join-Path -ChildPath $TempName;
        while ($p | Test-Path) {
            $TempName = [Guid]::NewGuid().ToString('n');
            $TempFolder = $TempPath | Join-Path -ChildPath $TempName;
        }
        try {
            if ($IgnoreFileNameMismatch.IsPresent) {
                if ($PSCmdlet.ParameterSetName -eq 'Path') {
                    ($Path | Get-Item -Force) | Where-Object { -not $_.PSIsContainer } | ForEach-Object {
                        if ($TempFolder | Test-Path) { Remove-Item -LiteralPath $TempFolder -Recurse -Force }
                        (New-Item -Path $TempPath -Name $TempName -ItemType Directory) | Out-Null;
                        if ($TempFolder | Test-Path -PathType Container) {
                            Expand-Archive -Path $_.FullName -DestinationPath $TempFolder;
                            [ScannedVsExtensionFile]$ScannedVsExtensionFile = Read-VsixManifest -ExtractedPath $TempFolder -SourcePath $_.FullName -CreationTime $_.CreationTimeUtc -LastWriteTime $_.LastAccessTimeUtc -FileLength $_.Length;
                            if ($null -ne $ScannedVsExtensionFile -and ([string]::IsNullOrWhiteSpace($ScannedVsExtensionFile.PackageId) -or [string]::IsNullOrWhiteSpace($ScannedVsExtensionFile.PackageId) -or $null -eq $ScannedVsExtensionFile.Version)) {
                                
                            }
                        }
                    }
                } else {
                    (Get-Item -LiteralPath $LiteralPath -Force) | ForEach-Object {
                        if ($TempFolder | Test-Path) { Remove-Item -LiteralPath $TempFolder -Recurse -Force }
                        (New-Item -Path $TempPath -Name $TempName -ItemType Directory) | Out-Null;
                        if ($TempFolder | Test-Path -PathType Container) {
                            Expand-Archive -Path $_.FullName -DestinationPath $TempFolder;
                            [ScannedVsExtensionFile]$ScannedVsExtensionFile = Read-VsixManifest -ExtractedPath $TempFolder -SourcePath $_.FullName -CreationTime $_.CreationTimeUtc -LastWriteTime $_.LastAccessTimeUtc -FileLength $_.Length;
                        }
                    }
                }
            } else {
                if ($PSCmdlet.ParameterSetName -eq 'Path') {
                    ($Path | Get-Item -Force) | Where-Object { -not $_.PSIsContainer } | ForEach-Object {
                        $M = $Script:VSIX_NAME_EXP.Match($_.BaseName);
                        [System.Management.Automation.SemanticVersion]$Version = $null;
                        if ($M.Success -and [System.Management.Automation.SemanticVersion]::TryParse($M.Groups[3].Value, [ref]$Version)) {
                            if ($TempFolder | Test-Path) { Remove-Item -LiteralPath $TempFolder -Recurse -Force }
                            (New-Item -Path $TempPath -Name $TempName -ItemType Directory) | Out-Null;
                            if ($TempFolder | Test-Path -PathType Container) {
                                Expand-Archive -Path $_.FullName -DestinationPath $TempFolder;
                                [ScannedVsExtensionFile]$ScannedVsExtensionFile = Read-VsixManifest -ExtractedPath $TempFolder -SourcePath $_.FullName -CreationTime $_.CreationTimeUtc -LastWriteTime $_.LastAccessTimeUtc -FileLength $_.Length;
                                if ($null -ne $ScannedVsExtensionFile) {
                                    $G = $M.Groups[2];
                                    if ($G.Value -ine $ScannedVsExtensionFile.PackageId) {
                                        if ([string]::IsNullOrWhiteSpace($ScannedVsExtensionFile.PackageId)) {
                                            $ScannedVsExtensionFile.PackageId = $G.Value;
                                        } else {
                                            Write-Error -Message "Package ID formatted in file name, $($G.Value | ConvertTo-Json), does not match package ID $($ScannedVsExtensionFile.PackageId) in manifest" -ErrorId 'PackageIdMismatch' -TargetObject $ScannedVsExtensionFile;
                                        }
                                    }
                                    $G = $M.Groups[1];
                                    if ($G.Value -ine $ScannedVsExtensionFile.PublisherId) {
                                        if ([string]::IsNullOrWhiteSpace($ScannedVsExtensionFile.PublisherId)) {
                                            $ScannedVsExtensionFile.PublisherId = $G.Value;
                                        } else {
                                            Write-Error -Message "Publisher ID formatted in file name, $($G.Value | ConvertTo-Json), does not match publisher ID $($ScannedVsExtensionFile.PublisherId) in manifest" -ErrorId 'PublisherIdMismatch' -TargetObject $ScannedVsExtensionFile;
                                        }
                                    }
                                    if ($null -eq $ScannedVsExtensionFile.Version) {
                                        $ScannedVsExtensionFile.Version = $Version;
                                    } else {
                                        if (-not $Version.Equals($ScannedVsExtensionFile.Version)) {
                                            Write-Error -Message "Version formatted in file name, $($M.Groups[3].Value | ConvertTo-Json), does not match version $($ScannedVsExtensionFile.Version) in manifest" -ErrorId 'VersionMismatch' -TargetObject $ScannedVsExtensionFile;
                                        }
                                    }
                                    $G = $M.Groups[4];
                                    if ($G.Success -and $G.Value -ine $ScannedVsExtensionFile.TargetPlatform) {
                                        Write-Error -Message "Target platform formatted in file name, $($G.Value | ConvertTo-Json), does not match target platform $($ScannedVsExtensionFile.TargetPlatform) in manifest" -ErrorId 'TargetPlatformMismatch' -TargetObject $ScannedVsExtensionFile;
                                    }
                                    $ScannedVsExtensionFile | Write-Output;
                                }
                            }
                        } else {
                            Write-Error -Message "File name format is invalid for $($_.Name) in $($_.DirectoryName)" -Category InvalidArgument -ErrorId 'InvalidPackageName' -TargetObject $_;
                            if ($TempFolder | Test-Path) { Remove-Item -LiteralPath $TempFolder -Recurse -Force }
                            (New-Item -Path $TempPath -Name $TempName -ItemType Directory) | Out-Null;
                            if ($TempFolder | Test-Path -PathType Container) {
                                Expand-Archive -Path $_.FullName -DestinationPath $TempFolder;
                                Read-VsixManifest -ExtractedPath $TempFolder -SourcePath $_.FullName -CreationTime $_.CreationTimeUtc -LastWriteTime $_.LastAccessTimeUtc -FileLength $_.Length;
                            }
                        }
                    }
                } else {
                    (Get-Item -LiteralPath $LiteralPath -Force) | ForEach-Object {
                        $M = $Script:VSIX_NAME_EXP.Match($_.BaseName);
                        [System.Management.Automation.SemanticVersion]$Version = $null;
                        if ($M.Success -and [System.Management.Automation.SemanticVersion]::TryParse($M.Groups[3].Value, [ref]$Version)) {
                            if ($TempFolder | Test-Path) { Remove-Item -LiteralPath $TempFolder -Recurse -Force }
                            (New-Item -Path $TempPath -Name $TempName -ItemType Directory) | Out-Null;
                            if ($TempFolder | Test-Path -PathType Container) {
                                Expand-Archive -Path $_.FullName -DestinationPath $TempFolder;
                                [ScannedVsExtensionFile]$ScannedVsExtensionFile = Read-VsixManifest -ExtractedPath $TempFolder -SourcePath $_.FullName -CreationTime $_.CreationTimeUtc -LastWriteTime $_.LastAccessTimeUtc -FileLength $_.Length;
                                if ($null -ne $ScannedVsExtensionFile) {
                                    $G = $M.Groups[2];
                                    if ($G.Value -ine $ScannedVsExtensionFile.PackageId) {
                                        if ([string]::IsNullOrWhiteSpace($ScannedVsExtensionFile.PackageId)) {
                                            $ScannedVsExtensionFile.PackageId = $G.Value;
                                        } else {
                                            Write-Error -Message "Package ID formatted in file name, $($G.Value | ConvertTo-Json), does not match package ID $($ScannedVsExtensionFile.PackageId) in manifest" -ErrorId 'PackageIdMismatch' -TargetObject $ScannedVsExtensionFile;
                                        }
                                    }
                                    $G = $M.Groups[1];
                                    if ($G.Value -ine $ScannedVsExtensionFile.PublisherId) {
                                        if ([string]::IsNullOrWhiteSpace($ScannedVsExtensionFile.PublisherId)) {
                                            $ScannedVsExtensionFile.PublisherId = $G.Value;
                                        } else {
                                            Write-Error -Message "Publisher ID formatted in file name, $($G.Value | ConvertTo-Json), does not match publisher ID $($ScannedVsExtensionFile.PublisherId) in manifest" -ErrorId 'PublisherIdMismatch' -TargetObject $ScannedVsExtensionFile;
                                        }
                                    }
                                    if ($null -eq $ScannedVsExtensionFile.Version) {
                                        $ScannedVsExtensionFile.Version = $Version;
                                    } else {
                                        if (-not $Version.Equals($ScannedVsExtensionFile.Version)) {
                                            Write-Error -Message "Version formatted in file name, $($M.Groups[3].Value | ConvertTo-Json), does not match version $($ScannedVsExtensionFile.Version) in manifest" -ErrorId 'VersionMismatch' -TargetObject $ScannedVsExtensionFile;
                                        }
                                    }
                                    $G = $M.Groups[4];
                                    if ($G.Success -and $G.Value -ine $ScannedVsExtensionFile.TargetPlatform) {
                                        Write-Error -Message "Target platform formatted in file name, $($G.Value | ConvertTo-Json), does not match target platform $($ScannedVsExtensionFile.TargetPlatform) in manifest" -ErrorId 'TargetPlatformMismatch' -TargetObject $ScannedVsExtensionFile;
                                    }
                                    $ScannedVsExtensionFile | Write-Output;
                                }
                            }
                        } else {
                            Write-Error -Message "File name format is invalid for $($_.Name) in $($_.DirectoryName)" -Category InvalidArgument -ErrorId 'InvalidPackageName' -TargetObject $_;
                            if ($TempFolder | Test-Path) { Remove-Item -LiteralPath $TempFolder -Recurse -Force }
                            (New-Item -Path $TempPath -Name $TempName -ItemType Directory) | Out-Null;
                            if ($TempFolder | Test-Path -PathType Container) {
                                Expand-Archive -Path $_.FullName -DestinationPath $TempFolder;
                                Read-VsixManifest -ExtractedPath $TempFolder -SourcePath $_.FullName -CreationTime $_.CreationTimeUtc -LastWriteTime $_.LastAccessTimeUtc -FileLength $_.Length;
                            }
                        }
                    }
                }
            }
        } finally {
            if ($TempFolder | Test-Path) { Remove-Item -LiteralPath $TempFolder -Recurse -Force }
        }
    }
}

Function Get-VsExtensionFromMarketPlace {
    <#
    .SYNOPSIS
        Get VS extension from marketplace
    .DESCRIPTION
        Gets the specified vsix file from the Visual Studio marketplace.
    .LINK
        https://github.com/microsoft/vscode/blob/b6648af1eac7021cc646d1a9f59e93026a7c1689/build/lib/extensions.ts#L229
    #>
    [CmdletBinding()]
    [OutputType([ScannedVsExtensionFile])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$Publisher,
        
        [Parameter(Mandatory = $true, Position = 1)]
        [string]$ID,
        
        [Parameter(Mandatory = $true, Position = 2)]
        [System.Management.Automation.SemanticVersion]$Version,
        
        [Parameter(Mandatory = $true)]
        [string]$RepositoryFolder,
        
        [string]$TargetPlatform
    )

    $UriBuilder = [System.UriBuilder]::new($MyInvocation.MyCommand.Module.PrivateData.BaseMarketPlaceUri);
    $UriBuilder.Path = "/_apis/public/gallery/publishers/$([Uri]::EscapeDataString($Publisher))/vsextensions/$([Uri]::EscapeDataString($ID))/$([Uri]::EscapeDataString($Version))/vspackage";
    $FileName = "$Publisher.$ID-$Version";
    if ($PSBoundParameters.ContainsKey('TargetPlatform')) {
        $UriBuilder.Query = "targetPlatform=$([Uri]::EscapeDataString($TargetPlatform))";
        $FileName = "$FileName@$TargetPlatform";
    }
    # $requestHeaders = [System.Collections.Generic.Dictionary[string,string]]::new();
    # $requestHeaders.Add('Accept','application/json; charset=utf-8; api-version=3.2-preview.1');
    # $requestHeaders.Add('Content-Type','application/json; charset=utf-8');
    $Response = Invoke-WebRequest -Uri $UriBuilder.Uri -Method Get<# -Headers $requestHeaders#> -UseBasicParsing;
    $p = $RepositoryFolder | Join-Path -ChildPath $FileName;
    Set-Content -LiteralPath $p -Value $Response.RawContent -AsByteStream -Force;
    if ($p | Test-Path -PathType Leaf) {
        Get-VsCodeExtensionMetaData -Path $p;
    }
}