enum RegexTokenType {
    LiteralCharacter;
    MetaCharacterEscape;
    LiteralEscape;
    CharacterEscape;
    AnyCharacter;
    CharacterClass;
    Range;
    Anchor;
    Alternate;
    Group;
    Error;
}

class RegexTokenItem {
    [string]$Text;
    [RegexTokenType]$Type;
    RegexTokenItem([string]$Text, [RegexTokenType]$Type) {
        if ($null -eq $Text) { $this.Text = '' } else { $this.Text = $Text }
        $this.Type = $Type;
    }
}

class TokenParseContext {
    [string]$Pattern;
    [int]$CurrentPosition;
    [System.Collections.ObjectModel.Collection[RegexTokenItem]]$Tokens;
    [RegexTokenItem]$LatestToken;

    [void] AddToken([string]$Text, [RegexTokenType]$Type) {
        if ($null -ne $this.LatestToken) {
            if ($this.LatestToken.Type -eq $Type) {
                $this.LatestToken.Text += $Text;
                return;
            }
            $this.Tokens.Add($this.LatestToken);
        }
        $this.LatestToken = [RegexTokenItem]::new($Text, $Type);
    }

    static [string] ReadOctalOrNullEscapeChars([string]$Text, [int]$StartAt) {
        if ($null -eq $Text -or $StartAt -ge $Text.Length) { return $null }
        $NextStart = $StartAt + 1;
        if ($NextStart -lt $Text.Length) {
            $c = $Text[$StartAt];
            if ($c -eq '0') {
                if ([TokenParseContext]::NzOctalChars.Contains($Text[$NextStart])) {
                    if (++$NextStart -lt $Text.Length -and [TokenParseContext]::OctalChars.Contains($Text[$NextStart])) { $NextStart++ }
                    return $Text.Substring($StartAt, $NextStart - $StartAt);
                }
                return '0';
            }
            if ([TokenParseContext]::NzOctalChars.Contains($c)) {
                if ([TokenParseContext]::OctalChars.Contains($Text[$NextStart])) {
                    if (++$NextStart -lt $Text.Length -and [TokenParseContext]::OctalChars.Contains($Text[$NextStart])) { $NextStart++ }
                    return $Text.Substring($StartAt, $NextStart - $StartAt);
                }
                return $c;
            }
        }
        return $null;
    }

    static [string] ReadHexEscapeChars([string]$Text, [int]$StartAt) {
        if ($null -eq $Text -or $StartAt -ge $Text.Length) { return $null }
        $NextStart = $StartAt + 1;
        if ($NextStart -lt $Text.Length -and [TokenParseContext]::HexChars.Contains($Text[$StartAt]) -and [TokenParseContext]::HexChars.Contains($Text[$NextStart])) {
            if (++$NextStart -lt $Text.Length -and [TokenParseContext]::HexChars.Contains($Text[$NextStart]) -and ++$NextStart -lt $Text.Length -and [TokenParseContext]::HexChars.Contains($Text[$NextStart])) {
                return $Text.Substring($StartAt, 4);
            }
            return $Text.Substring($StartAt, 2);
        }
        return $null;
    }

    static [string] ReadUnicodeEscapeChars([string]$Text, [int]$StartAt) {
        if ($null -eq $Text -or $StartAt -ge $Text.Length) { return $null }
        $NextStart = $StartAt + 4;
        if ($NextStart -gt $Text.Length) { return $null }
        for ($i = $StartAt; $i -lt $NextStart; $i++) {
            if (-not [TokenParseContext]::HexChars.Contains($Text[$i])) { return $null }
        }
        return $Text.Substring($StartAt, 4);
    }

    static [string] ReadUnicodeClass([string]$Text, [int]$StartAt) {
        if ($null -eq $Text -or $StartAt -ge $Text.Length -or $Text[$StartAt] -ne '{') { return $null }
        for ($i = $StartAt + 1; $i -lt $Text.Length; $i++) {
            $c = $Text[$i];
            if (-not [TokenParseContext]::UnicodeClassChars.Contains($c)) {
                if ($c -eq '}') { return $Text.Substring($StartAt, $i - $StartAt) }
                break;
            }
        }
        return $null;
    }

    [void] ParseCharacterClass() {
        if ($this.CurrentPosition -ge $this.Pattern.Length -or $this.Pattern[$this.CurrentPosition] -ne '[') { return }
        $ResetIndex = $this.Tokens.Count;
        $ResetToken = $this.LatestToken;
        if ($null -ne $this.LatestToken) { $this.LatestToken = [RegexTokenItem]::new($this.LatestToken.Text, $this.LatestToken.Type) }
        $this.AddToken('[', [RegexTokenType]::CharacterClass);
        $NextPosition = $this.CurrentPosition;
        while (++$NextPosition -lt $this.Pattern.Length) {
            $c = $this.Pattern[$NextPosition];
            switch ($c) {
                ']' {
                    $this.CurrentPosition = $NextPosition + 1;
                    $this.AddToken(']', [RegexTokenType]::CharacterClass);
                    return;
                }
                '-' {
                    if ($NextPosition -eq $this.CurrentPosition + 1) {
                        $this.AddToken('-', [RegexTokenType]::LiteralCharacter);
                    } else {
                        $n = $NextPosition + 1;
                        if ($n -lt $this.Pattern.Length -and $this.Pattern[$n] -eq ']') {
                            $this.AddToken('-', [RegexTokenType]::LiteralCharacter);
                        } else {
                            $this.AddToken('-', [RegexTokenType]::Range);
                        }
                    }
                    break;
                }
                '\' {
                    if (++$NextPosition -lt $this.Pattern.Length) {
                        $c = $this.Pattern[$NextPosition];
                        switch -casesensitive ($c) {
                            'c' {
                                if (++$NextPosition -lt $this.Pattern.Length) {
                                    $c = $this.Pattern[$NextPosition];
                                    if ([TokenParseContext]::ControlEscapeChars.Contains($c)) {
                                        $this.AddToken("\c$c", [RegexTokenType]::CharacterEscape);
                                    } else {
                                        $this.AddToken("\c$c", [RegexTokenType]::Error);
                                    }
                                    $NextPosition++;
                                }
                                break;
                            }
                            'x' {
                                if (++$NextPosition -lt $this.Pattern.Length) {
                                    $n = [TokenParseContext]::ReadHexEscapeChars($this.Pattern, $NextPosition);
                                    if ($null -ne $n) {
                                        $this.AddToken("\x$n", [RegexTokenType]::CharacterEscape);
                                        $NextPosition += $n.Length;
                                    } else {
                                        $this.AddToken($this.Pattern.Substring($NextPosition - 2, 3), [RegexTokenType]::Error);
                                        $NextPosition++;
                                    }
                                }
                                break;
                            }
                            'u' {
                                if (++$NextPosition -lt $this.Pattern.Length) {
                                    $n = [TokenParseContext]::ReadUnicodeEscapeChars($this.Pattern, $NextPosition);
                                    if ($null -ne $n) {
                                        $this.AddToken("\u$n", [RegexTokenType]::CharacterEscape);
                                        $NextPosition += $n.Length;
                                    } else {
                                        $this.AddToken($this.Pattern.Substring($NextPosition - 2, 3), [RegexTokenType]::Error);
                                        $NextPosition++;
                                    }
                                }
                                break;
                            }
                            Default {
                                if ($c -ieq 'p') {
                                    if (++$NextPosition -lt $this.Pattern.Length) {
                                        $n = [TokenParseContext]::ReadUnicodeClass($this.Pattern, $NextPosition);
                                        if ($null -ne $n) {
                                            $this.AddToken("\p$n", [RegexTokenType]::CharacterEscape);
                                            $NextPosition += $n.Length;
                                        } else {
                                            $this.AddToken($this.Pattern.Substring($NextPosition - 2, 4), [RegexTokenType]::Error);
                                            $NextPosition += 2;
                                        }
                                    }
                                    break;
                                }
                                if ([TokenParseContext]::OctalChars.Contains($c)) {
                                    $n = [TokenParseContext]::ReadOctalOrNullEscapeChars($this.Pattern, $NextPosition) - $NextPosition;
                                    $this.AddToken($this.Pattern.Substring($NextPosition - 1, $n + 1), [RegexTokenType]::CharacterEscape);
                                    break;
                                }
                                if ([TokenParseContext]::CharClassMetaChars.Contains($c)) {
                                    $this.AddToken($c, [RegexTokenType]::MetaCharacterEscape);
                                    break;
                                }
                                if ([TokenParseContext]::CharacterEscapeChars.Contains($c)) {
                                    $this.AddToken($c, [RegexTokenType]::CharacterEscape);
                                } else {
                                    $this.AddToken($c, [RegexTokenType]::LiteralEscape);
                                }
                                break;
                            }
                        }
                    }
                    break;
                }
                Default {
                    break;
                }
            }
        }
        $this.Reset($ResetIndex, $ResetToken);
        $this.AddToken('[', [RegexTokenType]::Error);
        $this.CurrentPosition++;
        
    }

    [void] Reset([int]$TokenCount, [RegexTokenItem]$LatestToken) {
        if ($TokenCount -eq 0) {
            $this.Tokens.Clear();
        } else {
            while ($this.Tokens.Count -gt $TokenCount) { $this.Tokens.RemoveAt($TokenCount) }
        }
        $this.LatestToken = $LatestToken;
    }

    hidden static [string]$AnchorEscapeChars = 'AGZz';
    hidden static [string]$CharacterEscapeChars = 'nrtabefvDdWwSs';
    hidden static [string]$PatternMetaChars = '[\^$.|?*+(){}';
    hidden static [string]$CharClassMetaChars = '[]\^-';
    hidden static [string]$ControlEscapeChars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz';
    hidden static [string]$UnicodeClassChars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    hidden static [string]$HexChars = 'ABCDEFabcdef0123456789';
    hidden static [string]$OctalChars = '01234567';
    hidden static [string]$NzOctalChars = '1234567';
    hidden static [string]$Modifiers = '1234567';

    [void] ParseGroup() {
        if ($this.CurrentPosition -ge $this.Pattern.Length -or $this.Pattern[$this.CurrentPosition] -ne '(') { return }
        $NextPosition = $this.CurrentPosition + 1;
        if ($NextPosition -ge $this.Pattern.Length) {
            $this.CurrentPosition = $this.Pattern.Length;
            $this.AddToken('(', [RegexTokenType]::Error);
            return;
        }
        $c = $this.Pattern[$NextPosition];
        if ($c -eq '?' && ++$NextPosition -lt $this.Pattern.Length)
        {
            $c = $this.Pattern[$NextPosition];
            $mArr = @([TokenParseContext]::Modifiers.ToCharArray());
            if ($mArr -ccontains $c) {
                $mArr = @($mArr | Where-Object { $_ -ne $c });
                if (++$NextPosition -lt $this.Pattern.Length) {
                    $c = $this.Pattern[$NextPosition];
                    if ($mArr -ccontains $c) {
                        $mArr = @($mArr | Where-Object { $_ -ne $c });
                        if (++$NextPosition -lt $this.Pattern.Length) {
                            $c = $this.Pattern[$NextPosition];
                            if ($mArr -ccontains $c) {
                                $mArr = @($mArr | Where-Object { $_ -ne $c });
                                if (++$NextPosition -lt $this.Pattern.Length -and $mArr -ccontains $this.Pattern[$NextPosition] -and ++$NextPosition -lt $this.Pattern.Length) { $c = $this.Pattern[$NextPosition] }
                            }
                        }
                    }
                }
            }
            if ($NextPosition -ge $this.Pattern.Length) {
                $this.AddToken($this.Pattern.Substring($this.CurrentPosition), [RegexTokenType]::Error);
                $this.CurrentPosition = $this.Pattern.Length;
                return;
            }
            if ($c -eq ')') {
                $NextPosition++;
                $this.AddToken($this.Pattern.Substring($this.CurrentPosition, $NextPosition - $this.CurrentPosition), [RegexTokenType]::Group);
                $this.CurrentPosition = $NextPosition;
                return;
            }
            if ($NextPosition -gt $this.CurrentPosition + 1) {
                if ($c -eq ':') {
                    $NextPosition++;
                    $this.AddToken($this.Pattern.Substring($this.CurrentPosition, $NextPosition - $this.CurrentPosition), [RegexTokenType]::Group);
                    $this.CurrentPosition = $NextPosition;
                } else {
                    $this.AddToken('(', [RegexTokenType]::Group);
                    $this.AddToken($this.Pattern.Substring($this.CurrentPosition + 1, $NextPosition - $this.CurrentPosition), [RegexTokenType]::Error);
    
                }
            } else {
                switch ($c) {
                    '<' {
                        if (++$NextPosition -lt $this.Pattern.Length) {
                            $c = $this.Pattern[$NextPosition];
                            switch ($c) {
                                '=' {
                                    $NextPosition++;
                                    $this.AddToken($this.Pattern.Substring($this.CurrentPosition, $NextPosition - $this.CurrentPosition), [RegexTokenType]::Group);
                                    break;
                                }
                                '!' {
                                    $NextPosition++;
                                    $this.AddToken($this.Pattern.Substring($this.CurrentPosition, $NextPosition - $this.CurrentPosition), [RegexTokenType]::Group);
                                    break;
                                }
                                Default {
                                    # TODO: Parse named group
                                    break;
                                }
                            }
                        }
                        break;
                    }
                    '''' {
                        # TODO: Parse named group
                        break;
                    }
                    '!' {
                        $NextPosition++;
                        $this.AddToken($this.Pattern.Substring($this.CurrentPosition, $NextPosition - $this.CurrentPosition), [RegexTokenType]::Group);
                        break;
                    }
                    '=' {
                        $NextPosition++;
                        $this.AddToken($this.Pattern.Substring($this.CurrentPosition, $NextPosition - $this.CurrentPosition), [RegexTokenType]::Group);
                        break;
                    }
                    ':' {
                        $NextPosition++;
                        $this.AddToken($this.Pattern.Substring($this.CurrentPosition, $NextPosition - $this.CurrentPosition), [RegexTokenType]::Group);
                        break;
                    }
                    '#' {
                        # TODO: Parse comment
                        break;
                    }
                }
            }
        }

    }

    [void] ParsePatternEscapeSequence() {
        if ($this.CurrentPosition -ge $this.Pattern.Length -or $this.Pattern[$this.CurrentPosition] -ne '\') { return }
        $NextPosition = $this.CurrentPosition + 1;
        if ($NextPosition -eq $this.Pattern.Length) {
            $this.AddToken('\', [RegexTokenType]::LiteralCharacter);
            return;
        }
    }

    TokenParseContext([string]$Pattern) {
        if ([string]::IsNullOrEmpty($Pattern)) { throw '$Pattern cannot be null or empty.' }
        $this.Pattern = $Pattern;
        $this.Tokens = [System.Collections.ObjectModel.Collection[RegexTokenItem]]::new();
        $c = $this.Pattern[0];
        $this.CurrentPosition = 1;
        switch ($this.LatestToken.Text) {
            '[' {
                $this.ParseCharacterClass();
                return;
            }
            '(' {
                $this.ParseGroup();
                return;
            }
            '\' {
                $this.ParsePatternEscapeSequence();
                return;
            }
            '^' {
                $this.LatestToken = [RegexTokenItem]::new($c, [RegexTokenType]::Anchor);
                break;
            }
            '$' {
                $this.LatestToken = [RegexTokenItem]::new($c, [RegexTokenType]::Anchor);
                break;
            }
            '.' {
                $this.LatestToken = [RegexTokenItem]::new($c, [RegexTokenType]::AnyCharacter);
                break;
            }
            '|' {
                $this.LatestToken = [RegexTokenItem]::new($c, [RegexTokenType]::Alternate);
                break;
            }
            '?' {
                $this.LatestToken = [RegexTokenItem]::new($c, [RegexTokenType]::Error);
                break;
            }
            '*' {
                $this.LatestToken = [RegexTokenItem]::new($c, [RegexTokenType]::Error);
                break;
            }
            '+' {
                $this.LatestToken = [RegexTokenItem]::new($c, [RegexTokenType]::Error);
                break;
            }
            Default {
                $this.LatestToken = [RegexTokenItem]::new($c, [RegexTokenType]::LiteralCharacter);
            }
        }
        if ($this.LatestToken.Type -eq [RegexTokenType]::Error) {
            $ErrorChars = '?*+';
            while ($this.CurrentPosition -lt $this.Pattern.Length -and $ErrorChars.Contains($this.Pattern[$this.CurrentPosition])) {
                $this.CurrentPosition++;
            }
            if ($this.CurrentPosition -gt 1) { $this.LatestToken.Text = $this.Pattern.Substring(0, $this.CurrentPosition) }
        } else {
            if ($this.LatestToken.Type -eq [RegexTokenType]::Anchor) {
                while ($this.CurrentPosition -lt $this.Pattern.Length) {
                    $c = $this.Pattern[$this.CurrentPosition];
                    if ($c -eq '\') {
                        if (++$this.CurrentPosition -eq $this.Pattern.Length -or -not [TokenParseContext]::AnchorEscapeChars.Contains($this.Pattern[$this.CurrentPosition])) {
                            $this.CurrentPosition--;
                            break;
                        }
                    } else {
                        if ($c -ne '$' -and $c -ne '^') { break }
                    }
                    $this.CurrentPosition++;
                }
                if ($this.CurrentPosition -gt 1) { $this.LatestToken.Text = $this.Pattern.Substring(0, $this.CurrentPosition) }
            }
        }
    }

    [void] Reset([TokenParseContext]$CopyFrom) { $this.TryReset($CopyFrom); }

    [void] AddCharacterClassTokens() {
        if ($this.CurrentPosition -ge $this.Pattern.Length -or $this.Pattern[$this.CurrentPosition] -ne '[') { return }
        if ($This.CurrentPosition -lt $this.Pattern.Length - 1) {
            $CharClassContentMatch = [TokenParseContext]::CharClassContentRegex.Match($this.Pattern, $this.CurrentPosition + 1);
            if ($CharClassContentMatch.Success) {
                $g = $CharClassContentMatch.Groups[1];
                if ($g.Success) {
                    $this.Tokens.Add(([TokenParseContext]::new('[', [RegexTokenType]::CharacterClass)));
                    $CharClassTokenMatch = [TokenParseContext]::CharClassTokenRegex.Match($g.Value);
                    $StartAt = 0;
                    while ($CharClassTokenMatch.Success) {
                        if ($CharClassTokenMatch.Index -gt $StartAt) {
                            $this.Tokens.Add(([TokenParseContext]::new($g.Value.Substring($StartAt, $CharClassTokenMatch.Index - $StartAt), [RegexTokenType]::LiteralEscape)));
                        }
                        $StartAt = $CharClassTokenMatch.Index + $CharClassTokenMatch.Length;
                        if ($CharClassTokenMatch.Groups['r'].Success) {
    
                        } else {
                            if ($CharClassTokenMatch.Groups['c'].Success) {
                                $this.Tokens.Add(([TokenParseContext]::new($CharClassTokenMatch.Value, [RegexTokenType]::CharacterEscape)));
                            } else {
                                if ($CharClassTokenMatch.Groups['m'].Success) {
                                    $this.Tokens.Add(([TokenParseContext]::new($CharClassTokenMatch.Value, [RegexTokenType]::MetaCharacterEscape)));
                                } else {
                                    $this.Tokens.Add(([TokenParseContext]::new($CharClassTokenMatch.Value, [RegexTokenType]::LiteralCharacter)));
                                }
                            }
                        }
                        $CharClassTokenMatch = [TokenParseContext]::CharClassTokenRegex.Match($g.Value, $StartAt);
                    }
                    if ($StartAt -lt $g.Length) {
                        $this.Tokens.Add(([TokenParseContext]::new($g.Value.Substring($StartAt), [RegexTokenType]::LiteralEscape)));
                    }
                    $this.Tokens.Add(([TokenParseContext]::new(']', [RegexTokenType]::CharacterClass)));
                } else {
                    $this.Tokens.Add(([TokenParseContext]::new('[]', [RegexTokenType]::CharacterClass)));
                }
                $this.CurrentPosition = $CharClassContentMatch.Index + $CharClassContentMatch.Length;
                return;
            }
        }
        $this.Tokens.Add(([TokenParseContext]::new('[', [RegexTokenType]::Error)));
        $this.CurrentPosition++;
    }
}

Function Get-EscapedTokens {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [TokenParseContext]$Context
    )
}

Function Get-CharacterClassTokens {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [TokenParseContext]$Context
    )
}

Function Convert-RegularExpressionToTokens {
    [CmdletBinding()]
    [OutputType([RegexTokenItem[]])]
    Param(
        # Regular Expression Pattern
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        [string]$Pattern
    )

    Process {
        $Context = [TokenParseContext]::new($Pattern);
        while ($Context.EndPosition -lt $Pattern.Length)
        {
            $NextContext = [TokenParseContext]::new($Context);
            do {
                switch ($Context.Pattern[$Index]) {
                    '[' {
                        if ($Context.Length -gt 0) {
                            [RegexTokenItem]::new($Context.GetText(), [RegexTokenType]::LiteralCharacters) | Write-Output;
                            $Context.MoveNext();
                        }
                        $Context.LiteralTextTokenTo($Index) | Write-Output;
                        (Get-CharacterClassTokens -Context $Context) | Write-Output;
                        break;
                    }
                    '\' {
                        $Context.LiteralTextTokenTo($Index) | Write-Output;
                        (Get-EscapedTokens -Context $Context) | Write-Output;
                        break;
                    }
                    '^' {
                        break;
                    }
                    '$' {
                        break;
                    }
                    '.' {
                        $Context.LiteralTextTokenTo($Index) | Write-Output;
                        [RegexTokenItem]::new('.', [RegexTokenType]::AnyCharacter) | Write-Output;
                        while ($Context.TryIncrement() && $Context)
                        break;
                    }
                    '|' {
                        break;
                    }
                    '?' {
                        break;
                    }
                    '*' {
                        break;
                    }
                    '+' {
                        break;
                    }
                    '(' {
                        break;
                    }
                    Default {
                        break;
                    }
                }
            } while (++$Index -lt $Pattern.Length);
        }
    }
}

Function Convert-RegularExpression {
    [CmdletBinding()]
    Param(
        # Regular Expression Pattern
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        [string]$Pattern,

        # Produces function result from array of RegexToken objects.
        [Parameter(Mandatory = $true, ParameterSetName = 'Converter')]
        [ScriptBlock]$Converter,

        # If present, do not serialize return value of $Converter invocation.
        [Parameter(ParameterSetName = 'Converter')]
        [switch]$DoNotSerialize,

        # If present, converts $Pattern to HTML.
        [Parameter(ParameterSetName = 'HTML')]
        [switch]$ToHtml
    )

    Process {
        $Context = [TokenParseContext]::new($Pattern);
        while ($Context.CurrentPosition -lt $Pattern.Length)
        {
            $Index = $Context.CurrentPosition;
            do {
                $c = $Pattern[$Index];
                switch ($c) {
                    '[' {

                        break;
                    }
                    '\' {
                        break;
                    }
                    '^' {
                        break;
                    }
                    '$' {
                        break;
                    }
                    '.' {
                        break;
                    }
                    '|' {
                        break;
                    }
                    '?' {
                        break;
                    }
                    '*' {
                        break;
                    }
                    '+' {
                        break;
                    }
                    '(' {
                        break;
                    }
                    Default {
                        break;
                    }
                }
            } while (++$Index -lt $Pattern.Length);
        }
    }
}