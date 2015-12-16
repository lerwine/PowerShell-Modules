$Script:AppName = 'CryptographyScripts';
$Script:AppFolderName = 'Apps';

$InstallInfo = @(
	@{ FileName = 'CredentialManager.ps1'; GenerateBatchFile = $true; Args = '-STA' },
	@{ FileName = 'EncryptFile.ps1'; GenerateBatchFile = $true; Args = '-STA' }
	@{ FileName = 'DecryptFile.ps1'; GenerateBatchFile = $true; Args = '-STA' }
);

$MyDocumentsPath = [System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::MyDocuments);
$ProgramFilesPath = [System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::ProgramFiles);
$InstallOptions = $env:PSModulePath.Split([System.IO.Path]::PathSeparator) | Resolve-Path | Split-Path -Parent | Where-Object {
    ($_ | Split-Path -Leaf) -ieq 'WindowsPowerShell' -and ($_.ToLower().StartsWith($MyDocumentsPath.ToLower()) -or $_.ToLower().StartsWith($ProgramFilesPath.ToLower()))
} | ForEach-Object { $_ | Join-Path -ChildPath $Script:AppFolderName | Join-Path -ChildPath $Script:AppName }

foreach ($Item in $InstallInfo) {
	$Item.BaseName = [System.IO.Path]::GetFileNameWithoutExtension($Item.FileName);
	if ($Item.GenerateBatchFile) { $Item.BatchFileName = '{0}.bat' -f $Item.BaseName }
}

$installedAt = @($InstallOptions | Where-Object { $path = $_; @($InstallInfo | ForEach-Object {
	$path | Join-Path -ChildPath $_.FileName;
	if ($_.BatchFileName -ne $null) { $path | Join-Path -ChildPath $_.BatchFileName }
} | Where-Object { $_ | Test-Path }).Count -gt 0});

if ($installedAt.Count -eq 0) {
	'Scripts not installed.' | Write-Warning;
	return;
}
if ($installedAt.Count -gt 1) {
	$Message = @(
        'Script files have been found at the following locations:',
        ($installedAt | ForEach-Object { "`t{0}" -f $_ } | Out-String),
        '',
		'Are you sure you want to remove all of these folders?'
    );
} else {
	$Message = @(
        'Script files have been found at the following location:',
        ("`t{0}" -f $installedAt),
        '',
		'Are you sure you want to remove this folder?'
    );
}

$Choices = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
$Choices.Add((New-Object -TypeName:'System.Management.Automation.Host.ChoiceDescription' -ArgumentList:"Yes"));
$Choices.Add((New-Object -TypeName:'System.Management.Automation.Host.ChoiceDescription' -ArgumentList:"No"));
$result = $Host.UI.PromptForChoice("Confirm Uninstall",$Message, $Choices, 1);
if ($result -ne $null -and $result -eq 0) {
    $installedAt | ForEach-Object { Remove-Item -Path:$_ -Force -Recurse; }
}
