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
