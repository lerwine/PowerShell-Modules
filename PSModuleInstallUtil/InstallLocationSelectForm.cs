using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management.Automation;
using System.Windows.Forms;

namespace PSModuleInstallUtil
{
    public partial class InstallLocationSelectForm : Form
    {
        private BindingList<InstallationLocationInfo> _dataSource = new BindingList<InstallationLocationInfo>();
        private ScriptBlock _onFormShown = null;
        private ScriptBlock _onModeChanged = null;
        private ScriptBlock _onSelectionChanged = null;
        private ScriptBlock _onFolderBrowsing = null;

        public bool ContinueButtonVisible
        {
            get { return this.continueButton.Visible; }
            set { this.continueButton.Visible = value; }
        }

        public bool IsAllUsersColumnVisible
        {
            get { return this.isAllUsersTextBoxColumn.Visible; }
            set { this.isAllUsersTextBoxColumn.Visible = value; }
        }

        public bool CanBeInstalledColumnVisible
        {
            get { return this.canInstallTextBoxColumn.Visible; }
            set { this.canInstallTextBoxColumn.Visible = value; }
        }

        public bool IsInstalledColumnVisible
        {
            get { return this.isInstalledTextBoxColumn.Visible; }
            set { this.isInstalledTextBoxColumn.Visible = value; }
        }

        public bool MessageColumnVisible
        {
            get { return this.messageTextboxColumn.Visible; }
            set
            {
                this.messageTextboxColumn.Visible = value;
                if (value)
                    this.pathTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                else
                    this.pathTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        public bool LocationTypeRadioButtonsVisible
        {
            get { return this.locationTypeTableLayoutPanel.Visible; }
            set { this.locationTypeTableLayoutPanel.Visible = value; }
        }

        public bool ActionButtonEnabled
        {
            get { return this.actionButton.Enabled; }
            set { this.actionButton.Enabled = value; }
        }

        public string ActionButtonText
        {
            get { return this.actionButton.Text; }
            set { this.continueButton.Text = (value == null) ? "" : value.Trim(); }
        }

        public string ErrorText
        {
            get { return (this.pathErrorLabel.Visible) ? this.pathErrorLabel.Text : ""; }
            set
            {
                this.pathErrorLabel.Text = (value == null) ? "" : value.Trim();
                this.pathErrorLabel.Visible = this.pathErrorLabel.Text.Length > 0;
            }
        }

        public bool IsMultiSelect
        {
            get { return this.installLocationsDataGridView.MultiSelect; }
            set { this.installLocationsDataGridView.MultiSelect = value; }
        }

        public bool IsCustomLocation
        {
            get { return this.customLocationadioButton.Checked; }
            set { this.customLocationadioButton.Checked = value; }
        }

        public ScriptBlock OnFormShown
        {
            get { return this._onFormShown; }
            set { this._onFormShown = value; }
        }

        public ScriptBlock OnModeChanged
        {
            get { return this._onModeChanged; }
            set { this._onModeChanged = value; }
        }

        public ScriptBlock OnSelectionChanged
        {
            get { return this._onSelectionChanged; }
            set { this._onSelectionChanged = value; }
        }

        public ScriptBlock OnFolderBrowsing
        {
            get { return this._onFolderBrowsing; }
            set { this._onFolderBrowsing = value; }
        }

        public string[] SelectedPaths
        {
            get
            {
                if (this.customLocationadioButton.Checked)
                {
                    if (this.customPathTextBox.Text.Trim().Length == 0)
                        return new string[0];

                    return new string[] { this.customPathTextBox.Text };
                }

                List<string> result = new List<string>();
                foreach (DataGridViewRow row in this.installLocationsDataGridView.SelectedRows)
                    result.Add((row.DataBoundItem as InstallationLocationInfo).Path);

                return result.ToArray();
            }
        }

        public InstallLocationSelectForm()
        {
            this.InitializeComponent();
        }

        public InstallationLocationInfo AddInstallationLocationInfo(string path, bool canInstall, bool isInstalled, bool isAllUsers)
        {
            InstallationLocationInfo item = new InstallationLocationInfo
            {
                CanBeInstalled = canInstall,
                IsAllUsers = isAllUsers,
                IsInstalled = isInstalled,
                Path = path
            };
            this._dataSource.Add(item);
            return item;
        }

        public InstallationLocationInfo AddInstallationLocationInfo(string path, bool canInstall, bool isInstalled)
        {
            return this.AddInstallationLocationInfo(path, canInstall, isInstalled, false);
        }

        public InstallationLocationInfo AddInstallationLocationInfo(string path, bool canInstall)
        {
            return this.AddInstallationLocationInfo(path, canInstall, false);
        }

        public InstallationLocationInfo AddInstallationLocationInfo(string path)
        {
            return this.AddInstallationLocationInfo(path, false);
        }
        
        private void InstallLocationSelectForm_Shown(object sender, EventArgs e)
        {
            for (int i = this._dataSource.Count - 1; i >= 0; i--)
            {
                if (this._dataSource[i] == null)
                    this._dataSource.RemoveAt(i);
            }

            if (this._dataSource.Count == 0)
            {
                this.commonLocationsRadioButton.Checked = false;
                this.customLocationadioButton.Checked = true;
                this.locationTypeTableLayoutPanel.Visible = false;
                this.installLocationsDataGridView.Visible = false;
                this.customPathTextBox.Visible = true;
                this.browseButton.Visible = true;
            }
            else
                this.installLocationsDataGridView.DataSource = this._dataSource;
            
            this.BringToFront();

            ScriptBlock sb = this.OnFormShown;
            if (sb != null)
                sb.Invoke(this);
        }

        private void commonLocationsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.commonLocationsRadioButton.Checked)
            {
                if (this.customLocationadioButton.Checked)
                    this.customLocationadioButton.Checked = false;
                this.UpdateCustomLocationMode();
            }
            else if (!this.customLocationadioButton.Checked)
                this.customLocationadioButton.Checked = true;
        }

        private void customLocationadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.customLocationadioButton.Checked)
            {
                if (this.commonLocationsRadioButton.Checked)
                    this.commonLocationsRadioButton.Checked = false;
                this.UpdateCustomLocationMode();
            }
            else if (!this.commonLocationsRadioButton.Checked)
                this.commonLocationsRadioButton.Checked = true;
        }

        private void UpdateCustomLocationMode()
        {
            if (this.customLocationadioButton.Checked)
            {
                this.customPathTextBox.Visible = true;
                this.browseButton.Visible = true;
                if (this.installLocationsDataGridView.Focused)
                    this.customPathTextBox.Focus();
                this.installLocationsDataGridView.Visible = false;
            }
            else
            {
                this.installLocationsDataGridView.Visible = true;
                if (this.customPathTextBox.Focused || this.browseButton.Focused)
                    this.installLocationsDataGridView.Focus();
                this.customPathTextBox.Visible = false;
                this.browseButton.Visible = false;
            }

            ScriptBlock sb = this.OnModeChanged;
            if (sb != null)
                sb.Invoke(this, this.customLocationadioButton.Checked);
        }

        private void installLocationsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            ScriptBlock sb = this.OnSelectionChanged;
            if (sb != null)
                sb.Invoke(this, this.SelectedPaths);
        }

        private void customPathTextBox_TextChanged(object sender, EventArgs e)
        {
            ScriptBlock sb = this.OnSelectionChanged;
            if (sb != null)
                sb.Invoke(this, this.SelectedPaths);
        }

        private void customPathTextBox_Leave(object sender, EventArgs e)
        {
            ScriptBlock sb = this.OnSelectionChanged;
            if (sb != null)
                sb.Invoke(this, this.SelectedPaths);
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                ScriptBlock sb = this.OnFolderBrowsing;
                if (sb != null)
                    sb.Invoke(this, dialog);
                dialog.ShowNewFolderButton = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                    this.customPathTextBox.Text = dialog.SelectedPath;
            }
        }

        private void actionButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Ignore;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
