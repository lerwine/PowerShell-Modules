Import-Module 'Erwine.Leonard.T.IOUtility' -ErrorAction Stop;
Import-Module 'Erwine.Leonard.T.XmlUtility' -ErrorAction Stop;

Add-Type -Path ($MyInvocation.MyCommand.Definition | Split-Path -Parent | Join-Path -ChildPath 'PasswordStorage.cs') `
    -ReferencedAssemblies (('mscorlib', 'System.Xml', 'System.Data', 'System.Management.Automation', 'System.Windows.Forms', 'System.Drawing') | ForEach-Object {
        [System.Reflection.Assembly]::LoadWithPartialName($_).Location
    }) -ErrorAction Stop;
    
$Script:AppDataPath = {
    if (-not ($args[0] | Test-Path)) {
        $Path = $args[0] | Split-Path -Parent;
        if (-not ($Path | Test-Path)) { New-Item -Path $Path -ItemType Directory | Out-Null }
        New-Item -Path $args[0] -ItemType Directory | Out-Null;
    }
    return $args[0];
}.Invoke((Get-AppDataPath -Company 'Erwine.Leonard.T' -ProductName 'PasswordStorage' -Version '1.0' -Local))[0];
$Script:CredentialsPath = $Script:AppDataPath | Join-Path -ChildPath 'Credentials.xml';
$Script:CredentialsXmlDocument = {
    if ($args[0] | Test-Path) {
        Read-XmlDocument -InputUri $args[0];
    } else {
        ([Xml]'<Credentials />')
    }
}.Invoke($Script:CredentialsPath)[0]

Function New-WindowOwner {
	<#
		.SYNOPSIS
			Create new window owner object.
 
		.DESCRIPTION
			Initializes a new object which implements System.Windows.Forms.IWin32Window, representing an owner window.

		.OUTPUTS
			System.Windows.Forms.IWin32Window. Path to selected file or folder.
            
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.iwin32window.aspx
            
        .LINK
            https://msdn.microsoft.com/en-us/library/system.diagnostics.process.getcurrentprocess.aspx
            
        .LINK
            https://msdn.microsoft.com/en-us/library/system.diagnostics.process.mainwindowhandle.aspx
            
        .LINK
            https://msdn.microsoft.com/en-us/library/system.windows.forms.control.handle.aspx
	#>
    [CmdletBinding()]
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        # The Win32 HWND handle of a window. If this is not specified, then the handle of the current process's main window is used.
        [Alias('HWND', 'Handle')]
        [System.IntPtr]$WindowHandle
    )
    
    Process {
        if ($PSBoundParameters.ContainsKey('WindowHandle')) {
            New-Object -TypeName 'PasswordStorageLib.WindowOwner' -ArgumentList $WindowHandle;
        } else {
            New-Object -TypeName 'PasswordStorageLib.WindowOwner' -ArgumentList ([System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle);
        }
    }
}

Function Show-EditForm {
    [CmdletBinding(DefaultParameterSetName = 'Edit')]
    Param(
        [Parameter(ParameterSetName = 'Edit')]
        [Parameter(Mandatory = $true, ParameterSetName = 'Delete')]
        [System.Xml.XmlElement]$XmlElement,
        
        [Parameter(Mandatory = $true, ParameterSetName = 'Delete')]
        [switch]$Delete
    )
    
    $Edit = $PSBoundParameters.ContainsKey('XmlElement');
    $Form = New-Object -TypeName 'System.Windows.Forms.Form' -Property @{
        Name = 'PasswordStorageEdit{0}' -f [System.Guid]::NewGuid().ToString('N');
        TopLevel = $true;
        Size = New-Object -TypeName 'System.Drawing.Size' -ArgumentList 800, 600;
    };
    if ($Edit) {
        $SelectedId = [System.Xml.XmlConvert]::ToInt32((Get-AttributeText -XmlElement $XmlElement -Name 'ID'));
    } else {
        $XmlElement = $Script:CredentialsXmlDocument.DocumentElement.AppendChild($Script:CredentialsXmlDocument.CreateElement('Credential'));
        $SelectedId = 0;
        while ($Script:CredentialsXmlDocument.SelectSingleNode(('/Credentials/Credential[@ID="{0}"]' -f $SelectedId)) -ne $null) { $SelectedId++ }
        Set-AttributeText -XmlElement $XmlElement -Name 'ID' -Value ([System.Xml.XmlConvert]::ToString($SelectedId));
        Set-AttributeText -XmlElement $XmlElement -Name 'Order' -Value ([System.Xml.XmlConvert]::ToString($Script:CredentialsXmlDocument.SelectNodes('/Credentials/Credential').Count - 1));
    }
    try {
        $InteractionProperties = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
            SelectedId = $SelectedId;
            SelectedName = Get-ElementText -XmlElement $XmlElement -Name 'Name';
            XmlElement = $XmlElement;
            mainForm = $Form;
            outerTableLayoutPanel = New-Object -TypeName 'System.Windows.Forms.TableLayoutPanel' -Property @{
                Name = 'outerTableLayoutPanel';
                Dock = [System.Windows.Forms.DockStyle]::Fill;
                AutoSize = $true;
            };
            nameHeadingLabel = New-Object -TypeName 'System.Windows.Forms.Label' -Property @{
                Name = 'nameHeadingLabel';
                Anchor = [System.Windows.Forms.AnchorStyles]::Top -bor [System.Windows.Forms.AnchorStyles]::Right;
                Text = 'Name:'
                AutoSize = $true;
            };
            nameTextBox = New-Object -TypeName 'System.Windows.Forms.TextBox' -Property @{
                Name = 'nameTextBox';
                Dock = [System.Windows.Forms.DockStyle]::Top;
            };
            nameErrorLabel = New-Object -TypeName 'System.Windows.Forms.Label' -Property @{
                Name = 'nameErrorLabel';
                Anchor = [System.Windows.Forms.AnchorStyles]::Top -bor [System.Windows.Forms.AnchorStyles]::Left;
                ForeColor = [System.Drawing.Color]::Red;
                Text = 'Name cannot be empty.';
                AutoSize = $true;
            };
            loginHeadingLabel = New-Object -TypeName 'System.Windows.Forms.Label' -Property @{
                Name = 'loginHeadingLabel';
                Anchor = [System.Windows.Forms.AnchorStyles]::Top -bor [System.Windows.Forms.AnchorStyles]::Right;
                Text = 'Login:'
                AutoSize = $true;
            };
            loginTextBox = New-Object -TypeName 'System.Windows.Forms.TextBox' -Property @{
                Name = 'loginTextBox';
                Dock = [System.Windows.Forms.DockStyle]::Top;
            };
            passwordHeadingLabel = New-Object -TypeName 'System.Windows.Forms.Label' -Property @{
                Name = 'passwordHeadingLabel';
                Anchor = [System.Windows.Forms.AnchorStyles]::Top -bor [System.Windows.Forms.AnchorStyles]::Right;
                Text = 'Password:'
                AutoSize = $true;
            };
            passwordTextBox = New-Object -TypeName 'System.Windows.Forms.TextBox' -Property @{
                Name = 'passwordTextBox';
                Dock = [System.Windows.Forms.DockStyle]::Top;
                UseSystemPasswordChar = $true;
            };
            confirmHeadingLabel = New-Object -TypeName 'System.Windows.Forms.Label' -Property @{
                Name = 'confirmHeadingLabel';
                Anchor = [System.Windows.Forms.AnchorStyles]::Top -bor [System.Windows.Forms.AnchorStyles]::Right;
                Text = 'Confirm:'
                AutoSize = $true;
            };
            confirmTextBox = New-Object -TypeName 'System.Windows.Forms.TextBox' -Property @{
                Name = 'confirmTextBox';
                Dock = [System.Windows.Forms.DockStyle]::Top;
                UseSystemPasswordChar = $true;
            };
            passwordErrorLabel = New-Object -TypeName 'System.Windows.Forms.Label' -Property @{
                Name = 'passwordErrorLabel';
                Anchor = [System.Windows.Forms.AnchorStyles]::Top -bor [System.Windows.Forms.AnchorStyles]::Left;
                ForeColor = [System.Drawing.Color]::Red;
                Text = 'Password and confirmation do not match.';
                Visible = $false;
                AutoSize = $true;
            };
            urlHeadingLabel = New-Object -TypeName 'System.Windows.Forms.Label' -Property @{
                Name = 'urlHeadingLabel';
                Anchor = [System.Windows.Forms.AnchorStyles]::Top -bor [System.Windows.Forms.AnchorStyles]::Left;
                Text = 'Url:'
                AutoSize = $true;
            };
            urlTextBox = New-Object -TypeName 'System.Windows.Forms.TextBox' -Property @{
                Name = 'urlTextBox';
                Dock = [System.Windows.Forms.DockStyle]::Top;
            };
            notesHeadingLabel = New-Object -TypeName 'System.Windows.Forms.Label' -Property @{
                Name = 'notesHeadingLabel';
                Anchor = [System.Windows.Forms.AnchorStyles]::Top -bor [System.Windows.Forms.AnchorStyles]::Left;
                Text = 'Notes:'
                AutoSize = $true;
            };
            notesTextBox = New-Object -TypeName 'System.Windows.Forms.TextBox' -Property @{
                Name = 'notesTextBox';
                Dock = [System.Windows.Forms.DockStyle]::Fill;
                WordWrap = $true;
                AutoSize = $true;
                AcceptsTab = $true;
                AcceptsReturn = $true;
                Multiline = $true;
                ScrollBars = [System.Windows.Forms.ScrollBars]::Vertical;
            };
            buttonTableLayoutPanel = New-Object -TypeName 'System.Windows.Forms.TableLayoutPanel' -Property @{
                Name = 'buttonTableLayoutPanel';
                Dock = [System.Windows.Forms.DockStyle]::Fill;
                AutoSize = $true;
            };
            okButton = New-Object -TypeName 'System.Windows.Forms.Button' -Property @{
                Name = 'okButton';
                DialogResult = [System.Windows.Forms.DialogResult]::OK;
                Anchor = [System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Right;
            };
            cancelButton = New-Object -TypeName 'System.Windows.Forms.Button' -Property @{
                Name = 'cancelButton';
                DialogResult = [System.Windows.Forms.DialogResult]::Cancel;
                Anchor = [System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Right;
            };
        } | Add-Member -MemberType ScriptMethod -Name 'NameTextBox_TextChanged' -PassThru -Value {
            $ErrorMessage = $null;
            if ($this.nameTextBox.Text.Trim() -eq '') {
                $ErrorMessage = 'Name cannot be empty.';
            } else {
                for ($s = $this.XmlElement.PreviousSibling; $s -ne $null; $s = $s.PreviousSibling) {
                    if ($s -is [System.Xml.XmlElement] -and (Get-ElementText -XmlElement $s -Name 'Name').Trim() -ieq $this.nameTextBox.Text.Trim()) {
                        $ErrorMessage = 'Another item has the same name.';
                        break;
                    }
                }
                if ($ErrorMessage -eq $null) {
                    for ($s = $this.XmlElement.NextSibling; $s -ne $null; $s = $s.NextSibling) {
                        if ($s -is [System.Xml.XmlElement] -and (Get-ElementText -XmlElement $s -Name 'Name').Trim() -ieq $this.nameTextBox.Text.Trim()) {
                            $ErrorMessage = 'Another item has the same name.';
                            break;
                        }
                    }
                }
            }
            
            if ($ErrorMessage -ne $null) {
                $this.nameErrorLabel.Text = 'Name cannot be empty.';
                $this.nameErrorLabel.Visible = $true;
                $this.okButton.Enabled = $false;
            } else {
                $this.nameErrorLabel.Visible = $false;
                $this.okButton.Enabled = -not $this.passwordErrorLabel.Visible;
            }
        } | Add-Member -MemberType ScriptMethod -Name 'PasswordTextBox_TextChanged' -PassThru -Value {
            if ($this.passwordTextBox.Text -ceq $this.confirmTextBox.Text) {
                $this.passwordErrorLabel.Visible = $false;
                $this.okButton.Enabled = -not $this.nameErrorLabel.Visible;
            } else {
                $this.passwordErrorLabel.Visible = $true;
                $this.okButton.Enabled = $false;
            }
        } | Add-Member -MemberType ScriptMethod -Name 'ConfirmTextBox_TextChanged' -PassThru -Value {
                if ($this.passwordTextBox.Text -ceq $this.confirmTextBox.Text) {
                    $this.passwordErrorLabel.Visible = $false;
                    $this.okButton.Enabled = -not $this.nameErrorLabel.Visible;
                } else {
                    $this.passwordErrorLabel.Visible = $true;
                    $this.okButton.Enabled = $false;
                }
        };

        if ($Delete) {
            $InteractionProperties.okButton.Text = 'Yes';
            $InteractionProperties.cancelButton.Text = 'No';
            $InteractionProperties.mainForm.Text = 'Delete "{0}"' -f $InteractionProperties.SelectedName;
        } else {
            $InteractionProperties.okButton.Text = 'OK';
            $InteractionProperties.cancelButton.Text = 'Cancel';
            if ($Edit) {
                $InteractionProperties.mainForm.Text = 'Edit "{0}"' -f $InteractionProperties.SelectedName;
            } else {
                $InteractionProperties.mainForm.Text = 'New Credential';
            }
        }
        
        $InteractionProperties.mainForm.Tag = $InteractionProperties;
        $InteractionProperties.nameTextBox.Tag = $InteractionProperties;
        $InteractionProperties.urlTextBox.Tag = $InteractionProperties;
        $InteractionProperties.loginTextBox.Tag = $InteractionProperties;
        $InteractionProperties.passwordTextBox.Tag = $InteractionProperties;
        $InteractionProperties.confirmTextBox.Tag = $InteractionProperties;
        $InteractionProperties.notesTextBox.Tag = $InteractionProperties;
        $InteractionProperties.okButton.Tag = $InteractionProperties;
        $InteractionProperties.cancelButton.Tag = $InteractionProperties;

        $InteractionProperties.mainForm.Controls.Add($InteractionProperties.outerTableLayoutPanel);
        $InteractionProperties.mainForm.AcceptButton = $InteractionProperties.okButton;
        $InteractionProperties.mainForm.CancelButton = $InteractionProperties.cancelButton;
        $InteractionProperties.outerTableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $InteractionProperties.outerTableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Percent, 50))) | Out-Null;
        $InteractionProperties.outerTableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $InteractionProperties.outerTableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Percent, 50))) | Out-Null;
        $InteractionProperties.outerTableLayoutPanel.RowStyles.Add((New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $InteractionProperties.outerTableLayoutPanel.RowStyles.Add((New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $InteractionProperties.outerTableLayoutPanel.RowStyles.Add((New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $InteractionProperties.outerTableLayoutPanel.RowStyles.Add((New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $InteractionProperties.outerTableLayoutPanel.RowStyles.Add((New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $InteractionProperties.outerTableLayoutPanel.RowStyles.Add((New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $InteractionProperties.outerTableLayoutPanel.RowStyles.Add((New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $InteractionProperties.outerTableLayoutPanel.RowStyles.Add((New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Percent, 100))) | Out-Null;
        $InteractionProperties.outerTableLayoutPanel.RowStyles.Add((New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.nameHeadingLabel, 0, 0);
        $InteractionProperties.outerTableLayoutPanel.SetRowSpan($InteractionProperties.nameHeadingLabel, 2);
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.nameTextBox, 1, 0);
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.nameErrorLabel, 1, 1);
        
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.nameHeadingLabel, 0, 0);
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.nameTextBox, 1, 0);
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.loginHeadingLabel, 2, 0);
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.loginTextBox, 3, 0);
        
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.nameErrorLabel, 1, 1);
        $InteractionProperties.outerTableLayoutPanel.SetColumnSpan($InteractionProperties.nameErrorLabel, 3);
        
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.passwordHeadingLabel, 0, 2);
        $InteractionProperties.outerTableLayoutPanel.SetRowSpan($InteractionProperties.passwordHeadingLabel, 2);
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.passwordTextBox, 1, 2);
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.confirmHeadingLabel, 2, 2);
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.confirmTextBox, 3, 2);
        
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.passwordErrorLabel, 1, 3);
        $InteractionProperties.outerTableLayoutPanel.SetColumnSpan($InteractionProperties.passwordErrorLabel, 3);
        
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.urlHeadingLabel, 0, 4);
        $InteractionProperties.outerTableLayoutPanel.SetColumnSpan($InteractionProperties.urlHeadingLabel, 4);
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.urlTextBox, 0, 5);
        $InteractionProperties.outerTableLayoutPanel.SetColumnSpan($InteractionProperties.urlTextBox, 4);
        
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.notesHeadingLabel, 0, 6);
        $InteractionProperties.outerTableLayoutPanel.SetColumnSpan($InteractionProperties.notesHeadingLabel, 6);
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.notesTextBox, 0, 7);
        $InteractionProperties.outerTableLayoutPanel.SetColumnSpan($InteractionProperties.notesTextBox, 4);
        
        $InteractionProperties.buttonTableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Percent, 100))) | Out-Null;
        $InteractionProperties.buttonTableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        
        $InteractionProperties.outerTableLayoutPanel.Controls.Add($InteractionProperties.buttonTableLayoutPanel, 1, 8);
        $InteractionProperties.outerTableLayoutPanel.SetColumnSpan($InteractionProperties.buttonTableLayoutPanel, 4);
        
        $InteractionProperties.buttonTableLayoutPanel.Controls.Add($InteractionProperties.okButton, 0, 0);
        $InteractionProperties.buttonTableLayoutPanel.Controls.Add($InteractionProperties.cancelButton, 1, 0);
        
        if (-not $Delete) {
            $InteractionProperties.nameTextBox.add_TextChanged({
                Param(
                    [Parameter(Mandatory = $true, Position = 0)]
                    [object]$Sender,
                    
                    [Parameter(Mandatory = $true, Position = 0)]
                    [System.EventArgs]$E
                )
                $Sender.Tag.NameTextBox_TextChanged();
            });

            $InteractionProperties.passwordTextBox.add_TextChanged({
                Param(
                    [Parameter(Mandatory = $true, Position = 0)]
                    [object]$Sender,
                    
                    [Parameter(Mandatory = $true, Position = 0)]
                    [System.EventArgs]$E
                )
                
                $Sender.Tag.PasswordTextBox_TextChanged();
            });

            $InteractionProperties.confirmTextBox.add_TextChanged({
                Param(
                    [Parameter(Mandatory = $true, Position = 0)]
                    [object]$Sender,
                    
                    [Parameter(Mandatory = $true, Position = 0)]
                    [System.EventArgs]$E
                )
                
                $Sender.Tag.ConfirmTextBox_TextChanged();
            });
        }
        
        if ($PSBoundParameters.ContainsKey('XmlElement')) {
            $InteractionProperties.nameTextBox.Text = $InteractionProperties.SelectedName;
            $InteractionProperties.urlTextBox.Text = Get-ElementText -XmlElement $XmlElement -Name 'Url';
            $InteractionProperties.loginTextBox.Text = Get-ElementText -XmlElement $XmlElement -Name 'Login';
            $Password = Get-ElementText -XmlElement $XmlElement -Name 'Password';
            if ($Password -ne '') {
                $PSCredential = New-Object -TypeName 'System.Management.Automation.PSCredential' -ArgumentList (Get-ElementText -XmlElement $XmlElement -Name 'Login'), ($Password | ConvertTo-SecureString);
                $InteractionProperties.passwordTextBox.Text = $PSCredential.GetNetworkCredential().Password;
                $InteractionProperties.confirmTextBox.Text = $InteractionProperties.passwordTextBox.Text;
            }
            $InteractionProperties.notesTextBox.Text = Get-ElementText -XmlElement $XmlElement -Name 'Notes';
            if ($Delete) {
                $InteractionProperties.nameTextBox.ReadOnly = $true;
                $InteractionProperties.nameErrorLabel.Text = 'Are you sure you want to delete this item?';
                $InteractionProperties.loginTextBox.ReadOnly = $true;
                $InteractionProperties.passwordTextBox.Visible = $false;
                $InteractionProperties.urlTextBox.ReadOnly = $true;
                $InteractionProperties.notesTextBox.ReadOnly = $true;
            }
        }
        
        if ($InteractionProperties.mainForm.ShowDialog((New-WindowOwner)) -eq [System.Windows.Forms.DialogResult]::OK) {
            $ParentNode = $XmlElement.ParentNode;
            if ($Delete) {
                $ParentNode.RemoveChild($XmlElement) | Out-Null;
            } else {
                Set-ElementText -XmlElement $XmlElement -Name 'Name' -Value $InteractionProperties.nameTextBox.Text;
                Set-ElementText -XmlElement $XmlElement -Name 'Login' -Value $InteractionProperties.loginTextBox.Text;
                if ($InteractionProperties.passwordTextBox.Text.Trim().Length -eq 0) {
                    Set-ElementText -XmlElement $XmlElement -Name 'Password' -Value '';
                } else {
                    $SecureString = $InteractionProperties.passwordTextBox.Text | ConvertTo-SecureString -AsPlainText -Force;
                    Set-ElementText -XmlElement $XmlElement -Name 'Password' -Value ($SecureString | ConvertFrom-SecureString);
                }
                Set-ElementText -XmlElement $XmlElement -Name 'Url' -Value $InteractionProperties.urlTextBox.Text;
                Set-ElementText -XmlElement $XmlElement -Name 'Notes' -Value $InteractionProperties.notesTextBox.Text;
            }
            $Items = @();
            $XmlNodeList = $ParentNode.SelectNodes('Credential');
            for ($i = 0; $i -lt $XmlNodeList.Count; $i++) {
                $c = $XmlNodeList.Item($i);
                $t = Get-AttributeText -XmlElement $c -Name 'Order';
                if ($t -eq $null -or $t -eq '') {
                    $Order = [int]::MaxValue;
                } else {
                    try {
                        $Order = [System.Xml.XmlConvert]::ToInt32($t.Trim());
                    } catch {
                        $Order = [int]::MaxValue;
                    }
                }
                $Items += (New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
                    Element = $c;
                    Order = $Order;
                });
            }
            $Items = @($Items | Sort-Object -Property 'Order');
            for ($i = 0; $i -lt $Items.Count; $i++) {
                if ($Items[$i].Order -ne $i) {
                    Set-AttributeText -XmlElement $Items[$i].Element -Name 'Order' -Value ([System.Xml.XmlConvert]::ToString($i));
                }
            }
            Save-CredentialsDocument;
        } else {
            if (-not ($Edit -or $Delete)) { $XmlElement.ParentNode.RemoveChild($XmlElement) | Out-Null }
        }
    } catch {
        if (-not ($Edit -or $Delete)) { $XmlElement.ParentNode.RemoveChild($XmlElement) | Out-Null }
        throw;
    } finally {
        $Form.Dispose();
    }
}

Function New-BrowseWindow {
    Param()
    
    if ([System.Windows.Forms.Clipboard]::ContainsText()) {
        $TextDataFormat = @(@(
            [System.Windows.Forms.TextDataFormat]::Rtf,
            [System.Windows.Forms.TextDataFormat]::Html,
            [System.Windows.Forms.TextDataFormat]::CommaSeparatedValue,
            [System.Windows.Forms.TextDataFormat]::UnicodeText,
            [System.Windows.Forms.TextDataFormat]::Text
        ) | Where-Object { [System.Windows.Forms.Clipboard]::ContainsText($_) });
        if ($TextDataFormat.Count -gt 0) {
            $ClipboardText = [System.Windows.Forms.Clipboard]::GetText($TextDataFormat[0]);
        } else {
            $ClipboardText = [System.Windows.Forms.Clipboard]::GetText();
        }
    } else {
        $ClipboardText = $null;
    }
    $Form = New-Object -TypeName 'System.Windows.Forms.Form' -Property @{
        Name = 'PasswordStorageBrowseForm{0}' -f [System.Guid]::NewGuid().ToString('N');
        Text = "Credential Listing";
        TopLevel = $true;
        Size = New-Object -TypeName 'System.Drawing.Size' -ArgumentList 1024, 768;
    };
    try {
        $BrowseWindow = New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
            SelectedId = $null;
            CurrentRowCellStyle = $null;
            EventArgs = @{};
            OrderChanged = $false;
            BrowserOptionsDataTable = Get-BrowserOptionsDataTable;
            DataSource = New-Object -TypeName 'System.Data.DataTable';
            Document = $Script:CredentialsXmlDocument;
            IdDataColumn = New-Object -TypeName 'System.Data.DataColumn' -Property @{
                AllowDBNull = $false;
                Caption = 'ID';
                ColumnName = 'ID';
                DataType = [int];
                Unique = $true;
            }
            NameDataColumn = New-Object -TypeName 'System.Data.DataColumn' -Property @{
                AllowDBNull = $false;
                Caption = 'Name';
                ColumnName = 'Name';
                DataType = [string];
            }
            LoginDataColumn = New-Object -TypeName 'System.Data.DataColumn' -Property @{
                AllowDBNull = $false;
                Caption = 'Login';
                ColumnName = 'Login';
                DataType = [string];
            }
            UrlDataColumn = New-Object -TypeName 'System.Data.DataColumn' -Property @{
                AllowDBNull = $false;
                Caption = 'Url';
                ColumnName = 'Url';
                DataType = [string];
            }
            PasswordDataColumn = New-Object -TypeName 'System.Data.DataColumn' -Property @{
                AllowDBNull = $false;
                Caption = 'Password';
                ColumnName = 'Password';
                DataType = [string];
            }
            OrderDataColumn = New-Object -TypeName 'System.Data.DataColumn' -Property @{
                AllowDBNull = $false;
                Caption = 'Order';
                ColumnName = 'Order';
                DataType = [int];
            }
            mainForm = $Form;
            outerTableLayoutPanel = New-Object -TypeName 'System.Windows.Forms.TableLayoutPanel' -Property @{
                Name = 'outerTableLayoutPanel';
                Dock = [System.Windows.Forms.DockStyle]::Fill;
                AutoSize = $true;
            };
            mainDataGridView = New-Object -TypeName 'System.Windows.Forms.DataGridView' -Property @{
                Name = 'mainDataGridView';
                Dock = [System.Windows.Forms.DockStyle]::Fill;
                ReadOnly = $true;
                AllowUserToAddRows = $false;
                AllowUserToDeleteRows = $false;
                AutoGenerateColumns = $false;
                AutoSize = $true;
                MultiSelect = $true;
                SelectionMode = [System.Windows.Forms.DataGridViewSelectionMode]::CellSelect
                #SelectionMode = [System.Windows.Forms.DataGridViewSelectionMode]::FullRowSelect
            };
            browserOptionsComboBox = New-Object -TypeName 'System.Windows.Forms.ComboBox' -Property @{
                Name = 'browserOptionsComboBox';
                Dock = [System.Windows.Forms.DockStyle]::Bottom;
                AutoSize = $true;
                DisplayMember = 'Name';
                ValueMember = 'Path';
                DropDownStyle = [System.Windows.Forms.ComboBoxStyle]::DropDown;
            }
            openUrlButton = New-Object -TypeName 'System.Windows.Forms.Button' -Property @{
                Name = 'openUrlButton';
                Text = 'Open Url';
                Anchor = [System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Right;
            };
            copyPasswordButton = New-Object -TypeName 'System.Windows.Forms.Button' -Property @{
                Name = 'copyPasswordButton';
                Text = 'Copy PW';
                Anchor = [System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Right;
            };
            moveUpButton = New-Object -TypeName 'System.Windows.Forms.Button' -Property @{
                Name = 'moveUpButton';
                Text = 'Move Up';
                Anchor = [System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Right;
            };
            moveDownButton = New-Object -TypeName 'System.Windows.Forms.Button' -Property @{
                Name = 'moveDownButton';
                Text = 'Move Down';
                Anchor = [System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Right;
            };
            editButton = New-Object -TypeName 'System.Windows.Forms.Button' -Property @{
                Name = 'editButton';
                Text = 'Edit';
                DialogResult = [System.Windows.Forms.DialogResult]::Yes;
                Anchor = [System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Right;
            };
            duplicateButton = New-Object -TypeName 'System.Windows.Forms.Button' -Property @{
                Name = 'duplicateButton';
                Text = 'Duplicate';
                Anchor = [System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Right;
            };
            newButton = New-Object -TypeName 'System.Windows.Forms.Button' -Property @{
                Name = 'newButton';
                Text = 'New';
                DialogResult = [System.Windows.Forms.DialogResult]::No;
                Anchor = [System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Right;
            };
            deleteButton = New-Object -TypeName 'System.Windows.Forms.Button' -Property @{
                Name = 'deleteButton';
                Text = 'Delete';
                DialogResult = [System.Windows.Forms.DialogResult]::Abort;
                Anchor = [System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Right;
            };
            exitButton = New-Object -TypeName 'System.Windows.Forms.Button' -Property @{
                Name = 'exitButton';
                Text = 'Exit';
                DialogResult = [System.Windows.Forms.DialogResult]::Cancel;
                Anchor = [System.Windows.Forms.AnchorStyles]::Bottom -bor [System.Windows.Forms.AnchorStyles]::Right;
            };
            idDataGridColumn = New-Object -TypeName 'System.Windows.Forms.DataGridViewTextBoxColumn' -Property @{
                DataPropertyName = 'ID';
                HeaderText = 'ID';
                ReadOnly = $true;
                AutoSizeMode = [System.Windows.Forms.DataGridViewAutoSizeColumnMode]::AllCells;
                SortMode = [System.Windows.Forms.DataGridViewColumnSortMode]::Automatic;
                Visible = $false;
            };
            nameDataGridColumn = New-Object -TypeName 'System.Windows.Forms.DataGridViewTextBoxColumn' -Property @{
                DataPropertyName = 'Name';
                HeaderText = 'Name';
                ReadOnly = $true;
                AutoSizeMode = [System.Windows.Forms.DataGridViewAutoSizeColumnMode]::AllCells;
                SortMode = [System.Windows.Forms.DataGridViewColumnSortMode]::Automatic;
            };
            loginDataGridColumn = New-Object -TypeName 'System.Windows.Forms.DataGridViewTextBoxColumn' -Property @{
                DataPropertyName = 'Login';
                HeaderText = 'Login';
                ReadOnly = $true;
                AutoSizeMode = [System.Windows.Forms.DataGridViewAutoSizeColumnMode]::AllCells;
                SortMode = [System.Windows.Forms.DataGridViewColumnSortMode]::Automatic;
            };
            urlDataGridColumn = New-Object -TypeName 'System.Windows.Forms.DataGridViewTextBoxColumn' -Property @{
                DataPropertyName = 'Url';
                HeaderText = 'Url';
                ReadOnly = $true;
                AutoSizeMode = [System.Windows.Forms.DataGridViewAutoSizeColumnMode]::Fill;
                SortMode = [System.Windows.Forms.DataGridViewColumnSortMode]::Automatic;
            };
            orderDataGridColumn = New-Object -TypeName 'System.Windows.Forms.DataGridViewTextBoxColumn' -Property @{
                DataPropertyName = 'Order';
                HeaderText = 'Order';
                ReadOnly = $true;
                AutoSizeMode = [System.Windows.Forms.DataGridViewAutoSizeColumnMode]::AllCells;
                SortMode = [System.Windows.Forms.DataGridViewColumnSortMode]::Automatic;
                Visible = $false;
            };
            TerminalButtonScriptBlock = {
                Param(
                    [Parameter(Mandatory = $true, Position = 0)]
                    [object]$Sender,
                    
                    [Parameter(Mandatory = $true, Position = 0)]
                    [System.EventArgs]$E
                )
                
                $BrowseWindow = $Sender.Tag;
                $BrowseWindow.mainForm.DialogResult = $Sender.DialogResult;
                $BrowseWindow.mainForm.Close();
            };
        } | Add-Member -MemberType ScriptMethod -Name 'GetSelectedRow' -PassThru -Value {
            if ($this.mainDataGridView.CurrentCell -ne $null -and $this.mainDataGridView.CurrentCell.OwningRow -ne $null -and $this.mainDataGridView.CurrentCell.OwningRow.DataBoundItem -ne $null) {
                return $this.mainDataGridView.CurrentCell.OwningRow.DataBoundItem.Row;
            }
        } | Add-Member -MemberType ScriptMethod -Name 'OnFormClosed' -PassThru -Value {
            $DataRow = $this.GetSelectedRow();
            if ($DataRow -ne $null) {
                $this.SelectedId = $DataRow[$this.IdDataColumn];
            } else {
                $this.SelectedId = $null;
            }
        } | Add-Member -MemberType ScriptMethod -Name 'DataGridView_DataBindingComplete' -PassThru -Value {
            if ($this.CurrentRowCellStyle -eq $null) {
                $this.CurrentRowCellStyle = New-Object -TypeName 'System.Windows.Forms.DataGridViewCellStyle' -ArgumentList $this.mainDataGridView.DefaultCellStyle;
                $this.CurrentRowCellStyle.BackColor = [System.Drawing.Color]::LightCoral;
                $this.mainDataGridView.Sort($this.orderDataGridColumn, [System.ComponentModel.ListSortDirection]::Ascending);
                $this.DataGridView_SelectionChanged() | Out-Null;
            }
        } | Add-Member -MemberType ScriptMethod -Name 'DataGridView_SelectionChanged' -PassThru -Value {
            if ($this.CurrentRowCellStyle -eq $null) { return }
            
            $RowIndexes = @($this.mainDataGridView.SelectedCells | ForEach-Object { $_.RowIndex });
            if ($this.mainDataGridView.SelectedCells.Count -eq 1) {
                foreach ($r in $this.mainDataGridView.Rows) {
                    if ($r.Index -ne $this.mainDataGridView.CurrentCell.RowIndex) {
                        $r.DefaultCellStyle = $this.mainDataGridView.DefaultCellStyle;
                    }
                }
                $this.mainDataGridView.CurrentCell.OwningRow.DefaultCellStyle = $this.CurrentRowCellStyle;
                $this.editButton.Enabled = $true;
                $this.deleteButton.Enabled = $true;
                $this.duplicateButton.Enabled = $true;
                $DataRow = $this.GetSelectedRow();
                if ($DataRow -eq $null) {
                    $this.openUrlButton.Enabled = $false;
                    $this.copyPasswordButton.Enabled = $false;
                    $this.moveUpButton.Enabled = $false;
                    $this.moveDownButton.Enabled = $false;
                } else {
                    $this.openUrlButton.Enabled = ($DataRow[$this.UrlDataColumn] -ne '');
                    $this.copyPasswordButton.Enabled = ($DataRow[$this.PasswordDataColumn] -ne '');
                    $RowIndex = $this.mainDataGridView.CurrentCell.OwningRow.Index;
                    $this.moveUpButton.Enabled = ($RowIndex -gt 0);
                    $this.moveDownButton.Enabled = ($RowIndex -lt ($this.mainDataGridView.RowCount - 1));
                }
            } else {
                $this.openUrlButton.Enabled = $true;
                $this.copyPasswordButton.Enabled = $false;
                $this.editButton.Enabled = $false;
                $this.deleteButton.Enabled = $false;
                $this.duplicateButton.Enabled = $false;
                $this.moveUpButton.Enabled = $false;
                $this.moveDownButton.Enabled = $false;
            }
        } | Add-Member -MemberType ScriptMethod -Name 'OpenUrlButton_Click' -PassThru -Value {
            $DataRow = $this.GetSelectedRow();
            if ($DataRow -eq $null) { return }
            $Url = $DataRow[$this.UrlDataColumn].Trim();
            if ($Url -ne '') {
                $Path = $this.browserOptionsComboBox.SelectedValue;
                if ($Path -eq $null -or $Path.Length -eq 0) {
                    Start-Process -FilePath $Url;
                } else {
                    Start-Process -FilePath $Path -ArgumentList $Url
                }
            }
        } | Add-Member -MemberType ScriptMethod -Name 'PasswordButton_Click' -PassThru -Value {
            $DataRow = $this.GetSelectedRow();
            if ($DataRow -eq $null) { return }
            $Password = $DataRow[$this.PasswordDataColumn];
            if ($Password -ne '') {
                $PSCredential = New-Object -TypeName 'System.Management.Automation.PSCredential' -ArgumentList $DataRow[$this.LoginDataColumn], ($Password | ConvertTo-SecureString);
                [System.Windows.Forms.Clipboard]::SetText($PSCredential.GetNetworkCredential().Password, [System.Windows.Forms.TextDataFormat]::Text);
                $PSCredential = $null;
            } else {
                [System.Windows.Forms.Clipboard]::Clear();
            }
        } | Add-Member -MemberType ScriptMethod -Name 'MoveUpButton_Click' -PassThru -Value {
            if ($this.mainDataGridView.SortOrder -eq [System.Windows.Forms.SortOrder]::Ascending) {
                $this.MoveUp();
            } else {
                $this.MoveDown();
            }
        } | Add-Member -MemberType ScriptMethod -Name 'MoveDownButton_Click' -PassThru -Value {
            if ($this.mainDataGridView.SortOrder -eq [System.Windows.Forms.SortOrder]::Ascending) {
                $this.MoveDown();
            } else {
                $this.MoveUp();
            }
        } | Add-Member -MemberType ScriptMethod -Name 'MoveUp' -PassThru -Value {
            $this.EventArgs.Clear();
            $this.EventArgs.Add('RowA', $this.GetSelectedRow());
            if ($this.EventArgs.RowA -eq $null) { return }
            $this.EventArgs.Add('RowB', ($this.DataSource.Rows | Where-Object { $_[$this.OrderDataColumn] -eq ($this.EventArgs.RowA[$this.OrderDataColumn] - 1) }));
            if ($this.EventArgs.RowB -eq $null) { return }
            if ($this.EventArgs.RowB -isnot [System.Data.DataRow]) { $this.EventArgs['RowB'] = $this.EventArgs.RowB[0] }
            $this.SwapRowOrder();
        } | Add-Member -MemberType ScriptMethod -Name 'MoveDown' -PassThru -Value {
            $this.EventArgs.Clear();
            $this.EventArgs.Add('RowA', $this.GetSelectedRow());
            if ($this.EventArgs.RowA -eq $null) { return }
            $this.EventArgs.Add('RowB', ($this.DataSource.Rows | Where-Object { $_[$this.OrderDataColumn] -eq ($this.EventArgs.RowA[$this.OrderDataColumn] + 1) }));
            if ($this.EventArgs.RowB -eq $null) { return }
            if ($this.EventArgs.RowB -isnot [System.Data.DataRow]) { $this.EventArgs['RowB'] = $this.EventArgs.RowB[0] }
            $this.SwapRowOrder();
        } | Add-Member -MemberType ScriptMethod -Name 'SwapRowOrder' -PassThru -Value {
            $RowA = $this.EventArgs.RowA;
            $RowB = $this.EventArgs.RowB;
            $Order = $RowA[$this.OrderDataColumn];
            $RowA.BeginEdit();
            $RowA[$this.OrderDataColumn] = $RowB[$this.OrderDataColumn];
            $RowA.EndEdit();
            $RowA.AcceptChanges();
            $RowB.BeginEdit();
            $RowB[$this.OrderDataColumn] = $Order;
            $RowB.EndEdit();
            $RowB.AcceptChanges();
            $Id = $RowA[$this.IdDataColumn];
            $XmlElement = $this.Document.SelectSingleNode(('/Credentials/Credential[@ID="{0}"]' -f $Id.ToString()));
            Set-AttributeText -XmlElement $XmlElement -Name 'Order' -Value ([System.Xml.XmlConvert]::ToString($RowA[$this.OrderDataColumn]));
            $Id = $RowB[$this.IdDataColumn];
            $XmlElement = $this.Document.SelectSingleNode(('/Credentials/Credential[@ID="{0}"]' -f $Id.ToString()));
            Set-AttributeText -XmlElement $XmlElement -Name 'Order' -Value ([System.Xml.XmlConvert]::ToString($Order));
            $this.OrderChanged = $true;
            $this.mainDataGridView.Sort($this.orderDataGridColumn, [System.ComponentModel.ListSortDirection]::Ascending);
        };
        $BrowseWindow.mainForm.Tag = $BrowseWindow;
        $BrowseWindow.mainDataGridView.Tag = $BrowseWindow;
        $BrowseWindow.moveUpButton.Tag = $BrowseWindow;
        $BrowseWindow.moveDownButton.Tag = $BrowseWindow;
        $BrowseWindow.openUrlButton.Tag = $BrowseWindow;
        $BrowseWindow.copyPasswordButton.Tag = $BrowseWindow;
        $BrowseWindow.editButton.Tag = $BrowseWindow;
        $BrowseWindow.newButton.Tag = $BrowseWindow;
        $BrowseWindow.deleteButton.Tag = $BrowseWindow;
        $BrowseWindow.exitButton.Tag = $BrowseWindow;
        $BrowseWindow.DataSource.Columns.Add($BrowseWindow.IdDataColumn);
        $BrowseWindow.DataSource.Columns.Add($BrowseWindow.NameDataColumn);
        $BrowseWindow.DataSource.Columns.Add($BrowseWindow.LoginDataColumn);
        $BrowseWindow.DataSource.Columns.Add($BrowseWindow.UrlDataColumn);
        $BrowseWindow.DataSource.Columns.Add($BrowseWindow.PasswordDataColumn);
        $BrowseWindow.DataSource.Columns.Add($BrowseWindow.OrderDataColumn);

        $BrowseWindow.mainForm.Controls.Add($BrowseWindow.outerTableLayoutPanel);
        $BrowseWindow.mainForm.AcceptButton = $BrowseWindow.editButton;
        $BrowseWindow.mainForm.CancelButton = $BrowseWindow.exitButton;
        $BrowseWindow.outerTableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Percent, 100))) | Out-Null;
        $BrowseWindow.outerTableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $BrowseWindow.outerTableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $BrowseWindow.outerTableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $BrowseWindow.outerTableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $BrowseWindow.outerTableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $BrowseWindow.outerTableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $BrowseWindow.outerTableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $BrowseWindow.outerTableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $BrowseWindow.outerTableLayoutPanel.ColumnStyles.Add((New-Object -TypeName 'System.Windows.Forms.ColumnStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $BrowseWindow.outerTableLayoutPanel.RowStyles.Add((New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::Percent, 100))) | Out-Null;
        $BrowseWindow.outerTableLayoutPanel.RowStyles.Add((New-Object -TypeName 'System.Windows.Forms.RowStyle' -ArgumentList ([System.Windows.Forms.SizeType]::AutoSize))) | Out-Null;
        $BrowseWindow.outerTableLayoutPanel.Controls.Add($BrowseWindow.mainDataGridView, 0, 0);
        $BrowseWindow.outerTableLayoutPanel.SetColumnSpan($BrowseWindow.mainDataGridView, 10);
        $BrowseWindow.outerTableLayoutPanel.Controls.Add($BrowseWindow.browserOptionsComboBox, 0, 1);
        $BrowseWindow.outerTableLayoutPanel.Controls.Add($BrowseWindow.openUrlButton, 1, 1);
        $BrowseWindow.outerTableLayoutPanel.Controls.Add($BrowseWindow.copyPasswordButton, 2, 1);
        $BrowseWindow.outerTableLayoutPanel.Controls.Add($BrowseWindow.moveUpButton, 3, 1);
        $BrowseWindow.outerTableLayoutPanel.Controls.Add($BrowseWindow.moveDownButton, 4, 1);
        $BrowseWindow.outerTableLayoutPanel.Controls.Add($BrowseWindow.editButton, 5, 1);
        $BrowseWindow.outerTableLayoutPanel.Controls.Add($BrowseWindow.duplicateButton, 6, 1);
        $BrowseWindow.outerTableLayoutPanel.Controls.Add($BrowseWindow.newButton, 7, 1);
        $BrowseWindow.outerTableLayoutPanel.Controls.Add($BrowseWindow.deleteButton, 8, 1);
        $BrowseWindow.outerTableLayoutPanel.Controls.Add($BrowseWindow.exitButton, 9, 1);
        $BrowseWindow.mainDataGridView.Columns.Add($BrowseWindow.nameDataGridColumn) | Out-Null;
        $BrowseWindow.mainDataGridView.Columns.Add($BrowseWindow.loginDataGridColumn) | Out-Null;
        $BrowseWindow.mainDataGridView.Columns.Add($BrowseWindow.urlDataGridColumn) | Out-Null;
        $BrowseWindow.mainDataGridView.Columns.Add($BrowseWindow.orderDataGridColumn) | Out-Null;
        $BrowseWindow.mainDataGridView.Columns.Add($BrowseWindow.idDataGridColumn) | Out-Null;
        $BrowseWindow.idDataGridColumn.Visible = $false;
        $BrowseWindow.orderDataGridColumn.Visible = $false;
        
        $XmlNodeList = $Script:CredentialsXmlDocument.SelectNodes('/Credentials/Credential');
        $BrowseWindow.DataSource.Clear();
        for ($i = 0; $i -lt $XmlNodeList.Count; $i++) {
            $XmlElement = $XmlNodeList.Item($i);
            $DataRow = $BrowseWindow.DataSource.NewRow();
            $DataRow.BeginEdit();
            $DataRow[$BrowseWindow.IdDataColumn] = [System.Xml.XmlConvert]::ToInt32((Get-AttributeText -XmlElement $XmlElement -Name 'ID'));
            $DataRow[$BrowseWindow.NameDataColumn] = Get-ElementText -XmlElement $XmlElement -Name 'Name';
            $DataRow[$BrowseWindow.LoginDataColumn] = Get-ElementText -XmlElement $XmlElement -Name 'Login';
            $DataRow[$BrowseWindow.UrlDataColumn] = Get-ElementText -XmlElement $XmlElement -Name 'Url';
            $DataRow[$BrowseWindow.PasswordDataColumn] = Get-ElementText -XmlElement $XmlElement -Name 'Password';
            $DataRow[$BrowseWindow.OrderDataColumn] = [System.Xml.XmlConvert]::ToInt32((Get-AttributeText -XmlElement $XmlElement -Name 'Order'));
            $DataRow.EndEdit();
            $BrowseWindow.DataSource.Rows.Add($DataRow);
            $DataRow.AcceptChanges();
        }
        $BrowseWindow.DataSource.AcceptChanges();
        
        $BrowseWindow.mainDataGridView.DataSource = $BrowseWindow.DataSource;
        $BrowseWindow.browserOptionsComboBox.DataSource = $BrowseWindow.BrowserOptionsDataTable;
        $BrowseWindow.browserOptionsComboBox.SelectedIndex = 0;

        $BrowseWindow.mainForm.add_FormClosed({
            Param(
                [Parameter(Mandatory = $true, Position = 0)]
                [object]$Sender,
                
                [Parameter(Mandatory = $true, Position = 1)]
                [System.Windows.Forms.FormClosedEventArgs]$E
            )
            $Sender.Tag.EventArgs.Clear();
            $Sender.Tag.EventArgs.Add('Args', $E);
            try {
                $Sender.Tag.OnFormClosed();
            } catch {
            }
        });
        $BrowseWindow.mainDataGridView.add_DataBindingComplete({
            Param(
                [Parameter(Mandatory = $true, Position = 0)]
                [object]$Sender,
                
                [Parameter(Mandatory = $true, Position = 1)]
                [System.Windows.Forms.DataGridViewBindingCompleteEventArgs]$E
            )
            $Sender.Tag.EventArgs.Clear();
            $Sender.Tag.EventArgs.Add('Args', $E);
            $Sender.Tag.DataGridView_DataBindingComplete();
        });
        $BrowseWindow.mainDataGridView.add_SelectionChanged({
            Param(
                [Parameter(Mandatory = $true, Position = 0)]
                [object]$Sender,
                
                [Parameter(Mandatory = $true, Position = 1)]
                [System.EventArgs]$E
            )
            $Sender.Tag.DataGridView_SelectionChanged();
        });
        $BrowseWindow.mainDataGridView.add_Sorted({
            Param(
                [Parameter(Mandatory = $true, Position = 0)]
                [System.Object]$Sender,
                [Parameter(Mandatory = $true, Position = 1)]
                [System.EventArgs]$E
            )
            $Sender.Tag.DataGridView_SelectionChanged();
        });
        $BrowseWindow.newButton.add_Click($BrowseWindow.TerminalButtonScriptBlock);
        $BrowseWindow.deleteButton.add_Click($BrowseWindow.TerminalButtonScriptBlock);
        $BrowseWindow.moveUpButton.add_Click({
            Param(
                [Parameter(Mandatory = $true, Position = 0)]
                [object]$Sender,
                
                [Parameter(Mandatory = $true, Position = 1)]
                [System.EventArgs]$E
            )
            
            $Sender.Tag.MoveUpButton_Click();
        });
        $BrowseWindow.moveDownButton.add_Click({
            Param(
                [Parameter(Mandatory = $true, Position = 0)]
                [object]$Sender,
                
                [Parameter(Mandatory = $true, Position = 1)]
                [System.EventArgs]$E
            )
            
            $Sender.Tag.MoveDownButton_Click();
        });
        $BrowseWindow.openUrlButton.add_Click({
            Param(
                [Parameter(Mandatory = $true, Position = 0)]
                [object]$Sender,
                
                [Parameter(Mandatory = $true, Position = 1)]
                [System.EventArgs]$E
            )
            
            $Sender.Tag.OpenUrlButton_Click();
        });
        $BrowseWindow.copyPasswordButton.add_Click({
            Param(
                [Parameter(Mandatory = $true, Position = 0)]
                [object]$Sender,
                
                [Parameter(Mandatory = $true, Position = 1)]
                [System.EventArgs]$E
            )
            
            $Sender.Tag.PasswordButton_Click();
        });
        
        (,$BrowseWindow) | Write-Output;
    } catch {
        $Form.Dispose();
        throw;
    }
}

Function Show-BrowseForm {
    Param(
        [Parameter(Mandatory = $true)]
        [PSObject]$BrowseWindow
    )
    
    if ([System.Windows.Forms.Clipboard]::ContainsText()) {
        $TextDataFormat = @(@(
            [System.Windows.Forms.TextDataFormat]::Rtf,
            [System.Windows.Forms.TextDataFormat]::Html,
            [System.Windows.Forms.TextDataFormat]::CommaSeparatedValue,
            [System.Windows.Forms.TextDataFormat]::UnicodeText,
            [System.Windows.Forms.TextDataFormat]::Text
        ) | Where-Object { [System.Windows.Forms.Clipboard]::ContainsText($_) });
        if ($TextDataFormat.Count -gt 0) {
            $ClipboardText = [System.Windows.Forms.Clipboard]::GetText($TextDataFormat[0]);
        } else {
            $ClipboardText = [System.Windows.Forms.Clipboard]::GetText();
        }
    } else {
        $ClipboardText = $null;
    }
    try {
        $XmlNodeList = $Script:CredentialsXmlDocument.SelectNodes('/Credentials/Credential');
        $NotProcessed = @{};
        if ($BrowseWindow.DataSource.Rows.Count -gt 0) {
            @($BrowseWindow.DataSource.Rows) | ForEach-Object {
                $NotProcessed[$_[$BrowseWindow.IdDataColumn]] = $_;
            }
        }
        for ($i = 0; $i -lt $XmlNodeList.Count; $i++) {
            $XmlElement = $XmlNodeList.Item($i);
            $Id = [System.Xml.XmlConvert]::ToInt32((Get-AttributeText -XmlElement $XmlElement -Name 'ID'));
            $DataRow = $NotProcessed[$Id];
            if ($DataRow -ne $null) {
                $DataRow.BeginEdit();
                $NotProcessed.Remove($Id);
                $s = Get-ElementText -XmlElement $XmlElement -Name 'Name';
                if ($DataRow[$BrowseWindow.NameDataColumn] -ne $s) { $DataRow[$BrowseWindow.NameDataColumn] = $s }
                $s = Get-ElementText -XmlElement $XmlElement -Name 'Login';
                if ($DataRow[$BrowseWindow.LoginDataColumn] -ne $s) { $Url[$BrowseWindow.LoginDataColumn] = $s }
                $s = Get-ElementText -XmlElement $XmlElement -Name 'Url';
                if ($DataRow[$BrowseWindow.UrlDataColumn] -ne $s) { $DataRow[$BrowseWindow.UrlDataColumn] = $s }
                $s = Get-ElementText -XmlElement $XmlElement -Name 'Password';
                if ($DataRow[$BrowseWindow.PasswordDataColumn] -ne $s) { $DataRow[$BrowseWindow.PasswordDataColumn] = $s }
                $o = [System.Xml.XmlConvert]::ToInt32((Get-AttributeText -XmlElement $XmlElement -Name 'Order'));
                if ($DataRow[$BrowseWindow.OrderDataColumn] -ne $o) { $DataRow[$BrowseWindow.OrderDataColumn] = $o }
                $DataRow.EndEdit();
                if ($DataRow.RowState -ne [System.Data.DataRowState]::Unchanged) { $DataRow.AcceptChanges(); }
            } else {
                $DataRow = $BrowseWindow.DataSource.NewRow();
                $DataRow.BeginEdit();
                $DataRow[$BrowseWindow.IdDataColumn] = $Id;
                $DataRow[$BrowseWindow.NameDataColumn] = Get-ElementText -XmlElement $XmlElement -Name 'Name';
                $DataRow[$BrowseWindow.LoginDataColumn] = Get-ElementText -XmlElement $XmlElement -Name 'Login';
                $DataRow[$BrowseWindow.UrlDataColumn] = Get-ElementText -XmlElement $XmlElement -Name 'Url';
                $DataRow[$BrowseWindow.PasswordDataColumn] = Get-ElementText -XmlElement $XmlElement -Name 'Password';
                $DataRow[$BrowseWindow.OrderDataColumn] = [System.Xml.XmlConvert]::ToInt32((Get-AttributeText -XmlElement $XmlElement -Name 'Order'));
                $DataRow.EndEdit();
                $BrowseWindow.DataSource.Rows.Add($DataRow);
                $DataRow.AcceptChanges();
            }
        }
        if ($NotProcessed.Count -gt 0) {
            $NotProcessed.Keys | ForEach-Object {
                $BrowseWindow.DataSource.Rows.Remove($NotProcessed[$_]);
            }
        }
        $BrowseWindow.DataSource.AcceptChanges();
        
        $DialogResult = $BrowseWindow.mainForm.ShowDialog((New-WindowOwner));
        if ($BrowseWindow.OrderChanged) { Save-CredentialsDocument }
        New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
            DialogResult = $DialogResult;
            SelectedId = $BrowseWindow.SelectedId;
        };
    } catch {
        throw;
    } finally {
        if ($ClipboardText -eq $null) {
            [System.Windows.Forms.Clipboard]::Clear();
        } else {
            if ($TextDataFormat.Count -gt 0) {
                [System.Windows.Forms.Clipboard]::SetText($ClipboardText, $TextDataFormat[0]);
            } else {
                [System.Windows.Forms.Clipboard]::SetText($ClipboardText);
            }
        }
    }
}

Function Save-CredentialsDocument {
    Param()
    
    $Settings = New-XmlWriterSettings -Indent $true;
    Write-XmlDocument -Document $Script:CredentialsXmlDocument -OutputFileName $Script:CredentialsPath -Settings $Settings;
}

Function Get-ElementText {
    Param(
        [Parameter(Mandatory = $true)]
        [System.Xml.XmlElement]$XmlElement,
        
        [Parameter(Mandatory = $true)]
        [string]$Name
    )
    
    $e = $XmlElement.SelectSingleNode($Name);
    if ($e -eq $null -or $e.IsEmpty) {
        "";
    } else {
        $e.InnerText;
    }
}

Function Get-AttributeText {
    Param(
        [Parameter(Mandatory = $true)]
        [System.Xml.XmlElement]$XmlElement,
        
        [Parameter(Mandatory = $true)]
        [string]$Name
    )
    
    $a = $XmlElement.SelectSingleNode('@' + $Name);
    if ($a -eq $null) {
        "";
    } else {
        $a.value;
    }
}

Function Set-ElementText {
    Param(
        [Parameter(Mandatory = $true)]
        [System.Xml.XmlElement]$XmlElement,
        
        [Parameter(Mandatory = $true)]
        [string]$Name,
        
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value
    )
    
    $e = $XmlElement.SelectSingleNode($Name);
    if ($e -eq $null) { $e = $XmlElement.AppendChild($XmlElement.OwnerDocument.CreateElement($Name)) }
    $e.InnerText = $Value
}

Function Set-AttributeText {
    Param(
        [Parameter(Mandatory = $true)]
        [System.Xml.XmlElement]$XmlElement,
        
        [Parameter(Mandatory = $true)]
        [string]$Name,
        
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value
    )
    
    $a = $XmlElement.SelectSingleNode('@' + $Name);
    if ($a -eq $null) { $a = $XmlElement.Attributes.Append($XmlElement.OwnerDocument.CreateAttribute($Name)) }
    $a.Value = $Value
}

Function Get-YesOrNo {
    Param(
        [Parameter(Mandatory = $true)]
        [string]$Caption,
        
        [Parameter(Mandatory = $true)]
        [string]$Message,
        
        [bool]$DefaultValue = $false
    )
    $Choices = New-Object -TypeName 'System.Collections.ObjectModel.Collection[System.Management.Automation.Host.ChoiceDescription]';
    $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList '_Yes'));
    $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList '_No'));
    $Choices.Add((New-Object -TypeName 'System.Management.Automation.Host.ChoiceDescription' -ArgumentList '_Cancel'));
    if ($DefaultValue) {
        $Index = $Host.UI.PromptForChoice($Caption, $Message, $Choices, 0);
    } else {
        $Index = $Host.UI.PromptForChoice($Caption, $Message, $Choices, 1);
    }
    if ($Index -ne $null) {
        if ($Index -eq 0) {
            $true;
        } else {
            if ($Index -eq 1) { $false }
        }
    }
}

Function Get-BrowserOptionsDataTable {
    Param();
    $BrowerOptionsDataTable = New-Object -TypeName 'System.Data.DataTable';
    $NameDataColumn = $BrowerOptionsDataTable.Columns.Add('Name', [string]);
    $NameDataColumn.AllowDBNull = $false;
    $PathDataColumn = $BrowerOptionsDataTable.Columns.Add('Path', [string]);
    $PathDataColumn.AllowDBNull = $false;
    $DataRow = $BrowerOptionsDataTable.NewRow();
    $DataRow[$NameDataColumn] = '(default)';
    $DataRow[$PathDataColumn] = '';
    $BrowerOptionsDataTable.Rows.Add($DataRow);
    $DataRow.AcceptChanges();
    foreach ($NameAndPath in @((
            @{ Name = 'FireFox'; Executable = 'firefox.exe'; }, 
            @{ Name = 'Chrome'; Executable = 'chrome.exe'; }, 
            @{ Name = 'Internet Explorer'; Executable = 'iexplore.exe'; }
        ) | Foreach-Object {
        @{ Name = $_.Name; Path = 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths' | Join-Path -ChildPath $_.Executable } } | Where-Object { $_.Path | Test-Path -PathType Container })) {
        $DataRow = $BrowerOptionsDataTable.NewRow();
        $DataRow[$NameDataColumn] = $NameAndPath.Name;
        $RegistryKey = Get-Item -Path $NameAndPath.Path;
        $DataRow[$PathDataColumn] = $RegistryKey.GetValue('');
        $BrowerOptionsDataTable.Rows.Add($DataRow);
        $DataRow.AcceptChanges();
    }
    $BrowerOptionsDataTable.AcceptChanges();

    return (,$BrowerOptionsDataTable);
}

$BrowseWindow = New-BrowseWindow;
try {
    $result = Show-BrowseForm -BrowseWindow $BrowseWindow;
    while ($result -ne $null -and ($result.DialogResult -ne [System.Windows.Forms.DialogResult]::Cancel)) {
        switch ($result.DialogResult) {
            { $_ -eq [System.Windows.Forms.DialogResult]::Yes } {
                Show-EditForm -XmlElement $Script:CredentialsXmlDocument.SelectSingleNode(('/Credentials/Credential[@ID="{0}"]' -f $result.SelectedId.ToString()));
                break;
            }
            { $_ -eq [System.Windows.Forms.DialogResult]::No } {
                Show-EditForm;
                break;
            }
            { $_ -eq [System.Windows.Forms.DialogResult]::Abort } {
                Show-EditForm -XmlElement $Script:CredentialsXmlDocument.SelectSingleNode(('/Credentials/Credential[@ID="{0}"]' -f $result.SelectedId.ToString())) -Delete;
                break;
            }
        }
        $result = Show-BrowseForm -BrowseWindow $BrowseWindow;
    }
} finally {
    $BrowseWindow.mainForm.Dispose();
}
