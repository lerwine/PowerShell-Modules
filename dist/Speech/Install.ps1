# Name of module being installed.
$Script:ModuleName = 'Erwine.Leonard.T.Speech';

# Get installation source paths.
$Script:InstallRoot = $MyInvocation.MyCommand.Definition | Split-Path -Parent;
$Script:SourceManifestPath = $Script:InstallRoot | Join-Path -ChildPath "$Script:ModuleName.psd1";

# Validate module manifest before installing.
if (-not $Script:SourceManifestPath | Test-Path -PathType Leaf) {
    Write-Error -Message 'Module manifest file not found' -Category ObjectNotFound -TargetObject $Script:SourceManifestPath;
    return;
}
$Script:NewModuleManifest = Test-ModuleManifest -Path $Script:SourceManifestPath -ErrorAction Continue;
if ($Script:NewModuleManifest -eq $null) {
    Write-Error -Message 'Unable to load module manifest to be installed. Cannot continue.' -Category ReadError -TargetObject $Script:SourceManifestPath;
    return;
}

Add-Type -TypeDefinition @'
namespace InstallCLR
{
    using System;
    using System.IO;
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    public enum InstallType
    {
        Other,
        Administrators,
        AllUsers,
        CurrentUser
    }
    public enum InstallAction
    {
        None,
        Install,
        Upgrade,
        Replace,
        Overwrite,
        Uninstall
    }
    public class ModuleTargetInfo
    {
        public FileInfo ManifestFile { get; set; }
        public DirectoryInfo InstallLocation { get { return ManifestFile.Directory; } }
        public DirectoryInfo InstallRoot { get { return ManifestFile.Directory.Parent; } }
        public PSModuleInfo ModuleInfo { get; set; }
        public InstallAction Action { get; private set; }
        public InstallType Type { get; set; }
        public bool IsInModulePath { get; set; }
        public string Message { get; private set; }
        public ModuleTargetInfo() { }
        public ModuleTargetInfo(PSModuleInfo targetModuleInfo)
        {
            if (targetModuleInfo == null)
                return;
            ModuleInfo = targetModuleInfo;
            ManifestFile = new FileInfo(Path.Combine(targetModuleInfo.Path, targetModuleInfo.Name + ".psd1"));
        }
        public ModuleTargetInfo(string installLocation, string moduleName)
        {
            if (String.IsNullOrEmpty(installLocation))
                return;
            ManifestFile = new FileInfo(Path.Combine(installLocation, moduleName + ".psd1"));
        }
        public void Initialize(PSModuleInfo sourceModuleInfo)
        {
            if (sourceModuleInfo == null)
                throw new ArgumentNullException("sourceModuleInfo");

            if (ModuleInfo != null)
            {
                if (sourceModuleInfo.Guid.Equals(ModuleInfo.Guid))
                {
                    if (sourceModuleInfo.Version < ModuleInfo.Version)
                    {
                        Action = InstallAction.Upgrade;
                        Message = String.Format("Version {0} is currently installed at this location.", ModuleInfo.Version);
                        return;
                    }
                
                    Action = InstallAction.Uninstall;

                    if (sourceModuleInfo.Version == ModuleInfo.Version)
                        Message = "Current Version is already installed at this location.";
                    else
                        Message = String.Format("Version {0} is currently installed at this location.", ModuleInfo.Version);
                    return;
                }
            
                Action = InstallAction.Replace;
                Message = String.Format("{0}, version {1}  ({2}) is currently installed at this location.", ModuleInfo.Name, ModuleInfo.Version, ModuleInfo.Guid);
                return;
            }
            
            if (ManifestFile == null)
            {
                Action = InstallAction.None;
                Message = "Installation location not defined.";
                return;
            }

            if (ManifestFile.Exists)
                Message = "Module manifest at installation location could not be parsed.";
            else
            {
                if (Directory.Exists(ManifestFile.FullName))
                    Message = "Directory exists where the manifest file would be.";
                else if (ManifestFile.Directory.Exists)
                    Message = "Installation location exists, but does not have an existing module manifest.";
                else if (File.Exists(ManifestFile.Directory.FullName)
                    Message = "File exists where installation location would be.";
                else if (ManifestFile.Directory.Parent.Exists)
                {
                    Action = InstallAction.Install;
                    Message = "OK";
                    return;
                }
                else
                {
                    if (File.Exists(ManifestFile.Directory.Parent.FullName)
                        Message = "A file exists where the install root should be.";
                    else
                        Message = "The selected install root does not exist.";
                    Action = InstallAction.None;
                    return;
                }
            }
            Action = InstallAction.Overwrite;
        }
        public static Collection<ModuleTargetInfo> CreateFromManifests(IEnumerable<PSModuleInfo> modules, PSModuleInfo matchingModule)
        {
            Collection<ModuleTargetInfo> result = new Collection<ModuleTargetInfo>();
            if (modules != null)
            {
                if (matchingModule == null)
                {
                    foreach (PSModuleInfo module in modules.Where(m => m != null))
                        result.Add(new ModuleTargetInfo(module));
                }
                else
                {
                    foreach (PSModuleInfo module in modules.Where(m => m != null))
                    {
                        ModuleTargetInfo mti = new ModuleTargetInfo(module);
                        if (String.Equals(mti.InstallLocation.Name, matchingModule.Name, StringComparison.InvariantCultureIgnoreCase))
                            result.Add(mti);
                    }
                }
            }
            return result;
        }
    }
}
'@ -ReferencedAssemblies ([System.Int32].Assembly.Location, [System.Uri].Assembly.Location, [System.Xml.Serialization.XmlAttributeAttribute].Assembly.Location, [System.Management.Automation.PSObject].Assembly.Location);

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
    [System.Environment+SpecialFolder]::Programs) | Get-SpecialFolderPath | Where-Object { -not [System.String]::IsNullOrEmpty($_) } | Select-Object -Unique);
$CurrentUserBasePaths = @(([System.Environment+SpecialFolder]::MyDocuments,
    [System.Environment+SpecialFolder]::ApplicationData,
    [System.Environment+SpecialFolder]::LocalApplicationData,
    [System.Environment+SpecialFolder]::Personal) | Get-SpecialFolderPath | Where-Object { -not [System.String]::IsNullOrEmpty($_) } | Select-Object -Unique);

$Script:AvailableModules = [InstallCLR.ModuleTargetInfo]::CreateFromManifests((Get-Module -ListAvailable), $Script:NewModuleManifest);

$Script:InstallOptions = @($env:PSModulePath.Split([System.IO.Path]::PathSeparator) | ForEach-Object {
    $Path = [System.IO.Path]::GetFullPath($_);
    $matching = @($Script:AvailableModules | Where-Object { $_.IsContainedWithin($Path) });
    if ($matching.Count -gt 0) {
        $matching[0] | Write-Output;
    } else {
        (New-Object -TypeName 'InstallCLR.ModuleTargetInfo' -ArgumentList $Path, $Script:NewModuleManifest.Name) | Write-Output;
    }
} | ForEach-Object {
    $_.IsInModulePath = $true;
    $Path = $_.InstallLocation.FullName;
    if (@($CurrentUserBasePaths | Where-Object { $Path.Length -gt $_.Length -and [System.String]::Equals($Path.Substring(0, $_.Length), $_, [System.StringComparison]::InvariantCultureIgnoreCase) }).Count -gt 0) {
        $_.Type = [InstallCLR.InstallType]::CurrentUser;
    } else {
        if (@($AllUsersBasePaths | Where-Object { $Path.Length -gt $_.Length -and [System.String]::Equals($Path.Substring(0, $_.Length), $_, [System.StringComparison]::InvariantCultureIgnoreCase) }).Count -gt 0) {
            $_.Type = [InstallCLR.InstallType]::AllUsers;
        } else {
            $_.Type = [InstallCLR.InstallType]::Administrators;
        }
    }
});
if (($Script:InstallOptions | Where-Object { $_.Type -eq [InstallCLR.InstallType]::CurrentUser }) -eq $null) {
    $m = New-Object -TypeName 'InstallCLR.ModuleTargetInfo' -ArgumentList ($CurrentUserBasePaths[0] | Join-Path -ChildPath 'WindowsPowerShell\Modules'), $Script:NewModuleManifest.Name;
    $m.Type = [InstallCLR.InstallType]::CurrentUser;
    $m.IsInModulePath = $false;
    $Script:InstallOptions = $Script:InstallOptions + @($m);
}
if (($Script:TargetLocations | Where-Object { $_.Type -eq [InstallCLR.InstallType]::AllUsers }) -eq $null) {
    $m = New-Object -TypeName 'InstallCLR.ModuleTargetInfo' -ArgumentList ($AllUsersBasePaths[0] | Join-Path -ChildPath 'WindowsPowerShell\Modules'), $Script:NewModuleManifest.Name;
    $m.Type = [InstallCLR.InstallType]::AllUsers;
    $m.IsInModulePath = $false;
    $Script:InstallOptions = $Script:InstallOptions + @($m);
}
if (($Script:TargetLocations | Where-Object { $_['Level'] -eq 1 }) -eq $null) {
    $m = New-Object -TypeName 'InstallCLR.ModuleTargetInfo' -ArgumentList ($AllUsersBasePaths[0] | Join-Path -ChildPath 'WindowsPowerShell\Modules'), $Script:NewModuleManifest.Name;
    $m.Type = [InstallCLR.InstallType]::AllUsers;
    $m.IsInModulePath = $false;
    $Script:InstallOptions = $Script:InstallOptions + @($m);

    $Script:TargetLocations = $Script:TargetLocations + @(@{
        InstallRoot = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList ($PSHOME | Join-Path -ChildPath 'Modules');
        IsInModulePath = $false;
        Level = 0;
    });
}

$InstallOptions = @($Script:TargetLocations | ForEach-Object {
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
        $_['Action'] = 'Overwrite';
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
                             $_['Action'] = 'Install';
                             $_['Message'] = 'OK';
                        }
                    } else {
                        $_['Message'] = 'A file already exists where the module install root does not exist.';
                    }
                }
            }
        }
    } else {
        if ($_['ModuleInfo'].Guid.Equals($Script:NewModuleManifest.Guid)) {
            if ($_['ModuleInfo'].Version -lt $Script:NewModuleManifest.Version) {
                $_['Action'] = 'Upgrade';
                $_['OkayToInstall'] = $true;
                $_['Message'] = "Version $($_['ModuleInfo'].Version) is currently installed";
            } else {
                if ($_['ModuleInfo'].Version -eq $Script:NewModuleManifest.Version) {
                    $_['Action'] = 'Repair';
                    $_['OkayToInstall'] = $true;
                    $_['Message'] = 'Already Installed';
                } else {
                    $_['Action'] = 'Downgrade';
                    $_['Message'] = "Version $($_['ModuleInfo'].Version) is currently installed";
                }
            }
        } else {
            $_['Message'] = $_['Message'] = "$($_['ModuleInfo'].Name), version $($_['ModuleInfo'].Version) ($($_['ModuleInfo'].Guid)) is currently installed at this location.";
            $_['Action'] = 'Replace';
        }
    }
    New-Object -TypeName 'System.Management.Automation.PSObject' -Property $_;
});

$InstallOptions | Select-Object -Property @{ Name="Path"; Expression = { $_.InstallLocation.FullName } }, 'Type', 'Action', 'Message' | Out-GridView -Title 'Select Installation Location' -OutputMode Single;
#$Choices = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
#$ChoiceDescription = New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList $Label, $Message;
