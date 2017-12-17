Param(
    [Parameter(Position = 0)]
    [string]$Configuration = 'Release',

    [Parameter(Position = 1)]
    [string]$Platform = 'AnyCPU'
)

Add-Type -AssemblyName 'Microsoft.Build', 'Microsoft.Build.Framework', 'Microsoft.Build.Utilities.v4.0' -ErrorAction Stop;
Add-Type -Path ($PSScriptRoot | Join-Path -ChildPath 'MsBuildLogHelper.cs') -ReferencedAssemblies 'System', 'Microsoft.Build', 'Microsoft.Build.Framework', 'Microsoft.Build.Utilities.v4.0', 'System.Management.Automation' -ErrorAction Stop;

if ($Project -ne $null) {
    $Project.ProjectCollection.UnloadAllProjects();
    $Project = $null;
}
$Project = [Microsoft.Build.Evaluation.Project]::new(($PSScriptRoot | Join-Path -ChildPath 'LteDev.csproj'));
if ($Project -eq $null) { return }
$Project.SetProperty('Configuration', $Configuration);
$Project.SetProperty('Platform', $Platform);
$Logger = [LteDev.MSBuildLogHelper]::new();
$Logger.StartBuild($Project);
$Logger.Verbosity = [Microsoft.Build.Framework.LoggerVerbosity]::Normal;
$Output = @();
while (-not $Logger.WaitBuild(1000)) {
    $v = $Logger.Read();
    if ($v.Count -gt 0) {
        $v | Out-String;
        $Output = $Output + @($v);
    }
}
$v = $Logger.Read();
if ($v.Count -gt 0) {
    $v | Out-String;
    $Output = $Output + @($v);
}

if ($Logger.EndBuild()) {
    'Build succeeded' | Write-Output;
} else {
    'Build Failed' | Write-Warning;
}

