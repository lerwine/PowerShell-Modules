using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace PSModuleInstallUtil
{
    public class PsProject : INotifyPropertyChanged
    {
        public const string xmlns = "http://schemas.microsoft.com/developer/msbuild/2003";
        public const string ElementName_Project = "Project";
        public const string ElementName_PropertyGroup = "PropertyGroup";

        public const string ElementName_ProjectGuid = "ProjectGuid";

        public Guid ProjectGuid
        {
            get
            {
                Guid? value = GetElementGuid(_defaultPropertyGroupElement, ElementName_ProjectGuid);
                if (!value.HasValue)
                    value = Guid.NewGuid();
                SetElementGuid(_defaultPropertyGroupElement, ElementName_ProjectGuid, value);
                return value.Value;
            }
        }

        public const string ElementName_Name = "Name";

        public string Name
        {
            get
            {
                string value = GetElementString(_defaultPropertyGroupElement, ElementName_Name, "");
                if (value == null)
                    value = Path.GetFileNameWithoutExtension(_fileName.Name);
                SetElementString(_defaultPropertyGroupElement, ElementName_Name, value);
                return value;
            }
            set { SetElementString(_defaultPropertyGroupElement, ElementName_Name, value); }
        }

        public const string ElementName_Author = "Author";

        public string Author
        {
            get { return GetElementString(_defaultPropertyGroupElement, ElementName_Author, ""); }
            set { SetElementString(_defaultPropertyGroupElement, ElementName_Name, value); }
        }

        public const string ElementName_CompanyName = "CompanyName";

        public string CompanyName
        {
            get { return GetElementString(_defaultPropertyGroupElement, ElementName_CompanyName, ""); }
            set { SetElementString(_defaultPropertyGroupElement, ElementName_CompanyName, value); }
        }

        public const string ElementName_Copyright = "Copyright";

        public string Copyright
        {
            get { return GetElementString(_defaultPropertyGroupElement, ElementName_Copyright, ""); }
            set { SetElementString(_defaultPropertyGroupElement, ElementName_Copyright, value); }
        }

        public const string ElementName_Description = "Description";

        public string Description
        {
            get { return GetElementString(_defaultPropertyGroupElement, ElementName_Description, ""); }
            set { SetElementString(_defaultPropertyGroupElement, ElementName_Description, value); }
        }

        public const string ElementName_ModuleGuid = "Guid";

        public Guid ModuleGuid
        {
            get
            {
                Guid? value = GetElementGuid(_defaultPropertyGroupElement, ElementName_ModuleGuid);
                if (!value.HasValue)
                    value = ProjectGuid;
                SetElementGuid(_defaultPropertyGroupElement, ElementName_ModuleGuid, value);
                return value.Value;
            }
        }

        public const string ElementName_FormatsToProcess = "FormatsToProcess";

        public string[] FormatsToProcess
        {
            get { return GetElementStringArray(_defaultPropertyGroupElement, ElementName_FormatsToProcess, new string[0]); }
            set { SetElementStringArray(_defaultPropertyGroupElement, ElementName_FormatsToProcess, value); }
        }

        public const string ElementName_FunctionsToProcess = "FunctionsToProcess";

        public string[] FunctionsToProcess
        {
            get { return GetElementStringArray(_defaultPropertyGroupElement, ElementName_FunctionsToProcess, new string[0]); }
            set { SetElementStringArray(_defaultPropertyGroupElement, ElementName_FunctionsToProcess, value); }
        }

        public const string ElementName_ModuleList = "ModuleList";

        public string[] ModuleList
        {
            get { return GetElementStringArray(_defaultPropertyGroupElement, ElementName_ModuleList, new string[0]); }
            set { SetElementStringArray(_defaultPropertyGroupElement, ElementName_ModuleList, value); }
        }

        public const string ElementName_ModuleToProcess = "ModuleToProcess";

        public string ModuleToProcess
        {
            get
            {
                string value = GetElementString(_defaultPropertyGroupElement, ElementName_ModuleToProcess, "");
                if (value == null)
                    value = Path.GetFileNameWithoutExtension(_fileName.Name);
                SetElementString(_defaultPropertyGroupElement, ElementName_ModuleToProcess, value);
                return value;
            }
            set { SetElementString(_defaultPropertyGroupElement, ElementName_ModuleToProcess, value); }
        }

        public const string ElementName_NestedModules = "NestedModules";

        public string[] NestedModules
        {
            get { return GetElementStringArray(_defaultPropertyGroupElement, ElementName_NestedModules, new string[0]); }
            set { SetElementStringArray(_defaultPropertyGroupElement, ElementName_NestedModules, value); }
        }

        public const string ElementName_TypesToProcess = "TypesToProcess";

        public string[] TypesToProcess
        {
            get { return GetElementStringArray(_defaultPropertyGroupElement, ElementName_TypesToProcess, new string[0]); }
            set { SetElementStringArray(_defaultPropertyGroupElement, ElementName_TypesToProcess, value); }
        }

        public const string ElementName_ClrVersion = "ClrVersion";

        public Guid? ClrVersion
        {
            get { return GetElementGuid(_defaultPropertyGroupElement, ElementName_ClrVersion); }
            set { SetElementGuid(_defaultPropertyGroupElement, ElementName_ClrVersion, value); }
        }

        public const string ElementName_PowerShellVersion = "PowerShellVersion";

        public Guid? PowerShellVersion
        {
            get { return GetElementGuid(_defaultPropertyGroupElement, ElementName_PowerShellVersion); }
            set { SetElementGuid(_defaultPropertyGroupElement, ElementName_PowerShellVersion, value); }
        }

        public const string ElementName_PowerShellHostVersion = "PowerShellHostVersion";

        public Guid? PowerShellHostVersion
        {
            get { return GetElementGuid(_defaultPropertyGroupElement, ElementName_PowerShellHostVersion); }
            set { SetElementGuid(_defaultPropertyGroupElement, ElementName_PowerShellHostVersion, value); }
        }

        public const string ElementName_ProcessorArchitecture = "ProcessorArchitecture";
        public ProcessorArchitecture? ProcessorArchitecture
        {
            get
            {
                string s = GetElementString(_defaultPropertyGroupElement, ElementName_Description, "");
                ProcessorArchitecture value;
                if (s.Length == 0 || !Enum.TryParse<ProcessorArchitecture>(s, true, out value))
                    return null;
                return value;
            }
            set { SetElementString(_defaultPropertyGroupElement, ElementName_Description, (value.HasValue) ? value.Value.ToString("F") : null); }
        }

        public const string ElementName_RequiredModules = "RequiredModules";

        public string[] RequiredModules
        {
            get { return GetElementStringArray(_defaultPropertyGroupElement, ElementName_RequiredModules, new string[0]); }
            set { SetElementStringArray(_defaultPropertyGroupElement, ElementName_RequiredModules, value); }
        }
        
        public const string ElementName_VariablesToExport = "VariablesToExport";

        public string[] VariablesToExport
        {
            get { return GetElementStringArray(_defaultPropertyGroupElement, ElementName_VariablesToExport, new string[0]); }
            set { SetElementStringArray(_defaultPropertyGroupElement, ElementName_VariablesToExport, value); }
        }

        public const string ElementName_CmdletsToExport = "CmdletsToExport";

        public string[] CmdletsToExport
        {
            get { return GetElementStringArray(_defaultPropertyGroupElement, ElementName_CmdletsToExport, new string[0]); }
            set { SetElementStringArray(_defaultPropertyGroupElement, ElementName_CmdletsToExport, value); }
        }

        public const string ElementName_AliasesToExport = "AliasesToExport";

        public string[] AliasesToExport
        {
            get { return GetElementStringArray(_defaultPropertyGroupElement, ElementName_AliasesToExport, new string[0]); }
            set { SetElementStringArray(_defaultPropertyGroupElement, ElementName_AliasesToExport, value); }
        }

        public const string ElementName_WarningLevel = "WarningLevel";
        public const string ElementName_DebugSymbols = "DebugSymbols";

        public ConditionalPropertyGroup DebugPropertyGroup { get { return _debugPropertyGroup; } }

        public ConditionalPropertyGroup ReleasePropertyGroup { get { return _releasePropertyGroup; } }

        public sealed class ConditionalPropertyGroup : INotifyPropertyChanged
        {
            internal ConditionalPropertyGroup(XmlElement element) { _element = element; }

            private XmlElement _element;

            public event PropertyChangedEventHandler PropertyChanged;

            public string[] DebugSymbols
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public int? WarningLevel
            {
                get
                {
                    string s = GetElementString(_element, ElementName_Description, "");
                    int value;
                    if (s.Length == 0 || !System.Int32.TryParse(s, out value))
                        return null;
                    return value;
                }
                set { SetElementString(_element, ElementName_Description, (value.HasValue) ? value.Value.ToString() : null); }
            }

            private void RaisePropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                    handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public const string ElementName_DefineConstants = "DefineConstants";
        public const string ElementName_ItemGroup = "ItemGroup";
        public const string ElementName_Content = "Content";
        public const string ElementName_Compile = "Compile";
        public const string ElementName_Configuration = "Configuration";
        public const string AttributeName_Condition = "Condition";
        public const string AttributeName_Include = "Include";
        public const string ConfigurationName_Debug = "Debug";
        public const string ConfigurationName_Release = "Release";
        public const string PlatformName_AnyCPU = "AnyCPU";

        public static readonly Regex ConfigurationConditionRegex = new Regex(@"^\s*'\$\(Configuration\)\|\$\(Platform\)'\s*==\s*'(?<configuration>Debug|Release)\|(?<platform>[^\(\)']+)'\s*$", RegexOptions.Compiled);
        private XmlDocument _xmlDocument;
        private FileInfo _fileName;
        private XmlNamespaceManager _nsmgr;
        private XmlElement _defaultPropertyGroupElement;
        private ConditionalPropertyGroup _debugPropertyGroup;
        private ConditionalPropertyGroup _releasePropertyGroup;
        private XmlElement _contentItemGroup = null;
        private XmlElement _compileItemGroup = null;

        public event PropertyChangedEventHandler PropertyChanged;

        internal static string GetElementString(XmlElement parentElement, string elementName, string defaultValue)
        {
            throw new NotImplementedException();
        }

        internal static string GetElementString(XmlElement parentElement, string elementName)
        {
            throw new NotImplementedException();
        }

        internal static string[] GetElementStringArray(XmlElement parentElement, string elementName, string[] defaultValue)
        {
            throw new NotImplementedException();
        }

        internal static string[] GetElementStringArray(XmlElement parentElement, string elementName)
        {
            throw new NotImplementedException();
        }

        internal static void SetElementString(XmlElement parentElement, string elementName, string value)
        {
            throw new NotImplementedException();
        }

        internal static void SetElementStringArray(XmlElement parentElement, string elementName, string[] values)
        {
            throw new NotImplementedException();
        }

        internal static Guid? GetElementGuid(XmlElement parentElement, string elementName)
        {
            throw new NotImplementedException();
        }

        internal static void SetElementGuid(XmlElement parentElement, string elementName, Guid? value)
        {
            throw new NotImplementedException();
        }

        public PsProject(FileInfo fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            XmlDocument xmlDocument = new XmlDocument();
            _fileName = fileName;
            if (fileName.Exists)
            {
                xmlDocument.Load(fileName.FullName);
                if (xmlDocument.DocumentElement == null)
                    throw new ArgumentException("No XML data loaded.");
                if (xmlDocument.DocumentElement.LocalName != ElementName_Project || xmlDocument.DocumentElement.NamespaceURI != xmlns)
                    throw new ArgumentException("Not a PowerShell project.");
            }
            else
                xmlDocument.AppendChild(xmlDocument.CreateElement(ElementName_Project, xmlns));
            Initialize(xmlDocument);
        }

        private PsProject(XmlDocument xmlDocument, FileInfo fileName)
        {
            _fileName = fileName;
            Initialize(xmlDocument);
        }

        private void Initialize(XmlDocument xmlDocument)
        {
            _xmlDocument = xmlDocument;
            _nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            _nsmgr.AddNamespace("p", xmlns);
            _defaultPropertyGroupElement = xmlDocument.SelectSingleNode("/p:Project/p:PropertyGroup[count(@Condition)=0]", _nsmgr) as XmlElement;
            if (_defaultPropertyGroupElement == null)
                AddDefaultPropertyGroup();
            var nodes = xmlDocument.SelectNodes("/p:Project/p:PropertyGroup/@Condition", _nsmgr).OfType<XmlAttribute>()
                .Select(a => new { Element = a.OwnerElement, Match = ConfigurationConditionRegex.Match(a.Value) }).Where(a => a.Match.Success)
                .Select(a => new { Element = a.Element, Configuration = a.Match.Groups["configuration"].Value, Platform = a.Match.Groups["platform"].Value });

            var m = nodes.FirstOrDefault(n => n.Configuration == ConfigurationName_Debug && n.Platform == PlatformName_AnyCPU);
            if (m == null)
                m = nodes.FirstOrDefault(n => n.Configuration == ConfigurationName_Debug);
            if (m == null)
                _debugPropertyGroup = new ConditionalPropertyGroup(AddConditionalPropertyGroup(true));
            else
                _debugPropertyGroup = new ConditionalPropertyGroup(m.Element);

            m = nodes.FirstOrDefault(n => n.Configuration == ConfigurationName_Release && n.Platform == PlatformName_AnyCPU);
            if (m == null)
                m = nodes.FirstOrDefault(n => n.Configuration == ConfigurationName_Release);
            if (m == null)
                _releasePropertyGroup = new ConditionalPropertyGroup(AddConditionalPropertyGroup(false));
            else
                _releasePropertyGroup = new ConditionalPropertyGroup(m.Element);

            _contentItemGroup = xmlDocument.SelectSingleNode("/p:Project/p:ItemGroup[not(count(p:Content)=0)])]", _nsmgr) as XmlElement;
            _compileItemGroup = xmlDocument.SelectSingleNode("/p:Project/p:ItemGroup[not(count(p:Compile)=0)])]", _nsmgr) as XmlElement;
        }

        private XmlElement AddConditionalPropertyGroup(bool isDebug)
        {
            XmlElement element = _xmlDocument.InsertBefore(_xmlDocument.CreateElement(ElementName_PropertyGroup), _defaultPropertyGroupElement) as XmlElement;
            if (isDebug)
                element.Attributes.Append(_xmlDocument.CreateAttribute(AttributeName_Condition)).Value = " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ";
            else
                element.Attributes.Append(_xmlDocument.CreateAttribute(AttributeName_Condition)).Value = " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ";
            if (isDebug)
                (element.AppendChild(_xmlDocument.CreateElement(ElementName_DebugSymbols, xmlns)) as XmlElement).AppendChild(_xmlDocument.CreateTextNode(XmlConvert.ToString(isDebug)));
            (element.AppendChild(_xmlDocument.CreateElement("DebugType", xmlns)) as XmlElement).AppendChild(_xmlDocument.CreateTextNode((isDebug) ? "full" : "pdbonly"));
            (element.AppendChild(_xmlDocument.CreateElement("Optimize", xmlns)) as XmlElement).AppendChild(_xmlDocument.CreateTextNode(XmlConvert.ToString(!isDebug)));
            if (isDebug)
            {
                (element.AppendChild(_xmlDocument.CreateElement("OutputPath", xmlns)) as XmlElement).AppendChild(_xmlDocument.CreateTextNode("bin\\Debug\\"));
                (element.AppendChild(_xmlDocument.CreateElement(ElementName_DefineConstants, xmlns)) as XmlElement).AppendChild(_xmlDocument.CreateTextNode("DEBUG;TRACE"));
            }
            else
            {
                (element.AppendChild(_xmlDocument.CreateElement("OutputPath", xmlns)) as XmlElement).AppendChild(_xmlDocument.CreateTextNode("bin\\Release\\"));
                (element.AppendChild(_xmlDocument.CreateElement(ElementName_DefineConstants, xmlns)) as XmlElement).AppendChild(_xmlDocument.CreateTextNode("TRACE"));
            }
            (element.AppendChild(_xmlDocument.CreateElement("ErrorReport", xmlns)) as XmlElement).AppendChild(_xmlDocument.CreateTextNode("prompt"));
            (element.AppendChild(_xmlDocument.CreateElement(ElementName_WarningLevel, xmlns)) as XmlElement).AppendChild(_xmlDocument.CreateTextNode("4"));
            return element;
        }

        private void AddDefaultPropertyGroup()
        {
            XmlElement refChild = _xmlDocument.SelectSingleNode("/p:*[0]", _nsmgr) as XmlElement;

            if (refChild == null)
                _defaultPropertyGroupElement = _xmlDocument.AppendChild(_xmlDocument.CreateElement(ElementName_PropertyGroup)) as XmlElement;
            else
                _defaultPropertyGroupElement = _xmlDocument.InsertBefore(_xmlDocument.CreateElement(ElementName_PropertyGroup), refChild) as XmlElement;

            refChild = _defaultPropertyGroupElement.AppendChild(_xmlDocument.CreateElement(ElementName_Configuration, xmlns)) as XmlElement;
            refChild.Attributes.Append(_xmlDocument.CreateAttribute(AttributeName_Condition)).Value = " '$(Configuration)' == '' ";
            refChild.AppendChild(_xmlDocument.CreateTextNode(ConfigurationName_Debug));
            (_defaultPropertyGroupElement.AppendChild(_xmlDocument.CreateElement("SchemaVersion", xmlns)) as XmlElement).AppendChild(_xmlDocument.CreateTextNode("2.0"));
            (_defaultPropertyGroupElement.AppendChild(_xmlDocument.CreateElement(ElementName_ProjectGuid, xmlns)) as XmlElement).AppendChild(_xmlDocument.CreateTextNode(Guid.NewGuid().ToString("b")));
            (_defaultPropertyGroupElement.AppendChild(_xmlDocument.CreateElement("OutputType", xmlns)) as XmlElement).AppendChild(_xmlDocument.CreateTextNode("Exe"));
            (_defaultPropertyGroupElement.AppendChild(_xmlDocument.CreateElement("RootNamespace", xmlns)) as XmlElement).AppendChild(_xmlDocument.CreateTextNode("MyApplication"));
            (_defaultPropertyGroupElement.AppendChild(_xmlDocument.CreateElement("AssemblyName", xmlns)) as XmlElement).AppendChild(_xmlDocument.CreateTextNode("MyApplication"));
        }

        public static IEnumerable<PsProject> FindProjects(DirectoryInfo directory, int maxDepth)
        {
            if (directory == null || !directory.Exists)
                yield break;

            FileInfo[] files = directory.GetFiles("*.pssproj", SearchOption.TopDirectoryOnly);
            if (files == null || files.Length == 0)
                yield break;

            foreach (FileInfo f in files)
            {
                PsProject result = PsProject.Load(f);
                if (result != null)
                    yield return result;
            }
        }

        public static PsProject Load(FileInfo fileInfo)
        {
            if (fileInfo == null || !fileInfo.Exists)
                return null;

            XmlDocument xmlDocument = new XmlDocument();
            try { xmlDocument.Load(fileInfo.FullName); }
            catch { return null; }
            if (xmlDocument.DocumentElement == null || xmlDocument.DocumentElement.LocalName != ElementName_Project || xmlDocument.DocumentElement.NamespaceURI != xmlns)
                return null;
            return new PsProject(xmlDocument, fileInfo);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
            try { OnPropertyChanged(args); }
            finally
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                    handler.Invoke(this, args);
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) { }
    }
}