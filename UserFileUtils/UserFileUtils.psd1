﻿@{

# Script module or binary module file associated with this manifest.
# RootModule = 'Module.psd1'
ModuleToProcess = 'UserFileUtils.psm1'

# Version number of this module.
ModuleVersion = '0.1.0.0'

# ID used to uniquely identify this module
GUID = '54b2b646-a773-4651-b466-4e91542f90b2'

# Author of this module
Author = 'Leonard T. Erwine'

# Company or vendor of this module
CompanyName = 'Leonard T. Erwine'

# Copyright statement for this module
Copyright = '(c) 2015 Leonard T. Erwine. All rights reserved.'

# Description of the functionality provided by this module
Description = 'Utility functions to manage files in user folders.'

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
RequiredAssemblies = @('System.Windows.Forms')

# Script files (.ps1) that are run in the caller's environment prior to importing this module.
# ScriptsToProcess = @()

# Type files (.ps1xml) to be loaded when importing this module
# TypesToProcess = @()

# Format files (.ps1xml) to be loaded when importing this module
# FormatsToProcess = @()

# Modules to import as nested modules of the module specified in RootModule/ModuleToProcess
# NestedModules = @()

# Functions to export from this module
FunctionsToExport = @( 'Get-SpecialFolderNames', 'Get-SpecialFolder', 'ConvertTo-SafeFileName', 'ConvertFrom-SafeFileName', 'Read-FilePath', 'Read-FolderPath', 
	'Get-MinBase64BlockSize', 'Read-IntegerFromStream', 'Read-LongIntegerFromStream', 'Read-LengthEncodedBytes', 'Write-IntegerToStream', 'Write-LongIntegerToStream', 
	'Write-LengthEncodedBytes', 'New-DataBuffer', 'ConvertTo-Base64String', 'ConvertFrom-Base64String', 'Read-DataBuffer', 'Write-DataBuffer', 'Get-TextEncoding',
	'New-XmlReaderSettings', 'New-XmlWriterSettings', 'Read-XmlDocument', 'ConvertTo-XmlString', 'ConvertFrom-XmlString', 'Get-XmlAttributeValue', 'Set-XmlAttribute',
	'Append-XmlElement', 'New-XmlDocument', 'Write-XmlDocument' )

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
