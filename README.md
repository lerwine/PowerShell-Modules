# Powershell-Modules

This project is a collection of various PowerShell modules, which I have been developing during personal studies and research.
Please reference the appropriate version-named branch for versions which are deemed releasable.

The PowerShell modules contained in this project were targeted for PowerShell Core 7.0 or later.

## Contents

### CertificateCryptography

Encrypt and decrypt using PKI technology.

### CredentialStorage

Provides secure credential storage and retrieval.

### GDIPlus

Binary Module to support graphic file manipulation.

### IOUtility

Utility functions to manage filesystem, console, and stream IO.

### LteDev

PowerShell development utility functions.

### NetworkUtility

### PSDB

Database command utilities

### XmlUtility

Provides advanced XML functionality.

## Requirements

PowerShell
: PowerShell Core 7.0+

## Dev Environment Setup

### Development Dependencies

To compile using Visual Studio Code or the build.ps1 script, will need install the .NET Core 8.0 SDK, which can be obtained from <https://www.microsoft.com/net/download/visual-studio-sdks>.

### Visual Studio Code

This project was recently converted from being developed from Visual Studio Ultimate to Visual Studio Code (<https://code.visualstudio.com/download>).
Additional environment configuration may be necessary for development, or to compile the projects.

#### Extensions

The "C# for Visual Studio Code (powered by OmniSharp)" extension is required for development.

This extension, as well as others which are recommended for development, are listed in the .vscode/extensions.json file, and
should appear in the Recommended extension listing in Visual Studio Code. Alternatively, you can refer to <https://sites.google.com/erwinefamily.net/vscodedevcheatsheet> for an example script that you can use in the terminal window to install multiple extensions.

To compile without Visual Studio, you will also need .

## Contributing

This Git script library is maintained by Leonard T. Erwine. If you wish to contribute to this project, simply edit a file and propose a change, or propose a new file at [My Public GitHub Website](<https://github.com/lerwine/PowerShell-Modules.git>).