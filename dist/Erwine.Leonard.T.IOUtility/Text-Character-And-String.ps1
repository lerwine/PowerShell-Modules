Enum CharacterClass : ulong {
    # [char]::IsAsciiHexDigitUpper($_) -and -not [char]::IsAsciiDigit($_)
    HexLetterUpper =               0x0000000000001;

    # [char]::IsAsciiHexDigitLower($_) -and -not [char]::IsAsciiDigit($_)
    HexLetterLower =               0x0000000000002;

    # [char]::IsAsciiLetter($_)
    AsciiLetter =                  0x0000000000003;

    # [char]::IsAsciiHexDigit($_) -and -not [char]::IsAsciiDigit($_)
    HexLetter =                    0x0000000000003;

    # [char]::IsAsciiLetterUpper($_) -and -not [char]::IsAsciiHexDigitUpper($_)
    NonHexAsciiLetterUpper =       0x0000000000004;

    # [char]::IsAsciiLetterUpper($_)
    AsciiLetterUpper =             0x0000000000005;

    # [char]::IsUpper($_) -and -not [char]::IsAsciiLetterUpper($_)
    NonAsciiLetterUpper =          0x0000000000008;

    # [char]::IsLower($_) -and -not [char]::IsAsciiLetterUpper($_)
    UppercaseLetter =              0x000000000000d;

    # [char]::IsAsciiLetterLower($_) -and -not [char]::IsAsciiHexDigitLower($_)
    NonHexAsciiLetterLower =       0x0000000000010;

    # [char]::IsAsciiLetterLower($_)
    AsciiLetterLower =             0x0000000000012;

    # [char]::IsAsciiLetter($_) -and -not [char]::IsAsciiHexDigit($_)
    NonHexAsciiLetter =            0x0000000000014;

    # [char]::IsLower($_) -and -not [char]::IsAsciiLetterLower($_)
    NonAsciiLetterLower =          0x0000000000020;

    # [char]::IsUpper($_) -and -not [char]::IsAsciiLetterLower($_)
    LowercaseLetter =              0x0000000000032;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::TitlecaseLetter
    TitlecaseLetter =              0x0000000000040;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierLetter
    ModifierLetter =               0x0000000000080;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherLetter
    OtherLetter =                  0x0000000000100;

    # [char]::IsLetter($_) -and -not [char]::IsAsciiLetter($_)
    NonAsciiLetter =               0x00000000001e8;

    # [char]::IsLetter($_)
    Letter =                       0x00000000001eb;

    # $_ -eq '0' -or $_ -eq '1'
    BinaryDigitNumber =            0x0000000000200;

    # $_ -gt '1' -and $_ -le '7'
    NonBinaryOctalDigit =          0x0000000000400;

    # $_ -ge '0' -and $_ -le '7'
    OctalDigit =                   0x0000000000600;

    # $_ -eq '8' -or $_ -eq '9'
    NonOctalAsciiDigit =           0x0000000000800;

    # [char]::IsAsciiDigit($_)
    AsciiDigit =                   0x0000000000e00;

    # [char]::IsAsciiHexDigitUpper($_)
    AsciiHexDigitUpper =           0x0000000000e01;

    # [char]::IsAsciiHexDigitLower($_)
    AsciiHexDigitLower =           0x0000000000e02;

    # [char]::IsAsciiHexDigit($_)
    AsciiHexDigit =                0x0000000000e03;

    # [char]::IsAsciiLetterOrDigit($_)
    AsciiLetterOrDigit =           0x0000000000e03;

    # [char]::IsDigit($_) -and -not [char]::IsAsciiDigit($_)
    NonAsciiDigit =                0x0000000001000;

    # [char]::IsAsciiLetterOrDigit($_)
    NonAsciiLetterOrDigit =        0x00000000011e8;

    # [char]::IsDigit($_)
    Digit =                        0x0000000001e00;

    # [char]::IsLetterOrDigit($_)
    LetterOrDigit =                0x0000000001feb;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::LetterNumber
    LetterNumber =                 0x0000000002000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherNumber
    OtherNumber =                  0x0000000004000;

    # [char]::IsNumber($_) -and -not [char]::IsAscii($_)
    NonAsciiNumber =               0x0000000007000;

    # [char]::IsNumber($_)
    Number =                       0x0000000007e00;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::NonSpacingMark
    NonSpacingMark =               0x0000000008000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpacingCombiningMark
    SpacingCombiningMark =         0x0000000010000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::EnclosingMark
    EnclosingMark =                0x0000000020000;

    # switch ([char]::GetUnicodeCategory($_)) { NonSpacingMark { return $true } SpacingCombiningMark { return $true } EnclosingMark { return $true } default { return $false } }
    Mark =                         0x0000000038000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation -and [char]::IsAscii($_)
    AsciiConnectorPunctuation =    0x0000000040000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation -and -not [char]::IsAscii($_)
    NonAsciiConnectorPunctuation = 0x0000000080000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation
    ConnectorPunctuation =         0x00000000c0000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::DashPunctuation -and [char]::IsAscii($_)
    AsciiDashPunctuation =         0x0000000100000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::DashPunctuation -and -not [char]::IsAscii($_)
    NonAsciiDashPunctuation =      0x0000000200000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::DashPunctuation
    DashPunctuation =              0x0000000300000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation -and [char]::IsAscii($_)
    AsciiOpenPunctuation =         0x0000000400000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation -and -not [char]::IsAscii($_)
    NonAsciiOpenPunctuation =      0x0000000800000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation
    OpenPunctuation =              0x0000000c00000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation -and [char]::IsAscii($_)
    AsciiClosePunctuation =        0x0000001000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation -and -not [char]::IsAscii($_)
    NonAsciiClosePunctuation =     0x0000002000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation
    ClosePunctuation =             0x0000003000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::InitialQuotePunctuation
    InitialQuotePunctuation =      0x0000004000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::FinalQuotePunctuation
    FinalQuotePunctuation =        0x0000008000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation -and [char]::IsAscii($_)
    OtherAsciiPunctuation =        0x0000010000000;

    # [char]::IsPunctuation($_) -and [char]::IsAscii($_)
    AsciiPunctuation =             0x0000011540000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation -and -not [char]::IsAscii($_)
    OtherNonAsciiPunctuation =     0x0000020000000;

    # [char]::IsPunctuation($_) -and -not [char]::IsAscii($_)
    NonAsciiPunctuation =          0x000002ea80000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation
    OtherPunctuation =             0x0000030000000;

    # [char]::IsPunctuation($_)
    Punctuation =                  0x000003ffc0000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::MathSymbol -and [char]::IsAscii($_)
    AsciiMathSymbol =              0x0000040000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::MathSymbol -and -not [char]::IsAscii($_)
    NonAsciiMathSymbol =           0x0000080000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::MathSymbol
    MathSymbol =                   0x00000c0000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol -and [char]::IsAscii($_)
    AsciiCurrencySymbol =          0x0000100000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol -and -not [char]::IsAscii($_)
    NonAsciiCurrencySymbol =       0x0000200000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol
    CurrencySymbol =               0x0000300000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol -and [char]::IsAscii($_)
    AsciiModifierSymbol =          0x0000400000000;

    # [char]::IsSymbol($_) -and [char]::IsAscii($_)
    AsciiSymbol =                  0x0000540000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol -and -not [char]::IsAscii($_)
    NonAsciiModifierSymbol =       0x0000800000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol
    ModifierSymbol =               0x0000c00000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherSymbol -and -not [char]::IsAscii($_)
    OtherSymbol =                  0x0001000000000;

    # [char]::IsSymbol($_) -and -not [char]::IsAscii($_)
    NonAsciiSymbol =               0x0001a80000000;

    # [char]::IsSymbol($_)
    Symbol =                       0x0001fc0000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator -and [char]::IsAscii($_)
    AsciiSpaceSeparator =          0x0002000000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator -and -not [char]::IsAscii($_)
    NonAsciiSpaceSeparator =       0x0004000000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator
    SpaceSeparator =               0x0006000000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::LineSeparator -and -not [char]::IsAscii($_)
    LineSeparator =                0x0008000000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ParagraphSeparator -and -not [char]::IsAscii($_)
    ParagraphSeparator =           0x0010000000000;

    # [char]::IsSeparator($_) -and -not [char]::IsAscii($_)
    NonAsciiSeparator =            0x001c000000000;

    # [char]::IsSeparator($_)
    Separator =                    0x001e000000000;

    # [char]::IsControl($_) -and [char]::IsAscii($_)
    AsciiControl =                 0x0020000000000;

    # [char]::IsWhiteSpace($_) -and [char]::IsAscii($_)
    AsciiWhiteSpace =              0x0022000000000;

    # [char]::IsAscii($_)
    Ascii =                        0x0022551540e03;

    # [char]::IsControl($_) -and -not [char]::IsAscii($_)
    NonAsciiControl =              0x0040000000000;

    # [char]::IsWhiteSpace($_) -and -not [char]::IsAscii($_)
    NonAsciiWhiteSpace =           0x005c000000000;

    # [char]::IsControl($_)
    Control =                      0x0060000000000;

    # [char]::IsWhiteSpace($_)
    WhiteSpace =                   0x007e000000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::Format
    Format =                       0x0080000000000;

    # [char]::IsHighSurrogate($_)
    HighSurrogate =                0x0100000000000;

    # [char]::IsLowSurrogate($_)
    LowSurrogate =                 0x0200000000000;

    # [char]::IsSurrogate($_)
    Surrogate =                    0x0300000000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::PrivateUse
    PrivateUse =                   0x0400000000000;

    # [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherNotAssigned
    OtherNotAssigned =             0x0800000000000;

    # [char]::IsSurrogate($_)
    NonAscii =                     0x0fddaaeab91e8;
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

Function Get-IndexOfCharacter {
	<#
		.SYNOPSIS
			Gets index of matching characters.

		.DESCRIPTION
			Gets the zero-based index of a character matching the specified values and/or CharacterClass.

		.OUTPUTS
			System.Int32. The zero-based index of the matching character or -1 if no match was found..

		.LINK
            Get-StringComparer

		.LINK
            Test-CharacterClass
	#>
    [CmdletBinding(DefaultParameterSetName = 'ClassOnly')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 0)]
        [AllowEmptyString()]
        # The string value to search
        [string]$InputString,

        [Parameter(ParameterSetName = 'ByChar')]
        [Parameter(Mandatory = $true, ParameterSetName = 'ClassOnly')]
        # Character class(es) to search for.
        [CharacterClass[]]$CharClass,

        [Parameter(Mandatory = $true, ParameterSetName = 'ByChar')]
        # Specific characters to search for.
        [char[]]$Value,

        [Parameter(ParameterSetName = 'ByChar')]
        # Character matching is not case-sensitive
        [switch]$CaseInsensitive,

        # Gets index of last matching character rather than first
        [switch]$Last,

        [ValidateRange(0, [int]::MaxValue)]
        # The zero-based index where to begin searching.
        [int]$StartIndex = 0,

        [ValidateRange(1, [int]::MaxValue)]
        # The number of characters to search.
        [int]$Length
    )

    Process {
        $EndIndex = $StartIndex + $Length;
        if ($EndIndex -gt $InputString.Length) { $EndIndex = $InputString.Length - 1 } else { $EndIndex-- }
        if ($Last.IsPresent) {
            $Index = $EndIndex;
            if ($PSCmdlet.ParameterSetName -eq 'ByChar') {
                if ($CaseInsensitive.IsPresent) {
                    if ($PSBoundParameters.ContainsKey('CharClass')) {
                        while ($Index -ge $StartIndex) {
                            $c = $InputString[$Index];
                            if ($Value -icontains $c -or ($c | Test-CharacterClass -Flags $CharClass)) { break }
                            $Index--;
                        }
                    } else {
                        while ($Index -ge $StartIndex) {
                            if ($InputString[$Index] -icontains $c) { break }
                            $Index--;
                        }
                    }
                } else {
                    if ($PSBoundParameters.ContainsKey('CharClass')) {
                        while ($Index -ge $StartIndex) {
                            $c = $InputString[$Index];
                            if ($Value -ccontains $c -or ($c | Test-CharacterClass -Flags $CharClass)) { break }
                            $Index--;
                        }
                    } else {
                        while ($Index -ge $StartIndex) {
                            if ($InputString[$Index] -ccontains $c) { break }
                            $Index--;
                        }
                    }
                }
            } else {
                while ($Index -ge $StartIndex) {
                    if ($InputString[$Index] | Test-CharacterClass -Flags $CharClass) { break }
                    $Index--;
                }
            }
            if ($Index -ge $StartIndex) {
                $Index | Write-Output;
            } else {
                -1 | Write-Output;
            }
        } else {
            $Index = $StartIndex;
            if ($PSCmdlet.ParameterSetName -eq 'ByChar') {
                if ($CaseInsensitive.IsPresent) {
                    if ($PSBoundParameters.ContainsKey('CharClass')) {
                        while ($Index -le $EndIndex) {
                            $c = $InputString[$Index];
                            if ($Value -icontains $c -or ($c | Test-CharacterClass -Flags $CharClass)) { break }
                            $Index++;
                        }
                    } else {
                        while ($Index -le $EndIndex) {
                            if ($InputString[$Index] -icontains $c) { break }
                            $Index++;
                        }
                    }
                } else {
                    if ($PSBoundParameters.ContainsKey('CharClass')) {
                        while ($Index -le $EndIndex) {
                            $c = $InputString[$Index];
                            if ($Value -ccontains $c -or ($c | Test-CharacterClass -Flags $CharClass)) { break }
                            $Index++;
                        }
                    } else {
                        while ($Index -le $EndIndex) {
                            if ($InputString[$Index] -ccontains $c) { break }
                            $Index++;
                        }
                    }
                }
            } else {
                while ($Index -le $EndIndex) {
                    if ($InputString[$Index] | Test-CharacterClass -Flags $CharClass) { break }
                    $Index++;
                }
            }
            if ($Index -le $EndIndex--) {
                $Index | Write-Output;
            } else {
                -1 | Write-Output;
            }
        }
    }
}

Function Get-CharacterClass {
	<#
		.SYNOPSIS
			Gets character classes of specified characters.

		.DESCRIPTION
			Gets the CharacterClass representing the character class of the sepcified character(s).

		.OUTPUTS
			CharacterClass. The character class.

		.LINK
            Test-CharacterClass

		.LINK
            Get-CharacterClass
	#>
    [CmdletBinding()]
    [OutputType([CharacterClass])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 0)]
        # The target character value.
        [char]$InputValue
    )

    Process {
        [System.Globalization.UnicodeCategory]$Category = [char]::GetUnicodeCategory($c);
        switch ($Category) {
            Surrogate {
                if ([char]::IsHighSurrogate($c)) {
                    [CharacterClass]::HighSurrogate | Write-Output;
                } else {
                    [CharacterClass]::LowSurrogate | Write-Output;
                }
                break;
            }
            UppercaseLetter {
                if ([char]::IsAsciiHexDigitUpper($c)) {
                    [CharacterClass]::HexLetterUpper | Write-Output;
                } else {
                    if ([char]::IsAsciiLetterUpper($c)) {
                        [CharacterClass]::NonHexAsciiLetterUpper | Write-Output;
                    } else {
                        [CharacterClass]::NonAsciiLetterUpper | Write-Output;
                    }
                }
                break;
            }
            LowercaseLetter {
                if ([char]::IsAsciiHexDigitLower($c)) {
                    [CharacterClass]::HexLetterLower | Write-Output;
                } else {
                    if ([char]::IsAsciiLetterLower($c)) {
                        [CharacterClass]::NonHexAsciiLetterLower | Write-Output;
                    } else {
                        [CharacterClass]::NonAsciiLetterLower | Write-Output;
                    }
                }
                break;
            }
            DecimalDigitNumber {
                if ([char]::IsAsciiDigit($c)) {
                    if ($c -le '1') {
                        [CharacterClass]::BinaryDigitNumber | Write-Output;
                    } else {
                        if ($c -gt '7') {
                            [CharacterClass]::NonOctalAsciiDigit | Write-Output;
                        } else {
                            [CharacterClass]::NonBinaryOctalDigit | Write-Output;
                        }
                    }
                } else {
                    [CharacterClass]::NonAsciiDigit | Write-Output;
                }
                break;
            }
            ConnectorPunctuation {
                if ([char]::IsAscii($c)) {
                    [CharacterClass]::AsciiConnectorPunctuation | Write-Output;
                } else {
                    [CharacterClass]::NonAsciiConnectorPunctuation | Write-Output;
                }
                break;
            }
            DashPunctuation {
                if ([char]::IsAscii($c)) {
                    [CharacterClass]::AsciiDashPunctuation | Write-Output;
                } else {
                    [CharacterClass]::NonAsciiDashPunctuation | Write-Output;
                }
                break;
            }
            OpenPunctuation {
                if ([char]::IsAscii($c)) {
                    [CharacterClass]::AsciiOpenPunctuation | Write-Output;
                } else {
                    [CharacterClass]::NonAsciiOpenPunctuation | Write-Output;
                }
                break;
            }
            ClosePunctuation {
                if ([char]::IsAscii($c)) {
                    [CharacterClass]::AsciiClosePunctuation | Write-Output;
                } else {
                    [CharacterClass]::NonAsciiClosePunctuation | Write-Output;
                }
                break;
            }
            OtherPunctuation {
                if ([char]::IsAscii($c)) {
                    [CharacterClass]::OtherAsciiPunctuation | Write-Output;
                } else {
                    [CharacterClass]::OtherNonAsciiPunctuation | Write-Output;
                }
                break;
            }
            MathSymbol {
                if ([char]::IsAscii($c)) {
                    [CharacterClass]::AsciiMathSymbol | Write-Output;
                } else {
                    [CharacterClass]::NonAsciiMathSymbol | Write-Output;
                }
                break;
            }
            CurrencySymbol {
                if ([char]::IsAscii($c)) {
                    [CharacterClass]::AsciiCurrencySymbol | Write-Output;
                } else {
                    [CharacterClass]::NonAsciiCurrencySymbol | Write-Output;
                }
                break;
            }
            ModifierSymbol {
                if ([char]::IsAscii($c)) {
                    [CharacterClass]::AsciiModifierSymbol | Write-Output;
                } else {
                    [CharacterClass]::NonAsciiModifierSymbol | Write-Output;
                }
                break;
            }
            SpaceSeparator {
                if ([char]::IsAscii($c)) {
                    [CharacterClass]::AsciiSpaceSeparator | Write-Output;
                } else {
                    [CharacterClass]::NonAsciiSpaceSeparator | Write-Output;
                }
                break;
            }
            Control {
                if ([char]::IsAscii($c)) {
                    [CharacterClass]::AsciiControl | Write-Output;
                } else {
                    [CharacterClass]::NonAsciiControl | Write-Output;
                }
                break;
            }
            default {
                [enum]::Parse([CharacterClass]$Category.ToString('F')) | Write-Output;
                break;
            }
        }
    }
}

Function Test-CharacterClass {
	<#
		.SYNOPSIS
			Indicates whether a value matches specified CharacterClass(es).

		.DESCRIPTION
			Tests whether the char or CharacterClass value is included in any of the CharacterClass flag values.

		.OUTPUTS
			System.Boolean. True if the target value matches specified character class flag values; otherwise, false.

		.LINK
            Get-CharacterClass
	#>
    [CmdletBinding(DefaultParameterSetName = 'Char')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true,  ParameterSetName = 'Char')]
        # The target character value.
        [char]$Value,

        [Parameter(Mandatory = $true, ValueFromPipeline = $true,  ParameterSetName = 'Enum')]
        # The target character class value.
        [CharacterClass]$Class,

        [Parameter(Mandatory = $true)]
        # The character classes to match.
        [CharacterClass[]]$Flags,

        # Inverts the match: Tests whether the value does not match any of the specified character classes.
        [switch]$IsNot
    )

    Process {
        [ulong]$Cv = 0;
        if ($PSCmdlet.ParameterSetName -eq 'Enum') {
            [ulong]$Cv = $Class;
        } else {
            [ulong]$Cv = $Value | Get-CharacterClass;
        }
        $Matched = $false;
        foreach ($f in $Flags) {
            if ((([ulong]$f) -band $Cv) -eq $Cv) {
                $Matched = $true;
                break;
            }
        }
        if ($IsNot.IsPresent) {
            (-not $Matched) | Write-Output;
        } else {
            $Matched | Write-Output;
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