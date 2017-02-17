using Microsoft.PowerShell.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Xml;

namespace PSModuleInstallUtil
{
    public class ModuleManifest
    {
        private string _name = "";
        private Guid? _guid = null;
        private Version _version = null;
        private string _rootModule = "";
        private string _description = "";
        private Collection<PSModuleInfo> _requiredModules = new Collection<PSModuleInfo>();
        private Collection<string> _aliasesToExport = new Collection<string>();
        private Collection<string> _cmdletsToExport = new Collection<string>();
        private Collection<string> _formatsToProcess = new Collection<string>();
        private Collection<string> _functionsToExport = new Collection<string>();
        private Collection<string> _typesToProcess = new Collection<string>();
        private Collection<string> _variablesToExport = new Collection<string>();
        private Collection<PSModuleInfo> _nestedModules = new Collection<PSModuleInfo>();
        private string _author = "";
        private string _companyName = "";
        private string _copyright = "";
        private string _helpInfoUri = "";
        private string _powerShellHostName = "";
        private Collection<string> _requiredAssemblies = new Collection<string>();
        private Collection<string> _scriptsToProcess = new Collection<string>();
        private Collection<string> _fileList = new Collection<string>();
        private Collection<ModuleSpecification> _moduleList = new Collection<ModuleSpecification>();
        private string _defaultCommandPrefix = "";
        private Collection<string> _exportedDscResources = new Collection<string>();
        private Hashtable _parsedModule = new Hashtable();
        /// <summary>
        #region Properties

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
        public string Name { get { return _name; } set { _name = value ?? ""; } }

        public ModuleType ModuleType { get; private set; }
        
#region Common Manifest Properties

        /// <summary>
        /// ID used to uniquely identify this module
        /// </summary>
        public Guid Guid
        {
            get
            {
                if (!_guid.HasValue)
                    _guid = Guid.NewGuid();
                return _guid.Value;
            }
            private set { _guid = value; }
        }

        /// <summary>
        /// Version number of this module.
        /// </summary>
        /// <remarks>Default value is 1.0</remarks>
        public Version Version
        {
            get
            {
                if (_version == null)
                    _version = new Version(1, 0);
                return _version;
            }
            set { _version = value; }
        }

        /// <summary>
        /// Script module or binary module file associated with this manifest. Previous versions of PowerShell called this element the ModuleToProcess.
        /// </summary>
        /// <remarks>Possible types for the root module can be empty (which will make this a Manifest module), the name of a script module (.psm1, which makes this a Script module), or the name of a binary module (.exe or .dll, which makes this a Binary module). Placing the name of a module manifest (.psd1) or a script file (.ps1) in this element will cause an error to occur.</remarks>
        public string RootModule { get { return _rootModule; } set { _rootModule = value ?? ""; } }

        /// <summary>
        /// Description of the functionality provided by this module
        /// </summary>
        public string Description { get { return _description; } set { _description = value ?? ""; } }

        /// <summary>
        /// Modules that must be imported into the global environment prior to importing this module
        /// </summary>
        /// <remarks>Note that this does not actually load any dependent modules – rather, it checks to see if the specified modules have already been loaded. (For example, some modules may already be loaded by a different module.)</remarks>
        public Collection<PSModuleInfo> RequiredModules { get { return _requiredModules; } set { _requiredModules = value ?? new Collection<PSModuleInfo>(); } }

        /// <summary>
        /// Specifies the aliases that the module exports (wildcard characters are permitted) to the caller’s session state.
        /// </summary>
        /// <remarks>By default, all aliases are exported. You can use this key to restrict the aliases that are exported by the module.
        /// The caller’s session state can be the global session state or, for nested modules, the session state of another module.When you are chaining nested modules, all aliases that are exported by a nested module will be ultimately exported to the global session state unless a module in the chain restricts the alias by using the AliasesToExport key.</remarks>
        public Collection<string> AliasesToExport { get { return _aliasesToExport; } set { _aliasesToExport = value ?? new Collection<string>(); } }

        /// <summary>
        /// Specifies the cmdlets that the module exports (wildcard characters are permitted).
        /// </summary>
        /// <remarks>By default, all cmdlets are exported. You can use this key to restrict the cmdlets that are exported by the module.
        /// The caller’s session state can be the global session state or, for nested modules, the session state of another module.When you are chaining nested modules, all cmdlets that are exported by a nested module will be ultimately exported to the global session state unless a module in the chain restricts the cmdlet by using the CmdletsToExport key.
        /// If the manifest also exports aliases for the cmdlets, this key can remove cmdlets whose aliases are listed in the AliasesToExport key, but this key cannot add cmdlet aliases to the list.</remarks>
        public Collection<string> CmdletsToExport { get { return _cmdletsToExport; } set { _cmdletsToExport = value ?? new Collection<string>(); } }

        /// <summary>
        /// Format files (.ps1xml) to be loaded when importing this module.
        /// </summary>
        public Collection<string> FormatsToProcess { get { return _formatsToProcess; } set { _formatsToProcess = value ?? new Collection<string>(); } }

        /// <summary>
        /// Specifies the functions that the module exports (wildcard characters are permitted) to the caller’s session state.
        /// </summary>
        /// <remarks>By default, all functions are exported. You can use this key to restrict the functions that are exported by the module.
        /// The caller’s session state can be the global session state or, for nested modules, the session state of another module.When chaining nested modules, all functions that are exported by a nested module will be exported to the global session state unless a module in the chain restricts the function by using the FunctionsToExport key.
        /// If the manifest also exports aliases for the functions, this key can remove functions whose aliases are listed in the AliasesToExport key, but this key cannot add function aliases to the list.</remarks>
        public Collection<string> FunctionsToExport { get { return _functionsToExport; } set { _functionsToExport = value ?? new Collection<string>(); } }
        /// <summary>
        /// Type files (.ps1xml) to be loaded when importing this module
        /// </summary>
        public Collection<string> TypesToProcess { get { return _typesToProcess; } set { _typesToProcess = value ?? new Collection<string>(); } }

        /// <summary>
        /// Specifies the variables that the module exports (wildcard characters are permitted) to the caller’s session state.
        /// </summary>
        /// <remarks> By default, all variables are exported. You can use this key to restrict the variables that are exported by the module.
        /// The caller’s session state can be the global session state or, for nested modules, the session state of another module.When you are chaining nested modules, all variables that are exported by a nested module will be exported to the global session state unless a module in the chain restricts the variable by using the VariablesToExport key.
        /// If the manifest also exports aliases for the variables, this key can remove variables whose aliases are listed in the AliasesToExport key, but this key cannot add variable aliases to the list.</remarks>
        public Collection<string> VariablesToExport { get { return _variablesToExport; } set { _variablesToExport = value ?? new Collection<string>(); } }

        /// <summary>
        /// Modules to import as nested modules of the module specified in RootModule/ModuleToProcess.
        /// </summary>
        /// <remarks>Adding a module name to this element is similar to calling Import-Module from within your script or assembly code. The main difference is that it’s easier to see what you are loading here in the manifest file. Also, if a module fails to load here, you will not yet have loaded your actual module.
        /// In addition to other modules, you may also load script(.ps1) files here.These files will execute in the context of the root module. (This is equivalent to dot sourcing the script in your root module.)</remarks>
        public Collection<PSModuleInfo> NestedModules { get { return _nestedModules; } set { _nestedModules = value ?? new Collection<PSModuleInfo>(); } }

        // TODO: Need to be able to serialize private data back out more reliably when module manifest is created.

        public const string ModuleDataKey_PrivateData = "PrivateData";

        // TODO: We should need to only extract private data once (when it is loaded).
        /// <summary>
        /// Specifies any private data that needs to be passed to the root module specified by the RootModule/ModuleToProcess key.
        /// </summary>
        public object PrivateData
        {
            get { return (_parsedModule.ContainsKey(ModuleDataKey_PrivateData)) ? _parsedModule[ModuleDataKey_PrivateData] : null; }
            set
            {
                if (value == null)
                {
                    if (_parsedModule.ContainsKey(ModuleDataKey_PrivateData))
                    {
                        if (_parsedModule[ModuleDataKey_PrivateData] != null && _parsedModule[ModuleDataKey_PrivateData] is Hashtable)
                        {
                            Hashtable p = _parsedModule[ModuleDataKey_PrivateData] as Hashtable;
                            if (p.ContainsKey(PrivateDataMetaKey_PSData) && p[PrivateDataMetaKey_PSData] != null && p[PrivateDataMetaKey_PSData] is Hashtable)
                            {
                                foreach (object key in p.Keys.Cast<object>().Where(k => !(k is string || PrivateDataMetaKey_PSData.Equals(k as string))).ToArray())
                                    p.Remove(key);
                                return;
                            }
                        }
                        _parsedModule.Remove(ModuleDataKey_PrivateData);
                    }
                    return;
                }
                if (!_parsedModule.ContainsKey(ModuleDataKey_PrivateData) || _parsedModule[ModuleDataKey_PrivateData] == null || !(value is Hashtable && _parsedModule[ModuleDataKey_PrivateData] is Hashtable && (_parsedModule[ModuleDataKey_PrivateData] as Hashtable).ContainsKey(PrivateDataMetaKey_PSData) && !(value as Hashtable).ContainsKey(PrivateDataMetaKey_PSData)))
                {
                    _parsedModule[ModuleDataKey_PrivateData] = value;
                    return;
                }
                
                Hashtable t = _parsedModule[ModuleDataKey_PrivateData] as Hashtable;
                _parsedModule[ModuleDataKey_PrivateData] = value;
                (_parsedModule[ModuleDataKey_PrivateData] as Hashtable)[PrivateDataMetaKey_PSData] = t[PrivateDataMetaKey_PSData];
            }
        }

#endregion

#region PowerShell 3.0+ Properties

        /// <summary>
        /// Author of this module
        /// </summary>
        public string Author { get { return _author; } set { _author = value ?? ""; } }

        /// <summary>
        /// Company or vendor of this module.
        /// </summary>
        /// <remarks>Default value is &quot;Unknown&quot;</remarks>
        public string CompanyName { get { return _companyName; } set { _companyName = value ?? ""; } }

        /// <summary>
        /// Copyright statement for this module.
        /// </summary>
        /// <remarks>Default value is &quot;© [currentYear] [Author]. All rights reserved.&quot;</remarks>
        public string Copyright { get { return _copyright; } set { _copyright = value ?? ""; } }

        /// <summary>
        /// HelpInfo URI of this module.
        /// </summary>
        public string HelpInfoUri { get { return _helpInfoUri; } set { _helpInfoUri = value ?? ""; } }

        /// <summary>
        /// Minimum version of the Windows PowerShell engine required by this module.
        /// </summary>
        public Version PowerShellVersion { get; set; }

        /// <summary>
        /// Specifies the name of the Windows PowerShell host that is required by the module.
        /// </summary>
        public string PowerShellHostName { get { return _powerShellHostName; } set { _powerShellHostName = value ?? ""; } }

        /// <summary>
        /// Minimum version of the Windows PowerShell host required by this module
        /// </summary>
        public Version PowerShellHostVersion { get; set; }

        /// <summary>
        /// Processor architecture (None, X86, Amd64) required by this module
        /// </summary>
        public ProcessorArchitecture ProcessorArchitecture { get; set; }

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
        public Collection<string> RequiredAssemblies { get { return _requiredAssemblies; } set { _requiredAssemblies = value ?? new Collection<string>(); } }
        
        /// Script (.ps1) files that are run in the caller's session state when the module is imported.
        /// </summary>
        /// <remarks>These scripts are run before any of the modules listed in the manifest are loaded</remarks>
        public Collection<string> ScriptsToProcess { get { return _scriptsToProcess; } set { _scriptsToProcess = value ?? new Collection<string>(); } }

        /// <summary>
        /// List of all files packaged with this module
        /// </summary>
        /// <remarks>As with ModuleList, FileList is to assist you as an inventory list, and is not otherwise processed.</remarks>
        public Collection<string> FileList { get { return _fileList; } set { _fileList = value ?? new Collection<string>(); } }

        /// <summary>
        /// Specifies all the modules that are packaged with this module.
        /// </summary>
        /// <remarks>These modules can be entered by name (a comma-separated string) or as a hash table with ModuleName and GUID keys. The hash table can also have an optional ModuleVersion key. The ModuleList key is designed to act as a module inventory. These modules are not automatically processed.</remarks>
        public Collection<ModuleSpecification> ModuleList { get { return _moduleList; } set { _moduleList = value ?? new Collection<ModuleSpecification>(); } }

#endregion

#region PowerShell 4.0+ Properties

        /// <summary>
        /// Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
        /// </summary>
        public string DefaultCommandPrefix { get { return _defaultCommandPrefix; } set { _defaultCommandPrefix = value ?? ""; } }

#endregion

#region PowerShell 5.0+ Properties

        public const string PrivateDataMetaKey_PSData = "PSData";
        public const string PrivateDataMetaKey_IconUri = "IconUri";
        public const string PrivateDataMetaKey_LicenseUri = "LicenseUri";
        public const string PrivateDataMetaKey_ProjectUri = "ProjectUri";
        public const string PrivateDataMetaKey_ReleaseNotes = "ReleaseNotes";
        public const string PrivateDataMetaKey_RepositorySourceLocation = "RepositorySourceLocation";
        public const string PrivateDataMetaKey_Tags = "Tags";
        public const string ErrorMessage_PrivateDataConflict = "The PrivateData parameter value must be a hash table to create the module manifest with the following parameter values Tags, ProjectUri, LicenseUri, IconUri or ReleaseNotes. Either remove the Tags, ProjectUri, LicenseUri, IconUri, or ReleaseNotes parameter values or wrap the contents of PrivateData in a hashtable.";
        public const string ErrorMessage_PSDataConflict = "The PSData m ember of the PrivateData parameter value must be a hash table to create the module manifest with the following parameter values Tags, ProjectUri, LicenseUri, IconUri or ReleaseNotes. Either remove the Tags, ProjectUri, LicenseUri, IconUri, or ReleaseNotes parameter values or remove the PSData key.";

        /// <summary>
        /// Exported DSC resources.
        /// </summary>
        public Collection<string> ExportedDscResources { get { return _exportedDscResources; } set { _exportedDscResources = value ?? new Collection<string>(); } }

        /// <summary>
        /// Icon Uri of this module.
        /// </summary>
        public Uri IconUri
        {
            get
            {
                if (PrivateData != null && PrivateData is Hashtable)
                {
                    Hashtable p = PrivateData as Hashtable;
                    if (p[PrivateDataMetaKey_PSData] != null && p[PrivateDataMetaKey_PSData] is Hashtable)
                    {
                        p = p[PrivateDataMetaKey_PSData] as Hashtable;
                        if (p[PrivateDataMetaKey_IconUri] != null)
                        {
                            if (p[PrivateDataMetaKey_IconUri] is Uri)
                                return p[PrivateDataMetaKey_IconUri] as Uri;
                            if (p[PrivateDataMetaKey_IconUri] is string)
                                p[PrivateDataMetaKey_IconUri] = new Uri(p[PrivateDataMetaKey_IconUri] as string, UriKind.RelativeOrAbsolute);
                            else
                                p[PrivateDataMetaKey_IconUri] = new Uri(p[PrivateDataMetaKey_IconUri].ToString(), UriKind.RelativeOrAbsolute);
                            return p[PrivateDataMetaKey_IconUri] as Uri;
                        }
                    }
                }

                return null;
            }
            set
            {
                if (PrivateData == null)
                    PrivateData = new Hashtable();
                else if (!(PrivateData is Hashtable))
                    throw new InvalidOperationException(ErrorMessage_PrivateDataConflict);
                Hashtable p = PrivateData as Hashtable;
                if (!p.ContainsKey(PrivateDataMetaKey_PSData))
                    p[PrivateDataMetaKey_PSData] = new Hashtable();
                else if (!(p[PrivateDataMetaKey_PSData] is Hashtable))
                    throw new InvalidOperationException(ErrorMessage_PSDataConflict);
                p = p[PrivateDataMetaKey_PSData] as Hashtable;
                if (value != null)
                    p[PrivateDataMetaKey_IconUri] = value;
                else if (p.ContainsKey(PrivateDataMetaKey_IconUri))
                    p.Remove(PrivateDataMetaKey_IconUri);
            }
        }

        /// <summary>
        /// License Uri of this module
        /// </summary>
        public Uri LicenseUri
        {
            get
            {
                if (PrivateData != null && PrivateData is Hashtable)
                {
                    Hashtable p = PrivateData as Hashtable;
                    if (p[PrivateDataMetaKey_PSData] != null && p[PrivateDataMetaKey_PSData] is Hashtable)
                    {
                        p = p[PrivateDataMetaKey_PSData] as Hashtable;
                        if (p[PrivateDataMetaKey_LicenseUri] != null)
                        {
                            if (p[PrivateDataMetaKey_LicenseUri] is Uri)
                                return p[PrivateDataMetaKey_LicenseUri] as Uri;
                            if (p[PrivateDataMetaKey_LicenseUri] is string)
                                p[PrivateDataMetaKey_LicenseUri] = new Uri(p[PrivateDataMetaKey_LicenseUri] as string, UriKind.RelativeOrAbsolute);
                            else
                                p[PrivateDataMetaKey_LicenseUri] = new Uri(p[PrivateDataMetaKey_LicenseUri].ToString(), UriKind.RelativeOrAbsolute);
                            return p[PrivateDataMetaKey_LicenseUri] as Uri;
                        }
                    }
                }

                return null;
            }
            set
            {
                if (PrivateData == null)
                    PrivateData = new Hashtable();
                else if (!(PrivateData is Hashtable))
                    throw new InvalidOperationException(ErrorMessage_PrivateDataConflict);
                Hashtable p = PrivateData as Hashtable;
                if (!p.ContainsKey(PrivateDataMetaKey_PSData))
                    p[PrivateDataMetaKey_PSData] = new Hashtable();
                else if (!(p[PrivateDataMetaKey_PSData] is Hashtable))
                    throw new InvalidOperationException(ErrorMessage_PSDataConflict);
                p = p[PrivateDataMetaKey_PSData] as Hashtable;
                if (value != null)
                    p[PrivateDataMetaKey_LicenseUri] = value;
                else if (p.ContainsKey(PrivateDataMetaKey_LicenseUri))
                    p.Remove(PrivateDataMetaKey_LicenseUri);
            }
        }

        /// <summary>
        /// Project Uri of this module
        /// </summary>
        public Uri ProjectUri
        {
            get
            {
                if (PrivateData != null && PrivateData is Hashtable)
                {
                    Hashtable p = PrivateData as Hashtable;
                    if (p[PrivateDataMetaKey_PSData] != null && p[PrivateDataMetaKey_PSData] is Hashtable)
                    {
                        p = p[PrivateDataMetaKey_PSData] as Hashtable;
                        if (p[PrivateDataMetaKey_ProjectUri] != null)
                        {
                            if (p[PrivateDataMetaKey_ProjectUri] is Uri)
                                return p[PrivateDataMetaKey_ProjectUri] as Uri;
                            if (p[PrivateDataMetaKey_ProjectUri] is string)
                                p[PrivateDataMetaKey_ProjectUri] = new Uri(p[PrivateDataMetaKey_ProjectUri] as string, UriKind.RelativeOrAbsolute);
                            else
                                p[PrivateDataMetaKey_ProjectUri] = new Uri(p[PrivateDataMetaKey_ProjectUri].ToString(), UriKind.RelativeOrAbsolute);
                            return p[PrivateDataMetaKey_ProjectUri] as Uri;
                        }
                    }
                }

                return null;
            }
            set
            {
                if (PrivateData == null)
                    PrivateData = new Hashtable();
                else if (!(PrivateData is Hashtable))
                    throw new InvalidOperationException(ErrorMessage_PrivateDataConflict);
                Hashtable p = PrivateData as Hashtable;
                if (!p.ContainsKey(PrivateDataMetaKey_PSData))
                    p[PrivateDataMetaKey_PSData] = new Hashtable();
                else if (!(p[PrivateDataMetaKey_PSData] is Hashtable))
                    throw new InvalidOperationException(ErrorMessage_PSDataConflict);
                p = p[PrivateDataMetaKey_PSData] as Hashtable;
                if (value != null)
                    p[PrivateDataMetaKey_ProjectUri] = value;
                else if (p.ContainsKey(PrivateDataMetaKey_ProjectUri))
                    p.Remove(PrivateDataMetaKey_ProjectUri);
            }
        }

        /// <summary>
        /// Release notes for this module
        /// </summary>
        public string ReleaseNotes
        {
            get
            {
                if (PrivateData != null && PrivateData is Hashtable)
                {
                    Hashtable p = PrivateData as Hashtable;
                    if (p[PrivateDataMetaKey_PSData] != null && p[PrivateDataMetaKey_PSData] is Hashtable)
                    {
                        p = p[PrivateDataMetaKey_PSData] as Hashtable;
                        if (p[PrivateDataMetaKey_ReleaseNotes] != null)
                        {
                            if (p[PrivateDataMetaKey_ReleaseNotes] is string)
                                return p[PrivateDataMetaKey_ReleaseNotes] as string;
                            if (p[PrivateDataMetaKey_ReleaseNotes] is string[])
                                p[PrivateDataMetaKey_ReleaseNotes] = String.Join(" ", p[PrivateDataMetaKey_ReleaseNotes] = p[PrivateDataMetaKey_ReleaseNotes] as string[]);
                            else if (p[PrivateDataMetaKey_ReleaseNotes] is IEnumerable<string>)
                                p[PrivateDataMetaKey_ReleaseNotes] = String.Join(" ", p[PrivateDataMetaKey_ReleaseNotes] = p[PrivateDataMetaKey_ReleaseNotes] as IEnumerable<string>);
                            else
                                p[PrivateDataMetaKey_ReleaseNotes] = p[PrivateDataMetaKey_ReleaseNotes].ToString();
                            return p[PrivateDataMetaKey_ReleaseNotes] as string;
                        }
                    }
                }

                return "";
            }
            set
            {
                if (PrivateData == null)
                    PrivateData = new Hashtable();
                else if (!(PrivateData is Hashtable))
                    throw new InvalidOperationException(ErrorMessage_PrivateDataConflict);
                Hashtable p = PrivateData as Hashtable;
                if (!p.ContainsKey(PrivateDataMetaKey_PSData))
                    p[PrivateDataMetaKey_PSData] = new Hashtable();
                else if (!(p[PrivateDataMetaKey_PSData] is Hashtable))
                    throw new InvalidOperationException(ErrorMessage_PSDataConflict);
                p = p[PrivateDataMetaKey_PSData] as Hashtable;
                if (!String.IsNullOrEmpty(value))
                    p[PrivateDataMetaKey_ReleaseNotes] = value;
                else if (p.ContainsKey(PrivateDataMetaKey_ReleaseNotes))
                    p.Remove(PrivateDataMetaKey_ReleaseNotes);
            }
        }

        /// <summary>
        /// Repository source location for this module
        /// </summary>
        public Uri RepositorySourceLocation
        {
            get
            {
                if (PrivateData != null && PrivateData is Hashtable)
                {
                    Hashtable p = PrivateData as Hashtable;
                    if (p[PrivateDataMetaKey_PSData] != null && p[PrivateDataMetaKey_PSData] is Hashtable)
                    {
                        p = p[PrivateDataMetaKey_PSData] as Hashtable;
                        if (p[PrivateDataMetaKey_RepositorySourceLocation] != null)
                        {
                            if (p[PrivateDataMetaKey_RepositorySourceLocation] is Uri)
                                return p[PrivateDataMetaKey_RepositorySourceLocation] as Uri;
                            if (p[PrivateDataMetaKey_RepositorySourceLocation] is string)
                                p[PrivateDataMetaKey_RepositorySourceLocation] = new Uri(p[PrivateDataMetaKey_RepositorySourceLocation] as string, UriKind.RelativeOrAbsolute);
                            else
                                p[PrivateDataMetaKey_RepositorySourceLocation] = new Uri(p[PrivateDataMetaKey_RepositorySourceLocation].ToString(), UriKind.RelativeOrAbsolute);
                            return p[PrivateDataMetaKey_RepositorySourceLocation] as Uri;
                        }
                    }
                }

                return null;
            }
            set
            {
                if (PrivateData == null)
                    PrivateData = new Hashtable();
                else if (!(PrivateData is Hashtable))
                    throw new InvalidOperationException(ErrorMessage_PrivateDataConflict);
                Hashtable p = PrivateData as Hashtable;
                if (!p.ContainsKey(PrivateDataMetaKey_PSData))
                    p[PrivateDataMetaKey_PSData] = new Hashtable();
                else if (!(p[PrivateDataMetaKey_PSData] is Hashtable))
                    throw new InvalidOperationException(ErrorMessage_PSDataConflict);
                p = p[PrivateDataMetaKey_PSData] as Hashtable;
                if (value != null)
                    p[PrivateDataMetaKey_RepositorySourceLocation] = value;
                else if (p.ContainsKey(PrivateDataMetaKey_RepositorySourceLocation))
                    p.Remove(PrivateDataMetaKey_RepositorySourceLocation);
            }
        }

        /// <summary>
        /// Tags for this module
        /// </summary>
        public Collection<string> Tags
        {
            get
            {
                if (PrivateData != null && PrivateData is Hashtable)
                {
                    Hashtable p = PrivateData as Hashtable;
                    if (p[PrivateDataMetaKey_PSData] != null && p[PrivateDataMetaKey_PSData] is Hashtable)
                    {
                        p = p[PrivateDataMetaKey_PSData] as Hashtable;
                        if (p[PrivateDataMetaKey_Tags] != null)
                        {
                            if (p[PrivateDataMetaKey_Tags] is Collection<string>)
                                return p[PrivateDataMetaKey_Tags] as Collection<string>;
                            if (p[PrivateDataMetaKey_Tags] is string[])
                                p[PrivateDataMetaKey_Tags] = new Collection<string>((p[PrivateDataMetaKey_Tags] as string[]).ToList());
                            else if (p[PrivateDataMetaKey_Tags] is string)
                                p[PrivateDataMetaKey_Tags] = new Collection<string>((new string[] { p[PrivateDataMetaKey_Tags] as string }).ToList());
                            if (p[PrivateDataMetaKey_Tags] is IEnumerable<string>)
                                p[PrivateDataMetaKey_Tags] = new Collection<string>((p[PrivateDataMetaKey_Tags] as IEnumerable<string>).ToList());
                            if (p[PrivateDataMetaKey_Tags] is IEnumerable)
                                p[PrivateDataMetaKey_Tags] = new Collection<string>((p[PrivateDataMetaKey_Tags] as IEnumerable).Cast<object>().Where(o => o != null).Select(o => (o is string) ? o as string : o.ToString()).ToList());
                            else
                                p[PrivateDataMetaKey_Tags] = new Collection<string>((new string[] { p[PrivateDataMetaKey_Tags].ToString() }).ToList());
                            return p[PrivateDataMetaKey_Tags] as Collection<string>;
                        }
                    }
                }

                return null;
            }
            set
            {
                if (PrivateData == null)
                    PrivateData = new Hashtable();
                else if (!(PrivateData is Hashtable))
                    throw new InvalidOperationException(ErrorMessage_PrivateDataConflict);
                Hashtable p = PrivateData as Hashtable;
                if (!p.ContainsKey(PrivateDataMetaKey_PSData))
                    p[PrivateDataMetaKey_PSData] = new Hashtable();
                else if (!(p[PrivateDataMetaKey_PSData] is Hashtable))
                    throw new InvalidOperationException(ErrorMessage_PSDataConflict);
                p = p[PrivateDataMetaKey_PSData] as Hashtable;
                if (value != null && value.Count  > 0)
                    p[PrivateDataMetaKey_Tags] = value;
                else if (p.ContainsKey(PrivateDataMetaKey_Tags))
                    p.Remove(PrivateDataMetaKey_Tags);
            }
        }

#endregion

#endregion

        public const string FileExtension_PSProject = ".psproj";
        public const string FileExtension_ModuleManifest = ".psd1";
        public const string Xmlns_VsProject = "http://schemas.microsoft.com/developer/msbuild/2003";

        public ModuleManifest(PSModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
                throw new ArgumentNullException("moduleInfo");

            if (String.IsNullOrEmpty(moduleInfo.Path) || !(ManifestFileInfo = new FileInfo(moduleInfo.Path)).Exists)
                throw new ArgumentException("Module Information object does not represent a module manifest.");

            Initialize(moduleInfo);
        }

        public ModuleManifest(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            FileInfo manifestFileInfo = new FileInfo(path);

            if (String.Compare(manifestFileInfo.Extension, FileExtension_PSProject, true) == 0)
            {
                if (!manifestFileInfo.Directory.Exists)
                    throw new ArgumentException("Project directory does not exist.");
                FileInfo projectFileInfo = manifestFileInfo;
                manifestFileInfo = new FileInfo(Path.Combine(projectFileInfo.DirectoryName, Path.GetFileNameWithoutExtension(projectFileInfo.Name) + FileExtension_ModuleManifest));
                FileInfo namedFileInfo = manifestFileInfo;
                XmlDocument xmlDocument = new XmlDocument();

                Exception projectLoadError = null;
                XmlNamespaceManager nsmgr = null;
                try
                {
                    xmlDocument.Load(projectFileInfo.FullName);
                    nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
                    nsmgr.AddNamespace("p", Xmlns_VsProject);
                }
                catch (Exception exception) { projectLoadError = exception; }
                Dictionary<string, FileInfo> projectModuleFiles = new Dictionary<string, FileInfo>();

                if (projectLoadError != null)
                {
                    XmlElement element = xmlDocument.SelectSingleNode("/p:Project/p:PropertyGroup[count(@Condition)=0]/p:Name", nsmgr) as XmlElement;
                    if (element != null && !element.IsEmpty)
                    {
                        string p = element.InnerText.Trim();
                        if (p.Length > 0)
                        {
                            try { namedFileInfo = new FileInfo(Path.Combine(projectFileInfo.DirectoryName, p + FileExtension_ModuleManifest)); }
                            catch { }
                        }
                    }

                    foreach (XmlAttribute attribute in xmlDocument.SelectNodes("/p:Project/p:ItemGroup/p:Compile/@Include", nsmgr))
                    {
                        string p = attribute.Value.Trim();
                        if (p.Length > 0)
                        {
                            try
                            {
                                FileInfo f = new FileInfo(Path.Combine(projectFileInfo.DirectoryName, p));
                                if (String.Compare(f.Extension, FileExtension_ModuleManifest, true) == 0)
                                    projectModuleFiles.Add(f.Name.ToLower(), f);
                            }
                            catch { }
                        }
                    }
                }

                if (namedFileInfo.Exists && projectModuleFiles.ContainsKey(namedFileInfo.Name.ToLower()))
                    ManifestFileInfo = namedFileInfo;
                else if (String.Compare(namedFileInfo.Name, manifestFileInfo.Name, true) != 0 && manifestFileInfo.Exists && projectModuleFiles.ContainsKey(manifestFileInfo.Name.ToLower()))
                    ManifestFileInfo = manifestFileInfo;
                else
                {
                    FileInfo f = null;
                    List<string> allNames = new List<string>(projectModuleFiles.Keys);
                    foreach (string n in allNames)
                    {
                        if (f == null)
                            f = projectModuleFiles[n];
                        if (projectModuleFiles[n].Exists)
                            f = projectModuleFiles[n];
                        else
                            projectModuleFiles.Remove(n);
                    }
                    if (projectModuleFiles.Count == 1)
                        ManifestFileInfo = f;
                    else if (!namedFileInfo.Exists && manifestFileInfo.Exists)
                        ManifestFileInfo = manifestFileInfo;
                    else
                        ManifestFileInfo = namedFileInfo;
                }
                if (!ManifestFileInfo.Exists)
                {
                    if (projectLoadError != null)
                        throw new ArgumentException("Module manifest not found", "path", projectLoadError);

                    throw new ArgumentException("Module manifest not found", "path");
                }
            }
            else
            {
                ManifestFileInfo = new FileInfo(path);
                if (!ManifestFileInfo.Exists)
                    throw new ArgumentException("Manifest file does not exist.");
            }
            
            Collection<PSObject> result;
            InitialSessionState iss = InitialSessionState.CreateDefault();
            // this.TestContext;
            using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
            {
                runspace.Open();
                runspace.SessionStateProxy.SetVariable("TestModulePath", ManifestFileInfo.FullName);
                using (PowerShell powershell = PowerShell.Create(RunspaceMode.NewRunspace))
                {
                    powershell.Runspace = runspace;
                    powershell.AddScript("Test-ModuleManifest -Path $TestModulePath;");
                    result = powershell.Invoke();
                    if (powershell.HadErrors)
                    {
                        Collection<ErrorRecord> errors = powershell.Streams.Error.ReadAll();
                        if (errors.Count == 1)
                            throw new Exception(errors[0].CategoryInfo.GetMessage(), errors[0].Exception);

                        throw new AggregateException("Multiple errors encountered: See inner exceptions for details.", errors.Select(e => e.Exception));
                    }
                }
            }

            PSModuleInfo moduleInfo;
            if (result.Count == 0 || result[0] == null || (moduleInfo = result[0].BaseObject as PSModuleInfo) == null)
                throw new Exception("PSModuleInfo object not returned.");

            Initialize(moduleInfo);
        }

        private void Initialize(PSModuleInfo moduleInfo)
        {
            Author = moduleInfo.Author;
            ClrVersion = moduleInfo.ClrVersion;
            CompanyName = moduleInfo.CompanyName;
            Copyright = moduleInfo.Copyright;
            Description = moduleInfo.Description;
            DotNetFrameworkVersion = moduleInfo.DotNetFrameworkVersion;
            AliasesToExport = new Collection<string>(moduleInfo.ExportedAliases.Keys.ToList());
            CmdletsToExport = new Collection<string>(moduleInfo.ExportedCmdlets.Keys.ToList());
            FormatsToProcess = new Collection<string>(moduleInfo.ExportedFormatFiles.ToList());
            FunctionsToExport = new Collection<string>(moduleInfo.ExportedFunctions.Keys.ToList());
            TypesToProcess = new Collection<string>(moduleInfo.ExportedTypeFiles.ToList());
            VariablesToExport = new Collection<string>(moduleInfo.ExportedVariables.Keys.ToList());
            // WorkflowsToExport = new Collection<string>(moduleInfo.ExportedWorkflows.Keys.ToList());
            FileList = new Collection<string>(moduleInfo.FileList.ToList());
            Guid = moduleInfo.Guid;
            HelpInfoUri = moduleInfo.HelpInfoUri;
            ModuleList = new Collection<ModuleSpecification>(moduleInfo.ModuleList.Where(m => m != null).Select(m =>
            {
                if (m is ModuleSpecification)
                    return m as ModuleSpecification;

                if (m is string)
                    return new ModuleSpecification(m as string);

                if (m is Hashtable)
                    return new ModuleSpecification(m as Hashtable);

                return new ModuleSpecification(m.ToString());
            }).ToList());
            ModuleType = moduleInfo.ModuleType;
            Name = moduleInfo.Name;
            NestedModules = new Collection<PSModuleInfo>(moduleInfo.NestedModules.ToList());
            PowerShellHostName = moduleInfo.PowerShellHostName;
            PowerShellHostVersion = moduleInfo.PowerShellHostVersion;
            PowerShellVersion = moduleInfo.PowerShellVersion;
            DefaultCommandPrefix = moduleInfo.Prefix;
            ProcessorArchitecture = moduleInfo.ProcessorArchitecture;
            RequiredAssemblies = new Collection<string>(moduleInfo.RequiredAssemblies.ToList());
            RequiredModules = new Collection<PSModuleInfo>(moduleInfo.RequiredModules.ToList());
            RootModule = moduleInfo.RootModule;
            ScriptsToProcess = new Collection<string>(moduleInfo.Scripts.ToList());
            Version = moduleInfo.Version;
        }
    }
}

#if PSV2
namespace Microsoft.PowerShell.Commands
{
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
}
#endif