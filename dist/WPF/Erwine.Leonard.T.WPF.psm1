Function Get-WpfModuleAssemblyPath {
	<#
		.SYNOPSIS
			Get module CLR type path.
 
		.DESCRIPTION
			Get path to custom CLR type assembly for module.
	#>
    [CmdletBinding()]
    Param()

    return $Script:AssemblyPath;
}

Function Assert-ValidXamlMarkup {
	<#
		.SYNOPSIS
			Asserts validity of XAML markup.
 
		.DESCRIPTION
			Writes an error if the XML Markup is not valid.
	#>
    [CmdletBinding()]
    Param(
		# XAML markup.
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[ValidateScript({ $_ -is [string] -or $_ -is [System.Xml.XmlDocument] })]
        [object]$Xml,
		
		# Whether to return the XAML parse results upon success.
		[switch]$PassThru
    )
    
    Process {
		$XamlLoadResult = $null;
		$success = $false;
		if ($Xml -is [string]) {
			$success = [Erwine.Leonard.T.WPF.XamlUtility]::TryParseXaml($Xml, [ref]$XamlLoadResult);
		} else {
			$success = [Erwine.Leonard.T.WPF.XamlUtility]::TryLoadXaml($Xml, [ref]$XamlLoadResult);
		}
		if ($success) {
			if ($PassThru) { $XamlLoadResult | Write-Output; }
		} else {
			if ($XamlLoadResult -ne $null) {
				if ($XamlLoadResult.Error -ne $null) {
					if ($XamlLoadResult.Error.Message -ne $null -and $XamlLoadResult.Error.Message.Trim().Length > 0) {
						Write-Error -Exception $XamlLoadResult.Error -Category ParserError -ErrorId 'Assert.ValidXamlMarkup.Failure' -Message $XamlLoadResult.Error.Message -TargetObject $XamlLoadResult;
					} else {
						Write-Error -Exception $XamlLoadResult.Error -Category ParserError -ErrorId 'Assert.ValidXamlMarkup.Failure' -Message 'Markup is invalid.' -TargetObject $XamlLoadResult;
					}
				} else {
					Write-Error -Message 'Markup is invalid.'-Category ParserError -ErrorId 'Assert.ValidXamlMarkup.Failure' -TargetObject $XamlLoadResult;
				}
			} else {
				Write-Error -Message 'Markup is invalid.'-Category ParserError -ErrorId 'Assert.ValidXamlMarkup.Failure';
			}
		}
	}
}

Function Test-XamlMarkup {
	<#
		.SYNOPSIS
			Check validity of XAML markup.
 
		.DESCRIPTION
			Returns boolean value (or message) to indicate whether the XML Markup is valid.
        
		.OUTPUTS
			System.Boolean. (If -not $ReturnResultObject) True if markup is valid; otherwise False.
			Erwine.Leonard.T.WPF.XamlLoadResult. (If $ReturnResultObject) Object which contains markup parse result.
	#>
    [CmdletBinding(DefaultParameterSetName = 'ReturnBoolean')]
    Param(
		# XAML markup.
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'ReturnBoolean')]
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'ReturnObject')]
		[ValidateScript({ $_ -is [string] -or $_ -is [System.Xml.XmlDocument] })]
        [object]$Xml,
		
		# Whether to return the result object, which includes any error information, instead of a boolean value.
        [Parameter(Mandatory = $true, ParameterSetName = 'ReturnObject')]
		[switch]$ReturnResultObject
    )
    
    Process {
		$XamlLoadResult = $null;
		$success = $false;
		if ($Xml -is [string]) {
			$success = [Erwine.Leonard.T.WPF.XamlUtility]::TryParseXaml($Xml, [ref]$XamlLoadResult);
		} else {
			$success = [Erwine.Leonard.T.WPF.XamlUtility]::TryLoadXaml($Xml, [ref]$XamlLoadResult);
		}
		
		if ($ReturnResultObject) {
			if ($XamlLoadResult -ne $null) { $XamlLoadResult | Write-Output }
		} else {
			$success | Write-Output;
		}
	}
}

Function New-XamlMarkup {
	<#
		.SYNOPSIS
			Create empty markup for new window.
 
		.DESCRIPTION
			Creates template XAML markup for creating a new window.

		.OUTPUTS
			System.Xml.XmlDocument. XAML markup for a new window.
	#>
    [CmdletBinding(DefaultParameterSetName = 'ImplicitNs')]
	[OutputType([System.Xml.XmlDocument])]
    Param(
		[Parameter(ParameterSetName = 'ImplicitNs')]
		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitNs')]
		[string]$ElementName = 'Window',
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitNs')]
		[string]$NamespaceURI
	)
    
	$XmlDocument = New-Object -TypeName 'System.Xml.XmlDocument';
	
	if (-not $PSBoundParameters.ContainsKey('NamespaceURI')) { $NamespaceURI = [Erwine.Leonard.T.WPF.XamlUtility]::XmlNamespaceURI_Presentation }
	$XmlDocument.AppendChild($XmlDocument.CreateElement($ElementName, $NamespaceURI));
	
	if ($NamespaceURI -ne [Erwine.Leonard.T.WPF.XamlUtility]::XmlNamespaceURI_Xaml) {
		$XmlDocument.DocumentElement.Attributes.Append($XmlDocument.CreateAttribute("x", [Erwine.Leonard.T.WPF.XamlUtility]::XmlNamespaceURI_Xml)).Value = XmlNamespaceURI_Xaml;
	}
	
	$XmlDocument | Write-Output;
}

Function Get-XamlMarkupProperty {
    [CmdletBinding()]
	[OutputType([System.Xml.XmlNode])]
    Param(
		[Parameter(Mandatory = $true)]
		[System.Xml.XmlElement]$Parent,
		
		[Parameter(Mandatory = $true)]
		[string]$Name,
		
		[string]$NamespaceURI,
		
		[switch]$ForceElement
	)
    
	$XmlAttribute = $null;
	$XmlElement = $null;
	$XmlNamespaceManager = $null;
	$Value = $null;
	if ($PSBoundParameters.ContainsKey('NamespaceURI')) {
		$XmlNamespaceManager = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $Parent.OwnerDocument.NameTable;
		$XmlNamespaceManager.AddNamespace('ns', $NamespaceURI);
		$Result = $Parent.SelectSingleNode(('ns:{0}' -f [System.Xml.XmlConvert]::EncodeLocalName($Name)), $XmlNamespaceManager);
	} else {
		$XmlNamespaceManager = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $Parent.OwnerDocument.NameTable;
		$XmlNamespaceManager.AddNamespace('ns', $Parent.NamespaceURI);
		$Result = $Parent.SelectSingleNode(('ns:{0}.{1}' -f $Parent.LocalName, [System.Xml.XmlConvert]::EncodeLocalName($Name)), $XmlNamespaceManager);
	}
	
	if ($Result -eq $null) {
		if ($PSBoundParameters.ContainsKey('NamespaceURI')) {
			$Result = $Parent.SelectSingleNode(('@ns:{0}' -f [System.Xml.XmlConvert]::EncodeLocalName($Name)), $XmlNamespaceManager);
		} else {
			$Result = $Parent.SelectSingleNode(('@{0}' -f [System.Xml.XmlConvert]::EncodeLocalName($Name)));
		}
		if ($ForceElement -and $Result -ne $null) {
			$Value = $Result.Value;
			$Parent.Attributes.Remove($Result) | Out-Null;
			$Result = $null;
		}
	}
	
	if ($Result -ne $null) {
		$Result | Write-Output;
	} else {
		if ($ForceElement){
			if ($PSBoundParameters.ContainsKey('NamespaceURI')) {
				$Parent.AppendChild($Parent.OwnerDocument.CreateElement($Parent.OwnerDocument.GetPrefixOfNamespace($NamespaceURI), [System.Xml.XmlConvert]::EncodeLocalName($Name), $NamespaceURI)) | Write-Output;
			} else {
				$Parent.AppendChild($Parent.OwnerDocument.CreateElement(('{0}.{1}' -f $Parent.LocalName, [System.Xml.XmlConvert]::EncodeLocalName($Name)), $Parent.NamespaceURI)) | Write-Output;
			}
		}
	}
}

Function Set-XamlMarkupProperty {
    [CmdletBinding()]
	[OutputType([System.Xml.XmlNode])]
    Param(
		[Parameter(Mandatory = $true)]
		[System.Xml.XmlElement]$Parent,
		
		[Parameter(Mandatory = $true)]
		[string]$Name,
		
		[string]$NamespaceURI,
		
		[string]$Value
	)
    
	$XmlAttribute = $null;
	$XmlElement = $null;
	$XmlNamespaceManager = $null;
	
	if ($PSBoundParameters.ContainsKey('NamespaceURI')) {
		$XmlNamespaceManager = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $Parent.OwnerDocument.NameTable;
		$XmlNamespaceManager.AddNamespace('ns', $NamespaceURI);
		$Result = $Parent.SelectSingleNode(('ns:{0}' -f [System.Xml.XmlConvert]::EncodeLocalName($Name)), $XmlNamespaceManager);
	} else {
		$XmlNamespaceManager = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $Parent.OwnerDocument.NameTable;
		$XmlNamespaceManager.AddNamespace('ns', $Parent.NamespaceURI);
		$Result = $Parent.SelectSingleNode(('ns:{0}.{1}' -f $Parent.LocalName, [System.Xml.XmlConvert]::EncodeLocalName($Name)), $XmlNamespaceManager);
	}
	
	if ($Result -ne $null) {
		$Parent.RemoveChild($Result) | Out-Null;
		if (-not $PSBoundParameters.ContainsKey('Value')) { return }
	} else {
		if ($PSBoundParameters.ContainsKey('NamespaceURI')) {
			$Result = $Parent.SelectSingleNode(('@ns:{0}' -f [System.Xml.XmlConvert]::EncodeLocalName($Name)), $XmlNamespaceManager);
		} else {
			$Result = $Parent.SelectSingleNode(('@{0}' -f [System.Xml.XmlConvert]::EncodeLocalName($Name)));
		}
		
		if (-not $PSBoundParameters.ContainsKey('Value'))
		{
			if ($Result -ne $null) { $Parent.Attributes.Remove($Result) | Out-Null }
			return;
		}
	}
	
	if ($Result -eq $null) {
		if ($PSBoundParameters.ContainsKey('NamespaceURI')) {
			$Result = $Parent.Attributes.Append($Parent.OwnerDocument.CreateAttribute($Parent.OwnerDocument.GetPrefixOfNamespace($NamespaceURI), [System.Xml.XmlConvert]::EncodeLocalName($Name), $NamespaceURI));
		} else {
			$Result = $Parent.Attributes.Append($Parent.OwnerDocument.CreateAttribute([System.Xml.XmlConvert]::EncodeLocalName($Name)));
		}
	}
	
	$Result.Value = $Value;
}

Function Get-XamlMarkupContent {
    [CmdletBinding()]
	[OutputType([System.Xml.XmlNode])]
    Param(
		[Parameter(Mandatory = $true)]
		[System.Xml.XmlElement]$Parent,
		
		[Parameter(Mandatory = $true)]
		[string]$Name,
		
		[string]$NamespaceURI
	)
    
	if (-not $PSBoundParameters.ContainsKey('NamespaceURI')) { $NamespaceURI = $Parent.NamespaceURI }
	$XmlNamespaceManager = New-Object -TypeName 'System.Xml.XmlNamespaceManager' -ArgumentList $Parent.OwnerDocument.NameTable;
	$XmlNamespaceManager.AddNamespace('ns', $NamespaceURI);
	$Parent.SelectNodes(('ns:{0}' -f [System.Xml.XmlConvert]::EncodeLocalName($Name)), $XmlNamespaceManager);
}

Function Append-XamlMarkupContent {
    [CmdletBinding()]
	[OutputType([System.Xml.XmlNode])]
    Param(
		[Parameter(Mandatory = $true)]
		[System.Xml.XmlElement]$Parent,
		
		[Parameter(Mandatory = $true)]
		[string]$Name,
		
		[string]$NamespaceURI
	)
    
	if (-not $PSBoundParameters.ContainsKey('NamespaceURI')) { $NamespaceURI = $Parent.NamespaceURI }
	$Parent.AppendChild($Parent.OwnerDocument.CreateElement([System.Xml.XmlConvert]::EncodeLocalName($Name), $NamespaceURI)) | Write-Output;
}

Function New-WpfWindow {
	<#
		.SYNOPSIS
			Create WPF Window proxy object.
 
		.DESCRIPTION
			Creates a new object to represent a WPF Window.

		.OUTPUTS
			Erwine.Leonard.T.WPF.WpfWindow. New WPF window proxy object.
	#>
    [CmdletBinding()]
    Param(
		# Window XAML markup.
		[ValidateScript({ try { $_ | Assert-ValidXamlMarkup; $true } catch { throw } })]
        [object]$WindowXaml,

		# ScriptBlock to invoke before window is created.
        [ScriptBlock]$BeforeWindowCreated,

		# ScriptBlock to invoke before window is shown.
        [ScriptBlock]$BeforeWindowShown,

		# ScriptBlock to invoke before window is closed.
        [ScriptBlock]$AfterWindowClosed,

		# Data to be made available to script blocks, which is also returned in the WpfWindow proxy object in the 'SynchronizedData' property.
        [Hashtable]$SynchronizedData
	)
    
	$WpfWindow = New-Object -TypeName 'Erwine.Leonard.T.WPF.WpfWindow';
	if ($PSBoundParameters.ContainsKey('WindowXaml')) {
		if ($WindowXaml -is [string]) {
			$WpfWindow.WindowXaml = $WindowXaml.DocumentElement.OuterXml;
		} else {
			$WpfWindow.WindowXaml = $WindowXaml;
		}
	}
	if ($PSBoundParameters.ContainsKey('BeforeWindowCreated')) { $WpfWindow.BeforeWindowCreated = $BeforeWindowCreated }
	if ($PSBoundParameters.ContainsKey('BeforeWindowShown')) { $WpfWindow.BeforeWindowShown = $BeforeWindowShown }
	if ($PSBoundParameters.ContainsKey('AfterWindowClosed')) { $WpfWindow.AfterWindowClosed = $AfterWindowClosed }
	if ($PSBoundParameters.ContainsKey('SynchronizedData')) {
		foreach ($key in $SynchronizedData.Keys) {
			$WpfWindow.SynchronizedData[$key] = $SynchronizedData[$key];
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
			Erwine.Leonard.T.WPF.WpfWindow. Window that was displayed.
	#>
    [CmdletBinding(DefaultParameterSetName = 'New')]
    Param(
		# Window proxy object.
		[Parameter(Mandatory = $true, ParameterSetName = 'Existing')]
        [Erwine.Leonard.T.WPF.WpfWindow]$WindowProxy,

		# Window XAML markup.
		[Parameter(Mandatory = $true, ParameterSetName = 'New')]
		[ValidateScript({ try { $_ | Assert-ValidXamlMarkup; $true } catch { throw } })]
        [object]$WindowXaml,

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
        [Hashtable]$SynchronizedData,
		
		# Indicates that no child windows will be displayed, and this will be system modal.
		[Parameter(ParameterSetName = 'New')]
		[switch]$AsDialog
	)
    $WpfWindow = $WindowProxy;
	if (-not $PSBoundParameters.ContainsKey('WindowProxy')) {
		$WpfWindow = New-Object -TypeName 'Erwine.Leonard.T.WPF.WpfWindow';
		if ($WindowXaml -is [string]) {
			$WpfWindow.WindowXaml = $WindowXaml.DocumentElement.OuterXml;
		} else {
			$WpfWindow.WindowXaml = $WindowXaml;
		}
		if ($PSBoundParameters.ContainsKey('BeforeWindowCreated')) { $WpfWindow.BeforeWindowCreated = $BeforeWindowCreated }
		if ($PSBoundParameters.ContainsKey('BeforeWindowShown')) { $WpfWindow.BeforeWindowShown = $BeforeWindowShown }
		if ($PSBoundParameters.ContainsKey('AfterWindowClosed')) { $WpfWindow.AfterWindowClosed = $AfterWindowClosed }
		if ($PSBoundParameters.ContainsKey('SynchronizedData')) {
			foreach ($key in $SynchronizedData.Keys) {
				$WpfWindow.SynchronizedData[$key] = $SynchronizedData[$key];
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