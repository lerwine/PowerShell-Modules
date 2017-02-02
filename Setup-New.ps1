cls
$Path = 'E:\Visual Studio 2015\Projects\PowerShell-Modules\master\PsNuGet\PsNuGet\Erwine.Leonard.T.PsNuGet.psd1';
$Script:InstallerDefaults = @{
    DefautModuleNamePrefix = 'Erwine.Leonard.T.';
    Author = 'Leonard T. Erwine';
    CompanyName = 'Leonard T. Erwine';
};

if ($PSVersionTable.PSVersion.Major -lt 3) { $Script:PSScriptRoot = $MyInvocation.InvocationName | Split-Path -Parent }

Add-Type -Path ($PSScriptRoot | Join-Path -ChildPath 'ModuleInstallInfo.cs') -ReferencedAssemblies ([System.Management.Automation.PSObject].Assembly.Location) -ErrorAction Stop;

Function Get-RootModule {
    [CmdletBinding(DefaultParameterSetName = 'Path')]
    [OutputType([string])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Path')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Name')]
        [string]$Path,

        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Name')]
        [string]$Name,

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'ModuleInfo')]
        [System.Management.Automation.PSModuleInfo]$ModuleInfo
    )
    
    Begin { Write-Debug -Message ('Begin Get-RootModule: ParameterSetName = {0}' -f $PSCmdlet.DefaultParameterSetName); }

    Process {
        
        $DirectoryPath = '';
        if ($ModuleInfo -ne $null) {
                
            Write-Verbose -Message ('Get-RootModule: ModuleInfo.Name = {0}' -f $ModuleInfo.Name);
            if ($ModuleInfo.Path -eq $null -or $ModuleInfo.Path.Trim().Length -eq 0) {
                Write-Error -Message 'Module manifest does not specify a path.' -Category ObjectNotFound -TargetObject $ModuleInfo;
            } else {
                Write-Debug -Message ('Get-RootModule: Getting parent path for "{0}"' -f $ModuleInfo.Path);
                $DirectoryPath = $ModuleInfo.Path | Split-Path -Parent;
                
                $RootModule = $ModuleInfo.RootModule;
                if ($ModuleInfo.RootModule -eq $null -or $ModuleInfo.RootModule.Trim().Length -eq 0) { $RootModule = $ModuleInfo.ModuleBase }
                if ($RootModule -eq $null -or $RootModule.Trim().Length -eq 0) {
                    Write-Verbose -Message ('Get-RootModule: Module "{0}" does not define a RootModule or ModuleBase' -f $ModuleInfo.Path);
                    if ($ModuleInfo.Name -eq $null -or $ModuleInfo.Name.Trim().Length -eq 0) {
                        $Name = [System.IO.Path]::GetFileNameWithoutExtension($ModuleInfo.Path);
                    } else {
                        $Name = $ModuleInfo.Name; 
                    }
                }
            }
        } else {
            if ($PSBoundParameters.ContainsKey('Name')) {
                Write-Debug -Message ('Process Get-RootModule: Path = {0}; Name = {1}' -f $Path, $Name);
            } else {
                Write-Debug -Message ('Process Get-RootModule: Path = {0}' -f $Path);
            }
            $DirectoryPath = $Path;
            if (Test-Path -Path $DirectoryPath -PathType Leaf) {
                $DirectoryPath = $DirectoryPath | Split-Path -Parent;
            } else {
                if (-not (Test-Path -Path $DirectoryPath -PathType Container)) {
                    $DirectoryPath = $DirectoryPath | Split-Path -Parent;
                    if (-not (Test-Path -Path $DirectoryPath -PathType Container)) { $DirectoryPath = $Path }
                }
            }
            if (-not $PSBoundParameters.ContainsKey('Name')) {
                $Name = [System.IO.Path]::GetFileNameWithoutExtension($Path);
                if ((-not ((Test-Path -Path ($DirectoryPath | Join-Path -ChildPath ($Name + '.psd1')) -PathType Leaf) -or $Name.StartsWith($Script:InstallerDefaults.DefautModuleNamePrefix))) -and (Test-Path -Path ($DirectoryPath | Join-Path -ChildPath ($Script:InstallerDefaults.DefautModuleNamePrefix + $Name + '.psd1')) -PathType Leaf)) {
                    $Name = $Script:InstallerDefaults.DefautModuleNamePrefix + $Name;
                }
            }
            if (Test-Path -Path ($DirectoryPath | Join-Path -ChildPath ($Name + '.psd1'))) {
                $LoadModuleError = $null;
                $ModuleInfo = Test-ModuleManifest -Path ($DirectoryPath | Join-Path -ChildPath ($Name + '.psd1')) -ErrorVariable 'LoadModuleError';
                if ($ModuleInfo -eq $null) {
                    if ($LoadModuleError -eq $null -or $LoadModuleError.Count -eq 0) {
                        Write-Error -Message 'Unable to load module information from source location.' -Category OpenError -TargetObject $Path;
                    }
                } else {
                    if ((-not $PSBoundParameters.ContainsKey('Name')) -and $ModuleInfo.Name -ne $null -and $ModuleInfo.Name.Trim().Length -gt 0) {
                        $Name = $ModuleInfo.Name;
                    }
                }
            }
        }
        if ($ModuleInfo -ne $null -and $ModuleInfo.RootModule -ne $null -and $ModuleInfo.RootModule.Trim().Length -gt 0) {
            ($DirectoryPath | Join-Path -ChildPath $ModuleInfo.RootModule) | Write-Output;
        } else {
            $RootModule = $null;
            foreach ($Ext in @('.psm1', '.dll', '.cdxml', '.xaml')) {
                if (-not ($PSBoundParameters.ContainsKey('Name') -or $Name.StartsWith($Script:InstallerDefaults.DefautModuleNamePrefix))) {
                    $RootModule = $DirectoryPath | Join-Path -ChildPath ($Script:InstallerDefaults.DefautModuleNamePrefix + $Name + $Ext);
                    if ($RootModule | Test-Path -PathType Leaf) { break }
                }

                $RootModule = $DirectoryPath | Join-Path -ChildPath ($Name + $Ext);
                if ($RootModule | Test-Path -PathType Leaf) {
                    break;
                } else {
                    $RootModule = $null;
                }
            }

            if ($RootModule -ne $null) {
                $RootModule | Write-Output;
            } else {
                if ($ModuleInfo -eq $null -or $ModuleInfo.Path -eq $null -or $ModuleInfo.Path.Trim().Length -eq 0) {
                    if ($PSBoundParameters.ContainsKey('ModuleInfo')) {
                        Write-Error -Message 'Unable to determine root module.' -Category ObjectNotFound -TargetObject $ModuleInfo;
                    } else {
                        Write-Error -Message 'Unable to determine root module.' -Category ObjectNotFound -TargetObject $Pathj;
                    }
                } else {
                    $ModuleInfo.Path | Write-Output;
                }
            }
        }
    }
    
    End { Write-Debug -Message ('End Get-RootModule: ParameterSetName = {0}' -f $PSCmdlet.DefaultParameterSetName); }
}

Function Open-ModuleManifest {
    [CmdletBinding()]
    [OutputType([System.Management.Automation.PSModuleInfo])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [string]$Path,

        [switch]$Create
    )

    Process {
        $ManifestPath = $Path;
        $Exists = $false;
        if (-not $Path.ToLower().EndsWith('.psd1')) {
            $DirectoryPath = $Path;
            if (Test-Path -Path $DirectoryPath -PathType Leaf) {
                $DirectoryPath = $DirectoryPath | Split-Path -Parent;
            } else {
                if (-not (Test-Path -Path $DirectoryPath -PathType Container)) {
                    $DirectoryPath = $DirectoryPath | Split-Path -Parent;
                    if (-not (Test-Path -Path $DirectoryPath -PathType Container)) { $DirectoryPath = $Path }
                }
            }
            $Name = [System.IO.Path]::GetFileNameWithoutExtension($Path);
            $ManifestPath = $null;
            if (-not $Name.StartsWith($Script:InstallerDefaults.DefautModuleNamePrefix)) {
                $ManifestPath = $DirectoryPath | Join-Path -ChildPath ($Script:InstallerDefaults.DefautModuleNamePrefix + $Name + '.psd1');
                $Exists = Test-Path -Path $ManifestPath -PathType Leaf;
            }
            if (-not $Exists) {
                $ManifestPath = $DirectoryPath | Join-Path -ChildPath ($Name + '.psd1');
            }
        }
        $ModuleManifest = $null;
        $ModuleLoadError = $null;
        if ($Exists -or (Test-Path -Path $ManifestPath -PathType Leaf)) {
            $ModuleManifest = Test-ModuleManifest -Path $ManifestPath -ErrorVariable 'ModuleLoadError';
            if ((Resolve-Path -Path $ModuleManifest.Path).Path -ine (Resolve-Path -Path $ManifestPath).Path) {
                Write-Error -Message "Module not loaded from source location." -Category OpenError -TargetObject $Path -ErrorVariable 'ModuleLoadError';
                $ModuleManifest = $null;
            }
        } else {
            if ($Create) {
                $RootModule = Get-RootModule -Path $Path -ErrorVariable 'ModuleLoadError';
                if ($RootModule -ne $null) {
                    if ($RootModule.ToLower().EndsWith('.psd1')) {
                        $ManifestPath = $RootModule;
                    } else {
                        $ManifestPath = ($RootModule | Split-Path -Parent) | Join-Path -ChildPath ([System.IO.Path]::GetFileNameWithoutExtension($RootModule) + '.psd1');
                    }
                    $RootModule = $RootModule | Split-Path -Leaf;
                    $NewModuleManifestArgs = @{
                        Path = $ManifestPath;
                        Author = $Script:InstallerDefaults.Author;
                        CompanyName = $Script:InstallerDefaults.CompanyName;
                        Copyright = ('(c) {0:yyyy} {1}. All rights reserved.' -f [System.DateTime]::Now, $Script:InstallerDefaults.CompanyName);
                        Guid = ([System.Guid]::NewGuid());
                        ModuleVersion = '1.0';
                        ErrorVariable = 'ModuleLoadError';
                        Description = '';
                        NestedModules = @();
                        TypesToProcess = @();
                        FormatsToProcess = @();
                        RequiredAssemblies = @();
                        FileList = @();
                    }
                    if ($PSVersionTable.PSVersion.Major -lt 3) {
                        $NewModuleManifestArgs['ModuleToProcess'] = $RootModule;
                    } else {
                        $NewModuleManifestArgs['RootModule'] = $RootModule;
                    }
                    New-ModuleManifest @NewModuleManifestArgs;
                    $ModuleManifest = Test-ModuleManifest -Path $ManifestPath -ErrorVariable 'ModuleLoadError';
                    if ((Resolve-Path -Path $ModuleManifest.Path).Path -ine (Resolve-Path -Path $ManifestPath).Path) {
                        Write-Error -Message "Module not loaded from source location." -Category OpenError -TargetObject $Path -ErrorVariable 'ModuleLoadError';
                        $ModuleManifest = $null;
                    }
                }
            } else {
                Write-Error -Message 'Module not loaded from source location.' -Category OpenError -TargetObject $Path -ErrorVariable 'ModuleLoadError';
            }
        }
        if ($ModuleManifest -eq $null) {
            if ($ModuleLoadError -eq $null -or $ModuleLoadError.Count -eq 0) { Write-Error -Message 'Module not loaded.' -Category OpenError -TargetObject $Path }
        } else {
            New-Object -TypeName 'Erwine.Leonard.T.Setup.ModuleInstallInfo' -ArgumentList $ModuleManifest, $false;
        }
    }
}

Function Test-HasModuleManifest {
    [CmdletBinding()]
    [OutputType([System.Management.Automation.PSModuleInfo])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [string]$Path,

        [switch]$Create
    )
    Process {
        $Exists = $null;
        $DirectoryPath = $Path;
        if (Test-Path -Path $Path -PathType Leaf) {
            if ($Path.ToLower().EndsWith('.psd1')) {
                $Exists = $true;
            } else {
                $DirectoryPath = $DirectoryPath | Split-Path -Parent;
            }
        } else {
            if (-not (Test-Path -Path $DirectoryPath -PathType Container)) {
                $DirectoryPath = $DirectoryPath | Split-Path -Parent;
                if (-not (Test-Path -Path $DirectoryPath -PathType Container)) { $Exists = $false }
            }
        }
        if ($Exists -eq $null) {
            $Name = [System.IO.Path]::GetFileNameWithoutExtension($Path);
            $Exists = $false;
            if (-not $Name.StartsWith($Script:InstallerDefaults.DefautModuleNamePrefix) -and (($DirectoryPath | Join-Path -ChildPath ($Script:InstallerDefaults.DefautModuleNamePrefix + $Name + '.psd1')) | Test-Path -PathType Leaf)) {
                $Exists = $true;
            } else {
                $Exists = ($DirectoryPath | Join-Path -ChildPath ($Name + '.psd1')) | Test-Path -PathType Leaf;
            }
        }

        $Exists | Write-Output;
    }
}

Function Test-IsModuleDirectory {
    [CmdletBinding()]
    [OutputType([bool])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [string]$Path,

        [switch]$Installable
    )

    Process {
        $Exists = $null;
        $DirectoryPath = $Path;
        if (Test-Path -Path $Path -PathType Leaf) {
            foreach ($Ext in @('.psd1', '.psm1', '.cdxml')) {
                if ($Path.ToLower().EndsWith($Ext)) {
                    $Exists = $true;
                    break;
                }
            }
            if ($Exists -eq $null -or $Installable) { $DirectoryPath = $DirectoryPath | Split-Path -Parent }
        } else {
            if (-not (Test-Path -Path $DirectoryPath -PathType Container)) {
                $DirectoryPath = $DirectoryPath | Split-Path -Parent;
                if (-not (Test-Path -Path $DirectoryPath -PathType Container)) { $Exists = $false }
            }
        }
        if ($Exists -eq $null) {
            $Name = [System.IO.Path]::GetFileNameWithoutExtension($Path);
            $Exists = $false;
            if ($Name.StartsWith($Script:InstallerDefaults.DefautModuleNamePrefix)) {
                foreach ($Ext in @('.psd1', '.psm1', '.cdxml')) {
                    if (($DirectoryPath | Join-Path -ChildPath ($Name + $Ext)) | Test-Path -PathType Leaf) {
                        $Exists = $true;
                        break;
                    }
                }
            } else {
                foreach ($Ext in @('.psd1', '.psm1', '.cdxml')) {
                    if ((($DirectoryPath | Join-Path -ChildPath ($Script:InstallerDefaults.DefautModuleNamePrefix + $Name + $Ext)) | Test-Path -PathType Leaf) -or (($DirectoryPath | Join-Path -ChildPath ($Name + $Ext)) | Test-Path -PathType Leaf)) {
                        $Exists = $true;
                        break;
                    }
                }
            }
        }

        if ($Exists -and $Installable -and -not (Test-HasModuleManifest -Path $Path)) {
            $Name = [System.IO.Path]::GetFileNameWithoutExtension($Path);
            $Exists = $false;
            $ProjFile = $null;
            if (-not $Name.StartsWith($Script:InstallerDefaults.DefautModuleNamePrefix)) {
                $ProjFile = $DirectoryPath | Join-Path -ChildPath ($Script:InstallerDefaults.DefautModuleNamePrefix + $Name + '.pssproj');
                if ($ProjFile | Test-Path -PathType Leaf) { $Exists = $true; }
            }
            if (-not $Exists) {
                $ProjFile = $DirectoryPath | Join-Path -ChildPath ($Name + '.pssproj');
                $Exists = $ProjFile | Test-Path -PathType Leaf;
            }
            if ($Exists) {
                $Xml
            }
        }

        $Exists | Write-Output;
    }
}

Function Get-ModuleDirectory {
    [CmdletBinding(DefaultParameterSetName = 'NoProgress')]
    [OutputType([string[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'NoProgress')]
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Progress')]
        [string]$Path,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Progress')]
        [string]$ProgressActivity,
        
        [Parameter(ParameterSetName = 'Progress')]
        [int]$ProgressId,
        
        [Parameter(ParameterSetName = 'Progress')]
        [int]$ProgressParentId,

        [int]$MaxDepth = 3,

        [switch]$Installable
    )

    Process {
        if (-not $PSBoundParameters.ContainsKey('ProgressActivity')) {
            $ProgressActivity = 'Searching for module install folders.';
        }
        if ($PSBoundParameters.ContainsKey('ProgressId')) {
            if ($PSBoundParameters.ContainsKey('ProgressParentId')) {
                Write-Progress -Activity $ProgressActivity -Status 'Scanning directory' -Id $ProgressId -ParentId $ProgressParentId -CurrentOperation $Path;
            } else {
                Write-Progress -Activity $ProgressActivity -Status 'Scanning directory' -Id $ProgressId -CurrentOperation $Path;
            }
        } else {
            if ($PSBoundParameters.ContainsKey('ProgressParentId')) {
                Write-Progress -Activity $ProgressActivity -Status 'Scanning directory' -ParentId $ProgressParentId -CurrentOperation $Path;
            } else {
                Write-Progress -Activity $ProgressActivity -Status 'Scanning directory' -CurrentOperation $Path;
            }
        }
        if (Test-Path -Path $Path -PathType Container) {
            $IsModuleDirectory = $false;
            if ($Installable) {
                $IsModuleDirectory = Test-IsModuleDirectory -Path $Path -Installable;
            } else {
                $IsModuleDirectory = Test-IsModuleDirectory -Path $Path;
            }
            if (Test-IsModuleDirectory -Path $Path) {
                $Path | Write-Output;
            } else {
                if ($MaxDepth -gt 1) {
                    $ChildItems = @();
                    if ($PSVersionTable.Version.Major -lt 3) {
                        $ChildItems = @(Get-ChildItem -Path $Path | Where-Object { $_.PSIsContainer });
                    } else {
                        $ChildItems = @(Get-ChildItem -Path $Path -Directory);
                    }
                    $Paths = $ChildItems | ForEach-Object { $_.FullName };
                    if ($Paths -ne $null) {
                        if ($Installable) {
                            if ($PSBoundParameters.ContainsKey('ProgressId')) {
                                if ($PSBoundParameters.ContainsKey('ProgressParentId')) {
                                    $Paths | Get-ModuleDirectory -MaxDepth ($MaxDepth - 1)-ProgressActivity $ProgressActivity -ProgressId $ProgressId -ProgressParentId $ProgressParentId -Installable;
                                } else {
                                    $Paths | Get-ModuleDirectory -MaxDepth ($MaxDepth - 1) -ProgressActivity $ProgressActivity -ProgressId $ProgressId -Installable;
                                }
                            } else {
                                if ($PSBoundParameters.ContainsKey('ProgressParentId')) {
                                    $Paths | Get-ModuleDirectory -MaxDepth ($MaxDepth - 1) -ProgressActivity $ProgressActivity -ProgressParentId $ProgressParentId -Installable;
                                } else {
                                    $Paths | Get-ModuleDirectory -MaxDepth ($MaxDepth - 1) -ProgressActivity $ProgressActivity -Installable;
                                }
                            }
                        } else {
                            if ($PSBoundParameters.ContainsKey('ProgressId')) {
                                if ($PSBoundParameters.ContainsKey('ProgressParentId')) {
                                    $Paths | Get-ModuleDirectory -MaxDepth ($MaxDepth - 1) -ProgressActivity $ProgressActivity -ProgressId $ProgressId -ProgressParentId $ProgressParentId;
                                } else {
                                    $Paths | Get-ModuleDirectory -MaxDepth ($MaxDepth - 1) -ProgressActivity $ProgressActivity -ProgressId $ProgressId;
                                }
                            } else {
                                if ($PSBoundParameters.ContainsKey('ProgressParentId')) {
                                    $Paths | Get-ModuleDirectory -MaxDepth ($MaxDepth - 1) -ProgressActivity $ProgressActivity -ProgressParentId $ProgressParentId;
                                } else {
                                    $Paths | Get-ModuleDirectory -MaxDepth ($MaxDepth - 1) -ProgressActivity $ProgressActivity;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    End {
        if (-not $PSBoundParameters.ContainsKey('ProgressActivity')) {
            if ($PSBoundParameters.ContainsKey('ProgressId')) {
                if ($PSBoundParameters.ContainsKey('ProgressParentId')) {
                    Write-Progress -Activity $ProgressActivity -Status 'Finished' -Id $ProgressId -ParentId $ProgressParentId -Completed;
                } else {
                    Write-Progress -Activity $ProgressActivity -Status 'Finished' -Id $ProgressId -Completed;
                }
            } else {
                if ($PSBoundParameters.ContainsKey('ProgressParentId')) {
                    Write-Progress -Activity $ProgressActivity -Status 'Finished' -ParentId $ProgressParentId -Completed;
                } else {
                    Write-Progress -Activity $ProgressActivity -Status 'Finished' -Completed;
                }
            }
        }
    }
}

Function Open-PSProject {
    [CmdletBinding()]
    [OutputType([System.Xml.XmlDocument])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [string]$Path,

        [switch]$Create
    )

    Process {
        $XmlPath = $Path;
        $Exists = $false;
        if (-not $Path.ToLower().EndsWith('.psproj')) {
            $DirectoryPath = $Path;
            if (Test-Path -Path $DirectoryPath -PathType Leaf) {
                $DirectoryPath = $DirectoryPath | Split-Path -Parent;
            } else {
                $Exists = Test-Path -Path $DirectoryPath -PathType Container;
                if (-not (Test-Path -Path $DirectoryPath -PathType Container)) {
                    $DirectoryPath = $DirectoryPath | Split-Path -Parent;
                    if (-not (Test-Path -Path $DirectoryPath -PathType Container)) { $DirectoryPath = $Path }
                }
            }
            $Name = [System.IO.Path]::GetFileNameWithoutExtension($Path);
            $ManifestPath = $null;
            if (-not $Name.StartsWith($Script:InstallerDefaults.DefautModuleNamePrefix)) {
                $ManifestPath = $DirectoryPath | Join-Path -ChildPath ($Script:InstallerDefaults.DefautModuleNamePrefix + $Name + '.psd1');
                $Exists = Test-Path -Path $ManifestPath -PathType Leaf;
            }
            if (-not $Exists) {
                $ManifestPath = $DirectoryPath | Join-Path -ChildPath ($Name + '.psd1');
            }
        }
        $ModuleManifest = $null;
        $ModuleLoadError = $null;
        if ($Exists -or (Test-Path -Path $ManifestPath -PathType Leaf)) {
            $ModuleManifest = Test-ModuleManifest -Path $ManifestPath -ErrorVariable 'ModuleLoadError';
            if ((Resolve-Path -Path $ModuleManifest.Path).Path -ine (Resolve-Path -Path $ManifestPath).Path) {
                Write-Error -Message "Module not loaded from source location." -Category OpenError -TargetObject $Path -ErrorVariable 'ModuleLoadError';
                $ModuleManifest = $null;
            }
        } else {
            if ($Create) {
                $RootModule = Get-RootModule -Path $Path -ErrorVariable 'ModuleLoadError';
                if ($RootModule -ne $null) {
                    if ($RootModule.ToLower().EndsWith('.psd1')) {
                        $ManifestPath = $RootModule;
                    } else {
                        $ManifestPath = ($RootModule | Split-Path -Parent) | Join-Path -ChildPath ([System.IO.Path]::GetFileNameWithoutExtension($RootModule) + '.psd1');
                    }
                    $RootModule = $RootModule | Split-Path -Leaf;
                    $ModuleManifest = New-ModuleManifest -Path $ManifestPath -RootModule $RootModule -Author $Script:InstallerDefaults.Author -CompanyName $Script:InstallerDefaults.CompanyName -Copyright ('(c) {0:yyyy} {1}. All rights reserved.' -f [System.DateTime]::Now, $Script:InstallerDefaults.CompanyName) -Guid ([System.Guid]::NewGuid()) -ModuleVersion '1.0' -ErrorVariable 'ModuleLoadError' -PassThru;
                }
            } else {
                Write-Error -Message 'Module not loaded from source location.' -Category OpenError -TargetObject $Path -ErrorVariable 'ModuleLoadError';
            }
        }
        if ($ModuleManifest -eq $null) {
            if ($ModuleLoadError -eq $null -or $ModuleLoadError.Count -eq 0) { Write-Error -Message 'Module not loaded.' -Category OpenError -TargetObject $Path }
        } else {
            $ModuleManifest | Write-Output;
        }
    }
}

Write-Progress -Activity 'Initializing' -Status 'Looking for installable modules' -Id 1;
$ModuleDirectories = @(Get-ModuleDirectory -Path $PSScriptRoot -Installable -ProgressActivity 'Searching for module install folders.' -ProgressId 2 -ProgressParentId 1);
Write-Progress -Activity 'Searching for module install folders.' -Status 'Finished' -Id 2 -ParentId 1 -Completed;
$FaultCount = 0;
$ModuleList = @($ModuleDirectories | ForEach-Object {
    $ModuleManifest = $null;
    Write-Progress -Activity 'Verifying module manifests.' -Status 'Opening' -Id 2 -ParentId 1 -CurrentOperation $_;
    $ModuleManifest = Open-ModuleManifest -Path $_ -ErrorAction SilentlyContinue;
    if ($ModuleManifest -eq $null) {
        Write-Progress -Activity 'Verifying module manifests.' -Status 'Creating' -Id 2 -ParentId 1 -CurrentOperation $_;
        $ModuleManifest = Open-ModuleManifest -Path $_ -Create -ErrorAction Continue;
    }
    if ($ModuleManifest -eq $null) { $FaultCount++ } else { $ModuleManifest | Write-Output }
});
('{0} installable modules found' -f $ModuleList.Count) | Write-Host;
Write-Progress -Activity 'Initializing' -Status 'Finished' -Id 1 -Completed;
$ModuleList | ForEach-Object { '{0} {1}' -f $_.DisplayName, $_.ModuleInfo.ModuleType }
<#
$ParseErrors = $null;
$Tokens = [System.Management.Automation.PSParser]::Tokenize([System.IO.File]::ReadAllText('E:\Visual Studio 2015\Projects\PowerShell-Modules\master\PsNuGet\PsNuGet\Erwine.Leonard.T.PsNuGet.psd1'), [ref]$ParseErrors);
$Tokens | Out-GridView;
#>