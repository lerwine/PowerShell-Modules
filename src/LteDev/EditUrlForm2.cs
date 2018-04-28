using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LteDev
{
    public class EditUrlForm2 : Form
    {
        public EditUrlForm2()
        {
            InitializeComponent();
        }
		
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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

		private int _previousTabIndex = 0;
		private DataRow _editingRow = null;
		
		#region Control Fields
		
        private TabControl _mainTabControl;
		
		#region parsingTabPage
		
        private TextBox _fullUrlTextBox;
		private Label _parseErrorLabel;
		
		#endregion

		#region builderTabPage
		
        private CheckBox _schemeCheckBox;
		private Label _schemeSeparatorLabel;
        private CheckBox _userNameCheckBox;
        private ComboBox _schemeComboBox;
        private TextBox _userNameBuildTextBox;
        private Label _passwordSeparatorLabel;
        private CheckBox _passwordCheckBox;
        private TextBox _passwordBuildTextBox;
        private Label _authSeparatorLabel;
		private Label _hostBuildLabel;
        private TextBox _hostBuildTextBox;
        private Label _portSeparatorLabel;
        private TextBox _portBuildTextBox;
        private TextBox _pathTextBox;
		private TextBox _passwordBuildTextBox;
        private CheckBox _portCheckBox;
        private CheckBox _queryCheckBox;
		private SplitContainer _queryBuildSplitContainer;
        private DataGridView _queryBuildDataGridView;
		private Label _queryBuildOrderLabel;
		private TextBox _queryBuildOrderTextBox;
		private TextBox _queryBuildKeyTextBox;
		private TextBox _queryBuildValueTextBox;
		private CheckBox _queryBuildHasValueCheckBox;
		private Button _querySaveButton;
		private Button _queryMoveUpButton;
		private Button _queryMoveDownButton;
		private Button _queryAddButton;
		private Button _queryInsertButton;
		private Button _queryCancelButton;
		private Button _queryDeleteButton;
		private CheckBox _fragmentCheckBox;
        private TextBox _fragmentBuildTextBox;
		private Label _buildErrorLabel;
		private DataTable _queryDataTable;
		private DataGridViewTextBoxColumn _queryBuildOrderTextColumn;
		private DataGridViewTextBoxColumn _queryBuildKeyTextColumn;
		private DataGridViewTextBoxColumn _queryBuildValueTextColumn;
		private DataGridViewCheckBoxColumn _queryBuildHasValueColumn;
		private DataTable _segmentsDataTable;
		private DataColumn _segmentOrderDataColumn;
		private DataColumn _segmentNameDataColumn;
		
		#endregion

		#region componentsTabPage
		
        private TextBox _schemeTextBox;
        private TextBox _userNameComponentTextBox;
		private Label _userNameEmptyLabel;
        private TextBox _passwordComponentTextBox;
		private Label _passwordEmptyLabel;
        private TextBox _hostComponentTextBox;
        private TextBox _portComponentTextBox;
		private Label _portDefinedLabel;
        private DataGridView _segmentsDataGridView;
        private Label _pathEmptyLabel;
        private DataGridView _queryComponentDataGridView;
        private Label _queryEmptyLabel;
        private TextBox _fragmentComponentTextBox;
        private Label _fragmentEmptyLabel;
		
		#endregion
		
		#endregion
		
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			#region Create Controls
			
            _mainTabControl = new TabControl();
			
            #region parsingTabPage
			
            TabPage parsingTabPage = new TabPage();
			TableLayoutPanel parsingTableLayoutPanel = new TableLayoutPanel();
			Label fullUrlLabel = new Label();
            _fullUrlTextBox = new TextBox();
            Button parseButton = new Button();
			_parseErrorLabel = new Label();
			
			#endregion

            #region builderTabPage
			
            TabPage builderTabPage = new TabPage();
            TableLayoutPanel builderTableLayoutPanel = new TableLayoutPanel();
            _userNameCheckBox = new CheckBox();
            _schemeComboBox = new ComboBox();
            _schemeSeparatorLabel = new Label();
            _userNameBuildTextBox = new TextBox();
            _passwordSeparatorLabel = new Label();
            _passwordCheckBox = new CheckBox();
            _authSeparatorLabel = new Label();
            _hostBuildLabel = new Label();
            _hostBuildTextBox = new TextBox();
            _portSeparatorLabel = new Label();
            _portBuildTextBox = new TextBox();
            Label pathLabel = new Label();
            _pathTextBox = new TextBox();
            _passwordBuildTextBox = new TextBox();
            _portCheckBox = new CheckBox();
            _queryCheckBox = new CheckBox();
			_queryBuildSplitContainer = new SplitContainer();
            _queryBuildDataGridView = new DataGridView();
			_queryBuildOrderTextColumn = new DataGridViewTextBoxColumn();
			_queryBuildKeyTextColumn = new DataGridViewTextBoxColumn();
			_queryBuildValueTextColumn = new DataGridViewTextBoxColumn();
			_queryBuildHasValueColumn = new DataGridViewCheckBoxColumn();
			TableLayoutPanel queryBuildDetailTableLayoutPanel = new TableLayoutPanel();
			_queryBuildOrderLabel = new Label();
			_queryBuildOrderTextBox = new TextBox();
		    Label queryBuildKeyLabel = new Label();
		    _queryBuildKeyTextBox = new TextBox();
		    Label queryBuildValueLabel = new Label();
		    _queryBuildValueTextBox = new TextBox();
		    _queryBuildHasValueCheckBox = new CheckBox();
			FlowLayoutPanel queryFlowLayoutPanel = new FlowLayoutPanel();
			_querySaveButton = new Button();
			_queryMoveUpButton = new Button();
			_queryAddButton = new Button();
			_queryMoveDownButton = new Button();
			_queryInsertButton = new Button();
			_queryCancelButton = new Button();
			_queryDeleteButton = new Button();
            _fragmentCheckBox = new CheckBox();
            _fragmentBuildTextBox = new TextBox();
            Button buildButton = new Button();
			_buildErrorLabel = new Label();
			
			#endregion

            #region componentsTabPage
			
            TabPage componentsTabPage = new TabPage();
            TableLayoutPanel componentsTableLayoutPanel = new TableLayoutPanel();
            _schemeCheckbox = new Checkbox();
            _schemeTextBox = new TextBox();
            Label userNameLabel = new Label();
            _userNameComponentTextBox = new TextBox();
		    _userNameEmptyLabel = new Label();
            Label passwordLabel = new Label();
            _passwordComponentTextBox = new TextBox();
		    _passwordEmptyLabel = new Label();
            Label hostComponentLabel = new Label();
            _hostComponentTextBox = new TextBox();
            Label portLabel = new Label();
            _portComponentTextBox = new TextBox();
		    _portDefinedLabel = new Label();
            Label segmentsLabel = new Label();
            _segmentsDataGridView = new DataGridView();
            _pathEmptyLabel = new Label();
            Label queryLabel = new Label();
            _queryComponentDataGridView = new DataGridView();
			DataGridViewTextBoxColumn queryComponentOrderTextColumn = new DataGridViewTextBoxColumn();
			DataGridViewTextBoxColumn queryComponentKeyTextColumn = new DataGridViewTextBoxColumn();
			DataGridViewTextBoxColumn queryComponentValueTextColumn = new DataGridViewTextBoxColumn();
			DataGridViewCheckBoxColumn queryComponentHasValueColumn = new DataGridViewCheckBoxColumn();
			DataGridViewButtonColumn queryBuildDeleteButtonColumn = new DataGridViewButtonColumn();
            _queryEmptyLabel = new Label();
            Label fragmentComponentLabel = new Label();
            _fragmentComponentTextBox = new TextBox();
            _fragmentEmptyLabel = new Label();
            Button parentDirButton = new Button();
			
			#endregion
			
			#endregion
			
			#region Data Tables
			
			#region queryDataTable
			
			_queryDataTable = new DataTable("Query");
			
			#region queryOrderDataColumn
			
			_queryOrderDataColumn = _queryDataTable.Columns.Add("Order", typeof(int));
			_queryOrderDataColumn.AllowDBNull = false;
			_queryOrderDataColumn.Caption = "Order";
			_queryOrderDataColumn.Unique = true;
			
			#endregion
			
			#region queryKeyDataColumn
			
			_queryKeyDataColumn = _queryDataTable.Columns.Add("Key", typeof(string));
			_queryKeyDataColumn.AllowDBNull = false;
			_queryKeyDataColumn.Caption = "Key";
			
			#endregion
			
			#region queryValueDataColumn
			
			_queryValueDataColumn = _queryDataTable.Columns.Add("Value", typeof(string));
			_queryValueDataColumn.AllowDBNull = false;
			_queryValueDataColumn.Caption = "Value";
			
			#endregion
			
			#region queryHasValueDataColumn
			
			_queryHasValueDataColumn = _queryDataTable.Columns.Add("HasValue", typeof(bool));
			_queryHasValueDataColumn.AllowDBNull = false;
			_queryHasValueDataColumn.Caption = "Has Value";
			
			#endregion
			
			#endregion
			
			#region segmentsDataTable
			
			_segmentsDataTable = new DataTable("Segments");
			
			#region segmentOrderDataColumn
			
			_segmentOrderDataColumn = _segmentsDataTable.Columns.Add("Order", typeof(int));
			_segmentOrderDataColumn.AllowDBNull = false;
			_segmentOrderDataColumn.Caption = "Order";
			_segmentOrderDataColumn.Unique = true;
			
			#endregion
			
			#region segmentNameDataColumn
			
			DataColumn _segmentNameDataColumn = _segmentsDataTable.Columns.Add("Name", typeof(string));
			_segmentNameDataColumn.AllowDBNull = false;
			_segmentNameDataColumn.Caption = "Name";
			
			#endregion
			
			#endregion
			
			#endregion
			
            #region Begin Init
			
            _mainTabControl.SuspendLayout();
            parsingTabPage.SuspendLayout();
            builderTabPage.SuspendLayout();
            componentsTabPage.SuspendLayout();
            builderTableLayoutPanel.SuspendLayout();
            _queryBuildSplitContainer.SuspendLayout();
            queryBuildDetailTableLayoutPanel.SuspendLayout();
            componentsTableLayoutPanel.SuspendLayout();
            _queryComponentSplitContainer.SuspendLayout();
            queryComponentDetailTableLayoutPanel.SuspendLayout();
			
            ((ISupportInitialize)(_queryBuildDataGridView)).BeginInit();
            ((ISupportInitialize)(_queryComponentDataGridView)).BeginInit();
            ((ISupportInitialize)(_segmentsDataGridView)).BeginInit();
			
            this.SuspendLayout();
			
			#endregion
			
			#region Initialize Controls
			
            _mainTabControl.Controls.Add(parsingTabPage);
            _mainTabControl.Controls.Add(builderTabPage);
            _mainTabControl.Controls.Add(componentsTabPage);
            _mainTabControl.Dock = DockStyle.Fill;
            _mainTabControl.Name = "mainTabControl";
            _mainTabControl.SelectedIndex = _previousTabIndex;
            _mainTabControl.TabIndex = _previousTabIndex;
			
            #region parsingTabPage
			
            parsingTabPage.Controls.Add(parsingTableLayoutPanel);
            parsingTabPage.Name = "parsingTabPage";
            parsingTabPage.Padding = new Padding(3);
            parsingTabPage.TabIndex = 1;
            parsingTabPage.Text = "Parser";
            parsingTabPage.UseVisualStyleBackColor = true;
            
            #region parsingTableLayoutPanel
			
            parsingTableLayoutPanel.AutoSize = true;
            parsingTableLayoutPanel.ColumnCount = 1;
            parsingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            parsingTableLayoutPanel.Controls.Add(fullUrlLabel, 0, 0);
            parsingTableLayoutPanel.Controls.Add(_fullUrlTextBox, 0, 1);
            parsingTableLayoutPanel.Controls.Add(parseButton, 0, 2);
            parsingTableLayoutPanel.Controls.Add(_parseErrorLabel, 0, 3);
            parsingTableLayoutPanel.Dock = DockStyle.Fill;
            parsingTableLayoutPanel.Name = "parsingTableLayoutPanel";
            parsingTableLayoutPanel.RowCount = 4;
            parsingTableLayoutPanel.RowStyles.Add(new RowStyle());
            parsingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            parsingTableLayoutPanel.RowStyles.Add(new RowStyle());
            parsingTableLayoutPanel.RowStyles.Add(new RowStyle());
            parsingTableLayoutPanel.TabStop = false;
            
			#endregion

            #region fullUrlLabel
			
            fullUrlLabel.AutoSize = true;
            fullUrlLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            fullUrlLabel.Name = "fullUrlLabel";
            fullUrlLabel.TabStop = false;
            fullUrlLabel.Text = "Full URL";
            
			#endregion

            #region fullUrlTextBox
			
            _fullUrlTextBox.Dock = DockStyle.Top;
            _fullUrlTextBox.Name = "fullUrlTextBox";
            _fullUrlTextBox.TabIndex = 5;
            
			#endregion

            #region parseButton
			
            parseButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            parseButton.Name = "parseButton";
            parseButton.Size = new Size(75, 23);
            parseButton.TabIndex = 4;
            parseButton.Text = "parseButton";
            parseButton.UseVisualStyleBackColor = true;
            
			#endregion

            #region parseErrorLabel
			
            _parseErrorLabel.AutoSize = true;
            _parseErrorLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _parseErrorLabel.Name = "parseErrorLabel";
            _parseErrorLabel.TabStop = false;
            _parseErrorLabel.Visible = false;
            _parseErrorLabel.Text = "";
			
			#endregion

			#endregion

            #region builderTabPage
			
            builderTabPage.Controls.Add(builderTableLayoutPanel);
            builderTabPage.Name = "builderTabPage";
            builderTabPage.Padding = new Padding(3);
            builderTabPage.TabIndex = 2;
            builderTabPage.Text = "Builder";
            builderTabPage.UseVisualStyleBackColor = true;
            
            #region builderTableLayoutPanel
			
            builderTableLayoutPanel.AutoSize = true;
            builderTableLayoutPanel.ColumnCount = 9;
            builderTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13.63636F));
            builderTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            builderTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13.63636F));
            builderTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            builderTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13.63636F));
            builderTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            builderTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27.27273F));
            builderTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            builderTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 31.81818F));
            builderTableLayoutPanel.Controls.Add(_schemeCheckbox, 0, 0);
            builderTableLayoutPanel.Controls.Add(_schemeComboBox, 0, 1);
            builderTableLayoutPanel.Controls.Add(_schemeSeparatorLabel, 1, 1);
            builderTableLayoutPanel.Controls.Add(_userNameCheckBox, 2, 0);
            builderTableLayoutPanel.Controls.Add(_userNameBuildTextBox, 2, 1);
            builderTableLayoutPanel.Controls.Add(_passwordSeparatorLabel, 3, 1);
            builderTableLayoutPanel.Controls.Add(_passwordCheckBox, 4, 0);
            builderTableLayoutPanel.Controls.Add(_passwordBuildTextBox, 4, 1);
            builderTableLayoutPanel.Controls.Add(_authSeparatorLabel, 5, 1);
            builderTableLayoutPanel.Controls.Add(_hostBuildLabel, 6, 0);
            builderTableLayoutPanel.Controls.Add(_hostBuildTextBox, 6, 1);
            builderTableLayoutPanel.Controls.Add(_portSeparatorLabel, 7, 1);
            builderTableLayoutPanel.Controls.Add(_portCheckBox, 8, 0);
            builderTableLayoutPanel.Controls.Add(_portBuildTextBox, 8, 1);
            builderTableLayoutPanel.Controls.Add(pathLabel, 0, 2);
            builderTableLayoutPanel.Controls.Add(_pathTextBox, 0, 3);
            builderTableLayoutPanel.Controls.Add(_queryCheckBox, 0, 4);
            builderTableLayoutPanel.Controls.Add(_queryBuildSplitContainer, 0, 5);
            builderTableLayoutPanel.Controls.Add(_fragmentCheckBox, 0, 6);
            builderTableLayoutPanel.Controls.Add(_fragmentBuildTextBox, 0, 7);
            builderTableLayoutPanel.Controls.Add(buildButton, 0, 8);
            builderTableLayoutPanel.Controls.Add(_buildErrorLabel, 0, 9);
            builderTableLayoutPanel.Dock = DockStyle.Fill;
            builderTableLayoutPanel.Name = "builderTableLayoutPanel";
            builderTableLayoutPanel.RowCount = 10;
            builderTableLayoutPanel.RowStyles.Add(new RowStyle());
            builderTableLayoutPanel.RowStyles.Add(new RowStyle());
            builderTableLayoutPanel.RowStyles.Add(new RowStyle());
            builderTableLayoutPanel.RowStyles.Add(new RowStyle());
            builderTableLayoutPanel.RowStyles.Add(new RowStyle());
            builderTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            builderTableLayoutPanel.RowStyles.Add(new RowStyle());
            builderTableLayoutPanel.RowStyles.Add(new RowStyle());
            builderTableLayoutPanel.RowStyles.Add(new RowStyle());
            builderTableLayoutPanel.RowStyles.Add(new RowStyle());
            builderTableLayoutPanel.TabStop = false;
            
			#endregion

            #region schemeCheckbox
			
            _schemeCheckbox.AutoSize = true;
            builderTableLayoutPanel.SetColumnSpan(_schemeCheckbox, 2);
            _schemeCheckbox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _schemeCheckbox.Name = "schemeCheckbox";
            _schemeCheckbox.TabStop = false;
            _schemeCheckbox.Text = "Scheme";
            
			#endregion

            #region userNameCheckBox
			
            _userNameCheckBox.AutoSize = true;
            builderTableLayoutPanel.SetColumnSpan(_userNameCheckBox, 2);
            _userNameCheckBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _userNameCheckBox.Name = "userNameCheckBox";
            _userNameCheckBox.TabIndex = 6;
            _userNameCheckBox.Text = "UserName";
            _userNameCheckBox.UseVisualStyleBackColor = true;
            _userNameCheckBox.Visible = false;
            
			#endregion

            #region schemeComboBox
			
            _schemeComboBox.Dock = DockStyle.Top;
            _schemeComboBox.FormattingEnabled = true;
            _schemeComboBox.Name = "schemeComboBox";
            _schemeComboBox.TabIndex = 7;
            _schemeComboBox.Visible = false;
            
			#endregion

            #region schemeSeparatorLabel
            
            _schemeSeparatorLabel.AutoSize = true;
            _schemeSeparatorLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _schemeSeparatorLabel.Name = "schemeSeparatorLabel";
            _schemeSeparatorLabel.TabStop = false;
            _schemeSeparatorLabel.Text = "://";
            _schemeSeparatorLabel.Visible = false;
            
			#endregion

            #region userNameBuildTextBox
            
            _userNameBuildTextBox.Dock = DockStyle.Top;
            _userNameBuildTextBox.Name = "userNameBuildTextBox";
            _userNameBuildTextBox.TabIndex = 8;
            _userNameBuildTextBox.Visible = false;
            
			#endregion

            #region passwordSeparatorLabel
            
            _passwordSeparatorLabel.AutoSize = true;
            _passwordSeparatorLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _passwordSeparatorLabel.Name = "passwordSeparatorLabel";
            _passwordSeparatorLabel.TabStop = false;
            _passwordSeparatorLabel.Text = ":";
            _passwordSeparatorLabel.Visible = false;
            
			#endregion

            #region passwordCheckBox
            
            _passwordCheckBox.AutoSize = true;
            builderTableLayoutPanel.SetColumnSpan(_passwordCheckBox, 2);
            _passwordCheckBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _passwordCheckBox.Name = "passwordCheckBox";
            _passwordCheckBox.TabIndex = 9;
            _passwordCheckBox.Text = "Password";
            _passwordCheckBox.UseVisualStyleBackColor = true;
            _passwordCheckBox.Visible = false;
            
			#endregion

            #region passwordBuildTextBox
            
            _passwordBuildTextBox.Dock = DockStyle.Top;
            _passwordBuildTextBox.Name = "passwordBuildTextBox";
            _passwordBuildTextBox.TabIndex = 13;
            
			#endregion

            #region authSeparatorLabel
            
            _authSeparatorLabel.AutoSize = true;
            _authSeparatorLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _authSeparatorLabel.Name = "authSeparatorLabel";
            _authSeparatorLabel.TabStop = false;
            _authSeparatorLabel.Text = "@";
            _authSeparatorLabel.Visible = false;
            
			#endregion

            #region hostBuildLabel
            
            _hostBuildLabel.AutoSize = true;
            builderTableLayoutPanel.SetColumnSpan(_hostBuildLabel, 2);
            _hostBuildLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _hostBuildLabel.Name = "hostBuildLabel";
            _hostBuildLabel.TabStop = false;
            _hostBuildLabel.Text = "Host";
            _hostBuildLabel.Visible = false;
            
			#endregion

            #region hostBuildTextBox
            
            _hostBuildTextBox.Dock = DockStyle.Top;
            _hostBuildTextBox.Name = "hostBuildTextBox";
            _hostBuildTextBox.TabIndex = 10;
            _hostBuildTextBox.Visible = false;
            
			#endregion

            #region portCheckBox
            
            _portCheckBox.AutoSize = true;
            _portCheckBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _portCheckBox.Name = "portCheckBox";
            _portCheckBox.TabIndex = 14;
            _portCheckBox.Text = "Port";
            _portCheckBox.UseVisualStyleBackColor = true;
            _portCheckBox.Visible = false;
            
			#endregion

            #region portSeparatorLabel
            
            _portSeparatorLabel.AutoSize = true;
            _portSeparatorLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _portSeparatorLabel.Name = "portSeparatorLabel";
            _portSeparatorLabel.TabStop = false;
            _portSeparatorLabel.Text = ":";
            _portSeparatorLabel.Visible = false;
            
			#endregion

            #region portBuildTextBox
            
            _portBuildTextBox.Dock = DockStyle.Top;
            _portBuildTextBox.Name = "portBuildTextBox";
            _portBuildTextBox.TabIndex = 11;
            _portBuildTextBox.Visible = false;
            
			#endregion

            #region pathLabel
            
            builderTableLayoutPanel.SetColumnSpan(pathLabel, 9);
            pathLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pathLabel.AutoSize = true;
            pathLabel.Name = "pathLabel";
            pathLabel.TabStop = false;
            pathLabel.Text = "Path";
            
			#endregion

            #region pathTextBox
            
            builderTableLayoutPanel.SetColumnSpan(_pathTextBox, 9);
            _pathTextBox.Dock = DockStyle.Top;
            _pathTextBox.Name = "pathTextBox";
            _pathTextBox.TabIndex = 12;
            
			#endregion

            #region queryCheckBox
            
            _queryCheckBox.AutoSize = true;
            builderTableLayoutPanel.SetColumnSpan(_queryCheckBox, 9);
            _queryCheckBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _queryCheckBox.Name = "queryCheckBox";
            _queryCheckBox.TabIndex = 15;
            _queryCheckBox.Text = "Query";
            _queryCheckBox.UseVisualStyleBackColor = true;
            
			#endregion

			#region queryBuildSplitContainer
			
            builderTableLayoutPanel.SetColumnSpan(_queryBuildSplitContainer, 9);
            _queryBuildSplitContainer.Dock = DockStyle.Fill;
            _queryBuildSplitContainer.Name = "queryBuildSplitContainer";
            _queryBuildSplitContainer.Panel1.Controls.Add(queryBuildDataGridView);
            _queryBuildSplitContainer.Panel2.Controls.Add(queryBuildDetailTableLayoutPanel);
			_queryBuildSplitContainer.TabIndex = 16;
			_queryBuildSplitContainer.Panel2Collapsed = true;
			_queryBuildSplitContainer.Visible = false;
            
            #region queryBuildDataGridView
            
            _queryBuildDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _queryBuildDataGridView.Dock = DockStyle.Fill;
            _queryBuildDataGridView.Name = "queryBuildDataGridView";
			_queryBuildDataGridView.AutoGenerateColumns = false;
			_queryBuildDataGridView.AllowUserToAddRows = false;
			_queryBuildDataGridView.AllowUserToDeleteRows = false;
			_queryBuildDataGridView.AllowUserToOrderColumns = false;
			_queryBuildDataGridView.AllowUserToResizeColumns = true;
			_queryBuildDataGridView.MultiSelect = false;
			_queryBuildDataGridView.ReadOnly = true;
			_queryBuildDataGridView.ScrollBars = ScrollBars.Vertical;
			_queryBuildDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            _queryBuildDataGridView.TabIndex = 16;
			
			#region Columns
			
			#region queryBuildOrderTextColumn
			
			_queryBuildOrderTextColumn = new DataGridViewTextBoxColumn();
			_queryBuildOrderTextColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			_queryBuildOrderTextColumn.DataPropertyName = "Order";
			_queryBuildOrderTextColumn.HeaderText = "Order";
			_queryBuildOrderTextColumn.Name = "queryBuildOrderTextColumn";
			_queryBuildOrderTextColumn.ReadOnly = true;
			_queryBuildOrderTextColumn.SortMode = DataGridViewColumnSortMode.Automatic;
			
			#endregion
			
			#region queryBuildKeyTextColumn
			
			_queryBuildKeyTextColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			_queryBuildKeyTextColumn.FillWeight = 25F;
			_queryBuildKeyTextColumn.DataPropertyName = "Key";
			_queryBuildKeyTextColumn.HeaderText = "Key";
			_queryBuildKeyTextColumn.Name = "queryBuildKeyTextColumn";
			_queryBuildKeyTextColumn.ReadOnly = true;
			_queryBuildKeyTextColumn.SortMode = DataGridViewColumnSortMode.Automatic;
			
			#endregion
			
			#region queryBuildValueTextColumn
			
			_queryBuildValueTextColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			_queryBuildValueTextColumn.FillWeight = 75F;
			_queryBuildValueTextColumn.DataPropertyName = "Value";
			_queryBuildValueTextColumn.HeaderText = "Value";
			_queryBuildValueTextColumn.Name = "queryBuildValueTextColumn";
			_queryBuildValueTextColumn.ReadOnly = true;
			_queryBuildValueTextColumn.SortMode = DataGridViewColumnSortMode.Automatic;
			
			#endregion
			
			#region queryBuildHasValueColumn
			
			_queryBuildHasValueColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			_queryBuildHasValueColumn.DataPropertyName = "HasValue";
			_queryBuildHasValueColumn.HeaderText = "Has Value";
			_queryBuildHasValueColumn.Name = "queryBuildHasValueColumn";
			_queryBuildHasValueColumn.ReadOnly = true;
			_queryBuildHasValueColumn.SortMode = DataGridViewColumnSortMode.Automatic;
			
			#endregion
			
			_queryBuildDataGridView.Columns.Add(_queryBuildOrderTextColumn);
			_queryBuildDataGridView.Columns.Add(_queryBuildKeyTextColumn);
			_queryBuildDataGridView.Columns.Add(_queryBuildValueTextColumn);
			_queryBuildDataGridView.Columns.Add(_queryBuildHasValueColumn);
			_queryBuildDataGridView.DataSource = new DataView(_queryDataTable);
			_queryBuildDataGridView.ColumnCount = 4;
			
			#endregion

			#endregion

            #region queryBuildDetailTableLayoutPanel
            
            queryBuildDetailTableLayoutPanel.AutoSize = true;
            queryBuildDetailTableLayoutPanel.ColumnCount = 2;
            queryBuildDetailTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            queryBuildDetailTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            queryBuildDetailTableLayoutPanel.Controls.Add(_queryBuildOrderLabel, 0, 0);
            queryBuildDetailTableLayoutPanel.Controls.Add(_queryBuildOrderTextBox, 1, 0);
            queryBuildDetailTableLayoutPanel.Controls.Add(queryBuildKeyLabel, 0, 1);
            queryBuildDetailTableLayoutPanel.Controls.Add(_queryBuildKeyTextBox, 1, 1);
            queryBuildDetailTableLayoutPanel.Controls.Add(queryBuildValueLabel, 0, 2);
            queryBuildDetailTableLayoutPanel.Controls.Add(_queryBuildValueTextBox, 1, 2);
            queryBuildDetailTableLayoutPanel.Controls.Add(_queryBuildHasValueCheckBox, 0, 3);
            queryBuildDetailTableLayoutPanel.Controls.Add(queryFlowLayoutPanel, 0, 4);
            queryBuildDetailTableLayoutPanel.Dock = DockStyle.Fill;
            queryBuildDetailTableLayoutPanel.Name = "queryBuildDetailTableLayoutPanel";
            queryBuildDetailTableLayoutPanel.RowCount = 5;
            queryBuildDetailTableLayoutPanel.RowStyles.Add(new RowStyle());
            queryBuildDetailTableLayoutPanel.RowStyles.Add(new RowStyle());
            queryBuildDetailTableLayoutPanel.RowStyles.Add(new RowStyle());
            queryBuildDetailTableLayoutPanel.RowStyles.Add(new RowStyle());
            queryBuildDetailTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            queryBuildDetailTableLayoutPanel.TabStop = false;
            
			#endregion

            #region queryBuildOrderLabel
            
            _queryBuildOrderLabel.AutoSize = true;
            buildButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _queryBuildOrderLabel.Name = "queryBuildOrderLabel";
            _queryBuildOrderLabel.TabStop = false;
            _queryBuildOrderLabel.Text = "Order:";
            _queryBuildOrderLabel.Visible = false;
            
			#endregion

            #region queryBuildOrderTextBox
            
            _queryBuildOrderTextBox.Dock = DockStyle.Top;
            _queryBuildOrderTextBox.Name = "queryBuildOrderTextBox";
            _queryBuildOrderTextBox.TabIndex = 10;
            _queryBuildOrderTextBox.ReadOnly = true;
            _queryBuildOrderTextBox.Visible = false;
            
			#endregion

            #region queryBuildKeyLabel
            
            queryBuildKeyLabel.AutoSize = true;
            buildButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            queryBuildKeyLabel.Name = "queryBuildKeyLabel";
            queryBuildKeyLabel.TabStop = false;
            queryBuildKeyLabel.Text = "Key:";
            
			#endregion

            #region queryBuildKeyTextBox
            
            _queryBuildKeyTextBox.Dock = DockStyle.Top;
            _queryBuildKeyTextBox.Name = "queryBuildKeyTextBox";
            _queryBuildKeyTextBox.TabIndex = 10;
            
			#endregion

            #region queryBuildValueLabel
            
            queryBuildValueLabel.AutoSize = true;
            buildButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            queryBuildValueLabel.Name = "queryBuildValueLabel";
            queryBuildValueLabel.TabStop = false;
            queryBuildValueLabel.Text = "Key:";
            
			#endregion

            #region queryBuildValueTextBox
            
            _queryBuildValueTextBox.Dock = DockStyle.Top;
            _queryBuildValueTextBox.Name = "queryBuildValueTextBox";
            _queryBuildValueTextBox.TabIndex = 10;
            
			#endregion

            #region queryBuildHasValueCheckBox
            
            _queryBuildHasValueCheckBox.AutoSize = true;
            queryBuildDetailTableLayoutPanel.SetColumnSpan(_queryBuildHasValueCheckBox, 2);
            _queryBuildHasValueCheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _queryBuildHasValueCheckBox.Name = "queryBuildHasValueCheckBox";
            _queryBuildHasValueCheckBox.TabIndex = 14;
            _queryBuildHasValueCheckBox.Text = "Has Value";
            _queryBuildHasValueCheckBox.UseVisualStyleBackColor = true;
            
			#endregion

            #region queryFlowLayoutPanel
            
            queryFlowLayoutPanel.AutoSize = true;
            queryBuildDetailTableLayoutPanel.SetColumnSpan(queryFlowLayoutPanel, 2);
            queryFlowLayoutPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            queryFlowLayoutPanel.Name = "queryFlowLayoutPanel";
            queryFlowLayoutPanel.TabStop = false;
            queryFlowLayoutPanel.Controls.Add(_querySaveButton);
            queryFlowLayoutPanel.Controls.Add(_queryMoveUpButton);
            queryFlowLayoutPanel.Controls.Add(_queryMoveDownButton);
            queryFlowLayoutPanel.Controls.Add(_queryAddButton);
            queryFlowLayoutPanel.Controls.Add(_queryInsertButton);
			queryFlowLayoutPanel.Controls.Add(_queryCancelButton);
            queryFlowLayoutPanel.Controls.Add(_queryDeleteButton);
			
			#endregion

            #region querySaveButton
            
            _querySaveButton.Name = "querySaveButton";
            _querySaveButton.Size = new Size(75, 23);
            _querySaveButton.TabIndex = 3;
            _querySaveButton.Text = "Save";
            _querySaveButton.UseVisualStyleBackColor = true;
            _querySaveButton.Visible = false;
            
			#endregion

            #region queryMoveUpButton
            
            _queryMoveUpButton.Name = "queryMoveUpButton";
            _queryMoveUpButton.Size = new Size(75, 23);
            _queryMoveUpButton.TabIndex = 3;
            _queryMoveUpButton.Text = "Move Up";
            _queryMoveUpButton.UseVisualStyleBackColor = true;
            _queryMoveUpButton.Visible = false;
            
			#endregion

            #region queryMoveDownButton
            
            _queryMoveDownButton.Name = "queryMoveDownButton";
            _queryMoveDownButton.Size = new Size(75, 23);
            _queryMoveDownButton.TabIndex = 3;
            _queryMoveDownButton.Text = "Move Down";
            _queryMoveDownButton.UseVisualStyleBackColor = true;
            _queryMoveDownButton.Visible = false;
            
			#endregion

            #region queryAddButton
            
            _queryAddButton.Name = "queryAddButton";
            _queryAddButton.Size = new Size(75, 23);
            _queryAddButton.TabIndex = 3;
            _queryAddButton.Text = "Add";
            _queryAddButton.UseVisualStyleBackColor = true;
            
			#endregion

            #region queryInsertButton
            
            _queryInsertButton.Name = "queryInsertButton";
            _queryInsertButton.Size = new Size(75, 23);
            _queryInsertButton.TabIndex = 3;
            _queryInsertButton.Text = "Insert";
            _queryInsertButton.UseVisualStyleBackColor = true;
            
			#endregion

            #region queryCancelButton
            
            _queryCancelButton.Name = "queryCancelButton";
            _queryCancelButton.Size = new Size(75, 23);
            _queryCancelButton.TabIndex = 3;
            _queryCancelButton.Text = "Cancel";
            _queryCancelButton.UseVisualStyleBackColor = true;
            _queryCancelButton.Visible = false;
            
			#endregion

            #region queryDeleteButton
            
            _queryDeleteButton.Name = "queryDeleteButton";
            _queryDeleteButton.Size = new Size(75, 23);
            _queryDeleteButton.TabIndex = 3;
            _queryDeleteButton.Text = "Delete";
            _queryDeleteButton.UseVisualStyleBackColor = true;
            _queryDeleteButton.Visible = false;
            
			#endregion

			#endregion
			
            #region fragmentCheckBox
            
            _fragmentCheckBox.AutoSize = true;
            builderTableLayoutPanel.SetColumnSpan(_fragmentCheckBox, 9);
            _fragmentCheckBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _fragmentCheckBox.Name = "fragmentCheckBox";
            _fragmentCheckBox.TabIndex = 15;
            _fragmentCheckBox.Text = "Fragment";
            _fragmentCheckBox.UseVisualStyleBackColor = true;
            
			#endregion

            #region fragmentBuildTextBox
            
            builderTableLayoutPanel.SetColumnSpan(_fragmentBuildTextBox, 9);
            _fragmentBuildTextBox.Dock = DockStyle.Top;
            _fragmentBuildTextBox.Name = "fragmentBuildTextBox";
            _fragmentBuildTextBox.TabIndex = 17;
            _fragmentBuildTextBox.Visible = false;
			
			#endregion

            #region buildButton
            
            builderTableLayoutPanel.SetColumnSpan(buildButton, 9);
            buildButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buildButton.Name = "buildButton";
            buildButton.Size = new Size(75, 23);
            buildButton.TabIndex = 3;
            buildButton.Text = "Build";
            buildButton.UseVisualStyleBackColor = true;
            
			#endregion

            #region buildErrorLabel
            
            _buildErrorLabel.AutoSize = true;
            builderTableLayoutPanel.SetColumnSpan(_buildErrorLabel, 9);
            _fragmentCheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _buildErrorLabel.Name = "buildErrorLabel";
            _buildErrorLabel.TabStop = false;
            _buildErrorLabel.Visible = false;
			
			#endregion

			#endregion

            #region componentsTabPage

            componentsTabPage.Controls.Add(componentsTableLayoutPanel);
            componentsTabPage.Name = "componentsTabPage";
            componentsTabPage.Padding = new Padding(3);
            componentsTabPage.TabIndex = 2;
            componentsTabPage.Text = "Components";
            componentsTabPage.UseVisualStyleBackColor = true;
            
            #region componentsTableLayoutPanel
            
            componentsTableLayoutPanel.AutoSize = true;
            componentsTableLayoutPanel.ColumnCount = 2;
            componentsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            componentsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            componentsTableLayoutPanel.Controls.Add(schemeComponentLabel, 0, 0);
            componentsTableLayoutPanel.Controls.Add(_schemeTextBox, 1, 0);
            componentsTableLayoutPanel.Controls.Add(userNameLabel, 2, 0);
            componentsTableLayoutPanel.Controls.Add(_userNameComponentTextBox, 3, 0);
            componentsTableLayoutPanel.Controls.Add(_userNameEmptyLabel, 3, 0);
            componentsTableLayoutPanel.Controls.Add(passwordLabel, 4, 0);
            componentsTableLayoutPanel.Controls.Add(_passwordComponentTextBox, 5, 0);
            componentsTableLayoutPanel.Controls.Add(_passwordEmptyLabel, 5, 0);
            componentsTableLayoutPanel.Controls.Add(hostComponentLabel, 6, 0);
            componentsTableLayoutPanel.Controls.Add(_hostComponentTextBox, 7, 0);
            componentsTableLayoutPanel.Controls.Add(portLabel, 8, 0);
            componentsTableLayoutPanel.Controls.Add(_portComponentTextBox, 9, 0);
            componentsTableLayoutPanel.Controls.Add(_portDefinedLabel, 10, 0);
            componentsTableLayoutPanel.Controls.Add(segmentsLabel, 0, 1);
            componentsTableLayoutPanel.Controls.Add(_segmentsDataGridView, 0, 2);
            componentsTableLayoutPanel.Controls.Add(_pathEmptyLabel, 0, 2);
            componentsTableLayoutPanel.Controls.Add(queryLabel, 0, 3);
            componentsTableLayoutPanel.Controls.Add(_queryComponentDataGridView, 0, 4);
            componentsTableLayoutPanel.Dock = DockStyle.Fill;
            componentsTableLayoutPanel.Name = "componentsTableLayoutPanel";
            componentsTableLayoutPanel.RowCount = 2;
            componentsTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            componentsTableLayoutPanel.RowStyles.Add(new RowStyle());
            componentsTableLayoutPanel.TabStop = false;
            
			#endregion

            #region schemeComponentLabel
            
            schemeComponentLabel.AutoSize = true;
            schemeComponentLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            schemeComponentLabel.Name = "schemeComponentLabel";
            schemeComponentLabel.TabStop = false;
            schemeComponentLabel.Text = "Scheme:";
            
			#endregion

            #region schemeTextBox
            
            _schemeTextBox.Dock = DockStyle.Top;
            _schemeTextBox.Name = "schemeTextBox";
            _schemeTextBox.TabIndex = 17;
			_schemeTextBox.ReadOnly = true;
            
			#endregion

            #region userNameLabel
            
            userNameLabel.AutoSize = true;
            userNameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            userNameLabel.Name = "userNameLabel";
            userNameLabel.TabStop = false;
            userNameLabel.Text = "User Name:";
            
			#endregion

            #region userNameComponentTextBox
            
            _userNameComponentTextBox.Dock = DockStyle.Top;
            _userNameComponentTextBox.Name = "userNameComponentTextBox";
            _userNameComponentTextBox.TabIndex = 17;
            _userNameComponentTextBox.Visible = false;
			_userNameComponentTextBox.ReadOnly = true;
            
			#endregion

            #region userNameEmptyLabel
            
            _userNameEmptyLabel.AutoSize = true;
            _userNameEmptyLabel.Anchor = AnchorStyles.Top | AnchorStyles.left;
            _userNameEmptyLabel.Name = "userNameEmptyLabel";
            _userNameEmptyLabel.TabStop = false;
            _userNameEmptyLabel.Text = "(not specified)";
            
			#endregion

            #region passwordLabel
            
            passwordLabel.AutoSize = true;
            passwordLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            passwordLabel.Name = "passwordLabel";
            passwordLabel.TabStop = false;
            passwordLabel.Text = "Password:";
            
			#endregion

            #region passwordComponentTextBox
            
            _passwordComponentTextBox.Dock = DockStyle.Top;
            _passwordComponentTextBox.Name = "passwordComponentTextBox";
            _passwordComponentTextBox.TabIndex = 17;
            _passwordComponentTextBox.Visible = false;
			_passwordComponentTextBox.ReadOnly = true;
            
			#endregion

            #region passwordEmptyLabel
            
            _passwordEmptyLabel.AutoSize = true;
            _passwordEmptyLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _passwordEmptyLabel.Name = "passwordEmptyLabel";
            _passwordEmptyLabel.TabStop = false;
            _passwordEmptyLabel.Text = "(not specified)";
            
			#endregion

            #region hostComponentLabel
            
            hostComponentLabel.AutoSize = true;
            hostComponentLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            hostComponentLabel.Name = "hostComponentLabel";
            hostComponentLabel.TabStop = false;
            hostComponentLabel.Text = "Host:";
			hostComponentLabel.ReadOnly = true;
            
			#endregion

            #region hostComponentTextBox
            
            _hostComponentTextBox.Dock = DockStyle.Top;
            _hostComponentTextBox.Name = "hostComponentTextBox";
            _hostComponentTextBox.TabIndex = 17;
            
			#endregion

            #region portLabel
            
            portLabel.AutoSize = true;
            portLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            portLabel.Name = "portLabel";
            portLabel.TabStop = false;
            portLabel.Text = "Port:";
            
			#endregion

            #region portComponentTextBox
            
            _portComponentTextBox.Dock = DockStyle.Top;
            _portComponentTextBox.Name = "portComponentTextBox";
            _portComponentTextBox.TabIndex = 17;
			_portComponentTextBox.ReadOnly = true;
            
			#endregion

            #region portDefinedLabel
            
            _portDefinedLabel.AutoSize = true;
            _portDefinedLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _portDefinedLabel.Name = "portDefinedLabel";
            _portDefinedLabel.TabStop = false;
            _portDefinedLabel.Text = "(using default)";
            
			#endregion

            #region segmentsLabel
            
            componentsTableLayoutPanel.SetColumnSpan(segmentsLabel, 9);
            segmentsLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            segmentsLabel.AutoSize = true;
            segmentsLabel.Name = "segmentsLabel";
            segmentsLabel.TabStop = false;
            segmentsLabel.Text = "Segments";
			
			#endregion

            #region segmentsDataGridView
            
            _segmentsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            componentsTableLayoutPanel.SetColumnSpan(_segmentsDataGridView, 9);
            _segmentsDataGridView.Dock = DockStyle.Fill;
            _segmentsDataGridView.Name = "segmentsDataGridView";
			_segmentsDataGridView.AutoGenerateColumns = false;
			_segmentsDataGridView.AllowUserToAddRows = false;
			_segmentsDataGridView.AllowUserToDeleteRows = false;
			_segmentsDataGridView.AllowUserToOrderColumns = false;
			_segmentsDataGridView.AllowUserToResizeColumns = true;
			_segmentsDataGridView.MultiSelect = true;
			_segmentsDataGridView.ReadOnly = true;
			_segmentsDataGridView.ScrollBars = ScrollBars.Vertical;
			_segmentsDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            _segmentsDataGridView.TabIndex = 16;
            _segmentsDataGridView.Visible = false;
			
			#region Columns
			
			#region segmentsOrderTextColumn
			
			segmentsOrderTextColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			segmentsOrderTextColumn.DataPropertyName = "Order";
			segmentsOrderTextColumn.HeaderText = "Order";
			segmentsOrderTextColumn.Name = "segmentsOrderTextColumn";
			segmentsOrderTextColumn.ReadOnly = true;
			segmentsOrderTextColumn.SortMode = DataGridViewColumnSortMode.Automatic;
			
			#endregion
			
			#region segmentsNameTextColumn
			
			segmentsNameTextColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			segmentsNameTextColumn.FillWeight = 25F;
			segmentsNameTextColumn.DataPropertyName = "Name";
			segmentsNameTextColumn.HeaderText = "Name";
			segmentsNameTextColumn.Name = "segmentsNameTextColumn";
			segmentsNameTextColumn.ReadOnly = true;
			segmentsNameTextColumn.SortMode = DataGridViewColumnSortMode.Automatic;
			
			#endregion
			
			_queryBuildDataGridView.Columns.Add(segmentsOrderTextColumn);
			_queryBuildDataGridView.Columns.Add(segmentsNameTextColumn);
			_segmentsDataGridView.ColumnCount = 2;
			_queryBuildDataGridView.DataSource = new DataView(_segmentsDataTable);
			
			#endregion

			#endregion

            #region pathEmptyLabel
            
            componentsTableLayoutPanel.SetColumnSpan(_pathEmptyLabel, 9);
            _pathEmptyLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _pathEmptyLabel.AutoSize = true;
            _pathEmptyLabel.Name = "pathEmptyLabel";
            _pathEmptyLabel.TabStop = false;
            _pathEmptyLabel.Text = "Path is empty";
			
			#endregion

            #region queryLabel
            
            componentsTableLayoutPanel.SetColumnSpan(queryLabel, 9);
            queryLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            queryLabel.AutoSize = true;
            queryLabel.Name = "queryLabel";
            queryLabel.TabStop = false;
            queryLabel.Text = "Query";
			
			#endregion

            #region queryComponentDataGridView
            
            _queryComponentDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _queryComponentDataGridView.Dock = DockStyle.Fill;
            _queryComponentDataGridView.Name = "queryComponentDataGridView";
			_queryComponentDataGridView.AutoGenerateColumns = false;
			_queryComponentDataGridView.AllowUserToAddRows = false;
			_queryComponentDataGridView.AllowUserToDeleteRows = false;
			_queryComponentDataGridView.AllowUserToOrderColumns = false;
			_queryComponentDataGridView.AllowUserToResizeColumns = true;
			_queryComponentDataGridView.MultiSelect = true;
			_queryComponentDataGridView.ReadOnly = true;
			_queryComponentDataGridView.ScrollBars = ScrollBars.Vertical;
			_queryComponentDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            _queryComponentDataGridView.TabIndex = 16;
            _queryComponentDataGridView.Visible = false;
			
			#region Columns
			
			#region queryComponentOrderTextColumn
			
			queryComponentOrderTextColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			queryComponentOrderTextColumn.DataPropertyName = "Order";
			queryComponentOrderTextColumn.HeaderText = "Order";
			queryComponentOrderTextColumn.Name = "queryComponentOrderTextColumn";
			queryComponentOrderTextColumn.ReadOnly = true;
			queryComponentOrderTextColumn.SortMode = DataGridViewColumnSortMode.Automatic;
			
			#endregion
			
			#region queryComponentKeyTextColumn
			
			queryComponentKeyTextColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			queryComponentKeyTextColumn.FillWeight = 25F;
			queryComponentKeyTextColumn.DataPropertyName = "Key";
			queryComponentKeyTextColumn.HeaderText = "Key";
			queryComponentKeyTextColumn.Name = "queryComponentKeyTextColumn";
			queryComponentKeyTextColumn.ReadOnly = true;
			queryComponentKeyTextColumn.SortMode = DataGridViewColumnSortMode.Automatic;
			
			#endregion
			
			#region queryComponentValueTextColumn
			
			queryComponentValueTextColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			queryComponentValueTextColumn.FillWeight = 75F;
			queryComponentValueTextColumn.DataPropertyName = "Value";
			queryComponentValueTextColumn.HeaderText = "Value";
			queryComponentValueTextColumn.Name = "queryComponentValueTextColumn";
			queryComponentValueTextColumn.ReadOnly = true;
			queryComponentValueTextColumn.SortMode = DataGridViewColumnSortMode.Automatic;
			
			#endregion
			
			#region queryComponentHasValueColumn
			
			queryComponentHasValueColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			queryComponentHasValueColumn.DataPropertyName = "HasValue";
			queryComponentHasValueColumn.HeaderText = "Has Value";
			queryComponentHasValueColumn.Name = "queryComponentHasValueColumn";
			queryComponentHasValueColumn.ReadOnly = true;
			queryComponentHasValueColumn.SortMode = DataGridViewColumnSortMode.Automatic;
			
			#endregion
			
			_queryComponentDataGridView.Columns.Add(queryComponentOrderTextColumn);
			_queryComponentDataGridView.Columns.Add(queryComponentKeyTextColumn);
			_queryComponentDataGridView.Columns.Add(queryComponentValueTextColumn);
			_queryComponentDataGridView.Columns.Add(queryComponentHasValueColumn);
			_queryComponentDataGridView.ColumnCount = 4;
			_queryComponentDataGridView.DataSource = new DataView(_queryDataTable);
			
			#endregion

			#endregion

            #region queryEmptyLabel
            
            componentsTableLayoutPanel.SetColumnSpan(_queryEmptyLabel, 9);
            _queryEmptyLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _queryEmptyLabel.AutoSize = true;
            _queryEmptyLabel.Name = "queryEmptyLabel";
            _queryEmptyLabel.TabStop = false;
            _queryEmptyLabel.Text = "Query not defined";
			
			#endregion

            #region fragmentComponentLabel
            
            fragmentComponentLabel.AutoSize = true;
            componentsTableLayoutPanel.SetColumnSpan(fragmentComponentLabel, 9);
            fragmentComponentLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            fragmentComponentLabel.Name = "fragmentComponentLabel";
            fragmentComponentLabel.TabStop = false;
            fragmentComponentLabel.Text = "Fragment";
            
			#endregion

            #region fragmentComponentTextBox
            
            componentsTableLayoutPanel.SetColumnSpan(_fragmentComponentTextBox, 9);
            _fragmentComponentTextBox.Dock = DockStyle.Top;
            _fragmentComponentTextBox.Name = "fragmentBuildTextBox";
            _fragmentComponentTextBox.TabIndex = 17;
            _fragmentComponentTextBox.Visible = false;
            
			#endregion

            #region fragmentEmptyLabel
            
            componentsTableLayoutPanel.SetColumnSpan(_fragmentEmptyLabel, 9);
            _fragmentEmptyLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _fragmentEmptyLabel.AutoSize = true;
            _fragmentEmptyLabel.Name = "fragmentEmptyLabel";
            _fragmentEmptyLabel.TabStop = false;
            _fragmentEmptyLabel.Text = "Fragment not defined";
			
			#endregion

            #region parentDirButton
            
            componentsTableLayoutPanel.SetColumnSpan(parentDirButton, 9);
            parentDirButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            parentDirButton.Name = "parentDirButton";
            parentDirButton.Size = new Size(75, 23);
            parentDirButton.TabIndex = 3;
            parentDirButton.Text = "Parent Segment";
            parentDirButton.UseVisualStyleBackColor = true;
            parentDirButton.Enabled = false;
            
			#endregion

			#endregion

            this.Size = new Size(800, 600);
            this.Controls.Add(_mainTabControl);
            this.Name = "EditUrlForm2";
            this.Text = "Edit Url";
			
			#endregion
			
			#region End Init
			
            _mainTabControl.ResumeLayout(false);
            parsingTabPage.ResumeLayout(false);
            parsingTabPage.PerformLayout();
            builderTabPage.ResumeLayout(false);
            builderTabPage.PerformLayout();
            componentsTabPage.ResumeLayout(false);
            componentsTabPage.PerformLayout();
            builderTableLayoutPanel.ResumeLayout(false);
            builderTableLayoutPanel.PerformLayout();
            _queryBuildSplitContainer.ResumeLayout(false);
            _queryBuildSplitContainer.PerformLayout();
            queryBuildDetailTableLayoutPanel.ResumeLayout(false);
            queryBuildDetailTableLayoutPanel.PerformLayout();
            componentsTableLayoutPanel.ResumeLayout(false);
            ((ISupportInitialize)(_queryBuildDataGridView)).EndInit();
            ((ISupportInitialize)(_queryComponentDataGridView)).EndInit();
            ((ISupportInitialize)(_segmentsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
			
			#endregion
			
			#region Attach Event Handlers
			
			_mainTabControl.TabIndexChanged += mainTabControl_TabIndexChanged;
			parseButton.Click += parseButton_Click;
			_schemeCheckbox.CheckedChanged += schemeCheckbox_CheckedChanged;
			_userNameCheckBox.CheckedChanged += userNameCheckBox_CheckedChanged;
			_passwordCheckBox.CheckedChanged += passwordCheckBox_CheckedChanged;
			_portCheckBox.CheckedChanged += portCheckBox_CheckedChanged;
			_queryCheckBox.CheckedChanged += queryCheckBox_CheckedChanged;
			_queryBuildSplitContainer.Panel2.ClientSizeChanged += queryBuildSplitContainer_Panel2_ClientSizeChanged
			_queryBuildHasValueCheckBox.CheckedChanged += queryBuildHasValueCheckBox_CheckedChanged;
			_queryBuildDataGridView.SelectionChanged += queryBuildDataGridView_SelectionChanged;
			_querySaveButton.Click += querySaveButton_Click;
			_queryMoveUpButton.Click += queryMoveUpButton_Click;
			_queryMoveDownButton.Click += queryMoveDownButton_Click;
			_queryAddButton.Click += queryAddButton_Click;
			_queryInsertButton.Click += queryInsertButton_Click;
			_queryCancelButton.Click += queryCancelButton_Click;
			_queryDeleteButton.Click += queryDeleteButton_Click;
			
			_fragmentCheckBox.CheckedChanged += fragmentCheckBox_CheckedChanged;
			buildButton.Click += buildButton_Click;
			parentDirButton.Click += parentDirButton_Click;
			
			#endregion
        }
    }
	
	private bool TryParseUri()
	{
		
	}
	
	private bool TryBuildUri()
	{
		
	}
	
	private void mainTabControl_TabIndexChanged(object sender, EventArgs e)
	{
		switch (_previousTabIndex)
		{
			case 0:
				if (TryParseUri())
				{
					_previousTabIndex = _mainTabControl.TabIndex;
					return;
				}
				break;
			case 1:
				if (TryBuildUri())
				{
					_previousTabIndex = _mainTabControl.TabIndex;
					return;
				}
				break;
			case 2:
				_previousTabIndex = _mainTabControl.TabIndex;
				return;
		}
		_mainTabControl.SelectedIndex = _previousTabIndex;
		_mainTabControl.TabIndex = _previousTabIndex;
	}
	
	private void parseButton_Click(object sender, EventArgs e) { TryParseUri(); }
	
	private void schemeCheckbox_CheckedChanged(object sender, EventArgs e)
	{
		if (_schemeCheckBox.Checked)
		{
            _schemeComboBox.Visible = true;
            _schemeSeparatorLabel.Visible = true;
            _userNameCheckBox.Visible = true;
			userNameCheckBox_CheckedChanged(_userNameCheckBox, e);
            _hostBuildLabel.Visible = true;
            _hostBuildTextBox.Visible = true;
			_portCheckBox.Visible = true;
            portCheckBox_CheckedChanged(_portCheckBox, e);
		}
		else
		{
            _userNameCheckBox.Visible = false;
            _schemeComboBox.Visible = false;
            _schemeSeparatorLabel.Visible = false;
            _userNameBuildTextBox.Visible = false;
            _passwordCheckBox.Visible = false;
            _passwordSeparatorLabel.Visible = false;
			_passwordBuildTextBox.Visible = false;
            _authSeparatorLabel.Visible = false;
            _hostBuildLabel.Visible = false;
            _hostBuildTextBox.Visible = false;
			_portCheckBox.Visible = false;
            _portSeparatorLabel.Visible = false;
            _portBuildTextBox.Visible = false;
		}
	}
	
	private void userNameCheckBox_CheckedChanged(object sender, EventArgs e)
	{
		if (_userNameCheckBox.Checked)
		{
			if (!_schemeCheckBox.Checked)
				return;
            _userNameBuildTextBox.Visible = true;
            _authSeparatorLabel.Visible = true;
            _passwordCheckBox.Visible = true;
			passwordCheckBox_CheckedChanged(_passwordCheckBox, e);
		}
		else
		{
            _userNameBuildTextBox.Visible = false;
            _passwordCheckBox.Visible = false;
            _passwordSeparatorLabel.Visible = false;
			_passwordBuildTextBox.Visible = false;
            _authSeparatorLabel.Visible = false;
		}
	}
	
	private void passwordCheckBox_CheckedChanged(object sender, EventArgs e)
	{
		if (_passwordCheckBox.Checked)
		{
			if (!_userNameCheckBox.Checked)
				return;
			_passwordSeparatorLabel.Visible = true;
			_passwordBuildTextBox.Visible = true;
		}
		else
		{
			_passwordSeparatorLabel.Visible = false;
			_passwordBuildTextBox.Visible = false;
		}
	}
	
	private void portCheckBox_CheckedChanged(object sender, EventArgs e)
	{
		if (_portCheckBox.Checked)
		{
			if (!_schemeCheckBox.Checked)
				return;
            _portSeparatorLabel.Visible = true;
            _portBuildTextBox.Visible = true;
		}
		else
		{
            _portSeparatorLabel.Visible = false;
            _portBuildTextBox.Visible = false;
		}
	}
	
	private void queryCheckBox_CheckedChanged(object sender, EventArgs e)
	{
		if (_queryCheckBox.Checked)
		{
            _queryBuildSplitContainer.Visible = true;
		}
		else
		{
            _queryBuildSplitContainer.Visible = false;
		}
	}
	
	private void queryBuildSplitContainer_Panel2_ClientSizeChanged(object sender, EventArgs e)
	{
		if (_queryBuildSplitContainer.Panel2Collapsed && !_queryCancelButton.Visible)
			_queryBuildSplitContainer.Panel2Collapsed = true;
	}
	
	private void queryBuildDataGridView_SelectionChanged(object sender, EventArgs e)
	{
		DataRow row = null;
		if (_queryBuildDataGridView.SelectedCells != null && _queryBuildDataGridView.SelectedCells.Count > 0 && _queryBuildDataGridView.SelectedCells[0].OwningRow.DataBoundItem != null)
		{
			if (_queryBuildDataGridView.SelectedCells[0].OwningRow.DataBoundItem is DataRowView)
				row = ((DataRowView)(_queryBuildDataGridView.SelectedCells[0].OwningRow.DataBoundItem)).Row;
			else if (_queryBuildDataGridView.SelectedCells[0].OwningRow.DataBoundItem is DataRow)
				row = (DataRow)(_queryBuildDataGridView.SelectedCells[0].OwningRow.DataBoundItem);
		}
		int order;
		if (_editingRow != null)
		{
			if (row != null && ReferenceEquals(row, _editingRow))
				return;
			DialogResult dr;
			order = (int)(_editingRow[_queryOrderDataColumn]);
			if (order < 0)
				dr = MessageBoxShow.Show(this, "New query item has not been saved. Add to query?", "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning,
					MessageBoxDefaultButton.Button3);
			else if ((string)(_editingRow[_queryKeyDataColumn]) != _queryBuildKeyTextBox.Text || (((bool)(_editingRow[_queryHasValueDataColumn])) ? !_queryBuildHasValueCheckBox.Checked ||
					(string)(_editingRow[_queryValueDataColumn]) != _queryBuildValueTextBox.Text : _queryBuildHasValueCheckBox.Checked))
				dr = MessageBoxShow.Show(this, "Query item has not been saved. Save changes?", "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning,
					MessageBoxDefaultButton.Button3);
			else
				dr = DialogResult.No;
			switch (dr)
			{
				case DialogResult.Yes:
					_editingRow[_queryKeyDataColumn] = _queryBuildKeyTextBox.Text;
					_editingRow[_queryHasValueDataColumn] = _queryBuildHasValueCheckBox.Checked;
					_editingRow[_queryValueDataColumn] = (_queryBuildHasValueCheckBox.Checked) ? _queryBuildValueTextBox.Text : "";
					if (order < 0)
					{
						_editingRow[_queryOrderDataColumn] = _queryDataTable.Rows.Count;
						_queryDataTable.Rows.Add(_editingRow);
					}
					_editingRow.AcceptChanges();
					break;
				case DialogResult.Cancel;
					if (_queryBuildDataGridView.SelectedCells != null && _queryBuildDataGridView.SelectedCells.Count > 0)
						_queryBuildDataGridView.ClearSelection();
					if (order >= 0)
					{
						foreach (DataGridViewRow r in _queryBuildDataGridView.Rows)
						{
							object obj = r.DataBoundItem;
							if (obj == null)
									continue;
							if (obj is DataRowView)
								obj = (obj as DataRowView).Row;
							if (obj != null && obj is DataRow)
							{
								DataRow dr = (DataRow)obj;
								if ((int)(dr[_queryOrderDataColumn]) == order)
								{
									r.Selected = true;
									break;
								}
							}
						}
					}
					if (_queryBuildDataGridView.SelectedCells == null || _queryBuildDataGridView.SelectedCells.Count == 0)
					{
						_queryBuildSplitContainer.Panel2Collapsed = true;
						_editingRow = null;
					}
					return;
			}
			_queryDataTable.AcceptChanges();
		}
		
		if (row == null)
		{
			if (_queryBuildDataGridView.SelectedCells != null && _queryBuildDataGridView.SelectedCells.Count > 0)
				_queryBuildDataGridView.ClearSelection();
			_queryBuildSplitContainer.Panel2Collapsed = true;
			_editingRow = null;
			return;
		}
		_editingRow = row;
		order = (int)(row[_queryOrderDataColumn]);
		_queryBuildOrderLabel.Visible = true;
		_queryBuildOrderTextBox.Visible = true;
		_queryBuildOrderTextBox.Text = order.ToString();
		_queryBuildSplitContainer.Panel2Collapsed = false;
		_queryBuildKeyTextBox.Text = (string)(row[_queryKeyDataColumn]);
		_queryBuildHasValueCheckBox.Checked = (bool)(row[_queryHasValueDataColumn]);
		_queryBuildKeyTextBox.Text = (_queryBuildHasValueCheckBox.Checked) ? (string)(row[_queryValueDataColumn]) : "";
		_querySaveButton.Visible = true;
		_queryMoveUpButton.Visible = true;
		_queryMoveUpButton.Enabled = order > 0;
		_queryMoveDownButton.Visible = true;
		_queryMoveDownButton.Enabled = order < (_queryDataTable.Rows.Count - 1);
		_queryAddButton.Visible = false;
		_queryInsertButton.Visible = false;
		_queryDeleteButton.Visible = true;
	}
	
	private void queryBuildHasValueCheckBox_CheckedChanged(object sender, EventArgs e)
	{
		if (!_queryCheckBox.Checked)
			return;
		
		if (_queryBuildHasValueCheckBox.Checked)
		{
			_queryBuildValueTextBox.Visible = true;
		}
		else
		{
            _queryBuildValueTextBox.Visible = false;
		}
	}
	
	private void querySaveButton_Click(object sender, EventArgs e)
	{
		_editingRow[_queryKeyDataColumn] = _queryBuildKeyTextBox.Text;
		_editingRow[_queryHasValueDataColumn] = _queryBuildHasValueCheckBox.Checked;
		_editingRow[_queryValueDataColumn] = (_queryBuildHasValueCheckBox.Checked) ? _queryBuildValueTextBox.Text : "";
		_editingRow.AcceptChanges();
		_queryDataTable.AcceptChanges();
		if (_queryBuildDataGridView.SelectedCells != null && _queryBuildDataGridView.SelectedCells.Count > 0)
			_queryBuildDataGridView.ClearSelection();
		_queryBuildSplitContainer.Panel2Collapsed = true;
		_editingRow = null;
	}
	
	private void queryMoveUpButton_Click(object sender, EventArgs e)
	{
		int order = (int)(_editingRow[_queryOrderDataColumn]) - 1;
		DataRow[] dr = _queryDataTable.Select("[Order] = " + order.ToString());
		if (dr.Length == 0)
			return;
		dr[0].BeginEdit();
		try { dr[0][_queryOrderDataColumn] = _queryDataTable.Rows.Count; }
		finally { dr[0].EndEdit(); }
		dr[0].AcceptChanges();
		try
		{
			_editingRow.BeginEdit();
			try {
				_editingRow[_queryOrderDataColumn] = order;
				_editingRow[_queryKeyDataColumn] = _queryBuildKeyTextBox.Text;
				_editingRow[_queryHasValueDataColumn] = _queryBuildHasValueCheckBox.Checked;
				_editingRow[_queryValueDataColumn] = (_queryBuildHasValueCheckBox.Checked) ? _queryBuildValueTextBox.Text : "";
			}
			finally { _editingRow.EndEdit(); }
			_editingRow.AcceptChanges();
			dr[0].BeginEdit();
			try { dr[0][_queryOrderDataColumn] = order + 1; }
			catch
			{
				_editingRow.BeginEdit();
				try { _editingRow[_queryOrderDataColumn] = order + 1; }
				finally { _editingRow.EndEdit(); }
				_editingRow.AcceptChanges();
				throw;
			}
			finally { dr[0].EndEdit(); }
			dr[0].AcceptChanges();
			_queryDataTable.AcceptChanges():
		} 
		catch
		{
			dr[0].BeginEdit();
			try { dr[0][_queryOrderDataColumn] = order; }
			finally { dr[0].EndEdit(); }
			dr[0].AcceptChanges();
			throw;
		}
		_queryBuildOrderTextBox.Text = order.ToString();
		if (order == 0)
			_queryMoveUpButton.Enabled = false;
	}
	
	private void queryMoveDownButton_Click(object sender, EventArgs e)
	{
		int order = (int)(_editingRow[_queryOrderDataColumn]) + 1;
		DataRow[] dr = _queryDataTable.Select("[Order] = " + order.ToString());
		if (dr.Length == 0)
			return;
		dr[0].BeginEdit();
		try { dr[0][_queryOrderDataColumn] = _queryDataTable.Rows.Count; }
		finally { dr[0].EndEdit(); }
		dr[0].AcceptChanges();
		try
		{
			_editingRow.BeginEdit();
			try {
				_editingRow[_queryOrderDataColumn] = order;
				_editingRow[_queryKeyDataColumn] = _queryBuildKeyTextBox.Text;
				_editingRow[_queryHasValueDataColumn] = _queryBuildHasValueCheckBox.Checked;
				_editingRow[_queryValueDataColumn] = (_queryBuildHasValueCheckBox.Checked) ? _queryBuildValueTextBox.Text : "";
			}
			finally { _editingRow.EndEdit(); }
			_editingRow.AcceptChanges();
			dr[0].BeginEdit();
			try { dr[0][_queryOrderDataColumn] = order - 1; }
			catch
			{
				_editingRow.BeginEdit();
				try { _editingRow[_queryOrderDataColumn] = order - 1; }
				finally { _editingRow.EndEdit(); }
				_editingRow.AcceptChanges();
				throw;
			}
			finally { dr[0].EndEdit(); }
			dr[0].AcceptChanges();
			_queryDataTable.AcceptChanges():
		} 
		catch
		{
			dr[0].BeginEdit();
			try { dr[0][_queryOrderDataColumn] = order; }
			finally { dr[0].EndEdit(); }
			dr[0].AcceptChanges();
			throw;
		}
		_queryBuildOrderTextBox.Text = order.ToString();
		if (order == _queryDataTable.Rows.Count - 1)
			_queryMoveDownButton.Enabled = false;
	}
	
	private void queryAddButton_Click(object sender, EventArgs e)
	{
		_editingRow[_queryKeyDataColumn] = _queryBuildKeyTextBox.Text;
		_editingRow[_queryHasValueDataColumn] = _queryBuildHasValueCheckBox.Checked;
		_editingRow[_queryValueDataColumn] = (_queryBuildHasValueCheckBox.Checked) ? _queryBuildValueTextBox.Text : "";
		_editingRow[_queryOrderDataColumn] = _queryDataTable.Rows.Count;
		_queryDataTable.Rows.Add(_editingRow);
		_editingRow.AcceptChanges();
		_queryDataTable.AcceptChanges();
		if (_queryBuildDataGridView.SelectedCells != null && _queryBuildDataGridView.SelectedCells.Count > 0)
			_queryBuildDataGridView.ClearSelection();
		_queryBuildSplitContainer.Panel2Collapsed = true;
		_editingRow = null;
	}
	
	private void queryInsertButton_Click(object sender, EventArgs e)
	{
		DataRow row = null;
		if (_queryBuildDataGridView.SelectedCells != null && _queryBuildDataGridView.SelectedCells.Count > 0 && _queryBuildDataGridView.SelectedCells[0].OwningRow.DataBoundItem != null)
		{
			if (_queryBuildDataGridView.SelectedCells[0].OwningRow.DataBoundItem is DataRowView)
				row = ((DataRowView)(_queryBuildDataGridView.SelectedCells[0].OwningRow.DataBoundItem)).Row;
			else if (_queryBuildDataGridView.SelectedCells[0].OwningRow.DataBoundItem is DataRow)
				row = (DataRow)(_queryBuildDataGridView.SelectedCells[0].OwningRow.DataBoundItem);
		}
		int order = (row == null) ? _queryDataTable.Rows.Count : (int)(row[_queryOrderDataColumn]);
		_editingRow[_queryOrderDataColumn] = order;
		
		if (order < _queryDataTable.Rows.Count)
		{
			foreach (DataRow dr in _queryDataTable.Select("[Order] >= " + order.ToString(), "Order DESC"))
			{
				dr.BeginEdit();
				try { dr[_queryOrderDataColumn] = (int)(dr[_queryOrderDataColumn]) + 1; }
				finally { dr.EndEdit(); }
				dr.AcceptChanges();
			}
			_queryDataTable.Rows.Insert(order, row);
		}
		else
			_queryDataTable.Rows.Add(row);
		row.AcceptChanges();
		_queryDataTable.AcceptChanges();
		
		_queryBuildDataGridView.ClearSelection();
		_queryBuildOrderTextBox.Text = "-1";
		_queryBuildKeyTextBox.Text = "";
		_queryBuildValueTextBox.Text = "";
		_queryBuildHasValueCheckBox.Checked = true;
		_queryBuildSplitContainer.Panel2Collapsed = true;
		_editingRow = null;
	}
	
	private void queryCancelButton_Click(object sender, EventArgs e)
	{
		if (_queryBuildDataGridView.SelectedCells != null && _queryBuildDataGridView.SelectedCells.Count > 0)
			_queryBuildDataGridView.ClearSelection();
		_queryBuildOrderTextBox.Text = "-1";
		_queryBuildKeyTextBox.Text = "";
		_queryBuildValueTextBox.Text = "";
		_queryBuildHasValueCheckBox.Checked = true;
		_queryBuildSplitContainer.Panel2Collapsed = true;
		_editingRow = null;
	}
	
	private void queryDeleteButton_Click(object sender, EventArgs e)
	{
		int order = (int)(_editingRow[_queryOrderDataColumn]);
		_queryBuildDataGridView.ClearSelection();
		_editingRow.Delete();
		_editingRow.AcceptChanges();
		_editingRow = null;
		foreach (DataRow dr in _queryDataTable.Select("[Order] > " + index.ToString(), "Order ASC"))
		{
			dr.BeginEdit();
			try { dr[_queryOrderDataColumn] = (int)(dr[_queryOrderDataColumn]) - 1; }
			finally { dr.EndEdit(); }
			dr.AcceptChanges();
		}
		_queryDataTable.AcceptChanges();
		_queryCancelButton.Visible = false;
		_queryBuildOrderTextBox.Text = "-1";
		_queryBuildKeyTextBox.Text = "";
		_queryBuildValueTextBox.Text = "";
		_queryBuildHasValueCheckBox.Checked = true;
		_queryBuildSplitContainer.Panel2Collapsed = true;
	}
			
	private void fragmentCheckBox_CheckedChanged(object sender, EventArgs e)
	{
		if (_fragmentCheckBox.Checked)
		{
            _fragmentBuildTextBox.Visible = true;
		}
		else
		{
            _fragmentBuildTextBox.Visible = false;
		}
	}
	
	private void buildButton_Click(object sender, EventArgs e) { TryBuildUri(); }
	
	private void parentDirButton_Click(object sender, EventArgs e)
	{
		
	}
}
