Function Convert-HsbToRgbOld {
    [CmdletBinding()]
    Param(
      [Parameter(Mandatory = $true, Position = 0)]
      [ValidateRange(0.0, 360.0)]
      [float]$H,
      
      [Parameter(Mandatory = $true, Position = 1)]
      [ValidateRange(0.0, 1.0)]
      [float]$S,
      
      [Parameter(Mandatory = $true, Position = 2)]
      [ValidateRange(0.0, 1.0)]
      [Alias('v')]
      [float]$B
    )
   
    [float]$fMax = 0.0;
    [float]$fMin = 0.0;
    if ($B -lt 0.5) {
        [float]$fMax = $b + ($b * $s);
        [float]$fMin = $b - ($b * $s);
    } else {
        [float]$fMax = $b - ($b * $s) + $s;
        [float]$fMin = $b + ($b * $s) - $s;
    }

    [int]$iSextant = [Math]::Floor($h / ([float](60)));
    if ($H -ge 300.0) {
        [float]$H -= 360.0;
    }
    [float]$H /= 60.0;
    [float]$H -= 2.0 * [Math]::Floor((($iSextant + 1.0) % 6.0) / 2.0);
    [float]$fMid = 0.0;
    if (($iSextant % 2) -eq 0) {
        [float]$fMid = $H * ($fMax - $fMin) + $fMin;
    } else {
        [float]$fMid = $fMin - $H * ($fMax - $fMin);
    }
      
    $iMax = [Convert]::ToInt32($fMax * 255);
    $iMid = [Convert]::ToInt32($fMid * 255);
    $iMin = [Convert]::ToInt32($fMin * 255);

    switch ($iSextant) {
        1 { return @($iMid, $iMax, $iMin) }
        2 { return @($iMin, $iMax, $iMid) }
        3 { return @($iMin, $iMid, $iMax) }
        4 { return @($iMid, $iMin, $iMax) }
        5 { return @($iMax, $iMin, $iMid) }
    }
  
    return @($iMax, $iMid, $iMin);
}

Function Convert-RgbToHsbOld {
    [CmdletBinding()]
    Param(
      [Parameter(Mandatory = $true, Position = 0)]
      [ValidateRange(0.0, 1.0)]
      [float]$R,
      
      [Parameter(Mandatory = $true, Position = 1)]
      [ValidateRange(0.0, 1.0)]
      [float]$G,
      
      [Parameter(Mandatory = $true, Position = 2)]
      [ValidateRange(0.0, 1.0)]
      [Alias('v')]
      [float]$B
    )

    $Min = 0.0;
    $Max = 1.0;
    if ($R -lt $G) {
        if ($B -lt $R) {
            $Min = $B;
            $Max = $G;
        } else {
            $Min = $R;
            if ($G -lt $B) { $Max = $B } else { $Max = $G }
        }
    } else {
        if ($B -lt $G) {
            # B < G < R
            $Min = $B;
            $Max = $R;
        } else {
            $Min = $G;
            if ($R -lt $B) { $Max = $B } else { $Max = $R }
        }
    }
    $Delta = $Max - $Min;
    $Saturation = 0.0;
    $Hue = 0.0;
    $Brightness = $Max;
    if ($Delta -gt 0) {
        if ($Max -eq $R) {
            $Hue = ($G - $B) / $Delta;
        } else {
            if ($Max -eq $G) {
                $Hue = 2.0 + ($B - $R) / $Delta;
            } else {
                $Hue = 4.0 + ($R - $G) / $Delta;
            }
        }
        $Hue *= 60.0;
        if ($Hue -lt 0.0) {
            $Hue += 360.0;
        }
        $mm = $Max + $Min;
        $Brightness = $mm / 2.0;
        if ($Brightness -le 0.5) {
            $Saturation = $Delta / $mm;
        } else {
            $Saturation = $Delta / (2.0 - $mm);
        }
    }
    "Min: $Min; Max: $Max" | Write-Host;
    return @($Hue, $Saturation, $Brightness);
}
Function Convert-RgbToHsv {
    [CmdletBinding()]
    Param(
      [Parameter(Mandatory = $true, Position = 0)]
      [ValidateRange(0, 255)]
      [int]$R,
      
      [Parameter(Mandatory = $true, Position = 1)]
      [ValidateRange(0, 255)]
      [int]$G,
      
      [Parameter(Mandatory = $true, Position = 2)]
      [ValidateRange(0, 255)]
      [int]$B
    )
    
    $RF = ([float]$R) / 255.0;
    $GF = ([float]$G) / 255.0;
    $BF = ([float]$B) / 255.0;
    $Cmax = $RF;
    $Cmin = $BF;
    if ($GF -gt $RF) {
        if ($BF -gt $GF) {
            $Cmax = $BF;
            $Cmin = $RF;
        } else {
            $Cmax = $GF;
            if ($BF -gt $RF) { $Cmin = $RF }
        }
    } else {
        if ($BF -gt $RF) {
            $Cmax = $BF;
            $CMin = $GF;
        } else {
            if ($BF -gt $GF) { $Cmin = $GF }
        }
    }
    $Delta = $Cmax - $CMin;
    $Hue = 0.0;
    if ($Delta -gt 0) {
        if ($Cmax -eq $RF) {
            $Hue = ($GF - $BF) / $Delta;
        } else {
            if ($Cmax -eq $GF) {
                $Hue = 2.0 + ($BF - $RF) / $Delta;
            } else {
                $Hue = 4.0 + ($RF - $GF) / $Delta;
            }
        }
        $Hue *= 60.0;
        if ($Hue -lt 0.0) {
            $Hue += 360.0;
        }
        $Saturation = $Cmax;
        if ($Saturation -ne 0) { $Saturation = $Delta / $Saturation }
    }
    return @($Hue, $Saturation, $Cmax);
}

Function Convert-HsvToRgb {
    [CmdletBinding()]
    Param(
      [Parameter(Mandatory = $true, Position = 0)]
      [ValidateRange(0.0, 360.0)]
      [float]$H,
      
      [Parameter(Mandatory = $true, Position = 1)]
      [ValidateRange(0.0, 1.0)]
      [float]$S,
      
      [Parameter(Mandatory = $true, Position = 2)]
      [ValidateRange(0.0, 1.0)]
      [float]$V
    )
    $C = $V * $S;
    [int]$iSextant = [Math]::Floor($h / ([float](60)));
    $iSextant = ($iSextant % 2) - 1;
    if ($iSextant -lt 0) { $iSextant *= -1 }
    $X = $C * (1.0 - $iSextant);
    $m = $V - $C;
    $RF = 0.0;
    $GF = 0.0;
    $BF = 0.0;
    if ($H -lt 60) {
        ($RF, $GF, $BF) = @($C, $X, 0.0);
    } else {
        if ($H -lt 120) {
            ($RF, $GF, $BF) = @($X, $C, 0.0);
        } else {
            if ($H -lt 180) {
                ($RF, $GF, $BF) = @(0.0, $C, $X);
            } else {
                if ($H -lt 240) {
                    ($RF, $GF, $BF) = @(0.0, $X, $C);
                } else {
                    if ($H -lt 300) {
                        ($RF, $GF, $BF) = @($X, 0.0, $C);
                    } else {
                        ($RF, $GF, $BF) = @($C, 0.0, $X);
                    }
                }
            }
        }
    }

    return @([Convert]::ToInt32(($RF + $m) * 255.0), [Convert]::ToInt32(($GF + $m) * 255.0), [Convert]::ToInt32(($BF + $m) * 255.0));
}

Function Convert-HsbToRgb {
    [CmdletBinding(DefaultParameterSetName = "FloatValues")]
    Param(
      [Parameter(Mandatory = $true, Position = 0, ParameterSetName = "FloatValues")]
      [ValidateRange(0.0, 360.0)]
      [Alias('Hue')]
      [float]$H,
      
      [Parameter(Mandatory = $true, Position = 1, ParameterSetName = "FloatValues")]
      [ValidateRange(0.0, 1.0)]
      [Alias('Saturation')]
      [float]$S,
      
      [Parameter(Mandatory = $true, Position = 2, ParameterSetName = "FloatValues")]
      [ValidateRange(0.0, 1.0)]
      [Alias('V')]
      [Alias('Brightness')]
      [float]$B,

      [Parameter(Position = 3, ParameterSetName = "FloatValues")]
      [ValidateRange(0.0, 1.0)]
      [Alias("Aplha")]
      [float]$A = 1.0,
      
      [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = "HsbColor")]
      [Erwine.Leonard.T.GDIPlus.HsbColorF]$HSB,

      [switch]$ToFloatingPoint
    )
   
    Process {
        $HsbColorF = $HSB;
        if (-not $PSBoundParameters.ContainsKey('HSB')) { $HsbColorF = [Erwine.Leonard.T.GDIPlus.HsbColorF]::new($H, $S, $B, $A) }
        if ($ToFloatingPoint.IsPresent) {
            $HsbColorF.AsRgbF() | Write-Output;
        } else {
            $HsbColorF.AsRgb32() | Write-Output;
        }
    }
}

Function Convert-Hsb32ToRgb {
    [CmdletBinding(DefaultParameterSetName = "FloatValues")]
    Param(
      [Parameter(Mandatory = $true, Position = 0, ParameterSetName = "IntValues")]
      [ValidateRange(0, 255)]
      [Alias('Hue')]
      [int]$H,
      
      [Parameter(Mandatory = $true, Position = 1, ParameterSetName = "IntValues")]
      [ValidateRange(0, 255)]
      [Alias('Saturation')]
      [int]$S,
      
      [Parameter(Mandatory = $true, Position = 2, ParameterSetName = "IntValues")]
      [ValidateRange(0, 255)]
      [Alias('V')]
      [Alias('Brightness')]
      [int]$B,

      [Parameter(Position = 3, ParameterSetName = "IntValues")]
      [ValidateRange(0, 255)]
      [Alias("Aplha")]
      [int]$A = 255,
      
      [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = "HsbColor")]
      [Erwine.Leonard.T.GDIPlus.HsbColor32]$HSB,

      [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = "HexString")]
      [ValidatePattern("^#?[a-fA-F\d]{6,8}")]
      [string]$Hex,

      [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = "Value")]
      [int]$Value,

      [switch]$ToFloatingPoint
    )
   
    Process {
        $HsbColor32 = $HSB;
        if ($PSBoundParameters.ContainsKey('Value')) {
            $HsbColor32 = [Erwine.Leonard.T.GDIPlus.HsbColor32]::new($Value);
        } else {
            if ($PSBoundParameters.ContainsKey('Hex')) {
                $HsbColor32 = [Erwine.Leonard.T.GDIPlus.HsbColor32]::Parse($Hex);
            } else {
                if (-not $PSBoundParameters.ContainsKey('HSB')) { $HsbColor32 = [Erwine.Leonard.T.GDIPlus.HsbColor32]::new($H, $S, $B, $A) }
            }
        }
        if ($ToFloatingPoint.IsPresent) {
            $HsbColor32.AsRgbF() | Write-Output;
        } else {
            $HsbColor32.AsRgb32() | Write-Output;
        }
    }
}

Function Convert-RgbFToHsb {
    [CmdletBinding()]
    Param(
      [Parameter(Mandatory = $true, Position = 0, ParameterSetName = "FloatValues")]
      [ValidateRange(0.0, 1.0)]
      [Alias('Red')]
      [float]$R,
      
      [Parameter(Mandatory = $true, Position = 1, ParameterSetName = "FloatValues")]
      [ValidateRange(0.0, 1.0)]
      [Alias('Green')]
      [float]$G,
      
      [Parameter(Mandatory = $true, Position = 2, ParameterSetName = "FloatValues")]
      [ValidateRange(0.0, 1.0)]
      [Alias('Blue')]
      [float]$B,

      [Parameter(Position = 3, ParameterSetName = "FloatValues")]
      [ValidateRange(0.0, 1.0)]
      [Alias("Aplha")]
      [float]$A = 1.0,
      
      [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = "RgbColorF")]
      [Erwine.Leonard.T.GDIPlus.RgbColorF]$RGB,

      [switch]$To32Bit
    )
   
    Process {
        $RgbColorF = $RGB;
        if (-not $PSBoundParameters.ContainsKey('RGB')) { $RgbColorF = [Erwine.Leonard.T.GDIPlus.RgbColorF]::new($R, $G, $B, $A) }
        if ($To32Bit.IsPresent) {
            $RgbColorF.AsHsb32() | Write-Output;
        } else {
            $RgbColorF.AsHsbF() | Write-Output;
        }
    }
}

Function Convert-RgbToHsb {
    [CmdletBinding()]
    Param(
      [Parameter(Mandatory = $true, Position = 0, ParameterSetName = "IntValues")]
      [ValidateRange(0, 255)]
      [Alias('Red')]
      [int]$R,
      
      [Parameter(Mandatory = $true, Position = 1, ParameterSetName = "IntValues")]
      [ValidateRange(0, 255)]
      [Alias('Green')]
      [int]$G,
      
      [Parameter(Mandatory = $true, Position = 2, ParameterSetName = "IntValues")]
      [ValidateRange(0, 255)]
      [Alias('Blue')]
      [int]$B,

      [Parameter(Position = 3, ParameterSetName = "IntValues")]
      [ValidateRange(0, 255)]
      [Alias("Aplha")]
      [int]$A = 255,
      
      [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = "RgbColor")]
      [Erwine.Leonard.T.GDIPlus.RgbColor32]$RGB,

      [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = "HexString")]
      [ValidatePattern("^#?[a-fA-F\d]{6,8}")]
      [string]$Hex,

      [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = "Value")]
      [int]$Value,

      [switch]$To32Bit
    )
   
    Process {
        $RgbColor32 = $HSB;
        if ($PSBoundParameters.ContainsKey('Value')) {
            $RgbColor32 = [Erwine.Leonard.T.GDIPlus.RgbColor32]::new($Value);
        } else {
            if ($PSBoundParameters.ContainsKey('Hex')) {
                $RgbColor32 = [Erwine.Leonard.T.GDIPlus.RgbColor32]::Parse($Hex);
            } else {
                if (-not $PSBoundParameters.ContainsKey('HSB')) { $RgbColor32 = [Erwine.Leonard.T.GDIPlus.RgbColor32]::new($H, $S, $B, $A) }
            }
        }
        if ($ToFloatingPoint.IsPresent) {
            $RgbColor32.AsHsb32() | Write-Output;
        } else {
            $RgbColor32.AsHsbF() | Write-Output;
        }
    }
}
