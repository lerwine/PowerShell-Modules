Param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [string]$Platform = 'AnyCPU',

    [ValidateSet('Test', 'Deploy', 'None')]
    [string]$Action = 'None',

    [switch]$ForceRebuild
)

Add-Type -AssemblyName 'Microsoft.Build', 'Microsoft.Build.Framework', 'Microsoft.Build.Utilities.v4.0' -ErrorAction Stop;
Add-Type -Path ($PSScriptRoot | Join-Path -ChildPath '..\LteDev\MsBuildLogHelper.cs') -ReferencedAssemblies 'System', 'Microsoft.Build', 'Microsoft.Build.Framework', 'Microsoft.Build.Utilities.v4.0', 'System.Management.Automation';

if ($Project -ne $null) {
    $Project.ProjectCollection.UnloadAllProjects();
    $Project = $null;
}
$csproj_path = $PSScriptRoot | Join-Path -ChildPath 'NetworkUtility.csproj';
$Project = [Microsoft.Build.Evaluation.Project]::new($csproj_path);
if ($Project -eq $null) { return }
$Project.SetProperty('Configuration', $Configuration) | Out-Null;
$Project.SetProperty('Platform', $Platform) | Out-Null;
$Project.ReevaluateIfNecessary();
$ShouldBuild = $ForceRebuild;
if (-not $ShouldBuild) {
    $TargetPath = $Project.GetPropertyValue('TargetPath');
    if (Test-Path -Path $TargetPath) {
        $LastBuildTime = Get-ItemPropertyValue -Name 'LastWriteTimeUtc' -Path $TargetPath;
        if ((Get-ItemPropertyValue -Name 'LastWriteTimeUtc' -Path $csproj_path) -gt $LastBuildTime) {
            $ShouldBuild = $true;
        } else {
            $NewModifications = @($Project.Items | Where-Object {
                if (('Compile', 'Content', 'EmbeddedResource') -contains $_.ItemType) {
                    $p = $PSScriptRoot | Join-Path -ChildPath $_.EvaluatedInclude;
                    (Test-Path -LiteralPath $p) -and (Get-ItemPropertyValue -Name 'LastWriteTimeUtc' -Path $p) -gt $LastBuildTime;
                } else { $false }
            });
            $ShouldBuild = ($NewModifications.Count -gt 0);
        }
    } else {
        $ShouldBuild = $true;
    }
}
if ($ShouldBuild) {
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

    $Properties = @{ Label = 'Type'; Expression = {
            $s = $_.GetType().Name;
            if ($s.Length -gt 9 -and $s.EndsWith('EventArgs')) { $s.Substring(0, $s.Length - 9) } else { $s }
        } }, 'File', 'Code', 'LineNumber', 'ColumnNumber', 'Message', 'Timestamp';

    $Succeeded = $Logger.EndBuild();
    $ErrorOutput = @();
    $Script:ErrorCount = 0;
    $Script:WarningCount = 0;
    if ($Output.Count -gt 0) {
        $ErrorOutput = @($Output | Where-Object { $_ -is [Microsoft.Build.Framework.BuildErrorEventArgs] -or $_ -is [Microsoft.Build.Framework.BuildWarningEventArgs] });
        if ($ErrorOutput.Count -gt 0) {
            $Script:ErrorCount = @($ErrorOutput | Where-Object { $_ -is [Microsoft.Build.Framework.BuildErrorEventArgs] }).Count;
            $Script:WarningCount = @($ErrorOutput | Where-Object { $_ -is [Microsoft.Build.Framework.BuildWarningEventArgs] }).Count;
        }
    }

    $Message = &{
        if ($Script:ErrorCount -eq 0) {
            if ($Script:WarningCount -eq 0) { return 'No errors or warnings' }
            if ($Script:WarningCount -eq 1) { return '1 warning' }
            return "$Script:WarningCount warnings";
        }
        if ($Script:ErrorCount -eq 1) {
            if ($Script:WarningCount -eq 0) { return '1 error' }
            if ($Script:WarningCount -eq 1) { return '1 error and 1 warning' }
            return "1 error and $Script:WarningCount warnings";
        }
        if ($Script:WarningCount -eq 0) { return "$Script:ErrorCount errors" }
        if ($Script:WarningCount -eq 1) { return "$Script:ErrorCount errors and 1 warning" }
        return "$Script:ErrorCount errors and $Script:WarningCount warnings";
    };

    if ($Succeeded) {
        "Build succeeded: $Message" | Write-Output;
    } else {
        "Build Failed: $Message" | Write-Warning;
    }

    if ($Output.Count -gt 0) {
        $Output | Select-Object -Property ($Properties) | Out-GridView -Title 'Build Output';
        if ($ErrorOutput.Count -gt 0) {
            ($ErrorOutput | Select-Object -Property $Properties) | Out-GridView -Title (&{
                if ($Script:ErrorCount -eq 0) { return 'Build Warnings' }
                return 'Build Errors'
            });
        }
    }
    if (-not $Succeeded) { return }
}

if ((Get-Module -Name 'Erwine.Leonard.T.NetworkUtility') -ne $null) { Remove-Module -Name 'Erwine.Leonard.T.NetworkUtility' -ErrorAction Stop }
$CmdErrors = @();
$CmdWarnings = @();
$ModuleInfo = $null;
$ModuleInfo = Import-module -Name (($PSScriptRoot | Join-Path -ChildPath $Project.GetPropertyValue('OutputPath')) | Join-Path -ChildPath "Erwine.Leonard.T.NetworkUtility.psd1") `
    -ErrorVariable 'CmdErrors' -WarningVariable 'CmdWarnings' -ErrorAction Continue -PassThru;
$ErrorObj = @();
if ($CmdErrors.Count -gt 0) {
    $ErrorObj = @(@($Error) | Select-Object -Property @{
        Label = 'Message'; Expression = { $_.ToString() } }, @{
        Label = 'InnerMessage'; Expression = {
            $m = $_.ToString();
            for ($e = $_.Exception; $e -ne $null; $e = $e.InnerException) {
                if ($e.Message -ne $m) { return $e.Message }
            }
            return '';
        } }, @{
        Label = 'Position'; Expression = { if ($_.InvocationInfo -eq $null) { return '' } $_.InvocationInfo.PositionMessage } }, @{
        Label = 'Category'; Expression = { if ($_.CategoryInfo -eq $null) { return '' } $_.CategoryInfo.GetMessage() } }, @{
        Label = 'Stack Trace'; Expression = { if ($_.ScriptStackTrace -eq $null) { return '' } $_.ScriptStackTrace } });
}
if ($CmdWarnings.Count -gt 0) { $CmdWarnings | Out-GridView -Title 'Module Load Warnings' }
if ($ErrorObj.Count -gt 0) { $ErrorObj | Out-GridView -Title 'Module Load Errors' }
if ($ModuleInfo -eq $null) {
    Write-Warning -Message 'Module load failed';
    return;
}
if ($ErrorObj.Count -gt 0) {
    Write-Warning -Message 'Test scripts not invoked due to errors during module load';
    return;
}

$TestScripts = @(Get-ChildItem -Path ($PSScriptRoot | Join-Path -ChildPath 'TestScripts') -Filter 'Test-*.ps1');
$ErrorObj = @();
$MessageObj = @();
$TestScripts | ForEach-Object {
    $CmdErrors = @();
    $CmdWarnings = @();
    $Name = $_.Name;
    Invoke-Expression -Command $_.FullName -ErrorVariable 'CmdErrors' -WarningVariable 'CmdWarnings' -ErrorAction Continue;
    if ($CmdErrors.Count -gt 0) {
        $ErrorObj = $ErrorObj + @(@($CmdErrors) | Select-Object -Property @{
            Label = 'Script'; Expression = { $Name } }, @{
            Label = 'Message'; Expression = { $_.ToString() } }, @{
            Label = 'InnerMessage'; Expression = {
                $m = $_.ToString();
                for ($e = $_.Exception; $e -ne $null; $e = $e.InnerException) {
                    if ($e.Message -ne $m) { return $e.Message }
                }
                return '';
            } }, @{
            Label = 'Position'; Expression = { if ($_.InvocationInfo -eq $null) { return '' } $_.InvocationInfo.PositionMessage } }, @{
            Label = 'Category'; Expression = { if ($_.CategoryInfo -eq $null) { return '' } $_.CategoryInfo.GetMessage() } }, @{
            Label = 'Stack Trace'; Expression = { if ($_.ScriptStackTrace -eq $null) { return '' } $_.ScriptStackTrace } });
    }
    if ($CmdWarnings.Count -gt 0) {
        $MessageObj = $MessageObj + @(@($CmdWarnings) | Select-Object -Property @{ Label = 'Type'; Expression = { 'Warning' } }, @{ Label = 'Script'; Expression = { $Name } }, 'Message');
    } else {
        if ($CmdErrors.Count -eq 0) { $MessageObj = $MessageObj + @([PSCustomObject]@{ Type = 'Success'; Script = $Name; Message = 'No errors or warnings' }) };
    }
}

if ($ErrorObj.Count -gt 0) {
    $ErrorObj | Out-GridView -Title 'Test Errors';
}

if ($WarningObj.Count -gt 0) {
    $WarningObj | Out-GridView -Title 'Test Warnings';
}