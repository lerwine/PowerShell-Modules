using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Windows.Forms;

namespace PSModuleInstallUtil
{
    public partial class SetupForm : Form
    {
        private BindingList<ModuleManifest> _installationLocations = new BindingList<ModuleManifest>();
        private ModuleManifest _source = null;
        private Collection<string> _destinationLocations = new Collection<string>();
        public string ModuleName
        {
            get
            {
                ModuleManifest source = this.Source;
                return (source == null) ? "" : source.ModuleName;
            }
        }
        public Guid? ModuleGUID
        {
            get
            {
                ModuleManifest source = this.Source;
                return (source == null) ? null : source.GUID;
            }
        }

        public ModuleManifest Source
        {
            get { return this._source; }
            set
            {
                this._source = value;
                if (value == null)
                {
                    this.moduleNameLabel.Text = "No module loaded.";
                    this.moduleDescriptionLabel.Text = "";
                    return;
                }

                this.moduleNameLabel.Text = String.Format("{0}, Version {1}", value.ModuleName, value.ModuleVersion);
                this.moduleDescriptionLabel.Text = value.ModuleDescription;
            }
        }

        public Collection<string> DestinationLocations { get { return this._destinationLocations; } }

        public SetupForm()
        {
            this.InitializeComponent();
            this.installationLocationsDataGridView.DataSource = this._installationLocations;
        }

        public SetupForm(ModuleManifest source) : this() { this.Source = source; }

        public void AddPSModulePaths()
        {
            foreach (string path in ModuleManifest.GetPSModulePaths())
                this.AddInstallPath(path);
        }

        private bool _ContainsPath(string path)
        {
            foreach (ModuleManifest m in this._installationLocations)
            {
                if (String.Compare(m.InstallRoot as string, path, true) == 0)
                    return true;
            }
            return false;
        }

        public bool ContainsPath(string path)
        {
            if (String.IsNullOrEmpty(path))
                return false;

            return this._ContainsPath(Path.GetFullPath(path));
        }

        public void AddInstallPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(path);

            path = path.Trim();
            if (path == "")
                throw new ArgumentException("Path cannot be empty.", "path");

            path = Path.GetFullPath(path);

            if (_ContainsPath(path))
                return;
            
            _installationLocations.Add(new ModuleManifest(path, ModuleName, ModuleGUID));

            bool isInstalled = false;
            foreach (DataGridViewRow row in this.installationLocationsDataGridView.SelectedRows)
            {
                ModuleManifest m = row.DataBoundItem as ModuleManifest;
                if (m != null && m.IsInstalled)
                {
                    isInstalled = true;
                    break;
                }
            }

            this.statusTextLabel.Text = (isInstalled) ? "Installed" : "Not installed / found";
        }

        private void showCustomLocationButton_Click(object sender, EventArgs e)
        {
            this.DestinationLocations.Clear();
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void installationLocationsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            bool? installEnabled = null, uninstallEnabled = null, updateEnabled = null;
            
            foreach (DataGridViewRow row in this.installationLocationsDataGridView.SelectedRows)
            {
                ModuleManifest m = row.DataBoundItem as ModuleManifest;
                if (m == null)
                    continue;

                if (!installEnabled.HasValue)
                    installEnabled = !m.IsInstalled && m.CanInstall;
                else if (m.IsInstalled || !m.CanInstall)
                    installEnabled = false;

                if (!uninstallEnabled.HasValue)
                    uninstallEnabled = m.IsInstalled;
                else if (!m.IsInstalled)
                    uninstallEnabled = false;

                if (!updateEnabled.HasValue)
                    updateEnabled = m.CanUpdate;
                else if (!m.CanUpdate)
                    updateEnabled = false;
            }

            installButton.Enabled = installEnabled.HasValue && installEnabled.Value;
            UninstallButton.Enabled = uninstallEnabled.HasValue && uninstallEnabled.Value;
            updateButton.Enabled = updateEnabled.HasValue && updateEnabled.Value;
        }

        private void installButton_Click(object sender, EventArgs e)
        {
            this.DestinationLocations.Clear();
            foreach (DataGridViewRow row in this.installationLocationsDataGridView.SelectedRows)
            {
                ModuleManifest m = row.DataBoundItem as ModuleManifest;
                if (m == null)
                    continue;
                this.DestinationLocations.Add(m.InstallRoot);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void UninstallButton_Click(object sender, EventArgs e)
        {
            this.DestinationLocations.Clear();
            foreach (DataGridViewRow row in this.installationLocationsDataGridView.SelectedRows)
            {
                ModuleManifest m = row.DataBoundItem as ModuleManifest;
                if (m == null)
                    continue;
                this.DestinationLocations.Add(m.InstallRoot);
            }
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            this.DestinationLocations.Clear();
            foreach (DataGridViewRow row in this.installationLocationsDataGridView.SelectedRows)
            {
                ModuleManifest m = row.DataBoundItem as ModuleManifest;
                if (m == null)
                    continue;
                this.DestinationLocations.Add(m.InstallRoot);
            }
            this.DialogResult = DialogResult.Retry;
            this.Close();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.DestinationLocations.Clear();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
