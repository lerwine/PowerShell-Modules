namespace PSModuleInstallUtil
{
    partial class InstallLocationSelectForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.outerTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.installLocationsDataGridView = new System.Windows.Forms.DataGridView();
            this.nameTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.locationTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.existsTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.isInstallableTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reasonTextboxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.isAllUsersTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.instructionsLabel = new System.Windows.Forms.Label();
            this.exitButton = new System.Windows.Forms.Button();
            this.locationTypeTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.commonLocationsRadioButton = new System.Windows.Forms.RadioButton();
            this.customLocationadioButton = new System.Windows.Forms.RadioButton();
            this.customPathTextBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.pathErrorLabel = new System.Windows.Forms.Label();
            this.installButton = new System.Windows.Forms.Button();
            this.uninstallButton = new System.Windows.Forms.Button();
            this.overwriteCheckBox = new System.Windows.Forms.CheckBox();
            this.outerTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.installLocationsDataGridView)).BeginInit();
            this.locationTypeTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // outerTableLayoutPanel
            // 
            this.outerTableLayoutPanel.ColumnCount = 3;
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.outerTableLayoutPanel.Controls.Add(this.installLocationsDataGridView, 0, 2);
            this.outerTableLayoutPanel.Controls.Add(this.instructionsLabel, 0, 0);
            this.outerTableLayoutPanel.Controls.Add(this.exitButton, 2, 7);
            this.outerTableLayoutPanel.Controls.Add(this.locationTypeTableLayoutPanel, 0, 1);
            this.outerTableLayoutPanel.Controls.Add(this.customPathTextBox, 0, 4);
            this.outerTableLayoutPanel.Controls.Add(this.browseButton, 2, 4);
            this.outerTableLayoutPanel.Controls.Add(this.pathErrorLabel, 0, 6);
            this.outerTableLayoutPanel.Controls.Add(this.installButton, 1, 7);
            this.outerTableLayoutPanel.Controls.Add(this.uninstallButton, 0, 7);
            this.outerTableLayoutPanel.Controls.Add(this.overwriteCheckBox, 0, 5);
            this.outerTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outerTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.outerTableLayoutPanel.Name = "outerTableLayoutPanel";
            this.outerTableLayoutPanel.RowCount = 8;
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.Size = new System.Drawing.Size(624, 441);
            this.outerTableLayoutPanel.TabIndex = 0;
            // 
            // installLocationsDataGridView
            // 
            this.installLocationsDataGridView.AllowUserToAddRows = false;
            this.installLocationsDataGridView.AllowUserToDeleteRows = false;
            this.installLocationsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.installLocationsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameTextBoxColumn,
            this.locationTextBoxColumn,
            this.existsTextBoxColumn,
            this.isInstallableTextBoxColumn,
            this.reasonTextboxColumn,
            this.isAllUsersTextBoxColumn});
            this.outerTableLayoutPanel.SetColumnSpan(this.installLocationsDataGridView, 3);
            this.installLocationsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.installLocationsDataGridView.Location = new System.Drawing.Point(3, 45);
            this.installLocationsDataGridView.Name = "installLocationsDataGridView";
            this.installLocationsDataGridView.ReadOnly = true;
            this.installLocationsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.installLocationsDataGridView.Size = new System.Drawing.Size(618, 299);
            this.installLocationsDataGridView.TabIndex = 5;
            this.installLocationsDataGridView.SelectionChanged += new System.EventHandler(this.installLocationsDataGridView_SelectionChanged);
            // 
            // nameTextBoxColumn
            // 
            this.nameTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.nameTextBoxColumn.DataPropertyName = "Name";
            this.nameTextBoxColumn.HeaderText = "Name";
            this.nameTextBoxColumn.Name = "nameTextBoxColumn";
            this.nameTextBoxColumn.ReadOnly = true;
            this.nameTextBoxColumn.Width = 60;
            // 
            // locationTextBoxColumn
            // 
            this.locationTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.locationTextBoxColumn.DataPropertyName = "RelativePath";
            this.locationTextBoxColumn.HeaderText = "Location";
            this.locationTextBoxColumn.Name = "locationTextBoxColumn";
            this.locationTextBoxColumn.ReadOnly = true;
            this.locationTextBoxColumn.Width = 73;
            // 
            // existsTextBoxColumn
            // 
            this.existsTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.existsTextBoxColumn.DataPropertyName = "ExistsText";
            this.existsTextBoxColumn.HeaderText = "Exists";
            this.existsTextBoxColumn.Name = "existsTextBoxColumn";
            this.existsTextBoxColumn.ReadOnly = true;
            this.existsTextBoxColumn.Width = 59;
            // 
            // isInstallableTextBoxColumn
            // 
            this.isInstallableTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.isInstallableTextBoxColumn.DataPropertyName = "IsInstallableText";
            this.isInstallableTextBoxColumn.HeaderText = "Is Installable";
            this.isInstallableTextBoxColumn.Name = "isInstallableTextBoxColumn";
            this.isInstallableTextBoxColumn.ReadOnly = true;
            this.isInstallableTextBoxColumn.Width = 90;
            // 
            // reasonTextboxColumn
            // 
            this.reasonTextboxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.reasonTextboxColumn.DataPropertyName = "Reason";
            this.reasonTextboxColumn.HeaderText = "Reason";
            this.reasonTextboxColumn.Name = "reasonTextboxColumn";
            this.reasonTextboxColumn.ReadOnly = true;
            // 
            // isAllUsersTextBoxColumn
            // 
            this.isAllUsersTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.isAllUsersTextBoxColumn.DataPropertyName = "IsAllUsersText";
            this.isAllUsersTextBoxColumn.HeaderText = "All Users";
            this.isAllUsersTextBoxColumn.Name = "isAllUsersTextBoxColumn";
            this.isAllUsersTextBoxColumn.ReadOnly = true;
            this.isAllUsersTextBoxColumn.Width = 73;
            // 
            // instructionsLabel
            // 
            this.instructionsLabel.AutoSize = true;
            this.outerTableLayoutPanel.SetColumnSpan(this.instructionsLabel, 3);
            this.instructionsLabel.Location = new System.Drawing.Point(3, 0);
            this.instructionsLabel.Name = "instructionsLabel";
            this.instructionsLabel.Size = new System.Drawing.Size(132, 13);
            this.instructionsLabel.TabIndex = 2;
            this.instructionsLabel.Text = "Select installation location.";
            // 
            // exitButton
            // 
            this.exitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.exitButton.Location = new System.Drawing.Point(546, 415);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(75, 23);
            this.exitButton.TabIndex = 4;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // locationTypeTableLayoutPanel
            // 
            this.locationTypeTableLayoutPanel.ColumnCount = 2;
            this.outerTableLayoutPanel.SetColumnSpan(this.locationTypeTableLayoutPanel, 3);
            this.locationTypeTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.locationTypeTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.locationTypeTableLayoutPanel.Controls.Add(this.commonLocationsRadioButton, 0, 0);
            this.locationTypeTableLayoutPanel.Controls.Add(this.customLocationadioButton, 1, 0);
            this.locationTypeTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.locationTypeTableLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this.locationTypeTableLayoutPanel.Name = "locationTypeTableLayoutPanel";
            this.locationTypeTableLayoutPanel.RowCount = 1;
            this.locationTypeTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.locationTypeTableLayoutPanel.Size = new System.Drawing.Size(618, 23);
            this.locationTypeTableLayoutPanel.TabIndex = 6;
            // 
            // commonLocationsRadioButton
            // 
            this.commonLocationsRadioButton.AutoSize = true;
            this.commonLocationsRadioButton.Checked = true;
            this.commonLocationsRadioButton.Location = new System.Drawing.Point(3, 3);
            this.commonLocationsRadioButton.Name = "commonLocationsRadioButton";
            this.commonLocationsRadioButton.Size = new System.Drawing.Size(115, 17);
            this.commonLocationsRadioButton.TabIndex = 0;
            this.commonLocationsRadioButton.TabStop = true;
            this.commonLocationsRadioButton.Text = "Common Locations";
            this.commonLocationsRadioButton.UseVisualStyleBackColor = true;
            this.commonLocationsRadioButton.CheckedChanged += new System.EventHandler(this.commonLocationsRadioButton_CheckedChanged);
            // 
            // customLocationadioButton
            // 
            this.customLocationadioButton.AutoSize = true;
            this.customLocationadioButton.Location = new System.Drawing.Point(124, 3);
            this.customLocationadioButton.Name = "customLocationadioButton";
            this.customLocationadioButton.Size = new System.Drawing.Size(104, 17);
            this.customLocationadioButton.TabIndex = 1;
            this.customLocationadioButton.Text = "Custom Location";
            this.customLocationadioButton.UseVisualStyleBackColor = true;
            this.customLocationadioButton.CheckedChanged += new System.EventHandler(this.customLocationadioButton_CheckedChanged);
            // 
            // customPathTextBox
            // 
            this.outerTableLayoutPanel.SetColumnSpan(this.customPathTextBox, 2);
            this.customPathTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.customPathTextBox.Location = new System.Drawing.Point(3, 350);
            this.customPathTextBox.Name = "customPathTextBox";
            this.customPathTextBox.Size = new System.Drawing.Size(537, 20);
            this.customPathTextBox.TabIndex = 7;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(546, 350);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 8;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // pathErrorLabel
            // 
            this.pathErrorLabel.AutoSize = true;
            this.outerTableLayoutPanel.SetColumnSpan(this.pathErrorLabel, 2);
            this.pathErrorLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.pathErrorLabel.Location = new System.Drawing.Point(3, 399);
            this.pathErrorLabel.Name = "pathErrorLabel";
            this.pathErrorLabel.Size = new System.Drawing.Size(117, 13);
            this.pathErrorLabel.TabIndex = 9;
            this.pathErrorLabel.Text = "Path must be specified.";
            // 
            // installButton
            // 
            this.installButton.Location = new System.Drawing.Point(465, 415);
            this.installButton.Name = "installButton";
            this.installButton.Size = new System.Drawing.Size(75, 23);
            this.installButton.TabIndex = 3;
            this.installButton.Text = "Install";
            this.installButton.UseVisualStyleBackColor = true;
            this.installButton.Click += new System.EventHandler(this.installButton_Click);
            // 
            // uninstallButton
            // 
            this.uninstallButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uninstallButton.Location = new System.Drawing.Point(384, 415);
            this.uninstallButton.Name = "uninstallButton";
            this.uninstallButton.Size = new System.Drawing.Size(75, 23);
            this.uninstallButton.TabIndex = 10;
            this.uninstallButton.Text = "Uninstall";
            this.uninstallButton.UseVisualStyleBackColor = true;
            this.uninstallButton.Click += new System.EventHandler(this.uninstallButton_Click);
            // 
            // overwriteCheckBox
            // 
            this.overwriteCheckBox.AutoSize = true;
            this.overwriteCheckBox.Location = new System.Drawing.Point(3, 379);
            this.overwriteCheckBox.Name = "overwriteCheckBox";
            this.overwriteCheckBox.Size = new System.Drawing.Size(71, 17);
            this.overwriteCheckBox.TabIndex = 11;
            this.overwriteCheckBox.Text = "Overwrite";
            this.overwriteCheckBox.UseVisualStyleBackColor = true;
            // 
            // InstallLocationSelectForm
            // 
            this.AcceptButton = this.installButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.exitButton;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.outerTableLayoutPanel);
            this.Name = "InstallLocationSelectForm";
            this.Text = "InstallLocationSelectForm";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.InstallLocationSelectForm_Shown);
            this.outerTableLayoutPanel.ResumeLayout(false);
            this.outerTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.installLocationsDataGridView)).EndInit();
            this.locationTypeTableLayoutPanel.ResumeLayout(false);
            this.locationTypeTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel outerTableLayoutPanel;
        private System.Windows.Forms.DataGridView installLocationsDataGridView;
        private System.Windows.Forms.Label instructionsLabel;
        private System.Windows.Forms.Button installButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.TableLayoutPanel locationTypeTableLayoutPanel;
        private System.Windows.Forms.RadioButton commonLocationsRadioButton;
        private System.Windows.Forms.RadioButton customLocationadioButton;
        private System.Windows.Forms.TextBox customPathTextBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label pathErrorLabel;
        private System.Windows.Forms.Button uninstallButton;
        private System.Windows.Forms.CheckBox overwriteCheckBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn locationTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn existsTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn isInstallableTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn reasonTextboxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn isAllUsersTextBoxColumn;
    }
}