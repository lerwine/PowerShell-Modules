using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Xml;

namespace Erwine.Leonard.T.WPF.Commands
{
    /// <summary>
    /// New-WpfWindow
    /// </summary>
    [Cmdlet(VerbsCommon.New, "WpfWindow", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(WpfWindow))]
    public class New_WpfWindow : XamlMarkupCmdlet
    {
        /*
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

         */
    }
}
