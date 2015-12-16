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

if ($installedAt.Count -gt 0) {
    @(
        'Script files have been found at the following locations',
        ($installedAt | ForEach-Object { "`t{0}" -f $_ } | Out-String),
        'Please run the uninstaller, first.'
    ) | Write-Warning;
    return;
}

$i = 0;
$Choices = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
$InstallOptions | ForEach-Object {
    $i = $i + 1;
    $Choices.Add((New-Object -TypeName:'System.Management.Automation.Host.ChoiceDescription' -ArgumentList:($i, $_)));
}

$result = $Host.UI.PromptForChoice("Select Module Location", (@($Choices | ForEach-Object { '{0}: {1}' -f $_.Label, $_.HelpMessage }) | Out-String).Trim(), $Choices, 0);
if ($result -eq $null -or $result -lt 0 -or $result -ge $Choices.Count) {
	'Aborted.' | Write-Warning;
	return;
}

$Script:InstallLocation = $Choices[$result].HelpMessage;

if (-not ($Script:InstallLocation | Test-Path)) {
	$AppsFolder = $Script:InstallLocation | Split-Path -Parent;
	if (-not ($AppsFolder | Test-Path)) {
		$PsFolder = $AppsFolder | Split-Path -Parent;
		if (-not ($PsFolder | Test-Path)) {
			$PsFolder = New-Item -Path:$PsFolder -ItemType:'Directory';
		}
		$AppsFolder = New-Item -Path:$AppsFolder -ItemType:'Directory';
	}
	$folder = New-Item -Path:$Script:InstallLocation -ItemType:'Directory';
    if ($folder -eq $null) {
        'Error creating destination folder.' | Write-Warning;
    	return;
    }
}

if ($PSScriptRoot -eq $null) { $PSScriptRoot = Get-Location }

$InstallInfo | ForEach-Object {
	$source = $PSScriptRoot | Join-Path -ChildPath $_.FileName;
	$destination = $Script:InstallLocation | Join-Path -ChildPath $_.FileName;
	Copy-Item -Path $source -Destination $destination;
	if ($_.BatchFileName -ne $null) {
		$destination = $Script:InstallLocation | Join-Path -ChildPath $_.BatchFileName;
		$a = $_.Args;
		if ($a -eq $null) {
			$a = '';
		} else {
			$a = ($a | Out-String).Trim();
			if ($a.Length -gt 0) { $a = ' {0}' -f $a }
		}
        [System.IO.File]::WriteAllLines($destination, @(
			'SET BatchPath=%~dp0',
			'pushd',
			'cd "%BatchPath%"',
			('powershell -ExecutionPolicy Bypass -File {0}{1}' -f $_.FileName, $a),
			'popd',
			'pause'
		));
	}
}

'Finished.' | Write-Host;
