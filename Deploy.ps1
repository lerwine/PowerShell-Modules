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
        if (Test-Path -LiteralPath $DistributionPath -PathType Container) {
            $ModuleManifestFileName = "$Script:ModuleName.psd1";
            if (
                (Test-Path -LiteralPath ($DistributionPath | Join-Path -ChildPath $ModuleManifestFileName)) -or `
                $null -eq ((Get-ChildItem -LiteralPath $DistributionPath -Filter '*.psd1' -Force) | Where-Object { $_.Name -ine $ModuleManifestFileName } | Select-Object -First 1)
            ) {
                Remove-Item $DistributionPath/* -Recurse -Force -ErrorAction Stop;
            } else {
                throw "Manifest file from one or more other modules found in $DistributionPath";
            }
        }
    }
}

Function Assert-TargetDirectory {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [string]$RelativePath,

        [Parameter(Mandatory = $true)]
        [string]$TargetContainerPath,

        [switch]$PassThru
    )

    Process {
        $TargetPath = $TargetContainerPath | Join-Path -ChildPath $RelativePath;
        if (Test-Path -LiteralPath $TargetPath) {
            if ($PassThru.IsPresent) {(Resolve-Path -LiteralPath $TargetPath).Path | Write-Output }
        } else {
            $Parent = $RelativePath | Split-Path -Parent;
            $Leaf = $RelativePath | Split-Path -Leaf;
            if ($Parent -eq '.') {
                $Parent = $TargetContainerPath;
            } else {
                $Parent = Assert-TargetDirectory -RelativePath $Parent -TargetContainerPath $TargetContainerPath -PassThru -ErrorAction Stop;
            }
            Write-Information -MessageData "Create: `"$($Parent | Join-Path -ChildPath $Leaf)`"";
            if ($PassThru.IsPresent) {
                (New-Item -Path $Parent -Name $Leaf -ItemType Directory -Force -ErrorAction Stop)?.FullName;
            } else {
                (New-Item -Path $Parent -Name $Leaf -ItemType Directory -Force -ErrorAction Stop) | Out-Null;
            }
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
        $TargetContainerPath = $DeploymentRoot | Join-Path -ChildPath $Script:ModuleName;
        if ($TargetContainerPath | Test-Path) {
            $TargetContainerPath = (Resolve-Path -LiteralPath $DeploymentRoot -ErrorAction Stop).Path;
            Clear-TargetPsModule -DeploymentRoot $DeploymentRoot -ErrorAction Stop;
        } else {
            $TargetContainerPath = (New-Item -Path $DeploymentRoot -Name $Script:ModuleName -ItemType Directory -ErrorAction Stop -Force).FullName;
        }
        $ManifestFileName = "$Script:ModuleName.psd1";
        $ModuleManifestPath = $Script:SourceModulePath | Join-Path -ChildPath $ManifestFileName;
        $ModuleManifestData = Import-PowerShellDataFile -LiteralPath $ModuleManifestPath -ErrorAction Stop;
        $AllFiles = @();
        $FileName = $ModuleManifestData['RootModule'];
        if ([string]::IsNullOrWhiteSpace($FileName)) {
            $FileName = "$Script:ModuleName.psm1";
            if (Test-Path -LiteralPath ($Script:SourceModulePath | Join-Path -ChildPath $FileName) -PathType Leaf) {
                $AllFiles = @([PSCustomObject]@{
                    Name = $FileName;
                    Setting = 'RootModule';
                });
            } else {
                $FileName = "$Script:ModuleName.dll";
                if (Test-Path -LiteralPath ($Script:SourceModulePath | Join-Path -ChildPath $FileName) -PathType Leaf) {
                    $AllFiles = @([PSCustomObject]@{
                        Name = $FileName;
                        Setting = 'RootModule';
                    });
                } else {
                    Write-Warning -Message "RootModule Setting in $ModuleManifestPath is empty, and neither $Script:ModuleName.psm1, nor $Script:ModuleName.dll was found.";
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
    
        $AllRelativePaths = @((@('.' | Join-Path -ChildPath $ManifestFileName) + ($AllFiles | ForEach-Object {
            $RP = $null;
            try { $RP = $_.Name | Resolve-Path -Relative -RelativeBasePath $Script:SourceModulePath -ErrorAction Stop }
            catch {
                Write-Warning -Message "$($_.Setting) setting in $ModuleManifestPath contains an file that could not be resolved: $($_.Name)";
            }
            if ($null -ne $RP) {
                if ($RP[0] -ne '.' -or $RP.Length -gt 1 -and $RP[1] -eq '.') {
                    Write-Warning -Message "$($_.Setting) setting in $ModuleManifestPath contains a path outside of the source module folder: $($_.Name)";
                } else {
                    $RP | Write-Output;
                }
            }
        })) | Select-Object -Unique);
        
        $FileName = '.' | Join-Path -ChildPath "about_$Script:ModuleName.help.txt";
        if (($AllRelativePaths -inotcontains $FileName) -and (Test-Path -LiteralPath ($Script:SourceModulePath | Join-Path -ChildPath $FileName) -PathType Leaf)) {
            $AllRelativePaths += @($FileName);
        }
        $FileName = '.' | Join-Path -ChildPath 'README.md';
        if (($AllRelativePaths -inotcontains $FileName) -and (Test-Path -LiteralPath ($Script:SourceModulePath | Join-Path -ChildPath $FileName) -PathType Leaf)) {
            $AllRelativePaths += @($FileName);
        }
        $FileName = '.' | Join-Path -ChildPath 'ReleaseNotes.md';
        if (($AllRelativePaths -inotcontains $FileName) -and (Test-Path -LiteralPath ($Script:SourceModulePath | Join-Path -ChildPath $FileName) -PathType Leaf)) {
            $AllRelativePaths += @($FileName);
        }
        @($AllRelativePaths | Where-Object { ($_ | Split-Path -Extension) -ieq '.dll' }) | ForEach-Object {
            ($_ | Split-Path -Parent) | Join-Path -ChildPath "$($_ | Split-Path -LeafBase).pdb";
        }
        @($AllRelativePaths | Where-Object { ($_ | Split-Path -Extension) -ieq '.dll' }) | ForEach-Object {
            $FileName = ($_ | Split-Path -Parent) | Join-Path -ChildPath "$($_ | Split-Path -LeafBase).pdb";
            if (($AllRelativePaths -inotcontains $FileName) -and (Test-Path -LiteralPath ($Script:SourceModulePath | Join-Path -ChildPath $FileName) -PathType Leaf)) {
                $AllRelativePaths += @($FileName);
            }
        };
        
        foreach ($RelativePath in $AllRelativePaths) {
            $Parent = $RelativePath | Split-Path -Parent;
            if ($Parent -ne '.') {
                Assert-TargetDirectory -RelativePath $Parent -TargetContainerPath $TargetContainerPath -ErrorAction Stop;
            }
            $SourcePath = $Script:SourceModulePath | Join-Path -ChildPath $RelativePath;
            $TargetPath = $TargetContainerPath | Join-Path -ChildPath $RelativePath;
            Write-Information -MessageData "Copy: `"$SourcePath`" => `"$TargetPath`"";
            Copy-Item -Path $SourcePath -Destination $TargetPath -Force -ErrorAction Stop;
        }
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

$Script:SourceModulePath = (Resolve-Path -LiteralPath ($ModuleManifestPath | Split-Path -Parent)).Path;
$Script:ModuleName = $ModuleManifestPath | Split-Path -LeafBase;

<#
$ModuleManifestData = Import-PowerShellDataFile -LiteralPath 'C:\Users\lerwine\source\repositories\PowerShell-Modules\dist\Erwine.Leonard.T.IOUtility\Erwine.Leonard.T.IOUtility.psd1' -ErrorAction Stop;
#>

if ($PublishLocalRepo.IsPresent) {
    Install-TargetPsModule -DeploymentRoot $DeploymentRoot -ErrorAction Stop;
    if ($Test.IsPresent) { Invoke-TestTask -DeploymentRoot $DeploymentRoot -ErrorAction Stop }
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

