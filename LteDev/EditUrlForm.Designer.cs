namespace LteDev
{
    partial class EditUrlForm
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
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.parseTabPage = new System.Windows.Forms.TabPage();
            this.parseTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.urlLabel = new System.Windows.Forms.Label();
            this.urlErrorLabel = new System.Windows.Forms.Label();
            this.buildButton = new System.Windows.Forms.Button();
            this.parseButton = new System.Windows.Forms.Button();
            this.buildTabPage = new System.Windows.Forms.TabPage();
            this.buildTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.userNameCheckBox = new System.Windows.Forms.CheckBox();
            this.schemeBuildTextBox = new System.Windows.Forms.TextBox();
            this.schemeSeparatorLabel = new System.Windows.Forms.Label();
            this.userNameBuildTextBox = new System.Windows.Forms.TextBox();
            this.passwordSeparatorLabel = new System.Windows.Forms.Label();
            this.passwordCheckBox = new System.Windows.Forms.CheckBox();
            this.authSeparatorLabel = new System.Windows.Forms.Label();
            this.hostCheckBox = new System.Windows.Forms.CheckBox();
            this.hostBuildTextBox = new System.Windows.Forms.TextBox();
            this.portSeparatorLabel = new System.Windows.Forms.Label();
            this.portBuildTextBox = new System.Windows.Forms.TextBox();
            this.pathBuildLabel = new System.Windows.Forms.Label();
            this.pathBuildTextBox = new System.Windows.Forms.TextBox();
            this.passwordBuildTextBox = new System.Windows.Forms.TextBox();
            this.portCheckBox = new System.Windows.Forms.CheckBox();
            this.queryCheckBox = new System.Windows.Forms.CheckBox();
            this.fragmentBuildCheckBox = new System.Windows.Forms.CheckBox();
            this.fragmentBuildTextBox = new System.Windows.Forms.TextBox();
            this.schemeBuildLabel = new System.Windows.Forms.Label();
            this.portMessageLabel = new System.Windows.Forms.Label();
            this.querySplitContainer = new System.Windows.Forms.SplitContainer();
            this.queryItemsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.queryBuildDataGridView = new System.Windows.Forms.DataGridView();
            this.addQueryItemButton = new System.Windows.Forms.Button();
            this.editQueryItemTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.queryItemOrderLabel = new System.Windows.Forms.Label();
            this.queryItemKeyLabel = new System.Windows.Forms.Label();
            this.queryItemOrderTextBox = new System.Windows.Forms.TextBox();
            this.queryItemKeyTextBox = new System.Windows.Forms.TextBox();
            this.queryItemValueTextBox = new System.Windows.Forms.TextBox();
            this.editQueryItemFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.saveQueryItemButton = new System.Windows.Forms.Button();
            this.moveUpQueryItemButton = new System.Windows.Forms.Button();
            this.moveDownQueryItemButton = new System.Windows.Forms.Button();
            this.addQueryItemButton = new System.Windows.Forms.Button();
            this.insertQueryItemButton = new System.Windows.Forms.Button();
            this.cancelQueryItemButton = new System.Windows.Forms.Button();
            this.deleteQueryItemButton = new System.Windows.Forms.Button();
            this.queryItemValueCheckBox = new System.Windows.Forms.CheckBox();
            this.buildUrlButton = new System.Windows.Forms.Button();
            this.buildErrorLabel = new System.Windows.Forms.Label();
            this.componentsTabPage = new System.Windows.Forms.TabPage();
            this.componentsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.schemeComponentLabel = new System.Windows.Forms.Label();
            this.schemeComponentTextBox = new System.Windows.Forms.TextBox();
            this.userNameComponentLabel = new System.Windows.Forms.Label();
            this.userNameComponentTextBox = new System.Windows.Forms.TextBox();
            this.passwordComponentLabel = new System.Windows.Forms.Label();
            this.passwordComponentTextBox = new System.Windows.Forms.TextBox();
            this.hostComponentLabel = new System.Windows.Forms.Label();
            this.hostComponentTextBox = new System.Windows.Forms.TextBox();
            this.portComponentLabel = new System.Windows.Forms.Label();
            this.portComponentTextBox = new System.Windows.Forms.TextBox();
            this.portComponentDescLabel = new System.Windows.Forms.Label();
            this.segmentsComponentLabel = new System.Windows.Forms.Label();
            this.segmentsComponentDataGridView = new System.Windows.Forms.DataGridView();
            this.queryNotIncludedLabel = new System.Windows.Forms.Label();
            this.queryComponentLabel = new System.Windows.Forms.Label();
            this.queryComponentDataGridView = new System.Windows.Forms.DataGridView();
            this.fragmentComponentLabel = new System.Windows.Forms.Label();
            this.fragmentComponentTextBox = new System.Windows.Forms.TextBox();
            this.parentDirButton = new System.Windows.Forms.Button();
            this.mainTabControl.SuspendLayout();
            this.parseTabPage.SuspendLayout();
            this.parseTableLayoutPanel.SuspendLayout();
            this.buildTabPage.SuspendLayout();
            this.buildTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.querySplitContainer)).BeginInit();
            this.querySplitContainer.Panel1.SuspendLayout();
            this.querySplitContainer.Panel2.SuspendLayout();
            this.querySplitContainer.SuspendLayout();
            this.queryItemsTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.queryBuildDataGridView)).BeginInit();
            this.editQueryItemTableLayoutPanel.SuspendLayout();
            this.editQueryItemFlowLayoutPanel.SuspendLayout();
            this.componentsTabPage.SuspendLayout();
            this.componentsTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.segmentsComponentDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.queryComponentDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // mainTabControl
            // 
            this.mainTabControl.AutoSize = true;
            this.mainTabControl.Controls.Add(this.parseTabPage);
            this.mainTabControl.Controls.Add(this.buildTabPage);
            this.mainTabControl.Controls.Add(this.componentsTabPage);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 0);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(784, 561);
            this.mainTabControl.TabIndex = 0;
            this.mainTabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.mainTabControl_Selecting);
            // 
            // parseTabPage
            // 
            this.parseTabPage.Controls.Add(this.parseTableLayoutPanel);
            this.parseTabPage.Location = new System.Drawing.Point(4, 22);
            this.parseTabPage.Name = "parseTabPage";
            this.parseTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.parseTabPage.Size = new System.Drawing.Size(776, 535);
            this.parseTabPage.TabIndex = 1;
            this.parseTabPage.Text = "Parse";
            this.parseTabPage.UseVisualStyleBackColor = true;
            // 
            // parseTableLayoutPanel
            // 
            this.parseTableLayoutPanel.AutoSize = true;
            this.parseTableLayoutPanel.ColumnCount = 2;
            this.parseTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.parseTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.parseTableLayoutPanel.Controls.Add(this.urlTextBox, 0, 1);
            this.parseTableLayoutPanel.Controls.Add(this.urlLabel, 0, 0);
            this.parseTableLayoutPanel.Controls.Add(this.urlErrorLabel, 0, 2);
            this.parseTableLayoutPanel.Controls.Add(this.buildButton, 0, 3);
            this.parseTableLayoutPanel.Controls.Add(this.parseButton, 1, 3);
            this.parseTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.parseTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.parseTableLayoutPanel.Name = "parseTableLayoutPanel";
            this.parseTableLayoutPanel.RowCount = 4;
            this.parseTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.parseTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.parseTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.parseTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.parseTableLayoutPanel.Size = new System.Drawing.Size(770, 81);
            this.parseTableLayoutPanel.TabStop = false;
            // 
            // urlTextBox
            // 
            this.parseTableLayoutPanel.SetColumnSpan(this.urlTextBox, 2);
            this.urlTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.urlTextBox.Location = new System.Drawing.Point(3, 16);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(764, 20);
            this.urlTextBox.TabIndex = 2;
            // 
            // urlLabel
            // 
            this.urlLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.urlLabel.AutoSize = true;
            this.parseTableLayoutPanel.SetColumnSpan(this.urlLabel, 2);
            this.urlLabel.Location = new System.Drawing.Point(3, 0);
            this.urlLabel.Name = "urlLabel";
            this.urlLabel.Size = new System.Drawing.Size(29, 13);
            this.urlLabel.TabStop = false;
            this.urlLabel.Text = "URL";
            // 
            // urlErrorLabel
            // 
            this.urlErrorLabel.AutoSize = true;
            this.parseTableLayoutPanel.SetColumnSpan(this.urlErrorLabel, 2);
            this.urlErrorLabel.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.urlErrorLabel.Location = new System.Drawing.Point(3, 39);
            this.urlErrorLabel.Name = "urlErrorLabel";
            this.urlErrorLabel.Size = new System.Drawing.Size(73, 13);
            this.urlErrorLabel.TabStop = false;
            this.urlErrorLabel.Text = "URL is empty.";
            // 
            // buildButton
            // 
            this.buildButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buildButton.Location = new System.Drawing.Point(611, 55);
            this.buildButton.Name = "buildButton";
            this.buildButton.Size = new System.Drawing.Size(75, 23);
            this.buildButton.TabIndex = 3;
            this.buildButton.Text = "Build";
            this.buildButton.UseVisualStyleBackColor = true;
            this.buildButton.Click += new System.EventHandler(this.buildButton_Click);
            // 
            // parseButton
            // 
            this.parseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.parseButton.Location = new System.Drawing.Point(692, 55);
            this.parseButton.Name = "parseButton";
            this.parseButton.Size = new System.Drawing.Size(75, 23);
            this.parseButton.TabIndex = 4;
            this.parseButton.Text = "Parse";
            this.parseButton.UseVisualStyleBackColor = true;
            this.parseButton.Click += new System.EventHandler(this.parseButton_Click);
            // 
            // buildTabPage
            // 
            this.buildTabPage.Controls.Add(this.buildTableLayoutPanel);
            this.buildTabPage.Location = new System.Drawing.Point(4, 22);
            this.buildTabPage.Name = "buildTabPage";
            this.buildTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.buildTabPage.Size = new System.Drawing.Size(776, 535);
            this.buildTabPage.TabIndex = 5;
            this.buildTabPage.Text = "Build";
            this.buildTabPage.UseVisualStyleBackColor = true;
            // 
            // buildTableLayoutPanel
            // 
            this.buildTableLayoutPanel.AutoSize = true;
            this.buildTableLayoutPanel.ColumnCount = 10;
            this.buildTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.63636F));
            this.buildTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buildTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.63636F));
            this.buildTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buildTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.63636F));
            this.buildTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buildTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.27273F));
            this.buildTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buildTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buildTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buildTableLayoutPanel.Controls.Add(this.schemeBuildLabel, 0, 0);
            this.buildTableLayoutPanel.Controls.Add(this.userNameCheckBox, 2, 0);
            this.buildTableLayoutPanel.Controls.Add(this.passwordCheckBox, 4, 0);
            this.buildTableLayoutPanel.Controls.Add(this.hostCheckBox, 6, 0);
            this.buildTableLayoutPanel.Controls.Add(this.portCheckBox, 8, 0);
            this.buildTableLayoutPanel.Controls.Add(this.schemeBuildTextBox, 0, 1);
            this.buildTableLayoutPanel.Controls.Add(this.schemeSeparatorLabel, 1, 1);
            this.buildTableLayoutPanel.Controls.Add(this.userNameBuildTextBox, 2, 1);
            this.buildTableLayoutPanel.Controls.Add(this.passwordSeparatorLabel, 3, 1);
            this.buildTableLayoutPanel.Controls.Add(this.passwordBuildTextBox, 4, 1);
            this.buildTableLayoutPanel.Controls.Add(this.authSeparatorLabel, 5, 1);
            this.buildTableLayoutPanel.Controls.Add(this.hostBuildTextBox, 6, 1);
            this.buildTableLayoutPanel.Controls.Add(this.portSeparatorLabel, 7, 1);
            this.buildTableLayoutPanel.Controls.Add(this.portBuildTextBox, 8, 1);
            this.buildTableLayoutPanel.Controls.Add(this.portMessageLabel, 9, 1);
            this.buildTableLayoutPanel.Controls.Add(this.pathBuildLabel, 0, 2);
            this.buildTableLayoutPanel.Controls.Add(this.pathBuildTextBox, 0, 3);
            this.buildTableLayoutPanel.Controls.Add(this.queryCheckBox, 0, 4);
            this.buildTableLayoutPanel.Controls.Add(this.querySplitContainer, 0, 5);
            this.buildTableLayoutPanel.Controls.Add(this.fragmentBuildCheckBox, 0, 6);
            this.buildTableLayoutPanel.Controls.Add(this.fragmentBuildTextBox, 0, 7);
            this.buildTableLayoutPanel.Controls.Add(this.buildUrlButton, 0, 8);
            this.buildTableLayoutPanel.Controls.Add(this.buildErrorLabel, 0, 9);
            this.buildTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buildTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.buildTableLayoutPanel.Name = "buildTableLayoutPanel";
            this.buildTableLayoutPanel.RowCount = 10;
            this.buildTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.buildTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.buildTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.buildTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.buildTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.buildTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.buildTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.buildTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.buildTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.buildTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.buildTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.buildTableLayoutPanel.Size = new System.Drawing.Size(770, 529);
            this.buildTableLayoutPanel.TabStop = false;
            // 
            // schemeBuildLabel
            // 
            this.schemeBuildLabel.AutoSize = true;
            this.buildTableLayoutPanel.SetColumnSpan(this.schemeBuildLabel, 2);
            this.schemeBuildLabel.Location = new System.Drawing.Point(3, 3);
            this.schemeBuildLabel.Name = "schemeBuildLabel";
            this.schemeBuildLabel.Size = new System.Drawing.Size(29, 13);
            this.schemeBuildLabel.TabStop = false;
            this.schemeBuildLabel.Text = "Scheme";
            this.schemeBuildLabel.Visible = false;
            // 
            // userNameCheckBox
            // 
            this.userNameCheckBox.AutoSize = true;
            this.buildTableLayoutPanel.SetColumnSpan(this.userNameCheckBox, 2);
            this.userNameCheckBox.Location = new System.Drawing.Point(142, 3);
            this.userNameCheckBox.Name = "userNameCheckBox";
            this.userNameCheckBox.Size = new System.Drawing.Size(76, 17);
            this.userNameCheckBox.TabIndex = 6;
            this.userNameCheckBox.Text = "UserName";
            this.userNameCheckBox.UseVisualStyleBackColor = true;
            this.userNameCheckBox.CheckedChanged += new System.EventHandler(this.userNameCheckBox_CheckedChanged);
            this.userNameCheckBox.Visible = false;
            // 
            // passwordCheckBox
            // 
            this.passwordCheckBox.AutoSize = true;
            this.buildTableLayoutPanel.SetColumnSpan(this.passwordCheckBox, 2);
            this.passwordCheckBox.Location = new System.Drawing.Point(271, 3);
            this.passwordCheckBox.Name = "passwordCheckBox";
            this.passwordCheckBox.Size = new System.Drawing.Size(72, 17);
            this.passwordCheckBox.TabIndex = 7;
            this.passwordCheckBox.Text = "Password";
            this.passwordCheckBox.UseVisualStyleBackColor = true;
            this.passwordCheckBox.CheckedChanged += new System.EventHandler(this.passwordCheckBox_CheckedChanged);
            this.passwordCheckBox.Visible = false;
            // 
            // hostCheckBox
            // 
            this.hostCheckBox.AutoSize = true;
            this.hostCheckBox.Location = new System.Drawing.Point(408, 0);
            this.hostCheckBox.Name = "hostCheckBox";
            this.hostCheckBox.Size = new System.Drawing.Size(65, 17);
            this.hostCheckBox.TabIndex = 8;
            this.hostCheckBox.Text = "Host";
            this.hostCheckBox.UseVisualStyleBackColor = true;
            this.hostCheckBox.CheckedChanged += new System.EventHandler(this.hostCheckBox_CheckedChanged);
            // 
            // portCheckBox
            // 
            this.portCheckBox.AutoSize = true;
            this.portCheckBox.Location = new System.Drawing.Point(650, 3);
            this.portCheckBox.Name = "portCheckBox";
            this.portCheckBox.Size = new System.Drawing.Size(45, 17);
            this.portCheckBox.TabIndex = 9;
            this.portCheckBox.Text = "Port";
            this.portCheckBox.UseVisualStyleBackColor = true;
            this.portCheckBox.CheckedChanged += new System.EventHandler(this.portCheckBox_CheckedChanged);
            this.portCheckBox.Visible = false;
            // 
            // schemeBuildTextBox
            // 
            this.schemeBuildTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.schemeBuildTextBox.Location = new System.Drawing.Point(3, 26);
            this.schemeBuildTextBox.Name = "schemeBuildTextBox";
            this.schemeBuildTextBox.Size = new System.Drawing.Size(107, 20);
            this.schemeBuildTextBox.TabIndex = 10;
            this.schemeBuildTextBox.Visible = false;
            // 
            // schemeSeparatorLabel
            // 
            this.schemeSeparatorLabel.AutoSize = true;
            this.schemeSeparatorLabel.Location = new System.Drawing.Point(116, 23);
            this.schemeSeparatorLabel.Name = "schemeSeparatorLabel";
            this.schemeSeparatorLabel.Size = new System.Drawing.Size(20, 13);
            this.schemeSeparatorLabel.TabStop = false;
            this.schemeSeparatorLabel.Text = "://";
            this.schemeSeparatorLabel.Visible = false;
            // 
            // userNameBuildTextBox
            // 
            this.userNameBuildTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.userNameBuildTextBox.Location = new System.Drawing.Point(142, 26);
            this.userNameBuildTextBox.Name = "userNameBuildTextBox";
            this.userNameBuildTextBox.Size = new System.Drawing.Size(107, 20);
            this.userNameBuildTextBox.TabIndex = 11;
            this.userNameBuildTextBox.Visible = false;
            // 
            // passwordSeparatorLabel
            // 
            this.passwordSeparatorLabel.AutoSize = true;
            this.passwordSeparatorLabel.Location = new System.Drawing.Point(255, 23);
            this.passwordSeparatorLabel.Name = "passwordSeparatorLabel";
            this.passwordSeparatorLabel.Size = new System.Drawing.Size(10, 13);
            this.passwordSeparatorLabel.TabStop = false;
            this.passwordSeparatorLabel.Text = ":";
            this.passwordSeparatorLabel.Visible = false;
            // 
            // passwordBuildTextBox
            // 
            this.passwordBuildTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.passwordBuildTextBox.Location = new System.Drawing.Point(271, 26);
            this.passwordBuildTextBox.Name = "passwordBuildTextBox";
            this.passwordBuildTextBox.Size = new System.Drawing.Size(107, 20);
            this.passwordBuildTextBox.TabIndex = 12;
            this.passwordBuildTextBox.Visible = false;
            // 
            // authSeparatorLabel
            // 
            this.authSeparatorLabel.AutoSize = true;
            this.authSeparatorLabel.Location = new System.Drawing.Point(384, 23);
            this.authSeparatorLabel.Name = "authSeparatorLabel";
            this.authSeparatorLabel.Size = new System.Drawing.Size(18, 13);
            this.authSeparatorLabel.TabStop = false;
            this.authSeparatorLabel.Text = "@";
            this.authSeparatorLabel.Visible = false;
            // 
            // hostBuildTextBox
            // 
            this.hostBuildTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.hostBuildTextBox.Location = new System.Drawing.Point(408, 26);
            this.hostBuildTextBox.Name = "hostBuildTextBox";
            this.hostBuildTextBox.Size = new System.Drawing.Size(220, 20);
            this.hostBuildTextBox.TabIndex = 13;
            this.hostBuildTextBox.Visible = false;
            // 
            // portSeparatorLabel
            // 
            this.portSeparatorLabel.AutoSize = true;
            this.portSeparatorLabel.Location = new System.Drawing.Point(634, 23);
            this.portSeparatorLabel.Name = "portSeparatorLabel";
            this.portSeparatorLabel.Size = new System.Drawing.Size(10, 13);
            this.portSeparatorLabel.TabStop = false;
            this.portSeparatorLabel.Text = ":";
            this.portSeparatorLabel.Visible = false;
            // 
            // portBuildTextBox
            // 
            this.portBuildTextBox.Location = new System.Drawing.Point(650, 26);
            this.portBuildTextBox.Name = "portBuildTextBox";
            this.portBuildTextBox.Size = new System.Drawing.Size(64, 20);
            this.portBuildTextBox.TabIndex = 14;
            this.portBuildTextBox.Visible = false;
            // 
            // portMessageLabel
            // 
            this.portMessageLabel.AutoSize = true;
            this.portMessageLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.portMessageLabel.Location = new System.Drawing.Point(720, 23);
            this.portMessageLabel.Name = "portMessageLabel";
            this.portMessageLabel.Size = new System.Drawing.Size(45, 13);
            this.portMessageLabel.TabStop = false;
            this.portMessageLabel.Text = "(default)";
            this.portMessageLabel.Visible = false;
            // 
            // pathBuildLabel
            // 
            this.pathBuildLabel.AutoSize = true;
            this.pathBuildLabel.Location = new System.Drawing.Point(3, 50);
            this.pathBuildLabel.Name = "pathBuildLabel";
            this.pathBuildLabel.Size = new System.Drawing.Size(29, 13);
            this.pathBuildLabel.TabStop = false;
            this.pathBuildLabel.Text = "Path";
            // 
            // pathBuildTextBox
            // 
            this.buildTableLayoutPanel.SetColumnSpan(this.pathBuildTextBox, 10);
            this.pathBuildTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.pathBuildTextBox.Location = new System.Drawing.Point(3, 66);
            this.pathBuildTextBox.Name = "pathBuildTextBox";
            this.pathBuildTextBox.Size = new System.Drawing.Size(764, 20);
            this.pathBuildTextBox.TabIndex = 15;
            // 
            // queryCheckBox
            // 
            this.queryCheckBox.AutoSize = true;
            this.buildTableLayoutPanel.SetColumnSpan(this.queryCheckBox, 9);
            this.queryCheckBox.Location = new System.Drawing.Point(3, 92);
            this.queryCheckBox.Name = "queryCheckBox";
            this.queryCheckBox.Size = new System.Drawing.Size(54, 17);
            this.queryCheckBox.TabIndex = 16;
            this.queryCheckBox.Text = "Query";
            this.queryCheckBox.UseVisualStyleBackColor = true;
            this.queryCheckBox.CheckedChanged += new System.EventHandler(this.queryCheckBox_CheckedChanged);
            // 
            // querySplitContainer
            // 
            this.buildTableLayoutPanel.SetColumnSpan(this.querySplitContainer, 10);
            this.querySplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.querySplitContainer.Location = new System.Drawing.Point(3, 115);
            this.querySplitContainer.Name = "querySplitContainer";
            this.querySplitContainer.Visible = false;
            this.querySplitContainer.Panel2Collapsed = true;
            // 
            // querySplitContainer.Panel1
            // 
            this.querySplitContainer.Panel1.Controls.Add(this.queryItemsTableLayoutPanel);
            // 
            // querySplitContainer.Panel2
            // 
            this.querySplitContainer.Panel2.Controls.Add(this.editQueryItemTableLayoutPanel);
            this.querySplitContainer.Size = new System.Drawing.Size(764, 330);
            this.querySplitContainer.SplitterDistance = 254;
            this.querySplitContainer.TabIndex = 17;
            // 
            // queryItemsTableLayoutPanel
            // 
            this.queryItemsTableLayoutPanel.AutoSize = true;
            this.queryItemsTableLayoutPanel.ColumnCount = 1;
            this.queryItemsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.queryItemsTableLayoutPanel.Controls.Add(this.queryBuildDataGridView, 0, 0);
            this.queryItemsTableLayoutPanel.Controls.Add(this.addQueryItemButton, 0, 1);
            this.queryItemsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryItemsTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.queryItemsTableLayoutPanel.Name = "queryItemsTableLayoutPanel";
            this.queryItemsTableLayoutPanel.RowCount = 2;
            this.queryItemsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.queryItemsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.queryItemsTableLayoutPanel.Size = new System.Drawing.Size(254, 330);
            this.queryItemsTableLayoutPanel.TabStop = false;
            // 
            // queryBuildDataGridView
            // 
            this.queryBuildDataGridView.AllowUserToAddRows = false;
            this.queryBuildDataGridView.AllowUserToDeleteRows = false;
            this.queryBuildDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.queryBuildDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryBuildDataGridView.Location = new System.Drawing.Point(3, 3);
            this.queryBuildDataGridView.MultiSelect = false;
            this.queryBuildDataGridView.Name = "queryBuildDataGridView";
            this.queryBuildDataGridView.ReadOnly = true;
            this.queryBuildDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.queryBuildDataGridView.Size = new System.Drawing.Size(248, 295);
            this.queryBuildDataGridView.TabIndex = 18;
            this.queryBuildDataGridView.CurrentCellChanged += new System.EventHandler(this.queryBuildDataGridView_CurrentCellChanged);
            // 
            // addQueryItemButton
            // 
            this.addQueryItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addQueryItemButton.Location = new System.Drawing.Point(176, 304);
            this.addQueryItemButton.Name = "addQueryItemButton";
            this.addQueryItemButton.Size = new System.Drawing.Size(75, 23);
            this.addQueryItemButton.TabIndex = 19;
            this.addQueryItemButton.Text = "Add";
            this.addQueryItemButton.UseVisualStyleBackColor = true;
            this.addQueryItemButton.Click += new System.EventHandler(this.addQueryItemButton_Click);
            // 
            // editQueryItemTableLayoutPanel
            // 
            this.editQueryItemTableLayoutPanel.AutoScroll = true;
            this.editQueryItemTableLayoutPanel.ColumnCount = 2;
            this.editQueryItemTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.editQueryItemTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.editQueryItemTableLayoutPanel.Controls.Add(this.queryItemOrderLabel, 0, 0);
            this.editQueryItemTableLayoutPanel.Controls.Add(this.queryItemOrderTextBox, 1, 0);
            this.editQueryItemTableLayoutPanel.Controls.Add(this.queryItemKeyLabel, 0, 1);
            this.editQueryItemTableLayoutPanel.Controls.Add(this.queryItemKeyTextBox, 1, 1);
            this.editQueryItemTableLayoutPanel.Controls.Add(this.queryItemValueCheckBox, 0, 2);
            this.editQueryItemTableLayoutPanel.Controls.Add(this.queryItemValueTextBox, 1, 2);
            this.editQueryItemTableLayoutPanel.Controls.Add(this.editQueryItemFlowLayoutPanel, 0, 3);
            this.editQueryItemTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editQueryItemTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.editQueryItemTableLayoutPanel.Name = "editQueryItemTableLayoutPanel";
            this.editQueryItemTableLayoutPanel.RowCount = 4;
            this.editQueryItemTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.editQueryItemTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.editQueryItemTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.editQueryItemTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.editQueryItemTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.editQueryItemTableLayoutPanel.Size = new System.Drawing.Size(506, 330);
            this.editQueryItemTableLayoutPanel.TabStop = false;
            // 
            // queryItemOrderLabel
            // 
            this.queryItemOrderLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.queryItemOrderLabel.AutoSize = true;
            this.queryItemOrderLabel.Location = new System.Drawing.Point(23, 0);
            this.queryItemOrderLabel.Name = "queryItemOrderLabel";
            this.queryItemOrderLabel.Size = new System.Drawing.Size(36, 13);
            this.queryItemOrderLabel.TabStop = false;
            this.queryItemOrderLabel.Text = "Order:";
            // 
            // queryItemOrderTextBox
            // 
            this.queryItemOrderTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.queryItemOrderTextBox.Location = new System.Drawing.Point(65, 3);
            this.queryItemOrderTextBox.Name = "queryItemOrderTextBox";
            this.queryItemOrderTextBox.Size = new System.Drawing.Size(438, 20);
            this.queryItemOrderTextBox.TabIndex = 20;
            // 
            // queryItemKeyLabel
            // 
            this.queryItemKeyLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.queryItemKeyLabel.AutoSize = true;
            this.queryItemKeyLabel.Location = new System.Drawing.Point(31, 26);
            this.queryItemKeyLabel.Name = "queryItemKeyLabel";
            this.queryItemKeyLabel.Size = new System.Drawing.Size(28, 13);
            this.queryItemKeyLabel.TabStop = false;
            this.queryItemKeyLabel.Text = "Key:";
            // 
            // queryItemKeyTextBox
            // 
            this.queryItemKeyTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.queryItemKeyTextBox.Location = new System.Drawing.Point(65, 29);
            this.queryItemKeyTextBox.Name = "queryItemKeyTextBox";
            this.queryItemKeyTextBox.Size = new System.Drawing.Size(438, 20);
            this.queryItemKeyTextBox.TabIndex = 21;
            // 
            // queryItemValueCheckBox
            // 
            this.queryItemValueCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.queryItemValueCheckBox.AutoSize = true;
            this.queryItemValueCheckBox.Location = new System.Drawing.Point(3, 55);
            this.queryItemValueCheckBox.Name = "queryItemValueCheckBox";
            this.queryItemValueCheckBox.Size = new System.Drawing.Size(56, 17);
            this.queryItemValueCheckBox.TabIndex = 22;
            this.queryItemValueCheckBox.Text = "Value:";
            this.queryItemValueCheckBox.UseVisualStyleBackColor = true;
            this.queryItemValueCheckBox.CheckedChanged += new System.EventHandler(this.queryItemValueCheckBox_CheckedChanged);
            // 
            // queryItemValueTextBox
            // 
            this.queryItemValueTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.queryItemValueTextBox.Location = new System.Drawing.Point(65, 55);
            this.queryItemValueTextBox.Name = "queryItemValueTextBox";
            this.queryItemValueTextBox.Size = new System.Drawing.Size(438, 20);
            this.queryItemValueTextBox.TabIndex = 23;
            this.queryItemValueTextBox.Visible = false;
            // 
            // editQueryItemFlowLayoutPanel
            // 
            this.editQueryItemFlowLayoutPanel.AutoSize = true;
            this.editQueryItemTableLayoutPanel.SetColumnSpan(this.editQueryItemFlowLayoutPanel, 2);
            this.editQueryItemFlowLayoutPanel.Controls.Add(this.saveQueryItemButton);
            this.editQueryItemFlowLayoutPanel.Controls.Add(this.moveUpQueryItemButton);
            this.editQueryItemFlowLayoutPanel.Controls.Add(this.moveDownQueryItemButton);
            this.editQueryItemFlowLayoutPanel.Controls.Add(this.addQueryItemButton);
            this.editQueryItemFlowLayoutPanel.Controls.Add(this.insertQueryItemButton);
            this.editQueryItemFlowLayoutPanel.Controls.Add(this.cancelQueryItemButton);
            this.editQueryItemFlowLayoutPanel.Controls.Add(this.deleteQueryItemButton);
            this.editQueryItemFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.editQueryItemFlowLayoutPanel.Location = new System.Drawing.Point(3, 81);
            this.editQueryItemFlowLayoutPanel.Name = "editQueryItemFlowLayoutPanel";
            this.editQueryItemFlowLayoutPanel.Size = new System.Drawing.Size(500, 58);
            this.editQueryItemFlowLayoutPanel.TabStop = false;
            this.queryItemValueTextBox.TabIndex = 24;
            // 
            // saveQueryItemButton
            // 
            this.saveQueryItemButton.Location = new System.Drawing.Point(3, 3);
            this.saveQueryItemButton.Name = "saveQueryItemButton";
            this.saveQueryItemButton.Size = new System.Drawing.Size(75, 23);
            this.saveQueryItemButton.TabIndex = 25;
            this.saveQueryItemButton.Text = "Save";
            this.saveQueryItemButton.UseVisualStyleBackColor = true;
            this.saveQueryItemButton.Visible = false;
            this.saveQueryItemButton.Click += new System.EventHandler(this.saveQueryItemButton_Click);
            // 
            // moveUpQueryItemButton
            // 
            this.moveUpQueryItemButton.Location = new System.Drawing.Point(84, 3);
            this.moveUpQueryItemButton.Name = "moveUpQueryItemButton";
            this.moveUpQueryItemButton.Size = new System.Drawing.Size(75, 23);
            this.moveUpQueryItemButton.TabIndex = 26;
            this.moveUpQueryItemButton.Text = "Move Up";
            this.moveUpQueryItemButton.UseVisualStyleBackColor = true;
            this.moveUpQueryItemButton.Visible = false;
            this.moveUpQueryItemButton.Click += new System.EventHandler(this.moveUpQueryItemButton_Click);
            // 
            // moveDownQueryItemButton
            // 
            this.moveDownQueryItemButton.Location = new System.Drawing.Point(165, 3);
            this.moveDownQueryItemButton.Name = "moveDownQueryItemButton";
            this.moveDownQueryItemButton.Size = new System.Drawing.Size(75, 23);
            this.moveDownQueryItemButton.TabIndex = 27;
            this.moveDownQueryItemButton.Text = "Move Down";
            this.moveDownQueryItemButton.UseVisualStyleBackColor = true;
            this.moveDownQueryItemButton.Visible = false;
            this.moveDownQueryItemButton.Click += new System.EventHandler(this.moveDownQueryItemButton_Click);
            // 
            // addQueryItemButton
            // 
            this.addQueryItemButton.Location = new System.Drawing.Point(246, 3);
            this.addQueryItemButton.Name = "addQueryItemButton";
            this.addQueryItemButton.Size = new System.Drawing.Size(75, 23);
            this.addQueryItemButton.TabIndex = 28;
            this.addQueryItemButton.Text = "Add";
            this.addQueryItemButton.UseVisualStyleBackColor = true;
            this.addQueryItemButton.Click += new System.EventHandler(this.addQueryItemButton_Click);
            // 
            // insertQueryItemButton
            // 
            this.insertQueryItemButton.Location = new System.Drawing.Point(327, 3);
            this.insertQueryItemButton.Name = "insertQueryItemButton";
            this.insertQueryItemButton.Size = new System.Drawing.Size(75, 23);
            this.insertQueryItemButton.TabIndex = 29;
            this.insertQueryItemButton.Text = "Insert";
            this.insertQueryItemButton.UseVisualStyleBackColor = true;
            this.insertQueryItemButton.Click += new System.EventHandler(this.insertQueryItemButton_Click);
            // 
            // cancelQueryItemButton
            // 
            this.cancelQueryItemButton.Location = new System.Drawing.Point(408, 3);
            this.cancelQueryItemButton.Name = "cancelQueryItemButton";
            this.cancelQueryItemButton.Size = new System.Drawing.Size(75, 23);
            this.cancelQueryItemButton.TabIndex = 30;
            this.cancelQueryItemButton.Text = "Cancel";
            this.cancelQueryItemButton.UseVisualStyleBackColor = true;
            this.cancelQueryItemButton.Click += new System.EventHandler(this.cancelQueryItemButton_Click);
            // 
            // deleteQueryItemButton
            // 
            this.deleteQueryItemButton.Location = new System.Drawing.Point(3, 32);
            this.deleteQueryItemButton.Name = "deleteQueryItemButton";
            this.deleteQueryItemButton.Size = new System.Drawing.Size(75, 23);
            this.deleteQueryItemButton.TabIndex = 31;
            this.deleteQueryItemButton.Text = "Delete";
            this.deleteQueryItemButton.UseVisualStyleBackColor = true;
            this.deleteQueryItemButton.Visible = false;
            this.deleteQueryItemButton.Click += new System.EventHandler(this.deleteQueryItemButton_Click);
            // 
            // fragmentBuildCheckBox
            // 
            this.fragmentBuildCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fragmentBuildCheckBox.AutoSize = true;
            this.fragmentBuildCheckBox.Location = new System.Drawing.Point(3, 448);
            this.fragmentBuildCheckBox.Name = "fragmentBuildCheckBox";
            this.fragmentBuildCheckBox.Size = new System.Drawing.Size(56, 17);
            this.fragmentBuildCheckBox.TabIndex = 32;
            this.fragmentBuildCheckBox.Text = "Fragment";
            this.fragmentBuildCheckBox.UseVisualStyleBackColor = true;
            this.fragmentBuildCheckBox.CheckedChanged += new System.EventHandler(this.fragmentBuildCheckBox_CheckedChanged);
            // 
            // fragmentBuildTextBox
            // 
            this.buildTableLayoutPanel.SetColumnSpan(this.fragmentBuildTextBox, 10);
            this.fragmentBuildTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.fragmentBuildTextBox.Location = new System.Drawing.Point(3, 464);
            this.fragmentBuildTextBox.Name = "fragmentBuildTextBox";
            this.fragmentBuildTextBox.Size = new System.Drawing.Size(764, 20);
            this.fragmentBuildTextBox.TabIndex = 33;
            this.fragmentBuildTextBox.Visible = false;
            // 
            // buildUrlButton
            // 
            this.buildUrlButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buildTableLayoutPanel.SetColumnSpan(this.buildUrlButton, 10);
            this.buildUrlButton.Location = new System.Drawing.Point(692, 490);
            this.buildUrlButton.Name = "buildUrlButton";
            this.buildUrlButton.Size = new System.Drawing.Size(75, 23);
            this.buildUrlButton.TabIndex = 34;
            this.buildUrlButton.Text = "Build";
            this.buildUrlButton.UseVisualStyleBackColor = true;
            this.buildUrlButton.Click += new System.EventHandler(this.buildUrlButton_Click);
            // 
            // buildErrorLabel
            // 
            this.buildErrorLabel.AutoSize = true;
            this.buildTableLayoutPanel.SetColumnSpan(this.buildErrorLabel, 10);
            this.buildErrorLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.buildErrorLabel.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.buildErrorLabel.Location = new System.Drawing.Point(3, 516);
            this.buildErrorLabel.Name = "buildErrorLabel";
            this.buildErrorLabel.Size = new System.Drawing.Size(764, 13);
            this.buildErrorLabel.TabStop = false;
            this.buildErrorLabel.Text = "Nothing to build.";
            // 
            // componentsTabPage
            // 
            this.componentsTabPage.Controls.Add(this.componentsTableLayoutPanel);
            this.componentsTabPage.Location = new System.Drawing.Point(4, 22);
            this.componentsTabPage.Name = "componentsTabPage";
            this.componentsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.componentsTabPage.Size = new System.Drawing.Size(776, 535);
            this.componentsTabPage.TabIndex = 35;
            this.componentsTabPage.Text = "Components";
            this.componentsTabPage.UseVisualStyleBackColor = true;
            // 
            // componentsTableLayoutPanel
            // 
            this.componentsTableLayoutPanel.AutoSize = true;
            this.componentsTableLayoutPanel.ColumnCount = 7;
            this.componentsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.componentsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.componentsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.componentsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.componentsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.componentsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.componentsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.componentsTableLayoutPanel.Controls.Add(this.schemeComponentLabel, 0, 0);
            this.componentsTableLayoutPanel.Controls.Add(this.schemeComponentTextBox, 1, 0);
            this.componentsTableLayoutPanel.Controls.Add(this.userNameComponentLabel, 2, 0);
            this.componentsTableLayoutPanel.Controls.Add(this.userNameComponentTextBox, 3, 0);
            this.componentsTableLayoutPanel.Controls.Add(this.passwordComponentLabel, 4, 0);
            this.componentsTableLayoutPanel.Controls.Add(this.passwordComponentTextBox, 5, 0);
            this.componentsTableLayoutPanel.Controls.Add(this.hostComponentLabel, 0, 1);
            this.componentsTableLayoutPanel.Controls.Add(this.hostComponentTextBox, 1, 1);
            this.componentsTableLayoutPanel.Controls.Add(this.portComponentLabel, 4, 1);
            this.componentsTableLayoutPanel.Controls.Add(this.portComponentTextBox, 5, 1);
            this.componentsTableLayoutPanel.Controls.Add(this.portComponentDescLabel, 6, 1);
            this.componentsTableLayoutPanel.Controls.Add(this.segmentsComponentLabel, 0, 2);
            this.componentsTableLayoutPanel.Controls.Add(this.segmentsComponentDataGridView, 0, 3);
            this.componentsTableLayoutPanel.Controls.Add(this.queryComponentLabel, 0, 4);
            this.componentsTableLayoutPanel.Controls.Add(this.queryComponentDataGridView, 0, 5);
            this.componentsTableLayoutPanel.Controls.Add(this.queryNotIncludedLabel, 0, 6);
            this.componentsTableLayoutPanel.Controls.Add(this.fragmentComponentLabel, 0, 7);
            this.componentsTableLayoutPanel.Controls.Add(this.fragmentComponentTextBox, 0, 8);
            this.componentsTableLayoutPanel.Controls.Add(this.parentDirButton, 0, 9);
            this.componentsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.componentsTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.componentsTableLayoutPanel.Name = "componentsTableLayoutPanel";
            this.componentsTableLayoutPanel.RowCount = 10;
            this.componentsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.componentsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.componentsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.componentsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.componentsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.componentsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.componentsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.componentsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.componentsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.componentsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.componentsTableLayoutPanel.Size = new System.Drawing.Size(770, 529);
            this.componentsTableLayoutPanel.TabStop = false;
            // 
            // schemeComponentLabel
            // 
            this.schemeComponentLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.schemeComponentLabel.AutoSize = true;
            this.schemeComponentLabel.Location = new System.Drawing.Point(5, 0);
            this.schemeComponentLabel.Name = "schemeComponentLabel";
            this.schemeComponentLabel.Size = new System.Drawing.Size(49, 13);
            this.schemeComponentLabel.TabStop = false;
            this.schemeComponentLabel.Text = "Scheme:";
            // 
            // schemeComponentTextBox
            // 
            this.schemeComponentTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.schemeComponentTextBox.Location = new System.Drawing.Point(60, 3);
            this.schemeComponentTextBox.Name = "schemeComponentTextBox";
            this.schemeComponentTextBox.ReadOnly = true;
            this.schemeComponentTextBox.Size = new System.Drawing.Size(206, 20);
            this.schemeComponentTextBox.TabIndex = 36;
            // 
            // userNameComponentLabel
            // 
            this.userNameComponentLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.userNameComponentLabel.AutoSize = true;
            this.userNameComponentLabel.Location = new System.Drawing.Point(272, 0);
            this.userNameComponentLabel.Name = "userNameComponentLabel";
            this.userNameComponentLabel.Size = new System.Drawing.Size(63, 13);
            this.userNameComponentLabel.TabStop = false;
            this.userNameComponentLabel.Text = "User Name:";
            // 
            // userNameComponentTextBox
            // 
            this.userNameComponentTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.userNameComponentTextBox.Location = new System.Drawing.Point(341, 3);
            this.userNameComponentTextBox.Name = "userNameComponentTextBox";
            this.userNameComponentTextBox.ReadOnly = true;
            this.userNameComponentTextBox.Size = new System.Drawing.Size(206, 20);
            this.userNameComponentTextBox.TabIndex = 37;
            // 
            // passwordComponentLabel
            // 
            this.passwordComponentLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.passwordComponentLabel.AutoSize = true;
            this.passwordComponentLabel.Location = new System.Drawing.Point(553, 0);
            this.passwordComponentLabel.Name = "passwordComponentLabel";
            this.passwordComponentLabel.Size = new System.Drawing.Size(56, 13);
            this.passwordComponentLabel.TabStop = false;
            this.passwordComponentLabel.Text = "Password:";
            // 
            // passwordComponentTextBox
            // 
            this.componentsTableLayoutPanel.SetColumnSpan(this.passwordComponentTextBox, 2);
            this.passwordComponentTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.passwordComponentTextBox.Location = new System.Drawing.Point(615, 3);
            this.passwordComponentTextBox.Name = "passwordComponentTextBox";
            this.passwordComponentTextBox.ReadOnly = true;
            this.passwordComponentTextBox.Size = new System.Drawing.Size(152, 20);
            this.passwordComponentTextBox.TabIndex = 38;
            // 
            // hostComponentLabel
            // 
            this.hostComponentLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.hostComponentLabel.AutoSize = true;
            this.hostComponentLabel.Location = new System.Drawing.Point(22, 26);
            this.hostComponentLabel.Name = "hostComponentLabel";
            this.hostComponentLabel.Size = new System.Drawing.Size(32, 13);
            this.hostComponentLabel.TabStop = false;
            this.hostComponentLabel.Text = "Host:";
            // 
            // hostComponentTextBox
            // 
            this.componentsTableLayoutPanel.SetColumnSpan(this.hostComponentTextBox, 3);
            this.hostComponentTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hostComponentTextBox.Location = new System.Drawing.Point(60, 29);
            this.hostComponentTextBox.Name = "hostComponentTextBox";
            this.hostComponentTextBox.ReadOnly = true;
            this.hostComponentTextBox.Size = new System.Drawing.Size(487, 20);
            this.hostComponentTextBox.TabIndex = 39;
            // 
            // portComponentLabel
            // 
            this.portComponentLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.portComponentLabel.AutoSize = true;
            this.portComponentLabel.Location = new System.Drawing.Point(580, 26);
            this.portComponentLabel.Name = "portComponentLabel";
            this.portComponentLabel.Size = new System.Drawing.Size(29, 13);
            this.portComponentLabel.TabStop = false;
            this.portComponentLabel.Text = "Port:";
            // 
            // portComponentTextBox
            // 
            this.portComponentTextBox.Location = new System.Drawing.Point(615, 29);
            this.portComponentTextBox.Name = "portComponentTextBox";
            this.portComponentTextBox.ReadOnly = true;
            this.portComponentTextBox.Size = new System.Drawing.Size(100, 20);
            this.portComponentTextBox.TabIndex = 40;
            // 
            // portComponentDescLabel
            // 
            this.portComponentDescLabel.AutoSize = true;
            this.portComponentDescLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.portComponentDescLabel.Location = new System.Drawing.Point(721, 26);
            this.portComponentDescLabel.Name = "portComponentDescLabel";
            this.portComponentDescLabel.Size = new System.Drawing.Size(45, 13);
            this.portComponentDescLabel.TabStop = false;
            this.portComponentDescLabel.Text = "(default)";
            // 
            // segmentsComponentLabel
            // 
            this.segmentsComponentLabel.AutoSize = true;
            this.componentsTableLayoutPanel.SetColumnSpan(this.segmentsComponentLabel, 7);
            this.segmentsComponentLabel.Location = new System.Drawing.Point(3, 52);
            this.segmentsComponentLabel.Name = "segmentsComponentLabel";
            this.segmentsComponentLabel.Size = new System.Drawing.Size(54, 13);
            this.segmentsComponentLabel.TabStop = false;
            this.segmentsComponentLabel.Text = "Segments";
            // 
            // segmentsComponentDataGridView
            // 
            this.segmentsComponentDataGridView.AllowUserToAddRows = false;
            this.segmentsComponentDataGridView.AllowUserToDeleteRows = false;
            this.segmentsComponentDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.componentsTableLayoutPanel.SetColumnSpan(this.segmentsComponentDataGridView, 7);
            this.segmentsComponentDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.segmentsComponentDataGridView.Location = new System.Drawing.Point(3, 68);
            this.segmentsComponentDataGridView.Name = "segmentsComponentDataGridView";
            this.segmentsComponentDataGridView.ReadOnly = true;
            this.segmentsComponentDataGridView.Size = new System.Drawing.Size(764, 179);
            this.segmentsComponentDataGridView.TabIndex = 41;
            // 
            // queryNotIncludedLabel
            // 
            this.queryNotIncludedLabel.AutoSize = true;
            this.componentsTableLayoutPanel.SetColumnSpan(this.queryNotIncludedLabel, 7);
            this.queryNotIncludedLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.queryNotIncludedLabel.Location = new System.Drawing.Point(3, 448);
            this.queryNotIncludedLabel.Name = "queryNotIncludedLabel";
            this.queryNotIncludedLabel.Size = new System.Drawing.Size(70, 13);
            this.queryNotIncludedLabel.TabStop = false;
            this.queryNotIncludedLabel.Text = "Not included.";
            // 
            // queryComponentLabel
            // 
            this.queryComponentLabel.AutoSize = true;
            this.queryComponentLabel.Location = new System.Drawing.Point(3, 250);
            this.queryComponentLabel.Name = "queryComponentLabel";
            this.queryComponentLabel.Size = new System.Drawing.Size(35, 13);
            this.queryComponentLabel.TabStop = false;
            this.queryComponentLabel.Text = "Query";
            // 
            // queryComponentDataGridView
            // 
            this.queryComponentDataGridView.AllowUserToAddRows = false;
            this.queryComponentDataGridView.AllowUserToDeleteRows = false;
            this.queryComponentDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.componentsTableLayoutPanel.SetColumnSpan(this.queryComponentDataGridView, 7);
            this.queryComponentDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryComponentDataGridView.Location = new System.Drawing.Point(3, 266);
            this.queryComponentDataGridView.Name = "queryComponentDataGridView";
            this.queryComponentDataGridView.ReadOnly = true;
            this.queryComponentDataGridView.Size = new System.Drawing.Size(764, 179);
            this.queryComponentDataGridView.TabIndex = 42;
            // 
            // fragmentComponentLabel
            // 
            this.fragmentComponentLabel.AutoSize = true;
            this.fragmentComponentLabel.Location = new System.Drawing.Point(3, 461);
            this.fragmentComponentLabel.Name = "fragmentComponentLabel";
            this.fragmentComponentLabel.Size = new System.Drawing.Size(51, 13);
            this.fragmentComponentLabel.TabStop = false;
            this.fragmentComponentLabel.Text = "Fragment";
            // 
            // fragmentComponentTextBox
            // 
            this.componentsTableLayoutPanel.SetColumnSpan(this.fragmentComponentTextBox, 7);
            this.fragmentComponentTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.fragmentComponentTextBox.Location = new System.Drawing.Point(3, 477);
            this.fragmentComponentTextBox.Name = "fragmentComponentTextBox";
            this.fragmentComponentTextBox.ReadOnly = true;
            this.fragmentComponentTextBox.Size = new System.Drawing.Size(764, 20);
            this.fragmentComponentTextBox.TabIndex = 43;
            // 
            // parentDirButton
            // 
            this.parentDirButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.componentsTableLayoutPanel.SetColumnSpan(this.parentDirButton, 7);
            this.parentDirButton.Location = new System.Drawing.Point(692, 503);
            this.parentDirButton.Name = "parentDirButton";
            this.parentDirButton.Size = new System.Drawing.Size(75, 23);
            this.parentDirButton.TabIndex = 44;
            this.parentDirButton.Text = "Parent Dir";
            this.parentDirButton.UseVisualStyleBackColor = true;
            this.parentDirButton.Enabled = false;
            this.parentDirButton.Click += new System.EventHandler(this.parentDirButton_Click);
            // 
            // EditUrlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.mainTabControl);
            this.Name = "EditUrlForm";
            this.Text = "Edit Url";
            this.mainTabControl.ResumeLayout(false);
            this.parseTabPage.ResumeLayout(false);
            this.parseTabPage.PerformLayout();
            this.parseTableLayoutPanel.ResumeLayout(false);
            this.parseTableLayoutPanel.PerformLayout();
            this.buildTabPage.ResumeLayout(false);
            this.buildTabPage.PerformLayout();
            this.buildTableLayoutPanel.ResumeLayout(false);
            this.buildTableLayoutPanel.PerformLayout();
            this.querySplitContainer.Panel1.ResumeLayout(false);
            this.querySplitContainer.Panel1.PerformLayout();
            this.querySplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.querySplitContainer)).EndInit();
            this.querySplitContainer.ResumeLayout(false);
            this.queryItemsTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.queryBuildDataGridView)).EndInit();
            this.editQueryItemTableLayoutPanel.ResumeLayout(false);
            this.editQueryItemTableLayoutPanel.PerformLayout();
            this.editQueryItemFlowLayoutPanel.ResumeLayout(false);
            this.componentsTabPage.ResumeLayout(false);
            this.componentsTabPage.PerformLayout();
            this.componentsTableLayoutPanel.ResumeLayout(false);
            this.componentsTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.segmentsComponentDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.queryComponentDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage parseTabPage;
        private System.Windows.Forms.TableLayoutPanel parseTableLayoutPanel;
        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.Label urlLabel;
        private System.Windows.Forms.Label urlErrorLabel;
        private System.Windows.Forms.Button buildButton;
        private System.Windows.Forms.Button parseButton;
        private System.Windows.Forms.TabPage buildTabPage;
        private System.Windows.Forms.TableLayoutPanel buildTableLayoutPanel;
        private System.Windows.Forms.Label schemeBuildLabel;
        private System.Windows.Forms.CheckBox userNameCheckBox;
        private System.Windows.Forms.CheckBox passwordCheckBox;
        private System.Windows.Forms.CheckBox hostCheckBox;
        private System.Windows.Forms.CheckBox portCheckBox;
        private System.Windows.Forms.TextBox schemeBuildTextBox;
        private System.Windows.Forms.Label schemeSeparatorLabel;
        private System.Windows.Forms.TextBox userNameBuildTextBox;
        private System.Windows.Forms.Label passwordSeparatorLabel;
        private System.Windows.Forms.TextBox passwordBuildTextBox;
        private System.Windows.Forms.Label authSeparatorLabel;
        private System.Windows.Forms.TextBox hostBuildTextBox;
        private System.Windows.Forms.Label portSeparatorLabel;
        private System.Windows.Forms.TextBox portBuildTextBox;
        private System.Windows.Forms.Label portMessageLabel;
        private System.Windows.Forms.Label pathBuildLabel;
        private System.Windows.Forms.TextBox pathBuildTextBox;
        private System.Windows.Forms.CheckBox queryCheckBox;
        private System.Windows.Forms.SplitContainer querySplitContainer;
        private System.Windows.Forms.TableLayoutPanel queryItemsTableLayoutPanel;
        private System.Windows.Forms.DataGridView queryBuildDataGridView;
        private System.Windows.Forms.Button addQueryItemButton;
        private System.Windows.Forms.TableLayoutPanel editQueryItemTableLayoutPanel;
        private System.Windows.Forms.Label queryItemOrderLabel;
        private System.Windows.Forms.TextBox queryItemOrderTextBox;
        private System.Windows.Forms.Label queryItemKeyLabel;
        private System.Windows.Forms.TextBox queryItemKeyTextBox;
        private System.Windows.Forms.CheckBox queryItemValueCheckBox;
        private System.Windows.Forms.TextBox queryItemValueTextBox;
        private System.Windows.Forms.FlowLayoutPanel editQueryItemFlowLayoutPanel;
        private System.Windows.Forms.Button saveQueryItemButton;
        private System.Windows.Forms.Button moveUpQueryItemButton;
        private System.Windows.Forms.Button moveDownQueryItemButton;
        private System.Windows.Forms.Button addQueryItemButton;
        private System.Windows.Forms.Button insertQueryItemButton;
        private System.Windows.Forms.Button cancelQueryItemButton;
        private System.Windows.Forms.Button deleteQueryItemButton;
        private System.Windows.Forms.CheckBox fragmentBuildCheckBox;
        private System.Windows.Forms.TextBox fragmentBuildTextBox;
        private System.Windows.Forms.Button buildUrlButton;
        private System.Windows.Forms.Label buildErrorLabel;
        private System.Windows.Forms.TabPage componentsTabPage;
        private System.Windows.Forms.TableLayoutPanel componentsTableLayoutPanel;
        private System.Windows.Forms.Label schemeComponentLabel;
        private System.Windows.Forms.TextBox schemeComponentTextBox;
        private System.Windows.Forms.Label userNameComponentLabel;
        private System.Windows.Forms.TextBox userNameComponentTextBox;
        private System.Windows.Forms.Label passwordComponentLabel;
        private System.Windows.Forms.TextBox passwordComponentTextBox;
        private System.Windows.Forms.Label hostComponentLabel;
        private System.Windows.Forms.TextBox hostComponentTextBox;
        private System.Windows.Forms.Label portComponentLabel;
        private System.Windows.Forms.TextBox portComponentTextBox;
        private System.Windows.Forms.Label portComponentDescLabel;
        private System.Windows.Forms.Label segmentsComponentLabel;
        private System.Windows.Forms.DataGridView segmentsComponentDataGridView;
        private System.Windows.Forms.DataGridView queryComponentDataGridView;
        private System.Windows.Forms.Label queryComponentLabel;
        private System.Windows.Forms.Label queryNotIncludedLabel;
        private System.Windows.Forms.Label fragmentComponentLabel;
        private System.Windows.Forms.TextBox fragmentComponentTextBox;
        private System.Windows.Forms.Button parentDirButton;
    }
}