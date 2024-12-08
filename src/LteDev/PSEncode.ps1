if ($null -eq $Script:SingleQuotedLiteralToEscapeRegex) {
    New-Variable -Name 'SingleQuotedLiteralToEscapeRegex' -Option ReadOnly -Scope 'Script' -Value ([regex]::new("['‘’]"));
    New-Variable -Name 'DoubleQuotedLiteralToEscapeRegex' -Option ReadOnly -Scope 'Script' -Value ([regex]::new('[\x00-0x19\$"`\x7f-\u2017\u201a-\uffff]'));
    New-Variable -Name 'AnyLiteralToEscapeRegex' -Option ReadOnly -Scope 'Script' -Value ([regex]::new('([\$"`“”''‘‘’’])'));
    New-Variable -Name 'SingleQuoteIncompatibleRegex' -Option ReadOnly -Scope 'Script' -Value ([regex]::new('[\x00-0x19\x7f-\u2017\u201a-\u201b\u201e-\uffff]'));
    New-Variable -Name 'EscapeChar' -Option Constant -Scope 'Script' -Value ([char]"`e");
    New-Variable -Name 'DeleteChar' -Option Constant -Scope 'Script' -Value ([char]"`u{7f}");
    $InnerDict = [System.Collections.Generic.Dictionary[Type,string]]::new();
    $InnerDict.Add([byte], "d$([byte]::MaxValue.ToString().Length)");
    $InnerDict.Add([sbyte], "d$([sbyte]::MaxValue.ToString().Length)");
    $InnerDict.Add([short], "d$([short]::MaxValue.ToString().Length)");
    $InnerDict.Add([ushort], "d$([ushort]::MaxValue.ToString().Length)");
    $InnerDict.Add([int], "d$([int]::MaxValue.ToString().Length)");
    $InnerDict.Add([uint], "d$([uint]::MaxValue.ToString().Length)");
    $InnerDict.Add([long], "d$([long]::MaxValue.ToString().Length)");
    $InnerDict.Add([ulong], "d$([ulong]::MaxValue.ToString().Length)");
    New-Variable -Name 'IntegerBase10PadFormats' -Option ReadOnly -Scope 'Script' -Value ([System.Collections.ObjectModel.ReadOnlyDictionary[Type,string]]::new($InnerDict));
    $InnerDict = [System.Collections.Generic.Dictionary[Type,string]]::new();
    $InnerDict.Add([byte], "x$([byte]::MaxValue.ToString('x').Length)");
    $InnerDict.Add([sbyte], "x$([sbyte]::MaxValue.ToString('x').Length)");
    $InnerDict.Add([short], "x$([short]::MaxValue.ToString('x').Length)");
    $InnerDict.Add([ushort], "x$([ushort]::MaxValue.ToString('x').Length)");
    $InnerDict.Add([int], "x$([int]::MaxValue.ToString('x').Length)");
    $InnerDict.Add([uint], "x$([uint]::MaxValue.ToString('x').Length)");
    $InnerDict.Add([long], "x$([long]::MaxValue.ToString('x').Length)");
    $InnerDict.Add([ulong], "x$([ulong]::MaxValue.ToString('x').Length)");
    New-Variable -Name 'IntegerHexPadFormats' -Option ReadOnly -Scope 'Script' -Value ([System.Collections.ObjectModel.ReadOnlyDictionary[Type,string]]::new($InnerDict));
    $InnerDict = [System.Collections.Generic.Dictionary[Type,string]]::new();
    $InnerDict.Add([byte], "b$([byte]::MaxValue.ToString('b').Length)");
    $InnerDict.Add([sbyte], "b$([sbyte]::MaxValue.ToString('b').Length)");
    $InnerDict.Add([short], "b$([short]::MaxValue.ToString('b').Length)");
    $InnerDict.Add([ushort], "b$([ushort]::MaxValue.ToString('b').Length)");
    $InnerDict.Add([int], "b$([int]::MaxValue.ToString('b').Length)");
    $InnerDict.Add([uint], "b$([uint]::MaxValue.ToString('b').Length)");
    $InnerDict.Add([long], "b$([long]::MaxValue.ToString('b').Length)");
    $InnerDict.Add([ulong], "b$([ulong]::MaxValue.ToString('b').Length)");
    New-Variable -Name 'IntegerBinaryPadFormats' -Option ReadOnly -Scope 'Script' -Value ([System.Collections.ObjectModel.ReadOnlyDictionary[Type,string]]::new($InnerDict));
}

Function ConvertTo-PsStringLiteral {
    [CmdletBinding(DefaultParameterSetName = 'PreferSingle')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        # The string to be converted to a PowerShell string literal.
        [object]$InputString,

        [Parameter(ParameterSetName = 'PreferSingle')]
        # Result literal is surrounded by single quotes unless a double-quoted literal is shorter or a character is a control character or non-ASCII.
        [switch]$PreferSingleQuotes,

        [Parameter(Mandatory = $true, ParameterSetName = 'PreferDouble')]
        # Result literal is surrounded by double quotes unless a single-quoted literal is shorter in length.
        [switch]$PreferDoubleQuotes,

        [Parameter(Mandatory = $true, ParameterSetName = 'AlwaysDouble')]
        # Result literal is always surrounded by double quotes.
        [switch]$AlwaysDoubleQuotes,

        [Parameter(Mandatory = $true, ParameterSetName = 'OmitQuotes')]
        # Do not include leading and trailing quotes. Characters are escapes as though they were part of a double-quoted string.
        [switch]$OmitQuotes
    )

    Process {
        if ($InputString -eq '') {
            if ($OmitQuotes.IsPresent) {
                $InputString | Write-Output;
            } else {
                if ($StringQuotes -eq 'PreferSingle') {
                    "''" | Write-Output;
                } else {
                    '""' | Write-Output;
                }
            }
        } else {
            $UseDoubleQuote = $true;
            switch ($StringQuotes) {
                'PreferSingle' {
                    if (-not $Script:SingleQuoteIncompatibleRegex.IsMatch($InputString)) {
                        if ($MatchCollection.Count -eq 0) {
                            $UseDoubleQuote = $false;
                        } else {
                            $d = $s = 0;
                            $MatchCollection | ForEach-Object {
                                switch ($_.Value) {
                                    "‘" { $s++; break; }
                                    "’" { $s++; break; }
                                    "'" { $s++; break; }
                                    '$' { $d++; break; }
                                    '"' { $d++; break; }
                                    '`' { $d++; break; }
                                    '“' { $d++; break; }
                                    '”' { $d++; break; }
                                }
                            }
                            if ($d -ge $s) { $UseDoubleQuote = $false }
                        }
                    }
                    break;
                }
                'PreferDouble' {
                    if (-not $Script:SingleQuoteIncompatibleRegex.IsMatch($InputString)) {
                        $MatchCollection = $Script:AnyLiteralToEscapeRegex.Matches($InputString);
                        if ($MatchCollection.Count -gt 0) {
                            $d = $s = 0;
                            $MatchCollection | ForEach-Object {
                                switch ($_.Value) {
                                    "‘" { $s++; break; }
                                    "’" { $s++; break; }
                                    "'" { $s++; break; }
                                    '$' { $d++; break; }
                                    '"' { $d++; break; }
                                    '`' { $d++; break; }
                                    '“' { $d++; break; }
                                    '”' { $d++; break; }
                                }
                            }
                            if ($d -gt $s) { $UseDoubleQuote = $false }
                        }
                    }
                    break;
                }
            }
            if ($UseDoubleQuote) {
                $m = $Script:DoubleQuotedLiteralToEscapeRegex.Match($InputString);
                if ($m.Success) {
                    $StringBuilder = [System.Text.StringBuilder]::new();
                    if (-not $OmitQuotes.IsPresent) { $StringBuilder.Append('"') | Out-Null }
                    switch ($m.Value) {
                        "`0" {
                            $StringBuilder.Append('`0') | Out-Null;
                            break;
                        }
                        "`a" {
                            $StringBuilder.Append('`a') | Out-Null;
                            break;
                        }
                        "`b" {
                            $StringBuilder.Append('`b') | Out-Null;
                            break;
                        }
                        "`t" {
                            $StringBuilder.Append('`t') | Out-Null;
                            break;
                        }
                        "`n" {
                            $StringBuilder.Append('`n') | Out-Null;
                            break;
                        }
                        "`v" {
                            $StringBuilder.Append('`v') | Out-Null;
                            break;
                        }
                        "`f" {
                            $StringBuilder.Append('`f') | Out-Null;
                            break;
                        }
                        "`r" {
                            $StringBuilder.Append('`r') | Out-Null;
                            break;
                        }
                        "`e" {
                            $StringBuilder.Append('`e') | Out-Null;
                            break;
                        }
                        '“' {
                            $StringBuilder.Append('““') | Out-Null;
                            break;
                        }
                        '”' {
                            $StringBuilder.Append('””') | Out-Null;
                            break;
                        }
                        default {
                            $c = $m.Value[0]
                            if ($c -lt $Script:EscapeChar -or $c -ge $Script:DeleteChar) {
                                $StringBuilder.Append('`u{').Append(([int]$c).ToString('x')).Append('}') | Out-Null;
                            } else {
                                $StringBuilder.Append('`').Append($c) | Out-Null;
                            }
                            break;
                        }
                    }
                    $StartAt = $m.Length;
                    while ($StartAt -lt $InputString.Length) {
                        $m = $Script:DoubleQuotedLiteralToEscapeRegex.Match($InputString, $StartAt);
                        if ($m.Success) {
                            $Len = $m.Index - $StartAt;
                            if ($Len -gt 0) {
                                $StringBuilder.Append($InputString.Substring($StartAt, $Len)) | Out-Null;
                            }
                            switch ($m.Value) {
                                "`0" {
                                    $StringBuilder.Append('`0') | Out-Null;
                                    break;
                                }
                                "`a" {
                                    $StringBuilder.Append('`a') | Out-Null;
                                    break;
                                }
                                "`b" {
                                    $StringBuilder.Append('`b') | Out-Null;
                                    break;
                                }
                                "`t" {
                                    $StringBuilder.Append('`t') | Out-Null;
                                    break;
                                }
                                "`n" {
                                    $StringBuilder.Append('`n') | Out-Null;
                                    break;
                                }
                                "`v" {
                                    $StringBuilder.Append('`v') | Out-Null;
                                    break;
                                }
                                "`f" {
                                    $StringBuilder.Append('`f') | Out-Null;
                                    break;
                                }
                                "`r" {
                                    $StringBuilder.Append('`r') | Out-Null;
                                    break;
                                }
                                "`e" {
                                    $StringBuilder.Append('`e') | Out-Null;
                                    break;
                                }
                                '“' {
                                    $StringBuilder.Append('““') | Out-Null;
                                    break;
                                }
                                '”' {
                                    $StringBuilder.Append('””') | Out-Null;
                                    break;
                                }
                                default {
                                    $c = $m.Value[0]
                                    if ($c -lt $Script:EscapeChar -or $c -ge $Script:DeleteChar) {
                                        $StringBuilder.Append('`u{').Append(([int]$c).ToString('x')).Append('}') | Out-Null;
                                    } else {
                                        $StringBuilder.Append('`').Append($c) | Out-Null;
                                    }
                                    break;
                                }
                            }
                            $StartAt = $m.Index + $m.Length;
                        } else {
                            $StringBuilder.Append($InputString.Substring($StartAt)) | Out-Null;
                            break;
                        }
                    }
                    if ($OmitQuotes.IsPresent) {
                        $StringBuilder.ToString() | Write-Output;
                    } else {
                        $StringBuilder.Append('"').ToString() | Write-Output;
                    }
                } else {
                    if ($OmitQuotes.IsPresent) {
                        $InputString | Write-Output;
                    } else {
                        "`"$InputString`"" | Write-Output;
                    }
                }
            } else {
                $m = $Script:SingleQuotedLiteralToEscapeRegex.Match($InputString);
                if ($m.Success) {
                    $StringBuilder = [System.Text.StringBuilder]::new("'").Append($m.Value).Append($m.Value);
                    $StartAt = $m.Length;
                    while ($StartAt -lt $InputString.Length) {
                        $m = $Script:SingleQuotedLiteralToEscapeRegex.Match($InputString, $StartAt);
                        if ($m.Success) {
                            $Len = $m.Index - $StartAt;
                            if ($Len -gt 0) {
                                $StringBuilder.Append($InputString.Substring($StartAt, $Len)) | Out-Null;
                            }
                            $StringBuilder.Append($m.Value).Append($m.Value) | Out-Null;
                            $StartAt = $m.Index + $m.Length;
                        } else {
                            $StringBuilder.Append($InputString.Substring($StartAt)) | Out-Null;
                            break;
                        }
                    }
                    $StringBuilder.Append("'").ToString() | Write-Output;
                } else {
                    "'$InputString'" | Write-Output;
                }
            }
        }
    }

}

Function ConvertTo-PsCharLiteral {
    [CmdletBinding(DefaultParameterSetName = 'PreferSingle')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        # Character value to convert to a PowerShell code literal.
        [char]$InputValue,

        [Parameter(ParameterSetName = 'PreferSingle')]
        # Result literal is surrounded by single quotes unless a double-quoted literal is shorter or a character is a control character or non-ASCII.
        [switch]$PreferSingleQuotes,

        [Parameter(Mandatory = $true, ParameterSetName = 'PreferDouble')]
        # Result literal is surrounded by double quotes unless a single-quoted literal is shorter in length.
        [switch]$PreferDoubleQuotes,

        [Parameter(Mandatory = $true, ParameterSetName = 'AlwaysDouble')]
        # Result literal is always surrounded by double quotes.
        [switch]$AlwaysDoubleQuotes,

        [Parameter(Mandatory = $true, ParameterSetName = 'OmitQuotes')]
        # Do not include leading and trailing quotes. Characters are escapes as though they were part of a double-quoted string.
        [switch]$OmitQuotes
    )

    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'OmitQuotes' {
                switch ($InputValue) {
                    "`0" { '`0' | Write-Output; break; }
                    "`a" { '`a' | Write-Output; break; }
                    "`b" { '`b' | Write-Output; break; }
                    "`t" { '`t' | Write-Output; break; }
                    "`n" { '`n' | Write-Output; break; }
                    "`v" { '`v' | Write-Output; break; }
                    "`f" { '`f' | Write-Output; break; }
                    "`r" { '`r' | Write-Output; break; }
                    "`e" { '`e' | Write-Output; break; }
                    '$' { '`$' | Write-Output; break; }
                    '"' { '`"' | Write-Output; break; }
                    '`' { '``' | Write-Output; break; }
                    "‘" { "‘" | Write-Output; break; }
                    "’" { "’" | Write-Output; break; }
                    '“' { '`“' | Write-Output; break; }
                    '”' { '`”' | Write-Output; break; }
                    default {
                        if ($InputValue -lt $Script:EscapeChar -or $InputValue -ge $Script:DeleteChar) {
                            "``u{$(([int]$InputValue).ToString('x'))}" | Write-Output;
                        } else {
                            $InputValue.ToString() | Write-Output;
                        }
                        break;
                    }
                }
                break;
            }
            'PreferDouble' {
                switch ($InputValue) {
                    "`0" { '"`0"' | Write-Output; break; }
                    "`a" { '"`a"' | Write-Output; break; }
                    "`b" { '"`b"' | Write-Output; break; }
                    "`t" { '"`t"' | Write-Output; break; }
                    "`n" { '"`n"' | Write-Output; break; }
                    "`v" { '"`v"' | Write-Output; break; }
                    "`f" { '"`f"' | Write-Output; break; }
                    "`r" { '"`r"' | Write-Output; break; }
                    "`e" { '"`e"' | Write-Output; break; }
                    "'" { "`"'`"" | Write-Output; break; }
                    '$' { "'`$'" | Write-Output; break; }
                    '"' { "'`"'" | Write-Output; break; }
                    '`' { "'``'" | Write-Output; break; }
                    "‘" { "`"‘`"" | Write-Output; break; }
                    "’" { "`"’`"" | Write-Output; break; }
                    '“' { "'““'" | Write-Output; break; }
                    '”' { "'””'" | Write-Output; break; }
                    default {
                        if ($InputValue -lt $Script:EscapeChar -or $InputValue -ge $Script:DeleteChar) {
                            "`"``u{$(([int]$InputValue).ToString('x'))}`"" | Write-Output;
                        } else {
                            "`"$InputValue`"" | Write-Output;
                        }
                        break;
                    }
                }
                break;
            }
            'AlwaysDouble' {
                switch ($InputValue) {
                    "`0" { '"`0"' | Write-Output; break; }
                    "`a" { '"`a"' | Write-Output; break; }
                    "`b" { '"`b"' | Write-Output; break; }
                    "`t" { '"`t"' | Write-Output; break; }
                    "`n" { '"`n"' | Write-Output; break; }
                    "`v" { '"`v"' | Write-Output; break; }
                    "`f" { '"`f"' | Write-Output; break; }
                    "`r" { '"`r"' | Write-Output; break; }
                    "`e" { '"`e"' | Write-Output; break; }
                    "'" { "`"'`"" | Write-Output; break; }
                    '$' { "`"```$`"" | Write-Output; break; }
                    '"' { "`"```"`"" | Write-Output; break; }
                    '`' { "`"`````"" | Write-Output; break; }
                    "‘" { "`"‘`"" | Write-Output; break; }
                    "’" { "`"’`"" | Write-Output; break; }
                    '“' { "`"``““`"" | Write-Output; break; }
                    '”' { "`"``””`"" | Write-Output; break; }
                    default {
                        if ($InputValue -lt $Script:EscapeChar -or $InputValue -ge $Script:DeleteChar) {
                            "`"``u{$(([int]$InputValue).ToString('x'))}`"" | Write-Output;
                        } else {
                            "`"$InputValue`"" | Write-Output;
                        }
                        break;
                    }
                }
                break;
            }
            default {
                switch ($InputValue) {
                    "`0" { '"`0"' | Write-Output; break; }
                    "`a" { '"`a"' | Write-Output; break; }
                    "`b" { '"`b"' | Write-Output; break; }
                    "`t" { '"`t"' | Write-Output; break; }
                    "`n" { '"`n"' | Write-Output; break; }
                    "`v" { '"`v"' | Write-Output; break; }
                    "`f" { '"`f"' | Write-Output; break; }
                    "`r" { '"`r"' | Write-Output; break; }
                    "`e" { '"`e"' | Write-Output; break; }
                    "'" { "`"'`"" | Write-Output; break; }
                    '$' { "'`$'" | Write-Output; break; }
                    '"' { "'`"'" | Write-Output; break; }
                    '`' { "'``'" | Write-Output; break; }
                    "‘" { "`"‘`"" | Write-Output; break; }
                    "’" { "`"’`"" | Write-Output; break; }
                    '“' { "'““'" | Write-Output; break; }
                    '”' { "'””'" | Write-Output; break; }
                    default {
                        if ($InputValue -lt $Script:EscapeChar -or $InputValue -ge $Script:DeleteChar) {
                            "`"``u{$(([int]$InputValue).ToString('x'))}`"" | Write-Output;
                        } else {
                            "'$InputValue'" | Write-Output;
                        }
                        break;
                    }
                }
                break;
            }
        }
    }
}

Function ConvertTo-PsIntegerLiteral {
    [CmdletBinding(DefaultParameterSetName = 'Base10')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [ValidateScript({ $_ -is [byte] -or $_ -is [sbyte] -or $_ -is [ushort] -or $_ -is [short] -or $_ -is [int] -or $_ -is [uint] -or $_ -is [long] -or $_ -is [ulong] -or $_ -is [bigint] })]
        [object]$InputValue,

        [ValidateSet('PreferSingle', 'PreferDouble', 'AlwaysDouble')]
        [string]$StringQuotes = 'PreferSingle',

        [switch]$OmitNumericSuffix,

        [Parameter(ParameterSetName = 'Base10')]
        [switch]$Base10,

        [Parameter(Mandatory = $true, ParameterSetName = 'Hexidecimal')]
        [switch]$Hexidecimal,

        [Parameter(Mandatory = $true, ParameterSetName = 'Binary')]
        [switch]$Binary,

        # Pad with zeroes according to bit length. This is ignored for [bigint] values.
        [switch]$ZeroPadded,

        [Parameter(ParameterSetName = 'Hexidecimal', HelpMessage = 'Omits the leading "0x"')]
        [Parameter(ParameterSetName = 'Binary', HelpMessage = 'Omits the leading "0b"')]
        [switch]$OmitPrefix,

        [ValidateSet('short', 'int', 'long')]
        [string]$MinBits
    )
    
    Process {
        $Value = $null;
        if ($PSBoundParameters.ContainsKey('MinBits')) {
            switch ($MinBits) {
                'short' {
                    switch ($InputValue) {
                        { $_ -is [byte] } { [uint]$Value = $InputValue; break; }
                        { $_ -is [sbyte] } { [int]$Value = $InputValue; break; }
                        { $_ -is [short] -or $_ -is [ushort] } { $Value = $InputValue; break; }
                        { $_ -is [uint] -or $_ -is [ulong] } {
                            if ($InputValue -le [short]::MaxValue) {
                                [short]$Value = $InputValue;
                            } else {
                                if ($InputValue -le [ushort]::MaxValue) { [ushort]$Value = $InputValue } else { $Value = $InputValue }
                            }
                            break;
                        }
                        default {
                            if ($InputValue -lt 0) {
                                if ($InputValue -ge [short]::MinValue) { [short]$Value = $InputValue } else { $Value = $InputValue }
                            } else {
                                if ($InputValue -lt [short]::MaxValue) {
                                    [int]$Value = $InputValue
                                } else {
                                    if ($InputValue -le [ushort]::MaxValue) { [ushort]$Value = $InputValue } else { $Value = $InputValue }
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
                'long' {
                    switch ($InputValue) {
                        { $_ -is [byte] -or $_ -is [ushort] -or $_ -is [uint] } { [ulong]$Value = $InputValue; break; }
                        { $_ -is [sbyte] -or $_ -is [short] -or $_ -is [int] } { [long]$Value = $InputValue; break; }
                        { $_ -is [bigint] } {
                            if ($InputValue -lt 0) {
                                if ($InputValue -ge [long]::MinValue) { [long]$Value = $InputValue } else { $Value = $InputValue }
                            } else {
                                if ($InputValue -lt [long]::MaxValue) {
                                    [long]$Value = $InputValue
                                } else {
                                    if ($InputValue -le [ulong]::MaxValue) { [ulong]$Value = $InputValue } else { $Value = $InputValue }
                                }
                            }
                            break;
                        }
                        default { $Value = $InputValue; break; }
                    }
                    break;
                }
                default {
                    switch ($InputValue) {
                        { $_ -is [byte] -or $_ -is [ushort] } { [uint]$Value = $InputValue; break; }
                        { $_ -is [sbyte] -or $_ -is [short] } { [int]$Value = $InputValue; break; }
                        { $_ -is [int] -or $_ -is [uint] } { $Value = $InputValue; break; }
                        { $_ -is [ulong] } {
                            if ($InputValue -le [int]::MaxValue) {
                                [int]$Value = $InputValue;
                            } else {
                                if ($InputValue -le [uint]::MaxValue) { [uint]$Value = $InputValue } else { $Value = $InputValue }
                            }
                            break;
                        }
                        default {
                            if ($InputValue -lt 0) {
                                if ($InputValue -ge [int]::MinValue) { [int]$Value = $InputValue } else { $Value = $InputValue }
                            } else {
                                if ($InputValue -lt [int]::MaxValue) {
                                    [int]$Value = $InputValue
                                } else {
                                    if ($InputValue -le [uint]::MaxValue) { [uint]$Value = $InputValue } else { $Value = $InputValue }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        } else {
            $Value = $InputValue;
        }
        switch ($PSCmdlet.ParameterSetName) {
            'Hexidecimal' {
                if ($OmitNumericSuffix.IsPresent) {
                    if ($ZeroPadded.IsPresent) {
                        if ($OmitPrefix.IsPresent) {
                            $Value.ToString($Script:IntegerHexPadFormats[$Value.GetType()]) | Write-Output;
                        } else {
                            "0x$($Value.ToString($Script:IntegerHexPadFormats[$Value.GetType()]))" | Write-Output;
                        }
                    } else {
                        if ($OmitPrefix.IsPresent) {
                            $Value.ToString('x') | Write-Output;
                        } else {
                            "0x$($Value.ToString('x'))" | Write-Output;
                        }
                    }
                } else {
                    if ($ZeroPadded.IsPresent) {
                        switch ($Value) {
                            { $_ -is [byte] } { "$($Value.ToString($Script:IntegerHexPadFormats[[byte]]))UY" | Write-Output;; break; }
                            { $_ -is [sbyte] } { "$($Value.ToString($Script:IntegerHexPadFormats[[sbyte]]))Y" | Write-Output;; break; }
                            { $_ -is [short] } { "$($Value.ToString($Script:IntegerHexPadFormats[[short]]))S" | Write-Output;; break; }
                            { $_ -is [ushort] } { "$($Value.ToString($Script:IntegerHexPadFormats[[ushort]]))US" | Write-Output;; break; }
                            { $_ -is [uint] } { "$($Value.ToString($Script:IntegerHexPadFormats[[uint]]))U" | Write-Output;; break; }
                            { $_ -is [long] } { "$($Value.ToString($Script:IntegerHexPadFormats[[long]]))L" | Write-Output;; break; }
                            { $_ -is [ulong] } { "$($Value.ToString($Script:IntegerHexPadFormats[[ulong]]))UL" | Write-Output;; break; }
                            { $_ -is [bigint] } { "$($Value.ToString('x'))N" | Write-Output; break; }
                            default { $Value.ToString($Script:IntegerHexPadFormats[[int]]) | Write-Output; break; }
                        }
                    } else {
                        switch ($Value) {
                            { $_ -is [byte] } { "$($Value.ToString('x'))UY" | Write-Output;; break; }
                            { $_ -is [sbyte] } { "$($Value.ToString('x'))Y" | Write-Output;; break; }
                            { $_ -is [short] } { "$($Value.ToString('x'))S" | Write-Output;; break; }
                            { $_ -is [ushort] } { "$($Value.ToString('x'))US" | Write-Output;; break; }
                            { $_ -is [uint] } { "$($Value.ToString('x'))U" | Write-Output;; break; }
                            { $_ -is [long] } { "$($Value.ToString('x'))L" | Write-Output;; break; }
                            { $_ -is [ulong] } { "$($Value.ToString('x'))UL" | Write-Output;; break; }
                            { $_ -is [bigint] } { "$($Value.ToString('x'))N" | Write-Output; break; }
                            default { $Value.ToString('x') | Write-Output; break; }
                        }
                    }
                }
                break;
            }
            'Binary' {
                if ($OmitNumericSuffix.IsPresent) {
                    if ($ZeroPadded.IsPresent) {
                        if ($OmitPrefix.IsPresent) {
                            $Value.ToString($Script:IntegerBinaryPadFormats[$Value.GetType()]) | Write-Output;
                        } else {
                            "0x$($Value.ToString($Script:IntegerBinaryPadFormats[$Value.GetType()]))" | Write-Output;
                        }
                    } else {
                        if ($OmitPrefix.IsPresent) {
                            $Value.ToString('x') | Write-Output;
                        } else {
                            "0x$($Value.ToString('x'))" | Write-Output;
                        }
                    }
                } else {
                    if ($ZeroPadded.IsPresent) {
                        switch ($Value) {
                            { $_ -is [byte] } { "$($Value.ToString($Script:IntegerBinaryPadFormats[[byte]]))UY" | Write-Output;; break; }
                            { $_ -is [sbyte] } { "$($Value.ToString($Script:IntegerBinaryPadFormats[[sbyte]]))Y" | Write-Output;; break; }
                            { $_ -is [short] } { "$($Value.ToString($Script:IntegerBinaryPadFormats[[short]]))S" | Write-Output;; break; }
                            { $_ -is [ushort] } { "$($Value.ToString($Script:IntegerBinaryPadFormats[[ushort]]))US" | Write-Output;; break; }
                            { $_ -is [uint] } { "$($Value.ToString($Script:IntegerBinaryPadFormats[[uint]]))U" | Write-Output;; break; }
                            { $_ -is [long] } { "$($Value.ToString($Script:IntegerBinaryPadFormats[[long]]))L" | Write-Output;; break; }
                            { $_ -is [ulong] } { "$($Value.ToString($Script:IntegerBinaryPadFormats[[ulong]]))UL" | Write-Output;; break; }
                            { $_ -is [bigint] } { "$($Value.ToString('b'))N" | Write-Output; break; }
                            default { $Value.ToString($Script:IntegerBinaryPadFormats[[int]]) | Write-Output; break; }
                        }
                    } else {
                        switch ($Value) {
                            { $_ -is [byte] } { "$($Value.ToString('b'))UY" | Write-Output;; break; }
                            { $_ -is [sbyte] } { "$($Value.ToString('b'))Y" | Write-Output;; break; }
                            { $_ -is [short] } { "$($Value.ToString('b'))S" | Write-Output;; break; }
                            { $_ -is [ushort] } { "$($Value.ToString('b'))US" | Write-Output;; break; }
                            { $_ -is [uint] } { "$($Value.ToString('b'))U" | Write-Output;; break; }
                            { $_ -is [long] } { "$($Value.ToString('b'))L" | Write-Output;; break; }
                            { $_ -is [ulong] } { "$($Value.ToString('b'))UL" | Write-Output;; break; }
                            { $_ -is [bigint] } { "$($Value.ToString('b'))N" | Write-Output; break; }
                            default { $Value.ToString('b') | Write-Output; break; }
                        }
                    }
                }
                break;
            }
            default {
                if ($OmitNumericSuffix.IsPresent) {
                    if ($ZeroPadded.IsPresent) {
                        $Value.ToString($Script:IntegerBase10PadFormats[$Value.GetType()]) | Write-Output;
                    } else {
                        $Value.ToString() | Write-Output;
                    }
                } else {
                    if ($ZeroPadded.IsPresent) {
                        switch ($Value) {
                            { $_ -is [byte] } { "$($Value.ToString($Script:IntegerBase10PadFormats[[byte]]))UY" | Write-Output;; break; }
                            { $_ -is [sbyte] } { "$($Value.ToString($Script:IntegerBase10PadFormats[[sbyte]]))Y" | Write-Output;; break; }
                            { $_ -is [short] } { "$($Value.ToString($Script:IntegerBase10PadFormats[[short]]))S" | Write-Output;; break; }
                            { $_ -is [ushort] } { "$($Value.ToString($Script:IntegerBase10PadFormats[[ushort]]))US" | Write-Output;; break; }
                            { $_ -is [uint] } { "$($Value.ToString($Script:IntegerBase10PadFormats[[uint]]))U" | Write-Output;; break; }
                            { $_ -is [long] } { "$($Value.ToString($Script:IntegerBase10PadFormats[[long]]))L" | Write-Output;; break; }
                            { $_ -is [ulong] } { "$($Value.ToString($Script:IntegerBase10PadFormats[[ulong]]))UL" | Write-Output;; break; }
                            { $_ -is [bigint] } { "$($Value)N" | Write-Output; break; }
                            default { $Value.ToString($Script:IntegerBase10PadFormats[[int]]) | Write-Output; break; }
                        }
                    } else {
                        switch ($Value) {
                            { $_ -is [byte] } { "$($Value)UY" | Write-Output;; break; }
                            { $_ -is [sbyte] } { "$($Value)Y" | Write-Output;; break; }
                            { $_ -is [short] } { "$($Value)S" | Write-Output;; break; }
                            { $_ -is [ushort] } { "$($Value)US" | Write-Output;; break; }
                            { $_ -is [uint] } { "$($Value)U" | Write-Output;; break; }
                            { $_ -is [long] } { "$($Value)L" | Write-Output;; break; }
                            { $_ -is [ulong] } { "$($Value)UL" | Write-Output;; break; }
                            { $_ -is [bigint] } { "$($Value)N" | Write-Output; break; }
                            default { $Value.ToString() | Write-Output; break; }
                        }
                    }
                }
                break;
            }
        }
    }
}

Function ConvertTo-PsScriptLiteral {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowNull()]
        [AllowEmptyString()]
        [ValidateScript({ $_ -is [string] -or $_ -is [char] -or $_ -is [byte] -or $_ -is [sbyte] -or $_ -is [short]  -or $_ -is [ushort] -or $_ -is [int] -or $_ -is [uint] -or $_ -is [long] -or `
            $_ -is [ulong] -or $_ -is [bigint] -or $_ -is [float] -or  $_ -is [double] -or $_ -is [decimal] -or $_ -is [bool] -or $_ -is [DateTime] -or $_ -is [DateOnly] -or $_ -is [TimeOnly] -or `
            $_ -is [Guid] -or $_ -is [TimeSpan] -or $_ -is [Uri] -or $_ -is [System.Management.Automation.SemanticVersion] -or $_ -is [Version] -or $_ -is [ScriptBlock] -or $_ -is [enum] })]
        [object]$InputObject,

        [ValidateSet('PreferSingle', 'PreferDouble', 'AlwaysDouble')]
        [string]$StringQuotes = 'PreferSingle',

        [ValidateSet('Base10', 'Hexidecimal', 'Binary')]
        [string]$IntegerFormat = 'Base10',

        [ValidateSet('short', 'int', 'long')]
        [string]$MinIntegerBits,

        [switch]$OmitNumericSuffix,

        [switch]$ZeroPaddedIntegers,

        [ValidateRange(0, [int]::MaxValue)]
        [int]$MinDecimalPlaces = 1,

        [ValidateRange(0, [int]::MaxValue)]
        [int]$MaxDecimalPlaces,

        [ValidateSet('Microseconds', 'Milliseconds', 'Seconds', 'Minutes', 'Hours', 'Days')]
        [string]$TruncateDateTime,

        [ValidateSet('Local', 'Utc')]
        [string]$DateTimeKind,

        [ValidateSet('Local', 'Utc')]
        [string]$DateTimeUnspecifiedAs
    )

    Process {
        if ($null -eq $InputObject) {
            '$null' | Write-Output;
        } else {
            switch ($InputObject) {
                $null { '$null' | Write-Output; break; }
                { $_ -is [char] } {
                    switch ($StringQuotes) {
                        'PreferDouble' {
                            ($InputObject | ConvertTo-PsCharLiteral -PreferDoubleQuotes) | Write-Output;
                            break;
                        }
                        'AlwaysDouble' {
                            ($InputObject | ConvertTo-PsCharLiteral -AlwaysDoubleQuotes) | Write-Output;
                            break;
                        }
                        default {
                            ($InputObject | ConvertTo-PsCharLiteral -PreferSingleQuotes) | Write-Output;
                            break;
                        }
                    }
                    break;
                }
                { $_ -is [byte] -or $_ -is [sbyte] -or $_ -is [short]  -or $_ -is [ushort] -or $_ -is [int] -or $_ -is [uint] -or $_ -is [long] -or $_ -is [ulong] -or $_ -is [bigint] } {
                    switch ($IntegerFormat) {
                        'Binary' {
                            if ($OmitNumericSuffix.IsPresent) {
                                if ($ZeroPaddedIntegers.IsPresent) {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Binary -ZeroPadded -OmitNumericSuffix) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Binary -ZeroPadded -OmitNumericSuffix) | Write-Output;
                                    }
                                } else {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Binary -OmitNumericSuffix) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Binary -OmitNumericSuffix) | Write-Output;
                                    }
                                }
                            } else {
                                if ($ZeroPaddedIntegers.IsPresent) {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Binary -ZeroPadded) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Binary -ZeroPadded) | Write-Output;
                                    }
                                } else {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Binary) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Binary) | Write-Output;
                                    }
                                }
                            }
                            break;
                        }
                        'Hexidecimal' {
                            if ($OmitNumericSuffix.IsPresent) {
                                if ($ZeroPaddedIntegers.IsPresent) {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Hexidecimal -ZeroPadded -OmitNumericSuffix) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Hexidecimal -ZeroPadded -OmitNumericSuffix) | Write-Output;
                                    }
                                } else {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Hexidecimal -OmitNumericSuffix) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Hexidecimal -OmitNumericSuffix) | Write-Output;
                                    }
                                }
                            } else {
                                if ($ZeroPaddedIntegers.IsPresent) {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Hexidecimal -ZeroPadded) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Hexidecimal -ZeroPadded) | Write-Output;
                                    }
                                } else {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Hexidecimal) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Hexidecimal) | Write-Output;
                                    }
                                }
                            }
                            break;
                        }
                        default {
                            if ($OmitNumericSuffix.IsPresent) {
                                if ($ZeroPaddedIntegers.IsPresent) {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Base10 -ZeroPadded -OmitNumericSuffix) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Base10 -ZeroPadded -OmitNumericSuffix) | Write-Output;
                                    }
                                } else {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Base10 -OmitNumericSuffix) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Base10 -OmitNumericSuffix) | Write-Output;
                                    }
                                }
                            } else {
                                if ($ZeroPaddedIntegers.IsPresent) {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Base10 -ZeroPadded) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Base10 -ZeroPadded) | Write-Output;
                                    }
                                } else {
                                    if ($PSBoundParameters.ContainsKey('MinIntegerBits')) {
                                       ( $InputObject | ConvertTo-PsIntegerLiteral -MinBits $MinIntegerBits -Base10) | Write-Output;
                                    } else {
                                        ( $InputObject | ConvertTo-PsIntegerLiteral -Base10) | Write-Output;
                                    }
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
                { $_ -is [float] -or  $_ -is [double] } {
                    $Value = $InputObject;
                    if ($PSBoundParameters.ContainsKey('MaxDecimalPlaces')) { $Value = [Math]::Round($InputObject, $MaxDecimalPlaces) }
                    $Code = $Value.ToString();
                    if ($MinDecimalPlaces -eq 0 -or $Code.Contains('E')) {
                        $Code | Write-Output;
                    } else {
                        $i = $Code.IndexOf('.');
                        if ($i -lt 0) {
                            "$Code.$([string]::new(([char]'0'), $MinDecimalPlaces))" | Write-Output;
                        } else {
                            $i = $MinDecimalPlaces - ($Code.Length - $i - 1);
                            if ($i -gt 0) {
                                "$Code$([string]::new(([char]'0'), $i))" | Write-Output;
                            } else {
                                $Code | Write-Output;
                            }
                        }
                    }
                    break;
                }
                { $_ -is [decimal] } {
                    $Value = $InputObject;
                    if ($PSBoundParameters.ContainsKey('MaxDecimalPlaces')) { $Value = [Math]::Round($InputObject, $MaxDecimalPlaces) }
                    $Code = $Value.ToString();
                    if ($MinDecimalPlaces -eq 0 -or $Code.Contains('E')) {
                        if ($OmitNumericSuffix.IsPresent) {
                            $Code | Write-Output;
                        } else {
                            "$($Code)D" | Write-Output;
                        }
                    } else {
                        $i = $Code.IndexOf('.');
                        if ($i -lt 0) {
                            if ($OmitNumericSuffix.IsPresent) {
                                "$Code.$([string]::new(([char]'0'), $MinDecimalPlaces))" | Write-Output
                            } else {
                                "$Code.$([string]::new(([char]'0'), $MinDecimalPlaces))D" | Write-Output
                            };
                        } else {
                            $i = $MinDecimalPlaces - ($Code.Length - $i - 1);
                            if ($i -gt 0) {
                                if ($OmitNumericSuffix.IsPresent) {
                                    "$Code$([string]::new(([char]'0'), $i))" | Write-Output;
                                } else {
                                    "$Code$([string]::new(([char]'0'), $i))D" | Write-Output;
                                }
                            } else {
                                if ($OmitNumericSuffix.IsPresent) {
                                    $Code | Write-Output;
                                } else {
                                    "$($Code)D" | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
                { $_ -is [bool] } {
                    if ($InputObject) { '$true' | Write-Output } else { '$false' | Write-Output }
                    break;
                }
                { $_ -is [DateTime] } {
                    [DateTime]$DateTime = $InputObject;
                    if ($PSBoundParameters.ContainsKey('DateTimeUnspecifiedAs') -and $DateTime.Kind -eq [System.DateTimeKind]::Unspecified) {
                        if ($DateTimeUnspecifiedAs -eq 'Utc') {
                            $DateTime = [DateTime]::SpecifyKind($DateTime, [DateTimeKind]::Utc);
                        } else {
                            $DateTime = [DateTime]::SpecifyKind($DateTime, [DateTimeKind]::Local);
                        }
                        if ($PSBoundParameters.Contains('DateTimeKind')) {
                            if ($DateTimeKind -eq 'Local') {
                                if ($DateTime.Kind -eq [DateTimeKind]::Utc) { $DateTime = $DateTime.ToLocalTime(); break; }
                            } else {
                                if ($DateTime.Kind -eq [DateTimeKind]::Local) { $DateTime = $DateTime.ToUniversalTime(); break; }
                            }
                        }
                    } else {
                        if ($PSBoundParameters.Contains('DateTimeKind')) {
                            if ($DateTimeKind -eq 'Local') {
                                switch ($DateTime.Kind) {
                                    Utc { $DateTime = $DateTime.ToLocalTime(); break; }
                                    Unspecified { $DateTime = [DateTime]::SpecifyKind($DateTime, [DateTimeKind]::Local); break; }
                                }
                            } else {
                                switch ($DateTime.Kind) {
                                    Local { $DateTime = $DateTime.ToUniversalTime(); break; }
                                    Unspecified { $DateTime = [DateTime]::SpecifyKind($DateTime, [DateTimeKind]::Utc); break; }
                                }
                            }
                        }
                    }
                    if ($PSBoundParameters.ContainsKey('TruncateDateTime')) {
                        switch ($TruncateDateTime) {
                            'Microseconds' {
                                if ($DateTime.Nanosecond -ne 0) {
                                    $DateTime = [DateTime]::new($DateTime.Year, $DateTime.Month, $DateTime.Day, $DateTime.Hour, $DateTime.Minute, $DateTime.Second, $DateTime.Millisecond, $DateTime.Microsecond, $DateTime.Kind);
                                }
                                break;
                            }
                            'Milliseconds' {
                                if ($DateTime.Nanosecond -ne 0 -or $DateTime.Microseconds -ne 0) {
                                    $DateTime = [DateTime]::new($DateTime.Year, $DateTime.Month, $DateTime.Day, $DateTime.Hour, $DateTime.Minute, $DateTime.Second, $DateTime.Millisecond, 0, $DateTime.Kind);
                                }
                                break;
                            }
                            'Seconds' {
                                if ($DateTime.Nanosecond -ne 0 -or $DateTime.Microseconds -ne 0 -or $DateTime.Millisecond -ne 0) {
                                    $DateTime = [DateTime]::new($DateTime.Year, $DateTime.Month, $DateTime.Day, $DateTime.Hour, $DateTime.Minute, $DateTime.Second, 0, 0, $DateTime.Kind);
                                }
                                break;
                            }
                            'Minutes' {
                                if ($DateTime.Nanosecond -ne 0 -or $DateTime.Microseconds -ne 0 -or $DateTime.Millisecond -ne 0 -or $DateTime.Seconds -ne 0) {
                                    $DateTime = [DateTime]::new($DateTime.Year, $DateTime.Month, $DateTime.Day, $DateTime.Hour, $DateTime.Minute, 0, 0, 0, $DateTime.Kind);
                                }
                                break;
                            }
                            'Hours' {
                                if ($DateTime.Nanosecond -ne 0 -or $DateTime.Microseconds -ne 0 -or $DateTime.Millisecond -ne 0 -or $DateTime.Seconds -ne 0 -or $DateTime.Minutes -ne 0) {
                                    $DateTime = [DateTime]::new($DateTime.Year, $DateTime.Month, $DateTime.Day, $DateTime.Hour, 0, 0, 0, 0, $DateTime.Kind);
                                }
                                break;
                            }
                            'Days' {
                                if ($DateTime.Nanosecond -ne 0 -or $DateTime.Microseconds -ne 0 -or $DateTime.Millisecond -ne 0 -or $DateTime.Seconds -ne 0 -or $DateTime.Minutes -ne 0 -or $DateTime.Hours -ne 0) {
                                    $DateTime = [DateTime]::new($DateTime.Year, $DateTime.Month, $DateTime.Day, 0, 0, 0, 0, 0, $DateTime.Kind);
                                }
                                break;
                            }
                        }
                    }
                    if ($DateTime.Nanosecond -ne 0) {
                        "[DateTime]::new($($DateTime.Ticks)L, [DateTimeKind]::$($DateTime.Kind.ToString('F')))" | Write-Output;
                    } else {
                        if ($DateTime.Microsecond -ne 0) {
                            "[DateTime]::new($($DateTime.Year), $($DateTime.Month), $($DateTime.Day), $($DateTime.Hour), $($DateTime.Minute), $($DateTime.Second), $($DateTime.Millisecond), $($DateTime.Microsecond), [DateTimeKind]::$($DateTime.Kind.ToString('F')))" | Write-Output;
                        } else {
                            if ($DateTime.Millisecond -ne 0) {
                                "[DateTime]::new($($DateTime.Year), $($DateTime.Month), $($DateTime.Day), $($DateTime.Hour), $($DateTime.Minute), $($DateTime.Second), $($DateTime.Millisecond), [DateTimeKind]::$($DateTime.Kind.ToString('F')))" | Write-Output;
                            } else {
                                "[DateTime]::new($($DateTime.Year), $($DateTime.Month), $($DateTime.Day), $($DateTime.Hour), $($DateTime.Minute), $($DateTime.Second), [DateTimeKind]::$($DateTime.Kind.ToString('F')))" | Write-Output;
                            }
                        }
                    }
                    break;
                }
                { $_ -is [DateOnly] } {
                    "[DateOnly]::new($($InputObject.Year), $($InputObject.Month), $($InputObject.Day))" | Write-Output;
                    break;
                }
                { $_ -is [TimeOnly] } {
                    [TimeOnly]$TimeOnly = $InputObject;
                    if ($PSBoundParameters.ContainsKey('TruncateDateTime')) {
                        switch ($TruncateDateTime) {
                            'Microseconds' {
                                if ($TimeOnly.Nanosecond -ne 0) {
                                    $TimeOnly = [TimeOnly]::new($TimeOnly.Hour, $TimeOnly.Minute, $TimeOnly.Second, $TimeOnly.Millisecond, $TimeOnly.Microsecond);
                                }
                                break;
                            }
                            'Milliseconds' {
                                if ($TimeOnly.Nanosecond -ne 0 -or $TimeOnly.Microsecond -ne 0) {
                                    $TimeOnly = [TimeOnly]::new($TimeOnly.Hour, $TimeOnly.Minute, $TimeOnly.Second, $TimeOnly.Millisecond, 0);
                                }
                                break;
                            }
                            'Seconds' {
                                if ($TimeOnly.Nanosecond -ne 0 -or $TimeOnly.Microsecond -ne 0 -or $TimeOnly.Millisecond -ne 0) {
                                    $TimeOnly = [TimeOnly]::new($TimeOnly.Hour, $TimeOnly.Minute, $TimeOnly.Second, 0, 0);
                                }
                                break;
                            }
                            'Minutes' {
                                if ($TimeOnly.Nanosecond -ne 0 -or $TimeOnly.Microsecond -ne 0 -or $TimeOnly.Millisecond -ne 0 -or $TimeOnly.Second -ne 0) {
                                    $TimeOnly = [TimeOnly]::new($TimeOnly.Hour, $TimeOnly.Minute, 0, 0, 0);
                                }
                                break;
                            }
                            'Hours' {
                                if ($TimeOnly.Nanosecond -ne 0 -or $TimeOnly.Microsecond -ne 0 -or $TimeOnly.Millisecond -ne 0 -or $TimeOnly.Second -ne 0 -or $TimeOnly.Minute -ne 0) {
                                    $TimeOnly = [TimeOnly]::new($TimeOnly.Hour, 0, 0, 0, 0);
                                }
                                break;
                            }
                        }
                    }
                    if ($TimeOnly.Nanosecond -ne 0) {
                        "[TimeOnly]::new($($TimeOnly.Ticks)L)" | Write-Output;
                    } else {
                        if ($TimeOnly.Microsecond -ne 0) {
                            "[TimeOnly]::new($($TimeOnly.Hour), $($TimeOnly.Minute), $($TimeOnly.Second), $($TimeOnly.Millisecond), $($TimeOnly.Microsecond))" | Write-Output;
                        } else {
                            if ($TimeOnly.Millisecond -ne 0) {
                                "[TimeOnly]::new($($TimeOnly.Hour), $($TimeOnly.Minute), $($TimeOnly.Second), $($TimeOnly.Millisecond))" | Write-Output;
                            } else {
                                if ($TimeOnly.Second -ne 0) {
                                    "[TimeOnly]::new($($TimeOnly.Hour), $($TimeOnly.Minute), $($TimeOnly.Second))" | Write-Output;
                                } else {
                                    "[TimeOnly]::new($($TimeOnly.Hour), $($TimeOnly.Minute))" | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
                { $_ -is [Guid] } {
                    if ($InputObject -eq [Guid]::Empty) {
                        '[Guid]::Empty' | Write-Output;
                    } else {
                        "[Guid]::new('$($InputObject.ToString('d'))')" | Write-Output;
                    }
                    break;
                }
                { $_ -is [TimeSpan] } {
                    if ($InputObject -eq [TimeSpan]::Zero) {
                        '[TimeSpan]::Zero' | Write-Output;
                    } else {
                        [TimeSpan]$TimeSpan = $InputObject;
                        if ($PSBoundParameters.ContainsKey('TruncateDateTime')) {
                            switch ($TruncateDateTime) {
                                'Microseconds' {
                                    if ($TimeSpan.Nanosecond -ne 0) {
                                        $TimeSpan = [TimeSpan]::new($TimeSpan.Hour, $TimeSpan.Minute, $TimeSpan.Second, $TimeSpan.Millisecond, $TimeSpan.Microsecond);
                                    }
                                    break;
                                }
                                'Milliseconds' {
                                    if ($TimeSpan.Nanosecond -ne 0 -or $TimeSpan.Microsecond -ne 0) {
                                        $TimeSpan = [TimeSpan]::new($TimeSpan.Hour, $TimeSpan.Minute, $TimeSpan.Second, $TimeSpan.Millisecond, 0);
                                    }
                                    break;
                                }
                                'Seconds' {
                                    if ($TimeSpan.Nanosecond -ne 0 -or $TimeSpan.Microsecond -ne 0 -or $TimeSpan.Millisecond -ne 0) {
                                        $TimeSpan = [TimeSpan]::new($TimeSpan.Hour, $TimeSpan.Minute, $TimeSpan.Second, 0, 0);
                                    }
                                    break;
                                }
                                'Minutes' {
                                    if ($TimeSpan.Nanosecond -ne 0 -or $TimeSpan.Microsecond -ne 0 -or $TimeSpan.Millisecond -ne 0 -or $TimeSpan.Second -ne 0) {
                                        $TimeSpan = [TimeSpan]::new($TimeSpan.Hour, $TimeSpan.Minute, 0, 0, 0);
                                    }
                                    break;
                                }
                                'Hours' {
                                    if ($TimeSpan.Nanosecond -ne 0 -or $TimeSpan.Microsecond -ne 0 -or $TimeSpan.Millisecond -ne 0 -or $TimeSpan.Second -ne 0 -or $TimeSpan.Minute -ne 0) {
                                        $TimeSpan = [TimeSpan]::new($TimeSpan.Hour, 0, 0, 0, 0);
                                    }
                                    break;
                                }
                            }
                        }
                        if ($TimeSpan.Nanosecond -ne 0) {
                            "[TimeSpan]::new($($TimeSpan.Ticks)L)" | Write-Output;
                        } else {
                            if ($TimeSpan.Microsecond -ne 0) {
                                "[TimeSpan]::new($($TimeSpan.Hour), $($TimeSpan.Minute), $($TimeSpan.Second), $($TimeSpan.Millisecond), $($TimeSpan.Microsecond))" | Write-Output;
                            } else {
                                if ($TimeSpan.Millisecond -ne 0) {
                                    "[TimeSpan]::new($($TimeSpan.Hour), $($TimeSpan.Minute), $($TimeSpan.Second), $($TimeSpan.Millisecond))" | Write-Output;
                                } else {
                                    if ($TimeSpan.Second -ne 0) {
                                        "[TimeSpan]::new($($TimeSpan.Hour), $($TimeSpan.Minute), $($TimeSpan.Second))" | Write-Output;
                                    } else {
                                        "[TimeSpan]::new($($TimeSpan.Hour), $($TimeSpan.Minute), 0)" | Write-Output;
                                    }
                                }
                            }
                        }
                    }
                    break;
                }
                { $_ -is [Uri] } {
                    if (([Uri]$InputObject).IsAbsoluteUri) {
                        "[Uri]::new('$($InputObject.AbsoluteUri)', [UriKind]::Absolute)" | Write-Output;
                    } else {
                        "[Uri]::new('$($InputObject.OriginalString | ConvertTo-PsStringLiteral)', [UriKind]::Relative)" | Write-Output;
                    }
                    break;
                }
                { $_ -is [Version] } {
                    if ($InputObject.Revision -lt 0) {
                        if ($InputObject.Build -lt 0) {
                            "[Version]::new($($InputObject.Major), $($InputObject.Minor))" | Write-Output;
                        } else {
                            "[Version]::new($($InputObject.Major), $($InputObject.Minor), $($InputObject.Build))" | Write-Output;
                        }
                    } else {
                        "[Version]::new($($InputObject.Major), $($InputObject.Minor), $($InputObject.Build), $($InputObject.Revision))" | Write-Output;
                    }
                    break;
                }
                { $_ -is [System.Management.Automation.SemanticVersion] } {
                    if ([string]::IsNullOrEmpty($InputObject.BuildLabel)) {
                        if ([string]::IsNullOrEmpty($InputObject.PreReleaseLabel)) {
                            if ($InputObject.Patch -lt 0) {
                                "[semver]::new($($InputObject.Major), $($InputObject.Minor))" | Write-Output;
                            } else {
                                "[semver]::new($($InputObject.Major), $($InputObject.Minor), $($InputObject.Patch))" | Write-Output;
                            }
                        } else {
                            "[semver]::new($($InputObject.Major), $($InputObject.Minor), $($InputObject.Patch), $($InputObject.PreReleaseLabel | ConvertTo-PsStringLiteral))" | Write-Output;
                        }
                    } else {
                        "[semver]::new($($InputObject.Major), $($InputObject.Minor), $($InputObject.Patch), $($InputObject.PreReleaseLabel | ConvertTo-PsStringLiteral), $($InputObject.BuildLabel | ConvertTo-PsStringLiteral))" | Write-Output;
                    }
                    break;
                }
                { $_ -is [ScriptBlock] } {
                    "{$InputObject}" | Write-Output;
                    break;
                }
                { $_ -is [enum] } {
                    $Type = $InputObject.GetType();
                    $Tn = [System.Management.Automation.LanguagePrimitives]::ConvertTypeNameToPSTypeName($Type);
                    if ($Type.GetCustomAttributes([System.FlagsAttribute], $false).Length -gt 0) {
                        $Ut = [enum]::GetUnderlyingType($Type);
                        if ([System.Management.Automation.LanguagePrimitives]::ConvertTo($InputObject, $Ut) -eq 0) {
                            "$($Tn)::$($InputObject.ToString('F'))" | Write-Output;
                        } else {
                            $FlagValues = @([enum]::GetValues($Type) | Where-Object { $InputObject.HasFlag($_) -and [System.Management.Automation.LanguagePrimitives]::ConvertTo($_, $Ut) -ne 0 } | ForEach-Object {
                                "$($Tn)::$($_.ToString('F'))" | Write-Output;
                            });
                            if ($FlagValues.Count -lt 2) {
                                "$($Tn)::$($InputObject.ToString('F'))" | Write-Output;
                            } else {
                                "($Tn($($FlagValues -join ' -bor ')))" | Write-Output;
                            }
                        }
                    } else {
                        "$($Tn)::$($InputObject.ToString('F'))" | Write-Output;
                    }
                    break;
                }
                default {
                    switch ($StringQuotes) {
                        'PreferDouble' {
                            ($InputObject | ConvertTo-PsStringLiteral -PreferDoubleQuotes) | Write-Output;
                            break;
                        }
                        'AlwaysDouble' {
                            ($InputObject | ConvertTo-PsStringLiteral -AlwaysDoubleQuotes) | Write-Output;
                            break;
                        }
                        default {
                            ($InputObject | ConvertTo-PsStringLiteral -PreferSingleQuotes) | Write-Output;
                            break;
                        }
                    }
                    break;
                }
            }
        }
    }
}