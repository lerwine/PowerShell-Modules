if ($null -eq $Script:VersionPathSeparatorChars) {
    Set-Variable -Name 'VersionPathSeparatorChars' -Scope 'Script' -Option ReadOnly -Value ([char[]]@('/', '\'));
    Set-Variable -Name 'SemverComponentSeparatorChars' -Scope 'Script' -Option ReadOnly -Value ([char[]]@('-', '+'));
}

class ExtensionIdentity {
    [string]$ID = '';
    [string]$Version = '';
    [string]$Platform = '';

    [string] ToString() {
        if ([string]::IsNullOrWhiteSpace($this.Platform)) {
            return "$($this.ID)-$($this.Version)";
        }
        return "$($this.ID)-$($this.Version)@$($this.Platform)";
    }
}

class ExtensionVsixManifest {
    [ValidateNotNull()]
    [ExtensionIdentity]$Identity = [ExtensionIdentity]::new();
    [string]$DisplayName = '';
    [string]$Description = '';
    [string]$Icon = '';
}

class VsixFileInfo : ExtensionVsixManifest {
    [ValidateNotNull()]
    [string]$Path;
    [bool]$FromManifest = $false;
}

class VsMarketPlaceExtensionVersion {
    [string]$Version;
    [DateTime]$LastUpdated;
    [string]$TargetPlatform;
}

class VsMarketPlaceQueryResult {
    [DateTime]$PublishedDate;
    [string]$PublisherName;
    [string]$Name;
    [string]$DisplayName;
    [string]$Description;
    [VsMarketPlaceExtensionVersion[]]$Versions;
}

Function Test-IsStringComparisonOrdinal {
    [CmdletBinding()]
    [OutputType([System.StringComparer])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [System.StringComparison]$Type
    )
    switch ($Type) {
        Ordinal {
            $true | Write-Output;
            break;
        }
        OrdinalIgnoreCase {
            $true | Write-Output;
            break;
        }
        default {
            $false | Write-Output;
            break;
        }
    }
}

Enum CharClassType {
    Symbol;
    Punctuation;
    WhiteSpaceOrControl;
    Digit;
    NonDigitNumber;
    Other;
}

Function Get-CharClassType {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        [string]$InputString,

        [ValidateRange(0, [int]::MaxValue)]
        [int]$Index
    )

    Process {
        if ($Index -ge $InputString.Length) {
            [CharClassType]::Other | Write-Output;
        } else {
            $Char = $InputString[$Index];
            if ([char]::IsSymbol($Char)) {
                [CharClassType]::Symbol | Write-Output;
            } else {
                if ([char]::IsPunctuation($Char)) {
                    [CharClassType]::Punctuation | Write-Output;
                } else {
                    if ([char]::IsDigit($Char)) {
                        [CharClassType]::Digit | Write-Output;
                    } else {
                        if ([char]::IsNumber($Char)) {
                            [CharClassType]::NonDigitNumber | Write-Output;
                        } else {
                            if ([char]::IsWhiteSpace($Char) -or [char]::IsControl($Char)) {
                                [CharClassType]::WhiteSpaceOrControl | Write-Output;
                            } else {
                                [CharClassType]::Other | Write-Output;
                            }
                        }
                    }
                }
            }
        }
    }

}

class IndexedCharClassType {
    [ValidateRange(-1, [int]::MaxValue)]
    [int]$Index = -1;

    [CharClassType]$Type;
}

Function Get-IndexOfCharType {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        [string]$InputString,

        [ValidateRange(0, [int]::MaxValue)]
        [int]$StartIndex = 0,

        [Parameter(Mandatory = $true, ParameterSetName = 'Symbol')]
        [switch]$Symbol,

        [Parameter(Mandatory = $true, ParameterSetName = 'Punctuation')]
        [switch]$Punctuation,

        [Parameter(Mandatory = $true, ParameterSetName = 'WhiteSpaceOrControl')]
        [switch]$WhiteSpaceOrControl,

        [Parameter(Mandatory = $true, ParameterSetName = 'Digit')]
        [switch]$Digit,

        [Parameter(Mandatory = $true, ParameterSetName = 'NonDigitNumber')]
        [switch]$NonDigitNumber,

        [Parameter(Mandatory = $true, ParameterSetName = 'Other')]
        [switch]$Other,

        [switch]$IsNot,

        [switch]$GetNextClass,

        [char[]]$NotMatching
    )

    Begin {
        if ($PSBoundParameters.ContainsKey('NotMatching')) {
            # TODO: Implement NotMatching
        } else {
            if ($GetNextClass.IsPresent) {
                if ($IsNot.IsPresent) {
                    switch ($PSCmdlet.ParameterSetName) {
                        'Symbol' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    [CharClassType]$NextType = $Value | Get-CharClassType -Index $Index;
                                    if ($NextType -ne [CharClassType]::Symbol) {
                                        [IndexedCharClassType]@{ Index = $Index; Type = $NextType } | Write-Output;
                                    } else {
                                        while (++$Index -lt $Len) {
                                            $NextType = $Value | Get-CharClassType -Index $Index
                                            if ($NextType -ne [CharClassType]::Symbol) { break }
                                        }
                                        if ($Index -lt $Len) {
                                            [IndexedCharClassType]@{ Index = $Index; Type = $NextType } | Write-Output;
                                        } else {
                                            [IndexedCharClassType]@{ Type = $NextType } | Write-Output;
                                        }
                                    }
                                } else {
                                    [IndexedCharClassType]@{ Type = [CharClassType]::Symbol} | Write-Output;
                                }
                            }
                            break;
                        }
                        'Punctuation' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    [CharClassType]$NextType = $Value | Get-CharClassType -Index $Index;
                                    if ($NextType -ne [CharClassType]::Punctuation) {
                                        [IndexedCharClassType]@{ Index = $Index; Type = $NextType } | Write-Output;
                                    } else {
                                        while (++$Index -lt $Len) {
                                            $NextType = $Value | Get-CharClassType -Index $Index
                                            if ($NextType -ne [CharClassType]::Punctuation) { break }
                                        }
                                        if ($Index -lt $Len) {
                                            [IndexedCharClassType]@{ Index = $Index; Type = $NextType } | Write-Output;
                                        } else {
                                            [IndexedCharClassType]@{ Type = $NextType } | Write-Output;
                                        }
                                    }
                                } else {
                                    [IndexedCharClassType]@{ Type = [CharClassType]::Punctuation} | Write-Output;
                                }
                            }
                            break;
                        }
                        'WhiteSpaceOrControl' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    [CharClassType]$NextType = $Value | Get-CharClassType -Index $Index;
                                    if ($NextType -ne [CharClassType]::WhiteSpaceOrControl) {
                                        [IndexedCharClassType]@{ Index = $Index; Type = $NextType } | Write-Output;
                                    } else {
                                        while (++$Index -lt $Len) {
                                            $NextType = $Value | Get-CharClassType -Index $Index
                                            if ($NextType -ne [CharClassType]::WhiteSpaceOrControl) { break }
                                        }
                                        if ($Index -lt $Len) {
                                            [IndexedCharClassType]@{ Index = $Index; Type = $NextType } | Write-Output;
                                        } else {
                                            [IndexedCharClassType]@{ Type = $NextType } | Write-Output;
                                        }
                                    }
                                } else {
                                    [IndexedCharClassType]@{ Type = [CharClassType]::WhiteSpaceOrControl} | Write-Output;
                                }
                            }
                            break;
                        }
                        'Digit' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    [CharClassType]$NextType = $Value | Get-CharClassType -Index $Index;
                                    if ($NextType -ne [CharClassType]::Digit) {
                                        [IndexedCharClassType]@{ Index = $Index; Type = $NextType } | Write-Output;
                                    } else {
                                        while (++$Index -lt $Len) {
                                            $NextType = $Value | Get-CharClassType -Index $Index
                                            if ($NextType -ne [CharClassType]::Digit) { break }
                                        }
                                        if ($Index -lt $Len) {
                                            [IndexedCharClassType]@{ Index = $Index; Type = $NextType } | Write-Output;
                                        } else {
                                            [IndexedCharClassType]@{ Type = $NextType } | Write-Output;
                                        }
                                    }
                                } else {
                                    [IndexedCharClassType]@{ Type = [CharClassType]::Digit} | Write-Output;
                                }
                            }
                            break;
                        }
                        'NonDigitNumber' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    [CharClassType]$NextType = $Value | Get-CharClassType -Index $Index;
                                    if ($NextType -ne [CharClassType]::NonDigitNumber) {
                                        [IndexedCharClassType]@{ Index = $Index; Type = $NextType } | Write-Output;
                                    } else {
                                        while (++$Index -lt $Len) {
                                            $NextType = $Value | Get-CharClassType -Index $Index
                                            if ($NextType -ne [CharClassType]::NonDigitNumber) { break }
                                        }
                                        if ($Index -lt $Len) {
                                            [IndexedCharClassType]@{ Index = $Index; Type = $NextType } | Write-Output;
                                        } else {
                                            [IndexedCharClassType]@{ Type = $NextType } | Write-Output;
                                        }
                                    }
                                } else {
                                    [IndexedCharClassType]@{ Type = [CharClassType]::NonDigitNumber} | Write-Output;
                                }
                            }
                            break;
                        }
                        default {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    [CharClassType]$NextType = $Value | Get-CharClassType -Index $Index;
                                    if ($NextType -ne [CharClassType]::Other) {
                                        [IndexedCharClassType]@{ Index = $Index; Type = $NextType } | Write-Output;
                                    } else {
                                        while (++$Index -lt $Len) {
                                            $NextType = $Value | Get-CharClassType -Index $Index
                                            if ($NextType -ne [CharClassType]::Other) { break }
                                        }
                                        if ($Index -lt $Len) {
                                            [IndexedCharClassType]@{ Index = $Index; Type = $NextType } | Write-Output;
                                        } else {
                                            [IndexedCharClassType]@{ Type = $NextType } | Write-Output;
                                        }
                                    }
                                } else {
                                    [IndexedCharClassType]@{ Type = [CharClassType]::Other} | Write-Output;
                                }
                            }
                            break;
                        }
                    }
                } else {
                    switch ($PSCmdlet.ParameterSetName) {
                        'Symbol' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    [CharClassType]$NextType = $Value | Get-CharClassType -Index $Index;
                                    if ($NextType -eq [CharClassType]::Symbol) {
                                        ($Index, ($Value | Get-CharClassType -Index ($Index + 1))) | Write-Output;
                                    } else {
                                        while (++$Index -lt $Len) {
                                            if ([char]::IsSymbol($Value[$Index])) { break }
                                        }
                                        if ($Index -lt $Len) {
                                            ($Index, ($Value | Get-CharClassType -Index ($Index + 1))) | Write-Output;
                                        } else {
                                            (-1, $NextType) | Write-Output;
                                        }
                                    }
                                } else {
                                    (-1, [CharClassType]::Other) | Write-Output;
                                }
                            }
                            break;
                        }
                        'Punctuation' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    [CharClassType]$NextType = $Value | Get-CharClassType -Index $Index;
                                    if ($NextType -eq [CharClassType]::Symbol) {
                                        ($Index, ($Value | Get-CharClassType -Index ($Index + 1))) | Write-Output;
                                    } else {
                                        while (++$Index -lt $Len) {
                                            if ([char]::IsPunctuation($Value[$Index])) { break }
                                        }
                                        if ($Index -lt $Len) {
                                            ($Index, ($Value | Get-CharClassType -Index ($Index + 1))) | Write-Output;
                                        } else {
                                            (-1, $NextType) | Write-Output;
                                        }
                                    }
                                } else {
                                    (-1, [CharClassType]::Other) | Write-Output;
                                }
                            }
                            break;
                        }
                        'WhiteSpaceOrControl' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    [CharClassType]$NextType = $Value | Get-CharClassType -Index $Index;
                                    if ($NextType -eq [CharClassType]::Symbol) {
                                        ($Index, ($Value | Get-CharClassType -Index ($Index + 1))) | Write-Output;
                                    } else {
                                        while (++$Index -lt $Len) {
                                            $Char = $Value[$Index];
                                            if ([char]::IsWhiteSpace($Char) -or [char]::IsControl($Char)) { break }
                                        }
                                        if ($Index -lt $Len) {
                                            ($Index, ($Value | Get-CharClassType -Index ($Index + 1))) | Write-Output;
                                        } else {
                                            (-1, $NextType) | Write-Output;
                                        }
                                    }
                                } else {
                                    (-1, [CharClassType]::Other) | Write-Output;
                                }
                            }
                            break;
                        }
                        'Digit' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    [CharClassType]$NextType = $Value | Get-CharClassType -Index $Index;
                                    if ($NextType -eq [CharClassType]::Symbol) {
                                        ($Index, ($Value | Get-CharClassType -Index ($Index + 1))) | Write-Output;
                                    } else {
                                        while (++$Index -lt $Len) {
                                            if ([char]::IsDigit($Value[$Index])) { break }
                                        }
                                        if ($Index -lt $Len) {
                                            ($Index, ($Value | Get-CharClassType -Index ($Index + 1))) | Write-Output;
                                        } else {
                                            (-1, $NextType) | Write-Output;
                                        }
                                    }
                                } else {
                                    (-1, [CharClassType]::Other) | Write-Output;
                                }
                            }
                            break;
                        }
                        'NonDigitNumber' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    [CharClassType]$NextType = $Value | Get-CharClassType -Index $Index;
                                    if ($NextType -eq [CharClassType]::Symbol) {
                                        ($Index, ($Value | Get-CharClassType -Index ($Index + 1))) | Write-Output;
                                    } else {
                                        while (++$Index -lt $Len) {
                                            $Char = $Value[$Index];
                                            if ([char]::IsNumber($Char) -and -not [char]::IsDigit($Char)) { break }
                                        }
                                        if ($Index -lt $Len) {
                                            ($Index, ($Value | Get-CharClassType -Index ($Index + 1))) | Write-Output;
                                        } else {
                                            (-1, $NextType) | Write-Output;
                                        }
                                    }
                                } else {
                                    (-1, [CharClassType]::Other) | Write-Output;
                                }
                            }
                            break;
                        }
                        default {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    [CharClassType]$NextType = $Value | Get-CharClassType -Index $Index;
                                    if ($NextType -eq [CharClassType]::Symbol) {
                                        ($Index, ($Value | Get-CharClassType -Index ($Index + 1))) | Write-Output;
                                    } else {
                                        while (++$Index -lt $Len) {
                                            $Char = $Value[$Index];
                                            if (-not ([char]::IsSymbol($Char) -or [char]::IsPunctuation($Char) -or [char]::IsWhiteSpace($Char) -or [char]::IsControl($Char) -or [char]::IsNumber($Char))) { break }
                                        }
                                        if ($Index -lt $Len) {
                                            ($Index, ($Value | Get-CharClassType -Index ($Index + 1))) | Write-Output;
                                        } else {
                                            (-1, $NextType) | Write-Output;
                                        }
                                    }
                                } else {
                                    (-1, [CharClassType]::Other) | Write-Output;
                                }
                            }
                            break;
                        }
                    }
                }
            } else {
                if ($IsNot.IsPresent) {
                    switch ($PSCmdlet.ParameterSetName) {
                        'Symbol' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    do {
                                        if (-not [char]::IsSymbol($Value[$Index])) { return $Index }
                                    } while (++$Index -lt $Len);
                                }
                                return -1;
                            }
                            break;
                        }
                        'Punctuation' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    do {
                                        if (-not [char]::IsPunctuation($Value[$Index])) { return $Index }
                                    } while (++$Index -lt $Len);
                                }
                                return -1;
                            }
                            break;
                        }
                        'WhiteSpaceOrControl' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    do {
                                        $Char = $Value[$Index];
                                        if (-not ([char]::IsWhiteSpace($Char) -or [char]::IsControl($Char))) { return $Index }
                                    } while (++$Index -lt $Len);
                                }
                                return -1;
                            }
                            break;
                        }
                        'Digit' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    do {
                                        if (-not [char]::IsDigit($Value[$Index])) { return $Index }
                                    } while (++$Index -lt $Len);
                                }
                                return -1;
                            }
                            break;
                        }
                        'NonDigitNumber' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    do {
                                        $Char = $Value[$Index];
                                        if (-not ([char]::IsNumber($Char) -and -not [char]::IsDigit($Char))) { return $Index }
                                    } while (++$Index -lt $Len);
                                }
                                return -1;
                            }
                            break;
                        }
                        default {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    do {
                                        $Char = $Value[$Index];
                                        if ([char]::IsSymbol($Char) -or [char]::IsPunctuation($Char) -or [char]::IsWhiteSpace($Char) -or [char]::IsControl($Char) -or [char]::IsNumber($Char)) { return $Index }
                                    } while (++$Index -lt $Len);
                                }
                                return -1;
                            }
                            break;
                        }
                    }
                } else {
                    switch ($PSCmdlet.ParameterSetName) {
                        'Symbol' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    do {
                                        if ([char]::IsSymbol($Value[$Index])) { return $Index }
                                    } while (++$Index -lt $Len);
                                }
                                return -1;
                            }
                            break;
                        }
                        'Punctuation' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    do {
                                        if ([char]::IsPunctuation($Value[$Index])) { return $Index }
                                    } while (++$Index -lt $Len);
                                }
                                return -1;
                            }
                            break;
                        }
                        'WhiteSpaceOrControl' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    do {
                                        $Char = $Value[$Index];
                                        if ([char]::IsWhiteSpace($Char) -or [char]::IsControl($Char)) { return $Index }
                                    } while (++$Index -lt $Len);
                                }
                                return -1;
                            }
                            break;
                        }
                        'Digit' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    do {
                                        if ([char]::IsDigit($Value[$Index])) { return $Index }
                                    } while (++$Index -lt $Len);
                                }
                                return -1;
                            }
                            break;
                        }
                        'NonDigitNumber' {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    do {
                                        $Char = $Value[$Index];
                                        if ([char]::IsNumber($Char) -and -not [char]::IsDigit($Char)) { return $Index }
                                    } while (++$Index -lt $Len);
                                }
                                return -1;
                            }
                            break;
                        }
                        default {
                            Function GetIndexOfCharType($Value) {
                                $Len = $Value.Length;
                                $Index = $StartIndex;
                                if ($Index -lt $Len) {
                                    do {
                                        $Char = $Value[$Index];
                                        if (-not ([char]::IsSymbol($Char) -or [char]::IsPunctuation($Char) -or [char]::IsWhiteSpace($Char) -or [char]::IsControl($Char) -or [char]::IsNumber($Char))) { return $Index }
                                    } while (++$Index -lt $Len);
                                }
                                return -1;
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    Process {
        (GetIndexOfCharType $InputString) | Write-Output;
    }
}

class CharClassSeparatedValue {
    [ValidateNotNull()]
    [AllowEmptyString()]
    [string]$Separator = '';

    [ValidateNotNull()]
    [AllowEmptyString()]
    [string]$Value = '';
}

Function Get-CharClassSeparatedValues {
    [CmdletBinding()]
    [OutputType([CharClassSeparatedValue[]])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        [string]$InputString,

        [ValidateRange(0, [int]::MaxValue)]
        [int]$StartIndex = 0,

        [Parameter(Mandatory = $true, ParameterSetName = 'Symbol')]
        [switch]$Symbol,

        [Parameter(Mandatory = $true, ParameterSetName = 'Punctuation')]
        [switch]$Punctuation,

        [Parameter(Mandatory = $true, ParameterSetName = 'WhiteSpaceOrControl')]
        [switch]$WhiteSpaceOrControl,

        [char[]]$NotMatching
    )

    Begin {
        if ($PSBoundParameters.ContainsKey('NotMatching')) {
            # TODO: Implement NotMatching
        } else {
            switch ($PSCmdlet.ParameterSetName) {
                'Symbol' {
                    Function GetCharClassSeparatedValues($Value) {
                        $SepIdx = $Value | Get-IndexOfCharType -StartIndex $StartIndex -Symbol;
                        if ($SepIdx -lt 0) {
                            if ($StartIndex -gt 0) {
                                if ($StartIndex -lt $Value.Length) {
                                    [CharClassSeparatedValue]@{ Value = $Value.Substring($StartIndex) } | Write-Output;
                                } else {
                                    [CharClassSeparatedValue]::new() | Write-Output;
                                }
                            } else {
                                [CharClassSeparatedValue]@{ Value = $Value } | Write-Output;
                            }
                        } else {
                            if ($SepIdx -gt $StartIndex) {
                                [CharClassSeparatedValue]@{ Value = $Value.Substring($StartIndex, $SepIdx - $StartIndex) } | Write-Output;
                            }
                            $NextIndex = $ValIdx = $Value | Get-IndexOfCharType -StartIndex ($SepIdx + 1) -IsNot -Symbol;
                            while ($ValIdx -gt 0) {
                                # Separator = $Value.Substring($SepIdx, $ValIdx - $SepIdx)
                                $NextIdx = $Value | Get-IndexOfCharType -StartIndex $PrevIdx -Symbol;
                                if ($NextIdx -lt 0) { break }
                                [CharClassSeparatedValue]@{
                                    Separator = $Value.Substring($SepIdx, $SepIdx - $ValIdx);
                                    Value = $Value.Substring($ValIdx, $ValIdx - $NextIdx);
                                } | Write-Output;
                                $SepIdx = $NextIndex;
                                $ValIdx = $Value | Get-IndexOfCharType -StartIndex ($SepIdx + 1) -IsNot -Symbol;
                            }
                            if ($NextIndex -gt 0) {
                                [CharClassSeparatedValue]@{ Separator = $Value.Substring($SepIdx) } | Write-Output;
                                # $ValIdx -lt 0
                            } else {
                                if ($ValIdx -gt 0) {
                                    [CharClassSeparatedValue]@{
                                        Separator = $Value.Substring($SepIdx, $SepIdx - $ValIdx);
                                        Value = $Value.Substring($ValIdx);
                                    } | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
                'Punctuation' {
                    Function GetCharClassSeparatedValues($Value) {
                        $SepIdx = $Value | Get-IndexOfCharType -StartIndex $StartIndex -Punctuation;
                        if ($SepIdx -lt 0) {
                            if ($StartIndex -gt 0) {
                                if ($StartIndex -lt $Value.Length) {
                                    [CharClassSeparatedValue]@{ Value = $Value.Substring($StartIndex) } | Write-Output;
                                } else {
                                    [CharClassSeparatedValue]::new() | Write-Output;
                                }
                            } else {
                                [CharClassSeparatedValue]@{ Value = $Value } | Write-Output;
                            }
                        } else {
                            if ($SepIdx -gt $StartIndex) {
                                [CharClassSeparatedValue]@{ Value = $Value.Substring($StartIndex, $SepIdx - $StartIndex) } | Write-Output;
                            }
                            $NextIndex = $ValIdx = $Value | Get-IndexOfCharType -StartIndex ($SepIdx + 1) -IsNot -Punctuation;
                            while ($ValIdx -gt 0) {
                                # Separator = $Value.Substring($SepIdx, $ValIdx - $SepIdx)
                                $NextIdx = $Value | Get-IndexOfCharType -StartIndex $PrevIdx -Punctuation;
                                if ($NextIdx -lt 0) { break }
                                [CharClassSeparatedValue]@{
                                    Separator = $Value.Substring($SepIdx, $SepIdx - $ValIdx);
                                    Value = $Value.Substring($ValIdx, $ValIdx - $NextIdx);
                                } | Write-Output;
                                $SepIdx = $NextIndex;
                                $ValIdx = $Value | Get-IndexOfCharType -StartIndex ($SepIdx + 1) -IsNot -Punctuation;
                            }
                            if ($NextIndex -gt 0) {
                                [CharClassSeparatedValue]@{ Separator = $Value.Substring($SepIdx) } | Write-Output;
                                # $ValIdx -lt 0
                            } else {
                                if ($ValIdx -gt 0) {
                                    [CharClassSeparatedValue]@{
                                        Separator = $Value.Substring($SepIdx, $SepIdx - $ValIdx);
                                        Value = $Value.Substring($ValIdx);
                                    } | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
                default {
                    Function GetCharClassSeparatedValues($Value) {
                        $SepIdx = $Value | Get-IndexOfCharType -StartIndex $StartIndex -WhiteSpaceOrControl;
                        if ($SepIdx -lt 0) {
                            if ($StartIndex -gt 0) {
                                if ($StartIndex -lt $Value.Length) {
                                    [CharClassSeparatedValue]@{ Value = $Value.Substring($StartIndex) } | Write-Output;
                                } else {
                                    [CharClassSeparatedValue]::new() | Write-Output;
                                }
                            } else {
                                [CharClassSeparatedValue]@{ Value = $Value } | Write-Output;
                            }
                        } else {
                            if ($SepIdx -gt $StartIndex) {
                                [CharClassSeparatedValue]@{ Value = $Value.Substring($StartIndex, $SepIdx - $StartIndex) } | Write-Output;
                            }
                            $NextIndex = $ValIdx = $Value | Get-IndexOfCharType -StartIndex ($SepIdx + 1) -IsNot -WhiteSpaceOrControl;
                            while ($ValIdx -gt 0) {
                                # Separator = $Value.Substring($SepIdx, $ValIdx - $SepIdx)
                                $NextIdx = $Value | Get-IndexOfCharType -StartIndex $PrevIdx -WhiteSpaceOrControl;
                                if ($NextIdx -lt 0) { break }
                                [CharClassSeparatedValue]@{
                                    Separator = $Value.Substring($SepIdx, $SepIdx - $ValIdx);
                                    Value = $Value.Substring($ValIdx, $ValIdx - $NextIdx);
                                } | Write-Output;
                                $SepIdx = $NextIndex;
                                $ValIdx = $Value | Get-IndexOfCharType -StartIndex ($SepIdx + 1) -IsNot -WhiteSpaceOrControl;
                            }
                            if ($NextIndex -gt 0) {
                                [CharClassSeparatedValue]@{ Separator = $Value.Substring($SepIdx) } | Write-Output;
                                # $ValIdx -lt 0
                            } else {
                                if ($ValIdx -gt 0) {
                                    [CharClassSeparatedValue]@{
                                        Separator = $Value.Substring($SepIdx, $SepIdx - $ValIdx);
                                        Value = $Value.Substring($ValIdx);
                                    } | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }
    }

    Process {
        (GetCharClassSeparatedValues $InputString) | Write-Output;
    }
}

# :#&?|/\;,=!

if ($null -eq $Script:VersionCoreSeparators) {
    New-Variable -Name 'VersionCoreSeparators' -Option Constant -Value ':#&?|/\;,.=!';
}

Function Compare-SemverVersionStrings {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$LVersion,

        [Parameter(Mandatory = $true)]
        [string]$RVersion,

        [Parameter(Mandatory = $true)]
        [System.StringComparer]$Comparer
    )

    $LIndex = $LVersion.IndexOfAny($Script:SemverComponentSeparatorChars);
    $RIndex = $RVersion.IndexOfAny($Script:SemverComponentSeparatorChars);
    if ($LIndex -lt 0) {
        if ($RIndex -lt 0) {
            return (CompareElements -LVersion $LVersion -RVersion $RVersion -Comparer $Comparer);
        }
        $Diff = CompareElements -LVersion $LVersion -RVersion $RVersion.Substring(0, $RIndex) -Comparer $Comparer;
        if ($Diff -eq 0) { return -1 }
        return $Diff;
    }

    if ($RIndex -lt 0) {
        $Diff = CompareElements -LVersion $LVersion.Substring(0, $LIndex) -RVersion $RVersion -Comparer $Comparer;
        if ($Diff -eq 0) { return 1 }
        return $Diff;
    }

    $Diff = CompareElements -LVersion $LVersion.Substring(0, $LIndex) -RVersion $RVersion.Substring(0, $RIndex) -Comparer $Comparer;
    if ($Diff -ne 0) { return $Diff }

    if ($LVersion[$LIndex] -eq '-') {
        if ($RVersion[$RIndex] -eq '+') { return 1 }
    } else {
        if ($RVersion[$RIndex] -eq '-') { return -1 }
    }

    $LText = $LVersion.Substring($LIndex + 1).Trim();
    $RText = $RVersion.Substring($RIndex + 1).Trim();

    if ($LText.Length -eq 0) {
        if ($RText.Length -eq 0) { return 0 }
        return -1;
    }
    if ($RText.Length -eq 0) { return 1 }
    return (Compare-SemverVersionStrings -LVersion $LText -RVersion $RText -Comparer $Comparer);
}

Function Select-SemanticVersion {
    <#
    .SYNOPSIS
        Filter by Semver versions.
    .DESCRIPTION
        Filters by SemanticVersion values.
    #>
    [CmdletBinding(DefaultParameterSetName = 'EqualTo')]
    [OutputType([System.Management.Automation.SemanticVersion])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [System.Management.Automation.SemanticVersion[]]$Version,

        [Parameter(Mandatory = $true, ParameterSetName = 'EqualTo')]
        [System.Management.Automation.SemanticVersion]$EqualTo,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotEqualTo')]
        [System.Management.Automation.SemanticVersion]$NotEqualTo,

        [Parameter(Mandatory = $true, ParameterSetName = 'GreaterThan')]
        [System.Management.Automation.SemanticVersion]$GreaterThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotGreaterThan')]
        [System.Management.Automation.SemanticVersion]$NotGreaterThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'LessThan')]
        [System.Management.Automation.SemanticVersion]$LessThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotLessThan')]
        [System.Management.Automation.SemanticVersion]$NotLessThan
    )

}

Function Select-BySemanticVersion {
    <#
    .SYNOPSIS
        Filter by Semver versions.
    .DESCRIPTION
        Filters by SemanticVersion values.
    #>
    [CmdletBinding(DefaultParameterSetName = 'EqualTo')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [object[]]$InputObject,

        [Parameter(Mandatory = $true)]
        [string]$PropertyName,

        [Parameter(Mandatory = $true, ParameterSetName = 'EqualTo')]
        [System.Management.Automation.SemanticVersion]$EqualTo,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotEqualTo')]
        [System.Management.Automation.SemanticVersion]$NotEqualTo,

        [Parameter(Mandatory = $true, ParameterSetName = 'GreaterThan')]
        [System.Management.Automation.SemanticVersion]$GreaterThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotGreaterThan')]
        [System.Management.Automation.SemanticVersion]$NotGreaterThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'LessThan')]
        [System.Management.Automation.SemanticVersion]$LessThan,

        [Parameter(Mandatory = $true, ParameterSetName = 'NotLessThan')]
        [System.Management.Automation.SemanticVersion]$NotLessThan,

        [switch]$NullSameAsZero,

        [switch]$IgnoreNonVersionValues
    )
    
    Begin {
        if ($NullSameAsZero.IsPresent) { $Local:Zero = [System.Management.Automation.SemanticVersion]::new(0) }
        [System.Management.Automation.SemanticVersion]$Local:VersionValue = $null;
    }
    Process {
        if ($NullSameAsZero.IsPresent) {
            switch ($PSCmdlet.ParameterSetName) {
                'NotEqualTo' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ($null -eq $Value) {
                            if (-not $Local:Zero.Equals($NotEqualTo)) { $Obj | Write-Output }
                        } else {
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
                                if (-not $Local:VersionValue.Equals($NotEqualTo)) { $Obj | Write-Output }
                            } else {
                                if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $NotEqualTo) -ne 0) {
                                    $Obj | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
                'GreaterThan' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ($null -eq $Value) {
                            if ($Local:Zero.CompareTo($GreaterThan) -gt 0) { $Obj | Write-Output }
                        } else {
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
                                if ($Local:VersionValue.CompareTo($GreaterThan) -gt 0) { $Obj | Write-Output }
                            } else {
                                if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $GreaterThan) -gt 0) {
                                    $Obj | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
                'NotGreaterThan' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ($null -eq $Value) {
                            if ($Local:Zero.CompareTo($NotGreaterThan) -le 0) { $Obj | Write-Output }
                        } else {
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
                                if ($Local:VersionValue.CompareTo($NotGreaterThan) -le 0) { $Obj | Write-Output }
                            } else {
                                if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $NotGreaterThan) -le 0) {
                                    $Obj | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
                'LessThan' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ($null -eq $Value) {
                            if ($Local:Zero.CompareTo($LessThan) -lt 0) { $Obj | Write-Output }
                        } else {
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
                                if ($Local:VersionValue.CompareTo($LessThan) -lt 0) { $Obj | Write-Output }
                            } else {
                                if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $LessThan) -lt 0) {
                                    $Obj | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
                'NotLessThan' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ($null -eq $Value) {
                            if ($Local:Zero.CompareTo($NotLessThan) -ge 0) { $Obj | Write-Output }
                        } else {
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
                                if ($Local:VersionValue.CompareTo($NotLessThan) -ge 0) { $Obj | Write-Output }
                            } else {
                                if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $NotLessThan) -ge 0) {
                                    $Obj | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
                default {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ($null -eq $Value) {
                            if ($Local:Zero.Equals($EqualTo)) { $Obj | Write-Output }
                        } else {
                            if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
                                if ($Local:VersionValue.Equals($EqualTo)) { $Obj | Write-Output }
                            } else {
                                if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $EqualTo) -eq 0) {
                                    $Obj | Write-Output;
                                }
                            }
                        }
                    }
                    break;
                }
            }
        } else {
            switch ($PSCmdlet.ParameterSetName) {
                'NotEqualTo' {
                    foreach ($Obj in $InputObject) {
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Obj.($PropertyName), [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
                            if ($null -eq $Local:VersionValue -or -not $Local:VersionValue.Equals($NotEqualTo)) { $Obj | Write-Output }
                        } else {
                            if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $NotEqualTo) -ne 0) {
                                $Obj | Write-Output;
                            }
                        }
                    }
                    break;
                }
                'GreaterThan' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
                            if ($null -ne $Local:VersionValue -and $Local:VersionValue.CompareTo($GreaterThan) -gt 0) { $Obj | Write-Output }
                        } else {
                            if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $GreaterThan) -gt 0) {
                                $Obj | Write-Output;
                            }
                        }
                    }
                    break;
                }
                'NotGreaterThan' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
                            if ($null -eq $Local:VersionValue -or $Local:VersionValue.CompareTo($NotGreaterThan) -le 0) { $Obj | Write-Output }
                        } else {
                            if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $NotGreaterThan) -le 0) {
                                $Obj | Write-Output;
                            }
                        }
                    }
                    break;
                }
                'LessThan' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
                            if ($null -eq $Local:VersionValue -or $Local:VersionValue.CompareTo($NotGreaterThan) -lt 0) { $Obj | Write-Output }
                        } else {
                            if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $NotGreaterThan) -lt 0) {
                                $Obj | Write-Output;
                            }
                        }
                    }
                    break;
                }
                'NotLessThan' {
                    foreach ($Obj in $InputObject) {
                        $Value = $Obj.($PropertyName);
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Value, [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
                            if ($null -ne $Local:VersionValue -and $Local:VersionValue.CompareTo($GreaterThan) -ge 0) { $Obj | Write-Output }
                        } else {
                            if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $GreaterThan) -ge 0) {
                                $Obj | Write-Output;
                            }
                        }
                    }
                    break;
                }
                default {
                    foreach ($Obj in $InputObject) {
                        if ([System.Management.Automation.LanguagePrimitives]::TryConvertTo($Obj.($PropertyName), [System.Management.Automation.SemanticVersion], [ref]$Local:VersionValue)) {
                            if ($null -ne $Local:VersionValue -and $Local:VersionValue.Equals($NotEqualTo)) { $Obj | Write-Output }
                        } else {
                            if ((-not $IgnoreNonVersionValues.IsPresent) -and [System.Management.Automation.LanguagePrimitives]::Compare($Value, $NotEqualTo) -eq 0) {
                                $Obj | Write-Output;
                            }
                        }
                    }
                    break;
                }
            }
        }
    }
}

Function Compare-SemanticVersion {
    <#
    .SYNOPSIS
        Compare Semver versions.
    .DESCRIPTION
        Compares two SemanticVersion values, returning a number indicating whether one version is less than, greater than, or equal to the other.
    #>
    [CmdletBinding(DefaultParameterSetName = 'ReturnsInt')]
    [OutputType([int], ParameterSetName = 'ReturnsInt')]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [Alias('Version')]
        [System.Management.Automation.SemanticVersion[]]$ReferenceVersion,

        [Parameter(Mandatory = $true)]
        [Alias('To')]
        [System.Management.Automation.SemanticVersion]$DifferenceVersion,

        [Parameter(Mandatory = $true, ParameterSetName = 'PassThru')]
        [switch]$PassThru
    )

    Process {
        if ($PassThru.IsPresent) {
            foreach ($Version in $ReferenceVersion) {
                ($Version | Add-Member -MemberType NoteProperty -Name 'ComparisonIndicator' -Value $Version.CompareTo($ReferenceVersion) -PassThru) | Write-Output;
            }
        } else {
            foreach ($Version in $ReferenceVersion) {
                $Version.CompareTo($ReferenceVersion) | Write-Output;
            }
        }
    }
}
Function Compare-VersionStrings {
    
}

Function New-ExtensionIdentity {
    <#
    .SYNOPSIS
        Create new ExtensionIdentity object.
    .DESCRIPTION
        Create a new object representing the ID, Version and Platform of a VSIX package.
    #>
    [CmdletBinding(DefaultParameterSetName = 'Parse')]
    [OutputType([ExtensionIdentity])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'Parse')]
        # The identity string to parse.
        [string[]]$InputString,

        [Parameter(Mandatory = $true, ParameterSetName = 'Identity')]
        # The publisher ID.
        [string]$Publisher,

        [Parameter(Mandatory = $true, ParameterSetName = 'Identity')]
        # The package ID.
        [string]$ID,

        [Parameter(Mandatory = $true, ParameterSetName = 'Identity')]
        # The version string.
        [string]$Version,

        [Parameter(ParameterSetName = 'Identity')]
        [AllowEmptyString()]
        # The platform identifier.
        [string]$Platform
    )

    Process {
        if ($PSCmdlet.ParameterSetName -eq 'Identity') {
            if ([string]::IsNullOrWhiteSpace($Platform)) {
                [ExtensionIdentity]@{
                    ID = "$Publisher.$Id";
                    Version = $Version;
                } | Write-Output;
            } else {
                [ExtensionIdentity]@{
                    ID = "$Publisher.$Id";
                    Version = $Version;
                    Platform = $Platform;
                } | Write-Output;
            }
        } else {
            foreach ($s in $InputString) {
                $v = $s;
                $i = $v.IndexOf('@');
                $p = '';
                if ($i -gt 0) {
                    $p = $v.Substring($i + 1);
                    $v = $v.Substring(0, $i);
                }
                $i = $v.IndexOf('-');
                if ($i -lt 0) {
                    [ExtensionIdentity]@{
                        ID = '';
                        Version = $v;
                        Platform = $p;
                    } | Write-Output;
                } else {
                    [ExtensionIdentity]@{
                        ID = $v.Substring(0, $i);
                        Version = $v.Substring($i + 1);
                        Platform = $p;
                    } | Write-Output;
                }
            }
        }
    }
}

Function Read-ExtensionVsixManifest {
    <#
    .SYNOPSIS
        Gets package information from VSIX files.
    .DESCRIPTION
        Reads extension manifest from VSIX package files.
    #>
    [CmdletBinding()]
    [OutputType([VsixFileInfo])]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'ByFile')]
        [Alias('FullName')]
        # Path to one or more VSIX package files.
        [string[]]$Path,

        [Parameter(Mandatory = $true, ParameterSetName = 'FromFiles')]
        # Path to a subdirectory containing VSIX package files.
        [VsixFileInfo[]]$File,

        [Parameter(Mandatory = $true, ParameterSetName = 'FromRepository')]
        # Path to a subdirectory containing VSIX package files.
        [string]$RepositoryPath,

        [Parameter(ParameterSetName = 'FromFiles')]
        [switch]$PassThru,

        [Parameter(ParameterSetName = 'FromFiles')]
        [switch]$Force,

        [Parameter(ParameterSetName = 'ByFile')]
        [Parameter(ParameterSetName = 'FromRepository')]
        [switch]$LoadManifest
    )

    Process {
        if ($PSCmdlet.ParameterSetName -eq 'FromRepository') {
            if ($LoadManifest.IsPresent) {
                (Get-ChildItem -LiteralPath $RepositoryPath -Filter '*.vsix') | Where-Object { -not $_.PSIsContainer } | Read-ExtensionVsixManifest -LoadManifest;
            } else {
                (Get-ChildItem -LiteralPath $RepositoryPath -Filter '*.vsix') | Where-Object { -not $_.PSIsContainer } | Read-ExtensionVsixManifest;
            }
        } else {
            if ($PSCmdlet.ParameterSetName -eq 'FromFiles') {
                Function LoadFromFile([VsixFileInfo]$VsixFileInfo) {
                    (Use-TempFolder {
                        Expand-Archive -LiteralPath $VsixFileInfo.Path -DestinationPath $_ -Force -ErrorAction Stop;
                        $TempPath = $_ | Join-Path -ChildPath 'extension.vsixmanifest';
                        if ($TempPath | Test-Path -PathType Leaf) {
                            [Xml]$Xml = Get-Content -LiteralPath $MPath -Force;
                            if ($null -ne $Xml) {
                                $nsmgr = [System.Xml.XmlNamespaceManager]::new($Xml.NameTable);
                                $nsmgr.AddNamespace('vsx', $Xml.DocumentElement.PSBase.NamespaceURI);
                                $XmlElement = $Xml.SelectSingleNode('/vsx:PackageManifest/vsx:Metadata/vsx:Identity', $nsmgr);
                                if ($null -ne $XmlElement) {
                                    $Identity = New-ExtensionIdentity -Publisher $IdentityElement.PSBase.GetAttribute('Publisher') -ID $IdentityElement.PSBase.GetAttribute('Id') -Version $IdentityElement.PSBase.GetAttribute('Version') -Platform $IdentityElement.PSBase.GetAttribute('TargetPlatform');
                                    if ($null -ne $Identity) {
                                        $VsixFileInfo.Identity = $Identity;
                                        $XmlElement = $Xml.SelectSingleNode('/vsx:PackageManifest/vsx:Metadata/vsx:DisplayName', $nsmgr);
                                        if ($null -ne $XmlElement -and -not $XmlElement.IsEmpty) { $VsixFileInfo.DisplayName = $XmlElement.InnerText }
                                        $XmlElement = $Xml.SelectSingleNode('/vsx:PackageManifest/vsx:Metadata/vsx:Description', $nsmgr);
                                        if ($null -ne $XmlElement -and -not $XmlElement.IsEmpty) { $VsixFileInfo.Description = $XmlElement.InnerText }
                                        $XmlElement = $Xml.SelectSingleNode('/vsx:PackageManifest/vsx:Metadata/vsx:Icon', $nsmgr);
                                        if ($null -ne $XmlElement -and -not $XmlElement.IsEmpty) { $VsixFileInfo.Icon = $XmlElement.InnerText }
                                        $TempPath = $_ | Join-Path -ChildPath 'extension/package.json';
                                        if ($TempPath | Test-Path -PathType Leaf) {
                                            $PackageJson = (Get-Content -LiteralPath $TempPath -Force) | ConvertFrom-Json -Depth 4;
                                            if ($null -ne $PackageJson) {
                                                if ([string]::IsNullOrWhiteSpace($VsixFileInfo.DisplayName)) { $VsixFileInfo.DisplayName = $PackageJson.displayName }
                                                if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Description)) { $VsixFileInfo.Description = $PackageJson.description }
                                                if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Icon)) { $VsixFileInfo.Icon = $PackageJson.icon }
                                            }
                                        }
                                        $VsixFileInfo.FromManifest = $true;
                                    }
                                }
                            }
                        }
                    });
                }
                if ($Force.IsPresent) {
                    if ($PassThru.IsPresent) {
                        foreach ($VsixFileInfo in $File) {
                            LoadFromFile($VsixFileInfo);
                            $VsixFileInfo | Write-Output;
                        }
                    } else {
                        foreach ($VsixFileInfo in $File) {
                            LoadFromFile($VsixFileInfo);
                        }
                    }
                } else {
                    if ($PassThru.IsPresent) {
                        foreach ($VsixFileInfo in $File) {
                            if (-not $VsixFileInfo.FromManifest) {
                                LoadFromFile($VsixFileInfo);
                            }
                            $VsixFileInfo | Write-Output;
                        }
                    } else {
                        foreach ($VsixFileInfo in $File) {
                            if (-not $VsixFileInfo.FromManifest) {
                                LoadFromFile($VsixFileInfo);
                            }
                        }
                    }
                }
            } else {
                if ($LoadManifest.IsPresent) {
                    foreach ($P in $Path) {
                        if (Test-Path -LiteralPath $P -PathType Leaf) {
                            (Use-TempFolder {
                                Expand-Archive -LiteralPath $P -DestinationPath $_ -Force -ErrorAction Stop;
                                $TempPath = $_ | Join-Path -ChildPath 'extension.vsixmanifest';
                                if ($TempPath | Test-Path -PathType Leaf) {
                                    [Xml]$Xml = Get-Content -LiteralPath $MPath -Force;
                                    if ($null -ne $Xml) {
                                        $nsmgr = [System.Xml.XmlNamespaceManager]::new($Xml.NameTable);
                                        $nsmgr.AddNamespace('vsx', $Xml.DocumentElement.PSBase.NamespaceURI);
                                        $XmlElement = $Xml.SelectSingleNode('/vsx:PackageManifest/vsx:Metadata/vsx:Identity', $nsmgr);
                                        if ($null -ne $XmlElement) {
                                            $Identity = New-ExtensionIdentity -Publisher $IdentityElement.PSBase.GetAttribute('Publisher') -ID $IdentityElement.PSBase.GetAttribute('Id') -Version $IdentityElement.PSBase.GetAttribute('Version') -Platform $IdentityElement.PSBase.GetAttribute('TargetPlatform');
                                            if ($null -ne $Identity) {
                                                $Manifest = [VsixFileInfo]@{
                                                    Identity = $Identity;
                                                    Path = $_.FullName;
                                                };
                                                $XmlElement = $Xml.SelectSingleNode('/vsx:PackageManifest/vsx:Metadata/vsx:DisplayName', $nsmgr);
                                                if ($null -ne $XmlElement -and -not $XmlElement.IsEmpty) { $Manifest.DisplayName = $XmlElement.InnerText }
                                                $XmlElement = $Xml.SelectSingleNode('/vsx:PackageManifest/vsx:Metadata/vsx:Description', $nsmgr);
                                                if ($null -ne $XmlElement -and -not $XmlElement.IsEmpty) { $Manifest.Description = $XmlElement.InnerText }
                                                $XmlElement = $Xml.SelectSingleNode('/vsx:PackageManifest/vsx:Metadata/vsx:Icon', $nsmgr);
                                                if ($null -ne $XmlElement -and -not $XmlElement.IsEmpty) { $Manifest.Icon = $XmlElement.InnerText }
                                                $TempPath = $_ | Join-Path -ChildPath 'extension/package.json';
                                                if ($TempPath | Test-Path -PathType Leaf) {
                                                    $PackageJson = (Get-Content -LiteralPath $TempPath -Force) | ConvertFrom-Json -Depth 4;
                                                    if ($null -ne $PackageJson) {
                                                        if ([string]::IsNullOrWhiteSpace($Manifest.DisplayName)) { $Manifest.DisplayName = $PackageJson.displayName }
                                                        if ([string]::IsNullOrWhiteSpace($Manifest.Description)) { $Manifest.Description = $PackageJson.description }
                                                        if ([string]::IsNullOrWhiteSpace($Manifest.Icon)) { $Manifest.Icon = $PackageJson.icon }
                                                    }
                                                }
                                                $Manifest | Write-Output;
                                            }
                                        }
                                    }
                                }
                            });
                        } else {
                            Write-Error -Message "File $P not found" -Category ObjectNotFound -ErrorId 'FileNotFound' -TargetObject $P -CategoryTargetName 'Path';
                        }
                    }
                } else {
                    foreach ($P in $Path) {
                        if (($P | Test-Path -IsValid) -and (Test-Path -LiteralPath $P -PathType Leaf)) {
                            $Identity = New-ExtensionIdentity -InputString ($P | Split-Path -LeafBase);
                            if ($null -ne $Identity) {
                                [VsixFileInfo]@{
                                    Identity = $Identity;
                                    Path = $_.FullName;
                                } | Write-Output;
                            }
                        } else {
                            Write-Error -Message "File $P not found" -Category ObjectNotFound -ErrorId 'FileNotFound' -TargetObject $P -CategoryTargetName 'Path';
                        }
                    }
                }
            }
        }
    }
}

Function Compare-ExtensionIdentity {
    <#
    .SYNOPSIS
        Compares 2 ExtensionIdentity objects.
    .DESCRIPTION
        Returns a numerical value indicating whether one ExtensionIdentity is less than, greater than, or equal to another.
    #>
    [CmdletBinding()]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # The ExtensionIdentity to be compared.
        [ExtensionIdentity]$Current,

        [Parameter(Mandatory = $true, Position = 1)]
        # The ExtensionIdentity to compare to.
        [ExtensionIdentity]$Other
    )

    if ([object]::ReferenceEquals($Current, $Other)) { return 0 }

    $diff = Compare-VersionStrings -LVersion $Current.ID -RVersion $Other.ID;
    if ($diff -ne 0 -or ($diff = Compare-VersionStrings -LVersion $Current.Version -RVersion $Other.Version) -ne 0) { return $diff }
    return [System.StringComparer]::OrdinalIgnoreCase.Compare($Current.Platform, $Other.Platform);
}

Function Optimize-ExtensionVsixManifestOrder {
    <#
    .SYNOPSIS
        Sorts ExtensionVsixManifest objects.
    .DESCRIPTION
        Sorts ExtensionVsixManifest by their Identity.
    #>
    [CmdletBinding(DefaultParameterSetName = 'Ascending')]
    [OutputType([ExtensionVsixManifest[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The ExtensionVsixManifest to be sorted.
        [ExtensionVsixManifest[]]$InputObject,

        [Parameter(Mandatory = $true, ParameterSetName = 'Descending')]
        # Sort items in descending order.
        [switch]$Descending,

        [Parameter(ParameterSetName = 'Ascending')]
        # Sort items in ascending order (default behavior).
        [switch]$Ascending,

        [ValidateSet('RemoveAndWarn', 'Remove', 'Keep', 'Warn', 'Error')]
        # Action to take when items with duplicate identifiers are encountered.
        [string]$DuplicateAction = 'RemoveAndWarn'
    )

    Begin {
        $SortedObjects = [System.Collections.Generic.LinkedList[ExtensionVsixManifest]]::new();
        $Position = -1;
    }

    Process {
        if ($Descending.IsPresent) {
            foreach ($Current in $InputObject) {
                $Position++;
                $SortedIndex = -1;
                [System.Collections.Generic.LinkedListNode]$Other = $SortedObjects.First;
                $Diff = 0;
                while ($null -ne $Other) {
                    $SortedIndex++;
                    $Diff = Compare-ExtensionIdentity -Current $Current.Identity -Other $Other.Value.Identity;
                    if ($Diff -le 0) { break }
                    $Other = $Other.Next;
                }
                if ($null -eq $Other) {
                    $SortedObjects.AddLast($Current) | Out-Null;
                } else {
                    if ($Diff -eq 0) {
                        if ($DuplicateAction -eq 'Remove') {
                            Write-Debug -Message "Skipping item $($Item.Identity) which has the same Identifier as a previous item.";
                            continue;
                        }
                        if ($DuplicateAction -eq 'RemoveAndWarn') {
                            Write-Warning -Message "Extension manifest at position $Position is duplicate of '$($Item.Identity)' at sorted index $SortedIndex";
                            continue;
                        }
                        switch ($DuplicateAction) {
                            'Warn' {
                                Write-Warning -Message "Extension manifest at position $Position is duplicate of '$($Item.Identity)' at sorted index $SortedIndex";
                                break;
                            }
                            'Error' {
                                Write-Error -Message "Extension manifest at position $Position is duplicate of '$($Item.Identity)' at sorted index $SortedIndex" -Category ResourceExists -ErrorId 'DuplicateExtensionVsixManifest' `
                                    -TargetObject $Item -CategoryActivity 'Optimize-ExtensionVsixManifestOrder' -CategoryReason "Existing extension manifest at index $SortedIndex had same identifier as a subsequent item";
                                break;
                            }
                        }
                    }
                    $SortedObjects.AddBefore($Other, $Current) | Out-Null;
                }
            }
        } else {
            foreach ($Current in $InputObject) {
                $Position++;
                $SortedIndex = -1;
                [System.Collections.Generic.LinkedListNode]$Other = $SortedObjects.Last;
                $Diff = 0;
                while ($null -ne $Other) {
                    $SortedIndex++;
                    $Diff = Compare-ExtensionIdentity -Current $Current.Identity -Other $Other.Value.Identity;
                    if ($Diff -ge 0) { break }
                    $Other = $Other.Previous;
                }
                if ($null -eq $Other) {
                    $SortedObjects.AddFirst($Current) | Out-Null;
                } else {
                    if ($Diff -eq 0) {
                        if ($DuplicateAction -eq 'Remove') {
                            Write-Debug -Message "Skipping item $($Item.Identity)Item which has the same Identifier as a previous item.";
                            continue;
                        }
                        switch ($DuplicateAction) {
                            'Warn' {
                                Write-Warning -Message "Extension manifest at position $Position is duplicate of '$($Item.Identity)' at sorted index $SortedIndex";
                                break;
                            }
                            'Error' {
                                Write-Error -Message "Extension manifest at position $Position is duplicate of '$($Item.Identity)' at sorted index $SortedIndex" -Category ResourceExists -ErrorId 'DuplicateExtensionVsixManifest' `
                                    -TargetObject $Item -CategoryActivity 'Optimize-ExtensionVsixManifestOrder' -CategoryReason "Existing extension manifest at index $SortedIndex had same identifier as a subsequent item";
                                break;
                            }
                        }
                    }
                    $SortedObjects.AddAfter($Other, $Current) | Out-Null;
                }
            }
        }
    }

    End {
        $SortedObjects | Write-Output;
    }
}

Function Read-VsixExtensionIndex {
    <#
    .SYNOPSIS
        Reads from the index file of an extension repository.
    .DESCRIPTION
        Reads the contents of the index.json file of a repository.
    #>
    [CmdletBinding()]
    [OutputType([ExtensionVsixManifest[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # The path to a folder that contains VSIX files.
        [string]$RepositoryPath
    )

    $Result = [System.Collections.ObjectModel.Collection[ExtensionVsixManifest]]::new();
    $IndexPath = $RepositoryPath | Join-Path -ChildPath 'index.json';
    if ($IndexPath | Test-Path -PathType Leaf) {
        $Content = $null;
        try { $Content = Get-Content -LiteralPath $IndexPath -Encoding utf8 }
        catch {
            Write-Error -ErrorRecord $_ -CategoryReason "Failed to read content from $IndexPath";
            Write-Output -InputObject $Result -NoEnumerate;
            return;
        }
        if ([string]::IsNullOrWhiteSpace($Content)) {
            Write-Error -Message 'Index file does not contain valid JSON data' -Category InvalidData -ErrorId 'EmptyIndexFile' -CategoryReason "$IndexFilePath is empty or contains only whitespace";
            Write-Output -InputObject $Result -NoEnumerate;
            return;
        }
        [object[]]$ParsedJson = $null;
        try { [object[]]$ParsedJson = @($Content | ConvertFrom-Json -Depth 3) }
        catch {
            Write-Error -ErrorRecord $_ -CategoryReason "Failed to parse content of $IndexPath as JSON";
            Write-Output -InputObject $Result -NoEnumerate;
            return;
        }
        $ObjNum = 0;
        foreach ($JsonElement in $ParsedJson) {
            $ObjNum++;
            try {
                if ([string]::IsNullOrWhiteSpace($JsonElement.Platform)) {
                    if ([string]::IsNullOrWhiteSpace($JsonElement.DisplayName)) {
                        if ([string]::IsNullOrWhiteSpace($JsonElement.Description)) {
                            [ExtensionVsixManifest]@{
                                Identity = [ExtensionIdentity] @{
                                    ID = $JsonElement.ID;
                                    Version = $JsonElement.Version;
                                };
                            } | Write-Output;
                        } else {
                            [ExtensionVsixManifest]@{
                                Identity = [ExtensionIdentity] @{
                                    ID = $JsonElement.ID;
                                    Version = $JsonElement.Version;
                                };
                                Description = $JsonElement.Description;
                            } | Write-Output;
                        }
                    } else {
                        if ([string]::IsNullOrWhiteSpace($JsonElement.Description)) {
                            [ExtensionVsixManifest]@{
                                Identity = [ExtensionIdentity] @{
                                    ID = $JsonElement.ID;
                                    Version = $JsonElement.Version;
                                };
                                DisplayName = $JsonElement.DisplayName;
                            } | Write-Output;
                        } else {
                            [ExtensionVsixManifest]@{
                                Identity = [ExtensionIdentity] @{
                                    ID = $JsonElement.ID;
                                    Version = $JsonElement.Version;
                                };
                                DisplayName = $JsonElement.DisplayName;
                                Description = $JsonElement.Description;
                            } | Write-Output;
                        }
                    }
                } else {
                    if ([string]::IsNullOrWhiteSpace($JsonElement.DisplayName)) {
                        if ([string]::IsNullOrWhiteSpace($JsonElement.Description)) {
                            [ExtensionVsixManifest]@{
                                Identity = [ExtensionIdentity] @{
                                    ID = $JsonElement.ID;
                                    Version = $JsonElement.Version;
                                    Platform = $JsonElement.Platform;
                                };
                            } | Write-Output;
                        } else {
                            [ExtensionVsixManifest]@{
                                Identity = [ExtensionIdentity] @{
                                    ID = $JsonElement.ID;
                                    Version = $JsonElement.Version;
                                    Platform = $JsonElement.Platform;
                                };
                                Description = $JsonElement.Description;
                            } | Write-Output;
                        }
                    } else {
                        if ([string]::IsNullOrWhiteSpace($JsonElement.Description)) {
                            [ExtensionVsixManifest]@{
                                Identity = [ExtensionIdentity] @{
                                    ID = $JsonElement.ID;
                                    Version = $JsonElement.Version;
                                    Platform = $JsonElement.Platform;
                                };
                                DisplayName = $JsonElement.DisplayName;
                            } | Write-Output;
                        } else {
                            [ExtensionVsixManifest]@{
                                Identity = [ExtensionIdentity] @{
                                    ID = $JsonElement.ID;
                                    Version = $JsonElement.Version;
                                    Platform = $JsonElement.Platform;
                                };
                                DisplayName = $JsonElement.DisplayName;
                                Description = $JsonElement.Description;
                            } | Write-Output;
                        }
                    }
                }
            }
            catch {
                Write-Error -ErrorRecord $_ -CategoryReason "Failed to convert object #$ObjNum $($JsonElement | ConvertTo-Json) in $IndexPath as JSON";
            }
        }
    }
}

Function Write-VsixExtensionIndex {
    <#
    .SYNOPSIS
        Writes to the index file of an extension repository.
    .DESCRIPTION
        Saves VSIX extension information to the index.json file of a repository.
    #>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        # Objects representing the VSIX extensions.
        [ExtensionVsixManifest[]]$InputObject,

        [Parameter(Mandatory = $true)]
        # The path to a folder that contains VSIX files.
        [string]$RepositoryPath
    )

    Begin {
        $AllItems = [System.Collections.ObjectModel.Collection[ExtensionVsixManifest]]::new();
    }

    Process {
        foreach ($Item in $InputObject) { $AllItems.Add($InputObject) }
    }

    End {
        $Sorted = @();
        if ($AllItems.Count -gt 1) {
            $Sorted = @($AllItems | Optimize-ExtensionVsixManifestOrder);
        } else {
            $Sorted = @($AllItems);
        }
        $IndexPath = $RepositoryPath | Join-Path -ChildPath 'index.json';
        try {
            (ConvertTo-Json -Depth 3 -InputObject ([object[]]@($Sorted | ForEach-Object {
                $Item = [PSCustomObject]@{
                    ID = $_.Identity.ID;
                    Version = $_.Identity.Version;
                };
                if (-not [string]::IsNullOrWhiteSpace($_.Identity.Platform)) {
                    $Item | Add-Member -MemberType NoteProperty -Name 'Platform' -Value $_.Identity.Platform;
                }
                if (-not [string]::IsNullOrWhiteSpace($_.DisplayName)) {
                    $Item | Add-Member -MemberType NoteProperty -Name 'DisplayName' -Value $_.DisplayName;
                }
                if (-not [string]::IsNullOrWhiteSpace($_.Description)) {
                    $Item | Add-Member -MemberType NoteProperty -Name 'Description' -Value $_.Description -PassThru;
                } else {
                    $Item | Write-Output;
                }
            }))) | Set-Content -LiteralPath $IndexPath;
        } catch {
            Write-Error -ErrorRecord $_ -CategoryReason "Failed to write to $IndexPath";
        }
    }
}

Function Select-VsixExtension {
    <#
    .SYNOPSIS
        Gets matching VSIX extensions according to identity.
    .DESCRIPTION
        Gets VSIX extensions that match the given identities.
    #>
    [CmdletBinding()]
    [OutputType([ExtensionVsixManifest[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        [ExtensionVsixManifest[]]$InputObject,

        [ExtensionIdentity[]]$Identity
    )

    Process {
        foreach ($Item in $InputObject) {
            foreach ($Id in $Identity) {
                if ((Compare-ExtensionIdentity -Current $Id -Other $Item.Identity) -eq 0) {
                    $Item | Write-Output;
                    break;
                }
            }
        }
    }
}

Function Skip-VsixExtension {
    <#
    .SYNOPSIS
        Gets VSIX extensions that do not match specifid identies.
    .DESCRIPTION
        Gets VSIX extensions that do not match the given identities.
    #>
    [CmdletBinding()]
    [OutputType([ExtensionVsixManifest[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        [ExtensionVsixManifest[]]$InputObject,

        [ExtensionIdentity[]]$Identity
    )

    Process {
        foreach ($Item in $InputObject) {
            $NoMatch = $true;
            foreach ($Id in $Identity) {
                if ((Compare-ExtensionIdentity -Current $Id -Other $Item.Identity) -eq 0) {
                    $NoMatch = $false;
                    break;
                }
            }
            if ($NoMatch) {
                $Item | Write-Output;
            }
        }
    }
}

Function Merge-VsixExtensions {
    <#
    .SYNOPSIS
        Merges a VSIX extension objects.
    .DESCRIPTION
        Merges extension file objects with extension index objects.
    #>
    [CmdletBinding(DefaultParameterSetName = 'Quicker')]
    [OutputType([ExtensionVsixManifest[]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [AllowEmptyCollection()]
        # Represents the existing package files in the repository.
        [VsixFileInfo[]]$InputObject,

        # Represents packages currently indexed.
        [Parameter(Mandatory = $true)]
        [ExtensionVsixManifest[]]$IndexItems,

        # If specified, all new packages are added to this collection.
        [System.Collections.ObjectModel.Collection[VsixFileInfo]]$Added,

        # If specified, all packages that no longer exist are added to this collection.
        [System.Collections.ObjectModel.Collection[ExtensionVsixManifest]]$Removed,

        [Parameter(ParameterSetName = 'ForceUpdate')]
        # If specified, all updated package info items are added to this collection.
        [System.Collections.ObjectModel.Collection[VsixFileInfo]]$Updated,

        [Parameter(Mandatory = $true, ParameterSetName = 'ForceUpdate')]
        # Load manifest from the source file for matching packages.
        [switch]$Force
    )

    Begin {
        $ToRemove = [System.Collections.Generic.LinkedList[ExtensionVsixManifest]]::new($IndexItems);
    }

    Process {
        if ($Force.IsPresent) {
            if ($PSBoundParameters.ContainsKey('Updated')) {
                if ($PSBoundParameters.ContainsKey('Added')) {
                    foreach ($VsixFileInfo in $InputObject) {
                        $Identity = $VsixFileInfo.Identity;
                        $NotMatched = $true;
                        for ($n  = $ToRemove.First; $null -ne $n; $n = $n.Next) {
                            $o = $n.Value.Identity;
                            if ((Compare-ExtensionIdentity -Current $Identity -Other $o) -eq 0) {
                                $NotMatched = $false;
                                $EmitFileInfo = $false;
                                if (-not $VsixFileInfo.FromManifest) { Read-ExtensionVsixManifest -File $VsixFileInfo }
                                if ($VsixFileInfo.FromManifest) {
                                    if ($Identity.ID -ine $o.ID -or $Identity.Version -ine $o.Version -or $Identity.Platform -ine $o.Platform) {
                                        $EmitFileInfo = $true;
                                        if ([string]::IsNullOrWhiteSpace($VsixFileInfo.DisplayName)) { $VsixFileInfo.DisplayName = $n.Value.DisplayName }
                                        if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Description)) { $VsixFileInfo.Description = $n.Value.Description }
                                        if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Icon)) { $VsixFileInfo.Icon = $n.Value.Icon }
                                    } else {
                                        if ($VsixFileInfo.DisplayName -ine $n.Value.DisplayName -and -not [string]::IsNullOrWhiteSpace($VsixFileInfo.DisplayName)) {
                                            $EmitFileInfo = $true;
                                            if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Description)) { $VsixFileInfo.Description = $n.Value.Description }
                                            if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Icon)) { $VsixFileInfo.Icon = $n.Value.Icon }
                                        } else {
                                            if ($VsixFileInfo.Description -ine $n.Value.Description -and -not [string]::IsNullOrWhiteSpace($VsixFileInfo.Description)) {
                                                $EmitFileInfo = $true;
                                                if ([string]::IsNullOrWhiteSpace($VsixFileInfo.DisplayName)) { $VsixFileInfo.DisplayName = $n.Value.DisplayName }
                                                if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Icon)) { $VsixFileInfo.Icon = $n.Value.Icon }
                                            } else {
                                                if ($VsixFileInfo.Icon -ine $n.Value.Icon -and -not [string]::IsNullOrWhiteSpace($VsixFileInfo.Icon)) {
                                                    $EmitFileInfo = $true;
                                                    if ([string]::IsNullOrWhiteSpace($VsixFileInfo.DisplayName)) { $VsixFileInfo.DisplayName = $n.Value.DisplayName }
                                                    if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Description)) { $VsixFileInfo.Description = $n.Value.Description }
                                                }
                                            }
                                        }
                                    }
                                }
                                if ($EmitFileInfo) {
                                    $Updated.Add($VsixFileInfo);
                                    $VsixFileInfo | Write-Output;
                                } else {
                                    $n.Value | Write-Output;
                                }
                                $ToRemove.Remove($n) | Out-Null;
                                break;
                            }
                        }
                        if ($NotMatched) {
                            Write-Information -MessageData "Adding $Identity";
                            if (-not $VsixFileInfo.FromManifest) { Read-ExtensionVsixManifest -File $VsixFileInfo }
                            if ($VsixFileInfo.FromManifest) {
                                $Added.Add($VsixFileInfo);
                                $VsixFileInfo | Write-Output;
                            }
                        }
                    }
                } else {
                    foreach ($VsixFileInfo in $InputObject) {
                        $Identity = $VsixFileInfo.Identity;
                        $NotMatched = $true;
                        for ($n  = $ToRemove.First; $null -ne $n; $n = $n.Next) {
                            if ((Compare-ExtensionIdentity -Current $Identity -Other $n.Value.Identity) -eq 0) {
                                $NotMatched = $false;
                                $EmitFileInfo = $false;
                                if (-not $VsixFileInfo.FromManifest) { Read-ExtensionVsixManifest -File $VsixFileInfo }
                                if ($VsixFileInfo.FromManifest) {
                                    if ($Identity.ID -ine $o.ID -or $Identity.Version -ine $o.Version -or $Identity.Platform -ine $o.Platform) {
                                        $EmitFileInfo = $true;
                                        if ([string]::IsNullOrWhiteSpace($VsixFileInfo.DisplayName)) { $VsixFileInfo.DisplayName = $n.Value.DisplayName }
                                        if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Description)) { $VsixFileInfo.Description = $n.Value.Description }
                                        if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Icon)) { $VsixFileInfo.Icon = $n.Value.Icon }
                                    } else {
                                        if ($VsixFileInfo.DisplayName -ine $n.Value.DisplayName -and -not [string]::IsNullOrWhiteSpace($VsixFileInfo.DisplayName)) {
                                            $EmitFileInfo = $true;
                                            if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Description)) { $VsixFileInfo.Description = $n.Value.Description }
                                            if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Icon)) { $VsixFileInfo.Icon = $n.Value.Icon }
                                        } else {
                                            if ($VsixFileInfo.Description -ine $n.Value.Description -and -not [string]::IsNullOrWhiteSpace($VsixFileInfo.Description)) {
                                                $EmitFileInfo = $true;
                                                if ([string]::IsNullOrWhiteSpace($VsixFileInfo.DisplayName)) { $VsixFileInfo.DisplayName = $n.Value.DisplayName }
                                                if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Icon)) { $VsixFileInfo.Icon = $n.Value.Icon }
                                            } else {
                                                if ($VsixFileInfo.Icon -ine $n.Value.Icon -and -not [string]::IsNullOrWhiteSpace($VsixFileInfo.Icon)) {
                                                    $EmitFileInfo = $true;
                                                    if ([string]::IsNullOrWhiteSpace($VsixFileInfo.DisplayName)) { $VsixFileInfo.DisplayName = $n.Value.DisplayName }
                                                    if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Description)) { $VsixFileInfo.Description = $n.Value.Description }
                                                }
                                            }
                                        }
                                    }
                                }
                                if ($EmitFileInfo) {
                                    $Updated.Add($VsixFileInfo);
                                    $VsixFileInfo | Write-Output;
                                } else {
                                    $n.Value | Write-Output;
                                }
                                $ToRemove.Remove($n) | Out-Null;
                                break;
                            }
                        }
                        if ($NotMatched) {
                            Write-Information -MessageData "Adding $Identity";
                            if (-not $VsixFileInfo.FromManifest) { Read-ExtensionVsixManifest -File $VsixFileInfo }
                            if ($VsixFileInfo.FromManifest) { $VsixFileInfo | Write-Output }
                        }
                    }
                }
            } else {
                if ($PSBoundParameters.ContainsKey('Added')) {
                    foreach ($VsixFileInfo in $InputObject) {
                        $Identity = $VsixFileInfo.Identity;
                        $NotMatched = $true;
                        for ($n  = $ToRemove.First; $null -ne $n; $n = $n.Next) {
                            $o = $n.Value.Identity;
                            if ((Compare-ExtensionIdentity -Current $Identity -Other $o) -eq 0) {
                                $NotMatched = $false;
                                $EmitFileInfo = $false;
                                if (-not $VsixFileInfo.FromManifest) { Read-ExtensionVsixManifest -File $VsixFileInfo }
                                if ($VsixFileInfo.FromManifest) {
                                    if ($Identity.ID -ine $o.ID -or $Identity.Version -ine $o.Version -or $Identity.Platform -ine $o.Platform) {
                                        $EmitFileInfo = $true;
                                        if ([string]::IsNullOrWhiteSpace($VsixFileInfo.DisplayName)) { $VsixFileInfo.DisplayName = $n.Value.DisplayName }
                                        if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Description)) { $VsixFileInfo.Description = $n.Value.Description }
                                        if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Icon)) { $VsixFileInfo.Icon = $n.Value.Icon }
                                    } else {
                                        if ($VsixFileInfo.DisplayName -ine $n.Value.DisplayName -and -not [string]::IsNullOrWhiteSpace($VsixFileInfo.DisplayName)) {
                                            $EmitFileInfo = $true;
                                            if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Description)) { $VsixFileInfo.Description = $n.Value.Description }
                                            if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Icon)) { $VsixFileInfo.Icon = $n.Value.Icon }
                                        } else {
                                            if ($VsixFileInfo.Description -ine $n.Value.Description -and -not [string]::IsNullOrWhiteSpace($VsixFileInfo.Description)) {
                                                $EmitFileInfo = $true;
                                                if ([string]::IsNullOrWhiteSpace($VsixFileInfo.DisplayName)) { $VsixFileInfo.DisplayName = $n.Value.DisplayName }
                                                if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Icon)) { $VsixFileInfo.Icon = $n.Value.Icon }
                                            } else {
                                                if ($VsixFileInfo.Icon -ine $n.Value.Icon -and -not [string]::IsNullOrWhiteSpace($VsixFileInfo.Icon)) {
                                                    $EmitFileInfo = $true;
                                                    if ([string]::IsNullOrWhiteSpace($VsixFileInfo.DisplayName)) { $VsixFileInfo.DisplayName = $n.Value.DisplayName }
                                                    if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Description)) { $VsixFileInfo.Description = $n.Value.Description }
                                                }
                                            }
                                        }
                                    }
                                }
                                if ($EmitFileInfo) {
                                    $VsixFileInfo | Write-Output;
                                } else {
                                    $n.Value | Write-Output;
                                }
                                $ToRemove.Remove($n) | Out-Null;
                                break;
                            }
                        }
                        if ($NotMatched) {
                            Write-Information -MessageData "Adding $Identity";
                            if (-not $VsixFileInfo.FromManifest) { Read-ExtensionVsixManifest -File $VsixFileInfo }
                            if ($VsixFileInfo.FromManifest) {
                                $Added.Add($VsixFileInfo);
                                $VsixFileInfo | Write-Output;
                            }
                        }
                    }
                } else {
                    foreach ($VsixFileInfo in $InputObject) {
                        $Identity = $VsixFileInfo.Identity;
                        $NotMatched = $true;
                        for ($n  = $ToRemove.First; $null -ne $n; $n = $n.Next) {
                            if ((Compare-ExtensionIdentity -Current $Identity -Other $n.Value.Identity) -eq 0) {
                                $NotMatched = $false;
                                $EmitFileInfo = $false;
                                if (-not $VsixFileInfo.FromManifest) { Read-ExtensionVsixManifest -File $VsixFileInfo }
                                if ($VsixFileInfo.FromManifest) {
                                    if ($Identity.ID -ine $o.ID -or $Identity.Version -ine $o.Version -or $Identity.Platform -ine $o.Platform) {
                                        $EmitFileInfo = $true;
                                        if ([string]::IsNullOrWhiteSpace($VsixFileInfo.DisplayName)) { $VsixFileInfo.DisplayName = $n.Value.DisplayName }
                                        if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Description)) { $VsixFileInfo.Description = $n.Value.Description }
                                        if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Icon)) { $VsixFileInfo.Icon = $n.Value.Icon }
                                    } else {
                                        if ($VsixFileInfo.DisplayName -ine $n.Value.DisplayName -and -not [string]::IsNullOrWhiteSpace($VsixFileInfo.DisplayName)) {
                                            $EmitFileInfo = $true;
                                            if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Description)) { $VsixFileInfo.Description = $n.Value.Description }
                                            if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Icon)) { $VsixFileInfo.Icon = $n.Value.Icon }
                                        } else {
                                            if ($VsixFileInfo.Description -ine $n.Value.Description -and -not [string]::IsNullOrWhiteSpace($VsixFileInfo.Description)) {
                                                $EmitFileInfo = $true;
                                                if ([string]::IsNullOrWhiteSpace($VsixFileInfo.DisplayName)) { $VsixFileInfo.DisplayName = $n.Value.DisplayName }
                                                if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Icon)) { $VsixFileInfo.Icon = $n.Value.Icon }
                                            } else {
                                                if ($VsixFileInfo.Icon -ine $n.Value.Icon -and -not [string]::IsNullOrWhiteSpace($VsixFileInfo.Icon)) {
                                                    $EmitFileInfo = $true;
                                                    if ([string]::IsNullOrWhiteSpace($VsixFileInfo.DisplayName)) { $VsixFileInfo.DisplayName = $n.Value.DisplayName }
                                                    if ([string]::IsNullOrWhiteSpace($VsixFileInfo.Description)) { $VsixFileInfo.Description = $n.Value.Description }
                                                }
                                            }
                                        }
                                    }
                                }
                                if ($EmitFileInfo) {
                                    $VsixFileInfo | Write-Output;
                                } else {
                                    $n.Value | Write-Output;
                                }
                                $ToRemove.Remove($n) | Out-Null;
                                break;
                            }
                        }
                        if ($NotMatched) {
                            Write-Information -MessageData "Adding $Identity";
                            if (-not $VsixFileInfo.FromManifest) { Read-ExtensionVsixManifest -File $VsixFileInfo }
                            if ($VsixFileInfo.FromManifest) { $VsixFileInfo | Write-Output }
                        }
                    }
                }
            }
        } else {
            if ($PSBoundParameters.ContainsKey('Added')) {
                foreach ($VsixFileInfo in $InputObject) {
                    $Identity = $VsixFileInfo.Identity;
                    $NotMatched = $true;
                    for ($n  = $ToRemove.First; $null -ne $n; $n = $n.Next) {
                        if ((Compare-ExtensionIdentity -Current $Identity -Other $n.Value.Identity) -eq 0) {
                            $NotMatched = $false;
                            $n.Value | Write-Output;
                            $ToRemove.Remove($n) | Out-Null;
                            break;
                        }
                    }
                    if ($NotMatched) {
                        Write-Information -MessageData "Adding $Identity";
                        if (-not $VsixFileInfo.FromManifest) { Read-ExtensionVsixManifest -File $VsixFileInfo }
                        if ($VsixFileInfo.FromManifest) {
                            $Added.Add($VsixFileInfo);
                            $VsixFileInfo | Write-Output;
                        }
                    }
                }
            } else {
                foreach ($VsixFileInfo in $InputObject) {
                    $Identity = $VsixFileInfo.Identity;
                    $NotMatched = $true;
                    for ($n  = $ToRemove.First; $null -ne $n; $n = $n.Next) {
                        if ((Compare-ExtensionIdentity -Current $Identity -Other $n.Value.Identity) -eq 0) {
                            $NotMatched = $false;
                            $n.Value | Write-Output;
                            $ToRemove.Remove($n) | Out-Null;
                            break;
                        }
                    }
                    if ($NotMatched) {
                        Write-Information -MessageData "Adding $Identity";
                        if (-not $VsixFileInfo.FromManifest) { Read-ExtensionVsixManifest -File $VsixFileInfo }
                        if ($VsixFileInfo.FromManifest) { $VsixFileInfo | Write-Output }
                    }
                }
            }
        }
    }

    End {
        $Node = $ToRemove.First;
        if ($null -ne $Node) {
            if ($PSBoundParameters.ContainsKey('Removed')) {
                do {
                    $Removed.Add($Node.value);
                    Write-Information -MessageData "Removed $($Node.Value.Identity)";
                    $Node = $Node.Next;
                } while ($null -ne $Node);
            } else {
                do {
                    Write-Information -MessageData "Removed $($Node.Value.Identity)";
                    $Node = $Node.Next;
                } while ($null -ne $Node);
            }
        }
    }
}

Function Get-VsExtensionFromMarketPlace {
    <#
    .SYNOPSIS
        Downloads a VSIX file from the marketplace.
    .DESCRIPTION
        Gets the VSIX extension matching the given publisher, ID, version, and platform.
    #>
    [CmdletBinding()]
    [OutputType([VsixFileInfo])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # The publisher identfier string.
        [string]$Publisher,

        [Parameter(Mandatory = $true, Position = 1)]
        # The package identfier string.
        [string]$ID,

        [Parameter(Mandatory = $true, Position = 2)]
        # The version string.
        [string]$Version,

        [Parameter(Mandatory = $true)]
        # The target repository folder where the package will be saved.
        [string]$RepositoryFolder,

        # The optional target platfrom
        [string]$TargetPlatform
    )

    $UriBuilder = [System.UriBuilder]::new($MyInvocation.MyCommand.Module.PrivateData.BaseMarketPlaceUri);
    $UriBuilder.Path = "/_apis/public/gallery/publishers/$([Uri]::EscapeDataString($Publisher))/vsextensions/$([Uri]::EscapeDataString($ID))/$([Uri]::EscapeDataString($Version))/vspackage";
    $FileName = "$Publisher.$ID-$Version";
    if ($PSBoundParameters.ContainsKey('TargetPlatform')) {
        $UriBuilder.Query = "targetPlatform=$([Uri]::EscapeDataString($TargetPlatform))";
        $FileName = "$FileName@$TargetPlatform";
    }
    # $requestHeaders = [System.Collections.Generic.Dictionary[string,string]]::new();
    # $requestHeaders.Add('Accept','application/json; charset=utf-8; api-version=3.2-preview.1');
    # $requestHeaders.Add('Content-Type','application/json; charset=utf-8');
    $Response = Invoke-WebRequest -Uri $UriBuilder.Uri -Method Get<# -Headers $requestHeaders#> -UseBasicParsing;
    $p = $RepositoryFolder | Join-Path -ChildPath $FileName;
    Set-Content -LiteralPath $p -Value $Response.RawContent -AsByteStream;
    if ($p | Test-Path -PathType Leaf) {
        Read-ExtensionVsixManifest -Path $p;
    }
}

Function Find-VsExtensionInMarketPlace {
    <#
    .SYNOPSIS
        Searches for VSIX file in the marketplace.
    .DESCRIPTION
        Gets information about available extensions that matching the given publisher, ID, and platform.
    #>
    [CmdletBinding()]
    [OutputType([VsMarketPlaceQueryResult])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # The publisher identfier string.
        [string]$Publisher,

        [Parameter(Mandatory = $true, Position = 0)]
        # The package identfier string.
        [string]$ID,

        # The optional target platfrom
        [string]$TargetPlatform,

        # Maximum number of 'pages' of data returned with each request.
        [int]$MaxPage = 10000,

        # Number of results per 'page' of returned data per request.
        [int]$PageSize = 100,

        [Parameter(ParameterSetName = 'ExplicitAttributes')]
        # Include verion information.
        [switch]$IncludeVersions,

        [Parameter(ParameterSetName = 'ExplicitAttributes')]
        # Include information about files in the package.
        [switch]$IncludeFiles,

        [Parameter(ParameterSetName = 'ExplicitAttributes')]
        # Include category and tag information.
        [switch]$IncludeCategoryAndTags,

        [Parameter(ParameterSetName = 'ExplicitAttributes')]
        [switch]$IncludeSharedAccounts,

        [switch]$ExcludeNonValidated,

        [switch]$IncludeVersionProperties,

        [switch]$IncludeInstallationTargets,

        [switch]$IncludeAssetUri,

        [switch]$IncludeStatistics,

        [switch]$IncludeLatestVersionOnly,

        [switch]$Unpublished,

        [switch]$IncludeNameConflictInfo,

        [Parameter(Mandatory = $true, ParameterSetName = 'AllAttributes')]
        [switch]$AllAttributes
    )

    $Flags = 0;
    if ($AllAttributes.IsPresent) {
        $Flags = 0x1f;
    } else {
        if ($IncludeVersions.IsPresent) { $Flags = 0x1 }
        if ($IncludeFiles.IsPresent) { $Flags = $Flags -bor 0x2 }
        if ($IncludeCategoryAndTags.IsPresent) { $Flags = $Flags -bor 0x4 }
        if ($IncludeSharedAccounts.IsPresent) { $Flags = $Flags -bor 0x8 }
        if ($IncludeVersionProperties.IsPresent) { $Flags = $Flags -bor 0x10 }
    }
    if ($ExcludeNonValidated.IsPresent) { $Flags = $Flags -bor 0x20 }
    if ($IncludeInstallationTargets.IsPresent) { $Flags = $Flags -bor 0x40 }
    if ($IncludeAssetUri.IsPresent) { $Flags = $Flags -bor 0x80 }
    if ($IncludeStatistics.IsPresent) { $Flags = $Flags -bor 0x100 }
    if ($IncludeLatestVersionOnly.IsPresent) { $Flags = $Flags -bor 0x200 }
    if ($Unpublished.IsPresent) { $Flags = $Flags -bor 0x1000 }
    if ($IncludeNameConflictInfo.IsPresent) { $Flags = $Flags -bor 0x8000 }

    $criteria = @([PSCustomObject]@{
        filterType = 7;
        value = "$Publisher.$ID";
    }, [PSCustomObject]@{
        filterType = 8;
        value = "Microsoft.VisualStudio.Code";
    });
    if ($PSBoundParameters.ContainsKey('TargetPlatform')) {
        $criteria += [PSCustomObject]@{
            filterType = 23;
            value = $TargetPlatform;
        }
    }
    $requestBody = [PSCustomObject]@{
        filters = ([object[]]@([PSCustomObject]@{
            criteria = ([object[]]$criteria);
            pageNumber = 1;
            pageSize = $PageSize;
            sortBy = 0;
            sortOrder = 0;
        }));
        assetTypes = (New-Object -TypeName 'System.Object[]' -ArgumentList 0);
        flags = $Flags;
    } | ConvertTo-Json -Depth 4;
    $requestHeaders = [System.Collections.Generic.Dictionary[string,string]]::new();
    $requestHeaders.Add('Accept','application/json; charset=utf-8; api-version=3.2-preview.1');
    $requestHeaders.Add('Content-Type','application/json; charset=utf-8');
    $UriBuilder = [System.UriBuilder]::new($MyInvocation.MyCommand.Module.PrivateData.BaseMarketPlaceUri);
    $UriBuilder.Path = "/_apis/public/gallery/extensionquery";
    $Response = Invoke-WebRequest -Uri $UriBuilder.Uri -Method POST -Headers $requestHeaders -Body $requestBody -UseBasicParsing;
    if ($null -ne $Response) {
        $Response.Content | Out-File -LiteralPath ($PSScriptRoot | Join-Path -ChildPath 'Example.json');
        ($Response.Content | ConvertFrom-Json).results | ForEach-Object {
            $_.extensions | ForEach-Object {
                $Item = [VsMarketPlaceQueryResult]@{
                    PublisherName = $_.publisher.displayName;
                    DisplayName = $_.displayName;
                    PublishedDate =  = [DateTime]::Parse($_.publishedDate);
                    Description = $_.shortDescription;
                };
                if ($Item.PublishedDate.Kind -eq [DateTimeKind]::Unspecified) {
                    $Item.PublishedDate = [DateTime]::SpecifyKind($Item.PublishedDate, [DateTimeKind]::Utc);
                } else {
                    if ($Item.PublishedDate.Kind -eq [DateTimeKind]::Local) { $Item.PublishedDate = $Item.PublishedDate.ToUniversalTime() }
                }
                $Versions = @($_.versions);
                if ($PSBoundParameters.ContainsKey('TargetPlatform')) {
                    $Versions = @($Versions | Where-Object { $_.targetPlatform -ieq $TargetPlatform });
                } else {
                    $Versions = @($Versions | Where-Object { [string]::IsNullOrWhiteSpace($_.targetPlatform) });
                }
                if ($Versions.Count -eq 0) {
                    $Versions = @($_.versions);
                }

                $Item.Versions = ([VsMarketPlaceExtensionVersion[]]@($Versions | ForEach-Object {
                    $v = [VsMarketPlaceExtensionVersion]@{
                        Version = $_.version;
                        LastUpdated = [DateTime]::Parse($_.lastUpdated);
                        TargetPlatform = $_.targetPlatform;
                    };
                    if ($v.LastUpdated.Kind -eq [DateTimeKind]::Unspecified) {
                        $v.LastUpdated = [DateTime]::SpecifyKind($v.LastUpdated, [DateTimeKind]::Utc);
                    } else {
                        if ($v.LastUpdated.Kind -eq [DateTimeKind]::Local) { $v.LastUpdated = $v.LastUpdated.ToUniversalTime() }
                    }
                    $v | Write-Output;
                }));

                $Item | Write-Output;
            }
        };
    }
}
