Param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [string]$Platform = 'AnyCPU',

    [ValidateSet('Test', 'Deploy', 'None')]
    [string]$Action = 'None',

    # Resources;Compile
    [ValidateSet('Build', 'Resources', 'Compile', 'Rebuild', 'Clean', 'Publish')]
    [string[]]$Targets = @('Build'),

    [switch]$ForceRebuild
)
$VSCodeExtensions = @();
$MSBuildBinPath = $null;
$MSBuildExePath = $null;
$ToolsVersion = '14.0';
$FailMessage = $null;
$VsCodeCommand = $null;
$VsCodeCommand = &{
    $c = $null;
    $c = @(Get-Command -Name 'code' -CommandType Application -ErrorAction SilentlyContinue);
    if ($c.Count -gt 0) { return $c[0].Path }
    if ($env:VSCODE_CWD -ne $null) {
        $p = [System.IO.Path]::Combine($env:VSCODE_CWD, 'bin\code.cmd');
        if (Test-Path -Path $p -PathType Leaf) { return $p }
    }
};
if ($VsCodeCommand -ne $null) {
    $VSCodeExtensions = @((((. $VsCodeCommand --list-extensions --show-versions) | Out-String).Trim() -split '\r\n?|\n') | ForEach-Object {
        $s = $_.Trim();
        if ($s.Length -gt 0) {
            $i = $s.IndexOf('@');
            $VersionValue = $null;
            $PathName = $s;
            if ($i -lt 0) {
                if ($s -eq 'ms-vscode.csharp') { $VersionValue = New-Object -TypeName 'System.Version' }
            } else {
                $n = $s.Substring(0, $i);
                $VersionString = $s.Substring($i + 1);
                $PathName = $n + "-" + $VersionString
                if ($n -eq 'ms-vscode.csharp') {
                    if (-not [System.Version]::TryParse($VersionString, [ref]$VersionValue)) { $VersionValue = New-Object -TypeName 'System.Version'; }
                }
            }
            if ($VersionValue -ne $null) {
                (New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ path = $PathName; version = $VersionValue }) | Write-Output;
            }
        }
    });
    if ($VSCodeExtensions.Count -eq 0) {
        $FailMessage = "The ms-vscode.csharp extension was not found.";
    }
} else {
    $FailMessage = 'Visual Studio Code was not found';
}
if ($VSCodeExtensions.Count -gt 0) {
    if ($VSCodeExtensions.Count -gt 1) {
        $VSCodeExtensions = $VSCodeExtensions | Sort-Object -Descending -Property 'version';
    }
    
    $MSBuildBinPath = [System.IO.Path]::Combine([System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::UserProfile), '.vscode\extensions', $VSCodeExtensions[0].path);
    if (Test-Path -Path $MSBuildBinPath -PathType Container) {
        $MSBuildBinPath = [System.IO.Path]::Combine($MSBuildBinPath, ".omnisharp\msbuild");
        if (Test-Path -Path $MSBuildBinPath -PathType Container) {
            $Items = @((Get-ChildItem -Path $MSBuildBinPath) | Where-Object { $_.PSIsContainer } | ForEach-Object {
                $v = $null;
                $s = $_.Name;
                if ($s.StartsWith('v')) { $s = $s.Substring(1) }
                if (-not [System.Version]::TryParse($s, [ref]$v)) { $v = New-Object -TypeName 'System.Version' }
                (New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ path = $_.FullName; tv = $s; v = $v }) | Write-Output;
            } | Sort-Object -Descending -Property 'v');
            if ($Items.Count -gt 0) {
                $ToolsVersion = $Items[0].tv;
                $MSBuildBinPath = [System.IO.Path]::Combine($Items[0].path, "Bin");
            } else {
                $MSBuildBinPath = [System.IO.Path]::Combine($MSBuildBinPath, "Bin");
            }
            if (Test-Path -Path $MSBuildBinPath -PathType Container) {
                $MSBuildExePath = [System.IO.Path]::Combine($MSBuildBinPath, 'MSBuild.exe');
                if (-not (Test-Path -Path $MSBuildExePath -PathType Leaf)) {
                    $FailMessage = "MSBuild.exe not found in " + $MSBuildBinPath;
                }
            } else {
                $FailMessage = "An MSBuild Bin directory not found at " + $MSBuildBinPath;
            }
        } else {
            $FailMessage = "The OmniSharp MSBuild folder was not found at " + $MSBuildBinPath;
        }
    } else {
        $FailMessage = "The ms-vscode.csharp extension folder was not found at " + $MSBuildBinPath;
    }
}

if ($FailMessage -ne $null) {
    $AssemblyTypes = @(Add-Type -AssemblyName 'Microsoft.Build' -ErrorAction SilentlyContinue -PassThru);
    if ($AssemblyTypes.Count -gt 0) {
        $Project = [Microsoft.Build.Evaluation.Project]::new();
        try {
            $ToolsetLocations = @($Project.ProjectCollection.Toolsets | ForEach-Object {
                $v = $null;
                $s = $_.ToolsVersion;
                if ($s.StartsWith('v')) { $s = $s.Substring(1) }
                if ([System.Version]::TryParse($s, [ref]$v)) {
                    $p = [System.IO.Path]::Combine($_.ToolsPath, "MSBuild.exe");
                    if (Test-Path -Path $p -PathType Leaf) {
                        (New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{ path = $_.ToolsPath; tv = $s; v = $v }) | Write-Output;
                    }
                }
            } | Sort-Object -Property 'version' -Descending);
            if ($ToolsetLocations.Count -eq 0) {
                $FailMessage = $FailMessage + " and no build toolsets with MSBuild.exe were found";
            } else {
                $FailMessage = $null;
                $ToolsVersion = $ToolsetLocations[0].tv;
                $MSBuildBinPath = $ToolsetLocations[0].path;
                $MSBuildExePath = [System.IO.Path]::Combine($ToolsetLocations[0], "MSBuild.exe");
            }
        } finally {
            $Project = $null;
        }
    } else {
        $FailMessage = $FailMessage + " and failed to load the Microsoft.Build assembly";
    }
}

if ($FailMessage -eq $null) {
    $SlnPath = (Resolve-Path -Path 'PowerShellModules.sln');
    Push-Location -Path (Get-Location);
    try {
        Set-Location -Path $MSBuildBinPath;
        Get-Location
        ./MsBuild.exe "/t:$($Targets -join ';')" "/p:VisualStudioVersion=$ToolsVersion;Configuration=$Configuration;Platform=$Platform" $SlnPath;
    } finally {
        Pop-Location;
    }
} else {
    $FailMessage;
}