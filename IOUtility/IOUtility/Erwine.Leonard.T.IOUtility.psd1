@{

# Script module or binary module file associated with this manifest.
ModuleToProcess = 'Erwine.Leonard.T.IOUtility.psm1'

# Version number of this module.
ModuleVersion = '0.3'

# ID used to uniquely identify this module
GUID = 'bd4390dc-a8ad-4bce-8d69-f53ccf8e4163'

# Author of this module
Author = 'Leonard T. Erwine'

# Company or vendor of this module
CompanyName = 'Leonard T. Erwine'

# Copyright statement for this module
Copyright = '(c) 2016 Leonard T. Erwine. All rights reserved.'

# Description of the functionality provided by this module
Description = 'Utility functions to manage filesystem and stream IO.'

# Minimum version of the Windows PowerShell engine required by this module
PowerShellVersion = '2.0'

# Name of the Windows PowerShell host required by this module
# PowerShellHostName = ''

# Minimum version of the Windows PowerShell host required by this module
# PowerShellHostVersion = ''

# Minimum version of Microsoft .NET Framework required by this module
DotNetFrameworkVersion = '2.0'

# Minimum version of the common language runtime (CLR) required by this module
CLRVersion = '2.0'

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
FunctionsToExport = @( 'Get-SpecialFolderNames', 'Get-SpecialFolder', 'ConvertTo-SafeFileName', 'ConvertFrom-SafeFileName', 'Get-AppDataPath',
	'Read-FileDialog', 'Get-MinBase64BlockSize', 'Read-IntegerFromStream', 'Read-LongIntegerFromStream', 'Write-IntegerToStream',
	'Write-LongIntegerToStream', 'Read-LengthEncodedBytes', 'Write-LengthEncodedBytes', 'ConvertTo-Base64String', 'ConvertFrom-Base64String',
    'Get-TextEncoding', 'New-MemoryStream', 'Test-IsNullOrWhitespace', 'Split-DelimitedText', 'Out-NormalizedText', 'Out-IndentedText',
    'Get-IndentLevel', 'Out-UnindentedText' )

# Cmdlets to export from this module
CmdletsToExport = '*'

# Variables to export from this module
VariablesToExport = '*'

# Aliases to export from this module
AliasesToExport = '*'

# List of all modules packaged with this module
# ModuleList = @()

# List of all files packaged with this module
FileList = @('DecodeRegexReplaceHandler.cs', 'EncodeRegexReplaceHandler.cs', 'LinqEmul.cs', 'RegexReplaceHandler.cs', 'RegularExpressions.cs', 'SchemaSetCollection.cs',
	'SchemaValidationError.cs', 'SchemaValidationHandler.cs', 'ScriptRegexReplaceHandler.cs', 'StreamHelper.cs', 'TextHelper.cs')

# Private data to pass to the module specified in RootModule/ModuleToProcess
# PrivateData = ''

# HelpInfo URI of this module
# HelpInfoURI = ''

# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
# DefaultCommandPrefix = ''

}
