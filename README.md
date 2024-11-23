# Powershell-Modules

This project is a collection of various PowerShell modules, which I have been developing during personal studies and research.
Please reference the appropriate version-named branch for versions which are deemed releasable.

The PowerShell modules contained in this project were targeted for PowerShell Core 7.0 or later.

## Modules

- [GDIPlus](dist/Erwine.Leonard.T.GDIPlus/README.md): Module providing GDI Plus Graphics capabilities.
- [Htmltility](dist/Erwine.Leonard.T.HtmlUtility/README.md): PowerShell HTML utility functions.
- [IOUtility](dist/Erwine.Leonard.T.IOUtility/README.md): General PowerShell I/O utility functions.
- [LteDev](dist/Erwine.Leonard.T.LteDev/README.md): PowerShell development utility functions.
- [SwPackage](dist/Erwine.Leonard.T.SwPackage/README.md): Functions for working with software packages.
- [WinIOUtility](dist/Erwine.Leonard.T.WinIOUtility/README.md): PowerShell I/O utility functions for windows-based systems.
- [XmlUtility](dist/Erwine.Leonard.T.XmlUtility/README.md): Provides XML functions for advanced manipulation and formatting of XML.

### Unpublished Modules

These are incomplete modules which are not yet ready to be published.

- [CredentialStorage](src/CredentialStorage/README.md): Provides password storage and retrieval functionality.
- [MsExcelUtil](src/MsExcelUtil/README.md): Provides commands for working with Microsoft Excel files.
- [NetworkUtility](src/NetworkUtility/README.md): PowerShell Network utility functions.
- [PSDB](src/PSDB/README.md): In-memory database provider for PowerShell.

## Building and Publishing

The term "publishing" refers to the PowerShell module being published to the `dist` folder of this repository.

Tasks for building and publishing modules are defined in [.vscode/tasks.json](./.vscode/tasks.json).
These tasks use the [Deploy.ps1](./Deploy.ps1) to copy the necessary files to the corresponding subdirectory under the `dist` folder.
The solution file for all binary modules is at [src/PowerShellModules.sln](./src/PowerShellModules.sln).

## Testing

This solution uses [Pester](https://pester.dev/)-based unit testing. Pester tests are distributed along with the PowerShell modules.
Binary modules may also have [NUnit](https://nunit.org/) tests for binary module components.
The [.vscode/launch.json](./.vscode/launch.json) contains configurations for executing `Pester` unit tests
and the [.vscode/tasks.json](./.vscode/tasks.json) contains tasks for executing the `NUnit` tests.

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

This Git script library is maintained by Leonard T. Erwine. If you wish to contribute to this project, simply edit a file and propose a change, or propose a new file at [My Public GitHub Website](<https://github.com/lerwine/PowerShell-Modules>).
