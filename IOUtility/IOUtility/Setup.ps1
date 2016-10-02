$Script:ModuleName = 'Erwine.Leonard.T.IOUtility';
$Script:IncludeDebugInformation = $true;
$Script:InstallRoot = [System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::MyDocuments) | Join-Path -ChildPath 'WindowsPowerShell\Modules';
# $Script:InstallRoot = $PSHome | Join-Path -ChildPath 'Modules';

if ($PSVersionTable.PSVersion.Major -lt 3) { $PSScriptRoot = $MyInvocation.MyCommand.Path | Split-Path -Parent }

$Script:ManifestProperties = [ScriptBlock]::Create([System.IO.File]::ReadAllText(($PSScriptRoot | Join-Path -ChildPath ('{0}.psd1' -f $Script:ModuleName))).Trim()).Invoke()[0]

foreach ($s in $Script:ManifestProperties.PrivateData.CompilerOptions.CustomTypeSourceFiles) {
    if ($Script:ManifestProperties.FileList -inotcontains $s) { $Script:ManifestProperties.FileList = $Script:ManifestProperties.FileList + @($s) }
}

$Defines = @();
if ($Script:IncludeDebugInformation) { $Defines = $Defines + 'DEBUG' }
if ($PSVersionTable.CLRVersion.Major -lt 3 -or ($PSVersionTable.PSVersion.Major -lt 3 -and $PSVersionTable.CLRVersion.Major -eq 3 -and $PSVersionTable.CLRVersion.Minor -lt 5)) {
    $Defines = $Defines + @('PSLEGACY', 'PSLEGACY2');
} else {
    if ($PSVersionTable.PSVersion.Major -lt 3 -or $PSVersionTable.CLRVersion.Major -lt 4) {
        $Defines = $Defines + @('PSLEGACY', 'PSLEGACY3');
    }
}
$CompilerParameters = '';
if ($Defines.Count -gt 0) { $CompilerParameters = '/define:{0}' -f ($Defines -join ';') }
$Script:ManifestProperties.PrivateData = @'
@{{
	CompilerOptions = @{{
		IncludeDebugInformation = ${0};
        CompilerParameters = '{1}';
        CustomTypeSourceFiles = {2};
	}};
}};
'@ -f $Script:IncludeDebugInformation.ToString().ToLower(), $CompilerParameters,
    (($Script:ManifestProperties.PrivateData.CompilerOptions.CustomTypeSourceFiles | ForEach-Object { "'{0}'" -f $_.Replace("'", "''") }) -join ', ');

$ManifestCode = [System.Text.RegularExpressions.Regex]::Replace(@'
@{

# Script module or binary module file associated with this manifest.
RootModule = Value

# Version number of this module.
ModuleVersion = Value

# ID used to uniquely identify this module
GUID = Value

# Author of this module
Author = Value

# Company or vendor of this module
CompanyName = Value

# Copyright statement for this module
Copyright = Value

# Description of the functionality provided by this module
Description = Value

# Minimum version of the Windows PowerShell engine required by this module
PowerShellVersion = Value

# Name of the Windows PowerShell host required by this module
PowerShellHostName = Value

# Minimum version of the Windows PowerShell host required by this module
PowerShellHostVersion = Value

# Minimum version of Microsoft .NET Framework required by this module
DotNetFrameworkVersion = Value

# Minimum version of the common language runtime (CLR) required by this module
CLRVersion = Value

# Processor architecture (None, X86, Amd64) required by this module
ProcessorArchitecture = Value

# Modules that must be imported into the global environment prior to importing this module
RequiredModules = Array

# Assemblies that must be loaded prior to importing this module
RequiredAssemblies = Array

# Script files (.ps1) that are run in the caller's environment prior to importing this module.
ScriptsToProcess = Array

# Type files (.ps1xml) to be loaded when importing this module
TypesToProcess = Array

# Format files (.ps1xml) to be loaded when importing this module
FormatsToProcess = Array

# Modules to import as nested modules of the module specified in RootModule/ModuleToProcess
NestedModules = Array

# Functions to export from this module
FunctionsToExport = Array

# Cmdlets to export from this module
CmdletsToExport = Value

# Variables to export from this module
VariablesToExport = Value

# Aliases to export from this module
AliasesToExport = Value

# List of all modules packaged with this module
ModuleList = Array

# List of all files packaged with this module
FileList = Array

# Private data to pass to the module specified in RootModule/ModuleToProcess
PrivateData = AsIs

# HelpInfo URI of this module
HelpInfoURI = Value

# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
DefaultCommandPrefix = Value

}
'@, '^(?<s>\s*)(?<k>[a-z][a-z\d_]*)(?<o>\s*=\s*)(?<t>Value|Array|AsIs)$', {
    $Key = $Args[0].Groups['k'].Value;
    $Field = $Key;
    $Type = $Args[0].Groups['t'].Value;
    if ($Key -eq 'RootModule' -and $PSVersionTable.PSVersion.Major -lt 3) { $Field = 'ModuleToProcess' }
	switch ($Key) {
		'PowerShellVersion' {
			'{0}PowerShellVersion{1}''{2}''' -f $Args[0].Groups['s'].Value, $Args[0].Groups['o'].Value, $PSVersionTable.PSVersion.ToString();
			break;
		}
		'CLRVersion' {
			'{0}CLRVersion{1}''{2}''' -f $Args[0].Groups['s'].Value, $Args[0].Groups['o'].Value, $PSVersionTable.CLRVersion.ToString();
			break;
		}
		'DotNetFrameworkVersion' {
			'{0}DotNetFrameworkVersion{1}''{2}''' -f $Args[0].Groups['s'].Value, $Args[0].Groups['o'].Value, $PSVersionTable.CLRVersion.ToString();
			break;
		}
		default
		{
			if ($Script:ManifestProperties.ContainsKey($Key)) {
				$Value = $Script:ManifestProperties[$Key];
				if ($Value -eq $null) {
					'{0}{1}{2}$null' -f $Args[0].Groups['s'].Value, $Field, $Args[0].Groups['o'].Value;
				} else {
					if ($Type -eq 'AsIs') {
						if ($Value -isnot [string]) { $Value = ($Value | Out-String) -replace '\r\n?$', '' }
						'{0}{1}{2}{3}' -f $Args[0].Groups['s'].Value, $Field, $Args[0].Groups['o'].Value, $Value;
					} else {
						if ($Value.GetType().IsArray) {
							if ($Value.Length -eq 0) {
								'{0}{1}{2}@()' -f $Args[0].Groups['s'].Value, $Field, $Args[0].Groups['o'].Value;
							} else {
								'{0}{1}{2}@({3})' -f $Args[0].Groups['s'].Value, $Field, $Args[0].Groups['o'].Value, (($Value | ForEach-Object {
									$s = $_;
									if ($_ -is [string]) { "'{0}'" -f $_.Replace("'", "''") } else { $_.ToString() }
								}) -join ', ')
							}
						} else {
							if ($Value -is [string]) {
								'{0}{1}{2}''{3}''' -f $Args[0].Groups['s'].Value, $Field, $Args[0].Groups['o'].Value, $Value.Replace("'", "''");
							} else {
								'{0}{1}{2}{3}' -f $Args[0].Groups['s'].Value, $Field, $Args[0].Groups['o'].Value, $Value;
							}
						}
					}
				}
			} else {
				if ($Type -eq 'Array') {
					"{0}# {1}{2}@()" -f $Args[0].Groups['s'].Value, $Field, $Args[0].Groups['o'].Value;
				} else {
					"{0}# {1}{2}''" -f $Args[0].Groups['s'].Value, $Field, $Args[0].Groups['o'].Value;
				}
			}
			break;
		}
	}
}, [System.Text.RegularExpressions.RegexOptions]::Multiline -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase);

$PrivateData = [ScriptBlock]::Create($Script:ManifestProperties.PrivateData).Invoke()[0];
$BasePathAndName = $PSScriptRoot | Join-Path -ChildPath $ModuleName;

$Local:Splat = @{
    TypeName = 'System.CodeDom.Compiler.CompilerParameters';
    ArgumentList = (,@(
		[System.Text.RegularExpressions.Regex].Assembly.Location,
        [System.Xml.XmlDocument].Assembly.Location,
        [System.Management.Automation.ScriptBlock].Assembly.Location,
		(Add-Type -AssemblyName 'System.Windows.Forms' -ErrorAction Stop -PassThru)[0].Assembly.Location,
		(Add-Type -AssemblyName 'System.Web.Services' -ErrorAction Stop -PassThru)[0].Assembly.Location
    ));
    Property = @{
        IncludeDebugInformation = $PrivateData.CompilerOptions.IncludeDebugInformation;
        CompilerOptions = '/doc:"{0}.xml"' -f $BasePathAndName;
    };
};

$FilesToCopy = @($Script:ManifestProperties.RootModule, ('{0}.xml' -f $ModuleName), ('about_{0}.help.txt' -f $ModuleName));

if ($PrivateData.CompilerOptions.IncludeDebugInformation) {
    $Local:Splat.Property.CompilerOptions += ' /pdb:"{0}.pdb"' -f $BasePathAndName;
    $FilesToCopy = $FilesToCopy + @('{0}.pdb' -f $ModuleName);
}

if ($PrivateData.CompilerOptions.CompilerParameters.Length -gt 0) { $Local:Splat.Property.CompilerOptions += (' ' + $PrivateData.CompilerOptions.CompilerParameters) }

Add-Type -Path ($PrivateData.CompilerOptions.CustomTypeSourceFiles | ForEach-Object { $PSScriptRoot | Join-Path -ChildPath $_ }) -CompilerParameters (New-Object @Local:Splat);

$DirectoryInfo = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList ([System.IO.Path]::Combine($Script:InstallRoot, $ModuleName));
if ($DirectoryInfo -eq $null) { return }
if (-not $DirectoryInfo.Exists) {
    if (-not $DirectoryInfo.Parent.Exists) {
        $DirectoryInfo.Parent.Create();
        $DirectoryInfo.Refresh();
        if (-not $DirectoryInfo.Parent.Exists) {
            Write-Warning -Message ('Failed to create {0}' -f $DirectoryInfo.Parent.FullName);
            return;
        }
    }
    $DirectoryInfo.Create();
    $DirectoryInfo.Refresh();
    if (-not $DirectoryInfo.Exists) {
        Write-Warning -Message ('Failed to create {0}' -f $DirectoryInfo.FullName);
        return;
    }
}

foreach ($f in $Script:ManifestProperties.FileList) { if ($FilesToCopy -inotcontains $f) { $FilesToCopy = $FilesToCopy + @($f) } }

$TargetPath = [System.IO.Path]::Combine($DirectoryInfo.FullName, ('{0}.psd1' -f $ModuleName));
('Creating {0}' -f $Targetpath) | Write-Host;
[System.IO.File]::WriteAllText($TargetPath, $ManifestCode);

('{0} => {1}' -f $PSScriptRoot, $DirectoryInfo.FullName) | Write-Host;

foreach ($f in $FilesToCopy) {
    $SourcePath = [System.IO.Path]::Combine($PSScriptRoot, $f);
    $f | Write-Host;
    Copy-Item -Path $SourcePath -Destination $DirectoryInfo.FullName;
}

'Finished.' | Write-Host;