Param(
	[Parameter(Mandatory = $true, Position = 0)]
	[ValidatePattern('^([^.]+\.)+psd1$')]
	[string]$ModuleManifest
)

if ($PSScriptRoot -eq $null) { $PSScriptRoot = Get-Location }
Add-Type -Path @(
    '.\_setup\InstallationLocationInfo.cs',
    '.\_setup\InstallLocationSelectForm.cs',
    '.\_setup\InstallLocationSelectForm.Designer.cs',
	'.\_setup\NotificationForm.cs',
	'.\_setup\NotificationForm.Designer.cs'
) -ReferencedAssemblies 'System', 'System.Drawing', 'System.Windows.Forms';

$Script:AllFiles = @{};

Function Get-ModulePaths {
	Param()

	if ($Env:PSModulePath -eq $null) {
		Write-Warning -Message 'The PSModulePath environment variable is not defined.';
		$Path = '';
	} else {
		if ($Env:PSModulePath.Trim().Length -eq 0) {
			Write-Warning -Message 'The PSModulePath environment variable is empty.';
		}
		$Path = $Env:PSModulePath;
	}
	if ($Path -eq '') {
		[System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::Personal) | Join-Path -ChildPath 'WindowsPowerShell\Modules';
		[System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::ProgramFiles) | Join-Path -ChildPath 'WindowsPowerShell\Modules';
		$Path = ((Get-Command 'powershell').Source | Split-Path -Parent) | Join-Path -ChildPath 'Modules';
		if ($Path | Test-Path -PathType Container) {
			$Path;
		} else {
			$p = [System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::System) | Join-Path -ChildPath 'WindowsPowerShell\v1.0\Modules';
			if ($p -eq $Path -or ($p | Test-Path -PathType Container)) {
				$p;
			} else {
				$Path;
			}
		}
	} else {
		$Path.Split([System.IO.Path]::PathSeparator);
	}
}

Function Get-RootModuleName {
	[CmdletBinding()]
	[OutputType([string])]
	Param()
	[System.IO.Path]::GetFileNameWithoutExtension($ModuleManifest)
}

Function Get-RootInstallSourcePath {
	[CmdletBinding()]
	[OutputType([string])]
	Param()
	$ModuleManifest | Split-Path -Parent;
}

Function Make-RelativePath {
	[CmdletBinding()]
	[OutputType([System.Management.Automation.PSObject])]
	Param(
		[Parameter(Mandatory = $true, ValueFromPipeline = $true)]
		[string]$Path
	)

	Process {
		if ($Path | Test-Path) {
			$Path | Resolve-Path -Relative;
		} else {
			$Parent = $Path | Split-Path -Parent;
			if ($Parent -eq $null) {
				[System.IO.Path]::GetFullPath($Parent);
			} else {
				Make-RelativePath -Path $Parent | Join-Path -ChildPath ($Path | Split-Path -Leaf);
			}
		}
	}

}

Function Get-InstallableStatus {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, ValueFromPipeline = $true)]
		[string]$Path,

		[Parameter(Mandatory = $true)]
		[bool]$ExpectDirectory
	)

	Process {
		$RelativePath = $Path | Make-RelativePath;
		if (-not $Script:AllFiles.ContainsKey($RelativePath)) {
			$Properties = @{
				Name = $Path | Split-Path -Leaf;
				ParentDirectory = [System.IO.Path]::GetFullPath($Path) | Split-Path -Parent;
				RelativePath = $RelativePath;
				ExpectDirectory = $ExpectDirectory
			};
			if ($Path | Test-Path -PathType Container) {
				if ($ExpectDirectory) {
					$Properties.Exists = $true;
					$Properties.Installable = $true;
				} else {
					$Properties.Exists = $false;
					$Properties.Installable = $false;
					$Properties.Reason = 'File expected, but subdirectory found.';
				}
			} else {
				if ($Path | Test-Path) {
					if ($ExpectDirectory) {
						$Properties.Exists = $false;
						$Properties.Installable = $false;
						$Properties.Reason = 'Subdirectory expected, but file found.';
					} else {
						$Properties.Exists = $true;
						$Properties.Installable = $true;
					}
				} else {
					$Properties.Exists = $false;
					$ExistingParent = $Path;
					do {
						$p = $ExistingParent | Split-Path -Parent;
						if ($p -eq $null) { break }
						$ExistingParent = $p;
					} while (-not ($ExistingParent | Test-Path));
					if ($ExistingParent | Test-Path -PathType Leaf) {
						$Properties.Installable = $false;
						$Properties.Reason = 'Subdirectory expected, but file found in path parent.';
					} else {
						$Properties.Installable = $true;
					}
				}
			}

			$Script:AllFiles[$RelativePath] = New-Object -TypeName 'PSModuleInstallUtil.InstallationLocationInfo' -Property $Properties;
		}

		$Script:AllFiles[$RelativePath] | Write-Output;
	}
}

Function Get-ModuleManifest {
	[CmdletBinding()]
	[OutputType([System.Management.Automation.PSObject])]
	Param(
		[Parameter(Mandatory = $true, ValueFromPipeline = $true)]
		[ValidatePattern('^([^.]+\.)+psd1$')]
		[string]$Path
	)

	Begin {
		$ModuleCodeRegex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '^\s*@?(?<m>\{.+$)', ([System.Text.RegularExpressions.RegexOptions]::Multiline);
		$ModuleFileRegex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '^([^.]+\.)+psd1$';
	}
	
	Process {
		$Properties = @{
			Name = [System.IO.Path]::GetFileNameWithoutExtension($Path);
			ManifestFile = $Path | Get-InstallableStatus;
			ModuleFiles = @{};
			NestedModules = @{};
		};

		$Properties.ModuleFiles[$Properties.ManifestFile.RelativePath] = $Properties.ManifestFile;
		
		if ($Properties.ManifestFile.Exists) {
			Set-Location -Path $Properties.RootPath;
			$text = [System.IO.File]::ReadAllText($Path);
			if ($text -ne $null) {
				$m = $ModuleCodeRegex.Match($text);
				if ($m.Success) {
					$Hashtable = [System.Management.Automation.ScriptBlock]::Create('@' + $m.Groups['m'].Value).Invoke(@());
					if ($Hashtable -ne $null) {
						$Properties.ModuleManifest = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Hashtable;
						if ($Properties.ModuleManifest.ScriptsToProcess -ne $null) {
							$FileArray = @($Properties.ModuleManifest.ScriptsToProcess);
						} else {
							$FileArray = @();
						}
						if ($Properties.ModuleManifest.TypesToProcess -ne $null) { $FileArray = $FileArray + $Properties.ModuleManifest.TypesToProcess }
						if ($Properties.ModuleManifest.FormatsToProcess -ne $null) { $FileArray = $FileArray + $Properties.ModuleManifest.FormatsToProcess }
						if ($Properties.ModuleManifest.FileList -ne $null) { $FileArray = $FileArray + $Properties.ModuleManifest.FileList }
						foreach ($file in $FileArray) {
							$InstallableStatus = $file | Get-InstallableStatus;
							if (-not $Properties.ModuleFiles.ContainsKey($InstallableStatus.RelativePath)) {
								$Properties.ModuleFiles[$InstallableStatus.RelativePath] = $InstallableStatus;
							}
						}
						if ($Properties.ModuleManifest.NestedModules -ne $null) {
							$Modules = @($Properties.ModuleManifest.NestedModules);
						} else {
							$Modules = @();
						}
						if ($Properties.ModuleManifest.ModuleList -ne $null) { $Modules = $Modules + @($Properties.ModuleManifest.ModuleList) }
						foreach ($p in $Modules) {
							if (($p | Test-Path -PathType Container) -or -not $ModuleFileRegex.IsMatch($p)) {
								$m = $p | Join-Path -ChildPath (($p | Split-Path -Leaf) + '.psd1');
							} else {
								$m = $p;
							}
							$r = $p | Make-RelativePath;
							if (-not $Properties.NestedModules.ContainsKey($r)) {
								$Properties.NestedModules[$r] = Get-ModuleManifest -Path $r;
							}
						}
					}
				}
			}
		}
		
		New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties;
	}
}

$RootName = Get-RootModuleName;
$ModulePaths = Get-ModulePaths | ForEach-Object { ($_ | Join-Path -ChildPath $RootName) | Join-Path -ChildPath ($RootName + '.psd1') } | Get-ModuleManifest
$InstallLocationSelectForm = New-Object -TypeName 'PSModuleInstallUtil.InstallLocationSelectForm' -Property @{ Text = 'Manage Installations' }
$InstallLocationSelectForm.

$installedAt = @($Targets | Where-Object { $_.Installable -and -not $_.Exists });
if ($installedAt.Count -gt 0) {
    $Message = (@(
        'This Module has been found at the following locations:',
        ($installedAt | ForEach-Object { "`t{0}" -f $_ } | Out-String),
        '',
		'Do you wish to overwrite?'
    ) | Out-String).Trim();
    $Choices = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
    $Choices.Add((New-Object -TypeName:'System.Management.Automation.Host.ChoiceDescription' -ArgumentList:"Yes"));
    $Choices.Add((New-Object -TypeName:'System.Management.Automation.Host.ChoiceDescription' -ArgumentList:"No"));
    $result = $Host.UI.PromptForChoice("Confirm Overwrite", $Message, $Choices, 1);
	if ($result -eq $null -or $result -ne 0) {
		'Aborted.' | Write-Warning;
		return;
	}
}

$i = 0;
$Choices = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
$env:PSModulePath.Split([System.IO.Path]::PathSeparator) | ForEach-Object {
    $i = $i + 1;
    $Choices.Add((New-Object -TypeName:'System.Management.Automation.Host.ChoiceDescription' -ArgumentList:($i, $_)));
}

$result = $Host.UI.PromptForChoice("Select Module Location", (@($Choices | ForEach-Object { '{0}: {1}' -f $_.Label, $_.HelpMessage }) | Out-String).Trim(), $Choices, 0);
$ModuleInstallLocation = $Choices[$result].HelpMessage | Join-Path -ChildPath:$ModuleBaseName;

if (-not ($ModuleInstallLocation | Test-Path)) {
    $folder = New-Item -Path:$ModuleInstallLocation -ItemType:'Directory';

    if ($folder -eq $null) {
        'Error creating destination folder.' | Write-Warning;
        return;
    }
}

Function Install-Module {
	Param(
		[Parameter(Mandatory = $true)]
		[ValidatePattern('*\.psd1')]
		[string]$SourceManifest,

		[Parameter(Mandatory = $true)]
		[string]$TargetDir
	)

	$ModuleName = [System.IO.Path]::GetFileNameWithoutExtension($SourceManifest);
	$SourceFolder = $SourceManifest | Split-Path -Parent;
	$ModuleManifestData = & $SourceManifest;
	if ($ModuleManifestData -eq $null) {
		throw ('Error reading module manifest data from "{0}".' -f $SourceManifest);
		return;
	}
	if ($ModuleManifestData.RootModule -eq $null -or $ModuleManifestData.RootModule -eq '') {
		if ($ModuleManifestData.ModuleToProcess -eq $null -or $ModuleManifestData.ModuleToProcess -eq '') {
			$RootModule = $SourceFolder | Join-Path -ChildPath ($ModuleName + '.psm1');
			if (-not ($RootModule | Test-Path -PathType Leaf)) {
				$RootModule = $SourceFolder | Join-Path -ChildPath ($ModuleName + '.dll');
				if (-not ($RootModule | Test-Path -PathType Leaf)) {
					throw ('Cannot find module at "{0}" named "{1}.psm1" or "{1}.dll".' -f $SourceFolder, $ModuleName);
				}
			}
		} else {
			$RootModule = $ModuleManifestData.ModuleToProcess;
		}
	} else {
		$RootModule = $ModuleManifestData.RootModule;
	}

	Copy-Item -Path:$SourceManifest -Destination:($TargetDir | Join-Path -ChildPath ($ModuleName + '.psd1')) -Force;
	Copy-Item -Path:$RootModule -Destination:($TargetDir | Join-Path -ChildPath ($RootModule | Split-Path -Leaf)) -Force;
	# Copy other files and nested modules
}
Install-Module -SourceManifest $PSScriptRoot | Join-Path -ChildPath:($ModuleBaseName + '.psd1') -TargetDir $ModuleInstallLocation -ErrorAction Stop;

$ModuleManifestData = $null;

if ($ModuleManifestData -eq $null) {
	('Error reading module manifest data from "{0}".' -f $ModuleManifestSourcePath) | Write-Warning;
}
$source = $PSScriptRoot | Join-Path -ChildPath:$fileName;
$destination = $ModuleInstallLocation | Join-Path -ChildPath:$fileName;
Copy-Item -Path:$source -Destination:$destination -Force;

$fileName = $ModuleBaseName + '.psd1';
$source = $PSScriptRoot | Join-Path -ChildPath:$fileName;
$destination = $ModuleInstallLocation | Join-Path -ChildPath:$fileName;
Copy-Item -Path:$source -Destination:$destination -Force;

'Finished.' | Write-Host;
