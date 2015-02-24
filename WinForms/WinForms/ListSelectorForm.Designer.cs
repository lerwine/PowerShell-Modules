namespace Erwine.Leonard.T.WinForms
{
    partial class ListSelectorForm
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
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.headingLabel = new System.Windows.Forms.Label();
            this.buttonsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.selectButton = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.optionsListBox = new System.Windows.Forms.ListBox();
            this.mainTableLayoutPanel.SuspendLayout();
            this.buttonsFlowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.ColumnCount = 1;
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayoutPanel.Controls.Add(this.headingLabel, 0, 0);
            this.mainTableLayoutPanel.Controls.Add(this.buttonsFlowLayoutPanel, 0, 2);
            this.mainTableLayoutPanel.Controls.Add(this.optionsListBox, 0, 1);
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.RowCount = 3;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(393, 261);
            this.mainTableLayoutPanel.TabIndex = 1;
            // 
            // headingLabel
            // 
            this.headingLabel.AutoSize = true;
            this.headingLabel.Location = new System.Drawing.Point(3, 0);
            this.headingLabel.Name = "headingLabel";
            this.headingLabel.Size = new System.Drawing.Size(35, 13);
            this.headingLabel.TabIndex = 0;
            this.headingLabel.Text = "label1";
            // 
            // buttonsFlowLayoutPanel
            // 
            this.buttonsFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonsFlowLayoutPanel.AutoSize = true;
            this.buttonsFlowLayoutPanel.Controls.Add(this.cancelButton);
            this.buttonsFlowLayoutPanel.Controls.Add(this.deleteButton);
            this.buttonsFlowLayoutPanel.Controls.Add(this.editButton);
            this.buttonsFlowLayoutPanel.Controls.Add(this.selectButton);
            this.buttonsFlowLayoutPanel.Controls.Add(this.newButton);
            this.buttonsFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonsFlowLayoutPanel.Location = new System.Drawing.Point(3, 232);
            this.buttonsFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.buttonsFlowLayoutPanel.Name = "buttonsFlowLayoutPanel";
            this.buttonsFlowLayoutPanel.Size = new System.Drawing.Size(390, 29);
            this.buttonsFlowLayoutPanel.TabIndex = 1;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(312, 3);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(234, 3);
            this.deleteButton.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 3;
            this.deleteButton.Text = "&Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Visible = false;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // editButton
            // 
            this.editButton.Location = new System.Drawing.Point(156, 3);
            this.editButton.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(75, 23);
            this.editButton.TabIndex = 2;
            this.editButton.Text = "&Edit";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Visible = false;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // selectButton
            // 
            this.selectButton.Location = new System.Drawing.Point(78, 3);
            this.selectButton.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.selectButton.Name = "selectButton";
            this.selectButton.Size = new System.Drawing.Size(75, 23);
            this.selectButton.TabIndex = 1;
            this.selectButton.Text = "&Select";
            this.selectButton.UseVisualStyleBackColor = true;
            this.selectButton.Click += new System.EventHandler(this.selectButton_Click);
            // 
            // newButton
            // 
            this.newButton.Location = new System.Drawing.Point(0, 3);
            this.newButton.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(75, 23);
            this.newButton.TabIndex = 0;
            this.newButton.Text = "&New";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Visible = false;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // optionsListBox
            // 
            this.optionsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optionsListBox.FormattingEnabled = true;
            this.optionsListBox.Location = new System.Drawing.Point(3, 16);
            this.optionsListBox.Name = "optionsListBox";
            this.optionsListBox.Size = new System.Drawing.Size(387, 213);
            this.optionsListBox.TabIndex = 2;
            this.optionsListBox.SelectedIndexChanged += new System.EventHandler(this.optionsListBox_SelectedIndexChanged);
            // 
            // ListSelectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 261);
            this.Controls.Add(this.mainTableLayoutPanel);
            this.MinimumSize = new System.Drawing.Size(409, 150);
            this.Name = "ListSelectorForm";
            this.Text = "ListSelectorForm";
            this.Activated += new System.EventHandler(this.ListSelectorForm_Activated);
            this.Deactivate += new System.EventHandler(this.ListSelectorForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ListSelectorForm_FormClosing);
            this.Load += new System.EventHandler(this.ListSelectorForm_Load);
            this.mainTableLayoutPanel.ResumeLayout(false);
            this.mainTableLayoutPanel.PerformLayout();
            this.buttonsFlowLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
        public System.Windows.Forms.Label headingLabel;
        private System.Windows.Forms.FlowLayoutPanel buttonsFlowLayoutPanel;
        public System.Windows.Forms.Button cancelButton;
        public System.Windows.Forms.Button deleteButton;
        public System.Windows.Forms.Button editButton;
        public System.Windows.Forms.Button selectButton;
        public System.Windows.Forms.Button newButton;
        public System.Windows.Forms.ListBox optionsListBox;
    }
}