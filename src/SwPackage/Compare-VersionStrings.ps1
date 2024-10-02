if ($null -eq $Script:VersionCoreSeparators) {
    New-Variable -Name 'VersionCoreSeparators' -Option ReadOnly -Value ([System.Collections.ObjectModel.ReadOnlyCollection[char]]::new(([char[]]('+', '-', '#', '?', '\', '/', '&', '=', ':', '|', '`', '^', '>', '<', '~'))));
    New-Variable -Name 'WhiteSpaceRegex' -Option ReadOnly -Value ([regex]::new('[\P{Z}\P{C}]'));
    New-Variable -Name 'NonWhiteSpacesRegex' -Option ReadOnly -Value ([regex]::new('[\P{Z}\P{C}]+'));
    New-Variable -Name 'SymbolRegex' -Option ReadOnly -Value ([regex]::new('\p{S}'));
    New-Variable -Name 'VersionStartRegex' -Option ReadOnly -Value ([regex]::new('\d+(\.\d+)*(-[^+]*)?(\+|$)'));
    # $SeparatorIndex -lt $Script:DotSeparatorIndex: $Script:VersionComponentSeparators[$SeparatorIndex]
    # $SeparatorIndex -eq $Script:DotSeparatorIndex: '.'
    # $SeparatorIndex -lt $Script:WsSeparatorIndex: $Script:VersionSubSeparators[$SeparatorIndex - ($Script:DotSeparatorIndex + 1)]
    # $SeparatorIndex -eq $Script:WsSeparatorIndex: WhiteSpace
    # $Script:VersionCoreSeparators.Length + 1: [char]::IsPunctuation()
    # $Script:VersionCoreSeparators.Length + 2: \s+
}

Function Compare-WsAlphaNumValues {
    <#
    .SYNOPSIS
        Compare semantic versions.
    .DESCRIPTION
        Compares two semantic version values, returning a number indicating whether one version is less than, greater than, or equal to the other.
    .OUTPUTS
        System.Int32. 0 if Version and Other are equal; greater than 1 if Version is greather than Other; otherwise, less than 1 if Version is less than Other.
    #>
    [CmdletBinding()]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value,

        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Other,

        [Parameter(Mandatory = $true)]
        [System.StringComparer]$Comparer,

        [Parameter(Mandatory = $true)]
        [bool]$IsOrdinal
    )
    
}

Function Compare-WsSeparatedAlphaNum {
    <#
    .SYNOPSIS
        Compare semantic versions.
    .DESCRIPTION
        Compares two semantic version values, returning a number indicating whether one version is less than, greater than, or equal to the other.
    .OUTPUTS
        System.Int32. 0 if Version and Other are equal; greater than 1 if Version is greather than Other; otherwise, less than 1 if Version is less than Other.
    #>
    [CmdletBinding()]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value,

        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Other,

        [switch]$WsNotNormalized,

        [Parameter(Mandatory = $true)]
        [System.StringComparer]$Comparer,

        [Parameter(Mandatory = $true)]
        [bool]$IsOrdinal
    )
    
    $Diff = 0;
    if ($WsNotNormalized.IsPresent) {
        if ($Value.Length -eq 0) {
            if ($Other.Length -eq 0) { $Diff = 0} else { $Diff = -1 }
            $Private:ValueArr = $null;
        } else {
            if ($Other.Length -eq 0) {
                $Diff = 1;
                $Private:ValueArr = $null;
            } else {
                $Private:ValueArr = $Script:WhiteSpaceRegex.Split($Value);
                $Private:OtherArr = $Script:WhiteSpaceRegex.Split($Other);
            }
        }
    } else {
        $ValueString = $Value | Optimize-WhiteSpace;
        $OtherString = $Other | Optimize-WhiteSpace;
        if ($ValueString.Length -eq 0) {
            if ($OtherString.Length -eq 0) { $Diff = 0} else { $Diff = -1 }
            $Private:ValueArr = $null;
        } else {
            if ($OtherString.Length -eq 0) {
                $Diff = 1;
                $Private:ValueArr = $null;
            } else {
                $Private:ValueArr = $ValueString.Split(' ');
                $Private:OtherArr = $OtherString.Split(' ');
            }
        }
    }

    if ($null -eq $ValueArr) {
        $Diff | Write-Output;
    } else {
        $Idx = 0;
        do {
            $Diff = Compare-WsAlphaNumValues -Value $ValueArr[$Idx] -Other $OtherArr[$Idx] -Comparer $Comparer -IsOrdinal $IsOrdinal;
        } while ($Idx -lt $ValueArr.Length -and $Idx -lt $OtherArr.Length);
    }
}

Function Compare-PunctuationDelimitedAlphaNum {
    [CmdletBinding()]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value,

        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Other,

        [Parameter(Mandatory = $true)]
        [System.StringComparer]$Comparer,

        [Parameter(Mandatory = $true)]
        [bool]$IsOrdinal,

        [switch]$WsNotNormalized
    )

    if ($WsNotNormalized.IsPresent) {
        $Private:ValueString = $Value;
        $Private:OtherString = $Other;
    } else {
        $Private:ValueString = $Value | Optimize-WhiteSpace;
        $Private:OtherString = $Other | Optimize-WhiteSpace;
    }

    if ($ValueString.Length -eq 0) {
        if ($OtherString.Length -eq 0) { 0 | Write-Output } else { -1 | Write-Output }
    } else {
        if ($OtherString.Length -eq 0) {
            1 | Write-Output;
        } else {
            [System.Text.RegularExpressions.MatchCollection]$ValueMatches = $Script:SymbolRegex.Matches($ValueString);
            [System.Text.RegularExpressions.MatchCollection]$OtherMatches = $Script:SymbolRegex.Matches($OtherString);
            if ($ValueMatches.Count -eq 0) {
                if ($OtherMatches.Count -eq 0) {
                    if ($WsNotNormalized.IsPresent) {
                        Compare-WsSeparatedAlphaNum -Value $ValueString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                    } else {
                        Compare-WsSeparatedAlphaNum -Value $ValueString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                    }
                } else {
                    if ($WsNotNormalized.IsPresent) {
                        $Private:Diff = Compare-WsSeparatedAlphaNum -Value $ValueString -Other $OtherString.Substring(0, $OtherMatches[0].Index) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                    } else {
                        $Private:Diff = Compare-WsSeparatedAlphaNum -Value $ValueString -Other $OtherString.Substring(0, $OtherMatches[0].Index) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                    }
                    if ($DIff -eq 0) { $Diff = -1 }
                }
            } else {
                if ($OtherMatches.Count -eq 0) {
                    if ($WsNotNormalized.IsPresent) {
                        Compare-WsSeparatedAlphaNum -Value $ValueString.Substring(0, $ValueMatches[0].Index) -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                    } else {
                        Compare-WsSeparatedAlphaNum -Value $ValueString.Substring(0, $ValueMatches[0].Index) -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                    }
                } else {
                    $VStart =  $OStart = $Idx = $Diff = 0;
                    if ($WsNotNormalized.IsPresent) {
                        do {
                            $VM = $ValueMatches[$Idx];
                            $OM = $OtherMatches[$Idx];
                            $VL = $VM.Index - $VStart;
                            $OL = $OM.Index - $OStart;
                            if ($VL -eq 0) {
                                if ($OL -eq 0) {
                                    $Diff = -1;
                                    break;
                                }
                            } else {
                                if ($OL -eq 0) {
                                    $Diff = 1;
                                    break;
                                }
                                $Diff = Compare-WsSeparatedAlphaNum -Value $ValueString.Substring($VStart, $VL) -Other $OtherString.Substring($OStart, $OL) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                if ($Diff -ne 0) { break }
                            }
                            $Diff = $VM.Value.CompareTo($OM.Value);
                            if ($Diff -ne 0) { break }
                            $VStart = $VM.Index + 1;
                            $OStart = $OM.Index + 1;
                        } while ($Idx -lt $ValueMatches.Count -and $Idx -lt $OtherMatches.Count);
                    } else {
                        do {
                            $VM = $ValueMatches[$Idx];
                            $OM = $OtherMatches[$Idx];
                            $VL = $VM.Index - $VStart;
                            $OL = $OM.Index - $OStart;
                            if ($VL -eq 0) {
                                if ($OL -eq 0) {
                                    $Diff = -1;
                                    break;
                                }
                            } else {
                                if ($OL -eq 0) {
                                    $Diff = 1;
                                    break;
                                }
                                $Diff = Compare-WsSeparatedAlphaNum -Value $ValueString.Substring($VStart, $VL) -Other $OtherString.Substring($OStart, $OL) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                if ($Diff -ne 0) { break }
                            }
                            $Diff = $VM.Value.CompareTo($OM.Value);
                            if ($Diff -ne 0) { break }
                            $VStart = $VM.Index + 1;
                            $OStart = $OM.Index + 1;
                        } while ($Idx -lt $ValueMatches.Count -and $Idx -lt $OtherMatches.Count);
                    }
                    if ($Diff -eq 0) {
                        $Diff = $ValueMatches.Count - $OtherMatches.Count;
                        if ($Diff -eq 0) {
                            if ($VStart -lt $ValueString.Length) {
                                if ($OStart -lt $OtherString.Length) {
                                    if ($WsNotNormalized.IsPresent) {
                                        $Diff = Compare-WsSeparatedAlphaNum -Value $ValueString.Substring($VStart) -Other $OtherString.Substring($OStart) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    } else {
                                        $Diff = Compare-WsSeparatedAlphaNum -Value $ValueString.Substring($VStart) -Other $OtherString.Substring($OStart) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                } else {
                                    $Diff = 1;
                                }
                            } else {
                                if ($OStart -lt $OtherString.Length) { $Diff = -1 }
                            }
                        }
                    }
                    if ($WsNotNormalized.IsPresent) {
                        $Private:Diff = Compare-WsSeparatedAlphaNum -Value $ValueString -Other $OtherString.Substring(0, $OtherMatches[0].Index) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                    } else {
                        $Private:Diff = Compare-WsSeparatedAlphaNum -Value $ValueString -Other $OtherString.Substring(0, $OtherMatches[0].Index) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                    }
                    if ($DIff -eq 0) { $Diff = -1 }
                }
            }
        }
    }
}

Function Compare-SymbolDelimitedAlphaNum {
    [CmdletBinding()]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value,

        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Other,

        [Parameter(Mandatory = $true)]
        [System.StringComparer]$Comparer,

        [Parameter(Mandatory = $true)]
        [bool]$IsOrdinal,

        [switch]$WsNotNormalized
    )

    if ($WsNotNormalized.IsPresent) {
        $Private:ValueString = $Value;
        $Private:OtherString = $Other;
    } else {
        $Private:ValueString = $Value | Optimize-WhiteSpace;
        $Private:OtherString = $Other | Optimize-WhiteSpace;
    }

    if ($ValueString.Length -eq 0) {
        if ($OtherString.Length -eq 0) { 0 | Write-Output } else { -1 | Write-Output }
    } else {
        if ($OtherString.Length -eq 0) {
            1 | Write-Output;
        } else {
            [System.Text.RegularExpressions.MatchCollection]$ValueMatches = $Script:SymbolRegex.Matches($ValueString);
            [System.Text.RegularExpressions.MatchCollection]$OtherMatches = $Script:SymbolRegex.Matches($ValueString);
            if ($ValueMatches.Count -eq 0) {
                if ($OtherMatches.Count -eq 0) {
                    if ($WsNotNormalized.IsPresent) {
                        Compare-PunctuationDelimitedAlphaNum -Value $ValueString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                    } else {
                        Compare-PunctuationDelimitedAlphaNum -Value $ValueString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                    }
                } else {
                    if ($WsNotNormalized.IsPresent) {
                        $Private:Diff = Compare-PunctuationDelimitedAlphaNum -Value $ValueString -Other $OtherString.Substring(0, $OtherMatches[0].Index) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                    } else {
                        $Private:Diff = Compare-PunctuationDelimitedAlphaNum -Value $ValueString -Other $OtherString.Substring(0, $OtherMatches[0].Index) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                    }
                    if ($DIff -eq 0) { $Diff = -1 }
                }
            } else {
                if ($OtherMatches.Count -eq 0) {
                    if ($WsNotNormalized.IsPresent) {
                        Compare-PunctuationDelimitedAlphaNum -Value $ValueString.Substring(0, $ValueMatches[0].Index) -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                    } else {
                        Compare-PunctuationDelimitedAlphaNum -Value $ValueString.Substring(0, $ValueMatches[0].Index) -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                    }
                } else {
                    $VStart =  $OStart = $Idx = $Diff = 0;
                    if ($WsNotNormalized.IsPresent) {
                        do {
                            $VM = $ValueMatches[$Idx];
                            $OM = $OtherMatches[$Idx];
                            $VL = $VM.Index - $VStart;
                            $OL = $OM.Index - $OStart;
                            if ($VL -eq 0) {
                                if ($OL -eq 0) {
                                    $Diff = -1;
                                    break;
                                }
                            } else {
                                if ($OL -eq 0) {
                                    $Diff = 1;
                                    break;
                                }
                                $Diff = Compare-PunctuationDelimitedAlphaNum -Value $ValueString.Substring($VStart, $VL) -Other $OtherString.Substring($OStart, $OL) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                if ($Diff -ne 0) { break }
                            }
                            $Diff = $VM.Value.CompareTo($OM.Value);
                            if ($Diff -ne 0) { break }
                            $VStart = $VM.Index + 1;
                            $OStart = $OM.Index + 1;
                        } while ($Idx -lt $ValueMatches.Count -and $Idx -lt $OtherMatches.Count);
                    } else {
                        do {
                            $VM = $ValueMatches[$Idx];
                            $OM = $OtherMatches[$Idx];
                            $VL = $VM.Index - $VStart;
                            $OL = $OM.Index - $OStart;
                            if ($VL -eq 0) {
                                if ($OL -eq 0) {
                                    $Diff = -1;
                                    break;
                                }
                            } else {
                                if ($OL -eq 0) {
                                    $Diff = 1;
                                    break;
                                }
                                $Diff = Compare-PunctuationDelimitedAlphaNum -Value $ValueString.Substring($VStart, $VL) -Other $OtherString.Substring($OStart, $OL) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                if ($Diff -ne 0) { break }
                            }
                            $Diff = $VM.Value.CompareTo($OM.Value);
                            if ($Diff -ne 0) { break }
                            $VStart = $VM.Index + 1;
                            $OStart = $OM.Index + 1;
                        } while ($Idx -lt $ValueMatches.Count -and $Idx -lt $OtherMatches.Count);
                    }
                    if ($Diff -eq 0) {
                        $Diff = $ValueMatches.Count - $OtherMatches.Count;
                        if ($Diff -eq 0) {
                            if ($VStart -lt $ValueString.Length) {
                                if ($OStart -lt $OtherString.Length) {
                                    if ($WsNotNormalized.IsPresent) {
                                        $Diff = Compare-PunctuationDelimitedAlphaNum -Value $ValueString.Substring($VStart) -Other $OtherString.Substring($OStart) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    } else {
                                        $Diff = Compare-PunctuationDelimitedAlphaNum -Value $ValueString.Substring($VStart) -Other $OtherString.Substring($OStart) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                } else {
                                    $Diff = 1;
                                }
                            } else {
                                if ($OStart -lt $OtherString.Length) { $Diff = -1 }
                            }
                        }
                    }
                    if ($WsNotNormalized.IsPresent) {
                        $Private:Diff = Compare-PunctuationDelimitedAlphaNum -Value $ValueString -Other $OtherString.Substring(0, $OtherMatches[0].Index) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                    } else {
                        $Private:Diff = Compare-PunctuationDelimitedAlphaNum -Value $ValueString -Other $OtherString.Substring(0, $OtherMatches[0].Index) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                    }
                    if ($DIff -eq 0) { $Diff = -1 }
                }
            }
        }
    }
}

Function Compare-CharSeparatedAlphaNum {
    [CmdletBinding()]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value,

        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Other,

        [Parameter(Mandatory = $true)]
        [ValidateRange(0, 13)]
        # The string comparar to use
        [int]$SeparatorIndex,

        [Parameter(Mandatory = $true)]
        [System.StringComparer]$Comparer,

        [Parameter(Mandatory = $true)]
        [bool]$IsOrdinal,

        [switch]$WsNotNormalized
    )

    if ($WsNotNormalized.IsPresent) {
        $Private:ValueString = $Value;
        $Private:OtherString = $Other;
    } else {
        $Private:ValueString = $Value | Optimize-WhiteSpace;
        $Private:OtherString = $Other | Optimize-WhiteSpace;
    }

    if ($ValueString.Length -eq 0) {
        if ($OtherString.Length -eq 0) { 0 | Write-Output } else { -1 | Write-Output }
    } else {
        if ($OtherString.Length -eq 0) {
            1 | Write-Output;
        } else {
            $Sep = $Script:VersionCoreSeparators[$SeparatorIndex];
            $ValueArr = $ValueString.Split($Sep);
            $OtherArr = $OtherString.Split($Sep);
            $NextIndex = $SeparatorIndex + 1;
            if ($NextIndex -lt $Script:VersionCoreSeparators.Count) {
                if ($WsNotNormalized) {
                    $Idx = 0;
                    $Diff = 0;
                    do {
                        $Diff = Compare-CharSeparatedAlphaNum -Value $Value -Other $Other -SeparatorIndex $NextIndex -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                        if ($Diff -ne 0) { break }
                        $Idx++;
                    } while ($Idx -lt $ValueArr.Length -and $Idx -lt $OtherArr.Length);
                    if ($Diff -eq 0) { $Diff = $ValueArr.Length = $OtherArr.Length }
                } else {
                    $Idx = 0;
                    $Diff = 0;
                    do {
                        $Diff = Compare-CharSeparatedAlphaNum -Value $Value -Other $Other -SeparatorIndex $NextIndex -Comparer $Comparer -IsOrdinal $IsOrdinal;
                        if ($Diff -ne 0) { break }
                        $Idx++;
                    } while ($Idx -lt $ValueArr.Length -and $Idx -lt $OtherArr.Length);
                    if ($Diff -eq 0) { $Diff = $ValueArr.Length = $OtherArr.Length }
                }
            } else {
                if ($WsNotNormalized) {
                    $Idx = 0;
                    $Diff = 0;
                    do {
                        $Diff = Compare-SymbolDelimitedAlphaNum -Value $Value -Other $Other -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                        if ($Diff -ne 0) { break }
                        $Idx++;
                    } while ($Idx -lt $ValueArr.Length -and $Idx -lt $OtherArr.Length);
                    if ($Diff -eq 0) { $Diff = $ValueArr.Length = $OtherArr.Length }
                } else {
                    $Idx = 0;
                    $Diff = 0;
                    do {
                        $Diff = Compare-SymbolDelimitedAlphaNum -Value $Value -Other $Other -Comparer $Comparer -IsOrdinal $IsOrdinal;
                        if ($Diff -ne 0) { break }
                        $Idx++;
                    } while ($Idx -lt $ValueArr.Length -and $Idx -lt $OtherArr.Length);
                    if ($Diff -eq 0) { $Diff = $ValueArr.Length = $OtherArr.Length }
                }
            }
        }
    }
}

Function Compare-SemanticVersionCore {
    <#
    .SYNOPSIS
        Compare semantic versions.
    .DESCRIPTION
        Compares two semantic version values, returning a number indicating whether one version is less than, greater than, or equal to the other.
    .OUTPUTS
        System.Int32. 0 if Version and Other are equal; greater than 1 if Version is greather than Other; otherwise, less than 1 if Version is less than Other.
    #>
    [CmdletBinding(DefaultParameterSetName = 'Type')]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        # The version string to be compared.
        [string]$Version,

        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        # The version string to compare to.
        [string]$Other,

        # Don't assume that corresponding version number elements are zero when one version string has more version number elements than the other.
        [switch]$DontAssumeZeroElements,

        # Do not normalize whitespace
        [switch]$WsNotNormalized,

        [Parameter(Mandatory = $true)]
        # The string comparar to use
        [System.StringComparer]$Comparer,

        [Parameter(Mandatory = $true)]
        [bool]$IsOrdinal
    )
    
}

Function Compare-SemanticVersion {
    <#
    .SYNOPSIS
        Compare semantic versions.
    .DESCRIPTION
        Compares two semantic version values, returning a number indicating whether one version is less than, greater than, or equal to the other.
    .OUTPUTS
        System.Int32. 0 if Version and Other are equal; greater than 1 if Version is greather than Other; otherwise, less than 1 if Version is less than Other.
    #>
    [CmdletBinding(DefaultParameterSetName = 'Type')]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        # The version string to be compared.
        [string]$Version,

        [Parameter(Mandatory = $true, Position = 1)]
        [AllowEmptyString()]
        # The version string to compare to.
        [string]$Other,

        # Don't assume that corresponding version number elements are zero when one version string has more version number elements than the other.
        [switch]$DontAssumeZeroElements,

        # Do not normalize whitespace
        [switch]$WsNotNormalized,

        [Parameter(Mandatory = $true, ParameterSetName = 'Type')]
        # The version string comparison type. The default is OrdinalIgnoreCase.
        [System.StringComparison]$ComparisonType = [System.StringComparison]::OrdinalIgnoreCase
    )
    
    Begin {
        $Comparer = $ComparisonType | Get-StringComparer;
        $IsOrdinal = ($ComparisonType -eq [System.StringComparison]::OrdinalIgnoreCase -or $ComparisonType -eq [System.StringComparison]::Ordinal);
    }

}

Function Compare-IdentifierNameString {
    [CmdletBinding()]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value,

        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        # The version string to compare to.
        [string]$Other,

        # Do not normalize whitespace
        [switch]$WsNotNormalized,

        [Parameter(Mandatory = $true)]
        # The string comparar to use
        [System.StringComparer]$Comparer,

        [Parameter(Mandatory = $true)]
        [bool]$IsOrdinal
    )

    if ($WsNotNormalized.IsPresent) {
        $Private:ValueString = $Value;
        $Private:OtherString = $Other;
    } else {
        $Private:ValueString = $Value | Optimize-WhiteSpace;
        $Private:OtherString = $Other | Optimize-WhiteSpace;
    }

    if ($ValueString.Length -eq 0) {
        if ($OtherString.Length -eq 0) { 0 | Write-Output } else { -1 | Write-Output }
    } else {
        if ($OtherString.Length -eq 0) {
            1 | Write-Output;
        } else {
            $ValueArr = $ValueString.Split('-');
            $OtherArr = $OtherString.Split('-');
            if ($WsNotNormalized) {
                $Idx = 0;
                $Diff = 0;
                do {
                    $Diff = Compare-CharSeparatedAlphaNum -Value $Value -Other $Other -SeparatorIndex 2 -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                    if ($Diff -ne 0) { break }
                    $Idx++;
                } while ($Idx -lt $ValueArr.Length -and $Idx -lt $OtherArr.Length);
                if ($Diff -eq 0) { $Diff = $ValueArr.Length = $OtherArr.Length }
            } else {
                $Idx = 0;
                $Diff = 0;
                do {
                    $Diff = Compare-CharSeparatedAlphaNum -Value $Value -Other $Other -SeparatorIndex 2 -Comparer $Comparer -IsOrdinal $IsOrdinal;
                    if ($Diff -ne 0) { break }
                    $Idx++;
                } while ($Idx -lt $ValueArr.Length -and $Idx -lt $OtherArr.Length);
                if ($Diff -eq 0) { $Diff = $ValueArr.Length = $OtherArr.Length }
            }
        }
    }
}

Function Compare-IdentityString {
    [CmdletBinding()]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value,

        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Other,

        [switch]$WsNotNormalized,

        [Parameter(Mandatory = $true)]
        [System.StringComparer]$Comparer,

        [Parameter(Mandatory = $true)]
        [bool]$IsOrdinal
    )

    if ($WsNotNormalized.IsPresent) {
        $Private:ValueString = $Value;
        $Private:OtherString = $Other;
    } else {
        $Private:ValueString = $Value | Optimize-WhiteSpace;
        $Private:OtherString = $Other | Optimize-WhiteSpace;
    }

    if ($ValueString.Length -eq 0) {
        if ($OtherString.Length -eq 0) { 0 | Write-Output } else { -1 | Write-Output }
    } else {
        if ($OtherString.Length -eq 0) {
            1 | Write-Output;
        } else {
            $ValueArr = $ValueString.Split('.');
            $OtherArr = $OtherString.Split('.');
            if ($WsNotNormalized) {
                $Idx = 0;
                $Diff = 0;
                do {
                    $Diff = Compare-IdentifierNameString -Value $ValueArr[$Idx] -Other $OtherArr[$Idx] -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                    if ($Diff -ne 0) { break }
                    $Idx++;
                } while ($Idx -lt $ValueArr.Length -and $Idx -lt $OtherArr.Length);
                if ($Diff -eq 0) { $Diff = $ValueArr.Length = $OtherArr.Length }
            } else {
                $Idx = 0;
                $Diff = 0;
                do {
                    $Diff = Compare-IdentifierNameString -Value $ValueArr[$Idx] -Other $OtherArr[$Idx] -Comparer $Comparer -IsOrdinal $IsOrdinal;
                    if ($Diff -ne 0) { break }
                    $Idx++;
                } while ($Idx -lt $ValueArr.Length -and $Idx -lt $OtherArr.Length);
                if ($Diff -eq 0) { $Diff = $ValueArr.Length = $OtherArr.Length }
            }
        }
    }
}

Function Compare-PackageVersion {
    <#
    .SYNOPSIS
        Compare version strings.
    .DESCRIPTION
        Compares two version strings where the assumed format is Publisher.ID@SemanticVersion or ID@SemanticVersion.
    .OUTPUTS
        System.Int32. 0 if Version and Other are equal; greater than 1 if Version is greather than Other; otherwise, less than 1 if Version is less than Other.
    #>
    [CmdletBinding(DefaultParameterSetName = 'Type')]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        # The version string to be compared.
        [string]$Version,

        [Parameter(Mandatory = $true, Position = 1)]
        [AllowEmptyString()]
        # The version string to compare to.
        [string]$Other,

        # Don't assume that corresponding version number elements are zero when one version string has more version number elements than the other.
        [switch]$DontAssumeZeroElements,

        # Do not normalize whitespace
        [switch]$WsNotNormalized,

        # The version string comparison type. The default is OrdinalIgnoreCase.
        [System.StringComparison]$ComparisonType = [System.StringComparison]::OrdinalIgnoreCase
    )

    Begin {
        $Comparer = $ComparisonType | Get-StringComparer;
        $IsOrdinal = ($ComparisonType -eq [System.StringComparison]::OrdinalIgnoreCase -or $ComparisonType -eq [System.StringComparison]::Ordinal);
    }

    Process {
        if ($WsNotNormalized.IsPresent) {
            $Private:VersionString = $Version;
            $Private:OtherString = $Other;
        } else {
            $Private:VersionString = $Version | Optimize-WhiteSpace;
            $Private:OtherString = $Other | Optimize-WhiteSpace;
        }

        if ($VersionString.Length -eq 0) {
            if ($OtherString.Length -eq 0) { 0 | Write-Output } else { -1 | Write-Output }
        } else {
            if ($OtherString.Length -eq 0) {
                1 | Write-Output;
            } else {
                $VersionIndex = $VersionString.IndexOf('@');
                $OtherIndex = $OtherString.IndexOf('@');
                if ($VersionIndex -lt 0) {
                    if ($OtherIndex -lt 0) {
                        $VersionIndex = $VersionString.IndexOf('.');
                        $OtherIndex = $OtherString.IndexOf('.');
                        if ($VersionIndex -lt 0) {
                            if ($OtherIndex -lt 0) {
                                if ($Script:VersionStartRegex.IsMatch($VersionString)) {
                                    if ($Script:VersionStartRegex.IsMatch($OtherString)) {
                                        if ($WsNotNormalized.IsPresent) {
                                            if ($DontAssumeZeroElements.IsPresent) {
                                                $Diff = Compare-SemanticVersionCore -Version $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized -DontAssumeZeroElements;
                                            } else {
                                                $Diff = Compare-SemanticVersionCore -Version $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                            }
                                        } else {
                                            if ($DontAssumeZeroElements.IsPresent) {
                                                $Diff = Compare-SemanticVersionCore -Version $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -DontAssumeZeroElements;
                                            } else {
                                                $Diff = Compare-SemanticVersionCore -Version $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                            }
                                        }
                                    } else {
                                        if ($DontAssumeZeroElements.IsPresent) { $Diff = -1 } else { $Diff = 1 }
                                    }
                                } else {
                                    if ($Script:VersionStartRegex.IsMatch($OtherString)) {
                                        if ($DontAssumeZeroElements.IsPresent) { $Diff = 1 } else { $Diff = -1 }
                                    } else {
                                        if ($WsNotNormalized.IsPresent) {
                                            $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                        } else {
                                            $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                        }
                                    }
                                }
                            } else {
                                if ($Script:VersionStartRegex.IsMatch($VersionString)) {
                                    if ($DontAssumeZeroElements.IsPresent) { $Diff = -1 } else { $Diff = 1 }
                                } else {
                                    if ($WsNotNormalized.IsPresent) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    } else {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                }
                            }
                        } else {
                            if ($OtherIndex -lt 0) {
                                if ($Script:VersionStartRegex.IsMatch($OtherString)) {
                                    if ($DontAssumeZeroElements.IsPresent) { $Diff = 1 } else { $Diff = -1 }
                                } else {
                                    if ($WsNotNormalized.IsPresent) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    } else {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                }
                            } else {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    if ($Diff -eq 0) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    }
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    if ($Diff -eq 0) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                }
                            }
                        }
                    } else {
                        $VersionIndex = $VersionString.IndexOf('.');
                        $OtherString = $OtherString.Substring(0, $OtherIndex + 1);
                        $OtherIndex = $OtherString.IndexOf('.');
                        if ($VersionIndex -lt 0) {
                            if ($Script:VersionStartRegex.IsMatch($VersionString)) {
                                if ($DontAssumeZeroElements.IsPresent) { $Diff = -1 } else { $Diff = 1 }
                            } else {
                                if ($OtherIndex -lt 0) {
                                    if ($WsNotNormalized.IsPresent) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    } else {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                } else {
                                    if ($WsNotNormalized.IsPresent) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    } else {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                }
                            }
                        } else {
                            if ($OtherIndex -lt 0) {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                }
                            } else {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    if ($Diff -eq 0) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    }
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    if ($Diff -eq 0) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                }
                            }
                        }
                    }
                } else {
                    if ($OtherIndex -lt 0) {
                        $VersionString = $VersionString.Substring(0, $VersionIndex);
                        $VersionIndex = $VersionString.IndexOf('.');
                        $OtherIndex = $OtherString.IndexOf('.');
                        if ($VersionIndex -lt 0) {
                            if ($OtherIndex -lt 0) {
                                if ($Script:VersionStartRegex.IsMatch($OtherString)) {
                                    if ($DontAssumeZeroElements.IsPresent) { $Diff = 1 } else { $Diff = -1 }
                                } else {
                                    if ($WsNotNormalized.IsPresent) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    } else {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                }
                            } else {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                }
                            }
                        } else {
                            if ($OtherIndex -lt 0) {
                                if ($Script:VersionStartRegex.IsMatch($OtherString)) {
                                    if ($DontAssumeZeroElements.IsPresent) { $Diff = 1 } else { $Diff = -1 }
                                } else {
                                    if ($WsNotNormalized.IsPresent) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    } else {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                }
                            } else {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    if ($Diff -eq 0) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    }
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring(0, $VersionIndex) -Other $OtherString.Substring(0, $OtherIndex) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    if ($Diff -eq 0) {
                                        $Diff = Compare-PackageIdentifierName -Value $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    }
                                }
                            }
                        }
                    } else {
                        $VersionID = $VersionString.Substring(0, $VersionIndex);
                        $OtherID = $OtherString.Substring(0, $OtherIndex);
                        $VIdx = $VersionID.IndexOf('.');
                        $OIdx = $OtherID.IndexOf('.');
                        if ($VIdx -lt 0) {
                            if ($OIdx -lt 0) {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionID -Other $OtherID -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    if ($Diff -eq 0) {
                                        if ($DontAssumeZeroElements.IsPresent) {
                                            $Diff = Compare-SemanticVersionCore -Version $VersionString.Substring($VersionIndex + 1)  -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized -DontAssumeZeroElements;
                                        } else {
                                            $Diff = Compare-SemanticVersionCore -Version $VersionString.Substring($VersionIndex + 1)  -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                        }
                                    }
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionID -Other $OtherID -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    if ($Diff -eq 0) {
                                        if ($DontAssumeZeroElements.IsPresent) {
                                            $Diff = Compare-SemanticVersionCore -Version $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -DontAssumeZeroElements;
                                        } else {
                                            $Diff = Compare-SemanticVersionCore -Version $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                        }
                                    }
                                }
                            } else {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionID -Other $OtherID.Substring(0, $OIdx) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionID -Other $OtherID.Substring(0, $OIdx) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                }
                                if ($Diff -eq 0) { $Diff = -1 }
                            }
                        } else {
                            if ($OIdx -lt 0) {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionID.Substring(0, $VIdx) -Other $OtherID -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionID.Substring(0, $VIdx) -Other $OtherID -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                }
                                if ($Diff -eq 0) { $Diff = 1 }
                            } else {
                                if ($WsNotNormalized.IsPresent) {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionID.Substring(0, $VIdx) -Other $OtherID.Substring(0, $OIdx) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                    if ($Diff -eq 0) {
                                        if ($DontAssumeZeroElements.IsPresent) {
                                            $Diff = Compare-SemanticVersionCore -Version $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized -DontAssumeZeroElements;
                                        } else {
                                            $Diff = Compare-SemanticVersionCore -Version $VersVersionString.Substring($VersionIndex + 1)ionString -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -WsNotNormalized;
                                        }
                                    }
                                } else {
                                    $Diff = Compare-PackageIdentifierName -Value $VersionID.Substring(0, $VIdx) -Other $OtherID.Substring(0, $OIdx) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                    if ($Diff -eq 0) {
                                        if ($DontAssumeZeroElements.IsPresent) {
                                            $Diff = Compare-SemanticVersionCore -Version $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal -DontAssumeZeroElements;
                                        } else {
                                            $Diff = Compare-SemanticVersionCore -Version $VersionString.Substring($VersionIndex + 1) -Other $OtherString.Substring($OtherIndex + 1) -Comparer $Comparer -IsOrdinal $IsOrdinal;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

Function Compare-PackageIdentity {
    <#
    .SYNOPSIS
        Compare version strings.
    .DESCRIPTION
        Compares two version strings where the assumed format is Publisher.ID-SemanticVersion@TargetPlatform, ID-SemanticVersion@TargetPlatform, Publisher.ID-SemanticVersion, or ID-SemanticVersion.
    .OUTPUTS
        System.Int32. 0 if Version and Other are equal; greater than 1 if Version is greather than Other; otherwise, less than 1 if Version is less than Other.
    #>
    [CmdletBinding()]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [AllowEmptyString()]
        # The version string to be compared.
        [string]$Version,

        [Parameter(Mandatory = $true, Position = 1)]
        [AllowEmptyString()]
        # The version string to compare to.
        [string]$Other,

        # Don't assume that corresponding version number elements are zero when one version string has more version number elements than the other.
        [switch]$DontAssumeZeroElements,

        # Do not normalize whitespace
        [switch]$WsNotNormalized,

        # The version string comparison type. The default is OrdinalIgnoreCase.
        [System.StringComparison]$ComparisonType = [System.StringComparison]::OrdinalIgnoreCase
    )

    Begin {
        $Comparer = $ComparisonType | Get-StringComparer;
        $IsOrdinal = ($ComparisonType -eq [System.StringComparison]::OrdinalIgnoreCase -or $ComparisonType -eq [System.StringComparison]::Ordinal);
    }

    Process {
        # Pub-Pub-Pub
        # Pub--Pub
        # Pub-Pub
        # Pub
        # ID-ID-ID
        # ID--ID
        # ID-ID
        # ID
        # Mjr.Mnr.Rev
        # Mjr.Mnr
        # Mjr
        
        # Pub.ID-Ver-Pre+Bld@Pfm
        # Pub.ID-Ver-Pre+Bld
        # Pub.ID-Ver-Pre@Pfm
        # Pub.ID-Ver+Bld@Pfm
        # ID-Ver-Pre+Bld@Pfm
        # Pub.ID-Ver-Pre
        # Pub.ID-Ver+Bld
        # Pub.ID-Ver@Pfm
        # Pub.ID+Bld@Pfm
        # ID-Ver-Pre+Bld
        # ID-Ver-Pre@Pfm
        # ID-Ver+Bld@Pfm
        # Pub.ID-Ver
        # Pub.ID+Bld
        # Pub.ID@Pfm
        # ID-Ver-Pre
        # ID-Ver+Bld
        # ID-Ver@Pfm
        # ID+Bld@Pfm
        # Pub.ID
        # ID-Ver
        # ID+Bld
        # ID@Pfm
        # ID
    }

}

Function Test-MyFunc {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [int]$Value,

        [Parameter(Mandatory = $true)]
        [int]$Other,
        
        [switch]$Alt
    )

    Begin {
        if ($Alt.IsPresent) {
            $Private:OtherValue = $Other - 1;
        } else {
            $Private:OtherValue = $Other;
        }
    }

    Process {
        "OtherValue: $($OtherValue | ConvertTo-Json)";
    }
}

Test-MyFunc -Value 1 -Other 0 -Alt;