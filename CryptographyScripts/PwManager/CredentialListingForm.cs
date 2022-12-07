using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace PwManager
{
    public class CredentialListingForm : Form
    {
        private Guid? _selectedId = null;
        private IContainer _components = new Container();
        private TableLayoutPanel _outerTableLayoutPanel = new TableLayoutPanel();
        private DataGridView _credentialsDataGridView = new DataGridView();
        private DataGridViewTextBoxColumn _idDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn _loginDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
        private DataGridViewTextBoxColumn _urlDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
        private DataGridViewButtonColumn _openDataGridViewButtonColumn = new DataGridViewButtonColumn();
        private DataGridViewButtonColumn _deleteDataGridViewButtonColumn = new DataGridViewButtonColumn();
        private Button _newButton = new Button();
        private Button _exitButton = new Button();

        public Guid? SelectedId { get { return _selectedId; } }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components.Dispose();
            base.Dispose(disposing);
        }

        public CredentialListingForm(Collection<CredentialItem> credentials)
        {
            // 
            // outerTableLayoutPanel
            // 
            _outerTableLayoutPanel.AutoSize = true;
            _outerTableLayoutPanel.ColumnCount = 2;
            _outerTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Auto));
            _outerTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _outerTableLayoutPanel.Dock = DockStyle.Fill;
            _outerTableLayoutPanel.Margin = new Padding(0);
            _outerTableLayoutPanel.Name = "outerTableLayoutPanel";
            _outerTableLayoutPanel.Padding = new Padding(0);
            _outerTableLayoutPanel.RowCount = 2;
            _outerTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _outerTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Auto));
            // 
            // credentialsDataGridView
            //
            _credentialsDataGridView.Name = "credentialsDataGridView";
            _credentialsDataGridView.AutoGenerateColumns = false;
            _credentialsDataGridView.AutoSize = true;
            _credentialsDataGridView.ColumnHeadersVisible = true;
            _credentialsDataGridView.DataSource = credentials;
            _credentialsDataGridView.Dock = DockStyle.Fill;
            _credentialsDataGridView.TabIndex = 0;
            _outerTableLayoutPanel.Controls.Add(_credentialsDataGridView, 0, 0);
            _outerTableLayoutPanel.SetColSpan(_credentialsDataGridView, 2);
            _credentialsDataGridView.CellClick += new DataGridViewCellEventHandler(credentialsDataGridView_CellClick);
            // 
            // idDataGridViewTextBoxColumn
            //
            _idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            _idDataGridViewTextBoxColumn.DataPropertyName = "ID";
            _idDataGridViewTextBoxColumn.ReadOnly = true;
            _idDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            _idDataGridViewTextBoxColumn.Visible = false;
            _credentialsDataGridView.Columns.Add(_idDataGridViewTextBoxColumn);
            // 
            // loginDataGridViewTextBoxColumn
            //
            _loginDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _loginDataGridViewTextBoxColumn.DataPropertyName = "Login";
            _loginDataGridViewTextBoxColumn.HeaderText = "Login";
            _loginDataGridViewTextBoxColumn.Name = "loginDataGridViewTextBoxColumn";
            _loginDataGridViewTextBoxColumn.ReadOnly = true;
            _loginDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            _credentialsDataGridView.Columns.Add(_loginDataGridViewTextBoxColumn);
            // 
            // urlDataGridViewTextBoxColumn
            //
            _urlDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            _urlDataGridViewTextBoxColumn.DataPropertyName = "URL";
            _urlDataGridViewTextBoxColumn.HeaderText = "URL";
            _urlDataGridViewTextBoxColumn.Name = "urlDataGridViewTextBoxColumn";
            _urlDataGridViewTextBoxColumn.ReadOnly = true;
            _urlDataGridViewTextBoxColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            _credentialsDataGridView.Columns.Add(_urlDataGridViewTextBoxColumn);
            // 
            // openDataGridViewButtonColumn
            //
            _openDataGridViewButtonColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _openDataGridViewButtonColumn.HeaderText = "";
            _openDataGridViewButtonColumn.Name = "openDataGridViewButtonColumn";
            _openDataGridViewButtonColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            _openDataGridViewButtonColumn.Text = "Open";
            _openDataGridViewButtonColumn.UseColumnTextForButtonValue = true;
            _credentialsDataGridView.Columns.Add(_openDataGridViewButtonColumn);
            // 
            // deleteDataGridViewButtonColumn
            //
            _deleteDataGridViewButtonColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _deleteDataGridViewButtonColumn.HeaderText = "";
            _deleteDataGridViewButtonColumn.Name = "deleteDataGridViewButtonColumn";
            _deleteDataGridViewButtonColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            _deleteDataGridViewButtonColumn.Text = "Delete";
            _deleteDataGridViewButtonColumn.UseColumnTextForButtonValue = true;
            _credentialsDataGridView.Columns.Add(_deleteDataGridViewButtonColumn);
            // 
            // newButton
            // 
            AcceptButton = new Button();
            _outerTableLayoutPanel.Controls.Add(AcceptButton, 0, 1);
            AcceptButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            AcceptButton.DialogResult = DialogResult.OK;
            AcceptButton.Name = "newButton";
            AcceptButton.Size = new Size(75, 23);
            AcceptButton.TabIndex = 1;
            AcceptButton.Text = "New";
            AcceptButton.UseVisualStyleBackColor = true;
            // 
            // exitButton
            // 
            CancelButton = new Button();
            _outerTableLayoutPanel.Controls.Add(CancelButton, 0, 2);
            CancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            AcceptButton.DialogResult = DialogResult.Cancel;
            CancelButton.Name = "exitButton";
            CancelButton.Size = new Size(75, 23);
            CancelButton.TabIndex = 2;
            CancelButton.Text = "Exit";
            CancelButton.UseVisualStyleBackColor = true;
            // 
            // CredentialListingForm
            // 
            ClientSize = new Size(800, 600);
            Controls.Add(_outerTableLayoutPanel);
            Name = "CredentialListingForm";
            Text = "Credential Listing";
            _outerTableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        void credentialsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex != _openDataGridViewButtonColumn.Index)
            {
                _selectedId = (Guid)[_credentialsDataGridView[_idDataGridViewTextBoxColumn.Index, e.RowIndex]];
                DialogResult = DialogResult.Yes;
            }
            else if (e.ColumnIndex == _deleteDataGridViewButtonColumn.Index)
            {
                _selectedId = (Guid)[_credentialsDataGridView[_idDataGridViewTextBoxColumn.Index, e.RowIndex]];
                DialogResult = DialogResult.No;
            }
        }
    }
}