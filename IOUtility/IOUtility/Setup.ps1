$ModuleBaseName = 'Erwine.Leonard.T.IOUtility';
Add-Type -Path (('ModuleManifest.cs', 'NotificationForm.cs', 'NotificationForm.Designer.cs', 'SetupForm.cs', 'SetupForm.Designer.cs') | ForEach-Object { $PSScriptRoot | Join-Path -ChildPath $_ }) `
	-ReferencedAssemblies 'System.Windows.Forms', 'System.Drawing', 'System.Data', 'System.Management.Automation', 'System.Xml';

$Script:Settings = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Script:Settings;

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