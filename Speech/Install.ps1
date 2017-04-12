$Script:ModuleName = 'Erwine.Leonard.T.Speech';
$Script:InstallRoot = $MyInvocation.MyCommand.Definition | Split-Path -Parent;
$Script:SourceManifestPath = $Script:InstallRoot | Join-Path -ChildPath "$Script:ModuleName.psd1";
if (-not $Script:SourceManifestPath | Test-Path -PathType Leaf) {
    Write-Error -Message 'Module manifest file not found' -Category ObjectNotFound -TargetObject $Script:SourceManifestPath;
    return;
}
$Script:NewModuleManifest = Test-ModuleManifest -Path $Script:SourceManifestPath -ErrorAction Continue;
if ($Script:NewModuleManifest -eq $null) {
    Write-Error -Message 'Unable to load module manifest to be installed. Cannot continue.' -Category ReadError -TargetObject $Script:SourceManifestPath;
    return;
}
Function Get-SpecialFolderPath {
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [System.Environment+SpecialFolder]$SpecialFolder
    )
    Process {
        $Path = [System.Environment]::GetFolderPath($SpecialFolder);
        if (-not [System.String]::IsNullOrEmpty($Path)) { [System.IO.Path]::GetFullPath($Path) }
    }
}

$AllUsersBasePaths = @(([System.Environment+SpecialFolder]::ProgramFiles,
    [System.Environment+SpecialFolder]::ProgramFilesX86,
    [System.Environment+SpecialFolder]::CommonApplicationData,
    [System.Environment+SpecialFolder]::CommonDesktopDirectory,
    [System.Environment+SpecialFolder]::CommonProgramFiles,
    [System.Environment+SpecialFolder]::CommonProgramFilesX86,
    [System.Environment+SpecialFolder]::CommonPrograms,
    [System.Environment+SpecialFolder]::Programs) | Get-SpecialFolderPath | Select-Object -Unique);
$CurrentUserBasePaths = @(([System.Environment+SpecialFolder]::MyDocuments,
    [System.Environment+SpecialFolder]::ApplicationData,
    [System.Environment+SpecialFolder]::LocalApplicationData,
    [System.Environment+SpecialFolder]::Personal) | Get-SpecialFolderPath | Select-Object -Unique);

$Script:AvailableModules = @(Get-Module -ListAvailable | ForEach-Object {
    $ManifestPath = [System.IO.Path]::GetFullPath($_.Path);
    $InstallLocation = $ManifestPath | Split-Path -Parent;
    New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
        ManifestPath = New-Object -TypeName 'System.IO.FileInfo' -ArgumentList $ManifestPath;
        InstallLocation = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList $InstallLocation;
        InstallRoot = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList ($InstallLocation | Split-Path -Parent);
        ModuleInfo = $_;
    };
});

$Script:TargetLocations = @($env:PSModulePath.Split([System.IO.Path]::PathSeparator) | ForEach-Object {
    $Path = [System.IO.Path]::GetFullPath($_);
    $Properties = @{
        InstallRoot = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList $Path;
        IsInModulePath = $true;
    };
    if (($CurrentUserBasePaths | Where-Object { $Path.Length -ge $Path -and [String.Equals]::($Path, $_.Substring(0, $Path.Length), [StringComparison]::InvariantCultureIgnoreCase) }) -ne $null) {
        $Properties['Level'] = 2;
    } else {
        if (($AllUsersBasePaths | Where-Object { $Path.Length -ge $Path -and [String.Equals]::($Path, $_.Substring(0, $Path.Length), [StringComparison]::InvariantCultureIgnoreCase) }) -ne $null) {
            $Properties['Level'] = 1;
        } else {
            $Properties['Level'] = 0;
        }
    }
    $Properties | Write-Output;
});
if (($Script:TargetLocations | Where-Object { $_['Level'] -eq 2 }) -eq $null) {
    $Script:TargetLocations = $Script:TargetLocations + @(@{
        InstallRoot = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList ($CurrentUserBasePaths[0] | Join-Path -ChildPath 'WindowsPowerShell\Modules');
        IsInModulePath = $false;
        Level = 2;
    });
}
if (($Script:TargetLocations | Where-Object { $_['Level'] -eq 1 }) -eq $null) {
    $Script:TargetLocations = $Script:TargetLocations + @(@{
        InstallRoot = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList ($AllUsersBasePaths[0] | Join-Path -ChildPath 'WindowsPowerShell\Modules');
        IsInModulePath = $false;
        Level = 1;
    });
}
if (($Script:TargetLocations | Where-Object { $_['Level'] -eq 1 }) -eq $null) {
    $Script:TargetLocations = $Script:TargetLocations + @(@{
        InstallRoot = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList ($PSHOME | Join-Path -ChildPath 'Modules');
        IsInModulePath = $false;
        Level = 0;
    });
}
$Script:TargetLocations | ForEach-Object {
    $Path = $_['Directory'].FullName;
    $ExistingModules = @($Script:AvailableModules | Where-Object { $_.InstallRoot.FullName -ieq $Path });
    if ($ExistingModules.Count -eq 0) {
        $_['InstallLocation'] = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList ($_['InstallRoot'].FullName | Join-Path $Script:ModuleName);
        $_['ManifestPath'] = New-Object -TypeName 'System.IO.FileInfo' -ArgumentList ($_['ManifestPath'].FullName | Join-Path "$Script:ModuleName.psd1");
        if ($_['ManifestPath'].Exists) {
            $_['ModuleInfo'] = Test-ModuleManifest -Path $_['ManifestPath'].FullName -ErrorAction Continue;
        } else {
            $_['ModuleInfo'] = $null;
        }
    } else {
        $_['InstallLocation'] = $ExistingModules[0].InstallLocation;
        $_['ManifestPath'] = $ExistingModules[0].ManifestPath;
        $_['ModuleInfo'] = $ExistingModules[0].ModuleInfo;
    }
    
    if ($_['InstallLocation'].Level -eq 0) {
        $_['Type'] = 'Administrators';
    } else {
        if ($_['InstallLocation'].Level -eq 1) {
            $_['Type'] = 'All Users';
        } else {
            $_['Type'] = 'Current User';
        }
    }
    
    $_['OkayToInstall'] = $false;
    if ($_['ModuleInfo'] -eq $null) {
        if ($_['ManifestPath'].Exists) {
            $_['Message'] = 'Unable to determine status of module at this location';
        } else {
            if ($_['ManifestPath'].FullName | Test-Path -PathType Container) {
                $_['Message'] = 'A directory already exists at the path for the module manifest at this location.';
            } else {
                if ($_['InstallLocation'].FullName | Test-Path) {
                    if ($_['InstallLocation'].FullName | Test-Path -PathType Leaf) {
                        $_['Message'] = 'A file already exists where the module install folder would be.';
                    } else {
                        $_['Message'] = 'The target install folder already exists, but does not contain a module manifest.';
                    }
                } else {
                    if ($_['InstallRoot'].FullName | Test-Path) {
                        if ($_['InstallRoot'].FullName | Test-Path -PathType Leaf) {
                            $_['Message'] = 'A file already exists where the module install root should be.';
                        } else {
                             $_['OkayToInstall'] = $true;
                        }
                    } else {
                        $_['Message'] = 'A file already exists where the module install root does not exist.';
                    }
                }
            }
        }
    } else {
        # TODO: See if module manifest is for module being installed, and if it is an earlier version.
        $_['OkayToInstall'] = $true;
    }
    New-Object -TypeName 'System.Management.Automation.PSObject' -Property $_;
}

$Choices = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
$ChoiceDescription = New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList $Label, $Message;
