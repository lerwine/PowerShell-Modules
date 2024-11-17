<#

https://code.visualstudio.com/api/get-started/extension-anatomy
https://code.visualstudio.com/api/references/extension-manifest
https://code.visualstudio.com/api/working-with-extensions/publishing-extension
https://code.visualstudio.com/api/references/vscode-api
https://marketplace.visualstudio.com/vscode

https://github.com/microsoft/vscode/blob/1.95.0/src/vs/platform/extensionManagement/common/extensionManagement.ts

#>

# https://github.com/microsoft/vscode/blob/1.95.0/src/vs/platform/extensions/common/extensions.ts#L288
enum TargetVsixPlatform {
	UNIVERSAL;
	WIN32_X64;
	WIN32_ARM64;
    WIN32_IA32;
	LINUX_X64;
	LINUX_ARM64;
	LINUX_ARMHF;
	ALPINE_X64;
	ALPINE_ARM64;
	DARWIN_X64;
	DARWIN_ARM64;
	WEB;
	UNKNOWN;
}

# https://github.com/microsoft/vscode/blob/1.95.0/src/vs/base/common/platform.ts#L116
enum VsixPlatform {
	Windows;
	Linux;
    Alpine;
	Mac;
	Web;
}

class RawVsCodeGalleryExtensionPublisher {
    # interface IRawGalleryExtensionPublisher: https://github.com/microsoft/vscode/blob/1.95.0/src/vs/platform/extensionManagement/common/extensionGalleryService.ts#L59

    [ValidateNotNullOrWhiteSpace()]
	[string]$PublisherId;

    [ValidateNotNullOrWhiteSpace()]
    [string]$PublisherName;

    [ValidateNotNullOrWhiteSpace()]
	[string]$DisplayName;

    [AllowNull()]
    [AllowEmptyString()]
    [string]$Domain = $null;

    [AllowNull()]
    [Nullable[bool]]$IsDomainVerified = $null;

    [string] ToString() {
        return $this.PublisherName;
    }

    static [RawVsCodeGalleryExtensionPublisher] FromVsExtensionObject([object]$JsonObj) {
        if ($null -eq $JsonObj) { return $null }
        $Result = [RawVsCodeGalleryExtensionPublisher]::new();
        try {
            $Result.PublisherId = $JsonObj.publisherId;
            $Result.PublisherName = $JsonObj.publisherName;
            $Result.DisplayName = $JsonObj.displayName;
            $Result.Domain = $JsonObj.domain;
            $Result.IsDomainVerified = $JsonObj.isDomainVerified;
        } catch {
            Write-Error -Exception $_.Exception -Message "Cannot create object of type `"RawVsCodeGalleryExtensionPublisher`". $($_.Exception.Message) $($_.FullyQualifiedErrorId)" -Category InvalidArgument -ErrorId 'ExceptionWhenSetting' -CategoryTargetName 'JsonObj' -TargetObject $JsonObj -ErrorAction Stop;
            return $null;
        }
        return $Result;
    }
}

class RawVsCodeGalleryExtensionVersion {
    # interface IRawGalleryExtensionVersion: https://github.com/microsoft/vscode/blob/1.95.0/src/vs/platform/extensionManagement/common/extensionGalleryService.ts#L44

    [ValidateNotNull()]
	[semver]$Version;

	[DateTime]$LastUpdated;

    [ValidateNotNull()]
	[Uri]$AssetUri;

    [ValidateNotNull()]
	[Uri]$FallbackAssetUri;

    [AllowNull()]
    [Nullable[TargetVsixPlatform]]$TargetPlatForm = $null;

    [string] ToString() {
        if ($null -ne $this.TargetPlatForm) {
            $tp = $this.TargetPlatForm | ConvertFrom-TargetVsixPlatform -UniversalReturnsBlank;
            if ($tp.Length -gt 0) {
                return "$($this.Version)@$tp";
            }
        }
        return $this.Version.ToString();
    }

    static [RawVsCodeGalleryExtensionVersion] FromVsExtensionObject([object]$JsonObj) {
        if ($null -eq $JsonObj) { return $null }
        $Result = [RawVsCodeGalleryExtensionVersion]::new();
        try {
            $Result.Version = $JsonObj.version;
            if (-not [string]::IsNullOrWhiteSpace($JsonObj.targetPlatForm)) {
                $Result.TargetPlatForm = $JsonObj.targetPlatForm | ConvertTo-TargetVsixPlatform;
            }
            $Result.LastUpdated = $JsonObj.lastUpdated;
            $Result.AssetUri = $JsonObj.assetUri;
            $Result.FallbackAssetUri = $JsonObj.fallbackAssetUri;
        } catch {
            Write-Error -Exception $_.Exception -Message "Cannot create object of type `"RawVsCodeGalleryExtensionVersion`". $($_.Exception.Message) $($_.FullyQualifiedErrorId)" -Category InvalidArgument -ErrorId 'ExceptionWhenSetting' -CategoryTargetName 'JsonObj' -TargetObject $JsonObj -ErrorAction Stop;
            return $null;
        }
        $Result.LastUpdated = $Result.LastUpdated | Optimize-DateTime -Utc;
        return $Result;
    }
}

class RawVsCodeGalleryExtension {
    # interface IRawGalleryExtension: https://github.com/microsoft/vscode/blob/1.95.0/src/vs/platform/extensionManagement/common/extensionGalleryService.ts

    [ValidateNotNullOrWhiteSpace()]
    [string]$ExtensionId;

    [ValidateNotNullOrWhiteSpace()]
    [ValidatePattern('^[a-z\d]+(?:-[a-z\d]+)*$')]
    [string]$ExtensionName;

    [ValidateNotNullOrWhiteSpace()]
    [string]$DisplayName;

    [AllowNull()]
    [AllowEmptyString()]
    [string]$ShortDescription = $null;

    [ValidateNotNull()]
    [RawVsCodeGalleryExtensionPublisher]$Publisher;

    [ValidateNotNull()]
    [RawVsCodeGalleryExtensionVersion[]]$Versions;

    [AllowNull()]
    [string[]] $Tags;

    [DateTime]$ReleaseDate;

    [DateTime]$PublishedDate;

    [DateTime]$LastUpdated;

    [AllowNull()]
    [AllowEmptyString()]
    [AllowEmptyCollection()]
    [string[]]$Categories = $null;

    [string] ToString() {
        return "$($this.Publisher.PublisherName).$($this.ExtensionName)";
    }

    static [RawVsCodeGalleryExtension] FromVsExtensionObject([object]$JsonObj) {
        if ($null -eq $JsonObj) { return $null }
        $Result = [RawVsCodeGalleryExtension]::new();
        try {
            $Result.ExtensionId = $JsonObj.extensionId;
            $Result.ExtensionName = $JsonObj.extensionName;
            $Result.DisplayName = $JsonObj.displayName;
            $Result.LastUpdated = $JsonObj.lastUpdated;
            $Result.PublishedDate = $JsonObj.publishedDate;
            $Result.ReleaseDate = $JsonObj.releaseDate;
            $Result.ShortDescription = $JsonObj.shortDescription;
            $Result.Categories = $JsonObj.categories;
            $Result.Tags = $JsonObj.tags;
        } catch {
            Write-Error -Exception $_.Exception -Message "Cannot create object of type `"RawVsCodeGalleryExtension`". $($_.Exception.Message) $($_.FullyQualifiedErrorId)" -Category InvalidArgument -ErrorId 'ExceptionWhenSetting' -CategoryTargetName 'JsonObj' -TargetObject $JsonObj -ErrorAction Stop;
            return $null;
        }
        <#
            "installationTargets": [
                {
                    "target": "Microsoft.VisualStudio.Code",
                    "targetVersion": ""
                }
            ],
            "deploymentType": 0
        #>
        try {
            $Result.Publisher = [RawVsCodeGalleryExtensionPublisher]::FromVsExtensionObject($_.publisher);
        } catch {
            Write-Error -Exception $_.Exception -Message "Cannot initialize `"Publisher`" property on type `"RawVsCodeGalleryExtension`". $($_.Exception.Message) $($_.FullyQualifiedErrorId)" -Category InvalidArgument -ErrorId 'ExceptionWhenSetting' -CategoryTargetName 'JsonObj' -TargetObject $JsonObj -ErrorAction Stop;
            return $null;
        }
        try {
            $Result.Versions = @($JsonObj.versions) | ForEach-Object { [RawVsCodeGalleryExtensionVersion]::FromVsExtensionObject($_) }
        } catch {
            Write-Error -Exception $_.Exception -Message "Cannot initialize `"Versions`" property on type `"RawVsCodeGalleryExtension`". $($_.Exception.Message) $($_.FullyQualifiedErrorId)" -Category InvalidArgument -ErrorId 'ExceptionWhenSetting' -CategoryTargetName 'JsonObj' -TargetObject $JsonObj -ErrorAction Stop;
            return $null;
        }
        $Result.ReleaseDate = $Result.ReleaseDate | Optimize-DateTime -Utc;
        $Result.PublishedDate = $Result.PublishedDate | Optimize-DateTime -Utc;
        $Result.LastUpdated = $Result.LastUpdated | Optimize-DateTime -Utc;
        return $Result;
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

class VsCodeExtensionBaseFileName {
    [ValidatePattern('^[a-z\d]+(?:-[a-z\d]+)*$')]
    [string]$Publisher;

    [ValidatePattern('^[a-z\d]+(?:-[a-z\d]+)*$')]
    [string]$Name;

    [System.Management.Automation.SemanticVersion]$Version;

    [TargetVsixPlatform]$TargetPlatform = [TargetVsixPlatform]::UNIVERSAL;
}

class VsCodeExtensionFile : VsCodeExtensionBaseFileName {
    [string]$PSPath;
}

class ScannedVsCodeExtensionFile : VsCodeExtensionFile {
    [string]$DisplayName;
    [AllowNull()]
    [string]$Description = $null;
    [AllowNull()]
    [string]$IconUri = $null;
    [AllowNull()]
    [object[]]$IconData = $null;
}

<#
Flag values for the VS extension gallery search service.
This is used by function Find-VsCodeExtensionInMarketPlace.
Example implementation in TypeScript: https://github.com/microsoft/vscode/blob/1.95.0/src/vs/platform/extensionManagement/common/extensionGalleryService.ts#L102
#>
enum VsExtensionSearchFlags {
	# None is used to retrieve only the basic extension details.
	None = 0x0;

	# IncludeVersions will return version information for extensions returned
	IncludeVersions = 0x1;

	# IncludeFiles will return information about which files were found
	# within the extension that were stored independent of the manifest.
	# When asking for files, versions will be included as well since files
	# are returned as a property of the versions.
	# These files can be retrieved using the path to the file without
	# requiring the entire manifest be downloaded.
	IncludeFiles = 0x2;

	# Include the Categories and Tags that were added to the extension definition.
	IncludeCategoryAndTags = 0x4;

    # Include the details about which accounts the extension has been shared
    # with if the extension is a private extension.
	IncludeSharedAccounts = 0x8;

	# Include properties associated with versions of the extension
	IncludeVersionProperties = 0x10;

	# Excluding non-validated extensions will remove any extension versions that
	# either are in the process of being validated or have failed validation.
	ExcludeNonValidated = 0x20;

	# Include the set of installation targets the extension has requested.
	IncludeInstallationTargets = 0x40;

	# Include the base uri for assets of this extension
	IncludeAssetUri = 0x80;

	# Include the statistics associated with this extension
	IncludeStatistics = 0x100;

	# When retrieving versions from a query, only include the latest
	# version of the extensions that matched. This is useful when the
	# caller doesn't need all the published versions. It will save a
	# significant size in the returned payload.
	IncludeLatestVersionOnly = 0x200;

	# The Unpublished extension flag indicates that the extension can't be installed/downloaded.
	# Users who have installed such an extension can continue to use the extension.
	Unpublished = 0x1000;

	# Include the details if an extension is in conflict list or not
	IncludeNameConflictInfo = 0x8000;
}

Function ConvertTo-TargetVsixPlatform {
    <#
    .SYNOPSIS
        Converts string value to target platform value.
    .DESCRIPTION
        Converts string value to TargetVsixPlatform enum value.
    .LINK
        ConvertFrom-TargetVsixPlatform
    #>
    [CmdletBinding()]
    [OutputType([TargetVsixPlatform])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [AllowNull()]
        [AllowEmptyString()]
        [Alias('TargetPlatform', 'Platform')]
        [string[]]$InputString,

        [switch]$Force
    )

    Process {
        foreach ($s in $InputString) {
            if ([string]::IsNullOrEmpty($s)) {
                [TargetVsixPlatform]::UNIVERSAL | Write-Output;
            } else {
                switch ($s.Trim()) {
                    'win32-x64' { [TargetVsixPlatform]::WIN32_X64 | Write-Output; break; }
                    'win32-arm64' { [TargetVsixPlatform]::WIN32_ARM64 | Write-Output; break; }
                    'win32-ia32' { [TargetVsixPlatform]::WIN32_IA32 | Write-Output; break; }
                    'win32-x32' { [TargetVsixPlatform]::WIN32_IA32 | Write-Output; break; }
                    'linux-x64' { [TargetVsixPlatform]::LINUX_X64 | Write-Output; break; }
                    'linux-arm64' { [TargetVsixPlatform]::LINUX_ARM64 | Write-Output; break; }
                    'linux-armhf' { [TargetVsixPlatform]::LINUX_ARMHF | Write-Output; break; }
                    'linux-armf' { [TargetVsixPlatform]::LINUX_ARMHF | Write-Output; break; }
                    'linux-arm' { [TargetVsixPlatform]::LINUX_ARMHF | Write-Output; break; }
                    'alpine-x64' { [TargetVsixPlatform]::ALPINE_X64 | Write-Output; break; }
                    'alpine-arm64' { [TargetVsixPlatform]::ALPINE_ARM64 | Write-Output; break; }
                    'darwin-x64' { [TargetVsixPlatform]::DARWIN_X64 | Write-Output; break; }
                    'darwin-arm64' { [TargetVsixPlatform]::DARWIN_ARM64 | Write-Output; break; }
                    'web' { [TargetVsixPlatform]::WEB | Write-Output; break; }
                    'universal' { [TargetVsixPlatform]::UNIVERSAL | Write-Output; break; }
                    default {
                        if ($Force.IsPresent) {
                            [TargetVsixPlatform]::UNKNOWN | Write-Output;
                        } else {
                            Write-Error -Message "Unknown platform type: $($_ | ConvertTo-Json)" -Category InvalidArgument -ErrorId 'InvalidTargetPlatform' -TargetObject $s -CategoryTargetName 'InputString';
                        }
                        break;
                    }
                }
            }
        }
    }

}

Function ConvertFrom-TargetVsixPlatform {
    <#
    .SYNOPSIS
        Converts VSIX target platform value to string value.
    .DESCRIPTION
        Converts enum TargetVsixPlatform value to string value.
    .LINK
        ConvertTo-TargetVsixPlatform
    .LINK
        https://github.com/microsoft/vscode/blob/1.95.0/src/vs/platform/extensionManagement/common/extensionManagement.ts#L34
    #>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [Alias('TargetPlatform', 'Platform')]
        [TargetVsixPlatform[]]$InputValue,

        [switch]$AsDisplayName,

        [switch]$Force,

        [switch]$UniversalReturnsBlank
    )

    Process {
        if ($UniversalReturnsBlank.IsPresent) {
            if ($AsDisplayName.IsPresent) {
                foreach ($T in $InputValue) {
                    switch ($T) {
                        WIN32_X64 { 'Windows 64 bit' | Write-Output; break; }
                        WIN32_ARM64 { 'Windows ARM' | Write-Output; break; }
                        WIN32_IA32 { 'Windows 32 bit' | Write-Output; break; }
                        LINUX_X64 { 'Linux 64 bit' | Write-Output; break; }
                        LINUX_ARM64 { 'Linux ARM 64' | Write-Output; break; }
                        LINUX_ARMHF { 'Linux ARM' | Write-Output; break; }
                        ALPINE_X64 { 'Alpine Linux 64 bit' | Write-Output; break; }
                        ALPINE_ARM64 { 'Alpine ARM 64' | Write-Output; break; }
                        DARWIN_X64 { 'Mac' | Write-Output; break; }
                        DARWIN_ARM64 { 'Mac Silicon' | Write-Output; break; }
                        WEB { 'Web' | Write-Output; break; }
                        UNIVERSAL { '' | Write-Output; break; }
                        default {
                            if ($Force.IsPresent) {
                                'Unknown' | Write-Output;
                            } else {
                                Write-Error -Message 'Value does not represent a valid Target VSIX Platform' -Category InvalidArgument -ErrorId 'InvalidVsixPlatform' -TargetObject $T -CategoryTargetName 'InputValue';
                            }
                            break;
                        }
                    }
                }
            } else {
                foreach ($T in $InputValue) {
                    switch ($T) {
                        WIN32_X64 { 'win32-x64' | Write-Output; break; }
                        WIN32_ARM64 { 'win32-arm64' | Write-Output; break; }
                        WIN32_IA32 { 'win32-ia32' | Write-Output; break; }
                        LINUX_X64 { 'linux-x64' | Write-Output; break; }
                        LINUX_ARM64 { 'linux-arm64' | Write-Output; break; }
                        LINUX_ARMHF { 'linux-armhf' | Write-Output; break; }
                        ALPINE_X64 { 'alpine-x64' | Write-Output; break; }
                        ALPINE_ARM64 { 'alpine-arm64' | Write-Output; break; }
                        DARWIN_X64 { 'darwin-x64' | Write-Output; break; }
                        DARWIN_ARM64 { 'darwin-arm64' | Write-Output; break; }
                        WEB { 'web' | Write-Output; break; }
                        UNIVERSAL { '' | Write-Output; break; }
                        default {
                            if ($Force.IsPresent) {
                                'unknown' | Write-Output;
                            } else {
                                Write-Error -Message 'Value does not represent a valid Target VSIX Platform' -Category InvalidArgument -ErrorId 'InvalidVsixPlatform' -TargetObject $T -CategoryTargetName 'InputValue';
                            }
                            break;
                        }
                    }
                }
            }
        } else {
            if ($AsDisplayName.IsPresent) {
                foreach ($T in $InputValue) {
                    switch ($T) {
                        WIN32_X64 { 'Windows 64 bit' | Write-Output; break; }
                        WIN32_ARM64 { 'Windows ARM' | Write-Output; break; }
                        WIN32_IA32 { 'Windows 32 bit' | Write-Output; break; }
                        LINUX_X64 { 'Linux 64 bit' | Write-Output; break; }
                        LINUX_ARM64 { 'Linux ARM 64' | Write-Output; break; }
                        LINUX_ARMHF { 'Linux ARM' | Write-Output; break; }
                        ALPINE_X64 { 'Alpine Linux 64 bit' | Write-Output; break; }
                        ALPINE_ARM64 { 'Alpine ARM 64' | Write-Output; break; }
                        DARWIN_X64 { 'Mac' | Write-Output; break; }
                        DARWIN_ARM64 { 'Mac Silicon' | Write-Output; break; }
                        WEB { 'Web' | Write-Output; break; }
                        UNIVERSAL { 'Universal' | Write-Output; break; }
                        default {
                            if ($Force.IsPresent) {
                                'Unknown' | Write-Output;
                            } else {
                                Write-Error -Message 'Value does not represent a valid Target VSIX Platform' -Category InvalidArgument -ErrorId 'InvalidVsixPlatform' -TargetObject $T -CategoryTargetName 'InputValue';
                            }
                            break;
                        }
                    }
                }
            } else {
                foreach ($T in $InputValue) {
                    switch ($T) {
                        WIN32_X64 { 'win32-x64' | Write-Output; break; }
                        WIN32_ARM64 { 'win32-arm64' | Write-Output; break; }
                        WIN32_IA32 { 'win32-ia32' | Write-Output; break; }
                        LINUX_X64 { 'linux-x64' | Write-Output; break; }
                        LINUX_ARM64 { 'linux-arm64' | Write-Output; break; }
                        LINUX_ARMHF { 'linux-armhf' | Write-Output; break; }
                        ALPINE_X64 { 'alpine-x64' | Write-Output; break; }
                        ALPINE_ARM64 { 'alpine-arm64' | Write-Output; break; }
                        DARWIN_X64 { 'darwin-x64' | Write-Output; break; }
                        DARWIN_ARM64 { 'darwin-arm64' | Write-Output; break; }
                        WEB { 'web' | Write-Output; break; }
                        UNIVERSAL { 'universal' | Write-Output; break; }
                        default {
                            if ($Force.IsPresent) {
                                'unknown' | Write-Output;
                            } else {
                                Write-Error -Message 'Value does not represent a valid Target VSIX Platform' -Category InvalidArgument -ErrorId 'InvalidVsixPlatform' -TargetObject $T -CategoryTargetName 'InputValue';
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}

Function Split-VsCodeExtensionBaseFileName {
    <#
    .SYNOPSIS
        Parses components of the base file name of VS Code VSIX extension packages.
    .DESCRIPTION
        Gets specified tokens from the formatted base file name of VS Code VSIX extension packages.
    #>
    [CmdletBinding(DefaultParameterSetName = '')]
    [OutputType([VsCodeExtensionBaseFileName], ParameterSetName = 'Components')]
    [OutputType([string], ParameterSetName = 'Publisher', 'ExtensionName')]
    [OutputType([semver], ParameterSetName = 'Version')]
    [OutputType([TargetVsixPlatform], ParameterSetName = 'TargetPlatForm')]
    Param(
        # The base file name to parse.
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [Alias('BaseName')]
        [string[]]$InputString,

        # Return all component values as a VsCodeExtensionBaseFileName object.
        [Parameter(ParameterSetName = 'Components')]
        [switch]$Components,

        # Gets the publisher name.
        [Parameter(Mandatory = $true, ParameterSetName = 'Publisher')]
        [switch]$Publisher,

        # Gets the extension name.
        [Parameter(Mandatory = $true, ParameterSetName = 'ExtensionName')]
        [switch]$ExtensionName,

        # Gets the version as a semver object.
        [Parameter(Mandatory = $true, ParameterSetName = 'Version')]
        [switch]$Version,

        # Gets the target platform as a TargetVsixPlatform value.
        [Parameter(Mandatory = $true, ParameterSetName = 'TargetPlatForm')]
        [switch]$TargetPlatForm
    )

    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'Publisher' {
                foreach ($BaseName in $InputString) {
                    $M = $Script:VSIX_BASENAME_PARSE_REGEX.Match($BaseName);
                    if ($M.Success) {
                        $M.Groups[1].Value.ToLower() | Write-Output;
                    } else {
                        Write-Error -Message "Input string is not a valid VS Code VSIX file basename: $($BaseName | ConvertTo-Json)" -Category InvalidArgument -ErrorId 'InvalidBaseName' -TargetObject $BaseName -CategoryTargetName 'InputString';
                    }
                }
                break;
            }
            'ExtensionName' {
                foreach ($BaseName in $InputString) {
                    $M = $Script:VSIX_BASENAME_PARSE_REGEX.Match($BaseName);
                    if ($M.Success) {
                        $M.Groups[2].Value.ToLower() | Write-Output;
                    } else {
                        Write-Error -Message "Input string is not a valid VS Code VSIX file basename: $($BaseName | ConvertTo-Json)" -Category InvalidArgument -ErrorId 'InvalidBaseName' -TargetObject $BaseName -CategoryTargetName 'InputString';
                    }
                }
                break;
            }
            'Version' {
                [semver]$SemVer = $null;
                foreach ($BaseName in $InputString) {
                    $M = $Script:VSIX_BASENAME_PARSE_REGEX.Match($BaseName);
                    if ($M.Success -and [semver]::TryParse($m.Groups[3].Value, [ref]$SemVer) -and $null -eq $SemVer.PreReleaseLabel -and $null -eq $SemVer.BuildLabel) {
                        $SemVer | Write-Output;
                    } else {
                        Write-Error -Message "Input string is not a valid VS Code VSIX file basename: $($BaseName | ConvertTo-Json)" -Category InvalidArgument -ErrorId 'InvalidBaseName' -TargetObject $BaseName -CategoryTargetName 'InputString';
                    }
                }
                break;
            }
            'TargetPlatForm' {
                foreach ($BaseName in $InputString) {
                    $M = $Script:VSIX_BASENAME_PARSE_REGEX.Match($BaseName);
                    if ($M.Success) {
                        $G = $M.Groups[4];
                        if ($G.Success) {
                            try { ($G.Value | ConvertTo-TargetVsixPlatform) | Write-Output }
                            catch {
                                Write-Error -Exception $_.Exception -Message "Input string is not a valid VS Code VSIX file basename: $($BaseName | ConvertTo-Json)" -Category InvalidArgument -ErrorId 'InvalidBaseName' -TargetObject $BaseName -CategoryTargetName 'InputString';
                            }
                        } else {
                            [TargetVsixPlatform]::UNIVERSAL | Write-Output;
                        }
                    } else {
                        Write-Error -Message "Input string is not a valid VS Code VSIX file basename: $($BaseName | ConvertTo-Json)" -Category InvalidArgument -ErrorId 'InvalidBaseName' -TargetObject $BaseName -CategoryTargetName 'InputString';
                    }
                }
                break;
            }
            default {
                [semver]$SemVer = $null;
                foreach ($BaseName in $InputString) {
                    $M = $Script:VSIX_BASENAME_PARSE_REGEX.Match($BaseName);
                    if ($M.Success -and [semver]::TryParse($m.Groups[3].Value, [ref]$SemVer) -and $null -eq $SemVer.PreReleaseLabel -and $null -eq $SemVer.BuildLabel) {
                        $G = $M.Groups[4];
                        if ($G.Success) {
                            try {
                                [VsCodeExtensionBaseFileName]@{
                                    Publisher = $M.Groups[1].Value.ToLower();
                                    Name = $M.Groups[2].Value.ToLower();
                                    Version = $SemVer;
                                    Platform = $G.Value | ConvertTo-TargetVsixPlatform;
                                } | Write-Output;
                            } catch {
                                Write-Error -Exception $_.Exception -Message "Input string is not a valid VS Code VSIX file basename: $($BaseName | ConvertTo-Json)" -Category InvalidArgument -ErrorId 'InvalidBaseName' -TargetObject $BaseName -CategoryTargetName 'InputString';
                            }
                        } else {
                            [VsCodeExtensionBaseFileName]@{
                                Publisher = $M.Groups[1].Value.ToLower();
                                Name = $M.Groups[2].Value.ToLower();
                                Version = $SemVer;
                            } | Write-Output;
                        }
                    } else {
                        Write-Error -Message "Input string is not a valid VS Code VSIX file basename: $($BaseName | ConvertTo-Json)" -Category InvalidArgument -ErrorId 'InvalidBaseName' -TargetObject $BaseName -CategoryTargetName 'InputString';
                    }
                }
                break;
            }
        }
    }
}

Function Get-TargetVsixPlatform {
    <#
    .SYNOPSIS
        Gets a TargetPlatform value from a platform and architecture type.
    .DESCRIPTION
        Gets the TargetPlatform enum value according to the platform and architecture types.
        Returns UNKNOWN if the combined platform and architecture type is not valid.
    #>
    [CmdletBinding()]
    Param(
        # The base platform type.
        [Parameter(Mandatory = $true, Position = 0)]
        [VsixPlatform]$Platform,

        # The architecture type.
        [Parameter(Position = 1)]
        [ValidateSet('x64', 'arm64', 'arm', 'armf', 'armhf', 'ia32', 'x32')]
        [string]$Arch
    )

	switch ($Platform) {
		Windows {
			if ($Arch -eq 'x64') { return [TargetPlatform]::WIN32_X64 }
			if ($Arch -eq 'arm64') { return [TargetPlatform]::WIN32_ARM64 }
			if ($Arch -eq 'ia32' -or $Arch -eq 'x32') { return [TargetPlatform]::WIN32_IA32 }
            break;
        }
		Linux {
			if ($Arch -eq 'x64') { return [TargetPlatform]::LINUX_X64 }
			if ($Arch -eq 'arm64') { return [TargetPlatform]::LINUX_ARM64 }
			if ($Arch -eq 'arm' -or $Arch -eq 'armf' -or $Arch -eq 'armhf') { return [TargetPlatform]::LINUX_ARMHF }
            break;
        }
		Alpine {
			if ($Arch -eq 'x64') { return [TargetPlatform]::ALPINE_X64 }
			if ($Arch -eq 'arm64') { return [TargetPlatform]::ALPINE_ARM64 }
            break;
        }
		Mac {
			if ($Arch -eq 'x64') { return [TargetPlatform]::DARWIN_X64 }
			if ($Arch -eq 'arm64') { return [TargetPlatform]::DARWIN_ARM64 }
            break;
        }
		Web  { return [TargetPlatform]::WEB; }
	}
    return [TargetPlatform]::UNKNOWN;
}

Function Get-VsExtensionsGalleryServiceUri {
    <#
    .SYNOPSIS
        Gets the VS gallery extension service URI.
    .DESCRIPTION
        Gets the URI to use for a call to the VS gallery extension service.
    .NOTES
        Information or caveats about the function e.g. 'This function is not supported in Linux'
    .LINK
        https://github.com/microsoft/vscode/blob/1.95.0/src/vs/platform/extensionManagement/common/extensionGalleryService.ts
    #>
    [CmdletBinding()]
    [OutputType([Uri])]
    Param(
        [Parameter(Mandatory = $true)]
        [ValidatePattern('(?:[\w!&-.:;=@\[\]~]+|%[\da-f]{2})+(?:/(?:[\w!&-.:;=@\[\]~]+|%[\da-f]{2})+)*$')]
        # The relative path component to append to the service URI.
        [string[]]$RelativePath,

        [ValidatePattern('[\w!$&-/=@\[\]~][\w!$&-/=?@\[\]~]*|%[\da-f]{2}(?:[\w!$&-/=?@\[\]~]+|%[\da-f]{2})*')]
        # The query string to append to the URI.
        [string]$Query,

        [ValidateScript({ return $_.IsAbsoluteUri })]
        # Base URI of the VS extensions gallery web service.
        [Uri]$ServiceUri
    )

    if ($PSBoundParameters.ContainsKey('ServiceUri')) {
        $Local:UriBuilder = [UriBuilder]::new($ServiceUri);
    } else {
        if ([string]::IsNullOrEmpty($MyInvocation.MyCommand.Module.PrivateData.VsExtensionsGalleryServiceUri)) {
            $Local:UriBuilder = [UriBuilder]::new('https://marketplace.visualstudio.com/_apis/public/gallery/');
        } else {
            try {
                [Uri]$Local:Uri = $MyInvocation.MyCommand.Module.PrivateData.VsExtensionsGalleryServiceUri;
            } catch {
                Write-Error -Exception $_.Exception -Message 'VsExtensionsGalleryServiceUri in Module PrivateData does not contain a valid absolute URI.' -Category InvalidOperation -ErrorId 'InvalidVsExtensionsGalleryServiceUri' `
                    -TargetObject $MyInvocation.MyCommand.Module.PrivateData.VsExtensionsGalleryServiceUri -CategoryTargetName 'VsExtensionsGalleryServiceUri' -ErrorAction Stop;
            }
            if (-not $Local:Uri.IsAbsoluteUri) {
                Write-Error -Message 'VsExtensionsGalleryServiceUri in Module PrivateData does not contain an absolute URI' -Category InvalidOperation -ErrorId 'InvalidVsExtensionsGalleryServiceUri' `
                    -TargetObject $MyInvocation.MyCommand.Module.PrivateData.VsExtensionsGalleryServiceUri -CategoryTargetName 'VsExtensionsGalleryServiceUri' -ErrorAction Stop;
            }
            $Local:UriBuilder = [UriBuilder]::new($Local:Uri);
        }
    }
    if (-not $Local:UriBuilder.Path.EndsWith('/')) { $Local:UriBuilder.Path = "$($Local:UriBuilder.Path)/" }
    $Local:UriBuilder.Path = $Local:UriBuilder.Path + ($RelativePath -join '/');
    if ($PSBoundParameters.ContainsKey('Query')) { $Local:UriBuilder.Query = "?$Query" }
    $UriBuilder.Fragment = $null;
    return $UriBuilder.Uri;
}

Function Find-VsCodeExtensionInMarketPlace {
    <#
    .SYNOPSIS
        Queries the VS Extension Marketplace for a package.
    .DESCRIPTION
        Calls the extensionquery web API for the VS extension marketplace gallery service.
    .LINK
        https://github.com/microsoft/vscode/blob/1.95.0/src/vs/platform/extensionManagement/common/extensionGalleryService.ts#L801
    .LINK
        https://github.com/microsoft/vscode/blob/1.95.0/src/vs/platform/extensionManagement/common/extensionManagement.ts#L368
    #>

    [CmdletBinding()]
    [OutputType([RawVsCodeGalleryExtension])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipelineByPropertyName = $true)]
        [ValidatePattern('^[a-z][a-z\d]*(?:-[a-z][a-z\d]*)*$')]
        [Alias('PublisherName')]
        # The publisher identifier string.
        [string]$Publisher,

        [Parameter(Mandatory = $true, Position = 1, ValueFromPipelineByPropertyName = $true)]
        [ValidatePattern('^[a-z][a-z\d]*(?:-[a-z][a-z\d]*)*$')]
        [Alias('ExtensionName', 'Name')]
        # The package identifier string.
        [string]$Extension,

        [Parameter(Position = 2, ValueFromPipelineByPropertyName = $true)]
        # The explicit target platform to look for.
        [TargetVsixPlatform]$TargetPlatform,

        [ValidateRange(1, [int]::MaxValue)]
        [int]$MaxPage = 10000,

        [ValidateRange(1, [int]::MaxValue)]
        [int]$PageNumber = 1,

        [ValidateRange(1, [int]::MaxValue)]
        [int]$PageSize = 100,

        # When retrieving versions from a query, only include the latest version of the extensions that matched. This is useful when the caller doesn't need all the published versions.
        [switch]$IncludeLatestVersionOnly,

        # Do not remove any extension versions that either are in the process of being validated or have failed validation.
        [switch]$IncludeNonValidated,

        [string]$InstallationTarget = 'Microsoft.VisualStudio.Code',

        [ValidateScript({ return $_.IsAbsoluteUri })]
        # Base URI of the VS extensions gallery web service.
        [Uri]$ServiceUri
    )

    [int]$Flags = [VsExtensionSearchFlags]::IncludeAssetUri -bor [VsExtensionSearchFlags]::IncludeCategoryAndTags -bor [VsExtensionSearchFlags]::IncludeInstallationTargets;
    if ($IncludeLatestVersionOnly.IsPresent) {
        $Flags = $Flags -bor [VsExtensionSearchFlags]::IncludeLatestVersionOnly;
    } else {
        $Flags = $Flags -bor [VsExtensionSearchFlags]::IncludeVersions;
    }
    if (-not $IncludeNonValidated.IsPresent) {
        $Flags = $Flags -bor [VsExtensionSearchFlags]::ExcludeNonValidated;
    }

    $criteria = @([PSCustomObject]@{
        filterType = 7;
        value = "$Publisher.$Extension";
    }, [PSCustomObject]@{
        filterType = 8;
        value = $InstallationTarget;
    });
    if ($PSBoundParameters.ContainsKey('TargetPlatform')) {
        $criteria += [PSCustomObject]@{
            filterType = 23;
            value = $TargetPlatform | ConvertFrom-TargetVsixPlatform;
        }
    }

    $requestBody = [PSCustomObject]@{
        filters = ([object[]]@([PSCustomObject]@{
            criteria = ([object[]]$criteria);
            pageNumber = $PageNumber;
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
    if ($PSBoundParameters.ContainsKey('ServiceUri')) {
        $Local:Uri = Get-VsExtensionsGalleryServiceUri -RelativePath 'extensionquery' -ServiceUri $ServiceUri;
    } else {
        $Local:Uri = Get-VsExtensionsGalleryServiceUri -RelativePath 'extensionquery';
    }

    $Response = Invoke-WebRequest -Uri $Local:Uri -Method POST -Headers $requestHeaders -Body $requestBody -UseBasicParsing;
    if ($null -ne $Response) {
        $Response.Content | Out-File -LiteralPath ($PSScriptRoot | Join-Path -ChildPath 'Example.json');
        $ResponseJson = $Response.Content | ConvertFrom-Json;
        $ResponseJson.results | ForEach-Object {
            $_.extensions | ForEach-Object { [RawVsCodeGalleryExtension]::FromVsExtensionObject($_) } | Where-Object { $null -ne $_ }
        };
    }
}

Function Get-VsCodeExtensionFromMarketPlace {
    <#
    .SYNOPSIS
        Downloads VS extension from the Visual Studio marketplace.
    .DESCRIPTION
        Gets the specified vsix file from the Visual Studio marketplace.
    .LINK
        https://github.com/microsoft/vscode/blob/1.95.0/build/lib/extensions.ts#L229
    .LINK
        https://github.com/microsoft/vscode/blob/1.95.0/src/vs/workbench/contrib/extensions/browser/extensionsActions.ts#L208
    #>
    [CmdletBinding()]
    [OutputType([VsCodeExtensionFile])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipelineByPropertyName = $true)]
        [ValidatePattern('^[a-z\d]+(?:-[a-z\d]+)*$')]
        [Alias('PublisherName')]
        [string]$Publisher,

        [Parameter(Mandatory = $true, Position = 1, ValueFromPipelineByPropertyName = $true)]
        [ValidatePattern('^[a-z\d]+(?:-[a-z\d]+)*$')]
        [Alias('ExtensionName', 'Name')]
        [string]$Extension,

        [Parameter(Mandatory = $true, Position = 2, ValueFromPipelineByPropertyName = $true)]
        [ValidateScript({ [string]::IsNullOrEmpty($Version.PreReleaseLabel) -and [string]::IsNullOrEmpty($Version.BuildLabel) })]
        [semver]$Version,

        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [TargetVsixPlatform]$TargetPlatform,

        [Parameter(Mandatory = $true)]
        [string]$RepositoryFolder,

        [ValidateScript({ return $_.IsAbsoluteUri })]
        [Uri]$ServiceUri
    )

    $FileName = "$Publisher.$Extension-$Version";
    if ($PSBoundParameters.ContainsKey('TargetPlatform')) {
        $tps = $TargetPlatform | ConvertFrom-TargetVsixPlatform;
        if ($PSBoundParameters.ContainsKey('ServiceUri')) {
            $Local:Uri = Get-VsExtensionsGalleryServiceUri -RelativePath 'publishers', $Publisher.ToLower(), 'vsextensions', $Extension.ToLower(), $Version, 'vspackage' -Query "targetPlatform=$tps" -ServiceUri $ServiceUri;
        } else {
            $Local:Uri = Get-VsExtensionsGalleryServiceUri -RelativePath 'publishers', $Publisher.ToLower(), 'vsextensions', $Extension.ToLower(), $Version, 'vspackage' -Query "targetPlatform=$tps";
        }
        $FileName = "$FileName@$tps";
    } else {
        if ($PSBoundParameters.ContainsKey('ServiceUri')) {
            $Local:Uri = Get-VsExtensionsGalleryServiceUri -RelativePath 'publishers', $Publisher.ToLower(), 'vsextensions', $Extension.ToLower(), $Version, 'vspackage' -ServiceUri $ServiceUri;
        } else {
            $Local:Uri = Get-VsExtensionsGalleryServiceUri -RelativePath 'publishers', $Publisher.ToLower(), 'vsextensions', $Extension, $Version.ToLower(), 'vspackage';
        }
    }
    $Response = Invoke-WebRequest -Uri $Local:Uri -Method Get -UseBasicParsing;
    $p = $RepositoryFolder | Join-Path -ChildPath $FileName;
    Set-Content -LiteralPath $p -Value $Response.RawContent -AsByteStream -Force;
    if ($p | Test-Path -PathType Leaf) {
        Get-VsCodeExtensionMetaData -Path $p;
    }
}

Function Get-VsCodeExtensionsInRepository {
    [CmdletBinding(DefaultParameterSetName = 'WcPath')]
    [OutputType([VsCodeExtensionFile])]
    Param(
        # Specifies a path to one or more locations. Wildcards are permitted.
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = "WcPath", ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true,
            HelpMessage = "Path to subdirectory representing a VS Code extension repository.")]
        [ValidateNotNullOrEmpty()]
        [SupportsWildcards()]
        [string[]]$Path,

        # Literal path to subdirectory representing a VS Code extension repository. Unlike the Path parameter, the value of the LiteralPath parameter is
        # used exactly as it is typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose it in single quotation marks.
        # Single quotation marks tell Windows PowerShell not to interpret any characters as escape sequences.
        [Parameter(Mandatory = $true, ParameterSetName = "LiteralPath", ValueFromPipelineByPropertyName = $true,
            HelpMessage = "Literal path to subdirectory representing a VS Code extension repository.")]
        [Alias("PSPath", "LP", "FullName")]
        [ValidateNotNullOrEmpty()]
        [string[]]$LiteralPath
    )

    Begin {
        function GetExtensions ([string]$DirectoryName) {
            (Get-ChildItem -LiteralPath $DirectoryName -Filter '*.vsix' -Force) | ForEach-Object {
                $M = $Script:VSIX_BASENAME_PARSE_REGEX.Match($_.BaseName);
                if ($M.Success) {
                    [semver]$Semver = $null;
                    if ([semver]::TryParse($M.Groups[3].Value, [ref]$Semver)) {
                        $G = $M.Groups[4];
                        if ($G.Success) {
                            $Platform = $G.Value | ConvertTo-TargetVsixPlatform -Force;
                            if ($Platform -eq [TargetVsixPlatform]::UNKNOWN) {
                                Write-Warning -Message "$($_.PSChildName | ConvertTo-Json) references an unknown platform (Container = $($_.PSParentPath | ConvertTo-Json))";
                            } else {
                                [VsCodeExtensionFile]@{
                                    Publisher = $M.Groups[1].Value.ToLower();
                                    Name = $M.Groups[2].Value.ToLower();
                                    Version = $Version;
                                    PSPath = $_.PSPath;
                                    Platform = $Platform;
                                } | Write-Output;
                            }
                        } else {
                            [VsCodeExtensionFile]@{
                                Publisher = $M.Groups[1].Value.ToLower();
                                Name = $M.Groups[2].Value.ToLower();
                                Version = $Version;
                                PSPath = $_.PSPath;
                            } | Write-Output;
                        }
                    } else {
                        Write-Warning -Message "Version component in $($_.PSChildName | ConvertTo-Json) does not conform to VS Code package file name (Container = $($_.PSParentPath | ConvertTo-Json))";
                    }
                } else {
                    Write-Warning -Message "$($_.PSChildName | ConvertTo-Json) does not conform to VS Code package file name (Container = $($_.PSParentPath | ConvertTo-Json))";
                }
            }
        }
    }

    Process {
        if ($PSCmdlet.ParameterSetName -eq 'LiteralPath') {
            foreach ($P in $LiteralPath) {
                if (Test-Path -LiteralPath $P -PathType Container) {
                    GetExtensions $P;
                } else {
                    if (Test-Path -LiteralPath $P) {
                        Write-Error -Message "Path does not refer to a subdirectory: $($P | ConvertTo-Json)" -Category InvalidArgument -ErrorId 'PathNotSubdirectory' -TargetObject $P -CategoryTargetName 'LiteralPath';
                    } else {
                        Write-Error -Message "Path not found: $($P | ConvertTo-Json)" -Category ObjectNotFound -ErrorId 'PathNotSubdirectory' -TargetObject $P -CategoryTargetName 'LiteralPath';
                    }
                }
            }
        } else {
            foreach ($P in $Path) {
                if ($P | Test-Path -PathType Container) {
                    try {
                        $Local:Pi = @($P | Resolve-Path -ErrorAction Stop);
                    } catch {
                        Write-Error -Exception $_.Exception -Category $_.CategoryInfo.Category -ErrorId 'ResolvePathError' -TargetObject $P -CategoryTargetName 'Path';
                        $Local:Pi = @();
                    }
                    $Local:Pi | ForEach-Object {
                        if (Test-Path -LiteralPath $_.Path -PathType Container) {
                            GetExtensions $_.Path;
                        }
                    }
                } else {
                    if (Test-Path -LiteralPath $P) {
                        Write-Error -Message "Path does not refer to a subdirectory: $($P | ConvertTo-Json)" -Category InvalidArgument -ErrorId 'PathNotSubdirectory' -TargetObject $P -CategoryTargetName 'Path';
                    } else {
                        Write-Error -Message "Path not found: $($P | ConvertTo-Json)" -Category ObjectNotFound -ErrorId 'PathNotSubdirectory' -TargetObject $P -CategoryTargetName 'Path';
                    }
                }
            }
        }
    }
}

if ($null -eq $Script:VSIX_BASENAME_PARSE_REGEX) {
    New-Variable -Name 'VSIX_ID_PATTERN' -Option Constant -Scope 'Script' -Value '^[a-z][a-z\d]*(?:-[a-z][a-z\d]*)*$' -Description 'Regular expression string that matches a VSIX extension or publisher name token';
    # https://github.com/microsoft/vscode/blob/1.95.0/src/vs/platform/extensionManagement/common/extensionManagement.ts#L16
    New-Variable -Name 'EXTENSION_IDENTIFIER_PATTERN' -Option Constant -Scope 'Script' -Value '^([a-z][a-z\d]*(?:-[a-z][a-z\d]*)*)\.([a-z][a-z\d]*(?:-[a-z][a-z\d]*)*)$' `
        -Description 'Regular expression string that matches a VSIX extension identifier. Group 1 matches the publisher name token, and group 2 matches the extension name token.';
    # https://github.com/microsoft/vscode/blob/1.95.0/src/vs/platform/extensionManagement/common/extensionManagement.ts#L17
    New-Variable -Name 'EXTENSION_IDENTIFIER_REGEX' -Option ReadOnly -Scope 'Script' `
        -Value ([regex]::new($Script:EXTENSION_IDENTIFIER_PATTERN,
            ([System.Text.RegularExpressions.RegexOptions]([System.Text.RegularExpressions.RegexOptions]::Compiled -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)))) `
        -Description 'Matches a VSIX extension identifier. Group 1 matches the publisher ID token, and group 2 matches the extension ID token.';
    New-Variable -Name 'VSIX_BASENAME_PARSE_REGEX' -Option ReadOnly -Scope 'Script' `
        -Value ([regex]::new('^([a-z][a-z\d]*(?:-[a-z][a-z\d]*)*).([a-z][a-z\d]*(?:-[a-z][a-z\d]*)*)-(\d+(?:\.\d+)*)(?:@(.+)?)?$',
            ([System.Text.RegularExpressions.RegexOptions]([System.Text.RegularExpressions.RegexOptions]::Compiled -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)))) `
        -Description 'Parses tokens from the BaseName of a VSIX package file (not for complete validation). Group 1 matches the publisher name, Group 2 matches the extension name, Group 3 matches the version, and Group 4 matches the optional target platform.';
   New-Variable -Name 'VS_GALLERY_VERSION_EXP' -Option ReadOnly -Scope 'Script' -Value ([regex]::new('^(\^|>=)?(?:(\d+)|x)(?:\.(?:(\d+)|x)(?:\.(?:(\d+)|x))?)?(?:-(.+))?$', [System.Text.RegularExpressions.RegexOptions]::Compiled));
    New-Variable -Name 'VS_GALLERY_NOT_BEFORE_EXP' -Option ReadOnly -Scope 'Script' -Value ([regex]::new('^-(\d{4})(\d{2})(\d{2})$', [System.Text.RegularExpressions.RegexOptions]::Compiled));
    New-Variable -Name 'VS_GALLERY_INSTALL_SOURCE_EXP' -Option ReadOnly -Scope 'Script' -Value ([regex]::new('^(gallery|vsix|resource)$', [System.Text.RegularExpressions.RegexOptions]::Compiled));
    New-Variable -Name 'VS_GALLERY_EXTENSION_KIND_EXP' -Option ReadOnly -Scope 'Script' -Value ([regex]::new('^(ui|workspace|web)$', [System.Text.RegularExpressions.RegexOptions]::Compiled));
}

