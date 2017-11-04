Function Install-AssemblyModule {
    <#
        .SYNOPSIS
            Compiles and installs assembly
    #>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [ValidateScript({ $_ | Test-Path -PathType Leaf })]
        # Source code files to compile.
        [string]$Compile,

        [Parameter(Mandatory = $true)]
        [ValidateScript({ ($_ | Split-Path -Parent | Test-Path -PathType Container) -and $_.ToLower().EndsWith('.dll') })]
        # Output path of assembly.
        [string]$OutputAssembly,
        
        [ValidatePattern('^[a-zA-Z_][a-zA-Z_\d]*$')]
        # Preprocessor symbols to define.
        [string[]]$Define,
        
        [ValidateScript({ ($_ | Test-Path -PathType Leaf) -and $_.ToLower().EndsWith('.snk') })]
        # Filename containing the cryptographic key for signing.
        [string]$KeyFile,
        
        [ValidateScript({ $_ | Test-Path -PathType Leaf })]
        # Assemblies referenced by the source code.
        [string[]]$ReferencedAssemblies,
        
        [ValidateScript({ $_ | Test-Path -PathType Leaf })]
        # .NET Framework resource files to include when compiling the assembly output.
        [string[]]$EmbeddedResources,

        # Create XML file from processed source code documentation comments.
        [switch]$XmlDoc,

        # Include debug information in the compiled executable.
        [switch]$IncludeDebugInformation
    )
    
    Begin { $SourceFiles = @() }
    Process { $SourceFiles += $Compile }
    End {
        $Local:TempPath = ($OutputAssembly | Split-Path -Parent) | Join-Path -ChildPath 'bin';
        if (-not ($Local:TempPath | Test-Path -PathType Container)) { New-Item -Path $Local:TempPath -ItemType Directory | Out-Null }
        $Local:Splat = @{
            TypeName = 'System.CodeDom.Compiler.CompilerParameters';
            Property = @{
                IncludeDebugInformation = $IncludeDebugInformation.IsPresent;
                TempFiles = New-Object -TypeName 'System.CodeDom.Compiler.TempFileCollection' -ArgumentList $Local:TempPath, $true;
                CompilerOptions = @{
                    define = @();
                    target = 'library';
                }
                OutputAssembly = $OutputAssembly
            }
        };
        try {
            if ($PSBoundParameters.ContainsKey('ReferencedAssemblies')) { $Local:Splat['ArgumentList'] = @(,$ReferencedAssemblies); }
            if ($PSBoundParameters.ContainsKey('Define')) {
                $Local:Splat['Property']['CompilerOptions']['define'] = $Define;
            }
            if ($IncludeDebugInformation) {
                $Local:Splat['Property']['CompilerOptions']['pdb'] = '"' + (($OutputAssembly | Split-Path -Parent) | Join-Path -ChildPath ([System.IO.Path]::GetFileNameWithoutExtension($OutputAssembly) + '.pdb')) + '"';
                $Local:Splat['Property']['CompilerOptions']['debug'] = $null;
                if ($Local:Splat['Property']['CompilerOptions']['define'] -cnotcontains 'DEBUG') {
                    $Local:Splat['Property']['CompilerOptions']['define'] += 'DEBUG'
                }
            }
            if ($XmlDoc) { $Local:Splat['Property']['CompilerOptions']['doc'] = '"' + (($OutputAssembly | Split-Path -Parent) | Join-Path -ChildPath ([System.IO.Path]::GetFileNameWithoutExtension($OutputAssembly) + '.xml')) + '"' }
            if ($Local:Splat['Property']['CompilerOptions']['define'].Count -eq 0) {
                $Local:Splat['Property']['CompilerOptions'].Remove('define');
            } else {
                $Local:Splat['Property']['CompilerOptions']['define'] = $Local:Splat['Property']['CompilerOptions']['define'] -join ';';
            }
            $Local:Splat['Property']['CompilerOptions'] = ($Local:Splat['Property']['CompilerOptions'].Keys | ForEach-Object {
                if ($Local:Splat['Property']['CompilerOptions'][$_] -eq $null) {
                    '/' + $_;
                } else {
                    '/' + $_ + ':' + $Local:Splat['Property']['CompilerOptions'][$_];
                }
            }) -join ' ';
            $CompilerParameters = New-Object @Local:Splat;
            if ($PSBoundParameters.ContainsKey('EmbeddedResources')) {
                $EmbeddedResources | ForEach-Object { $CompilerParameters.EmbeddedResources.Add($_) }
            }
            $OutputLocation = @(Add-Type -Path $SourceFiles -CompilerParameters $CompilerParameters -PassThru)[0].Assembly.Location;
            if ($OutputAssembly -ne $OutputLocation) {
                Copy-Item -Path $OutputLocation -Destination $OutputAssembly -Force;
            }
        } finally { $Local:Splat['Property']['TempFiles'].Dispose() }
    }
}

Add-Type -Path @(
    'C:\Users\lerwi\Documents\Visual Studio 2015\Projects\PowerShell-Modules\PSModuleInstallUtil\InstallationLocationInfo.cs',
    'C:\Users\lerwi\Documents\Visual Studio 2015\Projects\PowerShell-Modules\PSModuleInstallUtil\InstallLocationSelectForm.cs',
    'C:\Users\lerwi\Documents\Visual Studio 2015\Projects\PowerShell-Modules\PSModuleInstallUtil\InstallLocationSelectForm.Designer.cs'
) -ReferencedAssemblies 'System', 'System.Drawing', 'System.Windows.Forms';


Add-Type -AssemblyName 'System.Web.Services'
$Form = New-Object -TypeName 'PSModuleInstallUtil.InstallLocationSelectForm';
try {
    $InstallationLocationInfo = New-Object -TypeName 'PSModuleInstallUtil.InstallationLocationInfo' -Property @{
        Name = 'MyModule';
        ParentDirectory = 'C:\Users\lerwi\Documents\WindowsPowerShell\Modules';
        RelativePath = '\Users\lerwi\Documents\WindowsPowerShell\Modules';
        Reason = '';
        Exists = $false;
        IsInstallable = $true;
        IsAllUsers = $false;
        ExpectDirectory = $true;
    };
    $Form.AddInstallationLocationInfo($InstallationLocationInfo);
    $InstallationLocationInfo = New-Object -TypeName 'PSModuleInstallUtil.InstallationLocationInfo' -Property @{
        Name = 'MyModule';
        ParentDirectory = 'C:\Program Files\WindowsPowerShell\Modules';
        RelativePath = '\Program Files\WindowsPowerShell\Modules';
        Reason = 'File found where directory expected.';
        Exists = $false;
        IsInstallable = $false;
        IsAllUsers = $true;
        ExpectDirectory = $true;
    };
    $Form.AddInstallationLocationInfo($InstallationLocationInfo);
    $InstallationLocationInfo = New-Object -TypeName 'PSModuleInstallUtil.InstallationLocationInfo' -Property @{
        Name = 'MyModule';
        ParentDirectory = 'C:\Windows\system32\WindowsPowerShell\v1.0\Modules';
        RelativePath = '\Windows\system32\WindowsPowerShell\v1.0\Modules';
        Reason = '';
        Exists = $true;
        IsInstallable = $true;
        IsAllUsers = $true;
        ExpectDirectory = $true;
    };
    $Form.AddInstallationLocationInfo($InstallationLocationInfo);
    $DialogResult = $Form.ShowDialog();
} catch {
    throw;
} finally {
    $Form.Dispose();
}

