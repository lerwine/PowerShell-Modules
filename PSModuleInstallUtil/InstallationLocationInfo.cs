namespace PSModuleInstallUtil
{
    public class InstallationLocationInfo
    {
        private string _path = "";
        public string Path
        {
            get { return this._path; }
            set { this._path = (value == null) ? "" : value; }
        }
        private string _message = "";
        public string Message
        {
            get { return this._message; }
            set { this._message = (value == null) ? "" : value; }
        }
        private bool _isInstalled = false;
        public bool IsInstalled
        {
            get { return this._isInstalled; }
            set { this._isInstalled = value; }
        }
        public string IsInstalledText { get { return this.IsInstalled ? "Yes" : "No"; } }
        private bool _canBeInstalled = false;
        public bool CanBeInstalled
        {
            get { return this._canBeInstalled; }
            set { this._canBeInstalled = value; }
        }
        public string CanBeInstalledText { get { return this.CanBeInstalled ? "Yes" : "No"; } }
        private bool _isAllUsers = false;
        public bool IsAllUsers
        {
            get { return this._isAllUsers; }
            set { this._isAllUsers = value; }
        }
        public string IsAllUsersText { get { return this.IsAllUsers ? "Yes" : "No"; } }
    }
}
