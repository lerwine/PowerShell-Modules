Add-Type -AssemblyName 'System.Windows.Forms' -PassThru -ErrorAction Stop;

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
        
        [Parameter(ParameterSetName = 'Path')]
        # Whether to allow directory separator chars (/)
        [switch]$AllowDirectorySeparator,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Volume')]
        # Whether to allow volume separator chars (:)
        [switch]$AllowVolumeDirectorySeparator,
        
        # Whether to allow path separator chars (;)
        [switch]$AllowPathSeparator
    )
	<#
		.SYNOPSIS
			Converts a string to a usable file name.
 
		.DESCRIPTION
			Encodes a string in a format which is compatible with a file name, and can be converted back to the original text.
        
		.OUTPUTS
			System.String. Text encoded as a valid file name.

		.EXAMPLE
			ConvertTo-SafeFileName -InputText 'My *unsafe* file';

		.EXAMPLE
			'c:\my*path\User.string' | ConvertTo-SafeFileName -IgnorePathSeparatorChars;

		.EXAMPLE
		  '*.txt' | ConvertTo-SafeFileName -AllowExtension;
        
        .LINK
            ConvertFrom-SafeFileName
	#>
    
    Begin {
        $WhitespaceRegex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '\s+';
        $EscapedRegex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '^_0x[A-F\d]{4}_';
        $InvalidPathChars = @([System.IO.Path]::InvalidPathChars);
        if (-not $AllowDirectorySeparator) { $InvalidPathChars += @([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar) }
        if (-not $AllowVolumeDirectorySeparator) { $InvalidPathChars += [System.IO.Path]::VolumeSeparatorChar }
        if (-not $AllowPathSeparator) { $InvalidPathChars += [System.IO.Path]::PathSeparator }
    }
    
    Process {
        foreach ($Text in $InputText) {
            $NormalizedText = $WhitespaceRegex.Replace($Text.Trim());
            $StringBuilder = New-Object -TypeName:'System.Text.StringBuilder';
            for ($i = 0; $i -lt $NormalizedText.Length; $i++) {
                $c = $NormalizedText[$i]; 
                if ($c -eq '_' -and $Regex.IsMatch($NormalizedText.Substring($i, 8))) {
                    $StringBuilder.Append('_x005F_') | Out-Null;
                } else {
                    if ($InvalidPathChars -contains $c) {
                        $StringBuilder.AppendFormat('_0x{0:X4}_', [int]$c) | Out-Null;
                    } else {
                        $StringBuilder.Append($c) | Out-Null;
                    }
                }
            }
            $StringBuilder.ToString() | Write-Output;
        }
    }
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
		
			'' | ConvertFrom-SafeFileName;
        	.LINK
        		ConvertTo-SafeFileName
	#>
    
    Begin {
        $Regex = New-Object -TypeName 'System.Text.RegularExpressions.Regex' -ArgumentList '_0x(?<hex>[\dA-F]{4})_';
    }
    
    Process {
        foreach ($Text in $InputText) {
            $MatchCollection = $Regex.Matches($Text);
            if ($MatchCollection.Count -eq 0) {
                $Text | Write-Output;
            } else {
                $StringBuilder = New-Object -TypeName 'System.Text.StringBuilder';
                $LastIndex = 0;
                $MatchCollection | ForEach-Object {
                    $Match = $_;
                    if ($Match.Index -gt $LastIndex) { $StringBuilder.Append($Text.SubString($LastIndex, $Match.Index - $LastIndex)) | Out-Null }
                    ;
                    $StringBuilder.Append(([char]([System.Convert]::ToInt32($Match.Groups['hex'].Value, 16)))) | Out-Null;
                    $LastIndex = $Match.Index + $Match.Length;
                }
                
                if ($LastIndex -lt $Text.Length) { $StringBuilder.Append($Text.SubString($LastIndex)) }
            
                $StringBuilder.ToString() | Write-Output;
            }
        }
    }
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

Add-Type -TypeDefinition @'
namespace UserFileUtils {
    using System;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    public class StreamHelper {
    }
}
'@;

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
    
    return [UserFileUtils.StreamHelper]::MinBase64BlockSize;
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
    
    return [UserFileUtils.StreamHelper]::ReadInteger($Stream);
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
    
    return [UserFileUtils.StreamHelper]::ReadLongInteger($Stream);
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
    
    [UserFileUtils.StreamHelper]::WriteInteger($Stream, $Value);
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
    
    [UserFileUtils.StreamHelper]::WriteLongInteger($Stream, $Value);
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

    return ,[UserFileUtils.StreamHelper]::ReadLengthEncodedBytes($Stream);
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
            [UserFileUtils.StreamHelper]::WriteLengthEncodedBytes($Stream, $Bytes, $Offset, $Count);
        } else {
            [UserFileUtils.StreamHelper]::WriteLengthEncodedBytes($Stream, $Bytes, $Offset, $Bytes.Length - $Offset);
        }
    } else {
        [UserFileUtils.StreamHelper]::WriteLengthEncodedBytes($Stream, $Bytes);
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
            UserFileUtils.DataBuffer. Represents a re-usable data buffer.
        
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
	[CmdletBinding(DefaultParameterSetName = 'New')]
    [OutputType([System.Text.Encoding])]
	Param(
		[Parameter(Mandatory = $true, ParameterSetName = 'Name')]
        # The code page name of the encoding to be returned. Any value returned by the WebName property is valid.
		[string]$Name,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'CodePage')]
        # The code page identifier of the encoding to be returned.
		[int]$CodePage,
        
		[Parameter(Mandatory = $false, ParameterSetName = 'Default')]
        # Gets an encoding for the operating system's current ANSI code page
        [switch]$Default,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'ASCII')]
        # Gets an encoding for the ASCII (7-bit) character set.
        [switch]$ASCII,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'Unicode')]
        # Gets an encoding for the UTF-16 format using the little endian byte order.
        [switch]$Unicode,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'BigEndianUnicode')]
        # Gets an encoding for the UTF-16 format that uses the big endian byte order.
        [switch]$BigEndianUnicode,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'UTF7')]
        # Gets an encoding for the UTF-7 format.
        [switch]$UTF7,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'UTF8')]
        # Gets an encoding for the UTF-8 format
        [switch]$UTF8,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'UTF32')]
        # Gets an encoding for the UTF-32 format using the little endian byte order.
        [switch]$UTF32
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
    
    switch ($PSCmdlet.ParameterSetName) {
        'Name' { [System.Text.Encoding]::GetEncoding($Name); break; }
        'CodePage' { [System.Text.Encoding]::GetEncoding($CodePage); break; }
        'ASCII' { [System.Text.Encoding]::ASCII; break; }
        'Unicode' { [System.Text.Encoding]::Unicode; break; }
        'BigEndianUnicode' { [System.Text.Encoding]::BigEndianUnicode; break; }
        'UTF7' { [System.Text.Encoding]::UTF7; break; }
        'UTF8' { [System.Text.Encoding]::UTF8; break; }
        'UTF32' { [System.Text.Encoding]::UTF32; break; }
        default { [System.Text.Encoding]::Default; break; }
    }
}

Function New-XmlReaderSettings {
	[CmdletBinding(DefaultParameterSetName = 'New')]
    [OutputType([System.Xml.XmlReaderSettings])]
	Param(
		[Parameter(Mandatory = $false)]
        # Sets the XmlNameTable used for atomized string comparisons.
        [System.Xml.XmlNameTable]$NameTable,
        
		[Parameter(Mandatory = $false)]
        # Sets line number offset of the XmlReader object.
        [Int32]$LineNumberOffset,
        
		[Parameter(Mandatory = $false)]
        # Sets line position offset of the XmlReader object.
        [Int32]$LinePositionOffset,
        
		[Parameter(Mandatory = $false)]
        # Sets the level of conformance which the XmlReader will comply.
        [System.Xml.ConformanceLevel]$ConformanceLevel,
        
		[Parameter(Mandatory = $false)]
        # Sets a value indicating whether to do character checking.
        [bool]$CheckCharacters,
        
		[Parameter(Mandatory = $false)]
        # Sets a value indicating the maximum allowable number of characters in an XML document. A zero (0) value means no limits on the size of the XML document. A non-zero value specifies the maximum size, in characters.
        [Int64]$MaxCharactersInDocument,
        
		[Parameter(Mandatory = $false)]
        # Sets a value indicating the maximum allowable number of characters in a document that result from expanding entities.
        [Int64]$MaxCharactersFromEntities,
        
		[Parameter(Mandatory = $false)]
        # Sets a value indicating whether the XmlReader will perform validation or type assignment when reading.
        [System.Xml.ValidationType]$ValidationType,
        
		[Parameter(Mandatory = $false)]
        # Sets a value indicating the schema validation settings. This setting applies to XmlReader objects that validate schemas (ValidationType property set to ValidationType.Schema).
        [System.Xml.Schema.XmlSchemaValidationFlags]$ValidationFlags,
        
		[Parameter(Mandatory = $false)]
        # Sets the XmlSchemaSet to use when performing schema validation.
        [System.Xml.Schema.XmlSchemaSet]$Schemas,
        
		[Parameter(Mandatory = $false)]
        # Sets a value indicating whether to ignore insignificant white space.
        [bool]$IgnoreWhitespace,
        
		[Parameter(Mandatory = $false)]
        # Sets a value indicating whether to ignore processing instructions.
        [bool]$IgnoreProcessingInstructions,
        
		[Parameter(Mandatory = $false)]
        # Sets a value indicating whether to ignore comments.
        [bool]$IgnoreComments,
        
		[Parameter(Mandatory = $false)]
        # Sets a value indicating whether to prohibit document type definition (DTD) processing.
        [bool]$ProhibitDtd,
        
		[Parameter(Mandatory = $false)]
        # Sets a value indicating whether the underlying stream or TextReader should be closed when the reader is closed.
        [bool]$CloseInput,
        
		[Parameter(Mandatory = $false)]
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
		[Parameter(Mandatory = $false)]
        # Sets the type of text encoding to use
		[System.Text.Encoding]$Encoding,
        
		[Parameter(Mandatory = $false)]
        # Sets a value that indicates whether the XmlWriter should remove duplicate namespace declarations when writing XML content. The default behavior is for the writer to output all namespace declarations that are present in the writer's namespace resolver.
		[System.Xml.NewLineHandling]$NewLineHandling,
        
		[Parameter(Mandatory = $false)]
        # Sets the character string to use for line breaks
		[string]$NewLineChars,
        
		[Parameter(Mandatory = $false)]
        # Sets a value indicating whether to indent elements
        [bool]$Indent,
        
		[Parameter(Mandatory = $false)]
        # Sets the character string to use when indenting. This setting is used when the Indent property is set to true.
		[string]$IndentChars,
        
		[Parameter(Mandatory = $false)]
        # Sets a value indicating whether to write attributes on a new line
        [bool]$NewLineOnAttributes,
        
		[Parameter(Mandatory = $false)]
        # Sets a value indicating whether the XmlWriter should also close the underlying stream or TextWriter when the Close method is called
        [bool]$CloseOutput,
        
		[Parameter(Mandatory = $false)]
        # Sets the level of conformance that the XML writer checks the XML output for
		[System.Xml.ConformanceLevel]$ConformanceLevel,
        
		[Parameter(Mandatory = $false)]
        # Sets a value that indicates whether the XML writer should check to ensure that all characters in the document conform to the "2.2 Characters" section of the W3C XML 1.0 Recommendation
        [bool]$CheckCharacters,
        
		[Parameter(Mandatory = $false)]
        # Sets a value that indicates whether the XmlWriter does not escape URI attributes
        [bool]$DoNotEscapeUriAttributes,
        
		[Parameter(Mandatory = $false)]
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

Add-Type -TypeDefinition @'
namespace XmlHelpers
{
	using System;
	using System.Collections.ObjectModel;
	using System.IO;
	using System.Xml;
	using System.Xml.Schema;
	public class XmlValidationHandler : Collection<ValidationEventArgs>
	{
		private void ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e) { this.Add(e); }
		public XmlSchema ReadXmlSchema(TextReader textReader) { return XmlSchema.Read(textReader, this.ValidationEventHandler); }
		public XmlSchema ReadXmlSchema(Stream stream) { return XmlSchema.Read(stream, this.ValidationEventHandler); }
		public XmlSchema ReadXmlSchema(XmlReader reader) { return XmlSchema.Read(reader, this.ValidationEventHandler); }
	}
}
'@;

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

Function ConvertTo-XmlEncodedName {
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
	Process { [System.Xml.XmlConvert]::EncodeName($Value) }
}

Function ConvertTo-XmlEncodedNmToken {
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
	Process { [System.Xml.XmlConvert]::EncodeNmToken($Value) }
}

Function ConvertTo-XmlEncodedLocalName {
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
	Process { [System.Xml.XmlConvert]::EncodeLocalName($Value) }
}

Function ConvertFrom-XmlDecodeName {
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
	Process { [System.Xml.XmlConvert]::DecodeName($Value) }
}

Function ConvertTo-XmlBoolean {
	[CmdletBinding()]
	[OutputType([bool])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
	Process { [System.Xml.XmlConvert]::ToString($Value) }
}

Function ConvertTo-XmlChar {
	[CmdletBinding()]
	[OutputType([char])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
	Process { [System.Xml.XmlConvert]::ToString($Value) }
}

Function ConvertTo-XmlDecimal {
	[CmdletBinding()]
	[OutputType([decimal])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
	Process { [System.Xml.XmlConvert]::ToString($Value) }
}

Function ConvertTo-XmlSByte {
	[CmdletBinding()]
	[OutputType([byte])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
	Process { [System.Xml.XmlConvert]::ToString($Value) }
}

Function ConvertTo-XmlInt32 {
	[CmdletBinding()]
	[OutputType([int])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
	Process { [System.Xml.XmlConvert]::ToString($Value) }
}

Function ConvertTo-XmlInt64 {
	[CmdletBinding()]
	[OutputType([long])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
	Process { [System.Xml.XmlConvert]::ToString($Value) }
}

Function ConvertTo-XmlByte {
	[CmdletBinding()]
	[OutputType([Byte])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
	Process { [System.Xml.XmlConvert]::ToString($Value) }
}

Function ConvertTo-XmlUInt16 {
	[CmdletBinding()]
	[OutputType([UInt16])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
	Process { [System.Xml.XmlConvert]::ToString($Value) }
}

Function ConvertTo-XmlUInt32 {
	[CmdletBinding()]
	[OutputType([UInt32])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
	Process { [System.Xml.XmlConvert]::ToString($Value) }
}

Function ConvertTo-XmlUInt64 {
	[CmdletBinding()]
	[OutputType([UInt64])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
	Process { [System.Xml.XmlConvert]::ToString($Value) }
}

Function ConvertTo-XmlSingle {
	[CmdletBinding()]
	[OutputType([float])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
	Process { [System.Xml.XmlConvert]::ToString($Value) }
}

Function ConvertTo-XmlDouble {
	[CmdletBinding()]
	[OutputType([double])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value
	)
	Process { [System.Xml.XmlConvert]::ToString($Value) }
}

Function ConvertTo-XmlTimeSpan {
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[System.TimeSpan]$Value
	)
	Process { [System.Xml.XmlConvert]::ToString($Value) }
}

Function ConvertTo-XmlDateTime {
	[CmdletBinding(DefaultParameterSetName = 'Format')]
	[OutputType([System.DateTime])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value,
		
		[Parameter(Position = 1, ParameterSetName = 'Format')]
		[string]$Format,
		
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'XmlDateTimeSerializationMode')]
		[System.Xml.XmlDateTimeSerializationMode]$SerializationMode
	)
	Process { [System.Xml.XmlConvert]::ToString($Value, $Format) }
}

Function ConvertTo-XmlDateTimeOffset {
	[CmdletBinding()]
	[OutputType([System.DateTimeOffset])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Value,
		
		[Parameter(Position = 0, ValueFromPipeline = $true)]
		[string]$Format
	)
	Process { [System.Xml.XmlConvert]::ToString($Value, $Format) }
}

Function ConvertTo-XmlGuid {
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[Guid]$Value
	)
	Process { [System.Xml.XmlConvert]::ToString($Value) }
}

Function ConvertTo-XmlBinary {
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[byte[]]$Bytes,
		
		[Parameter(ParameterSetName = 'Base64')]
		[switch]$Base64,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Hex')]
		[switch]$Hex
	)
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

Function ConvertFrom-XmlBoolean {
	[CmdletBinding()]
	[OutputType([bool])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text
	)
	Process { [System.Xml.XmlConvert]::ToBoolean($Text) }
}

Function ConvertFrom-XmlChar {
	[CmdletBinding()]
	[OutputType([char])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text
	)
	Process { [System.Xml.XmlConvert]::ToChar($Text) }
}

Function ConvertFrom-XmlDecimal {
	[CmdletBinding()]
	[OutputType([decimal])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text
	)
	Process { [System.Xml.XmlConvert]::ToDecimal($Text) }
}

Function ConvertFrom-XmlSByte {
	[CmdletBinding()]
	[OutputType([System.SByte])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text
	)
	Process { [System.Xml.XmlConvert]::ToSByte($Text) }
}

Function ConvertFrom-XmlInt16 {
	[CmdletBinding()]
	[OutputType([System.Int16])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text
	)
	Process { [System.Xml.XmlConvert]::ToInt16($Text) }
}

Function ConvertFrom-XmlInt32 {
	[CmdletBinding()]
	[OutputType([int])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text
	)
	Process { [System.Xml.XmlConvert]::ToInt32($Text) }
}

Function ConvertFrom-XmlInt64 {
	[CmdletBinding()]
	[OutputType([long])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text
	)
	Process { [System.Xml.XmlConvert]::ToInt64($Text) }
}

Function ConvertFrom-XmlByte {
	[CmdletBinding()]
	[OutputType([byte])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text
	)
	Process { [System.Xml.XmlConvert]::ToByte($Text) }
}

Function ConvertFrom-XmlUInt16 {
	[CmdletBinding()]
	[OutputType([System.UInt16])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text
	)
	Process { [System.Xml.XmlConvert]::ToUInt16($Text) }
}

Function ConvertFrom-XmlUInt32 {
	[CmdletBinding()]
	[OutputType([System.UInt32])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text
	)
	Process { [System.Xml.XmlConvert]::ToUInt32($Text) }
}

Function ConvertFrom-XmlUInt64 {
	[CmdletBinding()]
	[OutputType([System.UInt64])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text
	)
	Process { [System.Xml.XmlConvert]::ToUInt64($Text) }
}

Function ConvertFrom-XmlSingle {
	[CmdletBinding()]
	[OutputType([float])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text
	)
	Process { [System.Xml.XmlConvert]::ToSingle($Text) }
}

Function ConvertFrom-XmlDouble {
	[CmdletBinding()]
	[OutputType([double])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text
	)
	Process { [System.Xml.XmlConvert]::ToDouble($Text) }
}

Function ConvertFrom-XmlTimeSpan {
	[CmdletBinding()]
	[OutputType([System.TimeSpan])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text
	)
	Process { [System.Xml.XmlConvert]::ToTimeSpan($Text) }
}

Function ConvertFrom-XmlDateTime {
	[CmdletBinding(DefaultParameterSetName = 'Format')]
	[OutputType([System.DateTime])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text,
		
		[Parameter(Position = 1, ParameterSetName = 'Format')]
		[string[]]$Format,
		
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'XmlDateTimeSerializationMode')]
		[System.Xml.XmlDateTimeSerializationMode[]]$SerializationMode
	)
	Process {
		if ($PSBoundParameters.ContainsKey('Format')) {
			[System.Xml.XmlConvert]::ToDateTime($Text, $Format);
		} else {
			[System.Xml.XmlConvert]::ToDateTime($Text);
		}
	}
}

Function ConvertFrom-XmlDateTimeOffset {
	[CmdletBinding()]
	[OutputType([System.DateTimeOffset])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text,
		
		[Parameter(Position = 1)]
		[string[]]$Format
	)
	Process {
		if ($PSBoundParameters.ContainsKey('Format')) {
			[System.Xml.XmlConvert]::ToDateTimeOffset($Text, $Format);
		} else {
			[System.Xml.XmlConvert]::ToDateTimeOffset($Text);
		}
	}
}

Function ConvertFrom-XmlGuid {
	[CmdletBinding()]
	[OutputType([System.Guid])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text
	)
	Process { [System.Xml.XmlConvert]::ToGuid($Text) }
}

Function ConvertFrom-XmlBinary {
	[CmdletBinding()]
	[OutputType([byte[]])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[byte[]]$Text,
		
		[Parameter(ParameterSetName = 'Base64')]
		[switch]$Base64,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Hex')]
		[switch]$Hex
	)
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

Function ConvertTo-XmlList {
	[CmdletBinding()]
	[OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string[]]$Text
	)

	Begin { $Items = @(); }

	Process { $Items += $Text }

	End { $Items -join ' ' }
}

Function ConvertFrom-XmlList {
	[CmdletBinding()]
	[OutputType([string[]])]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[string]$Text
	)

	Process { $Items -split '\s+' }
}

Function Add-XmlAttribute {
	[CmdletBinding(DefaultParameterSetName = 'Name')]
	[OutputType([System.Xml.XmlAttribute])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
		[System.Xml.XmlElement]$XmlElement,
		
		[Parameter(Mandatory = $true, Position = 1]
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
		
		[Parameter(Mandatory = $true, Position = 1]
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
