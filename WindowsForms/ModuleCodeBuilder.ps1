Add-Type -AssemblyName 'System.Windows.Forms';
Add-Type -AssemblyName 'System.Drawing';
$Script:IncludedTypes = @(
    [System.Windows.Forms.Button],
    [System.Windows.Forms.CheckBox],
    [System.Windows.Forms.CheckedListBox],
    [System.Windows.Forms.ComboBox],
    [System.Windows.Forms.DataGridView],
    [System.Windows.Forms.DateTimePicker],
    [System.Windows.Forms.FlowLayoutPanel],
    [System.Windows.Forms.Form],
    [System.Windows.Forms.GroupBox],
    [System.Windows.Forms.Label],
    [System.Windows.Forms.LinkLabel],
    [System.Windows.Forms.ListBox],
    [System.Windows.Forms.MaskedTextBox],
    [System.Windows.Forms.Menu],
    [System.Windows.Forms.NumericUpDown],
    [System.Windows.Forms.ProgressBar],
    [System.Windows.Forms.RadioButton],
    [System.Windows.Forms.RichTextBox],
    [System.Windows.Forms.ScrollBar],
    [System.Windows.Forms.SplitterPanel],
    [System.Windows.Forms.WebBrowser]
);
$Script:AllTypes = @();
Function Import-Type {
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [System.Type]$Type
    )

    Process {
        if (($Script:AllTypes | Where-Object { $_.Type -eq $Type }) -eq $null) {
            for ($t = $Type.BaseType; $t -ne $null; $t = $t.BaseType) { $t | Import-Type }
            $Type.GetInterfaces() | Import-Type;
            $Script:AllTypes += @{
                Type = $Type;
                Properties = @{}
            };
        }
    }
}

Function Get-PublicGetSetProperties {
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [System.Type]$Type
    )

    Process {
        $Type.GetProperties() | Where-Object {
            $MethodInfo = $_.GetGetMethod();
            if ($MethodInfo -ne $null -and $MethodInfo.IsPublic) { $MethodInfo = $_.GetSetMethod() }
            $MethodInfo -ne $null -and $MethodInfo.IsPublic
        };
    }
}

Function Get-PropertySource {
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [System.Type]$Type,
        [Parameter(Mandatory = $true)]
        [string]$Name
    )
    
    Begin {
        $PropertySource = $null;
    }

    Process {
        if ($PropertySource -eq $null) {
            $PropertyInfo = $Type | Get-PublicGetSetProperties | Where-Object { $_.Name -eq $Name };
            if ($PropertyInfo -ne $null) {
                if ($PropertyInfo.DeclaringType.BaseType -ne $null) {
                    $PropertySource = $PropertyInfo.DeclaringType.BaseType | Get-PropertySource -Name $Name;
                }
                if ($PropertySource -eq $null) {
                    foreach ($i in $PropertyInfo.DeclaringType.GetInterfaces()) {
                        $PropertySource = $i | Get-PropertySource -Name $Name;
                        if ($PropertySource -ne $null) { break }
                    }
                }
            }
            if ($Type.BaseType -ne $null) {
                $Property = $Type.BaseType | Get-PublicGetSetProperties | Where-Object { $_.Name -eq $Name };
                if ($Property -ne $null) {
                    $PropertySource = $Property.;
                }
            }
            if ($PropertySource -eq $null) {
                foreach ($i in $Type.GetInterfaces()) {
                    $PropertySource = $i | Get-BasePropertySource -Name $Name;
                    if ($PropertySource -ne $null) { break }
                }
            }
        }
    }
    End {
        $PropertySource;
    }
}

Function Write-Type {
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [System.Type]$Type
    )

    Process {
        $
        $Type | Get-PublicGetSetProperties
    }
}

$Script:IncludedTypes | Import-Type;
$Script:AllTypes | Write-Type;