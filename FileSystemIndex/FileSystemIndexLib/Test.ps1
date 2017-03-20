<#
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process
#>
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

Function Get-FileSystemDiff {
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$OriginalPath,
        
        [Parameter(Mandatory = $true, Position = 1)]
        [string]$NewPath
    )
    
    $Names = @();
    $StringDiff = $null;
    if (Test-Path -Path $OriginalPath -PathType Leaf) {
        if (Test-Path -Path $NewPath -PathType Leaf) {
            $OriginalItem = Get-Item -LiteralPath $OriginalPath;
            $NewItem = Get-Item -LiteralPath $NewPath;
            $Extension = [System.IO.Path]::GetExtension($OriginalPath);
            if (@('.cs', '.xml', '.csproj', '.pssproj', '.ps1', '.psm1', '.psd1', '.bat', '.txt', '.md', '.htm', '.html', '.xaml', '.css') -icontains $Extension) {
                Write-Progress -Activity "Looking for changes" -CurrentOperation "$OriginalPath => $NewPath" -Status 'Comparing contents';
                $StringDiff = New-Object -TypeName 'FileSystemIndexLib.StringDiff' -ArgumentList ([System.IO.File]::ReadAllText($OriginalPath)), $OriginalPath, $OriginalItem.LastWriteTime, ([System.IO.File]::ReadAllText($NewPath)), $NewPath, $NewItem.LastWriteTime;
                if ($StringDiff.IsModified) { $StringDiff.ToUnifiedDiff() }
            } else {
                if ($OriginalItem.Length -ne $NewItem.Length -or [System.IO.File]::ReadAllText($OriginalPath) -ne [System.IO.File]::ReadAllText($NewPath)) {
                    "File Changed: $NewPath";
                } else {
                    Write-Progress -Activity "Looking for changes" -CurrentOperation "$OriginalPath => $NewPath" -Status 'Comparing contents';
                    if ([System.IO.File]::ReadAllText($OriginalPath) -ne [System.IO.File]::ReadAllText($NewPath)) {
                        "File Changed: $NewPath";
                    }
                }
            }
        } else {
            "File Removed: $OriginalPath";
            if (Test-Path -Path $NewPath -PathType Container) {
                "Folder Added: $NewPath";
            }
        }
    } else {
        if (Test-Path -Path $OriginalPath -PathType Container) {
            if (Test-Path -Path $NewPath -PathType Container) {
                Write-Progress -Activity "Looking for changes" -CurrentOperation "$OriginalPath => $NewPath" -Status 'Processing folder';
                $Names = @((@(Get-ChildItem -LiteralPath $OriginalPath) + @(Get-ChildItem -LiteralPath $NewPath)) | ForEach-Object { $_.Name });
                if ($Names.Count -gt 0) {
                    ($Names | Sort-Object -Unique) | ForEach-Object {
                        Get-FileSystemDiff -OriginalPath ($OriginalPath | Join-Path -ChildPath $_) -NewPath ($NewPath | Join-Path -ChildPath $_);
                    }
                }
            } else {
                "Folder Removed: $OriginalPath";
                if (Test-Path -Path $NewPath -PathType Leaf) {
                    "File Added: $NewPath";
                }
            }
        } else {
            if (Test-Path -Path $NewPath -PathType Container) {;
                "Folder Added: $NewPath";
            } else {
                if (Test-Path -Path $NewPath -PathType Leaf) {
                    "File Added: $NewPath";
                }
            }
        }
    }
}
Get-FileSystemDiff -OriginalPath 'C:\Users\leonarde\Documents\WindowsPowerShell\PowerShell-Modules\previous' -NewPath 'C:\Users\leonarde\Documents\WindowsPowerShell\PowerShell-Modules\PowerShell-Modules-Work';
Write-Progress -Activity "Looking for changes" -Status 'Finished' -Completed;
