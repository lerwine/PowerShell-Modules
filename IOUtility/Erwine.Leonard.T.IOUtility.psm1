if ((Add-Type -AssemblyName 'System.Windows.Forms' -PassThru -ErrorAction Stop) -eq $null) { throw 'Cannot load assembly "System.Windows.Forms".' }

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
        
		# Whether to allow file extensions. If this switch is not present, then the '.' character will be encoded.
        [switch]$AllowExtension,
        
		# Whether to ignore path separator characters when encoding.
        [switch]$IgnorePathSeparatorChars
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
        [char[]]$InvalidFileNameChars = [System.IO.Path]::GetInvalidFileNameChars();
        if ($IgnorePathSeparatorChars) {
            [char[]]$InvalidFileNameChars = $InvalidFileNameChars | Where-Object { $char -ne [System.IO.Path]::DirectorySeparatorChar -and $char -ne [System.IO.Path]::AltDirectorySeparatorChar };
        }
        if ($InvalidFileNameChars -notcontains '_') { [char[]]$InvalidFileNameChars += [char]'_' }
        if (-not $AllowExtension) { [char[]]$InvalidFileNameChars += [char]'.' }
    }
    
    Process {
        foreach ($text in $InputText) {
            if ($text -ne $null -and $text.Length -gt 0) {
                $StringBuilder = New-Object -TypeName:'System.Text.StringBuilder';
                foreach ($char in $text.ToCharArray()) {
                    if ($InvalidFileNameChars -contains $char) {
                        $StringBuilder.AppendFormat('_0x{0:x2}_', [int]$char) | Out-Null;
                    } else {
                        $StringBuilder.Append($char) | Out-Null;
                    }
                }
                
                $StringBuilder.ToString() | Write-Output;
            }
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

		.NOTES
			This is just an example function.
        
        .LINK
            ConvertTo-SafeFileName
	#>
    
    Begin {
        $Regex = New-Object -TypeName:'System.Text.RegularExpressions.Regex' -ArgumentList:('_0x(?<hex>[\da-f]{2})_',
            ([System.Text.RegularExpressions.RegexOptions]::Compiled -bor [System.Text.RegularExpressions.RegexOptions]::Ignorecase));
    }
    
    Process {
        foreach ($text in $InputText) {
            if ($text -ne $null -and $text.Length -gt 0) {
                $MatchCollection = $Regex.Matches($text);
                if ($MatchCollection.Count -eq 0) {
                    $text | Write-Output;
                } else {
                    $StringBuilder = New-Object -TypeName:'System.Text.StringBuilder';
                    $previousEnd = 0;
                    $MatchCollection | ForEach-Object {
                        $Match = $_;
                        if ($Match.Index -gt $previousEnd) { $StringBuilder.Append($text.SubString($previousEnd, $Match.Index - $previousEnd)) | Out-Null }
                        [char]$char = [System.Convert]::ToInt32($Match.Groups['hex'].Value, 16);
                        $StringBuilder.Append($char) | Out-Null;
                        $previousEnd = $Match.Index + $Match.Length;
                    }
                    
                    if ($previousEnd -lt $text.Length) { $StringBuilder.Append($text.SubString($previousEnd)) }
                
                    $StringBuilder.ToString() | Write-Output;
                }
            }
        }
    }
}

Function New-ProductInfo {
    [CmdletBinding()]
	[OutputType([string])]
    Param(
        [Parameter(Mandatory = $true)]
		# Name of company
        [string]$Company,

        [Parameter(Mandatory = $true)]
		# Name of application
        [string]$Product,

        [Parameter(Mandatory = $false)]
		# Name of application
        [System.Version]$Version,

        [Parameter(Mandatory = $false)]
		# Name of application
        [string]$ComponentName,

        [Parameter(Mandatory = $false, ValueFromRemainingArguments = $true)]
		# Any other arbitrary parameters can be passed, which will become properties of the result object.
        [object[]]$AdditionalProperties
    )
	<#
		.SYNOPSIS
			Create product/component information object.
 
		.DESCRIPTION
			Creates an object which contains information about a product/component.
	#>
    
	$Properties = @{};
	foreach ($Key in $PSBoundParameters.Keys) { if ($Key -ne 'AdditionalProperties') { $Properties.Add($Key, $PSBoundParameters[$Key]) } }
	foreach ($Key in $AdditionalProperties.Keys) { $Properties.Add($Key, $AdditionalProperties[$Key]) }
	New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Properties;
}

Function Get-AppDataPath {
    [CmdletBinding(DefaultParameterSetName = 'Roaming')]
	[OutputType([string])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
		# Name of company
        [string]$Company,

        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
		# Name of application
        [string]$ProductName,

        [Parameter(Mandatory = $false, ValueFromPipelineByPropertyName = $true)]
		# Name of application
        [System.Version]$Version,

        [Parameter(Mandatory = $false, ValueFromPipelineByPropertyName = $true)]
		# Name of application
        [string]$ComponentName,

        [Parameter(Mandatory = $false)]
		# Create folder structure if it does not exist
        [switch]$Create,

        [Parameter(Mandatory = $false, ParameterSetName = 'Roaming')]
		# Create folder structure under roaming profile.
        [switch]$Roaming,

        [Parameter(Mandatory = $true, ParameterSetName = 'Local')]
		# Create folder structure under local profile.
        [switch]$Local,

        [Parameter(Mandatory = $true, ParameterSetName = 'Common')]
		# Create folder structure under common location.
        [switch]$Common
    )
	<#
		.SYNOPSIS
			Get path for a product
 
		.DESCRIPTION
			Constructs a path for application-specific data
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

		$AppDataPath = $AppDataPath | Join-Path -ChildPath ($Company | ConvertTo-SafeFileName -AllowExtension);
		
		if ($Create -and (-not ($AppDataPath | Test-Path -PathType Container))) {
			New-Item -Path $AppDataPath -ItemType Directory | Out-Null;
			if (-not ($AppDataPath | Test-Path -PathType Container)) {
				throw ('Unable to create {0} company path "{1}".' -f $PSCmdlet.ParameterSetName, $AppDataPath);
			}
		}

		$N = $ProductName;
		if ($PSBoundParameters.ContainsKey('Version')) { $N = '{0}_{1}_{2}' -f $N, $Version.Major, $Version.Minor }
		$AppDataPath = $AppDataPath | Join-Path -ChildPath ($N | ConvertTo-SafeFileName -AllowExtension);
		
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

Function Read-FilePath {
    [CmdletBinding(DefaultParameterSetName = "OpenFile")]
	[OutputType([string])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
		# Title of the file browser window.
        [string]$Title,
        
        [Parameter(Mandatory = $false)]
		# File name filter where the keys are the wildcard file filters, and the values are the descriptions.
        [HashTable]$Filter,
                
        [Parameter(Mandatory = $false)]
		# Initial file name.
        [string]$FileName,
        
        [Parameter(Mandatory = $false)]
		# Default extension.
        [string]$DefaultExt,

        [Parameter(Mandatory = $true, ParameterSetName = "FileExists")]
		# Indicates that the file must exist.
        [switch]$CheckFileExists,
        
        [Parameter(Mandatory = $false, ParameterSetName = "OpenFile")]
		# Indicates that the parent path must exist.
        [switch]$CheckPathExists,
        
        [Parameter(Mandatory = $false)]
		# Indicates that the default extension should be added, if not specified.
        [switch]$AddExtension,
        
        [Parameter(Mandatory = $false, ParameterSetName = "OpenFile")]
        [Parameter(Mandatory = $false, ParameterSetName = "FileExists")]
		# Explicitly indicate that the "Open File" dialog should be used.
        [switch]$OpenFile,
        
        [Parameter(Mandatory = $true, ParameterSetName = "SaveFile")]
		# Indicates that the "Save File" dialog should be used.
        [switch]$SaveFile
    )
	<#
		.SYNOPSIS
			Reads file path by prompting user.
 
		.DESCRIPTION
			Displays an open file dialog to allow the user to specify a file path.
			
		.OUTPUTS
			System.String. Path of file selected by user or null if no file was selected.

		.EXAMPLE
			$Path = Read-FilePath -Title 'Open config file' -Filter @{ '*.xml' = 'XML Files (*.xml)' }
			if ($Path -eq $null) { 'No file selected' | Write-Warning }
        
        .LINK
            Read-FolderPath
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.savefiledialog.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.openfiledialog.aspx
	#>
	
	$FileDialog = &{
		if ($SaveFile) {
			New-Object -TypeName 'System.Windows.Forms.SaveFileDialog'
		} else {
			New-Object -TypeName 'System.Windows.Forms.OpenFileDialog'
		}
	};
	
	try {
		if (-not [String]::IsNullOrEmpty($Title)) { $FileDialog.Title = $Title };
		$FileDialog.AddExtension = $false;
		if (-not $SaveFile) {
			$FileDialog.CheckFileExists = $CheckFileExists;
			if (-not $CheckFileExists) { $FileDialog.CheckPathExists = $CheckPathExists }
		}
		if (-not [String]::IsNullOrEmpty($FileName)) {
			$FileDialog.FileName = $FileName
			$FileDialog.InitialDirectory = $FileName | Split-Path -Parent;
		} else {
			$FileDialog.InitialDirectory =  [Environment]::GetFolderPath([Environment+SpecialFolder]::MyDocuments);
		}
		$AllFilters = @{};
		if ($PSBoundParameters.ContainsKey('Filter')) {
			$Filter.Keys | ForEach-Object { $AllFilters.Add($_, $Filter[$_]) }
		}
		if (-not $AllFilters.ContainsKey('*.*')) { $AllFilters.Add('*.*', 'All Files (*.*)') }
		$FileDialog.Filter = ($AllFilters.Keys | ForEach-Object { '{0}|{1}' -f $AllFilters[$_], $_ }) -join '|';

		$DialogResult = $FileDialog.ShowDialog();
		if ($DialogResult -eq [System.Windows.Forms.DialogResult]::OK) {
			$FileDialog.FileName | Write-Output;
		}
	} catch { throw; }
	finally {
		$FileDialog.Dispose();
	}
}

Function Read-FolderPath {
    [CmdletBinding()]
	[OutputType([string])]
    Param(
        [Parameter(Mandatory = $true)]
		# Description to be displayed in the folder browser window.
        [string]$Description,
        
        [Parameter(Mandatory = $false)]
		# Initially selected folder.
        [string]$SelectedPath,
        
        [Parameter(Mandatory = $false)]
		# Root folder for browsing.
		[System.Environment+SpecialFolder]$RootFolder,
        
        [Parameter(Mandatory = $false)]
		# Whether to show the 'New Folder' button, which allows the user to create new folders.
        [switch]$ShowNewFolderButton
    )
	<#
		.SYNOPSIS
			Reads subdirectory path by prompting user.
 
		.DESCRIPTION
			Displays an open folder dialog to allow the user to specify a subdirectory path.
			
		.OUTPUTS
			System.String. Path of folder selected by user or null if no folder was selected.

		.EXAMPLE
			$Path = Read-FolderPath -Description 'Open config location';
			if ($Path -eq $null) { 'No folder selected' | Write-Warning }
        
        .LINK
            Read-FilePath
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.savefiledialog.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.environment.specialfolder.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.folderbrowserdialog.aspx
	#>
    
    $FolderBrowserDialog = New-Object -TypeName 'System.Windows.Forms.FolderBrowserDialog';
    try {
        $FolderBrowserDialog.Description = $Description;
        $FolderBrowserDialog.ShowNewFolderButton = $ShowNewFolderButton;
		if ($PSBoundParameters.ContainsKey('SelectedPath')) { $FolderBrowserDialog.SelectedPath = $SelectedPath }
		if ($PSBoundParameters.ContainsKey('RootFolder')) { $FolderBrowserDialog.RootFolder = $RootFolder }

        if ($FolderBrowserDialog.ShowDialog() -eq [System.Windows.Forms.DialogResult]::OK) {
            $FolderBrowserDialog.SelectedPath | Write-Output;
        }
    } catch { throw; }
    finally {
        $FolderBrowserDialog.Dispose();
    }
}

Add-Type -TypeDefinition @'
namespace UserFileUtils {
    using System;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    public class StreamHelper {
		private static int? _minBase64BlockSize = null;
		public static int MinBase64BlockSize {
			get {
				if (StreamHelper._minBase64BlockSize.HasValue)
					return StreamHelper._minBase64BlockSize.Value;
				int minBase64BlockSize = 0;
				string s = "";
				Regex regex = new Regex(@"\s");
				do {
					minBase64BlockSize++;
					byte[] buffer = new byte[minBase64BlockSize];
					s = Convert.ToBase64String(buffer, 0, minBase64BlockSize, Base64FormattingOptions.InsertLineBreaks).Trim();
				} while (!regex.IsMatch(s));
				
				StreamHelper._minBase64BlockSize = minBase64BlockSize;
				return minBase64BlockSize;
			}
		}
        public static int ReadInteger(Stream inputStream) {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");
            
            byte[] buffer = new byte[sizeof(int)];
            if (inputStream.Read(buffer, 0, buffer.Length) != buffer.Length)
                throw new InvalidOperationException("Unexpected end of stream");
            
            return BitConverter.ToInt32(buffer, 0);
        }
        public static long ReadLongInteger(Stream inputStream) {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");
            
            byte[] buffer = new byte[sizeof(long)];
            if (inputStream.Read(buffer, 0, buffer.Length) != buffer.Length)
                throw new InvalidOperationException("Unexpected end of stream");
            
            return BitConverter.ToInt64(buffer, 0);
        }
        public static byte[] ReadLengthEncodedBytes(Stream inputStream) {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");
            
            int length = StreamHelper.ReadInteger(inputStream);
            if (length < 0)
                throw new InvalidOperationException("Invalid length");
                
            byte[] buffer = new byte[length];
            if (length > 0 && inputStream.Read(buffer, 0, length) != length)
                throw new InvalidOperationException("Unexpected end of stream");
                
            return buffer;
        }
        public static void WriteInteger(Stream outputStream, int value) {
            if (outputStream == null)
                throw new ArgumentNullException("outputStream");
            byte[] buffer = BitConverter.GetBytes(value);
            outputStream.Write(buffer, 0, buffer.Length);
        }
        public static void WriteLongInteger(Stream outputStream, long value) {
            if (outputStream == null)
                throw new ArgumentNullException("outputStream");
            byte[] buffer = BitConverter.GetBytes(value);
            outputStream.Write(buffer, 0, buffer.Length);
        }
        private static void _WriteLengthEncodedBytes(Stream outputStream, byte[] buffer, int offset, int count) {
            if (outputStream == null)
                throw new ArgumentNullException("outputStream");
            
            StreamHelper.WriteInteger(outputStream, count);
            if (count > 0)
                outputStream.Write(buffer, offset, count);
        }
        public static void WriteLengthEncodedBytes(Stream outputStream, byte[] buffer, int offset, int count) {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("Offset cannot be less than zero.", "offset");
            if (count < 0)
                throw new ArgumentOutOfRangeException("Count cannot be less than zero.", "count");
            if (offset > buffer.Length)
                throw new ArgumentOutOfRangeException("Offset cannot extend past the end of the buffer.", "offset");
            if (offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException("Offset pluc Count cannot extend past the end of the buffer.", "count");
            StreamHelper._WriteLengthEncodedBytes(outputStream, buffer, offset, count);
        }
        public static void WriteLengthEncodedBytes(Stream outputStream, byte[] buffer) {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            StreamHelper._WriteLengthEncodedBytes(outputStream, buffer, 0, buffer.Length);
        }
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
			Gets an instance of the Encoding class, which represents a character encodding class.
        
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

Function Read-XmlDocument {
	[CmdletBinding(DefaultParameterSetName = 'StreamB')]
	Param(
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'File')]
        [Alias('Path')]
        # The URI for the file containing the XML data. The XmlResolver object on the XmlReaderSettings object is used to convert the path to a canonical data representation.
		[string]$InputUri,
        
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Bytes')]
        # Byte array containing the XML data.
		[byte[]]$ByteArray,
        
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'StreamB')]
		[Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'StreamP')]
        # The stream that contains the XML data.
		[System.IO.Stream]$Stream,
        
		[Parameter(Mandatory = $false, Position = 1)]
        # The settings for the new XmlReader instance.
		[System.Xml.XmlReaderSettings]$Settings,
        
		[Parameter(Mandatory = $false, Position = 2, ParameterSetName = 'StreamB')]
		[Parameter(Mandatory = $false, Position = 2, ParameterSetName = 'Bytes')]
        # The base URI for the entity or document being read.
		[string]$BaseUri,
        
		[Parameter(Mandatory = $false, Position = 2, ParameterSetName = 'File')]
		[Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'StreamP')]
        # The context information required to parse the XML fragment. The context information can include the XmlNameTable to use, encoding, namespace scope, the current xml:lang and xml:space scope, base URI, and document type definition.
        [System.Xml.XmlParserContext]$ParserContext
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
    
    if ($PSBoundParameters.ContainsKey('Settings')) {
        $XmlReaderSettings = $Settings
    } else {
        $XmlReaderSettings = New-XmlReaderSettings;
    }
    
    switch ($PSCmdlet.ParameterSetName) {
        'File' {
            if ($PSBoundParameters.ContainsKey('ParserContext')) {
                $XmlReader = [System.Xml.XmlReader]::Create($Path, $XmlReaderSettings, $ParserContext);
            } else {
                $XmlReader = [System.Xml.XmlReader]::Create($Path, $XmlReaderSettings);
            }
            break;
        }
        'StreamP' { $XmlReader = [System.Xml.XmlReader]::Create($Stream, $XmlReaderSettings, $ParserContext); break; }
        'StreamB' {
            if ($PSBoundParameters.ContainsKey('BaseUri')) {
                $XmlReader = [System.Xml.XmlReader]::Create($Stream, $XmlReaderSettings, $BaseUri);
            } else {
                $XmlReader = [System.Xml.XmlReader]::Create($Stream, $XmlReaderSettings);
            }
            break;
        }
        default {
            $MemoryStream = New-Object -TypeName 'System.IO.MemoryStream' -ArgumentList (,$ByteArray);
            try {
                if ($PSBoundParameters.ContainsKey('BaseUri')) {
                    $XmlReader = [System.Xml.XmlReader]::Create($MemoryStream, $XmlReaderSettings, $BaseUri);
                } else {
                    $XmlReader = [System.Xml.XmlReader]::Create($MemoryStream, $XmlReaderSettings);
                }
                if ($XmlReader -eq $null) { $MemoryStream.Dispose(); }
            } catch {
                $MemoryStream.Dispose();
                throw;
            }
        }
    }
    
    if ($XmlReader -eq $null) {
        throw 'Unable to create xml reader.';
        return;
    }
    
    try {
        $XmlDocument = New-Object -TypeName 'System.Xml.XmlDocument';
        $XmlDocument.Load($XmlReader);
        $XmlDocument | Write-Output;
    } catch {
        throw;
    } finally {
        $XmlReader.Dispose();
        if ($AsString -and (-not $XmlWriterSettings.CloseOutput)) { $MemoryStream.Dispose() }
    }
}

Function ConvertTo-XmlString {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 0)]
        [AllowEmptyString()]
        # Value to be converted to an XML string
		[object]$Value,
        
		[Parameter(Mandatory = $false, Position = 1)]
        # The format structure that defines how to display the converted string. Valid formats include "yyyy-MM-ddTHH:mm:sszzzzzz" and its subsets. If Value is not a DateTime or DateTimeOffset, then this is ignored.
        [string]$Format = 'yyyy-MM-ddTHH:mm:sszzzzzz'
    )
	<#
		.SYNOPSIS
			Converts object to XML text.
 
		.DESCRIPTION
			Converts object to text suitable for XML content.
        
		.OUTPUTS
			System.String. The converted object.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlconvert.aspx
	#>
    
    Process {
        if ($Value -is [string]) {
            $Value;
        } else {
            if ($Value -is [bool] -or $value -is [byte] -or $value -is [char] -or $value -is [decimal] -or $value -is [double] -or $value -is [System.Guid] -or $value -is [System.Int16] -or $value -is [int] -or $value -is [long] -or `
                     $value -is [System.SByte] -or $value -is [float] -or $value -is [System.TimeSpan] -or $value -is [System.UInt16] -or $value -is [System.UInt32] -or $value -is [System.UInt64]) {
                [System.Xml.XmlConvert]::ToString($Value);
            } else {
                if ($Value -is [System.DateTime] -or $Value -is [System.DateTimeOffset]) {
                    [System.Xml.XmlConvert]::ToString($Value, $Format);
                } else {
                    $Value | Out-String -Stream;
                }
            }
        }
    }
}

Function ConvertFrom-XmlString {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 0)]
        [AllowEmptyString()]
        [Alias('Text', 'Value')]
        # Value to be converted to an XML string
		[string]$XmlString,
        
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Type')]
        # Type to be converted to.
		[Type]$Type,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'System.Boolean')]
        [Alias('Bool')]
        # Convert XmlString to System.Boolean
		[switch]$Boolean,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'System.Byte')]
        # Convert XmlString to System.Byte
		[switch]$Byte,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'System.Char')]
        # Convert XmlString to System.Char
		[switch]$Char,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'System.DateTime')]
        # Convert XmlString to System.DateTime
		[switch]$DateTime,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'System.DateTimeOffset')]
        # Convert XmlString to System.DateTimeOffset
		[switch]$DateTimeOffset,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'System.Decimal')]
        # Convert XmlString to System.Decimal
		[switch]$Decimal,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'System.Double')]
        # Convert XmlString to System.Double
		[switch]$Double,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'System.Guid')]
        # Convert XmlString to System.Guid
		[switch]$Guid,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'System.Int16')]
        # Convert XmlString to System.Int16
		[switch]$Int16,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'System.Int32')]
        [Alias('Int', 'Integer')]
        # Convert XmlString to System.Int32
		[switch]$Int32,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'System.Int64')]
        [Alias('Long')]
        # Convert XmlString to System.Int64
		[switch]$Int64,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'System.SByte')]
        # Convert XmlString to System.SByte
		[switch]$SByte,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'System.Single')]
        [Alias('Float')]
        # Convert XmlString to System.Single
		[switch]$Single,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'System.TimeSpan')]
        # Convert XmlString to System.TimeSpan
		[switch]$TimeSpan,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'System.UInt16')]
        # Convert XmlString to System.UInt16
		[switch]$UInt16,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'System.UInt32')]
        # Convert XmlString to System.UInt32
		[switch]$UInt32,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'System.UInt64')]
        # Convert XmlString to System.UInt64
		[switch]$UInt64,
        
		[Parameter(Mandatory = $false, ParameterSetName = 'Type')]
		[Parameter(Mandatory = $false, ParameterSetName = 'System.DateTime')]
		[Parameter(Mandatory = $false, ParameterSetName = 'System.DateTimeOffset')]
        [Alias('Format')]
        # An array containing the format structures to apply to the converted DateTime or DateTimeOffset. Valid formats include "yyyy-MM-ddTHH:mm:sszzzzzz" and its subsets. 
		[string[]]$Formats
    )
	<#
		.SYNOPSIS
			Converts XML text to an object.
 
		.DESCRIPTION
			Converts XML text to an object of the specified type.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.xml.xmlconvert.aspx
	#>
    
    Process {
        if ($PSBoundParameters.ContainsKey('Type')) {
            $TypeName = $Type.FullName;
        } else {
            $TypeName = $PSCmdlet.ParameterSetName;
        }
        switch ($TypeName) {
            'System.String' { $Value; break; }
            'System.Boolean' { if ($Value.Length -gt 0) { [System.Xml.XmlConvert]::ToBoolean($Value); } break; }
            'System.Byte' { if ($Value.Length -gt 0) { [System.Xml.XmlConvert]::ToByte($Value); } break; }
            'System.Char' { if ($Value.Length -gt 0) { [System.Xml.XmlConvert]::ToChar($Value); } break; }
            'System.DateTime' {
                if ($Value.Length -gt 0) {
                   if ($PSBoundParameters.ContainsKey('Formats')) {
                        [System.Xml.XmlConvert]::ToDateTime($Value, $Formats);
                    } else {
                        [System.Xml.XmlConvert]::ToDateTime($Value);
                    }
                }
                break;
            }
            'System.DateTimeOffset' {
                if ($Value.Length -gt 0) {
                   if ($PSBoundParameters.ContainsKey('Formats')) {
                        [System.Xml.XmlConvert]::ToDateTimeOffset($Value, $Formats);
                    } else {
                        [System.Xml.XmlConvert]::ToDateTimeOffset($Value);
                    }
                }
                break;
            }
            'System.Decimal' { if ($Value.Length -gt 0) { [System.Xml.XmlConvert]::ToDecimal($Value); } break; }
            'System.Double' { if ($Value.Length -gt 0) { [System.Xml.XmlConvert]::ToDouble($Value); } break; }
            'System.Guid' { if ($Value.Length -gt 0) { [System.Xml.XmlConvert]::ToGuid($Value); } break; }
            'System.Int16' { if ($Value.Length -gt 0) { [System.Xml.XmlConvert]::ToInt16($Value); } break; }
            'System.Int32' { if ($Value.Length -gt 0) { [System.Xml.XmlConvert]::ToInt32($Value); } break; }
            'System.Int64' { if ($Value.Length -gt 0) { [System.Xml.XmlConvert]::ToInt64($Value); } break; }
            'System.SByte' { if ($Value.Length -gt 0) { [System.Xml.XmlConvert]::ToSByte($Value); } break; }
            'System.Single' { if ($Value.Length -gt 0) { [System.Xml.XmlConvert]::ToSingle($Value); } break; }
            'System.TimeSpan' { if ($Value.Length -gt 0) { [System.Xml.XmlConvert]::ToTimeSpan($Value); } break; }
            'System.UInt16' { if ($Value.Length -gt 0) { [System.Xml.XmlConvert]::ToUInt16($Value); } break; }
            'System.UInt32' { if ($Value.Length -gt 0) { [System.Xml.XmlConvert]::ToUInt32($Value); } break; }
            'System.UInt64' { if ($Value.Length -gt 0) { [System.Xml.XmlConvert]::ToUInt64($Value); } break; }
        }
    }
}

Function Get-XmlAttributeValue {
	[CmdletBinding(DefaultParameterSetName = 'Implicit')]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
        # The parent element to which the element is to be appended.
		[System.Xml.XmlElement]$Element,
        
		[Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 1)]
        [Alias('Name')]
        # The local name of the new attribute.
		[string]$LocalName,
        
		[Parameter(Mandatory = $false, ParameterSetName = 'Implicit')]
		[Parameter(Mandatory = $true, ParameterSetName = 'Prefix')]
        # The namespace URI of the new attribute.
		[string]$NamespaceUri
    )
    
    Process {
        $XmlAttribute = $Element.Attributes[$LocalName, $NamespaceUri];
        if ($XmlAttribute -ne $null) { $XmlAttribute.Value }
    }
}

Function Set-XmlAttribute {
	[CmdletBinding(DefaultParameterSetName = 'Implicit')]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
        # The parent element to which the element is to be appended.
		[System.Xml.XmlElement]$Element,
        
		[Parameter(Mandatory = $true, Position = 1)]
        [Alias('Name')]
        # The local name of the new attribute.
		[string]$LocalName,
        
		[Parameter(Mandatory = $true, Position = 2)]
        # The local name of the new attribute.
		[object]$Value,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'Prefix')]
        # The prefix of the new attribute.
		[string]$Prefix,
        
		[Parameter(Mandatory = $false, ParameterSetName = 'Implicit')]
		[Parameter(Mandatory = $true, ParameterSetName = 'Prefix')]
        # The namespace URI of the new attribute.
		[string]$NamespaceUri,
        
		[Parameter(Mandatory = $false, Position = 3)]
        # The format structure that defines how to display the converted string. Valid formats include "yyyy-MM-ddTHH:mm:sszzzzzz" and its subsets. If Value is not a DateTime or DateTimeOffset, then this is ignored.
        [string]$Format
    )
    
    Process {
        $XmlAttribute = $Element.Attributes[$LocalName, $NamespaceUri];
        if ($XmlAttribute -eq $null) {
            if ($PSBoundParameters.ContainsKey('Prefix')) {
                $XmlAttribute = $Element.OwnerDocument.CreateAttribute($Prefix, $LocalName, $NamespaceUri);
            } else {
                if ($PSBoundParameters.ContainsKey('NamespaceUri')) {
                    $XmlAttribute = $Element.OwnerDocument.CreateAttribute($LocalName, $NamespaceUri);
                } else {
                    $XmlAttribute = $Element.OwnerDocument.CreateAttribute($LocalName);
                }
            }
            $XmlAttribute = $Element.Attributes.Append($XmlAttribute);
        }
        if ($Value -is [string]) {
            $XmlAttribute.Value = $Value;
        } else {
            if ($PSBoundParameters.ContainsKey('Format')) {
                $XmlAttribute.Value = $Value | ConvertTo-XmlString -Format $Format;
            } else {
                $XmlAttribute.Value = $Value | ConvertTo-XmlString;
            }
        }
    }
}

Function Add-XmlElement {
	[CmdletBinding(DefaultParameterSetName = 'Implicit')]
    [OutputType([System.Xml.XmlElement])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
        # The parent element to which the element is to be appended.
		[System.Xml.XmlElement]$Parent,
        
		[Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Import')]
        # The XmlElement which is to be imported and appended.
		[System.Xml.XmlElement]$Element,
        
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Implicit')]
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Prefix')]
        [Alias('Name')]
        # The local name of the new element.
		[string]$LocalName,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'Prefix')]
        # The prefix of the new element.
		[string]$Prefix,
        
		[Parameter(Mandatory = $false, ParameterSetName = 'Implicit')]
		[Parameter(Mandatory = $true, ParameterSetName = 'Prefix')]
        # The namespace URI of the new element.
		[string]$NamespaceUri
    )
    
    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'Import' { $XmlElement = $Parent.OwnerDocument.ImportNode($Element); break; }
            'Prefix' { $XmlElement = $Parent.OwnerDocument.CreateElement($Prefix, $LocalName, $NamespaceUri); break; }
            default {
                if ($PSBoundParameters.ContainsKey('NamespaceUri')) {
                    $XmlElement = $Parent.OwnerDocument.CreateElement($LocalName, $NamespaceUri);
                } else {
                    $XmlElement = $Parent.OwnerDocument.CreateElement($LocalName);
                }
            }
        }
    
        $Parent.AppendChild($XmlElement);
    }
}

Function New-XmlDocument {
	[CmdletBinding(DefaultParameterSetName = 'Implicit')]
    [OutputType([System.Xml.XmlDocument])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
        [Alias('Name')]
        [ValidatePattern('^[a-zA-Z_][\W_]*(\.[\W_]+)*$')]
        # The local name of the root element.
		[string]$RootName,
        
		[Parameter(Mandatory = $true, ParameterSetName = 'Prefix')]
        [ValidatePattern('^[a-zA-Z][\W_]*$')]
        # The prefix of the root element.
		[string]$Prefix,
        
		[Parameter(Mandatory = $false, ParameterSetName = 'Implicit')]
		[Parameter(Mandatory = $true, ParameterSetName = 'Prefix')]
        # The namespace URI of the root element.
		[string]$NamespaceUri
    )
    
    $XmlDocument = New-Object -TypeName 'System.Xml.XmlDocument';
    
    if ($PSBoundParameters.ContainsKey('Prefix')) {
        $DocumentElement = $XmlDocument.CreateElement($Prefix, $RootName, $NamespaceUri);
    } else {
        if ($PSBoundParameters.ContainsKey('NamespaceUri')) {
            $DocumentElement = $XmlDocument.CreateElement($RootName, $NamespaceUri);
        } else {
            $DocumentElement = $XmlDocument.CreateElement($RootName);
        }
    }
    
    $XmlDocument.AppendChild($DocumentElement);
}

Function Write-XmlDocument {
	[CmdletBinding(DefaultParameterSetName = 'SettingsArgs')]
    [OutputType([string], ParameterSetName = 'String')]
    [OutputType([byte[]], ParameterSetName = 'Bytes')]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
        # The XmlDocument containing the XML you wish to write.
		[System.Xml.XmlDocument]$XmlDocument,
        
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'File')]
        [Alias('Path')]
        # The file to which you want to write. The XmlWriter creates a file at the specified path and writes to it in XML 1.0 text syntax. The OutputFileName must be a file system path.
		[string]$OutputFileName,
        
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Stream')]
        # The stream to which you want to write. The XmlWriter writes XML 1.0 text syntax and appends it to the specified stream
		[System.IO.Stream]$Stream,
        
		[Parameter(Mandatory = $false, Position = 2, ParameterSetName = 'File')]
		[Parameter(Mandatory = $false, Position = 2, ParameterSetName = 'Stream')]
		[Parameter(Mandatory = $false, Position = 1, ParameterSetName = 'String')]
		[Parameter(Mandatory = $false, Position = 1, ParameterSetName = 'Bytes')]
        # The XmlWriterSettings object used to configure the new XmlWriter instance. If this is not specified, a XmlWriterSettings with default settings is used
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
    
    if ($PSBoundParameters.ContainsKey('Settings')) {
        $XmlWriterSettings = $Settings
    } else {
        $XmlWriterSettings = New-XmlWriterSettings;
    }
    
    switch ($PSCmdlet.ParameterSetName) {
        'File' { $XmlWriter = [System.Xml.XmlWriter]::Create($Path, $XmlWriterSettings); break; }
        'Stream' { $XmlWriter = [System.Xml.XmlWriter]::Create($Stream, $XmlWriterSettings); break; }
        default {
            $MemoryStream = New-Object -TypeName 'System.IO.MemoryStream';
            try {
                $XmlWriter = [System.Xml.XmlWriter]::Create($MemoryStream, $XmlWriterSettings);
                if ($XmlWriter -eq $null) { $MemoryStream.Dispose(); }
            } catch {
                $MemoryStream.Dispose();
                throw;
            }
        }
    }
    
    if ($XmlWriter -eq $null) {
        throw 'Unable to create xml writer.';
        return;
    }
    
    try {
        $XmlDocument.WriteTo($XmlWriter);
        $XmlWriter.Flush();
        if ($AsByteArray) {
            Write-Output -InputObject (,$MemoryStream.ToArray());
        } else {
            if ($AsString) { $XmlWriterSettings.Encoding.GetString($MemoryStream.ToArray()) | Write-Output }
        }
    } catch {
        throw;
    } finally {
        $XmlWriter.Dispose();
        if (($AsString -or $AsByteArray) -and (-not $XmlWriterSettings.CloseOutput)) { $MemoryStream.Dispose() }
    }
}