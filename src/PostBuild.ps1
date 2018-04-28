Param(
#    [Parameter(Mandatory = $true, Position = 0)]
    [string]$TargetName = 'Erwine.Leonard.T.GDIPlusLib',

#    [Parameter(Mandatory = $true, Position = 1)]
    [string]$TargetExt = '.dll',

#    [Parameter(Mandatory = $true, Position = 2)]
    [string]$ProjectName = 'GDIPlus',

#    [Parameter(Mandatory = $true, Position = 3)]
    [string]$ProjectDir = 'C:\Users\lerwi\GitHub\PowerShell-Modules\src\GDIPlus\',

#    [Parameter(Mandatory = $true, Position = 4)]
    [string]$OutDir = 'bin\Debug\',

#    [Parameter(Mandatory = $true, Position = 5)]
    [string]$SolutionDir = 'C:\Users\lerwi\GitHub\PowerShell-Modules\src\'
)
     
$ErrorActionPreference = [System.Management.Automation.ActionPreference]::Stop;
$WarningPreference = [System.Management.Automation.ActionPreference]::Continue;
$InformationPreference = [System.Management.Automation.ActionPreference]::Continue;

Function Normalize-Path {
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [string]$Path
    )

    Process {
        $n = $Path | Split-Path -Leaf;
        $d = $Path | Split-Path -Parent;
        if ($n.Length -eq 0) { return $d }
        ($d | Join-Path -ChildPath $n) | Write-Output;
    }
}
$OutPath = ((Join-Path -Path $ProjectDir -ChildPath $OutDir) | Resolve-Path).Path | Normalize-Path;
$DistPath = ((($SolutionDir | Join-Path -ChildPath '../dist') | Resolve-Path).Path | Join-Path -ChildPath $ProjectName) | Normalize-Path;
Write-Information -MessageData "'$TargetName', '$TargetExt', '$ProjectName', '$ProjectDir', '$OutDir' '$SolutionDir' Copying $OutPath to $DistPath";
$PsdPath = Join-Path -Path $OutPath -ChildPath "$TargetName.psd1";
if (-not ($PsdPath | Test-Path -PathType Leaf)) {
    $allPsds = @($OutPath | Get-ChildItem -Filter "*.psd1");
    $slnMatch = @($allPsds | Where-Object { $_.BaseName.EndsWith($ProjectName) });
    if ($allPsds.Count -eq 1) {
        $PsdPath = $allPsds[0].FullName;
    } else {
        if ($slnMatch.Count -gt 0) {
            $PsdPath = $slnMatch[0].FullName;
        } else {
            throw "Module manifest not found at $PsdPath.";
        }
    }
}
$ModuleManifest = Test-ModuleManifest -Path $PsdPath;

$FileList = @($PsdPath, ($OutPath | Join-Path -ChildPath $ModuleManifest.RootModule), (Join-Path -Path $OutPath -ChildPath "$TargetName$TargetExt")) + @($ModuleManifest.FileList) + @($ModuleManifest.NestedModules) + @($ModuleManifest.Scripts) + @($ModuleManifest.ScriptsToProcess) + @($ModuleManifest.ExportedFormatFiles) + @($ModuleManifest.ExportedTypeFiles);

$Path = Join-Path -Path $OutPath -ChildPath "$TargetName.pdb";
if ($Path | Test-Path -PathType Leaf) { $FileList = $FileList + @($Path); }
$Path = Join-Path -Path $OutPath -ChildPath "$TargetName.XML";
if ($Path | Test-Path -PathType Leaf) { $FileList = $FileList + @($Path); }
if ($DistPath | Test-Path -PathType Container) { Remove-Item -Path $DistPath -Recurse -Force }
(New-Item -ItemType Directory -Path $DistPath) | Out-Null;
$FileList = @($FileList | ForEach-Object { if ($_ -ne $null -and $_.Length -gt 0) { ($_ | Resolve-Path).Path } } | Select-Object -Unique);
foreach ($SourcePath in $FileList) {
    if (-not ($SourcePath | Test-Path -PathType Leaf)) { throw "$SourcePath not found." }
    $ParentDir = $SourcePath | Split-Path -Parent;
    $FileName = $SourcePath | Split-Path -Leaf;
    $SubDir = @();
    if ($ParentDir -ne $OutPath) {
        $SubDir = @($ParentDir | Split-Path -Leaf);
        $ParentDir = $ParentDir | Split-Path -Parent;
        while ($ParentDir -ne $OutPath) {
            $SubDir = @($ParentDir | Split-Path -Leaf) + $SubDir;
            $ParentDir = $ParentDir | Split-Path -Parent;
        }
    }
    $TargetPath = $DistPath;
    if ($SubDir.Count -gt 0) {
        $SubDir | ForEach-Object {
            $TargetPath = $TargetPath | Join-Path -ChildPath $_;
            if (-not ($TargetPath | Test-Path)) {
                (New-Item -ItemType Directory -Path $TargetPath) | Out-Null;
            }
        }
    }
    $TargetPath = $TargetPath | Join-Path -ChildPath $FileName;
    Write-Information -MessageData "$SourcePath => $TargetPath";
    Copy-Item -Path $SourcePath -Destination $TargetPath;
}
Write-Information -MessageData "$($FileList.Count) files copied.";