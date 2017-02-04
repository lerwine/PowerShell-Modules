$Path = 'E:\Visual Studio 2015\Projects\PowerShell-Modules\master\PsNuGet\PsNuGet\Erwine.Leonard.T.PsNuGet.psd1';
$Script:InstallerDefaults = @{
	DefautModuleNamePrefix = 'Erwine.Leonard.T.';
	Author = 'Leonard T. Erwine';
	CompanyName = 'Leonard T. Erwine';
};

if ($PSVersionTable.PSVersion.Major -lt 3) { $Script:$PSScriptRoot = $MyInvocation.InvocationName}
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

	Process {
		$DirectoryPath = '';
		if ($ModuleInfo -ne $null) {
			if ($ModuleInfo.Path -eq $null -or $ModuleInfo.Path.Trim().Length -eq 0) {
				Write-Error -Message 'Module manifest does not specify a path.' -Category ObjectNotFound -TargetObject $ModuleInfo;
			} else {
				$DirectoryPath = $ModuleInfo.Path | Split-Path -Parent;
				if ($ModuleInfo.RootModule -eq $null -or $ModuleInfo.RootModule.Trim().Length -eq 0) {
					if ($ModuleInfo.Name -eq $null -or $ModuleInfo.Name.Trim().Length -eq 0) {
						$Name = [System.IO.Path]::GetFileNameWithoutExtension($ModuleInfo.Path);
					} else {
						$Name = $ModuleInfo.Name 
					}
				}
			}
		} else {
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
				$MaxDepth--;
				if ($MaxDepth -gt 0) {
					$Paths = Get-ChildItem -Path $Path -Directory | ForEach-Object { $_.FullName };
					if ($Paths -ne $null) {
						if ($Installable) {
							if ($PSBoundParameters.ContainsKey('ProgressId')) {
								if ($PSBoundParameters.ContainsKey('ProgressParentId')) {
									$Paths | Get-ModuleDirectory -MaxDepth $MaxDepth -ProgressActivity $ProgressActivity -ProgressId $ProgressId -ProgressParentId $ProgressParentId -Installable;
								} else {
									$Paths | Get-ModuleDirectory -MaxDepth $MaxDepth -ProgressActivity $ProgressActivity -ProgressId $ProgressId -Installable;
								}
							} else {
								if ($PSBoundParameters.ContainsKey('ProgressParentId')) {
									$Paths | Get-ModuleDirectory -MaxDepth $MaxDepth -ProgressActivity $ProgressActivity -ProgressParentId $ProgressParentId -Installable;
								} else {
									$Paths | Get-ModuleDirectory -MaxDepth $MaxDepth -ProgressActivity $ProgressActivity -Installable;
								}
							}
						} else {
							if ($PSBoundParameters.ContainsKey('ProgressId')) {
								if ($PSBoundParameters.ContainsKey('ProgressParentId')) {
									$Paths | Get-ModuleDirectory -MaxDepth $MaxDepth -ProgressActivity $ProgressActivity -ProgressId $ProgressId -ProgressParentId $ProgressParentId;
								} else {
									$Paths | Get-ModuleDirectory -MaxDepth $MaxDepth -ProgressActivity $ProgressActivity -ProgressId $ProgressId;
								}
							} else {
								if ($PSBoundParameters.ContainsKey('ProgressParentId')) {
									$Paths | Get-ModuleDirectory -MaxDepth $MaxDepth -ProgressActivity $ProgressActivity -ProgressParentId $ProgressParentId;
								} else {
									$Paths | Get-ModuleDirectory -MaxDepth $MaxDepth -ProgressActivity $ProgressActivity;
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
$ModuleDirectories = @(Get-ModuleDirectory -Path 'E:\Visual Studio 2015\Projects\PowerShell-Modules\master' -Installable -ProgressActivity 'Searching for module install folders.' -ProgressId 2 -ProgressParentId 1);
Write-Progress -Activity 'Searching for module install folders.' -Status 'Finished' -Id 2 -ParentId 1 -Completed;
('{0} installable modules found' -f $ModuleDirectories.Count) | Write-Host;

Write-Progress -Activity 'Initializing' -Status 'Finished' -Id 1 -Completed;

<#
$ParseErrors = $null;
$Tokens = [System.Management.Automation.PSParser]::Tokenize([System.IO.File]::ReadAllText('E:\Visual Studio 2015\Projects\PowerShell-Modules\master\PsNuGet\PsNuGet\Erwine.Leonard.T.PsNuGet.psd1'), [ref]$ParseErrors);
$Tokens | Out-GridView;
#>