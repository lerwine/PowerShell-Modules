using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace PSModuleInstallUtil
{
    public class ModuleManifest
    {
        private FileInfo _manifestFile;
        Hashtable _moduleManifest = new Hashtable();
        private string _moduleDescription = null;
        private Version _moduleVersion = null;
        private string _rootModuleValue = null;
        private string _moduleName = null;
        private string _installType = null;
        private FileInfo _rootModuleFile = null;
        private bool? _isInstalled = null;
        private string _statusText = null;
        private bool? _canInstall = null;
        private bool? _canUpdate = null;

        public static IEnumerable<object> AsBaseEnumerable(IEnumerable obj, int level)
        {
            if (level < 1)
            {
                foreach (object o in obj)
                    yield return (o != null && o is PSObject) ? (o as PSObject).BaseObject : o;
            }
            else
            {
                foreach (object o in obj)
                    yield return EnumeratedRawValue((o != null && o is PSObject) ? (o as PSObject).BaseObject : o, level - 1);
            }
        }

        public static IEnumerable<object> AsObjectEnumerable(IEnumerable obj)
        {
            foreach (object o in obj)
                yield return o;
        }

        public static List<object> EnumeratedRawValue(object obj) { return EnumeratedRawValue(obj, 3); }
        
        public static List<object> EnumeratedRawValue(object obj, int level)
        {
            if ((obj = (obj != null && obj is PSObject) ? (obj as PSObject).BaseObject : obj) == null)
                return new List<object>();

            if (obj is string)
                return new List<object>(new object[] { obj });

            if (obj is object[] || obj is Collection<object> || obj is List<object> || obj is PSObject[] || obj is Collection<PSObject> || obj is List<PSObject> || obj is Hashtable[] || obj is Collection<Hashtable> || obj is List<Hashtable>)
                return new List<object>(AsBaseEnumerable(obj as IEnumerable, level - 1));

            if (obj is Hashtable)
            {
                if (level < 1)
                    return new List<object>(new object[] { obj });
                Hashtable hashTable = obj as Hashtable;
                Hashtable result = new Hashtable();
                foreach (object k in result.Keys)
                    result.Add(k, EnumeratedRawValue(hashTable[k], level - 1));
                return new List<object>(new object[] { result });
            }

            if (obj is IEnumerable)
                return new List<object>(AsObjectEnumerable(obj as IEnumerable));

            return new List<object>(new object[] { obj });
        }

        public List<object> GetModuleRawValues(string key)
        {
            return EnumeratedRawValue((this._moduleManifest.ContainsKey(key)) ? this._moduleManifest[key] : null);
        }

        public object FirstOrDefault(string key)
        {
            List<object> values = this.GetModuleRawValues(key);
            return (values.Count == 0) ? null : values[0];
        }

        public static IEnumerable<string> GetPSModulePaths()
        {
            string psModulePath = System.Environment.GetEnvironmentVariable("PSModulePath");
            if (String.IsNullOrEmpty(psModulePath))
            {
                yield return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "WindowsPowerShell\\Modules");
                yield return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "WindowsPowerShell\\Modules");
            }
            else
            {
                foreach (string path in psModulePath.Split(Path.PathSeparator))
                {
                    string s = path.Trim();
                    if (s != "")
                        yield return s;
                }
            }
        }

        public static bool IsSystemPath(string path)
        {
            return !String.IsNullOrEmpty(path) &&
                IsSubPathOf(path, Environment.GetFolderPath(Environment.SpecialFolder.System).ToLower());
        }

        public static bool IsAllUsersPath(string path)
        {
            return !String.IsNullOrEmpty(path) &&
                IsSubPathOf(path, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).ToLower());
        }

        public static bool IsCurrentUserPath(string path)
        {
            return !String.IsNullOrEmpty(path) &&
                IsSubPathOf(path, Environment.GetFolderPath(Environment.SpecialFolder.Personal).ToLower());
        }

        public static bool IsSubPathOf(string target, string parent)
        {
            if (String.IsNullOrEmpty(target) || String.IsNullOrEmpty(parent))
                return false;

            target = Path.GetFullPath(target);
            parent = Path.GetFullPath(parent);
            if (String.Compare(target, parent, true) == 0)
                return true;

            if (!parent.EndsWith("\\"))
                parent += "\\";

            return target.Length > parent.Length && String.Compare(target.Substring(0, parent.Length), parent, true) == 0;
        }

        public string ModuleDescription
        {
            get
            {
                string value = this._moduleDescription;
                if (value == null)
                {
                    value = (this.FirstOrDefault("Description") as string) ?? "";
                    this._moduleDescription = value;
                }

                return value;
            }
        }

        public Version ModuleVersion
        {
            get
            {
                Version value = this._moduleVersion;
                if (value == null)
                {
                    object obj = this.FirstOrDefault("ModuleVersion");
                    if (obj != null)
                    {
                        if (obj is Version)
                            value = (Version)obj;
                        else if (obj is string)
                        {
                            try
                            {
                                value = new Version(obj as string);
                            }
                            catch { }
                        }
                    }
                    if (value == null)
                        value = new Version();
                    this._moduleVersion = value;
                }

                return value;
            }
        }
        
        public string RootModuleValue
        {
            get
            {
                string rootModule = this._rootModuleValue;
                if (rootModule == null)
                {
                    if ((rootModule = this.FirstOrDefault("RootModule") as string) == null && (rootModule = this.FirstOrDefault("ModuleToProcess") as string) == null) 
                        rootModule = "";
                    this._rootModuleValue = rootModule;
                }

                return rootModule;
            }
        }
        
        public string ModuleName
        {
            get
            {
                string moduleName = this._moduleName;
                if (moduleName == null)
                {
                    moduleName = Path.GetFileNameWithoutExtension(this._manifestFile.Name);
                    this._moduleName = moduleName;
                }

                return moduleName;
            }
        }

        public string InstallType
        {
            get
            {
                string installType = this._installType;
                if (installType == null)
                {
                    if (IsSystemPath(this._manifestFile.FullName))
                        installType = "System";
                    else if (IsAllUsersPath(this._manifestFile.FullName))
                        installType = "All Users";
                    else if (IsCurrentUserPath(this._manifestFile.FullName))
                        installType = "Current User";
                    else
                        installType = "Other";

                    this._installType = installType;
                }

                return installType;
            }
        }

        public string InstallRoot { get { return (this._manifestFile.Directory.Parent == null) ? this._manifestFile.Directory.FullName : this._manifestFile.Directory.Parent.FullName; } }

        public FileInfo RootModuleFile
        {
            get
            {
                FileInfo rootModuleFile = this._rootModuleFile;
                if (rootModuleFile == null)
                {
                    string rootModuleValue = this.RootModuleValue;
                    if (rootModuleValue == "")
                    {
                        rootModuleFile = new FileInfo(Path.Combine(this._manifestFile.DirectoryName, this.ModuleName + ".dll"));
                        if (!rootModuleFile.Exists)
                            rootModuleFile = new FileInfo(Path.Combine(this._manifestFile.DirectoryName, this.ModuleName + ".psm1"));
                    }
                    else
                        rootModuleFile = new FileInfo(Path.Combine(this._manifestFile.DirectoryName, rootModuleValue));
                    this._rootModuleFile = rootModuleFile;
                }

                return rootModuleFile;
            }
        }
        
        public bool IsInstalled
        {
            get
            {
                bool? isInstalled = this._isInstalled;
                if (isInstalled.HasValue)
                    return isInstalled.Value;

                bool c, u, i;
                this.UpdateStatus(out c, out u, out i);
                return i;
            }
        }
        
        public bool CanInstall
        {
            get
            {
                bool? canInstall = this._canInstall;
                if (canInstall.HasValue)
                    return canInstall.Value;

                bool c, u, i;
                this.UpdateStatus(out c, out u, out i);
                return c;
            }
        }

        public bool CanUpdate
        {
            get
            {
                bool? canUpdate = this._canUpdate;
                if (canUpdate.HasValue)
                    return canUpdate.Value;

                bool c, u, i;
                this.UpdateStatus(out c, out u, out i);
                return u;
            }
        }

        private string UpdateStatus(out bool canInstall, out bool canUpdate, out bool isInstalled)
        {
            string statusText;
            if (this._manifestFile.Exists && this.RootModuleValue != "" && this.RootModuleFile.Exists)
            {
                canInstall = true;
                isInstalled = true;
                canUpdate = true;
                statusText = String.Format("Version {0} installed.", this.ModuleVersion);
            }
            else
            {
                isInstalled = false;
                if (Directory.Exists(this._manifestFile.FullName) || Directory.Exists(this.RootModuleFile.FullName))
                {
                    canInstall = false;
                    canUpdate = false;
                    statusText = "Cannot install (subdir conflict)";
                }
                else if (this._manifestFile.Exists || this.RootModuleFile.Exists)
                {
                    canInstall = false;
                    canUpdate = true;
                    statusText = "Partial install";
                }
                else if (this._manifestFile.Directory.Exists)
                {
                    canInstall = true;
                    canUpdate = false;
                    statusText = "Directory already exists";
                }
                else
                {
                    canUpdate = false;
                    canInstall = CanInstallat(this._manifestFile.DirectoryName, out statusText);
                    if (canInstall)
                        statusText = "Not installed";
                }
            }
            this._canInstall = canInstall;
            this._isInstalled = isInstalled;
            this._statusText = statusText;
            this._canUpdate = canUpdate;
            return statusText;
        }

        public string StatusText
        {
            get
            {
                string statusText = this._statusText;
                if (statusText != null)
                    return statusText;

                bool c, u, i;
                return this.UpdateStatus(out c, out u, out i);
            }
        }

        public static bool CanInstallat(string directoryName, out string statusText)
        {
            if (String.IsNullOrEmpty(directoryName))
            {
                statusText = "Empty directory name.";
                return false;
            }

            if (Directory.Exists(directoryName))
            {
                statusText = "Can install.";
                return true;
            }

            if (File.Exists(directoryName))
            {
                statusText = "Path naming conflict";
                return false;
            }

            directoryName = Path.GetDirectoryName(directoryName);
            if (String.IsNullOrEmpty(directoryName))
            {
                statusText = "Path not found.";
                return false;
            }

            return CanInstallat(directoryName, out statusText);
        }

        public ModuleManifest(string manifestPath)
        {
            if (manifestPath == null)
                throw new ArgumentNullException("manifestPath");

            manifestPath = manifestPath.Trim();
            if (manifestPath == "")
                throw new ArgumentException("Manifest Path cannot be empty.", "manifestPath");

            this._manifestFile = new FileInfo(manifestPath);
            this.Refresh();
        }

        public ModuleManifest(string installRoot, string moduleName)
        {
            if (installRoot == null)
                throw new ArgumentNullException("installRoot");

            if (moduleName == null)
                throw new ArgumentNullException("moduleName");

            installRoot = installRoot.Trim();
            if (installRoot == "")
                throw new ArgumentException("Install Root Path cannot be empty.", "installRoot");

            moduleName = moduleName.Trim();
            if (moduleName == "")
                throw new ArgumentException("Module Name cannot be empty.", "moduleName");

            string location = Path.Combine(installRoot, moduleName);
            string manifestPath = Path.Combine(location, moduleName + ".psd1");
            if (moduleName.ToLower().EndsWith(".psd1"))
            {
                string loc = Path.Combine(installRoot, moduleName.Substring(0, moduleName.Length - 5));
                if (Directory.Exists(loc) || !Directory.Exists(location))
                {
                    location = loc;
                    manifestPath = Path.Combine(location, moduleName + ".psd1");
                }
                if (!File.Exists(manifestPath))
                    manifestPath = Path.Combine(location, moduleName);
            }

            this._manifestFile = new FileInfo(manifestPath);
            this.Refresh();
        }

        public void Refresh()
        {
            this._manifestFile.Refresh();
            this._moduleDescription = null;
            this._moduleVersion = null;
            this._rootModuleValue = null;
            this._rootModuleFile = null;
            this._statusText = null;
            this._canInstall = null;
            this._isInstalled = null;
            this._canUpdate = null;
            this._moduleManifest.Clear();

            if (!this._manifestFile.Exists)
                return;

            try
            {
                ScriptBlock sb = ScriptBlock.Create(File.ReadAllText(this._manifestFile.FullName).Trim());
                Collection<PSObject> result = sb.Invoke();
                foreach (PSObject obj in result)
                {
                    if (obj != null && obj.BaseObject != null && obj.BaseObject is Hashtable)
                    {
                        this._moduleManifest = obj.BaseObject as Hashtable;
                        break;
                    }
                }
            }
            catch { }
        }
    }
}
