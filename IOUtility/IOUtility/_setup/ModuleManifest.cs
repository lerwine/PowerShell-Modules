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
        public string FullManifestPath { get; private set; }
        public string ModuleName { get; private set; }
        public string ModuleDescription { get { return Description; } }
        public string InstallRoot { get; private set; }
        public string RootModule { get; private set; }
        public Version ModuleVersion { get; private set; }
        public Guid? GUID { get; private set; }
        public Guid? ExpectedGuid { get; private set; }
        public string Author { get; private set; }
        public string CompanyName { get; private set; }
        public string Copyright { get; private set; }
        public string Description { get; private set; }
        public Version PowerShellVersion { get; private set; }
        public Version DotNetFrameworkVersion { get; private set; }
        public Version CLRVersion { get; private set; }
        public ReadOnlyCollection<string> RequiredModules { get; private set; }
        public ReadOnlyCollection<string> RequiredAssemblies { get; private set; }
        public ReadOnlyCollection<string> ScriptsToProcess { get; private set; }
        public ReadOnlyCollection<string> FormatsToProcess { get; private set; }
        public ReadOnlyCollection<string> TypesToProcess { get; private set; }
        public ReadOnlyCollection<string> NestedModules { get; private set; }
        public ReadOnlyCollection<string> FunctionsToExport { get; private set; }
        public ReadOnlyCollection<string> CmdletsToExport { get; private set; }
        public ReadOnlyCollection<string> VariablesToExport { get; private set; }
        public ReadOnlyCollection<string> AliasesToExport { get; private set; }
        public ReadOnlyCollection<string> ModuleList { get; private set; }
        public ReadOnlyCollection<string> FileList { get; private set; }
        public ReadOnlyCollection<ErrorRecord> Errors { get; private set; }
        public bool ManifestExists { get; private set; }
        public bool IsInstalled { get; private set; }
        public bool DirectoryExists { get; private set; }
        public bool CanInstall { get; private set; }
        public bool CanUpdate { get; private set; }

        public ModuleManifest(string directoryPath, string moduleName, Guid? expectedGuid)
        {
            if (directoryPath == null)
                throw new ArgumentNullException("directoryPath");

            if (directoryPath.Trim() == "")
                throw new ArgumentException("Path cannot be empty.", "directoryPath");

            InstallRoot = Path.GetFullPath(directoryPath);
            if (String.IsNullOrEmpty(InstallRoot))
                throw new ArgumentException("Invalid path.", "directoryPath");
            ModuleName = Path.GetFileName(moduleName);
            if (ModuleName != moduleName || moduleName.Trim() == ".psd1")
                throw new ArgumentException("Invalid module name.", "moduleName");
            if (ModuleName.EndsWith(".psd1"))
                ModuleName = ModuleName.Substring(0, ModuleName.Length - 4);
            FullManifestPath = Path.Combine(InstallRoot, ModuleName + ".psd1");
            ExpectedGuid = expectedGuid;
            Refresh();
        }

        public void Refresh()
        {
            ManifestExists = File.Exists(FullManifestPath);
            DirectoryExists = Directory.Exists(InstallRoot);
            RootModule = "";
            ModuleVersion = null;
            GUID = null;
            Author = "";
            CompanyName = "";
            Copyright = "";
            Description = "";
            PowerShellVersion = null;
            DotNetFrameworkVersion = null;
            CLRVersion = null;
            RequiredModules = new ReadOnlyCollection<string>(new string[0]);
            RequiredAssemblies = new ReadOnlyCollection<string>(new string[0]);
            ScriptsToProcess = new ReadOnlyCollection<string>(new string[0]);
            TypesToProcess = new ReadOnlyCollection<string>(new string[0]);
            FormatsToProcess = new ReadOnlyCollection<string>(new string[0]);
            NestedModules = new ReadOnlyCollection<string>(new string[0]);
            FunctionsToExport = new ReadOnlyCollection<string>(new string[0]);
            CmdletsToExport = new ReadOnlyCollection<string>(new string[0]);
            VariablesToExport = new ReadOnlyCollection<string>(new string[0]);
            AliasesToExport = new ReadOnlyCollection<string>(new string[0]);
            ModuleList = new ReadOnlyCollection<string>(new string[0]);
            FileList = new ReadOnlyCollection<string>(new string[0]);
            Errors = new ReadOnlyCollection<ErrorRecord>(new ErrorRecord[0]);

            if (!ManifestExists)
            {
                RootModule = ModuleName + ".psm1";
                IsInstalled = false;
                CanInstall = Directory.Exists(InstallRoot);
                CanUpdate = false;
                return;
            }

            Hashtable hashtable;
            using (StreamReader reader = new StreamReader(FullManifestPath))
            {
                using (PowerShell ps = PowerShell.Create(RunspaceMode.NewRunspace))
                {
                    ps.AddScript(reader.ReadToEnd(), true);
                    Collection<PSObject> mm = ps.Invoke();
                    if (ps.HadErrors)
                        Errors = new ReadOnlyCollection<ErrorRecord>(ps.Streams.Error.ToArray());
                    hashtable = mm.Where(o => o != null).Select(o => o.BaseObject).OfType<Hashtable>().FirstOrDefault();
                }
            }

            if (hashtable == null)
            {
                IsInstalled = false;
                RootModule = ModuleName + ".psm1";
                CanInstall = Directory.Exists(InstallRoot);
                CanUpdate = false;
                return;
            }

            if (hashtable.ContainsKey("RootModule"))
                RootModule = PSObjectHelper.AsTrimmedString(hashtable, "RootModule", "");
            else
                RootModule = PSObjectHelper.AsTrimmedString(hashtable, "ModuleToProcess", "");
            ModuleVersion = PSObjectHelper.AsVersion(hashtable, "ModuleVersion");
            GUID = PSObjectHelper.AsGuid(hashtable, "GUID");
            Author = PSObjectHelper.AsTrimmedString(hashtable, "Author");
            CompanyName = PSObjectHelper.AsTrimmedString(hashtable, "CompanyName");
            Copyright = PSObjectHelper.AsTrimmedString(hashtable, "Copyright");
            Description = PSObjectHelper.AsTrimmedString(hashtable, "Description");
            PowerShellVersion = PSObjectHelper.AsVersion(hashtable, "PowerShellVersion");
            DotNetFrameworkVersion = PSObjectHelper.AsVersion(hashtable, "DotNetFrameworkVersion");
            CLRVersion = PSObjectHelper.AsVersion(hashtable, "CLRVersion");
            RequiredModules = new ReadOnlyCollection<string>(PSObjectHelper.AsTrimmedStringList(hashtable, "RequiredModules"));
            RequiredAssemblies = new ReadOnlyCollection<string>(PSObjectHelper.AsTrimmedStringList(hashtable, "RequiredAssemblies"));
            ScriptsToProcess = new ReadOnlyCollection<string>(PSObjectHelper.AsTrimmedStringList(hashtable, "ScriptsToProcess"));
            TypesToProcess = new ReadOnlyCollection<string>(PSObjectHelper.AsTrimmedStringList(hashtable, "TypesToProcess"));
            FormatsToProcess = new ReadOnlyCollection<string>(PSObjectHelper.AsTrimmedStringList(hashtable, "FormatsToProcess"));
            NestedModules = new ReadOnlyCollection<string>(PSObjectHelper.AsTrimmedStringList(hashtable, "NestedModules"));
            FunctionsToExport = new ReadOnlyCollection<string>(PSObjectHelper.AsTrimmedStringList(hashtable, "FunctionsToExport"));
            CmdletsToExport = new ReadOnlyCollection<string>(PSObjectHelper.AsTrimmedStringList(hashtable, "CmdletsToExport"));
            VariablesToExport = new ReadOnlyCollection<string>(PSObjectHelper.AsTrimmedStringList(hashtable, "VariablesToExport"));
            AliasesToExport = new ReadOnlyCollection<string>(PSObjectHelper.AsTrimmedStringList(hashtable, "AliasesToExport"));
            ModuleList = new ReadOnlyCollection<string>(PSObjectHelper.AsTrimmedStringList(hashtable, "ModuleList"));
            FileList = new ReadOnlyCollection<string>(PSObjectHelper.AsTrimmedStringList(hashtable, "FileList"));
            IsInstalled = RootModule != "" && Errors.Count == 0 && File.Exists(Path.Combine(InstallRoot, RootModule));
            CanUpdate = IsInstalled && ExpectedGuid.HasValue && GUID.HasValue && ExpectedGuid.Value.Equals(GUID.Value);
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
        
        private static IEnumerable<char> _NormalizePathSeparators(string path)
        {
            bool previousIsSeparator = false;
            bool nonSeparatorEmitted = false;
            foreach (char c in path.ToCharArray())
            {
                if (c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)
                    previousIsSeparator = true;
                else
                {
                    nonSeparatorEmitted = true;
                    if (previousIsSeparator)
                        yield return Path.DirectorySeparatorChar;
                    previousIsSeparator = false;
                    yield return c;
                }
            }
            if (previousIsSeparator && !nonSeparatorEmitted)
                yield return Path.DirectorySeparatorChar;
        }

        public static string ResolvePath(string path)
        {
            if (String.IsNullOrEmpty(path))
                return "";

            return Path.GetFullPath(new String(_NormalizePathSeparators(path).ToArray()));
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

            target = ResolvePath(target);
            parent = ResolvePath(parent);
            if (String.Compare(target, parent, true) == 0)
                return true;

            if (!parent.EndsWith("\\"))
                parent += "\\";

            return target.Length > parent.Length && String.Compare(target.Substring(0, parent.Length), parent, true) == 0;
        }
        
    }
}
