Add-Type -Path 'C:\Users\lerwi\GitHub\PowerShell-ModulesP2\src\UnitTestProject1\App_Data\ColorStructTestDataSet.Designer.cs' -ReferencedAssemblies 'System.Data', 'System.Data.DataSetExtensions', 'System.Xml' -ErrorAction Stop;

$DataSet = [UnitTestProject1.App_Data.ColorStructTestData]::new();

[System.Drawing.Color].GetProperties() | Where-Object { $_.PropertyType -eq [System.Drawing.Color]; } | ForEach-Object {
    [System.Drawing.Color]$Color = $_.GetValue($null);
    if ($Color.A -ne 0) {
        $rgb = [int]::Parse($Color.ToArgb().ToString('X8').Substring(2), [System.Globalization.NumberStyles]::HexNumber);
        [UnitTestProject1.App_Data.ColorStructTestData+Rgb32Row]$Rgb32Row = @($DataSet.Rgb32.Rows) | Where-Object { $_.RGB -eq $rgb } | Select-Object -First 1;
        $Hue = $Color.GetHue();
        $Saturation = $Color.GetSaturation();
        $Brightness = $Color.GetBrightness();
        if ($null -eq $Rgb32Row) {
            $Rgb32Row = $DataSet.Rgb32.NewRgb32Row();
            $Rgb32Row.RGB = $rgb;
            $Rgb32Row.Red = $Color.R;
            $Rgb32Row.Green = $Color.G;
            $Rgb32Row.Blue = $Color.B;
            $DataSet.Rgb32.AddRgb32Row($Rgb32Row);
        }
        [UnitTestProject1.App_Data.ColorStructTestData+HslFloatRow]$HslFloatRow = @($DataSet.HslFloat.Rows) | Where-Object { $_.Hue -eq $Hue -and $_.Saturation -eq $Saturation -and $_.Brightness -eq $Brightness } | Select-Object -First 1;
        if ($null -eq $HslFloatRow) {
            $HslFloatRow = $DataSet.HslFloat.NewHslFloatRow();
            $HslFloatRow.Brightness = $Brightness;
            $HslFloatRow.Hue = $Hue;
            $HslFloatRow.ID = [Guid]::NewGuid();
            $HslFloatRow.Saturation = $Saturation;
            $DataSet.HslFloat.AddHslFloatRow($HslFloatRow);
            $HslFloatRow.AcceptChanges();
        }
        [UnitTestProject1.App_Data.ColorStructTestData+RgbFloatRow]$RgbFloatRow = @($DataSet.RgbFloat.Rows) | Where-Object { $_.Red -eq ([float]($Color.R)) -and $_.Green -eq ([float]($Color.G)) -and $_.Blue -eq ([float]($Color.B)) } | Select-Object -First 1;
        if ($null -eq $RgbFloatRow) {
            $RgbFloatRow = $DataSet.RgbFloat.NewRgbFloatRow();
            $RgbFloatRow.ID = [Guid]::NewGuid();
            $RgbFloatRow.Red = ([float]($Color.R));
            $RgbFloatRow.Green = ([float]($Color.R));
            $RgbFloatRow.Blue = ([float]($Color.B));
            $DataSet.RgbFloat.AddRgbFloatRow($RgbFloatRow);
            $RgbFloatRow.AcceptChanges();
        }
        if ($Rgb32Row.IsHSLNull()) {
            $Rgb32Row.HSL = $HslFloatRow.ID;
            if ($Rgb32Row.IsFloatValuesNull()) { $Rgb32Row.FloatValues = $RgbFloatRow.ID }
            $Rgb32Row.AcceptChanges();
        } else {
            if ($Rgb32Row.IsFloatValuesNull()) {
                $Rgb32Row.FloatValues = $RgbFloatRow.ID;
                $Rgb32Row.AcceptChanges();
            }
        }
        if ($RgbFloatRow.IsByteValuesNull()) {
            $RgbFloatRow.ByteValues = $rgb;
            if ($RgbFloatRow.IsHSLNull()) { $RgbFloatRow.HSL = $HslFloatRow.ID }
            $RgbFloatRow.AcceptChanges();
        } else {
            if ($RgbFloatRow.IsHSLNull()) {
                $RgbFloatRow.HSL = $HslFloatRow.ID;
                $RgbFloatRow.AcceptChanges();
            }
        }
        if ($HslFloatRow.IsRGBNull()) {
            $HslFloatRow.RGB = $RgbFloatRow.ID;
            $RgbFloatRow.AcceptChanges();
        }
        [UnitTestProject1.App_Data.ColorStructTestData+DrawingColorRow]$DrawingColorRow = @($DataSet.DrawingColor.Rows) | Where-Object { $_.Name -eq $Color.Name } | Select-Object -First 1;
        if ($null -eq $DrawingColorRow) {
            $DataSet.DrawingColor.AddDrawingColorRow($Rgb32Row, $Color.Name).AcceptChanges();
        }
    }
}

[System.Drawing.Color].GetProperties() | Where-Object { $_.PropertyType -eq [System.Windows.Media.Color] -and $_.CanRead -and $_.GetMethod.IsStatic; } | ForEach-Object {
    [System.Windows.Media.Color]$Color = $_.GetValue($null);
    if ($Color.A -ne 0) {
        $dc = [System.Drawing.Color]::FromArgb(255, $Color.R, $Color.G, $Color.B);
        $rgb = [int]::Parse($dc.ToArgb().ToString('X8').Substring(2), [System.Globalization.NumberStyles]::HexNumber);
        [UnitTestProject1.App_Data.ColorStructTestData+Rgb32Row]$Rgb32Row = @($DataSet.Rgb32.Rows) | Where-Object { $_.RGB -eq $rgb } | Select-Object -First 1;
        $Hue = $dc.GetHue();
        $Saturation = $dc.GetSaturation();
        $Brightness = $dc.GetBrightness();
        if ($null -eq $Rgb32Row) {
            $Rgb32Row = $DataSet.Rgb32.NewRgb32Row();
            $Rgb32Row.RGB = $rgb;
            $Rgb32Row.Red = $Color.R;
            $Rgb32Row.Green = $Color.G;
            $Rgb32Row.Blue = $Color.B;
            $DataSet.Rgb32.AddRgb32Row($Rgb32Row);
        }
        [UnitTestProject1.App_Data.ColorStructTestData+HslFloatRow]$HslFloatRow = @($DataSet.HslFloat.Rows) | Where-Object { $_.Hue -eq $Hue -and $_.Saturation -eq $Saturation -and $_.Brightness -eq $Brightness } | Select-Object -First 1;
        if ($null -eq $HslFloatRow) {
            $HslFloatRow = $DataSet.HslFloat.NewHslFloatRow();
            $HslFloatRow.Brightness = $Brightness;
            $HslFloatRow.Hue = $Hue;
            $HslFloatRow.ID = [Guid]::NewGuid();
            $HslFloatRow.Saturation = $Saturation;
            $DataSet.HslFloat.AddHslFloatRow($HslFloatRow);
            $HslFloatRow.AcceptChanges();
        }
        [UnitTestProject1.App_Data.ColorStructTestData+RgbFloatRow]$RgbFloatRow = @($DataSet.RgbFloat.Rows) | Where-Object { $_.Red -eq ([float]($Color.R)) -and $_.Green -eq ([float]($Color.G)) -and $_.Blue -eq ([float]($Color.B)) } | Select-Object -First 1;
        if ($null -eq $RgbFloatRow) {
            $RgbFloatRow = $DataSet.RgbFloat.NewRgbFloatRow();
            $RgbFloatRow.ID = [Guid]::NewGuid();
            $RgbFloatRow.Red = ([float]($Color.R));
            $RgbFloatRow.Green = ([float]($Color.R));
            $RgbFloatRow.Blue = ([float]($Color.B));
            $DataSet.RgbFloat.AddRgbFloatRow($RgbFloatRow);
            $RgbFloatRow.AcceptChanges();
        }
        if ($Rgb32Row.IsHSLNull()) {
            $Rgb32Row.HSL = $HslFloatRow.ID;
            if ($Rgb32Row.IsFloatValuesNull()) { $Rgb32Row.FloatValues = $RgbFloatRow.ID }
            $Rgb32Row.AcceptChanges();
        } else {
            if ($Rgb32Row.IsFloatValuesNull()) {
                $Rgb32Row.FloatValues = $RgbFloatRow.ID;
                $Rgb32Row.AcceptChanges();
            }
        }
        if ($RgbFloatRow.IsByteValuesNull()) {
            $RgbFloatRow.ByteValues = $rgb;
            if ($RgbFloatRow.IsHSLNull()) { $RgbFloatRow.HSL = $HslFloatRow.ID }
            $RgbFloatRow.AcceptChanges();
        } else {
            if ($RgbFloatRow.IsHSLNull()) {
                $RgbFloatRow.HSL = $HslFloatRow.ID;
                $RgbFloatRow.AcceptChanges();
            }
        }
        if ($HslFloatRow.IsRGBNull()) {
            $HslFloatRow.RGB = $RgbFloatRow.ID;
            $RgbFloatRow.AcceptChanges();
        }
        [UnitTestProject1.App_Data.ColorStructTestData+WindowsColorRow]$WindowsColorRow = @($DataSet.WindowsColor.Rows) | Where-Object { $_.Name -eq $_.Name } | Select-Object -First 1;
        if ($null -eq $WindowsColorRow) {
            $DataSet.WindowsColor.AddWindowsColorRow($Rgb32Row, $_.Name).AcceptChanges();
        }
    }
}

($Text = @'
    black	#000000	 
 	silver	#c0c0c0	 
 	gray	#808080	 
 	white	#ffffff	 
 	maroon	#800000	 
 	red	#ff0000	 
 	purple	#800080	 
 	fuchsia	#ff00ff	 
 	green	#008000	 
 	lime	#00ff00	 
 	olive	#808000	 
 	yellow	#ffff00	 
 	navy	#000080	 
 	blue	#0000ff	 
 	teal	#008080	 
 	aqua	#00ffff	 
 	orange	#ffa500	 
 	aliceblue	#f0f8ff	 
 	antiquewhite	#faebd7	 
 	aquamarine	#7fffd4	 
 	azure	#f0ffff	 
 	beige	#f5f5dc	 
 	bisque	#ffe4c4	 
 	blanchedalmond	#ffebcd	 
 	blueviolet	#8a2be2	 
 	brown	#a52a2a	 
 	burlywood	#deb887	 
 	cadetblue	#5f9ea0	 
 	chartreuse	#7fff00	 
 	chocolate	#d2691e	 
 	coral	#ff7f50	 
 	cornflowerblue	#6495ed	 
 	cornsilk	#fff8dc	 
 	crimson	#dc143c	 
 	cyan	#00ffff	 
 	darkblue	#00008b	 
 	darkcyan	#008b8b	 
 	darkgoldenrod	#b8860b	 
 	darkgray	#a9a9a9	 
 	darkgreen	#006400	 
 	darkgrey	#a9a9a9	 
 	darkkhaki	#bdb76b	 
 	darkmagenta	#8b008b	 
 	darkolivegreen	#556b2f	 
 	darkorange	#ff8c00	 
 	darkorchid	#9932cc	 
 	darkred	#8b0000	 
 	darksalmon	#e9967a	 
 	darkseagreen	#8fbc8f	 
 	darkslateblue	#483d8b	 
 	darkslategray	#2f4f4f	 
 	darkslategrey	#2f4f4f	 
 	darkturquoise	#00ced1	 
 	darkviolet	#9400d3	 
 	deeppink	#ff1493	 
 	deepskyblue	#00bfff	 
 	dimgray	#696969	 
 	dimgrey	#696969	 
 	dodgerblue	#1e90ff	 
 	firebrick	#b22222	 
 	floralwhite	#fffaf0	 
 	forestgreen	#228b22	 
 	gainsboro	#dcdcdc	 
 	ghostwhite	#f8f8ff	 
 	gold	#ffd700	 
 	goldenrod	#daa520	 
 	greenyellow	#adff2f	 
 	grey	#808080	 
 	honeydew	#f0fff0	 
 	hotpink	#ff69b4	 
 	indianred	#cd5c5c	 
 	indigo	#4b0082	 
 	ivory	#fffff0	 
 	khaki	#f0e68c	 
 	lavender	#e6e6fa	 
 	lavenderblush	#fff0f5	 
 	lawngreen	#7cfc00	 
 	lemonchiffon	#fffacd	 
 	lightblue	#add8e6	 
 	lightcoral	#f08080	 
 	lightcyan	#e0ffff	 
 	lightgoldenrodyellow	#fafad2	 
 	lightgray	#d3d3d3	 
 	lightgreen	#90ee90	 
 	lightgrey	#d3d3d3	 
 	lightpink	#ffb6c1	 
 	lightsalmon	#ffa07a	 
 	lightseagreen	#20b2aa	 
 	lightskyblue	#87cefa	 
 	lightslategray	#778899	 
 	lightslategrey	#778899	 
 	lightsteelblue	#b0c4de	 
 	lightyellow	#ffffe0	 
 	limegreen	#32cd32	 
 	linen	#faf0e6	 
 	magenta #ff00ff	 
 	mediumaquamarine	#66cdaa	 
 	mediumblue	#0000cd	 
 	mediumorchid	#ba55d3	 
 	mediumpurple	#9370db	 
 	mediumseagreen	#3cb371	 
 	mediumslateblue	#7b68ee	 
 	mediumspringgreen	#00fa9a	 
 	mediumturquoise	#48d1cc	 
 	mediumvioletred	#c71585	 
 	midnightblue	#191970	 
 	mintcream	#f5fffa	 
 	mistyrose	#ffe4e1	 
 	moccasin	#ffe4b5	 
 	navajowhite	#ffdead	 
 	oldlace	#fdf5e6	 
 	olivedrab	#6b8e23	 
 	orangered	#ff4500	 
 	orchid	#da70d6	 
 	palegoldenrod	#eee8aa	 
 	palegreen	#98fb98	 
 	paleturquoise	#afeeee	 
 	palevioletred	#db7093	 
 	papayawhip	#ffefd5	 
 	peachpuff	#ffdab9	 
 	peru	#cd853f	 
 	pink	#ffc0cb	 
 	plum	#dda0dd	 
 	powderblue	#b0e0e6	 
 	rosybrown	#bc8f8f	 
 	royalblue	#4169e1	 
 	saddlebrown	#8b4513	 
 	salmon	#fa8072	 
 	sandybrown	#f4a460	 
 	seagreen	#2e8b57	 
 	seashell	#fff5ee	 
 	sienna	#a0522d	 
 	skyblue	#87ceeb	 
 	slateblue	#6a5acd	 
 	slategray	#708090	 
 	slategrey	#708090	 
 	snow	#fffafa	 
 	springgreen	#00ff7f	 
 	steelblue	#4682b4	 
 	tan	#d2b48c	 
 	thistle	#d8bfd8	 
 	tomato	#ff6347	 
 	turquoise	#40e0d0	 
 	violet	#ee82ee	 
 	wheat	#f5deb3	 
 	whitesmoke	#f5f5f5	 
 	yellowgreen	#9acd32	 
    rebeccapurple	#663399	
'@ -split '\r\n?|\n') | ForEach-Object {
    $a = @($_.Trim() -split '\s+');
    $r = [int]::Parse($a[1].Substring(1, 2), [System.Globalization.NumberStyles]::HexNumber);
    $g = [int]::Parse($a[1].Substring(3, 2), [System.Globalization.NumberStyles]::HexNumber);
    $b = [int]::Parse($a[1].Substring(5, 2), [System.Globalization.NumberStyles]::HexNumber);
    $dc = [System.Drawing.Color]::FromArgb(255, $R, $G, $B);
    $rgb = [int]::Parse($dc.ToArgb().ToString('X8').Substring(2), [System.Globalization.NumberStyles]::HexNumber);
    [UnitTestProject1.App_Data.ColorStructTestData+Rgb32Row]$Rgb32Row = @($DataSet.Rgb32.Rows) | Where-Object { $_.RGB -eq $rgb } | Select-Object -First 1;
    $Hue = $dc.GetHue();
    $Saturation = $dc.GetSaturation();
    $Brightness = $dc.GetBrightness();
    if ($null -eq $Rgb32Row) {
        $Rgb32Row = $DataSet.Rgb32.NewRgb32Row();
        $Rgb32Row.RGB = $rgb;
        $Rgb32Row.Red = $r;
        $Rgb32Row.Green = $g;
        $Rgb32Row.Blue = $b;
        $DataSet.Rgb32.AddRgb32Row($Rgb32Row);
    }
    [UnitTestProject1.App_Data.ColorStructTestData+HslFloatRow]$HslFloatRow = @($DataSet.HslFloat.Rows) | Where-Object { $_.Hue -eq $Hue -and $_.Saturation -eq $Saturation -and $_.Brightness -eq $Brightness } | Select-Object -First 1;
    if ($null -eq $HslFloatRow) {
        $HslFloatRow = $DataSet.HslFloat.NewHslFloatRow();
        $HslFloatRow.Brightness = $Brightness;
        $HslFloatRow.Hue = $Hue;
        $HslFloatRow.ID = [Guid]::NewGuid();
        $HslFloatRow.Saturation = $Saturation;
        $DataSet.HslFloat.AddHslFloatRow($HslFloatRow);
        $HslFloatRow.AcceptChanges();
    }
    [UnitTestProject1.App_Data.ColorStructTestData+RgbFloatRow]$RgbFloatRow = @($DataSet.RgbFloat.Rows) | Where-Object { $_.Red -eq ([float]$r) -and $_.Green -eq ([float]$g) -and $_.Blue -eq ([float]$b) } | Select-Object -First 1;
    if ($null -eq $RgbFloatRow) {
        $RgbFloatRow = $DataSet.RgbFloat.NewRgbFloatRow();
        $RgbFloatRow.ID = [Guid]::NewGuid();
        $RgbFloatRow.Red = ([float]$r);
        $RgbFloatRow.Green = ([float]$g);
        $RgbFloatRow.Blue = ([float]$b);
        $DataSet.RgbFloat.AddRgbFloatRow($RgbFloatRow);
        $RgbFloatRow.AcceptChanges();
    }
    if ($Rgb32Row.IsHSLNull()) {
        $Rgb32Row.HSL = $HslFloatRow.ID;
        if ($Rgb32Row.IsFloatValuesNull()) { $Rgb32Row.FloatValues = $RgbFloatRow.ID }
        $Rgb32Row.AcceptChanges();
    } else {
        if ($Rgb32Row.IsFloatValuesNull()) {
            $Rgb32Row.FloatValues = $RgbFloatRow.ID;
            $Rgb32Row.AcceptChanges();
        }
    }
    if ($RgbFloatRow.IsByteValuesNull()) {
        $RgbFloatRow.ByteValues = $rgb;
        if ($RgbFloatRow.IsHSLNull()) { $RgbFloatRow.HSL = $HslFloatRow.ID }
        $RgbFloatRow.AcceptChanges();
    } else {
        if ($RgbFloatRow.IsHSLNull()) {
            $RgbFloatRow.HSL = $HslFloatRow.ID;
            $RgbFloatRow.AcceptChanges();
        }
    }
    if ($HslFloatRow.IsRGBNull()) {
        $HslFloatRow.RGB = $RgbFloatRow.ID;
        $RgbFloatRow.AcceptChanges();
    }
    
    [UnitTestProject1.App_Data.ColorStructTestData+WebColorRow]$WebColorRow = @($DataSet.WebColor.Rows) | Where-Object { $_.Name -eq $a[0] } | Select-Object -First 1;
    if ($null -eq $WebColorRow) {
        $DataSet.WebColor.AddWebColorRow($Rgb32Row, $a[0]).AcceptChanges();
    }
}

@(@($DataSet.HslFloat.Rows) | Where-Object { $_.IsByteValuesNull() }) | ForEach-Object {
    if ($_.IsByteValuesNull()) {
        $v = [Math]::Round(($_.Hue / ([float]3.6)) * ([float]2.56), 0);
        if ($v -eq 256) { $v = 255 }
        [byte]$h = $v;
        [byte]$s = [Math]::Round($_.Saturation / ([float]255), 0);
        [byte]$l = [Math]::Round($_.Brightness / ([float]255), 0);
        $hsl = [BitConverter]::ToInt32(([byte[]]@( $l, $s, $h, 0)), 0);
        [UnitTestProject1.App_Data.ColorStructTestData+Hsl32Row]$Hsl32Row = @($DataSet.Hsl32.Rows) | Where-Object { $_.HSL -eq $hsl } | Select-Object -First 1;
        if ($null -eq $Hsl32Row) {
            $Hsl32Row = $DataSet.Hsl32.NewHsl32Row();
            $Hsl32Row.HSL = $hsl;
            $Hsl32Row.Hue = $h;
            $Hsl32Row.Saturation = $s;
            $Hsl32Row.Brightness = $l;
            $DataSet.Hsl32.AddHsl32Row($Hsl32Row);
            $Hsl32Row.AcceptChanges();
        }
        $_.ByteValues = $hsl;
        $_.AcceptChanges();
    }
}

@(@($DataSet.RgbFloat.Rows) | Where-Object { $_.IsByteValuesNull() }) | ForEach-Object {
    if ($_.IsByteValuesNull()) {
        [byte]$r = [Math]::Round($_.Red / ([float]255), 0);
        [byte]$g = [Math]::Round($_.Green / ([float]255), 0);
        [byte]$b = [Math]::Round($_.Blue / ([float]255), 0);
        $rgb = [BitConverter]::ToInt32(([byte[]]($r, $g, $b, 0)), 0);
        [UnitTestProject1.App_Data.ColorStructTestData+Rgb32Row]$Rgb32Row = @($DataSet.Rgb32.Rows) | Where-Object { $_.RGB -eq $rgb } | Select-Object -First 1;
        if ($null -eq $Rgb32Row) {
            $Rgb32Row = $DataSet.Rgb32.NewHsl32Row();
            $Rgb32Row.RGB = $rgb;
            $Rgb32Row.Red = $r;
            $Rgb32Row.Green = $g;
            $Rgb32Row.Blue = $b;
            $DataSet.Rgb32.AddRgb32Row($Rgb32Row);
            $Rgb32Row.AcceptChanges();
        }
        $_.ByteValues = $rgb;
        $_.AcceptChanges();
    }
}

@(@($DataSet.Hsl32.Rows) | Where-Object { $_.IsFloatValuesNull() }) | ForEach-Object {
    if ($_.IsFloatValuesNull()) {
        [float]$h = ($_.Hue / ([float]2.56)) * ([float]3.6);
        [float]$s = $_.Saturation / ([float]255);
        [float]$l = $_.Brightness / ([float]255);
        [UnitTestProject1.App_Data.ColorStructTestData+HslFloatRow]$HslFloatRow = @($DataSet.HslFloat.Rows) | Where-Object { $_.Hue -eq $h -and $_.Saturation -eq $s -and $_.Brightness -eq $l } | Select-Object -First 1;
        if ($null -eq $HslFloatRow) {
            $HslFloatRow = $DataSet.HslFloat.NewHslFloatRow();
            $HslFloatRow.ID = [Guid]::NewGuid();
            $HslFloatRow.Brightness = $l;
            $HslFloatRow.Saturation = $s;
            $HslFloatRow.Hue = $h;
            $HslFloatRow.ByteValues = $_.HSL;
            $DataSet.HslFloat.AddHslFloatRow($HslFloatRow);
            $HslFloatRow.AcceptChanges();
        }
        $_.FloatValues = $HslFloatRow.ID;
        $_.AcceptChanges();
    }
}

@(@($DataSet.Hsl32.Rows) | Where-Object { $_.IsRGBNull() }) | ForEach-Object {
    if ($_.IsRGBNull()) {
    }
}

@(@($DataSet.Rgb32.Rows) | Where-Object { $_.IsFloatValuesNull() }) | ForEach-Object {
    if ($_.IsFloatValuesNull()) {
    }
}

@(@($DataSet.Rgb32.Rows) | Where-Object { $_.IsHSLNull() }) | ForEach-Object {
    if ($_.IsHSLNull()) {
    }
}

$XmlWriterSettings = [System.Xml.XmlWriterSettings]::new();
$XmlWriterSettings.Indent = $true;
$XmlWriterSettings.Encoding = [System.Text.UTF8Encoding]::new($false, $false);
$XmlWriter = [System.Xml.XmlWriter]::Create('C:\Users\lerwi\GitHub\PowerShell-ModulesP2\src\UnitTestProject1\App_Data\ColorStructTestData.xml', $XmlWriterSettings);
try {
    $DataSet.WriteXml($XmlWriter, [System.Data.XmlWriteMode]::WriteSchema);
    $XmlWriter.Flush();
} finally { $XmlWriter.Close() }