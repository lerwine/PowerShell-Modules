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
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.buildTabPage = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.userNameCheckBox = new System.Windows.Forms.CheckBox();
            this.schemeComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.userNameBuildTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.passwordCheckBox = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.hostBuildLabel = new System.Windows.Forms.Label();
            this.hostBuildTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.portBuildTextBox = new System.Windows.Forms.TextBox();
            this.pathBuildLabel = new System.Windows.Forms.Label();
            this.pathBuildTextBox = new System.Windows.Forms.TextBox();
            this.passwordBuildTextBox = new System.Windows.Forms.TextBox();
            this.portCheckBox = new System.Windows.Forms.CheckBox();
            this.queryCheckBox = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.schemeCheckBox = new System.Windows.Forms.CheckBox();
            this.portMessageLabel = new System.Windows.Forms.Label();
            this.querySplitContainer = new System.Windows.Forms.SplitContainer();
            this.queryItemsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.queryDataGridView = new System.Windows.Forms.DataGridView();
            this.addButton = new System.Windows.Forms.Button();
            this.queryItemTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.queryOrderLabel = new System.Windows.Forms.Label();
            this.queryKeyLabel = new System.Windows.Forms.Label();
            this.queryOrderTextBox = new System.Windows.Forms.TextBox();
            this.queryKeyTextBox = new System.Windows.Forms.TextBox();
            this.queryValueTextBox = new System.Windows.Forms.TextBox();
            this.queryFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.saveQueryButton = new System.Windows.Forms.Button();
            this.moveUpQueryButton = new System.Windows.Forms.Button();
            this.moveDownQueryButton = new System.Windows.Forms.Button();
            this.addQueryButton = new System.Windows.Forms.Button();
            this.insertQueryButton = new System.Windows.Forms.Button();
            this.cancelQueryButton = new System.Windows.Forms.Button();
            this.deleteQueryButton = new System.Windows.Forms.Button();
            this.queryValueCheckBox = new System.Windows.Forms.CheckBox();
            this.buildUrlButton = new System.Windows.Forms.Button();
            this.buildErrorLabel = new System.Windows.Forms.Label();
            this.componentsTabPage = new System.Windows.Forms.TabPage();
            this.componentsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.schemeComponentTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.userNameComponentTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.passwordComponentTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.hostComponentTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.portComponentTextBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.segmentsComponentDataGridView = new System.Windows.Forms.DataGridView();
            this.queryNotIncludedLabel = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.queryComponentDataGridView = new System.Windows.Forms.DataGridView();
            this.label15 = new System.Windows.Forms.Label();
            this.fragmentComponentTextBox = new System.Windows.Forms.TextBox();
            this.parentDirButton = new System.Windows.Forms.Button();
            this.mainTabControl.SuspendLayout();
            this.parseTabPage.SuspendLayout();
            this.parseTableLayoutPanel.SuspendLayout();
            this.buildTabPage.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.querySplitContainer)).BeginInit();
            this.querySplitContainer.Panel1.SuspendLayout();
            this.querySplitContainer.Panel2.SuspendLayout();
            this.querySplitContainer.SuspendLayout();
            this.queryItemsTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.queryDataGridView)).BeginInit();
            this.queryItemTableLayoutPanel.SuspendLayout();
            this.queryFlowLayoutPanel.SuspendLayout();
            this.componentsTabPage.SuspendLayout();
            this.componentsTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.segmentsComponentDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.queryComponentDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.parseTabPage);
            this.mainTabControl.Controls.Add(this.buildTabPage);
            this.mainTabControl.Controls.Add(this.componentsTabPage);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 0);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(784, 561);
            this.mainTabControl.TabIndex = 1;
            this.mainTabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.mainTabControl_Selecting);
            // 
            // parseTabPage
            // 
            this.parseTabPage.Controls.Add(this.parseTableLayoutPanel);
            this.parseTabPage.Controls.Add(this.dateTimePicker1);
            this.parseTabPage.Location = new System.Drawing.Point(4, 22);
            this.parseTabPage.Name = "parseTabPage";
            this.parseTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.parseTabPage.Size = new System.Drawing.Size(776, 535);
            this.parseTabPage.TabIndex = 0;
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
            this.parseTableLayoutPanel.TabIndex = 1;
            // 
            // urlTextBox
            // 
            this.parseTableLayoutPanel.SetColumnSpan(this.urlTextBox, 2);
            this.urlTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.urlTextBox.Location = new System.Drawing.Point(3, 16);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(764, 20);
            this.urlTextBox.TabIndex = 0;
            // 
            // urlLabel
            // 
            this.urlLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.urlLabel.AutoSize = true;
            this.parseTableLayoutPanel.SetColumnSpan(this.urlLabel, 2);
            this.urlLabel.Location = new System.Drawing.Point(3, 0);
            this.urlLabel.Name = "urlLabel";
            this.urlLabel.Size = new System.Drawing.Size(29, 13);
            this.urlLabel.TabIndex = 1;
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
            this.urlErrorLabel.TabIndex = 2;
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
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(0, 0);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 0;
            // 
            // buildTabPage
            // 
            this.buildTabPage.Controls.Add(this.tableLayoutPanel2);
            this.buildTabPage.Location = new System.Drawing.Point(4, 22);
            this.buildTabPage.Name = "buildTabPage";
            this.buildTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.buildTabPage.Size = new System.Drawing.Size(776, 535);
            this.buildTabPage.TabIndex = 1;
            this.buildTabPage.Text = "Build";
            this.buildTabPage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 10;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.63636F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.63636F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.63636F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.27273F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.userNameCheckBox, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.schemeComboBox, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label2, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.userNameBuildTextBox, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.label3, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.passwordCheckBox, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.label4, 5, 1);
            this.tableLayoutPanel2.Controls.Add(this.hostBuildLabel, 6, 0);
            this.tableLayoutPanel2.Controls.Add(this.hostBuildTextBox, 6, 1);
            this.tableLayoutPanel2.Controls.Add(this.label6, 7, 1);
            this.tableLayoutPanel2.Controls.Add(this.portBuildTextBox, 8, 1);
            this.tableLayoutPanel2.Controls.Add(this.pathBuildLabel, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.pathBuildTextBox, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.passwordBuildTextBox, 4, 1);
            this.tableLayoutPanel2.Controls.Add(this.portCheckBox, 8, 0);
            this.tableLayoutPanel2.Controls.Add(this.queryCheckBox, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.textBox7, 0, 7);
            this.tableLayoutPanel2.Controls.Add(this.schemeCheckBox, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.portMessageLabel, 9, 1);
            this.tableLayoutPanel2.Controls.Add(this.querySplitContainer, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.buildUrlButton, 0, 8);
            this.tableLayoutPanel2.Controls.Add(this.buildErrorLabel, 0, 9);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 10;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(770, 529);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // userNameCheckBox
            // 
            this.userNameCheckBox.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.userNameCheckBox, 2);
            this.userNameCheckBox.Location = new System.Drawing.Point(142, 3);
            this.userNameCheckBox.Name = "userNameCheckBox";
            this.userNameCheckBox.Size = new System.Drawing.Size(76, 17);
            this.userNameCheckBox.TabIndex = 1;
            this.userNameCheckBox.Text = "UserName";
            this.userNameCheckBox.UseVisualStyleBackColor = true;
            this.userNameCheckBox.CheckedChanged += new System.EventHandler(this.userNameCheckBox_CheckedChanged);
            // 
            // schemeComboBox
            // 
            this.schemeComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.schemeComboBox.FormattingEnabled = true;
            this.schemeComboBox.Location = new System.Drawing.Point(3, 26);
            this.schemeComboBox.Name = "schemeComboBox";
            this.schemeComboBox.Size = new System.Drawing.Size(107, 21);
            this.schemeComboBox.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(116, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "://";
            // 
            // userNameBuildTextBox
            // 
            this.userNameBuildTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.userNameBuildTextBox.Location = new System.Drawing.Point(142, 26);
            this.userNameBuildTextBox.Name = "userNameBuildTextBox";
            this.userNameBuildTextBox.Size = new System.Drawing.Size(107, 20);
            this.userNameBuildTextBox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(255, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = ":";
            // 
            // passwordCheckBox
            // 
            this.passwordCheckBox.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.passwordCheckBox, 2);
            this.passwordCheckBox.Location = new System.Drawing.Point(271, 3);
            this.passwordCheckBox.Name = "passwordCheckBox";
            this.passwordCheckBox.Size = new System.Drawing.Size(72, 17);
            this.passwordCheckBox.TabIndex = 6;
            this.passwordCheckBox.Text = "Password";
            this.passwordCheckBox.UseVisualStyleBackColor = true;
            this.passwordCheckBox.CheckedChanged += new System.EventHandler(this.passwordCheckBox_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(384, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "@";
            // 
            // hostBuildLabel
            // 
            this.hostBuildLabel.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.hostBuildLabel, 2);
            this.hostBuildLabel.Location = new System.Drawing.Point(408, 0);
            this.hostBuildLabel.Name = "hostBuildLabel";
            this.hostBuildLabel.Size = new System.Drawing.Size(29, 13);
            this.hostBuildLabel.TabIndex = 9;
            this.hostBuildLabel.Text = "Host";
            // 
            // hostBuildTextBox
            // 
            this.hostBuildTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.hostBuildTextBox.Location = new System.Drawing.Point(408, 26);
            this.hostBuildTextBox.Name = "hostBuildTextBox";
            this.hostBuildTextBox.Size = new System.Drawing.Size(220, 20);
            this.hostBuildTextBox.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(634, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(10, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = ":";
            // 
            // portBuildTextBox
            // 
            this.portBuildTextBox.Location = new System.Drawing.Point(650, 26);
            this.portBuildTextBox.Name = "portBuildTextBox";
            this.portBuildTextBox.Size = new System.Drawing.Size(64, 20);
            this.portBuildTextBox.TabIndex = 12;
            // 
            // pathBuildLabel
            // 
            this.pathBuildLabel.AutoSize = true;
            this.pathBuildLabel.Location = new System.Drawing.Point(3, 50);
            this.pathBuildLabel.Name = "pathBuildLabel";
            this.pathBuildLabel.Size = new System.Drawing.Size(29, 13);
            this.pathBuildLabel.TabIndex = 14;
            this.pathBuildLabel.Text = "Path";
            // 
            // pathBuildTextBox
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.pathBuildTextBox, 10);
            this.pathBuildTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.pathBuildTextBox.Location = new System.Drawing.Point(3, 66);
            this.pathBuildTextBox.Name = "pathBuildTextBox";
            this.pathBuildTextBox.Size = new System.Drawing.Size(764, 20);
            this.pathBuildTextBox.TabIndex = 15;
            // 
            // passwordBuildTextBox
            // 
            this.passwordBuildTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.passwordBuildTextBox.Location = new System.Drawing.Point(271, 26);
            this.passwordBuildTextBox.Name = "passwordBuildTextBox";
            this.passwordBuildTextBox.Size = new System.Drawing.Size(107, 20);
            this.passwordBuildTextBox.TabIndex = 17;
            // 
            // portCheckBox
            // 
            this.portCheckBox.AutoSize = true;
            this.portCheckBox.Location = new System.Drawing.Point(650, 3);
            this.portCheckBox.Name = "portCheckBox";
            this.portCheckBox.Size = new System.Drawing.Size(45, 17);
            this.portCheckBox.TabIndex = 18;
            this.portCheckBox.Text = "Port";
            this.portCheckBox.UseVisualStyleBackColor = true;
            this.portCheckBox.CheckedChanged += new System.EventHandler(this.portCheckBox_CheckedChanged);
            // 
            // queryCheckBox
            // 
            this.queryCheckBox.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.queryCheckBox, 9);
            this.queryCheckBox.Location = new System.Drawing.Point(3, 92);
            this.queryCheckBox.Name = "queryCheckBox";
            this.queryCheckBox.Size = new System.Drawing.Size(54, 17);
            this.queryCheckBox.TabIndex = 19;
            this.queryCheckBox.Text = "Query";
            this.queryCheckBox.UseVisualStyleBackColor = true;
            this.queryCheckBox.CheckedChanged += new System.EventHandler(this.queryCheckBox_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.label7, 9);
            this.label7.Location = new System.Drawing.Point(3, 448);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Fragment";
            // 
            // textBox7
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.textBox7, 10);
            this.textBox7.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox7.Location = new System.Drawing.Point(3, 464);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(764, 20);
            this.textBox7.TabIndex = 23;
            // 
            // schemeCheckBox
            // 
            this.schemeCheckBox.AutoSize = true;
            this.schemeCheckBox.Location = new System.Drawing.Point(3, 3);
            this.schemeCheckBox.Name = "schemeCheckBox";
            this.schemeCheckBox.Size = new System.Drawing.Size(65, 17);
            this.schemeCheckBox.TabIndex = 24;
            this.schemeCheckBox.Text = "Scheme";
            this.schemeCheckBox.UseVisualStyleBackColor = true;
            this.schemeCheckBox.CheckedChanged += new System.EventHandler(this.schemeCheckBox_CheckedChanged);
            // 
            // portMessageLabel
            // 
            this.portMessageLabel.AutoSize = true;
            this.portMessageLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.portMessageLabel.Location = new System.Drawing.Point(720, 23);
            this.portMessageLabel.Name = "portMessageLabel";
            this.portMessageLabel.Size = new System.Drawing.Size(45, 13);
            this.portMessageLabel.TabIndex = 25;
            this.portMessageLabel.Text = "(default)";
            // 
            // querySplitContainer
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.querySplitContainer, 10);
            this.querySplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.querySplitContainer.Location = new System.Drawing.Point(3, 115);
            this.querySplitContainer.Name = "querySplitContainer";
            // 
            // querySplitContainer.Panel1
            // 
            this.querySplitContainer.Panel1.Controls.Add(this.queryItemsTableLayoutPanel);
            // 
            // querySplitContainer.Panel2
            // 
            this.querySplitContainer.Panel2.Controls.Add(this.queryItemTableLayoutPanel);
            this.querySplitContainer.Size = new System.Drawing.Size(764, 330);
            this.querySplitContainer.SplitterDistance = 254;
            this.querySplitContainer.TabIndex = 26;
            // 
            // queryItemsTableLayoutPanel
            // 
            this.queryItemsTableLayoutPanel.AutoSize = true;
            this.queryItemsTableLayoutPanel.ColumnCount = 1;
            this.queryItemsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.queryItemsTableLayoutPanel.Controls.Add(this.queryDataGridView, 0, 0);
            this.queryItemsTableLayoutPanel.Controls.Add(this.addButton, 0, 1);
            this.queryItemsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryItemsTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.queryItemsTableLayoutPanel.Name = "queryItemsTableLayoutPanel";
            this.queryItemsTableLayoutPanel.RowCount = 2;
            this.queryItemsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.queryItemsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.queryItemsTableLayoutPanel.Size = new System.Drawing.Size(254, 330);
            this.queryItemsTableLayoutPanel.TabIndex = 0;
            // 
            // queryDataGridView
            // 
            this.queryDataGridView.AllowUserToAddRows = false;
            this.queryDataGridView.AllowUserToDeleteRows = false;
            this.queryDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.queryDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryDataGridView.Location = new System.Drawing.Point(3, 3);
            this.queryDataGridView.MultiSelect = false;
            this.queryDataGridView.Name = "queryDataGridView";
            this.queryDataGridView.ReadOnly = true;
            this.queryDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.queryDataGridView.Size = new System.Drawing.Size(248, 295);
            this.queryDataGridView.TabIndex = 0;
            this.queryDataGridView.CurrentCellChanged += new System.EventHandler(this.queryDataGridView_CurrentCellChanged);
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.Location = new System.Drawing.Point(176, 304);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // queryItemTableLayoutPanel
            // 
            this.queryItemTableLayoutPanel.AutoScroll = true;
            this.queryItemTableLayoutPanel.ColumnCount = 2;
            this.queryItemTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.queryItemTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.queryItemTableLayoutPanel.Controls.Add(this.queryOrderLabel, 0, 0);
            this.queryItemTableLayoutPanel.Controls.Add(this.queryKeyLabel, 0, 1);
            this.queryItemTableLayoutPanel.Controls.Add(this.queryOrderTextBox, 1, 0);
            this.queryItemTableLayoutPanel.Controls.Add(this.queryKeyTextBox, 1, 1);
            this.queryItemTableLayoutPanel.Controls.Add(this.queryValueTextBox, 1, 2);
            this.queryItemTableLayoutPanel.Controls.Add(this.queryFlowLayoutPanel, 0, 3);
            this.queryItemTableLayoutPanel.Controls.Add(this.queryValueCheckBox, 0, 2);
            this.queryItemTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.queryItemTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.queryItemTableLayoutPanel.Name = "queryItemTableLayoutPanel";
            this.queryItemTableLayoutPanel.RowCount = 4;
            this.queryItemTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.queryItemTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.queryItemTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.queryItemTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.queryItemTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.queryItemTableLayoutPanel.Size = new System.Drawing.Size(506, 330);
            this.queryItemTableLayoutPanel.TabIndex = 0;
            // 
            // queryOrderLabel
            // 
            this.queryOrderLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.queryOrderLabel.AutoSize = true;
            this.queryOrderLabel.Location = new System.Drawing.Point(23, 0);
            this.queryOrderLabel.Name = "queryOrderLabel";
            this.queryOrderLabel.Size = new System.Drawing.Size(36, 13);
            this.queryOrderLabel.TabIndex = 0;
            this.queryOrderLabel.Text = "Order:";
            // 
            // queryKeyLabel
            // 
            this.queryKeyLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.queryKeyLabel.AutoSize = true;
            this.queryKeyLabel.Location = new System.Drawing.Point(31, 26);
            this.queryKeyLabel.Name = "queryKeyLabel";
            this.queryKeyLabel.Size = new System.Drawing.Size(28, 13);
            this.queryKeyLabel.TabIndex = 1;
            this.queryKeyLabel.Text = "Key:";
            // 
            // queryOrderTextBox
            // 
            this.queryOrderTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.queryOrderTextBox.Location = new System.Drawing.Point(65, 3);
            this.queryOrderTextBox.Name = "queryOrderTextBox";
            this.queryOrderTextBox.Size = new System.Drawing.Size(438, 20);
            this.queryOrderTextBox.TabIndex = 4;
            // 
            // queryKeyTextBox
            // 
            this.queryKeyTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.queryKeyTextBox.Location = new System.Drawing.Point(65, 29);
            this.queryKeyTextBox.Name = "queryKeyTextBox";
            this.queryKeyTextBox.Size = new System.Drawing.Size(438, 20);
            this.queryKeyTextBox.TabIndex = 5;
            // 
            // queryValueTextBox
            // 
            this.queryValueTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.queryValueTextBox.Location = new System.Drawing.Point(65, 55);
            this.queryValueTextBox.Name = "queryValueTextBox";
            this.queryValueTextBox.Size = new System.Drawing.Size(438, 20);
            this.queryValueTextBox.TabIndex = 6;
            // 
            // queryFlowLayoutPanel
            // 
            this.queryFlowLayoutPanel.AutoSize = true;
            this.queryItemTableLayoutPanel.SetColumnSpan(this.queryFlowLayoutPanel, 2);
            this.queryFlowLayoutPanel.Controls.Add(this.saveQueryButton);
            this.queryFlowLayoutPanel.Controls.Add(this.moveUpQueryButton);
            this.queryFlowLayoutPanel.Controls.Add(this.moveDownQueryButton);
            this.queryFlowLayoutPanel.Controls.Add(this.addQueryButton);
            this.queryFlowLayoutPanel.Controls.Add(this.insertQueryButton);
            this.queryFlowLayoutPanel.Controls.Add(this.cancelQueryButton);
            this.queryFlowLayoutPanel.Controls.Add(this.deleteQueryButton);
            this.queryFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.queryFlowLayoutPanel.Location = new System.Drawing.Point(3, 81);
            this.queryFlowLayoutPanel.Name = "queryFlowLayoutPanel";
            this.queryFlowLayoutPanel.Size = new System.Drawing.Size(500, 58);
            this.queryFlowLayoutPanel.TabIndex = 7;
            // 
            // saveQueryButton
            // 
            this.saveQueryButton.Location = new System.Drawing.Point(3, 3);
            this.saveQueryButton.Name = "saveQueryButton";
            this.saveQueryButton.Size = new System.Drawing.Size(75, 23);
            this.saveQueryButton.TabIndex = 0;
            this.saveQueryButton.Text = "Save";
            this.saveQueryButton.UseVisualStyleBackColor = true;
            this.saveQueryButton.Click += new System.EventHandler(this.saveQueryButton_Click);
            // 
            // moveUpQueryButton
            // 
            this.moveUpQueryButton.Location = new System.Drawing.Point(84, 3);
            this.moveUpQueryButton.Name = "moveUpQueryButton";
            this.moveUpQueryButton.Size = new System.Drawing.Size(75, 23);
            this.moveUpQueryButton.TabIndex = 1;
            this.moveUpQueryButton.Text = "Move Up";
            this.moveUpQueryButton.UseVisualStyleBackColor = true;
            this.moveUpQueryButton.Click += new System.EventHandler(this.moveUpQueryButton_Click);
            // 
            // moveDownQueryButton
            // 
            this.moveDownQueryButton.Location = new System.Drawing.Point(165, 3);
            this.moveDownQueryButton.Name = "moveDownQueryButton";
            this.moveDownQueryButton.Size = new System.Drawing.Size(75, 23);
            this.moveDownQueryButton.TabIndex = 2;
            this.moveDownQueryButton.Text = "Move Down";
            this.moveDownQueryButton.UseVisualStyleBackColor = true;
            this.moveDownQueryButton.Click += new System.EventHandler(this.moveDownQueryButton_Click);
            // 
            // addQueryButton
            // 
            this.addQueryButton.Location = new System.Drawing.Point(246, 3);
            this.addQueryButton.Name = "addQueryButton";
            this.addQueryButton.Size = new System.Drawing.Size(75, 23);
            this.addQueryButton.TabIndex = 3;
            this.addQueryButton.Text = "Add";
            this.addQueryButton.UseVisualStyleBackColor = true;
            this.addQueryButton.Click += new System.EventHandler(this.addQueryButton_Click);
            // 
            // insertQueryButton
            // 
            this.insertQueryButton.Location = new System.Drawing.Point(327, 3);
            this.insertQueryButton.Name = "insertQueryButton";
            this.insertQueryButton.Size = new System.Drawing.Size(75, 23);
            this.insertQueryButton.TabIndex = 4;
            this.insertQueryButton.Text = "Insert";
            this.insertQueryButton.UseVisualStyleBackColor = true;
            this.insertQueryButton.Click += new System.EventHandler(this.insertQueryButton_Click);
            // 
            // cancelQueryButton
            // 
            this.cancelQueryButton.Location = new System.Drawing.Point(408, 3);
            this.cancelQueryButton.Name = "cancelQueryButton";
            this.cancelQueryButton.Size = new System.Drawing.Size(75, 23);
            this.cancelQueryButton.TabIndex = 6;
            this.cancelQueryButton.Text = "Cancel";
            this.cancelQueryButton.UseVisualStyleBackColor = true;
            this.cancelQueryButton.Click += new System.EventHandler(this.cancelQueryButton_Click);
            // 
            // deleteQueryButton
            // 
            this.deleteQueryButton.Location = new System.Drawing.Point(3, 32);
            this.deleteQueryButton.Name = "deleteQueryButton";
            this.deleteQueryButton.Size = new System.Drawing.Size(75, 23);
            this.deleteQueryButton.TabIndex = 5;
            this.deleteQueryButton.Text = "Delete";
            this.deleteQueryButton.UseVisualStyleBackColor = true;
            this.deleteQueryButton.Click += new System.EventHandler(this.deleteQueryButton_Click);
            // 
            // queryValueCheckBox
            // 
            this.queryValueCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.queryValueCheckBox.AutoSize = true;
            this.queryValueCheckBox.Location = new System.Drawing.Point(3, 55);
            this.queryValueCheckBox.Name = "queryValueCheckBox";
            this.queryValueCheckBox.Size = new System.Drawing.Size(56, 17);
            this.queryValueCheckBox.TabIndex = 3;
            this.queryValueCheckBox.Text = "Value:";
            this.queryValueCheckBox.UseVisualStyleBackColor = true;
            this.queryValueCheckBox.CheckedChanged += new System.EventHandler(this.queryValueCheckBox_CheckedChanged);
            // 
            // buildUrlButton
            // 
            this.buildUrlButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.buildUrlButton, 10);
            this.buildUrlButton.Location = new System.Drawing.Point(692, 490);
            this.buildUrlButton.Name = "buildUrlButton";
            this.buildUrlButton.Size = new System.Drawing.Size(75, 23);
            this.buildUrlButton.TabIndex = 27;
            this.buildUrlButton.Text = "Build";
            this.buildUrlButton.UseVisualStyleBackColor = true;
            this.buildUrlButton.Click += new System.EventHandler(this.buildUrlButton_Click);
            // 
            // buildErrorLabel
            // 
            this.buildErrorLabel.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.buildErrorLabel, 10);
            this.buildErrorLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.buildErrorLabel.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.buildErrorLabel.Location = new System.Drawing.Point(3, 516);
            this.buildErrorLabel.Name = "buildErrorLabel";
            this.buildErrorLabel.Size = new System.Drawing.Size(764, 13);
            this.buildErrorLabel.TabIndex = 28;
            this.buildErrorLabel.Text = "Nothing to build.";
            // 
            // componentsTabPage
            // 
            this.componentsTabPage.Controls.Add(this.componentsTableLayoutPanel);
            this.componentsTabPage.Location = new System.Drawing.Point(4, 22);
            this.componentsTabPage.Name = "componentsTabPage";
            this.componentsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.componentsTabPage.Size = new System.Drawing.Size(776, 535);
            this.componentsTabPage.TabIndex = 2;
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
            this.componentsTableLayoutPanel.Controls.Add(this.label1, 0, 0);
            this.componentsTableLayoutPanel.Controls.Add(this.schemeComponentTextBox, 1, 0);
            this.componentsTableLayoutPanel.Controls.Add(this.label5, 2, 0);
            this.componentsTableLayoutPanel.Controls.Add(this.userNameComponentTextBox, 3, 0);
            this.componentsTableLayoutPanel.Controls.Add(this.label8, 4, 0);
            this.componentsTableLayoutPanel.Controls.Add(this.passwordComponentTextBox, 5, 0);
            this.componentsTableLayoutPanel.Controls.Add(this.label9, 0, 1);
            this.componentsTableLayoutPanel.Controls.Add(this.hostComponentTextBox, 1, 1);
            this.componentsTableLayoutPanel.Controls.Add(this.label10, 4, 1);
            this.componentsTableLayoutPanel.Controls.Add(this.portComponentTextBox, 5, 1);
            this.componentsTableLayoutPanel.Controls.Add(this.label11, 6, 1);
            this.componentsTableLayoutPanel.Controls.Add(this.label12, 0, 2);
            this.componentsTableLayoutPanel.Controls.Add(this.segmentsComponentDataGridView, 0, 3);
            this.componentsTableLayoutPanel.Controls.Add(this.queryComponentDataGridView, 0, 5);
            this.componentsTableLayoutPanel.Controls.Add(this.label14, 0, 4);
            this.componentsTableLayoutPanel.Controls.Add(this.queryNotIncludedLabel, 0, 6);
            this.componentsTableLayoutPanel.Controls.Add(this.label15, 0, 7);
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
            this.componentsTableLayoutPanel.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Scheme:";
            // 
            // schemeComponentTextBox
            // 
            this.schemeComponentTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.schemeComponentTextBox.Location = new System.Drawing.Point(60, 3);
            this.schemeComponentTextBox.Name = "schemeComponentTextBox";
            this.schemeComponentTextBox.ReadOnly = true;
            this.schemeComponentTextBox.Size = new System.Drawing.Size(206, 20);
            this.schemeComponentTextBox.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(272, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "User Name:";
            // 
            // userNameComponentTextBox
            // 
            this.userNameComponentTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.userNameComponentTextBox.Location = new System.Drawing.Point(341, 3);
            this.userNameComponentTextBox.Name = "userNameComponentTextBox";
            this.userNameComponentTextBox.ReadOnly = true;
            this.userNameComponentTextBox.Size = new System.Drawing.Size(206, 20);
            this.userNameComponentTextBox.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(553, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Password:";
            // 
            // passwordComponentTextBox
            // 
            this.componentsTableLayoutPanel.SetColumnSpan(this.passwordComponentTextBox, 2);
            this.passwordComponentTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.passwordComponentTextBox.Location = new System.Drawing.Point(615, 3);
            this.passwordComponentTextBox.Name = "passwordComponentTextBox";
            this.passwordComponentTextBox.ReadOnly = true;
            this.passwordComponentTextBox.Size = new System.Drawing.Size(152, 20);
            this.passwordComponentTextBox.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(22, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Host:";
            // 
            // hostComponentTextBox
            // 
            this.componentsTableLayoutPanel.SetColumnSpan(this.hostComponentTextBox, 3);
            this.hostComponentTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hostComponentTextBox.Location = new System.Drawing.Point(60, 29);
            this.hostComponentTextBox.Name = "hostComponentTextBox";
            this.hostComponentTextBox.ReadOnly = true;
            this.hostComponentTextBox.Size = new System.Drawing.Size(487, 20);
            this.hostComponentTextBox.TabIndex = 7;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(580, 26);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "Port:";
            // 
            // portComponentTextBox
            // 
            this.portComponentTextBox.Location = new System.Drawing.Point(615, 29);
            this.portComponentTextBox.Name = "portComponentTextBox";
            this.portComponentTextBox.ReadOnly = true;
            this.portComponentTextBox.Size = new System.Drawing.Size(100, 20);
            this.portComponentTextBox.TabIndex = 9;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label11.Location = new System.Drawing.Point(721, 26);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(45, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "(default)";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.componentsTableLayoutPanel.SetColumnSpan(this.label12, 7);
            this.label12.Location = new System.Drawing.Point(3, 52);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(54, 13);
            this.label12.TabIndex = 11;
            this.label12.Text = "Segments";
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
            this.segmentsComponentDataGridView.TabIndex = 12;
            // 
            // queryNotIncludedLabel
            // 
            this.queryNotIncludedLabel.AutoSize = true;
            this.componentsTableLayoutPanel.SetColumnSpan(this.queryNotIncludedLabel, 7);
            this.queryNotIncludedLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.queryNotIncludedLabel.Location = new System.Drawing.Point(3, 448);
            this.queryNotIncludedLabel.Name = "queryNotIncludedLabel";
            this.queryNotIncludedLabel.Size = new System.Drawing.Size(70, 13);
            this.queryNotIncludedLabel.TabIndex = 13;
            this.queryNotIncludedLabel.Text = "Not included.";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 250);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(35, 13);
            this.label14.TabIndex = 14;
            this.label14.Text = "Query";
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
            this.queryComponentDataGridView.TabIndex = 15;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(3, 461);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(51, 13);
            this.label15.TabIndex = 16;
            this.label15.Text = "Fragment";
            // 
            // fragmentComponentTextBox
            // 
            this.componentsTableLayoutPanel.SetColumnSpan(this.fragmentComponentTextBox, 7);
            this.fragmentComponentTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.fragmentComponentTextBox.Location = new System.Drawing.Point(3, 477);
            this.fragmentComponentTextBox.Name = "fragmentComponentTextBox";
            this.fragmentComponentTextBox.ReadOnly = true;
            this.fragmentComponentTextBox.Size = new System.Drawing.Size(764, 20);
            this.fragmentComponentTextBox.TabIndex = 17;
            // 
            // parentDirButton
            // 
            this.parentDirButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.componentsTableLayoutPanel.SetColumnSpan(this.parentDirButton, 7);
            this.parentDirButton.Location = new System.Drawing.Point(692, 503);
            this.parentDirButton.Name = "parentDirButton";
            this.parentDirButton.Size = new System.Drawing.Size(75, 23);
            this.parentDirButton.TabIndex = 18;
            this.parentDirButton.Text = "Parent Dir";
            this.parentDirButton.UseVisualStyleBackColor = true;
            this.parentDirButton.Click += new System.EventHandler(this.parentDirButton_Click);
            // 
            // EditUrlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.mainTabControl);
            this.Name = "EditUrlForm";
            this.Text = "EditUrlForm";
            this.mainTabControl.ResumeLayout(false);
            this.parseTabPage.ResumeLayout(false);
            this.parseTabPage.PerformLayout();
            this.parseTableLayoutPanel.ResumeLayout(false);
            this.parseTableLayoutPanel.PerformLayout();
            this.buildTabPage.ResumeLayout(false);
            this.buildTabPage.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.querySplitContainer.Panel1.ResumeLayout(false);
            this.querySplitContainer.Panel1.PerformLayout();
            this.querySplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.querySplitContainer)).EndInit();
            this.querySplitContainer.ResumeLayout(false);
            this.queryItemsTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.queryDataGridView)).EndInit();
            this.queryItemTableLayoutPanel.ResumeLayout(false);
            this.queryItemTableLayoutPanel.PerformLayout();
            this.queryFlowLayoutPanel.ResumeLayout(false);
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
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.TabPage buildTabPage;
        private System.Windows.Forms.TabPage componentsTabPage;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox userNameCheckBox;
        private System.Windows.Forms.ComboBox schemeComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox userNameBuildTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox passwordCheckBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label hostBuildLabel;
        private System.Windows.Forms.TextBox hostBuildTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox portBuildTextBox;
        private System.Windows.Forms.Label pathBuildLabel;
        private System.Windows.Forms.TextBox pathBuildTextBox;
        private System.Windows.Forms.TextBox passwordBuildTextBox;
        private System.Windows.Forms.CheckBox portCheckBox;
        private System.Windows.Forms.CheckBox queryCheckBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.CheckBox schemeCheckBox;
        private System.Windows.Forms.Label portMessageLabel;
        private System.Windows.Forms.SplitContainer querySplitContainer;
        private System.Windows.Forms.TableLayoutPanel queryItemTableLayoutPanel;
        private System.Windows.Forms.Label queryOrderLabel;
        private System.Windows.Forms.Label queryKeyLabel;
        private System.Windows.Forms.CheckBox queryValueCheckBox;
        private System.Windows.Forms.TextBox queryOrderTextBox;
        private System.Windows.Forms.TextBox queryKeyTextBox;
        private System.Windows.Forms.TextBox queryValueTextBox;
        private System.Windows.Forms.FlowLayoutPanel queryFlowLayoutPanel;
        private System.Windows.Forms.Button saveQueryButton;
        private System.Windows.Forms.Button moveUpQueryButton;
        private System.Windows.Forms.Button moveDownQueryButton;
        private System.Windows.Forms.Button addQueryButton;
        private System.Windows.Forms.Button insertQueryButton;
        private System.Windows.Forms.Button cancelQueryButton;
        private System.Windows.Forms.Button deleteQueryButton;
        private System.Windows.Forms.TableLayoutPanel queryItemsTableLayoutPanel;
        private System.Windows.Forms.DataGridView queryDataGridView;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button buildUrlButton;
        private System.Windows.Forms.Label buildErrorLabel;
        private System.Windows.Forms.TableLayoutPanel componentsTableLayoutPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox schemeComponentTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox userNameComponentTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox passwordComponentTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox hostComponentTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox portComponentTextBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DataGridView segmentsComponentDataGridView;
        private System.Windows.Forms.DataGridView queryComponentDataGridView;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label queryNotIncludedLabel;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox fragmentComponentTextBox;
        private System.Windows.Forms.Button parentDirButton;
    }
}