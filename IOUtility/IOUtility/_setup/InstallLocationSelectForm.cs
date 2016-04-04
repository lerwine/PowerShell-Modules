using System;

namespace PSModuleInstallUtil
{
    public partial class InstallLocationSelectForm : System.Windows.Forms.Form
    {
        private System.ComponentModel.BindingList<InstallationLocationInfo> _dataSource = new System.ComponentModel.BindingList<InstallationLocationInfo>();
        
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
            get { return this.isInstallableTextBoxColumn.Visible; }
            set { this.isInstallableTextBoxColumn.Visible = value; }
        }

        public bool ExistsColumnVisible
        {
            get { return this.existsTextBoxColumn.Visible; }
            set { this.existsTextBoxColumn.Visible = value; }
        }

        public bool ReasonColumnVisible
        {
            get { return this.reasonTextboxColumn.Visible; }
            set
            {
                this.reasonTextboxColumn.Visible = value;
                if (value)
                    this.locationTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
                else
                    this.locationTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        public bool LocationTypeRadioButtonsVisible
        {
            get { return this.locationTypeTableLayoutPanel.Visible; }
            set { this.locationTypeTableLayoutPanel.Visible = value; }
        }

        private bool _actionButtonEnabled = true;

        public bool ActionButtonEnabled
        {
            get { return this._actionButtonEnabled; }
            set
            {
                this._actionButtonEnabled = value;
                this.actionButton.Enabled = value && this.ErrorText.Length == 0;
            }
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
                this.actionButton.Enabled = this.ActionButtonEnabled && this.ErrorText.Length == 0;
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

                System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>();
                foreach (System.Windows.Forms.DataGridViewRow row in this.installLocationsDataGridView.SelectedRows)
                    result.Add((row.DataBoundItem as InstallationLocationInfo).ParentDirectory);

                return result.ToArray();
            }
        }

        public InstallLocationSelectForm()
        {
            this.InitializeComponent();
        }
        
        public void AddInstallationLocationInfo(InstallationLocationInfo installationLocationInfo)
        {
            this._dataSource.Add(installationLocationInfo);
        }
        
        private void InstallLocationSelectForm_Shown(object sender, System.EventArgs e)
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

            this.UpdateCustomLocationMode();

            this.BringToFront();
        }

        private void commonLocationsRadioButton_CheckedChanged(object sender, System.EventArgs e)
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

        private void customLocationadioButton_CheckedChanged(object sender, System.EventArgs e)
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
                this.actionButton.Visible = false;
                if (this.customPathTextBox.Text.Trim() == "")
                    this.ErrorText = "You must specify a module installation location.";
                else
                {
                    try
                    {
                        if (System.IO.Directory.Exists(this.customPathTextBox.Text))
                            this.ErrorText = "";
                        else if (System.IO.File.Exists(this.customPathTextBox.Text))
                            this.ErrorText = "Path must be a subdirectory.";
                        else
                            this.ErrorText = "Path does not exist.";
                    }
                    catch (Exception exc)
                    {
                        this.ErrorText = exc.Message;
                    }
                }
            }
            else
            {
                this.installLocationsDataGridView.Visible = true;
                if (this.customPathTextBox.Focused || this.browseButton.Focused)
                    this.installLocationsDataGridView.Focus();
                this.customPathTextBox.Visible = false;
                this.browseButton.Visible = false;
                this.actionButton.Visible = true;
                this.ErrorText = "";
            }
        }

        private void browseButton_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.Close();
        }

        private void actionButton_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void continueButton_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.Close();
        }

        private void cancelButton_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
