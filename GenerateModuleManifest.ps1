Param(
    [Parameter(Position = 0)]
    [string]$ProjectFilePath = 'E:\Visual Studio 2015\Projects\PowerShell-Modules\master\Speech\Speech.csproj',
    
    [string]$ModuleManifestPath = 'E:\Visual Studio 2015\Projects\PowerShell-Modules\master\Speech\Erwine.Leonard.T.Speech.psd1',
    
    [string]$RootModulePath = 'E:\Visual Studio 2015\Projects\PowerShell-Modules\master\Speech\bin\Debug\Erwine.Leonard.T.Speech.dll'
)

$ProjectRoot = $ProjectFilePath | Split-Path -Parent;
$ProjectXml = New-Object -TypeName 'System.Xml.XmlDocument';
$ProjectXml.Load($ProjectFilePath);
if ($ProjectXml.DocumentElement -eq $null) { throw 'Unable to load project file'; }
$nsmgr = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $ProjectXml.NameTable;
$nsmgr.AddNamespace('msb', 'http://schemas.microsoft.com/developer/msbuild/2003');
$IsDll = $ProjectXml.SelectSingleNode('/msb:Project/msb:PropertyGroup[msb:OutputType="library"]', $nsmgr) -ne $null;
if (-not $PSBoundParameters.ContainsKey('ModuleManifestPath')) {
    $ModuleManifestPath = '';
    $Attributes = @(@($ProjectXml.SelectNodes('/msb:Project/msb:ItemGroup/msb:Content/@Include', $nsmgr)) | Where-Object { $_.Value.ToLower().EndsWith('.psd1') });
    if ($Attributes.Count -eq 0) {
        $Attributes = @(@($ProjectXml.SelectNodes('/msb:Project/msb:ItemGroup/msb:Compile/@Include', $nsmgr)) | Where-Object { $_.Value.ToLower().EndsWith('.psd1') });
        if ($Attributes.Count -eq 0) {
            $Attributes = @(@($ProjectXml.SelectNodes('/msb:Project/msb:ItemGroup/msb:None/@Include', $nsmgr)) | Where-Object { $_.Value.ToLower().EndsWith('.psd1') });
        }
    }
    if ($Attributes.Count -eq 1) {
        $ModuleManifestPath = $ProjectRoot | Join-Path -ChildPath $Attributes[0].Value;
    } else {
        if ($Attributes.Count -eq 0) {
            if ($IsDll) {
                $XmlElement = $ProjectXml.SelectSingleNode('/msb:Project/msb:PropertyGroup/msb:AssemblyName', $nsmgr);
                if ($XmlElement -ne $null -and (-not $XmlElement.IsEmpty)) {
                    $ModuleManifestPath = $XmlElement.InnerText.Trim();
                }
            }
        } else {
            if ($IsDll) {
                $XmlElement = $ProjectXml.SelectSingleNode('/msb:Project/msb:PropertyGroup/msb:AssemblyName', $nsmgr);
                if ($XmlElement -ne $null -and (-not $XmlElement.IsEmpty)) {
                    $s = $XmlElement.InnerText.Trim();
                    if ($s.Length -gt 0 -and @($Attributes | Where-Object { $_.Value -ieq $s}).Count -gt 0) {
                        $ModuleManifestPath = $s;
                    }
                }
            }
            if ($ModuleManifestPath.Length -eq 0) {
                $s = $ProjectFilePath | Split-Path -Leaf;
                if ($s.Length -gt 0 -and @($Attributes | Where-Object { $_.Value -ieq $s}).Count -gt 0) {
                    $ModuleManifestPath = $s;
                }
            }
        }
        if ($ModuleManifestPath.Length -eq 0) {
            if ($IsDll) {
                $XmlElement = $ProjectXml.SelectSingleNode('/msb:Project/msb:PropertyGroup/msb:AssemblyName', $nsmgr);
                if ($XmlElement -ne $null -and (-not $XmlElement.IsEmpty)) {
                    $ModuleManifestPath = $XmlElement.InnerText.Trim();
                }
            }
            if ($ModuleManifestPath.Length -eq 0) {
                $ModuleManifestPath = $ProjectFilePath | Split-Path -Leaf;
            }
        }
        $ModuleManifestPath = $ProjectRoot | Join-Path -ChildPath "$ModuleManifestPath.psd1";
    }
}
$ModuleManifest = @{ };
if (Test-Path -LiteralPath $ModuleManifestPath) {
    $ScriptBlock = [ScriptBlock]::Create([System.IO.File]::ReadAllText($ModuleManifestPath));
    $arr = @($ScriptBlock.Invoke());
    if ($arr.Count -gt 0 -and $arr[0] -is [Hashtable]) {
        $ModuleManifest = $arr[0];
    }
}

if ($PSBoundParameters.ContainsKey('RootModulePath')) {
    $ModuleManifest['RootModule'] | Split-Path -Leaf;
} else {
    if ($IsDll) {
        $XmlElement = $ProjectXml.SelectSingleNode('/msb:Project/msb:PropertyGroup/msb:AssemblyName', $nsmgr);
        if ($XmlElement -ne $null -and (-not $XmlElement.IsEmpty)) {
            $s = $XmlElement.InnerText.Trim();
            if ($s.Length -gt 0) {
                $ModuleManifest['RootModule'] = "$s.dll";
            }
        }
        if ($ModuleManifest['RootModule'] -eq $null) {
            $Attributes = @(@($ProjectXml.SelectNodes('/msb:Project/msb:ItemGroup/msb:Content/@Include', $nsmgr)) | Where-Object { $_.Value.ToLower().EndsWith('.psm1') });
            if ($Attributes.Count -eq 0) {
                $Attributes = @(@($ProjectXml.SelectNodes('/msb:Project/msb:ItemGroup/msb:Compile/@Include', $nsmgr)) | Where-Object { $_.Value.ToLower().EndsWith('.psm1') });
                if ($Attributes.Count -eq 0) {
                    $Attributes = @(@($ProjectXml.SelectNodes('/msb:Project/msb:ItemGroup/msb:None/@Include', $nsmgr)) | Where-Object { $_.Value.ToLower().EndsWith('.psm1') });
                }
            }
            if ($Attributes.Count -eq 1) {
                $ModuleManifest['RootModule'] = $Attributes[0].Value;
            } else {
                $ModuleManifest['RootModule'] = "$([System.IO.Path]::GetFileNameWithoutExtension($ModuleManifestPath)).dll";
            }
        }
    } else {
        if ($ModuleManifest['RootModule'] -eq $null) {
            $Attributes = @(@($ProjectXml.SelectNodes('/msb:Project/msb:ItemGroup/msb:Content/@Include', $nsmgr)) | Where-Object { $_.Value.ToLower().EndsWith('.psm1') });
            if ($Attributes.Count -eq 0) {
                $Attributes = @(@($ProjectXml.SelectNodes('/msb:Project/msb:ItemGroup/msb:Compile/@Include', $nsmgr)) | Where-Object { $_.Value.ToLower().EndsWith('.psm1') });
                if ($Attributes.Count -eq 0) {
                    $Attributes = @(@($ProjectXml.SelectNodes('/msb:Project/msb:ItemGroup/msb:None/@Include', $nsmgr)) | Where-Object { $_.Value.ToLower().EndsWith('.psm1') });
                }
            }
            if ($Attributes.Count -eq 1) {
                $ModuleManifest['RootModule'] = $Attributes[0].Value;
            } else {
                $ModuleManifest['RootModule'] = "$([System.IO.Path]::GetFileNameWithoutExtension($ModuleManifestPath)).psm1";
            }
        }
    }
    $RootModulePath = $ProjectRoot | Join-Path -ChildPath $ModuleManifest['RootModule'];
}

$XmlElement = $ProjectXml.SelectSingleNode('/msb:Project/msb:PropertyGroup/msb:Guid', $nsmgr);
$Guid = [System.Guid]::Empty;
if ($XmlElement -ne $null -and (-not $XmlElement.IsEmpty) -and [System.Guid]::TryParse($XmlElement.InnerText.Trim(), [ref]$Guid)) {
    $ModuleManifest['GUID'] = $Guid.ToString('d');
} else {
    if ($ModuleManifest['GUID'] -eq $null) {
        $XmlElement = $ProjectXml.SelectSingleNode('/msb:Project/msb:PropertyGroup/msb:ProjectGuid', $nsmgr);
        if ($XmlElement -ne $null -and (-not $XmlElement.IsEmpty) -and [System.Guid]::TryParse($XmlElement.InnerText.Trim(), [ref]$Guid)) {
            $ModuleManifest['GUID'] = $Guid.ToString('d');
        }
    }
}

[System.Reflection.Assembly]$Assembly = $null;
if ([System.IO.Path]::GetExtension($RootModulePath) -ieq '.dll' -and ($RootModulePath | Test-Path -PathType Leaf)) {
    [System.Reflection.Assembly]$Assembly = @(Add-Type -Path $RootModulePath)[0].Assembly;
}

$XmlElement = $ProjectXml.SelectSingleNode('/msb:Project/msb:PropertyGroup/msb:Version', $nsmgr);
[System.Version]$Version = $null;
if ($XmlElement -eq $null -or $XmlElement.IsEmpty -or -not [System.Version]::TryParse($XmlElement.InnerText.Trim(), [ref]$Version)) {
    if ($Assembly -eq $null) {
        if ($ModuleManifest['Version'] -eq $null -or -not [System.Version]::TryParse($ModuleManifest['Version'].Trim(), [ref]$Version)) {
            $Version = New-Object -TypeName 'System.Version' -ArgumentList 1, 0;
        }
    } else {
        $ModuleManifest['Version'] = $Assembly.GetName().Version.ToString();
    }
}

if ($Version -ne $null) {
    if ($Version.Revision -eq 0) {
        if ($Version.Build -eq 0) {
            $ModuleManifest['Version'] = $Version.ToString(2);
        } else {
            $ModuleManifest['Version'] = $Version.ToString(3);
        }
    } else {
        $ModuleManifest['Version'] = $Version.ToString();
    }
} else {
    if ($ModuleManifest['Version'] -eq $null) { $ModuleManifest['Version'] = '1.0' }
}
if ($ModuleManifest['GUID'] -eq $null) {
    if ($Assembly -eq $null) {
        throw 'Unable to determine GUID';
    }
    $a = @($Assembly.GetCustomAttributes([System.Runtime.InteropServices.GuidAttribute], $true));
    [System.Runtime.InteropServices.GuidAttribute]$g = $a[0];
    $Guid = [System.Guid]::Empty;
    if ($a.Count -gt 0 -and [System.Guid]::TryParse($a[0].Value.Trim(), [ref]$Guid)) {
        $ModuleManifest['GUID'] = $Guid.ToString('d');
    } else {
        throw 'Unable to determine GUID';
    }
}

@"
#
# Module manifest for module '$([System.IO.Path]::GetFileNameWithoutExtension($ModuleManifestPath))'
#
# Generated on: $([System.DateTime]::Now.ToString('yyyy-MM-dd HH:mm:ss'))
#
@{
# Script module or binary module file associated with this manifest.
RootModule = '$($ModuleManifest['RootModule'])'

# Version number of this module.
ModuleVersion = '$($ModuleManifest['Version'])'

# ID used to uniquely identify this module.
GUID = '$($ModuleManifest['GUID'])'

# Author of this module.
Author = 'Leonard T. Erwine'

# Company or vendor of this module
CompanyName = 'Leonard T. Erwine'

# Copyright statement for this module
Copyright = 'Copyright © Leonard T. Erwine 2017. All rights reserved.'

# Description of the functionality provided by this module
Description = 'Provides Text-To-Speech Functionality'

# Minimum version of the Windows PowerShell engine required by this module
# PowerShellVersion = ''

# Name of the Windows PowerShell host required by this module
# PowerShellHostName = ''

# Minimum version of the Windows PowerShell host required by this module
# PowerShellHostVersion = ''

# Minimum version of Microsoft .NET Framework required by this module
# DotNetFrameworkVersion = ''

# Minimum version of the common language runtime (CLR) required by this module
# CLRVersion = ''

# Processor architecture (None, X86, Amd64) required by this module
# ProcessorArchitecture = ''

# Modules that must be imported into the global environment prior to importing this module
# RequiredModules = @()

# Assemblies that must be loaded prior to importing this module
# RequiredAssemblies = @()

# Script files (.ps1) that are run in the caller's environment prior to importing this module.
# ScriptsToProcess = @()

# Type files (.ps1xml) to be loaded when importing this module
# TypesToProcess = @()

# Format files (.ps1xml) to be loaded when importing this module
# FormatsToProcess = @()

# Modules to import as nested modules of the module specified in RootModule/ModuleToProcess
# NestedModules = @()

# Functions to export from this module
FunctionsToExport = '*'

# Cmdlets to export from this module
CmdletsToExport = '*'

# Variables to export from this module
VariablesToExport = '*'

# Aliases to export from this module
AliasesToExport = '*'

# List of all modules packaged with this module
# ModuleList = @()

# List of all files packaged with this module
# FileList = @()

# Private data to pass to the module specified in RootModule/ModuleToProcess
# PrivateData = ''

# HelpInfo URI of this module
# HelpInfoURI = ''

# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
# DefaultCommandPrefix = ''

}
"@