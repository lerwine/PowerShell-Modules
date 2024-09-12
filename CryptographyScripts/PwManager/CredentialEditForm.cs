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
    public class CredentialEditForm : Form
    {
        private CredentialItem _credential;
        private IContainer _components = new Container();
        private TableLayoutPanel _outerTableLayoutPanel = new TableLayoutPanel();
        private Label _loginLabel = new Label();
        private TextBox _loginTextBox = new TextBox();
        private Label _passwordLabel = new Label();
        private TextBox _passwordTextBox = new TextBox();
        private Label _confirmLabel = new Label();
        private TextBox _confirmTextBox = new TextBox();
        private Label _validationLabel = new Label();
        private Label _urlLabel = new Label();
        private TextBox _urlTextBox = new TextBox();
        private Label _notesLabel = new Label();
        private TextBox _notesTextBox = new TextBox();
        private TableLayoutPanel _buttonsTableLayoutPanel = new TableLayoutPanel();
        private Button _copyPwButton = new Button();
        private Button _deleteButton = new Button();

        public CredentialEditForm(CredentialItem credential)
        {
            if ((_credential = credential) == null)
                throw new ArgumentNullException("credential");
            // 
            // outerTableLayoutPanel
            // 
            _outerTableLayoutPanel.AutoSize = true;
            _outerTableLayoutPanel.ColumnCount = 4;
            _outerTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _outerTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _outerTableLayoutPanel.Dock = DockStyle.Fill;
            _outerTableLayoutPanel.Margin = new Padding(0);
            _outerTableLayoutPanel.Name = "outerTableLayoutPanel";
            _outerTableLayoutPanel.Padding = new Padding(0);
            _outerTableLayoutPanel.RowCount = 10;
            _outerTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Auto));
            _outerTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Auto));
            _outerTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Auto));
            _outerTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Auto));
            _outerTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Auto));
            _outerTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Auto));
            _outerTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Auto));
            _outerTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Auto));
            _outerTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _outerTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Auto));
            // 
            // loginLabel
            // 
            _outerTableLayoutPanel.Controls.Add(_loginLabel, 0, 0);
            _outerTableLayoutPanel.SetColSpan(_loginLabel, 2);
            _loginLabel.Name = "loginLabel";
            _loginLabel.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            _loginLabel.AutoSize = true;
            // 
            // loginTextBox
            // 
            _outerTableLayoutPanel.Controls.Add(_loginTextBox, 0, 1);
            _outerTableLayoutPanel.SetColSpan(_loginTextBox, 2);
            _loginTextBox.Name = "loginTextBox";
            _loginTextBox.Dock = DockStyle.Top;
            _loginTextBox.AutoSize = true;
            // 
            // passwordLabel
            // 
            _outerTableLayoutPanel.Controls.Add(_passwordLabel, 0, 2);
            _passwordLabel.Name = "passwordLabel";
            _passwordLabel.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            _passwordLabel.AutoSize = true;
            // 
            // passwordTextBox
            // 
            _outerTableLayoutPanel.Controls.Add(_passwordTextBox, 0, 3);
            _passwordTextBox.Name = "passwordTextBox";
            _passwordTextBox.Dock = DockStyle.Top;
            _passwordTextBox.AutoSize = true;
            // 
            // confirmLabel
            // 
            _outerTableLayoutPanel.Controls.Add(_confirmLabel, 1, 2);
            _confirmLabel.Name = "confirmLabel";
            _confirmLabel.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            _confirmLabel.AutoSize = true;
            // 
            // confirmTextBox
            // 
            _outerTableLayoutPanel.Controls.Add(_confirmTextBox, 1, 3);
            _confirmTextBox.Name = "confirmTextBox";
            _confirmTextBox.Dock = DockStyle.Top;
            _confirmTextBox.AutoSize = true;
            // 
            // validationLabel
            // 
            _outerTableLayoutPanel.Controls.Add(_validationLabel, 0, 4);
            _outerTableLayoutPanel.SetColSpan(_validationLabel, 2);
            _validationLabel.Name = "validationLabel";
            _validationLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            _validationLabel.AutoSize = true;
            // 
            // urlLabel
            // 
            _outerTableLayoutPanel.Controls.Add(_urlLabel, 0, 5);
            _outerTableLayoutPanel.SetColSpan(_urlLabel, 2);
            _urlLabel.Name = "urlLabel";
            _urlLabel.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            _urlLabel.AutoSize = true;
            // 
            // urlTextBox
            // 
            _outerTableLayoutPanel.Controls.Add(_urlTextBox, 0, 6);
            _outerTableLayoutPanel.SetColSpan(_urlTextBox, 2);
            _urlTextBox.Name = "urlTextBox";
            _urlTextBox.Dock = DockStyle.Top;
            _urlTextBox.AutoSize = true;
            // 
            // notesLabel
            // 
            _outerTableLayoutPanel.Controls.Add(_notesLabel, 0, 7);
            _outerTableLayoutPanel.SetColSpan(_notesLabel, 2);
            _notesLabel.Name = "notesLabel";
            _notesLabel.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            _notesLabel.AutoSize = true;
            // 
            // notesTextBox
            // 
            _outerTableLayoutPanel.Controls.Add(_notesTextBox, 0, 8);
            _outerTableLayoutPanel.SetColSpan(_notesTextBox, 2);
            _notesTextBox.Dock = DockStyle.Fill;
            _notesTextBox.Name = "notesTextBox";
            _notesTextBox.AutoSize = true;
            // 
            // buttonsTableLayoutPanel
            // 
            _outerTableLayoutPanel.Controls.Add(_buttonsTableLayoutPanel, 0, 9);
            _outerTableLayoutPanel.SetColSpan(_buttonsTableLayoutPanel, 2);
            _buttonsTableLayoutPanel.AutoSize = true;
            _buttonsTableLayoutPanel.ColumnCount = 4;
            _buttonsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _buttonsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Auto));
            _buttonsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Auto));
            _buttonsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Auto));
            _buttonsTableLayoutPanel.Dock = DockStyle.Bottom;
            _buttonsTableLayoutPanel.Margin = new Padding(0);
            _buttonsTableLayoutPanel.Name = "buttonsTableLayoutPanel";
            _buttonsTableLayoutPanel.Padding = new Padding(0);
            _buttonsTableLayoutPanel.RowCount = 1;
            _buttonsTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Auto));
            // 
            // copyPwButton
            // 
            _buttonsTableLayoutPanel.Controls.Add(_copyPwButton, 0, 0);
            _copyPwButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _copyPwButton.DialogResult = DialogResult.OK;
            _copyPwButton.Name = "copyPwButton";
            _copyPwButton.Size = new Size(75, 23);
            _copyPwButton.TabIndex = 1;
            _copyPwButton.Text = "Copy PW";
            _copyPwButton.UseVisualStyleBackColor = true;
            // 
            // deleteButton
            // 
            _buttonsTableLayoutPanel.Controls.Add(_deleteButton, 1, 0);
            _deleteButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _deleteButton.DialogResult = DialogResult.OK;
            _deleteButton.Name = "deleteButton";
            _deleteButton.Size = new Size(75, 23);
            _deleteButton.TabIndex = 1;
            _deleteButton.Text = "Delete";
            _deleteButton.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            AcceptButton = new Button();
            _buttonsTableLayoutPanel.Controls.Add(AcceptButton, 2, 0);
            AcceptButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            AcceptButton.DialogResult = DialogResult.OK;
            AcceptButton.Name = "saveButton";
            AcceptButton.Size = new Size(75, 23);
            AcceptButton.TabIndex = 1;
            AcceptButton.Text = "Save";
            AcceptButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            CancelButton = new Button();
            _buttonsTableLayoutPanel.Controls.Add(CancelButton, 3, 0);
            CancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            AcceptButton.DialogResult = DialogResult.Cancel;
            CancelButton.Name = "cancelButton";
            CancelButton.Size = new Size(75, 23);
            CancelButton.TabIndex = 2;
            CancelButton.Text = "Cancel";
            CancelButton.UseVisualStyleBackColor = true;
            // 
            // CredentialEditForm
            // 
            ClientSize = new Size(800, 600);
            Controls.Add(_outerTableLayoutPanel);
            Name = "CredentialEditForm";
            Text = "Edit Credential";
            _outerTableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}