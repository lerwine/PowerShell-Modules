using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace PSModuleInstallUtil
{
    public class ModuleManifest
    {
        private Hashtable _manifestData;
        
        public FileInfo ManifestFileInfo { get; private set; }

        public string DisplayName
        {
            get
            {
                string name = Name;
                if (name != null && (name = name.Trim()).Length > 0)
                    return name;
                name = RootModule;
                if (String.IsNullOrEmpty(name))
                    return Guid.ToString("n");
                try { return Path.GetFileNameWithoutExtension(name); } catch { return name; }
            }
        }

        /// <summary>
        /// Name of module
        /// </summary>
        public string Name { get; private set; }

        public ModuleType ModuleType { get; private set; }

        public string ModuleBase { get; private set; }
        
        #region Common Manifest Properties

        /// <summary>
        /// ID used to uniquely identify this module
        /// </summary>
        public Guid Guid { get; private set; }

        /// <summary>
        /// Version number of this module.
        /// </summary>
        /// <remarks>Default value is 1.0</remarks>
        public Version Version { get; private set; }

        /// <summary>
        /// Script module or binary module file associated with this manifest. Previous versions of PowerShell called this element the ModuleToProcess.
        /// </summary>
        /// <remarks>Possible types for the root module can be empty (which will make this a Manifest module), the name of a script module (.psm1, which makes this a Script module), or the name of a binary module (.exe or .dll, which makes this a Binary module). Placing the name of a module manifest (.psd1) or a script file (.ps1) in this element will cause an error to occur.</remarks>
        public string RootModule { get; private set; }

        /// <summary>
        /// Description of the functionality provided by this module
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Modules that must be imported into the global environment prior to importing this module
        /// </summary>
        /// <remarks>Note that this does not actually load any dependent modules – rather, it checks to see if the specified modules have already been loaded. (For example, some modules may already be loaded by a different module.)</remarks>
        public ReadOnlyCollection<PSModuleInfo> RequiredModules { get; private set; }

        /// <summary>
        /// Specifies the aliases that the module exports (wildcard characters are permitted) to the caller’s session state.
        /// </summary>
        /// <remarks>By default, all aliases are exported. You can use this key to restrict the aliases that are exported by the module.
        /// The caller’s session state can be the global session state or, for nested modules, the session state of another module.When you are chaining nested modules, all aliases that are exported by a nested module will be ultimately exported to the global session state unless a module in the chain restricts the alias by using the AliasesToExport key.</remarks>
        public Dictionary<string, AliasInfo> AliasesToExport { get; private set; }

        /// <summary>
        /// Specifies the cmdlets that the module exports (wildcard characters are permitted).
        /// </summary>
        /// <remarks>By default, all cmdlets are exported. You can use this key to restrict the cmdlets that are exported by the module.
        /// The caller’s session state can be the global session state or, for nested modules, the session state of another module.When you are chaining nested modules, all cmdlets that are exported by a nested module will be ultimately exported to the global session state unless a module in the chain restricts the cmdlet by using the CmdletsToExport key.
        /// If the manifest also exports aliases for the cmdlets, this key can remove cmdlets whose aliases are listed in the AliasesToExport key, but this key cannot add cmdlet aliases to the list.</remarks>
        public Dictionary<string, CmdletInfo> ExportedCmdlets { get; private set; }

        /// <summary>
        /// Format files (.ps1xml) to be loaded when importing this module.
        /// </summary>
        public ReadOnlyCollection<string> FormatsToProcess { get; private set; }

        /// <summary>
        /// Specifies the functions that the module exports (wildcard characters are permitted) to the caller’s session state.
        /// </summary>
        /// <remarks>By default, all functions are exported. You can use this key to restrict the functions that are exported by the module.
        /// The caller’s session state can be the global session state or, for nested modules, the session state of another module.When chaining nested modules, all functions that are exported by a nested module will be exported to the global session state unless a module in the chain restricts the function by using the FunctionsToExport key.
        /// If the manifest also exports aliases for the functions, this key can remove functions whose aliases are listed in the AliasesToExport key, but this key cannot add function aliases to the list.</remarks>
        public Dictionary<string, FunctionInfo> FunctionsToExport { get; private set; }

        /// <summary>
        /// Type files (.ps1xml) to be loaded when importing this module
        /// </summary>
        public ReadOnlyCollection<string> TypesToProcess { get; private set; }

        /// <summary>
        /// Specifies the variables that the module exports (wildcard characters are permitted) to the caller’s session state.
        /// </summary>
        /// <remarks> By default, all variables are exported. You can use this key to restrict the variables that are exported by the module.
        /// The caller’s session state can be the global session state or, for nested modules, the session state of another module.When you are chaining nested modules, all variables that are exported by a nested module will be exported to the global session state unless a module in the chain restricts the variable by using the VariablesToExport key.
        /// If the manifest also exports aliases for the variables, this key can remove variables whose aliases are listed in the AliasesToExport key, but this key cannot add variable aliases to the list.</remarks>
        public Dictionary<string, PSVariable> VariablesToExport { get; private set; }

        /// <summary>
        /// Modules to import as nested modules of the module specified in RootModule/ModuleToProcess.
        /// </summary>
        /// <remarks>Adding a module name to this element is similar to calling Import-Module from within your script or assembly code. The main difference is that it’s easier to see what you are loading here in the manifest file. Also, if a module fails to load here, you will not yet have loaded your actual module.
        /// In addition to other modules, you may also load script(.ps1) files here.These files will execute in the context of the root module. (This is equivalent to dot sourcing the script in your root module.)</remarks>
        public ReadOnlyCollection<PSModuleInfo> NestedModules { get; private set; }

        /// <summary>
        /// Specifies any private data that needs to be passed to the root module specified by the RootModule/ModuleToProcess key.
        /// </summary>
        public object PrivateData { get; private set; }

        #endregion

        #region PowerShell 3.0+ Properties

        /// <summary>
        /// Author of this module
        /// </summary>
        public string Author { get; private set; }

        /// <summary>
        /// Company or vendor of this module.
        /// </summary>
        /// <remarks>Default value is &quot;Unknown&quot;</remarks>
        public string CompanyName { get; private set; }

        /// <summary>
        /// Copyright statement for this module.
        /// </summary>
        /// <remarks>Default value is &quot;© [currentYear] [Author]. All rights reserved.&quot;</remarks>
        public string Copyright { get; private set; }

        /// <summary>
        /// HelpInfo URI of this module.
        /// </summary>
        public string HelpInfoUri { get; private set; }

        /// <summary>
        /// Minimum version of the Windows PowerShell engine required by this module.
        /// </summary>
        public Version PowerShellVersion { get; private set; }

        /// <summary>
        /// Specifies the name of the Windows PowerShell host that is required by the module.
        /// </summary>
        public string PowerShellHostName { get; private set; }

        /// <summary>
        /// Minimum version of the Windows PowerShell host required by this module
        /// </summary>
        public Version PowerShellHostVersion { get; private set; }

        /// <summary>
        /// Processor architecture (None, X86, Amd64) required by this module
        /// </summary>
        public ProcessorArchitecture ProcessorArchitecture { get; private set; }

        /// <summary>
        /// Minimum version of the common language runtime (CLR) required by this module
        /// </summary>
        public Version ClrVersion { get; private set; }

        /// <summary>
        /// Minimum version of Microsoft .NET Framework required by this module
        /// </summary>
        public Version DotNetFrameworkVersion { get; private set; }

        /// <summary>
        /// Assemblies that must be loaded prior to importing this module
        /// </summary>
        /// <remarks>Note that unlike RequiredModules, PowerShell will load the RequiredAssemblies if they are not already loaded.</remarks>
        public IEnumerable<string> RequiredAssemblies { get; private set; }

        /// <summary>
        /// Script (.ps1) files that are run in the caller's session state when the module is imported.
        /// </summary>
        /// <remarks>These scripts are run before any of the modules listed in the manifest are loaded</remarks>
        public IEnumerable<string> ScriptsToProcess { get; private set; }

        /// <summary>
        /// List of all files packaged with this module
        /// </summary>
        /// <remarks>As with ModuleList, FileList is to assist you as an inventory list, and is not otherwise processed.</remarks>
        public IEnumerable<string> FileList { get; private set; }

        /// <summary>
        /// Specifies all the modules that are packaged with this module.
        /// </summary>
        /// <remarks>These modules can be entered by name (a comma-separated string) or as a hash table with ModuleName and GUID keys. The hash table can also have an optional ModuleVersion key. The ModuleList key is designed to act as a module inventory. These modules are not automatically processed.</remarks>
        public IEnumerable<object> ModuleList { get; private set; }

        #endregion

        #region PowerShell 4.0+ Properties

        /// <summary>
        /// Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
        /// </summary>
        public string Prefix { get; private set; }

        #endregion

        #region PowerShell 5.0+ Properties

        /// <summary>
        /// Exported DSC resources.
        /// </summary>
        public ReadOnlyCollection<string> ExportedDscResources { get; private set; }

        /// <summary>
        /// Icon Uri of this module.
        /// </summary>
        public Uri IconUri { get; private set; }

        /// <summary>
        /// License Uri of this module
        /// </summary>
        public Uri LicenseUri { get; private set; }

        /// <summary>
        /// Project Uri of this module
        /// </summary>
        public Uri ProjectUri { get; private set; }

        /// <summary>
        /// Release notes for this module
        /// </summary>
        public string ReleaseNotes { get; private set; }

        /// <summary>
        /// Repository source location for this module
        /// </summary>
        public Uri RepositorySourceLocation { get; private set; }

        /// <summary>
        /// Tags for this module
        /// </summary>
        public IEnumerable<string> Tags { get; private set; }

        #endregion

        public ModuleManifest(PSModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
                throw new ArgumentNullException("moduleInfo");

            if (String.IsNullOrEmpty(moduleInfo.Path) || !(ManifestFileInfo = new FileInfo(moduleInfo.Path)).Exists)
                throw new ArgumentException("Module Information object does not represent a module manifest.");

            Initialize(moduleInfo, ScriptBlock.Create(File.ReadAllText(moduleInfo.Path)));
        }

        public ModuleManifest(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            ManifestFileInfo = new FileInfo(path);
            if (!ManifestFileInfo.Exists)
                throw new ArgumentException("Manifest file does not exist.");

            ScriptBlock scriptBlock = ScriptBlock.Create(File.ReadAllText(ManifestFileInfo.FullName));
            Initialize(new PSModuleInfo(scriptBlock), scriptBlock);
        }

        private void Initialize(PSModuleInfo moduleInfo, ScriptBlock scriptBlock)
        {
            Collection<PSObject> result = scriptBlock.Invoke();
            if (result.Count != 1 || result[0] == null || result[0].BaseObject == null || !(result[0].BaseObject is Hashtable))
                throw new FormatException("Module manifest did not return a single Hashtable object.");
            _manifestData = result[0].BaseObject as Hashtable;

            Name = moduleInfo.Name;
            Guid = moduleInfo.Guid;
            Version = moduleInfo.Version;
            ModuleType = moduleInfo.ModuleType;
            RootModule = moduleInfo.RootModule;
            ModuleBase = moduleInfo.ModuleBase;
            Description = moduleInfo.Description;
            RequiredModules = moduleInfo.RequiredModules;
            AliasesToExport = moduleInfo.ExportedAliases;
            ExportedCmdlets = moduleInfo.ExportedCmdlets;
            FormatsToProcess = moduleInfo.ExportedFormatFiles;
            FunctionsToExport = moduleInfo.ExportedFunctions;
            TypesToProcess = moduleInfo.ExportedTypeFiles;
            VariablesToExport = moduleInfo.ExportedVariables;
            NestedModules = moduleInfo.NestedModules;
            PrivateData = moduleInfo.PrivateData;

#if PSV2
            Author = GetManifestDataAsString("Author", "");
            CompanyName = GetManifestDataAsString("CompanyName", "");
            Copyright = GetManifestDataAsString("Copyright", "");
            HelpInfoUri = GetManifestDataAsString("HelpInfoUri", "");
            PowerShellVersion = GetManifestDataAsVersion("PowerShellVersion", null);
            PowerShellHostName = GetManifestDataAsString("PowerShellHostName", "");
            PowerShellHostVersion = GetManifestDataAsVersion("PowerShellHostVersion", null);
            ProcessorArchitecture = moduleInfo.ProcessorArchitecture;
            ClrVersion = GetManifestDataAsVersion("ClrVersion", null);
            DotNetFrameworkVersion = GetManifestDataAsVersion("DotNetFrameworkVersion", null);
            RequiredAssemblies = GetManifestDataAsIEnumerableString("RequiredAssemblies", new string[0]);
            ScriptsToProcess = GetManifestDataAsIEnumerableString("ScriptsToProcess", new string[0]);
            FileList = GetManifestDataAsIEnumerableString("FileList", new string[0]);
            if (_manifestData.ContainsKey("ModuleList"))
                ModuleList = ModuleSpecification.Create(_manifestData["ModuleList"]);
            else
                ModuleList = new object[0];
#else
            Author = moduleInfo.Author;
            CompanyName = moduleInfo.CompanyName;
            Copyright = moduleInfo.Copyright;
            HelpInfoUri = moduleInfo.HelpInfoUri;
            PowerShellVersion = moduleInfo.PowerShellVersion;
            PowerShellHostName = moduleInfo.PowerShellHostName;
            PowerShellHostVersion = moduleInfo.PowerShellHostVersion;
            ProcessorArchitecture = moduleInfo.ProcessorArchitecture;
            ClrVersion = moduleInfo.ClrVersion;
            DotNetFrameworkVersion = moduleInfo.DotNetFrameworkVersion;
            RequiredAssemblies = moduleInfo.RequiredAssemblies;
            ScriptsToProcess = moduleInfo.Scripts;
            FileList = moduleInfo.FileList;
            ModuleList = moduleInfo.ModuleList;
#endif
#if PSV4 || PSV5
            Prefix = moduleInfo.Prefix;
#else
            Prefix = GetManifestDataAsString("Prefix", "");
#endif
#if PSV5
            ExportedDscResources = moduleInfo.ExportedDscResources;
            IconUri = moduleInfo.IconUri;
            LicenseUri = moduleInfo.LicenseUri;
            ProjectUri = moduleInfo.ProjectUri;
            ReleaseNotes = moduleInfo.ReleaseNotes;
            RepositorySourceLocation = moduleInfo.RepositorySourceLocation;
            Tags = moduleInfo.Tags;
#else
#if PSV2
            ExportedDscResources = new ReadOnlyCollection<string>(new List<string>(GetManifestDataAsIEnumerableString("ExportedDscResources", new string[0])));
#else
            ExportedDscResources = new ReadOnlyCollection<string>(GetManifestDataAsIEnumerableString("ExportedDscResources", new string[0]).ToArray());
#endif
            IconUri = GetManifestPrivatePSDataAsUri("IconUri", null);
            LicenseUri = GetManifestPrivatePSDataAsUri("LicenseUri", null);
            ProjectUri = GetManifestPrivatePSDataAsUri("ProjectUri", null);
            ReleaseNotes = GetManifestPrivatePSDataAsString("ReleaseNotes", null);
            RepositorySourceLocation = GetManifestPrivatePSDataAsUri("RepositorySourceLocation", null);
            Tags = GetManifestPrivatePSDataAsIEnumerableString("ReleaseNotes", new string[0]);
#endif

        }
#if !PSV5

        private static IEnumerable<string> EnumerateAsStrings(IEnumerable enumerable)
        {
            foreach (object obj in enumerable)
            {
                if (obj == null)
                    yield return null;

                yield return (obj is string) ? obj as string : obj.ToString();
            }
        }

        private static IEnumerable<string> GetValueAsIEnumerableString(object value, IEnumerable<string> defaultValue)
        {
            if (value == null)
                return defaultValue;
            if (value is string)
                return new string[] { value as string };

            if (value is IEnumerable<string>)
                return value as IEnumerable<string>;

#if !PSV2
            if (value is IEnumerable<object>)
                return (value as IEnumerable<object>).Where(o => o != null).Select(o => (o is string) ? o as string : o.ToString());

#endif
            if (value is IEnumerable)
                return EnumerateAsStrings(value as IEnumerable);

            return new string[] { value.ToString() };
        }

        private static string GetValueAsSingleString(object value, string defaultValue)
        {
            if (value == null)
                return defaultValue;

            string s;
            if (value is string)
                s = value as string;
            else if (value is IEnumerable)
            {
                s = null;
                foreach (string a in GetValueAsIEnumerableString(value, new string[0]))
                {
                    if (a == null)
                        continue;
                    if (a.Length > 0)
                    {
                        s = a;
                        break;
                    }
                    if (s == null)
                        s = a;
                }
            }
            else
                s = value.ToString();

            if (s == null || (s.Trim().Length == 0 && defaultValue != null))
                return defaultValue;

            return s;
        }

        private IEnumerable<string> GetManifestDataAsIEnumerableString(string key, IEnumerable<string> defaultValue)
        {
            if (!_manifestData.ContainsKey(key))
                return defaultValue;

            return GetValueAsIEnumerableString(_manifestData[key], defaultValue);
        }
#if PSV2

        public class ModuleSpecification
        {
            private Guid? _guid = null;
            private string _name = "";
            private Version _version = null;
            public Guid? Guid { get { return _guid; } }
            public string Name { get { return _name; } }
            public Version Version { get { return _version; } }

            private static IEnumerable<object> Enumerate(IEnumerable value)
            {
                foreach (object obj in value)
                {
                    if (obj == null)
                        continue;

                    if (value is string)
                        yield return new ModuleSpecification(value as string);

                    if (value is Hashtable)
                        yield return new ModuleSpecification(value as Hashtable);

                    yield return new ModuleSpecification(value.ToString());
                }
            }

            public static IEnumerable<object> Create(object value)
            {
                if (value == null)
                    return new object[0];

                if (value is string)
                    return new object[] { new ModuleSpecification(value as string) };

                if (value is Hashtable)
                    return new object[] { new ModuleSpecification(value as Hashtable) };

                if (value is IEnumerable)
                    return Enumerate(value as IEnumerable);

                return new object[] { new ModuleSpecification(value.ToString()) };
            }

            public ModuleSpecification(string moduleName) { _name = (moduleName == null) ? "" : moduleName; }

            public ModuleSpecification(Hashtable moduleSpecification)
            {
                if (moduleSpecification == null)
                    return;

                _name = (moduleSpecification.ContainsKey("ModuleName") && moduleSpecification["ModuleName"] == null || 
                    moduleSpecification["ModuleName"] is string) ? moduleSpecification["ModuleName"] as string : moduleSpecification["ModuleName"].ToString();
                if (_name == null || (_name = _name.Trim()).Length == 0)
                {
                    string m = (moduleSpecification.ContainsKey("Name") && moduleSpecification["Name"] == null ||
                        moduleSpecification["Name"] is string) ? moduleSpecification["Name"] as string : moduleSpecification["Name"].ToString();
                    if (m != null)
                        _name = m.Trim();
                    else
                        _name = "";
                }

                string s = (moduleSpecification.ContainsKey("Guid") && moduleSpecification["Guid"] == null ||
                    moduleSpecification["Guid"] is string) ? moduleSpecification["Guid"] as string : moduleSpecification["Guid"].ToString();
                if (s == null || (s = s.Trim()).Length == 0)
                {
                    string m = (moduleSpecification.ContainsKey("GUID") && moduleSpecification["GUID"] == null ||
                        moduleSpecification["GUID"] is string) ? moduleSpecification["GUID"] as string : moduleSpecification["GUID"].ToString();
                    if (m != null)
                        s = m.Trim();
                    else
                        s = "";
                }

                Guid g;
                if (s.Length > 0 && !System.Guid.TryParse(s, out g))
                    _guid = g;
                else
                    _guid = null;

                s = (moduleSpecification.ContainsKey("ModuleVersion") && moduleSpecification["ModuleVersion"] == null ||
                    moduleSpecification["ModuleVersion"] is string) ? moduleSpecification["ModuleVersion"] as string : moduleSpecification["ModuleVersion"].ToString();
                if (s == null || (s = s.Trim()).Length == 0)
                {
                    string m = (moduleSpecification.ContainsKey("Version") && moduleSpecification["Version"] == null ||
                        moduleSpecification["Version"] is string) ? moduleSpecification["Version"] as string : moduleSpecification["Version"].ToString();
                    if (m != null)
                        s = m.Trim();
                    else
                        s = "";
                }

                Version v;
                if (s.Length > 0 && !System.Version.TryParse(s, out v))
                    _version = v;
                else
                    _version = null;
            }
        }

        private string GetManifestDataAsString(string key, string defaultValue)
        {
            if (!_manifestData.ContainsKey(key))
                return defaultValue;

            return GetValueAsSingleString(_manifestData[key], defaultValue);
        }

        private Version GetManifestDataAsVersion(string key, Version defaultValue)
        {
            string s = GetManifestDataAsString(key, "").Trim();
            Version v;

            if (s.Length > 0 && Version.TryParse(s, out v))
                return v;

            return defaultValue;
        }
#endif

        private IEnumerable<string> GetManifestPrivatePSDataAsIEnumerableString(string key, IEnumerable<string> defaultValue)
        {
            if (!_manifestData.ContainsKey("PrivateData"))
                return defaultValue;

            Hashtable privateData = _manifestData["PrivateData"] as Hashtable;

            if (privateData == null || !privateData.ContainsKey("PrivateData") || (privateData = privateData["PSData"] as Hashtable) == null || !privateData.ContainsKey(key))
                return defaultValue;

            return GetValueAsIEnumerableString(privateData[key], defaultValue);
        }

        private string GetManifestPrivatePSDataAsString(string key, string defaultValue)
        {
            if (!_manifestData.ContainsKey("PrivateData"))
                return defaultValue;

            Hashtable privateData = _manifestData["PrivateData"] as Hashtable;

            if (privateData == null || !privateData.ContainsKey("PrivateData") || (privateData = privateData["PSData"] as Hashtable) == null || !privateData.ContainsKey(key))
                return defaultValue;

            return GetValueAsSingleString(privateData[key], defaultValue);
        }

        private Uri GetManifestPrivatePSDataAsUri(string key, Uri defaultValue)
        {
            string s = GetManifestPrivatePSDataAsString(key, "").Trim();
            Uri u;

            if (s.Length > 0 && Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out u))
                return u;

            return defaultValue;
        }

#endif
    }
}