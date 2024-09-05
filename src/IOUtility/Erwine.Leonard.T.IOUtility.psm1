if ($null -eq $Script:InvalidFileNameChars) {
    New-Variable -Name 'InvalidFileNameChars' -Option ReadOnly -Scope 'Script' -Value (&{
        [System.Management.Automation.ProviderInfo]$FileSystemProvider = Get-PSProvider -PSProvider 'FileSystem';
        if ($FileSystemProvider.AltItemSeparator -ine $FileSystemProvider.ItemSeparator -and -not [string]::IsNullOrEmpty($FileSystemProvider.AltItemSeparator)) {
            if ($FileSystemProvider.VolumeSeparatedByColon) { return ([string[]]@($FileSystemProvider.ItemSeparator, $FileSystemProvider.AltItemSeparator, ':')); }
            return ([string[]]@($FileSystemProvider.ItemSeparator, $FileSystemProvider.AltItemSeparator));
        }
        if ($FileSystemProvider.VolumeSeparatedByColon) { return ([string[]]@($FileSystemProvider.ItemSeparator, ':')); }
        return ([string[]]@($FileSystemProvider.ItemSeparator));
    });
    
    New-Variable -Name 'Int16ByteLength' -Scope 'Script' -Option Constant -Value [System.BitConverter]::GetBytes([Int16]0).Length;
    New-Variable -Name 'UInt16ByteLength' -Scope 'Script' -Option Constant -Value [System.BitConverter]::GetBytes([UInt16]0).Length;
    New-Variable -Name 'Int32ByteLength' -Scope 'Script' -Option Constant -Value [System.BitConverter]::GetBytes([int]0).Length;
    New-Variable -Name 'UInt32ByteLength' -Scope 'Script' -Option Constant -Value [System.BitConverter]::GetBytes([UInt32]0).Length;
    New-Variable -Name 'Int64ByteLength' -Scope 'Script' -Option Constant -Value [System.BitConverter]::GetBytes([Int64]0).Length;
    New-Variable -Name 'UInt64ByteLength' -Scope 'Script' -Option Constant -Value [System.BitConverter]::GetBytes([UInt64]0).Length;
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
        Function ConvertChars([string]$Text) {
            [char[]]$Converted = ($Text.ToCharArray() | ForEach-Object {
                if ($Script:InvalidFileNameChars -icontains $_) {
                    "_0x$(([int]$_).ToString('x4'))_".ToCharArray() | Write-Output;
                } else {
                    $_ | Write-Output;
                }
            });
            if ($Converted.Length -eq $InputText.Length) { return $InputText }
            return [string]::new($Converted);
        }
    }

	Process {
        foreach ($Text in $InputText) {
            if ($Text[0] -eq ' ') {
                if ($Text.Length -eq 1) {
                    '_0020_' | Write-Output;
                } else {
                    if ($Text[-1] -eq ' ') {
                        if ($Text.Length -eq 2) {
                            '_0020__0020_' | Write-Output;
                        } else {
                            ('_0020_' + (ConvertChars($Text.Substring(1, $Text.Length - 2))) + '_0020_') | Write-Output;
                        }
                    } else {
                        ('_0020_' + (ConvertChars($Text.Substring(1, $Text.Length - 2)))) | Write-Output;
                    }
                }
            } else {
                if ($Text[-1] -eq ' ') {
                    ((ConvertChars($Text.Substring(0, $Text.Length - 1))) + '_0020_') | Write-Output;
                } else {
                    ConvertChars($Text) | Write-Output;
                }
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

Function Read-ShortIntegerFromStream {
	<#
		.SYNOPSIS
			Read integer value from a stream.
 
		.DESCRIPTION
			Reads bytes from a stream and converts them to an integer.
			
		.OUTPUTS
			System.Int32. Integer value read from stream.
		
		.LINK
			Write-ShortIntegerToStream
		
		.LINK
			Read-UnsignedShortIntegerFromStream
		
		.LINK
			Read-IntegerFromStream
		
		.LINK
			Read-LongIntegerFromStream
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	[OutputType([Int16])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
        [ValidateScript({ $_.CanRead })]
		# Stream from which to read the bytes of an integer value.
		[System.IO.Stream]$Stream
	)
	
    $Buffer = New-Object -TypeName 'System.Byte' -ArgumentList $Script:Int16ByteLength;
    if ($Stream.CanSeek) {
        $Position = $Stream.Position;
        $Count = $Stream.Read($Buffer, 0, $Script:Int16ByteLength);
        if ($Count -eq $Script:Int16ByteLength) { return [System.BitConverter]::ToInt16($Buffer, 0) }
        $Stream.Seek($Position, [System.IO.SeekOrigin]::Begin) | Out-Null;
    } else {
        $Count = $Stream.Read($Buffer, 0, $Script:Int16ByteLength);
        if ($Count -eq $Script:Int16ByteLength) { return [System.BitConverter]::ToInt16($Buffer, 0) }
    }
    Write-Error -Message 'Unexpected end of stream' -Category InvalidOperation -ErrorId 'UnexpectedEOF' `
            -CategoryReason "[System.IO.Stream].Read(byte[], 0, $Script:Int16ByteLength) returned $Count";
}

Function Read-UnsignedShortIntegerFromStream {
	<#
		.SYNOPSIS
			Read integer value from a stream.
 
		.DESCRIPTION
			Reads bytes from a stream and converts them to an integer.
			
		.OUTPUTS
			System.Int32. Integer value read from stream.
		
		.LINK
			Write-UnsignedShortIntegerToStream
		
		.LINK
			Read-ShortIntegerFromStream
		
		.LINK
			Read-UnsignedIntegerFromStream
		
		.LINK
			Read-UnsignedLongIntegerFromStream
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	[OutputType([UInt16])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
        [ValidateScript({ $_.CanRead })]
		# Stream from which to read the bytes of an integer value.
		[System.IO.Stream]$Stream
	)
	
    $Buffer = New-Object -TypeName 'System.Byte' -ArgumentList $Script:UInt16ByteLength;
    if ($Stream.CanSeek) {
        $Position = $Stream.Position;
        $Count = $Stream.Read($Buffer, 0, $Script:UInt16ByteLength);
        if ($Count -eq $Script:UInt16ByteLength) { return [System.BitConverter]::ToUInt16($Buffer, 0) }
        $Stream.Seek($Position, [System.IO.SeekOrigin]::Begin) | Out-Null;
    } else {
        $Count = $Stream.Read($Buffer, 0, $Script:UInt16ByteLength);
        if ($Count -eq $Script:Int16ByteLength) { return [System.BitConverter]::ToUInt16($Buffer, 0) }
    }
    Write-Error -Message 'Unexpected end of stream' -Category InvalidOperation -ErrorId 'UnexpectedEOF' `
            -CategoryReason "[System.IO.Stream].Read(byte[], 0, $Script:UInt16ByteLength) returned $Count";
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
			Read-UnsignedIntegerFromStream
		
		.LINK
			Read-ShortIntegerFromStream
		
		.LINK
			Read-LongIntegerFromStream
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	[OutputType([int])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
        [ValidateScript({ $_.CanRead })]
		# Stream from which to read the bytes of an integer value.
		[System.IO.Stream]$Stream
	)
	
    $Buffer = New-Object -TypeName 'System.Byte' -ArgumentList $Script:Int32ByteLength;
    if ($Stream.CanSeek) {
        $Position = $Stream.Position;
        $Count = $Stream.Read($Buffer, 0, $Script:Int32ByteLength);
        if ($Count -eq $Script:Int32ByteLength) { return [System.BitConverter]::ToInt32($Buffer, 0) }
        $Stream.Seek($Position, [System.IO.SeekOrigin]::Begin) | Out-Null;
    } else {
        $Count = $Stream.Read($Buffer, 0, $Script:Int32ByteLength);
        if ($Count -eq $Script:Int32ByteLength) { return [System.BitConverter]::ToInt32($Buffer, 0) }
    }
    Write-Error -Message 'Unexpected end of stream' -Category InvalidOperation -ErrorId 'UnexpectedEOF' `
            -CategoryReason "[System.IO.Stream].Read(byte[], 0, $Script:Int32ByteLength) returned $Count";
}

Function Read-UnsignedIntegerFromStream {
	<#
		.SYNOPSIS
			Read unsigned integer value from a stream.
 
		.DESCRIPTION
			Reads bytes from a stream and converts them to an unsigned integer.
			
		.OUTPUTS
			System.Int32. Integer value read from stream.
		
		.LINK
			Write-UnsignedIntegerToStream
		
		.LINK
			Read-IntegerFromStream
		
		.LINK
			Read-UnsignedShortIntegerFromStream
		
		.LINK
			Read-UnsignedLongIntegerFromStream
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	[OutputType([UInt32])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
        [ValidateScript({ $_.CanRead })]
		# Stream from which to read the bytes of an integer value.
		[System.IO.Stream]$Stream
	)
	
    $Buffer = New-Object -TypeName 'System.Byte' -ArgumentList $Script:UInt32ByteLength;
    if ($Stream.CanSeek) {
        $Position = $Stream.Position;
        $Count = $Stream.Read($Buffer, 0, $Script:UInt32ByteLength);
        if ($Count -eq $Script:UInt32ByteLength) { return [System.BitConverter]::ToUInt32($Buffer, 0) }
        $Stream.Seek($Position, [System.IO.SeekOrigin]::Begin) | Out-Null;
    } else {
        $Count = $Stream.Read($Buffer, 0, $Script:UInt32ByteLength);
        if ($Count -eq $Script:UInt32ByteLength) { return [System.BitConverter]::ToUInt32($Buffer, 0) }
    }
    Write-Error -Message 'Unexpected end of stream' -Category InvalidOperation -ErrorId 'UnexpectedEOF' `
            -CategoryReason "[System.IO.Stream].Read(byte[], 0, $Script:UInt32ByteLength) returned $Count";
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
			Read-UnsignedLongIntegerFromStream
		
		.LINK
			Read-ShortIntegerFromStream
		
		.LINK
			Read-IntegerFromStream
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	[OutputType([long])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
        [ValidateScript({ $_.CanRead })]
		# Stream from which to read the bytes of a long integer value.
		[System.IO.Stream]$Stream
	)
	
    $Buffer = New-Object -TypeName 'System.Byte' -ArgumentList $Script:Int64ByteLength;
    if ($Stream.CanSeek) {
        $Position = $Stream.Position;
        $Count = $Stream.Read($Buffer, 0, $Script:Int64ByteLength);
        if ($Count -eq $Script:Int64ByteLength) { return [System.BitConverter]::ToInt64($Buffer, 0) }
        $Stream.Seek($Position, [System.IO.SeekOrigin]::Begin) | Out-Null;
    } else {
        $Count = $Stream.Read($Buffer, 0, $Script:Int64ByteLength);
        if ($Count -eq $Script:Int64ByteLength) { return [System.BitConverter]::ToInt64($Buffer, 0) }
    }
    Write-Error -Message 'Unexpected end of stream' -Category InvalidOperation -ErrorId 'UnexpectedEOF' `
            -CategoryReason "[System.IO.Stream].Read(byte[], 0, $Script:Int64ByteLength) returned $Count";
}

Function Read-UnsignedLongIntegerFromStream {
	<#
		.SYNOPSIS
			Read unsigned long integer value from a stream.
 
		.DESCRIPTION
			Reads bytes from a stream and converts them to an unsigned long integer.
			
		.OUTPUTS
			System.UInt64. Unsigned Long Integer value read from stream.
		
		.LINK
			Write-UnsignedLongIntegerToStream
		
		.LINK
			Read-LongIntegerFromStream
		
		.LINK
			Read-UnsignedShortIntegerFromStream
		
		.LINK
			Read-UnsignedIntegerFromStream
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	[OutputType([UInt64])]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
        [ValidateScript({ $_.CanRead })]
		# Stream from which to read the bytes of a long integer value.
		[System.IO.Stream]$Stream
	)
	
    $Buffer = New-Object -TypeName 'System.Byte' -ArgumentList $Script:UInt64ByteLength;
    if ($Stream.CanSeek) {
        $Position = $Stream.Position;
        $Count = $Stream.Read($Buffer, 0, $Script:UInt64ByteLength);
        if ($Count -eq $Script:UInt64ByteLength) { return [System.BitConverter]::ToUInt64($Buffer, 0) }
        $Stream.Seek($Position, [System.IO.SeekOrigin]::Begin) | Out-Null;
    } else {
        $Count = $Stream.Read($Buffer, 0, $Script:UInt64ByteLength);
        if ($Count -eq $Script:UInt64ByteLength) { return [System.BitConverter]::ToUInt64($Buffer, 0) }
    }
    Write-Error -Message 'Unexpected end of stream' -Category InvalidOperation -ErrorId 'UnexpectedEOF' `
            -CategoryReason "[System.IO.Stream].Read(byte[], 0, $Script:UInt64ByteLength) returned $Count";
}

Function Write-ShortIntegerToStream {
	<#
		.SYNOPSIS
			Write integer value to a stream.
 
		.DESCRIPTION
			Writes an integer value to the Stream as an array of bytes.
		
		.LINK
			Read-ShortIntegerFromStream
		
		.LINK
			Write-UnsignedShortIntegerToStream
		
		.LINK
			Write-IntegerToStream
		
		.LINK
			Write-LongIntegerToStream
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
        [ValidateScript({ $_.CanWrite })]
		# Stream to write integer value to
		[System.IO.Stream]$Stream,
		
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		# Integer value to be written
		[Int16]$Value
	)
	
	Process {
        $Buffer = [System.BitConverter]::GetBytes($Value);
        $Stream.Write($Buffer, 0, $Buffer.Length) | Out-Null;
    }
}

Function Write-UnsignedShortIntegerToStream {
	<#
		.SYNOPSIS
			Write unsigned integer value to a stream.
 
		.DESCRIPTION
			Writes an unsigned integer value to the Stream as an array of bytes.
		
		.LINK
			Read-UnsignedShortIntegerFromStream
		
		.LINK
			Write-ShortIntegerToStream
		
		.LINK
			Write-UnsignedIntegerToStream
		
		.LINK
			Write-UnsignedLongIntegerToStream
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
        [ValidateScript({ $_.CanWrite })]
		# Stream to write unsigned short integer value to
		[System.IO.Stream]$Stream,
		
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		# Integer value to be written
		[UInt16]$Value
	)
	
	Process {
        $Buffer = [System.BitConverter]::GetBytes($Value);
        $Stream.Write($Buffer, 0, $Buffer.Length) | Out-Null;
    }
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
			Write-UnsignedIntegerToStream
		
		.LINK
			Write-ShortIntegerToStream
		
		.LINK
			Write-LongIntegerToStream
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
        [ValidateScript({ $_.CanWrite })]
		# Stream to write integer value to
		[System.IO.Stream]$Stream,
		
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		# Integer value to be written
		[int]$Value
	)
	
	Process {
        $Buffer = [System.BitConverter]::GetBytes($Value);
        $Stream.Write($Buffer, 0, $Buffer.Length) | Out-Null;
    }
}

Function Write-UnsignedIntegerToStream {
	<#
		.SYNOPSIS
			Write unsigned integer value to a stream.
 
		.DESCRIPTION
			Writes an unsigned integer value to the Stream as an array of bytes.
		
		.LINK
			Read-UnsignedIntegerFromStream
		
		.LINK
			Write-IntegerToStream
		
		.LINK
			Write-ShortIntegerToStream
		
		.LINK
			Write-UnsignedLongIntegerToStream
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
        [ValidateScript({ $_.CanWrite })]
		# Stream to write unsigned integer value to
		[System.IO.Stream]$Stream,
		
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		# Integer value to be written
		[UInt32]$Value
	)
	
	Process {
        $Buffer = [System.BitConverter]::GetBytes($Value);
        $Stream.Write($Buffer, 0, $Buffer.Length) | Out-Null;
    }
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
			Write-UnsignedLongIntegerToStream
		
		.LINK
			Write-ShortIntegerToStream
		
		.LINK
			Write-IntegerToStream
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
        [ValidateScript({ $_.CanWrite })]
		# Stream to write long integer value to
		[System.IO.Stream]$Stream,
		
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		# Long Integer value to be written
		[long]$Value
	)
	
	Process {
        $Buffer = [System.BitConverter]::GetBytes($Value);
        $Stream.Write($Buffer, 0, $Buffer.Length) | Out-Null;
    }
}

Function Write-UnsignedLongIntegerToStream {
	<#
		.SYNOPSIS
			Write unsigned long integer value to a stream.
 
		.DESCRIPTION
			Writes an unsigned long integer value to the Stream as an array of bytes.
		
		.LINK
			Read-UnsignedLongIntegerFromStream
		
		.LINK
			Write-LongIntegerToStream
		
		.LINK
			Write-ShortIntegerToStream
		
		.LINK
			Write-UnsignedIntegerToStream
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true, Position = 0)]
        [ValidateScript({ $_.CanWrite })]
		# Stream to write unsigned long integer value to
		[System.IO.Stream]$Stream,
		
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		# Long Integer value to be written
		[UInt64]$Value
	)
	
	Process {
        $Buffer = [System.BitConverter]::GetBytes($Value);
        $Stream.Write($Buffer, 0, $Buffer.Length) | Out-Null;
    }
}

Function Read-TinyLengthEncodedBytes {
	<#
		.SYNOPSIS
			Read 8 bit length-encoded array of bytes from a stream.
 
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
        [ValidateScript({ $_.CanRead })]
		# Stream to read length-encoded data from.
		[System.IO.Stream]$Stream
	)

    if ($Stream.CanSeek) {
        $Position = $Stream.Position;
        [int]$Length = $Stream.GetByte();
        $Buffer = New-Object -TypeName 'System.Byte' -ArgumentList ($Length);
        if ($Length -gt 0) {
            $Count = $Stream.Read($Buffer, 0, $Length);
            if ($Count -ne $Length) {
                $Stream.Seek($Position, [System.IO.SeekOrigin]::Begin) | Out-Null;
                Write-Error -Message 'Unexpected end of stream' -Category InvalidOperation -ErrorId 'UnexpectedEOF' `
                        -CategoryReason "[System.IO.Stream].Read(byte[], 0, $Length) returned $Count";
            } else {
                Write-Output -InputObject $Buffer -NoEnumerate;
            }
        } else {
            Write-Output -InputObject $Buffer -NoEnumerate;
        }
    } else {
        [int]$Length = $Stream.GetByte();
        if ($Length -lt 0) {
            Write-Error -Message 'Invalid length value' -Category InvalidOperation -ErrorId 'InvalidLength' `
                    -CategoryReason "Read-IntegerFromStream returned $Length";
        } else {
            $Buffer = New-Object -TypeName 'System.Byte' -ArgumentList $Length;
            $Count = $Stream.Read($Buffer, 0, $Length);
            if ($Count -ne $Length) {
                Write-Error -Message 'Unexpected end of stream' -Category InvalidOperation -ErrorId 'UnexpectedEOF' `
                        -CategoryReason "[System.IO.Stream].Read(byte[], 0, $Length) returned $Count";
            } else {
                Write-Output -InputObject $Buffer -NoEnumerate;
            }
        }
    }
}

Function Read-ShortLengthEncodedBytes {
	<#
		.SYNOPSIS
			Read 16 bit length-encoded array of bytes from a stream.
 
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
        [ValidateScript({ $_.CanRead })]
		# Stream to read length-encoded data from.
		[System.IO.Stream]$Stream
	)

    if ($Stream.CanSeek) {
        $Position = $Stream.Position;
        [int]$Length = Read-UnsignedShortIntegerFromStream -Stream $Stream;
        $Buffer = New-Object -TypeName 'System.Byte' -ArgumentList ($Length);
        if ($Length -gt 0) {
            $Count = $Stream.Read($Buffer, 0, $Length);
            if ($Count -ne $Length) {
                $Stream.Seek($Position, [System.IO.SeekOrigin]::Begin) | Out-Null;
                Write-Error -Message 'Unexpected end of stream' -Category InvalidOperation -ErrorId 'UnexpectedEOF' `
                        -CategoryReason "[System.IO.Stream].Read(byte[], 0, $Length) returned $Count";
            } else {
                Write-Output -InputObject $Buffer -NoEnumerate;
            }
        } else {
            Write-Output -InputObject $Buffer -NoEnumerate;
        }
    } else {
        [int]$Length = Read-UnsignedShortIntegerFromStream -Stream $Stream;
        if ($Length -lt 0) {
            Write-Error -Message 'Invalid length value' -Category InvalidOperation -ErrorId 'InvalidLength' `
                    -CategoryReason "Read-IntegerFromStream returned $Length";
        } else {
            $Buffer = New-Object -TypeName 'System.Byte' -ArgumentList $Length;
            $Count = $Stream.Read($Buffer, 0, $Length);
            if ($Count -ne $Length) {
                Write-Error -Message 'Unexpected end of stream' -Category InvalidOperation -ErrorId 'UnexpectedEOF' `
                        -CategoryReason "[System.IO.Stream].Read(byte[], 0, $Length) returned $Count";
            } else {
                Write-Output -InputObject $Buffer -NoEnumerate;
            }
        }
    }
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
        [ValidateScript({ $_.CanRead })]
		# Stream to read length-encoded data from.
		[System.IO.Stream]$Stream
	)

    $Length = 0;
    if ($Stream.CanSeek) {
        $Position = $Stream.Position;
        $Length = Read-IntegerFromStream -Stream $Stream;
        if ($Length -lt 0) {
            $Stream.Seek($Position, [System.IO.SeekOrigin]::Begin) | Out-Null;
            Write-Error -Message 'Invalid length value' -Category InvalidOperation -ErrorId 'InvalidLength' `
                    -CategoryReason "Read-IntegerFromStream returned $Length";
        } else {
            $Buffer = New-Object -TypeName 'System.Byte' -ArgumentList $Length;
            if ($Length -gt 0) {
                $Count = $Stream.Read($Buffer, 0, $Length);
                if ($Count -ne $Length) {
                    $Stream.Seek($Position, [System.IO.SeekOrigin]::Begin) | Out-Null;
                    Write-Error -Message 'Unexpected end of stream' -Category InvalidOperation -ErrorId 'UnexpectedEOF' `
                            -CategoryReason "[System.IO.Stream].Read(byte[], 0, $Length) returned $Count";
                } else {
                    Write-Output -InputObject $Buffer -NoEnumerate;
                }
            } else {
                Write-Output -InputObject $Buffer -NoEnumerate;
            }
        }
    } else {
        $Length = Read-IntegerFromStream -Stream $Stream;
        if ($Length -lt 0) {
            Write-Error -Message 'Invalid length value' -Category InvalidOperation -ErrorId 'InvalidLength' `
                    -CategoryReason "Read-IntegerFromStream returned $Length";
        } else {
            $Buffer = New-Object -TypeName 'System.Byte' -ArgumentList $Length;
            if ($Length -gt 0) {
                $Count = $Stream.Read($Buffer, 0, $Length);
                if ($Count -ne $Length) {
                    Write-Error -Message 'Unexpected end of stream' -Category InvalidOperation -ErrorId 'UnexpectedEOF' `
                            -CategoryReason "[System.IO.Stream].Read(byte[], 0, $Length) returned $Count";
                } else {
                    Write-Output -InputObject $Buffer -NoEnumerate;
                }
            } else {
                Write-Output -InputObject $Buffer -NoEnumerate;
            }
        }
    }
}

Function Write-TinyLengthEncodedBytes {
	<#
		.SYNOPSIS
			Writes 8-bit length-encoded data a stream.
 
		.DESCRIPTION
			Writes a 8-bit length-encoded byte array to the Stream.
		
		.LINK
			Read-TinyLengthEncodedBytes
		
		.LINK
			Write-ShortLengthEncodedBytes
		
		.LINK
			Write-LengthEncodedBytes
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true)]
		# Stream to write length-encoded data from
		[System.IO.Stream]$Stream,

		[Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
		# Bytes to write
		[byte[]]$Bytes,

		[Parameter(Mandatory = $false)]
        [ValidateRange(0, [int]::MaxValue)]
		# Offset within the array of bytes to be writing
		[int]$Offset = 0,

		[Parameter(Mandatory = $false)]
		# Number of bytes to write
		[byte]$Count
	)

    if ($PSBoundParameters.ContainsKey('Offset') -and $Offset -gt 0) {
        if ($PSBoundParameters.ContainsKey('Count')) {
            if (([long]$Offset) + ([long]$Count) -gt ([long]($Bytes.Length))) {
                Write-Error -Message 'Offset + Count is greater than the number of bytes' -Category InvalidOperation -ErrorId 'OutOfRange' `
                        -CategoryReason "Offset $Offset + Count $Count is greater than $($Bytes.Length)";
            } else {
                $Stream.WriteByte($Count);
                if ($Count -gt 0) {
                    $Written = $Stream.Write($Bytes, $Offset, $Count);
                    if ($Written -ne $Count) {
                        Write-Error -Message 'Not all bytes were written' -Category InvalidOperation -ErrorId 'BytesNotWritten' `
                                -CategoryReason "[System.IO.Stream].Write(byte[], $Offset, $Count) returned $Written";
                    }
                }
            }
        } else {
            $c = $Bytes.Length - $Offset;
            if ($c -lt 0) {
                Write-Error -Message 'Offset is greater than the number of bytes' -Category InvalidOperation -ErrorId 'OutOfRange' `
                        -CategoryReason "Offset $Offset is greater than $($Bytes.Length)";
            } else {
                $Stream.WriteByte(([byte]$c));
                if ($c -gt 0) {
                    $Written = $Stream.Write($Bytes, $Offset, $c);
                    if ($Written -ne $c) {
                        Write-Error -Message 'Not all bytes were written' -Category InvalidOperation -ErrorId 'BytesNotWritten' `
                                -CategoryReason "[System.IO.Stream].Write(byte[], $Offset, $c) returned $Written";
                    }
                }
            }
        }
    } else {
        if ($PSBoundParameters.ContainsKey($Count)) {
            if ($Count -gt $Bytes.Length) {
                Write-Error -Message 'Count is greater than the number of bytes' -Category InvalidOperation -ErrorId 'OutOfRange' `
                        -CategoryReason "Count $Count is greater than $($Bytes.Length)";
            } else {
                $Stream.WriteByte($Count);
                if ($Count -gt 0) {
                    $Written = $Stream.Write($Bytes, 0, $Count);
                    if ($Written -ne $Count) {
                        Write-Error -Message 'Not all bytes were written' -Category InvalidOperation -ErrorId 'BytesNotWritten' `
                                -CategoryReason "[System.IO.Stream].Write(byte[], 0, $Count) returned $Written";
                    }
                }
            }
        } else {
            $c = $Bytes.Length;
            $Stream.WriteByte(([byte]$c));
            if ($c -gt 0) {
                $Written = $Stream.Write($Bytes, 0, $c);
                if ($Written -ne $c) {
                    Write-Error -Message 'Not all bytes were written' -Category InvalidOperation -ErrorId 'BytesNotWritten' `
                            -CategoryReason "[System.IO.Stream].Write(byte[], 0, $c) returned $Written";
                }
            }
        }
    }
}

Function Write-ShortLengthEncodedBytes {
	<#
		.SYNOPSIS
			Writes 16-bit length-encoded data a stream.
 
		.DESCRIPTION
			Writes a 16-bit length-encoded byte array to the Stream.
		
		.LINK
			Read-ShortLengthEncodedBytes
		
		.LINK
			Write-TinyLengthEncodedBytes
		
		.LINK
			Write-LengthEncodedBytes
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true)]
		# Stream to write length-encoded data from
		[System.IO.Stream]$Stream,

		[Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
		# Bytes to write
		[byte[]]$Bytes,

		[Parameter(Mandatory = $false)]
        [ValidateRange(0, [int]::MaxValue)]
		# Offset within the array of bytes to be writing
		[int]$Offset = 0,

		[Parameter(Mandatory = $false)]
		# Number of bytes to write
		[UInt16]$Count
	)

    if ($PSBoundParameters.ContainsKey('Offset') -and $Offset -gt 0) {
        if ($PSBoundParameters.ContainsKey('Count')) {
            if (([long]$Offset) + ([long]$Count) -gt ([long]($Bytes.Length))) {
                Write-Error -Message 'Offset + Count is greater than the number of bytes' -Category InvalidOperation -ErrorId 'OutOfRange' `
                        -CategoryReason "Offset $Offset + Count $Count is greater than $($Bytes.Length)";
            } else {
                Write-UnsignedShortIntegerToStream -Stream $Stream -Value $Count;
                if ($Count -gt 0) {
                    $Written = $Stream.Write($Bytes, $Offset, $Count);
                    if ($Written -ne $Count) {
                        Write-Error -Message 'Not all bytes were written' -Category InvalidOperation -ErrorId 'BytesNotWritten' `
                                -CategoryReason "[System.IO.Stream].Write(byte[], $Offset, $Count) returned $Written";
                    }
                }
            }
        } else {
            $c = $Bytes.Length - $Offset;
            if ($c -lt 0) {
                Write-Error -Message 'Offset is greater than the number of bytes' -Category InvalidOperation -ErrorId 'OutOfRange' `
                        -CategoryReason "Offset $Offset is greater than $($Bytes.Length)";
            } else {
                Write-UnsignedShortIntegerToStream -Stream $Stream -Value ([UInt16]$c);
                if ($c -gt 0) {
                    $Written = $Stream.Write($Bytes, $Offset, $c);
                    if ($Written -ne $c) {
                        Write-Error -Message 'Not all bytes were written' -Category InvalidOperation -ErrorId 'BytesNotWritten' `
                                -CategoryReason "[System.IO.Stream].Write(byte[], $Offset, $c) returned $Written";
                    }
                }
            }
        }
    } else {
        if ($PSBoundParameters.ContainsKey($Count)) {
            if ($Count -gt $Bytes.Length) {
                Write-Error -Message 'Count is greater than the number of bytes' -Category InvalidOperation -ErrorId 'OutOfRange' `
                        -CategoryReason "Count $Count is greater than $($Bytes.Length)";
            } else {
                Write-UnsignedShortIntegerToStream -Stream $Stream -Value $Count;
                if ($Count -gt 0) {
                    $Written = $Stream.Write($Bytes, 0, $Count);
                    if ($Written -ne $Count) {
                        Write-Error -Message 'Not all bytes were written' -Category InvalidOperation -ErrorId 'BytesNotWritten' `
                                -CategoryReason "[System.IO.Stream].Write(byte[], 0, $Count) returned $Written";
                    }
                }
            }
        } else {
            $c = $Bytes.Length;
            Write-UnsignedShortIntegerToStream -Stream $Stream -Value ([UInt16]$c);
            if ($c -gt 0) {
                $Written = $Stream.Write($Bytes, 0, $c);
                if ($Written -ne $c) {
                    Write-Error -Message 'Not all bytes were written' -Category InvalidOperation -ErrorId 'BytesNotWritten' `
                            -CategoryReason "[System.IO.Stream].Write(byte[], 0, $c) returned $Written";
                }
            }
        }
    }
}

Function Write-LengthEncodedBytes {
	<#
		.SYNOPSIS
			Writes length-encoded data a stream.
 
		.DESCRIPTION
			Writes a 32-bit length-encoded byte array to the Stream.
		
		.LINK
			Read-LengthEncodedBytes
		
		.LINK
			Write-TinyLengthEncodedBytes
		
		.LINK
			Write-ShortLengthEncodedBytes
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.io.stream.aspx
	#>
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true)]
		# Stream to write length-encoded data from
		[System.IO.Stream]$Stream,

		[Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
		# Bytes to write
		[byte[]]$Bytes,

		[Parameter(Mandatory = $false)]
        [ValidateRange(0, [int]::MaxValue)]
		# Offset within the array of bytes to be writing
		[int]$Offset = 0,

		[Parameter(Mandatory = $false)]
        [ValidateRange(0, [int]::MaxValue)]
		# Number of bytes to write
		[int]$Count
	)

    if ($PSBoundParameters.ContainsKey('Offset') -and $Offset -gt 0) {
        if ($PSBoundParameters.ContainsKey('Count')) {
            if (([long]$Offset) + ([long]$Count) -gt ([long]($Bytes.Length))) {
                Write-Error -Message 'Offset + Count is greater than the number of bytes' -Category InvalidOperation -ErrorId 'OutOfRange' `
                        -CategoryReason "Offset $Offset + Count $Count is greater than $($Bytes.Length)";
            } else {
                Write-IntegerToStream -Stream $Stream -Value $Count;
                if ($Count -gt 0) {
                    $Written = $Stream.Write($Bytes, $Offset, $Count);
                    if ($Written -ne $Count) {
                        Write-Error -Message 'Not all bytes were written' -Category InvalidOperation -ErrorId 'BytesNotWritten' `
                                -CategoryReason "[System.IO.Stream].Write(byte[], $Offset, $Count) returned $Written";
                    }
                }
            }
        } else {
            $c = $Bytes.Length - $Offset;
            if ($c -lt 0) {
                Write-Error -Message 'Offset is greater than the number of bytes' -Category InvalidOperation -ErrorId 'OutOfRange' `
                        -CategoryReason "Offset $Offset is greater than $($Bytes.Length)";
            } else {
                Write-IntegerToStream -Stream $Stream -Value $c;
                if ($c -gt 0) {
                    $Written = $Stream.Write($Bytes, $Offset, $c);
                    if ($Written -ne $c) {
                        Write-Error -Message 'Not all bytes were written' -Category InvalidOperation -ErrorId 'BytesNotWritten' `
                                -CategoryReason "[System.IO.Stream].Write(byte[], $Offset, $c) returned $Written";
                    }
                }
            }
        }
    } else {
        if ($PSBoundParameters.ContainsKey($Count)) {
            if ($Count -gt $Bytes.Length) {
                Write-Error -Message 'Count is greater than the number of bytes' -Category InvalidOperation -ErrorId 'OutOfRange' `
                        -CategoryReason "Count $Count is greater than $($Bytes.Length)";
            } else {
                Write-IntegerToStream -Stream $Stream -Value $Count;
                if ($Count -gt 0) {
                    $Written = $Stream.Write($Bytes, 0, $Count);
                    if ($Written -ne $Count) {
                        Write-Error -Message 'Not all bytes were written' -Category InvalidOperation -ErrorId 'BytesNotWritten' `
                                -CategoryReason "[System.IO.Stream].Write(byte[], 0, $Count) returned $Written";
                    }
                }
            }
        } else {
            $c = $Bytes.Length;
            Write-IntegerToStream -Stream $Stream -Value $c;
            if ($c -gt 0) {
                $Written = $Stream.Write($Bytes, 0, $c);
                if ($Written -ne $c) {
                    Write-Error -Message 'Not all bytes were written' -Category InvalidOperation -ErrorId 'BytesNotWritten' `
                            -CategoryReason "[System.IO.Stream].Write(byte[], 0, $c) returned $Written";
                }
            }
        }
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
	
    if ($null -ne $Script:MinBase64BlockSize) { return $Script:MinBase64BlockSize }
    $Regex = [System.Text.RegularExpressions.Regex]::new('\s');
    $MinSize = 0;
    $e = '';
    do {
        $MinSize++;
        $Buffer = New-Object -TypeName 'System.Byte' -ArgumentList $MinSize;
        $e = [System.Convert]::ToBase64String($Buffer, 0, $MinSize, [System.Base64FormattingOptions]::InsertLineBreaks).Trim();
    } while (-not $Regex.IsMatch($e));
    Set-Variable -Name '' -Scope 'Script' -Option Constant -Value $MinSize;
	return $MinSize;
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
        [ValidateRange(0, [int]::MaxValue)]
		# Offset within data buffer, in bytes, to begin encoding.
		[int]$Offset,
		
		[Parameter(Mandatory = $false)]
        [ValidateRange(1, [int]::MaxValue)]
		# Number of bytes to encode
		[int]$Length,
		
		[Parameter(Mandatory = $false)]
		# Whether to insert line breaks
		[switch]$InsertLineBreaks
	)
	
	if ($PSBoundParameters.ContainsKey('Length')) {
        if ($PSBoundParameters.ContainsKey('Offset')) {
            if (([long]$Offset) + ([long]$Length) -gt ([long]($Buffer.Length))) {
                Write-Error -Message 'Offset + Length is greater than the number of bytes' -Category InvalidOperation -ErrorId 'OutOfRange' `
                        -CategoryReason "Offset $Offset + Length $Length is greater than $($Buffer.Length)";
            } else {
                if ($InsertLineBreaks.IsPresent) {
                    [System.Convert]::ToBase64String($Buffer, $Offset, $Length, [System.Base64FormattingOptions]::InsertLineBreaks);
                } else {
                    [System.Convert]::ToBase64String($Buffer, $Offset, $Length, [System.Base64FormattingOptions]::None);
                }
            }
        } else {
            if ($Length -gt $Buffer.Length) {
                Write-Error -Message 'Length is greater than the number of bytes' -Category InvalidOperation -ErrorId 'OutOfRange' `
                        -CategoryReason "Length $Length is greater than $($Buffer.Length)";
            } else {
                if ($InsertLineBreaks.IsPresent) {
                    [System.Convert]::ToBase64String($Buffer, 0, $Length, [System.Base64FormattingOptions]::InsertLineBreaks);
                } else {
                    [System.Convert]::ToBase64String($Buffer, 0, $Length, [System.Base64FormattingOptions]::None);
                }
            }
        }
	} else {
        if ($PSBoundParameters.ContainsKey('Offset')) {
            $Count = $Buffer.Length - $Offset;
            if ($Count -lt 1) {
                Write-Error -Message 'Offset is not less than than the number of bytes' -Category InvalidOperation -ErrorId 'OutOfRange' `
                        -CategoryReason "Offset $Offset is not less than $($Buffer.Length)";
            } else {
                if ($InsertLineBreaks.IsPresent) {
                    [System.Convert]::ToBase64String($Buffer, $Offset, $Count, [System.Base64FormattingOptions]::InsertLineBreaks);
                } else {
                    [System.Convert]::ToBase64String($Buffer, $Offset, $Count, [System.Base64FormattingOptions]::None);
                }
            }
        } else {
            if ($InsertLineBreaks.IsPresent) {
                [System.Convert]::ToBase64String($Buffer, 0, $Buffer.Length, [System.Base64FormattingOptions]::InsertLineBreaks);
            } else {
                [System.Convert]::ToBase64String($Buffer, 0, $Buffer.Length, [System.Base64FormattingOptions]::None);
            }
        }
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
				for ($Node = $Xml.FirstChild; $null -ne $Node; $Node = $Node.NextSibling) {
					if ($Node.NodeType -eq [System.Xml.XmlNodeType]::XmlDeclaration) {
						$XmlDeclaration = $Node;
						break;
					}
				}
				if ($null -eq $XmlDeclaration) {
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
        [ValidateRange(0, [int]::MaxValue)]
		# The index into buffer at which the stream begins.
		[int]$Index = 0,
		
		[Parameter(Position = 2, ParameterSetName = 'Buffer')]
        [ValidateRange(0, [int]::MaxValue)]
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

Function Get-StringComparer {
	<#
		.SYNOPSIS
			Gets a core string comparer object.
 
		.DESCRIPTION
			Gets a core comparer object for comparing string values..
		
		.OUTPUTS
			System.StringComparer. The string comparer object.
		
		.LINK
			https://msdn.microsoft.com/en-us/library/system.stringcomparer.aspx
	#>
    [CmdletBinding()]
    [OutputType([System.StringComparer])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        # The string comparer type
        [System.StringComparison]$Type
    )
    switch ($Type) {
        CurrentCulture {
            [System.StringComparer]::CurrentCulture | Write-Output;
            break;
        }
        InvariantCulture {
            [System.StringComparer]::InvariantCulture | Write-Output;
            break;
        }
        InvariantCultureIgnoreCase {
            [System.StringComparer]::InvariantCultureIgnoreCase | Write-Output;
            break;
        }
        Ordinal {
            [System.StringComparer]::Ordinal | Write-Output;
            break;
        }
        OrdinalIgnoreCase {
            [System.StringComparer]::Ordinal | Write-Output;
            break;
        }
        default {
            [System.StringComparer]::CurrentCultureIgnoreCase | Write-Output;
            break;
        }
    }
}

Function Optimize-WhiteSpace {
    <#
    .SYNOPSIS
        Normalizes white space in strings.
    .DESCRIPTION
        Normalizes consecutive white space characters and white space characters that are not the space character (32) to a single space character.
    #>
    [CmdletBinding(DefaultParameterSetName = "NullToEmpty")]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        [AllowEmptyCollection()]
        [AllowNull()]
        # The string to normalize.
        [string[]]$InputString,

        [Parameter(ParameterSetName = "NullToEmpty")]
        # Convert null input strings to empty strings.
        [switch]$NullToEmpty,

        [Parameter(Mandatory = $true, ParameterSetName = "EmptyToNull")]
        # Convert strings that were normalized to empty strings to null values.
        [switch]$EmptyToNull,

        # Whitespace is not trimmed before normalizing.
        [switch]$DoNotTrim
    )

    Begin {
        if ($null -eq $Script:Optimize_WhiteSpace_Regex) {
            New-Variable -Name 'Optimize_WhiteSpace_Regex' -Option ReadOnly -Scope 'Script' -Value ([System.Text.RegularExpressions.Regex]::new('(?! )\s+| \s+', [System.Text.RegularExpressions.RegexOptions]::Compiled))
        }
        $NullResult = $null;
        if ($NullToEmpty.IsPresent) {
            $NullResult = '';
            if ($DoNotTrim.IsPresent) {
                function OptimizeWhiteSpace ([string]$Value) {
                    if ($Value.Length -eq 0) {
                        $Value | Write-Output;
                    } else {
                        $Script:Optimize_WhiteSpace_Regex.Replace($Value, ' ') | Write-Output;
                    }
                }
            } else {
                function OptimizeWhiteSpace ([string]$Value) {
                    if (($Value = $Value.Trim()).Length -eq 0) {
                        $Value | Write-Output;
                    } else {
                        $Script:Optimize_WhiteSpace_Regex.Replace($Value, ' ') | Write-Output;
                    }
                }
            }
        } else {

            if ($EmptyToNull.IsPresent) {
                if ($DoNotTrim.IsPresent) {
                    function OptimizeWhiteSpace ([string]$Value) {
                        if ($Value.Length -eq 0) {
                            Write-Output -InputObject $null;
                        } else {
                            $Result = $Script:Optimize_WhiteSpace_Regex.Replace($Value, ' ');
                            if ($Result -eq ' ') {
                                Write-Output -InputObject $null;
                            } else {
                                $Result | Write-Output;
                            }
                        }
                    }
                } else {
                    function OptimizeWhiteSpace ([string]$Value) {
                        if (($Value = $Value.Trim()).Length -eq 0) {
                            Write-Output -InputObject $null;
                        } else {
                            $Script:Optimize_WhiteSpace_Regex.Replace($Value, ' ') | Write-Output;
                        }
                    }
                }
            } else {
                if ($DoNotTrim.IsPresent) {
                    function OptimizeWhiteSpace ([string]$Value) {
                        if ($Value.Length -eq 0) {
                            Write-Output -InputObject $Value;
                        } else {
                            $Script:Optimize_WhiteSpace_Regex.Replace($Value, ' ') | Write-Output;
                        }
                    }
                } else {
                    function OptimizeWhiteSpace ([AllowNull()][string]$Value) {
                        if (($Value = $Value.Trim()).Length -eq 0) {
                            Write-Output -InputObject $Value;
                        } else {
                            $Script:Optimize_WhiteSpace_Regex.Replace($Value, ' ') | Write-Output;
                        }
                    }
                }
            }
        }
    }

    Process {
        if ($null -eq $InputString) {
            Write-Output -InputObject $NullResult;
        } else {
            $InputString | ForEach-Object {
                if ($null -eq $_) {
                    Write-Output -InputObject $NullResult;
                } else {
                    OptimizeWhiteSpace $_;
                }
            }
        }
    }
}

Function Remove-ZeroPadding {
    <#
    .SYNOPSIS
        Remove zero-padding from strings.
    .DESCRIPTION
        Removes extra leading zeroes from strings representing numerical values.
    #>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        [string]$InputString,

        [switch]$AllowNegativeSign,

        [switch]$KeepNegativeSign,

        [switch]$AllowPositiveSign,

        [switch]$KeepPositiveSign,

        [ValidateRange(0, [int]::MaxValue)]
        [int]$StartIndex = 0
    )

    Process {
        $LastIndex = $InputString.Length - 1;
        if ($StartIndex -ge $LastIndex) {
            $InputString | Write-Output;
        } else {
            $Index = $StartIndex;
            $First = $Current = $InputString[$StartIndex];
            switch ($Current) {
                '-' {
                    if ($AllowNegativeSign.IsPresent) {
                        $Current = $InputString[++$Index];
                    }
                    break;
                }
                '+' {
                    if ($AllowPositiveSign.IsPresent) {
                        $Current = $InputString[++$Index];
                    }
                    break;
                }
            }
            if ($Current -eq '0') {
                while ($Index -lt $LastIndex) {
                    $Index++;
                    $Current = $InputString[$Index];
                    if ($Current -ne '0') { break }
                }
                if ($Current -eq '0') {
                    $KeepSign = $false;
                    switch ($First) {
                        '-' { $KeepSign = $KeepNegativeSign.IsPresent; break }
                        '+' { $KeepSign = $KeepPositiveSign.IsPresent; break }
                    }
                    if ($KeepSign) {
                        if ($StartIndex -gt 0) {
                            "$($InputString.Substring(0, $StartIndex))$First$($InputString.Substring($Index))";
                        } else {
                            if ($Index -eq 1) {
                                $InputString | Write-Output;
                            } else {
                                "$First$($InputString.Substring($Index))" | Write-Output;
                            }
                        }
                    } else {
                        if ($StartIndex -gt 0) {
                            "$($InputString.Substring(0, $StartIndex))$($InputString.Substring($Index))";
                        } else {
                            $InputString.Substring($Index) | Write-Output;
                        }
                    }
                } else {
                    if ([char]::IsAsciiDigit($Current)) {
                        if ($First -eq '+') {
                            if ($KeepPositiveSign.IsPresent) {
                                if ($StartIndex -gt 0) {
                                    "$($InputString.Substring(0, $StartIndex))+$($InputString.Substring($Index))" | Write-Output;
                                } else {
                                    if ($Index -eq 1) {
                                        $InputString | Write-Output;
                                    } else {
                                        "+$($InputString.Substring($Index))" | Write-Output;
                                    }
                                }
                            } else {
                                if ($StartIndex -gt 0) {
                                    "$($InputString.Substring(0, $StartIndex))$($InputString.Substring($Index))" | Write-Output;
                                } else {
                                    $InputString.Substring($Index) | Write-Output;
                                }
                            }
                        } else {
                            if ($First -eq '-') {
                                if ($StartIndex -gt 0) {
                                    "$($InputString.Substring(0, $StartIndex))-$($InputString.Substring($Index))" | Write-Output;
                                } else {
                                    if ($Index -eq 1) {
                                        $InputString | Write-Output;
                                    } else {
                                        "-$($InputString.Substring($Index))" | Write-Output;
                                    }
                                }
                            } else {
                                if ($StartIndex -gt 0) {
                                    ($InputString.Substring(0, $StartIndex) + $InputString.Substring($Index)) | Write-Output;
                                } else {
                                    $InputString.Substring($Index) | Write-Output;
                                }
                            }
                        }
                    } else {
                        if ($First -eq '+') {
                            if ($KeepPositiveSign.IsPresent) {
                                if ($StartIndex -gt 0) {
                                    "+0$($InputString.Substring(0, $StartIndex))$($InputString.Substring($Index))";
                                } else {
                                    "+0$($InputString.Substring($Index))" | Write-Output;
                                }
                            } else {
                                if ($StartIndex -gt 0) {
                                    "0$($InputString.Substring(0, $StartIndex))$($InputString.Substring($Index))";
                                } else {
                                    "0$($InputString.Substring($Index))" | Write-Output;
                                }
                            }
                        } else {
                            if ($First -eq '-') {
                                if ($KeepNegativeSign.IsPresent) {
                                    if ($StartIndex -gt 0) {
                                        "-0$($InputString.Substring(0, $StartIndex))$($InputString.Substring($Index))";
                                    } else {
                                        "-0$($InputString.Substring($Index))" | Write-Output;
                                    }
                                } else {
                                    if ($StartIndex -gt 0) {
                                        "0$($InputString.Substring(0, $StartIndex))$($InputString.Substring($Index))";
                                    } else {
                                        "0$($InputString.Substring($Index))" | Write-Output;
                                    }
                                }
                            } else {
                                if ($StartIndex -gt 0) {
                                    "0$($InputString.Substring(0, $StartIndex))$($InputString.Substring($Index))";
                                } else {
                                    "0$($InputString.Substring($Index))" | Write-Output;
                                }
                            }
                        }
                    }
                }
            } else {
                if ([char]::IsAsciiDigit($Current)) {
                    if ($First -eq '-') {
                        $InputString | Write-Output;
                    } else {
                        if ($First -eq '+') {
                            # Index = $StartIndex + 1
                            if ($KeepPositiveSign.IsPresent) {
                                $InputString | Write-Output;
                            } else {
                                if ($StartIndex -gt 0) {
                                    "$($InputString.Substring(0, $StartIndex))$($InputString.Substring($Index))" | Write-Output;
                                } else {
                                    $InputString.Substring($Index) | Write-Output;
                                }
                            }
                        } else {
                            $InputString | Write-Output;
                        }
                    }
                } else {
                    $InputString | Write-Output;
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

Function Expand-GZip {
    <#
    .SYNOPSIS
        Decompresses GZip files.
    .DESCRIPTION
        Decompresses the specified GZip-compressed file(s).
    #>
    [CmdletBinding(DefaultParameterSetName = 'Items')]
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'Items')]
        [SupportsWildcards()]
        # Path to one or more locations of files to decrypt. The default behavior is to expand all .gz, and .tgz files in the current directory.
        [string[]]$Path,
    
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'LiteralItems')]
        [Alias("PSPath", "FullName")]
        # Literal path to one or more locations of files to decrypt.
        [string[]]$LiteralPath,
    
        # Literal path of output directory. If not specified, it will use the current subdirectory as the output path.
        [ValidateScript({ $_ | Test-Path -PathType Container })]
        [string]$OutputDirectory,
    
        [ValidateScript({ $_ | Test-ValidFileSystemFileName })]
        # Default output file extension if the input file doesn't have a compound extension.
        # If this is not specified, files without a compound extension will be given the '.data' extension.
        # Files with the extension '.tgz' are treated as though they have a '.tar.gz' compound extension.
        [string]$DefaultExt = '.data',

        # Force overwrite of existing files.
        [switch]$Force
    )

    Begin {
        $FileSystemProviderName = [Microsoft.PowerShell.Commands.FileSystemProvider]::ProviderName;
        $InputFileItems = @();
        $Ext = $DefaultExt;
        if ($PSBoundParameters.ContainsKey('DefaultExt') -and -not $Ext.StartsWith('.')) { $Ext = ".$Ext" }
        $OutputPathInfo = $null;
        $OutputDirectoryInfo = $null;
        if ($PSBoundParameters.ContainsKey('OutputDirectory')) {
            $OutputPathInfo = Resolve-Path -LiteralPath $OutputDirectory;
        } else {
            $OutputPathInfo = Resolve-Path -LiteralPath (Get-Location).Path;
        }
        if ($null -eq $OutputPathInfo) { break }
        if ($OutputPathInfo.Provider.Name -eq $FileSystemProviderName) {
            $OutputDirectoryInfo = [System.IO.DirectoryInfo]::new($OutputPathInfo.Path);
            if ($null -ne $OutputDirectoryInfo -and -not $OutputDirectoryInfo.Exists) { $OutputDirectoryInfo = $null }
        }
    }

    Process {
        if ($null -eq $OutputContainer) { continue }
        if ($PSCmdlet.ParameterSetName -eq 'LiteralItems') {
            $InputFileItems += @((Resolve-Path -LiteralPath $LiteralPath) | ForEach-Object { Get-Item -LiteralPath $_.Path });
        } else {
            $InputFileItems += @((Resolve-Path -Path $Path) | ForEach-Object { Get-Item -LiteralPath $_.Path });
        }
    }

    End {
        $Buffer = New-Object 'System.Byte[]' -ArgumentList 32768;
        $InputFileItems = @($InputFileItems | Where-Object {
            if ($_.PSIsContainer) { 
                if ($_.PSProvider.Name -eq $FileSystemProviderName -and $_.FullName -is [string] -and $_.FullName.Length -gt 0) {
                    Write-Error -Message "`"$($Item.FullName)`" does not refer to a file." -Category InvalidArgument -ErrorId 'NotAfile' -TargetObject $Item;
                } else {
                    Write-Error -Message "`"$($Item.PSPath)`" does not refer to a file." -Category InvalidArgument -ErrorId 'NotAfile' -TargetObject $Item;
                }
                return $false;
            }
            return $true;
        });
        $TotalBytes = 0.0;
        foreach ($Item in $InputFileItems) {
            if ($Item.Length -is [long] -and $Item.Length -gt 0) {
                $TotalBytes += ([double]($Item.Length));
            }
        }

        $BytesDecompressed = 0.0;
        $Buffer = New-Object 'System.Byte[]' -ArgumentList 32768;
        $PercentComplete = -1;
        $ActivityName = 'Decompressing GZIP Files';
        if ($ResolvedItems.Count -eq 1) { $ActivityName = 'Decompressing GZIP File' }
        $FileNumber = 0;
        foreach ($OriginalSourceFile in $InputFileItems) {
            $PercentComplete = 0;
            if ($TotalBytes -eq 0.0) {
                $PercentComplete = $FileNumber * 100 / $InputFileItems.Count;
            } else {
                $PercentComplete = [int](($BytesDecompressed * 100.0) / $TotalBytes);
            }
            $FileNumber++;
            $StatusMessage = "$FileNumber of $($InputFileItems.Count) files";
            $CurrentOperation = $null;
            if ($OriginalSourceFile.PSProvider.Name -eq $FileSystemProviderName -and $OriginalSourceFile.FullName -is [string] -and $OriginalSourceFile.FullName.Length -gt 0) {
                $CurrentOperation = $OriginalSourceFile.FullName;
            } else {
                $CurrentOperation = $OriginalSourceFile.PSPath;
            }
            Write-Progress -Activity $ActivityName -Status $StatusMessage -CurrentOperation $CurrentOperation -PercentComplete $PercentComplete;
            $NextBytesDecompressed = $BytesDecompressed;
            if ($OriginalSourceFile.Length -is [long] -and $OriginalSourceFile.Length -gt 0) { $NextBytesDecompressed += ([double]($OriginalSourceFile.Length)) }

            $FinalOutputPath = $null;
            $InputExtension = $null;
            if ($OriginalSourceFile.BaseName -is [string] -and $OriginalSourceFile.BaseName.Length -gt 0 -and $OriginalSourceFile.Extension -is [string]) {
                $FinalOutputPath = $OriginalSourceFile.BaseName;
                $InputExtension = $OriginalSourceFile.Extension;
            } else {
                $FinalOutputPath = $OriginalSourceFile.PSPath | Split-Path -LeafBase;
                $InputExtension = $OriginalSourceFile.PSPath | Split-Path -Extension;
            }
            if ($InputExtension -ieq '.tgz') {
                $FinalOutputPath = "$FinalOutputPath.tar";
            } else {
                if ([string]::IsNullOrEmpty(($FinalOutputPath | Split-Path -Extension))) { $FinalOutputPath += $Ext }
            }
            $InFileInfo = $null;
            if ($OriginalSourceFile -is [System.IO.FileInfo]) {
                $InFileInfo = $OriginalSourceFile;
            } else {
                $InFileInfo = [System.IO.FileInfo]::new([System.IO.Path]::GetTempFileName());
                $InFileInfo.Delete();
                Copy-Item -LiteralPath $OriginalSourceFile.PSPath -Destination $InFileInfo.FullName -Force;
                $InFileInfo.Refresh();
                if (-not $InFileInfo.Exists) {
                    $BytesDecompressed = $NextBytesDecompressed;
                    continue;
                }
            }
            try {
                $FinalOutputPath = $OutputPathInfo.Path | Join-Path -ChildPath $FinalOutputPath;
                $IntermediateOutputPath = $FinalOutputPath;
                if ($null -eq $OutputDirectoryInfo) {
                    $IntermediateOutputPath = [System.IO.Path]::GetTempFileName();
                    [System.IO.File]::Delete($IntermediateOutputPath);
                }
                try {
                    $InputStream = [System.IO.FileStream]::new($InFileInfo.FullName, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::Read);
                    try {
                        $OutputStream = $null;
                        if ($null -ne ($OutputStream = [System.IO.FileStream]::new($IntermediateOutputPath, [System.IO.FileMode]::Create, [System.IO.FileAccess]::Write, [System.IO.FileShare]::None))) {
                            try {
                                $GzipStream = [System.IO.Compression.GzipStream]::new($InputStream, [System.IO.Compression.CompressionMode]::Decompress);
                                try {
                                    $Count = $GzipStream.Read($Buffer, 0, $Buffer.Length);
                                    if ($TotalBytes -gt 0.0) {
                                        $LastPc = $PercentComplete;
                                        while ($Count -gt 0) {
                                            $OutputStream.Write($Buffer, 0, $Count);
                                            $Position = $BytesDecompressed + ([double]$InputStream.Position);
                                            if ($Position -gt $NextBytesDecompressed) { $Position = $NextBytesDecompressed }
                                            [int]$p = ($BytesDecompressed * 100.0) / $TotalBytes;
                                            if ($p -ne $LastPc) {
                                                $LastPc = $p;
                                                Write-Progress -Activity $ActivityName -Status $StatusMessage -CurrentOperation $CurrentOperation -PercentComplete $p;
                                            }
                                            $Count = $GzipStream.Read($Buffer, 0, $Buffer.Length);
                                        }
                                    } else {
                                        while ($Count -gt 0) {
                                            $OutputStream.Write($Buffer, 0, $Count);
                                            $Count = $GzipStream.Read($Buffer, 0, $Buffer.Length);
                                        }
                                    }
                                }
                                finally { $GzipStream.Close() }
                                $OutputStream.Flush();
                            }
                            finally { $OutputStream.Close() }
                            $OutFileInfo = [System.IO.FileInfo]::new($IntermediateOutputPath);
                            if ($null -ne $OutFileInfo -and $OutFileInfo.Exists) {
                                if ($IntermediateOutputPath -ne $FinalOutputPath) {
                                    try {
                                        Copy-Item -LiteralPath $OutFileInfo.FullName -Destination $FinalOutputPath -Force;
                                        Write-Information -MessageData "Decompressed $($OutFileInfo.Length) bytes from: $($OriginalSourceFile.FullName)`n    to: $FinalOutputPath" -InformationAction Continue;
                                    } catch {
                                        Write-Error -ErrorRecord $_ -CategoryReason "Exception copying $($OutFileInfo.FullName) to $FinalOutputPath";
                                    }
                                } else {
                                    Write-Information -MessageData "Decompressed $($OutFileInfo.Length) bytes from: $($OriginalSourceFile.FullName)`n    to: $FinalOutputPath" -InformationAction Continue;
                                }
                            }
                        }
                    } finally {
                        $InputStream.Close();
                    }
                } finally {
                    if ($IntermediateOutputPath -ne $FinalOutputPath) {
                        [System.IO.File]::Delete($IntermediateOutputPath);
                    }
                }
            } finally {
                if ($OriginalSourceFile -isnot [System.IO.FileInfo]) { $InFileInfo.Delete() }
            }
            $BytesDecompressed = $NextBytesDecompressed;
        }
    }
}

Function Use-TempFolder {
    <#
    .SYNOPSIS
        Creates a temporary folder that is automatically deleted.
    .DESCRIPTION
        Creates a temporary folder and runs scripts, deleting the temp folder when complete.
    #>
    [CmdletBinding(DefaultParameterSetName = 'TempPathVar')]
    Param (
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        # The script block(s) to run.
        [ScriptBlock[]]$Process,

        # The name of the variable containing the temp folder path (without the leading '$'). The default is _.
        [Parameter(ParameterSetName = 'TempPathVar')]
        [ValidatePattern('^[_a-z]\w*$')]
        [string]$TempPathVar = '_',

        [Parameter(ParameterSetName = 'TempPathVar')]
        [PSVariable[]]$ContextVariables,

        # The temp folder path variable will contain the DirectoryInfo object. If this is not specified, it will contain the full path string.
        [switch]$TempPathItem,

        [Parameter(Mandatory = $true, ParameterSetName = 'PassAsArg')]
        # Passes the folder path as the first argument.
        [switch]$PassAsArg
    )

    Begin {
        $TempPath = [System.IO.Path]::GetTempPath();
        $FolderName = [Guid]::NewGuid().ToString('n');
        $FullPath = $TempPath | Join-Path -ChildPath $FolderName;
        while ($FullPath | Test-Path) {
            $FolderName = [Guid]::NewGuid().ToString('n');
            $FullPath = $TempPath | Join-Path -ChildPath $FolderName;
        }
        $DirectoryInfo = New-Item -Path $TempPath -Name $FolderName -ItemType Directory -Force;
        $TempValue = $null;
        if ($TempPathItem.IsPresent) {
            $TempValue = $DirectoryInfo;
        } else {
            $TempValue = $DirectoryInfo.FullName;
        }
        [System.Collections.Generic.List[PSVariable]]$Variables = $null;
        if (-not $PassAsArg.IsPresent) {
            $Variables = [System.Collections.Generic.List[PSVariable]]::new();
            $Variables.Add([PSVariable]::new($TempPathVar, $TempValue, [System.Management.Automation.ScopedItemOptions]::ReadOnly));
            if ($PSBoundParameters.ContainsKey('ContextVariables')) {
                foreach ($v in $ContextVariables) {
                    if ($v.Name -ine $TempPathVar) { $Variables.Add($v) }
                }
            }
        }
    }

    Process {
        if ($PassAsArg.IsPresent) {
            foreach ($sb in $Process) {
                $sb.Invoke($TempValue);
            }
        } else {
            foreach ($sb in $Process) {
                $sb.InvokeWithContext($null, $Variables);
            }
        }
    }

    End {
        Remove-Item -LiteralPath $DirectoryInfo.FullName -Recurse -Force;
    }
}