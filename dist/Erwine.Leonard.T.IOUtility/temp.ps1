<#
#[System.Linq.Enumerable]::Range([int]0, [int]65536) | Group-Object -Property { Expression = { [char]::GetUnicodeCategory($_) } };
$Grouped = @([System.Linq.Enumerable]::Range([int]0, [int]65536) | ForEach-Object { [char]$_ } | Group-Object -Property @{
    Expression = {
        if ([char]::IsHighSurrogate($_)) { return 'HighSurrogate' }
        if ([char]::IsLowSurrogate($_)) { return 'LowSurrogate' }
        $C = $_;
        [System.Globalization.UnicodeCategory]$Category = [char]::GetUnicodeCategory($_);
        switch ($Category) {
            UppercaseLetter {
                if ([char]::IsAsciiHexDigitUpper($c)) { return 'HexLetterUpper' }
                if ([char]::IsAsciiLetterUpper($c)) { return 'NonHexAsciiLetterUpper'; }
                return 'NonAsciiLetterUpper';
            }
            LowercaseLetter {
                if ([char]::IsAsciiHexDigitLower($c)) { return 'HexLetterLower' }
                if ([char]::IsAsciiLetterLower($c)) { return 'NonHexAsciiLetterLower'; }
                return 'NonAsciiLetterLower';
            }
            DecimalDigitNumber {
                if ([char]::IsAsciiDigit($c)) {
                    if ($c -lt '1') { return 'BinaryDigitNumber'}
                    if ($c -gt '7') { return 'NonOctalAsciiDigit' }
                    return 'NonBinaryOctalDigit';
                }
                return 'NonAsciiDecimalDigit';
            }
            ConnectorPunctuation {
                if ([char]::IsAscii($c)) { return 'AsciiConnectorPunctuation' }
                return 'NonAsciiConnectorPunctuation'
            }
            DashPunctuation {
                if ([char]::IsAscii($c)) { return 'AsciiDashPunctuation' }
                return 'NonAsciiDashPunctuation'
            }
            OpenPunctuation {
                if ([char]::IsAscii($c)) { return 'AsciiOpenPunctuation' }
                return 'NonAsciiOpenPunctuation'
            }
            ClosePunctuation {
                if ([char]::IsAscii($c)) { return 'AsciiClosePunctuation' }
                return 'NonAsciiClosePunctuation'
            }
            OtherPunctuation {
                if ([char]::IsAscii($c)) { return 'OtherAsciiPunctuation' }
                return 'OtherNonAsciiPunctuation'
            }
            MathSymbol {
                if ([char]::IsAscii($c)) { return 'AsciiMathSymbol' }
                return 'NonAsciiMathSymbol'
            }
            CurrencySymbol {
                if ([char]::IsAscii($c)) { return 'AsciiCurrencySymbol' }
                return 'NonAsciiCurrencySymbol'
            }
            ModifierSymbol {
                if ([char]::IsAscii($c)) { return 'AsciiModifierSymbol' }
                return 'NonAsciiModifierSymbol'
            }
            SpaceSeparator {
                if ([char]::IsAscii($c)) { return 'AsciiSpaceSeparator' }
                return 'NonAsciiSpaceSeparator'
            }
            Control {
                if ([char]::IsAscii($c)) { return 'AsciiControl' }
                return 'NonAsciiControl'
            }

        }
        return $Category.ToString('F');
            # 'IsLetter', 'IsWhiteSpace', 'IsUpper', 'IsLower', 'IsPunctuation', 'IsLetterOrDigit', 'IsControl', 'IsNumber', 'IsSeparator', 'IsSurrogate', 'IsSymbol', '', ''
    }
});
$Grouped | ForEach-Object {
    $HasAscii = ([char]::IsAscii($_.Group[0]));
    $HasNonAscii = -not $HasAscii;
    if ($HasAscii) {
        foreach ($c in $_.Group) {
            if (-not [char]::IsAscii($c)) {
                $HasNonAscii = $true;
                break;
            }
        }
    } else {
        foreach ($c in $_.Group) {
            if ([char]::IsAscii($c)) {
                $HasAscii = $true;
                break;
            }
        }
    }
    if ($HasAscii) {
        if ($HasNonAscii) {
            Write-Warning -Message "$($_.Name) has both ASCII and non-ASCII";
        } else {
            Write-Information -MessageData "$($_.Name) is ASCII" -InformationAction Continue;
        }
    } else {
        Write-Information -MessageData "$($_.Name) is non-ASCII" -InformationAction Continue;
    }
}

$PrimaryFlagNames = @(
    'HexLetterUpper', 'HexLetterLower', 'NonHexAsciiLetterUpper', 'NonAsciiLetterUpper', 'NonHexAsciiLetterLower', 'NonAsciiLetterLower', 'TitlecaseLetter', 'ModifierLetter', 'OtherLetter',
    'BinaryDigitNumber', 'NonBinaryOctalDigit', 'NonOctalAsciiDigit', 'NonAsciiDecimalDigit', 'LetterNumber', 'OtherNumber',
    'NonSpacingMark', 'SpacingCombiningMark', 'EnclosingMark',
    'AsciiConnectorPunctuation', 'NonAsciiConnectorPunctuation', 'AsciiDashPunctuation', 'NonAsciiDashPunctuation', 'AsciiOpenPunctuation', 'NonAsciiOpenPunctuation', 'AsciiClosePunctuation', 'NonAsciiClosePunctuation', 'InitialQuotePunctuation', 'FinalQuotePunctuation', 'OtherAsciiPunctuation', 'OtherNonAsciiPunctuation',
    'AsciiMathSymbol', 'NonAsciiMathSymbol', 'AsciiCurrencySymbol', 'NonAsciiCurrencySymbol', 'AsciiModifierSymbol', 'NonAsciiModifierSymbol', 'OtherSymbol',
    'AsciiSpaceSeparator', 'NonAsciiSpaceSeparator', 'LineSeparator', 'ParagraphSeparator',
    'AsciiControl', 'NonAsciiControl', 
    'Format',
    'HighSurrogate', 'LowSurrogate',
    'PrivateUse',
    'OtherNotAssigned'
);
[long]$FlagValue = 0x01;
($PrimaryFlagNames | ForEach-Object {
    $Name = $_;
    $g = $Grouped | Where-Object { $_.Name -eq $Name } | Select-Object -First 1;
    if ($null -eq $g) {
        Write-Warning -Message "Cannnot find primar group $Name";
    } else {
        $Comment = 'ASCII';
        if (-not [char]::IsAscii($g.Group[0])) { $Comment = 'Non-ASCII'}
        [PSCustomObject]@{
            Name = $Name;
            Value = $FlagValue;
            Category = [char]::GetUnicodeCategory($g.Group[0]);
            Comment = $Comment;
        };
    }
    $FlagValue = $FlagValue -shl 1;
} | Sort-Object -Property 'Value') | ForEach-Object {
    @"
    # $($_.Comment)
    [PSCustomObject]@{
        Name = '$($_.Name)';
        Value = ([long]0x$($_.Value.ToString('x12')));
        Test = { [char]::GetUnicodeCategory(`$args[0]) -eq [System.Globalization.UnicodeCategory]::$($_.Category.ToString('F')) };
    },
"@
}
#>
#<#

# Format
# Surrogate
# IsSurrogate

# PrivateUse
# OtherNotAssigned



$EnumMemberInfo = @(
    # ASCII; [System.Globalization.UnicodeCategory]::UppercaseLetter
    [PSCustomObject]@{
        Name = 'HexLetterUpper';
        Value = ([long]0x000000000001);
        Test = { [char]::IsAsciiHexDigitUpper($args[0]) -and -not [char]::IsAsciiDigit($args[0]) };
    },
    # ASCII; [System.Globalization.UnicodeCategory]::LowercaseLetter
    [PSCustomObject]@{
        Name = 'HexLetterLower';
        Value = ([long]0x000000000002);
        Test = { [char]::IsAsciiHexDigitLower($args[0]) -and -not [char]::IsAsciiDigit($args[0]) };
    },
    # ASCII; [System.Globalization.UnicodeCategory]::UppercaseLetter
    [PSCustomObject]@{
        Name = 'NonHexAsciiLetterUpper';
        Value = ([long]0x000000000004);
        Test = { [char]::IsAsciiLetterUpper($args[0]) -and -not [char]::IsAsciiHexDigitUpper($args[0]) };
    },
    # Non-ASCII; [System.Globalization.UnicodeCategory]::UppercaseLetter
    [PSCustomObject]@{
        Name = 'NonAsciiLetterUpper';
        Value = ([long]0x000000000008);
        Test = { [char]::IsUpper($args[0]) -and -not [char]::IsAsciiLetterUpper($args[0]) };
    },
    # ASCII; [System.Globalization.UnicodeCategory]::LowercaseLetter
    [PSCustomObject]@{
        Name = 'NonHexAsciiLetterLower';
        Value = ([long]0x000000000010);
        Test = { [char]::IsAsciiLetterLower($args[0]) -and -not [char]::IsAsciiHexDigitLower($args[0]) };
    },
    # Non-ASCII; [System.Globalization.UnicodeCategory]::LowercaseLetter
    [PSCustomObject]@{
        Name = 'NonAsciiLetterLower';
        Value = ([long]0x000000000020);
        Test = { [char]::IsLower($args[0]) -and -not [char]::IsAsciiLetterLower($args[0]) };
    },
    # ASCII; [System.Globalization.UnicodeCategory]::UppercaseLetter
    [PSCustomObject]@{
        Name = 'AsciiLetterUpper';
        Combines = ([string[]]@('HexLetterUpper', 'NonHexAsciiLetterUpper'));
        Test = { [char]::IsAsciiLetterUpper($args[0]) };
    },
    # ASCII; [System.Globalization.UnicodeCategory]::LowercaseLetter
    [PSCustomObject]@{
        Name = 'AsciiLetterLower';
        Combines = ([string[]]@('HexLetterLower', 'NonHexAsciiLetterLower'));
        Test = { [char]::IsAsciiLetterLower($args[0]) };
    },
    # ASCII
    [PSCustomObject]@{
        Name = 'AsciiLetter';
        Combines = ([string[]]@('AsciiLetterUpper', 'AsciiLetterLower'));
        Test = { [char]::IsAsciiLetter($args[0]) };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'TitlecaseLetter';
        Value = ([long]0x000000000040);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::TitlecaseLetter };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'ModifierLetter';
        Value = ([long]0x000000000080);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::ModifierLetter };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'OtherLetter';
        Value = ([long]0x000000000100);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::OtherLetter };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'NonAsciiLetter';
        Combines = ([string[]]@('NonAsciiLetterUpper', 'NonHexAsciiLetterLower', 'TitlecaseLetter', 'ModifierLetter', 'OtherLetter'));
        Test = { [char]::IsLetter($args[0]) -and -not [char]::IsAsciiLetter($args[0]) };
    },
    # Both
    [PSCustomObject]@{
        Name = 'UppercaseLetter';
        Combines = ([string[]]@('AsciiLetterUpper', 'NonAsciiLetterUpper'));
        Test = { [char]::IsUpper($args[0]) };
    },
    # Both
    [PSCustomObject]@{
        Name = 'LowercaseLetter';
        Combines = ([string[]]@('AsciiLetterLower', 'NonAsciiLetterLower'));
        Test = { [char]::IsLower($args[0]) };
    },
    # Both
    [PSCustomObject]@{
        Name = 'Letter';
        Combines = ([string[]]@('AsciiLetter', 'NonAsciiLetter'));
        Test = { [char]::IsLetter($args[0]) };
    },
 
    # ASCII; [System.Globalization.UnicodeCategory]::DecimalDigitNumber
    [PSCustomObject]@{
        Name = 'BinaryDigitNumber';
        Value = ([long]0x000000000200);
        Test = { $args[0] -eq '0' -or $args[0] -eq '1' };
    },
    # ASCII; [System.Globalization.UnicodeCategory]::DecimalDigitNumber
    [PSCustomObject]@{
        Name = 'NonBinaryOctalDigit';
        Value = ([long]0x000000000400);
        Test = { $args[0] -gt '1' -and $args[0] -le '7' };
    },
    # ASCII; [System.Globalization.UnicodeCategory]::DecimalDigitNumber
    [PSCustomObject]@{
        Name = 'OctalDigit';
        Combines = ([string[]]@('BinaryDigitNumber', 'NonBinaryOctalDigit'));
        Test = { $args[0] -ge '0' -and $args[0] -le '7' };
    },
    # ASCII; [System.Globalization.UnicodeCategory]::DecimalDigitNumber
    [PSCustomObject]@{
        Name = 'NonOctalAsciiDigit';
        Value = ([long]0x000000000800);
        Test = { $args[0] -eq '8' -or $args[0] -eq '9' };
    },
    # ASCII; [System.Globalization.UnicodeCategory]::DecimalDigitNumber
    [PSCustomObject]@{
        Name = 'AsciiDigit';
        Combines = ([string[]]@('OctalDigit', 'NonOctalAsciiDigit'));
        Test = { [char]::IsAsciiDigit($args[0]) };
    },
    # Non-ASCII; [System.Globalization.UnicodeCategory]::DecimalDigitNumber
    [PSCustomObject]@{
        Name = 'NonAsciiDecimalDigit';
        Value = ([long]0x000000001000);
        Test = { [char]::IsDigit($args[0]) -and -not [char]::IsAsciiDigit($args[0]) };
        AltTest = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::DecimalDigitNumber -and -not [char]::IsAsciiDigit($args[0]) };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'LetterNumber';
        Value = ([long]0x000000002000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::LetterNumber };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'OtherNumber';
        Value = ([long]0x000000004000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::OtherNumber };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'OtheNonAsciiNumber';
        Combines = ([string[]]@('AsciiDigit', 'NonAsciiDecimalDigit', 'LetterNumber', 'OtherNumber'));
        AltTest = { [char]::IsNumber($args[0]) -and -not [char]::IsAsciiDigit($args[0]) };
    },
    # Both; [System.Globalization.UnicodeCategory]::DecimalDigitNumber
    [PSCustomObject]@{
        Name = 'Digit';
        Combines = ([string[]]@('AsciiDigit', 'OtheNonAsciiNumber'));
        Test = { [char]::IsDigit($args[0]) };
    },
    # Both
    [PSCustomObject]@{
        Name = 'Number';
        Combines = ([string[]]@('AsciiDigit', 'NonAsciiDecimalDigit'));
        Test = { [char]::IsNumber($args[0]) };
    },
    
    # ASCII
    [PSCustomObject]@{
        Name = 'AsciiHexDigitUpper';
        Combines = ([string[]]@('AsciiDigit', 'HexLetterUpper'));
        Test = { [char]::IsAsciiHexDigitUpper($args[0]) };
    },
    
    # ASCII
    [PSCustomObject]@{
        Name = 'AsciiHexDigitLower';
        Combines = ([string[]]@('AsciiDigit', 'HexLetterLower'));
        Test = { [char]::IsAsciiHexDigitLower($args[0]) };
    },
    
    # ASCII
    [PSCustomObject]@{
        Name = 'AsciiHexDigit';
        Combines = ([string[]]@('AsciiDigit', 'HexLetterUpper', 'HexLetterLower'));
        Test = { [char]::IsAsciiHexDigit($args[0]) };
    },

    # ASCII
    [PSCustomObject]@{
        Name = 'AsciiLetterOrDigit';
        Combines = ([string[]]@('AsciiLetter', 'AsciiDigit'));
        Test = { [char]::IsAsciiLetterOrDigit($args[0]) };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'NonAsciiLetterOrDigit';
        Combines = ([string[]]@('NonAsciiLetter', 'NonAsciiDecimalDigit'));
        Test = { [char]::IsLetterOrDigit($args[0]) -and -not [char]::IsAsciiLetterOrDigit($args[0]) };
    },

    # Both
    [PSCustomObject]@{
        Name = 'LetterOrDigit';
        Combines = ([string[]]@('AsciiLetterOrDigit', 'NonAsciiLetterOrDigit'));
        Test = { [char]::IsLetterOrDigit($args[0]) };
    },

    # Non-ASCII
    [PSCustomObject]@{
        Name = 'NonSpacingMark';
        Value = ([long]0x000000008000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::NonSpacingMark };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'SpacingCombiningMark';
        Value = ([long]0x000000010000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::SpacingCombiningMark };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'EnclosingMark';
        Value = ([long]0x000000020000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::EnclosingMark };
    },

    # Non-ASCII
    [PSCustomObject]@{
        Name = 'Mark';
        Combines = ([string[]]@('NonSpacingMark', 'SpacingCombiningMark', 'EnclosingMark'));
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation };
    },

    # ASCII
    [PSCustomObject]@{
        Name = 'AsciiConnectorPunctuation';
        Value = ([long]0x000000040000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation -and [char]::IsAscii($args[0]) };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'NonAsciiConnectorPunctuation';
        Value = ([long]0x000000080000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation -and -not [char]::IsAscii($args[0]) };
    },
    # Both; [System.Globalization.UnicodeCategory]::ConnectorPunctuation
    [PSCustomObject]@{
        Name = 'ConnectorPunctuation';
        Combines = ([string[]]@('AsciiConnectorPunctuation', 'NonAsciiConnectorPunctuation'));
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation };
    },
    # ASCII
    [PSCustomObject]@{
        Name = 'AsciiDashPunctuation';
        Value = ([long]0x000000100000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::DashPunctuation -and [char]::IsAscii($args[0]) };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'NonAsciiDashPunctuation';
        Value = ([long]0x000000200000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::DashPunctuation -and -not [char]::IsAscii($args[0]) };
    },
    # Both; [System.Globalization.UnicodeCategory]::DashPunctuation
    [PSCustomObject]@{
        Name = 'DashPunctuation';
        Combines = ([string[]]@('AsciiDashPunctuation', 'NonAsciiDashPunctuation'));
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::DashPunctuation };
    },
    # ASCII
    [PSCustomObject]@{
        Name = 'AsciiOpenPunctuation';
        Value = ([long]0x000000400000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation -and [char]::IsAscii($args[0]) };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'NonAsciiOpenPunctuation';
        Value = ([long]0x000000800000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation -and -not [char]::IsAscii($args[0]) };
    },
    # Both; [System.Globalization.UnicodeCategory]::OpenPunctuation
    [PSCustomObject]@{
        Name = 'OpenPunctuation';
        Combines = ([string[]]@('AsciiOpenPunctuation', 'NonAsciiOpenPunctuation'));
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation };
    },
    # ASCII
    [PSCustomObject]@{
        Name = 'AsciiClosePunctuation';
        Value = ([long]0x000001000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation -and [char]::IsAscii($args[0]) };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'NonAsciiClosePunctuation';
        Value = ([long]0x000002000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation -and -not [char]::IsAscii($args[0]) };
    },
    # Both; [System.Globalization.UnicodeCategory]::ClosePunctuation
    [PSCustomObject]@{
        Name = 'ClosePunctuation';
        Combines = ([string[]]@('AsciiClosePunctuation', 'NonAsciiClosePunctuation'));
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'InitialQuotePunctuation';
        Value = ([long]0x000004000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::InitialQuotePunctuation };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'FinalQuotePunctuation';
        Value = ([long]0x000008000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::FinalQuotePunctuation };
    },
    # ASCII
    [PSCustomObject]@{
        Name = 'OtherAsciiPunctuation';
        Value = ([long]0x000010000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation -and [char]::IsAscii($args[0]) };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'OtherNonAsciiPunctuation';
        Value = ([long]0x000020000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation -and -not [char]::IsAscii($args[0]) };
    },
    # Both; [System.Globalization.UnicodeCategory]::OtherPunctuation
    [PSCustomObject]@{
        Name = 'OtherPunctuation';
        Combines = ([string[]]@('OtherAsciiPunctuation', 'NonAsciiClosePunctuation'));
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation };
    },

    # ASCII
    [PSCustomObject]@{
        Name = 'AsciiPunctuation';
        Combines = ([string[]]@('AsciiConnectorPunctuation', 'AsciiDashPunctuation', 'AsciiOpenPunctuation', 'AsciiClosePunctuation', 'OtherAsciiPunctuation'));
        Test = { [char]::IsPunctuation($args[0]) -and [char]::IsAscii($args[0]) };
    },

    # Non-ASCII
    [PSCustomObject]@{
        Name = 'NonAsciiPunctuation';
        Combines = ([string[]]@('NonAsciiConnectorPunctuation', 'NonAsciiDashPunctuation', 'NonAsciiOpenPunctuation', 'NonAsciiClosePunctuation', 'InitialQuotePunctuation', 'FinalQuotePunctuation', 'OtherNonAsciiPunctuation'));
        Test = { [char]::IsPunctuation($args[0]) -and -not [char]::IsAscii($args[0]) };
    },

    # Both
    [PSCustomObject]@{
        Name = 'AsciiPunctuation';
        Combines = ([string[]]@('AsciiPunctuation', 'NonAsciiPunctuation'));
        Test = { [char]::IsPunctuation($args[0]) };
    },

    # ASCII; [System.Globalization.UnicodeCategory]::MathSymbol
    [PSCustomObject]@{
        Name = 'AsciiMathSymbol';
        Value = ([long]0x000040000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::MathSymbol -and [char]::IsAscii($args[0]) };
    },
    # Non-ASCII; [System.Globalization.UnicodeCategory]::MathSymbol
    [PSCustomObject]@{
        Name = 'NonAsciiMathSymbol';
        Value = ([long]0x000080000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::MathSymbol -and -not [char]::IsAscii($args[0]) };
    },
    
    # Both; [System.Globalization.UnicodeCategory]::MathSymbol
    [PSCustomObject]@{
        Name = 'MathSymbol';
        Combines = ([string[]]@('AsciiMathSymbol', 'NonAsciiMathSymbol'));
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::MathSymbol };
    },
    # ASCII; [System.Globalization.UnicodeCategory]::CurrencySymbol
    [PSCustomObject]@{
        Name = 'AsciiCurrencySymbol';
        Value = ([long]0x000100000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol -and [char]::IsAscii($args[0]) };
    },
    # Non-ASCII; [System.Globalization.UnicodeCategory]::CurrencySymbol
    [PSCustomObject]@{
        Name = 'NonAsciiCurrencySymbol';
        Value = ([long]0x000200000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol -and -not [char]::IsAscii($args[0]) };
    },
    # Both; [System.Globalization.UnicodeCategory]::MathSymbol
    [PSCustomObject]@{
        Name = 'CurrencySymbol';
        Combines = ([string[]]@('AsciiCurrencySymbol', 'NonAsciiCurrencySymbol'));
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol };
    },

    # ASCII; [System.Globalization.UnicodeCategory]::ModifierSymbol
    [PSCustomObject]@{
        Name = 'AsciiModifierSymbol';
        Value = ([long]0x000400000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol -and [char]::IsAscii($args[0]) };
    },
    # Non-ASCII; [System.Globalization.UnicodeCategory]::ModifierSymbol
    [PSCustomObject]@{
        Name = 'NonAsciiModifierSymbol';
        Value = ([long]0x000800000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol -and -not [char]::IsAscii($args[0]) };
    },
    # Both; [System.Globalization.UnicodeCategory]::ModifierSymbol
    [PSCustomObject]@{
        Name = 'ModifierSymbol';
        Combines = ([string[]]@('AsciiModifierSymbol', 'NonAsciiModifierSymbol'));
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol };
    },

    # Non-ASCII
    [PSCustomObject]@{
        Name = 'OtherSymbol';
        Value = ([long]0x001000000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::OtherSymbol };
    },
    # ASCII
    [PSCustomObject]@{
        Name = 'AsciiSymbol';
        Combines = ([string[]]@('AsciiMathSymbol', 'AsciiCurrencySymbol', 'AsciiModifierSymbol'));
        Test = { [char]::IsSymbol($args[0]) -and [char]::IsAscii($args[0]) };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'NonAsciiSymbol';
        Combines = ([string[]]@('NonAsciiMathSymbol', 'NonAsciiCurrencySymbol', 'NonAsciiModifierSymbol', 'OtherSymbol'));
        Test = { [char]::IsSymbol($args[0]) -and -not [char]::IsAscii($args[0]) };
    },
    # Both
    [PSCustomObject]@{
        Name = 'Symbol';
        Combines = ([string[]]@('AsciiSymbol', 'NonAsciiSymbol'));
        Test = { [char]::IsSymbol($args[0]) };
    },
    
    # ASCII; [System.Globalization.UnicodeCategory]::SpaceSeparator
    [PSCustomObject]@{
        Name = 'AsciiSpaceSeparator';
        Value = ([long]0x002000000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator };
    },
    # Non-ASCII; [System.Globalization.UnicodeCategory]::SpaceSeparator
    [PSCustomObject]@{
        Name = 'NonAsciiSpaceSeparator';
        Value = ([long]0x004000000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator };
    },
    # Both; [System.Globalization.UnicodeCategory]::SpaceSeparator
    [PSCustomObject]@{
        Name = 'SpaceSeparator';
        Combines = ([string[]]@('AsciiSpaceSeparator', 'NonAsciiSpaceSeparator'));
        Test = { [char]::IsAsciiLetterUpper($args[0]) };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'LineSeparator';
        Value = ([long]0x008000000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::LineSeparator };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'ParagraphSeparator';
        Value = ([long]0x010000000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::ParagraphSeparator };
    },
    # Both
    [PSCustomObject]@{
        Name = 'Separator';
        Combines = ([string[]]@('SpaceSeparator', 'LineSeparator', 'ParagraphSeparator'));
        Test = { [char]::IsSeparator($args[0]) };
    },

    # ASCII; [System.Globalization.UnicodeCategory]::Control
    [PSCustomObject]@{
        Name = 'AsciiControl';
        Value = ([long]0x020000000000);
        Test = { [char]::IsControl($args[0]) -and [char]::IsAscii($args[0]) };
    },
    # Non-ASCII; [System.Globalization.UnicodeCategory]::Control
    [PSCustomObject]@{
        Name = 'NonAsciiControl';
        Value = ([long]0x040000000000);
        Test = { [char]::IsControl($args[0]) -and -not [char]::IsAscii($args[0]) };
    },
    # Both; [System.Globalization.UnicodeCategory]::Control
    [PSCustomObject]@{
        Name = 'Control';
        Combines = ([string[]]@('AsciiControl', 'NonAsciiControl'));
        Test = { [char]::IsControl($args[0]) };
    },
    # Both
    [PSCustomObject]@{
        Name = 'AsciiWhiteSpace';
        Combines = ([string[]]@('AsciiSpaceSeparator', 'AsciiControl'));
        Test = { [char]::IsWhiteSpace($args[0]) -and [char]::IsAscii($args[0]) };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'NonAsciiWhiteSpace';
        Combines = ([string[]]@('AsciiControl', 'NonAsciiControl'));
        Test = { [char]::IsWhiteSpace($args[0]) -and -not [char]::IsAscii($args[0]) };
    },
    # Both
    [PSCustomObject]@{
        Name = 'WhiteSpace';
        Combines = ([string[]]@('AsciiWhiteSpace', 'NonAsciiWhiteSpace'));
        Test = { [char]::IsWhiteSpace($args[0]) };
    },

    # ASCII
    [PSCustomObject]@{
        Name = 'Ascii';
        Combines = ([string[]]@('AsciiLetterOrDigit', 'AsciiPunctuation', 'AsciiSymbol', 'AsciiWhiteSpace'));
        Test = { [char]::IsAscii($args[0]) };
    },
   
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'Format';
        Value = ([long]0x080000000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::Format };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'HighSurrogate';
        Value = ([long]0x100000000000);
        Test = { [char]::IsHighSurrogate($args[0]) };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'LowSurrogate';
        Value = ([long]0x200000000000);
        Test = { [char]::IsLowSurrogate($args[0]) };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'Surrogate';
        Combines = ([string[]]@('HighSurrogate', 'LowSurrogate'));
        Test = { [char]::IsSurrogate($args[0]) };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'PrivateUse';
        Value = ([long]0x400000000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::PrivateUse };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'OtherNotAssigned';
        Value = ([long]0x800000000000);
        Test = { [char]::GetUnicodeCategory($args[0]) -eq [System.Globalization.UnicodeCategory]::OtherNotAssigned };
    },
    # Non-ASCII
    [PSCustomObject]@{
        Name = 'NonAscii';
        Combines = ([string[]]@('NonAsciiLetterOrDigit', 'Mark', 'NonAsciiPunctuation', 'NonAsciiSymbol', 'NonAsciiWhiteSpace', 'Format', 'Surrogate', 'PrivateUse', 'OtherNotAssigned'));
        Test = { [char]::IsAscii($args[0]) };
    }
);
$EnumDict = $EnumMemberInfo | Group-Object -Property 'Name' -AsHashTable;
$HasNoValue = @($EnumMemberInfo | Where-Object { $null -eq $_.Value });
while ($HasNoValue.Count -gt 0) {
    foreach ($E in $HasNoValue) {
        [long]$f = 0;
        $GotAllFlags = $true;
        foreach ($n in $E.Combines) {
            $v = $EnumDict[$n];
            if ($null -eq $v.Value) {
                $GotAllFlags = $false;
                break;
            }
            [long]$f = $f -bor $v.Value;
        }
        if ($GotAllFlags) {
            $E | Add-Member -MemberType NoteProperty -Name 'Value' -Value $f;
        }
    }
    $HasNoValue = @($EnumMemberInfo | Where-Object { $null -eq $_.Value });
}
foreach ($E in ($EnumMemberInfo | Sort-Object -Property 'Value')) {
    
}
#>