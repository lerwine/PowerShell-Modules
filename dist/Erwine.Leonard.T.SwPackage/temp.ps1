[char[]]$Script:TestChars = @(("`t", "`n", ' ', '$', '+', '0', '2', '7', '9', 'A', '^', 'z', '|', '£', '¦', '±', 'µ', '¼', 'À', 'æ', 'ǂ', '˅', '„', 'ˮ', 'Ⅵ', '(', ')', '-', '_', '¡', '₎', '«', '־', '»', '‿') | Sort-Object);

Function CharsToDisplay([char[]]$Values) {
    $Code = @($Values | Sort-Object | ForEach-Object { if ($_ -eq "`n") { '"`n"' } else { if ($_ -eq "`t") { '"`t"' } else { "`"$_`"" } } });
    switch ($Code.Count) {
        1 { return $Code[0] }
        2 { return "$($Code[0]) and $($Code[0])" }
    }
    return return "$(($Code | Select-Object -SkipLast 1) -join ', '), and $($Code[-1])";
}
Function CharsToArrayCode([char[]]$Values) {
    return (($Values | Sort-Object | ForEach-Object { if ($_ -eq "`n") { '"`n"' } else { if ($_ -eq "`t") { '"`t"' } else { "'$_'" } } }) -join ', ');
}

[char].GetMethod('Ascii')
Function Get-TestCode {
    [CmdletBinding(DefaultParameterSetName = 'MethodOnly')]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$Flags,

        [string]$AsciiFlags,

        [string]$NonAsciiFlags,

        [Parameter(Mandatory = $true)]
        [string]$SimilarDisplay,

        [Parameter(Mandatory = $true, ParameterSetName = 'Category')]
        [System.Globalization.UnicodeCategory]$Category,

        [Parameter(Mandatory = $true, ParameterSetName = 'MethodOnly')]
        [Parameter(ParameterSetName = 'Category')]
        [ValidateSet('IsAscii', 'IsAsciiLetter', 'IsAsciiLetterLower', 'IsAsciiLetterUpper', 'IsAsciiDigit', 'IsAsciiLetterOrDigit', 'IsAsciiHexDigit', 'IsAsciiHexDigitUpper', 'IsAsciiHexDigitLower', 'IsDigit',
            'IsLetter', 'IsWhiteSpace', 'IsUpper', 'IsLower', 'IsPunctuation', 'IsLetterOrDigit', 'IsControl', 'IsNumber', 'IsSeparator', 'IsSurrogate', 'IsSymbol', 'IsHighSurrogate', 'IsLowSurrogate')]
        [string]$Method
    )

    [char[]]$MatchingAll = @();
    [char[]]$MatchingAscii = @();
    [char[]]$MatchingNonAscii = @();
    [char[]]$Similar = @();
    [char[]]$Other = @();
    [System.Reflection.MethodInfo]$MethodInfo = $null;
    if ($PSBoundParameters.ContainsKey('Category')) {
        $MatchingAll = @($Script:TestChars | Where-Object { [char]::GetUnicodeCategory($_) -eq $Category });
        if ($PSBoundParameters.ContainsKey('Method')) {
            $MethodInfo = [char].GetMethod($Method, ([Type[]]@([char])));
            $Similar = @($Script:TestChars | Where-Object { $MethodInfo.Invoke($null, $_) -and [char]::GetUnicodeCategory($_) -ne $Category });
            $Other = @($Script:TestChars | Where-Object { -not $MethodInfo.Invoke($null, $_) });
        } else {
            $Other = @($Script:TestChars | Where-Object { [char]::GetUnicodeCategory($_) -ne $Category });
        }
    } else {
        $MethodInfo = [char].GetMethod($Method, ([Type[]]@([char])));
        $MatchingAll = @($Script:TestChars | Where-Object { $MethodInfo.Invoke($null, $_) });
        $Other = @($Script:TestChars | Where-Object { -not $MethodInfo.Invoke($null, $_) });
    }
    if ($MatchingAll.Count -eq 0) {
        Write-Warning -Message "No matching characters found";
    } else {
        @"

Describe 'Test-CharacterClassFlags -Flags $Flags' {
    Context 'IsNot.Present = `$false' {
"@ | Write-Output;
        if ($MatchingAll.Length -eq 1) {
                @"
        It '$((CharsToDisplay $MatchingAll).Replace("'", '"')) should return true' {
            `$Actual = Test-CharacterClassFlags -Value $(CharsToArrayCode $MatchingAll) -Flags $Flags -ErrorAction Stop;
            `$Actual | Should -BeTrue -Because '$((CharsToDisplay $MatchingAll).Replace("'", '"'))';
        }
"@ | Write-Output;
        } else {
            if ($MatchingAll.Length -lt 4) {
                    @"
        It '$((CharsToDisplay $MatchingAll).Replace("'", '"')) should return true' {
            foreach (`$Value in ($(CharsToArrayCode $MatchingAll))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $Flags -ErrorAction Stop;
                `$Actual | Should -BeTrue -Because ""`$Value"";
            }
        }
"@ | Write-Output;
            } else {
                    @"
        It '$Flags characters should return true' {
            foreach (`$Value in ($(CharsToArrayCode $MatchingAll))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $Flags -ErrorAction Stop;
                `$Actual | Should -BeTrue -Because ""`$Value"";
            }
        }
"@ | Write-Output;
            }
        }
        if ($Similar.Length -gt 0) {
        @"

        It 'Other $SimilarDisplay characters should return false' {
            foreach (`$Value in ($(CharsToArrayCode $Similar))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $Flags -ErrorAction Stop;
                `$Actual | Should -BeFalse -Because ""`$Value"";
            }
        }
"@
        }
    @"

        It 'Non-$SimilarDisplay characters should return false' {
            foreach (`$Value in ($(CharsToArrayCode $Other))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $Flags -ErrorAction Stop;
                `$Actual | Should -BeFalse -Because ""`$Value"";
            }
        }
    }

    Context 'IsNot.Present = `$true' {
"@
        if ($MatchingAll.Length -eq 1) {
                @"
        It '$((CharsToDisplay $MatchingAll).Replace("'", '"')) should return true' {
            `$Actual = Test-CharacterClassFlags -Value $(CharsToArrayCode $MatchingAll) -IsNot -Flags $Flags -ErrorAction Stop;
            `$Actual | Should -BeFalse -Because '$((CharsToDisplay $MatchingAll).Replace("'", '"'))';
        }
"@ | Write-Output;
        } else {
            if ($MatchingAll.Length -lt 4) {
                    @"
        It '$((CharsToDisplay $MatchingAll).Replace("'", '"')) should return true' {
            foreach (`$Value in ($(CharsToArrayCode $MatchingAll))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -IsNot -Flags $Flags -ErrorAction Stop;
                `$Actual | Should -BeFalse -Because ""`$Value"";
            }
        }
"@ | Write-Output;
            } else {
                    @"
        It '$Flags characters should return false' {
            foreach (`$Value in ($(CharsToArrayCode $MatchingAll))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -IsNot -Flags $Flags -ErrorAction Stop;
                `$Actual | Should -BeFalse -Because ""`$Value"";
            }
        }
"@ | Write-Output;
            }
        }
        if ($Similar.Length -gt 0) {
        @"

        It 'Other $SimilarDisplay characters should return true' {
            foreach (`$Value in ($(CharsToArrayCode $Similar))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -IsNot -Flags $Flags -ErrorAction Stop;
                `$Actual | Should -BeTrue -Because ""`$Value"";
            }
        }
"@
        }
    @"

        It 'Non-$SimilarDisplay characters should return true' {
            foreach (`$Value in ($(CharsToArrayCode $Other))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -IsNot -Flags $Flags -ErrorAction Stop;
                `$Actual | Should -BeTrue -Because ""`$Value"";
            }
        }
    }
}
"@
        [char[]]$MatchingAscii = @();
        [char[]]$MatchingNonAscii = @();
        if ($PSBoundParameters.ContainsKey('AsciiFlags') -and $PSBoundParameters.ContainsKey('NonAsciiFlags')) {
            [char[]]$MatchingAscii = @($MatchingAll | Where-Object { [char]::IsAscii($_) });
            [char[]]$MatchingNonAscii = @($MatchingAll | Where-Object { -not [char]::IsAscii($_) });
        }
        if ($MatchingAscii.Length -gt 0 -and $MatchingNonAscii.Length -gt 0) {
            @"

Describe 'Test-CharacterClassFlags -Flags $AsciiFlags' {
    Context 'IsNot.Present = `$false' {
"@
            if ($MatchingAscii.Length -eq 1) {
                @"

        It '$((CharsToDisplay $MatchingAscii).Replace("'", '"')) should return true' {
            `$Actual = Test-CharacterClassFlags -Value $(CharsToArrayCode $MatchingAscii) -Flags $AsciiFlags -ErrorAction Stop;
            `$Actual | Should -BeTrue -Because '$((CharsToDisplay $MatchingAscii).Replace("'", '"'))';
        }
"@
            } else {
                if ($MatchingAscii.Length -lt 4) {
                    @"

        It '$((CharsToDisplay $MatchingAscii).Replace("'", '"')) should return true' {
            foreach (`$Value in ($(CharsToArrayCode $MatchingAscii))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $AsciiFlags -ErrorAction Stop;
                `$Actual | Should -BeTrue -Because ""`$Value"";
            }
        }
"@
                } else {
                    @"
        It '$AsciiFlags characters should return true' {
            foreach (`$Value in ($(CharsToArrayCode $MatchingAscii))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $AsciiFlags -ErrorAction Stop;
                `$Actual | Should -BeTrue -Because ""`$Value"";
            }
        }
"@
                }
            }
            @"

        It 'Other $SimilarDisplay characters should return false' {
            foreach (`$Value in ($(CharsToArrayCode @($Similar + $MatchingNonAscii)))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $AsciiFlags -ErrorAction Stop;
                `$Actual | Should -BeFalse -Because ""`$Value"";
            }
        }

        It 'Non-$SimilarDisplay characters should return false' {
            foreach (`$Value in ($(CharsToArrayCode $Other))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $AsciiFlags -ErrorAction Stop;
                `$Actual | Should -BeFalse -Because ""`$Value"";
            }
        }
    }

    Context 'IsNot.Present = `$true' {
"@
            if ($MatchingAscii.Length -eq 1) {
                @"

        It '$((CharsToDisplay $MatchingAscii).Replace("'", '"')) should return false' {
            `$Actual = Test-CharacterClassFlags -Value $(CharsToArrayCode $MatchingAscii) -Flags $AsciiFlags -ErrorAction Stop;
            `$Actual | Should -BeFalse -Because '$((CharsToDisplay $MatchingAscii).Replace("'", '"'))';
        }
"@
            } else {
                if ($MatchingAscii.Length -lt 4) {
                    @"

        It '$((CharsToDisplay $MatchingAscii).Replace("'", '"')) should return false' {
            foreach (`$Value in ($(CharsToArrayCode $MatchingAscii))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $AsciiFlags -ErrorAction Stop;
                `$Actual | Should -BeFalse -Because ""`$Value"";
            }
        }
"@
                } else {
                    @"
        It '$AsciiFlags characters should return false' {
            foreach (`$Value in ($(CharsToArrayCode $MatchingAscii))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $AsciiFlags -ErrorAction Stop;
                `$Actual | Should -BeFalse -Because ""`$Value"";
            }
        }
"@
                }
            }
            @"

        It 'Other $SimilarDisplay characters should return true' {
            foreach (`$Value in ($(CharsToArrayCode @($Similar + $MatchingNonAscii)))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $AsciiFlags -ErrorAction Stop;
                `$Actual | Should -BeTrue -Because ""`$Value"";
            }
        }

        It 'Non-$SimilarDisplay characters should return true' {
            foreach (`$Value in ($(CharsToArrayCode $Other))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $AsciiFlags -ErrorAction Stop;
                `$Actual | Should -BeTrue -Because ""`$Value"";
            }
        }
    }
}
"@

@"

Describe 'Test-CharacterClassFlags -Flags $NonAsciiFlags' {
    Context 'IsNot.Present = `$false' {
"@
            if ($MatchingNonAscii.Length -eq 1) {
                @"

        It '$((CharsToDisplay $MatchingNonAscii).Replace("'", '"')) should return true' {
            `$Actual = Test-CharacterClassFlags -Value $(CharsToArrayCode $MatchingNonAscii) -Flags $NonAsciiFlags -ErrorAction Stop;
            `$Actual | Should -BeTrue -Because '$((CharsToDisplay $MatchingNonAscii).Replace("'", '"'))';
        }
"@
            } else {
                if ($MatchingAscii.Length -lt 4) {
                    @"

        It '$((CharsToDisplay $MatchingNonAscii).Replace("'", '"')) should return true' {
            foreach (`$Value in ($(CharsToArrayCode $MatchingNonAscii))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $NonAsciiFlags -ErrorAction Stop;
                `$Actual | Should -BeTrue -Because ""`$Value"";
            }
        }
"@
                } else {
                    @"
        It '$NonAsciiFlags characters should return true' {
            foreach (`$Value in ($(CharsToArrayCode $MatchingNonAscii))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $NonAsciiFlags -ErrorAction Stop;
                `$Actual | Should -BeTrue -Because ""`$Value"";
            }
        }
"@
                }
            }
            @"

        It 'Other $SimilarDisplay characters should return false' {
            foreach (`$Value in ($(CharsToArrayCode @($Similar + $MatchingAscii)))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $NonAsciiFlags -ErrorAction Stop;
                `$Actual | Should -BeFalse -Because ""`$Value"";
            }
        }

        It 'Non-$SimilarDisplay characters should return false' {
            foreach (`$Value in ($(CharsToArrayCode $Other))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $NonAsciiFlags -ErrorAction Stop;
                `$Actual | Should -BeFalse -Because ""`$Value"";
            }
        }
    }

    Context 'IsNot.Present = `$true' {
"@
            if ($MatchingNonAscii.Length -eq 1) {
                @"

        It '$((CharsToDisplay $MatchingNonAscii).Replace("'", '"')) should return false' {
            `$Actual = Test-CharacterClassFlags -Value $(CharsToArrayCode $MatchingNonAscii) -Flags $NonAsciiFlags -ErrorAction Stop;
            `$Actual | Should -BeFalse -Because '$((CharsToDisplay $MatchingNonAscii).Replace("'", '"'))';
        }
"@
            } else {
                if ($MatchingNonAscii.Length -lt 4) {
                    @"

        It '$((CharsToDisplay $MatchingNonAscii).Replace("'", '"')) should return false' {
            foreach (`$Value in ($(CharsToArrayCode $MatchingNonAscii))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $NonAsciiFlags -ErrorAction Stop;
                `$Actual | Should -BeFalse -Because ""`$Value"";
            }
        }
"@
                } else {
                    @"
        It '$NonAsciiFlags characters should return false' {
            foreach (`$Value in ($(CharsToArrayCode $MatchingNonAscii))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $NonAsciiFlags -ErrorAction Stop;
                `$Actual | Should -BeFalse -Because ""`$Value"";
            }
        }
"@
                }
            }
            @"

        It 'Other $SimilarDisplay characters should return true' {
            foreach (`$Value in ($(CharsToArrayCode @($Similar + $MatchingAscii)))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $NonAsciiFlags -ErrorAction Stop;
                `$Actual | Should -BeTrue -Because ""`$Value"";
            }
        }

        It 'Non-$SimilarDisplay characters should return true' {
            foreach (`$Value in ($(CharsToArrayCode $Other))) {
                `$Actual = Test-CharacterClassFlags -Value `$Value -Flags $NonAsciiFlags -ErrorAction Stop;
                `$Actual | Should -BeTrue -Because ""`$Value"";
            }
        }
    }
}
"@
        }
    }
}

# Get-TestCode -Flags 'Symbol' -SimilarDisplay 'Symbol' -Method 'IsSymbol';
Get-TestCode -Flags 'ModifierSymbol' -SimilarDisplay 'Symbol' -Method 'IsSymbol' -AsciiFlags 'AsciiModifierSymbol' -NonAsciiFlags 'NonAsciiModifierSymbol' -Category ModifierSymbol;