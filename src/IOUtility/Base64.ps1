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
