if ($null -eq $Script:Int16ByteLength) {
    New-Variable -Name 'Int16ByteLength' -Scope 'Script' -Option Constant -Value [System.BitConverter]::GetBytes([Int16]0).Length;
    New-Variable -Name 'UInt16ByteLength' -Scope 'Script' -Option Constant -Value [System.BitConverter]::GetBytes([UInt16]0).Length;
    New-Variable -Name 'Int32ByteLength' -Scope 'Script' -Option Constant -Value [System.BitConverter]::GetBytes([int]0).Length;
    New-Variable -Name 'UInt32ByteLength' -Scope 'Script' -Option Constant -Value [System.BitConverter]::GetBytes([UInt32]0).Length;
    New-Variable -Name 'Int64ByteLength' -Scope 'Script' -Option Constant -Value [System.BitConverter]::GetBytes([Int64]0).Length;
    New-Variable -Name 'UInt64ByteLength' -Scope 'Script' -Option Constant -Value [System.BitConverter]::GetBytes([UInt64]0).Length;
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
