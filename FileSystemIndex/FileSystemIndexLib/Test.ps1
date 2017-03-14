Function Initialize-CurrentModule {
    [CmdletBinding()]
    Param()
	
	$Local:BaseName = $PSScriptRoot | Join-Path -ChildPath $MyInvocation.MyCommand.Module.Name;
	
    $Local:ModuleManifest = Test-ModuleManifest -Path ($PSScriptRoot | Join-Path -ChildPath ('{0}.psd1' -f $MyInvocation.MyCommand.Module.Name));
}

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

$CommonSequenceSearcher1 = New-Object -TypeName 'FileSystemIndexLib.CommonSequenceSearcher[string]' -ArgumentList (,[string[]]@('line1', 'line2', 'line3', 'line4', 'line5'));
$CommonSequenceSearcher2 = New-Object -TypeName 'FileSystemIndexLib.CommonSequenceSearcher[string]' -ArgumentList (,[string[]]@('line1', 'line3', 'line4', 'line5' ));

$currentIndex = 0;
$otherIndex = 0;
$Length = $CommonSequenceSearcher1.GetLongestCommonSequence($CommonSequenceSearcher2, $Host.UI, [ref]$currentIndex, [ref]$otherIndex);
"Length = $Length; currentIndex = $currentIndex; otherIndex = $otherIndex"

