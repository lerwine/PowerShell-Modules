$Script:SetupManifestFileName = 'SetupManifestData.xml';

Function Get-ModuleSources {
	Param([string]$Path)

	$p = $Path | Combine-Path -ChildPath $Script:SetupManifestFileName;
	if (Test-Path -Path $p -PathType Leaf) {
		$XmlDocument = New-Object -TypeName 'System.Xml.XmlDocument';
		$XmlDocument.Load($p);
		New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
			Path = $Path;
			Manifest = $XmlDocument
		} | Write-Output;
	} else {
		[System.IO.Directory]::GetDirectories($Path) | ForEach-Object { Get-ModuleSources -Path $_ }
	}
}

Function Install-Module {
	Param(
		[Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
		[string]$SourcePath,
		
		[Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
		[System.Xml.XmlDocument]$ManifestProperties,

		[switch]$System
	)

    

    
	$InstallTargetRoot = $null;
	if ($System) {
		$InstallTargetRoot = $PSHome | Join-Path -ChildPath 'Modules';
	} else {
		$InstallTargetRoot = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList $InstallTargetRoot;
	}
	
	if (-not $InstallTargetRoot.Exists) {
		$InstallParent = $InstallTargetRoot.Parent;
		if (-not $InstallParent.Exists) {
			$InstallParent.Create();
			$InstallParent.Refresh();
			if (-not $InstallParent.Exists) { throw 'Unable to create install root parent folder' }
		}
		$InstallTargetRoot.Create();
		$InstallTargetRoot.Refresh();
		if (-not $InstallTargetRoot.Exists) { throw 'Unable to create install root folder' }
	}

	$ModuleName = $ManifestProperties.ManifestData.Name;

	$BasePathAndName = $SourcePath | Join-Path -ChildPath $ModuleName;

	$CompilerParametersSplat = @{
		TypeName = 'System.CodeDom.Compiler.CompilerParameters';
		Property = @{
			IncludeDebugInformation = ($ManifestProperties.ManifestData.CompilerOptions.IncludeDebugInformation -eq 'true');
			CompilerOptions = '/doc:"{0}.xml"' -f $BasePathAndName;
		};
	};

	$XsltArgumentList = New-Object -TypeName 'System.Xml.Xsl.XsltArgumentList';
	$XsltArgumentList.AddParam('PowerShellVersion', '', $PSVersionTable.PSVersion.ToString(2));
	$XsltArgumentList.AddParam('CLRVersion', '', $PSVersionTable.CLRVersion.ToString(2));
	if ($PSVersionTable.CLRVersion.Major -lt 4) {
		$XsltArgumentList.AddParam('DotNetFrameworkVersion', '', '3.5');
		if ($PSVersionTable.CLRVersion.Major -lt 3) {
			$XsltArgumentList.AddParam('ConditionalCompilationSymbols', '', 'PSLEGACY;PSLEGACY2');
			if ($ManifestProperties.ManifestData.CompilerOptions.IncludeDebugInformation -eq 'true') {
				$CompilerParametersSplat.Property.CompilerOptions = $CompilerParametersSplat.Property.CompilerOptions + ' /define:DEBUG;PSLEGACY;PSLEGACY2';
			} else {
				$CompilerParametersSplat.Property.CompilerOptions = $CompilerParametersSplat.Property.CompilerOptions + ' /define:PSLEGACY;PSLEGACY2';
			}
		} else {
			$XsltArgumentList.AddParam('ConditionalCompilationSymbols', '', 'PSLEGACY;PSLEGACY3');
			if ($ManifestProperties.ManifestData.CompilerOptions.IncludeDebugInformation -eq 'true') {
				$CompilerParametersSplat.Property.CompilerOptions = $CompilerParametersSplat.Property.CompilerOptions + ' /define:DEBUG;PSLEGACY;PSLEGACY3';
			} else {
				$CompilerParametersSplat.Property.CompilerOptions = $CompilerParametersSplat.Property.CompilerOptions + ' /define:PSLEGACY;PSLEGACY3';
			}
		}
	} else {
		$XsltArgumentList.AddParam('DotNetFrameworkVersion', '', $PSVersionTable.CLRVersion.ToString(2));
		if ($ManifestProperties.ManifestData.CompilerOptions.IncludeDebugInformation -eq 'true') {
			$CompilerParametersSplat.Property.CompilerOptions = $Local:Splat.Property.CompilerOptions + ' /define:DEBUG;PSLEGACY;PSLEGACY3';
		}
	}

	$FilesToCopy = @(('{0}.psm1' -f $ModuleName), ('{0}.xml' -f $ModuleName));
	$SourceFiles = @($ManifestProperties.ManifestData.FileList.Function | ForEach-Object { $_.Name });
	if ($SourceFiles.Count -gt 0) {
		$FilesToCopy += $SourceFiles;

		if ($ManifestProperties.ManifestData.CompilerOptions.IncludeDebugInformation -eq 'true') {
			$CompilerParametersSplat.Property.CompilerOptions += ' /pdb:"{0}.pdb"' -f $BasePathAndName;
			$FilesToCopy = $FilesToCopy + @('{0}.pdb' -f $ModuleName);
		}
		$CompilerParametersSplat.ArgumentList = (,@($ManifestProperties.ManifestData.CompilerOptions.SelectNodes("AssemblyReference") | ForEach-Object {
			(Add-Type -AssemblyName $_.InnerText -PassThru -ErrorAction Stop)[0].Assembly.Location;
		}));

		'Testing compilation' | Write-Host;
		if ((Add-Type -Path ($SourceFiles | ForEach-Object { $SourcePath | Join-Path -ChildPath $_ }) -CompilerParameters (New-Object @CompilerParametersSplat) -PassThru) -eq $null) {
			throw 'Error test compiling source code.'
		}
	}

	$DirectoryInfo = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList ([System.IO.Path]::Combine($InstallTargetRoot.FullName, $ModuleName));
	if ($DirectoryInfo -eq $null) { return }
	if (-not $DirectoryInfo.Exists) {
		$DirectoryInfo.Create();
		$DirectoryInfo.Refresh();
		if (-not $DirectoryInfo.Exists) {
			Write-Warning -Message ('Failed to create {0}' -f $DirectoryInfo.FullName);
			return;
		}
	}

	('{0} => {1}' -f $SourcePath, $DirectoryInfo.FullName) | Write-Host;

	foreach ($f in $FilesToCopy) {
		$sp = $SourcePath | Join-Path -ChildPath $f;
		$f | Write-Host;
		Copy-Item -Path $sp -Destination $DirectoryInfo.FullName -Force;
	}

	$TargetPath = [System.IO.Path]::Combine($DirectoryInfo.FullName, ('about_{0}.help.txt' -f $ModuleName));
	('Creating {0}' -f $Targetpath) | Write-Host;
	$XslTransform = New-Object -TypeName 'System.Xml.Xsl.XslTransform';
	if ($PSVersionTable.PSVersion.Major -lt 3) {
		$XslTransform.Load((($MyInvocation.MyCommand.Path | Split-Path -Parent) | Join-Path -ChildPath 'about.xslt'));
	} else {
		$XslTransform.Load(($PSScriptRoot | Join-Path -ChildPath 'ManifestToAbout.xslt'));
	}
	$XmlWriterSettings = New-Object -TypeName 'System.Xml.XmlWriterSettings' -Property @{
		ConformanceLevel = [System.Xml.ConformanceLevel]::Auto;
		CheckCharacters = $false;
		Indent = $true;
	};
	$XmlWriter = [System.Xml.XmlWriter]::Create($TargetPath, $XmlWriterSettings);
	$XslTransform.Transform($ManifestProperties, $null, $XmlWriter, $null);
	$XmlWriter.Close();

	$TargetPath = [System.IO.Path]::Combine($DirectoryInfo.FullName, ('{0}.psd1' -f $ModuleName));
	('Creating {0}' -f $Targetpath) | Write-Host;
	$XslTransform = New-Object -TypeName 'System.Xml.Xsl.XslTransform';
	if ($PSVersionTable.PSVersion.Major -lt 3) {
		$XslTransform.Load((($MyInvocation.MyCommand.Path | Split-Path -Parent) | Join-Path -ChildPath 'ManifestToPsd.xslt'));
	} else {
		$XslTransform.Load(($PSScriptRoot | Join-Path -ChildPath 'ManifestToPsd.xslt'));
	}
	$XmlWriter = [System.Xml.XmlWriter]::Create($TargetPath, $XmlWriterSettings);
	$XslTransform.Transform($ManifestProperties, $XsltArgumentList, $XmlWriter, $null);
	$XmlWriter.Close();
	'Finished.' | Write-Host;
}


if ($PSVersionTable.PSVersion.Major -lt 3) {
	Get-ModuleSources -Path ($MyInvocation.MyCommand.Path | Split-Path -Parent);
} else {
	Get-ModuleSources -Path $PSScriptRoot;
}