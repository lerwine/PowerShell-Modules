#
# Module manifest for module 'Erwine.Leonard.T.HtmlUtility'
#
# Generated by: lerwine
#
# Generated on: 10/30/2024
#

@{

# Script module or binary module file associated with this manifest.
RootModule = 'Erwine.Leonard.T.HtmlUtility.psm1'

# Version number of this module.
ModuleVersion = '0.0.1'

# Supported PSEditions
# CompatiblePSEditions = @()

# ID used to uniquely identify this module
GUID = '16fc5ba8-6161-4fab-a951-ecd286e47267'

# Author of this module
Author = 'Leonard T. Erwine'

# Company or vendor of this module
CompanyName = 'Leonard T. Erwine'

# Copyright statement for this module
Copyright = '(c) 2024 Leonard Thomas Erwine. All rights reserved.'

# Description of the functionality provided by this module
Description = 'Provides utility functions that are useful for parsing and modifying HTML.'

# Minimum version of the PowerShell engine required by this module
# PowerShellVersion = ''

# Name of the PowerShell host required by this module
# PowerShellHostName = ''

# Minimum version of the PowerShell host required by this module
# PowerShellHostVersion = ''

# Minimum version of Microsoft .NET Framework required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
# DotNetFrameworkVersion = ''

# Minimum version of the common language runtime (CLR) required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
# ClrVersion = ''

# Processor architecture (None, X86, Amd64) required by this module
# ProcessorArchitecture = ''

# Modules that must be imported into the global environment prior to importing this module
# RequiredModules = @('Erwine.Leonard.T.IOUtility')

# Assemblies that must be loaded prior to importing this module
# RequiredAssemblies = @()

# Script files (.ps1) that are run in the caller's environment prior to importing this module.
# ScriptsToProcess = @()

# Type files (.ps1xml) to be loaded when importing this module
# TypesToProcess = @()

# Format files (.ps1xml) to be loaded when importing this module
# FormatsToProcess = @()

# Modules to import as nested modules of the module specified in RootModule/ModuleToProcess
NestedModules = @('HtmlUtility.dll')

# Functions to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no functions to export.
FunctionsToExport = @('New-MarkdownPipeline', 'Convert-TextToMarkdownDocument', 'Open-MarkdownDocument', 'Get-MarkdownDescendants')

# Cmdlets to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no cmdlets to export.
CmdletsToExport = @('ConvertFrom-HtmlString', 'ConvertFrom-HtmlDocument', 'Select-HtmlNode', 'Select-MarkdownObject')

# Variables to export from this module
VariablesToExport = '*'

# Aliases to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no aliases to export.
AliasesToExport = @()

# DSC resources to export from this module
# DscResourcesToExport = @()

# List of all modules packaged with this module
# ModuleList = @()

# List of all files packaged with this module
FileList = @('MarkdownFunctions.ps1', 'Resources/from-md.css', 'Resources/highlight.css', 'Resources/katex-copytex.min.css', 'Resources/katex-copytex.min.js', 'Resources/katex.min.css',
    'Resources/markdown.css', 'Resources/vscode-github.css', 'about_Erwine.Leonard.T.HtmlUtility.help.txt', 'README.md', 'ConvertFrom-HtmlDocument.tests.ps1',
    'ConvertFrom-HtmlString.tests.ps1', 'Select-HtmlNode.tests.ps1', 'Select-MarkdownObject.tests.ps1')

# Private data to pass to the module specified in RootModule/ModuleToProcess. This may also contain a PSData hashtable with additional module metadata used by PowerShell.
PrivateData = @{

    PSData = @{

        # Tags applied to this module. These help with module discovery in online galleries.
        # Tags = @()

        # A URL to the license for this module.
		LicenseUri = 'https://github.com/lerwine/PowerShell-Modules/blob/master/LICENSE'

        # A URL to the main website for this project.
		ProjectUri = 'https://github.com/lerwine/PowerShell-Modules/tree/master/HtmlUtility'

        # A URL to an icon representing this module.
        # IconUri = ''

        # ReleaseNotes of this module
        # ReleaseNotes = ''

        # Prerelease string of this module
        # Prerelease = ''

        # Flag to indicate whether the module requires explicit user acceptance for install/update/save
        # RequireLicenseAcceptance = $false

        # External dependent modules of this module
        # ExternalModuleDependencies = @()

    } # End of PSData hashtable

} # End of PrivateData hashtable

# HelpInfo URI of this module
# HelpInfoURI = ''

# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
# DefaultCommandPrefix = ''

}

