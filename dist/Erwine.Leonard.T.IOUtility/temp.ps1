
Function ConvertTo-PSLiteral {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [char]$Value,

        [switch]$ForceDoubleQuotes
    )

    Process {
        switch ($Value) {
            "`a" { return '"`a"' }
            "`b" { return '"`b"' }
            "`t" { return '"`t"' }
            "`n" { return '"`n"' }
            "`v" { return '"`v"' }
            "`f" { return '"`f"' }
            "`r" { return '"`r"' }
            "`e" { return '"`e"' }
            "'" { return "`"'`"" }
            '`' {
                if ($ForceDoubleQuotes.IsPresent) { return "'``'" }
                return "'``'";
            }
            '$' {
                if ($ForceDoubleQuotes.IsPresent) { return "'`$'" }
                return "'`$'";
            }
            '"' {
                if ($ForceDoubleQuotes.IsPresent) { return "'`"'" }
                return "'`"'";
            }
        }
        if ([char]::IsControl($Value)) { return "`"``u{$(([int]$Value).ToString('x4'))}`"" }
        if ([char]::IsAscii($Value) -or [char]::IsLetterOrDigit($Value) -or [char]::IsNumber($Value) -or [char]::IsPunctuation($Value) -or [char]::IsSymbol($Value)) {
            if ($ForceDoubleQuotes.IsPresent) { return "`"$Value`"" }
            return "'$Value'";
        }
        return "`"``u{$(([int]$Value).ToString('x4'))}`"";
    }
}
<#
$PrimaryEnumMembers = [ordered]@{
    HexLetterUpper = [PSCustomObject]@{
        Test = { [char]::IsAsciiHexDigitUpper($_) -and -not [char]::IsAsciiDigit($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::UppercaseLetter));
    };
    HexLetterLower = [PSCustomObject]@{
        Test = { [char]::IsAsciiHexDigitLower($_) -and -not [char]::IsAsciiDigit($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::LowercaseLetter));
    };
    NonHexAsciiLetterUpper = [PSCustomObject]@{
        Test = { [char]::IsAsciiLetterUpper($_) -and -not [char]::IsAsciiHexDigitUpper($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::UppercaseLetter));
    };
    NonAsciiLetterUpper = [PSCustomObject]@{
        Test = { [char]::IsUpper($_) -and -not [char]::IsAsciiLetterUpper($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::UppercaseLetter));
    };
    NonHexAsciiLetterLower = [PSCustomObject]@{
        Test = { [char]::IsAsciiLetterLower($_) -and -not [char]::IsAsciiHexDigitLower($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::LowercaseLetter));
    };
    NonAsciiLetterLower = [PSCustomObject]@{
        Test = { [char]::IsLowe($_) -and -not [char]::IsAsciiLetterLower($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::LowercaseLetter));
    };
    TitlecaseLetter = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::TitlecaseLetter };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::TitlecaseLetter));
    };
    ModifierLetter = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierLetter };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ModifierLetter));
    };
    OtherLetter = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherLetter };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OtherLetter));
    };
    BinaryDigitNumber = [PSCustomObject]@{
        Test = { $_ -eq '0' -or $_ -eq '1' };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber));
    };
    NonBinaryOctalDigit = [PSCustomObject]@{
        Test = { $_ -gt '1' -and $_ -le '7' };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber));
    };
    NonOctalAsciiDigit = [PSCustomObject]@{
        Test = { $_ -eq '8' -or $_ -eq '9' };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber));
    };
    NonAsciiDigit = [PSCustomObject]@{
        Test = { [char]::IsDigit($_) -and -not [char]::IsAsciiDigit($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber));
    };
    LetterNumber = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::LetterNumber };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::LetterNumber));
    };
    OtherNumber = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherNumber };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OtherNumber));
    };
    NonSpacingMark = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::NonSpacingMark };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::NonSpacingMark));
    };
    SpacingCombiningMark = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpacingCombiningMark };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::SpacingCombiningMark));
    };
    EnclosingMark = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::EnclosingMark };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::EnclosingMark));
    };
    AsciiConnectorPunctuation = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ConnectorPunctuation));
    };
    NonAsciiConnectorPunctuation = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ConnectorPunctuation));
    };
    AsciiDashPunctuation = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::DashPunctuation -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DashPunctuation));
    };
    NonAsciiDashPunctuation = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::DashPunctuation -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DashPunctuation));
    };
    AsciiOpenPunctuation = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OpenPunctuation));
    };
    NonAsciiOpenPunctuation = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OpenPunctuation));
    };
    AsciiClosePunctuation = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ClosePunctuation));
    };
    NonAsciiClosePunctuation = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ClosePunctuation));
    };
    InitialQuotePunctuation = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::InitialQuotePunctuation };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::InitialQuotePunctuation));
    };
    FinalQuotePunctuation = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::FinalQuotePunctuation };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::FinalQuotePunctuation));
    };
    OtherAsciiPunctuation = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OtherPunctuation));
    };
    OtherNonAsciiPunctuation = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OtherPunctuation));
    };
    AsciiMathSymbol = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::MathSymbol -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::MathSymbol));
    };
    NonAsciiMathSymbol = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::MathSymbol -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::MathSymbol));
    };
    AsciiCurrencySymbol = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol -and [char]::IsAscii($_) };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::CurrencySymbol));
        IsAscii = $true;
    };
    NonAsciiCurrencySymbol = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::CurrencySymbol));
    };
    AsciiModifierSymbol = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ModifierSymbol));
    };
    NonAsciiModifierSymbol = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ModifierSymbol));
    };
    OtherSymbol = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherSymbol -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OtherSymbol));
    };
    AsciiSpaceSeparator = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::SpaceSeparator));
    };
    NonAsciiSpaceSeparator = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::SpaceSeparator));
    };
    LineSeparator = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::LineSeparator -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::LineSeparator));
    };
    ParagraphSeparator = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ParagraphSeparator -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ParagraphSeparator));
    };
    AsciiControl = [PSCustomObject]@{
        Test = { [char]::IsControl($_) -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::Control));
    };
    NonAsciiControl = [PSCustomObject]@{
        Test = { [char]::IsControl($_) -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::Control));
    };
    Format = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::Format };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::Format));
    };
    HighSurrogate = [PSCustomObject]@{
        Test = { [char]::IsHighSurrogate($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::Surrogate));
    };
    LowSurrogate = [PSCustomObject]@{
        Test = { [char]::IsLowSurrogate($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::Surrogate));
    };
    PrivateUse = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::PrivateUse };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::PrivateUse));
    };
    OtherNotAssigned = [PSCustomObject]@{
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherNotAssigned };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OtherNotAssigned));
    };
};
[long]$FlagValue = 0x01;
$PrimaryEnumMembers.Values | ForEach-Object {
    $_ | Add-Member -MemberType NoteProperty -Name 'Characters' -Value ([System.Collections.ObjectModel.Collection[char]]::new());
    $_ | Add-Member -MemberType NoteProperty -Name 'Value' -Value $FlagValue;
    $FlagValue = $FlagValue -shl 1;
}
$LongestName = 2;
$AllEnumMembers = @{};
$PrimaryEnumMembers.Keys | ForEach-Object {
    $len = $_.Length;
    if ($len -gt $LongestName) { $LongestName = $len }
    $AllEnumMembers[$_] = $PrimaryEnumMembers[$_] | Add-Member -MemberType NoteProperty -Name 'Name' -Value $_ -PassThru;
}
$PercentComplete = -1;
#[System.Linq.Enumerable]::Range([int]0, [int]65536) | Group-Object -Property { Expression = { [char]::GetUnicodeCategory($_) } };
[System.Linq.Enumerable]::Range([int]0, [int]65536) | ForEach-Object {
    [char]$c = $_;
    [int]$pct = [Math]::Floor((100.0 * $_) / 65536.0);
    if ($pct -ne $PercentComplete) {
        $PercentComplete = $pct;
        Write-Progress -Activity 'Hash Character Types' -Status "$($_ + 1) of 65536" -PercentComplete $pct;
    }
    [System.Globalization.UnicodeCategory]$Category = [char]::GetUnicodeCategory($c);
    $Key = $Category.ToString('F');
    switch ($Category) {
        Surrogate {
            if ([char]::IsHighSurrogate($c)) {
                $Key = 'HighSurrogate';
            } else {
                if ([char]::IsLowSurrogate($c)) { $Key = 'LowSurrogate' }
            }
            break;
        }
        UppercaseLetter {
            if ([char]::IsAsciiHexDigitUpper($c)) {
                $Key = 'HexLetterUpper';
            } else {
                if ([char]::IsAsciiLetterUpper($c)) { $Key = 'NonHexAsciiLetterUpper' } else {  $Key = 'NonAsciiLetterUpper' }
            }
            break;
        }
        LowercaseLetter {
            if ([char]::IsAsciiHexDigitLower($c)) {
                $Key = 'HexLetterLower';
            } else {
                if ([char]::IsAsciiLetterLower($c)) { $Key = 'NonHexAsciiLetterLower' } else {  $Key = 'NonAsciiLetterLower' }
            }
            break;
        }
        DecimalDigitNumber {
            if ([char]::IsAsciiDigit($c)) {
                if ($c -le '1') {
                    $Key = 'BinaryDigitNumber';
                } else {
                    if ($c -gt '7') { $Key = 'NonOctalAsciiDigit' } else { $Key = 'NonBinaryOctalDigit' }
                }
            } else {
                $Key = 'NonAsciiDigit';
            }
            break;
        }
        ConnectorPunctuation {
            if ([char]::IsAscii($c)) { $Key = 'AsciiConnectorPunctuation' } else { $Key = 'NonAsciiConnectorPunctuation' }
            break;
        }
        DashPunctuation {
            if ([char]::IsAscii($c)) { $Key = 'AsciiDashPunctuation' } else { $Key = 'NonAsciiDashPunctuation' }
            break;
        }
        OpenPunctuation {
            if ([char]::IsAscii($c)) { $Key = 'AsciiOpenPunctuation' } else { $Key = 'NonAsciiOpenPunctuation' }
            break;
        }
        ClosePunctuation {
            if ([char]::IsAscii($c)) { $Key = 'AsciiClosePunctuation' } else { $Key = 'NonAsciiClosePunctuation' }
            break;
        }
        OtherPunctuation {
            if ([char]::IsAscii($c)) { $Key = 'OtherAsciiPunctuation' } else { $Key = 'OtherNonAsciiPunctuation' }
            break;
        }
        MathSymbol {
            if ([char]::IsAscii($c)) { $Key = 'AsciiMathSymbol' } else { $Key = 'NonAsciiMathSymbol' }
            break;
        }
        CurrencySymbol {
            if ([char]::IsAscii($c)) { $Key = 'AsciiCurrencySymbol' } else { $Key = 'NonAsciiCurrencySymbol' }
            break;
        }
        ModifierSymbol {
            if ([char]::IsAscii($c)) { $Key = 'AsciiModifierSymbol' } else { $Key = 'NonAsciiModifierSymbol' }
            break;
        }
        SpaceSeparator {
            if ([char]::IsAscii($c)) { $Key = 'AsciiSpaceSeparator' } else { $Key = 'NonAsciiSpaceSeparator' }
            break;
        }
        Control {
            if ([char]::IsAscii($c)) { $Key = 'AsciiControl' } else { $Key = 'NonAsciiControl' }
            break;
        }
    }
    if ($PrimaryEnumMembers.Contains($Key)) {
        $PrimaryEnumMembers[$Key].Characters.Add($c);
    } else {
        Write-Warning -Message "Key $Key not found for $($c | ConvertTo-PSLiteral) (0x$($_.ToString('x4')))";
    }
};
Write-Progress -Activity 'Hash Character Types' -Status "Hashed 65536 characters" -PercentComplete 100 -Completed;

# $PrimaryFlagNames = @(
#     'HexLetterUpper', 'HexLetterLower', 'NonHexAsciiLetterUpper', 'NonAsciiLetterUpper',
#     'NonHexAsciiLetterLower', 'NonAsciiLetterLower',
#     'TitlecaseLetter', 'ModifierLetter', 'OtherLetter',
#     'BinaryDigitNumber', 'NonBinaryOctalDigit', 'NonOctalAsciiDigit', 'NonAsciiDigit', 'LetterNumber', 'OtherNumber',
#     'NonSpacingMark', 'SpacingCombiningMark', 'EnclosingMark',
#     'AsciiConnectorPunctuation', 'NonAsciiConnectorPunctuation', 'AsciiDashPunctuation', 'NonAsciiDashPunctuation', 'AsciiOpenPunctuation', 'NonAsciiOpenPunctuation', 'AsciiClosePunctuation', 'NonAsciiClosePunctuation', 'InitialQuotePunctuation', 'FinalQuotePunctuation', 'OtherAsciiPunctuation', 'OtherNonAsciiPunctuation',
#     'AsciiMathSymbol', 'NonAsciiMathSymbol', 'AsciiCurrencySymbol', 'NonAsciiCurrencySymbol', 'AsciiModifierSymbol', 'NonAsciiModifierSymbol', 'OtherSymbol',
#     'AsciiSpaceSeparator', 'NonAsciiSpaceSeparator', 'LineSeparator', 'ParagraphSeparator',
#     'AsciiControl', 'NonAsciiControl', 
#     'Format',
#     'HighSurrogate', 'LowSurrogate',
#     'PrivateUse',
#     'OtherNotAssigned'
# );

(
    [PSCustomObject]@{
        Name = 'HexLetter';
        Aggregate = @('HexLetterUpper', 'HexLetterLower');
        Test = { [char]::IsAsciiHexDigit($_) -and -not [char]::IsAsciiDigit($_) };
    },
    [PSCustomObject]@{
        Name = 'AsciiLetterUpper';
        Aggregate = @('HexLetterUpper', 'NonHexAsciiLetterUpper');
        Test = { [char]::IsAsciiLetterUpper($_) };
    },
    [PSCustomObject]@{
        Name = 'AsciiLetterLower';
        Aggregate = @('HexLetterLower', 'NonHexAsciiLetterLower');
        Test = { [char]::IsAsciiLetterLower($_) };
    },
    [PSCustomObject]@{
        Name = 'AsciiLetter';
        Aggregate = @('AsciiLetterUpper', 'AsciiLetterLower');
        Test = { [char]::IsAsciiLetter($_) };
    },
    [PSCustomObject]@{
        Name = 'NonAsciiLetter';
        Aggregate = @('NonAsciiLetterUpper', 'NonAsciiLetterLower', 'TitlecaseLetter', 'ModifierLetter', 'OtherLetter');
        Test = { [char]::IsLetter($_) -and -not [char]::IsAsciiLetter($_) };
    },
    [PSCustomObject]@{
        Name = 'UppercaseLetter';
        Aggregate = @('AsciiLetterUpper', 'NonAsciiLetterUpper');
        Test = { [char]::IsLower($_) -and -not [char]::IsAsciiLetterUpper($_) };
    },
    [PSCustomObject]@{
        Name = 'LowercaseLetter';
        Aggregate = @('AsciiLetterLower', 'NonAsciiLetterLower');
        Test = { [char]::IsUpper($_) -and -not [char]::IsAsciiLetterLower($_) };
    },
    [PSCustomObject]@{
        Name = 'Letter';
        Aggregate = @('AsciiLetter', 'NonAsciiLetter');
        Test = { [char]::IsLetter($_) };
    },
    [PSCustomObject]@{
        Name = 'OctalDigit';
        Aggregate = @('BinaryDigitNumber', 'NonBinaryOctalDigit');
        Test = { $_ -ge '0' -and $_ -le '7' };
    },
    [PSCustomObject]@{
        Name = 'AsciiDigit';
        Aggregate = @('OctalDigit', 'NonOctalAsciiDigit');
        Test = { [char]::IsAsciiDigit($_) };
    },
    [PSCustomObject]@{
        Name = 'Digit';
        Aggregate = @('AsciiDigit', 'NonAsciiDigit');
        Test = { [char]::IsDigit($_) };
    },
    [PSCustomObject]@{
        Name = 'NonAsciiNumber';
        Aggregate = @('NonAsciiDigit', 'LetterNumber', 'OtherNumber');
        Test = { [char]::IsNumber($_) -and -not [char]::IsAscii($_) };
    },
    [PSCustomObject]@{
        Name = 'Number';
        Aggregate = @('AsciiDigit', 'NonAsciiNumber');
        Test = { [char]::IsNumber($_) };
    },
    [PSCustomObject]@{
        Name = 'AsciiHexDigitUpper';
        Aggregate = @('AsciiDigit', 'HexLetterUpper');
        Test = { [char]::IsAsciiHexDigitUpper($_) };
    },
    [PSCustomObject]@{
        Name = 'AsciiHexDigitLower';
        Aggregate = @('AsciiDigit', 'HexLetterLower');
        Test = { [char]::IsAsciiHexDigitLower($_) };
    },
    [PSCustomObject]@{
        Name = 'AsciiHexDigit';
        Aggregate = @('AsciiDigit', 'HexLetter');
        Test = { [char]::IsAsciiHexDigit($_) };
    },
    [PSCustomObject]@{
        Name = 'AsciiLetterOrDigit';
        Aggregate = @('AsciiDigit', 'AsciiLetter');
        Test = { [char]::IsAsciiLetterOrDigit($_) };
    },
    [PSCustomObject]@{
        Name = 'NonAsciiLetterOrDigit';
        Aggregate = @('NonAsciiDigit', 'NonAsciiLetter');
        Test = { [char]::IsAsciiLetterOrDigit($_) };
    },
    [PSCustomObject]@{
        Name = 'LetterOrDigit';
        Aggregate = @('AsciiLetterOrDigit', 'NonAsciiLetterOrDigit');
        Test = { [char]::IsLetterOrDigit($_) };
    },
    [PSCustomObject]@{
        Name = 'Mark';
        Aggregate = @('NonSpacingMark', 'SpacingCombiningMark', 'EnclosingMark');
        Test = { switch ([char]::GetUnicodeCategory($_)) { NonSpacingMark { return $true } SpacingCombiningMark { return $true } EnclosingMark { return $true } default { return $false } } };
    },
    [PSCustomObject]@{
        Name = 'ConnectorPunctuation';
        Aggregate = @('AsciiConnectorPunctuation', 'NonAsciiConnectorPunctuation');
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation };
    },
    [PSCustomObject]@{
        Name = 'DashPunctuation';
        Aggregate = @('AsciiDashPunctuation', 'NonAsciiDashPunctuation');
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::DashPunctuation };
    },
    [PSCustomObject]@{
        Name = 'OpenPunctuation';
        Aggregate = @('AsciiOpenPunctuation', 'NonAsciiOpenPunctuation');
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation };
    },
    [PSCustomObject]@{
        Name = 'ClosePunctuation';
        Aggregate = @('AsciiClosePunctuation', 'NonAsciiClosePunctuation');
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation };
    },
    [PSCustomObject]@{
        Name = 'OtherPunctuation';
        Aggregate = @('OtherAsciiPunctuation', 'OtherNonAsciiPunctuation');
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation };
    },
    [PSCustomObject]@{
        Name = 'AsciiPunctuation';
        Aggregate = @('AsciiConnectorPunctuation', 'AsciiDashPunctuation', 'AsciiOpenPunctuation', 'AsciiClosePunctuation', 'OtherAsciiPunctuation');
        Test = { [char]::IsPunctuation($_) -and [char]::IsAscii($_) };
    },
    [PSCustomObject]@{
        Name = 'NonAsciiPunctuation';
        Aggregate = @('NonAsciiConnectorPunctuation', 'NonAsciiDashPunctuation', 'NonAsciiOpenPunctuation', 'NonAsciiClosePunctuation', 'InitialQuotePunctuation', 'FinalQuotePunctuation', 'OtherNonAsciiPunctuation');
        Test = { [char]::IsPunctuation($_) -and -not [char]::IsAscii($_) };
    },
    [PSCustomObject]@{
        Name = 'Punctuation';
        Aggregate = @('AsciiPunctuation', 'NonAsciiPunctuation');
        Test = { [char]::IsPunctuation($_) };
    },
    [PSCustomObject]@{
        Name = 'MathSymbol';
        Aggregate = @('AsciiMathSymbol', 'NonAsciiMathSymbol');
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::MathSymbol };
    },
    [PSCustomObject]@{
        Name = 'CurrencySymbol';
        Aggregate = @('AsciiCurrencySymbol', 'NonAsciiCurrencySymbol');
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol };
    },
    [PSCustomObject]@{
        Name = 'ModifierSymbol';
        Aggregate = @('AsciiModifierSymbol', 'NonAsciiModifierSymbol');
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol };
    },
    [PSCustomObject]@{
        Name = 'AsciiSymbol';
        Aggregate = @('AsciiMathSymbol', 'AsciiCurrencySymbol', 'AsciiModifierSymbol');
        Test = { [char]::IsSymbol($_) -and [char]::IsAscii($_) };
    },
    [PSCustomObject]@{
        Name = 'NonAsciiSymbol';
        Aggregate = @('NonAsciiMathSymbol', 'NonAsciiCurrencySymbol', 'NonAsciiModifierSymbol', 'OtherSymbol');
        Test = { [char]::IsSymbol($_) -and -not [char]::IsAscii($_) };
    },
    [PSCustomObject]@{
        Name = 'Symbol';
        Aggregate = @('AsciiSymbol', 'NonAsciiSymbol');
        Test = { [char]::IsSymbol($_) };
    },
    [PSCustomObject]@{
        Name = 'SpaceSeparator';
        Aggregate = @('AsciiSpaceSeparator', 'NonAsciiSpaceSeparator');
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator };
    },
    [PSCustomObject]@{
        Name = 'NonAsciiSeparator';
        Aggregate = @('NonAsciiSpaceSeparator', 'LineSeparator', 'ParagraphSeparator');
        Test = { [char]::IsSeparator($_) -and -not [char]::IsAscii($_) };
    },
    [PSCustomObject]@{
        Name = 'Separator';
        Aggregate = @('AsciiSpaceSeparator', 'NonAsciiSeparator');
        Test = { [char]::IsSeparator($_) };
    },
    [PSCustomObject]@{
        Name = 'Control';
        Aggregate = @('AsciiControl', 'NonAsciiControl');
        Test = { [char]::IsControl($_) };
    },
    [PSCustomObject]@{
        Name = 'AsciiWhiteSpace';
        Aggregate = @('AsciiSpaceSeparator', 'AsciiControl');
        Test = { [char]::IsWhiteSpace($_) -and [char]::IsAscii($_) };
    },
    [PSCustomObject]@{
        Name = 'NonAsciiWhiteSpace';
        Aggregate = @('NonAsciiSeparator', 'NonAsciiControl');
        Test = { [char]::IsWhiteSpace($_) -and -not [char]::IsAscii($_) };
    },
    [PSCustomObject]@{
        Name = 'WhiteSpace';
        Aggregate = @('AsciiWhiteSpace', 'NonAsciiWhiteSpace');
        Test = { [char]::IsWhiteSpace($_) };
    },
    [PSCustomObject]@{
        Name = 'Surrogate';
        Aggregate = @('HighSurrogate', 'LowSurrogate');
        Test = { [char]::IsSurrogate($_) };
    },
    [PSCustomObject]@{
        Name = 'Ascii';
        Aggregate = @('AsciiLetterOrDigit', 'AsciiPunctuation', 'AsciiSymbol', 'AsciiWhiteSpace');
        Test = { [char]::IsAscii($_) };
    },
    [PSCustomObject]@{
        Name = 'NonAscii';
        Aggregate = @('NonAsciiLetterOrDigit', 'Mark', 'NonAsciiPunctuation', 'NonAsciiSymbol', 'NonAsciiWhiteSpace', 'Format', 'Surrogate', 'PrivateUse','OtherNotAssigned');
        Test = { [char]::IsSurrogate($_) };
    }
) | ForEach-Object {
    $len = $_.Name.Length;
    if ($len -gt $LongestName) { $LongestName = $len }
    [long]$Flags = 0;
    $FoundAll = $true;
    $Categories = @();
    $AsciiCount = $NonAsciiCount = 0;
    foreach ($n in $_.Aggregate) {
        if ($AllEnumMembers.ContainsKey($n)) {
            $Item = $AllEnumMembers[$n];
            $Flags = $Flags -bor $Item.Value;
            $Categories += @($Item.Categories);
            if ($null -eq $Item.IsAscii) {
                $AsciiCount = $NonAsciiCount = 1;
            } else {
                if ($Item.IsAscii) { $AsciiCount++ } else { $NonAsciiCount++ }
            }
        } else {
            $FoundAll = $false;
            Write-Warning -Message "Could not find item with key $n";
            break;
        }
    }
    if ($FoundAll) {
        if ($NonAsciiCount -eq 0) {
            $AllEnumMembers.Add($_.Name, [PSCustomObject]@{
                Test = $_.Test;
                IsAscii = $true;
                Comment = $_.Aggregate -join ' -bor ';
                Categories = ([System.Globalization.UnicodeCategory[]]@($Categories | Select-Object -Unique));
                Value = $Flags;
                Name = $_.Name;
            });
        } else {
            if ($AsciiCount -eq 0) {
                $AllEnumMembers.Add($_.Name, [PSCustomObject]@{
                    Test = $_.Test;
                    IsAscii = $false;
                    Comment = $_.Aggregate -join ' -bor ';
                    Categories = ([System.Globalization.UnicodeCategory[]]@($Categories | Select-Object -Unique));
                    Value = $Flags;
                    Name = $_.Name;
                });
            } else {
                $AllEnumMembers.Add($_.Name, [PSCustomObject]@{
                    Test = $_.Test;
                    Comment = $_.Aggregate -join ' -bor ';
                    Categories = ([System.Globalization.UnicodeCategory[]]@($Categories | Select-Object -Unique));
                    Value = $Flags;
                    Name = $_.Name;
                });
            }
        }
    }
}

$Writer = [System.IO.StringWriter]::new();
$LongestName++;
foreach ($E in ($AllEnumMembers.Values | Sort-Object -Property 'Value')) {
    if ($null -ne $E.Comment) {
        $Writer.Write('# ');
        $Writer.WriteLine($E.Comment);
    }
    $Writer.WriteLine('[PSCustomObject]@{');
    $Writer.Write("    Name = '");
    $Writer.Write($E.Name);
    $Writer.WriteLine("';");
    $Writer.Write("    Value = 0x");
    $Writer.Write($E.Value.ToString('x12'));
    $Writer.WriteLine(";");
    if ($null -eq $E.Test) {
        Write-Warning -Message "$($E.Name) does not specify Test"
    } else {
        $Writer.Write("    Test = { ");
        $Writer.Write($E.Test.ToString().Trim());
        $Writer.WriteLine(' };');
    }
    if ($null -ne $E.IsAscii) {
        $Writer.Write('    IsAscii = $');
        $Writer.Write($E.IsAscii.ToString().ToLower());
        $Writer.WriteLine(";");
    }
    $Writer.Write('    Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::');
    $Writer.Write($E.Categories[0].ToString('F'));
    foreach ($uc in ($E.Categories | Select-Object -Skip 1)) {
        $Writer.Write(', [System.Globalization.UnicodeCategory]::');
        $Writer.Write($uc.ToString('F'));
    }
    $Writer.WriteLine("));");
    if ($null -ne $E.Characters) {
        $Writer.Write('    Matching = ([char[]]@(');
        $Writer.Write(($E.Characters[0] | ConvertTo-PSLiteral));
        if ($E.Characters.Count -gt 25) {
            foreach ($c in (($E.Characters | Select-Object -Skip 1) | Select-Object -First 24)) {
                $Writer.Write(', ');
                $Writer.Write(($c | ConvertTo-PSLiteral));
            }
        } else {
            foreach ($c in ($E.Characters | Select-Object -Skip 1)) {
                $Writer.Write(', ');
                $Writer.Write(($c | ConvertTo-PSLiteral));
            }
        }
        $Writer.WriteLine("));");
    }
    $Writer.WriteLine('},');
}
$Writer.ToString()
#>

#<#
$Script:CommonCharacters = ([char[]]@("`t", "`n", ' ', '0', '9', 'A', 'F', 'N', 'Z', 'a', 'f', 'n', 'z', '_', '(', '[', ']', '}', '+', '=', '$','^', '`', '˅', '˥',  '¢', '£', '⁀', '︴', '٤', '߁', 'À', 'Ç', 'µ',
    'ß', 'ǅ', 'ᾫ', '©', '°',, 'ʺ', 'ˇ', 'ǂ', 'ח', "`u{2160}", 'Ⅱ', '²', '¾', '֊', '־', '〰', '„', '⁅', '⁆', '⁾', '«', '“', '»', '”', '¡', '§', '±', '⅀', "`u{008c}", "`u{0090}", "`u{d806}", "`u{dc14}",
    "`u{0308}", "`u{0310}", "`u{0982}", "`u{09be}", "`u{20e2}", "`u{20e3}", "`u{2000}", "`u{3000}", "`u{2028}", "`u{2029}", "`u{0605}", "`u{fffb}", "`u{e00b}", "`u{e00e}", "`u{05ce}", "`u{05ec}"));
$EnumMemberSpecs = @(
    [PSCustomObject]@{
        Name = 'HexLetterUpper';
        Value = 0x000000000001;
        Test = { [char]::IsAsciiHexDigitUpper($_) -and -not [char]::IsAsciiDigit($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::UppercaseLetter));
        Matching = ([char[]]@('A', 'B', 'C', 'D', 'E', 'F'));
        Similar = @('AsciiHexDigitLower', 'NonHexAsciiLetterUpper', 'NonAsciiLetterUpper');
    },
    [PSCustomObject]@{
        Name = 'HexLetterLower';
        Value = 0x000000000002;
        Test = { [char]::IsAsciiHexDigitLower($_) -and -not [char]::IsAsciiDigit($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::LowercaseLetter));
        Matching = ([char[]]@('a', 'b', 'c', 'd', 'e', 'f'));
        Similar = @('AsciiHexDigitUpper', 'NonHexAsciiLetterLower', 'NonAsciiLetterLower');
    },
    # HexLetterUpper -bor HexLetterLower
    [PSCustomObject]@{
        Name = 'HexLetter';
        Value = 0x000000000003;
        Test = { [char]::IsAsciiHexDigit($_) -and -not [char]::IsAsciiDigit($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::UppercaseLetter, [System.Globalization.UnicodeCategory]::LowercaseLetter));
        Matching = ([char[]]@('A', 'B', 'C', 'D', 'E', 'F', 'a', 'b', 'c', 'd', 'e', 'f'));
        Similar = @('AsciiDigit', 'NonHexAsciiLetter', 'NonAsciiLetter');
    },
    [PSCustomObject]@{
        Name = 'NonHexAsciiLetterUpper';
        Value = 0x000000000004;
        Test = { [char]::IsAsciiLetterUpper($_) -and -not [char]::IsAsciiHexDigitUpper($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::UppercaseLetter));
        Matching = ([char[]]@('G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'));
        Similar = @('AsciiDigit', 'NonHexAsciiLetterLower', 'NonAsciiLetterUpper');
    },
    # HexLetterUpper -bor NonHexAsciiLetterUpper
    [PSCustomObject]@{
        Name = 'AsciiLetterUpper';
        Value = 0x000000000005;
        Test = { [char]::IsAsciiLetterUpper($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::UppercaseLetter));
        Matching = ([char[]]@('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'));
        Similar = @('AsciiDigit', 'AsciiLetterLower', 'NonAsciiLetterUpper');
    },
    [PSCustomObject]@{
        Name = 'NonAsciiLetterUpper';
        Value = 0x000000000008;
        Test = { [char]::IsUpper($_) -and -not [char]::IsAsciiLetterUpper($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::UppercaseLetter));
        Matching = ([char[]]@('À', 'Æ', 'Ç', 'È', 'Í', 'Ø', 'Ù'));
        Similar = @('AsciiLetterUpper', 'NonAsciiLetterLower', 'TitlecaseLetter', 'ModifierLetter', 'OtherLetter');
    },
    # AsciiLetterUpper -bor NonAsciiLetterUpper
    [PSCustomObject]@{
        Name = 'UppercaseLetter';
        Value = 0x00000000000d;
        Test = { [char]::IsLower($_) -and -not [char]::IsAsciiLetterUpper($_) };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::UppercaseLetter));
        Matching = ([char[]]@('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'À', 'Æ', 'Ç', 'È', 'Í', 'Ø', 'Ù'));
        Similar = @('LowercaseLetter', 'TitlecaseLetter', 'ModifierLetter', 'OtherLetter');
    },
    [PSCustomObject]@{
        Name = 'NonHexAsciiLetterLower';
        Value = 0x000000000010;
        Test = { [char]::IsAsciiLetterLower($_) -and -not [char]::IsAsciiHexDigitLower($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::LowercaseLetter));
        Matching = ([char[]]@('g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'));
        Similar = @('NonHexAsciiLetterUpper', 'TitlecaseLetter', 'ModifierLetter', 'OtherLetter');
    },
    # HexLetterLower -bor NonHexAsciiLetterLower
    [PSCustomObject]@{
        Name = 'AsciiLetterLower';
        Value = 0x000000000012;
        Test = { [char]::IsAsciiLetterLower($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::LowercaseLetter));
        Matching = ([char[]]@('a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'));
        Similar = @('NonAsciiLetterLower', 'AsciiLetterUpper');
    },
    # NonHexAsciiLetterUpper -bor NonHexAsciiLetterLower
    [PSCustomObject]@{
        Name = 'NonHexAsciiLetter';
        Value = 0x000000000014;
        Test = { [char]::IsAsciiLetter($_) -and -not [char]::IsAsciiHexDigit($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::LowercaseLetter));
        Matching = ([char[]]@('G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'));
        Similar = @('AsciiDigit', 'HexLetter', 'NonAsciiLetter');
    },
    # AsciiLetterUpper -bor AsciiLetterLower
    [PSCustomObject]@{
        Name = 'AsciiLetter';
        Value = 0x000000000017;
        Test = { [char]::IsAsciiLetter($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::UppercaseLetter, [System.Globalization.UnicodeCategory]::LowercaseLetter));
        Matching = ([char[]]@('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'));
        Similar = @('AsciiDigit', 'NonAsciiLetter');
    },
    [PSCustomObject]@{
        Name = 'NonAsciiLetterLower';
        Value = 0x000000000020;
        Test = { [char]::IsLowe($_) -and -not [char]::IsAsciiLetterLower($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::LowercaseLetter));
        Matching = ([char[]]@('µ', 'ß', 'à', 'æ', 'ç', 'è', 'í', 'ö'));
        Similar = @('NonAsciiLetterUpper', 'AsciiLetterLower');
    },
    # AsciiLetterLower -bor NonAsciiLetterLower
    [PSCustomObject]@{
        Name = 'LowercaseLetter';
        Value = 0x000000000032;
        Test = { [char]::IsUpper($_) -and -not [char]::IsAsciiLetterLower($_) };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::LowercaseLetter));
        Matching = ([char[]]@('a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'µ', 'ß', 'à', 'æ', 'ç', 'è', 'í', 'ö'));
        Similar = @('UppercaseLetter', 'TitlecaseLetter', 'ModifierLetter', 'OtherLetter');
    },
    [PSCustomObject]@{
        Name = 'TitlecaseLetter';
        Value = 0x000000000040;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::TitlecaseLetter };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::TitlecaseLetter));
        Matching = ([char[]]@('ǅ', 'ǈ', 'ᾈ', 'ᾚ', 'ᾫ'));
        Similar = @('UppercaseLetter', 'LowercaseLetter', 'ModifierLetter', 'OtherLetter');
    },
    [PSCustomObject]@{
        Name = 'ModifierLetter';
        Value = 0x000000000080;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierLetter };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ModifierLetter));
        Matching = ([char[]]@('ʰ', 'ʺ', 'ˇ', 'ˉ', 'ˌ'));
        Similar = @('UppercaseLetter', 'LowercaseLetter', 'TitlecaseLetter', 'OtherLetter');
    },
    [PSCustomObject]@{
        Name = 'OtherLetter';
        Value = 0x000000000100;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherLetter };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OtherLetter));
        Matching = ([char[]]@('ª', 'ƻ', 'ǁ', 'ǂ', 'ח', 'ך', 'נ'));
        Similar = @('UppercaseLetter', 'LowercaseLetter', 'TitlecaseLetter', 'ModifierLetter');
    },
    # NonAsciiLetterUpper -bor NonAsciiLetterLower -bor TitlecaseLetter -bor ModifierLetter -bor OtherLetter
    [PSCustomObject]@{
        Name = 'NonAsciiLetter';
        Value = 0x0000000001e8;
        Test = { [char]::IsLetter($_) -and -not [char]::IsAsciiLetter($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::UppercaseLetter, [System.Globalization.UnicodeCategory]::LowercaseLetter,
            [System.Globalization.UnicodeCategory]::TitlecaseLetter, [System.Globalization.UnicodeCategory]::ModifierLetter, [System.Globalization.UnicodeCategory]::OtherLetter));
        Matching = ([char[]]@('À', 'Æ', 'Ç', 'È', 'Í', 'Ø', 'Ù', 'µ', 'ß', 'à', 'æ', 'ç', 'è', 'í', 'ö', 'ǅ', 'ǈ', 'ᾈ', 'ᾚ', 'ᾫ', 'ʰ', 'ʺ', 'ˇ', 'ˉ', 'ˌ'));
        Similar = @('AsciiLetter');
    },
    # AsciiLetter -bor NonAsciiLetter
    [PSCustomObject]@{
        Name = 'Letter';
        Value = 0x0000000001ff;
        Test = { [char]::IsLetter($_) };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::UppercaseLetter, [System.Globalization.UnicodeCategory]::LowercaseLetter,
            [System.Globalization.UnicodeCategory]::TitlecaseLetter, [System.Globalization.UnicodeCategory]::ModifierLetter, [System.Globalization.UnicodeCategory]::OtherLetter));
        Matching = ([char[]]@('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'À', 'Æ', 'Ç', 'È', 'Í', 'Ø', 'Ù', 'µ', 'ß', 'à', 'æ', 'ç', 'è', 'í', 'ö', 'ǅ', 'ǈ', 'ᾈ', 'ᾚ', 'ᾫ', 'ʰ', 'ʺ', 'ˇ', 'ˉ', 'ˌ'));
            Similar = @('Digit', 'LetterNumber', 'OtherNumber');
    },
    [PSCustomObject]@{
        Name = 'BinaryDigitNumber';
        Value = 0x000000000200;
        Test = { $_ -eq '0' -or $_ -eq '1' };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber));
        Matching = ([char[]]@('0', '1'));
        Similar = @('NonBinaryOctalDigit', 'NonOctalAsciiDigit', 'HexLetter', 'NonAsciiDigit');
    },
    [PSCustomObject]@{
        Name = 'NonBinaryOctalDigit';
        Value = 0x000000000400;
        Test = { $_ -gt '1' -and $_ -le '7' };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber));
        Matching = ([char[]]@('2', '3', '4', '5', '6', '7'));
        Similar = @('BinaryDigitNumber', 'NonOctalAsciiDigit', 'HexLetter', 'NonAsciiDigit');
    },
    # BinaryDigitNumber -bor NonBinaryOctalDigit
    [PSCustomObject]@{
        Name = 'OctalDigit';
        Value = 0x000000000600;
        Test = { $_ -ge '0' -and $_ -le '7' };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber));
        Matching = ([char[]]@('0', '1', '2', '3', '4', '5', '6', '7'));
        Similar = @('NonBinaryOctalDigit', 'NonOctalAsciiDigit', 'HexLetter', 'NonAsciiDigit');
    },
    [PSCustomObject]@{
        Name = 'NonOctalAsciiDigit';
        Value = 0x000000000800;
        Test = { $_ -eq '8' -or $_ -eq '9' };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber));
        Matching = ([char[]]@('8', '9'));
        Similar = @('OctalDigit', 'HexLetter', 'NonAsciiDigit');
    },
    # OctalDigit -bor NonOctalAsciiDigit
    [PSCustomObject]@{
        Name = 'AsciiDigit';
        Value = 0x000000000e00;
        Test = { [char]::IsAsciiDigit($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber));
        Matching = ([char[]]@('0', '1', '2', '3', '4', '5', '6', '7', '8', '9'));
        Similar = @('HexLetter', 'NonAsciiDigit');
    },
    # AsciiDigit -bor HexLetterUpper
    [PSCustomObject]@{
        Name = 'AsciiHexDigitUpper';
        Value = 0x000000000e01;
        Test = { [char]::IsAsciiHexDigitUpper($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber, [System.Globalization.UnicodeCategory]::UppercaseLetter));
        Matching = ([char[]]@('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'));
        Similar = @('NonHexAsciiLetter', 'HexLetterLower', 'NonAsciiDigit');
    },
    # AsciiDigit -bor HexLetterLower
    [PSCustomObject]@{
        Name = 'AsciiHexDigitLower';
        Value = 0x000000000e02;
        Test = { [char]::IsAsciiHexDigitLower($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber, [System.Globalization.UnicodeCategory]::LowercaseLetter));
        Matching = ([char[]]@('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'));
        Similar = @('NonHexAsciiLetter', 'HexLetterUpper', 'NonAsciiDigit');
    },
    # AsciiDigit -bor HexLetter
    [PSCustomObject]@{
        Name = 'AsciiHexDigit';
        Value = 0x000000000e03;
        Test = { [char]::IsAsciiHexDigit($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber, [System.Globalization.UnicodeCategory]::UppercaseLetter,
            [System.Globalization.UnicodeCategory]::LowercaseLetter));
        Matching = ([char[]]@('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'a', 'b', 'c', 'd', 'e', 'f'));
        Similar = @('NonHexAsciiLetter', 'NonAsciiDigit');
    },
    # AsciiDigit -bor AsciiLetter
    [PSCustomObject]@{
        Name = 'AsciiLetterOrDigit';
        Value = 0x000000000e17;
        Test = { [char]::IsAsciiLetterOrDigit($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber, [System.Globalization.UnicodeCategory]::UppercaseLetter,
            [System.Globalization.UnicodeCategory]::LowercaseLetter));
        Matching = ([char[]]@('0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'));
            Similar = @('NonAsciiDigit', 'NonAsciiLetter');
    },
    [PSCustomObject]@{
        Name = 'NonAsciiDigit';
        Value = 0x000000001000;
        Test = { [char]::IsDigit($_) -and -not [char]::IsAsciiDigit($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber));
        Matching = ([char[]]@('٤', '۶', '۸', '߁', '߂', '߃', '߄'));
        Similar = @('AsciiDigit', 'LetterNumber', 'OtherNumber');
    },
    # NonAsciiDigit -bor NonAsciiLetter
    [PSCustomObject]@{
        Name = 'NonAsciiLetterOrDigit';
        Value = 0x0000000011e8;
        Test = { [char]::IsAsciiLetterOrDigit($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber, [System.Globalization.UnicodeCategory]::UppercaseLetter,
            [System.Globalization.UnicodeCategory]::LowercaseLetter, [System.Globalization.UnicodeCategory]::TitlecaseLetter, [System.Globalization.UnicodeCategory]::ModifierLetter,
            [System.Globalization.UnicodeCategory]::OtherLetter));
        Matching = ([char[]]@('٤', '۶', '۸', '߁', '߂', '߃', '߄', 'À', 'Æ', 'Ç', 'È', 'Í', 'Ø', 'Ù', 'µ', 'ß', 'à', 'æ', 'ç', 'è', 'í', 'ö', 'ǅ', 'ǈ', 'ᾈ', 'ᾚ', 'ᾫ', 'ʰ', 'ʺ', 'ˇ', 'ˉ', 'ˌ'));
        Similar = @('AsciiLetterOrDigit', 'LetterNumber', 'OtherNumber');
    },
    # AsciiDigit -bor NonAsciiDigit
    [PSCustomObject]@{
        Name = 'Digit';
        Value = 0x000000001e00;
        Test = { [char]::IsDigit($_) };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber));
        Matching = ([char[]]@('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '٤', '۶', '۸', '߁', '߂', '߃', '߄'));
        Similar = @('LetterNumber', 'OtherNumber');
    },
    # AsciiLetterOrDigit -bor NonAsciiLetterOrDigit
    [PSCustomObject]@{
        Name = 'LetterOrDigit';
        Value = 0x000000001fff;
        Test = { [char]::IsLetterOrDigit($_) };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber, [System.Globalization.UnicodeCategory]::UppercaseLetter,
            [System.Globalization.UnicodeCategory]::LowercaseLetter, [System.Globalization.UnicodeCategory]::TitlecaseLetter, [System.Globalization.UnicodeCategory]::ModifierLetter,
            [System.Globalization.UnicodeCategory]::OtherLetter));
        Matching = ([char[]]@('0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '٤', '۶', '۸', '߁', '߂', '߃', '߄', 'À', 'Æ', 'Ç', 'È', 'Í', 'Ø', 'Ù', 'µ', 'ß', 'à', 'æ', 'ç', 'è', 'í', 'ö', 'ǅ', 'ǈ', 'ᾈ', 'ᾚ', 'ᾫ', 'ʰ', 'ʺ', 'ˇ', 'ˉ', 'ˌ'));
            Similar = @('Symbol', 'Punctuation');
    },
    [PSCustomObject]@{
        Name = 'LetterNumber';
        Value = 0x000000002000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::LetterNumber };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::LetterNumber));
        Matching = ([char[]]@('ᛯ', 'ᛰ', "`u{2160}", 'Ⅱ', 'Ⅳ', "`u{2164}", "`u{2169}", "`u{216c}", "`u{216d}", "`u{216e}", "`u{216f}", "`u{2170}", 'ⅱ', 'ⅳ', "`u{2174}"));
        Similar = @('Digit', 'OtherNumber');
    },
    [PSCustomObject]@{
        Name = 'OtherNumber';
        Value = 0x000000004000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherNumber };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OtherNumber));
        Matching = ([char[]]@('²', '¾', '৹', '௱', '౹', '౺'));
        Similar = @('Digit', 'LetterNumber');
    },
    # NonAsciiDigit -bor LetterNumber -bor OtherNumber
    [PSCustomObject]@{
        Name = 'NonAsciiNumber';
        Value = 0x000000007000;
        Test = { [char]::IsNumber($_) -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber, [System.Globalization.UnicodeCategory]::LetterNumber,
            [System.Globalization.UnicodeCategory]::OtherNumber));
        Matching = ([char[]]@('٤', '۶', '۸', '߁', '߂', '߃', '߄',
            'ᛯ', 'ᛰ', "`u{2160}", 'Ⅱ', 'Ⅳ', "`u{2164}", "`u{2169}", "`u{216c}", "`u{216d}", "`u{216e}", "`u{216f}", "`u{2170}", 'ⅱ', 'ⅳ', "`u{2174}",
            '²', '¾', '৹', '௱', '౹', '౺'));
        Similar = @('AsciiNumber');
    },
    # AsciiDigit -bor NonAsciiNumber
    [PSCustomObject]@{
        Name = 'Number';
        Value = 0x000000007e00;
        Test = { [char]::IsNumber($_) };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber, [System.Globalization.UnicodeCategory]::LetterNumber,
            [System.Globalization.UnicodeCategory]::OtherNumber));
        Matching = ([char[]]@('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '٤', '۶', '۸', '߁', '߂', '߃', '߄',
            'ᛯ', 'ᛰ', "`u{2160}", 'Ⅱ', 'Ⅳ', "`u{2164}", "`u{2169}", "`u{216c}", "`u{216d}", "`u{216e}", "`u{216f}", "`u{2170}", 'ⅱ', 'ⅳ', "`u{2174}",
            '²', '¾', '৹', '௱', '౹', '౺'));
        Similar = @('Letter');
    },
    [PSCustomObject]@{
        Name = 'NonSpacingMark';
        Value = 0x000000008000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::NonSpacingMark };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::NonSpacingMark));
        Matching = ([char[]]@("`u{0300}", "`u{0308}", "`u{0310}", "`u{0314}", "`u{0317}"));
        Similar = @('SpacingCombiningMark', 'EnclosingMark');
    },
    [PSCustomObject]@{
        Name = 'SpacingCombiningMark';
        Value = 0x000000010000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpacingCombiningMark };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::SpacingCombiningMark));
        Matching = ([char[]]@("`u{0903}", "`u{093b}", "`u{0949}", "`u{094f}", "`u{0982}", "`u{09be}", "`u{09c8}", "`u{0a3e}"));
        Similar = @('NonSpacingMark', 'EnclosingMark');
    },
    [PSCustomObject]@{
        Name = 'EnclosingMark';
        Value = 0x000000020000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::EnclosingMark };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::EnclosingMark));
        Matching = ([char[]]@("`u{1abe}", "`u{20dd}", "`u{20de}", "`u{20df}", "`u{20e0}", "`u{20e2}", "`u{20e3}", "`u{20e4}"));
        Similar = @('NonSpacingMark', 'EnclosingMark');
    },
    # NonSpacingMark -bor SpacingCombiningMark -bor EnclosingMark
    [PSCustomObject]@{
        Name = 'Mark';
        Value = 0x000000038000;
        Test = { switch ([char]::GetUnicodeCategory($_)) { NonSpacingMark { return $true } SpacingCombiningMark { return $true } EnclosingMark { return $true } default { return $false } } };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::NonSpacingMark, [System.Globalization.UnicodeCategory]::SpacingCombiningMark,
            [System.Globalization.UnicodeCategory]::EnclosingMark));
        Matching = ([char[]]@("`u{0300}", "`u{0308}", "`u{0310}", "`u{0314}", "`u{0317}",
            "`u{0903}", "`u{093b}", "`u{0949}", "`u{094f}", "`u{0982}", "`u{09be}", "`u{09c8}", "`u{0a3e}",
            "`u{1abe}", "`u{20dd}", "`u{20de}", "`u{20df}", "`u{20e0}", "`u{20e2}", "`u{20e3}", "`u{20e4}"));
        Similar = @('Punctuation', 'Symbol', 'LetterOrDigit');
    },
    [PSCustomObject]@{
        Name = 'AsciiConnectorPunctuation';
        Value = 0x000000040000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ConnectorPunctuation));
        Matching = ([char[]]@('_'));
        Similar = @('AsciiDashPunctuation', 'AsciiOpenPunctuation', 'AsciiClosePunctuation', 'OtherAsciiPunctuation', 'NonAsciiPunctuation');
    },
    [PSCustomObject]@{
        Name = 'NonAsciiConnectorPunctuation';
        Value = 0x000000080000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ConnectorPunctuation));
        Matching = ([char[]]@('‿', '⁀', '⁔', '︳', '︴'));
        Similar = @('AsciiPunctuation', 'NonAsciiDashPunctuation', 'NonAsciiOpenPunctuation', 'NonAsciiClosePunctuation', 'OtherNonAsciiPunctuation', 'InitialQuotePunctuation', 'FinalQuotePunctuation');
    },
    # AsciiConnectorPunctuation -bor NonAsciiConnectorPunctuation
    [PSCustomObject]@{
        Name = 'ConnectorPunctuation';
        Value = 0x0000000c0000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ConnectorPunctuation));
        Matching = ([char[]]@('_', '‿', '⁀', '⁔', '︳', '︴'));
        Similar = @('DashPunctuation', 'OpenPunctuation', 'ClosePunctuation', 'InitialQuotePunctuation', 'FinalQuotePunctuation', 'OtherPunctuation');
    },
    [PSCustomObject]@{
        Name = 'AsciiDashPunctuation';
        Value = 0x000000100000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::DashPunctuation -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DashPunctuation));
        Matching = ([char[]]@('-'));
        Similar = @('AsciiConnectorPunctuation', 'AsciiOpenPunctuation', 'AsciiClosePunctuation', 'OtherAsciiPunctuation', 'NonAsciiPunctuation');
    },
    [PSCustomObject]@{
        Name = 'NonAsciiDashPunctuation';
        Value = 0x000000200000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::DashPunctuation -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DashPunctuation));
        Matching = ([char[]]@('֊', '־', '᠆', '—', '⸺', '〰', '﹣'));
        Similar = @('AsciiPunctuation', 'NonAsciiConnectorPunctuation', 'NonAsciiOpenPunctuation', 'NonAsciiClosePunctuation', 'OtherNonAsciiPunctuation', 'InitialQuotePunctuation', 'FinalQuotePunctuation');
    },
    # AsciiDashPunctuation -bor NonAsciiDashPunctuation
    [PSCustomObject]@{
        Name = 'DashPunctuation';
        Value = 0x000000300000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::DashPunctuation };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DashPunctuation));
        Matching = ([char[]]@('-', '֊', '־', '᠆', '—', '⸺', '〰', '﹣'));
        Similar = @('ConnectorPunctuation', 'OpenPunctuation', 'ClosePunctuation', 'InitialQuotePunctuation', 'FinalQuotePunctuation', 'OtherPunctuation');
    },
    [PSCustomObject]@{
        Name = 'AsciiOpenPunctuation';
        Value = 0x000000400000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OpenPunctuation));
        Matching = ([char[]]@('(', '[', '{'));
        Similar = @('AsciiConnectorPunctuation', 'AsciiDashPunctuation', 'AsciiClosePunctuation', 'OtherAsciiPunctuation', 'NonAsciiPunctuation');
    },
    [PSCustomObject]@{
        Name = 'NonAsciiOpenPunctuation';
        Value = 0x000000800000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OpenPunctuation));
        Matching = ([char[]]@('„', '⁅', '⁽', '❪', '❬', '❰', '⟅', '⟪', '⦃'));
        Similar = @('AsciiPunctuation', 'NonAsciiConnectorPunctuation', 'NonAsciiDashPunctuation', 'NonAsciiClosePunctuation', 'OtherNonAsciiPunctuation', 'InitialQuotePunctuation', 'FinalQuotePunctuation');
    },
    # AsciiOpenPunctuation -bor NonAsciiOpenPunctuation
    [PSCustomObject]@{
        Name = 'OpenPunctuation';
        Value = 0x000000c00000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OpenPunctuation));
        Matching = ([char[]]@('(', '[', '{', '„', '⁅', '⁽', '❪', '❬', '❰', '⟅', '⟪', '⦃'));
        Similar = @('ConnectorPunctuation', 'DashPunctuation', 'ClosePunctuation', 'InitialQuotePunctuation', 'FinalQuotePunctuation', 'OtherPunctuation');
    },
    [PSCustomObject]@{
        Name = 'AsciiClosePunctuation';
        Value = 0x000001000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ClosePunctuation));
        Matching = ([char[]]@(')', ']', '}'));
        Similar = @('AsciiConnectorPunctuation', 'AsciiDashPunctuation', 'AsciiOpenPunctuation', 'OtherAsciiPunctuation', 'NonAsciiPunctuation');
    },
    [PSCustomObject]@{
        Name = 'NonAsciiClosePunctuation';
        Value = 0x000002000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ClosePunctuation));
        Matching = ([char[]]@('⁆', '⁾', '❫', '❭', '❱', '⟆', '⟫', '⦄'));
        Similar = @('AsciiPunctuation', 'NonAsciiConnectorPunctuation', 'NonAsciiDashPunctuation', 'NonAsciiOpenPunctuation', 'OtherNonAsciiPunctuation', 'InitialQuotePunctuation', 'FinalQuotePunctuation');
    },
    # AsciiClosePunctuation -bor NonAsciiClosePunctuation
    [PSCustomObject]@{
        Name = 'ClosePunctuation';
        Value = 0x000003000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ClosePunctuation));
        Matching = ([char[]]@(')', ']', '}', '⁆', '⁾', '❫', '❭', '❱', '⟆', '⟫', '⦄'));
        Similar = @('ConnectorPunctuation', 'DashPunctuation', 'OpenPunctuation', 'InitialQuotePunctuation', 'FinalQuotePunctuation', 'OtherPunctuation');
    },
    [PSCustomObject]@{
        Name = 'InitialQuotePunctuation';
        Value = 0x000004000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::InitialQuotePunctuation };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::InitialQuotePunctuation));
        Matching = ([char[]]@('«', '“', '‟', '⸂', '⸄', '⸉', '⸌', '⸜', '⸠'));
        Similar = @('ConnectorPunctuation', 'DashPunctuation', 'OpenPunctuation', 'ClosePunctuation', 'FinalQuotePunctuation', 'OtherPunctuation');
    },
    [PSCustomObject]@{
        Name = 'FinalQuotePunctuation';
        Value = 0x000008000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::FinalQuotePunctuation };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::FinalQuotePunctuation));
        Matching = ([char[]]@('»', '”', '⸃', '⸅', '⸊', '⸍', '⸝', '⸡'));
        Similar = @('ConnectorPunctuation', 'DashPunctuation', 'OpenPunctuation', 'ClosePunctuation', 'InitialQuotePunctuation', 'OtherPunctuation');
    },
    [PSCustomObject]@{
        Name = 'OtherAsciiPunctuation';
        Value = 0x000010000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OtherPunctuation));
        Matching = ([char[]]@('!', '"', '#', '%', '&', "'", '*', ',', '.', '/', ':', ';', '?', '@', '\'));
        Similar = @('AsciiConnectorPunctuation', 'AsciiDashPunctuation', 'AsciiOpenPunctuation', 'AsciiClosePunctuation', 'NonAsciiPunctuation');
    },
    # AsciiConnectorPunctuation -bor AsciiDashPunctuation -bor AsciiOpenPunctuation -bor AsciiClosePunctuation -bor OtherAsciiPunctuation
    [PSCustomObject]@{
        Name = 'AsciiPunctuation';
        Value = 0x000011540000;
        Test = { [char]::IsPunctuation($_) -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ConnectorPunctuation, [System.Globalization.UnicodeCategory]::DashPunctuation,
            [System.Globalization.UnicodeCategory]::OpenPunctuation, [System.Globalization.UnicodeCategory]::ClosePunctuation, [System.Globalization.UnicodeCategory]::OtherPunctuation));
        Matching = ([char[]]@('_', '-', '(', '[', '{', ')', ']', '}', '!', '"', '#', '%', '&', "'", '*', ',', '.', '/', ':', ';', '?', '@', '\'));
        Similar = @('NonAsciiPunctuation', 'Symbol');
    },
    [PSCustomObject]@{
        Name = 'OtherNonAsciiPunctuation';
        Value = 0x000020000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OtherPunctuation));
        Matching = ([char[]]@('¡', '§', '¿', '՟', '׆', '؉', '؛', '؝'));
        Similar = @('AsciiPunctuation', 'NonAsciiConnectorPunctuation', 'NonAsciiDashPunctuation', 'NonAsciiOpenPunctuation', 'NonAsciiClosePunctuation', 'InitialQuotePunctuation', 'FinalQuotePunctuation');
    },
    # NonAsciiConnectorPunctuation -bor NonAsciiDashPunctuation -bor NonAsciiOpenPunctuation -bor NonAsciiClosePunctuation -bor InitialQuotePunctuation -bor FinalQuotePunctuation -bor OtherNonAsciiPunctuation     
    [PSCustomObject]@{
        Name = 'NonAsciiPunctuation';
        Value = 0x00002ea80000;
        Test = { [char]::IsPunctuation($_) -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ConnectorPunctuation, [System.Globalization.UnicodeCategory]::DashPunctuation,
            [System.Globalization.UnicodeCategory]::OpenPunctuation, [System.Globalization.UnicodeCategory]::ClosePunctuation, [System.Globalization.UnicodeCategory]::InitialQuotePunctuation,
            [System.Globalization.UnicodeCategory]::FinalQuotePunctuation, [System.Globalization.UnicodeCategory]::OtherPunctuation));
        Matching = ([char[]]@('‿', '⁀', '⁔', '︳', '︴', '֊', '־', '᠆', '—', '⸺', '〰', '﹣', '„', '⁅', '⁽', '❪', '❬', '❰', '⟅', '⟪', '⦃', '⁆', '⁾', '❫', '❭', '❱', '⟆', '⟫', '⦄',
            '«', '“', '‟', '⸂', '⸄', '⸉', '⸌', '⸜', '⸠', '»', '”', '⸃', '⸅', '⸊', '⸍', '⸝', '⸡', '¡', '§', '¿', '՟', '׆', '؉', '؛', '؝'));
            Similar = @('AsciiPunctuation', 'Symbol');
    },
    # OtherAsciiPunctuation -bor OtherNonAsciiPunctuation
    [PSCustomObject]@{
        Name = 'OtherPunctuation';
        Value = 0x000030000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OtherPunctuation));
        Matching = ([char[]]@('!', '"', '#', '%', '&', "'", '*', ',', '.', '/', ':', ';', '?', '@', '\', '¡', '§', '¿', '՟', '׆', '؉', '؛', '؝'));
        Similar = @('ConnectorPunctuation', 'DashPunctuation', 'OpenPunctuation', 'ClosePunctuation', 'InitialQuotePunctuation', 'FinalQuotePunctuation');
    },
    # AsciiPunctuation -bor NonAsciiPunctuation
    [PSCustomObject]@{
        Name = 'Punctuation';
        Value = 0x00003ffc0000;
        Test = { [char]::IsPunctuation($_) };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ConnectorPunctuation, [System.Globalization.UnicodeCategory]::DashPunctuation,
            [System.Globalization.UnicodeCategory]::OpenPunctuation, [System.Globalization.UnicodeCategory]::ClosePunctuation, [System.Globalization.UnicodeCategory]::OtherPunctuation,
            [System.Globalization.UnicodeCategory]::InitialQuotePunctuation, [System.Globalization.UnicodeCategory]::FinalQuotePunctuation));
        Matching = ([char[]]@('_', '-', '(', '[', '{', ')', ']', '}', '!', '"', '#', '%', '&', "'", '*', ',', '.', '/', ':', ';', '?', '@', '\',
            '‿', '⁀', '⁔', '︳', '︴', '֊', '־', '᠆', '—', '⸺', '〰', '﹣', '„', '⁅', '⁽', '❪', '❬', '❰', '⟅', '⟪', '⦃', '⁆', '⁾', '❫', '❭', '❱', '⟆', '⟫', '⦄',
            '«', '“', '‟', '⸂', '⸄', '⸉', '⸌', '⸜', '⸠', '»', '”', '⸃', '⸅', '⸊', '⸍', '⸝', '⸡', '¡', '§', '¿', '՟', '׆', '؉', '؛', '؝'));
            Similar = @('Symbol', 'Mark');
    },
    [PSCustomObject]@{
        Name = 'AsciiMathSymbol';
        Value = 0x000040000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::MathSymbol -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::MathSymbol));
        Matching = ([char[]]@('+', '<', '=', '>', '|', '~'));
        Similar = @('AsciiCurrencySymbol', 'AsciiModifierSymbol', 'NonAsciiSymbol');
    },
    [PSCustomObject]@{
        Name = 'NonAsciiMathSymbol';
        Value = 0x000080000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::MathSymbol -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::MathSymbol));
        Matching = ([char[]]@('±', '÷', '϶', '⅀', '⅁', '⅂', '⅃', '⅄', '←', '↑'));
        Similar = @('AsciiSymbol', 'NonAsciiCurrencySymbol', 'NonAsciiModifierSymbol', 'OtherSymbol');
    },
    # AsciiMathSymbol -bor NonAsciiMathSymbol
    [PSCustomObject]@{
        Name = 'MathSymbol';
        Value = 0x0000c0000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::MathSymbol };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::MathSymbol));
        Matching = ([char[]]@('+', '<', '=', '>', '|', '~', '±', '÷', '϶', '⅀', '⅁', '⅂', '⅃', '⅄', '←', '↑'));
        Similar = @('CurrencySymbol', 'ModifierSymbol', 'OtherSymbol');
    },
    [PSCustomObject]@{
        Name = 'AsciiCurrencySymbol';
        Value = 0x000100000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::CurrencySymbol));
        Matching = ([char[]]@('$'));
        Similar = @('AsciiMathSymbol', 'AsciiModifierSymbol', 'NonAsciiSymbol');
    },
    [PSCustomObject]@{
        Name = 'NonAsciiCurrencySymbol';
        Value = 0x000200000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::CurrencySymbol));
        Matching = ([char[]]@('¢', '£', '¤', '¥', '֏', '؋', '฿', '₤', '₩'));
        Similar = @('AsciiSymbol', 'NonAsciiMathSymbol', 'NonAsciiModifierSymbol', 'OtherSymbol');
    },
    # AsciiCurrencySymbol -bor NonAsciiCurrencySymbol
    [PSCustomObject]@{
        Name = 'CurrencySymbol';
        Value = 0x000300000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::CurrencySymbol));
        Matching = ([char[]]@('$', '¢', '£', '¤', '¥', '֏', '؋', '฿', '₤', '₩'));
        Similar = @('MathSymbol', 'ModifierSymbol', 'OtherSymbol');
    },
    [PSCustomObject]@{
        Name = 'AsciiModifierSymbol';
        Value = 0x000400000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ModifierSymbol));
        Matching = ([char[]]@('^', '`'));
        Similar = @('AsciiMathSymbol', 'AsciiCurrencySymbol', 'NonAsciiSymbol');
    },
    # AsciiMathSymbol -bor AsciiCurrencySymbol -bor AsciiModifierSymbol
    [PSCustomObject]@{
        Name = 'AsciiSymbol';
        Value = 0x000540000000;
        Test = { [char]::IsSymbol($_) -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::MathSymbol, [System.Globalization.UnicodeCategory]::CurrencySymbol,
            [System.Globalization.UnicodeCategory]::ModifierSymbol));
        Matching = ([char[]]@('+', '<', '=', '>', '|', '~', '$', '^', '`'));
        Similar = @('NonAsciiSymbol');
    },
    [PSCustomObject]@{
        Name = 'NonAsciiModifierSymbol';
        Value = 0x000800000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ModifierSymbol));
        Matching = ([char[]]@('˅', '˓', '˔', '˖', '˘', '˚', '˝', '˥'));
        Similar = @('AsciiSymbol', 'NonAsciiMathSymbol', 'NonAsciiCurrencySymbol', 'OtherSymbol');
    },
    # AsciiModifierSymbol -bor NonAsciiModifierSymbol
    [PSCustomObject]@{
        Name = 'ModifierSymbol';
        Value = 0x000c00000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ModifierSymbol));
        Matching = ([char[]]@('^', '`', '˅', '˓', '˔', '˖', '˘', '˚', '˝', '˥'));
        Similar = @('MathSymbol', 'CurrencySymbol', 'OtherSymbol');
    },
    [PSCustomObject]@{
        Name = 'OtherSymbol';
        Value = 0x001000000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherSymbol -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OtherSymbol));
        Matching = ([char[]]@('©', '®', '°', '҂', '۞', '۩', '۾', '߶', '৺', '୰', '౿'));
        Similar = @('MathSymbol', 'CurrencySymbol', 'ModifierSymbol');
    },
    # NonAsciiMathSymbol -bor NonAsciiCurrencySymbol -bor NonAsciiModifierSymbol -bor OtherSymbol
    [PSCustomObject]@{
        Name = 'NonAsciiSymbol';
        Value = 0x001a80000000;
        Test = { [char]::IsSymbol($_) -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::MathSymbol, [System.Globalization.UnicodeCategory]::CurrencySymbol,
            [System.Globalization.UnicodeCategory]::ModifierSymbol, [System.Globalization.UnicodeCategory]::OtherSymbol));
        Matching = ([char[]]@('±', '÷', '϶', '⅀', '⅁', '⅂', '⅃', '⅄', '←', '↑', '¢', '£', '¤', '¥', '֏', '؋', '฿', '₤', '₩', '˅', '˓', '˔', '˖', '˘', '˚', '˝', '˥',
            '©', '®', '°', '҂', '۞', '۩', '۾', '߶', '৺', '୰', '౿'));
        Similar = @('AsciiSymbol');
    },
    # AsciiSymbol -bor NonAsciiSymbol
    [PSCustomObject]@{
        Name = 'Symbol';
        Value = 0x001fc0000000;
        Test = { [char]::IsSymbol($_) };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::MathSymbol, [System.Globalization.UnicodeCategory]::CurrencySymbol,
            [System.Globalization.UnicodeCategory]::ModifierSymbol, [System.Globalization.UnicodeCategory]::OtherSymbol));
        Matching = ([char[]]@('+', '<', '=', '>', '|', '~', '$', '^', '`',
            '±', '÷', '϶', '⅀', '⅁', '⅂', '⅃', '⅄', '←', '↑', '¢', '£', '¤', '¥', '֏', '؋', '฿', '₤', '₩', '˅', '˓', '˔', '˖', '˘', '˚', '˝', '˥',
            '©', '®', '°', '҂', '۞', '۩', '۾', '߶', '৺', '୰', '౿'));
        Similar = @('Punctuation');
    },
    [PSCustomObject]@{
        Name = 'AsciiSpaceSeparator';
        Value = 0x002000000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::SpaceSeparator));
        Matching = ([char[]]@(' '));
        Similar = @('NonAsciiWhiteSpace');
    },
    [PSCustomObject]@{
        Name = 'NonAsciiSpaceSeparator';
        Value = 0x004000000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::SpaceSeparator));
        Matching = ([char[]]@("`u{00a0}", "`u{1680}", "`u{2000}", "`u{2008}", "`u{200a}", "`u{202f}", "`u{205f}", "`u{3000}"));
        Similar = @('AsciiSpaceSeparator', 'LineSeparator', 'ParagraphSeparator', 'Control');
    },
    # AsciiSpaceSeparator -bor NonAsciiSpaceSeparator
    [PSCustomObject]@{
        Name = 'SpaceSeparator';
        Value = 0x006000000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::SpaceSeparator));
        Matching = ([char[]]@(' ', "`u{00a0}", "`u{1680}", "`u{2000}", "`u{2008}", "`u{200a}", "`u{202f}", "`u{205f}", "`u{3000}"));
        Similar = @('LineSeparator', 'ParagraphSeparator', 'Control');
    },
    [PSCustomObject]@{
        Name = 'LineSeparator';
        Value = 0x008000000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::LineSeparator -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::LineSeparator));
        Matching = ([char[]]@("`u{2028}"));
        Similar = @('SpaceSeparator', 'ParagraphSeparator', 'Control');
    },
    [PSCustomObject]@{
        Name = 'ParagraphSeparator';
        Value = 0x010000000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ParagraphSeparator -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::ParagraphSeparator));
        Matching = ([char[]]@("`u{2029}"));
        Similar = @('SpaceSeparator', 'LineSeparator', 'Control');
    },
    # NonAsciiSpaceSeparator -bor LineSeparator -bor ParagraphSeparator
    [PSCustomObject]@{
        Name = 'NonAsciiSeparator';
        Value = 0x01c000000000;
        Test = { [char]::IsSeparator($_) -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::SpaceSeparator, [System.Globalization.UnicodeCategory]::LineSeparator,
            [System.Globalization.UnicodeCategory]::ParagraphSeparator));
        Matching = ([char[]]@("`u{00a0}", "`u{1680}", "`u{2000}", "`u{2008}", "`u{200a}", "`u{202f}", "`u{205f}", "`u{3000}", "`u{2028}", "`u{2029}"));
        Similar = @('AsciiSpaceSeparator');
    },
    # AsciiSpaceSeparator -bor NonAsciiSeparator
    [PSCustomObject]@{
        Name = 'Separator';
        Value = 0x01e000000000;
        Test = { [char]::IsSeparator($_) };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::SpaceSeparator, [System.Globalization.UnicodeCategory]::LineSeparator,
            [System.Globalization.UnicodeCategory]::ParagraphSeparator));
        Matching = ([char[]]@(' ', "`u{00a0}", "`u{1680}", "`u{2000}", "`u{2008}", "`u{200a}", "`u{202f}", "`u{205f}", "`u{3000}", "`u{2028}", "`u{2029}"));
        Similar = @('LineSeparator', 'ParagraphSeparator', 'Control');
    },
    [PSCustomObject]@{
        Name = 'AsciiControl';
        Value = 0x020000000000;
        Test = { [char]::IsControl($_) -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::Control));
        Matching = ([char[]]@("`t", "`n", "`v", "`f", "`r"));
        Similar = @('Separator', 'NonAsciiControl');
    },
    # AsciiSpaceSeparator -bor AsciiControl
    [PSCustomObject]@{
        Name = 'AsciiWhiteSpace';
        Value = 0x022000000000;
        Test = { [char]::IsWhiteSpace($_) -and [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::SpaceSeparator, [System.Globalization.UnicodeCategory]::Control));
        Matching = ([char[]]@("`t", "`n", "`v", "`f", "`r", ' '));
        Similar = @('NonAsciiControl');
    },
    # AsciiLetterOrDigit -bor AsciiPunctuation -bor AsciiSymbol -bor AsciiWhiteSpace
    [PSCustomObject]@{
        Name = 'Ascii';
        Value = 0x022551540e17;
        Test = { [char]::IsAscii($_) };
        IsAscii = $true;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber, [System.Globalization.UnicodeCategory]::UppercaseLetter,
            [System.Globalization.UnicodeCategory]::LowercaseLetter, [System.Globalization.UnicodeCategory]::ConnectorPunctuation, [System.Globalization.UnicodeCategory]::DashPunctuation,
            [System.Globalization.UnicodeCategory]::OpenPunctuation, [System.Globalization.UnicodeCategory]::ClosePunctuation, [System.Globalization.UnicodeCategory]::OtherPunctuation,
            [System.Globalization.UnicodeCategory]::MathSymbol, [System.Globalization.UnicodeCategory]::CurrencySymbol, [System.Globalization.UnicodeCategory]::ModifierSymbol,
            [System.Globalization.UnicodeCategory]::SpaceSeparator, [System.Globalization.UnicodeCategory]::Control));
        Matching = ([char[]]@("`t", "`n", "`v", "`f", "`r", ' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '_', '-', '(', '[', '{', ')', ']', '}', '!', '"', '#', '%', '&', "'", '*', ',', '.', '/', ':', ';', '?', '@', '\',
            '+', '<', '=', '>', '|', '~', '$', '^', '`'));
        Similar = @('NonAscii');
    },
    [PSCustomObject]@{
        Name = 'NonAsciiControl';
        Value = 0x040000000000;
        Test = { [char]::IsControl($_) -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::Control));
        Matching = ([char[]]@("`u{0080}", "`u{0084}", "`u{0088}", "`u{008c}", "`u{0090}", "`u{009c}", "`u{009f}"));
        Similar = @('Separator', 'AsciiControl');
    }, 
    # NonAsciiSeparator -bor NonAsciiControl
    [PSCustomObject]@{
        Name = 'NonAsciiWhiteSpace';
        Value = 0x05c000000000;
        Test = { [char]::IsWhiteSpace($_) -and -not [char]::IsAscii($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::SpaceSeparator, [System.Globalization.UnicodeCategory]::LineSeparator,
            [System.Globalization.UnicodeCategory]::ParagraphSeparator, [System.Globalization.UnicodeCategory]::Control));
        Matching = ([char[]]@("`u{00a0}", "`u{1680}", "`u{2000}", "`u{2008}", "`u{200a}", "`u{202f}", "`u{205f}", "`u{3000}", "`u{2028}", "`u{2029}",
            "`u{0080}", "`u{0084}", "`u{0088}", "`u{008c}", "`u{0090}", "`u{009c}", "`u{009f}"));
        Similar = @('AsciiSpaceSeparator', 'AsciiControl');
    },
    # AsciiControl -bor NonAsciiControl
    [PSCustomObject]@{
        Name = 'Control';
        Value = 0x060000000000;
        Test = { [char]::IsControl($_) };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::Control));
        Matching = ([char[]]@("`t", "`n", "`v", "`f", "`r", "`u{0080}", "`u{0084}", "`u{0088}", "`u{008c}", "`u{0090}", "`u{009c}", "`u{009f}"));
        Similar = @('Separator');
    },
    # AsciiWhiteSpace -bor NonAsciiWhiteSpace
    [PSCustomObject]@{
        Name = 'WhiteSpace';
        Value = 0x07e000000000;
        Test = { [char]::IsWhiteSpace($_) };
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::SpaceSeparator, [System.Globalization.UnicodeCategory]::Control,
            [System.Globalization.UnicodeCategory]::LineSeparator, [System.Globalization.UnicodeCategory]::ParagraphSeparator));
        Matching = ([char[]]@("`t", "`n", "`v", "`f", "`r", ' ',
            "`u{00a0}", "`u{1680}", "`u{2000}", "`u{2008}", "`u{200a}", "`u{202f}", "`u{205f}", "`u{3000}", "`u{2028}", "`u{2029}",
            "`u{0080}", "`u{0084}", "`u{0088}", "`u{008c}", "`u{0090}", "`u{009c}", "`u{009f}"));
            Similar = @('Format', 'Surrogate');
    },
    [PSCustomObject]@{
        Name = 'Format';
        Value = 0x080000000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::Format };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::Format));
        Matching = ([char[]]@("`u{00ad}", "`u{0601}", "`u{0605}", "`u{061c}", "`u{070f}", "`u{200d}", "`u{2060}", "`u{206a}", "`u{206f}", "`u{feff}", "`u{fffb}"));
        Similar = @('WhiteSpace', 'Surrogate');
    },
    [PSCustomObject]@{
        Name = 'HighSurrogate';
        Value = 0x100000000000;
        Test = { [char]::IsHighSurrogate($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::Surrogate));
        Matching = ([char[]]@("`u{d800}", "`u{d803}", "`u{d806}", "`u{d809}", "`u{d80c}", "`u{d80f}", "`u{d812}", "`u{d815}", "`u{d818}"));
        Similar = @('WhiteSpace', 'LowSurrogate');
    },
    [PSCustomObject]@{
        Name = 'LowSurrogate';
        Value = 0x200000000000;
        Test = { [char]::IsLowSurrogate($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::Surrogate));
        Matching = ([char[]]@("`u{dc00}", "`u{dc02}", "`u{dc05}", "`u{dc08}", "`u{dc0b}", "`u{dc0e}", "`u{dc11}", "`u{dc14}", "`u{dc17}", "`u{dc18}"));
        Similar = @('WhiteSpace', 'HighSurrogate');
    },
    # HighSurrogate -bor LowSurrogate
    [PSCustomObject]@{
        Name = 'Surrogate';
        Value = 0x300000000000;
        Test = { [char]::IsSurrogate($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::Surrogate));
        Matching = ([char[]]@("`u{d800}", "`u{d803}", "`u{d806}", "`u{d809}", "`u{d80c}", "`u{d80f}", "`u{d812}", "`u{d815}", "`u{d818}",
            "`u{dc00}", "`u{dc02}", "`u{dc05}", "`u{dc08}", "`u{dc0b}", "`u{dc0e}", "`u{dc11}", "`u{dc14}", "`u{dc17}", "`u{dc18}"));
            Similar = @('Format', 'WhiteSpace');
    },
    [PSCustomObject]@{
        Name = 'PrivateUse';
        Value = 0x400000000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::PrivateUse };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::PrivateUse));
        Matching = ([char[]]@("`u{e002}", "`u{e005}", "`u{e008}", "`u{e00b}", "`u{e00e}", "`u{e011}", "`u{e014}", "`u{e017}", "`u{e018}"));
        Similar = @('OtherNotAssigned', 'WhiteSpace');
    },
    [PSCustomObject]@{
        Name = 'OtherNotAssigned';
        Value = 0x800000000000;
        Test = { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherNotAssigned };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::OtherNotAssigned));
        Matching = ([char[]]@("`u{0378}", "`u{0381}", "`u{038b}", "`u{0530}", "`u{058b}", "`u{05c8}", "`u{05cb}", "`u{05ce}", "`u{05ec}"));
        Similar = @('PrivateUse', 'WhiteSpace');
    },
    # NonAsciiLetterOrDigit -bor Mark -bor NonAsciiPunctuation -bor NonAsciiSymbol -bor NonAsciiWhiteSpace -bor Format -bor Surrogate -bor PrivateUse -bor OtherNotAssigned
    [PSCustomObject]@{
        Name = 'NonAscii';
        Value = 0xfddaaeab91e8;
        Test = { [char]::IsSurrogate($_) };
        IsAscii = $false;
        Categories = ([System.Globalization.UnicodeCategory[]]@([System.Globalization.UnicodeCategory]::DecimalDigitNumber, [System.Globalization.UnicodeCategory]::UppercaseLetter,
            [System.Globalization.UnicodeCategory]::LowercaseLetter, [System.Globalization.UnicodeCategory]::TitlecaseLetter, [System.Globalization.UnicodeCategory]::ModifierLetter,
            [System.Globalization.UnicodeCategory]::OtherLetter, [System.Globalization.UnicodeCategory]::NonSpacingMark, [System.Globalization.UnicodeCategory]::SpacingCombiningMark,
            [System.Globalization.UnicodeCategory]::EnclosingMark, [System.Globalization.UnicodeCategory]::ConnectorPunctuation, [System.Globalization.UnicodeCategory]::DashPunctuation,
            [System.Globalization.UnicodeCategory]::OpenPunctuation, [System.Globalization.UnicodeCategory]::ClosePunctuation, [System.Globalization.UnicodeCategory]::InitialQuotePunctuation,
            [System.Globalization.UnicodeCategory]::FinalQuotePunctuation, [System.Globalization.UnicodeCategory]::OtherPunctuation, [System.Globalization.UnicodeCategory]::MathSymbol,
            [System.Globalization.UnicodeCategory]::CurrencySymbol, [System.Globalization.UnicodeCategory]::ModifierSymbol, [System.Globalization.UnicodeCategory]::OtherSymbol,
            [System.Globalization.UnicodeCategory]::SpaceSeparator, [System.Globalization.UnicodeCategory]::LineSeparator, [System.Globalization.UnicodeCategory]::ParagraphSeparator,
            [System.Globalization.UnicodeCategory]::Control, [System.Globalization.UnicodeCategory]::Format, [System.Globalization.UnicodeCategory]::Surrogate,
            [System.Globalization.UnicodeCategory]::PrivateUse, [System.Globalization.UnicodeCategory]::OtherNotAssigned));
        Matching = ([char[]]@('٤', '۶', '۸', '߁', '߂', '߃', '߄', 'À', 'Æ', 'Ç', 'È', 'Í', 'Ø', 'Ù', 'µ', 'ß', 'à', 'æ', 'ç', 'è', 'í', 'ö', 'ǅ', 'ǈ', 'ᾈ', 'ᾚ', 'ᾫ', 'ʰ', 'ʺ', 'ˇ', 'ˉ', 'ˌ',
            '‿', '⁀', '⁔', '︳', '︴', '֊', '־', '᠆', '—', '⸺', '〰', '﹣', '„', '⁅', '⁽', '❪', '❬', '❰', '⟅', '⟪', '⦃', '⁆', '⁾', '❫', '❭', '❱', '⟆', '⟫', '⦄',
            '«', '“', '‟', '⸂', '⸄', '⸉', '⸌', '⸜', '⸠', '»', '”', '⸃', '⸅', '⸊', '⸍', '⸝', '⸡', '¡', '§', '¿', '՟', '׆', '؉', '؛', '؝',
            '±', '÷', '϶', '⅀', '⅁', '⅂', '⅃', '⅄', '←', '↑', '¢', '£', '¤', '¥', '֏', '؋', '฿', '₤', '₩', '˅', '˓', '˔', '˖', '˘', '˚', '˝', '˥',
            '©', '®', '°', '҂', '۞', '۩', '۾', '߶', '৺', '୰', '౿',
            "`u{00a0}", "`u{1680}", "`u{2000}", "`u{2008}", "`u{200a}", "`u{202f}", "`u{205f}", "`u{3000}", "`u{2028}", "`u{2029}",
            "`u{0080}", "`u{0084}", "`u{0088}", "`u{008c}", "`u{0090}", "`u{009c}", "`u{009f}",
            "`u{00ad}", "`u{0601}", "`u{0605}", "`u{061c}", "`u{070f}", "`u{200d}", "`u{2060}", "`u{206a}", "`u{206f}", "`u{feff}", "`u{fffb}",
            "`u{d800}", "`u{d803}", "`u{d806}", "`u{d809}", "`u{d80c}", "`u{d80f}", "`u{d812}", "`u{d815}", "`u{d818}",
            "`u{dc00}", "`u{dc02}", "`u{dc05}", "`u{dc08}", "`u{dc0b}", "`u{dc0e}", "`u{dc11}", "`u{dc14}", "`u{dc17}", "`u{dc18}",
            "`u{e002}", "`u{e005}", "`u{e008}", "`u{e00b}", "`u{e00e}", "`u{e011}", "`u{e014}", "`u{e017}", "`u{e018}",
            "`u{0378}", "`u{0381}", "`u{038b}", "`u{0530}", "`u{058b}", "`u{05c8}", "`u{05cb}", "`u{05ce}", "`u{05ec}"));
            Similar = @('Ascii');
    }
);
Function Write-PesterItStatement {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [char]$Matching,
        
        [Parameter(Mandatory = $true)]
        [bool]$ShouldBe,
        
        [Parameter(Mandatory = $true)]
        [string]$Flags,
        
        [string]$Description,
        
        [Parameter(Mandatory = $true)]
        [System.IO.StreamWriter]$Writer,

        [switch]$IsNot
    )

    Begin {
        $Desc = $Description;
        if (-not $PSBoundParameters.ContainsKey('Description')) { $Desc = $Flags }
        $AllChars = [System.Collections.ObjectModel.Collection[char]]::new();
    }

    Process { $AllChars.Add($Matching) }

    End {
        switch ($AllChars.Count) {
            1 {
                $Writer.Write("        It '$(($AllChars[0] | ConvertTo-PSLiteral -ForceDoubleQuotes)) should return ");
                $Writer.Write($ShouldBe.ToString().ToLower());
                $Writer.WriteLine("' {");
                $Writer.Write('            $Actual = Test-CharacterClassFlags -Value ');
                $Writer.Write(($AllChars[0] | ConvertTo-PSLiteral));
                if ($IsNot.IsPresent) { $Writer.Write(' -IsNot') }
                $Writer.Write(' -Flags ');
                $Writer.Write($Flags);
                $Writer.WriteLine(' -ErrorAction Stop;');
                $Writer.Write('            $Actual | Should -Be');
                $Writer.Write($ShouldBe.ToString());
                $Writer.Write(' -Because ');
                $Writer.Write(($AllChars[0] | ConvertTo-PSLiteral -ForceDoubleQuotes));
                $Writer.WriteLine(';');
                $Writer.WriteLine('        }');
                break;
            }
            2 {
                $Writer.Write("        It '$(($AllChars[0] | ConvertTo-PSLiteral -ForceDoubleQuotes)) and $(($AllChars[1] | ConvertTo-PSLiteral -ForceDoubleQuotes)) should return ");
                $Writer.Write($ShouldBe.ToString().ToLower());
                $Writer.WriteLine("' {");
                $Writer.Write('            $Actual = Test-CharacterClassFlags -Value ');
                $Writer.Write(($AllChars[0] | ConvertTo-PSLiteral));
                if ($IsNot.IsPresent) { $Writer.Write(' -IsNot') }
                $Writer.Write(' -Flags ');
                $Writer.Write($Flags);
                $Writer.WriteLine(' -ErrorAction Stop;');
                $Writer.Write('            $Actual | Should -Be');
                $Writer.Write($ShouldBe.ToString());
                $Writer.Write(' -Because ');
                $Writer.Write(($AllChars[0] | ConvertTo-PSLiteral -ForceDoubleQuotes));
                $Writer.WriteLine(';');
                $Writer.Write('            $Actual = Test-CharacterClassFlags -Value ');
                $Writer.Write(($AllChars[1] | ConvertTo-PSLiteral));
                if ($IsNot.IsPresent) { $Writer.Write(' -IsNot') }
                $Writer.Write(' -Flags ');
                $Writer.Write($Flags);
                $Writer.WriteLine(' -ErrorAction Stop;');
                $Writer.Write('            $Actual | Should -Be');
                $Writer.Write($ShouldBe.ToString());
                $Writer.Write(' -Because ');
                $Writer.Write(($AllChars[1] | ConvertTo-PSLiteral -ForceDoubleQuotes));
                $Writer.WriteLine(';');
                $Writer.WriteLine('        }');
                break;
            }
            default {
                $Writer.Write("        It '$Desc characters should return ");
                $Writer.Write($ShouldBe.ToString().ToLower());
                $Writer.WriteLine("' {");
                $Writer.Write('            foreach ($c in @(([char[]](');
                $Writer.Write(($AllChars[0] | ConvertTo-PSLiteral));
                foreach ($c in ($AllChars | Select-Object -Skip 1)) {
                    $Writer.Write(', ');
                    $Writer.Write(($c | ConvertTo-PSLiteral));
                }
                $Writer.WriteLine(', "")))) {');
                $Writer.Write('                $Actual = Test-CharacterClassFlags -Value $c');
                if ($IsNot.IsPresent) { $Writer.Write(' -IsNot') }
                $Writer.Write(' -Flags ');
                $Writer.Write($Flags);
                $Writer.WriteLine(' -ErrorAction Stop;');
                $Writer.Write('                $Actual | Should -Be');
                $Writer.Write($ShouldBe.ToString());
                $Writer.Write(" -Because `"0x([int]`$c).ToString('x4')`";");
                $Writer.WriteLine('            }');
                $Writer.WriteLine('        }');
                break;
            }
        }
    }
}
Function Write-PesterDescribeStatement {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [string]$Name,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [long]$Value,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [ScriptBlock]$Test,
        
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [bool]$IsAscii,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [char[]]$Matching,
        
        [Parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [string[]]$Similar,
        
        [Parameter(Mandatory = $true)]
        [System.IO.StreamWriter]$Writer,

        [Parameter(Mandatory = $true)]
        [PSObject[]]$EnumMemberSpecs
    )
    
    Process {
        $Writer.WriteLine();
        $Writer.Write("Describe 'Test-CharacterClassFlags -Flags ");
        $Writer.Write($E.Name);
        $Writer.WriteLine("' {");
        $Writer.WriteLine("    Context 'IsNot.Present = `$false' {");
        Write-PesterItStatement -Matching $Matching -ShouldBe $true -Flags $Name -Writer $Writer;
        $AllChars = [System.Collections.ObjectModel[char]]::new();
        foreach ($c in $Matching) { $AllChars.Add($_) }
        foreach ($N in $Similar) {
            $E = $EnumMemberSpecs | Where-Object { $_.Name -eq $N } | Select-Object -First 1;
            foreach ($c in $E.Matching) { $AllChars.Add($_) }
            $Writer.WriteLine();
            Write-PesterItStatement -Matching $E.Matching -ShouldBe $false -Flags $Name -Description $E.Name -Writer $Writer;
        }
        [char[]]$Other = @($Script:CommonCharacters | Where-Object { $AllChars -cnotcontains $_ });
        $Writer.WriteLine();
        Write-PesterItStatement -Matching $Other -ShouldBe $false -Flags $Name -Description 'Other' -Writer $Writer;
        $Writer.WriteLine("    }");
        $Writer.WriteLine();
        $Writer.WriteLine("    Context 'IsNot.Present = `$true' {");
        Write-PesterItStatement -Matching $Matching -ShouldBe $false -Flags $Name -Writer $Writer;
        $AllChars = [System.Collections.ObjectModel[char]]::new();
        foreach ($c in $Matching) { $AllChars.Add($_) }
        foreach ($N in $Similar) {
            $E = $EnumMemberSpecs | Where-Object { $_.Name -eq $N } | Select-Object -First 1;
            foreach ($c in $E.Matching) { $AllChars.Add($_) }
            $Writer.WriteLine();
            Write-PesterItStatement -Matching $E.Matching -ShouldBe $true -Flags $Name -Description $E.Name -Writer $Writer;
        }
        [char[]]$Other = @($Script:CommonCharacters | Where-Object { $AllChars -cnotcontains $_ });
        $Writer.WriteLine();
        Write-PesterItStatement -Matching $Other -ShouldBe $true -Flags $Name -Description 'Other' -Writer $Writer;
        $Writer.WriteLine("    }");
        $Writer.WriteLine("}");
    }
}
#>
$Writer = [System.IO.StreamWriter]::new(($PSScriptRoot | Join-Path -ChildPath 'Test-CharacterClassFlags.tests.ps1'), $false, [System.Text.Encoding.Utf8Encoding]::new($false, $false));
try {
    $Writer.WriteLine("Import-Module -Name (`$PSScriptRoot | Join-Path -ChildPath './Erwine.Leonard.T.IOUtility.psd1') -ErrorAction Stop;");
    $Writer.WriteLine();
    $Writer.WriteLine('<#');
    $Writer.WriteLine('Import-Module Pester');
    $Writer.WriteLine('#>');
    foreach ($E in ($EnumMemberSpecs | Sort-Object -Property 'Value')) {
    }
    $Writer.Flush();
} finally {
    $Writer.Close();
}