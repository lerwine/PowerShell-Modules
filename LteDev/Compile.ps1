({
    $MSBuild = [System.Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory() | Join-Path -ChildPath 'MSBuild.exe';
    if (-not ($MSBuild | Test-Path -PathType Leaf)) {
        Write-Warning -Message "Unable to build project: MSBuild.exe not found in $([System.Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory()).";
        return;
    }

    $ErrorsPath = [Guid]::NewGuid().ToString('N');
    $WarningsPath = $args[0] | Join-Path -ChildPath "BuildWarnings_$ErrorsPath.txt";
    $ErrorsPath = $args[0] | Join-Path -ChildPath "BuildErrors_$ErrorsPath.txt";

    $BuildSwitches = ("/flp1:errorsonly;logfile=$ErrorsPath", "/flp2:warningsonly;logfile=$WarningsPath");
    $ProjFile = $args[0] | Join-Path -ChildPath 'IOUtilityCLR.csproj'

    $Old_VerbosePreference = $VerbosePreference;
    $VerbosePreference = [System.Management.Automation.ActionPreference]::Continue;
    # C:\
    # http://user:pw@hostname/
    # [^:\\/]+\:(//([^@:]*(:[^@]*)?@)?[^:\\/]+(:\d+)?)?
    # \\machine\share\
    # [\\/]{2}[^:\\/]+[\\/][^:\\/]+
    # C:
    # \WINDOWS\Microsoft.NET\Framework64\v4.0.30319\Microsoft.Common.targets(983,5): warning MSB3644: The';
    # 
    $ErrorLines = @();
    $WarningLines = @();
    $Success = $false;
    try {
        & $MSBuild $BuildSwitches $ProjFile | ForEach-Object {
            Write-Verbose -Message $_;
        };
        if ($ErrorsPath | Test-Path -PathType Leaf) {
            $ErrorLines = @(Get-Content -LiteralPath $ErrorsPath);
            Remove-Item -LiteralPath $ErrorsPath;
        }
        if ($WarningsPath | Test-Path -PathType Leaf) {
            $WarningLines = @(Get-Content -LiteralPath $WarningsPath);
            Remove-Item -LiteralPath $WarningsPath;
        }

        if ($ErrorLines.Count -gt 0) {
            $ErrorLines | ForEach-Object {
                $Host.UI.WriteErrorLine("$($_.ToString().Trim())`r`n");
            }
        }

        if ($WarningLines.Count -gt 0) {
            $WarningLines | ForEach-Object {
                $Host.UI.WriteWarningLine("$($_.ToString().Trim())`r`n");
            }
        } else {
            if ($ErrorLines.Count -eq 0) {
                Write-Verbose -Message "Completed with no errors or warnings";
            }
        }
    } finally {
        $VerbosePreference = $Old_VerbosePreference;
    }
}).Invoke($PSScriptRoot);
