$ModuleBaseName = 'WinForms';

$ModulePaths = $env:PSModulePath.Split([System.IO.Path]::PathSeparator);

$installedAt = @($ModulePaths | ForEach-Object { $_ | Join-Path -ChildPath:$ModuleBaseName } | Where-Object { Test-Path -Path:$_ });

if ($installedAt.Count -gt 0) {
    @(
        'Module folders have been found at the following locations',
        ($installedAt | ForEach-Object { "`t{0}" -f $_ } | Out-String),
        'Please run the uninstaller, first.'
    ) | Write-Warning;
    return;
}

$i = 0;
$choices = @(
    $env:PSModulePath.Split([System.IO.Path]::PathSeparator) | ForEach-Object {
        $i = $i + 1;
        (New-Object -TypeName:'System.Management.Automation.Host.ChoiceDescription' -ArgumentList:($i, $_))
    }
);

$result = $Host.UI.PromptForChoice("Select Module Location", (@($choices | ForEach-Object { '{0}: {1}' -f $_.Label, $_.HelpMessage }) | Out-String), $choices, 1);
$ModuleInstallLocation = $choices[$result].HelpMessage | Join-Path -ChildPath:$ModuleBaseName;

$folder = New-Item -Path:$ModuleInstallLocation -ItemType:'Directory';

if ($folder -eq $null) {
    'Error creating destination folder.' | Write-Warning;
    return;
}

$fileName = $ModuleBaseName + '.dll';
$source = $PSScriptRoot | Join-Path -ChildPath:$fileName;
$destination = $ModuleInstallLocation | Join-Path -ChildPath:$fileName;
Copy-Item -Path:$source -Destination:$destination;

$fileName = $ModuleBaseName + '.psd1';
$source = $PSScriptRoot | Join-Path -ChildPath:$DllFileName;
$destination = $ModuleInstallLocation | Join-Path -ChildPath:$DllFileName;
Copy-Item -Path:$source -Destination:$destination;

'Finished.' | Write-Host;