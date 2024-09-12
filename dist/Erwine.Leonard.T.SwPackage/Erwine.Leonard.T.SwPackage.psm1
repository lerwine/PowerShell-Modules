if ($null -eq $Script:VersionPathSeparatorChars) {
    Set-Variable -Name 'VersionPathSeparatorChars' -Scope 'Script' -Option ReadOnly -Value ([char[]]@('/', '\'));
    Set-Variable -Name 'SemverComponentSeparatorChars' -Scope 'Script' -Option ReadOnly -Value ([char[]]@('-', '+'));
}

class VersionStringElement {
    [Nullable[char]]$Separator;
    [string]$Value;
}

class VsCodeExtensionManifest {
    # [ValidateNotNullOrWhiteSpace()]
    [string]$ID = '';

    # [ValidateNotNullOrWhiteSpace()]
    [string]$Version = '';

    # [ValidateNotNull()]
    [string]$DisplayName = '';

    # [ValidateNotNull()]
    [string]$Platform = '';

    # [ValidateNotNull()]
    [string]$Description = '';

    # [ValidateNotNull()]
    [string]$Icon = '';

    static [VsCodeExtensionManifest] Create([string]$Publisher, [string]$Id, [string]$Version) {
        return [VsCodeExtensionManifest]@{
            ID = "$Publisher.$Id";
            Version = $Version;
        };
    }

    static [int] Compare([VsCodeExtensionManifest]$X, [VsCodeExtensionManifest]$Y) {
        if ($null -eq $X) {
            if ($null -eq $Y) { return 0 }
            return -1;
        }
        if ($null -eq $Y) { return 1 }
        if ([object]::ReferenceEquals($X, $Y)) { return 0 }
        $diff = Compare-VersionStrings -LVersion $X.ID -RVersion $Y.ID;
        if ($diff -eq 0) {
            $diff = Compare-VersionStrings -LVersion $X.Version -RVersion $Y.Version;
            if ($diff -eq 0) {
                if ($X.Platform -ilt $Y.Platform) { return -1 }
                if ($X.Platform -igt $Y.Platform) { return 1 }
            }
        }
        return $diff;
    }

    [string] ToPackageFileName() {
        if ([string]::IsNullOrWhiteSpace($this.Platform)) {
            return "$($this.ID)-$($this.Version).vsix";
        }
        return "$($this.ID)-$($this.Version)@$($this.Platform).vsix";
    }
}

class VsixPackageInfo : VsCodeExtensionManifest {
    # [ValidateNotNullOrEmpty()]
    [string]$Path;
}

class VsixExtensionPlatform {
}

class VsixExtensionVersion {
    [ValidateNotNull()]
    [AllowEmptyCollection()]
    [System.Collections.Generic.Dictionary[string,VsixExtensionPlatform]]$Platforms = [System.Collections.Generic.Dictionary[string,VsixExtensionPlatform]]::new([System.StringComparer]::OrdinalIgnoreCase);
}

class VsixExtensionInfo {
    [string]$DisplayName = '';

    [string]$Description = '';

    [ValidateNotNull()]
    [AllowEmptyCollection()]
    [System.Collections.Generic.Dictionary[string,VsixExtensionVersion]]$Versions = [System.Collections.Generic.Dictionary[string,VsixExtensionVersion]]::new([System.StringComparer]::OrdinalIgnoreCase);
}

class MarketplacePublisher {
    [string]$DisplayName = '';

    [ValidateNotNull()]
    [AllowEmptyCollection()]
    [System.Collections.Generic.Dictionary[string,VsixExtensionInfo]]$Extensions = [System.Collections.Generic.Dictionary[string,VsixExtensionInfo]]::new([System.StringComparer]::OrdinalIgnoreCase);
}

class VsCodeManifestIndex {
    [ValidateNotNull()]
    [AllowEmptyCollection()]
    [System.Collections.Generic.Dictionary[string,MarketplacePublisher]]$Publishers = [System.Collections.Generic.Dictionary[string,MarketplacePublisher]]::new([System.StringComparer]::OrdinalIgnoreCase);

    [MarketplacePublisher] GetPublisher([string]$Identifier) {
        [MarketplacePublisher]$Result = $null;
        if ($this.Publishers.TryGetValue($Identifier, [ref] $Result)) { return $Result }
        return $null;
    }

    [void] AddPackage([string]$PublisherID, [string]$ExtensionID, [string]$Version, [string]$Platform) {
        [MarketplacePublisher]$Publisher = $this.GetPublisher($PublisherID);
    }
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
        Compares two version strings.
    #>
    [CmdletBinding(DefaultParameterSetName = 'Generic')]
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

Function Read-VsixPackageInfo {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true, ParameterSetName = 'ByFile')]
        [Alias('FullName')]
        [string[]]$Path,

        [Parameter(Mandatory = $true, ParameterSetName = 'FromRepository')]
        [string]$RepositoryPath
    )

    Begin {
        $TempRoot = [System.IO.Path]::GetTempPath();
    }

    Process {
        if ($PSCmdlet.ParameterSetName -eq 'FromRepository') {
            (Get-ChildItem -LiteralPath $RepositoryPath -Filter '*.vsix') | Where-Object { -not $_.PSIsContainer } | Read-VsixPackageInfo;
        } else {
            foreach ($P in $Path) {
                if (Test-Path -LiteralPath $P -PathType Leaf) {
                    $Manifest = (Use-TempFolder {
                        Expand-Archive -LiteralPath $P -DestinationPath $_ -Force -ErrorAction Stop;
                        $TempPath = $_ | Join-Path -ChildPath 'extension.vsixmanifest';
                        if ($TempPath | Test-Path -PathType Leaf) {
                            [Xml]$Xml = Get-Content -LiteralPath $MPath -Force;
                            if ($null -ne $Xml) {
                                $nsmgr = [System.Xml.XmlNamespaceManager]::new($Xml.NameTable);
                                $nsmgr.AddNamespace('vsx', $Xml.DocumentElement.PSBase.NamespaceURI);
                                $XmlElement = $Xml.SelectSingleNode('/vsx:PackageManifest/vsx:Metadata/vsx:Identity', $nsmgr);
                                if ($null -ne $XmlElement) {
                                    $Manifest = [VsCodeExtensionManifest]::Create($IdentityElement.PSBase.GetAttribute('Publisher'), $IdentityElement.PSBase.GetAttribute('Id'), $IdentityElement.PSBase.GetAttribute('Version'));
                                    $Manifest.Platform = $IdentityElement.PSBase.GetAttribute('TargetPlatform');
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
                    });
                    $DirName = [Guid]::NewGuid().ToString('n');
                    $TempPath = $TempRoot | Join-Path -ChildPath $DirName;
                    while ($TempPath | Test-Path) {
                        $DirName = [Guid]::NewGuid().ToString('n');
                        $TempPath = $TempRoot | Join-Path -ChildPath $DirName;
                    }
                    (New-Item -Path $TempRoot -Name $DirName -ItemType Directory) | Out-Null;
                    if (-not ($TempPath | Test-Path)) { continue }
                } else {
                    Write-Error -Message "File $P not found" -Category ObjectNotFound -ErrorId 'FileNotFound' -TargetObject $P -CategoryTargetName 'Path';
                }
            }
        }
    }
}

Function Optimize-VsCodeManifestIndex {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNull()]
        [AllowEmptyCollection()]
        [System.Collections.ObjectModel.Collection[VsCodeExtensionManifest]]$ManifestIndex
    )

    $LastIndex = $ManifestIndex.Count - 1;
    if ($LastIndex -lt 1) { return }

    $Comparer = [System.StringComparer]::OrdinalIgnoreCase;

    while ($LastIndex -gt 0) {
        [VsCodeExtensionManifest]$Largest = $ManifestIndex[0];
        $LargestIndex = 0;
        $CurrentIndex = 1;
        do {
            [VsCodeExtensionManifest]$Current = $ManifestIndex[$CurrentIndex];
            $diff = Compare-VersionStrings -LVersion $Largest.ID -RVersion $Current.ID;
            if ($diff -eq 0) {
                $diff = Compare-VersionStrings -LVersion $Largest.Version -RVersion $Current.Version;
                if ($diff -eq 0) { $diff = $Comparer.Compare($Largest.Platform, $Current.Platform); }
            }
            if ($diff -le 0) {
                $LargestIndex = $CurrentIndex;
                $Largest = $Current;
            }
            $CurrentIndex++;
        } while ($CurrentIndex -le $LastIndex);
        if ($LargestIndex -lt $LastIndex) {
            $ManifestIndex[$LargestIndex] = $ManifestIndex[$LastIndex];
            $ManifestIndex[$LastIndex] = $Largest;
        }
        $LastIndex--;
    }
}

Function Read-VsCodeManifestIndex {
    [CmdletBinding()]
    [OutputType([System.Collections.ObjectModel.Collection[VsCodeExtensionManifest]])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$RepositoryPath
    )

    $Result = [System.Collections.ObjectModel.Collection[VsCodeExtensionManifest]]::new();
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
                            $Result.Add(([VsCodeExtensionManifest]@{
                                ID = $JsonElement.ID;
                                Version = $JsonElement.Version;
                            }));
                        } else {
                            $Result.Add(([VsCodeExtensionManifest]@{
                                ID = $JsonElement.ID;
                                Version = $JsonElement.Version;
                                Description = $JsonElement.Description;
                            }));
                        }
                    } else {
                        if ([string]::IsNullOrWhiteSpace($JsonElement.Description)) {
                            $Result.Add(([VsCodeExtensionManifest]@{
                                ID = $JsonElement.ID;
                                Version = $JsonElement.Version;
                                DisplayName = $JsonElement.DisplayName;
                            }));
                        } else {
                            $Result.Add(([VsCodeExtensionManifest]@{
                                ID = $JsonElement.ID;
                                Version = $JsonElement.Version;
                                DisplayName = $JsonElement.DisplayName;
                                Description = $JsonElement.Description;
                            }));
                        }
                    }
                } else {
                    if ([string]::IsNullOrWhiteSpace($JsonElement.DisplayName)) {
                        if ([string]::IsNullOrWhiteSpace($JsonElement.Description)) {
                            $Result.Add(([VsCodeExtensionManifest]@{
                                ID = $JsonElement.ID;
                                Version = $JsonElement.Version;
                                Platform = $JsonElement.Platform;
                            }));
                        } else {
                            $Result.Add(([VsCodeExtensionManifest]@{
                                ID = $JsonElement.ID;
                                Version = $JsonElement.Version;
                                Platform = $JsonElement.Platform;
                                Description = $JsonElement.Description;
                            }));
                        }
                    } else {
                        if ([string]::IsNullOrWhiteSpace($JsonElement.Description)) {
                            $Result.Add(([VsCodeExtensionManifest]@{
                                ID = $JsonElement.ID;
                                Version = $JsonElement.Version;
                                Platform = $JsonElement.Platform;
                                DisplayName = $JsonElement.DisplayName;
                            }));
                        } else {
                            $Result.Add(([VsCodeExtensionManifest]@{
                                ID = $JsonElement.ID;
                                Version = $JsonElement.Version;
                                Platform = $JsonElement.Platform;
                                DisplayName = $JsonElement.DisplayName;
                                Description = $JsonElement.Description;
                            }));
                        }
                    }
                }
            }
            catch {
                Write-Error -ErrorRecord $_ -CategoryReason "Failed to convert object #$ObjNum $($JsonElement | ConvertTo-Json) in $IndexPath as JSON";
            }
        }
    }
    Write-Output -InputObject $Result -NoEnumerate;
}

Function Write-VsCodeManifestIndex {
    [CmdletBinding()]
    [OutputType()]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$RepositoryPath,

        [Parameter(Mandatory = $true)]
        [ValidateNotNull()]
        [AllowEmptyCollection()]
        [System.Collections.ObjectModel.Collection[VsCodeExtensionManifest]]$ManifestIndex
    )

    Optimize-VsCodeManifestIndex -ManifestIndex $ManifestIndex;


    $IndexPath = $RepositoryPath | Join-Path -ChildPath 'index.json';
    try {
        (($ManifestIndex | ForEach-Object {
            $Item = [PSCustomObject]@{
                ID = $_.ID;
                Version = $_.Version;
            };
            if (-not [string]::IsNullOrWhiteSpace($_.Platform)) {
                $Item | Add-Member -MemberType NoteProperty -Name 'Platform' -Value $_.Platform;
            }
            if (-not [string]::IsNullOrWhiteSpace($_.DisplayName)) {
                $Item | Add-Member -MemberType NoteProperty -Name 'DisplayName' -Value $_.DisplayName;
            }
            if (-not [string]::IsNullOrWhiteSpace($_.Description)) {
                $Item | Add-Member -MemberType NoteProperty -Name 'Description' -Value $_.Description -PassThru;
            } else {
                $Item | Write-Output;
            }
        }) | ConvertTo-Json -Depth 3) | Set-Content -LiteralPath $IndexPath;
    } catch {
        Write-Error -ErrorRecord $_ -CategoryReason "Failed to write to $IndexPath";
    }
}

# class VsExtensionInfo {
#     [string]$PublisherName;
#     [string]$Name;
#     [string]$DisplayName;
#     [string]$Description;
# }

# class VsMarketPlaceQueryResult : VsExtensionInfo {
#     [DateTime]$PublishedDate;
#     [VsMarketPlaceExtensionVersion[]]$Versions;
# }

# Function Get-VsExtensionFromMarketPlace {
#     [CmdletBinding()]
#     [OutputType([VsMarketPlaceQueryResult])]
#     Param(
#         [Parameter(Mandatory = $true, Position = 0)]
#         [string]$Publisher,

#         [Parameter(Mandatory = $true, Position = 1)]
#         [string]$ID,

#         [Parameter(Mandatory = $true, Position = 2)]
#         [string]$Version,

#         [Parameter(Mandatory = $true)]
#         [string]$RepositoryFolder,

#         [string]$TargetPlatform
#     )

#     $Ps
#     $UriBuilder = [System.UriBuilder]::new($MyInvocation.MyCommand.Module.PrivateData.BaseMarketPlaceUri);
#     $UriBuilder.Path = "/_apis/public/gallery/publishers/$([Uri]::EscapeDataString($Publisher))/vsextensions/$([Uri]::EscapeDataString($ID))/$([Uri]::EscapeDataString($Version))/vspackage";
#     $FileName = "$Publisher.$ID-$Version";
#     if ($PSBoundParameters.ContainsKey('TargetPlatform')) {
#         $UriBuilder.Query = "targetPlatform=$([Uri]::EscapeDataString($TargetPlatform))";
#         $FileName = "$FileName@$TargetPlatform";
#     }
#     # $requestHeaders = [System.Collections.Generic.Dictionary[string,string]]::new();
#     # $requestHeaders.Add('Accept','application/json; charset=utf-8; api-version=3.2-preview.1');
#     # $requestHeaders.Add('Content-Type','application/json; charset=utf-8');
#     $Response = Invoke-WebRequest -Uri $UriBuilder.Uri -Method Get<# -Headers $requestHeaders#> -UseBasicParsing;
#     $p = $RepositoryFolder | Join-Path -ChildPath $FileName;
#     Set-Content -LiteralPath $p -Value $Response.RawContent -AsByteStream;
#     if ($p | Test-Path -PathType Leaf) {
#         Get-VsCodeExtensionMetaData -Path $p;
#     }
# }

# Function Find-VsExtensionFromMarketPlace {
#     [CmdletBinding()]
#     [OutputType([VsMarketPlaceQueryResult])]
#     Param(
#         [Parameter(Mandatory = $true, Position = 0)]
#         [string]$Publisher,

#         [Parameter(Mandatory = $true, Position = 0)]
#         [string]$ID,

#         [string]$TargetPlatform,

#         [int]$MaxPage = 10000,

#         [int]$PageSize = 100,

#         [Parameter(ParameterSetName = 'ExplicitAttributes')]
#         [switch]$IncludeVersions,

#         [Parameter(ParameterSetName = 'ExplicitAttributes')]
#         [switch]$IncludeFiles,

#         [Parameter(ParameterSetName = 'ExplicitAttributes')]
#         [switch]$IncludeCategoryAndTags,

#         [Parameter(ParameterSetName = 'ExplicitAttributes')]
#         [switch]$IncludeSharedAccounts,

#         [switch]$ExcludeNonValidated,

#         [switch]$IncludeVersionProperties,

#         [switch]$IncludeInstallationTargets,

#         [switch]$IncludeAssetUri,

#         [switch]$IncludeStatistics,

#         [switch]$IncludeLatestVersionOnly,

#         [switch]$Unpublished,

#         [switch]$IncludeNameConflictInfo,

#         [Parameter(Mandatory = $true, ParameterSetName = 'AllAttributes')]
#         [switch]$AllAttributes
#     )

#     $Flags = 0;
#     if ($AllAttributes.IsPresent) {
#         $Flags = 0x1f;
#     } else {
#         if ($IncludeVersions.IsPresent) { $Flags = 0x1 }
#         if ($IncludeFiles.IsPresent) { $Flags = $Flags -bor 0x2 }
#         if ($IncludeCategoryAndTags.IsPresent) { $Flags = $Flags -bor 0x4 }
#         if ($IncludeSharedAccounts.IsPresent) { $Flags = $Flags -bor 0x8 }
#         if ($IncludeVersionProperties.IsPresent) { $Flags = $Flags -bor 0x10 }
#     }
#     if ($ExcludeNonValidated.IsPresent) { $Flags = $Flags -bor 0x20 }
#     if ($IncludeInstallationTargets.IsPresent) { $Flags = $Flags -bor 0x40 }
#     if ($IncludeAssetUri.IsPresent) { $Flags = $Flags -bor 0x80 }
#     if ($IncludeStatistics.IsPresent) { $Flags = $Flags -bor 0x100 }
#     if ($IncludeLatestVersionOnly.IsPresent) { $Flags = $Flags -bor 0x200 }
#     if ($Unpublished.IsPresent) { $Flags = $Flags -bor 0x1000 }
#     if ($IncludeNameConflictInfo.IsPresent) { $Flags = $Flags -bor 0x8000 }

#     $criteria = @([PSCustomObject]@{
#         filterType = 7;
#         value = "$Publisher.$ID";
#     }, [PSCustomObject]@{
#         filterType = 8;
#         value = "Microsoft.VisualStudio.Code";
#     });
#     if ($PSBoundParameters.ContainsKey('TargetPlatform')) {
#         $criteria += [PSCustomObject]@{
#             filterType = 23;
#             value = $TargetPlatform;
#         }
#     }
#     $requestBody = [PSCustomObject]@{
#         filters = ([object[]]@([PSCustomObject]@{
#             criteria = ([object[]]$criteria);
#             pageNumber = 1;
#             pageSize = $PageSize;
#             sortBy = 0;
#             sortOrder = 0;
#         }));
#         assetTypes = (New-Object -TypeName 'System.Object[]' -ArgumentList 0);
#         flags = $Flags;
#     } | ConvertTo-Json -Depth 4;
#     $requestHeaders = [System.Collections.Generic.Dictionary[string,string]]::new();
#     $requestHeaders.Add('Accept','application/json; charset=utf-8; api-version=3.2-preview.1');
#     $requestHeaders.Add('Content-Type','application/json; charset=utf-8');
#     $UriBuilder = [System.UriBuilder]::new($MyInvocation.MyCommand.Module.PrivateData.BaseMarketPlaceUri);
#     $UriBuilder.Path = "/_apis/public/gallery/extensionquery";
#     $Response = Invoke-WebRequest -Uri $UriBuilder.Uri -Method POST -Headers $requestHeaders -Body $requestBody -UseBasicParsing;
#     if ($null -ne $Response) {
#         $Response.Content | Out-File -LiteralPath ($PSScriptRoot | Join-Path -ChildPath 'Example.json');
#         ($Response.Content | ConvertFrom-Json).results | ForEach-Object {
#             $_.extensions | ForEach-Object {
#                 $Item = [VsMarketPlaceQueryResult]@{
#                     PublisherName = $_.publisher.displayName;
#                     DisplayName = $_.displayName;
#                     PublishedDate =  = [DateTime]::Parse($_.publishedDate);
#                     Description = $_.shortDescription;
#                 };
#                 if ($Item.PublishedDate.Kind -eq [DateTimeKind]::Unspecified) {
#                     $Item.PublishedDate = [DateTime]::SpecifyKind($Item.PublishedDate, [DateTimeKind]::Utc);
#                 } else {
#                     if ($Item.PublishedDate.Kind -eq [DateTimeKind]::Local) { $Item.PublishedDate = $Item.PublishedDate.ToUniversalTime() }
#                 }
#                 $Versions = @($_.versions);
#                 if ($PSBoundParameters.ContainsKey('TargetPlatform')) {
#                     $Versions = @($Versions | Where-Object { $_.targetPlatform -ieq $TargetPlatform });
#                 } else {
#                     $Versions = @($Versions | Where-Object { [string]::IsNullOrWhiteSpace($_.targetPlatform) });
#                 }
#                 if ($Versions.Count -eq 0) {
#                     $Versions = @($_.versions);
#                 }

#                 $Item.Versions = ([VsMarketPlaceExtensionVersion[]]@($Versions | ForEach-Object {
#                     $v = [VsMarketPlaceExtensionVersion]@{
#                         Version = $_.version;
#                         LastUpdated = [DateTime]::Parse($_.lastUpdated);
#                         TargetPlatform = $_.targetPlatform;
#                     };
#                     if ($v.LastUpdated.Kind -eq [DateTimeKind]::Unspecified) {
#                         $v.LastUpdated = [DateTime]::SpecifyKind($v.LastUpdated, [DateTimeKind]::Utc);
#                     } else {
#                         if ($v.LastUpdated.Kind -eq [DateTimeKind]::Local) { $v.LastUpdated = $v.LastUpdated.ToUniversalTime() }
#                     }
#                     $v | Write-Output;
#                 }));

#                 $Item | Write-Output;
#             }
#         };
#     }
# }
