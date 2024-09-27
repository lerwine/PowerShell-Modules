[CmdletBinding(DefaultParameterSetName = 'PublishLocal')]
Param(
    [Parameter(Mandatory = $true)]
    [ValidateScript({
        (Test-Path -LiteralPath $_ -PathType Leaf) -and (($_ | Split-Path -Extension) -ieq '.psd1')
    })]
    [string]$ModuleManifestPath,

    [Parameter(Mandatory = $true, ParameterSetName = 'Test')]
    [Parameter(Mandatory = $true, ParameterSetName = 'Clean')]
    [Parameter(Mandatory = $true, ParameterSetName = 'PublishLocal')]
    [ValidateScript({ Test-Path -LiteralPath $_ -PathType Container })]
    [string]$DeploymentRoot,

    [Parameter(ParameterSetName = 'Deploy')]
    [ValidateScript({ Test-Path -LiteralPath $_ -PathType Container })]
    [string[]]$DeployTo,
    
    [Parameter(ParameterSetName = 'UnDeploy')]
    [ValidateScript({ Test-Path -LiteralPath $_ -PathType Container })]
    [string[]]$UnDeployFrom,
    
    [Parameter(Mandatory = $true, ParameterSetName = 'Clean')]
    [switch]$CleanLocalRepo,
    
    [Parameter(ParameterSetName = 'Test')]
    [Parameter(Mandatory = $true, ParameterSetName = 'PublishLocal')]
    [switch]$PublishLocalRepo,
    
    [Parameter(Mandatory = $true, ParameterSetName = 'Test')]
    [switch]$Test,
    
    [Parameter(ParameterSetName = 'Deploy')]
    [switch]$DeployPSModulePath,

    [Parameter(ParameterSetName = 'UnDeploy')]
    [switch]$UnDeployPSModulePath
)

Function Read-ModulePath {
    [CmdletBinding()]
    Param()

    if ([string]::IsNullOrWhiteSpace($Env:PSModulePath)) {
        Write-Warning -Message 'PSModulePath environment variable is empty';
    } else {
        $PotentialTargetPaths = @(($Env:PSModulePath.Split(';') | Where-Object {
            if ($_ | Test-Path -PathType Container) { return $true }
            if ($_ | Test-Path -PathType Leaf) {
                Write-Warning -Message "Ignoring PSModulePath item that refers to a file: $_";
                return $false;
            }
            $Parent = $_ | Split-Path -Parent;
            if ([string]::IsNullOrEmpty($Parent)) {
                Write-Warning -Message "Ignoring PSModulePath item that does not exist: $_";
                return $false;
            }
            if (Test-Path -LiteralPath $Parent -PathType Container) { return $true }
            Write-Warning -Message "Ignoring PSModulePath item whose parent subdirectory does not exist: $_";
            return $false;
        }));
        if ($PotentialTargetPaths.Count -gt 0) {
            $i = 0;
            $PotentialTargetPaths | ForEach-Object {
                $i++;
                "$i : $_" | Write-Host;
            }
            $Response = Read-Host -Prompt 'Enter the number corresponding to the target PS Module root path or empty to cancel';
            if ([string]::IsNullOrWhiteSpace($Response)) { return }
            if ([int]::TryParse($Response.Trim(), [ref]$i) -and $i -gt 0 -and $i -le $PotentialTargetPaths.Count) {
                return $PotentialTargetPaths[$i - 1];
            }
            throw "Invalid item number";
        } else {
            Write-Warning -Message 'No valid PS Module paths found';
        }
    }
    $ModulePath = Read-Host -Prompt 'Enter the PS Module root folder path';
    if (-not ($ModulePath | Test-Path -IsValid)) { throw 'Invalid module path string' }
    if (-not (Test-Path -LiteralPath $ModulePath -PathType Container)) {
        if (Test-Path -LiteralPath $ModulePath -PathType Leaf) { throw "`'$ModulePath`" does not refer to a subdirectory." }
        throw "Subdirectory `'$ModulePath`" not found.";
    }
    if ($null -ne ((Get-ChildItem -LiteralPath $ModulePath -Filter '*.psd1' -Force) | Select-Object -First 1)) {
        throw "Cannot deploy: Subdirectory `'$ModulePath`" contains one or more module manifest files and may not be a PS Module root folder.";
    }
    return $ModulePath;
}

Function Clear-TargetPsModule {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory=$true, ValueFromPipeline = $true)]
        [string]$DeploymentRoot
    )
    
    Process {
        $DistributionPath = $DeploymentRoot | Join-Path -ChildPath $Script:ModuleName;
        if ($DistributionPath | Test-Path -PathType Container) {
            $ModuleManifestFileName = "$Script:ModuleName.psd1";
            if ($null -ne ((Get-ChildItem -LiteralPath $DistributionPath -Filter '*.psd1' -Force) | Where-Object { $_.Name -ine $ModuleManifestFileName } | Select-Object -First 1)) {
                throw "Manifest file from one or more other modules found in $DistributionPath";
            }
            Remove-Item $DistributionPath/* -Recurse -Force -ErrorAction Stop;
        }
    }
}

Function Install-TargetPsModule {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory=$true, ValueFromPipeline = $true)]
        [string]$DeploymentRoot
    )
    
    Process {
        $FullPath = $ModulePath;
        if ($ModulePath | Test-Path) {
            $FullPath = (Resolve-Path -LiteralPath $ModulePath -ErrorAction Stop).Path;
            Clear-TargetPsModule -ModulePath $FullPath -ErrorAction Stop;
        } else {
            $FullPath = (New-Item -Path $ModulePath -Name ($ModulePath | Split-Path -Leaf) -ItemType Directory -ErrorAction Stop -Force).FullName;
        }
        Copy-Item -Path $Script:SourceModulePath\* -Destination $FullPath -Recurse -Exclude $Exclude 'PSScriptAnalyzerSettings.psd', 'temp.*' -Force -ErrorAction Stop;
    }
}

Function Uninstall-TargetPsModule {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory=$true, ValueFromPipeline = $true)]
        [string]$DeploymentRoot
    )
    
    Process {
        $FullPath = (Resolve-Path -LiteralPath $ModulePath -ErrorAction Stop).Path;
        $ModuleManifestFileName = "$Script:ModuleName.psd1";
        if (Test-Path -LiteralPath ($FullPath | Join-Path -ChildPath $ModuleManifestFileName) -PathType Leaf) {
            Remove-Item $FullPath -Recurse -Force -ErrorAction Stop;
        } else {
            if ($null -ne ((Get-ChildItem -LiteralPath $FullPath -Filter '*.psd1' -Force) | Select-Object -First 1)) {
                throw "Manifest file from one or more other modules found in $FullPath";
            }
            if ($null -ne ((Get-ChildItem -LiteralPath $FullPath -Force) | Select-Object -First 1)) {
                throw "Module manifest file $ModuleManifestFileName not found in $FullPath";
            }
        }
    }
}

Function Invoke-TestTask {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [ValidateScript({ Test-Path -LiteralPath $_ -PathType Container })]
        [string]$DeploymentRoot
    )
    
    $DistributionPath = $DeploymentRoot | Join-Path -ChildPath $Script:ModuleName;
    if (-not (Test-Path -LiteralPath ($DistributionPath | Join-Path -ChildPath "$Script:ModuleName.psd1") -PathType Leaf)) {
        Install-TargetPsModule -ModulePath $DistributionPath -ErrorAction Stop;
    }
    if ($null -eq (Get-Module -Name 'Pester')) {
        Import-Module -Name Pester -ErrorAction Stop;
    }
    Invoke-Pester $DistributionPath;
}

Function Get-FilesToDeploy {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$ModuleManifestPath
    )

    $ModuleManifestData = Import-PowerShellDataFile -LiteralPath $ModuleManifestPath -ErrorAction Stop;
    $ModuleManifestRoot = ($ModuleManifestPath | Resolve-Path -ErrorAction Stop).Path | Split-Path -Parent;
    $AllFiles = @();
    $FileName = $ModuleManifestData['RootModule'];
    if ([string]::IsNullOrWhiteSpace($FileName)) {
        $FileName = "$Script:ModuleName.psm1";
        if (Test-Path -LiteralPath ($ModuleManifestRoot | Join-Path -ChildPath $FileName) -PathType Leaf) {
            $AllFiles = @([PSCustomObject]@{
                Name = $FileName;
                Setting = 'RootModule';
            });
        } else {
            $FileName = "$Script:ModuleName.dll";
            if (Test-Path -LiteralPath ($ModuleManifestRoot | Join-Path -ChildPath $FileName) -PathType Leaf) {
                $AllFiles = @([PSCustomObject]@{
                    Name = $FileName;
                    Setting = 'RootModule';
                });
            } else {
                Write-Warning -Message "RootModule Setting in $ModuleManifestPath is empty and not default module could be found.";
            }
        }
    } else {
        $AllFiles = @([PSCustomObject]@{
            Name = $FileName;
            Setting = 'RootModule';
        });
    }
    ('ScriptsToProcess', 'TypesToProcess', 'FormatsToProcess', 'NestedModules', 'ModuleList', 'FileList') | ForEach-Object {
        if ($ModuleManifestData.ContainsKey($_)) {
            foreach ($FileName in $ModuleManifestData[$_] | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }) {
                $AllFiles += @([PSCustomObject]@{
                    Name = $FileName;
                    Setting = $_;
                });
            }
        }
    }
    
    $AllRelativePaths = @(@('.' | Join-Path -ChildPath ($ModuleManifestPath | Split-Path -Leaf)) + ($AllFiles | ForEach-Object {
        if (Test-Path -LiteralPath $_.Name -IsValid) {
            $FullPath = $ModuleManifestRoot | Join-Path -ChildPath $FileName;
            if (Test-Path -LiteralPath $FullPath -PathType Leaf) {
                $RelativePath = $FullPath | Resolve-Path -Relative -RelativeBasePath $ModuleManifestRoot -ErrorAction Stop;
                $P = $RelativePath | Split-Path -Parent;
                while ($p -ne '.') {
                    if ([string]::IsNullOrEmpty($p) -or $p -eq '..') {
                        Write-Warning -Message "$($_.Setting) setting in $ModuleManifestPath contains a path outside of the source module folder: $($_.OriginalName)";
                        break;
                    }
                }
                if ($p -eq '.') { $RelativePath | Write-Output; }
            } else {
                Write-Warning -Message "$($_.Setting) setting in $ModuleManifestPath contains an file that doesn't exit: $($_.Name)";
            }
        } else {
            Write-Warning -Message "$($_.Setting) setting in $ModuleManifestPath contains an invalid file name: $($_.Name)";
        }
    })) | Select-Object -Unique;
    $FileName = '.' | Join-Path -ChildPath "about_$Script:ModuleName.help.txt";
    if (($AllRelativePaths -inotcontains $FileName) -and (Test-Path -LiteralPath ($ModuleManifestRoot | Join-Path -ChildPath $FileName) -PathType Leaf)) {
        $AllRelativePaths += @($FileName);
    }
    $FileName = '.' | Join-Path -ChildPath 'README.md';
    if (($AllRelativePaths -inotcontains $FileName) -and (Test-Path -LiteralPath ($ModuleManifestRoot | Join-Path -ChildPath $FileName) -PathType Leaf)) {
        $AllRelativePaths += @($FileName);
    }
    $FileName = '.' | Join-Path -ChildPath 'ReleaseNotes.md';
    if (($AllRelativePaths -inotcontains $FileName) -and (Test-Path -LiteralPath ($ModuleManifestRoot | Join-Path -ChildPath $FileName) -PathType Leaf)) {
        $AllRelativePaths += @($FileName);
    }
    $AllRelativePaths | Write-Output;
}

$Script:SourceModulePath = (Resolve-Path -LiteralPath ($ModuleManifestPath | Split-Path -Parent)).Path;
$Script:ModuleName = $ModuleManifestPath | Split-Path -LeafBase;


<#
$ModuleManifestData = Import-PowerShellDataFile -LiteralPath 'C:\Users\lerwine\source\repositories\PowerShell-Modules\dist\Erwine.Leonard.T.IOUtility\Erwine.Leonard.T.IOUtility.psd1' -ErrorAction Stop;
#>

if ($PublishLocalRepo.IsPresent) {
    Get-FilesToDeploy -ModuleManifestPath $ModuleManifestPath;
    # Install-TargetPsModule -ModulePath ($DeploymentRoot | Join-Path -ChildPath $Script:ModuleName) -ErrorAction Stop;
    # if ($Test.IsPresent) { Invoke-TestTask -DeploymentRoot $DeploymentRoot -ErrorAction Stop }
} else {
    if ($CleanLocalRepo.IsPresent) {
        Clear-TargetPsModule -DeploymentRoot $DeploymentRoot -ErrorAction Stop;
    } else {
        if ($Test.IsPresent) {
            Invoke-TestTask -DeploymentRoot $DeploymentRoot -ErrorAction Stop;
        } else {
            if ($PSBoundParameters.ContainsKey('DeployTo')) {
                if ($PSBoundParameters.ContainsKey('DeployPSModulePath')) {
                    ((Read-ModulePath) + $DeployTo) | Install-TargetPsModule -ErrorAction Stop;
                } else {
                    $DeployTo | Install-TargetPsModule -ErrorAction Stop;
                }
            } else {
                if ($PSBoundParameters.ContainsKey('DeployPSModulePath')) {
                    Read-ModulePath | Install-TargetPsModule -ErrorAction Stop;
                } else {
                    if ($PSBoundParameters.ContainsKey('UnDeployPSModulePath')) {
                        if ($PSBoundParameters.ContainsKey('UnDeployFrom')) {
                            ((Read-ModulePath) + $UnDeployFrom) | Uninstall-TargetPsModule -ErrorAction Stop;
                        } else {
                            $UnDeployFrom | Uninstall-TargetPsModule -ErrorAction Stop;
                        }
                    } else {
                        $UnDeployFrom | Uninstall-TargetPsModule -ErrorAction Stop;
                    }
                }
            }
        }
    }
}

