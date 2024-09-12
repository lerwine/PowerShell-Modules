if ($null -eq $Script:VersionPathSeparatorChars) {
    Set-Variable -Name 'VersionPathSeparatorChars' -Scope 'Script' -Option ReadOnly -Value ([char[]]@('/', '\'));
    Set-Variable -Name 'SemverComponentSeparatorChars' -Scope 'Script' -Option ReadOnly -Value ([char[]]@('-', '+'));
}

class VersionStringElement {
    [Nullable[char]]$Separator;
    [string]$Value;
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

Function Compare-VersionCore {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$LVersion,

        [Parameter(Mandatory = $true)]
        [string]$RVersion,

        [Parameter(Mandatory = $true)]
        [System.StringComparer]$Comparer
    )

    $LNsElements = $LVersion.Split(':');
    $RNsElements = $RVersion.Split(':');
    $NsEnd = $LNsElements.Length;
    $NDiff = $NsEnd - $RNsElements.Length;
    if ($NDiff -gt $0) { $NsEnd = $RNsElements.Length }
    for ($NsIndex = 0; $NsIndex -lt $NsEnd; $NsIndex++) {
        $LNs = $LNsElements[$NsIndex].Trim();
        $RNs = $RNsElements[$NsIndex].Trim();
        if ($LNs.Length -eq 0) {
            if ($RNs.Length -gt 0) { return -1 }
        } else {
            if ($RNs.Length -eq 0) { return 1 }
            $LPElements = $LNs.Split($Script:VersionPathSeparatorChars);
            $RPElements = $RNs.Split($Script:VersionPathSeparatorChars);
            $PEnd = $LPElements.Length;
            $PDiff = $PEnd - $RPElements.Length;
            if ($PEnd -gt $0) { $NsEnd = $RPElements.Length }
            for ($PIndex = 0; $PIndex -lt $PEnd; $PIndex++) {
                $LPs = $LPElements[$PIndex].Trim();
                $RPs = $RPElements[$PIndex].Trim();
                if ($LPs.Length -eq 0) {
                    if ($RPs.Length -gt 0) { return -1 }
                } else {
                    if ($RPs.Length -eq 0) { return 1 }
                    $LSegElements = $LPs.Split('.');
                    $RSegElements = $RPs.Split('.');
                    $SegEnd = $LSegElements.Length;
                    if ($SegEnd -gt $RSegElements.Length) { $NsEnd = $RSegElements.Length }
                    for ($SegIndex = 0; $SegIndex -lt $SegEnd; $SegIndex++) {
                        $LWElements = $LSegElements[$SegIndex].Trim().Split(' ');
                        $RWElements = $RSegElements[$SegIndex].Trim().Split(' ');
                        $WEnd = $LWElements.Length;
                        $DfltDiff = $WEnd - $RWElements.Length;
                        if ($DfltDiff -gt 0) { $WEnd = $RWElements.Length }
                        for ($i = 0; $i -lt $WEnd; $i++) {
                            $L = $LWElements[$i];
                            $R = $RWElements[$i];
                            if ($L.Length -eq 0) {
                                if ($R.Length -gt 0) { return -1 }
                            } else {
                                if ($R.Length -eq 0) { return 1 }
                            }
                            $L = $L | Remove-ZeroPadding;
                            $R = $R | Remove-ZeroPadding;
                            if ($L[0] -eq '0') {
                                if ($R[0] -ne '0') { return -1 }
                                if ($L.Length -eq 1) {
                                    if ($R.Length -ne 1) { return -1 }
                                } else {
                                    if ($R.Length -eq 1) { return 1 }
                                    $Diff = $Comparer.Compare($L, $R);
                                    if ($Diff -ne 0) { return $Diff }
                                }
                            } else {
                                if ($R[0] -eq '0') { return 1 }
                                if ([char]::IsAsciiDigit($L[0])) {
                                    if (-not [char]::IsAsciiDigit($R[0])) { return -1 }
                                    $i = 1;
                                    while ($i -lt $L.Length) {
                                        if ($i -eq $R.Length) {
                                            if (-not [char]::IsAsciiDigit($L[$i])) {
                                                $Diff = $Comparer.Compare($L.Substring(0, $i), $R);
                                                if ($Diff -ne 0) { return $Diff }
                                            }
                                            return 1;
                                        }
                                        if ([char]::IsAsciiDigit($L[$i])) {
                                            if (-not [char]::IsAsciiDigit($R[$i])) { return 1 }
                                        } else {
                                            if ([char]::IsAsciiDigit($R[$i])) { return -1 }
                                            break;
                                        }
                                        $i++;
                                    }

                                    if ($i -eq $l.Length -and $i -lt $R.Length) {
                                        if ([char]::IsAsciiDigit($R[$i])) { return -1 }
                                        $Diff = $Comparer.Compare($L, $R.Substring(0, $i));
                                        if ($Diff -ne 0) { return $Diff }
                                        return -1;
                                    }
                                    $Diff = $Comparer.Compare($L, $R);
                                    if ($Diff -ne 0) { return $Diff }
                                } else {
                                    if ([char]::IsAsciiDigit($R[0])) { return 1 }
                                    $Diff = $Comparer.Compare($L, $R);
                                    if ($Diff -ne 0) { return $Diff }
                                }
                            }
                        }
                        if ($DfltDiff -ne 0) { return $DfltDiff }
                    }
                    if ($SegEnd -lt $LSegElements.Length) {
                        for ($i = $SegEnd; $i -lt $LSegElements.Length; $i++) {
                            $LWElements = $LSegElements[$i].Trim().Split(' ', 2);
                            if ($LWElements.Length -gt 1 -or ($LWElements[0].Length -ne 0 -and ($LWElements[0] | Remove-ZeroPadding) -ne '0')) { return 1 }
                        }
                    } else {
                        for ($i = $SegEnd; $i -lt $RSegElements.Length; $i++) {
                            $RWElements = $RSegElements[$i].Trim().Split(' ', 2);
                            if ($RWElements.Length -gt 1 -or ($RWElements[0].Length -ne 0 -and ($RWElements[0] | Remove-ZeroPadding) -ne '0')) { return 1 }
                        }
                    }
                }
            }
            if ($PDiff -ne 0) { return $PDiff }
        }
    }
    return $NDiff;
}

Function Compare-VersionCoreExact {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$LVersion,

        [Parameter(Mandatory = $true)]
        [string]$RVersion,

        [Parameter(Mandatory = $true)]
        [System.StringComparer]$Comparer
    )

    $LNsElements = $LVersion.Split(':');
    $RNsElements = $RVersion.Split(':');
    $NsEnd = $LNsElements.Length;
    $NDiff = $NsEnd - $RNsElements.Length;
    if ($NDiff -gt $0) { $NsEnd = $RNsElements.Length }
    for ($NsIndex = 0; $NsIndex -lt $NsEnd; $NsIndex++) {
        $LNs = $LNsElements[$NsIndex].Trim();
        $RNs = $RNsElements[$NsIndex].Trim();
        if ($LNs.Length -eq 0) {
            if ($RNs.Length -gt 0) { return -1 }
        } else {
            if ($RNs.Length -eq 0) { return 1 }
            $LPElements = $LNs.Split($Script:VersionPathSeparatorChars);
            $RPElements = $RNs.Split($Script:VersionPathSeparatorChars);
            $PEnd = $LPElements.Length;
            $PDiff = $PEnd - $RPElements.Length;
            if ($PEnd -gt $0) { $NsEnd = $RPElements.Length }
            for ($PIndex = 0; $PIndex -lt $PEnd; $PIndex++) {
                $LPs = $LPElements[$PIndex].Trim();
                $RPs = $RPElements[$PIndex].Trim();
                if ($LPs.Length -eq 0) {
                    if ($RPs.Length -gt 0) { return -1 }
                } else {
                    if ($RPs.Length -eq 0) { return 1 }
                    $LSegElements = $LPs.Split('.');
                    $RSegElements = $RPs.Split('.');
                    $SegEnd = $LSegElements.Length;
                    $SegDiff = $SegEnd - $RSegElements.Length;
                    if ($SegDiff -gt 0) { $NsEnd = $RSegElements.Length }
                    for ($SegIndex = 0; $SegIndex -lt $SegEnd; $SegIndex++) {
                        $LWElements = $LSegElements[$SegIndex].Trim().Split(' ');
                        $RWElements = $RSegElements[$SegIndex].Trim().Split(' ');
                        $WEnd = $LWElements.Length;
                        $DfltDiff = $WEnd - $RWElements.Length;
                        if ($DfltDiff -gt 0) { $WEnd = $RWElements.Length }
                        for ($i = 0; $i -lt $WEnd; $i++) {
                            $L = $LWElements[$i];
                            $R = $RWElements[$i];
                            if ($L.Length -eq 0) {
                                if ($R.Length -gt 0) { return -1 }
                            } else {
                                if ($R.Length -eq 0) { return 1 }
                            }
                            $L = $L | Remove-ZeroPadding;
                            $R = $R | Remove-ZeroPadding;
                            if ($L[0] -eq '0') {
                                if ($R[0] -ne '0') { return -1 }
                                if ($L.Length -eq 1) {
                                    if ($R.Length -ne 1) { return -1 }
                                } else {
                                    if ($R.Length -eq 1) { return 1 }
                                    $Diff = $Comparer.Compare($L, $R);
                                    if ($Diff -ne 0) { return $Diff }
                                }
                            } else {
                                if ($R[0] -eq '0') { return 1 }
                                if ([char]::IsAsciiDigit($L[0])) {
                                    if (-not [char]::IsAsciiDigit($R[0])) { return -1 }
                                    $i = 1;
                                    while ($i -lt $L.Length) {
                                        if ($i -eq $R.Length) {
                                            if (-not [char]::IsAsciiDigit($L[$i])) {
                                                $Diff = $Comparer.Compare($L.Substring(0, $i), $R);
                                                if ($Diff -ne 0) { return $Diff }
                                            }
                                            return 1;
                                        }
                                        if ([char]::IsAsciiDigit($L[$i])) {
                                            if (-not [char]::IsAsciiDigit($R[$i])) { return 1 }
                                        } else {
                                            if ([char]::IsAsciiDigit($R[$i])) { return -1 }
                                            break;
                                        }
                                        $i++;
                                    }

                                    if ($i -eq $l.Length -and $i -lt $R.Length) {
                                        if ([char]::IsAsciiDigit($R[$i])) { return -1 }
                                        $Diff = $Comparer.Compare($L, $R.Substring(0, $i));
                                        if ($Diff -ne 0) { return $Diff }
                                        return -1;
                                    }
                                    $Diff = $Comparer.Compare($L, $R);
                                    if ($Diff -ne 0) { return $Diff }
                                } else {
                                    if ([char]::IsAsciiDigit($R[0])) { return 1 }
                                    $Diff = $Comparer.Compare($L, $R);
                                    if ($Diff -ne 0) { return $Diff }
                                }
                            }
                        }
                        if ($DfltDiff -ne 0) { return $DfltDiff }
                    }
                    if ($SegDiff -ne 0) { return $SegDiff }
                }
            }
            if ($PDiff -ne 0) { return $PDiff }
        }
    }
    return $NDiff;
}

Function Compare-NonOrdinalVersionCore {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$LVersion,

        [Parameter(Mandatory = $true)]
        [string]$RVersion,

        [Parameter(Mandatory = $true)]
        [System.StringComparer]$Comparer
    )

    $LNsElements = $LVersion.Split(':');
    $RNsElements = $RVersion.Split(':');
    $NsEnd = $LNsElements.Length;
    $NDiff = $NsEnd - $RNsElements.Length;
    if ($NDiff -gt $0) { $NsEnd = $RNsElements.Length }
    for ($NsIndex = 0; $NsIndex -lt $NsEnd; $NsIndex++) {
        $LNs = $LNsElements[$NsIndex].Trim();
        $RNs = $RNsElements[$NsIndex].Trim();
        if ($LNs.Length -eq 0) {
            if ($RNs.Length -gt 0) { return -1 }
        } else {
            if ($RNs.Length -eq 0) { return 1 }
            $LPElements = $LNs.Split($Script:VersionPathSeparatorChars);
            $RPElements = $RNs.Split($Script:VersionPathSeparatorChars);
            $PEnd = $LPElements.Length;
            $PDiff = $PEnd - $RPElements.Length;
            if ($PEnd -gt $0) { $NsEnd = $RPElements.Length }
            for ($PIndex = 0; $PIndex -lt $PEnd; $PIndex++) {
                $LPs = $LPElements[$PIndex].Trim();
                $RPs = $RPElements[$PIndex].Trim();
                if ($LPs.Length -eq 0) {
                    if ($RPs.Length -gt 0) { return -1 }
                } else {
                    if ($RPs.Length -eq 0) { return 1 }
                    $LSegElements = $LPs.Split('.');
                    $RSegElements = $RPs.Split('.');
                    $SegEnd = $LSegElements.Length;
                    if ($SegEnd -gt $RSegElements.Length) { $NsEnd = $RSegElements.Length }
                    for ($SegIndex = 0; $SegIndex -lt $SegEnd; $SegIndex++) {
                        $LWElements = $LSegElements[$SegIndex].Trim().Split(' ');
                        $RWElements = $RSegElements[$SegIndex].Trim().Split(' ');
                        $WEnd = $LWElements.Length;
                        $DfltDiff = $WEnd - $RWElements.Length;
                        if ($DfltDiff -gt 0) { $WEnd = $RWElements.Length }
                        for ($i = 0; $i -lt $WEnd; $i++) {
                            $L = $LWElements[$i];
                            $R = $RWElements[$i];
                            if ($L.Length -eq 0) {
                                if ($R.Length -gt 0) { return -1 }
                            } else {
                                if ($R.Length -eq 0) { return 1 }
                            }
                            $Diff = $Comparer.Compare($L, $R);
                            if ($Diff -ne 0) { return $Diff }
                        }
                        if ($DfltDiff -ne 0) { return $DfltDiff }
                    }
                    if ($SegEnd -lt $LSegElements.Length) {
                        for ($i = $SegEnd; $i -lt $LSegElements.Length; $i++) {
                            $LWElements = $LSegElements[$i].Trim().Split(' ', 2);
                            if ($LWElements.Length -gt 1 -or ($LWElements[0].Length -ne 0 -and $LWElements[0] -ne '0')) { return 1 }
                        }
                    } else {
                        for ($i = $SegEnd; $i -lt $RSegElements.Length; $i++) {
                            $RWElements = $RSegElements[$i].Trim().Split(' ', 2);
                            if ($RWElements.Length -gt 1 -or ($RWElements[0].Length -ne 0 -and $RWElements[0] -ne '0')) { return 1 }
                        }
                    }
                }
            }
            if ($PDiff -ne 0) { return $PDiff }
        }
    }
    return $NDiff;
}

Function Compare-NonOrdinalVersionCoreExact {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$LVersion,

        [Parameter(Mandatory = $true)]
        [string]$RVersion,

        [Parameter(Mandatory = $true)]
        [System.StringComparer]$Comparer
    )

    $LNsElements = $LVersion.Split(':');
    $RNsElements = $RVersion.Split(':');
    $NsEnd = $LNsElements.Length;
    $NDiff = $NsEnd - $RNsElements.Length;
    if ($NDiff -gt $0) { $NsEnd = $RNsElements.Length }
    for ($NsIndex = 0; $NsIndex -lt $NsEnd; $NsIndex++) {
        $LNs = $LNsElements[$NsIndex].Trim();
        $RNs = $RNsElements[$NsIndex].Trim();
        if ($LNs.Length -eq 0) {
            if ($RNs.Length -gt 0) { return -1 }
        } else {
            if ($RNs.Length -eq 0) { return 1 }
            $LPElements = $LNs.Split($Script:VersionPathSeparatorChars);
            $RPElements = $RNs.Split($Script:VersionPathSeparatorChars);
            $PEnd = $LPElements.Length;
            $PDiff = $PEnd - $RPElements.Length;
            if ($PEnd -gt $0) { $NsEnd = $RPElements.Length }
            for ($PIndex = 0; $PIndex -lt $PEnd; $PIndex++) {
                $LPs = $LPElements[$PIndex].Trim();
                $RPs = $RPElements[$PIndex].Trim();
                if ($LPs.Length -eq 0) {
                    if ($RPs.Length -gt 0) { return -1 }
                } else {
                    if ($RPs.Length -eq 0) { return 1 }
                    $LSegElements = $LPs.Split('.');
                    $RSegElements = $RPs.Split('.');
                    $SegEnd = $LSegElements.Length;
                    $SegDiff = $SegEnd - $RSegElements.Length;
                    if ($SegDiff -gt 0) { $NsEnd = $RSegElements.Length }
                    for ($SegIndex = 0; $SegIndex -lt $SegEnd; $SegIndex++) {
                        $LWElements = $LSegElements[$SegIndex].Trim().Split(' ');
                        $RWElements = $RSegElements[$SegIndex].Trim().Split(' ');
                        $WEnd = $LWElements.Length;
                        $DfltDiff = $WEnd - $RWElements.Length;
                        if ($DfltDiff -gt 0) { $WEnd = $RWElements.Length }
                        for ($i = 0; $i -lt $WEnd; $i++) {
                            $L = $LWElements[$i];
                            $R = $RWElements[$i];
                            if ($L.Length -eq 0) {
                                if ($R.Length -gt 0) { return -1 }
                            } else {
                                if ($R.Length -eq 0) { return 1 }
                            }
                            $Diff = $Comparer.Compare($L, $R);
                            if ($Diff -ne 0) { return $Diff }
                        }
                        if ($DfltDiff -ne 0) { return $DfltDiff }
                    }
                    if ($SegDiff -ne 0) { return $SegDiff }
                }
            }
            if ($PDiff -ne 0) { return $PDiff }
        }
    }
    return $NDiff;
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

Function Compare-GenericVersionStrings {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$LVersion,

        [Parameter(Mandatory = $true)]
        [string]$RVersion,

        [Parameter(Mandatory = $true)]
        [System.StringComparer]$Comparer
    )

    $LIndex = $LVersion.IndexOf('@');
    $RIndex = $RVersion.IndexOf('@');
    if ($LIndex -lt 0) {
        if ($RIndex -lt 0) {
            return (Compare-SemverVersionStrings -LVersion $LVersion -RVersion $RVersion -Comparer $Comparer);
        }
        $Diff = Compare-SemverVersionStrings -LVersion $LVersion -RVersion $RVersion.Substring(0, $RIndex) -Comparer $Comparer;
        if ($Diff -eq 0) { return -1 }
        return $Diff;
    }

    if ($RIndex -lt 0) {
        $Diff = Compare-SemverVersionStrings -LVersion $LVersion.Substring(0, $LIndex) -RVersion $RVersion -Comparer $Comparer;
        if ($Diff -eq 0) { return 1 }
        return $Diff;
    }

    $Diff = Compare-SemverVersionStrings -LVersion $LVersion.Substring(0, $LIndex) -RVersion $RVersion.Substring(0, $RIndex) -Comparer $Comparer;
    if ($Diff -ne 0) { return $Diff }

    $LText = $LVersion.Substring($LIndex + 1).Trim();
    $RText = $RVersion.Substring($RIndex + 1).Trim();

    if ($LText.Length -eq 0) {
        if ($RText.Length -eq 0) { return 0 }
        return -1;
    }
    if ($RText.Length -eq 0) { return 1 }
    return (Compare-GenericVersionStrings -LVersion $LText -RVersion $RText -Comparer $Comparer);
}

Function Compare-VersionStrings {
    <#
    .SYNOPSIS
        Compare version strings.
    .DESCRIPTION
        Compares two version strings, returning a number indicating whether one version is less than, greater than, or equal to the other.
    #>
    [CmdletBinding(DefaultParameterSetName = 'Generic')]
    [OutputType([int])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [AllowEmptyString()]
        [AllowNull()]
        # The version string to be compared.
        [string]$LVersion,

        [Parameter(Mandatory = $true, Position = 1)]
        [AllowEmptyString()]
        [AllowNull()]
        # The version string to compare to.
        [string]$RVersion,

        # Use Semantic Versioning scheme.
        [Parameter(Mandatory = $true, ParameterSetName = 'SemVer')]
        [switch]$SemVer,

        # Use dot-separated versioning scheme.
        [Parameter(Mandatory = $true, ParameterSetName = 'DotSeparated')]
        [switch]$DotSeparated,

        # Don't assume that corresponding version number elements are zero when one version string has more version number elements than the other.
        [switch]$DontAssumeZeroElements,

        # Do not treat null version strings as though they are empty strings.
        [switch]$NullNotSameAsEmpty,

        [System.StringComparison]$ComparisonType = [System.StringComparison]::OrdinalIgnoreCase
    )

    Begin {
        $Comparer = $ComparisonType | Get-StringComparer;
        if ($DotSeparated.IsPresent) {
            if ($ComparisonType | Test-IsStringComparisonOrdinal) {
                if ($DontAssumeZeroElements.IsPresent) {
                    Set-Alias -Name 'CompareVersionStrings' -Value 'Compare-VersionCoreExact';
                } else {
                    Set-Alias -Name 'CompareVersionStrings' -Value 'Compare-VersionCore';
                }
            } else {
                if ($DontAssumeZeroElements.IsPresent) {
                    Set-Alias -Name 'CompareVersionStrings' -Value 'Compare-NonOrdinalVersionCoreExact';
                } else {
                    Set-Alias -Name 'CompareVersionStrings' -Value 'Compare-NonOrdinalVersionCore';
                }
            }
        } else {
            if ($ComparisonType | Test-IsStringComparisonOrdinal) {
                if ($DontAssumeZeroElements.IsPresent) {
                    Set-Alias -Name 'CompareElements' -Value 'Compare-VersionCoreExact';
                } else {
                    Set-Alias -Name 'CompareElements' -Value 'Compare-VersionCore';
                }
            } else {
                if ($DontAssumeZeroElements.IsPresent) {
                    Set-Alias -Name 'CompareElements' -Value 'Compare-NonOrdinalVersionCoreExact';
                } else {
                    Set-Alias -Name 'CompareElements' -Value 'Compare-NonOrdinalVersionCore';
                }
            }
            if ($SemVer.IsPresent) {
                Set-Alias -Name 'CompareVersionStrings' -Value 'Compare-SemverVersionStrings';
            } else {
                Set-Alias -Name 'CompareVersionStrings' -Value 'Compare-GenericVersionStrings';
            }
        }
    }

    Process {
        if ($null -eq $LVersion) {
            if ($NullNotSameAsEmpty.IsPresent) {
                if ($null -eq $RVersion) { return 0 }
            } else {
                if ([string]::IsNullOrEmpty($RVersion)) { return 0 }
            }
            return -1;
        }
        if ($null -eq $RVersion) {
            if ($NullNotSameAsEmpty.IsPresent -or $LVersion.Length -gt 0) { return 1 }
            return 0;
        }
        if ($LVersion.Length -eq 0) {
            if ($RVersion.Length -eq 0) { return 0 }
            return -1;
        }
        if ($RVersion.Length -eq 0) { return 1 }
        return CompareVersionStrings -LVersion $LVersion -RVersion $RVersion -Comparer $Comparer;
    }
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

Function Find-VsExtensionFromMarketPlace {
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
