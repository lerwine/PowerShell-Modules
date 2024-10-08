$Script:Regex = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
	Whitespace = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '\s', ([System.Text.RegularExpressions.RegexOptions]::Compiled);
	UrlEncodedItem = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '(^|&)(?<key>[^&=]*)(=(?<value>[^&]*))?', ([System.Text.RegularExpressions.RegexOptions]::Compiled);
};

Function Get-SpecialFolderNames {
	<#
		.SYNOPSIS
			Get special folder names.
		 
		.DESCRIPTION
			Returns a list of names that can be used to refer to actual spcial folder paths.
		
		.OUTPUTS
			System.String[]. List of non-empty string values.
		
		.LINK
			Get-SpecialFolder
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.environment.specialfolder.aspx
	#>
	[CmdletBinding()]
	[OutputType([string[]])]
	Param()
	if ($PSVersionTable.ClrVersion.Major -lt 4) {
		[System.Enum]::GetNames([System.Environment+SpecialFolder]) + @('ProgramFilesX86', 'CommonProgramFilesX86', 'Windows');
	} else {
		[System.Enum]::GetNames([System.Environment+SpecialFolder])
	}
}

Function Get-SpecialFolder {
	<#
		.SYNOPSIS
			Get special folder path.
 
		.DESCRIPTION
			Converts special folder enumerated value to string path.
		
		.OUTPUTS
			System.String. Path of special folder.

		.EXAMPLE
			$WindowsPath = Get-SpecialFolder -Name 'Windows';

		.EXAMPLE
			$MyDocumentsPath = Get-SpecialFolder [System.Environment+SpecialFolder]::MyDocuments;
		
		.LINK
			Get-SpecialFolderNames
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.environment.specialfolder.aspx
	#>
	[CmdletBinding(DefaultParameterSetName = 'Enum')]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Enum')]
		# Enumerated folder value.
		[System.Environment+SpecialFolder]$Folder,
		
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'String')]
		[ValidateScript({(Get-SpecialFolderNames) -icontains $_})]
		# Name of special folder.
		[string]$Name
	)
	Process {
		if ($PSBoundParameters.ContainsKey('Folder')) {
			[System.Environment]::GetFolderPath($Folder);
		} else {
			if ($PSVersionTable.ClrVersion.Major -lt 4) {
				switch ($Name) {
					'CommonProgramFilesX86' { [System.Environment]::GetEnvironmentVariable('CommonProgramFiles(x86)'); break; }
					'ProgramFilesX86' { [System.Environment]::GetEnvironmentVariable('ProgramFiles(x86)'); break; }
					'Windows' { [System.Environment]::GetEnvironmentVariable('SystemRoot'); break; }
					default { [System.Environment]::GetFolderPath([System.Enum]::Parse([System.Environment+SpecialFolder], $Name, $true)); break; }
				}
			} else {
				[System.Environment]::GetFolderPath([System.Enum]::Parse([System.Environment+SpecialFolder], $Name, $true));
			}
		}
	}
}

Function Get-AppDataPath {
	<#
		.SYNOPSIS
			Get path for application data storage.
 
		.DESCRIPTION
			Constructs a path for application-specific data.

		.OUTPUTS
			System.String. Path to application data storage folder.
	#>
	[CmdletBinding(DefaultParameterSetName = 'Roaming')]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		# Name of company
		[string]$Company,

		[Parameter(Mandatory = $true, Position = 1)]
		# Name of application
		[string]$ProductName,

		[Parameter(Position = 2)]
		# Version of application
		[System.Version]$Version,

		[Parameter(Position = 3)]
		# Name of component
		[string]$ComponentName,

		# Create folder structure if it does not exist
		[switch]$Create,

		[Parameter(ParameterSetName = 'Roaming')]
		# Create folder structure under roaming profile.
		[switch]$Roaming,

		[Parameter(ParameterSetName = 'Local')]
		# Create folder structure under local profile.
		[switch]$Local,

		[Parameter(ParameterSetName = 'Common')]
		# Create folder structure under common location.
		[switch]$Common
	)
	
	Process {
		switch ($PSCmdlet.ParameterSetName) {
			'Common' { $AppDataPath = [System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::CommonApplicationData); break; }
			'Local' { $AppDataPath = [System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::LocalApplicationData); break; }
			default { $AppDataPath = [System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::ApplicationData); break; }
		}
		
		if ($Create -and (-not ($AppDataPath | Test-Path -PathType Container))) {
			throw ('Unable to find {0} path "{1}".' -f $PSCmdlet.ParameterSetName, $AppDataPath);
		}

		$AppDataPath = $AppDataPath | Join-Path -ChildPath ($Company | ConvertTo-SafeFileName);
		
		if ($Create -and (-not ($AppDataPath | Test-Path -PathType Container))) {
			New-Item -Path $AppDataPath -ItemType Directory | Out-Null;
			if (-not ($AppDataPath | Test-Path -PathType Container)) {
				throw ('Unable to create {0} company path "{1}".' -f $PSCmdlet.ParameterSetName, $AppDataPath);
			}
		}

		$N = $ProductName;
		if ($PSBoundParameters.ContainsKey('Version')) { $N = '{0}_{1}_{2}' -f $N, $Version.Major, $Version.Minor }
		$AppDataPath = $AppDataPath | Join-Path -ChildPath ($N | ConvertTo-SafeFileName);
		
		if ($Create -and (-not ($AppDataPath | Test-Path -PathType Container))) {
			New-Item -Path $AppDataPath -ItemType Directory | Out-Null;
			if (-not ($AppDataPath | Test-Path -PathType Container)) {
				throw ('Unable to create {0} product path "{1}".' -f $PSCmdlet.ParameterSetName, $AppDataPath);
			}
		}

		if ($PSBoundParameters.ContainsKey('ComponentName')) {
			$AppDataPath = $AppDataPath | Join-Path -ChildPath ($ComponentName | ConvertTo-SafeFileName -AllowExtension);
		
			if ($Create -and (-not ($AppDataPath | Test-Path -PathType Container))) {
				New-Item -Path $AppDataPath -ItemType Directory | Out-Null;
				if (-not ($AppDataPath | Test-Path -PathType Container)) {
					throw ('Unable to create {0} component path "{1}".' -f $PSCmdlet.ParameterSetName, $AppDataPath);
				}
			}
		}

		$AppDataPath | Write-Output;
	}
}

Function New-WindowOwner {
	<#
		.SYNOPSIS
			Create new window owner object.
 
		.DESCRIPTION
			Initializes a new object which implements System.Windows.Forms.IWin32Window, representing an owner window.

		.OUTPUTS
			System.Windows.Forms.IWin32Window. Path to selected file or folder.
			
		.LINK
			https://msdn.microsoft.com/en-us/library/system.windows.forms.iwin32window.aspx
			
		.LINK
			https://msdn.microsoft.com/en-us/library/system.diagnostics.process.getcurrentprocess.aspx
			
		.LINK
			https://msdn.microsoft.com/en-us/library/system.diagnostics.process.mainwindowhandle.aspx
			
		.LINK
			https://msdn.microsoft.com/en-us/library/system.windows.forms.control.handle.aspx
	#>
	[CmdletBinding()]
	[OutputType([System.Windows.Forms.IWin32Window])]
	Param(
		[Parameter(Position = 0, ValueFromPipeline = $true)]
		[Alias('HWND', 'Handle')]
		# The Win32 HWND handle of a window. If this is not specified, then the handle of the current process's main window is used.
		[System.IntPtr]$WindowHandle
	)
	
	Process {
		if ($PSBoundParameters.ContainsKey('WindowHandle')) {
			New-Object -TypeName 'IOUtility.WindowOwner' -ArgumentList $WindowHandle;
		} else {
			New-Object -TypeName 'IOUtility.WindowOwner' -ArgumentList ([System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle);
		}
	}
}

Function Test-FileDialogFilter {
	[CmdletBinding()]
	Param(
		[Paramter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowNull()]
        [AllowEmptyString()]
        [AllowEmptyCollection()]
		[string[]]$InputText
    )

    Begin {
        $Success = $true;
        if ($null -eq $Script:TestFileDialogFilterRegex) {
            $Script:TestFileDialogFilterRegex = [System.Text.RegularExpressions.Regex]::new('^[^|]+\|^[^|]+(\|[^|]+\|^[^|]+)*$', [System.Text.RegularExpressions.RegexOptions]::Compiled);
        }
    }
    Process {
        if ($Success) {
            if ($null -eq $InputText -or $InputText.Length -eq 0) {
                $Success = $false;
            } else {
                foreach ($Filter in $InputText) {
                    if ($null -eq $Filter -or -not $Script:TestFileDialogFilterRegex.IsMatch($Filter)) {
                        $Success = $false;
                        break;
                    }
                }
            }
        }
    }
    End { $Success | Write-Output }
}

Function Read-FileDialog {
	<#
		.SYNOPSIS
			Prompt user for filesystem path.
 
		.DESCRIPTION
			Uses a dialog to prompt the user for the path to a file or folder.

		.OUTPUTS
			System.String. Path to selected file or folder.
			
		.LINK
			https://msdn.microsoft.com/en-us/library/system.windows.forms.openfiledialog.aspx
			
		.LINK
			https://msdn.microsoft.com/en-us/library/system.windows.forms.savefiledialog.aspx
			
		.LINK
			https://msdn.microsoft.com/en-us/library/system.windows.forms.folderbrowserdialog.aspx

		.LINK
			New-WindowOwner
	#>
	[CmdletBinding()]
	Param(
		[Alias('FileName')]
		# Path to initially selected file or folder.
		[string]$SelectedPath,
		
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[Parameter(ParameterSetName = 'OpenFileDialog')]
		# Indicates whether the dialog box displays a warning if the user specifies a file name that does not exist.
		[bool]$CheckFileExists,
		
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[Parameter(ParameterSetName = 'OpenFileDialog')]
		# Indicates whether the dialog box displays a warning if the user specifies a path that does not exist.
		[bool]$CheckPathExists,
		
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[Parameter(ParameterSetName = 'OpenFileDialog')]
		# Indicates whether the dialog box returns the location of the file referenced by the shortcut or whether it returns the location of the shortcut (.lnk).
		[bool]$DereferenceLinks,
		
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[Parameter(ParameterSetName = 'OpenFileDialog')]
		# Indicates whether the dialog box restores the directory to the previously selected directory before closing.
		[bool]$RestoreDirectory,
		
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[Parameter(ParameterSetName = 'OpenFileDialog')]
		# Custom places to be added to the dialog.
		[System.Windows.Forms.FileDialogCustomPlace[]]$CustomPlaces,
		
		[Parameter(ParameterSetName = 'OpenFileDialog')]
		# Indicates whether the dialog box allows multiple files to be selected.
		[bool]$Multiselect,
		
		[Parameter(ParameterSetName = 'FolderBrowserDialog')]
		# Indicates whether the New Folder button appears in the folder browser dialog box.
		[bool]$ShowNewFolderButton,
		
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[Parameter(ParameterSetName = 'OpenFileDialog')]
		# Indicates whether the dialog box supports displaying and saving files that have multiple file name extensions.
		[bool]$SupportMultiDottedExtensions,
		
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[Parameter(ParameterSetName = 'OpenFileDialog')]
		# Indicates whether the dialog box accepts only valid Win32 file names.
		[bool]$ValidateNames,
		
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[Parameter(ParameterSetName = 'OpenFileDialog')]
		# Sets the index of the filter currently selected in the file dialog box.
		[int]$FilterIndex,
		
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[Parameter(ParameterSetName = 'OpenFileDialog')]
		# Indicates whether the dialog box automatically adds an extension to a file name if the user omits the extension.
		[bool]$AddExtension,
		
		[Parameter(ParameterSetName = 'FolderBrowserDialog')]
		# Sets the descriptive text displayed above the tree view control in the dialog box.
		[string]$Description,
		
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[Parameter(ParameterSetName = 'OpenFileDialog')]
		# Sets the file dialog box title
		[string]$Title,
		
		# Indicates whether the Save As dialog box displays a warning if the user specifies a file name that already exists.
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[bool]$OverwritePrompt,
		
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[Parameter(ParameterSetName = 'OpenFileDialog')]
		# Sets the initial directory displayed by the file dialog box.
		[string]$InitialDirectory,
		
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[Parameter(ParameterSetName = 'OpenFileDialog')]
        [ValidateScript({
            $sb = {
                if ($null -eq $args[0]) {
                    if ($null -eq $args[1] -or $args[1] -is [string]) { return $args[1] }
                    return "$($args[1])";
                }
                if ($args[0] -is [string]) { return $args[0] }
                return "$($args[0])";
            };
            
            foreach ($o in @($_)) {
                if ($o -is [string]) {
                    if (-not ($o | Test-FileDialogFilter)) { return $false }
                } else {
                    $s = &$sb $o.Pattern;
                    if ($null -eq $s -or ($s = $s.Trim()).Length -eq 0 -or $s.Contains('|')) { return $false }
                    $s = &$sb $o.Description $o.Label;
                    if ($null -ne $s -and ($s = $s.Trim()).Length -gt 0 -and $s.Contains('|')) { return $false }
                }
            }
            return $true;
        })]
		# Sets the current file name filter, which determines the choices that appear in the "Save as file type" or "Files of type" box in the dialog box. This can be a string containing pairs of descriptions and filters, each separated by a '|' character. This can also be a collection of Hashtable objects with a Key named 'Pattern' which contains the file matching pattern and an optional 'Description' or 'Label' key that represents the description of the pattern.
		[object[]]$Filter,
		
		# Sets the root folder where the browsing starts from.
		[Parameter(ParameterSetName = 'FolderBrowserDialog')]
		[System.Environment+SpecialFolder]$RootFolder,
		
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[Parameter(ParameterSetName = 'OpenFileDialog')]
		# Sets the default file name extension.
		[string]$DefaultExt,
		
		[Parameter(ParameterSetName = 'OpenFileDialog')]
		# Indicates whether the dialog box contains a read-only check box. 
		[bool]$ShowReadOnly,
		
		[Parameter(ParameterSetName = 'OpenFileDialog')]
		# Indicates whether the read-only check box is selected.
		[bool]$ReadOnlyChecked,
		
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[Parameter(ParameterSetName = 'OpenFileDialog')]
		# Indicates whether the Help button is displayed in the file dialog box.
		[bool]$ShowHelp,
		
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		# Indicates whether the dialog box prompts the user for permission to create a file if the user specifies a file that does not exist.
		[bool]$CreatePrompt,
		
		# Owner window. If this is not specified, then the current process's main window will be the owner.
		[System.Windows.Forms.IWin32Window]$Owner,
		
		[Parameter(ParameterSetName = 'OpenFileDialog')]
		# Use the 'Open File' dialog. This is the default, if 'Save' or 'Folder' is not specified.
		[switch]$Open,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'SaveFileDialog')]
		# Use the 'Save File' dialog.
		[switch]$Save,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'FolderBrowserDialog')]
		# Use the 'Folder Browser' dialog.
		[switch]$Folder
	)
	
	$Dialog = New-Object -TypeName ('System.Windows.Forms.{0}' -f $PSCmdlet.ParameterSetName);
	try {
		if ($PSBoundParameters.ContainsKey('SelectedPath')) {
			if ($PSCmdlet.ParameterSetName -eq 'FolderBrowserDialog') {
				$Dialog.SelectedPath = $SelectedPath
			} else {
				$Dialog.FileName = $SelectedPath
			}
		}
		if ($PSCmdlet.ParameterSetName -eq 'FolderBrowserDialog') {
			if ($PSBoundParameters.ContainsKey('ShowNewFolderButton')) { $Dialog.ShowNewFolderButton = $ShowNewFolderButton }
			if ($PSBoundParameters.ContainsKey('RootFolder')) { $Dialog.RootFolder = $RootFolder }
			if ($PSBoundParameters.ContainsKey('Description')) { $Dialog.Description = $Description }
		} else {
			if ($PSBoundParameters.ContainsKey('CheckFileExists')) { $Dialog.CheckFileExists = $CheckFileExists }
			if ($PSBoundParameters.ContainsKey('DereferenceLinks')) { $Dialog.DereferenceLinks = $DereferenceLinks }
			if ($PSBoundParameters.ContainsKey('CheckPathExists')) { $Dialog.CheckPathExists = $CheckPathExists }
			if ($PSBoundParameters.ContainsKey('RestoreDirectory')) { $Dialog.RestoreDirectory = $RestoreDirectory }
			if ($PSBoundParameters.ContainsKey('CustomPlaces')) {
				foreach ($c in $CustomPlaces) { $Dialog.CustomPlaces.Add($c) }
			}
			if ($PSBoundParameters.ContainsKey('SupportMultiDottedExtensions')) { $Dialog.SupportMultiDottedExtensions = $SupportMultiDottedExtensions }
			if ($PSBoundParameters.ContainsKey('ValidateNames')) { $Dialog.ValidateNames = $ValidateNames }
			if ($PSBoundParameters.ContainsKey('FilterIndex')) { $Dialog.FilterIndex = $FilterIndex }
			if ($PSBoundParameters.ContainsKey('DefaultExt')) { $Dialog.DefaultExt = $DefaultExt }
			if ($PSBoundParameters.ContainsKey('ShowHelp')) { $Dialog.ShowHelp = $ShowHelp }
			if ($PSBoundParameters.ContainsKey('Title')) { $Dialog.Title = $Title }
			if ($PSBoundParameters.ContainsKey('InitialDirectory')) { $Dialog.InitialDirectory = $InitialDirectory }
			if ($PSBoundParameters.ContainsKey('Filter')) {
                $Dialog.Filter = (@($Filter) | ForEach-Object {
                    if ($_ -is [string]) {
                        $_
                    } else {
                        $d = $_.Description;
                        if ($null -eq $d) { $d = $_.Label }
                        if ($d -isnot [string]) { $d = $d.ToString() }
                        $e = $_.Pattern;
                        if ($e -isnot [string]) { $e = $e.ToString() }
                        "$d|$e"
                    }
                }) -join '|';
            }
			if ($PSBoundParameters.ContainsKey('AddExtension')) { $Dialog.AddExtension = $AddExtension }
			if ($PSCmdlet.ParameterSetName -eq 'OpenFileDialog') {
				if ($PSBoundParameters.ContainsKey('Multiselect')) { $Dialog.Multiselect = $Multiselect }
				if ($PSBoundParameters.ContainsKey('ShowReadOnly')) { $Dialog.ShowReadOnly = $ShowReadOnly }
				if ($PSBoundParameters.ContainsKey('ReadOnlyChecked')) { $Dialog.ReadOnlyChecked = $ReadOnlyChecked }
			} else {
				if ($PSBoundParameters.ContainsKey('OverwritePrompt')) { $Dialog.OverwritePrompt = $OverwritePrompt }
			}
		}
		
		if (-not $PSBoundParameters.ContainsKey('Owner')) { $Owner = New-WindowOwner }

		if ($Dialog.ShowDialog($Owner) -eq [System.Windows.Forms.DialogResult]::OK) {
			switch ($PSCmdlet.ParameterSetName) {
				'FolderBrowserDialog' {
					$Dialog.SelectedPath | Write-Output;
					break;
				}
				'SaveFileDialog' {
					$Dialog.FileName | Write-Output;
					break;
				}
				default {
					if ($Dialog.Multiselect) {
						$Dialog.FileNames | Write-Output;
					} else {
						$Dialog.FileName | Write-Output;
					}
					break;
				}
			}
		}
	} catch {
		throw;
	} finally {
		$Dialog.Dispose();
	}
}