$ModuleBaseName = 'LteUtils';
$ModuleInfo = $null;
Try {
    Import-Module -Name:$ModuleBaseName;
    $ModuleInfo = Get-Module -Name:$ModuleBaseName;
} Catch {
    $ModuleInfo = $null;
}
if ($ModuleInfo -eq $null) {
    $ModulePaths = $env:PSModulePath.Split([System.IO.Path]::PathSeparator);

    $folderAt = @($ModulePaths | ForEach-Object { $_ | Join-Path -ChildPath:$ModuleBaseName } | Where-Object { Test-Path -Path:$_ });

    if ($folderAt.Count -gt 0) {
        @(
            'Uninstall failed: Module folders have been found at the following locations',
            ($folderAt | ForEach-Object { "`t{0}" -f $_ } | Out-String),
            'This must be manually removed, before this module can be re-installed.'
        ) | Write-Warning;
    }
} else {
    $result = $Host.UI.PromptForChoice("Confirm Uninstall", ('Are you sure you want to remove module {0} from {1}?' -f $ModuleBaseName, $ModuleInfo.ModuleBase), @(
        (New-Object -TypeName:'System.Management.Automation.Host.ChoiceDescription' -ArgumentList:"Yes"),
        (New-Object -TypeName:'System.Management.Automation.Host.ChoiceDescription' -ArgumentList:"No")), 1);
    if ($result -ne $null -and $result -eq 0) {
        Remove-Module -Name:$ModuleBaseName;
        Remove-Item -Path:($ModuleInfo.ModuleBase) -Force -Recurse;
    }
}