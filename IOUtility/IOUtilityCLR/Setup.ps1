Code: E:\Visual Studio 2015\Projects\PowerShell-Modules\master\IOUtility\IOUtilityCLR\DecodeRegexReplaceHandler.cs
Code: E:\Visual Studio 2015\Projects\PowerShell-Modules\master\IOUtility\IOUtilityCLR\EncodeRegexReplaceHandler.cs
Code: E:\Visual Studio 2015\Projects\PowerShell-Modules\master\IOUtility\IOUtilityCLR\MimeTypeDetector.cs
Code: E:\Visual Studio 2015\Projects\PowerShell-Modules\master\IOUtility\IOUtilityCLR\ModuleBuilder.cs
Code: E:\Visual Studio 2015\Projects\PowerShell-Modules\master\IOUtility\IOUtilityCLR\RegexReplaceHandler.cs
Code: E:\Visual Studio 2015\Projects\PowerShell-Modules\master\IOUtility\IOUtilityCLR\RegularExpressions.cs
Code: E:\Visual Studio 2015\Projects\PowerShell-Modules\master\IOUtility\IOUtilityCLR\ScriptRegexReplaceHandler.cs
Code: E:\Visual Studio 2015\Projects\PowerShell-Modules\master\IOUtility\IOUtilityCLR\StreamHelper.cs
Code: E:\Visual Studio 2015\Projects\PowerShell-Modules\master\IOUtility\IOUtilityCLR\TextHelper.cs
Code: E:\Visual Studio 2015\Projects\PowerShell-Modules\master\IOUtility\IOUtilityCLR\WindowOwner.cs
Item: E:\Visual Studio 2015\Projects\PowerShell-Modules\master\IOUtility\IOUtilityCLR\Erwine.Leonard.T.IOUtility.psm1
Item: E:\Visual Studio 2015\Projects\PowerShell-Modules\master\IOUtility\IOUtilityCLR\KnownMimeTypes.xml
Item: E:\Visual Studio 2015\Projects\PowerShell-Modules\master\IOUtility\IOUtilityCLR\ModuleDefinition.xml
$ModuleBaseName = 'Erwine.Leonard.T.IOUtility';
Add-Type -Path (
    ('DecodeRegexReplaceHandler.cs', 'EncodeRegexReplaceHandler.cs', 'MimeTypeDetector.cs', 'ModuleBuilder.cs', 'RegexReplaceHandler.cs', 'RegularExpressions.cs', 'ScriptRegexReplaceHandler.cs', 'StreamHelper.cs', 'TextHelper.cs', 'WindowOwner.cs') | ForEach-Object {
        ForEach-Object { $PSScriptRoot | Join-Path -ChildPath $_ }
    ) -ReferencedAssemblies 'System.Windows.Forms', 'System.Drawing', 'System.Data', 'System.Management.Automation', 'System.Xml';
$ModuleBaseName = 'Erwine.Leonard.T.IOUtility';
Add-Type -Path (('ModuleManifest.cs', 'NotificationForm.cs', 'NotificationForm.Designer.cs', 'SetupForm.cs', 'SetupForm.Designer.cs') | ForEach-Object { $PSScriptRoot | Join-Path -ChildPath $_ }) `
	-ReferencedAssemblies 'System.Windows.Forms', 'System.Drawing', 'System.Data', 'System.Management.Automation', 'System.Xml';

('System.Windows.Forms', 'System.Drawing', 'System.Data') | ForEach-Object {
	if ((Add-Type -AssemblyName $_ -PassThru -ErrorAction Stop) -eq $null) { throw ('Cannot load assembly "{0}".' -f $_) }
}

Function Invoke-InstallModule {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, ValueFromPipeline = $true)]
		[string]$Path
	)
	
	Process {

	}
}

Function Invoke-UninstallModule {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, ValueFromPipeline = $true)]
		[string]$Path
	)
	
	Process {

	}
}

Function Invoke-UpdateModule {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, ValueFromPipeline = $true)]
		[string]$Path
	)
	
	Process {

	}
}

$Script:ModuleManifest = New-Object -TypeName 'PSModuleInstallUtilModuleManifest' -ArgumentList ($PSScriptRoot | Join-Path -ChildPath 'Erwine.Leonard.T.IOUtility.psd1');
do {
	$SetupForm = New-Object -TypeName 'PSModuleInstallUtil.SetupForm' -ArgumentList $Script:ModuleManifest;
	try {
		$DialogResult = $SetupForm.ShowDialog();
		$Paths = $SetupForm.DestinationLocations;
	} catch {
		throw;
	}
	finally {
		$SetupForm.Dispose();
	}
	switch ($DialogResult) {
		{ $_ -eq [System.Windows.Forms.DialogResult]::OK } {
			$Paths | Invoke-InstallModule;
			break;
		}
		{ $_ -eq [System.Windows.Forms.DialogResult]::Yes } {
			break;
		}
		{ $_ -eq [System.Windows.Forms.DialogResult]::No } {
			$Paths | Invoke-UninstallModule;
			break;
		}
		{ $_ -eq [System.Windows.Forms.DialogResult]::Retry} {
			$Paths | Invoke-UpdateModule;
			break;
		}
	}
} while ($DialogResult -ne [System.Windows.Forms.DialogResult]::Cancel);