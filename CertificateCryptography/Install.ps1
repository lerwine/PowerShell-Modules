$ModuleName = 'Erwine.Leonard.T.CertificateCryptography';
if ($PSScriptRoot -eq $null) { $PSScriptRoot = (Get-Location).Path }
$ModuleManifestPath = $PSScriptRoot | Join-Path -ChildPath ('{0}.psd1' -f $ModuleName);
if (-not ($ModuleManifestPath | Test-Path -PathType Leaf)) {
    Write-Error -Message ('Module file not found at "{0}".' -f $ModuleManifestPath) -Category ObjectNotFound -ErrorAction Stop;
    return;
}
try {
    $SourceModuleInfo = $ModuleManifestPath | Test-ModuleManifest -ErrorAction Stop;
} catch {
    Write-Error -ErrorRecord $_ -ErrorAction Continue;
    Write-Error -Message ('Error loading module file from "{0}".' -f $ModuleManifestPath) -Category OpenErro -ErrorAction Stop;
    return;
}

$CreatablePaths = @([System.Environment+SpecialFolder]::MyDocuments, [System.Environment+SpecialFolder]::ProgramFiles) | ForEach-Object {
    New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList ([System.IO.Path]::Combine([System.Environment]::GetFolderPath($_), 'WindowsPowerShell\Modules'));
};

$InstalledModules = @(Get-Module -ListAvailable | Where-Object { $_.Name -ieq $SourceModuleInfo.Name });
$InstallData = @(
    $env:PSModulePath.Split([System.IO.Path]::PathSeparator) | ForEach-Object {
        $InstallTarget = @{
            DirectoryInfo = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList ($_ | Join-Path -ChildPath $SourceModuleInfo.Name);
            InstalledModule = $null;
            Message = @();
            ErrorCategory = $null;
        };
        foreach ($m in $InstalledModules) {
            $InstalledBase = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList $m.ModuleBase;
            if ($InstalledBase.FullName -ieq $InstallTarget.DirectoryInfo.FullName) {
                $InstallTarget.InstalledModule = $m;
                if ($m.Guid -ne $SourceModuleInfo.Guid) {
                    $InstallTarget.Message = @(
                        ('Another module of the same name is located at "{0}".' -f $m.ModuleBase),
                        'That module must be manually removed before this one can be installed there.'
                    );
                    $InstallTarget.ErrorCategory = [System.Management.Automation.ErrorCategory]::PermissionDenied;
                } else {
                    if ($m.Version -gt $SourceModuleInfo.Version) {
                        $InstallTarget.Message = @(
                            ('The module located at "{0}" is a later version ({1})' -f $m.ModuleBase, $m.Version),
                            ('If you install the module here, the version would be rolled back to {0}.' -f $SourceModuleInfo.Version)
                        );
                    }
                }
                break;
            }
        }
        
        if ($InstallTarget.InstalledModule -eq $null) {
            if ($Installtarget.DirectoryInfo.Exists) {
                $InstallTarget.Message = @(
                    ('A folder of the same name exists at "{0}", but does not contain a valid module manifest.' -f $Installtarget.DirectoryInfo),
                    'This folder must be manually removed before this module can be installed there.'
                );
                $InstallTarget.ErrorCategory = [System.Management.Automation.ErrorCategory]::PermissionDenied;
            } else {
                if (-not $Installtarget.DirectoryInfo.Parent.Exists) {
                    $cp = @($CreatablePaths | Where-Object { $Installtarget.DirectoryInfo.FullName.Length -ge $_.FullName -and $Installtarget.FullName.Substring(0, $_.DirectoryInfo.FullName.Length) -ieq $_.DirectoryInfo.FullName });
                    if ($cp.Count -eq 0) {
                        $InstallTarget.Message = @(
                            ('Module path root folder "{0}" does not exist.' -f $Installtarget.DirectoryInfo.Parent),
                            'This folder must be manually created before this module can be installed there.'
                        );
                        $InstallTarget.ErrorCategory = [System.Management.Automation.ErrorCategory]::ObjectNotFound;
                    } else {
                        if (-not $cp[0].Exists) {
                            if (-not $cp[0].Parent.Exists) {
                                if ($cp[0].Parent.Parent -eq $null) {
                                    $InstallTarget.Message = @(
                                        ('Module path root folder "{0}" does not exist.' -f $cp[0].Parent),
                                        'This folder must be manually created before this module can be installed there.'
                                    );
                                    $InstallTarget.ErrorCategory = [System.Management.Automation.ErrorCategory]::ObjectNotFound;
                                } else {
                                    if (-not $cp[0].Parent.Parent.Exists) {
                                        $InstallTarget.Message = @(
                                            ('Module path root folder "{0}" does not exist.' -f $cp[0].Parent.Parent),
                                            'This folder must be manually created before this module can be installed there.'
                                        );
                                        $InstallTarget.ErrorCategory = [System.Management.Automation.ErrorCategory]::ObjectNotFound;
                                    }
                                }
                            } else {
                                $InstallTarget.Message = @('Subdirectory "{0}" will be created if you install the module here.' -f $cp[0]);
                            }
                        }
                    }
                }
            }
        }
        $InstallTarget | Write-Output;
    }
);

$Messages = @($InstallData | Where-Object { $_.Message.Count -gt 0 });
if ($Messages.Count -gt 0) {
    $Messages | ForEach-Object {
        if ($_.ErrorCategory -eq $null) {
            ($_.Message | Out-String -Stream) | Write-Warning -WarningAction Continue;
        } else {
            ($_.Message | Out-String -Stream) | Write-Error -Category $_.ErrorCategory -ErrorAction Continue;
        }
    }
    $CriticalErrors = $Messages | Where-Object { $_.ErrorCategory -ne $null }
    if ($CriticalErrors -ne $null) { return }
}

if ($InstallData.Count -gt 0) {
    $defaultIndex = 0;
    for ($i = 0; $i -lt $InstallData.Count; $i++) {
        if ($defaultIndex -gt 0 -and $InstallData[$i].InstalledModule -ne $null) { $defaultIndex = $i + 1 }
        $Choices.Add((New-Object -TypeName:'System.Management.Automation.Host.ChoiceDescription' -ArgumentList:($i + 1, $InstallData[$i].DirectoryInfo.FullName)));
    }
    $Choices.Add((New-Object -TypeName:'System.Management.Automation.Host.ChoiceDescription' -ArgumentList:(0, 'Use a custom location')));
    if ($defaultIndex -eq 0) { $defaultIndex = 1 }
    $result = $Host.UI.PromptForChoice('Select target location', 'Select a target module location', $Choices, $defaultIndex);
    if ($result -eq $null -or $result -lt 0 -or $result -gt $InstallData.Count) {
        'Aborted.' | Write-Warning -WarningAction Stop;
        return;
    }
    if ($result -gt 0) { $InstallTarget = $InstallData[$result - 1] }
}

if ($InstallTarget -ne $null) {
    if ($InstallTarget.Message.Count -gt 0) {
        @(for ($i = 1; $i -lt $InstallTarget.Message.Count; $i++) { $InstallTarget.Message[$i - 1] }) | Write-Warning -WarningAction Continue;
        $InstallTarget.Message[$InstallTarget.Message.Count - 1] | Write-Warning -WarningAction Inquire;
    }
} else {
    'Custom location not yet implemented.' Write-Warning -WarningAction Stop;
    return;
}

if (-not $InstallTarget.Directory.Exists) {
    if (-not $InstallTarget.Directory.Parent.Exists) {
        if (-not $InstallTarget.Directory.Parent.Parent.Exists) {
            try {
                $InstallTarget.Directory.Parent.Parent.Create();
                $InstallTarget.Directory.Parent.Parent.Refresh();
                if (-not $InstallTarget.Directory.Parent.Parent.Exists) {  }
            } catch {
                Write-Error -ErrorRecord $_ -ErrorAction Continue;
                Write-Error -Message ('Unable to create directory "{0}".' -f $InstallTarget.Directory.Parent.Parent.FullName) -Category PermissionDeined -ErrorAction Stop;
            }
        }
        try {
            $InstallTarget.Directory.Parent.Create();
            $InstallTarget.Directory.Parent.Refresh();
            if (-not $InstallTarget.Directory.Parent.Exists) {  }
        } catch {
            Write-Error -ErrorRecord $_ -ErrorAction Continue;
            Write-Error -Message ('Unable to create directory "{0}".' -f $InstallTarget.Directory.Parent.FullName) -Category PermissionDeined -ErrorAction Stop;
        }
    }
    try {
        $InstallTarget.Directory.Create();
        $InstallTarget.Directory.Refresh();
        if (-not $InstallTarget.Directory.Exists) {  }
    } catch {
        Write-Error -ErrorRecord $_ -ErrorAction Continue;
        Write-Error -Message ('Unable to create directory "{0}".' -f $InstallTarget.Directory.FullName) -Category PermissionDeined -ErrorAction Stop;
    }
}

Copy-Item -Path $ModuleManifestPath -Destination $InstallTarget.Directory;

# TODO: Get rest of them
$fileName = $ModuleBaseName + '.psd1';
$source = $PSScriptRoot | Join-Path -ChildPath:$fileName;
$destination = $ModuleInstallLocation | Join-Path -ChildPath:$fileName;
Copy-Item -Path:$source -Destination:$destination -Force;

'Finished.' | Write-Host;
