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
	public class DataBuffer {
		private byte[] _data;
		public byte[] Data { get { return this._data; } }
		public int Capacity { get { return this._data.Length; } }
		private static int? _minBase64BlockSize = null;
		public static int MinBase64BlockSize {
			get {
				if (DataBuffer._minBase64BlockSize.HasValue)
					return DataBuffer._minBase64BlockSize.Value;
				int minBase64BlockSize = 0;
				string s = "";
				Regex regex = new Regex(@"\s");
				do {
					minBase64BlockSize++;
					byte[] buffer = new byte[minBase64BlockSize];
					s = Convert.ToBase64String(buffer, 0, minBase64BlockSize, Base64FormattingOptions.InsertLineBreaks).Trim();
				} while (!regex.IsMatch(s));
				
				DataBuffer._minBase64BlockSize = minBase64BlockSize;
				return minBase64BlockSize;
			}
		}
		internal DataBuffer(byte[] data) {
			if (data == null)
                throw new ArgumentNullException("data");
			this._data = data;
		}
		internal DataBuffer(byte[] data, int minCapacity) {
			if (data == null)
				this._data = new byte[minCapacity];
			else if (data.Length < minCapacity) {
				this._data = new byte[minCapacity];
				Array.Copy(data, 0, this._data, 0, data.Length);
			} else
				this._data = data;
		}
		public DataBuffer(int capacity) {
            if (capacity < 1)
                throw new ArgumentOutOfRangeException("Capacity cannot be less than 1.", "capacity");
			this._data = new byte[capacity];
		}
		public int Read(Stream stream, int offset, int count) {
			if (stream == null)
                throw new ArgumentNullException("stream");
			return stream.Read(this._data, offset, count);
		}
		public void Write(Stream stream, int offset, int count) {
			if (stream == null)
                throw new ArgumentNullException("stream");
			stream.Write(this._data, offset, count);
		}
		public string ToBase64String(int offset, int length, bool insertLineBreaks) {
			if (insertLineBreaks)
				return Convert.ToBase64String(this._data, offset, length, Base64FormattingOptions.InsertLineBreaks);
			return Convert.ToBase64String(this._data, offset, length);
		}
		public static DataBuffer FromBase64String(string s) {
			byte[] data = Convert.FromBase64String(s);
			if (data.Length == 0)
				return null;
				
			return new DataBuffer(data);
		}
		public static DataBuffer FromBase64String(string s, int minCapacity) {
			return new DataBuffer(Convert.FromBase64String(s), minCapacity);
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
    
    return [UserFileUtils.DataBuffer]::MinBase64BlockSize;
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
	#>
    
    return [UserFileUtils.StreamHelper]::ReadLongInteger($Stream);
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
	#>

    return ,[UserFileUtils.StreamHelper]::ReadLengthEncodedBytes($Stream);
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
	#>
    
    [UserFileUtils.StreamHelper]::WriteLongInteger($Stream, $Value);
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

Function New-DataBuffer {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
		# Minimum capacity, in bytes, of the new data buffer.
        [int]$Capacity,
		
		[Parameter(Mandatory = $false)]
		# Ensures capacity value can be evenly divided by the minimum base-64 block size.
		[switch]$Base64Encoding
    )
	<#
		.SYNOPSIS
			Create new data buffer object.
 
		.DESCRIPTION
			Initialize a new object for the purpose of buffering data.
	#>
	
	if ($Base64Encoding) {
        $c = $Capacity - $Capacity % [UserFileUtils.DataBuffer]::MinBase64BlockSize;
		if ($c -lt [UserFileUtils.DataBuffer]::MinBase64BlockSize) { $c = [UserFileUtils.DataBuffer]::MinBase64BlockSize }
		New-Object -TypeName 'UserFileUtils.DataBuffer' -ArgumentList $c;
	} else {
		New-Object -TypeName 'UserFileUtils.DataBuffer' -ArgumentList $Capacity;
	}
}

Function ConvertTo-Base64String {
    [CmdletBinding()]
    [OutputType([string])]
    Param(
        [Parameter(Mandatory = $true)]
		# Data buffer to be converted to base-64 encoded text.
        [UserFileUtils.DataBuffer]$Buffer,
		
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
	#>
	
	if ($PSBoundParameters.ContainsKey('Length')) {
		$Buffer.ToBase64String($Offset, $Length, $InsertLineBreaks.IsPresent);
	} else {
		$Buffer.ToBase64String($Offset, $Buffer.Capacity, $InsertLineBreaks.IsPresent);
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
			UserFileUtils.DataBuffer. Data buffer object.
	#>
	
	if ($PSBoundParameters.ContainsKey('MinCapacity')) {
		[UserFileUtils.DataBuffer]::FromBase64String($InputString, $MinCapacity);
	} else {
		[UserFileUtils.DataBuffer]::FromBase64String($InputString);
	}
}

Function Read-DataBuffer {
    [CmdletBinding()]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true)]
		# Data buffer which will contain the data that was read.
        [UserFileUtils.DataBuffer]$Buffer,
		
        [Parameter(Mandatory = $true)]
		# Stream to read data from.
        [System.IO.Stream]$Stream,
		
        [Parameter(Mandatory = $false)]
		# Offset within data buffer, in bytes, to begin reading.
        [int]$Offset = 0,
		
        [Parameter(Mandatory = $false)]
		# Number of bytes to read
        [int]$Count
    )
	<#
		.SYNOPSIS
			Read block of data into data buffer.
 
		.DESCRIPTION
			Reads a block of data into the data buffer.

		.OUTPUTS
			System.Integer. The number of bytes read from the Stream.
	#>
	
	
	if ($PSBoundParameters.ContainsKey('Count')) {
		$Buffer.Read($Stream, $Offset, $Count);
	} else {
		$Buffer.Read($Stream, $Offset, $Buffer.Capacity);
	}
}

Function Write-DataBuffer {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
		# Data buffer containing data to be written
        [UserFileUtils.DataBuffer]$Buffer,
		
        [Parameter(Mandatory = $true)]
		# Stream to write data to.
        [System.IO.Stream]$Stream,
		
        [Parameter(Mandatory = $false)]
		# Offset within data buffer, in bytes, to begin writing.
        [int]$Offset = 0,
		
        [Parameter(Mandatory = $false)]
		# Number of bytes to write
        [int]$Count
    )
	<#
		.SYNOPSIS
			Write block of data to Stream.
 
		.DESCRIPTION
			Writes a block of data from the data buffer, to the Stream.
	#>
	
	if ($PSBoundParameters.ContainsKey('Count')) {
		$Buffer.Write($Stream, $Offset, $Count);
	} else {
		$Buffer.Write($Stream, $Offset, $Buffer.Capacity);
	}
}