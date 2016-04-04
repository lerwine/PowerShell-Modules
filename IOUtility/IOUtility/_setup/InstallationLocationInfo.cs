namespace PSModuleInstallUtil
{
    public class InstallationLocationInfo
    {
        private string _name = "";
        public string Name
        {
            get { return this._name; }
            set { this._name = (value == null) ? "" : value; }
        }
        private string _parentdirectory = "";
        public string ParentDirectory
        {
            get { return this._parentdirectory; }
            set { this._parentdirectory = (value == null) ? "" : value; }
        }
        private string _relativePath = "";
        public string RelativePath
        {
            get { return this._relativePath; }
            set { this._relativePath = (value == null) ? "" : value; }
        }
        private string _reason = "";
        public string Reason
        {
            get { return this._reason; }
            set { this._reason = (value == null) ? "" : value; }
        }
        private bool _exists = false;
        public bool Exists
        {
            get { return this._exists; }
            set { this._exists = value; }
        }
        public string ExistsText { get { return this.Exists ? "Yes" : "No"; } }
        private bool _isInstallable = false;
        public bool IsInstallable
        {
            get { return this._isInstallable; }
            set { this._isInstallable = value; }
        }
        public string IsInstallableText { get { return this.IsInstallable ? "Yes" : "No"; } }
        private bool _isAllUsers = false;
        public bool IsAllUsers
        {
            get { return this._isAllUsers; }
            set { this._isAllUsers = value; }
        }
        public string IsAllUsersText { get { return this.IsAllUsers ? "Yes" : "No"; } }
        private bool _expectDirectory = false;
        public bool ExpectDirectory
        {
            get { return this._expectDirectory; }
            set { this._expectDirectory = value; }
        }
    }
}
