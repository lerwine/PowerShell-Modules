$ModuleBaseName = 'Erwine.Leonard.T.IOUtility';
$Script:Settings = New-Object -TypeName 'System.Management.Automation.PSObject' -Property $Script:Settings;

('System.Windows.Forms', 'System.Drawing', 'System.Data') | ForEach-Object {
	if ((Add-Type -AssemblyName $_ -PassThru -ErrorAction Stop) -eq $null) { throw ('Cannot load assembly "{0}".' -f $_) }
}

Add-Type -TypeDefinition @'
namespace Erwine.Leonard.T.IOUtility {
    public class ModuleInstallInfo {
        public string Target { get; set; }
        public bool IsInstalled { get; set; }
        public bool CanInstall { get; set; }
        public string Message { get; set; }
    }
}
'@;

Function New-Size {
	[CmdletBinding()]
	[OutputType([System.Drawing.Size])]
	Param(
		[Parameter(Mandatory = $true)]
		[int]$Width,
		
		[Parameter(Mandatory = $true)]
		[int]$Height
	)

	New-Object -TypeName 'System.Drawing.Size' -ArgumentList $Width, $Height;
}

Function Add-ColumnStyle {
	[CmdletBinding(DefaultParameterSetName = 'Size')]
	Param(
		[Parameter(Mandatory = $true)]
		[ValidateScript({ $_.Tag -ne $null -and $_.Tag -is [PSObject] -and $_.Tag.Form -ne $null -and $_.Tag.Form -is [System.Windows.Forms.Form] })]
		[System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel,
		
		[Parameter(Mandatory = $false, ParameterSetName = 'Size')]
		[Parameter(Mandatory = $true, ParameterSetName = 'Width')]
		[System.Windows.Forms.SizeType]$SizeType,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Width')]
		[float]$Width,
		
		[Parameter(Mandatory = $true)]
		[ValidateRange(0, 256)]
		[int]$Count = 1
	)

	$TableLayoutPanel.ColumnCount += $Count;

	for ($i = 0; $i -lt $Count; $i++) {
		if ($PSBoundParameters.ContainsKey('SizeType')) {
			if ($PSBoundParameters.ContainsKey('Width')) {
				$ColumnStyle = New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList $SizeType, $Width;
			} else {
				$ColumnStyle = New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList $SizeType;
			}
		} else {
			$ColumnStyle = New-Object -TypeName 'System.Windows.Forms.ColumnStyle';
		}
	
		$TableLayoutPanel.ColumnStyles.Add($ColumnStyle) | Out-Null;
	}
}

Function Add-RowStyle {
	[CmdletBinding(DefaultParameterSetName = 'Size')]
	Param(
		[Parameter(Mandatory = $true)]
		[ValidateScript({ $_.Tag -ne $null -and $_.Tag -is [PSObject] -and $_.Tag.Form -ne $null -and $_.Tag.Form -is [System.Windows.Forms.Form] })]
		[System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel,
		
		[Parameter(Mandatory = $false, ParameterSetName = 'Size')]
		[Parameter(Mandatory = $true, ParameterSetName = 'Width')]
		[System.Windows.Forms.SizeType]$SizeType,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'Width')]
		[float]$Height,
		
		[Parameter(Mandatory = $true)]
		[ValidateRange(0, 256)]
		[int]$Count = 1
	)

	$TableLayoutPanel.RowCount += $Count;

	for ($i = 0; $i -lt $Count; $i++) {
		if ($PSBoundParameters.ContainsKey('SizeType')) {
			if ($PSBoundParameters.ContainsKey('Width')) {
				$RowStyle = New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList $SizeType, $Width;
			} else {
				$RowStyle = New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList $SizeType;
			}
		} else {
			$RowStyle = New-Object -TypeName 'System.Windows.Forms.RowStyle';
		}
	
		$TableLayoutPanel.RowStyles.Add($RowStyle) | Out-Null;
	}
}

Function Add-TableLayoutPanelControl {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true)]
		[System.Windows.Forms.TableLayoutPanel]$TableLayoutPanel,
		
		[Parameter(Mandatory = $true)]
		[System.Windows.Forms.Control]$Control,
		
		[Parameter(Mandatory = $true)]
		[int]$Column,
		
		[Parameter(Mandatory = $true)]
		[int]$Row,
		
		[Parameter(Mandatory = $false)]
		[ValidateRange(0, 256)]
		[int]$ColumnSpan = 1,
		
		[Parameter(Mandatory = $false)]
		[ValidateRange(0, 256)]
		[int]$RowSpan = 1
	)
	
   $TableLayoutPanel.Controls.Add($Control, $Column, $Row);
   if ($ColumnSpan -gt 1) {  $TableLayoutPanel.SetColumnSpan($Control, $ColumnSpan) }
   if ($RowSpan -gt 1) {  $TableLayoutPanel.SetRowSpan($Control, $RowSpan) }
}

Function Apply-ControlProperties {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true)]
		[ValidateScript({ $_.Form -ne $null -and $_.Form -is [System.Windows.Forms.Form] })]
		[PSObject]$Window,
		
		[Parameter(Mandatory = $false)]
		[string]$Text,
		
		[Parameter(Mandatory = $false)]
		[System.Windows.Forms.DockStyle]$Dock,
		
		[Parameter(Mandatory = $false)]
		[System.Drawing.Color]$ForeColor,

		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitSize')]
		[int]$Width,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitSize')]
		[int]$Height,

		[Parameter(Mandatory = $false, ParameterSetName = 'ImplicitSize')]
		[switch]$NotAutoSize
	)

	$Label = New-Object -TypeName 'System.Windows.Forms.Label';
    $Label.Name = $Name;
	if ($PSCmdlet.ParameterSetName -eq 'ExplicitSize') {
		$Label.AutoSize = $false;
		$Label.Size = New-Size -Width $Width -Height $Height
	} else {
		$Label.AutoSize = -not $NotAutoSize;
	}
    $Label.Text = $Text;
    $Label.Tag = $Window;
	if ($PSBoundParameters.ContainsKey('Dock')) { $Label.Dock = $Dock }
	if ($PSBoundParameters.ContainsKey('ForeColor')) { $Label.ForeColor = $ForeColor }
	$Window.Controls | Add-Member -MemberType NoteProperty -Name $Name -Value $Label;
}

Function Add-Label {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true)]
		[ValidateScript({ $_.Form -ne $null -and $_.Form -is [System.Windows.Forms.Form] })]
		[PSObject]$Window,
		
		[Parameter(Mandatory = $true)]
		[string]$Name,
		
		[Parameter(Mandatory = $true)]
		[string]$Text,
		
		[Parameter(Mandatory = $false)]
		[System.Windows.Forms.DockStyle]$Dock,
		
		[Parameter(Mandatory = $false)]
		[System.Drawing.Color]$ForeColor,

		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitSize')]
		[int]$Width,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitSize')]
		[int]$Height,

		[Parameter(Mandatory = $false, ParameterSetName = 'ImplicitSize')]
		[switch]$NotAutoSize
	)

	$Label = New-Object -TypeName 'System.Windows.Forms.Label';
    $Label.Name = $Name;
	if ($PSCmdlet.ParameterSetName -eq 'ExplicitSize') {
		$Label.AutoSize = $false;
		$Label.Size = New-Size -Width $Width -Height $Height
	} else {
		$Label.AutoSize = -not $NotAutoSize;
	}
    $Label.Text = $Text;
    $Label.Tag = $Window;
	if ($PSBoundParameters.ContainsKey('Dock')) { $Label.Dock = $Dock }
	if ($PSBoundParameters.ContainsKey('ForeColor')) { $Label.ForeColor = $ForeColor }
	$Window.Controls | Add-Member -MemberType NoteProperty -Name $Name -Value $Label;
}

Function Add-TableLayoutPanel {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true)]
		[ValidateScript({ $_.Form -ne $null -and $_.Form -is [System.Windows.Forms.Form] })]
		[PSObject]$Window,
		
		[Parameter(Mandatory = $true)]
		[string]$Name,
		
		[Parameter(Mandatory = $false)]
		[System.Windows.Forms.DockStyle]$Dock,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitSize')]
		[int]$Width,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitSize')]
		[int]$Height,

		[Parameter(Mandatory = $false)]
		[switch]$SuspendLayout,
		
		[Parameter(Mandatory = $false, ParameterSetName = 'ImplicitSize')]
		[switch]$NotAutoSize
	)

	$TableLayoutPanel = New-Object -TypeName 'System.Windows.Forms.TableLayoutPanel';
    $TableLayoutPanel.Name = $Name;
	if ($PSCmdlet.ParameterSetName -eq 'ExplicitSize') {
		$TableLayoutPanel.AutoSize = $false;
		$TableLayoutPanel.Size = New-Size -Width $Width -Height $Height
	} else {
		$TableLayoutPanel.AutoSize = -not $NotAutoSize;
	}
    $TableLayoutPanel.Tag = $Window;
	if ($SuspendLayout) { $TableLayoutPanel.SuspendLayout() }
	if ($PSBoundParameters.ContainsKey('Dock')) { $TableLayoutPanel.Dock = $Dock }
	$Window.Controls | Add-Member -MemberType NoteProperty -Name $Name -Value $TableLayoutPanel;
}

Function Add-DataGridView {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true)]
		[ValidateScript({ $_.Form -ne $null -and $_.Form -is [System.Windows.Forms.Form] })]
		[PSObject]$Window,
		
		[Parameter(Mandatory = $true)]
		[string]$Name,
		
		[Parameter(Mandatory = $false)]
		[System.Windows.Forms.DockStyle]$Dock,
		
		[Parameter(Mandatory = $false)]
		[System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode]$ColumnHeadersHeightSizeMode = [System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode]::AutoSize,
		
		[Parameter(Mandatory = $false)]
		[System.Windows.Forms.DataGridViewSelectionMode]$SelectionMode = [System.Windows.Forms.DataGridViewSelectionMode]::FullRowSelect,
		
		[Parameter(Mandatory = $false)]
		[int]$TabIndex,
		
		[Parameter(Mandatory = $false)]
		[ScriptBlock]$OnSelectionChanged,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitSize')]
		[int]$Width,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitSize')]
		[int]$Height,
		
		[Parameter(Mandatory = $false)]
		[switch]$NotTabStop,
		
		[Parameter(Mandatory = $false, ParameterSetName = 'ImplicitSize')]
		[switch]$NotAutoSize,
		
		[Parameter(Mandatory = $false)]
		[switch]$AllowUserToAddRows,
		
		[Parameter(Mandatory = $false)]
		[switch]$AllowUserToDeleteRows,
		
		[Parameter(Mandatory = $false)]
		[switch]$CanEdit,
		
		[Parameter(Mandatory = $false)]
		[switch]$BeginInit
	)

	$DataGridView = New-Object -TypeName 'System.Windows.Forms.DataGridView';
    $DataGridView.Name = $Name;
    $DataGridView.TabStop = -not $NotTabStop;
    $DataGridView.Tag = $Window;
	if ($BeginInit) { $DataGridView.BeginInit() }
	if ($PSBoundParameters.ContainsKey('Dock')) { $DataGridView.Dock = $Dock }
	if ($PSBoundParameters.ContainsKey('TabIndex')) { $DataGridView.TabIndex = $TabIndex }
	if ($PSCmdlet.ParameterSetName -eq 'ExplicitSize') {
		$DataGridView.AutoSize = $false;
		$DataGridView.Size = New-Size -Width $Width -Height $Height
	} else {
		$DataGridView.AutoSize = -not $NotAutoSize;
	}
    $DataGridView.AllowUserToAddRows = $AllowUserToAddRows;
    $DataGridView.AllowUserToDeleteRows = $AllowUserToDeleteRows;
    $DataGridView.ColumnHeadersHeightSizeMode = $ColumnHeadersHeightSizeMode;
    $DataGridView.ReadOnly = -not $CanEdit;
    $DataGridView.SelectionMode = $SelectionMode;
	$DataGridView.Tag = $Window;
	$Window.Controls | Add-Member -MemberType NoteProperty -Name $Name -Value $DataGridView;
	if ($PSBoundParameters.ContainsKey('OnSelectionChanged')) {
		$Window | Add-Member -MemberType ScriptMethod -Name ('_{0}_SelectionChanged' -f $Name) -Value $OnSelectionChanged;
		$DataGridView.add_SelectionChanged({
			Param(
				[Parameter(Mandatory = $true, Position = 0)]
				[System.Windows.Forms.DataGridView]$sender,
				[Parameter(Mandatory = $true, Position = 1)]
				[System.EventArgs]$e
			)
			$sender.Tag.('_{0}_SelectionChanged' -f $sender.Name)($sender, $e);
		});
	}
}

Function Add-DataGridViewTextBoxColumn {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true)]
		[ValidateScript({ $_.Tag -ne $null -and $_.Tag -is [PSObject] -and $_.Tag.Form -ne $null -and $_.Tag.Form -is [System.Windows.Forms.Form] })]
		[System.Windows.Forms.DataGridView]$DataGridView,
		
		[Parameter(Mandatory = $true)]
		[string]$Name,
		
		[Parameter(Mandatory = $true)]
		[string]$DataPropertyName,
		
		[Parameter(Mandatory = $false)]
		[string]$HeaderText,
		
		[Parameter(Mandatory = $false)]
		[System.Windows.Forms.DataGridViewAutoSizeColumnMode]$AutoSizeMode = [System.Windows.Forms.DataGridViewAutoSizeColumnMode]::AllCells,
		
		[Parameter(Mandatory = $false)]
		[int]$Width,
		
		[Parameter(Mandatory = $false)]
		[switch]$CanEdit
	)
	
	$DataGridViewTextBoxColumn = New-Object -TypeName 'System.Windows.Forms.DataGridViewTextBoxColumn';
    $DataGridViewTextBoxColumn.Name = $Name;
    $DataGridViewTextBoxColumn.DataPropertyName = $DataPropertyName;
    $DataGridViewTextBoxColumn.AutoSizeMode = $AutoSizeMode;
    $DataGridViewTextBoxColumn.ReadOnly = -not $CanEdit;
	if ($PSBoundParameters.ContainsKey('Width')) { $DataGridViewTextBoxColumn.Width = $Width }
	$DataGridView.Columns.Add($DataGridViewTextBoxColumn) | Out-Null;
	$DataGridView.Tag.Controls | Add-Member -MemberType NoteProperty -Name $Name -Value $DataGridViewTextBoxColumn;
}

Function Add-Button {
	[CmdletBinding(DefaultParameterSetName = 'ImplicitSize')]
	Param(
		[Parameter(Mandatory = $true)]
		[ValidateScript({ $_.Form -ne $null -and $_.Form -is [System.Windows.Forms.Form] })]
		[PSObject]$Window,
		
		[Parameter(Mandatory = $true)]
		[string]$Name,
		
		[Parameter(Mandatory = $true)]
		[string]$Text,
		
		[Parameter(Mandatory = $false)]
		[System.Windows.Forms.DockStyle]$Dock,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitSize')]
		[int]$Width,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitSize')]
		[int]$Height,
		
		[Parameter(Mandatory = $false)]
		[int]$TabIndex,
		
		[Parameter(Mandatory = $false)]
		[ScriptBlock]$OnClick,
		
		[Parameter(Mandatory = $false)]
		[switch]$NotTabStop,
		
		[Parameter(Mandatory = $false, ParameterSetName = 'ImplicitSize')]
		[switch]$NotAutoSize,
		
		[Parameter(Mandatory = $false)]
		[switch]$DoNotUseVisualStyleBackColor
	)

	$Button = New-Object -TypeName 'System.Windows.Forms.Button';
    $Button.Name = $Name;
	if ($PSCmdlet.ParameterSetName -eq 'ExplicitSize') {
		$Button.AutoSize = $false;
		$Button.Size = New-Size -Width $Width -Height $Height
	} else {
		$Button.AutoSize = -not $NotAutoSize;
	}
    $Button.Text = $Text;
    $Button.TabStop = -not $NotTabStop;
    $Button.UseVisualStyleBackColor = -not $DoNotUseVisualStyleBackColor;
    $Button.Tag = $Window;
	if ($PSBoundParameters.ContainsKey('Dock')) { $Button.Dock = $Dock }
	if ($PSBoundParameters.ContainsKey('TabIndex')) { $Button.TabIndex = $TabIndex }
	$Window.Controls | Add-Member -MemberType NoteProperty -Name $Name -Value $Button;
	if ($PSBoundParameters.ContainsKey('OnClick')) {
		$Window | Add-Member -MemberType ScriptMethod -Name ('_{0}_Click' -f $Name) -Value $OnClick;
		$Button.add_Click({
			Param(
				[Parameter(Mandatory = $true, Position = 0)]
				[System.Windows.Forms.Button]$sender,
				[Parameter(Mandatory = $true, Position = 1)]
				[System.EventArgs]$e
			)
			$sender.Tag.('_{0}_Click' -f $sender.Name)($sender, $e);
		});
	}
}

Function Add-RadioButton {
	[CmdletBinding(DefaultParameterSetName = 'ImplicitSize')]
	Param(
		[Parameter(Mandatory = $true)]
		[ValidateScript({ $_.Form -ne $null -and $_.Form -is [System.Windows.Forms.Form] })]
		[PSObject]$Window,
		
		[Parameter(Mandatory = $true)]
		[string]$Name,
		
		[Parameter(Mandatory = $true)]
		[AllowEmptyString()]
		[string]$Text,
		
		[Parameter(Mandatory = $false)]
		[System.Windows.Forms.DockStyle]$Dock,
		
		[Parameter(Mandatory = $false)]
		[int]$TabIndex,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitSize')]
		[int]$Width,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitSize')]
		[int]$Height,

		[Parameter(Mandatory = $false)]
		[ScriptBlock]$OnCheckedChanged,
		
		[Parameter(Mandatory = $false)]
		[switch]$Checked,
		
		[Parameter(Mandatory = $false, ParameterSetName = 'ImplicitSize')]
		[switch]$NotAutoSize,
		
		[Parameter(Mandatory = $false)]
		[switch]$NotTabStop,
		
		[Parameter(Mandatory = $false)]
		[switch]$DoNotUseVisualStyleBackColor
	)

	$RadioButton = New-Object -TypeName 'System.Windows.Forms.RadioButton';
    $RadioButton.Name = $Name;
    $RadioButton.Checked = $Checked;
    $RadioButton.TabStop = -not $NotTabStop;
	if ($PSCmdlet.ParameterSetName -eq 'ExplicitSize') {
		$RadioButton.AutoSize = $false;
		$RadioButton.Size = New-Size -Width $Width -Height $Height
	} else {
		$RadioButton.AutoSize = -not $NotAutoSize;
	}
    $RadioButton.Text = $Text;
    $RadioButton.UseVisualStyleBackColor = -not $DoNotUseVisualStyleBackColor;
    $RadioButton.Tag = $Window;
	if ($PSBoundParameters.ContainsKey('Dock')) { $RadioButton.Dock = $Dock }
	if ($PSBoundParameters.ContainsKey('TabIndex')) { $RadioButton.TabIndex = $TabIndex }
	$Window.Controls | Add-Member -MemberType NoteProperty -Name $Name -Value $RadioButton;
	if ($PSBoundParameters.ContainsKey('OnCheckedChanged')) {
		$Window | Add-Member -MemberType ScriptMethod -Name ('_{0}_CheckedChanged' -f $Name) -Value $OnCheckedChanged;
		$RadioButton.add_CheckedChanged({
			Param(
				[Parameter(Mandatory = $true, Position = 0)]
				[System.Windows.Forms.RadioButton]$sender,
				[Parameter(Mandatory = $true, Position = 1)]
				[System.EventArgs]$e
			)
			$sender.Tag.('_{0}_CheckedChanged' -f $sender.Name)($sender, $e);
		});
	}
}

Function Add-CheckBox {
	[CmdletBinding(DefaultParameterSetName = 'ImplicitSize')]
	Param(
		[Parameter(Mandatory = $true)]
		[ValidateScript({ $_.Form -ne $null -and $_.Form -is [System.Windows.Forms.Form] })]
		[PSObject]$Window,
		
		[Parameter(Mandatory = $true)]
		[string]$Name,
		
		[Parameter(Mandatory = $true)]
		[AllowEmptyString()]
		[string]$Text,
		
		[Parameter(Mandatory = $false)]
		[System.Windows.Forms.DockStyle]$Dock,
		
		[Parameter(Mandatory = $false)]
		[int]$TabIndex,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitSize')]
		[int]$Width,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitSize')]
		[int]$Height,

		[Parameter(Mandatory = $false)]
		[ScriptBlock]$OnCheckedChanged,
		
		[Parameter(Mandatory = $false)]
		[switch]$Checked,
		
		[Parameter(Mandatory = $false, ParameterSetName = 'ImplicitSize')]
		[switch]$NotAutoSize,
		
		[Parameter(Mandatory = $false)]
		[switch]$NotTabStop,
		
		[Parameter(Mandatory = $false)]
		[switch]$DoNotUseVisualStyleBackColor
	)

	$CheckBox = New-Object -TypeName 'System.Windows.Forms.CheckBox';
    $CheckBox.Name = $Name;
    $CheckBox.Checked = $Checked;
    $CheckBox.TabStop = -not $NotTabStop;
	if ($PSCmdlet.ParameterSetName -eq 'ExplicitSize') {
		$CheckBox.AutoSize = $false;
		$CheckBox.Size = New-Size -Width $Width -Height $Height
	} else {
		$CheckBox.AutoSize = -not $NotAutoSize;
	}
    $CheckBox.Text = $Text;
    $CheckBox.UseVisualStyleBackColor = -not $DoNotUseVisualStyleBackColor;
    $CheckBox.Tag = $Window;
	if ($PSBoundParameters.ContainsKey('Dock')) { $CheckBox.Dock = $Dock }
	if ($PSBoundParameters.ContainsKey('TabIndex')) { $CheckBox.TabIndex = $TabIndex }
	$Window.Controls | Add-Member -MemberType NoteProperty -Name $Name -Value $CheckBox;
	if ($PSBoundParameters.ContainsKey('OnCheckedChanged')) {
		$Window | Add-Member -MemberType ScriptMethod -Name ('_{0}_CheckedChanged' -f $Name) -Value $OnCheckedChanged;
		$CheckBox.add_CheckedChanged({
			Param(
				[Parameter(Mandatory = $true, Position = 0)]
				[System.Windows.Forms.CheckBox]$sender,
				[Parameter(Mandatory = $true, Position = 1)]
				[System.EventArgs]$e
			)
			$sender.Tag.('_{0}_CheckedChanged' -f $sender.Name)($sender, $e);
		});
	}
}

Function Add-TextBox {
	[CmdletBinding(DefaultParameterSetName = 'ImplicitSize')]
	Param(
		[Parameter(Mandatory = $true)]
		[ValidateScript({ $_.Form -ne $null -and $_.Form -is [System.Windows.Forms.Form] })]
		[PSObject]$Window,
		
		[Parameter(Mandatory = $true)]
		[string]$Name,
		
		[Parameter(Mandatory = $false)]
		[AllowEmptyString()]
		[string]$Text,
		
		[Parameter(Mandatory = $false)]
		[System.Windows.Forms.DockStyle]$Dock,
		
		[Parameter(Mandatory = $false)]
		[int]$TabIndex,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitSize')]
		[int]$Width,
		
		[Parameter(Mandatory = $true, ParameterSetName = 'ExplicitSize')]
		[int]$Height,
		
		[Parameter(Mandatory = $false)]
		[ScriptBlock]$OnTextChanged,
		
		[Parameter(Mandatory = $false)]
		[ScriptBlock]$OnLeave,
		
		[Parameter(Mandatory = $false)]
		[switch]$ReadOnly,
		
		[Parameter(Mandatory = $false, ParameterSetName = 'ImplicitSize')]
		[switch]$NotAutoSize,
		
		[Parameter(Mandatory = $false)]
		[switch]$NotTabStop,
		
		[Parameter(Mandatory = $false)]
		[switch]$DoNotUseVisualStyleBackColor
	)

	$TextBox = New-Object -TypeName 'System.Windows.Forms.TextBox';
    $TextBox.Name = $Name;
    $TextBox.TabStop = -not $NotTabStop;
	if ($PSCmdlet.ParameterSetName -eq 'ExplicitSize') {
		$TextBox.AutoSize = $false;
		$TextBox.Size = New-Size -Width $Width -Height $Height
	} else {
		$TextBox.AutoSize = -not $NotAutoSize;
	}
	if ($PSBoundParameters.ContainsKey('Text')) { $TextBox.Text = $Text }
    $TextBox.ReadOnly = $ReadOnly;
    $TextBox.UseVisualStyleBackColor = -not $DoNotUseVisualStyleBackColor;
    $TextBox.Tag = $Window;
	if ($PSBoundParameters.ContainsKey('Dock')) { $TextBox.Dock = $Dock }
	if ($PSBoundParameters.ContainsKey('TabIndex')) { $TextBox.TabIndex = $TabIndex }
	$Window.Controls | Add-Member -MemberType NoteProperty -Name $Name -Value $TextBox;
	if ($PSBoundParameters.ContainsKey('OnTextChanged')) {
		$Window | Add-Member -MemberType ScriptMethod -Name ('_{0}_TextChanged' -f $Name) -Value $OnTextChanged;
		$TextBox.add_TextChanged({
			Param(
				[Parameter(Mandatory = $true, Position = 0)]
				[System.Windows.Forms.TextBox]$sender,
				[Parameter(Mandatory = $true, Position = 1)]
				[System.EventArgs]$e
			)
			$sender.Tag.('_{0}_TextChanged' -f $sender.Name)($sender, $e);
		});
	}
	if ($PSBoundParameters.ContainsKey('OnLeave')) {
		$Window | Add-Member -MemberType ScriptMethod -Name ('_{0}_Leave' -f $Name) -Value $OnLeave;
		$TextBox.add_Leave({
			Param(
				[Parameter(Mandatory = $true, Position = 0)]
				[System.Windows.Forms.TextBox]$sender,
				[Parameter(Mandatory = $true, Position = 1)]
				[System.EventArgs]$e
			)
			$sender.Tag.('_{0}_Leave' -f $sender.Name)($sender, $e);
		});
	}
}

Function New-WindowObject {
	[CmdletBinding()]
	Param(
		[Parameter(Mandatory = $true)]
		[string]$Name,
		
		[Parameter(Mandatory = $true)]
		[string]$Title,
		
		[Parameter(Mandatory = $false)]
		[ScriptBlock]$OnShown
	)

	$Window = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
		Form = New-Object -TypeName 'System.Windows.Forms.Form';
		Controls = New-Object -TypeName 'System.Management.Automation.PSObject';
	}
	$Window.Controls | Add-Member -MemberType NoteProperty -Name $Name -Value $Window.Form;
    $Window.Form.SuspendLayout();
	$Window.Form.Name = $Name;
	$Window.Form.Title = $Title;
	$Window.Form.Tag = $Window;
	$Window | Add-Member -MemberType ScriptMethod -Name 'ShowDialog' -Value {
		$this.Form.ShowDialog();
	}
	$Window | Add-Member -MemberType ScriptMethod -Name 'Close' -Value {
		Param(
			[Parameter(Mandatory = $true, Position = 0)]
			[System.Windows.Forms.DialogResult]$DialogResult
		)
		$this.DialogResult = $DialogResult;
		$this.Form.Close();
	}
	$Window | Add-Member -MemberType ScriptProperty -Name 'Title' -Value { return $this.Form.Title; } -SecondValue {
		Param(
			[Parameter(Mandatory = $true, Position = 0)]
			[string]$Title
		)
		$this.Form.Title = $Title
	};
	$Window | Add-Member -MemberType ScriptProperty -Name 'DialogResult' -Value { return $this.Form.DialogResult; } -SecondValue {
		Param(
			[Parameter(Mandatory = $true, Position = 0)]
			[System.Windows.Forms.DialogResult]$DialogResult
		)
		$this.Form.DialogResult = $DialogResult
	};
	if ($PSBoundParameters.ContainsKey('OnShown')) {
		$Window | Add-Member -MemberType ScriptMethod -Name ('_{0}_Shown' -f $Name) -Value $OnShown;
		$Window.add_Shown({
			Param(
				[Parameter(Mandatory = $true, Position = 0)]
				[System.Windows.Forms.TextBox]$sender,
				[Parameter(Mandatory = $true, Position = 1)]
				[System.EventArgs]$e
			)
			$sender.Tag.('_{0}_Shown' -f $Name)($sender, $e);
		});
	}
	$Window | Write-Output;
}

$Window = New-WindowObject -Name 'InstallLocationSelectForm' -Title 'Select installation location' -OnShown {
	$this.Form.BringToFront();
};
Add-TableLayoutPanel -Window $Window -Name 'outerTableLayoutPanel' -Dock Fill -TabIndex 0 -SuspendLayout;
Add-ColumnStyle -TableLayoutPanel $Window.outerTableLayoutPanel -SizeType Percent -Width 100;
Add-ColumnStyle -TableLayoutPanel $Window.outerTableLayoutPanel -Count 2;
Add-RowStyle -TableLayoutPanel $Window.outerTableLayoutPanel -Count 2;
Add-RowStyle -TableLayoutPanel $Window.outerTableLayoutPanel -SizeType Percent -Height 100;
Add-RowStyle -TableLayoutPanel $Window.outerTableLayoutPanel -Count 3;
Add-RowStyle -TableLayoutPanel $Window.outerTableLayoutPanel -SizeType Percent -Height 100;
Add-RowStyle -TableLayoutPanel $Window.outerTableLayoutPanel;
Add-Label -Window $Window -Name 'instructionsLabel' -Dock Fill -Text 'Select installation location.' -TabIndex 2;
Add-TableLayoutPanelControl -TableLayoutPanel $Window.outerTableLayoutPanel -Control $Window.instructionsLabel -Row 0 -Column 0 -ColumnSpan 3;
Add-TableLayoutPanel -Window $Window -Name 'locationTypeTableLayoutPanel' -Dock Fill -TabIndex 0 -SuspendLayout;
Add-ColumnStyle -TableLayoutPanel $Window.locationTypeTableLayoutPanel -SizeType Percent -Width 100;
Add-ColumnStyle -TableLayoutPanel $Window.locationTypeTableLayoutPanel;
Add-RowStyle -TableLayoutPanel $Window.locationTypeTableLayoutPanel;
Add-TableLayoutPanelControl -TableLayoutPanel $Window.outerTableLayoutPanel -Control $Window.locationTypeTableLayoutPanel -Row 1 -Column 0 -ColumnSpan 3;
Add-RadioButton -Window $Window -Name 'commonLocationsRadioButton' -Dock Fill -Text 'Common Locations' -Checked -TabIndex 3 -OnCheckedChanged {

};
Add-TableLayoutPanelControl -TableLayoutPanel $Window.locationTypeTableLayoutPanel -Control $Window.commonLocationsRadioButton -Row 0 -Column 0;
Add-RadioButton -Window $Window -Name 'customLocationadioButton' -Dock Fill -Text 'Custom Location' -TabIndex 1 -OnCheckedChanged {

};
Add-TableLayoutPanelControl -TableLayoutPanel $Window.locationTypeTableLayoutPanel -Control $Window.customLocationadioButton -Row 0 -Column 0;
Add-DataGridView -Window $Window -Name 'installLocationsDataGridView' -Dock Fill -TabIndex 5 -BeginInit -OnSelectionChanged {

};
Add-TableLayoutPanelControl -TableLayoutPanel $Window.outerTableLayoutPanel -Control $Window.installLocationsDataGridView -Row 2 -Column 0 -ColumnSpan 3;
Add-DataGridViewTextBoxColumn -DataGridView $Window.installLocationsDataGridView -Name 'pathTextBoxColumn' -DataPropertyName 'Path' -HeaderText 'Path';
Add-DataGridViewTextBoxColumn -DataGridView $Window.installLocationsDataGridView -Name 'isInstalledTextBoxColumn' -DataPropertyName 'IsInstalledText' -HeaderText 'Is Installed';
Add-DataGridViewTextBoxColumn -DataGridView $Window.installLocationsDataGridView -Name 'canInstallTextBoxColumn' -DataPropertyName 'CanBeInstalledText' -HeaderText 'Can Install';
Add-DataGridViewTextBoxColumn -DataGridView $Window.installLocationsDataGridView -Name 'isAllUsersTextBoxColumn' -DataPropertyName 'IsAllUsersText' -HeaderText 'All Users';
Add-DataGridViewTextBoxColumn -DataGridView $Window.installLocationsDataGridView -Name 'messageTextboxColumn' -DataPropertyName 'Message' -HeaderText 'Message' -AutoSizeMode Fill;

Add-TextBox -Window $Window -Name 'customPathTextBox' -Dock Top -TabIndex 7 -OnTextChanged {

} -OnLeave {

};
Add-TableLayoutPanelControl -TableLayoutPanel $Window.outerTableLayoutPanel -Control $Window.customPathTextBox -Row 4 -Column 0;

Add-Button -Window $Window -Name 'browseButton' -Text 'Browse' -Width 75 -Height 23 -TabIndex 8 -OnClick {

};
Add-TableLayoutPanelControl -TableLayoutPanel $Window.outerTableLayoutPanel -Control $Window.browseButton -Row 4 -Column 2;

Add-RadioButton -Window $Window -Name 'overwriteCheckBox' -Dock Fill -Text 'Overwrite' -TabIndex 11 -OnCheckedChanged {
};
Add-TableLayoutPanelControl -TableLayoutPanel $Window.outerTableLayoutPanel -Control $Window.overwriteCheckBox -Row 5 -Column 0 -ColumnSpan 3;

Add-Label -Window $Window -Name 'pathErrorLabel' -Text 'Path must be specified.' -Dock Fill -ForeColor ([System.Drawing.Color]::DarkRed);
Add-TableLayoutPanelControl -TableLayoutPanel $Window.outerTableLayoutPanel -Control $Window.pathErrorLabel -Row 6 -Column 0 -ColumnSpan 2;

Add-Button -Window $Window -Name 'actionButton' -Text 'Uninstall' -Width 75 -Height 23 -TabIndex 10 -OnClick {
};
Add-TableLayoutPanelControl -TableLayoutPanel $Window.outerTableLayoutPanel -Control $Window.actionButton -Row 7 -Column 0;

Add-Button -Window $Window -Name 'continueButton' -Text 'Continue' -Width 75 -Height 23 -TabIndex 3 -OnClick {
};
Add-TableLayoutPanelControl -TableLayoutPanel $Window.outerTableLayoutPanel -Control $Window.continueButton -Row 7 -Column 1;

Add-Button -Window $Window -Name 'cancelButton' -Text 'Cancel' -Width 75 -Height 23 -TabIndex 4 -OnClick {
};
Add-TableLayoutPanelControl -TableLayoutPanel $Window.outerTableLayoutPanel -Control $Window.cancelButton -Row 7 -Column 2;

$Data = New-Object -TypeName 'System.ComponentModel.BindingList[Erwine.Leonard.T.IOUtility.ModuleInstallInfo]';
foreach ($modulePath in $env:PSModulePath.Split([System.IO.Path]::PathSeparator)) {
	$ModuleInstallInfo = New-Object -TypeName 'Erwine.Leonard.T.IOUtility.ModuleInstallInfo';
    $ModuleInstallInfo.Target = $modulePath | Join-Path -ChildPath 'ModuleBaseName';
    if ((Test-Path -Path $modulePath -PathType Container)) {
        if ((Test-Path -Path $ModuleInstallInfo.Target -PathType Container)) {
            switch (@(('.psm1', 'psd1') | ForEach-Object { $ModuleInstallInfo.Target | Join-Path -ChildPath ($ModuleBaseName + $_) } | Where-Object { Test-Path -Path $_ -PathType Leaf }).Count) {
                0 {
                    $ModuleInstallInfo.CanInstall = $false;
                    $ModuleInstallInfo.IsInstalled = $false;
                    $ModuleInstallInfo.Message = 'Folder of same name must be manually removed.';
                    break;
                }
                1 {
                    $ModuleInstallInfo.CanInstall = $false;
                    $ModuleInstallInfo.IsInstalled = $false;
                    $ModuleInstallInfo.Message = 'Incomplete installation must be manually removed.';
                    break;
                }
                default {
                    $ModuleInstallInfo.CanInstall = $true;
                    $ModuleInstallInfo.IsInstalled = $true;
                    $ModuleInstallInfo.Message = 'Module is currently installed at this location.';
                    break;
                }
            }
        } else {
            $ModuleInstallInfo.CanInstall = $true;
            $ModuleInstallInfo.IsInstalled = $true;
            $ModuleInstallInfo.Message = 'Module can be installed at this location.';
        }
    } else {
        $ModuleInstallInfo.CanInstall = $false;
        $ModuleInstallInfo.IsInstalled = $false;
        $ModuleInstallInfo.Message = 'Module root path must be manually created.';
    }
    $Data.Add($ModuleInstallInfo);
};

$CanUninstall = New-Object -TypeName 'System.ComponentModel.BindingList[Erwine.Leonard.T.IOUtility.ModuleInstallInfo]';
$Data | Where-Object { $_.IsInstalled } | ForEach-Object { $CanUninstall.Add($_) }


$CanInstall = New-Object -TypeName 'System.ComponentModel.BindingList[Erwine.Leonard.T.IOUtility.ModuleInstallInfo]';
$Data | Where-Object { $_.CanInstall } | ForEach-Object { $CanInstall.Add($_) }


$Form = New-Object -TypeName 'System.Windows.Forms.Form';
$TableLayoutPanel = New-Object -TypeName 'System.Windows.Forms.TableLayoutPanel';
$TableLayoutPanel.Dock = [System.Windows.Forms.DockStyle]::Fill;
$Form.Controls.Add($TableLayoutPanel) | Out-Null;
$TableLayoutPanel.ColumnCount = 2;
$TableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Percent, 100.0))) | Out-Null;
$TableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
$TableLayoutPanel.RowCount = 3;
$TableLayoutPanel.RowStyles.Add((New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
$TableLayoutPanel.RowStyles.Add((New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Percent, 100.0))) | Out-Null;
$TableLayoutPanel.RowStyles.Add((New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
$Label = New-Object -TypeName 'System.Windows.Forms.Label';
$Label.Dock = [System.Windows.Forms.DockStyle]::Fill;
$Label.Margin = New-Object -TypeName 'System.Windows.Forms.Padding' -ArgumentList 8, 8, 8, 0
$Label.Text = 'Click "Install" for the location where you wish to install this module.';
$TableLayoutPanel.Controls.Add($Label, 0, 0) | Out-Null;
$TableLayoutPanel.SetColumnSpan($Label, 2);
$DataGridView = New-Object -TypeName 'System.Windows.Forms.DataGridView';
$DataGridView.AllowUserToAddRows = $false;
$DataGridView.AllowUserToDeleteRows = $false;
$DataGridView.AutoGenerateColumns = $false;
$DataGridView.ColumnHeadersVisible = $false;
$DataGridView.Dock = [System.Windows.Forms.DockStyle]::Fill;
$DataGridView.Margin = New-Object -TypeName 'System.Windows.Forms.Padding' -ArgumentList 8, 8, 8, 0;
$DataGridView.MultiSelect = $false;
$DataGridView.ReadOnly = $true;
$TableLayoutPanel.Controls.Add($DataGridView, 0, 1) | Out-Null;
$TableLayoutPanel.SetColumnSpan($DataGridView, 2);
$DataGridViewTextBoxColumn = New-Object -TypeName 'System.Windows.Forms.DataGridViewTextBoxColumn';
$DataGridViewTextBoxColumn.AutoSizeMode = [System.Windows.Forms.DataGridViewAutoSizeColumnMode]::Fill;
$DataGridViewTextBoxColumn.DataPropertyName = 'Target';
$DataGridViewTextBoxColumn.ReadOnly = $true;
$DataGridViewTextBoxColumn.SortMode = [System.Windows.Forms.DataGridViewColumnSortMode]::Automatic;
$DataGridView.Columns.Add($DataGridViewTextBoxColumn) | Out-Null;
$DataGridViewButtonColumn = New-Object -TypeName 'System.Windows.Forms.DataGridViewButtonColumn';
$DataGridViewButtonColumn.AutoSizeMode = [System.Windows.Forms.DataGridViewAutoSizeColumnMode]::AllCells;
$DataGridViewButtonColumn.ReadOnly = $true;
$DataGridViewButtonColumn.SortMode = [System.Windows.Forms.DataGridViewColumnSortMode]::NotSortable;
$DataGridViewButtonColumn.Text = 'Install';
$DataGridView.Columns.Add($DataGridViewButtonColumn) | Out-Null;
$DataGridView.DataSource = $CanInstall;
$DataGridView.add_KeyPress({
    Param(
        [Parameter(Position = 0)]
        [System.Windows.Forms.DataGridView]$Sender,
        
        [Parameter(Position = 1)]
        [System.Windows.Forms.KeyEventArgs]$KeyEventArgs
    )

    if ($Sender.SelectedRows.Count -gt 0 -and $KeyEventArgs.KeyCode -eq [System.Windows.Forms.Keys]::Enter -and (-not ($KeyEventArgs.Alt -or $KeyEventArgs.Control -or $KeyEventArgs.Shift))) {
        [System.Windows.Forms.Form]$Form = $Sender.Parent.Parent;
        $Form.Tag = $Sender.SelectedRows[0].DataBoundItem;
        $Form.DialogResult = [System.Windows.Forms.DialogResult]::OK;
        $Form.Close();
    }
});
$DataGridView.add_CellContentClick({
    Param(
        [Parameter(Position = 0)]
        [System.Windows.Forms.DataGridView]$Sender,
        
        [Parameter(Position = 1)]
        [System.Windows.Forms.DataGridViewCellEventArgs]$DataGridViewCellEventArgs
    )

    if ($DataGridViewCellEventArgs.ColumnIndex -ge 0 -and $DataGridViewCellEventArgs.RowIndex -ge 0 -and $DataGridViewCellEventArgs.ColumnIndex -lt $Sender.ColumnCount -and $DataGridViewCellEventArgs.RowIndex -lt $Sender.RowCount) {
        $DataGridViewColumn = $Sender.Columns[$DataGridViewCellEventArgs.ColumnIndex];
        if ($DataGridViewColumn -is [System.Windows.Forms.DataGridViewButtonColumn]) {
            [System.Windows.Forms.Form]$Form = $Sender.Parent.Parent;
            $Form.Tag = $Sender.Rows[$DataGridViewCellEventArgs.RowIndex].DataBoundItem;
            $Form.DialogResult = [System.Windows.Forms.DialogResult]::OK;
            $Form.Close();
        }
    }
});
$Button = New-Object -TypeName 'System.Windows.Forms.Button';
$Button.Anchor = [System.Windows.Forms.AnchorStyles]::Right -bor [System.Windows.Forms.AnchorStyles]::Top;
$Button.Size = New-Object -TypeName 'System.Drawing.Size' -ArgumentList 75, 25;
$Button.Margin = New-Object -TypeName 'System.Windows.Forms.Padding' -ArgumentList 8, 8, 0, 8;
$Button.Text = 'Back';
$TableLayoutPanel.Controls.Add($Button, 0, 2) | Out-Null;
$Button.add_Click({
    Param(
        [Parameter(Position = 0)]
        [System.Windows.Forms.Button]$Sender,
        
        [Parameter(Position = 1)]
        [System.EventArgs]$EventArgs
    )

    [System.Windows.Forms.Form]$Form = $Sender.Parent.Parent;
    $Form.DialogResult = [System.Windows.Forms.DialogResult]::Retry;
    $Form.Close();
});
$Button = New-Object -TypeName 'System.Windows.Forms.Button';
$Button.Size = New-Object -TypeName 'System.Drawing.Size' -ArgumentList 75, 25;
$Button.Margin = New-Object -TypeName 'System.Windows.Forms.Padding' -ArgumentList 0, 8, 8, 8;
$Button.Text = 'Cancel';
$TableLayoutPanel.Controls.Add($Button, 1, 2) | Out-Null;
$Button.add_Click({
    Param(
        [Parameter(Position = 0)]
        [System.Windows.Forms.Button]$Sender,
        
        [Parameter(Position = 1)]
        [System.EventArgs]$EventArgs
    )

    [System.Windows.Forms.Form]$Form = $Sender.Parent.Parent;
    $Form.DialogResult = [System.Windows.Forms.DialogResult]::Cancel;
    $Form.Close();
});
$Form.CancelButton = $Button;
$Form.add_Shown({
    Param(
        [Parameter(Position = 0)]
        [System.Windows.Forms.Form]$Sender,
        
        [Parameter(Position = 1)]
        [System.EventArgs]$EventArgs
    )

    $Sender.BringToFront();
});

$DialogResult = $Form.ShowDialog();
$DialogResult;
$Form.Tag;
$Form.Dispose();

if ($DataTable.Select('[IsInstalled]=True').Length -gt 0) {
	$Form = New-Object -TypeName 'System.Windows.Forms.Form';

    $Message = (@(
        'This Module has been found at the following locations:',
        ($installedAt | ForEach-Object { "`t{0}" -f $_ } | Out-String),
        '',
		'Do you wish to overwrite?'
    ) | Out-String).Trim();
    $Choices = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
    $Choices.Add((New-Object -TypeName:'System.Management.Automation.Host.ChoiceDescription' -ArgumentList:"Yes"));
    $Choices.Add((New-Object -TypeName:'System.Management.Automation.Host.ChoiceDescription' -ArgumentList:"No"));
    $result = $Host.UI.PromptForChoice("Confirm Overwrite", $Message, $Choices, 1);
	if ($result -eq $null -or $result -ne 0) {
		'Aborted.' | Write-Warning;
		return;
	}
}

$i = 0;
$Choices = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
$env:PSModulePath.Split([System.IO.Path]::PathSeparator) | ForEach-Object {
    $i = $i + 1;
    $Choices.Add((New-Object -TypeName:'System.Management.Automation.Host.ChoiceDescription' -ArgumentList:($i, $_)));
}


$result = $Host.UI.PromptForChoice("Select Module Location", (@($Choices | ForEach-Object { '{0}: {1}' -f $_.Label, $_.HelpMessage }) | Out-String).Trim(), $Choices, 0);
$ModuleInstallLocation = $Choices[$result].HelpMessage | Join-Path -ChildPath:$ModuleBaseName;

if (-not ($ModuleInstallLocation | Test-Path)) {
    $folder = New-Item -Path:$ModuleInstallLocation -ItemType:'Directory';

    if ($folder -eq $null) {
        'Error creating destination folder.' | Write-Warning;
        return;
    }
}

if ($PSScriptRoot -eq $null) { $PSScriptRoot = Get-Location }
$fileName = $ModuleBaseName + '.psm1';
$source = $PSScriptRoot | Join-Path -ChildPath:$fileName;
$destination = $ModuleInstallLocation | Join-Path -ChildPath:$fileName;
Copy-Item -Path:$source -Destination:$destination -Force;

$fileName = $ModuleBaseName + '.psd1';
$source = $PSScriptRoot | Join-Path -ChildPath:$fileName;
$destination = $ModuleInstallLocation | Join-Path -ChildPath:$fileName;
Copy-Item -Path:$source -Destination:$destination -Force;

'Finished.' | Write-Host;
