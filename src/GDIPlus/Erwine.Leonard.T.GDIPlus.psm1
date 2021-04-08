Function Convert-HsbToRgb {
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

Function Convert-HsbToRgb {
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

Function Convert-RgbToHsb {
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

$Color = [System.Drawing.Color]::FromArgb(255, 200, 127, 64);
$hsb = Convert-RgbToHsb -R ($Color.R / 255.0) -G ($Color.G / 255.0) -B ($Color.B / 255.0);
"$($Color.R),$($Color.G),$($Color.B) ($($Color.R / 255.0),$($Color.G / 255.0),$($Color.B / 255.0))";
"Expected: $($Color.GetHue()); Actual: $($hsb[0])";
"Expected: $($Color.GetSaturation()); Actual: $($hsb[1])";
"Expected: $($Color.GetBrightness()); Actual: $($hsb[2])";
[System.Drawing.Color].IsValueType