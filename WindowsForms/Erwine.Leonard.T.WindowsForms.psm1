('System.Windows.Forms', 'System.Drawing') | ForEach-Object {
	if ((Add-Type -AssemblyName $_ -PassThru -ErrorAction Stop) -eq $null) { throw ('Cannot load assembly "{0}".' -f $_) }
}

Function New-DrawingPoint {
    [CmdletBinding(DefaultParameterSetName = 'Size')]
    [OutputType([System.Drawing.Point])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Values')]
		[Alias('Horizontal')]
        # The horizontal position of the point.
        [int]$X,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
		[Alias('Vertical')]
        # The vertical position of the point.
        [string]$Y,
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Size')]
        # A Size that specifies the coordinates for the new Point.
        [System.Drawing.Size]$Size
    )
    <#
        .SYNOPSIS
            Create new Point structure.

        .DESCRIPTION
            Creates a new Point structure from the specified coordinates.
        
        .OUTPUTS
            System.Drawing.Point. Represents an ordered pair of integer x- and y-coordinates that defines a point in a two-dimensional plane.
        
        .LINK
            New-DrawingSize
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.point.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.size.aspx
    #>
    
    if ($PSBoundParameters.ContainsKey('Size')) {
        New-Object -TypeName 'System.Drawing.Point' -ArgumentList $Size;
    } else {
        New-Object -TypeName 'System.Drawing.Point' -ArgumentList $X, $Y;
    }
}

Function New-DrawingSize {
    [CmdletBinding(DefaultParameterSetName = 'Point')]
    [OutputType([System.Drawing.Size])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Values')]
        # The width component of the new Size.
        [int]$Width,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
        # The height component of the new Size.
        [string]$Height,
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Point')]
        # The Point structure from which to initialize this Size structure.
        [System.Drawing.Point]$Point
    )
    <#
        .SYNOPSIS
            Create new Size structure.

        .DESCRIPTION
            Creates a new Size structure from the specified dimensions.
        
        .OUTPUTS
            System.Drawing.Size. Stores an ordered pair of integers, which specify a Height and Width.
        
        .LINK
            New-DrawingPoint
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.size.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.point.aspx
    #>
    
    if ($PSBoundParameters.ContainsKey('Point')) {
        New-Object -TypeName 'System.Drawing.Size' -ArgumentList $Point;
    } else {
        New-Object -TypeName 'System.Drawing.Size' -ArgumentList $X, $Y;
    }
}

Function New-DrawingColor {
    [CmdletBinding(DefaultParameterSetName = 'FromArgb')]
    [OutputType([System.Drawing.Size])]
    Param(
        [Parameter(Mandatory = $false, Position = 3, ParameterSetName = 'FromArgb')]
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'ApplyAlpha')]
        [ValidateRange(0, 255)]
        [Alias('A')]
        # The alpha component.
        [int]$Alpha,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'FromArgb')]
        [ValidateRange(0, 255)]
        [Alias('R')]
        # The red component.
        [int]$Red,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'FromArgb')]
        [ValidateRange(0, 255)]
        [Alias('G')]
        # The green component.
        [int]$Green,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'FromArgb')]
        [ValidateRange(0, 255)]
        [Alias('B')]
        # The blue component.
        [int]$Blue,
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'FromName')]
        # A string that is the name of a predefined color. Valid names are the same as the names of the elements of the KnownColor enumeration.
        [string]$Name,
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'FromKnownColor')]
        [Alias('Known')]
        # An element of the KnownColor enumeration.
        [System.Drawing.KnownColor]$KnownColor,
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'ApplyAlpha')]
        [Alias('Base', 'Color')]
        # The Color from which to create the new Color.
        [System.Drawing.Color]$BaseColor
    )
    <#
        .SYNOPSIS
            Create new Color structure.

        .DESCRIPTION
            Creates a new Color structure from the name, enumerated value or ARGB components.
        
        .OUTPUTS
            System.Drawing.Color. Represents an ARGB (alpha, red, green, blue) color.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.color.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.knowncolor.aspx
    #>
    
    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'ApplyAlpha' { [System.Drawing.Color]::FromArgb($Alpha, $BaseColor); break; }
            'Known' { [System.Drawing.Color]::FromKnownColor($KnownColor); break; }
            default {
                if ($PSBoundParameters.ContainsKey('Alpha')) {
                    [System.Drawing.Color]::FromArgb($Alpha, $Red, $Green, $Blue);
                } else {
                    [System.Drawing.Color]::FromArgb($Red, $Green, $Blue);
                }
            }
        }
    }
}

Function Get-GraphicsUnit {
    [CmdletBinding()]
    [OutputType([System.Drawing.GraphicsUnit])]
    Param(
        [Parameter(Mandatory = $true, ParameterSetName = 'Display')]
        # Specifies the unit of measure of the display device. Typically pixels for video displays, and 1/100 inch for printers.
        [switch]$Display,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Document')]
        # Specifies the document unit (1/300 inch) as the unit of measure.
        [switch]$Document,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Inch')]
        # Specifies the inch as the unit of measure.
        [switch]$Inch,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Millimeter')]
		[Alias('MM')]
        # Specifies the millimeter as the unit of measure.
        [switch]$Millimeter,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Pixel')]
		[Alias('Px')]
        # Specifies a device pixel as the unit of measure.
        [switch]$Pixel,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Point')]
		[Alias('Pt')]
        # Specifies a printer's point (1/72 inch) as the unit of measure.
        [switch]$Point,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'World')]
        # Specifies the world coordinate system unit as the unit of measure.
        [switch]$World
    )
    <#
        .SYNOPSIS
            Get GraphicsUnit value.

        .DESCRIPTION
            Gets a value which specifies the unit of measure for the given data.
        
        .OUTPUTS
            System.Drawing.GraphicsUnit. Specifies the unit of measure for the given data.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.graphicsunit.aspx
    #>
    
    [System.Enum]::Parse([System.Drawing.GraphicsUnit], $PSCmdlet.ParameterSetName);
}

Function Get-FontStyle {
    [CmdletBinding(DefaultParameterSetName = 'Explicit')]
    [OutputType([System.Drawing.FontStyle])]
    Param(
        [Parameter(Mandatory = $true, ParameterSetName = 'Regular')]
        # Normal text.
        [switch]$Regular,

        [Parameter(Mandatory = $false, ParameterSetName = 'Explicit')]
		[Alias('B')]
        # Bold text.
        [switch]$Bold,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'Explicit')]
		[Alias('I')]
        # Italic text.
        [switch]$Italic,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'Explicit')]
		[Alias('S')]
        # Text with a line through the middle.
        [switch]$Strikeout,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'Explicit')]
		[Alias('U')]
        # Underlined text.
        [switch]$Underline
    )
    <#
        .SYNOPSIS
            Get FontStyle value.

        .DESCRIPTION
            Gets a value which specifies style information to be applied to text.
        
        .OUTPUTS
            System.Drawing.FontStyle. Specifies style information applied to text.
        
        .LINK
            Get-Font
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.fontstyle.aspx
        
        .NOTES
            If no switch parameters are specified, then Regular will be returned.
    #>
    
    if ($PSCmdlet.ParameterSetName -eq 'Regular' -or $PSBoundParameters.Count -eq 0) {
        [System.Drawing.FontStyle]::Regular | Write-Output;
    } else {
        $Values = @($PSBoundParameters.Keys | ForEach-Object { [System.Enum]::Parse([System.Drawing.FontStyle], $_) });
        if ($Values.Count -eq 0) {
            $Values[0] | Write-Output;
        } else {
            $FontStyle = $Values[0];
            for ($i = 1; $i -lt $Values.Count; $i++) { [System.Drawing.FontStyle]$FontStyle = $FontStyle -bor $Values[$i] }
            $FontStyle | Write-Output;
            break;
        }
    }
}

Function Get-GenericFontFamily {
    [CmdletBinding()]
    [OutputType([System.Drawing.Text.GenericFontFamilies])]
    Param(
        [Parameter(Mandatory = $true, ParameterSetName = 'Monospace')]
        # A generic Monospace FontFamily object.
        [switch]$Monospace,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'SansSerif')]
        # A generic Sans Serif FontFamily object.
        [switch]$SansSerif,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Serif')]
        # A generic Serif FontFamily object.
        [switch]$Serif
    )
    <#
        .SYNOPSIS
            Get GenericFontFamilies value.

        .DESCRIPTION
            Gets a value which specifies a generic FontFamily object.
        
        .OUTPUTS
            System.Drawing.Text.GenericFontFamilies. Specifies a generic FontFamily object.
        
        .LINK
            New-FontFamily
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.text.genericfontfamilies.aspx
    #>
    
    [System.Enum]::Parse([System.Drawing.Text.GenericFontFamilies], $PSCmdlet.ParameterSetName);
}

Function New-FontFamily {
    [CmdletBinding(DefaultParameterSetName = 'Generic')]
    [OutputType([System.Drawing.FontFamily])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Generic')]
        # The GenericFontFamilies from which to create the new FontFamily.
        [System.Drawing.Text.GenericFontFamilies]$Generic,
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Name')]
        # The name of a TrueType font family installed on the current computer.
        [string]$Name
    )
    <#
        .SYNOPSIS
            Create new FontFamily object.

        .DESCRIPTION
            Initializes a new FontFamily object from a generic font family or from a font family name.
        
        .OUTPUTS
            System.Drawing.FontFamily. Defines a group of type faces having a similar basic design and certain variations in styles.
        
        .EXAMPLE
            # Create generic sans-serif font family
            Get-GenericFontFamily -SansSerif | New-FontFamily;
        
        .EXAMPLE
            # Create generic serif font family
            $FontFamily = New-FontFamily -Generic Serif;
        
        .EXAMPLE
            # Create font families by name
            $FontFamily = ('Times New Roman', 'Arial') | New-FontFamily;
        
        .EXAMPLE
            # Create generic monospace font family from explicit enum value
            $FontFamily = [System.Drawing.Text.GenericFontFamilies]::Monospace | New-FontFamily;
        
        .LINK
            Get-GenericFontFamily
            
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.FontFamily.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.text.genericfontfamilies.aspx
    #>
    
    Process {
        if ($PSCmdlet.ParameterSetName -eq 'Generic') {
            New-Object -TypeName 'System.Drawing.FontFamily' -ArgumentList $Generic;
        } else {
            New-Object -TypeName 'System.Drawing.FontFamily' -ArgumentList $Name;
        }
    }
}

Function New-DrawingFont {
    [CmdletBinding(DefaultParameterSetName = 'FromFamily')]
    [OutputType([System.Drawing.Font])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'FromFamily')]
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'GdiCharSet')]
		[Alias('Family')]
        # The FontFamily of the new Font.
        [System.Drawing.FontFamily]$FontFamily,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'FromFamily')]
        [Parameter(Mandatory = $false, Position = 1, ParameterSetName = 'FromBaseFont')]
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'GdiCharSet')]
        # The em-size of the new font in the units specified by the Unit parameter.
        [float]$Size,
        
        [Parameter(Mandatory = $false, Position = 3, ParameterSetName = 'FromFamily')]
        [Parameter(Mandatory = $true, Position = 3, ParameterSetName = 'GdiCharSet')]
        # The GraphicsUnit of the new font.
        [System.Drawing.GraphicsUnit]$Unit,
        
        [Parameter(Mandatory = $true, Position = 4, ParameterSetName = 'GdiCharSet')]
		[Alias('CharSet')]
        # A Byte that specifies a GDI character set to use for this font.
        [byte]$GdiCharSet,
        
        [Parameter(Mandatory = $false, Position = 5, ParameterSetName = 'GdiCharSet')]
		[Alias('Vertical')]
        # A Boolean value indicating whether the new font is derived from a GDI vertical font.
        [bool]$GdiVerticalFont,
        
        [Parameter(Mandatory = $false, Position = 2, ParameterSetName = 'FromFamily')]
        [Parameter(Mandatory = $false, Position = 2, ParameterSetName = 'FromBaseFont')]
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'GdiCharSet')]
        # The FontStyle to apply to the new Font. Multiple values of the FontStyle enumeration can be combined with the -bor operator.
        [System.Drawing.FontStyle]$Style = [System.Drawing.FontStyle]::Regular,
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'FromBaseFont')]
        [Alias('Base', 'Font')]
        # The existing Font from which to create the new Font.
        [System.Drawing.Font]$BaseFont
    )
    <#
        .SYNOPSIS
            Create new Font object.

        .DESCRIPTION
            Initializes a new Font that uses the specified parameters.
        
        .OUTPUTS
            System.Drawing.Font. Defines a particular format for text, including font face, size, and style attributes.
        
        .EXAMPLE
            # Create generic sans-serif font
            Get-GenericFontFamily -SansSerif | New-FontFamily | New-DrawingFont;
        
        .EXAMPLE
            # Create generic serif font family
            $Font = New-FontFamily -Generic Serif;
        
        .EXAMPLE
            # Create 12-point bold font by name
            $Font = ('Times New Roman', 'Arial') | New-FontFamily | New-DrawingFont -Size 12 -Unit (Get-GraphicsUnit -Pixel) -Style (Get-FontStyle -Bold);
        
        .EXAMPLE
            # Get existing font with 'Italic' and 'Underline' turned on
            $NewFont = $ExistingFont | New-DrawingFont -Style (Get-FontStyle -Italic -Underline);
        
        .LINK
            New-FontFamily
        
        .LINK
            Get-GraphicsUnit
           
        .LINK
            Get-FontStyle
            
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.font.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.fontstyle.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.graphicsunit.aspx
    #>
    
    Process {
        switch ($PSCmdlet.ParameterSetName) {
            'GdiCharSet' {
                if ($PSBoundParameters.ContainsKey('GdiVerticalFont')) {
                    New-Object -TypeName 'System.Drawing.Font' -ArgumentList $FontFamily, $Size, $Style, $Unit, $GdiCharSet, $GdiVerticalFont;
                } else {
                    New-Object -TypeName 'System.Drawing.Font' -ArgumentList $FontFamily, $Size, $Style, $Unit, $GdiCharSet;
                }
            }
            'FromFamily' {
                if ($PSBoundParameters.ContainsKey('Unit')) {
                    New-Object -TypeName 'System.Drawing.Font' -ArgumentList $FontFamily, $Size, $Style, $Unit;
                } else {
                    if ($PSBoundParameters.ContainsKey('Style')) {
                        New-Object -TypeName 'System.Drawing.Font' -ArgumentList $FontFamily, $Size, $Style;
                    } else {
                        New-Object -TypeName 'System.Drawing.Font' -ArgumentList $FontFamily, $Size;
                    }
                }
            }
            default {
                if ($PSBoundParameters.ContainsKey('Style') -and (-not $PSBoundParameters.ContainsKey('Size'))) {
                    New-Object -TypeName 'System.Drawing.Font' -ArgumentList $BaseFont, $Style;
                } else {
                    if ($PSBoundParameters.ContainsKey('Size')) {
                        $EmSize = $Size;
                    } else {
                        $EmSize = $BaseFont.Size;
                    }
                    if ($PSBoundParameters.ContainsKey('Style')) {
                        $FontStyle = $Style;
                    } else {
                        $FontStyle = $BaseFont.Style;
                    }
                    New-Object -TypeName 'System.Drawing.Font' -ArgumentList $BaseFont.FontFamily, $EmSize, $FontStyle, $BaseFont.Unit, $BaseFont.GdiCharSet, $BaseFont.GdiVerticalFont;
                }
            }
        }
    }
}

Function New-FormsPadding {
    [CmdletBinding(DefaultParameterSetName = 'All')]
    [OutputType([System.Windows.Forms.Padding])]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'All')]
        # The alpha component.
        [int]$All,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Explicit')]
        # The red component.
        [int]$Left,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Explicit')]
        # The green component.
        [int]$Top,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Explicit')]
        # The blue component.
        [int]$Right,
        
        [Parameter(Mandatory = $true, Position = 3, ParameterSetName = 'Explicit')]
        # The blue component.
        [int]$Bottom
    )
    <#
        .SYNOPSIS
            Create new Padding structure.

        .DESCRIPTION
            Initializes a new instance of the Padding class using a specified padding size(s)).
        
        .OUTPUTS
            System.Windows.Forms.Padding. Represents padding or margin information associated with a user interface (UI) element.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.padding.aspx
        
        .LINK
            Set-FormControlProperties
    #>
    
	if ($PSCmdlet.ParameterSetName -eq 'All') {
		New-Object -TypeName 'System.Windows.Forms.Padding' -ArgumentList $All;
	} else {
		New-Object -TypeName 'System.Windows.Forms.Padding' -ArgumentList $Left, $Top, $Right, $Bottom;
	}
}
New-Alias -Name 'New-FormsMargin' -Value 'New-FormsPadding' -Scope Global -Force;

Function Show-FormsControl {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose Visible property is to be set.
        [System.Windows.Forms.Control]$Control
	)
    <#
        .SYNOPSIS
            Set Visible to true.

        .DESCRIPTION
            Sets Visible property to true for form controls.
        
        .LINK
            Hide-FormsControl
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
    #>

	Process { $Control.Visible = $true; }
}

Function Hide-FormsControl {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose Visible property is to be set.
        [System.Windows.Forms.Control]$Control
	)
    <#
        .SYNOPSIS
            Set Visible to false.

        .DESCRIPTION
            Sets Visible property to false for form controls.
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            Show-FormsControl
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
    #>

	Process { $Control.Visible = $false; }
}

Function Enable-FormsControl {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose Enabled property is to be set.
        [System.Windows.Forms.Control]$Control
	)
    <#
        .SYNOPSIS
            Set Enabled to true.

        .DESCRIPTION
            Sets Enabled property to true for form controls.
        
        .LINK
            Disable-FormsControl
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
    #>

	Process { $Control.Enabled = $true; }
}

Function Disable-FormsControl {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose Enabled property is to be set.
        [System.Windows.Forms.Control]$Control
	)
    <#
        .SYNOPSIS
            Set Enabled to false.

        .DESCRIPTION
            Sets Enabled property to false for form controls.
        
        .LINK
            Enable-FormsControl
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
    #>

	Process { $Control.Enabled = $false; }
}



Function Set-FormControlProperties {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose properties are to be set.
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $false)]
        # Sets the name of the control.
        [string]$Name,

        [Parameter(Mandatory = $false)]
        # A bitwise combination of the AnchorStyles values.
        [System.Windows.Forms.AnchorStyles]$Anchor,

        [Parameter(Mandatory = $false)]
		[Alias('Background')]
        # Sets the background color of the control.
        [System.Drawing.Color]$BackColor,

        [Parameter(Mandatory = $false)]
        # Determines how the control should be docked within its parent container.
        [System.Windows.Forms.DockStyle]$Dock,

        [Parameter(Mandatory = $false)]
        # Indicates whether the control can respond to user interaction.
        [bool]$Enabled,

        [Parameter(Mandatory = $false)]
        # Sets the font of the text displayed by the control.
        [System.Drawing.Font]$Font,

        [Parameter(Mandatory = $false)]
        [ValidateRange(0, 2147483647)]
        # Sets FontHeight property, which is the height of the font of the control.
        [int]$FontHeight,

        [Parameter(Mandatory = $false)]
		[Alias('Foreground')]
        # Sets the foreground color of the control.
        [System.Drawing.Color]$ForeColor,
        
        [Parameter(Mandatory = $false)]
        # A Point that represents the location of the control.
        [System.Drawing.Point]$Location,
        
        [Parameter(Mandatory = $false)]
        # A Padding representing the space between controls.
        [System.Windows.Forms.Padding]$Margin,
        
        [Parameter(Mandatory = $false)]
		[Alias('MaxSize')]
        # A Padding representing the space between controls.
        [System.Drawing.Size]$MaximumSize,
        
        [Parameter(Mandatory = $false)]
		[Alias('MinSize')]
        # A Size that represents the size of the control.
        [System.Drawing.Size]$MinimumSize,
        
        [Parameter(Mandatory = $false)]
        # A Padding representing the padding within the control.
        [System.Windows.Forms.Padding]$Padding,
        
        [Parameter(Mandatory = $false)]
        # A Size that represents the size of the control.
        [System.Drawing.Size]$Size,
        
        [Parameter(Mandatory = $false)]
        [ValidateRange(0, 2147483647)]
        # Sets the TabIndex Property, which represents the tab order of the control within its container.
        [int]$TabIndex,
        
        [Parameter(Mandatory = $false)]
        # Sets the TabStop property, which indicates whether the user can give the focus to this control using the TAB key.
        [bool]$TabStop,
        
        [Parameter(Mandatory = $false)]
        # Sets the Tag property, which is the object that contains data about the control.
        [object]$Tag,
        
        [Parameter(Mandatory = $false)]
        # Sets the Text property, which is the text associated with the control.
        [string]$Text,
        
        [Parameter(Mandatory = $false)]
        # Sets the Visible property, which indicates whether the control and all its child controls are displayed.
        [bool]$Visible,
        
        [Parameter(Mandatory = $false)]
        # Handle event which occurs when the control is clicked.
        [ScriptBlock]$OnClick,
        
        [Parameter(Mandatory = $false)]
        # Handle event which occurs when the control receives focus.
        [ScriptBlock]$OnGotFocus,
        
        [Parameter(Mandatory = $false)]
        # Handle event which occurs when a key is pressed while the control has focus.
        [ScriptBlock]$OnKeyDown,
        
        [Parameter(Mandatory = $false)]
        # Handle event which occurs when a character, space or backspace key is pressed while the control has focus.
        [ScriptBlock]$OnKeyPress,
        
        [Parameter(Mandatory = $false)]
        # Handle event which occurs when a key is released while the control has focus.
        [ScriptBlock]$OnKeyUp,
        
        [Parameter(Mandatory = $false)]
        # Handle event which occurs when the control loses focus.
        [ScriptBlock]$OnLostFocus,
        
        [Parameter(Mandatory = $false)]
        # Handle event which occurs when the control is clicked by the mouse.
        [ScriptBlock]$OnMouseClick,
        
        [Parameter(Mandatory = $false)]
        # Handle event which occurs when the Text property value changes.
        [ScriptBlock]$OnTextChanged
    )
    <#
        .SYNOPSIS
            Set common form control properties.

        .DESCRIPTION
            Sets common properties for form controls.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
        
        .LINK
            Set-FormControlAnchor
        
        .LINK
            Set-FormControlBackColor
        
        .LINK
            Set-FormControlDock
        
        .LINK
            Set-FormControlFont
            
        .LINK
            Set-FormControlForeColor
			
        .LINK
			Set-FormControlLocation
            
        .LINK
			Set-FormControlMargin
            
        .LINK
			Set-MaximumFormControlSize
			
        .LINK
			Set-MinimumFormControlSize
			
        .LINK
			Set-FormControlPadding
            
        .LINK
			Set-FormControlSize
			
        .LINK
			Set-FormControlOnClick
			
        .LINK
			Set-FormControlOnGotFocus
			
        .LINK
			Set-FormControlOnKeyDown
			
        .LINK
			Set-FormControlOnKeyPress
			
        .LINK
			Set-FormControlOnKeyUp
			
        .LINK
			Set-FormControlOnLostFocus
			
        .LINK
			Set-FormControlOnMouseClick
			
        .LINK
			Set-FormControlOnTextChanged
			
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.anchorstyles.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.color.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.dockstyle.aspx
            
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.font.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.padding.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.size.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.point.aspx
    #>
    
    Process {
        if ($PSBoundParameters.ContainsKey('Name')) { $Control.Name = $Name }
        if ($PSBoundParameters.ContainsKey('Anchor')) { $Control.Anchor = $Anchor }
        if ($PSBoundParameters.ContainsKey('BackColor')) { $Control.BackColor = $BackColor }
        if ($PSBoundParameters.ContainsKey('Dock')) { $Control.Dock = $Dock }
        if ($PSBoundParameters.ContainsKey('Enabled')) { $Control.Dock = $Dock }
        if ($PSBoundParameters.ContainsKey('Font')) { $Control.Font = $Font }
        if ($PSBoundParameters.ContainsKey('FontHeight')) { $Control.FontHeight = $FontHeight }
        if ($PSBoundParameters.ContainsKey('ForeColor')) { $Control.ForeColor = $ForeColor }
        if ($PSBoundParameters.ContainsKey('Location')) { $Control.Location = $Location }
        if ($PSBoundParameters.ContainsKey('Margin')) { $Control.Margin = $Margin }
        if ($PSBoundParameters.ContainsKey('MaximumSize')) { $Control.MaximumSize = $MaximumSize }
        if ($PSBoundParameters.ContainsKey('MinimumSize')) { $Control.MinimumSize = $MinimumSize }
        if ($PSBoundParameters.ContainsKey('Padding')) { $Control.Padding = $Padding }
        if ($PSBoundParameters.ContainsKey('Size')) { $Control.Size = $Size }
        if ($PSBoundParameters.ContainsKey('TabIndex')) { $Control.TabIndex = $TabIndex }
        if ($PSBoundParameters.ContainsKey('TabStop')) { $Control.TabStop = $TabStop }
        if ($PSBoundParameters.ContainsKey('Tag')) { $Control.Tag = $Tag }
        if ($PSBoundParameters.ContainsKey('Text')) { $Control.Text = $Text }
        if ($PSBoundParameters.ContainsKey('Visible')) { $Control.Visible = $Visible }
        if ($PSBoundParameters.ContainsKey('OnClick')) { $Button.add_Click($OnClick) }
        if ($PSBoundParameters.ContainsKey('OnGotFocus')) { $Button.add_GotFocus($OnGotFocus) }
        if ($PSBoundParameters.ContainsKey('OnKeyDown')) { $Button.add_KeyDown($OnKeyDown) }
        if ($PSBoundParameters.ContainsKey('OnKeyPress')) { $Button.add_KeyPress($OnKeyPress) }
        if ($PSBoundParameters.ContainsKey('OnKeyUp')) { $Button.add_KeyUp($OnKeyUp) }
        if ($PSBoundParameters.ContainsKey('OnLostFocus')) { $Button.add_LostFocus($OnLostFocus) }
        if ($PSBoundParameters.ContainsKey('OnMouseClick')) { $Button.add_MouseClick($OnMouseClick) }
        if ($PSBoundParameters.ContainsKey('OnTextChanged')) { $Button.add_TextChanged($OnTextChanged) }
    }
}

Function Set-ScrollableControlProperties {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The scrollable control whose properties are to be set.
        [System.Windows.Forms.ScrollableControl]$Control,
        
        [Parameter(Mandatory = $false)]
        # Sets a value indicating whether the container enables the user to scroll to any controls placed outside of its visible boundaries.
        [bool]$AutoScroll,
        
        [Parameter(Mandatory = $false)]
        # Sets the size of the auto-scroll margin in pixels.
        [System.Drawing.Size]$AutoScrollMargin,
        
        [Parameter(Mandatory = $false)]
        # determines the minimum size of the virtual area through which the user can scroll.
        [System.Drawing.Size]$AutoScrollMinSize,
        
        [Parameter(Mandatory = $false)]
        # Sets the location of the auto-scroll position, in pixels.
        [System.Drawing.Point]$AutoScrollPosition
    )
    <#
        .SYNOPSIS
            Set common scrollable control properties.

        .DESCRIPTION
            Sets common properties for scrollable controls.
        
        .LINK
			New-DrawingSize
			
        .LINK
			New-DrawingPoint
        
        .LINK
            Set-FormControlProperties

        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.scrollablecontrol.aspx
			
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.size.aspx
			
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.point.aspx
	#>
    
    Process {
        if ($PSBoundParameters.ContainsKey('AutoScroll')) { $Control.AutoScroll = $AutoScroll }
        if ($PSBoundParameters.ContainsKey('AutoScrollMargin')) { $Control.AutoScrollMargin = $AutoScrollMargin }
        if ($PSBoundParameters.ContainsKey('AutoScrollMinSize')) { $Control.AutoScrollMinSize = $AutoScrollMinSize }
        if ($PSBoundParameters.ContainsKey('AutoScrollPosition')) { $Control.AutoScrollPosition = $AutoScrollPosition }
    }
}

Function Set-FormButtonProperties {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The button whose properties are to be set
        [System.Windows.Forms.Button]$Button,
        
        [Parameter(Mandatory = $false)]
		[Alias('Result')]
        # Sets a value that is returned to the parent form when the button is clicked.
        [System.Windows.Forms.DialogResult]$DialogResult,
        
        [Parameter(Mandatory = $false)]
        # Indicates whether the control can respond to user interaction.
        [bool]$Enabled,

        [Parameter(Mandatory = $false)]
        # Sets the Tag property, which is the object that contains data about the control.
        [object]$Tag,
        
        [Parameter(Mandatory = $false)]
        # Sets the Text property, which is the text associated with the control.
        [string]$Text,
        
        [Parameter(Mandatory = $false)]
        # Sets the Visible property, which indicates whether the control and all its child controls are displayed.
        [bool]$Visible,
        
        [Parameter(Mandatory = $false)]
        # Handle event which occurs when the button is clicked.
        [ScriptBlock]$OnClick,
        
        [Parameter(Mandatory = $false)]
        # Handle event which occurs when the user double-clicks the Button control.
        [ScriptBlock]$OnDoubleClick,
        
        [Parameter(Mandatory = $false)]
        # Handle event which occurs when the button is clicked.
        [ScriptBlock]$OnMouseClick,
        
        [Parameter(Mandatory = $false)]
        # Handle event which occurs when the user double-clicks the Button control with the mouse.
        [ScriptBlock]$OnMouseDoubleClick
    )
    <#
        .SYNOPSIS
            Set common button control properties.

        .DESCRIPTION
            Sets common properties for button controls.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.button.aspx
			
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.dialogresult.aspx
			
        .LINK
			Set-FormControlOnClick
			
        .LINK
			Set-ButtonControlOnDoubleClick
        
        .LINK
			Set-FormControlOnMouseClick
			
        .LINK
			Set-ButtonControlOnMouseDoubleClick
        
        .LINK
            Set-FormControlProperties
    #>
    
    Process {
        if ($PSBoundParameters.ContainsKey('DialogResult')) { $Button.DialogResult = $DialogResult }
        if ($PSBoundParameters.ContainsKey('Enabled')) { $Button.Enabled = $Enabled }
        if ($PSBoundParameters.ContainsKey('Tag')) { $Button.Tag = $Tag }
        if ($PSBoundParameters.ContainsKey('Text')) { $Button.Text = $Text }
        if ($PSBoundParameters.ContainsKey('Visible')) { $Button.Visible = $Visible }
        if ($PSBoundParameters.ContainsKey('OnClick')) { $Button.add_Click($OnClick) }
        if ($PSBoundParameters.ContainsKey('OnDoubleClick')) { $Button.add_DoubleClick($OnDoubleClick) }
        if ($PSBoundParameters.ContainsKey('OnMouseClick')) { $Button.add_MouseClick($OnMouseClick) }
        if ($PSBoundParameters.ContainsKey('OnMouseDoubleClick')) { $Button.add_MouseDoubleClick($OnMouseDoubleClick) }
    }
}

Function Set-FormControlAnchor {
    [CmdletBinding(DefaultParameterSetName = 'Switches')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The form whose anchor setting is to be applied
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Enum')]
        # A bitwise combination of the AnchorStyles values.
        [System.Windows.Forms.AnchorStyles]$Anchor,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'Switches')]
        # The form is not displayed modally.
        [switch]$Bottom,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'Switches')]
        # The form is not displayed modally.
        [switch]$Left,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'Switches')]
        # The form is not displayed modally.
        [switch]$Right,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'Switches')]
        # The form is not displayed modally.
        [switch]$Top,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'None')]
        # The form is not displayed modally.
        [switch]$None
    )
    <#
        .SYNOPSIS
            Set button Anchor property.

        .DESCRIPTION
            Sets the edges of the container to which a control is bound and determines how a control is resized with its parent. The default is Top and Left.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.anchorstyles.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
        
        .NOTES
            Use the Anchor property to define how a control is automatically resized as its parent control is resized.
            Anchoring a control to its parent control ensures that the anchored edges remain in the same position relative to the edges of the parent control when the parent control is resized.
            
            You can anchor a control to one or more edges of its container.
            For example, if you have a Form with a Button whose Anchor property value is set to Top and Bottom,
            the Button is stretched to maintain the anchored distance to the top and bottom edges of the Form as the Height of the Form is increased.

    #>
    
    Begin {
        switch ($PSCmdlet.ParameterSetName) {
            'Enum' { $AnchorStyles = $Anchor; break; }
            'None' { $AnchorStyles = [System.Windows.Forms.AnchorStyles]::None; break; }
            default {
                $AnchorStyles = [System.Windows.Forms.AnchorStyles]::None;
                $PSBoundParameters.Keys | ForEach-Object {
                    if ($_ -ne 'Control') { [System.Windows.Forms.AnchorStyles]$AnchorStyles = $AnchorStyles -bor [System.Enum]::Parse([System.Windows.Forms.AnchorStyles], $_) }
                }
            }
        }
    }
    
    Process { $Control.Anchor = $AnchorStyles }
}

Function Set-FormControlBackColor {
    [CmdletBinding(DefaultParameterSetName = 'Switches')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The form whose background color setting is to be applied
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'BackColor')]
		[Alias('Background', 'Color')]
        # The Color to use for the form control background.
        [System.Drawing.Color]$BackColor,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'FromArgb')]
        [ValidateRange(0, 255)]
        [Alias('R')]
        # The red component.
        [int]$Red,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'FromArgb')]
        [ValidateRange(0, 255)]
        [Alias('G')]
        # The green component.
        [int]$Green,
        
        [Parameter(Mandatory = $true, Position = 3, ParameterSetName = 'FromArgb')]
        [ValidateRange(0, 255)]
        [Alias('B')]
        # The blue component.
        [int]$Blue,
        
        [Parameter(Mandatory = $false, Position = 4, ParameterSetName = 'FromArgb')]
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'ApplyAlpha')]
        [ValidateRange(0, 255)]
        [Alias('A')]
        # The alpha component.
        [int]$Alpha,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'FromName')]
        # A string that is the name of a predefined color. Valid names are the same as the names of the elements of the KnownColor enumeration.
        [string]$Name,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'FromKnownColor')]
        [Alias('Known')]
        # An element of the KnownColor enumeration.
        [System.Drawing.KnownColor]$KnownColor,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'ApplyAlpha')]
        [Alias('Base')]
        # The base Color to apply to the form control background.
        [System.Drawing.Color]$BaseColor
    )
    <#
        .SYNOPSIS
            Set control BackColor property.

        .DESCRIPTION
            Sets the background color of the control.
        
        .LINK
            New-DrawingColor
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.color.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.knowncolor.aspx
        
        .NOTES
            Use the Anchor property to define how a control is automatically resized as its parent control is resized.
            Anchoring a control to its parent control ensures that the anchored edges remain in the same position relative to the edges of the parent control when the parent control is resized.
            
            You can anchor a control to one or more edges of its container.
            For example, if you have a Form with a Button whose Anchor property value is set to Top and Bottom,
            the Button is stretched to maintain the anchored distance to the top and bottom edges of the Form as the Height of the Form is increased.

    #>
    
    Begin {
        if ($PSCmdlet.ParameterSetName -eq 'BackColor') {
            $Color = $BackColor;
        } else {
            $splat = @{};
            $PSBoundParameters.Keys | ForEach-Object {
                if ($_ -ne 'Control') { $splat.Add($_,  $PSBoundParameters[$_]) }
            }
            $Color = New-DrawingColor @splat;
        }
    }
    
    Process { $Control.BackColor = $Color }
}

Function Set-FormControlDock {
    [CmdletBinding(DefaultParameterSetName = 'None')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # Control whose dock style is to be set
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Enum')]
        # Determines how the control should be docked within its parent container.
        [System.Windows.Forms.DockStyle]$Dock,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'None')]
        # The control is not docked.
        [switch]$None,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Top')]
        # The control's top edge is docked to the top of its containing control
        [switch]$Top,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Bottom')]
        # The control's bottom edge is docked to the bottom of its containing control.
        [switch]$Bottom,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Left')]
        # The control's left edge is docked to the left edge of its containing control.
        [switch]$Left,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Right')]
        # The control's right edge is docked to the right edge of its containing control.
        [switch]$Right,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Fill')]
        # All the control's edges are docked to the all edges of its containing control and sized appropriately.
        [switch]$Fill
    )
    <#
        .SYNOPSIS
            Sets control Dock property.

        .DESCRIPTION
            Determines how a control should be docked within its parent container.
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.dockstyle.aspx
            
        .NOTES
            When a control is docked to an edge of its container, it is always positioned flush against that edge when the container is resized.
            If more than one control is docked to an edge, the controls appear side by side according to their z-order;
            controls higher in the z-order are positioned farther from the container's edge.
            
            If Left, Right, Top, or Bottom is selected, the specified and opposite edges of the control are resized to the size of the containing control's corresponding edges.
            If Fill is selected, all four sides of the control are resized to match the containing control's edges.

    #>
    
    Begin {
        if ($PSCmdlet.ParameterSetName -eq 'Enum') {
            $DockStyle = $Dock;
        } else {
            $DockStyle = [System.Enum]::Parse([System.Windows.Forms.DockStyle], $PSCmdlet.ParameterSetName);
        }
    }
    
    Process { $Control.Dock = $DockStyle }
}

Function Set-FormControlFont {
    [CmdletBinding(DefaultParameterSetName = 'Switches')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The form whose font setting is to be applied
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'FromFont')]
        # The Font to apply to the text displayed by the control.
        [System.Drawing.Font]$Font,
        
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'FromFamily')]
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, ParameterSetName = 'GdiCharSet')]
		[Alias('Family')]
        # The FontFamily of the new Font.
        [System.Drawing.FontFamily]$FontFamily,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'FromFamily')]
        [Parameter(Mandatory = $false, ParameterSetName = 'FromFont')]
        [Parameter(Mandatory = $true, ParameterSetName = 'GdiCharSet')]
        # The em-size of the new font in the units specified by the Unit parameter.
        [float]$Size,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'FromFamily')]
        [Parameter(Mandatory = $true, ParameterSetName = 'GdiCharSet')]
        # The GraphicsUnit of the new font.
        [System.Drawing.GraphicsUnit]$Unit,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'GdiCharSet')]
		[Alias('CharSet')]
        # A Byte that specifies a GDI character set to use for this font.
        [byte]$GdiCharSet,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'GdiCharSet')]
		[Alias('Vertical')]
        # A Boolean value indicating whether the new font is derived from a GDI vertical font.
        [bool]$GdiVerticalFont,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'FromFamily')]
        [Parameter(Mandatory = $false, ParameterSetName = 'FromFont')]
        [Parameter(Mandatory = $true, ParameterSetName = 'GdiCharSet')]
        # The FontStyle to apply to the new Font. Multiple values of the FontStyle enumeration can be combined with the -bor operator.
        [System.Drawing.FontStyle]$Style = [System.Drawing.FontStyle]::Regular
    )
    <#
        .SYNOPSIS
            Set control Font property.

        .DESCRIPTION
            Sets the font to apply to the text displayed by the control.
        
        .EXAMPLE
            # Set generic sans-serif font
            Get-GenericFontFamily -SansSerif | New-FontFamily | New-DrawingFont;
        
        .EXAMPLE
            # Create generic serif font family
            $Font = New-FontFamily -Generic Serif;
        
        .EXAMPLE
            # Create 12-point bold font by name
            $Font = ('Times New Roman', 'Arial') | New-FontFamily | New-DrawingFont -Size 12 -Unit (Get-GraphicsUnit -Pixel) -Style (Get-FontStyle -Bold);
        
        .EXAMPLE
            # Get existing font with 'Italic' and 'Underline' turned on
            $NewFont = $ExistingFont | New-DrawingFont -Style (Get-FontStyle -Italic -Underline);
        
        .LINK
            New-FormsFont
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.color.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.knowncolor.aspx
    #>
    
    Begin {
        if ($PSCmdlet.ParameterSetName -eq 'FromFont' -and (-not ($PSBoundParameters.ContainsKey('Size') -or $PSBoundParameters.ContainsKey('Style')))) {
            $ControlFont = $Font;
        } else {
            $splat = @{};
            $PSBoundParameters.Keys | ForEach-Object {
                if ($_ -ne 'Control') { $splat.Add($_,  $PSBoundParameters[$_]) }
            }
            $ControlFont = New-FormsFont @splat;
        }
    }
    
    Process { $Control.Font = $ControlFont }
}

Function Set-FormControlForeColor {
    [CmdletBinding(DefaultParameterSetName = 'Switches')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The form whose foreground color setting is to be applied
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'ForeColor')]
        [Alias('Color', 'Foreground')]
        # The Color to use for the form control foreground.
        [System.Drawing.Color]$ForeColor,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'FromArgb')]
        [ValidateRange(0, 255)]
        [Alias('R')]
        # The red component.
        [int]$Red,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'FromArgb')]
        [ValidateRange(0, 255)]
        [Alias('G')]
        # The green component.
        [int]$Green,
        
        [Parameter(Mandatory = $true, Position = 3, ParameterSetName = 'FromArgb')]
        [ValidateRange(0, 255)]
        [Alias('B')]
        # The blue component.
        [int]$Blue,
        
        [Parameter(Mandatory = $false, Position = 4, ParameterSetName = 'FromArgb')]
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'ApplyAlpha')]
        [ValidateRange(0, 255)]
        [Alias('A')]
        # The alpha component.
        [int]$Alpha,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'FromName')]
        # A string that is the name of a predefined color. Valid names are the same as the names of the elements of the KnownColor enumeration.
        [string]$Name,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'FromKnownColor')]
        [Alias('Known')]
        # An element of the KnownColor enumeration.
        [System.Drawing.KnownColor]$KnownColor,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'ApplyAlpha')]
        [Alias('Base')]
        # The base Color to apply to the form control foreground.
        [System.Drawing.Color]$BaseColor
    )
    <#
        .SYNOPSIS
            Set control ForeColor property.

        .DESCRIPTION
            Sets the foreground color of the control.
        
        .LINK
            New-DrawingColor
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.color.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.knowncolor.aspx
        
        .LINK
            Set-FormControlProperties
    #>
    
    Begin {
        if ($PSCmdlet.ParameterSetName -eq 'ForeColor') {
            $Color = $ForeColor;
        } else {
            $splat = @{};
            $PSBoundParameters.Keys | ForEach-Object {
                if ($_ -ne 'Control') { $splat.Add($_,  $PSBoundParameters[$_]) }
            }
            $Color = New-DrawingColor @splat;
        }
    }
    
    Process { $Control.BackColor = $Color }
}

Function Set-FormControlLocation {
    [CmdletBinding(DefaultParameterSetName = 'Location')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The form whose size is to be set
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
		[Alias('Horizontal')]
        # The horizontal position of the control.
        [int]$X,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Values')]
		[Alias('Vertical')]
        # The vertical position of the control.
        [string]$Y,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Location')]
        # A Point that represents the location of the control.
        [System.Drawing.Point]$Location
    )
    <#
        .SYNOPSIS
            Set control Location property.

        .DESCRIPTION
            Sets the coordinates of the upper-left corner of the control relative to the upper-left corner of its container.
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            New-DrawingPoint
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.point.aspx
    #>
    
    Begin {
        if ($PSBoundParameters.ContainsKey('Location')) {
            $Point = $Location;
        } else {
            $Point = New-DrawingSize -Width $Width -Height $Height;
        }
    }
    
    Process { $Control.Location = $Point; }
}

Function Set-FormControlMargin {
    [CmdletBinding(DefaultParameterSetName = 'All')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose margin is to be set
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'All')]
        # The margin size, in pixels, for all edges.
        [int]$All,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Explicit')]
        # The margin size, in pixels, for the left edge.
        [int]$Left,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Explicit')]
        # The margin size, in pixels, for the top edge.
        [int]$Top,
        
        [Parameter(Mandatory = $true, Position = 3, ParameterSetName = 'Explicit')]
        # The margin size, in pixels, for the right edge.
        [int]$Right,
        
        [Parameter(Mandatory = $true, Position = 4, ParameterSetName = 'Explicit')]
        # The margin size, in pixels, for the bottom edge.
        [int]$Bottom,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Margin')]
        # A Padding representing the space between controls.
        [System.Windows.Forms.Padding]$Margin
    )
    <#
        .SYNOPSIS
            Set control Margin property.

        .DESCRIPTION
            Sets the space between controls.
        
        .LINK
            New-FormsMargin
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.padding.aspx
    #>
    
    Begin {
        if ($PSBoundParameters.ContainsKey('Margin')) {
            $Padding = $Margin;
        } else {
			$splat = @{};
			$PSBoundParameters.Keys | ForEach-Object { if ($_ -ne 'Control') { $splat.Add($_, $PSBoundParameters[$_]) } }
			$Padding = New-FormsMargin @splat;
        }
    }
    Process { $Control.Margin = $Padding; }
}

Function Set-MaximumFormControlSize {
    [CmdletBinding(DefaultParameterSetName = 'Size')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose maximum size is to be set
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
        # The maximum width of the control.
        [int]$Width,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Values')]
        # The maximum height of the control.
        [string]$Height,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Size')]
        # A Size that represents the maximum size of the control.
        [System.Drawing.Size]$Size
    )
    <#
        .SYNOPSIS
            Set control MaximumSize property.

        .DESCRIPTION
            Sets the maximum size the control can be resized to.
        
        .LINK
            New-DrawingSize
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.size.aspx
    #>
    
    Begin {
        if ($PSBoundParameters.ContainsKey('Width')) {
            $MaximumSize = $Size;
        } else {
            $MaximumSize = New-DrawingSize -Width $Width -Height $Height;
        }
    }
    Process { $Control.MaximumSize = $MaximumSize; }
}

Function Set-MinimumFormControlSize {
    [CmdletBinding(DefaultParameterSetName = 'Size')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose minimum size is to be set
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
        # The minimum width of the control.
        [int]$Width,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Values')]
        # The minimum height of the control.
        [string]$Height,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Size')]
        # A Size that represents the minimum size of the control.
        [System.Drawing.Size]$Size
    )
    <#
        .SYNOPSIS
            Set control MinimumSize property.

        .DESCRIPTION
            Sets the minimum size the control can be resized to.
        
        .LINK
            New-DrawingSize
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.size.aspx
    #>
    
    Begin {
        if ($PSBoundParameters.ContainsKey('Width')) {
            $MinimumSize = $Size;
        } else {
            $MinimumSize = New-DrawingSize -Width $Width -Height $Height;
        }
    }
    Process { $Control.MinimumSize = $MinimumSize; }
}

Function Set-FormControlPadding {
    [CmdletBinding(DefaultParameterSetName = 'All')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose padding is to be set
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'All')]
        # The padding size, in pixels, for all edges.
        [int]$All,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Explicit')]
        # The padding size, in pixels, for the left edge.
        [int]$Left,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Explicit')]
        # The padding size, in pixels, for the top edge.
        [int]$Top,
        
        [Parameter(Mandatory = $true, Position = 3, ParameterSetName = 'Explicit')]
        # The padding size, in pixels, for the right edge.
        [int]$Right,
        
        [Parameter(Mandatory = $true, Position = 4, ParameterSetName = 'Explicit')]
        # The padding size, in pixels, for the bottom edge.
        [int]$Bottom,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Padding')]
        # A Padding representing the space between controls.
        [System.Windows.Forms.Padding]$Padding
    )
    <#
        .SYNOPSIS
            Set control Padding property.

        .DESCRIPTION
            Sets the padding within the control.
        
        .LINK
            New-FormsPadding
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.padding.aspx
    #>
    
    Begin {
        if ($PSBoundParameters.ContainsKey('Padding')) {
            $Padding = $Padding;
        } else {
			$splat = @{};
			$PSBoundParameters.Keys | ForEach-Object { if ($_ -ne 'Control') { $splat.Add($_, $PSBoundParameters[$_]) } }
			$Padding = New-FormsPadding @splat;
        }
    }
    Process { $Control.Padding = $Padding; }
}

Function Set-FormControlSize {
    [CmdletBinding(DefaultParameterSetName = 'Size')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose size is to be set
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
        # The width of the control.
        [int]$Width,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Values')]
        # The height of the control.
        [string]$Height,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Size')]
        # A Size that represents the size of the control.
        [System.Drawing.Size]$Size
    )
    <#
        .SYNOPSIS
            Set control Size property.

        .DESCRIPTION
            Sets the size of the control.
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            New-DrawingSize
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.size.aspx
    #>
    
    Begin {
        if ($PSBoundParameters.ContainsKey('Size')) {
            $ControlSize = $Size;
        } else {
            $ControlSize = New-DrawingSize -Width $Width -Height $Height;
        }
    }
    Process { $Control.Size = $ControlSize; }
}

Function Set-FormControlOnClick {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose Click event handler is to be set
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
        # ScriptBlock which handles the event that occurs when the button is clicked.
        [ScriptBlock]$OnClick
    )
    <#
        .SYNOPSIS
            Set Click event handler.

        .DESCRIPTION
            Sets event handler for the OnClick event, which gets invoked when the button is clicked.
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
		
		.NOTES
			The control which is the target of the event will be the first parameter when $OnClick is invoked.
    #>
    
    Process { $Control.add_Click($OnClick) }
}
			
Function Set-FormControlOnGotFocus {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose Click event handler is to be set
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
        # ScriptBlock which handles the event that occurs when the control receives focus.
        [ScriptBlock]$OnGotFocus
    )
    <#
        .SYNOPSIS
            Set GotFocus event handler.

        .DESCRIPTION
            Sets event handler for the OnGotFocus event, which gets invoked when the control receives focus.
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
		
		.NOTES
			The control which is the target of the event will be the first parameter when $OnGotFocus is invoked.
    #>
    
    Process { $Control.add_GotFocus($OnGotFocus) }
}
			
Function Set-FormControlOnKeyDown {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose KeyDown event handler is to be set
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
        # ScriptBlock which handles the event that occurs when a key is pressed while the control has focus.
        [ScriptBlock]$OnKeyDown
    )
    <#
        .SYNOPSIS
            Set KeyDown event handler.

        .DESCRIPTION
            Sets event handler for the OnKeyDown event, which gets invoked when a key is pressed while the control has focus.
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.keyeventargs.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
		
		.NOTES
			The control which is the target of the event will be the first parameter when $OnKeyDown is invoked.
			A [System.Windows.Forms.KeyEventArgs] value will be the second parameter.
    #>
    
    Process { $Control.add_KeyDown($OnKeyDown) }
}
			
Function Set-FormControlOnKeyPress {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose KeyPress event handler is to be set
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
        # ScriptBlock which handles the event that occurs when a character, space or backspace key is pressed while the control has focus.
        [ScriptBlock]$OnKeyPress
    )
    <#
        .SYNOPSIS
            Set KeyPress event handler.

        .DESCRIPTION
            Sets event handler for the OnKeyPress event, which gets invoked when a character, space or backspace key is pressed while the control has focus.
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.keypresseventargs.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
		
		.NOTES
			The control which is the target of the event will be the first parameter when $OnKeyPress is invoked.
			A [System.Windows.Forms.KeyPressEventArgs] value will be the second parameter.
    #>
    
    Process { $Control.add_KeyPress($OnKeyPress) }
}
			
Function Set-FormControlOnKeyUp {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose KeyUp event handler is to be set
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
        # ScriptBlock which handles the event that occurs when a key is released while the control has focus.
        [ScriptBlock]$OnKeyUp
    )
    <#
        .SYNOPSIS
            Set KeyUp event handler.

        .DESCRIPTION
            Sets event handler for the OnKeyUp event, which gets invoked when a key is released while the control has focus.
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.keyeventargs.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
		
		.NOTES
			The control which is the target of the event will be the first parameter when $OnKeyUp is invoked.
			A [System.Windows.Forms.KeyEventArgs] value will be the second parameter.
    #>
    
    Process { $Control.add_KeyUp($OnKeyUp) }
}
			
Function Set-FormControlOnLostFocus {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose LostFocus event handler is to be set
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
        # ScriptBlock which handles the event that occurs when the control loses focus.
        [ScriptBlock]$OnLostFocus
    )
    <#
        .SYNOPSIS
            Set LostFocus event handler.

        .DESCRIPTION
            Sets event handler for the OnLostFocus event, which gets invoked when the control loses focus.
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
		
		.NOTES
			The control which is the target of the event will be the first parameter when $OnLostFocus is invoked.
    #>
    
    Process { $Control.add_LostFocus($OnLostFocus) }
}
			
Function Set-FormControlOnMouseClick {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose MouseClick event handler is to be set
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
        # ScriptBlock which handles the event that occurs when the control is clicked by the mouse.
        [ScriptBlock]$OnMouseClick
    )
    <#
        .SYNOPSIS
            Set MouseClick event handler.

        .DESCRIPTION
            Sets event handler for the OnMouseClick event, which gets invoked when the control is clicked by the mouse.
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.mouseeventargs.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
		
		.NOTES
			The control which is the target of the event will be the first parameter when $OnMouseClick is invoked.
			A [System.Windows.Forms.MouseEventArgs] value will be the second parameter.
    #>
    
    Process { $Control.add_MouseClick($OnMouseClick) }
}
			
Function Set-FormControlOnTextChanged {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The control whose TextChanged event handler is to be set
        [System.Windows.Forms.Control]$Control,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
        # ScriptBlock which handles the event that occurs when the Text property value changes.
        [ScriptBlock]$OnTextChanged
    )
    <#
        .SYNOPSIS
            Set TextChanged event handler.

        .DESCRIPTION
            Sets event handler for the OnTextChanged event, gets invoked when the Text property value changes.
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
		
		.NOTES
			The control which is the target of the event will be the first parameter when $OnTextChanged is invoked.
    #>
    
    Process { $Control.add_TextChanged($OnTextChanged) }
}

Function Set-ButtonControlOnDoubleClick {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The button whose DoubleClick event handler is to be set
        [System.Windows.Forms.Button]$Button,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
        # ScriptBlock which handles the event that occurs when the user double-clicks the Button control.
        [ScriptBlock]$OnDoubleClick
    )
    <#
        .SYNOPSIS
            Set DoubleClick event handler.

        .DESCRIPTION
            Sets event handler for the OnDoubleClick event, gets invoked when the user double-clicks the Button control.
        
        .LINK
            New-FormsButton
        
        .LINK
            Set-FormButtonProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.mouseeventargs.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.button.aspx
		
		.NOTES
			The button which is the target of the event will be the first parameter when $OnDoubleClick is invoked.
			A [System.Windows.Forms.MouseEventArgs] value will be the second parameter.
    #>
    
    Process { $Button.add_DoubleClick($OnDoubleClick) }
}
        
Function Set-ButtonControlOnMouseDoubleClick {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The button whose MouseDoubleClick event handler is to be set
        [System.Windows.Forms.Button]$Button,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
        # ScriptBlock which handles the event that occurs when the user double-clicks the Button control with the mouse.
        [ScriptBlock]$OnMouseDoubleClick
    )
    <#
        .SYNOPSIS
            Set MouseDoubleClick event handler.

        .DESCRIPTION
            Sets event handler for the OnMouseDoubleClick event, gets invoked when the user double-clicks the Button control with the mouse.
        
        .LINK
            New-FormsButton
        
        .LINK
            Set-FormButtonProperties
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.button.aspx
		
		.NOTES
			The button which is the target of the event will be the first parameter when $OnMouseDoubleClick is invoked.
    #>
    
    Process { $Button.add_MouseDoubleClick($OnMouseDoubleClick) }
}

Function New-FormsButton {
    [CmdletBinding()]
    [OutputType([System.Windows.Forms.Button])]
    Param(
        [Parameter(Mandatory = $true)]
        [ValidatePattern('^[\S]+$')]
        # Name of the button.
        [string]$Name,
        
        [Parameter(Mandatory = $true)]
        # Sets the Text property, which is the text associated with the control.
        [string]$Text,
        
        [Parameter(Mandatory = $false)]
		[Alias('Result')]
        # Sets a value that is returned to the parent form when the button is clicked.
        [System.Windows.Forms.DialogResult]$DialogResult,

        [Parameter(Mandatory = $false)]
        # Sets the Tag property, which is the object that contains data about the control.
        [object]$Tag,
        
        [Parameter(Mandatory = $false)]
        # Handle event which occurs when the button is clicked.
        [ScriptBlock]$OnClick,
        
        [Parameter(Mandatory = $false)]
        # Handle event which occurs when the user double-clicks the Button control.
        [ScriptBlock]$OnDoubleClick,
        
        [Parameter(Mandatory = $false)]
        # Handle event which occurs when the button is clicked.
        [ScriptBlock]$OnMouseClick,
        
        [Parameter(Mandatory = $false)]
        # Handle event which occurs when the user double-clicks the Button control with the mouse.
        [ScriptBlock]$OnMouseDoubleClick,

        [Parameter(Mandatory = $false)]
        # A bitwise combination of the AnchorStyles values.
        [System.Windows.Forms.AnchorStyles]$Anchor,

        [Parameter(Mandatory = $false)]
        # Determines how the control should be docked within its parent container.
        [System.Windows.Forms.DockStyle]$Dock,

        [Parameter(Mandatory = $false)]
        # A Point that represents the location of the control.
        [System.Drawing.Point]$Location,
        
        [Parameter(Mandatory = $false)]
        # A Padding representing the space between controls.
        [System.Windows.Forms.Padding]$Margin,
        
        [Parameter(Mandatory = $false)]
        # A Size that represents the size of the control.
        [System.Drawing.Size]$Size,
        
        [Parameter(Mandatory = $false)]
        [ValidateRange(0, 2147483647)]
        # Sets the TabIndex Property, which represents the tab order of the control within its container.
        [int]$TabIndex,
        
        [Parameter(Mandatory = $false)]
        # Sets the TabStop property, which indicates whether the user can give the focus to this control using the TAB key.
        [bool]$TabStop,
        
        [Parameter(Mandatory = $false)]
        # Sets the Visible property to false.
        [switch]$NotVisible,
        
        [Parameter(Mandatory = $false)]
        # The control cannot respond to user interaction.
        [switch]$Disabled
    )
    <#
        .SYNOPSIS
            Create new Button control.

        .DESCRIPTION
            Initializes a new instance of the Button class.
        
        .OUTPUTS
            System.Windows.Forms.Button. Represents a Windows button control.
        
        .LINK
            Set-FormButtonProperties
        
        .LINK
            Set-FormControlProperties
        
        .LINK
            Set-FormAcceptButton
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.button.aspx
        
        .LINK
            Set-FormControlAnchor
        
        .LINK
            Set-FormControlDock
        
        .LINK
			Set-FormControlLocation
            
        .LINK
			Set-FormControlMargin
            
        .LINK
			Set-FormControlSize
			
        .LINK
			Set-FormControlOnClick
			
        .LINK
			Set-ButtonControlOnDoubleClick
        
        .LINK
			Set-FormControlOnMouseClick
			
        .LINK
			Set-ButtonControlOnMouseDoubleClick
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.anchorstyles.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.dockstyle.aspx
            
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.padding.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.size.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.point.aspx
    #>
    
    $Button = New-Object -TypeName 'System.Windows.Forms.Button';
	
	$buttonSplat = @{ Button = $Button; Visible = (-not $NotVisible); Enabled = (-not $Disabled) };
	$controlSpat = @{ Control = $Button };
	$buttonKeys = @('DialogResult', 'DialogResult', 'Tag', 'Text', 'OnClick', 'OnDoubleClick', 'OnMouseClick', 'OnMouseDoubleClick');
	foreach ($key in $PSBoundParameters.Keys) {
		if ($buttonKeys -icontains $key) {
			$buttonSplat.Add($key, $PSBoundParameters[$key]);
		} else {
			if ($key -ine 'NotVisible' -and $key -ine 'Disabled') { $controlSpat.Add($key, $PSBoundParameters[$key]) };
		}
	}
	
	Set-FormControlProperties @controlSpat;
	Set-FormButtonProperties @buttonSplat;
	
	$Button | Write-Output;
}

Function Add-FormControl  {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # Parent container control.
        [System.Windows.Forms.ContainerControl]$Parent,
        
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true)]
        # Child control to add to parent
        [System.Windows.Forms.Control]$Child,
        
        # If set, then the child control is returned.
        [switch]$Passthru
    )
    <#
        .SYNOPSIS
            Add control to parent

        .DESCRIPTION
            Adds a control to the parent container control.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.containercontrol.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
    #>
    
	Process {
        $Parent.Controls.Add($Child);
        if ($PassThru) { $Child | Write-Output }
    }
}

Function Set-FormStartPosition {
    [CmdletBinding(DefaultParameterSetName = 'WindowsDefaultBounds')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The form whose start position is to be set
        [System.Windows.Forms.Form]$Form,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'WindowsDefaultBounds')]
		[Alias('DefaultBounds')]
        # The form is positioned at the Windows default location and has the bounds determined by Windows default.
        [switch]$WindowsDefaultBounds,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'CenterParent')]
        # The form is centered within the bounds of its parent form.
        [switch]$CenterParent,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'CenterScreen')]
        # The form is centered on the current display, and has the dimensions specified in the form's size.
        [switch]$CenterScreen,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Manual')]
        # The position of the form is determined by the Location property.
        [switch]$Manual,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'WindowsDefaultLocation')]
		[Alias('DefaultLocation')]
        # The form is positioned at the Windows default location and has the dimensions specified in the form's size.
        [switch]$WindowsDefaultLocation,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'FormStartPosition')]
        # A FormStartPosition that represents the starting position of the form.
        [System.Windows.Forms.FormStartPosition]$StartPosition
    )
    <#
        .SYNOPSIS
            Set form StartPosition property.

        .DESCRIPTION
            Sets the starting position of the form at run time.
        
        .OUTPUTS
            System.Windows.Forms.FormStartPosition. Specifies the initial position of a form.
        
        .LINK
            New-WindowObject
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.form.aspx
           
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.formstartposition.aspx
    #>
    
    Begin {
        if ($PSBoundParameters.ContainsKey('StartPosition')) {
            $FormStartPosition = $StartPosition;
        } else {
            $FormStartPosition = [System.Enum]::Parse([System.Windows.Forms.FormStartPosition], $PSCmdlet.ParameterSetName);
        }
    }
    Process { $Form.StartPosition = $FormStartPosition }
}

Function Set-FormSize {
    [CmdletBinding(DefaultParameterSetName = 'Size')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The form whose size is to be set
        [System.Windows.Forms.Form]$Form,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
        # The width of the form.
        [int]$Width,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Values')]
        # The height of the form.
        [string]$Height,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Size')]
        # A Size that represents the size of the form.
        [System.Drawing.Size]$Size
    )
    <#
        .SYNOPSIS
            Set form Size property.

        .DESCRIPTION
            Sets the size of the form.
        
        .LINK
            New-WindowObject
        
        .LINK
            New-DrawingSize
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.form.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.size.aspx
    #>
    
    Begin {
        $AllForms = @();
    }
    
    Process {
        $AllForms += $Form;
    }
    
    End {
        if ($PSBoundParameters.ContainsKey('Size')) {
            $AllForms | Set-FormControlSize -Size $Size;
        } else {
            $AllForms | Set-FormControlSize -Width $Width -Height $Height;
        }
    }
}

Function Set-FormLocation {
    [CmdletBinding(DefaultParameterSetName = 'Location')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The form whose location is to be set
        [System.Windows.Forms.Form]$Form,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
		[Alias('Horizontal')]
        # The horizontal position of the form.
        [int]$X,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Values')]
		[Alias('Vertical')]
        # The vertical position of the form.
        [string]$Y,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Location')]
        # A Point that represents the location of the form.
        [System.Drawing.Point]$Location
    )
    <#
        .SYNOPSIS
            Set form Location property.

        .DESCRIPTION
            Sets the Point that represents the upper-left corner of the Form in screen coordinates.
        
        .LINK
            New-WindowObject
        
        .LINK
            New-DrawingPoint
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.form.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.point.aspx
    #>
    
    Begin {
        $AllForms = @();
    }
    
    Process {
        $AllForms += $Form;
    }
    
    End {
        if ($PSBoundParameters.ContainsKey('Size')) {
            $AllForms | Set-FormControlLocation -Location $Location;
        } else {
            $AllForms | Set-FormControlLocation -X $X -Y $Y;
        }
    }
}

Function Set-MaximumFormSize {
    [CmdletBinding(DefaultParameterSetName = 'Size')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The form whose maximum size is to be set
        [System.Windows.Forms.Form]$Form,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
        # The maximum width of the form.
        [int]$Width,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Values')]
        # The maximum height of the form.
        [string]$Height,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Size')]
        # A Size that represents the maximum size of the form.
        [System.Drawing.Size]$Size
    )
    <#
        .SYNOPSIS
            Set form MaximumSize property.

        .DESCRIPTION
            Sets the maximum size the form can be resized to.
        
        .LINK
            New-WindowObject
        
        .LINK
            New-DrawingSize
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.form.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.size.aspx
    #>
    
    Begin {
        $AllForms = @();
    }
    
    Process {
        $AllForms += $Form;
    }
    
    End {
        if ($PSBoundParameters.ContainsKey('Size')) {
            $AllForms | Set-MaximumFormControlSize -Location $Location;
        } else {
            $AllForms | Set-MaximumFormControlSize -X $X -Y $Y;
        }
    }
}

Function Set-MinimumFormSize {
    [CmdletBinding(DefaultParameterSetName = 'Size')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        # The form whose minimum size is to be set
        [System.Windows.Forms.Form]$Form,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Values')]
        # The minimum width of the form.
        [int]$Width,
        
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Values')]
        # The minimum height of the form.
        [string]$Height,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Size')]
        # A Size that represents the minimum size of the form.
        [System.Drawing.Size]$Size
    )
    <#
        .SYNOPSIS
            Set form MinimumSize property.

        .DESCRIPTION
            Sets the minimum size the form can be resized to.
        
        .LINK
            New-WindowObject
        
        .LINK
            New-DrawingSize
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.form.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.drawing.size.aspx
    #>
    
    Begin {
        $AllForms = @();
    }
    
    Process {
        $AllForms += $Form;
    }
    
    End {
        if ($PSBoundParameters.ContainsKey('Size')) {
            $AllForms | Set-MinimumFormControlSize -Location $Location;
        } else {
            $AllForms | Set-MinimumFormControlSize -X $X -Y $Y;
        }
    }
}

Function Set-FormAcceptButton {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # The form whose maximum size is to be set
        [System.Windows.Forms.Form]$Form,
        
        [Parameter(Mandatory = $true, Position = 1)]
        # The maximum width of the form.
        [System.Windows.Forms.Button]$Button
    )
    <#
        .SYNOPSIS
            Create new Button.

        .DESCRIPTION
            Initializes a new instance of the Button class.
        
        .OUTPUTS
            System.Windows.Forms.Button. Represents a Windows button control.
        
        .LINK
            New-FormsButton
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.button.aspx
    #>
    
    $Form.AcceptButton = $Button;
}

Function New-WindowObject {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [ValidatePattern('^[\S]+(\.\S+)*$')]
        # Name of the form.
        [string]$Name,
        
        [Parameter(Mandatory = $true)]
        # Window title.
        [string]$Title,
        
        [Parameter(Mandatory = $false)]
        # Sets the size of the auto-scroll margin in pixels.
        [System.Drawing.Size]$AutoScrollMargin,
        
        [Parameter(Mandatory = $false)]
        # determines the minimum size of the virtual area through which the user can scroll.
        [System.Drawing.Size]$AutoScrollMinSize,
        
        [Parameter(Mandatory = $false)]
        # Sets the location of the auto-scroll position, in pixels.
        [System.Drawing.Point]$AutoScrollPosition,
        
        [Parameter(Mandatory = $false)]
        # Sets a value indicating whether the container enables the user to scroll to any controls placed outside of its visible boundaries.
        [switch]$NoAutoScroll,
        
        [Parameter(Mandatory = $false)]
        # The form is not displayed as a top-level window.
        [switch]$NotTopLevel,
        
        [Parameter(Mandatory = $false)]
        # Maximize button is not displayed in the caption bar of the form
        [switch]$NoMaximizeBox,
        
        [Parameter(Mandatory = $false)]
        # Minimize button is not displayed in the caption bar of the form
        [switch]$NoMinimizeBox
    )
    <#
        .SYNOPSIS
            Create new windows form.

        .DESCRIPTION
            Initializes a new instance of System.Windows.Forms.Form.
        
        .OUTPUTS
            System.Windows.Forms.Form. Represents a windows form.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.form.aspx
			
        .LINK
            Set-FormAcceptButton

        .LINK
			Set-FormLocation
    
        .LINK
            Set-MinimumFormSize
            
        .LINK
            Set-MaximumFormSize
			
        .LINK
            Set-FormSize
        
        .LINK
            Set-FormStartPosition
			
        .LINK
			Set-ScrollableControlProperties
			
        .LINK
            Set-FormControlProperties
            
        .LINK
            Set-FormControlBackColor
        
        .LINK
            Set-FormControlFont
            
        .LINK
            Set-FormControlForeColor
    #>
    
    $Form = New-Object -TypeName 'System.Windows.Forms.Form';
    $Form.Name = $Name;
    $Form.Text = $Title;
    $Form.AutoScroll = -not $NoAutoScroll.IsPresent;
    $Form.TopLevel = -not $NotTopLevel.IsPresent;
    $Form.MaximizeBox = -not $NoMaximizeBox.IsPresent;
    $Form.MinimizeBox = -not $NoMaximizeBox.IsPresent;
	if ($PSBoundParameters.ContainsKey('AutoScrollMargin')) { $Form.AutoScrollMargin = $AutoScrollMargin }
	if ($PSBoundParameters.ContainsKey('AutoScrollMinSize')) { $Form.AutoScrollMinSize = $AutoScrollMinSize }
	if ($PSBoundParameters.ContainsKey('AutoScrollPosition')) { $Form.AutoScrollPosition = $AutoScrollPosition }
    
    $Form | Write-Output;
}

Function Get-ParentWindowsForm {
    [CmdletBinding()]
    [OutputType([System.Windows.Forms.Form])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # The control whose parent form is to be returned.
        [System.Windows.Forms.Control]$Control
    )
    <#
        .SYNOPSIS
            Get parent windows form.

        .DESCRIPTION
            Searches parent chain for windows form.
        
        .OUTPUTS
            System.Windows.Forms.Form. Represents a windows form.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.form.aspx
    #>
    
	for ($c = $Control; $c -ne $null; $c = $c.Parent) {
		if ($c -is [System.Windows.Forms.Form]) { return $c }
	}
}

Function New-TableLayoutPanel {
    [CmdletBinding()]
    [OutputType([System.Windows.Forms.TableLayoutPanel])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [ValidatePattern('^[\S]+$')]
        # Name of the control.
        [string]$Name,

        [Parameter(Mandatory = $false)]
        # A bitwise combination of the AnchorStyles values.
        [System.Windows.Forms.AnchorStyles]$Anchor,

        [Parameter(Mandatory = $false)]
        # Determines how the control should be docked within its parent container.
        [System.Windows.Forms.DockStyle]$Dock,

        [Parameter(Mandatory = $false)]
        # A Point that represents the location of the control.
        [System.Drawing.Point]$Location,
        
        [Parameter(Mandatory = $false)]
        # A Padding representing the space between controls.
        [System.Windows.Forms.Padding]$Margin,
        
        [Parameter(Mandatory = $false)]
        # A Size that represents the size of the control.
        [System.Drawing.Size]$Size,
        
        [Parameter(Mandatory = $false)]
        # Sets the Visible property to false.
        [switch]$NotVisible
    )
    <#
        .SYNOPSIS
            Create new table layout panel.

        .DESCRIPTION
            Initializes a new instance of System.Windows.Forms.TableLayoutPanel.
        
        .OUTPUTS
            System.Windows.Forms.TableLayoutPanel. Represents a panel that dynamically lays out its contents in a grid composed of rows and columns.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
			
        .LINK
			New-LayoutPanelRowStyle
			
        .LINK
			New-LayoutPanelColumnStyle
			
        .LINK
			Get-LayoutPanelRowStyle
			
        .LINK
			Add-LayoutPanelRowStyle
			
        .LINK
			Push-LayoutPanelRowStyle
			
        .LINK
			Remove-LayoutPanelRowStyle
			
        .LINK
			Clear-LayoutPanelRowStyles
			
        .LINK
			Get-LayoutPanelRowStyle
			
        .LINK
			Add-LayoutPanelRowStyle
			
        .LINK
			Push-LayoutPanelRowStyle
			
        .LINK
			Remove-LayoutPanelColumnStyle
			
        .LINK
			Clear-LayoutPanelColumnStyles
		
        .LINK
            Add-TableLayoutControl
            
        .LINK
            Remove-TableLayoutControl
            
        .LINK
            Clear-TableLayoutControls
            
        .LINK
			Set-ScrollableControlProperties
			
        .LINK
            Set-FormControlProperties
    #>
    
    $TableLayoutPanel = New-Object -TypeName 'System.Windows.Forms.TableLayoutPanel';
    $TableLayoutPanel.Name = $Name;
    $TableLayoutPanel.Visible = -not $NotVisible;
	if ($PSBoundParameters.ContainsKey('Visible')) { $TableLayoutPanel.Visible = $Visible }
	if ($PSBoundParameters.ContainsKey('Anchor')) { $TableLayoutPanel.Anchor = $Anchor }
	if ($PSBoundParameters.ContainsKey('Dock')) { $TableLayoutPanel.Dock = $Dock }
	if ($PSBoundParameters.ContainsKey('Location')) { $TableLayoutPanel.Location = $Location }
	if ($PSBoundParameters.ContainsKey('Margin')) { $TableLayoutPanel.Margin = $Margin }
	if ($PSBoundParameters.ContainsKey('Size')) { $TableLayoutPanel.Size = $Size }
	if ($PSBoundParameters.ContainsKey('Visible')) { $TableLayoutPanel.Visible = $Visible }
    
    $TableLayoutPanel | Write-Output;
}

Function New-LayoutPanelColumnStyle {
    [CmdletBinding(DefaultParameterSetName = 'AutoSize')]
    Param(
        [Parameter(Mandatory = $false, Position = 0, ParameterSetName = 'Absolute')]
        [Parameter(Mandatory = $false, Position = 0, ParameterSetName = 'Percent')]
        [float]$Width,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'AutoSize')]
        [switch]$AutoSize,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Absolute')]
        [switch]$Absolute,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Percent')]
        [switch]$Percent
    )
    
    switch ($Cmdlet.ParameterSetName) {
        'Absolute' {
            if ($PSBoundParameters.ContainsKey('Width')) {
                New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Absolute, $Width);
            } else {
                New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Absolute);
            }
            break;
        }
        'Percent' {
            if ($PSBoundParameters.ContainsKey('Width')) {
                New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Percent, $Width);
            } else {
                New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Percent);
            }
            break;
        }
        default {
            if ($PSBoundParameters.ContainsKey('AutoSize')) {
                New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize);
            } else {
                New-Object -TypeName 'System.Windows.Forms.ColumnStyle';
            }
            break;
        }
    }
}

Function Get-LayoutPanelColumnStyle {
    [CmdletBinding()]
    [OutputType([System.Windows.Forms.ColumnStyle])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
		[Alias('Panel')]
		# The table layout panel from which to retrieve column styles.
        [System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel,
        
        [Parameter(Mandatory = $false, Position = 1, ValueFromPipeline = $true)]
        [ValidateRange(0, 2147483647)]
		# Index of the column style to return.
        [int]$Index
    )
    <#
        .SYNOPSIS
            Gets column style(s).

        .DESCRIPTION
            Gets column style(s) for the table layout panel.
        
        .OUTPUTS
            System.Windows.Forms.ColumnStyle. Represents the look and feel of a column in a table layout.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.columnstyle.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
			
        .LINK
			Set-LayoutPanelColumnStyle
			
        .LINK
			Set-LayoutPanelColumnWidth
					
        .LINK
			Set-LayoutPanelSizeType
			
        .LINK
			Add-LayoutPanelColumnStyle
			
        .LINK
			Push-LayoutPanelColumnStyle
			
        .LINK
			Remove-LayoutPanelColumnStyle
			
        .LINK
			Clear-LayoutPanelColumnStyles
			
        .LINK
			New-TableLayoutPanel
			
        .LINK
			Get-LayoutPanelRowStyle
    #>
    
    Process {
        if ($PSBoundParameters.ContainsKey('Index')) {
            if ($TableLayoutPanel.ColumnStyles.Count -gt $Index) { $TableLayoutPanel.ColumnStyles[$Index] }
        } else {
            if ($TableLayoutPanel.ColumnStyles.Count -gt 0) { $TableLayoutPanel.ColumnStyles | Write-Output }
        }
    }
}

Function Set-LayoutPanelColumnStyle {
    [CmdletBinding(DefaultParameterSetName = 'StyleAutoSize')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'PanelAbsolute')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'PanelPercent')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'StyleAbsolute')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'StylePercent')]
		# Width of column
        [float]$Width,
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'StyleAutoSize')]
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'StyleAbsolute')]
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'StylePercent')]
		[Alias('Style')]
		# Column style whose properties are to be set.
        [System.Windows.Forms.ColumnStyle]$ColumnStyle,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'PanelAutoSize')]
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'PanelAbsolute')]
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'PanelPercent')]
		[Alias('Panel')]
		# Table layout panel whose column style properties are to be set
        [System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel,
        
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'PanelAutoSize')]
        [Parameter(Mandatory = $true, Position = 2, ValueFromPipeline = $true, ParameterSetName = 'PanelAbsolute')]
        [Parameter(Mandatory = $true, Position = 2, ValueFromPipeline = $true, ParameterSetName = 'PanelPercent')]
		# Index of column style whose properties are to be set
        [System.Windows.Forms.TableLayoutPanel]$Index,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'StyleAutoSize')]
        [Parameter(Mandatory = $true, ParameterSetName = 'PanelAutoSize')]
		# Column is auto-sized
        [switch]$AutoSize,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'StyleAbsolute')]
        [Parameter(Mandatory = $true, ParameterSetName = 'PanelAbsolute')]
		# Column is absolutely-sized
        [switch]$Absolute,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'StylePercent')]
        [Parameter(Mandatory = $true, ParameterSetName = 'PanelPercent')]
		# Column size is based upon a percentage weight
        [switch]$Percent
    )
    <#
        .SYNOPSIS
            Sets column style(s).

        .DESCRIPTION
            Sets column style(s) for the table layout panel.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.columnstyle.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
			
        .LINK
			Get-LayoutPanelRowStyle
			
        .LINK
			Set-LayoutPanelRowHeight
			
        .LINK
			Set-LayoutPanelSizeType
			
        .LINK
			New-TableLayoutPanel
			
        .LINK
			New-LayoutPanelRowStyle
			
        .LINK
			Set-LayoutPanelColumnStyle
    #>
    
	Begin {
		$SizeType = [System.Enum]::Parse([System.Windows.Forms.SizeType], $PSCmdlet.ParameterSetName.Substring(5));
	}
	
	Process {
		if ($PSBoundParameters.ContainsKey('ColumnStyle')) {
			$ColumnStyle.SizeType = $SizeType;
			$ColumnStyle.Width = $Width;
		} else {
			$c = Get-LayoutPanelColumnStyle -TableLayoutPanel $TableLayoutPanel -Index $Index;
			if ($c -ne $null) { 
				$c.SizeType = $SizeType;
				$c.Width = $Width;
			}
		}
	}
}

Function Set-LayoutPanelColumnWidth {
    [CmdletBinding(DefaultParameterSetName = 'StyleAutoSize')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Style')]
		[Alias('Style')]
		# Column style whose properties are to be set.
        [System.Windows.Forms.ColumnStyle]$ColumnStyle,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Panel')]
		[Alias('Panel')]
		# Table layout panel whose column style properties are to be set
        [System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel,
        
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Panel')]
		# Index of column style whose properties are to be set
        [System.Windows.Forms.TableLayoutPanel]$Index,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Style')]
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Panel')]
		# Width of column
        [float]$Width
    )
    <#
        .SYNOPSIS
            Sets column style(s).

        .DESCRIPTION
            Sets column style(s) for the table layout panel.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.columnstyle.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
			
        .LINK
			Get-LayoutPanelRowStyle
			
        .LINK
			Set-LayoutPanelRowStyle
			
        .LINK
			Set-LayoutPanelSizeType
			
        .LINK
			New-TableLayoutPanel
			
        .LINK
			New-LayoutPanelRowStyle
			
        .LINK
			Set-LayoutPanelColumnWidth
    #>
	
	Process {
		if ($PSBoundParameters.ContainsKey('ColumnStyle')) {
			$ColumnStyle.Width = $Width;
		} else {
			$c = Get-LayoutPanelColumnStyle -TableLayoutPanel $TableLayoutPanel -Index $Index;
			if ($c -ne $null) { $c.Width = $Width }
		}
	}
}

Function Add-LayoutPanelColumnStyle {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
		[Alias('Panel')]
		# The table layout panel to which column styles are to be added.
        [System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel,
        
        [Parameter(Mandatory = $false, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Absolute')]
        [Parameter(Mandatory = $false, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Percent')]
		# Width of column
        [float]$Width,
        
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'ColumnStyle')]
		[Alias('Style')]
		# The row column to be added.
        [System.Windows.Forms.ColumnStyle]$ColumnStyle,
        
        [Parameter(Mandatory = $false, Position = 1, ParameterSetName = 'AutoSize')]
        [ValidateRange(1, 2147483647)]
		# Number of auto-size columns to add
        [int]$Count = 1,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'AutoSize')]
		# Add auto-sized column(s)
        [switch]$AutoSize,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Absolute')]
		# Add absolutely-sizes column(s)
        [switch]$Absolute,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Percent')]
		# Add column(s) whose widths are based upon a percentage of remaining available space.
        [switch]$Percent
    )
    <#
        .SYNOPSIS
            Adds a column style.

        .DESCRIPTION
            Adds a column style to the table layout panel.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.columnstyle.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
			
        .LINK
			Get-LayoutPanelColumnStyle
			
        .LINK
			Push-LayoutPanelColumnStyle
			
        .LINK
			Remove-LayoutPanelColumnStyle
			
        .LINK
			Clear-LayoutPanelColumnStyles
			
        .LINK
			New-TableLayoutPanel
			
        .LINK
			Add-LayoutPanelRowStyle
    #>
    
	Begin {
		if ($PSCmdlet.ParameterSetName -eq 'Absolute' -or $PSCmdlet.ParameterSetName -eq 'Percent') {
			$splat = @{};
			foreach ($k in $PSBoundParameters.Keys) { if ($k -ne 'TableLayoutPanel') { $splat.Add($k, $PSBoundParameters[$k]) } }
		}
	}
	
    Process {
		switch ($PSCmdlet.ParameterSetName) {
			'ColumnStyle' { $TableLayoutPanel.ColumnStyles.Add($ColumnStyle) | Out-Null; break; }
			'AutoSize' { for ($i = 0; $i -lt $Count; $i++) { $TableLayoutPanel.ColumnStyles.Add((New-LayoutPanelColumnStyle -AutoSize)) | Out-Null; break; } }
			default { $TableLayoutPanel.ColumnStyles.Add((New-LayoutPanelColumnStyle @splat)) | Out-Null; break; }
		}
	}
	
	End {
		$MaxCol = $TableLayoutPanel.ColumnStyles.Count;
		foreach ($Control in $TableLayoutPanel.Controls) {
			$Value = $TableLayoutPanel.GetColumn($Control) + $TableLayoutPanel.GetColumnSpan($Control);
			if ($Value -gt $MaxCol) { $MaxCol = $Value }
		}
		if ($TableLayoutPanel.ColumnCount -ne $MaxCol) { $TableLayoutPanel.ColumnCount = $MaxCol }
	}
}

Function Push-LayoutPanelColumnStyle {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
		[Alias('Panel')]
		# The table layout panel to which column styles are to be added.
        [System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel,
        
        [Parameter(Mandatory = $true, Position = 1)]
        [ValidateRange(0, 2147483647)]
		# Index at which the column style(s) are to be inserted.
        [int]$Index,
        
        [Parameter(Mandatory = $false, Position = 2, ValueFromPipeline = $true, ParameterSetName = 'Absolute')]
        [Parameter(Mandatory = $false, Position = 2, ValueFromPipeline = $true, ParameterSetName = 'Percent')]
		# Width of column(s)
        [float]$Width,
        
        [Parameter(Mandatory = $true, Position = 2, ValueFromPipeline = $true, ParameterSetName = 'ColumnStyle')]
		[Alias('Style')]
		# The row column to be added.
        [System.Windows.Forms.ColumnStyle]$ColumnStyle,
        
        [Parameter(Mandatory = $false, Position = 2, ParameterSetName = 'AutoSize')]
        [ValidateRange(1, 2147483647)]
		# Number of auto-size columns to add
        [int]$Count = 1,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'AutoSize')]
		# Add auto-sized column(s)
        [switch]$AutoSize,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Absolute')]
		# Add absolutely-sizes column(s)
        [switch]$Absolute,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Percent')]
		# Add column(s) whose widths are based upon a percentage of remaining available space.
        [switch]$Percent
    )
    <#
        .SYNOPSIS
            Inserts a column style.

        .DESCRIPTION
            Inserts a column style into the table layout panel.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.columnstyle.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
			
        .LINK
			Get-LayoutPanelColumnStyle
			
        .LINK
			Add-LayoutPanelColumnStyle
			
        .LINK
			Remove-LayoutPanelColumnStyle
			
        .LINK
			Clear-LayoutPanelColumnStyles
			
        .LINK
			New-TableLayoutPanel
			
        .LINK
			Push-LayoutPanelRowStyle
    #>
    
	Begin {
		$CurrentIndex = $Index;
		if ($PSCmdlet.ParameterSetName -eq 'Absolute' -or $PSCmdlet.ParameterSetName -eq 'Percent') {
			$splat = @{};
			foreach ($k in $PSBoundParameters.Keys) { if ($k -ne 'TableLayoutPanel') { $splat.Add($k, $PSBoundParameters[$k]) } }
		}
	}
	
    Process {
		switch ($PSCmdlet.ParameterSetName) {
			'ColumnStyle' { $TableLayoutPanel.ColumnStyles.Insert($CurrentIndex, $ColumnStyle); $CurrentIndex++; break; }
			'AutoSize' { for ($i = 0; $i -lt $Count; $i++) { $TableLayoutPanel.ColumnStyles.Insert($CurrentIndex, (New-LayoutPanelColumnStyle -AutoSize)); $CurrentIndex++; break; } }
			default { $TableLayoutPanel.ColumnStyles.Insert($CurrentIndex, (New-LayoutPanelColumnStyle @splat)); $CurrentIndex++; break; }
		}
	}
	
	End {
		$MaxCol = $TableLayoutPanel.ColumnStyles.Count;
		foreach ($Control in $TableLayoutPanel.Controls) {
			$Value = $TableLayoutPanel.GetColumn($Control) + $TableLayoutPanel.GetColumnSpan($Control);
			if ($Value -gt $MaxCol) { $MaxCol = $Value }
		}
		if ($TableLayoutPanel.ColumnCount -ne $MaxCol) { $TableLayoutPanel.ColumnCount = $MaxCol }
	}
}

Function Remove-LayoutPanelColumnStyle {
    [CmdletBinding(DefaultParameterSetName = 'ByObject')]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
		[Alias('Panel')]
		# The table layout panel from which to retrieve column styles.
        [System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel,
        
        [Parameter(Mandatory = $false, Position = 1, ParameterSetName = 'ByIndex')]
        [ValidateRange(0, 2147483647)]
		# Index of the column style to remove.
        [int]$Index,
        
        [Parameter(Mandatory = $false, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'ByObject')]
		[Alias('Style')]
		# Column style to remove.
        [System.Windows.Forms.ColumnStyle]$ColumnStyle
    )
    <#
        .SYNOPSIS
            Removes a column style.

        .DESCRIPTION
            Removes a column style from the table layout panel.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.columnstyle.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
			
        .LINK
			Get-LayoutPanelColumnStyle
			
        .LINK
			Add-LayoutPanelColumnStyle
			
        .LINK
			Push-LayoutPanelColumnStyle
			
        .LINK
			Clear-LayoutPanelColumnStyles
			
        .LINK
			New-TableLayoutPanel
			
        .LINK
			Remove-LayoutPanelRowStyle
    #>
    
    Process {
		if ($PSBoundParameters.ContainsKey('Index')) { $i = $Index } else { $i = $TableLayoutPanel.ColumnStyles.IndexOf($ColumnStyle) }
		$TableLayoutPanel.ColumnStyles.RemoveAt($Index);
		$MaxCol = $TableLayoutPanel.ColumnStyles.Count;
		foreach ($Control in $TableLayoutPanel.Controls) {
			$Value = $TableLayoutPanel.GetColumn($Control) + $TableLayoutPanel.GetColumnSpan($Control);
			if ($Value -gt $MaxCol) { $MaxCol = $Value}
		}
		if ($TableLayoutPanel.ColumnCount -gt $MaxCol) { $TableLayoutPanel.ColumnCount = $MaxCol }
    }
}

Function Clear-LayoutPanelColumnStyles {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[Alias('Panel')]
		# The table layout panel from which to clear column styles.
        [System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel
    )
    <#
        .SYNOPSIS
            Clears all column styles.

        .DESCRIPTION
            Clears all column styles from the table layout panel.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.columnstyle.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
			
        .LINK
			Get-LayoutPanelColumnStyle
			
        .LINK
			Add-LayoutPanelColumnStyle
			
        .LINK
			Push-LayoutPanelColumnStyle
			
        .LINK
			Remove-LayoutPanelColumnStyles
			
        .LINK
			New-TableLayoutPanel
			
        .LINK
			Clear-LayoutPanelRowStyle
    #>
    
    Process {
		$TableLayoutPanel.ColumnStyles.Clear();
		$MaxCol = 0;
		foreach ($Control in $TableLayoutPanel.Controls) {
			$Value = $TableLayoutPanel.GetColumn($Control) + $TableLayoutPanel.GetColumnSpan($Control);
			if ($Value -gt $MaxCol) { $MaxCol = $Value}
		}
		if ($TableLayoutPanel.ColumnCount -gt $MaxCol) { $TableLayoutPanel.ColumnCount = $MaxCol }
    }
}

Function New-LayoutPanelRowStyle {
    [CmdletBinding(DefaultParameterSetName = 'AutoSize')]
    Param(
        [Parameter(Mandatory = $false, Position = 0, ParameterSetName = 'Absolute')]
        [Parameter(Mandatory = $false, Position = 0, ParameterSetName = 'Percent')]
        [float]$Height,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'AutoSize')]
        [switch]$AutoSize,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Absolute')]
        [switch]$Absolute,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Percent')]
        [switch]$Percent
    )
    
    switch ($Cmdlet.ParameterSetName) {
        'Absolute' {
            if ($PSBoundParameters.ContainsKey('Height')) {
                New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Absolute, $Height);
            } else {
                New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Absolute);
            }
            break;
        }
        'Percent' {
            if ($PSBoundParameters.ContainsKey('Height')) {
                New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Percent, $Height);
            } else {
                New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Percent);
            }
            break;
        }
        default {
            if ($PSBoundParameters.ContainsKey('AutoSize')) {
                New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize);
            } else {
                New-Object -TypeName 'System.Windows.Forms.RowStyle';
            }
            break;
        }
    }
}

Function Get-LayoutPanelRowStyle {
    [CmdletBinding()]
    [OutputType([System.Windows.Forms.RowStyle])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
		[Alias('Panel')]
		# The table layout panel from which to retrieve row styles.
        [System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel,
        
        [Parameter(Mandatory = $false, Position = 1, ValueFromPipeline = $true)]
        [ValidateRange(0, 2147483647)]
		# Index of the row style to return.
        [int]$Index
    )
    <#
        .SYNOPSIS
            Gets row style(s).

        .DESCRIPTION
            Gets row style(s) for the table layout panel.
        
        .OUTPUTS
            System.Windows.Forms.RowStyle. Represents the look and feel of a row in a table layout.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.rowstyle.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
			
        .LINK
			Set-LayoutPanelRowStyle
			
        .LINK
			Set-LayoutPanelRowHeight
					
        .LINK
			Set-LayoutPanelSizeType
			
        .LINK
			Add-LayoutPanelRowStyle
			
        .LINK
			Push-LayoutPanelRowStyle
			
        .LINK
			Remove-LayoutPanelRowStyle
			
        .LINK
			Clear-LayoutPanelRowStyles
			
        .LINK
			New-TableLayoutPanel
			
        .LINK
			Get-LayoutPanelColumnStyle
    #>
    
    Process {
        if ($PSBoundParameters.ContainsKey('Index')) {
            if ($TableLayoutPanel.RowStyles.Count -gt $Index) { $TableLayoutPanel.RowStyles[$Index] }
        } else {
            if ($TableLayoutPanel.RowStyles.Count -gt 0) { $TableLayoutPanel.RowStyles | Write-Output }
        }
    }
}

Function Set-LayoutPanelRowStyle {
    [CmdletBinding(DefaultParameterSetName = 'StyleAutoSize')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'PanelAbsolute')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'PanelPercent')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'StyleAbsolute')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'StylePercent')]
		# Height of row
        [float]$Height,
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'StyleAutoSize')]
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'StyleAbsolute')]
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'StylePercent')]
		[Alias('Style')]
		# Row style whose properties are to be set.
        [System.Windows.Forms.RowStyle]$RowStyle,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'PanelAutoSize')]
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'PanelAbsolute')]
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'PanelPercent')]
		[Alias('Panel')]
		# Table layout panel whose row style properties are to be set
        [System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel,
        
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'PanelAutoSize')]
        [Parameter(Mandatory = $true, Position = 2, ValueFromPipeline = $true, ParameterSetName = 'PanelAbsolute')]
        [Parameter(Mandatory = $true, Position = 2, ValueFromPipeline = $true, ParameterSetName = 'PanelPercent')]
		# Index of row style whose properties are to be set
        [System.Windows.Forms.TableLayoutPanel]$Index,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'StyleAutoSize')]
        [Parameter(Mandatory = $true, ParameterSetName = 'PanelAutoSize')]
		# Column is auto-sized
        [switch]$AutoSize,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'StyleAbsolute')]
        [Parameter(Mandatory = $true, ParameterSetName = 'PanelAbsolute')]
		# Column is absolutely-sized
        [switch]$Absolute,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'StylePercent')]
        [Parameter(Mandatory = $true, ParameterSetName = 'PanelPercent')]
		# Column size is based upon a percentage weight
        [switch]$Percent
    )
    <#
        .SYNOPSIS
            Sets row style(s).

        .DESCRIPTION
            Sets row style(s) for the table layout panel.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.rowstyle.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
			
        .LINK
			Get-LayoutPanelRowStyle
			
        .LINK
			Set-LayoutPanelRowHeight
			
        .LINK
			Set-LayoutPanelSizeType
			
        .LINK
			New-TableLayoutPanel
			
			
        .LINK
			New-LayoutPanelRowStyle
			
        .LINK
			Set-LayoutPanelColumnStyle
    #>
    
	Begin {
		$SizeType = [System.Enum]::Parse([System.Windows.Forms.SizeType], $PSCmdlet.ParameterSetName.Substring(5));
	}
	
	Process {
		if ($PSBoundParameters.ContainsKey('RowStyle')) {
			$RowStyle.SizeType = $SizeType;
			$RowStyle.Height = $Height;
		} else {
			$r = Get-LayoutPanelRowStyle -TableLayoutPanel $TableLayoutPanel -Index $Index;
			if ($r -ne $null) { 
				$r.SizeType = $SizeType;
				$r.Height = $Height;
			}
		}
	}
}

Function Set-LayoutPanelRowHeight {
    [CmdletBinding(DefaultParameterSetName = 'StyleAutoSize')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'Style')]
		[Alias('Style')]
		# Row style whose properties are to be set.
        [System.Windows.Forms.RowStyle]$RowStyle,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'Panel')]
		[Alias('Panel')]
		# Table layout panel whose row style properties are to be set
        [System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel,
        
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Panel')]
		# Index of row style whose properties are to be set
        [System.Windows.Forms.TableLayoutPanel]$Index,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'Style')]
        [Parameter(Mandatory = $true, Position = 2, ParameterSetName = 'Panel')]
		# Height of row
        [float]$Height
    )
    <#
        .SYNOPSIS
            Sets row style(s).

        .DESCRIPTION
            Sets row style(s) for the table layout panel.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.rowstyle.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
			
        .LINK
			Get-LayoutPanelRowStyle
			
        .LINK
			Set-LayoutPanelRowStyle
			
        .LINK
			Set-LayoutPanelSizeType
			
        .LINK
			New-TableLayoutPanel
			
        .LINK
			New-LayoutPanelRowStyle
			
        .LINK
			Set-LayoutPanelColumnWidth
    #>
	
	Process {
		if ($PSBoundParameters.ContainsKey('RowStyle')) {
			$RowStyle.Height = $Height;
		} else {
			$r = Get-LayoutPanelRowStyle -TableLayoutPanel $TableLayoutPanel -Index $Index;
			if ($r -ne $null) { $r.Height = $Height }
		}
	}
}

Function Add-LayoutPanelRowStyle {
    [CmdletBinding()]
    [OutputType([System.Windows.Forms.RowStyle])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
		[Alias('Panel')]
		# The table layout panel to which row styles are to be added.
        [System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel,
        
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'RowStyle')]
		[Alias('Style')]
		# The row style to be added.
        [System.Windows.Forms.RowStyle]$RowStyle,
        
        [Parameter(Mandatory = $false, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Absolute')]
        [Parameter(Mandatory = $false, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'Percent')]
		# Height of row
        [float]$Height,
        
        [Parameter(Mandatory = $false, Position = 1, ParameterSetName = 'AutoSize')]
        [ValidateRange(1, 2147483647)]
		# Number of auto-size rows to add
        [int]$Count = 1,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'AutoSize')]
		# Add auto-sized row(s)
        [switch]$AutoSize,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Absolute')]
		# Add absolutely-sizes row(s)
        [switch]$Absolute,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Percent')]
		# Add row(s) whose heights are based upon a percentage of remaining available space.
        [switch]$Percent
    )
    <#
        .SYNOPSIS
            Adds a row style.

        .DESCRIPTION
            Adds a row style to the table layout panel.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.rowstyle.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
			
        .LINK
			Get-LayoutPanelRowStyle
			
        .LINK
			Push-LayoutPanelRowStyle
			
        .LINK
			Remove-LayoutPanelRowStyle
			
        .LINK
			Clear-LayoutPanelRowStyles
			
        .LINK
			New-TableLayoutPanel
			
        .LINK
			Add-LayoutPanelColumnStyle
    #>
    
	Begin {
		if ($PSCmdlet.ParameterSetName -eq 'Absolute' -or $PSCmdlet.ParameterSetName -eq 'Percent') {
			$splat = @{};
			foreach ($k in $PSBoundParameters.Keys) { if ($k -ne 'TableLayoutPanel') { $splat.Add($k, $PSBoundParameters[$k]) } }
		}
	}
	
    Process {
		switch ($PSCmdlet.ParameterSetName) {
			'RowStyle' { $TableLayoutPanel.RowStyles.Add($RowStyle) | Out-Null; break; }
			'AutoSize' { for ($i = 0; $i -lt $Count; $i++) { $TableLayoutPanel.RowStyles.Add((New-LayoutPanelRowStyle -AutoSize)) | Out-Null; break; } }
			default { $TableLayoutPanel.RowStyles.Add((New-LayoutPanelRowStyle @splat)) | Out-Null; break; }
		}
	}
	
	End {
		$MaxRow = $TableLayoutPanel.RowStyles.Count;
		foreach ($Control in $TableLayoutPanel.Controls) {
			$Value = $TableLayoutPanel.GetRow($Control) + $TableLayoutPanel.GetRowSpan($Control);
			if ($Value -gt $MaxRow) { $MaxRow = $Value }
		}
		if ($TableLayoutPanel.RowCount -ne $MaxRow) { $TableLayoutPanel.RowCount = $MaxRow }
	}
}

Function Push-LayoutPanelRowStyle {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
		[Alias('Panel')]
		# The table layout panel to which column styles are to be added.
        [System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel,
        
        [Parameter(Mandatory = $true, Position = 1)]
        [ValidateRange(0, 2147483647)]
		# Index at which the column style(s) are to be inserted.
        [int]$Index,
        
        [Parameter(Mandatory = $false, Position = 2, ValueFromPipeline = $true, ParameterSetName = 'Absolute')]
        [Parameter(Mandatory = $false, Position = 2, ValueFromPipeline = $true, ParameterSetName = 'Percent')]
		# Width of column(s)
        [float]$Width,
        
        [Parameter(Mandatory = $true, Position = 2, ValueFromPipeline = $true, ParameterSetName = 'RowStyle')]
		[Alias('Style')]
		# The row column to be added.
        [System.Windows.Forms.RowStyle]$RowStyle,
        
        [Parameter(Mandatory = $false, Position = 2, ParameterSetName = 'AutoSize')]
        [ValidateRange(1, 2147483647)]
		# Number of auto-size columns to add
        [int]$Count = 1,
        
        [Parameter(Mandatory = $false, ParameterSetName = 'AutoSize')]
		# Add auto-sized column(s)
        [switch]$AutoSize,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Absolute')]
		# Add absolutely-sizes column(s)
        [switch]$Absolute,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Percent')]
		# Add column(s) whose widths are based upon a percentage of remaining available space.
        [switch]$Percent
    )
    <#
        .SYNOPSIS
            Inserts a column style.

        .DESCRIPTION
            Inserts a column style into the table layout panel.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.rowstyle.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
			
        .LINK
			Get-LayoutPanelRowStyle
			
        .LINK
			Add-LayoutPanelRowStyle
			
        .LINK
			Remove-LayoutPanelRowStyle
			
        .LINK
			Clear-LayoutPanelRowStyles
			
        .LINK
			New-TableLayoutPanel
			
        .LINK
			Push-LayoutPanelColumnStyle
    #>
    
	Begin {
		$CurrentIndex = $Index;
		if ($PSCmdlet.ParameterSetName -eq 'Absolute' -or $PSCmdlet.ParameterSetName -eq 'Percent') {
			$splat = @{};
			foreach ($k in $PSBoundParameters.Keys) { if ($k -ne 'TableLayoutPanel') { $splat.Add($k, $PSBoundParameters[$k]) } }
		}
	}
	
    Process {
		switch ($PSCmdlet.ParameterSetName) {
			'RowStyle' { $TableLayoutPanel.RowStyles.Insert($CurrentIndex, $RowStyle); $CurrentIndex++; break; }
			'AutoSize' { for ($i = 0; $i -lt $Count; $i++) { $TableLayoutPanel.RowStyles.Insert($CurrentIndex, (New-LayoutPanelRowStyle -AutoSize)); $CurrentIndex++; break; } }
			default { $TableLayoutPanel.RowStyles.Insert($CurrentIndex, (New-LayoutPanelRowStyle @splat)); $CurrentIndex++; break; }
		}
	}
	
	End {
		$MaxRow = $TableLayoutPanel.RowStyles.Count;
		foreach ($Control in $TableLayoutPanel.Controls) {
			$Value = $TableLayoutPanel.GetRow($Control) + $TableLayoutPanel.GetRowSpan($Control);
			if ($Value -gt $MaxRow) { $MaxRow = $Value }
		}
		if ($TableLayoutPanel.RowCount -ne $MaxRow) { $TableLayoutPanel.RowCount = $MaxRow }
	}
}

Function Remove-LayoutPanelRowStyle {
    [CmdletBinding(DefaultParameterSetName = 'ByObject')]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
		[Alias('Panel')]
		# The table layout panel from which to remove row styles.
        [System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel,
        
        [Parameter(Mandatory = $false, Position = 1, ParameterSetName = 'ByIndex')]
        [ValidateRange(0, 2147483647)]
		# Index of the row style to remove.
        [int]$Index,
        
        [Parameter(Mandatory = $false, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'ByObject')]
		[Alias('Style')]
		# Row style to remove.
        [System.Windows.Forms.RowStyle]$RowStyle
    )
    <#
        .SYNOPSIS
            Removes row style(s).

        .DESCRIPTION
            Removes row style(s) from the table layout panel.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.rowstyle.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
			
        .LINK
			Get-LayoutPanelRowStyle
			
        .LINK
			Add-LayoutPanelRowStyle
			
        .LINK
			Push-LayoutPanelRowStyle
			
        .LINK
			Clear-LayoutPanelRowStyles
			
        .LINK
			New-TableLayoutPanel
			
        .LINK
			Remove-LayoutPanelColumnStyle
    #>
    
    Process {
		if ($PSBoundParameters.ContainsKey('Index')) { $i = $Index } else { $i = $TableLayoutPanel.RowStyles.IndexOf($RowStyle) }
		$TableLayoutPanel.RowStyles.RemoveAt($Index);
		$MaxRow = $TableLayoutPanel.RowStyles.Count;
		foreach ($Control in $TableLayoutPanel.Controls) {
			$Value = $TableLayoutPanel.GetRow($Control) + $TableLayoutPanel.GetRowSpan($Control);
			if ($Value -gt $MaxRow) { $MaxRow = $Value}
		}
		if ($TableLayoutPanel.RowCount -gt $MaxRow) { $TableLayoutPanel.RowCount = $MaxRow }
    }
}

Function Clear-LayoutPanelRowStyles {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[Alias('Panel')]
		# The table layout panel from which to clear row styles.
        [System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel
    )
    <#
        .SYNOPSIS
            Clears all row styles.

        .DESCRIPTION
            Clears all row styles from the table layout panel.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.rowstyle.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
			
        .LINK
			Get-LayoutPanelRowStyle
			
        .LINK
			Add-LayoutPanelRowStyle
			
        .LINK
			Push-LayoutPanelRowStyle
			
        .LINK
			Remove-LayoutPanelRowStyles
			
        .LINK
			New-TableLayoutPanel
			
        .LINK
			Clear-LayoutPanelColumnStyle
    #>
    
    Process {
		$TableLayoutPanel.RowStyles.Clear();
		$MaxRow = 0;
		foreach ($Control in $TableLayoutPanel.Controls) {
			$Value = $TableLayoutPanel.GetRow($Control) + $TableLayoutPanel.GetRowSpan($Control);
			if ($Value -gt $MaxRow) { $MaxRow = $Value}
		}
		if ($TableLayoutPanel.RowCount -gt $MaxRow) { $TableLayoutPanel.RowCount = $MaxRow }
    }
}

Function Set-LayoutPanelSizeType {
    [CmdletBinding(DefaultParameterSetName = 'ObjAutoSize')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'ObjAutoSize')]
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'ObjAbsolute')]
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true, ParameterSetName = 'ObjPercent')]
		[Alias('Style')]
        # RowStyle or ColumnStyle to set
        [System.Windows.Forms.TableLayoutStyle]$TableLayoutStyle,
        
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'RowAutoSize')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'RowAbsolute')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'RowPercent')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'ColAutoSize')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'ColAbsolute')]
        [Parameter(Mandatory = $true, Position = 0, ParameterSetName = 'ColPercent')]
		[Alias('Panel')]
		# The table layout panel to which row or column styles are to be added.
        [System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel,
        
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'RowAutoSize')]
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'RowAbsolute')]
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'RowPercent')]
        [ValidateRange(0, 2147483647)]
		# Index at which the row style(s) are to be modified.
        [int]$Row,
        
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'ColAutoSize')]
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'ColAbsolute')]
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true, ParameterSetName = 'ColPercent')]
        [ValidateRange(0, 2147483647)]
		# Index at which the column style(s) are to be modified.
        [int]$Column,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'ObjAutoSize')]
        [Parameter(Mandatory = $true, ParameterSetName = 'RowAutoSize')]
        [Parameter(Mandatory = $true, ParameterSetName = 'ColAutoSize')]
		# Row or Column is auto-sized
        [switch]$AutoSize,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'ObjAbsolute')]
        [Parameter(Mandatory = $true, ParameterSetName = 'RowAbsolute')]
        [Parameter(Mandatory = $true, ParameterSetName = 'ColAbsolute')]
		# Row or Column is absolutely-sized
        [switch]$Absolute,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'ObjPercent')]
        [Parameter(Mandatory = $true, ParameterSetName = 'RowPercent')]
        [Parameter(Mandatory = $true, ParameterSetName = 'ColPercent')]
		# Row or Column size is based upon a percentage weight
        [switch]$Percent
    )
    <#
        .SYNOPSIS
            Sets row or column size type.

        .DESCRIPTION
            Sets row or column size type(s) for the table layout panel.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutstyle.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
			
        .LINK
			Get-LayoutPanelRowStyle
			
        .LINK
			Get-LayoutPanelColumnStyle
			
        .LINK
			Set-LayoutPanelRowHeight
			
        .LINK
			Set-LayoutPanelColumnWidth
			
        .LINK
			New-LayoutPanelRowStyle
			
        .LINK
			New-LayoutPanelColumnStyle
    #>
    
    Begin {
		$SizeType = [System.Enum]::Parse([System.Windows.Forms.SizeType], $PSCmdlet.ParameterSetName.Substring(3));
    }
    
    Process {
		switch ($PSCmdlet.ParameterSetName.Substring(0, 3)) {
			'Row' { ($TableLayoutPanel | Get-LayoutPanelRowStyle -Index $Row).SizeType = $SizeType; break; }
			'Col' { ($TableLayoutPanel | Get-LayoutPanelColumnStyle -Index $Column).SizeType = $SizeType; break; }
			default { $TableLayoutStyle.SizeType = $SizeType; break; }
		}
	}
}

Function Add-TableLayoutControl  {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
		[Alias('TableLayoutPanel', 'Panel')]
        # Parent table layout pane.
        [System.Windows.Forms.TableLayoutPanel]$Parent,
        
        [Parameter(Mandatory = $true, Position = 1)]
        # Child control to add to table layout panel
        [System.Windows.Forms.Control]$Child,
        
        [Parameter(Mandatory = $true, Position = 2)]
        [ValidateRange(0, 2147483647)]
		[Alias('ColumnIndex', 'Col')]
        # Column for child control
        [int]$Column,
        
        [Parameter(Mandatory = $true, Position = 3)]
        [ValidateRange(0, 2147483647)]
		[Alias('RowIndex')]
        # Row for child control
        [int]$Row,
        
        [Parameter(Mandatory = $false)]
        [ValidateRange(1, 2147483647)]
		[Alias('ColSpan')]
        # Column Span for child control
        [int]$ColumnSpan,
        
        [Parameter(Mandatory = $false)]
        [ValidateRange(1, 2147483647)]
        # Row Span for child control
        [int]$RowSpan
    )
    <#
        .SYNOPSIS
            Add control to a table layout panel.

        .DESCRIPTION
            Adds a control to the parent container control.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
			
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
    #>
    
	Process {
		$Parent.Controls.Add($Child, $Column, $Row);
		if ($RowSpan -gt 1) { $Parent.SetRowSpan($Child, $RowSpan) }
		if ($ColumnSpan -gt 1) { $Parent.SetColumnSpan($Child, $ColumnSpan) }
		if ($Column + $ColumnSpan -gt $Parent.ColumnCount) { $Parent.ColumnCount = $Column + $ColumnSpan }
		if ($Row + $RowSpan -gt $Parent.RowCount) { $Parent.RowCount = $Row + $RowSpan }
	}
}

Function Remove-TableLayoutControl  {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
		[Alias('TableLayoutPanel', 'Panel')]
        # Parent table layout panel.
        [System.Windows.Forms.TableLayoutPanel]$Parent,
        
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true)]
        # Child control to remove from table layout panel
        [System.Windows.Forms.Control]$Child
    )
    <#
        .SYNOPSIS
            Remove control to a table layout panel.

        .DESCRIPTION
            Adds a control to the parent container control.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
		#>
    
	Process { $Parent.Controls.Remove($Child) }
	
	End {
		$MaxRow = $Parent.RowStyles.Count;
		foreach ($Control in $Parent.Controls) {
			$Value = $Parent.GetRow($Control) + $Parent.GetRowSpan($Control);
			if ($Value -gt $MaxRow) { $MaxRow = $Value}
		}
		if ($Parent.RowCount -gt $MaxRow) { $Parent.RowCount = $MaxRow }
		$MaxCol = $Parent.ColumnStyles.Count;
		foreach ($Control in $Parent.Controls) {
			$Value = $Parent.GetColumn($Control) + $Parent.GetColumnSpan($Control);
			if ($Value -gt $MaxCol) { $MaxCol = $Value}
		}
		if ($Parent.ColumnCount -gt $MaxCol) { $Parent.ColumnCount = $MaxCol }
	}
}

Function Clear-TableLayoutControls  {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
		[Alias('Panel')]
        # Table layout panel to clear all controls from.
        [System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel
    )
    <#
        .SYNOPSIS
            Remove all control from table layout panel.

        .DESCRIPTION
            Removes all controls from the parent container control.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.tablelayoutpanel.aspx
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.aspx
    #>
    
	Process {
		$TableLayoutPanel.Controls.Clear();
		if ($TableLayoutPanel.RowCount -gt $TableLayoutPanel.RowStyles.Count) { $TableLayoutPanel.RowCount = $TableLayoutPanel.RowStyles.Count }
		if ($TableLayoutPanel.ColumnCount -gt $TableLayoutPanel.ColumnStyles.Count) { $TableLayoutPanel.ColumnCount = $TableLayoutPanel.ColumnStyles.Count }
	}
}

Function New-FormsLabel {
    [CmdletBinding()]
    [OutputType([System.Windows.Forms.Label])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [ValidatePattern('^[\S]+$')]
        # Name of the control.
        [string]$Name,
        
        [Parameter(Mandatory = $true, Position = 0)]
		[AllowEmptyString()]
        # Sets the text associated with this control.
        [string]$Text,
        
        [Parameter(Mandatory = $false)]
        # A bitwise combination of the AnchorStyles values.
        [System.Windows.Forms.AnchorStyles]$Anchor,

        [Parameter(Mandatory = $false)]
        # Determines how the control should be docked within its parent container.
        [System.Windows.Forms.DockStyle]$Dock,

        [Parameter(Mandatory = $false)]
        # A Point that represents the location of the control.
        [System.Drawing.Point]$Location,
        
        [Parameter(Mandatory = $false)]
        # A Padding representing the space between controls.
        [System.Windows.Forms.Padding]$Margin,
        
        [Parameter(Mandatory = $false)]
        # A Size that represents the size of the control.
        [System.Drawing.Size]$Size,
        
        [Parameter(Mandatory = $false)]
        # Sets the Visible property to false.
        [switch]$NotVisible
    )
    <#
        .SYNOPSIS
            Create new label control.

        .DESCRIPTION
            Initializes a new instance of System.Windows.Forms.Label.
        
        .OUTPUTS
            System.Windows.Forms.Label. Represents a standard Windows label.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.label.aspx
			
        .LINK
            Set-FormControlProperties
    #>
    
    $Label = New-Object -TypeName 'System.Windows.Forms.Label';
    $Label.Name = $Name;
    $Label.Text = $Text;
    $Label.Visible = -not $NotVisible;
	if ($PSBoundParameters.ContainsKey('Visible')) { $Label.Visible = $Visible }
	if ($PSBoundParameters.ContainsKey('Anchor')) { $Label.Anchor = $Anchor }
	if ($PSBoundParameters.ContainsKey('Dock')) { $Label.Dock = $Dock }
	if ($PSBoundParameters.ContainsKey('Location')) { $Label.Location = $Location }
	if ($PSBoundParameters.ContainsKey('Margin')) { $Label.Margin = $Margin }
	if ($PSBoundParameters.ContainsKey('Size')) { $Label.Size = $Size }
    
    $Label | Write-Output;
}

Function New-DataGridView {
    [CmdletBinding()]
    [OutputType([System.Windows.Forms.DataGridView])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [ValidatePattern('^[\S]+$')]
        # Name of the control.
        [string]$Name,
        
        [Parameter(Mandatory = $false)]
        # A bitwise combination of the AnchorStyles values.
        [System.Windows.Forms.AnchorStyles]$Anchor,

        [Parameter(Mandatory = $false)]
        # Determines how the control should be docked within its parent container.
        [System.Windows.Forms.DockStyle]$Dock,

        [Parameter(Mandatory = $false)]
        # A Point that represents the location of the control.
        [System.Drawing.Point]$Location,
        
        [Parameter(Mandatory = $false)]
        # A Padding representing the space between controls.
        [System.Windows.Forms.Padding]$Margin,
        
        [Parameter(Mandatory = $false)]
        # A Size that represents the size of the control.
        [System.Drawing.Size]$Size,
        
        [Parameter(Mandatory = $false)]
        # Occurs when a key is pressed while the DataGridView has the focus.
        [ScriptBlock]$OnKeyPress,
        
        [Parameter(Mandatory = $false)]
        # Occurs when the cell content is clicked.
        [ScriptBlock]$OnCellContentClick,
        
        # Occurs when the selection is changed.
        [ScriptBlock]$OnSelectionChanged,
        
        [Parameter(Mandatory = $false)]
        # Sets the Visible p`roperty to false.
        [switch]$NotVisible,
        
        [Parameter(Mandatory = $false)]
        # The control cannot respond to user interaction.
        [switch]$Disabled,
        
        [Parameter(Mandatory = $false)]
        # The option to add rows is displayed to the user.
        [switch]$AllowUserToAddRows,
        
        [Parameter(Mandatory = $false)]
        # User is allowed to delete rows from the DataGridView.
        [switch]$AllowUserToDeleteRows,
        
        [Parameter(Mandatory = $false)]
        # Columns are created automatically when the DataSource or DataMember properties are set.
        [switch]$AutoGenerateColumns,
        
        [Parameter(Mandatory = $false)]
        # Column header row is not displayed.
        [switch]$HideColumnHeaders,
        
        [Parameter(Mandatory = $false)]
        # User is allowed to select more than one cell, row, or column of the DataGridView at a time.
        [switch]$MultiSelect,
        
        [Parameter(Mandatory = $false)]
        # User cannot edit the cells of the DataGridView control.
        [switch]$ReadOnly
    )
    <#
        .SYNOPSIS
            Create new data grid view control.

        .DESCRIPTION
            Initializes a new instance of System.Windows.Forms.DataGridView.
        
        .OUTPUTS
            System.Windows.Forms.DataGridView. Displays data in a customizable grid.
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview.aspx
			
        .LINK
			Add-DataGridViewTextBoxColumn
			
        .LINK
			Set-ScrollableControlProperties
			
        .LINK
            Set-FormControlProperties
    #>
    
    $DataGridView = New-Object -TypeName 'System.Windows.Forms.DataGridView';
    $DataGridView.Name = $Name;
	if ($PSBoundParameters.ContainsKey('Visible')) { $DataGridView.Visible = $Visible }
	if ($PSBoundParameters.ContainsKey('Anchor')) { $DataGridView.Anchor = $Anchor }
	if ($PSBoundParameters.ContainsKey('Dock')) { $DataGridView.Dock = $Dock }
	if ($PSBoundParameters.ContainsKey('Location')) { $DataGridView.Location = $Location }
	if ($PSBoundParameters.ContainsKey('Margin')) { $DataGridView.Margin = $Margin }
	if ($PSBoundParameters.ContainsKey('Size')) { $DataGridView.Size = $Size }
	if ($PSBoundParameters.ContainsKey('Visible')) { $DataGridView.Visible = $Visible }
	if ($PSBoundParameters.ContainsKey('OnKeyPress')) { $DataGridView.add_KeyPress($OnKeyPress) }
	if ($PSBoundParameters.ContainsKey('OnCellContentClick')) { $DataGridView.add_CellContentClick($OnCellContentClick) }
	if ($PSBoundParameters.ContainsKey('OnSelectionChanged')) { $DataGridView.add_SelectionChanged($OnSelectionChanged) }
	$DataGridView.AllowUserToAddRows = $AllowUserToAddRows;
	$DataGridView.AllowUserToDeleteRows = $AllowUserToDeleteRows;
	$DataGridView.AutoGenerateColumns = $AutoGenerateColumns;
	$DataGridView.ColumnHeadersVisible = -not $HideColumnHeaders;
	$DataGridView.MultiSelect = $MultiSelect;
	$DataGridView.ReadOnly = $ReadOnly;
    $DataGridView.Visible = -not $NotVisible;
    $DataGridView.Enabled = -not $Disabled;
    
    $DataGridView | Write-Output;
}

Function Add-DataGridViewTextBoxColumn {
    [CmdletBinding()]
    [OutputType([System.Windows.Forms.DataGridViewTextBoxColumn])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # DataGridView to which column is to be added.
        [System.Windows.Forms.DataGridView]$DataGridView,
        
        [Parameter(Mandatory = $true, Position = 1)]
        [ValidatePattern('^[\S]+$')]
        # Sets the name of the data source property or database column to which the DataGridViewColumn is bound.
        [string]$DataPropertyName,
        
        [Parameter(Mandatory = $false)]
        # Sets the caption text on the column's header cell.
        [string]$HeaderText,
        
        [Parameter(Mandatory = $false)]
        # Sets the mode by which the column automatically adjusts its width.
        [System.Windows.Forms.DataGridViewAutoSizeColumnMode]$AutoSizeMode,
        
        [Parameter(Mandatory = $false)]
        # Sets a value that represents the width of the column when it is in fill mode relative to the widths of other fill-mode columns in the control.
        [float]$FillWeight,
        
        [Parameter(Mandatory = $false)]
        # Sets the sort mode for the column.
        [System.Windows.Forms.DataGridViewColumnSortMode]$SortMode,
        
        [Parameter(Mandatory = $false)]
        # Width, in pixels, of column.
        [int]$Width,
        
        [Parameter(Mandatory = $false)]
        # Column will not move when a user scrolls the DataGridView control horizontally.
        [switch]$Frozen,
        
        [Parameter(Mandatory = $false)]
        # User cannot edit the column's cells.
        [switch]$ReadOnly,
        
        [Parameter(Mandatory = $false)]
        # Column is not visible.
        [switch]$NotVisible,
        
        [Parameter(Mandatory = $false)]
        # Returns the DataGridViewTextBoxColumn that was added to the DataGridView.
        [switch]$PassThru
    )
    <#
        .SYNOPSIS
            Add a new text box column to a data grid view control.

        .DESCRIPTION
            Initializes a new instance of System.Windows.Forms.DataGridViewTextBoxColumn and adds it to a DataGridView.
        
        .OUTPUTS
            System.Windows.Forms.DataGridViewTextBoxColumn. Hosts a collection of DataGridViewTextBoxCell cells.
			
        .LINK
			Add-DataGridViewButtonColumn
			
        .LINK
			New-DataGridView
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.datagridviewtextboxcolumn.aspx
			
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.datagridviewtextboxcell.aspx
    #>
    
    $DataGridViewTextBoxColumn = New-Object -TypeName 'System.Windows.Forms.DataGridViewTextBoxColumn';
    $DataGridViewTextBoxColumn.DataPropertyName = $DataPropertyName;
	if ($PSBoundParameters.ContainsKey('HeaderText')) { $DataGridViewTextBoxColumn.HeaderText = $HeaderText }
	if ($PSBoundParameters.ContainsKey('AutoSizeMode')) { $DataGridViewTextBoxColumn.AutoSizeMode = $AutoSizeMode }
	if ($PSBoundParameters.ContainsKey('FillWeight')) { $DataGridViewTextBoxColumn.FillWeight = $FillWeight }
	if ($PSBoundParameters.ContainsKey('SortMode')) { $DataGridViewTextBoxColumn.SortMode = $SortMode }
	if ($PSBoundParameters.ContainsKey('Width')) { $DataGridViewTextBoxColumn.Width = $Width }
	if ($PSBoundParameters.ContainsKey('Size')) { $DataGridViewTextBoxColumn.Size = $Size }
	$DataGridViewTextBoxColumn.Frozen = $Frozen;
	$DataGridViewTextBoxColumn.ReadOnly = $ReadOnly;
	$DataGridViewTextBoxColumn.Visible = -not $NotVisible;
    
    if ($PassThru) { $DataGridViewTextBoxColumn | Write-Output };
}

Function Add-DataGridViewButtonColumn {
    [CmdletBinding(DefaultParameterSetName = 'StaticText')]
    [OutputType([System.Windows.Forms.DataGridViewButtonColumn])]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        # DataGridView to which column is to be added.
        [System.Windows.Forms.DataGridView]$DataGridView,
        
        [Parameter(Mandatory = $true, Position = 1, ParameterSetName = 'StaticText')]
        # Sets the default text displayed on the button cell.
        [string]$Text,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'BoundText')]
        [ValidatePattern('^[\S]+$')]
        # Sets the name of the data source property or database column to which the DataGridViewColumn is bound.
        [string]$DataPropertyName,
        
        [Parameter(Mandatory = $false)]
        # Sets the mode by which the column automatically adjusts its width.
        [System.Windows.Forms.DataGridViewAutoSizeColumnMode]$AutoSizeMode = [System.Windows.Forms.DataGridViewAutoSizeColumnMode]::AllCells,
        
        [Parameter(Mandatory = $false)]
        # Sets a value that represents the width of the column when it is in fill mode relative to the widths of other fill-mode columns in the control.
        [float]$FillWeight,
        
        [Parameter(Mandatory = $false)]
        # Sets the caption text on the column's header cell.
        [string]$HeaderText,
        
        [Parameter(Mandatory = $false)]
        # Sets the sort mode for the column.
        [System.Windows.Forms.DataGridViewColumnSortMode]$SortMode = [System.Windows.Forms.DataGridViewColumnSortMode]::NotSortable,
        
        [Parameter(Mandatory = $false)]
        # Width, in pixels, of column.
        [int]$Width,
        
        [Parameter(Mandatory = $false)]
        # Column will not move when a user scrolls the DataGridView control horizontally.
        [switch]$Frozen,
        
        [Parameter(Mandatory = $false)]
        # User can edit the column's cells.
        [switch]$CanEdit,
        
        [Parameter(Mandatory = $false)]
        # Column is not visible.
        [switch]$NotVisible,
        
        [Parameter(Mandatory = $false)]
        # Returns the DataGridViewButtonColumn that was added to the DataGridView.
        [switch]$PassThru
    )
    <#
        .SYNOPSIS
            Add a new button column to a data grid view control.

        .DESCRIPTION
            Initializes a new instance of System.Windows.Forms.DataGridViewButtonColumn and adds it to a DataGridView.
        
        .OUTPUTS
            System.Windows.Forms.DataGridViewButtonColumn. Hosts a collection of DataGridViewButtonCell objects.
			
        .LINK
			Add-DataGridViewTextBoxColumn
			
        .LINK
			New-DataGridView
        
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.datagridviewbuttoncolumn.aspx
			
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.datagridviewtextboxcell.aspx
    #>
    
    $DataGridViewButtonColumn = New-Object -TypeName 'System.Windows.Forms.DataGridViewButtonColumn';
	if ($PSBoundParameters.ContainsKey('DataPropertyName')) {
		$DataGridViewButtonColumn.DataPropertyName = $DataPropertyName;
	} else {
		$DataGridViewButtonColumn.Text = $Text;
	}
	if ($PSBoundParameters.ContainsKey('HeaderText')) { $DataGridViewButtonColumn.HeaderText = $HeaderText }
	if ($PSBoundParameters.ContainsKey('Text') -or $PSBoundParameters.ContainsKey('AutoSizeMode')) { $DataGridViewButtonColumn.AutoSizeMode = $AutoSizeMode }
	if ($PSBoundParameters.ContainsKey('FillWeight')) { $DataGridViewButtonColumn.FillWeight = $FillWeight }
	if ($PSBoundParameters.ContainsKey('Text') -or $PSBoundParameters.ContainsKey('SortMode')) { $DataGridViewButtonColumn.SortMode = $SortMode }
	if ($PSBoundParameters.ContainsKey('Width')) { $DataGridViewButtonColumn.Width = $Width }
	if ($PSBoundParameters.ContainsKey('Size')) { $DataGridViewButtonColumn.Size = $Size }
	$DataGridViewButtonColumn.Frozen = $Frozen;
	$DataGridViewButtonColumn.ReadOnly = -not $CanEdit;
	$DataGridViewButtonColumn.Visible = -not $NotVisible;
    
    if ($PassThru) { $DataGridViewButtonColumn | Write-Output };
}
