$ModuleBaseName = 'IOUtility';

$ModulePaths = $env:PSModulePath.Split([System.IO.Path]::PathSeparator);

$installedAt = @($ModulePaths | ForEach-Object { $_ | Join-Path -ChildPath:$ModuleBaseName } | Where-Object { Test-Path -Path:$_ });

if ($installedAt.Count -gt 0) {
    $Message = (@(
        'This Module has been found at the following locations:',
        ($installedAt | ForEach-Object { "`t{0}" -f $_ } | Out-String),
        '',
		'Do you wish to overwrite?'
    ) | Out-String).Trim();
    $Choices = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
    $Choices.Add((New-Object -TypeName:'System.Management.Automation.Host.ChoiceDescription' -ArgumentList:"Yes"));
    $Choices.Add((New-Object -TypeName:'System.Management.Automation.Host.ChoiceDescription' -ArgumentList:"No"));
    $result = $Host.UI.PromptForChoice("Confirm Overwrite", $Message, $Choices, 1);
	if ($result -eq $null -or $result -ne 0) {
		'Aborted.' | Write-Warning;
		return;
	}
}

$i = 0;
$Choices = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
$env:PSModulePath.Split([System.IO.Path]::PathSeparator) | ForEach-Object {
    $i = $i + 1;
    $Choices.Add((New-Object -TypeName:'System.Management.Automation.Host.ChoiceDescription' -ArgumentList:($i, $_)));
}


$result = $Host.UI.PromptForChoice("Select Module Location", (@($Choices | ForEach-Object { '{0}: {1}' -f $_.Label, $_.HelpMessage }) | Out-String).Trim(), $Choices, 0);
$ModuleInstallLocation = $Choices[$result].HelpMessage | Join-Path -ChildPath:$ModuleBaseName;

if (-not ($ModuleInstallLocation | Test-Path)) {
    $folder = New-Item -Path:$ModuleInstallLocation -ItemType:'Directory';

    if ($folder -eq $null) {
        'Error creating destination folder.' | Write-Warning;
        return;
    }
}

if ($PSScriptRoot -eq $null) { $PSScriptRoot = Get-Location }
$fileName = $ModuleBaseName + '.psm1';
$source = $PSScriptRoot | Join-Path -ChildPath:$fileName;
$destination = $ModuleInstallLocation | Join-Path -ChildPath:$fileName;
Copy-Item -Path:$source -Destination:$destination -Force;

$fileName = $ModuleBaseName + '.psd1';
$source = $PSScriptRoot | Join-Path -ChildPath:$fileName;
$destination = $ModuleInstallLocation | Join-Path -ChildPath:$fileName;
Copy-Item -Path:$source -Destination:$destination -Force;

'Finished.' | Write-Host;
