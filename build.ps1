Param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [string]$Platform = 'AnyCPU',

    [ValidateSet('Test', 'Deploy', 'None')]
    [string]$Action = 'None',

    [ValidateSet('Build', 'Resources', 'Compile', 'Rebuild', 'Clean', 'Publish')]
    [string[]]$Targets = @('Build'),

    [ValidateSet('CertificateCryptography', 'CredentialStorage', 'GDIPlus', 'WinIOUtility', 'LteDev', 'NetworkUtility', 'PSDB', 'PsMsBuildHelper', 'XmlUtility')]
    [string]$Project,

    [int]$TermMinWidth = 2048,

    [int]$TermMinHeight = 6000,

    [string]$MsBuildBin = 'C:\Program Files (x86)\MSBuild\14.0\Bin'
)

$old_InformationPreference = $InformationPreference;
$InformationPreference = [System.Management.Automation.ActionPreference]::Ignore;
try {
    $InformationPreference = [System.Management.Automation.ActionPreference]::Continue;
    Write-Information -MessageData "Host: $((Get-Host).Name)"
    $HostRawUI = (Get-Host).UI.RawUI;
    if ($null -eq $HostRawUI.BufferSize) {
        Write-Information -MessageData "Buffer size was null";
        $HostRawUI.BufferSize = New-Object -TypeName ''System.Management.Automation.Host.Size'' -ArgumentList $TermMinWidth, $TermMinHeight;
    } else {
        Write-Information -MessageData "Buffer size was $($HostRawUI.BufferSize.Width), $($HostRawUI.BufferSize.Height)";
        if ($HostRawUI.BufferSize.Width -lt $TermMinWidth) { $HostRawUI.BufferSize.Width = $TermMinWidth }
        if ($HostRawUI.BufferSize.Height -lt $TermMinHeight) { $HostRawUI.BufferSize.Height = $TermMinHeight }
        Write-Information -MessageData "Buffer size is now $($HostRawUI.BufferSize.Width), $($HostRawUI.BufferSize.Height)";
    }
    if ($null -ne $HostRawUI.MaxWindowSize) {
        if ($HostRawUI.MaxWindowSize.Width -lt $HostRawUI.BufferSize.Width) {
            $HostRawUI.MaxWindowSize.Width = $HostRawUI.BufferSize.Width;
        }
        if ($HostRawUI.MaxWindowSize.Height -lt $HostRawUI.BufferSize.Height) {
            $HostRawUI.MaxWindowSize.Height = $HostRawUI.BufferSize.Height;
        }
    }
} finally {
    $InformationPreference = $old_InformationPreference;
}

$Script:SolutionFilePath = $PSScriptRoot | Join-Path -ChildPath $SolutionFile;
$Script:SolutionDirectory = $Script:SolutionFilePath | Split-Path -Parent;
$MSBuildExePath = $MsBuildBin | Join-Path -ChildPath 'MSBuild.exe';
if ($null -ne $Project -and $Project.Trim().Length -gt 0) {
    . $MSBuildExePath "/t:$($Targets -join ';')" "/verbosity:Detailed" "/p:Configuration=`"$Configuration`"" "/p:Platform=`"$Platform`"" "/logger:XmlMsBuildLogger,PsMsBuildHelper.dll;BuildResult_$Project.xml" "src/$Project/$Project.csproj";
#    . $MSBuildExePath "/t:$($Targets -join ';')" "/verbosity:Detailed" "/p:Configuration=`"$Configuration`"" "/p:Platform=`"$Platform`"" "src/$Project/$Project.csproj";
#    "$MSBuildExePath `"/t:$($Targets -join ';')`" `"/verbosity:Detailed`" `"/p:Configuration=`"$Configuration`"`" `"/p:Platform=`"$Platform`"`" `"src/$Project/$Project.csproj`"";
#    Add-Type -Path ($PSScriptRoot | Join-Path -ChildPath "PsMsBuildHelper.dll") -ErrorAction Stop;
#    $Success = [PsMsBuildHelper.XmlMsBuildLogger]::BuildProject(($PSScriptRoot | Join-Path -ChildPath "src/$Project/$Project.csproj"), ($PSScriptRoot | Join-Path -ChildPath "BuildResult_$Project.xml"), $Configuration, $Platform, $Targets);
#    if ($null -eq $Success) {
#        $Host.SetShouldExit(1);
#        exit;
#    }
#    if ($Success -is [bool]) {
#        if (-not $Success) {
#            $Host.SetShouldExit(2);
#            exit;
#        }
#    } else {
#        $Success | Out-String;
#        $Host.SetShouldExit(3);
#        exit;
#    }
} else {
    . $MSBuildExePath "/t:$($Targets -join ';')" "/verbosity:Detailed" "/p:Configuration=`"$Configuration`"" "/p:Platform=`"$Platform`"" "/logger:XmlMsBuildLogger,PsMsBuildHelper.dll;BuildResult.xml" "src/PowerShellModules.sln";
#    . $MSBuildExePath "/t:$($Targets -join ';')" "/verbosity:Detailed" "/p:Configuration=`"$Configuration`"" "/p:Platform=`"$Platform`"" "src/PowerShellModules.sln";
    "$MSBuildExePath `"/t:$($Targets -join ';')`" `"/verbosity:Detailed`"`"/p:Configuration=`"$Configuration`"`" `"/p:Platform=`"$Platform`"`" `"src/PowerShellModules.sln`"";
}
