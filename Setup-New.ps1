cls
$Path = 'E:\Visual Studio 2015\Projects\PowerShell-Modules\master\PsNuGet\PsNuGet\Erwine.Leonard.T.PsNuGet.psd1';
$Script:InstallerDefaults = @{
    DefautModuleNamePrefix = 'Erwine.Leonard.T.';
    Author = 'Leonard T. Erwine';
    CompanyName = 'Leonard T. Erwine';
};

if ($PSVersionTable.PSVersion.Major -lt 3) { $Script:PSScriptRoot = $MyInvocation.InvocationName | Split-Path -Parent }

Function Install-AssemblyModule {
    <#
        .SYNOPSIS
            Compiles and installs assembly
    #>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [ValidateScript({ $_ | Test-Path -PathType Leaf })]
        # Source code files to compile.
        [string]$Compile,

        [Parameter(Mandatory = $true)]
        [ValidateScript({ ($_ | Split-Path -Parent | Test-Path -PathType Container) -and $_.ToLower().EndsWith('.dll') })]
        # Output path of assembly.
        [string]$OutputAssembly,
        
        [ValidatePattern('^[a-zA-Z_][a-zA-Z_\d]*$')]
        # Preprocessor symbols to define.
        [string[]]$Define,
        
        [ValidateScript({ ($_ | Test-Path -PathType Leaf) -and $_.ToLower().EndsWith('.snk') })]
        # Filename containing the cryptographic key for signing.
        [string]$KeyFile,
        
        [ValidateScript({ $_ | Test-Path -PathType Leaf })]
        # Assemblies referenced by the source code.
        [string[]]$ReferencedAssemblies,
        
        [ValidateScript({ $_ | Test-Path -PathType Leaf })]
        # .NET Framework resource files to include when compiling the assembly output.
        [string[]]$EmbeddedResources,

        # Create XML file from processed source code documentation comments.
        [switch]$XmlDoc,
		
        # Include debug information in the compiled executable.
        [switch]$IncludeDebugInformation,

        # Include System.dll, System.Management.Automation.dll and, if applicable, System.Core.dll.
        [switch]$IncludeSystemAssemblies,

        # Define TRACE preprocessor symbol.
        [switch]$DefineTrace
    )
    
    Begin { $SourceFiles = @() }
    Process { $SourceFiles += $Compile }
    End {
        $Local:TempPath = ($OutputAssembly | Split-Path -Parent) | Join-Path -ChildPath 'bin';
        if (-not ($Local:TempPath | Test-Path -PathType Container)) { New-Item -Path $Local:TempPath -ItemType Directory | Out-Null }
        $Local:Splat = @{
            TypeName = 'System.CodeDom.Compiler.CompilerParameters';
            Property = @{
                IncludeDebugInformation = $IncludeDebugInformation.IsPresent;
                TempFiles = New-Object -TypeName 'System.CodeDom.Compiler.TempFileCollection' -ArgumentList $Local:TempPath, $true;
                CompilerOptions = @{
                    define = @();
                    target = 'library';
                }
                OutputAssembly = $OutputAssembly
            }
        };
        try {
            if ($PSBoundParameters.ContainsKey('ReferencedAssemblies')) { $Local:Splat['ArgumentList'] = @(,$ReferencedAssemblies); }
			if ($IncludeSystemAssemblies) {
				$Local:AssemblyPaths = @(((Add-Type -AssemblyName 'System.dll' -PassThru)[0], [System.Management.Automation.PSObject]) | ForEach-Object { $_.Assembly.Location });
				if ($PSVersionTable.PSVersion.Major -gt 3) { $Local:AssemblyPaths += (Add-Type -AssemblyName 'System.Core.Dll' -PassThru)[0].Assembly.Location }
				if ($Local:Splat['ArgumentList'] -eq $null) {
					$Local:Splat['ArgumentList'] = @(,$Local:AssemblyPaths);
				}
				$Local:AssemblyPaths | ForEach-Object {
					if ($Local:Splat['ArgumentList'][0] -inotcontains $_) {
						$Local:Splat['ArgumentList'][0] += $_;
					}
				}
			}
            if ($PSBoundParameters.ContainsKey('Define')) {
                $Local:Splat['Property']['CompilerOptions']['define'] = $Define;
            }
            $Local:VersionDefine = 'PSV' + $PSVersionTable.PSVersion.Major.ToString();
            if ($PSVersionTable.PSVersion.Major -gt 5) { $Local:VersionDefine = 'PSV5' }

            if ($Local:Splat['Property']['CompilerOptions']['define'] -cnotcontains $Local:VersionDefine) {
                $Local:Splat['Property']['CompilerOptions']['define'] += $Local:VersionDefine
            }

            if ($IncludeDebugInformation) {
                $Local:Splat['Property']['CompilerOptions']['pdb'] = '"' + (($OutputAssembly | Split-Path -Parent) | Join-Path -ChildPath ([System.IO.Path]::GetFileNameWithoutExtension($OutputAssembly) + '.pdb')) + '"';
                $Local:Splat['Property']['CompilerOptions']['debug'] = $null;
                if ($Local:Splat['Property']['CompilerOptions']['define'] -cnotcontains 'DEBUG') {
                    $Local:Splat['Property']['CompilerOptions']['define'] += 'DEBUG'
                }
            }
            if ($DefineTrace -and $Local:Splat['Property']['CompilerOptions']['define'] -cnotcontains 'TRACE') {
                $Local:Splat['Property']['CompilerOptions']['define'] += 'TRACE'
            }
            if ($XmlDoc) { $Local:Splat['Property']['CompilerOptions']['doc'] = '"' + (($OutputAssembly | Split-Path -Parent) | Join-Path -ChildPath ([System.IO.Path]::GetFileNameWithoutExtension($OutputAssembly) + '.xml')) + '"' }
            if ($Local:Splat['Property']['CompilerOptions']['define'].Count -eq 0) {
                $Local:Splat['Property']['CompilerOptions'].Remove('define');
            } else {
                $Local:Splat['Property']['CompilerOptions']['define'] = $Local:Splat['Property']['CompilerOptions']['define'] -join ';';
            }
            $Local:Splat['Property']['CompilerOptions'] = ($Local:Splat['Property']['CompilerOptions'].Keys | ForEach-Object {
                if ($Local:Splat['Property']['CompilerOptions'][$_] -eq $null) {
                    '/' + $_;
                } else {
                    '/' + $_ + ':' + $Local:Splat['Property']['CompilerOptions'][$_];
                }
            }) -join ' ';
            $CompilerParameters = New-Object @Local:Splat;
            if ($PSBoundParameters.ContainsKey('EmbeddedResources')) {
                $EmbeddedResources | ForEach-Object { $CompilerParameters.EmbeddedResources.Add($_) }
            }
            $OutputLocation = @(Add-Type -Path $SourceFiles -CompilerParameters $CompilerParameters -PassThru)[0].Assembly.Location;
            if ($OutputAssembly -ne $OutputLocation) {
                Copy-Item -Path $OutputLocation -Destination $OutputAssembly -Force | Out-Null;
            }
        } finally { $Local:Splat['Property']['TempFiles'].Dispose() }
    }
}

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
            New-Object -TypeName 'PSModuleInstallUtil.ModuleManifest' -ArgumentList $ModuleManifest;
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
        [ValidateScript({ $_.EndsWith('.psroj')})]
        [string]$Path,

        [switch]$Create
    )

    Process {
        $XmlDocument = New-Object -TypeName 'System.Xml.XmlDocument';
        if (Test-Path -Path $Path -PathType Leaf) {
            $XmlDocument.Load($Path);
        } else {
            [Xml]$XmlDocument = @'
<Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <SchemaVersion>2.0</SchemaVersion>
    <ProjectGuid><!-- TODO: Need project Guid --></ProjectGuid>
    <OutputType>Exe</OutputType>
    <RootNamespace>MyApplication</RootNamespace>
    <AssemblyName>MyApplication</AssemblyName>
    <Name><!-- TODO: Need module Name --></Name>
    <Author><!-- TODO: Need module Author --></Author>
    <CompanyName><!-- TODO: Need module CompanyName --></CompanyName>
    <Copyright><!-- TODO: Need module Copyright --></Copyright>
    <Description><!-- TODO: Need module Description --></Description>
    <Guid><!-- TODO: Need module Guid --></Guid>
    <Version><!-- TODO: Need module Version --></Version>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE<!-- TODO: Need other constants here --></DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="Erwine.Leonard.T.WPF.psm1" />
    <Compile Include="Erwine.Leonard.T.WPF.psd1" />
  </ItemGroup>
  <Import Project="$(MSBuildBinPath)\Microsoft.CSharp.targets" />
  <Target Name="Build" />
</Project>
'@;
        }
        $XmlDocument | Write-Output;
    }
}

$SetupDllPath = $PSScriptRoot | Join-Path -ChildPath 'Setup.dll';

Write-Progress -Activity 'Initializing' -Status 'Loading setup modules' -Id 1;
if (Test-Path -Path $SetupDllPath -PathType Leaf) {
    Add-Type -AssemblyName $SetupDllPath -ErrorAction Stop;
} else {
	$ReferencedAssemblies = @(('System.Windows.Forms.dll', 'System.Drawing.dll', 'System.Xml.dll') | ForEach-Object { (Add-Type -AssemblyName $_ -PassThru)[0].Assembly.Location });
	$SourcePath = $PSScriptRoot | Join-Path -ChildPath 'PSModuleInstallUtil';
	$EmbeddedResources = @(('MainForm.resx', 'NotificationForm.resx') | ForEach-Object { $SourcePath | Join-Path -ChildPath $_ });
	(('ModuleManifest.cs', 'MainForm.cs', 'MainForm.Designer.cs', 'NotificationForm.cs', 'NotificationForm.Designer.cs') | ForEach-Object { $SourcePath | Join-Path -ChildPath $_ }) | Install-AssemblyModule -OutputAssembly $SetupDllPath -ReferencedAssemblies $ReferencedAssemblies -IncludeSystemAssemblies -IncludeDebugInformation -DefineTrace -ErrorAction Stop;
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