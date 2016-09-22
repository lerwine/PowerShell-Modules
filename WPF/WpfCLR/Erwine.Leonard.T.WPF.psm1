Function Initialize-CurrentModule {
    [CmdletBinding()]
    Param()
	
	$Local:BaseName = $PSScriptRoot | Join-Path -ChildPath $MyInvocation.MyCommand.Module.Name;
	if (($Local:BaseName + ".dll") | Test-Path -PathType Leaf) {
		if ((Add-Type -AssemblyName ($Local:BaseName + ".dll") -PassThru) -ne $null) { return }
	}
	
    $Local:Splat = @{
        TypeName = 'System.CodeDom.Compiler.CompilerParameters';
        ArgumentList = (,@(
    		[System.Text.RegularExpressions.Regex].Assembly.Location,
            [System.Xml.XmlDocument].Assembly.Location,
            [System.Management.Automation.ScriptBlock].Assembly.Location,
            (Add-Type -AssemblyName 'PresentationFramework' -ErrorAction Stop -PassThru)[0].Assembly.Location,
            (Add-Type -AssemblyName 'PresentationCore' -ErrorAction Stop -PassThru)[0].Assembly.Location,
            (Add-Type -AssemblyName 'WindowsBase' -ErrorAction Stop -PassThru)[0].Assembly.Location
        ));
    };
    $Local:ModuleManifest = Test-ModuleManifest -Path ($PSScriptRoot | Join-Path -ChildPath ('{0}.psd1' -f $MyInvocation.MyCommand.Module.Name));
    if ($Local:ModuleManifest.PrivateData.CompilerOptions -ne $null) {
        if ($Local:ModuleManifest.PrivateData.CompilerOptions.IncludeDebugInformation -ne $null) {
            try {
                [bool]$Local:b = $Local:ModuleManifest.PrivateData.CompilerOptions.IncludeDebugInformation;
                $Local:Splat.Property = @{
                    IncludeDebugInformation = $Local:b;
                    CompilerOptions = '/pdb:"{0}.pdb" /define:DEBUG' -f $Local:BaseName;
                }
            } catch { }
        }
        if ($PSVersionTable.CLRVersion.Major -lt 3 -or ($PSVersionTable.CLRVersion.Major -eq 3 -and $PSVersionTable.CLRVersion.Minor -lt 5)) {
            if ($Local:Splat.Property -eq $null) {
                $Local:Splat.Property = @{ CompilerOptions = '/pdb:"{0}.pdb" /define:PSLEGACY;PSLEGACY2' -f $Local:BaseName }
            } else {
                $Local:Splat.Property.CompilerOptions += ';PSLEGACY;PSLEGACY2';
            }
        } else {
            if ($PSVersionTable.CLRVersion.Major -lt 3) {
                if ($Local:Splat.Property -eq $null) {
                    $Local:Splat.Property = @{ CompilerOptions = '/pdb:"{0}.pdb" /define:PSLEGACY;PSLEGACY3' -f $Local:BaseName }
                } else {
                    $Local:Splat.Property.CompilerOptions += ';PSLEGACY;PSLEGACY3';
                }
            }
        }
    }
    
	if ($Local:Splat.Property -eq $null) {
		$Local:Splat.Property = @{ CompilerOptions = '/doc:"{0}.xml" /out:"{0}.dll"' -f $Local:BaseName }
	} else {
		$Local:Splat.Property.CompilerOptions += (' /doc:"{0}.xml" /out:"{0}.dll"' -f $Local:BaseName);
	}
    Add-Type -Path (('ReadOnlyDictionary.cs', 'RelayCommand.cs', 'ThisObj.cs', 'WindowProcessInternal.cs',
		'WpfWindow.cs') | ForEach-Object { $PSScriptRoot | Join-Path -ChildPath $_ }) -CompilerParameters (New-Object @Local:Splat);
}

Initialize-CurrentModule

Function Assert-ValidXamlMarkup {
	<#
		.SYNOPSIS
			Asserts validity of XAML markup.
 
		.DESCRIPTION
			Throws an error if the XML Markup is not valid.
	#>
    [CmdletBinding()]
    Param(
		# XAML markup.
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [string]$Xml,
		
		# Whether to return the parsed XAML.
		[switch]$PassThru
    )
    
    Process {
		$XmlDocument = $null;
		[WpfCLR.WpfWindow]::AssertValidXaml($Xml, [ref]$XmlDocument);
		if ($PassThru -and $XmlDocument -ne $null) { $XmlDocument | Write-Output }
	}
}

Function Test-XamlMarkup {
	<#
		.SYNOPSIS
			Check validity of XAML markup.
 
		.DESCRIPTION
			Returns boolean value (or message) to indicate whether the XML Markup is valid.
        
		.OUTPUTS
			System.Boolean. (If -not $ReturnErrorMessage) True if markup is valid; otherwise False.
			System.String. (If $ReturnErrorMessage) Null if markup is valid; otherwise error message text.
	#>
    [CmdletBinding(DefaultParameterSetName = 'ReturnBoolean')]
	[OutputType([bool], ParameterSetName = 'ReturnBoolean')]
	[OutputType([string], ParameterSetName = 'ReturnMessage')]
    Param(
		# XAML markup.
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'ReturnBoolean')]
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'ReturnMessage')]
        [string]$Xml,
		
		# Whether to return the error message instead of a boolean value.
        [Parameter(Mandatory = $true, ParameterSetName = 'ReturnMessage')]
		[switch]$ReturnErrorMessage
    )
    
    Process {
		$Message = '';
		if ([WpfCLR.WpfWindow]::TryValidateXaml($Xml, [ref]$Message)) {
			if (-not $ReturnErrorMessage) { $true }
		} else {
			if ($ReturnErrorMessage) { $Message } else { $false }
		}
	}
}

Function New-XamlWindowSource {
	<#
		.SYNOPSIS
			Create empty markup for new window.
 
		.DESCRIPTION
			Creates template XAML markup for creating a new window.

		.OUTPUTS
			System.Xml.XmlDocument. XAML markup for a new window.
	#>
    [CmdletBinding()]
	[OutputType([System.Xml.XmlDocument])]
    Param()
    
	[Xml]$XmlDocument = [WpfCLR.WpfWindow]::Xaml_EmptyWindow;
	$XmlDocument | Write-Output;
}

Function New-WpfWindow {
	<#
		.SYNOPSIS
			Create WPF Window proxy object.
 
		.DESCRIPTION
			Creates a new object to represent a WPF Window.

		.OUTPUTS
			WpfCLR.WpfWindow. New WPF window proxy object.
	#>
    [CmdletBinding()]
    Param(
		# Window XAML markup.
		[ValidateScript({ $_ | Assert-ValidXamlMarkup })]
        [string]$WindowXaml,

		# ScriptBlock to invoke before window is created.
        [ScriptBlock]$BeforeWindowCreated,

		# ScriptBlock to invoke before window is shown.
        [ScriptBlock]$BeforeWindowShown,

		# ScriptBlock to invoke before window is closed.
        [ScriptBlock]$AfterWindowClosed,

		# Data to be made available to script blocks, which is also returned in the WpfWindow proxy object in the 'SynchronizedData' property.
        [Hashtable]$InstanceData
	)
    
	$WpfWindow = New-Object -TypeName 'WpfCLR.WpfWindow';
	if ($PSBoundParameters.ContainsKey('WindowXaml')) { $WpfWindow.WindowXaml = $WindowXaml }
	if ($PSBoundParameters.ContainsKey('BeforeWindowCreated')) { $WpfWindow.BeforeWindowCreated = $BeforeWindowCreated }
	if ($PSBoundParameters.ContainsKey('BeforeWindowShown')) { $WpfWindow.BeforeWindowShown = $BeforeWindowShown }
	if ($PSBoundParameters.ContainsKey('AfterWindowClosed')) { $WpfWindow.AfterWindowClosed = $AfterWindowClosed }
	if ($PSBoundParameters.ContainsKey('InstanceData')) {
		foreach ($key in $InstanceData.Keys) {
			$WpfWindow.SynchronizedData[$key] = $InstanceData[$key];
		}
	}
	
	$WpfWindow | Write-Output;
}

Function Show-WpfWindow {
	<#
		.SYNOPSIS
			Show a WPF Window.
 
		.DESCRIPTION
			Shows a WPF Window from a proxy object.

		.OUTPUTS
			WpfCLR.WpfWindow. Window that was displayed.
	#>
    [CmdletBinding(DefaultParameterSetName = 'New')]
    Param(
		# Window proxy object.
		[Parameter(Mandatory = $true, ParameterSetName = 'Existing')]
        [WpfCLR.WpfWindow]$WindowProxy,

		# Window XAML markup.
		[Parameter(Mandatory = $true, ParameterSetName = 'New')]
		[ValidateScript({ $_ | Assert-ValidXamlMarkup })]
        [string]$WindowXaml,

		# ScriptBlock to invoke before window is created.
		[Parameter(ParameterSetName = 'New')]
        [ScriptBlock]$BeforeWindowCreated,

		# ScriptBlock to invoke before window is shown.
		[Parameter(ParameterSetName = 'New')]
        [ScriptBlock]$BeforeWindowShown,

		# ScriptBlock to invoke before window is closed.
		[Parameter(ParameterSetName = 'New')]
        [ScriptBlock]$AfterWindowClosed,

		# Data to be made available to script blocks, which is also returned in the WpfWindow proxy object in the 'SynchronizedData' property.
		[Parameter(ParameterSetName = 'New')]
        [Hashtable]$InstanceData,
		
		# Indicates that no child windows will be displayed, and this will be system modal.
		[Parameter(ParameterSetName = 'New')]
		[switch]$AsDialog
	)
    $WpfWindow = $WindowProxy;
	if (-not $PSBoundParameters.ContainsKey('WindowProxy')) {
		$WpfWindow = New-Object -TypeName 'WpfCLR.WpfWindow';
		$WpfWindow.WindowXaml = $WindowXaml;
		if ($PSBoundParameters.ContainsKey('BeforeWindowCreated')) { $WpfWindow.BeforeWindowCreated = $BeforeWindowCreated }
		if ($PSBoundParameters.ContainsKey('BeforeWindowShown')) { $WpfWindow.BeforeWindowShown = $BeforeWindowShown }
		if ($PSBoundParameters.ContainsKey('AfterWindowClosed')) { $WpfWindow.AfterWindowClosed = $AfterWindowClosed }
		if ($PSBoundParameters.ContainsKey('InstanceData')) {
			foreach ($key in $InstanceData.Keys) {
				$WpfWindow.SynchronizedData[$key] = $InstanceData[$key];
			}
		}
	}
	if ($AsDialog) {
		$WpfWindow.ShowDialog($Host) | Out-Null;
	} else {
		if (-not $DoNotShow -and $PSBoundParameters.ContainsKey('WindowXaml')) {
			$WpfWindow.Show($Host) | Out-Null;
		}
	}
	
	$WpfWindow | Write-Output;
}