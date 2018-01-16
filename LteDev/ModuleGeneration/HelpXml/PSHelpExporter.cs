using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace LteDev.HelpXml
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class PSHelpExporter
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
    {
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
        public static readonly XNamespace Xmlns_msh = "http://msh";
        public static readonly XNamespace Xmlns_maml = "http://schemas.microsoft.com/maml/2004/10";
        public static readonly XNamespace Xmlns_command = "http://schemas.microsoft.com/maml/dev/command/2004/10";
        public static readonly XNamespace Xmlns_dev = "http://schemas.microsoft.com/maml/dev/2004/10";
        public static readonly XNamespace Xmlns_MSHelp = "http://msdn.microsoft.com/mshelp";
        public const string NCName_name = "name";
        public const string NCName_description = "description";
        public const string NCName_para = "para";
        public const string NCName_helpItems = "helpItems";
        public const string AttributeName_schema = "schema";
        public const string HelpFileNameAppend = "-Help.xml";
        public const string NCName_list = "list";
        public const string NCName_listItem = "listItem";
        public const string NCName_table = "table";
        public const string NCName_tableHeader = "tableHeader";
        public const string NCName_row = "row";
        public const string NCName_entry = "entry";
        public const string NCName_command = "command";
        public const string NCName_details = "details";
        public const string NCName_copyright = "copyright";
        public const string NCName_verb = "verb";
        public const string NCName_version = "version";
        public const string NCName_noun = "noun";
        public const string NCName_vendor = "vendor";
        public const string NCName_products = "products";
        public const string NCName_product = "products";
        public const string NCName_syntax = "syntax";
        public const string NCName_parameters = "parameters";
        public const string NCName_inputTypes = "inputTypes";
        public const string NCName_returnValues = "returnValues";
        public const string NCName_terminatingErrors = "terminatingErrors";
        public const string NCName_nonTerminatingErrors = "nonTerminatingErrors";
        public const string NCName_alertSet = "alertSet";
        public const string NCName_examples = "examples";
        public const string NCName_relatedLinks = "relatedLinks";
        public const string NCName_parameter = "parameter";
        public const string NCName_members = "members";
        public const string NCName_member = "member";
        public const string NCName_summary = "summary";
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member

        private XDocument _helpItemsDocument;
        private Commands.Export_PSHelpXml _hostCmdlet;
        private Assembly _rootModuleAssembly;
        private PSModuleInfo _rootModuleInfo;
        private Dictionary<string, PSModuleInfo> _allModules = new Dictionary<string, PSModuleInfo>();
        private Dictionary<string, XDocument> _allAssemblyDocuments = new Dictionary<string, XDocument>();
        private Dictionary<string, Tuple<Type, CmdletAttribute>> _allCmdletAttributes = new Dictionary<string, Tuple<Type, CmdletAttribute>>();

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
        public XElement HelpItemsElement { get { return _helpItemsDocument.Root; } }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
        public PSHelpExporter(Commands.Export_PSHelpXml hostCmdlet)
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
        {
            _hostCmdlet = hostCmdlet;
        }
        
        internal void ProcessRecord(PSModuleInfo moduleInfo)
        {
            _helpItemsDocument = new XDocument(Xmlns_msh.GetName(NCName_helpItems));
            _rootModuleInfo = moduleInfo;
            if (moduleInfo.ModuleType == ModuleType.Binary && !String.IsNullOrWhiteSpace(moduleInfo.Path) && File.Exists(moduleInfo.Path))
                _rootModuleAssembly = Assembly.LoadFrom(moduleInfo.Path);
            else
                _rootModuleAssembly = null;

            ProcessModule(moduleInfo);
        }

        private void ProcessModule(PSModuleInfo moduleInfo)
        {
            if (moduleInfo.ExportedCmdlets != null)
            {
                foreach (KeyValuePair<string, CmdletInfo> kvp in moduleInfo.ExportedCmdlets)
                    ProcessCmdlet(kvp.Value);
            }
            if (moduleInfo.ExportedFunctions != null)
            {
                foreach (KeyValuePair<string, FunctionInfo> kvp in moduleInfo.ExportedFunctions)
                    ProcessFunction(kvp.Value);
            }
        }

        private void ProcessCmdlet(CmdletInfo value)
        {
            if (_helpItemsDocument.Root.Elements(Xmlns_command.GetName(NCName_command)).Elements(Xmlns_command.GetName(NCName_details))
                    .Elements(Xmlns_command.GetName(NCName_name)).Any(e => !e.IsEmpty && e.Value.Trim() == value.Name))
                return;
            string key = value.ImplementingType.Assembly.Location;
            XDocument assemblyDocument;
            if (_allAssemblyDocuments.ContainsKey(key))
                assemblyDocument = _allAssemblyDocuments[key];
            else
            {
                string path = Path.Combine(Path.GetDirectoryName(key), Path.GetFileNameWithoutExtension(key) + ".XML");
                assemblyDocument = null;
                if (File.Exists(path))
                {
                    try { assemblyDocument = XDocument.Load(path); }
                    catch { }
                }
                _allAssemblyDocuments.Add(key, assemblyDocument);
            }
            XElement helpDescription = new XElement(Xmlns_maml.GetName(NCName_description));
            XElement helpDetails = new XElement(Xmlns_command.GetName(NCName_details),
                new XElement(Xmlns_command.GetName(NCName_name), value.Name),
                helpDescription
            );
            XElement helpCommand = new XElement(Xmlns_command.GetName(NCName_command),
                helpDetails
            );
            XElement assemblyType;
            if (assemblyDocument == null)
                assemblyType = null;
            else
            {
                key = "T:" + value.ImplementingType.FullName;
                assemblyType = assemblyDocument.Root.Elements(NCName_members).Elements(NCName_member).Attributes(NCName_name).Where(a => a.Value == key)
                    .Select(a => a.Parent).FirstOrDefault();
            }
            XElement assemblySummary;
            if (assemblyType == null)
                assemblySummary = null;
            else
                assemblySummary = assemblyType.Elements(NCName_summary).Where(e => !e.IsEmpty && e.Elements().Any() || e.Value.Trim().Length > 0).FirstOrDefault();
            if (assemblySummary == null)
                helpDescription.Add(new XElement(Xmlns_maml.GetName(NCName_para), new XComment("Synoposis goes here")));
            else
                AddMamlBlockOnly(assemblySummary.Nodes(), helpDescription);
#warning Not implemented
            throw new NotImplementedException();
        }

        private void AddMamlBlockOnly(IEnumerable<XNode> enumerable, XElement helpDescription)
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        private void ToMaml(IEnumerator<XNode> enumerator, XElement target)
        {
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is XComment)
                {
                    target.Add(enumerator.Current);
                    continue;
                }

                //if (enumerator.Current is XText || enumerator.Current is XCData)
                //{
                //    if (target.IsHelpDocPara())
                //}
#warning Not implemented
                throw new NotImplementedException();
            }
        }

        private void AddMamlInlineOnly(IEnumerable<XNode> source, XElement target)
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        private void AddMamlList(XElement listElement, XElement target)
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        private void AddMamlBlock(XElement blockElement, XElement target)
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        private void ProcessFunction(FunctionInfo value)
        {
            if (_helpItemsDocument.Root.Elements(Xmlns_command.GetName(NCName_command)).Elements(Xmlns_command.GetName(NCName_details))
                    .Elements(Xmlns_command.GetName(NCName_name)).Any(e => !e.IsEmpty && e.Value.Trim() == value.Name))
                return;
        }
    }
}