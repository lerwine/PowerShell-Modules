class CharacterClass {
    [ValidateNotNullOrWhiteSpace()]
    [string]$Name;

    [ValidateNotNull()]
    [ScriptBlock]$FilterScript;

    [ValidateNotNull()]
    [System.Collections.ObjectModel.Collection[CharacterClass]]$RelatedClasses = [System.Collections.ObjectModel.Collection[CharacterClass]]::new();

    [ulong] GetFlags() { return [ulong]::MinValue }
    
    [bool] ShouldHaveAsciiChar() { return $false }

    [bool] HasAsciiChar() { return $false }

    [bool] ShouldHaveNonAsciiChar() { return $false }

    [bool] HasNonAsciiChar() { return $false }

    [bool] ShouldContainCategory([System.Globalization.UnicodeCategory]$Category) { return $false }

    [bool] ContainsCategory([System.Globalization.UnicodeCategory]$Category) { return $false }
}

class PrimaryCharacterClass : CharacterClass {
    [ValidateRange(0, 63)]
    [int]$BitIndex;

    [ValidateNotNull()]
    [System.Collections.ObjectModel.Collection[char]]$AllCharacters = [System.Collections.ObjectModel.Collection[char]]::new();

    [AllowNull()]
    [System.Globalization.UnicodeCategory[]]$AllCategories = $null;

    [AllowNull()]
    [System.Globalization.UnicodeCategory[]]$ContainedCategories = $null;

    [ValidateNotNull()]
    [ValidateCount(1, [int]::MaxValue)]
    [char[]]$TestCharacters;

    hidden [System.Nullable[bool]]$_ShouldHaveAsciiChar = $null;

    hidden [System.Nullable[bool]]$_HasAsciiChar = $null;

    hidden [System.Nullable[bool]]$_ShouldHaveNonAsciiChar = $null;

    hidden [System.Nullable[bool]]$_HasNonAsciiChar = $null;

    [ulong] GetFlags() { return ([ulong]1) -shl $this.BitIndex }
    
    [bool] ShouldHaveAsciiChar() {
        if ($null -ne $this._ShouldHaveAsciiChar) {
            $this._ShouldHaveAsciiChar = [char]::IsAscii($this.AllCharacters[0]);
            if ($this._ShouldHaveAsciiChar) {
                $this._ShouldHaveNonAsciiChar = $false;
                foreach ($c in ($this.AllCharacters | Select-Object -Skip 1)) {
                    if (-not [char]::IsAscii($c)) {
                        $this._ShouldHaveNonAsciiChar = $true;
                        break;
                    }
                }
            } else {
                $this._ShouldHaveNonAsciiChar = $true;
                foreach ($c in ($this.AllCharacters | Select-Object -Skip 1)) {
                    if ([char]::IsAscii($c)) {
                        $this._ShouldHaveAsciiChar = $true;
                        break;
                    }
                }
            }
        }
        return $this._ShouldHaveAsciiChar;
    }

    [bool] HasAsciiChar() {
        if ($null -ne $this._HasAsciiChar) {
            $this._HasAsciiChar = [char]::IsAscii($this.TestCharacters[0]);
            if ($this._HasAsciiChar) {
                $this._HasNonAsciiChar = $false;
                foreach ($c in ($this.TestCharacters | Select-Object -Skip 1)) {
                    if (-not [char]::IsAscii($c)) {
                        $this._HasNonAsciiChar = $true;
                        break;
                    }
                }
            } else {
                $this._HasNonAsciiChar = $true;
                foreach ($c in ($this.TestCharacters | Select-Object -Skip 1)) {
                    if ([char]::IsAscii($c)) {
                        $this._HasAsciiChar = $true;
                        break;
                    }
                }
            }
        }
        return $this._HasAsciiChar;
    }

    [bool] ShouldHaveNonAsciiChar() {
        if ($null -ne $this._ShouldHaveNonAsciiChar) {
            $this._ShouldHaveNonAsciiChar = -not [char]::IsAscii($this.AllCharacters[0]);
            if ($this._ShouldHaveNonAsciiChar) {
                $this._ShouldHaveAsciiChar = $false;
                foreach ($c in ($this.AllCharacters | Select-Object -Skip 1)) {
                    if ([char]::IsAscii($c)) {
                        $this._ShouldHaveAsciiChar = $true;
                        break;
                    }
                }
            } else {
                $this._ShouldHaveAsciiChar = $true;
                foreach ($c in ($this.AllCharacters | Select-Object -Skip 1)) {
                    if (-not [char]::IsAscii($c)) {
                        $this._ShouldHaveNonAsciiChar = $true;
                        break;
                    }
                }
            }
        }
        return $this._ShouldHaveNonAsciiChar;
    }

    [bool] HasNonAsciiChar() {
        if ($null -ne $this._HasNonAsciiChar) {
            $this._HasNonAsciiChar = -not [char]::IsAscii($this.TestCharacters[0]);
            if ($this._HasNonAsciiChar) {
                $this._HasAsciiChar = $false;
                foreach ($c in ($this.TestCharacters | Select-Object -Skip 1)) {
                    if ([char]::IsAscii($c)) {
                        $this._HasAsciiChar = $true;
                        break;
                    }
                }
            } else {
                $this._HasAsciiChar = $true;
                foreach ($c in ($this.TestCharacters | Select-Object -Skip 1)) {
                    if (-not [char]::IsAscii($c)) {
                        $this._HasNonAsciiChar = $true;
                        break;
                    }
                }
            }
        }
        return $this._HasNonAsciiChar;
    }

    [bool] ShouldContainCategory([System.Globalization.UnicodeCategory]$Category) {
        if ($null -eq $this.AllCategories) {
            $this.AllCategories = ($this.AllCharacters | ForEach-Object { [char]::GetUnicodeCategory($_) } | Sort-Object -Unique);
        }
        return $this.AllCategories -contains $Category;
    }

    [bool] ContainsCategory([System.Globalization.UnicodeCategory]$Category) {
        if ($null -eq $this.ContainedCategories) {
            $this.ContainedCategories = ($this.TestCharacters | ForEach-Object { [char]::GetUnicodeCategory($_) } | Sort-Object -Unique);
        }
        return $this.ContainedCategories -contains $Category;
    }
}

class AggregateCharacterClass : CharacterClass {
    [ValidateNotNull()]
    [ValidateCount(1, [int]::MaxValue)]
    [CharacterClass[]]$SubClasses;
    
    [ulong] GetFlags() {
        $Flags = [ulong]::MinValue;
        foreach ($c in $this.SubClasses) { $Flags = $Flags -bor $c.GetFlags() }
        return $Flags;
    }
    
    [bool] ShouldHaveAsciiChar() {
        foreach ($c in $this.SubClasses) {
            if ($c.ShouldHaveAsciiChar()) { return $true }
        }
        return $false;
    }

    [bool] HasAsciiChar() {
        foreach ($c in $this.SubClasses) {
            if ($c.HasAsciiChar()) { return $true }
        }
        return $false;
    }

    [bool] ShouldHaveNonAsciiChar() {
        foreach ($c in $this.SubClasses) {
            if ($c.ShouldHaveNonAsciiChar()) { return $true }
        }
        return $false;
    }

    [bool] HasNonAsciiChar() {
        foreach ($c in $this.SubClasses) {
            if ($c.HasNonAsciiChar()) { return $true }
        }
        return $false;
    }

    [bool] ShouldContainCategory([System.Globalization.UnicodeCategory]$Category) {
        foreach ($c in $this.SubClasses) {
            if ($c.ShouldContainCategory($Category)) { return $true }
        }
        return $false;
    }

    [bool] ContainsCategory([System.Globalization.UnicodeCategory]$Category) {
        foreach ($c in $this.SubClasses) {
            if ($c.ContainsCategory($Category)) { return $true }
        }
        return $false;
    }
}

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
    New-Variable -Name 'CommonCharacters' -Option ReadOnly -Scope 'Script' -Value ([System.Collections.ObjectModel.ReadOnlyCollection[char]]::new(([char[]]@("`t", "`n", ' ', '0', '9', 'A', 'F', 'N', 'Z',
        'a', 'f', 'n', 'z', '_', '(', '[', ']', '}', '+', '=', '$','^', '`', '˅', '˥',  '¢', '£', '⁀', '︴', '٤', '߁', 'À', 'Ç', 'µ', 'ß', 'ǅ', 'ᾫ', '©', '°', 'ʺ', 'ˇ', 'ǂ', 'ח', "`u{2160}", 'Ⅱ', '²',
        '¾', '֊', '־', '〰', '„', '⁅', '⁆', '⁾', '«', '“', '»', '”', '¡', '§', '±', '⅀', "`u{008c}", "`u{0090}", "`u{d806}", "`u{dc14}", "`u{0308}", "`u{0310}", "`u{0982}", "`u{09be}", "`u{20e2}",
        "`u{20e3}", "`u{2000}", "`u{3000}", "`u{2028}", "`u{2029}", "`u{0605}", "`u{fffb}", "`u{e00b}", "`u{e00e}", "`u{05ce}", "`u{05ec}"))));
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

Function Test-CharacterClass {
    [CmdletBinding()]
    [OutputType([bool])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [char[]]$InputValue,

        [CharacterClass]$Class,

        [switch]$Aggregated
    )

    Process {
        if ($Aggregated.IsPresent) {
            foreach ($c in $InputValue) {
                if (-not ($Class.FilterScript.InvokeWithContext($null, ([System.Collections.Generic.List[PSVariable]]@([PSVariable]::new('_', $c, [System.Management.Automation.ScopedItemOptions]::ReadOnly)))))) { return $false }
            }
        } else {
            foreach ($c in $InputValue) {
                if ($Class.FilterScript.InvokeWithContext($null, ([System.Collections.Generic.List[PSVariable]]@([PSVariable]::new('_', $c, [System.Management.Automation.ScopedItemOptions]::ReadOnly))))) {
                    $true | Write-Output;
                } else {
                    $false | Write-Output;
                }
            }
        }
    }

    End {
        if ($Aggregated.IsPresent) { $true | Write-Output }
    }
}

Function Get-AllCharacters {
    [CmdletBinding()]
    [OutputType([char[]])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        [CharacterClass[]]$InputClasses
    )

    Process {
        foreach ($c in $InputClasses) {
            if ($c -is [PrimaryCharacterClass]) {
                $c.AllCharacters | Write-Output;
            } else {
                $c.SubClasses | Get-TestCharacters;
            }
        }
    }
}

Function Get-TestCharacters {
    [CmdletBinding()]
    [OutputType([char[]])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [CharacterClass[]]$InputClasses
    )

    Process {
        foreach ($c in $InputClasses) {
            if ($c -is [PrimaryCharacterClass]) {
                $c.TestCharacters | Write-Output;
            } else {
                $c.SubClasses | Get-TestCharacters;
            }
        }
    }
}

Function Assert-UniqueReference {
    [CmdletBinding()]
    [OutputType([CharacterClass])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        [object[]]$InputObject,

        [swtich]$PassThru
    )

    Begin { $Emitted = @() }

    Process {
        if ($PassThru.IsPresent) {
            foreach ($obj in $InputObject) {
                foreach ($E in $Emitted) {
                    if ([object]::ReferenceEquals($E, $obj)) {
                        Write-Error -Message "Item of same reference already exists" -Category ResourceExists -ErrorId 'DupilcateReferenceDetected' `
                            -TargetObject $obj -CategoryActivity "ReferenceEquals" -CategoryReason "Current object is same instance as previous object" -ErrorAction Stop;
                    }
                }
                $obj | Write-Output;
                $Emitted += $obj;
            }
        } else {
            foreach ($obj in $InputObject) {
                foreach ($E in $Emitted) {
                    if ([object]::ReferenceEquals($E, $obj)) {
                        Write-Error -Message "Item of same reference already exists" -Category ResourceExists -ErrorId 'DupilcateReferenceDetected' `
                            -TargetObject $obj -CategoryActivity "ReferenceEquals" -CategoryReason "Current object is same instance as previous object" -ErrorAction Stop;
                    }
                }
                $Emitted += $obj;
            }
        }
    }
}

Function Assert-ValidRelatedClassHierarchy {
    [CmdletBinding()]
    [OutputType([CharacterClass])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        [CharacterClass[]]$InputClasses,

        [switch]$ReturnRelated
    )

    Process {
        if ($ReturnRelated.IsPresent) {
            foreach ($CharacterClass in $InputClasses) {
                try { $CharacterClass.RelatedClasses | Assert-UniqueReference -ErrorAction Stop }
                catch {
                    Write-Error -Exception $_.Exception -Message "Character class $($CharacterClass.Name) has duplicate related classes" -Category $_.CategoryInfo.Category -ErrorId 'DuplicateRelatedClass' `
                        -TargetObject $_.TargetObject -CategoryActivity $_.CategoryInfo.Activity -CategoryReason 'Assert-UniqueReference threw an error on RelatedClasses property' -ErrorAction Stop;
                }
                foreach ($RelatedClass in $CharacterClass.RelatedClasses) {
                    if ([object]::ReferenceEquals($CharacterClass, $RelatedClass)) {
                        Write-Error -Message "Character class $($CharacterClass.Name) includes a circular reference to itself" -Category DeadlockDetected -ErrorId 'CircularReferenceDetected' `
                            -TargetObject $CharacterClass -CategoryActivity "ReferenceEquals" -CategoryReason "RelatedClasses contains same instance as parent character class" -ErrorAction Stop;
                    }
                    $RelatedClass | Write-Output;
                    $SubRelated = @($RelatedClass | Assert-ValidRelatedClassHierarchy -ReturnRelated);
                    foreach ($r in $SubRelated) {
                        if ([object]::ReferenceEquals($CharacterClass, $r)) {
                            Write-Error -Message "Character class $($CharacterClass.Name) includes a circular reference to itself within $($RelatedClass.Name)" -Category DeadlockDetected -ErrorId 'CircularReferenceDetected' `
                                -TargetObject $RelatedClass -CategoryActivity "ReferenceEquals" -CategoryReason "Nested RelatedClasses contains same instance as parent character class" -ErrorAction Stop;
                        }
                        $r | Write-Output;
                    }
                }
            }
        } else {
            foreach ($CharacterClass in $InputClasses) {
                try { $CharacterClass.RelatedClasses | Assert-UniqueReference -ErrorAction Stop }
                catch {
                    Write-Error -Exception $_.Exception -Message "Character class $($CharacterClass.Name) has duplicate related classes" -Category $_.CategoryInfo.Category -ErrorId 'DuplicateRelatedClass' `
                        -TargetObject $_.TargetObject -CategoryActivity $_.CategoryInfo.Activity -CategoryReason 'Assert-UniqueReference threw an error on RelatedClasses property' -ErrorAction Stop;
                }
                foreach ($RelatedClass in $CharacterClass.RelatedClasses) {
                    if ([object]::ReferenceEquals($CharacterClass, $RelatedClass)) {
                        Write-Error -Message "Character class $($CharacterClass.Name) includes a circular reference to itself" -Category DeadlockDetected -ErrorId 'CircularReferenceDetected' `
                            -TargetObject $CharacterClass -CategoryActivity "ReferenceEquals" -CategoryReason "RelatedClasses contains same instance as parent character class" -ErrorAction Stop;
                    }
                    foreach ($r in ($RelatedClass | Assert-ValidRelatedClassHierarchy -ReturnRelated)) {
                        if ([object]::ReferenceEquals($CharacterClass, $r)) {
                            Write-Error -Message "Character class $($CharacterClass.Name) includes a circular reference to itself within $($RelatedClass.Name)" -Category DeadlockDetected -ErrorId 'CircularReferenceDetected' `
                                -TargetObject $RelatedClass -CategoryActivity "ReferenceEquals" -CategoryReason "Nested RelatedClasses contains same instance as parent character class" -ErrorAction Stop;
                        }
                    }
                }
            }
        }
    }
}

Function Assert-ValidSubClassHierarchy {
    [CmdletBinding()]
    [OutputType([CharacterClass])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        [AggregateCharacterClass[]]$InputClasses,

        [switch]$ReturnSub
    )

    Process {
        if ($ReturnSub.IsPresent) {
            foreach ($CharacterClass in $InputClasses) {
                try { $CharacterClass.SubClasses | Assert-UniqueReference -ErrorAction Stop }
                catch {
                    Write-Error -Exception $_.Exception -Message "Character class $($CharacterClass.Name) has duplicate sub-classes" -Category $_.CategoryInfo.Category -ErrorId 'DuplicateSubClass' `
                        -TargetObject $_.TargetObject -CategoryActivity $_.CategoryInfo.Activity -CategoryReason 'Assert-UniqueReference threw an error on SubClasses property' -ErrorAction Stop;
                }
                foreach ($RelatedClass in ($CharacterClass.SubClasses | Where-Object { $_ -is [AggregateCharacterClass] })) {
                    if ([object]::ReferenceEquals($CharacterClass, $RelatedClass)) {
                        Write-Error -Message "Character class $($CharacterClass.Name) includes a circular sub-class reference to itself" -Category DeadlockDetected -ErrorId 'CircularReferenceDetected' `
                            -TargetObject $CharacterClass -CategoryActivity "ReferenceEquals" -CategoryReason "SubClasses contains same instance as parent character class" -ErrorAction Stop;
                    }
                    $RelatedClass | Write-Output;
                    $SubRelated = @($RelatedClass | Assert-ValidSubClassHierarchy -ReturnSub);
                    foreach ($r in $SubRelated) {
                        if ([object]::ReferenceEquals($CharacterClass, $r)) {
                            Write-Error -Message "Character class $($CharacterClass.Name) includes a circular sub-class reference to itself within $($RelatedClass.Name)" -Category DeadlockDetected -ErrorId 'CircularReferenceDetected' `
                                -TargetObject $RelatedClass -CategoryActivity "ReferenceEquals" -CategoryReason "Nested SubClasses contains same instance as parent character class" -ErrorAction Stop;
                        }
                        $r | Write-Output;
                    }
                }
            }
        } else {
            foreach ($CharacterClass in $InputClasses) {
                foreach ($RelatedClass in ($CharacterClass.SubClasses | Where-Object { $_ -is [AggregateCharacterClass] })) {
                    if ([object]::ReferenceEquals($CharacterClass, $RelatedClass)) {
                        Write-Error -Message "Character class $($CharacterClass.Name) includes a circular sub-class reference to itself" -Category DeadlockDetected -ErrorId 'CircularReferenceDetected' `
                            -TargetObject $CharacterClass -CategoryActivity "ReferenceEquals" -CategoryReason "SubClasses contains same instance as parent character class" -ErrorAction Stop;
                    }
                    foreach ($r in ($RelatedClass | Assert-ValidSubClassHierarchy -ReturnSub)) {
                        if ([object]::ReferenceEquals($CharacterClass, $r)) {
                            Write-Error -Message "Character class $($CharacterClass.Name) includes a circular sub-class reference to itself within $($RelatedClass.Name)" -Category DeadlockDetected -ErrorId 'CircularReferenceDetected' `
                                -TargetObject $RelatedClass -CategoryActivity "ReferenceEquals" -CategoryReason "Nested SubClasses contains same instance as parent character class" -ErrorAction Stop;
                        }
                    }
                }
            }
        }
    }
}

Function Get-RelatedClasses {
    [CmdletBinding()]
    [OutputType([CharacterClass])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        [CharacterClass[]]$InputClasses,

        [switch]$Recurse,

        [switch]$CheckCircularReferences
    )

    Process {
        if ($Recurse.IsPresent) {
            foreach ($CharacterClass in $InputClasses) {
                foreach ($RelatedClass in $CharacterClass.RelatedClasses) {
                    $RelatedClass | Write-Output;
                    $RelatedClass | Get-RelatedClasses -Recurse;
                }
            }
        } else {
            foreach ($CharacterClass in $InputClasses) {
                $CharacterClass.RelatedClasses | Write-Output;
            }
        }
    }
}

Function Assert-ValidCharacterClass {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        [CharacterClass[]]$InputClasses
    )

    Process {
        foreach ($CharacterClass in $InputClasses) {
            $AllChars = @($CharacterClass | Get-AllCharacters);
            foreach ($c in $AllChars) {
                if (-not ($c | Test-CharacterClass -Class $CharacterClass)) {
                    Write-Error -Message "FilterScript for character class $($CharacterClass.Name) fails to match character $($c | ConvertTo-PsCharLiteral): $($CharacterClass.FilterScript)" -Category InvalidOperation -ErrorId 'FilterScriptValidationFailure' `
                        -TargetObject $c -CategoryActivity "Test-CharacterClass" -CategoryReason "Test-CharacterClass using $($CharacterClass.Name) returned false" -ErrorAction Stop;
                }
            }
            if ($CharacterClass -is [PrimaryCharacterClass]) {
                foreach ($c in ($CharacterClass | Get-TestCharacters)) {
                    if ($AllChars -inotcontains $c) {
                        Write-Error -Message "Test character $($c | ConvertTo-PsCharLiteral) does not belong to character class $($CharacterClass.Name)" -Category InvalidData -ErrorId 'TestCharacterValidationFailure' `
                            -TargetObject $c -CategoryActivity "-inotcontains" -CategoryReason "Results of Get-AllCharacters for $($CharacterClass.Name) does not contain test character" -ErrorAction Stop;
                    }
                }
                if ($CharacterClass.ShouldHaveAsciiChar()) {
                    if (-not $CharacterClass.HasAsciiChar()) {
                        Write-Error -Message "Character class $($CharacterClass.Name) includes one or more ASCII characters, but the test characters do not" -Category InvalidData -ErrorId 'TestCharacterValidationFailure' `
                            -TargetObject $c -CategoryActivity "HasAsciiChar()" -CategoryReason "ShouldHaveAsciiChar() retrned true, but HasAsciiChar() returned false" -ErrorAction Stop;
                    }
                }
                if ($CharacterClass.ShouldHaveNonAsciiChar()) {
                    if (-not $CharacterClass.HasNonAsciiChar()) {
                        Write-Error -Message "Character class $($CharacterClass.Name) includes one or more non-ASCII characters, but the test characters do not" -Category InvalidData -ErrorId 'TestCharacterValidationFailure' `
                            -TargetObject $c -CategoryActivity "HasNonAsciiChar()" -CategoryReason "ShouldHaveNonAsciiChar() retrned true, but HasNonAsciiChar() returned false" -ErrorAction Stop;
                    }
                }
            } else {
                $CharacterClass | Assert-ValidSubClassHierarchy -ErrorAction Stop;
            }
            $CharacterClass | Assert-ValidRelatedClassHierarchy -ErrorAction Stop;
            foreach ($RelatedClass in ($CharacterClass | Get-RelatedClasses)) {
                foreach ($c in $AllChars) {
                    if ($c | Test-CharacterClass -Class $RelatedClass) {
                        if (($RelatedClass | Get-AllCharacters) -contains $c) {
                            Write-Error -Message "Character class $($CharacterClass.Name) and related class $($RelatedClass.Name) both include the $($c | ConvertTo-PsCharLiteral) character" -Category InvalidData -ErrorId 'RelatedCharacterValidationFailure' `
                                -TargetObject $c -CategoryActivity "Test-CharacterClass" -CategoryReason "Detected overlap between character classes $($CharacterClass.Name) and $($RelatedClass.Name)" -ErrorAction Stop;
                        }
                    }
                }
            }
        }
    }
}

Function New-PrimaryCharacterClass {
    [CmdletBinding()]
    [OutputType([PrimaryCharacterClass])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [ValidatePattern('^[A-Z][a-z]+$')]
        [string]$Name,
        
        [Parameter(Mandatory = $true, Position = 1)]
        [ValidateRange(0, 63)]
        [int]$BitIndex,

        [Parameter(Mandatory = $true, Position = 2)]
        [ScriptBlock]$FilterScript,

        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [char[]]$InputCharacters,

        [Hashtable]$AddTo,

        [switch]$PassThru
    )
    
    Begin { $AllCharacters = [System.Collections.ObjectModel.Collection[char]]::new() }

    Process {
        foreach ($c in $InputCharacters) { $AllCharacters.Add($c) }
    }

    End {
        $CharacterClass = [PrimaryCharacterClass]@{
            Name = $Name;
            BitIndex = $BitIndex;
            FilterScript = $FilterScript;
            TestCharacters = ([char[]]($AllCharacters | Sort-Object -Unique -CaseSensitive));
        };
        if ($PSBoundParameters.ContainsKey('AddTo')) {
            $AddTo.Add($Name, $CharacterClass);
            if ($PassThru.IsPresent) { $CharacterClass | Write-Output }
        } else {
            $CharacterClass | Write-Output;
        }
    }
}

Function New-AggregateCharacterClass {
    [CmdletBinding(DefaultParameterSetName = 'Default')]
    [OutputType([AggregateCharacterClass])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [ValidatePattern('^[A-Z][a-z]+$')]
        [string]$Name,
        
        [Parameter(Mandatory = $true, Position = 2)]
        [ScriptBlock]$FilterScript,

        [Parameter(ValueFromPipeline = $true)]
        [CharacterClass[]]$SubClasses,

        [Hashtable]$AddTo,

        [switch]$PassThru
    )
    
    Begin { $AllClasses = @{} }

    Process {
        if ($PSBoundParameters.ContainsKey('SubClasses')) {
            foreach ($c in $SubClasses) {
                if ($AllClasses.ContainsKey($c.Name)) {
                    if (-not [object]::ReferenceEquals($AllClasses[$c.Name], $c)) {
                        Write-Error -Message "Another sub-class with the name $($.Name) was already included.";
                    }
                } else {
                    $AllClasses.Add($c.Name, $c);
                }
            }
        }
    }

    End {
        $CharacterClass = [AggregateCharacterClass]@{
            Name = $Name;
            FilterScript = $FilterScript;
            SubClasses = ($AllClasses.Values | Sort-Object -Property @{ Expression = { $_.GetFlags() }});
        };
        if ($PSBoundParameters.ContainsKey('AddTo')) {
            $AddTo.Add($Name, $CharacterClass);
            if ($PassThru.IsPresent) { $CharacterClass | Write-Output }
        } else {
            $CharacterClass | Write-Output;
        }
    }
}

Function Add-SimilarCharacterClass {
    [CmdletBinding(DefaultParameterSetName = 'Default')]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [PrimaryCharacterClass]$CharacterClass,
        
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [CharacterClass[]]$InputObject
    )
}

Function Initialize-CharacterClasses {
    [CmdletBinding()]
    Param()

    $CharacterClassTable = @{};

    New-PrimaryCharacterClass -Name 'HexLetterUpper'                -BitIndex 0 -FilterScript { [char]::IsAsciiHexDigitUpper($_) -and -not [char]::IsAsciiDigit($_) } `
        -InputCharacters 'A', 'B', 'C', 'D', 'E', 'F' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'HexLetterLower'                -BitIndex 1 -FilterScript { [char]::IsAsciiHexDigitLower($_) -and -not [char]::IsAsciiDigit($_) } `
        -InputCharacters 'a', 'b', 'c', 'd', 'e', 'f' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'NonHexAsciiLetterUpper'        -BitIndex 2 -FilterScript { [char]::IsAsciiLetterUpper($_) -and -not [char]::IsAsciiHexDigitUpper($_) } `
        -InputCharacters 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'NonAsciiLetterUpper'           -BitIndex 3 -FilterScript  { [char]::IsUpper($_) -and -not [char]::IsAsciiLetterUpper($_) } `
        -InputCharacters 'À', 'Æ', 'Ç', 'È', 'Í', 'Ø', 'Ù' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'NonHexAsciiLetterLower'        -BitIndex 4 -FilterScript { [char]::IsAsciiLetterLower($_) -and -not [char]::IsAsciiHexDigitLower($_) } `
        -InputCharacters 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'NonAsciiLetterLower'           -BitIndex 5 -FilterScript { [char]::IsLower($_) -and -not [char]::IsAsciiLetterLower($_) } `
        -InputCharacters 'µ', 'ß', 'à', 'æ', 'ç', 'è', 'í', 'ö' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'TitlecaseLetter'               -BitIndex 6 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::TitlecaseLetter } `
        -InputCharacters 'ǅ', 'ǈ', 'ᾈ', 'ᾚ', 'ᾫ' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'ModifierLetter'                -BitIndex 7 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierLetter } `
        -InputCharacters 'ʰ', 'ʺ', 'ˇ', 'ˉ', 'ˌ' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'OtherLetter'                   -BitIndex 8 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherLetter } `
        -InputCharacters 'ª', 'ƻ', 'ǁ', 'ǂ', 'ח', 'ך', 'נ' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'BinaryDigitNumber'             -BitIndex 9 -FilterScript { $_ -eq '0' -or $_ -eq '1' } `
        -InputCharacters '0', '1' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'NonBinaryOctalDigit'           -BitIndex 10 -FilterScript { $_ -gt '1' -and $_ -le '7' } `
        -InputCharacters '2', '3', '4', '5', '6', '7' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'NonOctalAsciiDigit'            -BitIndex 11 -FilterScript { $_ -eq '8' -or $_ -eq '9' } `
        -InputCharacters '8', '9' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'NonAsciiDigit'                 -BitIndex 12 -FilterScript { [char]::IsDigit($_) -and -not [char]::IsAsciiDigit($_) } `
        -InputCharacters '٤', '۶', '۸', '߁', '߂', '߃', '߄' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'LetterNumber'                  -BitIndex 13 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::LetterNumber } `
        -InputCharacters 'ᛯ', 'ᛰ', "`u{2160}", 'Ⅱ', 'Ⅳ', "`u{2164}", "`u{2169}", "`u{216c}", "`u{216d}", "`u{216e}", "`u{216f}", "`u{2170}", 'ⅱ', 'ⅳ', "`u{2174}" -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'OtherNumber'                   -BitIndex 14 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherNumber } `
        -InputCharacters '²', '¾', '৹', '௱', '౹', '౺' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'NonSpacingMark'                -BitIndex 15 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::NonSpacingMark } `
        -InputCharacters "`u{0300}", "`u{0308}", "`u{0310}", "`u{0314}", "`u{0317}" -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'SpacingCombiningMark'          -BitIndex 16 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpacingCombiningMark } `
        -InputCharacters "`u{0903}", "`u{093b}", "`u{0949}", "`u{094f}", "`u{0982}", "`u{09be}", "`u{09c8}", "`u{0a3e}" -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'EnclosingMark'                 -BitIndex 17 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::EnclosingMark } `
        -InputCharacters "`u{1abe}", "`u{20dd}", "`u{20de}", "`u{20df}", "`u{20e0}", "`u{20e2}", "`u{20e3}", "`u{20e4}" -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'AsciiConnectorPunctuation'     -BitIndex 18 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation -and [char]::IsAscii($_) } `
        -InputCharacters '_' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'NonAsciiConnectorPunctuation'  -BitIndex 19 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation -and -not [char]::IsAscii($_) } `
        -InputCharacters '‿', '⁀', '⁔', '︳', '︴' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'AsciiDashPunctuation'          -BitIndex 20 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::DashPunctuation -and [char]::IsAscii($_) } `
        -InputCharacters '-' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'NonAsciiDashPunctuation'       -BitIndex 21 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::DashPunctuation -and -not [char]::IsAscii($_) } `
        -InputCharacters '֊', '־', '᠆', '—', '⸺', '〰', '﹣' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'AsciiOpenPunctuation'          -BitIndex 22 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation -and [char]::IsAscii($_) } `
        -InputCharacters '(', '[', '{' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'NonAsciiOpenPunctuation'       -BitIndex 23 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation -and -not [char]::IsAscii($_) } `
        -InputCharacters '„', '⁅', '⁽', '❪', '❬', '❰', '⟅', '⟪', '⦃' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'AsciiClosePunctuation'         -BitIndex 24 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation -and [char]::IsAscii($_) } `
        -InputCharacters ')', ']', '}' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'NonAsciiClosePunctuation'      -BitIndex 25 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation -and -not [char]::IsAscii($_) } `
        -InputCharacters '⁆', '⁾', '❫', '❭', '❱', '⟆', '⟫', '⦄' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'InitialQuotePunctuation'       -BitIndex 26 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::InitialQuotePunctuation } `
        -InputCharacters '«', '“', '‟', '⸂', '⸄', '⸉', '⸌', '⸜', '⸠' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'FinalQuotePunctuation'         -BitIndex 27 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::FinalQuotePunctuation } `
        -InputCharacters '»', '”', '⸃', '⸅', '⸊', '⸍', '⸝', '⸡' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'OtherAsciiPunctuation'         -BitIndex 28 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation -and [char]::IsAscii($_) } `
        -InputCharacters '!', '"', '#', '%', '&', "'", '*', ',', '.', '/', ':', ';', '?', '@', '\' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'OtherNonAsciiPunctuation'      -BitIndex 29 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation -and -not [char]::IsAscii($_) } `
        -InputCharacters '¡', '§', '¿', '՟', '׆', '؉', '؛', '؝' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'AsciiMathSymbol'               -BitIndex 30 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::MathSymbol -and [char]::IsAscii($_) } `
        -InputCharacters '+', '<', '=', '>', '|', '~' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'NonAsciiMathSymbol'            -BitIndex 31 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::MathSymbol -and -not [char]::IsAscii($_) } `
        -InputCharacters '±', '÷', '϶', '⅀', '⅁', '⅂', '⅃', '⅄', '←', '↑' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'AsciiCurrencySymbol'           -BitIndex 32 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol -and [char]::IsAscii($_) } `
        -InputCharacters '$' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'NonAsciiCurrencySymbol'        -BitIndex 33 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol -and -not [char]::IsAscii($_) } `
        -InputCharacters '¢', '£', '¤', '¥', '֏', '؋', '฿', '₤', '₩' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'AsciiModifierSymbol'           -BitIndex 34 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol -and [char]::IsAscii($_) } `
        -InputCharacters '^', '`' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'NonAsciiModifierSymbol'        -BitIndex 35 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol -and -not [char]::IsAscii($_) } `
        -InputCharacters '˅', '˓', '˔', '˖', '˘', '˚', '˝', '˥' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'OtherSymbol'                   -BitIndex 36 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherSymbol -and -not [char]::IsAscii($_) } `
        -InputCharacters '©', '®', '°', '҂', '۞', '۩', '۾', '߶', '৺', '୰', '౿' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'AsciiSpaceSeparator'           -BitIndex 37 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator -and [char]::IsAscii($_) } `
        -InputCharacters ' ' -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'NonAsciiSpaceSeparator'        -BitIndex 38 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator -and -not [char]::IsAscii($_) } `
        -InputCharacters "`u{00a0}", "`u{1680}", "`u{2000}", "`u{2008}", "`u{200a}", "`u{202f}", "`u{205f}", "`u{3000}" -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'LineSeparator'                 -BitIndex 39 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::LineSeparator -and -not [char]::IsAscii($_) } `
        -InputCharacters "`u{2028}" -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'ParagraphSeparator'            -BitIndex 40 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ParagraphSeparator -and -not [char]::IsAscii($_) } `
        -InputCharacters "`u{2029}" -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'AsciiControl'                  -BitIndex 41 -FilterScript { [char]::IsControl($_) -and [char]::IsAscii($_) } `
        -InputCharacters "`t", "`n", "`v", "`f", "`r" -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'NonAsciiControl'               -BitIndex 42 -FilterScript { [char]::IsControl($_) -and -not [char]::IsAscii($_) } `
        -InputCharacters "`u{0080}", "`u{0084}", "`u{0088}", "`u{008c}", "`u{0090}", "`u{009c}", "`u{009f}" -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'Format'                        -BitIndex 43 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::Format } `
        -InputCharacters "`u{00ad}", "`u{0601}", "`u{0605}", "`u{061c}", "`u{070f}", "`u{200d}", "`u{2060}", "`u{206a}", "`u{206f}", "`u{feff}", "`u{fffb}" -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'HighSurrogate'                 -BitIndex 44 -FilterScript { [char]::IsHighSurrogate($_) } `
        -InputCharacters "`u{d800}", "`u{d803}", "`u{d806}", "`u{d809}", "`u{d80c}", "`u{d80f}", "`u{d812}", "`u{d815}", "`u{d818}" -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'LowSurrogate'                  -BitIndex 45 -FilterScript { [char]::IsLowSurrogate($_) } `
        -InputCharacters "`u{dc00}", "`u{dc02}", "`u{dc05}", "`u{dc08}", "`u{dc0b}", "`u{dc0e}", "`u{dc11}", "`u{dc14}", "`u{dc17}", "`u{dc18}" -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'PrivateUse'                    -BitIndex 46 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::PrivateUse } `
        -InputCharacters "`u{e002}", "`u{e005}", "`u{e008}", "`u{e00b}", "`u{e00e}", "`u{e011}", "`u{e014}", "`u{e017}", "`u{e018}" -AddTo $CharacterClassTable;
    New-PrimaryCharacterClass -Name 'OtherNotAssigned'              -BitIndex 47 -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherNotAssigned } `
        -InputCharacters "`u{0378}", "`u{0381}", "`u{038b}", "`u{0530}", "`u{058b}", "`u{05c8}", "`u{05cb}", "`u{05ce}", "`u{05ec}" -AddTo $CharacterClassTable;
    
    New-AggregateCharacterClass -Name 'HexLetter' -FilterScript { [char]::IsAsciiHexDigit($_) -and -not [char]::IsAsciiDigit($_) } `
        -SubClasses $CharacterClassTable['HexLetterUpper'], $CharacterClassTable['HexLetterLower'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'AsciiLetterUpper' -FilterScript { [char]::IsAsciiLetterUpper($_) } `
        -SubClasses $CharacterClassTable['HexLetterUpper'], $CharacterClassTable['NonHexAsciiLetterUpper'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'UppercaseLetter' -FilterScript { [char]::IsLower($_) -and -not [char]::IsAsciiLetterUpper($_) } `
        -SubClasses $CharacterClassTable['AsciiLetterUpper'], $CharacterClassTable['NonAsciiLetterUpper'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'AsciiLetterLower' -FilterScript { [char]::IsAsciiLetterLower($_) } `
        -SubClasses $CharacterClassTable['HexLetterLower'], $CharacterClassTable['NonHexAsciiLetterLower'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'NonHexAsciiLetter' -FilterScript { [char]::IsAsciiLetter($_) -and -not [char]::IsAsciiHexDigit($_) } `
        -SubClasses $CharacterClassTable['NonHexAsciiLetterUpper'], $CharacterClassTable['NonHexAsciiLetterLower'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'AsciiLetter' -FilterScript { [char]::IsAsciiLetter($_) } `
        -SubClasses $CharacterClassTable['HexLetterUpper'], $CharacterClassTable['HexLetterLower'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'LowercaseLetter' -FilterScript { [char]::IsUpper($_) -and -not [char]::IsAsciiLetterLower($_) } `
        -SubClasses $CharacterClassTable['AsciiLetterLower'], $CharacterClassTable['NonAsciiLetterLower'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'NonAsciiLetter' -FilterScript { [char]::IsLetter($_) -and -not [char]::IsAsciiLetter($_) } `
        -SubClasses $CharacterClassTable['NonAsciiLetterUpper'], $CharacterClassTable['NonAsciiLetterLower'], $CharacterClassTable['TitlecaseLetter'], $CharacterClassTable['ModifierLetter'], $CharacterClassTable['OtherLetter'] `
        -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'Letter' -FilterScript { [char]::IsLetter($_) } `
        -SubClasses $CharacterClassTable['AsciiLetter'], $CharacterClassTable['NonAsciiLetter'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'OctalDigit' -FilterScript { $_ -ge '0' -and $_ -le '7' } `
        -SubClasses $CharacterClassTable['BinaryDigitNumber'], $CharacterClassTable['NonBinaryOctalDigit'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'AsciiDigit' -FilterScript { [char]::IsAsciiDigit($_) } `
        -SubClasses $CharacterClassTable['OctalDigit'], $CharacterClassTable['NonOctalAsciiDigit'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'AsciiHexDigitUpper' -FilterScript { [char]::IsAsciiHexDigitUpper($_) } `
        -SubClasses $CharacterClassTable['AsciiDigit'], $CharacterClassTable['HexLetterUpper'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'AsciiHexDigitLower' -FilterScript { [char]::IsAsciiHexDigitLower($_) } `
        -SubClasses $CharacterClassTable['AsciiDigit'], $CharacterClassTable['HexLetterLower'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'AsciiHexDigit' -FilterScript { [char]::IsAsciiHexDigit($_) } `
        -SubClasses $CharacterClassTable['AsciiDigit'], $CharacterClassTable['HexLetter'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'AsciiLetterOrDigit' -FilterScript { [char]::IsAsciiLetterOrDigit($_) } `
        -SubClasses $CharacterClassTable['AsciiDigit'], $CharacterClassTable['AsciiLetter'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'NonAsciiLetterOrDigit' -FilterScript { [char]::IsAsciiLetterOrDigit($_) } `
        -SubClasses $CharacterClassTable['NonAsciiDigit'], $CharacterClassTable['NonAsciiLetter'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'Digit' -FilterScript { [char]::IsDigit($_) } `
        -SubClasses $CharacterClassTable['AsciiDigit'], $CharacterClassTable['NonAsciiDigit'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'LetterOrDigit' -FilterScript { [char]::IsLetterOrDigit($_) } `
        -SubClasses $CharacterClassTable['AsciiLetterOrDigit'], $CharacterClassTable['NonAsciiLetterOrDigit'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'NonAsciiNumber' -FilterScript { [char]::IsNumber($_) -and -not [char]::IsAscii($_) } `
        -SubClasses $CharacterClassTable['NonAsciiDigit'], $CharacterClassTable['LetterNumber'], $CharacterClassTable['OtherNumber'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'Number' -FilterScript { [char]::IsNumber($_) } `
        -SubClasses $CharacterClassTable['AsciiDigit'], $CharacterClassTable['NonAsciiNumber'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'Mark' -FilterScript { switch ([char]::GetUnicodeCategory($_)) { NonSpacingMark { return $true } SpacingCombiningMark { return $true } EnclosingMark { return $true } default { return $false } } } `
        -SubClasses $CharacterClassTable['NonSpacingMark'], $CharacterClassTable['SpacingCombiningMark'], $CharacterClassTable['EnclosingMark'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'ConnectorPunctuation' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ConnectorPunctuation } `
        -SubClasses $CharacterClassTable['AsciiConnectorPunctuation'], $CharacterClassTable['NonAsciiConnectorPunctuation'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'DashPunctuation' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::DashPunctuation } `
        -SubClasses $CharacterClassTable['AsciiDashPunctuation'], $CharacterClassTable['NonAsciiDashPunctuation'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'OpenPunctuation' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OpenPunctuation } `
        -SubClasses $CharacterClassTable['AsciiOpenPunctuation'], $CharacterClassTable['NonAsciiOpenPunctuation'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'ClosePunctuation' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ClosePunctuation } `
        -SubClasses $CharacterClassTable['AsciiClosePunctuation'], $CharacterClassTable['NonAsciiClosePunctuation'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'AsciiPunctuation' -FilterScript { [char]::IsPunctuation($_) -and [char]::IsAscii($_) } `
        -SubClasses $CharacterClassTable['AsciiConnectorPunctuation'], $CharacterClassTable['AsciiDashPunctuation'], $CharacterClassTable['AsciiOpenPunctuation'], $CharacterClassTable['AsciiClosePunctuation'],
        $CharacterClassTable['OtherAsciiPunctuation'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'NonAsciiPunctuation' -FilterScript { [char]::IsPunctuation($_) -and -not [char]::IsAscii($_) } `
        -SubClasses $CharacterClassTable['NonAsciiConnectorPunctuation'], $CharacterClassTable['NonAsciiDashPunctuation'], $CharacterClassTable['NonAsciiOpenPunctuation'], $CharacterClassTable['NonAsciiClosePunctuation'],
            $CharacterClassTable['InitialQuotePunctuation'], $CharacterClassTable['FinalQuotePunctuation'], $CharacterClassTable['OtherNonAsciiPunctuation'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'OtherPunctuation' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::OtherPunctuation } `
        -SubClasses $CharacterClassTable['OtherAsciiPunctuation'], $CharacterClassTable['OtherNonAsciiPunctuation'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'Punctuation' -FilterScript { [char]::IsPunctuation($_) } `
        -SubClasses $CharacterClassTable['AsciiPunctuation'], $CharacterClassTable['NonAsciiPunctuation'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'MathSymbol' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::MathSymbol } `
        -SubClasses $CharacterClassTable['AsciiMathSymbol'], $CharacterClassTable['NonAsciiMathSymbol'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'CurrencySymbol' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::CurrencySymbol } `
        -SubClasses $CharacterClassTable['AsciiCurrencySymbol'], $CharacterClassTable['NonAsciiCurrencySymbol'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'AsciiSymbol' -FilterScript { [char]::IsSymbol($_) -and [char]::IsAscii($_) } `
        -SubClasses $CharacterClassTable['AsciiMathSymbol'], $CharacterClassTable['AsciiCurrencySymbol'], $CharacterClassTable['AsciiModifierSymbol'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'ModifierSymbol' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::ModifierSymbol } `
        -SubClasses $CharacterClassTable['AsciiModifierSymbol'], $CharacterClassTable['NonAsciiModifierSymbol'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'NonAsciiSymbol' -FilterScript { [char]::IsSymbol($_) -and -not [char]::IsAscii($_) } `
        -SubClasses $CharacterClassTable['NonAsciiMathSymbol'], $CharacterClassTable['NonAsciiCurrencySymbol'], $CharacterClassTable['NonAsciiModifierSymbol'], $CharacterClassTable['OtherSymbol'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'Symbol' -FilterScript { [char]::IsSymbol($_) } `
        -SubClasses $CharacterClassTable['AsciiSymbol'], $CharacterClassTable['NonAsciiSymbol'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'SpaceSeparator' -FilterScript { [char]::GetUnicodeCategory($_) -eq [System.Globalization.UnicodeCategory]::SpaceSeparator } `
        -SubClasses $CharacterClassTable['AsciiSpaceSeparator'], $CharacterClassTable['NonAsciiSpaceSeparator'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'NonAsciiSeparator' -FilterScript { [char]::IsSeparator($_) -and -not [char]::IsAscii($_) } `
        -SubClasses $CharacterClassTable['NonAsciiSpaceSeparator'], $CharacterClassTable['LineSeparator'], $CharacterClassTable['ParagraphSeparator'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'Separator' -FilterScript { [char]::IsSeparator($_) } `
        -SubClasses $CharacterClassTable['AsciiSpaceSeparator'], $CharacterClassTable['NonAsciiSeparator'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'AsciiWhiteSpace' -FilterScript { [char]::IsWhiteSpace($_) -and [char]::IsAscii($_) } `
        -SubClasses $CharacterClassTable['AsciiSpaceSeparator'], $CharacterClassTable['AsciiControl'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'Ascii' -FilterScript { [char]::IsAscii($_) } `
        -SubClasses $CharacterClassTable['AsciiLetterOrDigit'], $CharacterClassTable['AsciiPunctuation'], $CharacterClassTable['AsciiSymbol'], $CharacterClassTable['AsciiWhiteSpace'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'NonAsciiWhiteSpace' -FilterScript { [char]::IsWhiteSpace($_) -and -not [char]::IsAscii($_) } `
        -SubClasses $CharacterClassTable['NonAsciiSeparator'], $CharacterClassTable['NonAsciiControl'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'Control' -FilterScript { [char]::IsControl($_) } `
        -SubClasses $CharacterClassTable['AsciiControl'], $CharacterClassTable['NonAsciiControl'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'WhiteSpace' -FilterScript { [char]::IsWhiteSpace($_) } `
        -SubClasses $CharacterClassTable['AsciiWhiteSpace'], $CharacterClassTable['NonAsciiWhiteSpace'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'Surrogate' -FilterScript { [char]::IsSurrogate($_) } `
        -SubClasses $CharacterClassTable['HighSurrogate'], $CharacterClassTable['LowSurrogate'] -AddTo $CharacterClassTable;
    New-AggregateCharacterClass -Name 'NonAscii' -FilterScript { [char]::IsSurrogate($_) } `
        -SubClasses $CharacterClassTable['NonAsciiLetterOrDigit'], $CharacterClassTable['Mark'], $CharacterClassTable['NonAsciiPunctuation'], $CharacterClassTable['NonAsciiSymbol'], $CharacterClassTable['NonAsciiWhiteSpace'],
            $CharacterClassTable['Format'], $CharacterClassTable['Surrogate'], $CharacterClassTable['PrivateUse'], $CharacterClassTable['OtherNotAssigned'] -AddTo $CharacterClassTable;
        
    ($CharacterClassTable['AsciiHexDigitLower'], $CharacterClassTable['NonHexAsciiLetterUpper'], $CharacterClassTable['NonAsciiLetterUpper']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['HexLetterUpper'];
    ($CharacterClassTable['AsciiHexDigitUpper'], $CharacterClassTable['NonHexAsciiLetterLower'], $CharacterClassTable['NonAsciiLetterLower']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['HexLetterLower'];
    ($CharacterClassTable['AsciiDigit'], $CharacterClassTable['NonHexAsciiLetterLower'], $CharacterClassTable['NonAsciiLetterUpper']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['NonHexAsciiLetterUpper'];
    ($CharacterClassTable['AsciiLetterUpper'], $CharacterClassTable['NonAsciiLetterLower'], $CharacterClassTable['TitlecaseLetter'], $CharacterClassTable['ModifierLetter'], $CharacterClassTable['OtherLetter']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['NonAsciiLetterUpper'];
    ($CharacterClassTable['NonHexAsciiLetterUpper'], $CharacterClassTable['TitlecaseLetter'], $CharacterClassTable['ModifierLetter'], $CharacterClassTable['OtherLetter']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['NonHexAsciiLetterLower'];
    ($CharacterClassTable['AsciiLetterUpper'], $CharacterClassTable['AsciiLetterLower']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['NonAsciiLetterLower'];
    ($CharacterClassTable['UppercaseLetter'], $CharacterClassTable['LowercaseLetter'], $CharacterClassTable['ModifierLetter'], $CharacterClassTable['OtherLetter']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['TitlecaseLetter'];
    ($CharacterClassTable['UppercaseLetter'], $CharacterClassTable['LowercaseLetter'], $CharacterClassTable['TitlecaseLetter'], $CharacterClassTable['OtherLetter']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['ModifierLetter'];
    ($CharacterClassTable['UppercaseLetter'], $CharacterClassTable['LowercaseLetter'], $CharacterClassTable['TitlecaseLetter'], $CharacterClassTable['ModifierLetter']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['OtherLetter'];
    ($CharacterClassTable['NonBinaryOctalDigit'], $CharacterClassTable['NonOctalAsciiDigit'], $CharacterClassTable['HexLetter'], $CharacterClassTable['NonAsciiDigit']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['BinaryDigitNumber'];
    ($CharacterClassTable['BinaryDigitNumber'], $CharacterClassTable['NonOctalAsciiDigit'], $CharacterClassTable['HexLetter'], $CharacterClassTable['NonAsciiDigit']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['NonBinaryOctalDigit'];
    ($CharacterClassTable['OctalDigit'], $CharacterClassTable['HexLetter'], $CharacterClassTable['NonAsciiDigit']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['NonOctalAsciiDigit'];
    ($CharacterClassTable['AsciiDigit'], $CharacterClassTable['LetterNumber'], $CharacterClassTable['OtherNumber']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['NonAsciiDigit'];
    ($CharacterClassTable['Digit'], $CharacterClassTable['OtherNumber']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['LetterNumber'];
    ($CharacterClassTable['Digit'], $CharacterClassTable['LetterNumber']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['OtherNumber'];
    ($CharacterClassTable['SpacingCombiningMark'], $CharacterClassTable['EnclosingMark']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['NonSpacingMark'];
    ($CharacterClassTable['NonSpacingMark'], $CharacterClassTable['EnclosingMark']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['SpacingCombiningMark'];
    ($CharacterClassTable['NonSpacingMark'], $CharacterClassTable['EnclosingMark']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['EnclosingMark'];
    ($CharacterClassTable['AsciiDashPunctuation'], $CharacterClassTable['AsciiOpenPunctuation'], $CharacterClassTable['AsciiClosePunctuation'], $CharacterClassTable['OtherAsciiPunctuation'], $CharacterClassTable['NonAsciiPunctuation']) `
        | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['AsciiConnectorPunctuation'];
    ($CharacterClassTable['AsciiPunctuation'], $CharacterClassTable['NonAsciiDashPunctuation'], $CharacterClassTable['NonAsciiOpenPunctuation'], $CharacterClassTable['NonAsciiClosePunctuation'], $CharacterClassTable['OtherNonAsciiPunctuation'], $CharacterClassTable['InitialQuotePunctuation'],
        $CharacterClassTable['FinalQuotePunctuation']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['NonAsciiConnectorPunctuation'];
    ($CharacterClassTable['AsciiConnectorPunctuation'], $CharacterClassTable['AsciiOpenPunctuation'], $CharacterClassTable['AsciiClosePunctuation'], $CharacterClassTable['OtherAsciiPunctuation'], $CharacterClassTable['NonAsciiPunctuation']) `
        | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['AsciiDashPunctuation'];
    ($CharacterClassTable['AsciiPunctuation'], $CharacterClassTable['NonAsciiConnectorPunctuation'], $CharacterClassTable['NonAsciiOpenPunctuation'], $CharacterClassTable['NonAsciiClosePunctuation'], $CharacterClassTable['OtherNonAsciiPunctuation'], $CharacterClassTable['InitialQuotePunctuation'],
        $CharacterClassTable['FinalQuotePunctuation']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['NonAsciiDashPunctuation'];
    ($CharacterClassTable['AsciiConnectorPunctuation'], $CharacterClassTable['AsciiDashPunctuation'], $CharacterClassTable['AsciiClosePunctuation'], $CharacterClassTable['OtherAsciiPunctuation'], $CharacterClassTable['NonAsciiPunctuation']) `
        | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['AsciiOpenPunctuation'];
    ($CharacterClassTable['AsciiPunctuation'], $CharacterClassTable['NonAsciiConnectorPunctuation'], $CharacterClassTable['NonAsciiDashPunctuation'], $CharacterClassTable['NonAsciiClosePunctuation'], $CharacterClassTable['OtherNonAsciiPunctuation'], $CharacterClassTable['InitialQuotePunctuation'],
        $CharacterClassTable['FinalQuotePunctuation']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['NonAsciiOpenPunctuation'];
    ($CharacterClassTable['AsciiConnectorPunctuation'], $CharacterClassTable['AsciiDashPunctuation'], $CharacterClassTable['AsciiOpenPunctuation'], $CharacterClassTable['OtherAsciiPunctuation'], $CharacterClassTable['NonAsciiPunctuation']) `
        | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['AsciiClosePunctuation'];
    ($CharacterClassTable['AsciiPunctuation'], $CharacterClassTable['NonAsciiConnectorPunctuation'], $CharacterClassTable['NonAsciiDashPunctuation'], $CharacterClassTable['NonAsciiOpenPunctuation'], $CharacterClassTable['OtherNonAsciiPunctuation'], $CharacterClassTable['InitialQuotePunctuation'],
        $CharacterClassTable['FinalQuotePunctuation']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['NonAsciiClosePunctuation'];
    ($CharacterClassTable['ConnectorPunctuation'], $CharacterClassTable['DashPunctuation'], $CharacterClassTable['OpenPunctuation'], $CharacterClassTable['ClosePunctuation'], $CharacterClassTable['FinalQuotePunctuation'], $CharacterClassTable['OtherPunctuation']) `
        | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['InitialQuotePunctuation'];
    ($CharacterClassTable['ConnectorPunctuation'], $CharacterClassTable['DashPunctuation'], $CharacterClassTable['OpenPunctuation'], $CharacterClassTable['ClosePunctuation'], $CharacterClassTable['InitialQuotePunctuation'], $CharacterClassTable['OtherPunctuation']) `
        | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['FinalQuotePunctuation'];
    ($CharacterClassTable['AsciiConnectorPunctuation'], $CharacterClassTable['AsciiDashPunctuation'], $CharacterClassTable['AsciiOpenPunctuation'], $CharacterClassTable['AsciiClosePunctuation'], $CharacterClassTable['NonAsciiPunctuation']) `
        | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['OtherAsciiPunctuation'];
    ($CharacterClassTable['AsciiPunctuation'], $CharacterClassTable['NonAsciiConnectorPunctuation'], $CharacterClassTable['NonAsciiDashPunctuation'], $CharacterClassTable['NonAsciiOpenPunctuation'], $CharacterClassTable['NonAsciiClosePunctuation'], $CharacterClassTable['InitialQuotePunctuation'],
        $CharacterClassTable['FinalQuotePunctuation']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['OtherNonAsciiPunctuation'];
    ($CharacterClassTable['AsciiCurrencySymbol'], $CharacterClassTable['AsciiModifierSymbol'], $CharacterClassTable['NonAsciiSymbol']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['AsciiMathSymbol'];
    ($CharacterClassTable['AsciiSymbol'], $CharacterClassTable['NonAsciiCurrencySymbol'], $CharacterClassTable['NonAsciiModifierSymbol'], $CharacterClassTable['OtherSymbol']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['NonAsciiMathSymbol'];
    ($CharacterClassTable['AsciiMathSymbol'], $CharacterClassTable['AsciiModifierSymbol'], $CharacterClassTable['NonAsciiSymbol']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['AsciiCurrencySymbol'];
    ($CharacterClassTable['AsciiSymbol'], $CharacterClassTable['NonAsciiMathSymbol'], $CharacterClassTable['NonAsciiModifierSymbol'], $CharacterClassTable['OtherSymbol']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['NonAsciiCurrencySymbol'];
    ($CharacterClassTable['AsciiMathSymbol'], $CharacterClassTable['AsciiCurrencySymbol'], $CharacterClassTable['NonAsciiSymbol']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['AsciiModifierSymbol'];
    ($CharacterClassTable['AsciiSymbol'], $CharacterClassTable['NonAsciiMathSymbol'], $CharacterClassTable['NonAsciiCurrencySymbol'], $CharacterClassTable['OtherSymbol']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['NonAsciiModifierSymbol'];
    ($CharacterClassTable['MathSymbol'], $CharacterClassTable['CurrencySymbol'], $CharacterClassTable['ModifierSymbol']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['OtherSymbol'];
    ($CharacterClassTable['NonAsciiWhiteSpace']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['AsciiSpaceSeparator'];
    ($CharacterClassTable['AsciiSpaceSeparator'], $CharacterClassTable['LineSeparator'], $CharacterClassTable['ParagraphSeparator'], $CharacterClassTable['Control']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['NonAsciiSpaceSeparator'];
    ($CharacterClassTable['SpaceSeparator'], $CharacterClassTable['ParagraphSeparator'], $CharacterClassTable['Control']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['LineSeparator'];
    ($CharacterClassTable['SpaceSeparator'], $CharacterClassTable['LineSeparator'], $CharacterClassTable['Control']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['ParagraphSeparator'];
    ($CharacterClassTable['Separator'], $CharacterClassTable['NonAsciiControl']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['AsciiControl'];
    ($CharacterClassTable['Separator'], $CharacterClassTable['AsciiControl']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['NonAsciiControl']; 
    ($CharacterClassTable['WhiteSpace'], $CharacterClassTable['Surrogate']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['Format'];
    ($CharacterClassTable['WhiteSpace'], $CharacterClassTable['LowSurrogate']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['HighSurrogate'];
    ($CharacterClassTable['WhiteSpace'], $CharacterClassTable['HighSurrogate']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['LowSurrogate'];
    ($CharacterClassTable['OtherNotAssigned'], $CharacterClassTable['WhiteSpace']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['PrivateUse'];
    ($CharacterClassTable['PrivateUse'], $CharacterClassTable['WhiteSpace']) | Add-SimilarCharacterClass -CharacterClass $CharacterClassTable['OtherNotAssigned'];

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
        if ($CharacterClassTable.ContainsKey($Key)) {
            $CharacterClass = $CharacterClassTable[$Key];
            if ($CharacterClass -isnot [PrimaryCharacterClass]) {
                Write-Error -Message "Character $($c | ConvertTo-PsCharLiteral) mapped to non-primary class $Key" -Category InvalidOperation -ErrorId 'InvalidKey' -TargetObject $c -ErrorAction Stop;
            }
            $CharacterClass.AllCharacters.Add($c);
        } else {
            Write-Error -Message "Character $($c | ConvertTo-PsCharLiteral) mapped to non-existent primary class $Key" -Category InvalidOperation -ErrorId 'InvalidKey' -TargetObject $c -ErrorAction Stop;
        }
    }
    Write-Progress -Activity 'Hash Character Types' -Status "65536 characters added" -PercentComplete 100 -Completed;
    $CharacterClassTable.Values | Write-Output;
}

Function Import-CharacterClasses {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    $CharacterClassTable = @{};
    $AggregateMap = @{};

    foreach ($obj in (Import-Clixml -LiteralPath $Path -ErrorAction Stop)) {
        if ($null -ne $obj.BitIndex) {
            New-PrimaryCharacterClass -Name $obj.Name -BitIndex $obj.BitIndex -FilterScript $obj.FilterScript -InputCharacters $obj.TestCharacters -AddTo $CharacterClassTable;
        } else {
            New-AggregateCharacterClass -Name $obj.Name -FilterScript $obj.FilterScript -AddTo $CharacterClassTable;
            $AggregateMap[$obj.Name] = $obj.SubClasses;
        }
    }
    foreach ($Name in $AggregateMap.Keys) {
        [AggregateCharacterClass]$CharacterClass = $CharacterClassTable[$Name];
        $SubClasses = @();
        foreach ($n in $AggregateMap[$Name]) {
            if ($CharacterClassTable.ContainsKey($n)) {
                $SubClasses += $CharacterClassTable[$n];
            } else {
                Write-Error -Message "$Name contains a reference to non-existent class $n" -Category InvalidArgument -ErrorId 'CharacterClassNotFound' -TargetObject $n -ErrorAction Stop;
            }
        }
        $CharacterClass.SubClasses = ($SubClasses | Sort-Object -Property @{ Expression = { $_.GetFlags() }});
    }
    $CharacterClassTable.Values | Write-Output;
}

Function Export-CharacterClasses {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [CharacterClass[]]$InputObject,

        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    Begin { $AllCharacterClasses = @() }

    Process { $AllCharacterClasses += $InputObject }

    End {
        ($AllCharacterClasses | Sort-Object -Property @{ Expression = { $_.GetFlags() } }) | ForEach-Object {
            $Item =  [PSCustomObject]@{
                Name = $_.Name;
                FilterScript = $_.FilterScript;
                RelatedClasses = ([string[]]@($_.RelatedClasses | ForEach-Object { $_.Name }));
            };
            if  ($_ -is [PrimaryCharacterClass]) {
                $Item | Add-Member -MemberType NoteProperty -Name 'BitIndex' -Value $_.BitIndex;
                $Item | Add-Member -MemberType NoteProperty -Name 'TestCharacters' -Value $_.TestCharacters -PassThru;
            } else {
                $Item | Add-Member -MemberType NoteProperty -Name 'SubClasses' -Value ([string[]]@($_.SubClasses | ForEach-Object { $_.Name })) -PassThru;
            }
        } | Export-Clixml -LiteralPath $Path -Force;
    }
}

Function Get-CharacterClassPsCode {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [CharacterClass[]]$InputObject
    )

    Begin { $AllItems = @() }

    Process { $AllItems += $InputObject }

    End {
        $AllItems = @($AllItems | Sort-Object -Property @{ Expression = { $_.GetFlags() } });
        $LongestName = 1;
        $AllItems | ForEach-Object {
            $Len = $_.Name.Length;
            if ($Len -gt $LongestName) { $LongestName = $Len }
        }
        $LongestName++;
        [char]$sp = ' ';

        (
            'Enum CharacterClass : ulong {',
            "    # $($AllItems[0].FilterScript.ToString().Trim())",
            "    $($AllItems[0].Name) =$([string]::new($sp, $LongestName - $AllItems[0].Name.Length))0x$($AllItems[0].GetFlags().ToString('x13'));"
        ) | Write-Output;
        
        ($AllItems | Select-Object -Skip 1) | ForEach-Object {
            (
                '',
                "    # $($_.FilterScript.ToString().Trim())",
                "    $($_.Name) =$([string]::new($sp, $LongestName - $_.Name.Length))0x$($_.GetFlags().ToString('x13'));"
            ) | Write-Output;
        }
        '}' | Write-Output;
    }
}

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
        $Writer.WriteLine();
        $AllChars = @($AllChars | Sort-Object -CaseSensitive);
        switch ($AllChars.Count) {
            1 {
                $Writer.Write("        It '$(($AllChars[0] | ConvertTo-PsCharLiteral -AlwaysDoubleQuotes)) should return ");
                $Writer.Write($ShouldBe.ToString().ToLower());
                $Writer.WriteLine("' {");
                $Writer.Write('            $Actual = Test-CharacterClass -Value ');
                $Writer.Write(($AllChars[0] | ConvertTo-PsCharLiteral));
                if ($IsNot.IsPresent) { $Writer.Write(' -IsNot') }
                $Writer.Write(' -Flags ');
                $Writer.Write($Flags);
                $Writer.WriteLine(' -ErrorAction Stop;');
                $Writer.Write('            $Actual | Should -Be');
                $Writer.Write($ShouldBe.ToString());
                $Writer.Write(' -Because ');
                $Writer.Write(($AllChars[0] | ConvertTo-PsCharLiteral));
                $Writer.WriteLine(';');
                $Writer.WriteLine('        }');
                break;
            }
            2 {
                $Writer.Write("        It '$(($AllChars[0] | ConvertTo-PsCharLiteral -AlwaysDoubleQuotes)) and $(($AllChars[1] | ConvertTo-PsCharLiteral -AlwaysDoubleQuotes)) should return ");
                $Writer.Write($ShouldBe.ToString().ToLower());
                $Writer.WriteLine("' {");
                $Writer.Write('            $Actual = Test-CharacterClass -Value ');
                $Writer.Write(($AllChars[0] | ConvertTo-PsCharLiteral));
                if ($IsNot.IsPresent) { $Writer.Write(' -IsNot') }
                $Writer.Write(' -Flags ');
                $Writer.Write($Flags);
                $Writer.WriteLine(' -ErrorAction Stop;');
                $Writer.Write('            $Actual | Should -Be');
                $Writer.Write($ShouldBe.ToString());
                $Writer.Write(' -Because ');
                $Writer.Write(($AllChars[0] | ConvertTo-PsCharLiteral));
                $Writer.WriteLine(';');
                $Writer.Write('            $Actual = Test-CharacterClass -Value ');
                $Writer.Write(($AllChars[1] | ConvertTo-PsCharLiteral));
                if ($IsNot.IsPresent) { $Writer.Write(' -IsNot') }
                $Writer.Write(' -Flags ');
                $Writer.Write($Flags);
                $Writer.WriteLine(' -ErrorAction Stop;');
                $Writer.Write('            $Actual | Should -Be');
                $Writer.Write($ShouldBe.ToString());
                $Writer.Write(' -Because ');
                $Writer.Write(($AllChars[1] | ConvertTo-PsCharLiteral));
                $Writer.WriteLine(';');
                $Writer.WriteLine('        }');
                break;
            }
            default {
                $Writer.Write("        It '$Desc characters should return ");
                $Writer.Write($ShouldBe.ToString().ToLower());
                $Writer.WriteLine("' {");
                $Writer.Write('            foreach ($c in @(([char[]](');
                $Writer.Write(($AllChars[0] | ConvertTo-PsCharLiteral));
                foreach ($c in ($AllChars | Select-Object -Skip 1)) {
                    $Writer.Write(', ');
                    $Writer.Write(($c | ConvertTo-PsCharLiteral));
                }
                $Writer.WriteLine(', "")))) {');
                $Writer.Write('                $Actual = Test-CharacterClass -Value $c');
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
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [CharacterClass]$InputClass,

        [Parameter(Mandatory = $true)]
        [System.IO.StreamWriter]$Writer
    )
    
    Process {
        $Writer.WriteLine();
        $Writer.Write("Describe 'Test-CharacterClass -Flags ");
        $Writer.Write($InputClass.Name);
        $Writer.WriteLine("' {");
        $Writer.Write("    Context 'IsNot.Present = `$false' {");
        [char[]]$TestCharacters = ($InputClass | Get-TestCharacters | Sort-Object -CaseSensitive);
        $TestCharacters | Write-PesterItStatement -ShouldBe $true -Flags $InputClass.Name -Writer $Writer;
        $AllChars = [System.Collections.ObjectModel.Collection[char]]::new();
        $TestCharacters | ForEach-Object { $AllChars.Add($_) }
        $AllRelatedClases = @($InputClass | Get-RelatedClasses);
        foreach ($RelatedClass in $AllRelatedClases) {
            [char[]]$Matching = ($RelatedClass | Get-TestCharacters);
            $Matching | Write-PesterItStatement -ShouldBe $false -Flags $RelatedClass.Name -Writer $Writer;
            $Matching | ForEach-Object { $AllChars.Add($_) }
        }
        [char[]]$Other = ($Script:CommonCharacters | Where-Object { $AllChars -cnotcontains $_ });
        $Other | Write-PesterItStatement -ShouldBe $false -Flags 'Other' -Writer $Writer;
        $Writer.WriteLine("    }");
        $Writer.WriteLine();
        $Writer.Write("    Context 'IsNot.Present = `$true' {");
        $TestCharacters | Write-PesterItStatement -ShouldBe $false -IsNot -Flags $InputClass.Name -Writer $Writer;
        foreach ($RelatedClass in $AllRelatedClases) {
            ($RelatedClass | Get-TestCharacters) | Write-PesterItStatement -ShouldBe $true -IsNot -Flags $RelatedClass.Name -Writer $Writer;
        }
        $Other | Write-PesterItStatement -ShouldBe $true -IsNot -Flags 'Other' -Writer $Writer;
        $Writer.WriteLine("    }");
        $Writer.WriteLine("}");
    }
}

Function Write-CharacterClassTestCode {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [CharacterClass[]]$InputClass,
        
        [Parameter(Mandatory = $true)]
        [System.IO.StreamWriter]$Writer
    )

    Begin { $AllItems = @() }

    Process { $AllItems += $InputClass }
    
    End {
        $Writer.WriteLine("Import-Module -Name (`$PSScriptRoot | Join-Path -ChildPath 'Erwine.Leonard.T.IOUtility.psd1') -ErrorAction Stop;");
        $Writer.WriteLine();
        $Writer.WriteLine('<#');
        $Writer.WriteLine('Import-Module Pester');
        $Writer.WriteLine('#>');
        ($AllItems | Sort-Object -Property @{ Expression = { $_.GetFlags() } }) | Write-PesterDescribeStatement -Writer $Writer;
    }
}

[CharacterClass[]]$AllCharacterClasses = Initialize-CharacterClasses;

# $AllCharacterClasses | Get-CharacterClassPsCode;

$AllCharacterClasses | Export-CharacterClasses -Path ($PSScriptRoot | Join-Path -ChildPath 'test.xml');
$Writer = [System.IO.StreamWriter]::new(($PSScriptRoot | Join-Path -ChildPath 'Test-CharacterClass.tests.ps1'), $false, [System.Text.UTF8Encoding]::new($false, $false));
try {
    $AllCharacterClasses | Write-CharacterClassTestCode -Writer $Writer;
    $Writer.Flush();
} finally {
    $Writer.Close();
}