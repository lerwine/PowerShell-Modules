Function ConvertTo-XmlString  {
	[CmdletBinding(DefaultParameterSetName = 'Value')]
    [OutputType([string])]
	Param(
		[Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 0, ParameterSetName = 'Value')]
		[AllowEmptyString()]
		[object]$Value,
		
		[Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 0, ParameterSetName = 'Guid')]
		[System.Guid[]]$Guid,
		
		[Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 0, ParameterSetName = 'DateTimeFormat')]
		[Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 0, ParameterSetName = 'DateTimeOption')]
		[System.DateTime[]]$DateTime,
		
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'DateTimeOffset')]
		[System.DateTimeOffset[]]$DateTimeOffset,
		
		[Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Base64Encode')]
		[AllowEmptyCollection()]
		[byte[]]$Bytes,
		
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'DateTimeFormat')]
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'DateTimeOffset')]
		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Guid')]
		[string]$Format,

		[Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'DateTimeOption')]
		[System.Xml.XmlDateTimeSerializationMode]$DateTimeOption,

		[Parameter(ParameterSetName = 'Base64Encode')]
		[switch]$Base64,

		[Parameter(Mandatory = $true, ParameterSetName = 'Hex')]
		[switch]$Hex
	)
	<#
		.SYNOPSIS
			Converts values to XML text.
 
		.DESCRIPTION
			Converts values to xml-formatted text.
        
		.OUTPUTS
			System.String. The XML value as a string.
	#>

	Process {
		switch ($PSCmdlet.ParameterSetName) {
			'DateTimeFormat' { (($DateTime | ForEach-Object { [System.Xml.XmlConvert]::ToString($_, $Format) }) -join ' ') | Write-Output; break; }
			'DateTimeOption' { (($DateTime | ForEach-Object { [System.Xml.XmlConvert]::ToString($DateTime, $DateTimeOption) }) -join ' ') | Write-Output; break; }
			'DateTimeOffset' { (($DateTimeOffset | ForEach-Object { [System.Xml.XmlConvert]::ToString($DateTimeOffset, $Format) }) -join ' ') | Write-Output; break; }
			'Guid' {
				if ($PSBoundParameters.ContainsKey('Format')) {
					(($Guid | ForEach-Object { $Guid.ToString($Format) }) -join ' ') | Write-Output;
				} else {
					(($Guid | ForEach-Object { [System.Xml.XmlConvert]::ToString($Guid, $Format) }) -join ' ') | Write-Output;
				}
				
				break;
			}
			'Base64Encode' {
				if ($Bytes.Length -eq 0) {
					'' | Write-Output;
				} else {
					[System.Convert]::ToBase64String($Bytes, [System.Base64FormattingOptions]::InsertLineBreaks);
				}
				break;
			}
			'Hex' {
				if ($Bytes.Length -eq 0) {
					'' | Write-Output;
				} else {
					-join ($Bytes | ForEach-Object { ([int]$_).ToString('X2') });
				}
				break;
			}
			default {
				switch ($Value) {
					{ $_ -is [string] } { $_ | Write-Output; break; }
					{ $_ -is [string[]] } { ($_ -join ' ') | Write-Output; break; }
					{ $_ -is [bool] } { [System.Xml.XmlConvert]::ToString($_) | Write-Output; break; }
					{ $_ -is [bool[]] } { (($_ | ForEach-Object { [System.Xml.XmlConvert]::ToString($_) }) -join ' ') | Write-Output; break; }
					{ $_ -is [byte] } { [System.Xml.XmlConvert]::ToString($_) | Write-Output; break; }
					{ $_ -is [byte[]] } {
						if ($_.Length -eq 0) {
							'' | Write-Output;
						} else {
							[System.Convert]::ToBase64CharArray($_, [System.Base64FormattingOptions]::InsertLineBreaks) | Write-Output;
						}
					}
					{ $_ -is [char] } { [System.Xml.XmlConvert]::ToString($_) | Write-Output; break; }
					{ $_ -is [char[]] } { (($_ | ForEach-Object { [System.Xml.XmlConvert]::ToString($_) }) -join ' ') | Write-Output; break; }
					{ $_ -is [decimal] } { [System.Xml.XmlConvert]::ToString($_) | Write-Output; break; }
					{ $_ -is [decimal[]] } { (($_ | ForEach-Object { [System.Xml.XmlConvert]::ToString($_) }) -join ' ') | Write-Output; break; }
					{ $_ -is [double] } { [System.Xml.XmlConvert]::ToString($_) | Write-Output; break; }
					{ $_ -is [double[]] } { (($_ | ForEach-Object { [System.Xml.XmlConvert]::ToString($_) }) -join ' ') | Write-Output; break; }
					{ $_ -is [float] } { [System.Xml.XmlConvert]::ToString($_) | Write-Output; break; }
					{ $_ -is [float[]] } { (($_ | ForEach-Object { [System.Xml.XmlConvert]::ToString($_) }) -join ' ') | Write-Output; break; }
					{ $_ -is [int] } { [System.Xml.XmlConvert]::ToString($_) | Write-Output; break; }
					{ $_ -is [int[]] } { (($_ | ForEach-Object { [System.Xml.XmlConvert]::ToString($_) }) -join ' ') | Write-Output; break; }
					{ $_ -is [long] } { [System.Xml.XmlConvert]::ToString($_) | Write-Output; break; }
					{ $_ -is [long[]] } { (($_ | ForEach-Object { [System.Xml.XmlConvert]::ToString($_) }) -join ' ') | Write-Output; break; }
					{ $_ -is [sbyte] } { [System.Xml.XmlConvert]::ToString($_) | Write-Output; break; }
					{ $_ -is [sbyte[]] } { (($_ | ForEach-Object { [System.Xml.XmlConvert]::ToString($_) }) -join ' ') | Write-Output; break; }
					{ $_ -is [short] } { [System.Xml.XmlConvert]::ToString($_) | Write-Output; break; }
					{ $_ -is [short[]] } { (($_ | ForEach-Object { [System.Xml.XmlConvert]::ToString($_) }) -join ' ') | Write-Output; break; }
					{ $_ -is [System.DateTime] } { [System.Xml.XmlConvert]::ToString($_, 'yyyy-MM-ddTHH:mm:sszzzzzz') | Write-Output; break; }
					{ $_ -is [System.DateTime[]] } { (($_ | ForEach-Object { [System.Xml.XmlConvert]::ToString($_, 'yyyy-MM-ddTHH:mm:sszzzzzz') }) -join ' ') | Write-Output; break; }
					{ $_ -is [System.DateTimeOffset] } { [System.Xml.XmlConvert]::ToString($_, 'yyyy-MM-ddTHH:mm:sszzzzzz') | Write-Output }
					{ $_ -is [System.DateTimeOffset[]] } { (($_ | ForEach-Object { [System.Xml.XmlConvert]::ToString($_, 'yyyy-MM-ddTHH:mm:sszzzzzz') }) -join ' ') | Write-Output; break; }
					{ $_ -is [System.Guid] } { [System.Xml.XmlConvert]::ToString($_) | Write-Output; break; }
					{ $_ -is [System.Guid[]] } { (($_ | ForEach-Object { [System.Xml.XmlConvert]::ToString($_) }) -join ' ') | Write-Output; break; }
					{ $_ -is [System.TimeSpan] } { [System.Xml.XmlConvert]::ToString($_) | Write-Output; break; }
					{ $_ -is [System.TimeSpan[]] } { (($_ | ForEach-Object { [System.Xml.XmlConvert]::ToString($_) }) -join ' ') | Write-Output; break; }
					{ $_ -is [System.UInt32] } { [System.Xml.XmlConvert]::ToString($_) | Write-Output; break; }
					{ $_ -is [System.UInt32[]] } { (($_ | ForEach-Object { [System.Xml.XmlConvert]::ToString($_) }) -join ' ') | Write-Output; break; }
					{ $_ -is [System.UInt64] } { [System.Xml.XmlConvert]::ToString($_) | Write-Output; break; }
					{ $_ -is [System.UInt64[]] } { (($_ | ForEach-Object { [System.Xml.XmlConvert]::ToString($_) }) -join ' ') | Write-Output; break; }
					{ $_ -is [System.UInt16] } { [System.Xml.XmlConvert]::ToString($_) | Write-Output; break; }
					{ $_ -is [System.UInt16[]] } { (($_ | ForEach-Object { [System.Xml.XmlConvert]::ToString($_) }) -join ' ') | Write-Output; break; }
					default { (($_ | Out-String) -replace '(\r\n?|\n)$') | Write-Output; break; }
				}
			}
		}
	}
}

Function ConvertFrom-XmlString  {
	[CmdletBinding(DefaultParameterSetName = 'DateTime')]
	[OutputType([bool], ParameterSetName = 'Boolean')]
	[OutputType([bool[]], ParameterSetName = 'BooleanArray')]
	[OutputType([byte], ParameterSetName = 'Byte')]
	[OutputType([byte[]], ParameterSetName = 'ByteArray')]
	[OutputType([char], ParameterSetName = 'Char')]
	[OutputType([char[]], ParameterSetName = 'CharArray')]
	[OutputType([decimal], ParameterSetName = 'Decimal')]
	[OutputType([decimal[]], ParameterSetName = 'DecimalArray')]
	[OutputType([double], ParameterSetName = 'Double')]
	[OutputType([double[]], ParameterSetName = 'DoubleArray')]
	[OutputType([float], ParameterSetName = 'Single')]
	[OutputType([float[]], ParameterSetName = 'SingleArray')]
	[OutputType([int], ParameterSetName = 'Int32')]
	[OutputType([int[]], ParameterSetName = 'Int32Array')]
	[OutputType([long], ParameterSetName = 'Int64')]
	[OutputType([long[]], ParameterSetName = 'Int64Array')]
	[OutputType([sbyte], ParameterSetName = 'SByte')]
	[OutputType([sbyte[]], ParameterSetName = 'SByteArray')]
	[OutputType([System.Int16], ParameterSetName = 'Int16')]
	[OutputType([System.Int16[]], ParameterSetName = 'Int16Array')]
	[OutputType([System.DateTime], ParameterSetName = 'DateTime')]
	[OutputType([System.DateTime[]], ParameterSetName = 'DateTimeArray')]
	[OutputType([System.DateTime], ParameterSetName = 'DateTimeOption')]
	[OutputType([System.DateTime[]], ParameterSetName = 'DateTimeOptionArray')]
	[OutputType([System.DateTimeOffset], ParameterSetName = 'DateTimeOffset')]
	[OutputType([System.DateTimeOffset[]], ParameterSetName = 'DateTimeOffsetArray')]
	[OutputType([System.Guid], ParameterSetName = 'Guid')]
	[OutputType([System.Guid[]], ParameterSetName = 'GuidArray')]
	[OutputType([System.TimeSpan], ParameterSetName = 'TimeSpan')]
	[OutputType([System.TimeSpan[]], ParameterSetName = 'TimeSpanArray')]
	[OutputType([System.UInt32], ParameterSetName = 'UInt32')]
	[OutputType([System.UInt32[]], ParameterSetName = 'UInt32Array')]
	[OutputType([System.UInt64], ParameterSetName = 'UInt64')]
	[OutputType([System.UInt64[]], ParameterSetName = 'UInt64Array')]
	[OutputType([System.UInt16], ParameterSetName = 'UInt16')]
	[OutputType([System.UInt16[]], ParameterSetName = 'UInt16Array')]
	Param(
		[Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 0, ParameterSetName = 'Value')]
		[AllowEmptyString()]
		[string]$Value,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Boolean')]
		[switch]$Boolean,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'BooleanArray')]
		[switch[]]$BooleanArray,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Byte')]
		[switch]$Byte,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ByteArray')]
		[switch[]]$ByteArray,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Char')]
		[switch]$Char,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'CharArray')]
		[switch[]]$CharArray,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Decimal')]
		[switch]$Decimal,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'DecimalArray')]
		[switch[]]$DecimalArray,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Double')]
		[switch]$Double,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'DoubleArray')]
		[switch[]]$DoubleArray,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Single')]
		[switch]$Single,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'SingleArray')]
		[switch[]]$SingleArray,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Int32')]
		[switch]$Int32,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Int32Array')]
		[switch[]]$Int32Array,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Int64')]
		[switch]$Int64,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Int64Array')]
		[switch[]]$Int64Array,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'SByte')]
		[switch]$SByte,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'SByteArray')]
		[switch[]]$SByteArray,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Int16')]
		[switch]$Int16,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Int16Array')]
		[switch[]]$Int16Array,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'DateTime')]
		[Parameter(Mandatory = $true, ParameterSetName = 'DateTimeOption')]
		[switch]$DateTime,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'DateTimeArray')]
		[Parameter(Mandatory = $true, ParameterSetName = 'DateTimeOptionArray')]
		[switch[]]$DateTimeArray,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'DateTimeOffset')]
		[switch]$DateTimeOffset,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'DateTimeOffsetArray')]
		[switch[]]$DateTimeOffsetArray,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Guid')]
		[switch]$Guid,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'GuidArray')]
		[switch[]]$GuidArray,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'TimeSpan')]
		[switch]$TimeSpan,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'TimeSpanArray')]
		[switch[]]$TimeSpanArray,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'UInt32')]
		[switch]$UInt32,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'UInt32Array')]
		[switch[]]$UInt32Array,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'UInt64')]
		[switch]$UInt64,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'UInt64Array')]
		[switch[]]$UInt64Array,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'UInt16')]
		[switch]$UInt16,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'UInt16Array')]
		[switch[]]$UInt16Array,
		
		[Parameter(Position = 1, ParameterSetName = 'DateTime')]
		[Parameter(Position = 1, ParameterSetName = 'DateTimeOffset')]
		[Parameter(Position = 1, ParameterSetName = 'DateTimeArray')]
		[Parameter(Position = 1, ParameterSetName = 'DateTimeOffsetArray')]
		[string[]]$Format,
		
		[Parameter(Position = 1, ParameterSetName = 'DateTimeOption')]
		[Parameter(Position = 1, ParameterSetName = 'DateTimeOptionArray')]
		[System.Xml.XmlDateTimeSerializationMode]$DateTimeOption,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Base64Encode')]
		[switch]$Base64,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Guid')]
		[Parameter(Mandatory = $true, ParameterSetName = 'GuidArray')]
		[switch]$Clr,

		[Parameter(Mandatory = $true, ParameterSetName = 'Hex')]
		[switch]$Hex
	)
	<#
		.SYNOPSIS
			Converts XML string to a value.
 
		.DESCRIPTION
			Converts XML-formatted text into a value.
	#>

	Process {
		switch ($PSCmdlet.ParameterSetName) {
			'Boolean' { [System.Xml.XmlConvert]::ToBoolean($Value) | Write-Output; break; }
			'BooleanArray' {
				foreach ($s in ($Value -split '\s+')) {
					if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToBoolean($s) | Write-Output }
				}
				break;
			}
			'Byte' { [System.Xml.XmlConvert]::ToByte($Value) | Write-Output; break; }
			'ByteArray' {
				foreach ($s in ($Value -split '\s+')) {
					if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToByte($s) | Write-Output }
				}
				break;
			}
			'Base64Encode' { [System.Convert]::FromBase64String($Value.Trim()); break; }
			'Nex' {
				$str = ($Value -replace '\s+', '').ToUpper();
				for ($s = 0; $s -lt $str.Length; $s += 2) {
					$c = $Single.SubString($s, 1);
					switch ($c) {
						'A' { $b = 160; break; }
						'B' { $b = 176; break; }
						'C' { $b = 192; break; }
						'D' { $b = 208; break; }
						'E' { $b = 224; break; }
						'F' { $b = 240; break; }
						default { $b = [System.Int32]::Parse($c) * 16; break; }
					}
					$c = $Single.SubString($s + 1, 1);
					switch ($c) {
						'A' { ([byte]($b + 10)) | Write-Output; break; }
						'B' { ([byte]($b + 11)) | Write-Output; break; }
						'C' { ([byte]($b + 12)) | Write-Output; break; }
						'D' { ([byte]($b + 13)) | Write-Output; break; }
						'E' { ([byte]($b + 14)) | Write-Output; break; }
						'F' { ([byte]($b + 15)) | Write-Output; break; }
						default { ([byte]($b + [System.Int32]::Parse($c))) | Write-Output; break; }
					}
				}
				break;
			}
			'Char' { [System.Xml.XmlConvert]::ToChar($Value) | Write-Output; break; }
			'CharArray' {
				foreach ($s in ($Value -split '\s+')) {
					if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToChar($s) | Write-Output }
				}
				break;
			}
			'Decimal' { [System.Xml.XmlConvert]::ToDecimal($Value) | Write-Output; break; }
			'DecimalArray' {
				foreach ($s in ($Value -split '\s+')) {
					if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToDecimal($s) | Write-Output }
				}
				break;
			}
			'Double' { [System.Xml.XmlConvert]::ToDouble($Value) | Write-Output; break; }
			'DoubleArray' {
				foreach ($s in ($Value -split '\s+')) {
					if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToDouble($s) | Write-Output }
				}
				break;
			}
			'Guid' { if ($Clr) { [System.Guid]::Parse($Value) | Write-Output } else { [System.Xml.XmlConvert]::ToGuid($Value) | Write-Output }; break; }
			'GuidArray' {
				if ($Clr) {
					foreach ($s in ($Value -split '\s+')) {
						if ($s.Length -gt 0) { [System.Guid]::Parse($s) | Write-Output }
					}
				} else {
					foreach ($s in ($Value -split '\s+')) {
						if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToGuid($s) | Write-Output }
					}
				}
				break;
			}
			'Int16' { [System.Xml.XmlConvert]::ToInt16($Value) | Write-Output; break; }
			'Int16Array' {
				foreach ($s in ($Value -split '\s+')) {
					if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToInt16($s) | Write-Output }
				}
				break;
			}
			'Int32' { [System.Xml.XmlConvert]::ToInt32($Value) | Write-Output; break; }
			'Int64' { [System.Xml.XmlConvert]::ToInt64($Value) | Write-Output; break; }
			'Int64Array' {
				foreach ($s in ($Value -split '\s+')) {
					if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToInt64($s) | Write-Output }
				}
				break;
			}
			'SByte' { [System.Xml.XmlConvert]::ToSByte($Value) | Write-Output; break; }
			'SByteArray' {
				foreach ($s in ($Value -split '\s+')) {
					if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToSByte($s) | Write-Output }
				}
				break;
			}
			'Single' { [System.Xml.XmlConvert]::ToSingle($Value) | Write-Output; break; }
			'SingleArray' {
				foreach ($s in ($Value -split '\s+')) {
					if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToSingle($s) | Write-Output }
				}
				break;
			}
			'TimeSpan' { [System.Xml.XmlConvert]::ToTimeSpan($Value) | Write-Output; break; }
			'TimeSpanArray' {
				foreach ($s in ($Value -split '\s+')) {
					if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToTimeSpan($s) | Write-Output }
				}
				break;
			}
			'UInt16' { [System.Xml.XmlConvert]::ToUInt16($Value) | Write-Output; break; }
			'UInt16Array' {
				foreach ($s in ($Value -split '\s+')) {
					if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToUInt16($s) | Write-Output }
				}
				break;
			}
			'UInt32' { [System.Xml.XmlConvert]::ToUInt32($Value) | Write-Output; break; }
			'UInt32Array' {
				foreach ($s in ($Value -split '\s+')) {
					if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToUInt32($s) | Write-Output }
				}
				break;
			}
			'UInt64' { [System.Xml.XmlConvert]::ToUInt64($Value) | Write-Output; break; }
			'UInt64Array' {
				foreach ($s in ($Value -split '\s+')) {
					if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToUInt64($s) | Write-Output }
				}
				break;
			}
			'DateTimeOption' { [System.Xml.XmlConvert]::ToDateTime($Value, $dateTimeOption) | Write-Output; break; }
			'DateTimeOptionArray' {
				foreach ($s in ($Value -split '\s+')) {
					if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToDateTime($s, $dateTimeOption) | Write-Output }
				}
				break;
			}
			'DateTime' {
				if ($PSBoundParameters.ContainsKey('Format')) {
					[System.Xml.XmlConvert]::ToDateTime($Value, $Format) | Write-Output;
				} else {
					[System.Xml.XmlConvert]::ToDateTime($Value) | Write-Output;
				}
				break;
			}
			'DateTimeArray' {
				if ($PSBoundParameters.ContainsKey('Format')) {
					foreach ($s in ($Value -split '\s+')) {
						if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToDateTime($s, $Format) | Write-Output }
					}
				} else {
					foreach ($s in ($Value -split '\s+')) {
						if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToDateTime($s) | Write-Output }
					}
				}
				break;
			}
			'DateTimeOffset' {
				if ($PSBoundParameters.ContainsKey('Format')) {
					[System.Xml.XmlConvert]::ToDateTimeOffset($Value, $Format) | Write-Output;
				} else {
					[System.Xml.XmlConvert]::ToDateTimeOffset($Value) | Write-Output;
				}
				break;
			}
			'DateTimeOffsetArray' {
				if ($PSBoundParameters.ContainsKey('Format')) {
					foreach ($s in ($Value -split '\s+')) {
						if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToDateTimeOffset($s, $Format) | Write-Output }
					}
				} else {
					foreach ($s in ($Value -split '\s+')) {
						if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToDateTimeOffset($s) | Write-Output }
					}
				}
				break;
			}
			'default' {
				foreach ($s in ($Value -split '\s+')) {
					if ($s.Length -gt 0) { [System.Xml.XmlConvert]::ToInt32($s) | Write-Output }
				}
				break;
			}
		}
	}
}
