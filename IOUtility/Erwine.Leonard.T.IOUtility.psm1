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

Function ConvertTo-SafeFileName {
	<#
		.SYNOPSIS
			Converts a string to a usable file name / path.
 
		.DESCRIPTION
			Encodes a string in a format which is compatible with a file name / path, and can be converted back to the original text.
		
		.OUTPUTS
			System.String. Text encoded as a valid file name / path.

		.EXAMPLE
			ConvertTo-SafeFileName -InputText 'My *unsafe* file';

		.EXAMPLE
			'c:\my*path\User.string' | ConvertTo-SafeFileName -FullPath;
		
		.LINK
			ConvertFrom-SafeFileName
	#>
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		# String to convert to file name
		[string[]]$InputText,
		
		[Parameter(ParameterSetName = 'FileName')]
		# Only allow file names. This is the default behavior.
		[switch]$FileName,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'RelativePath')]
		# Only allow relative paths
		[switch]$RelativePath,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'FullPath')]
		# Allow full path specification
		[switch]$FullPath
	)
	
	Begin {
		switch ($PSCmdlet.ParameterSetName){
			'RelativePath' {
				$EncodeRegexReplaceHandler = New-Object -TypeName 'IOUtility.EncodeRegexReplaceHandler' -ArgumentList ([IOUtility.RegularExpressions]::InvalidRelativePathChars);
				break;
			}
			'FullPath' {
				$EncodeRegexReplaceHandler = New-Object -TypeName 'IOUtility.EncodeRegexReplaceHandler' -ArgumentList ([IOUtility.RegularExpressions]::InvalidPathChars);
				break;
			}
			default {
				$EncodeRegexReplaceHandler = New-Object -TypeName 'IOUtility.EncodeRegexReplaceHandler' -ArgumentList ([IOUtility.RegularExpressions]::InvalidFileNameChars);
				break;
			}
		}
	}
	
	Process { foreach ($Text in $InputText) { $EncodeRegexReplaceHandler.Replace($Text) } }
}

Function ConvertFrom-SafeFileName {
	<#
		.SYNOPSIS
			Decodes a file name back to the original text.
 
		.DESCRIPTION
			If a file name was creating using 'ConvertTo-SafeFileName', this method will convert it back.

		.OUTPUTS
			System.String. Encoded file name decoded to its original text.

		.EXAMPLE
			'File_0x005F_Name' | ConvertFrom-SafeFileName;

		.LINK
				ConvertTo-SafeFileName
	#>
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		# File name to decode
		[string[]]$InputText
	)
	
	Begin {
		$DecodeRegexReplaceHandler = New-Object -TypeName 'IOUtility.DecodeRegexReplaceHandler';
	}
	
	Process { foreach ($Text in $InputText) { $DecodeRegexReplaceHandler.Replace($Text) } }
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
        if ($Script:TestFileDialogFilterRegex -eq $null) {
            $Script:TestFileDialogFilterRegex = [System.Text.RegularExpressions.Regex]::new('^[^|]+\|^[^|]+(\|[^|]+\|^[^|]+)*$', [System.Text.RegularExpressions.RegexOptions]::Compiled);
        }
    }
    Process {
        if ($Success) {
            if ($InputText -eq $null -or $InputText.Length -eq 0) {
                $Success = $false;
            } else {
                foreach ($Filter in $InputText) {
                    if ($Filter -eq $null -or -not $Script:TestFileDialogFilterRegex.IsMatch($Filter)) {
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
                $s = $null;
                if ($args[0] -eq $null) {
                    if ($args[1] -eq $null -or $args[1] -is [string]) { return $args[1] }
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
                    if ($s -eq $null -or ($s = $s.Trim()).Length -eq 0 -or $s.Contains('|')) { return $false }
                    $s = &$sb $o.Description $o.Label;
                    if ($s -ne $null -and ($s = $s.Trim()).Length -gt 0 -and $s.Contains('|')) { return $false }
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
                        if ($d -eq $null) { $d = $_.Label }
                        if ($d -isnot [string]) { $d = $d.ToString() }
                        $e = $_.Extension;
                        if ($e -eq $null) { $e = $_.Ext }
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

Function Get-MinBase64BlockSize {
	<#
		.SYNOPSIS
			Get minimum base-64 encoding block size.
 
		.DESCRIPTION
			Get minimum base-64 encoding block size when you intend on emitting line-separated chunks of base64-encoded data.
			
		.OUTPUTS
			System.Int32. Minimum block size for line-separated chunks of base64-encoded data.
	#>
	[CmdletBinding()]
	[OutputType([int])]
	Param()
	
	return [IOUtility.StreamHelper]::MinBase64BlockSize;
}

Function Read-IntegerFromStream {
	<#
		.SYNOPSIS
			Read integer value from a stream.
 
		.DESCRIPTION
			Reads bytes from a stream and converts them to an integer.
			
		.OUTPUTS
			System.Int32. Integer value read from stream.
		
		.LINK
			Write-IntegerToStream
		
		.LINK
			Read-LongIntegerFromStream
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	[OutputType([int])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		# Stream from which to read the bytes of an integer value.
		[System.IO.Stream]$Stream
	)
	
	return [IOUtility.StreamHelper]::ReadInteger($Stream);
}

Function Read-LongIntegerFromStream {
	<#
		.SYNOPSIS
			Read long integer value from a stream.
 
		.DESCRIPTION
			Reads bytes from a stream and converts them to a long integer.
			
		.OUTPUTS
			System.Int64. Long Integer value read from stream.
		
		.LINK
			Write-LongIntegerToStream
		
		.LINK
			Read-IntegerFromStream
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	[OutputType([long])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		# Stream from which to read the bytes of a long integer value.
		[System.IO.Stream]$Stream
	)
	
	return [IOUtility.StreamHelper]::ReadLongInteger($Stream);
}

Function Write-IntegerToStream {
	<#
		.SYNOPSIS
			Write integer value to a stream.
 
		.DESCRIPTION
			Writes an integer value to the Stream as an array of bytes.
		
		.LINK
			Read-IntegerFromStream
		
		.LINK
			Write-LongIntegerToStream
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		# Stream to write integer value to
		[System.IO.Stream]$Stream,
		
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		# Integer value to be written
		[int]$Value
	)
	
	Process { [IOUtility.StreamHelper]::WriteInteger($Stream, $Value) }
}

Function Write-LongIntegerToStream {
	<#
		.SYNOPSIS
			Write long integer value to a stream.
 
		.DESCRIPTION
			Writes a long integer value to the Stream as an array of bytes.
		
		.LINK
			Read-LongIntegerFromStream
		
		.LINK
			Write-IntegerToStream
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		# Stream to write long integer value to
		[System.IO.Stream]$Stream,
		
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		# Long Integer value to be written
		[long]$Value
	)
	
	Process { [IOUtility.StreamHelper]::WriteLongInteger($Stream, $Value) }
}

Function Read-LengthEncodedBytes {
	<#
		.SYNOPSIS
			Read length-encoded array of bytes from a stream.
 
		.DESCRIPTION
			Reads a length value from the Stream, and then reads the associated number of bytes.
			
		.OUTPUTS
			System.Byte[]. Array of length-encoded bytes read from stream.
		
		.LINK
			Write-LengthEncodedBytes
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	[OutputType([System.Byte[]])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		# Stream to read length-encoded data from.
		[System.IO.Stream]$Stream
	)

	return ,[IOUtility.StreamHelper]::ReadLengthEncodedBytes($Stream);
}

Function Write-LengthEncodedBytes {
	<#
		.SYNOPSIS
			Writes length-encoded data a stream.
 
		.DESCRIPTION
			Writes a length-encoded byte array to the Stream.
		
		.LINK
			Read-LengthEncodedBytes
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true)]
		# Stream to write length-encoded data from
		[System.IO.Stream]$Stream,

		[Parameter(Mandatory = $true)]
		# Bytes to write
		[byte[]]$Bytes,

		[Parameter(Mandatory = $false)]
		# Offset within the array of bytes to be writing
		[int]$Offset = 0,

		[Parameter(Mandatory = $false)]
		# Number of bytes to write
		[int]$Count
	)

	if ($PSBoundParameters.ContainsKey('Offset') -or $PSBoundParameters.ContainsKey('Count')) {
		if ($PSBoundParameters.ContainsKey('Count')) {
			[IOUtility.StreamHelper]::WriteLengthEncodedBytes($Stream, $Bytes, $Offset, $Count);
		} else {
			[IOUtility.StreamHelper]::WriteLengthEncodedBytes($Stream, $Bytes, $Offset, $Bytes.Length - $Offset);
		}
	} else {
		[IOUtility.StreamHelper]::WriteLengthEncodedBytes($Stream, $Bytes);
	}
}

Function ConvertTo-Base64String {
	<#
		.SYNOPSIS
			Convert data buffer to base-64 encoded text.
 
		.DESCRIPTION
			Converts the contents of a data buffer to line-separated base64-encoded text.

		.OUTPUTS
			System.String. Line-separated base64-encoded data.
		
		.LINK
			ConvertFrom-Base64String
	#>
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true)]
		# Data buffer to be converted to base-64 encoded text.
		[byte[]]$Buffer,
		
		[Parameter(Mandatory = $false)]
		# Offset within data buffer, in bytes, to begin encoding.
		[int]$Offset = 0,
		
		[Parameter(Mandatory = $false)]
		# Number of bytes to encode
		[int]$Length,
		
		[Parameter(Mandatory = $false)]
		# Whether to insert line breaks
		[switch]$InsertLineBreaks
	)
	
	$l = $Length;
	if ($PSBoundParameters.ContainsKey('Length')) {
		$l = $Length;
	} else {
		$l = $Buffer.Length - $Offset;
	}
	if ($InsertLineBreaks) {
		[System.Convert]::ToBase64String($Buffer, $Offset, $Length, [System.Base64FormattingOptions]::InsertLineBreaks);
	} else {
		[System.Convert]::ToBase64String($Buffer, $Offset, $Length, [System.Base64FormattingOptions]::None);
	}
}

Function ConvertFrom-Base64String {
	<#
		.SYNOPSIS
			Convert base-64 encoded text to a data buffer.
 
		.DESCRIPTION
			Converts the base-64 encoded text to a data buffer object.

		.OUTPUTS
			System.Byte[]. Array of bytes decoded.
		
		.LINK
			ConvertTo-Base64String
	#>
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true)]
		[ValidatePattern('^\s*[a-zA-Z\d+/]*((\r\n?|\n)[a-zA-Z\d+/]*)*((\r\n?|\n)?==?)?')]
		# Base-64 encoded text
		[string]$InputString,
		
		[Parameter(Mandatory = $false)]
		[ValidateRange(1, 2147483647)]
		# Minimum capacity, in bytes, of the returned data buffer.
		[int]$MinCapacity
	)
	
	$Buffer = [System.Convert]::FromBase64String($InputString);
	if ($PSBoundParameters.ContainsKey('MinCapacity') -and $MinCapacity -gt $Buffer.Length) {
		[System.Array]::Resize([ref]$Buffer, $MinCapacity);
	}

	return ,$Buffer;
}

Function Get-TextEncoding {
	<#
		.SYNOPSIS
			Gets an instance of the Encoding class.
 
		.DESCRIPTION
			Gets an instance of the Encoding class, which represents a character encoding.
		
		.OUTPUTS
			System.Text.Encoding. Represents the character encoding.
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.text.encoding.aspx
	#>
	[CmdletBinding(DefaultParameterSetName = 'Name')]
	[OutputType([System.Text.Encoding])]
	Param(
		[Parameter(Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Name')]
		# The code page name of the encoding to be returned. Any value returned by the WebName property is valid.
		[string]$Name,

		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Codepage')]
		# The code page identifier of the encoding to be returned.
		[int]$Codepage,

		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'ContentType')]
		# Get encoding from mime type
		[System.Net.Mime.ContentType]$ContentType,

		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'XmlDocument')]
		# Get encoding from XML document's xml declaration.
		[System.Xml.XmlDocument]$Xml,
		
		[Parameter(ParameterSetName = 'XmlDocument')]
		[Parameter(ParameterSetName = 'ContentType')]
		# Default encoding to use if the encoding could not be determined.
		[System.Text.Encoding]$DefaultValue = [System.Text.Encoding]::UTF8,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'UTF8')]
		# Gets an encoding for the UTF-8 format.
		[switch]$UTF8,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ASCII')]
		# Gets an encoding for the ASCII (7-bit) character set.
		[switch]$ASCII,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'BigEndianUnicode')]
		# Gets an encoding for the UTF-16 format that uses the big endian byte order.
		[switch]$BigEndianUnicode,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Unicode')]
		# Gets an encoding for the UTF-16 format using the little endian byte order.
		[Alias('UTF16')]
		[switch]$Unicode,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'UTF32')]
		# Gets an encoding for the UTF-32 format using the little endian byte order.
		[switch]$UTF32,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'UTF7')]
		# Gets an encoding for the UTF-7 format.
		[switch]$UTF7,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Default')]
		# Gets an encoding for the operating system's current ANSI code page.
		[switch]$Default
	)

	Process {
		switch ($PSCmdlet.ParameterSetName) {
			'UTF8' { [System.Text.Encoding]::UTF8; break; }
			'ASCII' { [System.Text.Encoding]::ASCII; break; }
			'BigEndianUnicode' { [System.Text.Encoding]::BigEndianUnicode; break; }
			'Unicode' { [System.Text.Encoding]::Unicode; break; }
			'UTF32' { [System.Text.Encoding]::UTF32; break; }
			'UTF7' { [System.Text.Encoding]::UTF7; break; }
			'Default' { [System.Text.Encoding]::Default; break; }
			'Codepage' { [System.Text.Encoding]::GetEncoding($Codepage); break; }
			'ContentType' {
				if ([System.String]::IsNullOrEmpty($ContentType.CharSet)) {
					$DefaultValue | Write-Output;
				} else {
					Get-TextEncoding -Name $ContentType.CharSet;
				}
			}
			'XmlDocument' {
				$XmlDeclaration = $null;
				for ($Node = $Xml.FirstChild; $Node -ne $null; $Node = $Node.NextSibling) {
					if ($Node.NodeType -eq [System.Xml.XmlNodeType]::XmlDeclaration) {
						$XmlDeclaration = $Node;
						break;
					}
				}
				if ($XmlDeclaration -eq $null) {
					$DefaultValue | Write-Output;
				} else {
					Get-TextEncoding -Name $XmlDeclaration.Encoding;
				}
			}
			default {
				if ($PSBoundParameters.ContainsKey('Name')) {
					[System.Text.Encoding]::GetEncoding($Name);
				} else {
					[System.Text.Encoding]::GetEncodings();
				}
			}
		}
	}
}

Function New-MemoryStream {
	<#
		.SYNOPSIS
			Creates a stream whose backing store is memory.
 
		.DESCRIPTION
			Initializes a new instance of the MemoryStream class.
		
		.OUTPUTS
			System.IO.MemoryStream. The stream whose backing store is memory.
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.memorystream.aspx
	#>
	[CmdletBinding(DefaultParameterSetName = 'Opt')]
	[OutputType([System.IO.MemoryStream])]
	Param(
		[Parameter(Position = 0, ParameterSetName = 'Opt')]
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Buffer')]
		[AllowEmptyCollection()]
		[Alias('Bytes')]
		# Initializes a new non-resizable instance of the MemoryStream class based an array of bytes.
		[byte[]]$Buffer,

		[Parameter(Position = 1, ParameterSetName = 'Buffer')]
		# The index into buffer at which the stream begins.
		[int]$Index = 0,
		
		[Parameter(Position = 2, ParameterSetName = 'Buffer')]
		# The length of the stream in bytes.
		[int]$Count,
		
		[Parameter(Position = 3, ParameterSetName = 'Buffer')]
		# The setting of the CanWrite property, which determines whether the stream supports writing.
		[bool]$CanWrite = $true,
		
		[Parameter(Position = 4, ParameterSetName = 'Buffer')]
		# $true to enable GetBuffer, which returns the unsigned byte array from which the stream was created; otherwise, $false.
		[bool]$PubliclyVisible
	)

	if ($PSBoundParameters.ContainsKey('Index') -or $PSBoundParameters.ContainsKey('PubliclyVisible') -and -not $PSBoundParameters.ContainsKey('Count')) {
		if ($Index -lt $Buffer.Length) {
			$Count = $Buffer.Length - $Index;
		} else {
			$Count = 0;
		}
	}
	
	if ($PSBoundParameters.ContainsKey('Buffer')) {
		if ($PSBoundParameters.ContainsKey('CanWrite')) {
			if ($PSBoundParameters.ContainsKey('PubliclyVisible')) {
				New-Object -TypeName 'System.IO.MemoryStream' -ArgumentList (,$Buffer), $Index, $Count, $CanWrite, $PubliclyVisible;
			} else {
				if ($PSBoundParameters.ContainsKey('Index') -or $PSBoundParameters.ContainsKey('Count')) {
					New-Object -TypeName 'System.IO.MemoryStream' -ArgumentList (,$Buffer), $Index, $Count, $CanWrite;
				} else {
					New-Object -TypeName 'System.IO.MemoryStream' -ArgumentList (,$Buffer), $CanWrite;
				}
			}
		} else {
			if ($PSBoundParameters.ContainsKey('Index') -or $PSBoundParameters.ContainsKey('Count')) {
				New-Object -TypeName 'System.IO.MemoryStream' -ArgumentList (,$Buffer), $Index, $Count;
			} else {
				New-Object -TypeName 'System.IO.MemoryStream' -ArgumentList (,$Buffer);
			}
		}
	} else {
		New-Object -TypeName 'System.IO.MemoryStream';
	}
}

Function New-DataTable {
	<#
		.SYNOPSIS
			Creates a new DataTable object.
 
		.DESCRIPTION
			Initializes a new instance of the System.Data.DataTable class.
		
		.OUTPUTS
			System.Data.DataTable. The new DataTable object.
		
		.LINK
			Add-DataColumn

		.LINK
			https://msdn.microsoft.com/en-us/library/system.data.datatable.aspx
	#>
	[CmdletBinding(DefaultParameterSetName = 'Opt')]
	[OutputType([System.Data.DataTable])]
	Param(
		[Parameter(Position = 0, ParameterSetName = 'Opt')]
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Namespace')]
		[Alias('Name')]
		# The name to give the table.
		[string]$TableName,

		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Namespace')]
		[Alias('Namespace')]
		# The namespace for the XML representation of the data stored in the DataTable.
		[string]$TableNamespace,

		[Parameter(Position = 2, ParameterSetName = 'Namespace')]
		# The namespace prefix for the XML representation of the data stored in the System.Data.DataTable
		[string]$Prefix,

		# Indicates whether string comparisons within the table are case-sensitive
		[switch]$CaseSensitive
	)
	
	if ($PSBoundParameters.ContainsKey('TableName')) {
		if ($PSBoundParameters.ContainsKey('TableNamespace')) {
			$DataTable = New-Object -TypeName 'System.Data.DataTable' -ArgumentList $TableName, $TableNamespace;
		} else {
			$DataTable = New-Object -TypeName 'System.Data.DataTable' -ArgumentList $TableName;
		}
	} else {
		$DataTable = New-Object -TypeName 'System.Data.DataTable';
	}

	$DataTable.CaseSensitive = $CaseSensitive.IsPresent;
	
	if ($PSBoundParameters.ContainsKey('Prefix')) { $DataTable.Prefix = $Prefix }
}

Function Add-DataColumn {
	<#
		.SYNOPSIS
			Adds a new DataColumn object to a DataTable.
 
		.DESCRIPTION
			Creates a new instance of the System.Data.DataColumn class and adds it to a System.Data.DataTable object.
		
		.OUTPUTS
			System.Data.DataColumn. The new DataColumn which was added to the DataTable objedct.
		
		.LINK
			New-DataTable

		.LINK
			https://msdn.microsoft.com/en-us/library/system.data.datacolumn.aspx

		.LINK
			https://msdn.microsoft.com/en-us/library/system.data.datatable.columns.aspx

		.LINK
			https://msdn.microsoft.com/en-us/library/system.data.datacolumncollection.add.aspx
	#>
	[CmdletBinding(DefaultParameterSetName = 'Opt')]
	[OutputType([System.Data.DataColumn])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		[Alias('Table')]
		# DataTable to add the DataColumn to.
		[System.Data.DataTable]$DataTable,

		[Parameter(Position = 1, ParameterSetName = 'Opt')]
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Expression')]
		[Alias('Name')]
		# A string that represents the name of the column to be added
		[string]$ColumnName,

		[Parameter(Position = 2, ParameterSetName = 'Opt')]
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Expression')]
		# A supported column type.
		[System.Type]$DataType,

		[Parameter(Mandatory = $true, Position = 3, ParameterSetName = 'Expression')]
		[Alias('Expression')]
		# An expression to calculate the value of a column, or create an aggregate column. The return type of an expression is determined by the System.Data.DataColumn.DataType of the column
		[string]$Expr,

		[Parameter(Position = 4, ParameterSetName = 'Expression')]
		[Alias('MappingType')]
		# One of the System.Data.MappingType values
		[System.Data.MappingType]$Type,
		
		# The caption for the column.
		[string]$Caption,
		
		# Indicates whether null values are allowed in this column for rows that belong to the table.
		[switch]$AllowDBNull,

		# Indicates whether the column automatically increments the value of the column for new rows added to the table.
		[switch]$AutoIncrement,

		# The starting value for a column when AutoIncrement is set.
		[long]$AutoIncrementSeed,

		# The increment used by a column when AutoIncrement is set.
		[long]$AutoIncrementStep,
		
		# The DateTimeMode for the column.
		[System.Data.DataSetDateTime]$DateTimeMode,

		[AllowNull()]
		[AllowEmptyString()]
		# The default value for the column when you are creating new rows.
		[object]$DefaultValue,

		# The maximum length of a text column.
		[int]$MaxLength,

		# The position of the column in the System.Data.DataColumnCollection collection.
		[int]$Ordinal,

		# Indicates whether the column allows for changes as soon as a row has been added to the table.
		[switch]$ReadOnly,

		# Indicates whether the values in each row of the column must be unique.
		[switch]$Unique,

		# Indicates whether the new data column is returned.
		[switch]$PassThru
	)
	
	if ($PSCmdlet.ParameterSetName -eq 'Expression') {
		if ($PSBoundParameters.ContainsKey('Type')) {
			$DataColumn = New-Object -TypeName 'System.Data.DataColumn' -ArgumentList $ColumnName, $DataType, $Expr, $Type;
		} else {
			$DataColumn = New-Object -TypeName 'System.Data.DataColumn' -ArgumentList $ColumnName, $DataType, $Expr;
		}
	}
	if ($PSBoundParameters.ContainsKey('DataType')) {
		$DataColumn = New-Object -TypeName 'System.Data.DataColumn' -ArgumentList $ColumnName, $DataType;
	} else {
		if ($PSBoundParameters.ContainsKey('ColumnName')) {
			$DataColumn = New-Object -TypeName 'System.Data.DataColumn' -ArgumentList $ColumnName;
		} else {
			$DataColumn = New-Object -TypeName 'System.Data.DataColumn';
		}
	}
	
	if ($PSBoundParameters.ContainsKey('Prefix')) { $DataTable.Prefix = $Prefix }

	$DataTable.Columns.Add($DataColumn);
	
	$DataColumn.AllowDBNull = $AllowDBNull.IsPresent;
	$DataColumn.AutoIncrement = $AutoIncrement.IsPresent;
	$DataColumn.ReadOnly = $ReadOnly.IsPresent;
	$DataColumn.Unique = $Unique.IsPresent;
	if ($PSBoundParameters.ContainsKey('Caption')) { $DataColumn.Caption = $Caption }
	if ($PSBoundParameters.ContainsKey('AutoIncrementSeed')) { $DataColumn.AutoIncrementSeed = $AutoIncrementSeed }
	if ($PSBoundParameters.ContainsKey('AutoIncrementStep')) { $DataColumn.AutoIncrementStep = $AutoIncrementStep }
	if ($PSBoundParameters.ContainsKey('DateTimeMode')) { $DataColumn.DateTimeMode = $DateTimeMode }
	if ($PSBoundParameters.ContainsKey('DefaultValue')) { $DataColumn.DefaultValue = $DefaultValue }
	if ($PSBoundParameters.ContainsKey('MaxLength')) { $DataColumn.MaxLength = $MaxLength }
	if ($PSBoundParameters.ContainsKey('Ordinal')) { $DataColumn.Ordinal = $Ordinal }
	if ($PassThru) { $DataColumn | Write-Output }
}

Function Test-IsNullOrWhitespace {
	<#
		.SYNOPSIS
			Tests if string is null or whitespace.
 
		.DESCRIPTION
			This is intended to act the same as [System.String]::IsNullOrWhitespace() from later .NET versions.
		
		.OUTPUTS
			System.Boolean. Indicates whether the input text was null, empty or whitespace.
		
		.LINK
			Out-NormalizedText
		
		.LINK
			Split-DelimitedText
		
		.LINK
			Out-IndentedText
		
		.LINK
			Out-UnindentedText
		
		.LINK
			Get-IndentLevel
	#>
	[CmdletBinding()]
	[OutputType([bool])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[AllowEmptyString()]
		[AllowNull()]
		# String to be tested whether it is null, empty or consists only of whitespace.
		[string]$InputString
	)
	
	Process {
		if ($InputString -eq $null -or $InputString -eq '') {
			$false | Write-Output;
		} else {
			$IsWhiteSpace = $true;
			foreach ($c in $InputString.ToCharArray()) {
				if (-not [System.Char]::IsWhiteSpace($c)) {
					$IsWhiteSpace = $false;
					break;
				}
			}
			$IsWhiteSpace | Write-Output;
		}
	}               
}

Function Split-DelimitedText {
	<#
		.SYNOPSIS
			Splits text by delimiter.
 
		.DESCRIPTION
			Splits text according to a delimiter pattern.
		
		.OUTPUTS
			System.String[]. The text separated by delimiters.
		
		.LINK
			Out-NormalizedText
		
		.LINK
			Out-IndentedText
		
		.LINK
			Out-UnindentedText
		
		.LINK
			Get-IndentLevel
		
		.LINK
			Test-IsNullOrWhitespace
	#>
	[CmdletBinding()]
	[OutputType([string[]])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[AllowEmptyString()]
		# Text to be split by delimiter pattern.
		[string[]]$InputString,
		
		[ValidateScript({
			try {
				$Regex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList $_;
			} catch {
				$Regex = $null;
			}
			$Regex -ne $null
		})]
		[Alias('DelimiterPattern', 'Delimiter')]
		# Pattern to use for detecting newlines. Default is newline ('\r\n?|\n').
		[string]$Pattern = '\r\n?|\n',
		
		[Alias('RegexOptions', 'RegexOption', 'Option')]
		# Options for the delimiter pattern. Note: You can use "Compiled" to optimize for large pipelines.
		[System.Text.RegularExpressions.RegexOptions[]]$PatternOption
	)
	
	Begin {
		if ($PSBoundParameters.ContainsKey('PatternOption')) {
			$RegexOptions = $PatternOption[0];
			for ($i = 1; $i -lt $PatternOption.Length; $i++) {
				[System.Text.RegularExpressions.RegexOptions]$RegexOptions = $RegexOptions -bor  $PatternOption[$i];
			}
			$Regex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList $NewLinePattern, $RegexOptions;
		} else {
			$Regex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList $NewLinePattern;
		}
	}
	
	Process { $InputString | ForEach-Object { if ($_ -eq '') { $_ | Write-Output } else { $Regex.Split($_) | Write-Output } } }
}

Function Out-NormalizedText {
	<#
		.SYNOPSIS
			Normalizes text.
 
		.DESCRIPTION
			Normalizes input strings.
		
		.OUTPUTS
			System.String. The normalized text.
		
		.LINK
			Split-DelimitedText
		
		.LINK
			Out-IndentedText
		
		.LINK
			Out-UnindentedText
		
		.LINK
			Get-IndentLevel
		
		.LINK
			Test-IsNullOrWhitespace
	#>
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[AllowEmptyString()]
		# Text to be split by delimiter pattern.
		[string]$InputString,
		
		[Parameter(Position = 1, ParameterSetName = 'ByPattern')]
		[ValidateScript({
			try {
				$Regex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList $_;
			} catch {
				$Regex = $null;
			}
			$Regex -ne $null
		})]
		[Alias('DelimiterPattern', 'Delimiter')]
		# Pattern to use for normalizing text. Default is multi-whitespace: '(?(?= )\s{2,}|\s+)'.
		[string]$Pattern = '(?(?= )\s{2,}|\s+)',
		
		[Parameter(Position = 2, ParameterSetName = 'ByPattern')]
		[Alias('Replace')]
		# Text to replace where Pattern matches
		[string]$ReplaceWith = ' ',
		
		[Parameter(ParameterSetName = 'ByPattern')]
		[Alias('RegexOptions', 'RegexOption', 'Option')]
		# Options for the normalization pattern. Note: You can use "Compiled" to optimize for large pipelines.
		[System.Text.RegularExpressions.RegexOptions[]]$PatternOption,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Trim')]
		[switch]$Trim,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'TrimStart')]
		[switch]$TrimStart,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'TrimEnd')]
		[switch]$TrimEnd
	)
	
	Begin {
		if ($PSCmdlet.ParameterSetName -eq 'ByPattern') {
			if ($PSBoundParameters.ContainsKey('PatternOption')) {
				$RegexOptions = $PatternOption[0];
				for ($i = 1; $i -lt $PatternOption.Length; $i++) {
					[System.Text.RegularExpressions.RegexOptions]$RegexOptions = $RegexOptions -bor  $PatternOption[$i];
				}
				$Regex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList $NewLinePattern, $RegexOptions;
			} else {
				$Regex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList $NewLinePattern;
			}
		}
	}
	
	Process {
		if ($PSCmdlet.ParameterSetName -eq 'ByPattern') {
			$Regex.Replace($InputString, $ReplaceWith) | Write-Output
		} else {
			if ($InputString -eq '') { 
				$InputString | Write-Output;
			} else {
				if ($Trim) {
					$InputString.Trim() | Write-Output;
				} else {
					if ($TrimStart) { $InputString.TrimStart() | Write-Output } else {  $InputString.TrimEnd() | Write-Output }
				}
			}
		}
	}
}

Function Out-IndentedText {
	<#
		.SYNOPSIS
			Indents text.
 
		.DESCRIPTION
			Prepends indent text to input text.
		
		.OUTPUTS
			System.String. The indented text.
		
		.LINK
			Out-UnindentedText
		
		.LINK
			Get-IndentLevel
		
		.LINK
			Out-NormalizedText
		
		.LINK
			Split-DelimitedText
	#>
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[AllowEmptyString()]
		# Text to be indented.
		[string]$InputString,
		
		# Number of times to indent text
		[int]$Level = 1,
		
		# Text to use for indenting. Default is 4 spaces.
		[string]$IndentText = '    ',
		
		# Indicates that zero-length lines are to be indented as well.
		[switch]$IndentEmptyLine
	)
	
	Begin {
		$Indent = $IndentText;
		for ($i = 1; $i -lt $Level; $i++) { $Indent += $IndentText }
	}
	
	Process {
		if ($Level -gt 0 -and ($IndentEmptyLine -or $InputString -ne '')) {
			($Indent + $InputString) | Write-Output;
		} else {
			$InputString | Write-Output;
		}
	}
}

Function Get-IndentLevel {
	<#
		.SYNOPSIS
			Get number of times text is indented.
 
		.DESCRIPTION
			Determines number of times text has been indented.
		
		.OUTPUTS
			System.Int32. The number of indentations detected.
		
		.LINK
			Out-IndentedText
		
		.LINK
			Out-UnindentedText
		
		.LINK
			Split-DelimitedText
		
		.LINK
			Out-NormalizedText
	#>
	[CmdletBinding(DefaultParameterSetName = 'ByPattern')]
	[OutputType([int])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[AllowEmptyString()]
		# Text to be un-indented.
		[string]$InputString,
		
		[Parameter(ParameterSetName = 'ByPattern')]
		[AllowEmptyString()]
		# Pattern to detect indentation. Default is tab or 1 to 4 whitespaces at the beginning of the text: '^(\t|[^\S\t]{1,4})'
		[string]$Pattern = '^(\t|[^\S\t]{1,4})',
		
		[Parameter(ParameterSetName = 'ByPattern')]
		[Alias('RegexOptions', 'RegexOption', 'Option')]
		# Options for the indent detection pattern. Note: You can use "Compiled" to optimize for large pipelines.
		[System.Text.RegularExpressions.RegexOptions[]]$PatternOption,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ByString')]
		# Text which represents an indentation.
		[string]$IndentText
	)
	
	Begin {
		if ($PSCmdlet.ParameterSetName -eq 'ByPattern') {
			if ($PSBoundParameters.ContainsKey('PatternOption')) {
				$RegexOptions = $PatternOption[0];
				for ($i = 1; $i -lt $PatternOption.Length; $i++) {
					[System.Text.RegularExpressions.RegexOptions]$RegexOptions = $RegexOptions -bor  $PatternOption[$i];
				}
				$Regex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList $NewLinePattern, $RegexOptions;
			} else {
				$Regex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList $NewLinePattern;
			}
		}
	}
	
	Process {
		$Level = 0;
		
		if ($InputString -ne '') {
			if ($PSCmdlet.ParameterSetName -eq 'ByPattern') {
				$M = $Regex.Match($InputString);
				while ($M.Success -and $M.Length -gt 0) {
					$Level++;
					if (($M.Index + $M.Length) -eq $InputString.Length) { break }
					$M = $Regex.Match($InputString, $M.Index + $M.Length);
				}
			} else {
				if ($InputString.StartsWith($IndentText)) {
					$Level++;
					for ($i = $IndentText.Length; ($i + $IndentText.Length) -le $InputString.Length; $i+= $IndentText.Length) {
						if ($InputString.Substring($i, $IndentText.Length) -ne $IndentText) { break }
						$Level++;
					}
				}
			}
		}
		
		$Level | Write-Output;
	}
}

Function Out-UnindentedText {
	<#
		.SYNOPSIS
			Un-indents text.
 
		.DESCRIPTION
			Removes indentation from input text.
		
		.OUTPUTS
			System.String[]. The text with indentation removed.
		
		.LINK
			Get-IndentLevel
		
		.LINK
			Out-IndentedText
		
		.LINK
			Split-DelimitedText
		
		.LINK
			Out-NormalizedText
	#>
	[CmdletBinding(DefaultParameterSetName = 'ByPattern')]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[AllowEmptyString()]
		# Text to be un-indented.
		[string]$InputString,
		
		[Parameter(ParameterSetName = 'ByString')]
		# Number of times to un-indent text
		[int]$Level = 1,
		
		[Parameter(ParameterSetName = 'ByPattern')]
		[AllowEmptyString()]
		# Pattern to detect indentation. Default is tab or 1 to 4 whitespaces at the beginning of the text: '^(\t|[^\S\t]{1,4})'
		[string]$Pattern = '^(\t|[^\S\t]{1,4})',
		
		[Parameter(ParameterSetName = 'ByPattern')]
		[Alias('RegexOptions', 'RegexOption', 'Option')]
		# Options for the indent detection pattern. Note: You can use "Compiled" to optimize for large pipelines.
		[System.Text.RegularExpressions.RegexOptions[]]$PatternOption,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ByString')]
		# Text which represents an indentation.
		[string]$IndentText
	)
	
	Begin {
		if ($PSCmdlet.ParameterSetName -eq 'ByPattern') {
			if ($PSBoundParameters.ContainsKey('PatternOption')) {
				$RegexOptions = $PatternOption[0];
				for ($i = 1; $i -lt $PatternOption.Length; $i++) {
					[System.Text.RegularExpressions.RegexOptions]$RegexOptions = $RegexOptions -bor  $PatternOption[$i];
				}
				$Regex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList $NewLinePattern, $RegexOptions;
			} else {
				$Regex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList $NewLinePattern;
			}
		} else {
			$Indent = $IndentText;
			for ($i = 1; $i -lt $Level; $i++) { $Indent += $IndentText }
		}
	}
	
	Process {
		if ($InputString -eq '') {
			$InputString | Write-Output;
		} else {
			if ($PSCmdlet.ParameterSetName -eq 'ByPattern') {
				$M = $Regex.Match($InputString);
				if ($M.Success) {
					do {
						$Index = $M.Index + $M.Length;
						$M = $Regex.Match($InputString, $M.Index + $M.Length);
					} while ($M.Success -and $M.Length -gt 0);
					$InputString.Substring($Index) | Write-Output;
				} else {
					$InputString | Write-Output;
				}
			} else {
				if ($InputString.StartsWith($IndentText)) {
					$Index = $IndentText.Length;
					while (($Index + $IndentText.Length) -le $InputString.Length -and $InputString.Substring($Index, $InputString.Length) -eq $InputString) { $Index += $IndentText.Length }
					$InputString.Substring($Index) | Write-Output;
				} else {
					$InputString | Write-Output;
				}
			}
		}
	}
}

Function Compare-FileSystemInfo {
	<#
		.SYNOPSIS
			Compares 2 filesystem items.
 
		.DESCRIPTION
			Compares 2 file system items by paths.
		
		.OUTPUTS
			System.Management.Automation.PSObject. Comparison results.
	#>
	[CmdletBinding(DefaultParameterSetName = 'Optional')]
	[OutputType([System.Management.Automation.PSObject])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipelineByPropertyName = $true)]
		# Path used as a reference for comparison.
		[string]$ReferencePath,
		
		[Parameter(Mandatory = $true, Position = 1, ValueFromPipelineByPropertyName = $true)]
		# Specifies the path that is compared to the reference path.
		[string]$DifferencePath,
		
		# Indicates that the comparison should recurse into subdirectories.
		[switch]$DoNotRecurse,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ExcludeDifferent')]
		#  Displays only the compared items that are equal.
		[switch]$ExcludeDifferent,
		
		[Parameter(ParameterSetName = 'Optional')]
		#  Displays compared items that are equal. By default, only reference and difference items that differ are displayed.
		[switch]$IncludeEqual
	)
	
	Process {
		$Properties = @{
			ReferencePath = $ReferencePath;
			ReferenceInfo = $null;
			DifferencePath = $DifferencePath;
			DifferenceInfo = $null;
			AreEqual = $false;
			Message = '';
			Type = [Microsoft.PowerShell.Commands.TestPathType]::Any;
		};
		
		if ([System.IO.Directory]::Exists($ReferencePath)) {
			$Properties['ReferenceInfo'] = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList $ReferencePath;
			$Properties['Type'] = [Microsoft.PowerShell.Commands.TestPathType]::Container;
			if ([System.IO.File]::Exists($DifferencePath)) {
				$Properties['DifferenceInfo'] = New-Object -TypeName 'System.IO.FileInfo' -ArgumentList $DifferencePath;
				$Properties['Message'] = 'Reference is a subdirectory, but difference path is a file.';
			} else {
				$Properties['DifferenceInfo'] = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList $DifferencePath;
				$Properties['AreEqual'] = $Properties['DifferenceInfo'].Exists;
				if (-not $Properties['AreEqual']) {
					$Properties['Message'] = 'Difference subdirectory does not exist';
				}
			}
		} else {
			if ([System.IO.File]::Exists($ReferencePath)) {
				$Properties['ReferenceInfo'] = New-Object -TypeName 'System.IO.FileInfo' -ArgumentList $ReferencePath;
				$Properties['Type'] = [Microsoft.PowerShell.Commands.TestPathType]::Leaf;
				if ([System.IO.Directory]::Exists($DifferencePath)) {
					$Properties['DifferenceInfo'] = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList $DifferencePath;
					$Properties['Message'] = 'Reference is a file, but difference path is a subdirectory.';
				} else {
					$Properties['DifferenceInfo'] = New-Object -TypeName 'System.IO.FileInfo' -ArgumentList $DifferencePath;
					$Properties['AreEqual'] = $Properties['DifferenceInfo'].Exists;
					if (-not $Properties['AreEqual']) {
						$Properties['Message'] = 'Difference file does not exist';
					}
				}
			} else {
				if ([System.IO.Directory]::Exists($DifferencePath)) {
					$Properties['Type'] = [Microsoft.PowerShell.Commands.TestPathType]::Container;
					$Properties['DifferenceInfo'] = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList $DifferencePath;
					$Properties['ReferenceInfo'] = New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList $ReferencePath;
					$Properties['Message'] = 'Reference subdirectory does not exist.';
				} else {
					$Properties['ReferenceInfo'] = New-Object -TypeName 'System.IO.FileInfo' -ArgumentList $ReferencePath;
					$Properties['DifferenceInfo'] = New-Object -TypeName 'System.IO.FileInfo' -ArgumentList $DifferencePath;
					$Properties['AreEqual'] = -not $Properties['DifferenceInfo'].Exists;
					if ($Properties['AreEqual']) {
						$Properties['Message'] = 'Neither reference nor difference paths exist.';
					} else {
						$Properties['Type'] = [Microsoft.PowerShell.Commands.TestPathType]::Container;
						$Properties['Message'] = 'Reference file does not exist.';
					}
				}
			}
		}
		
		if ($Properties['Type'] -eq [Microsoft.PowerShell.Commands.TestPathType]::Leaf) {
			if ($Properties['AreEqual']) {
				[System.Management.Automation.PSObject[]]$Differences = @(Compare-Object -ReferenceObject (Get-Content $Properties['ReferenceInfo'].FullName) -DifferenceObject (Get-Content $Properties['DifferenceInfo'].FullName));
				if ($Differences.Count -gt 0) {
					$Properties['AreEqual'] = $false;
					$Properties['Message'] = '{0} differences found.' -f $Differences.Count;
				}
			} else {
				[System.Management.Automation.PSObject[]]$Differences = @();
			}
			$Properties.Add('Differences', $Differences);
		}
		
		if ($Properties['AreEqual']) {
			if ($IncludeEqual) {
				$Properties['Message'] = 'Both paths are equal.';
				(New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties) | Write-Output;
			}
		} else {
			if (-not $ExcludeDifferent) {
				(New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties) | Write-Output;
			}
		}
		
		if ($Properties['AreEqual'] -and $Properties['ReferenceInfo'] -is [System.IO.DirectoryInfo] -and $Properties['ReferenceInfo'].Exists -and -not $DoNotRecurse) {
			$ReferenceContents = $Properties['ReferenceInfo'].GetFileSystemInfos();
			$DifferenceContents = $Properties['DifferenceInfo'].GetFileSystemInfos();
			$AllNames = @(($ReferenceContents + $DifferenceContents) | ForEach-Object { $_.Name.ToLower() } | Select-Object -Unique | Sort-Object);
			$AllNames | ForEach-Object {
				$Name = $_;
				$r = $ReferenceContents | Where-Object { $_.Name -ieq $Name };
				$d = $DifferenceContents | Where-Object { $_.Name -ieq $Name };
				if ($d -eq $null) {
					$d = [System.IO.Path]::Combine($Properties['DifferenceInfo'].FullName, $r.Name);
					$r = $r.FullName;
				} else {
					if ($r -eq $null) {
						$r = [System.IO.Path]::Combine($Properties['ReferenceInfo'].FullName, $d.Name);
					} else {
						$r = $r.FullName;
					}
					$d = $d.FullName;
				}
				if ($IncludeEqual) {
					Compare-FileSystemInfo -ReferencePath $r -DifferencePath $d -IncludeEqual | Write-Output;
				} else {
					if ($ExcludeDifferent) {
						Compare-FileSystemInfo -ReferencePath $r -DifferencePath $d -ExcludeDifferent | Write-Output;
					} else {
						Compare-FileSystemInfo -ReferencePath $r -DifferencePath $d | Write-Output;
					}
				}
			}
		}
	}
}

Function Test-PathsAreEqual {
	<#
		.SYNOPSIS
			Determines if 2 paths are equal.
 
		.DESCRIPTION
			Determines if 2 paths point to the same location.
		
		.INPUTS
			System.String. The path being compared.
		
		.OUTPUTS
			System.Boolean. True if TargetPath is equal to SourcePath; otherwise, false.
	#>
	[OutputType([bool])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[ValidateScript({ $_ | Test-Path -IsValid })]
		# Path being compared.
		[string[]]$TargetPath,
		
		[Parameter(Mandatory = $true, Position = 1)]
		[ValidateScript({ $_ | Test-Path -IsValid })]
		# Path being compared to.
		[string]$SourcePath,
		
		#  Specifies a user account that has permission to perform this action. The default is the current user.
		[PSCredential]$Credential,
		
		#  Includes the command in the active transaction. This parameter is valid only when a transaction is in progress. For more information, see about_Transactions.
		[switch]$UseTransaction
	)
	
	Begin {
		$Success = $true;
		$FullSourcePath = @($SourcePath);
		if ($SourcePath | Test-Path) {
			if ($PSBoundParameters.ContainsKey('Credential')) {
				if ($UseTransaction) {
					$FullSourcePath = @($SourcePath | Resolve-Path -Credential $Credential -UseTransaction);
				} else {
					$FullSourcePath = @($SourcePath | Resolve-Path -Credential $Credential);
				}
			} else {
				if ($UseTransaction) {
					$FullSourcePath = @($SourcePath | Resolve-Path -UseTransaction);
				} else {
					$FullSourcePath = @($SourcePath | Resolve-Path);
				}
			}
		}
		$SourceSegments = @();
		$FullSourcePath | ForEach-Object {
			$Parent = $_;
			$Arr = @();
			while ($Parent.Length -gt 0) {
				$Arr = @($Parent | Split-Path -Leaf) + $Arr;
				$Parent = $Parent | Split-Path -Parent;
			}
			$SourceSegments += (, $Arr);
		}
	}
	
	Process {
		if ($Success) {
			foreach ($Path in $TargetPath) {
				$FullTargetPath = @($Path);
				if ($Path | Test-Path) {
					if ($PSBoundParameters.ContainsKey('Credential')) {
						if ($UseTransaction) {
							$FullTargetPath = @($Path | Resolve-Path -Credential $Credential -UseTransaction);
						} else {
							$FullTargetPath = @($Path | Resolve-Path -Credential $Credential);
						}
					} else {
						if ($UseTransaction) {
							$FullTargetPath = @($Path | Resolve-Path -UseTransaction);
						} else {
							$FullTargetPath = @($Path | Resolve-Path);
						}
					}
				}
				$TargetSegments = @();
				$FullTargetPath | ForEach-Object {
					$Parent = $_;
					$Arr = @();
					while ($Parent.Length -gt 0) {
						$Arr = @($Parent | Split-Path -Leaf) + $Arr;
						$Parent = $Parent | Split-Path -Parent;
					}
					$TargetSegments += (, $Arr);
				}
				if ($TargetSegments.Count -eq $SourceSegments.Count) {
					for ($p = 0; $p -lt $SourceSegments.Count; $p++) {
						$SrcArr = $SourceSegments[$p];
						$TgtArr = $TargetSegments[$p];
						if ($SrcArr.Count -eq $TgtArr.Count) {
							for ($i = 0; $i -lt $SrcArr.Count; $i++) {
								if ($SrcArr[$i] -ine $TgtArr[$i]) {
									$Success = $false;
									break;
								}
							}
						} else {
							$Success = $false;
							break;
						}
					}
					if (-not $Success) { break }
				} else {
					$Success = $false;
					break;
				}
			}
		}
	}
	
	End { $Success | Write-Output }
}