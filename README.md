# Powershell-Modules #

This project is a collection of various PowerShell modules, which I have been developing during personal studies and research.
Please reference the appropriate version-named branch for versions which are deemed releasable.

This is a development branch, not intended to be releasable.

The modules in this branch have been targeted for PowerShell 4.0 or greater.
Branches which start with the name 'legacy', are intended to be compatible with systems which only have PowerShell 2.0 and supports only c# 2.0 for custom types and assemblies.

## Contents ##

### CertificateCryptography ###

Encrypt and decrypt using PKI technology.

### CredentialStorage ###

Provides secure credential storage and retrieval.

### CryptographyScripts ###

Powershell scripts to support encrypting and decrypting data.

### GDIPlus ###

Binary Module to support graphic file manipulation.

### IOUtility ###

Utility functions to manage filesystem, console, and stream IO.

### LteDev ###

PowerShell development utility functions.

### LteUtils ###

System and web admin utility functions.

### NetworkUtility ###

Utility functions for network access.

### WindowsForms ###

Script Module to support windows forms in Powershell.

### WinForms ###

Module providing GUI components.

### WPF ###

Display WPF windows.

### XmlUtility ###

Provides advanced XML functionality.

## Dev Environment Setup ##

### Compiling manually or with VS Code ###

To compile using Visual Studio Code or the build.ps1 script, will need install the .NET Framework 4.6 SDK Developer Pack, which can be obtained from <https://www.microsoft.com/net/download/visual-studio-sdks>. You will also
need to install Microsoft Build Tools (<https://www.microsoft.com/en-us/download/details.aspx?id=48159>).

### PowerShell 4.0+ ###

The PowerShell modules contained in this project were targeted for PowerShell 4.0 or later.

### Visual Studio Code ###

This project was recently converted from being developed from Visual Studio Ultimate to Visual Studio Code (<https://code.visualstudio.com/download>).
Additional environment configuration may be necessary for development, or to compile the projects.

#### Extensions ####

The "C# for Visual Studio Code (powered by OmniSharp)" extension is required for development.

This extension, as well as others which are recommended for development, are listed in the .vscode/extensions.json file, and
should appear in the Recommended extension listing in Visual Studio Code. Alternatively, you can refer to <https://sites.google.com/erwinefamily.net/vscodedevcheatsheet> for an example script that you can use in the terminal window to install multiple extensions.

To compile without Visual Studio, you will also need .

## Contributing ##

This Git script library is maintained by Leonard T. Erwine. If you wish to contribute to this project, simply edit a file and propose a change, or propose a new file at [My Public GitHub Website](https://github.com/lerwine/PowerShell-Modules.git).