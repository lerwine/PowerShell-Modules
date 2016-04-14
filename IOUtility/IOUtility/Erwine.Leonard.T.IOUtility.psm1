Add-Type -AssemblyName 'System.Windows.Forms' -ErrorAction Stop;
Add-Type -Path 'DecodeRegexReplaceHandler.cs', 'EncodeRegexReplaceHandler.cs', 'LinqEmul.cs', 'RegexReplaceHandler.cs', 'RegularExpressions.cs', 'ScriptRegexReplaceHandler.cs',
        'StreamHelper.cs', 'TextHelper.cs' `
	-ReferencedAssemblies 'System', 'System.Core', 'System.Management.Automation', 'System.Net.Http', 'System.Xml';

$Script:Regex = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
    Whitespace = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '\s', ([System.Text.RegularExpressions.RegexOptions]::Compiled);
    UrlEncodedItem = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '(^|&)(?<key>[^&=]*)(=(?<value>[^&]*))?', ([System.Text.RegularExpressions.RegexOptions]::Compiled);
};

Function Get-SpecialFolderNames {
	[CmdletBinding()]
	[OutputType([string[]])]
    Param()
    <#
        .SYNOPSIS
			Get special folder names
         
        .DESCRIPTION
			Returns a list of names that can be used to refer to actual spcial folder paths.
        
        .OUTPUTS
			System.String[]. List of non-empty string values.
        
        .LINK
            Get-SpecialFolder
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.environment.specialfolder.aspx
    #>
    if ($PSVersionTable.ClrVersion.Major -lt 4) {
        [System.Enum]::GetNames([System.Environment+SpecialFolder]) + @('ProgramFilesX86', 'CommonProgramFilesX86', 'Windows');
    } else {
        [System.Enum]::GetNames([System.Environment+SpecialFolder])
    }
}

Function Get-SpecialFolder {
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
	<#
		.SYNOPSIS
			Get special folder path
 
		.DESCRIPTION
			Converts special folder enumerated value to string path
        
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
    [CmdletBinding()]
	[OutputType([string])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		# String to convert to file name
        [string[]]$InputText,
        
        [Parameter(ParameterSetName = 'FileName')]
        # Only allow file names. This is the default.
        [switch]$FileName,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'RelativePath')]
        # Only allow relative paths
        [switch]$RelativePath,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'FullPath')]
        # Allow full path specification
        [switch]$FullPath
    )
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
    
    Begin {
		switch ($PSCmdlet.ParameterSetName){
			'RelativePath' {
				$EncodeRegexReplaceHandler = New-Object -TypeName 'IOUtilityCLR.EncodeRegexReplaceHandler' -ArgumentList ([IOUtilityCLR.RegularExpressions]::InvalidRelativePathChars);
				break;
			}
			'FullPath' {
				$EncodeRegexReplaceHandler = New-Object -TypeName 'IOUtilityCLR.EncodeRegexReplaceHandler' -ArgumentList ([IOUtilityCLR.RegularExpressions]::InvalidPathChars);
				break;
			}
			default {
				$EncodeRegexReplaceHandler = New-Object -TypeName 'IOUtilityCLR.EncodeRegexReplaceHandler' -ArgumentList ([IOUtilityCLR.RegularExpressions]::InvalidFileNameChars);
				break;
			}
		}
    }
    
    Process { foreach ($Text in $InputText) { $EncodeRegexReplaceHandler.Replace($Text) } }
}

Function ConvertFrom-SafeFileName {
    [CmdletBinding()]
	[OutputType([string])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		# File name to decode
        [string[]]$InputText
    )
	<#
		.SYNOPSIS
			Decodes a file name back to the original text
 
		.DESCRIPTION
			If a file name was creating using 'ConvertTo-SafeFileName', this method will convert it back.

		.OUTPUTS
			System.String. Encoded file name decoded to its original text.

		.EXAMPLE
			'File_0x005F_Name' | ConvertFrom-SafeFileName;

        .LINK
        		ConvertTo-SafeFileName
	#>
    
    Begin {
        $DecodeRegexReplaceHandler = New-Object -TypeName 'IOUtilityCLR.DecodeRegexReplaceHandler';
    }
    
    Process { foreach ($Text in $InputText) { $DecodeRegexReplaceHandler.Replace($Text) } }
}

Function Get-AppDataPath {
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
	<#
		.SYNOPSIS
			Get path for application data storage.
 
		.DESCRIPTION
			Constructs a path for application-specific data.

		.OUTPUTS
			System.String. Path to application data storage folder.
	#>
    
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

Function Read-FileDialog {
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
        # Indicates whether the dialog box restores the directory to the previously selected directory before closing
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
        
		[Parameter(ParameterSetName = 'SaveFileDialog')]
        # Indicates whether the Save As dialog box displays a warning if the user specifies a file name that already exists.
		[bool]$OverwritePrompt,
        
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[Parameter(ParameterSetName = 'OpenFileDialog')]
        # Sets the initial directory displayed by the file dialog box.
		[string]$InitialDirectory,
        
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[Parameter(ParameterSetName = 'OpenFileDialog')]
        # Sets the current file name filter string, which determines the choices that appear in the "Save as file type" or "Files of type" box in the dialog box.
		[string]$Filter,
        
		[Parameter(ParameterSetName = 'FolderBrowserDialog')]
        # Sets the root folder where the browsing starts from.
		[System.Environment+SpecialFolder]$RootFolder,
        
		[Parameter(ParameterSetName = 'SaveFileDialog')]
		[Parameter(ParameterSetName = 'OpenFileDialog')]
        # Sets the default file name extension
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
	#>
    
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
            if ($PSBoundParameters.ContainsKey('Filter')) { $Dialog.Filter = $Filter }
            if ($PSBoundParameters.ContainsKey('AddExtension')) { $Dialog.AddExtension = $AddExtension }
            if ($PSCmdlet.ParameterSetName -eq 'OpenFileDialog') {
                if ($PSBoundParameters.ContainsKey('Multiselect')) { $Dialog.Multiselect = $Multiselect }
                if ($PSBoundParameters.ContainsKey('ShowReadOnly')) { $Dialog.ShowReadOnly = $ShowReadOnly }
                if ($PSBoundParameters.ContainsKey('ReadOnlyChecked')) { $Dialog.ReadOnlyChecked = $ReadOnlyChecked }
            } else {
                if ($PSBoundParameters.ContainsKey('OverwritePrompt')) { $Dialog.OverwritePrompt = $OverwritePrompt }
            }
        }

        if ($Dialog.ShowDialog() -eq [System.Windows.Forms.DialogResult]::OK) {
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
    [CmdletBinding()]
    [OutputType([int])]
    Param()
	<#
		.SYNOPSIS
			Get minimum base-64 encoding block size.
 
		.DESCRIPTION
			Get minimum base-64 encoding block size when you intend on emitting line-separated chunks of base64-encoded data.
			
		.OUTPUTS
			System.Int32. Minimum block size for line-separated chunks of base64-encoded data.
	#>
    
    return [IOUtilityCLR.StreamHelper]::MinBase64BlockSize;
}

Function Read-IntegerFromStream {
    [CmdletBinding()]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
		# Stream from which to read the bytes of an integer value.
        [System.IO.Stream]$Stream
    )
	<#
		.SYNOPSIS
			Read integer value from stream.
 
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
    
    return [IOUtilityCLR.StreamHelper]::ReadInteger($Stream);
}

Function Read-LongIntegerFromStream {
    [CmdletBinding()]
    [OutputType([long])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
		# Stream from which to read the bytes of a long integer value.
        [System.IO.Stream]$Stream
    )
	<#
		.SYNOPSIS
			Read long integer value from stream.
 
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
    
    return [IOUtilityCLR.StreamHelper]::ReadLongInteger($Stream);
}

Function Write-IntegerToStream {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
		# Stream to write integer value to
        [System.IO.Stream]$Stream,
        
        [Parameter(Mandatory = $true, Position = 0)]
		# Integer value to be written
        [int]$Value
    )
	<#
		.SYNOPSIS
			Write integer value to a Stream.
 
		.DESCRIPTION
			Writes an integer value to the Stream as an array of bytes.
        
        .LINK
            Read-IntegerFromStream
        
        .LINK
            Write-LongIntegerToStream
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
    
    [IOUtilityCLR.StreamHelper]::WriteInteger($Stream, $Value);
}

Function Write-LongIntegerToStream {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
		# Stream to write long integer value to
        [System.IO.Stream]$Stream,
        
        [Parameter(Mandatory = $true, Position = 0)]
		# Long Integer value to be written
        [long]$Value
    )
	<#
		.SYNOPSIS
			Write long integer value to a Stream.
 
		.DESCRIPTION
			Writes a long integer value to the Stream as an array of bytes.
        
        .LINK
            Read-LongIntegerFromStream
        
        .LINK
            Write-IntegerToStream
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
    
    [IOUtilityCLR.StreamHelper]::WriteLongInteger($Stream, $Value);
}

Function Read-LengthEncodedBytes {
    [CmdletBinding()]
    [OutputType([System.Byte[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
		# Stream to read length-encoded data from.
        [System.IO.Stream]$Stream
    )
	<#
		.SYNOPSIS
			Read length-encoded array of bytes from a Stream.
 
		.DESCRIPTION
			Reads a length value from the Stream, and then reads the associated number of bytes.
			
		.OUTPUTS
			System.Byte[]. Array of length-encoded bytes read from stream.
        
        .LINK
            Write-LengthEncodedBytes
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>

    return ,[IOUtilityCLR.StreamHelper]::ReadLengthEncodedBytes($Stream);
}

Function Write-LengthEncodedBytes {
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
	<#
		.SYNOPSIS
			Writes length-encoded data a Stream.
 
		.DESCRIPTION
			Writes a length-encoded byte array to the Stream.
        
        .LINK
            Read-LengthEncodedBytes
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>

    if ($PSBoundParameters.ContainsKey('Offset') -or $PSBoundParameters.ContainsKey('Count')) {
        if ($PSBoundParameters.ContainsKey('Count')) {
            [IOUtilityCLR.StreamHelper]::WriteLengthEncodedBytes($Stream, $Bytes, $Offset, $Count);
        } else {
            [IOUtilityCLR.StreamHelper]::WriteLengthEncodedBytes($Stream, $Bytes, $Offset, $Bytes.Length - $Offset);
        }
    } else {
        [IOUtilityCLR.StreamHelper]::WriteLengthEncodedBytes($Stream, $Bytes);
    }
}

Function ConvertTo-Base64String {
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
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
		# Base-64 encoded text
        [string]$InputString,
		
        [Parameter(Mandatory = $false)]
		# Minimum capacity, in bytes, of the returned data buffer.
        [int]$MinCapacity
    )
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
	
	$Buffer = [System.Convert]::FromBase64String($InputString);
	if ($PSBoundParameters.ContainsKey('MinCapacity') -and $MinCapacity -gt $Buffer.Length) {
		[System.Array]::Resize([ref]$Buffer, $MinCapacity);
	}

	return ,$Buffer;
}

Function Get-TextEncoding {
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
        [Alias('UTF16')]
        # Gets an encoding for the UTF-16 format using the little endian byte order.
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
	[CmdletBinding(DefaultParameterSetName = 'Opt')]
	[OutputType([System.IO.MemoryStream])]
	Param(
		[Parameter(Position = 0, ParameterSetName = 'Opt')]
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Buffer')]
        [Alias('Bytes')]
        # Initializes a new non-resizable instance of the MemoryStream class based an array of bytes.
		[string]$Buffer,

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