#
# Module manifest for module 'Erwine.Leonard.T.Speech'
#
# Generated on: 2017-04-21 19:45:32
#
@{
# Script module or binary module file associated with this manifest.
RootModule = 'Erwine.Leonard.T.Speech.dll'

# Version number of this module.
ModuleVersion = '0.1'

# ID used to uniquely identify this module.
GUID = '12c6248d-575e-4d2a-b90a-a7bf8fbcd904'

# Author of this module.
Author = 'Leonard T. Erwine'

# Company or vendor of this module
CompanyName = 'Leonard T. Erwine'

# Copyright statement for this module
Copyright = 'Copyright © Leonard T. Erwine 2017. All rights reserved.'

# Description of the functionality provided by this module
Description = 'Provides Text-To-Speech Functionality'

# Minimum version of the Windows PowerShell engine required by this module
PowerShellVersion = '4.0'

# Name of the Windows PowerShell host required by this module
# PowerShellHostName = ''

# Minimum version of the Windows PowerShell host required by this module
# PowerShellHostVersion = ''

# Minimum version of Microsoft .NET Framework required by this module
DotNetFrameworkVersion = '4.0'

# Minimum version of the common language runtime (CLR) required by this module
CLRVersion = '4.0'

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
CmdletsToExport = @('ConvertTo-PromptBuilder', 'ConvertTo-Ssml', 'Get-TextToSpeechStatus', 'Import-SpokenText', 'Resume-TextToSpeech', 'Start-TextToSpeech', 'Stop-TextToSpeech', 'Suspend-TextToSpeech', 'Wait-TextToSpeech')

# Variables to export from this module
VariablesToExport = '*'

# Aliases to export from this module
AliasesToExport = '*'

# List of all modules packaged with this module
# ModuleList = @()

# List of all files packaged with this module
# FileList = @()

# Private data to pass to the module specified in RootModule/ModuleToProcess
PrivateData = @{
	PSData = @{
		# Tags applied to this module. These help with module discovery in online galleries.
		# Tags = @()

		# A URL to the license for this module.
		LicenseUri = 'https://github.com/lerwine/PowerShell-Modules/blob/Work/LICENSE'

		# A URL to the main website for this project.
		ProjectUri = 'https://github.com/lerwine/PowerShell-Modules'

		# A URL to an icon representing this module.
		# IconUri = ''

		# ReleaseNotes of this module
		# ReleaseNotes = ''

	} # End of PSData hashtable
} # End of PrivateData hashtable

# HelpInfo URI of this module
# HelpInfoURI = ''

# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
# DefaultCommandPrefix = ''

}