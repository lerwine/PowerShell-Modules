if (({
    $BasePath = $args[0] | Split-Path -Parent;
    $SourceFiles = @((Get-ChildItem -Path $BasePath -Filter '*.cs') | ForEach-Object { $_.FullName });
    $Assemblies = @(('System', 'System.Core', 'System.Xml', 'System.Management.Automation') | ForEach-Object { [System.Reflection.Assembly]::LoadWithPartialName($_).Location });
    Add-Type -Path $SourceFiles -CompilerParameters (New-Object -TypeName 'System.CodeDom.Compiler.CompilerParameters' -ArgumentList (,$Assemblies) -Property @{
        IncludeDebugInformation = $true
        CompilerOptions = '/define:TRACE;DEBUG';
    }) -ErrorAction Stop;
    $SourceFiles | Write-Output;
}).Invoke($MyInvocation.MyCommand.Definition) -eq $null) { return }

$FileSystemDiff = New-Object -TypeName 'FileSystemIndexLib.FileSystemDiff' -ArgumentList 'C:\Users\leonarde\Documents\WindowsPowerShell\PowerShell-Modules\PowerShell-Modules-Work', 'C:\Users\leonarde\Documents\WindowsPowerShell\PowerShell-Modules\previous';
