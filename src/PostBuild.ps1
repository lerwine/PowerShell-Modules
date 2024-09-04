Param(
    [Parameter(Mandatory = $true)]
    [string]$TargetName,

    [Parameter(Mandatory = $true)]
    [string]$TargetExt,

    [Parameter(Mandatory = $true)]
    [string]$ProjectName,

    [Parameter(Mandatory = $true)]
    [string]$ProjectDir,

    [Parameter(Mandatory = $true)]
    [string]$OutDir,

    [Parameter(Mandatory = $true)]
    [string]$SolutionDir
)

$ErrorActionPreference = [System.Management.Automation.ActionPreference]::Stop;
$WarningPreference = [System.Management.Automation.ActionPreference]::Continue;
$InformationPreference = [System.Management.Automation.ActionPreference]::Continue;

Function Optimize-Path {
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

$OutPath = ((Join-Path -Path $ProjectDir -ChildPath $OutDir) | Resolve-Path).Path | Optimize-Path;
$DistPath = ((($SolutionDir | Join-Path -ChildPath '../dist') | Resolve-Path).Path | Join-Path -ChildPath "Erwine.Leonard.T.$ProjectName") | Optimize-Path;
$PsdPath = Join-Path -Path $OutPath -ChildPath "Erwine.Leonard.T.$ProjectName.psd1";
if (-not ($PsdPath | Test-Path -PathType Leaf)) { throw "Module manifest not found at $PsdPath." }
$ModuleManifest = Test-ModuleManifest -Path $PsdPath;
if ($null -eq $ModuleManifest.RootModule -or $ModuleManifest.RootModule.Trim().Length -eq 0) { throw "Module manifest not dfined for $PsdPath." }

$FileList = @($PsdPath, ($OutPath | Join-Path -ChildPath $ModuleManifest.RootModule), (Join-Path -Path $OutPath -ChildPath "$TargetName$TargetExt")) + @($ModuleManifest.FileList) + @($ModuleManifest.NestedModules) + @($ModuleManifest.Scripts) + @($ModuleManifest.ScriptsToProcess) + @($ModuleManifest.ExportedFormatFiles) + @($ModuleManifest.ExportedTypeFiles);

$Path = Join-Path -Path $OutPath -ChildPath "$TargetName.pdb";
if ($Path | Test-Path -PathType Leaf) { $FileList = $FileList + @($Path); }
$Path = Join-Path -Path $OutPath -ChildPath "$TargetName.XML";
if ($Path | Test-Path -PathType Leaf) { $FileList = $FileList + @($Path); }
if ($DistPath | Test-Path -PathType Container) { Remove-Item -Path $DistPath -Recurse -Force }
(New-Item -ItemType Directory -Path $DistPath) | Out-Null;
$FileList = @($FileList | ForEach-Object { if ($null -ne $_ -and $_.Length -gt 0) { ($_ | Resolve-Path).Path } } | Select-Object -Unique);
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