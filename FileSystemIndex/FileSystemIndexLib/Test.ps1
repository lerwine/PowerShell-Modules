if (({
    $BasePath = $args[0] | Split-Path -Parent;
    $SourceFiles = @((Get-ChildItem -Path $BasePath -Filter '*.cs') | ForEach-Object { $_.FullName });
    $Assemblies = @(('System', 'System.Core', 'System.Xml', 'System.Management.Automation') | ForEach-Object { [System.Reflection.Assembly]::LoadWithPartialName($_).Location });
    Add-Type -Path $SourceFiles -CompilerParameters (New-Object -TypeName 'System.CodeDom.Compiler.CompilerParameters' -ArgumentList (,$Assemblies) -Property @{
        IncludeDebugInformation = $true
        CompilerOptions = '/define:TRACE;DEBUG';
    }) -ErrorAction Stop;
    $SourceFiles | Write-Output;
}).Invoke($MyInvocation.MyCommand.Definition).Count -eq 0) { return }

Function Get-PathDiff {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        $OriginalPath,
        
        [Parameter(Mandatory = $true, Position = 1)]
        $NewPath
    )

    if (($OriginalPath | Test-Path -PathType Container)) {
        if (($NewPath | Test-Path -PathType Container)) {
            $OriginalList = New-Object -TypeName 'System.Collections.Generic.List[System.String]';
            Get-ChildItem -Path $OriginalPath | ForEach-Object { $OriginalList.Add($_.Name) };
            $OriginalList.Sort([System.StringComparer]::InvariantCultureIgnoreCase);
            $NewList = New-Object -TypeName 'System.Collections.Generic.List[System.String]';
            Get-ChildItem -Path $NewPath | ForEach-Object { $NewList.Add($_.Name) };
            $NewList.Sort([System.StringComparer]::InvariantCultureIgnoreCase);
            $UnifiedFileDiff = New-Object -TypeName 'FileSystemIndexLib.UnifiedFileDiff';
            $UnifiedFileDiff.OriginalFile = $OriginalPath;
            $UnifiedFileDiff.NewFile = $NewPath;
        } else {
        }
    } else {
        if (($NewPath | Test-Path -PathType Container)) {
        } else {
        }
    }
}

$CommonSequenceSearcher1 = New-Object -TypeName 'FileSystemIndexLib.CommonSequenceSearcher[string]' -ArgumentList (,[string[]]@('line1', 'line2', 'line3', 'line4', 'line5'));
$CommonSequenceSearcher2 = New-Object -TypeName 'FileSystemIndexLib.CommonSequenceSearcher[string]' -ArgumentList (,[string[]]@('line1', 'line3', 'line4', 'line5' ));

$currentIndex = 0;
$otherIndex = 0;
$Length = $CommonSequenceSearcher1.GetLongestCommonSequence($CommonSequenceSearcher2, $Host.UI, [ref]$currentIndex, [ref]$otherIndex);
"Length = $Length; currentIndex = $currentIndex; otherIndex = $otherIndex"

