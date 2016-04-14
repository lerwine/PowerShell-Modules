Add-Type -AssemblyName 'System.Windows.Forms' -ErrorAction Stop;
Add-Type -Path 'DecodeRegexReplaceHandler.cs', 'EncodeRegexReplaceHandler.cs', 'LinqEmul.cs', 'RegexReplaceHandler.cs', 'RegularExpressions.cs', 'SchemaSetCollection.cs',
	'SchemaValidationError.cs', 'SchemaValidationHandler.cs', 'ScriptRegexReplaceHandler.cs', 'StreamHelper.cs', 'TextHelper.cs' `
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

Function New-XmlReaderSettings {
	[CmdletBinding(DefaultParameterSetName = 'New')]
    [OutputType([System.Xml.XmlReaderSettings])]
	Param(
        # Sets the XmlNameTable used for atomized string comparisons.
        [System.Xml.XmlNameTable]$NameTable,
        
        # Sets line number offset of the XmlReader object.
        [Int32]$LineNumberOffset,
        
        # Sets line position offset of the XmlReader object.
        [Int32]$LinePositionOffset,
        
        # Sets the level of conformance which the XmlReader will comply.
        [System.Xml.ConformanceLevel]$ConformanceLevel,
        
        # Sets a value indicating whether to do character checking.
        [bool]$CheckCharacters,
        
        # Sets a value indicating the maximum allowable number of characters in an XML document. A zero (0) value means no limits on the size of the XML document. A non-zero value specifies the maximum size, in characters.
        [Int64]$MaxCharactersInDocument,
        
        # Sets a value indicating the maximum allowable number of characters in a document that result from expanding entities.
        [Int64]$MaxCharactersFromEntities,
        
        # Sets a value indicating whether the XmlReader will perform validation or type assignment when reading.
        [System.Xml.ValidationType]$ValidationType,
        
        # Sets a value indicating the schema validation settings. This setting applies to XmlReader objects that validate schemas (ValidationType property set to ValidationType.Schema).
        [System.Xml.Schema.XmlSchemaValidationFlags]$ValidationFlags,
        
        # Sets the XmlSchemaSet to use when performing schema validation.
        [System.Xml.Schema.XmlSchemaSet]$Schemas,
        
        # Sets a value indicating whether to ignore insignificant white space.
        [bool]$IgnoreWhitespace,
        
        # Sets a value indicating whether to ignore processing instructions.
        [bool]$IgnoreProcessingInstructions,
        
        # Sets a value indicating whether to ignore comments.
        [bool]$IgnoreComments,
        
        # Sets a value indicating whether to prohibit document type definition (DTD) processing.
        [bool]$ProhibitDtd,
        
        # Sets a value indicating whether the underlying stream or TextReader should be closed when the reader is closed.
        [bool]$CloseInput,
        
        # XmlReaderSettings to be cloned.
        [System.Xml.XmlReaderSettings]$Settings
	)
	<#
		.SYNOPSIS
			Creates an instance of the XmlReaderSettings class.
 
		.DESCRIPTION
			Initializes a new instance of the XmlReaderSettings class for use with an xml reader.
        
		.OUTPUTS
			System.Xml.XmlReaderSettings. Specifies a set of features to support on the XmlReader object created by the Create method.
        
        .LINK
            Read-XmlDocument
        
        .LINK
            New-XmlWriterSettings
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlreadersettings.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlnametable.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.conformancelevel.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.validationtype.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.schema.xmlschemavalidationflags.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.schema.xmlschemaset.aspx
	#>
    
    if ($PSBoundParameters.ContainsKey('Settings')) {
        $XmlReaderSettings = $Settings.Clone();
    } else {
        $XmlReaderSettings = New-Object -TypeName 'System.Xml.XmlReaderSettings';
    }
    if ($PSBoundParameters.ContainsKey('NameTable')) { $XmlReaderSettings.NameTable = $NameTable }
    if ($PSBoundParameters.ContainsKey('LineNumberOffset')) { $XmlReaderSettings.LineNumberOffset = $LineNumberOffset }
    if ($PSBoundParameters.ContainsKey('LinePositionOffset')) { $XmlReaderSettings.LinePositionOffset = $LinePositionOffset }
    if ($PSBoundParameters.ContainsKey('ConformanceLevel')) { $XmlReaderSettings.ConformanceLevel = $ConformanceLevel }
    if ($PSBoundParameters.ContainsKey('CheckCharacters')) { $XmlReaderSettings.CheckCharacters = $CheckCharacters }
    if ($PSBoundParameters.ContainsKey('MaxCharactersInDocument')) { $XmlReaderSettings.MaxCharactersInDocument = $MaxCharactersInDocument }
    if ($PSBoundParameters.ContainsKey('MaxCharactersFromEntities')) { $XmlReaderSettings.MaxCharactersFromEntities = $MaxCharactersFromEntities }
    if ($PSBoundParameters.ContainsKey('ValidationType')) { $XmlReaderSettings.ValidationType = $ValidationType }
    if ($PSBoundParameters.ContainsKey('ValidationFlags')) { $XmlReaderSettings.ValidationFlags = $ValidationFlags }
    if ($PSBoundParameters.ContainsKey('Schemas')) { $XmlReaderSettings.Schemas = $Schemas }
    if ($PSBoundParameters.ContainsKey('IgnoreWhitespace')) { $XmlReaderSettings.IgnoreWhitespace = $IgnoreWhitespace }
    if ($PSBoundParameters.ContainsKey('IgnoreProcessingInstructions')) { $XmlReaderSettings.IgnoreProcessingInstructions = $IgnoreProcessingInstructions }
    if ($PSBoundParameters.ContainsKey('IgnoreComments')) { $XmlReaderSettings.IgnoreComments = $IgnoreComments }
    if ($PSBoundParameters.ContainsKey('ProhibitDtd')) { $XmlReaderSettings.ProhibitDtd = $ProhibitDtd }
    if ($PSBoundParameters.ContainsKey('CloseInput')) { $XmlReaderSettings.CloseInput = $CloseInput }
    $XmlReaderSettings | Write-Output;
}

Function New-XmlWriterSettings {
	[CmdletBinding()]
    [OutputType([System.Xml.XmlWriterSettings])]
	Param(
        # Sets the type of text encoding to use
		[System.Text.Encoding]$Encoding,
        
        # Sets a value that indicates whether the XmlWriter should remove duplicate namespace declarations when writing XML content. The default behavior is for the writer to output all namespace declarations that are present in the writer's namespace resolver.
		[System.Xml.NewLineHandling]$NewLineHandling,
        
        # Sets the character string to use for line breaks
		[string]$NewLineChars,
        
        # Sets a value indicating whether to indent elements
        [bool]$Indent,
        
        # Sets the character string to use when indenting. This setting is used when the Indent property is set to true.
		[string]$IndentChars,
        
        # Sets a value indicating whether to write attributes on a new line
        [bool]$NewLineOnAttributes,
        
        # Sets a value indicating whether the XmlWriter should also close the underlying stream or TextWriter when the Close method is called
        [bool]$CloseOutput,
        
        # Sets the level of conformance that the XML writer checks the XML output for
		[System.Xml.ConformanceLevel]$ConformanceLevel,
        
        # Sets a value that indicates whether the XML writer should check to ensure that all characters in the document conform to the "2.2 Characters" section of the W3C XML 1.0 Recommendation
        [bool]$CheckCharacters,
        
        # Sets a value that indicates whether the XmlWriter does not escape URI attributes
        [bool]$DoNotEscapeUriAttributes,
        
        # XmlWriterSettings object to be cloned.
        [System.Xml.XmlWriterSettings]$Settings
	)
	<#
		.SYNOPSIS
			Creates an instance of the XmlWriterSettings class.
 
		.DESCRIPTION
			Initializes a new instance of the XmlWriterSettings class for use with an xml writer.
        
		.OUTPUTS
			System.Xml.XmlWriterSettings. Specifies a set of features to support on the XmlWriter object created by the XmlWriter.Create method.
        
        .LINK
            Write-XmlDocument
        
        .LINK
            Get-TextEncoding
        
        .LINK
            New-XmlReaderSettings
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlwritersettings.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.text.encoding.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.newlinehandling.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.conformancelevel.aspx
	#>
    
    if ($PSBoundParameters.ContainsKey('Settings')) {
        $XmlWriterSettings = $Settings.Clone();
    } else {
        $XmlWriterSettings = New-Object -TypeName 'System.Xml.XmlWriterSettings';
    }
    
    if ($PSBoundParameters.ContainsKey('Encoding')) { $XmlWriterSettings.Encoding = $Encoding }
    if ($PSBoundParameters.ContainsKey('OmitXmlDeclaration')) { $XmlWriterSettings.OmitXmlDeclaration = $OmitXmlDeclaration }
    if ($PSBoundParameters.ContainsKey('NewLineHandling')) { $XmlWriterSettings.NewLineHandling = $NewLineHandling }
    if ($PSBoundParameters.ContainsKey('NewLineChars')) { $XmlWriterSettings.NewLineChars = $NewLineChars }
    if ($PSBoundParameters.ContainsKey('Indent')) { $XmlWriterSettings.Indent = $Indent }
    if ($PSBoundParameters.ContainsKey('IndentChars')) { $XmlWriterSettings.IndentChars = $IndentChars }
    if ($PSBoundParameters.ContainsKey('NewLineOnAttributes')) { $XmlWriterSettings.NewLineOnAttributes = $NewLineOnAttributes }
    if ($PSBoundParameters.ContainsKey('CloseOutput')) { $XmlWriterSettings.CloseOutput = $CloseOutput }
    if ($PSBoundParameters.ContainsKey('ConformanceLevel')) { $XmlWriterSettings.ConformanceLevel = $ConformanceLevel }
    if ($PSBoundParameters.ContainsKey('CheckCharacters')) { $XmlWriterSettings.CheckCharacters = $CheckCharacters }
    if ($PSBoundParameters.ContainsKey('DoNotEscapeUriAttributes')) { $XmlWriterSettings.DoNotEscapeUriAttributes = $DoNotEscapeUriAttributes }
    $XmlWriterSettings | Write-Output;
}

Function Read-XmlDocument {
	[CmdletBinding(DefaultParameterSetName = 'File')]
	[OutputType([System.Xml.XmlDocument])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'File')]
        [Alias('Path')]
        # The URI for the file containing the XML data. The XmlResolver object on the XmlReaderSettings object is used to convert the path to a canonical data representation.
		[string]$InputUri,
        
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Stream')]
        # The stream that contains the XML data.
		[System.IO.Stream]$Stream,
        
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'TextReader')]
        # The stream that contains the XML data.
		[System.IO.TextReader]$TextReader,
        
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'XmlReader')]
        # The stream that contains the XML data.
		[System.Xml.XmlReader]$XmlReader,
        
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Bytes')]
        # Byte array containing the XML data.
		[byte[]]$ByteArray,
        
		[Parameter(Position = 1)]
        # The settings for the new XmlReader instance.
		[System.Xml.XmlReaderSettings]$Settings,
        
		[Parameter(Position = 2, ParameterSetName = 'File')]
		[Parameter(Position = 2, ParameterSetName = 'Stream')]
		[Parameter(Position = 2, ParameterSetName = 'TextReader')]
		[Parameter(Position = 2, ParameterSetName = 'Bytes')]
        # The base URI for the entity or document being read.
		[string]$BaseUri
	)
	<#
		.SYNOPSIS
			Reads XML data.
 
		.DESCRIPTION
			Initializes a new instance of the XmlDocument class and loads the XML data.
        
		.OUTPUTS
			System.Xml.XmlDocument. The xml document which was read.
        
        .LINK
            New-XmlReaderSettings
        
        .LINK
            Write-XmlDocument
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmldocument.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlreadersettings.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlreader.aspx
	#>
    
	if ($PSCmdlet.ParameterSetName -eq 'Bytes') {
		$Stream = New-Object -TypeName 'System.IO.MemoryStream' -ArgumentList (,$ByteArray);
		if ($Stream -eq $null) { return }
	}
	
	switch ($PSCmdlet.ParameterSetName) {
		'File' { $InputObj = $InputUri; break; }
		'TextReader' { $InputObj = $TextReader; break; }
		'XmlReader' { $InputObj = $XmlReader; break; }
		default { $InputObj = $Stream; break; }
	}
	
	if ($PSBoundParameters.ContainsKey('Settings')) {
		if ($PSBoundParameters.ContainsKey('BaseUri')) {
			$Reader = [System.Xml.XmlReader]::Create($InputObj, $Settings, $BaseUri);
		} else {
			$Reader = [System.Xml.XmlReader]::Create($InputObj, $Settings);
		}
	} else {
		if ($PSBoundParameters.ContainsKey('BaseUri')) {
			$Reader = [System.Xml.XmlReader]::Create($InputObj, $null, $BaseUri);
		} else {
			$Reader = $InputObj;
		}
	}

	if ($Reader -eq $null -and -not ($PSBoundParameters.ContainsKey('Settings') -or $PSBoundParameters.ContainsKey('BaseUri'))) { return }
	
	try {
        $XmlDocument = New-Object -TypeName 'System.Xml.XmlDocument';
		$XmlDocument.Load($Reader);
		if ($XmlDocument.DocumentElement -ne $null) { $XmlDocument | Write-Output }
	} catch {
		throw;
	} finally {
		if ($Reader -is [System.Xml.XmlReader] -and -not $PSBoundParameters.ContainsKey('XmlReader')) { $XmlReader.Dispose() }
		if ($Stream -ne $null -and -not $PSBoundParameters.ContainsKey('Stream')) { $Stream.Dispose() }
	}
}

Function Write-XmlDocument {
	[CmdletBinding(DefaultParameterSetName = 'File')]
    [OutputType([string], ParameterSetName = 'String')]
    [OutputType([byte[]], ParameterSetName = 'Bytes')]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		[Alias('XmlDocument')]
        # The XmlDocument containing the XML you wish to write.
		[System.Xml.XmlDocument]$Document,
        
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'File')]
        [Alias('Path')]
        # The file to which you want to write.
		[string]$OutputFileName,
        
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Stream')]
        # The stream to which you want to write.
		[System.IO.Stream]$Stream,
        
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'TextWriter')]
        # The System.IO.TextWriter to which you want to write.
		[System.IO.TextWriter]$TextWriter,
        
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'XmlWriter')]
        # The System.Xml.XmlWriter object that you want to use as the underlying writer.
		[System.Xml.XmlWriter]$XmlWriter,
        
		[Parameter(Position = 2)]
        # The XmlWriterSettings object used to configure the new XmlWriter instance. If this is not specified, a XmlWriterSettings with default settings is used.
		[System.Xml.XmlWriterSettings]$Settings,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'Bytes')]
        # Returns the written XML data as an array of bytes.
        [switch]$AsByteArray,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'String')]
        # Returns the written XML data as a string.
        [switch]$AsString
	)
	<#
		.SYNOPSIS
			Writes XML data with custom options.
 
		.DESCRIPTION
			Writes XML data contained within an XmlDocument object with custom output options.
        
		.OUTPUTS
			System.Byte[]. The XML data as a byte array. This is only when using the AsByteArray switch.
        
		.OUTPUTS
			System.String. The XML data as a string. This is only when using the AsString switch.
        
        .LINK
            New-XmlWriterSettings
        
        .LINK
            New-XmlDocument
            
        .LINK
            Read-XmlDocument
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmldocument.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlwritersettings.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlwriter.aspx
	#>
    
	if ($AsByteArray -or $AsString) {
		$Stream = New-Object -TypeName 'System.IO.MemoryStream';
		if ($Stream -eq $null) { return }
	}
	
	switch ($PSCmdlet.ParameterSetName) {
		'File' { $OutputObj = $OutputFileName; break; }
		'TextWriter' { $OutputObj = $TextWriter; break; }
		'XmlWriter' { $OutputObj = $XmlWriter; break; }
		default { $OutputObj = $Stream; break; }
	}
	
	if ($PSBoundParameters.ContainsKey('Settings')) {
		$Writer = [System.Xml.XmlWriter]::Create($OutputObj, $Settings);
	} else {
		$Writer = $OutputObj;
	}

    if ($Writer -eq $null -and $PSBoundParameters.ContainsKey('Settings')) {
        throw 'Unable to create xml writer.';
        return;
    }
    
    try {
        $XmlDocument.WriteTo($Writer);
		if ($Writer -isnot [string]) { $XmlWriter.Flush() }
        if ($AsByteArray) {
            Write-Output -InputObject (,$Stream.ToArray());
        } else {
            if ($AsString) {
				if ($XmlDocument.FirstChild.NodeType -eq [System.Xml.XmlNodeType]::XmlDeclaration) {
					try {
						$Encoding = [System.Text.Encoding]::GetEncoding($XmlDocument.FirstChild.Encoding);
					} catch {
						$Encoding = [System.Text.Encoding]::UTF8;
					}
				} else {
					$Encoding = [System.Text.Encoding]::UTF8;
				}
				$Encoding.GetString($Stream.ToArray()) | Write-Output;
			}
        }
    } catch {
        throw;
    } finally {
		if ($Writer -is [System.Xml.XmlWriter] -and -not $PSBoundParameters.ContainsKey('XmlWriter')) { $Writer.Dispose() }
		if ($Stream -ne $null -and -not $PSBoundParameters.ContainsKey('Stream')) { $Stream.Dispose() }
    }
}

Function ConvertTo-XmlEncodedNmToken {
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The name to be encoded. 
		[string]$Value
	)
	<#
		.SYNOPSIS
			Verifies the name is valid according to the XML specification.
 
		.DESCRIPTION
			This method guarantees that the name is valid according to the XML specification.
            For example, if you passed this method the invalid name 70+, it returns 70_x002b_ which is a valid XML name.
        
		.OUTPUTS
			System.String. The encoded name.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlconvert.encodenmtoken.aspx
	#>
	Process { [System.Xml.XmlConvert]::EncodeNmToken($Value) }
}

Function ConvertTo-XmlEncodedName {
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # A name to be translated
		[string]$Value
	)
	<#
		.SYNOPSIS
			Converts the name to a valid XML name.
 
		.DESCRIPTION
			This method translates invalid characters, such as spaces or half-width Katakana, that need to be mapped to XML names without the support or presence of schemas.
            The invalid characters are translated into escaped numeric entity encodings.

		.OUTPUTS
			System.String. The name with any invalid characters replaced by an escape string.
        
        .LINK
            ConvertFrom-XmlEncodedName
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlconvert.encodename.aspx
	#>
	Process { [System.Xml.XmlConvert]::EncodeName($Value) }
}

Function ConvertTo-XmlEncodedLocalName {
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The name to be encoded
		[string]$Value
	)
	<#
		.SYNOPSIS
			Converts the name to a valid XML local name.
 
		.DESCRIPTION
			This method is similar to the EncodeName method except that it encodes the colon character, which guarantees that the name can be used as the local name part of a namespace qualified name.
            For example, if you passed this method the invalid name a:b, it returns a_x003a_b, which is a valid local name.

		.OUTPUTS
			System.String. The encoded name.
        
        .LINK
            ConvertFrom-XmlEncodedName
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlconvert.encodelocalname.aspx
	#>
	Process { [System.Xml.XmlConvert]::EncodeLocalName($Value) }
}

Function ConvertFrom-XmlEncodedName {
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The name to be transformed
		[string]$Value
	)
	<#
		.SYNOPSIS
			Decodes a name.
 
		.DESCRIPTION
			This method does the reverse of the ConvertTo-XmlEncodedName and ConvertTo-XmlEncodedLocalName Functions.

		.OUTPUTS
			System.String. The decoded  name.
        
        .LINK
            ConvertTo-XmlEncodedName
        
        .LINK
            ConvertTo-XmlEncodedLocalName
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlconvert.decodename.aspx
	#>
	Process { [System.Xml.XmlConvert]::DecodeName($Value) }
}

Function ConvertTo-XmlString {
	[CmdletBinding(DefaultParameterSetName = 'DateTime')]
	[OutputType([string])]
	Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Boolean')]
        # Converts boolean value to XML string
        [System.Boolean]$Boolean,

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Char')]
        # Converts char value to XML string
        [System.Char]$Char,

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Decimal')]
        # Converts decimal value to XML string
        [System.Decimal]$Decimal,

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'SByte')]
        # Converts signed byte value to XML string
        [System.SByte]$SByte,

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Int16')]
        [Alias('Int16s')]
        # Converts short value to XML string
        [System.Int16]$Short,

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Int32')]
        [Alias('Int32')]
        # Converts integer value to XML string
        [System.Int32]$Int,

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Int64')]
        [Alias('Int64')]
        # Converts long value to XML string
        [System.Int64]$Long,

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Byte')]
        # Converts unsigned byte value to XML string
        [System.Byte]$Byte,

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'UInt16')]
        [Alias('UInt16')]
        # Converts unsigned short value to XML string
        [System.UInt16]$UShort,

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'UInt32')]
        [Alias('UInt32')]
        # Converts unsigned integer value to XML string
        [System.UInt32]$UInt,

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'UInt64')]
        [Alias('UInt64')]
        # Converts unsigned long value to XML string
        [System.UInt64]$ULong,

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Single')]
        [Alias('Single')]
        # Converts float value to XML string
        [float]$Float,

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Double')]
        # Converts double value to XML string
        [double]$Double,

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'TimeSpan')]
        # Converts TimeSpan to XML string
        [System.TimeSpan]$TimeSpan,
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'DateTime')]
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'DateTimeOption')]
        # Converts DateTime to XML string
        [System.DateTime]$DateTime,

        [Parameter(Mandatory = $true, ParameterSetName = 'DateTimeOption')]
        # DateTime value serialization option
        [System.Xml.XmlDateTimeSerializationMode]$DateTimeOption,

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'DateTimeOffset')]
        # Converts DateTime Offset to XML string
        [System.DateTimeOffset]$DateTimeOffset,

        [Parameter(ParameterSetName = 'DateTime')]
        [Parameter(ParameterSetName = 'DateTimeOffset')]
        # Format for DateTime string value
        [System.String]$Format = 'yyyy-MM-ddTHH:mm:sszzzzzz',

        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Guid')]
        # Converts Guid to XML string
        [System.Guid]$Guid
    )
	<#
		.SYNOPSIS
			Converts data to XML String.
 
		.DESCRIPTION
			Converts strongly typed data to an equivalent XML String representation.

		.OUTPUTS
			System.String. The converted value.
        
        .LINK
            ConvertFrom-XmlString
        
        .LINK
            ConvertTo-XmlBinary
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlconvert.tostring.aspx
	#>

    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'Boolean' {
                [System.Xml.XmlConvert]::ToString($Boolean);
                break;
            }
            'Char' {
                [System.Xml.XmlConvert]::ToString($Char);
                break;
            }
            'Decimal' {
                [System.Xml.XmlConvert]::ToString($Decimal);
                break;
            }
            'SByte' {
                [System.Xml.XmlConvert]::ToString($SByte);
                break;
            }
            'Int16' {
                [System.Xml.XmlConvert]::ToString($Short);
                break;
            }
            'Int32' {
                [System.Xml.XmlConvert]::ToString($Int);
                break;
            }
            'Int64' {
                [System.Xml.XmlConvert]::ToString($Long);
                break;
            }
            'Byte' {
                [System.Xml.XmlConvert]::ToString($Byte);
                break;
            }
            'UInt16' {
                [System.Xml.XmlConvert]::ToString($UShort);
                break;
            }
            'UInt32' {
                [System.Xml.XmlConvert]::ToString($UInt);
                break;
            }
            'UInt64' {
                [System.Xml.XmlConvert]::ToString($ULong);
                break;
            }
            'Single' {
                [System.Xml.XmlConvert]::ToString($Float);
                break;
            }
            'Double' {
                [System.Xml.XmlConvert]::ToString($Double);
                break;
            }
            'TimeSpan' {
                [System.Xml.XmlConvert]::ToString($TimeSpan);
                break;
            }
            'DateTimeOption' {
                [System.Xml.XmlConvert]::ToString($DateTime, $DateTimeOption);
                break;
            }
            'DateTimeOffset' {
                [System.Xml.XmlConvert]::ToString($DateTimeOffset, $Format);
                break;
            }
            'Guid' {
                [System.Xml.XmlConvert]::ToString($Guid);
                break;
            }
            default {
                [System.Xml.XmlConvert]::ToString($DateTime, $Format);
                break;
            }
        }
    }
}

Function ConvertFrom-XmlString {
	[CmdletBinding(DefaultParameterSetName = 'DateTime')]
    [OutputType([System.Boolean], ParameterSetName = 'Boolean')]
    [OutputType([System.Char], ParameterSetName = 'Char')]
    [OutputType([System.Decimal], ParameterSetName = 'Decimal')]
    [OutputType([System.SByte], ParameterSetName = 'SByte')]
    [OutputType([System.Int16], ParameterSetName = 'Int16')]
    [OutputType([System.Int32], ParameterSetName = 'Int32')]
    [OutputType([System.Int64], ParameterSetName = 'Int64')]
    [OutputType([System.Byte], ParameterSetName = 'Byte')]
    [OutputType([System.UInt16], ParameterSetName = 'UInt16')]
    [OutputType([System.UInt32], ParameterSetName = 'UInt32')]
    [OutputType([System.UInt64], ParameterSetName = 'UInt64')]
    [OutputType([System.Single], ParameterSetName = 'Single')]
    [OutputType([System.Double], ParameterSetName = 'Double')]
    [OutputType([System.TimeSpan], ParameterSetName = 'TimeSpan')]
    [OutputType([System.DateTime], ParameterSetName = 'DateTime')]
    [OutputType([System.DateTimeOffset], ParameterSetName = 'DateTimeOffset')]
    [OutputType([System.Guid], ParameterSetName = 'Guid')]
    Param(
        [Parameter(Mandatory = $true, ParameterSetName = 'Boolean')]
        # Converts an XML string to a Boolean value.
        [switch]$Boolean,

        [Parameter(Mandatory = $true, ParameterSetName = 'Char')]
        # Converts an XML string to a Char value.
        [switch]$Char,

        [Parameter(Mandatory = $true, ParameterSetName = 'Decimal')]
        # Converts an XML string to a Decimal value.
        [switch]$Decimal,

        [Parameter(Mandatory = $true, ParameterSetName = 'SByte')]
        # Converts an XML string to an SByte value.
        [switch]$SByte,

        [Parameter(Mandatory = $true, ParameterSetName = 'Int16')]
        # Converts an XML string to an Int16 value.
		[Alias('Int16')]
        [switch]$Short,

        [Parameter(Mandatory = $true, ParameterSetName = 'Int32')]
        # Converts an XML string to an Int32 value.
		[Alias('Int32')]
        [switch]$Int,

        [Parameter(Mandatory = $true, ParameterSetName = 'Int64')]
        # Converts an XML string to an Int64 value.
		[Alias('Int64')]
        [switch]$Long,

        [Parameter(Mandatory = $true, ParameterSetName = 'Byte')]
        # Converts an XML string to a Byte value.
        [switch]$Byte,

        [Parameter(Mandatory = $true, ParameterSetName = 'UInt16')]
        # Converts an XML string to a UInt32 value.
		[Alias('UInt16')]
        [switch]$UShort,

        [Parameter(Mandatory = $true, ParameterSetName = 'UInt32')]
        # Converts an XML string to a UInt32 value.
		[Alias('UInt32')]
        [switch]$UInt,

        [Parameter(Mandatory = $true, ParameterSetName = 'UInt64')]
        # Converts an XML string to a UInt64 value.
		[Alias('UInt64')]
        [switch]$ULong,

        [Parameter(Mandatory = $true, ParameterSetName = 'Single')]
        # Converts an XML string to a Single value.
		[Alias('Single')]
        [switch]$Float,

        [Parameter(Mandatory = $true, ParameterSetName = 'Double')]
        # Converts an XML string to a Double value.
        [switch]$Double,

        [Parameter(Mandatory = $true, ParameterSetName = 'TimeSpan')]
        # Converts an XML string to a TimeSpan value.
        [switch]$TimeSpan,

        [Parameter(Mandatory = $true, ParameterSetName = 'Guid')]
        # Converts an XML string to a Guid value.
        [switch]$Guid,

        [Parameter(Mandatory = $true, ParameterSetName = 'DateTime')]
        [Parameter(Mandatory = $true, ParameterSetName = 'DateTimeOption')]
        # Converts an XML string to a DateTime value.
        [switch]$DateTime,

        [Parameter(Mandatory = $true, ParameterSetName = 'DateTimeOffset')]
        # Converts an XML string to a DateTimeOffset value.
        [switch]$DateTimeOffset,
        
        [Parameter(ParameterSetName = 'DateTime')]
        [Parameter(ParameterSetName = 'DateTimeOffset')]
        # Format for DateTime input value
        [System.String[]]$Format,

        [Parameter(Mandatory = $true, ParameterSetName = 'DateTimeOption')]
        # DateTime value deserialization option
        [System.Xml.XmlDateTimeSerializationMode]$DateTimeOption
    )
	<#
		.SYNOPSIS
			Converts data from XML String.
 
		.DESCRIPTION
			Converts an XML string representation to strongly typed data.

		.OUTPUTS
			System.Object. The converted data.
        
        .LINK
            ConvertTo-XmlString
        
        .LINK
            ConvertFrom-XmlBinary
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlconvert.aspx
	#>

    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'Boolean' {
                [System.Xml.XmlConvert]::ToBoolean($InputString);
                break;
            }
            'Char' {
                [System.Xml.XmlConvert]::ToChar($InputString);
                break;
            }
            'Decimal' {
                [System.Xml.XmlConvert]::ToDecimal($InputString);
                break;
            }
            'SByte' {
                [System.Xml.XmlConvert]::ToSByte($InputString);
                break;
            }
            'Int16' {
                [System.Xml.XmlConvert]::ToInt16($InputString);
                break;
            }
            'Int32' {
                [System.Xml.XmlConvert]::ToInt32($InputString);
                break;
            }
            'Int64' {
                [System.Xml.XmlConvert]::ToInt64($InputString);
                break;
            }
            'Byte' {
                [System.Xml.XmlConvert]::ToByte($InputString);
                break;
            }
            'UInt16' {
                [System.Xml.XmlConvert]::ToUInt16($InputString);
                break;
            }
            'UInt32' {
                [System.Xml.XmlConvert]::ToUInt32($InputString);
                break;
            }
            'UInt64' {
                [System.Xml.XmlConvert]::ToUInt64($InputString);
                break;
            }
            'Single' {
                [System.Xml.XmlConvert]::ToSingle($InputString);
                break;
            }
            'Double' {
                [System.Xml.XmlConvert]::ToDouble($InputString);
                break;
            }
            'TimeSpan' {
                [System.Xml.XmlConvert]::ToTimeSpan($InputString);
                break;
            }
            'DateTimeOption' {
                [System.Xml.XmlConvert]::ToDateTime($InputString, $DateTimeOption);
                break;
            }
            'DateTimeOffset' {
                if ($PSBoundParameters.ContainsKey('Format')) {
                    if ($Format.Length -eq 1) {
                        [System.Xml.XmlConvert]::ToDateTimeOffset($InputString, $Format[0]);
                    } else {
                        [System.Xml.XmlConvert]::ToDateTimeOffset($InputString, $Format);
                    }
                } else {
                    [System.Xml.XmlConvert]::ToDateTimeOffset($InputString);
                }
                break;
            }
            'Guid' {
                [System.Xml.XmlConvert]::ToGuid($InputString);
                break;
            }
            default {
                if ($PSBoundParameters.ContainsKey('Format')) {
                    if ($Format.Length -eq 1) {
                        [System.Xml.XmlConvert]::ToDateTime($InputString, $Format[0]);
                    } else {
                        [System.Xml.XmlConvert]::ToDateTime($InputString, $Format);
                    }
                } else {
                    [System.Xml.XmlConvert]::ToDateTime($InputString);
                }
                break;
            }
        }
    }
}

Function ConvertTo-XmlBinary {
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[AllowEmptyCollection()]
		# The binary data to be encoded
		[byte[]]$Bytes,
		
		[Parameter(ParameterSetName = 'Base64')]
		# Encode binary data as base-64 text. This is the default.
		[switch]$Base64,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Hex')]
		# Encode binary data as a hexidecimal string.
		[switch]$Hex
	)
	<#
		.SYNOPSIS
			Converts byte array XML String.
 
		.DESCRIPTION
			Converts a byte array to an XML string representation.

		.OUTPUTS
			System.String. The converted binary data.
        
        .LINK
            ConvertFrom-XmlBinary
        
        .LINK
            ConvertTo-XmlString
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.convert.tobase64string.aspx
	#>

	Process {
		if ($Bytes.Length -eq 0) {
			'' | Write-Output;
		} else {
			if ($Hex) {
				(-join ($Bytes | ForEach-Object { ([int]$_).ToString('X2') })) | Write-Output;
			} else {
				[System.Convert]::ToBase64String($Bytes, [System.Base64FormattingOptions]::InsertLineBreaks);
			}
		}
	}
}

Function ConvertFrom-XmlBinary {
	[CmdletBinding()]
	[OutputType([byte[]])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[AllowEmptyString()]
		# The text to be decoded.
		[string]$Text,
		
		[Parameter(ParameterSetName = 'Base64')]
		# Decodes text as base-64
		[switch]$Base64,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Hex')]
		# Decodes text as hexidecimal values
		[switch]$Hex
	)
	<#
		.SYNOPSIS
			Converts XML String to byte array.
 
		.DESCRIPTION
			Converts an XML string representation to a byte array.

		.OUTPUTS
			System.Byte[]. The converted binary data.
        
        .LINK
            ConvertTo-XmlBinary
        
        .LINK
            ConvertFrom-XmlString
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.convert.frombase64string.aspx
	#>

	Process {
		$Text = $Text.Trim();
		if ($Text.Length -eq 0) {
			Write-Output -InputObject (,(New-Object -TypeName 'System.Byte[]' -ArgumentList 0));
		} else {
			if ($Hex) {
				$List = New-Object -TypeName 'System.Collections.Generic.List[char]';
				if ($Text.StartsWith('0x') -or $Text.StartsWith('0X')) { $Text = $Text.Substring(2) }
				$Text = [IOUtilityCLR.RegularExpressions]::Whitespace.Replace($Text, '');
				if ($Text.Length -gt 0) {
					for ($i = 0; $i -lt $Text.Length; $i+=2) {
						$List.Add(([char]([int]::Parse($Text.Substring($i, 2), [System.Globalization.NumberStyles]::HexNumber))));
						Write-Output -InputObject (,$List.ToArray());
					}
				}
				Write-Output -InputObject (,$List.ToArray());
			} else {
				Write-Output -InputObject (,[System.Convert]::FromBase64String($Text.Trim()));
			}
		}
	}
}

Function ConvertTo-XmlList {
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		# Text values to be added to the XML list
		[string[]]$Text
	)
	<#
		.SYNOPSIS
			Converts string values to an XML list.
 
		.DESCRIPTION
			Converts string values to an XML string representation of a list.

		.OUTPUTS
			System.String. The converted strings.
        
        .LINK
            ConvertFrom-XmlList
	#>

	Begin { $Items = @(); }

	Process { $Items += $Text }

	End {
		$EncodeRegexReplaceHandler = New-Object -TypeName 'IOUtilityCLR.EncodeRegexReplaceHandler' -ArgumentList ([IOUtilityCLR.RegularExpressions]::WhitespaceEncode)
		($Items | ForEach-Object { $EncodeRegexReplaceHandler.Replace($_) }) -join ' ';
	}
}

Function ConvertFrom-XmlList {
	[CmdletBinding()]
	[OutputType([string[]])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		# Text representing an XML list
		[string]$Text,

		# XML list items are not encoded
		[switch]$NoDecode
	)
	<#
		.SYNOPSIS
			Converts XML list string item values.
 
		.DESCRIPTION
			Converts an XML string representation of a list to individual string values.

		.OUTPUTS
			System.String[]. The list items.

        .LINK
            ConvertFrom-XmlList
	#>

	Begin { $DecodeRegexReplaceHandler = New-Object -TypeName 'IOUtilityCLR.DecodeRegexReplaceHandler' }
	Process {
		if ($NoDecode) {
			$Items -split '\s+';
		} else {
			($Items -split '\s+') | ForEach-Object { $DecodeRegexReplaceHandler.Replace($_) }
		}
	}
}

Function Add-XmlAttribute {
	[CmdletBinding(DefaultParameterSetName = 'Name')]
	[OutputType([System.Xml.XmlAttribute])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		[System.Xml.XmlElement]$XmlElement,
		
		[Parameter(Mandatory = $true, Position = 1)]
		[AllowEmptyString()]
		[string]$Value,
		
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Prefix')]
		[string]$Prefix,
		
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Name')]
		[string]$Name,
		
		[Parameter(Mandatory = $true, Position = 3, ParameterSetName = 'Prefix')]
		[string]$LocalName,
		
		[Parameter(Mandatory = $true, Position = 4, ParameterSetName = 'Prefix')]
		[Parameter(Position = 3, ParameterSetName = 'Name')]
		[string]$Namespace,

		[switch]$PassThru
	)
}

Function Set-XmlAttribute {
	[CmdletBinding(DefaultParameterSetName = 'Name')]
	[OutputType([System.Xml.XmlAttribute])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		[System.Xml.XmlElement]$XmlElement,
		
		[Parameter(Mandatory = $true, Position = 1)]
		[string]$Value,
		
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Prefix')]
		[string]$Prefix,
		
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Name')]
		[string]$Name,
		
		[Parameter(Mandatory = $true, Position = 3, ParameterSetName = 'Prefix')]
		[string]$LocalName,
		
		[Parameter(Mandatory = $true, Position = 4, ParameterSetName = 'Prefix')]
		[Parameter(Position = 3, ParameterSetName = 'Name')]
		[string]$Namespace,

		[switch]$Create
	)
}

Function Add-XmlElement {
	[CmdletBinding(DefaultParameterSetName = 'Name')]
	[OutputType([System.Xml.XmlElement])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		[System.Xml.XmlElement]$ParentElement,
		
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Prefix')]
		[string]$Prefix,
		
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Name')]
		[string]$Name,
		
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Prefix')]
		[string]$LocalName,
		
		[Parameter(Mandatory = $true, Position = 3, ParameterSetName = 'Prefix')]
		[Parameter(Position = 2, ParameterSetName = 'Name')]
		[string]$Namespace
	)
}

Function Add-XmlTextElement {
	[CmdletBinding(DefaultParameterSetName = 'Name')]
	[OutputType([System.Xml.XmlElement])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		[System.Xml.XmlElement]$ParentElement,
		
		[Parameter(Mandatory = $true, Position = 1)]
		[AllowEmptyString()]
		[string]$InnerText,
		
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Prefix')]
		[string]$Prefix,
		
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Name')]
		[string]$Name,
		
		[Parameter(Mandatory = $true, Position = 3, ParameterSetName = 'Prefix')]
		[string]$LocalName,
		
		[Parameter(Mandatory = $true, Position = 4, ParameterSetName = 'Prefix')]
		[Parameter(Position = 3, ParameterSetName = 'Name')]
		[string]$Namespace,

		[switch]$PassThru
	)
}

Function Set-XmlText {
	[CmdletBinding(DefaultParameterSetName = 'Name')]
	[OutputType([System.Xml.XmlElement])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		[System.Xml.XmlElement]$ParentElement,
		
		[Parameter(Mandatory = $true, Position = 1)]
		[AllowEmptyString()]
		[string]$InnerText,
		
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Prefix')]
		[string]$Prefix,
		
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Name')]
		[string]$Name,
		
		[Parameter(Mandatory = $true, Position = 3, ParameterSetName = 'Prefix')]
		[string]$LocalName,
		
		[Parameter(Mandatory = $true, Position = 4, ParameterSetName = 'Prefix')]
		[Parameter(Position = 3, ParameterSetName = 'Name')]
		[string]$Namespace
	)
}
