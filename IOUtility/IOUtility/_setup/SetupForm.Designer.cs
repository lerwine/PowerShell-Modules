namespace PSModuleInstallUtil
{
    partial class SetupForm
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
            this.moduleNameLabel = new System.Windows.Forms.Label();
            this.moduleDescriptionLabel = new System.Windows.Forms.Label();
            this.statusHeadingLabel = new System.Windows.Forms.Label();
            this.statusTextLabel = new System.Windows.Forms.Label();
            this.installationLocationsDataGridView = new System.Windows.Forms.DataGridView();
            this.installTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.installPathColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.installButton = new System.Windows.Forms.Button();
            this.UninstallButton = new System.Windows.Forms.Button();
            this.updateButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.showCustomLocationButton = new System.Windows.Forms.Button();
            this.installationLocationsHeadingLabel = new System.Windows.Forms.Label();
            this.outerTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.installationLocationsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // outerTableLayoutPanel
            // 
            this.outerTableLayoutPanel.ColumnCount = 5;
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.outerTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.outerTableLayoutPanel.Controls.Add(this.moduleNameLabel, 0, 0);
            this.outerTableLayoutPanel.Controls.Add(this.moduleDescriptionLabel, 0, 1);
            this.outerTableLayoutPanel.Controls.Add(this.statusHeadingLabel, 0, 2);
            this.outerTableLayoutPanel.Controls.Add(this.statusTextLabel, 1, 2);
            this.outerTableLayoutPanel.Controls.Add(this.installationLocationsDataGridView, 0, 4);
            this.outerTableLayoutPanel.Controls.Add(this.installButton, 0, 5);
            this.outerTableLayoutPanel.Controls.Add(this.UninstallButton, 2, 5);
            this.outerTableLayoutPanel.Controls.Add(this.updateButton, 3, 5);
            this.outerTableLayoutPanel.Controls.Add(this.exitButton, 4, 5);
            this.outerTableLayoutPanel.Controls.Add(this.showCustomLocationButton, 3, 3);
            this.outerTableLayoutPanel.Controls.Add(this.installationLocationsHeadingLabel, 0, 3);
            this.outerTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outerTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.outerTableLayoutPanel.Name = "outerTableLayoutPanel";
            this.outerTableLayoutPanel.RowCount = 6;
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.outerTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.outerTableLayoutPanel.Size = new System.Drawing.Size(669, 452);
            this.outerTableLayoutPanel.TabIndex = 0;
            // 
            // moduleNameLabel
            // 
            this.moduleNameLabel.AutoSize = true;
            this.outerTableLayoutPanel.SetColumnSpan(this.moduleNameLabel, 5);
            this.moduleNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.moduleNameLabel.Location = new System.Drawing.Point(3, 0);
            this.moduleNameLabel.Name = "moduleNameLabel";
            this.moduleNameLabel.Size = new System.Drawing.Size(104, 16);
            this.moduleNameLabel.TabIndex = 0;
            this.moduleNameLabel.Text = "Module Name";
            // 
            // moduleDescriptionLabel
            // 
            this.moduleDescriptionLabel.AutoSize = true;
            this.outerTableLayoutPanel.SetColumnSpan(this.moduleDescriptionLabel, 5);
            this.moduleDescriptionLabel.Location = new System.Drawing.Point(3, 16);
            this.moduleDescriptionLabel.Name = "moduleDescriptionLabel";
            this.moduleDescriptionLabel.Size = new System.Drawing.Size(98, 13);
            this.moduleDescriptionLabel.TabIndex = 1;
            this.moduleDescriptionLabel.Text = "Module Description";
            // 
            // statusHeadingLabel
            // 
            this.statusHeadingLabel.AutoSize = true;
            this.statusHeadingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusHeadingLabel.Location = new System.Drawing.Point(3, 32);
            this.statusHeadingLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.statusHeadingLabel.Name = "statusHeadingLabel";
            this.statusHeadingLabel.Size = new System.Drawing.Size(47, 13);
            this.statusHeadingLabel.TabIndex = 2;
            this.statusHeadingLabel.Text = "Status:";
            // 
            // statusTextLabel
            // 
            this.statusTextLabel.AutoSize = true;
            this.outerTableLayoutPanel.SetColumnSpan(this.statusTextLabel, 4);
            this.statusTextLabel.Location = new System.Drawing.Point(56, 32);
            this.statusTextLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.statusTextLabel.Name = "statusTextLabel";
            this.statusTextLabel.Size = new System.Drawing.Size(106, 13);
            this.statusTextLabel.TabIndex = 3;
            this.statusTextLabel.Text = "Not found / installed.";
            // 
            // installationLocationsDataGridView
            // 
            this.installationLocationsDataGridView.AllowUserToAddRows = false;
            this.installationLocationsDataGridView.AllowUserToDeleteRows = false;
            this.installationLocationsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.installationLocationsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.installTypeColumn,
            this.statusColumn,
            this.installPathColumn});
            this.outerTableLayoutPanel.SetColumnSpan(this.installationLocationsDataGridView, 5);
            this.installationLocationsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.installationLocationsDataGridView.Location = new System.Drawing.Point(3, 77);
            this.installationLocationsDataGridView.Name = "installationLocationsDataGridView";
            this.installationLocationsDataGridView.ReadOnly = true;
            this.installationLocationsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.installationLocationsDataGridView.Size = new System.Drawing.Size(663, 343);
            this.installationLocationsDataGridView.TabIndex = 5;
            this.installationLocationsDataGridView.SelectionChanged += new System.EventHandler(this.installationLocationsDataGridView_SelectionChanged);
            // 
            // installTypeColumn
            // 
            this.installTypeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.installTypeColumn.DataPropertyName = "InstallType";
            this.installTypeColumn.HeaderText = "Type";
            this.installTypeColumn.Name = "installTypeColumn";
            this.installTypeColumn.ReadOnly = true;
            this.installTypeColumn.Width = 56;
            // 
            // statusColumn
            // 
            this.statusColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.statusColumn.DataPropertyName = "StatusText";
            this.statusColumn.HeaderText = "Status";
            this.statusColumn.Name = "statusColumn";
            this.statusColumn.ReadOnly = true;
            this.statusColumn.Width = 62;
            // 
            // installPathColumn
            // 
            this.installPathColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.installPathColumn.DataPropertyName = "InstallRoot";
            this.installPathColumn.HeaderText = "Path";
            this.installPathColumn.Name = "installPathColumn";
            this.installPathColumn.ReadOnly = true;
            // 
            // installButton
            // 
            this.installButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outerTableLayoutPanel.SetColumnSpan(this.installButton, 2);
            this.installButton.Enabled = false;
            this.installButton.Location = new System.Drawing.Point(348, 426);
            this.installButton.Name = "installButton";
            this.installButton.Size = new System.Drawing.Size(75, 23);
            this.installButton.TabIndex = 6;
            this.installButton.Text = "Install";
            this.installButton.UseVisualStyleBackColor = true;
            this.installButton.Click += new System.EventHandler(this.installButton_Click);
            // 
            // UninstallButton
            // 
            this.UninstallButton.Enabled = false;
            this.UninstallButton.Location = new System.Drawing.Point(429, 426);
            this.UninstallButton.Name = "UninstallButton";
            this.UninstallButton.Size = new System.Drawing.Size(75, 23);
            this.UninstallButton.TabIndex = 7;
            this.UninstallButton.Text = "Uninstall";
            this.UninstallButton.UseVisualStyleBackColor = true;
            this.UninstallButton.Click += new System.EventHandler(this.UninstallButton_Click);
            // 
            // updateButton
            // 
            this.updateButton.Enabled = false;
            this.updateButton.Location = new System.Drawing.Point(510, 426);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(75, 23);
            this.updateButton.TabIndex = 8;
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.Location = new System.Drawing.Point(591, 426);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(75, 23);
            this.exitButton.TabIndex = 9;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // showCustomLocationButton
            // 
            this.showCustomLocationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outerTableLayoutPanel.SetColumnSpan(this.showCustomLocationButton, 2);
            this.showCustomLocationButton.Location = new System.Drawing.Point(541, 48);
            this.showCustomLocationButton.Name = "showCustomLocationButton";
            this.showCustomLocationButton.Size = new System.Drawing.Size(125, 23);
            this.showCustomLocationButton.TabIndex = 4;
            this.showCustomLocationButton.Text = "Add Custom Location";
            this.showCustomLocationButton.UseVisualStyleBackColor = true;
            this.showCustomLocationButton.Click += new System.EventHandler(this.showCustomLocationButton_Click);
            // 
            // installationLocationsHeadingLabel
            // 
            this.installationLocationsHeadingLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.installationLocationsHeadingLabel.AutoSize = true;
            this.outerTableLayoutPanel.SetColumnSpan(this.installationLocationsHeadingLabel, 3);
            this.installationLocationsHeadingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.installationLocationsHeadingLabel.Location = new System.Drawing.Point(3, 61);
            this.installationLocationsHeadingLabel.Name = "installationLocationsHeadingLabel";
            this.installationLocationsHeadingLabel.Size = new System.Drawing.Size(132, 13);
            this.installationLocationsHeadingLabel.TabIndex = 10;
            this.installationLocationsHeadingLabel.Text = "Installation Locations:";
            // 
            // SetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 452);
            this.Controls.Add(this.outerTableLayoutPanel);
            this.Name = "SetupForm";
            this.Text = "Setup";
            this.TopMost = true;
            this.outerTableLayoutPanel.ResumeLayout(false);
            this.outerTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.installationLocationsDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel outerTableLayoutPanel;
        private System.Windows.Forms.Label moduleNameLabel;
        private System.Windows.Forms.Label moduleDescriptionLabel;
        private System.Windows.Forms.Label statusHeadingLabel;
        private System.Windows.Forms.Label statusTextLabel;
        private System.Windows.Forms.DataGridView installationLocationsDataGridView;
        private System.Windows.Forms.Button installButton;
        private System.Windows.Forms.Button UninstallButton;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button showCustomLocationButton;
        private System.Windows.Forms.Label installationLocationsHeadingLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn installTypeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn statusColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn installPathColumn;
    }
}