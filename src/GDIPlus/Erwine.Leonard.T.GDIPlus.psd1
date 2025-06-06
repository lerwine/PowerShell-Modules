@{
# Script module or binary module file associated with this manifest.
RootModule = 'Erwine.Leonard.T.GDIPlus.psm1'

# Version number of this module.
ModuleVersion = '0.2'

# ID used to uniquely identify this module
GUID = '17e9a105-e515-4b0a-bc94-527661128625'

# Author of this module
Author = 'Leonard T. Erwine'

# Company or vendor of this module
CompanyName = 'Leonard T. Erwine'

# Copyright statement for this module
Copyright = '(c) 2014 Leonard T. Erwine. All rights reserved.'

# Description of the functionality provided by this module
Description = 'Module providing GDI Plus Graphics capabilities.'

# Minimum version of the Windows PowerShell engine required by this module
PowerShellVersion = '4.0'

# Name of the Windows PowerShell host required by this module
# PowerShellHostName = ''

# Minimum version of the Windows PowerShell host required by this module
# PowerShellHostVersion = ''

# Minimum version of Microsoft .NET Framework required by this module
DotNetFrameworkVersion = '4.6'

# Minimum version of the common language runtime (CLR) required by this module
CLRVersion = '4.0'

# Processor architecture (None, X86, Amd64) required by this module
ProcessorArchitecture = 'MSIL'

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
NestedModules = @('GDIPlus.dll')

# Functions to export from this module
FunctionsToExport = '*'

# Cmdlets to export from this module
CmdletsToExport = @('ConvertTo-CroppedImage', 'ConvertTo-ResizedImage', 'Copy-Image', 'Enter-DrawingSurface', 'Exit-DrawingSurface', 'New-Image', 'Open-Image', 'Sync-DrawingSurface', 'Convert-HsbToRgb', 'Convert-RgbToHsb')

# Variables to export from this module
VariablesToExport = '*'

# Aliases to export from this module
AliasesToExport = '*'

# DSC resources to export from this module
# DscResourcesToExport = @()

# List of all modules packaged with this module
# ModuleList = @()

# List of all files packaged with this module
FileList = @('README.md', 'Palette/README.md', 'Palette/LICENSE.htm', 'about_Erwine.Leonard.T.GDIPlus.help.txt')

# Private data to pass to the module specified in RootModule/ModuleToProcess. This may also contain a PSData hashtable with additional module metadata used by PowerShell.
PrivateData = @{

    PSData = @{

        # Tags applied to this module. These help with module discovery in online galleries.
        Tags = 'GDI', 'Image Quantizer'

        # A URL to the license for this module.
        LicenseUri = 'https://github.com/lerwine/PowerShell-Modules/tree/master/LICENSE'

        # A URL to the main website for this project.
        ProjectUri = 'https://github.com/lerwine/PowerShell-Modules/tree/master/GDIPlus'

        # A URL to an icon representing this module.
        # IconUri = ''

        # ReleaseNotes of this module
        # ReleaseNotes = ''

    } # End of PSData hashtable

} # End of PrivateData hashtable

# HelpInfo URI of this module
HelpInfoURI = 'https://github.com/lerwine/PowerShell-Modules/tree/master/GDIPlus'

# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
# DefaultCommandPrefix = ''

}