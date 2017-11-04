using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev.HelpXml
{
    public class PSCommandReader
    {
        public const string XmlDoc_IdentifierPrefix_Namespace = "N";
        public const string XmlDoc_IdentifierPrefix_Type = "T";
        public const string XmlDoc_IdentifierPrefix_Field = "F";
        public const string XmlDoc_IdentifierPrefix_Property = "P";
        public const string XmlDoc_IdentifierPrefix_Method = "M";
        public const string XmlDoc_IdentifierPrefix_Event = "E";
        public const string XmlDoc_IdentifierPrefix_ErrorString = "!";
        public const string XmlDoc_AttributeName_name = "name";

        class HelpGenContext
        {
            protected internal XNamespace Xmlns_maml;

            protected internal XNamespace Xmlns_command;

            protected internal XNamespace Xmlns_managed;

            protected internal XNamespace Xmlns_dev;

            protected internal Dictionary<string, XDocument> AssemblyDocCache;
            
            public XDocument HelpItems { get; private set; }

            protected HelpGenContext(HelpGenContext ctx)
            {

            }

            protected internal HelpGenContext()
            {
                Xmlns_maml = XNamespace.Get("http://schemas.microsoft.com/maml/2004/10");
                Xmlns_dev = XNamespace.Get("http://schemas.microsoft.com/maml/dev/2004/10");
                Xmlns_managed = XNamespace.Get("http://schemas.microsoft.com/maml/dev/managed/2004/10");
                Xmlns_command = XNamespace.Get("http://schemas.microsoft.com/maml/dev/command/2004/10");
                AssemblyDocCache = new Dictionary<string, XDocument>();
                HelpItems = new XDocument(new XElement(XName.Get("helpItems", "http://msh"),
                    new XAttribute(XNamespace.Xmlns.GetName("maml"), Xmlns_maml.NamespaceName),
                    new XAttribute(XNamespace.Xmlns.GetName("command"), Xmlns_command.NamespaceName),
                    new XAttribute(XNamespace.Xmlns.GetName("dev"), Xmlns_dev.NamespaceName)));

            }
        }

        class ModuleHelpGenContext : HelpGenContext
        {
            protected internal PSModuleInfo ModuleInfo { get; private set; }

            protected ModuleHelpGenContext(ModuleHelpGenContext ctx) : base(ctx)
            {

            }

            protected internal ModuleHelpGenContext(PSModuleInfo module)
                : base()
            {
                ModuleInfo = module;
            }
        }

        interface ICommandHelpGenContext
        {
            CommandInfo CommandInfo { get; }
        }

        class CommandHelpGenContext<T> : ModuleHelpGenContext, ICommandHelpGenContext
            where T : CommandInfo
        {
            protected internal T CommandInfo { get; private set; }

            CommandInfo ICommandHelpGenContext.CommandInfo { get { return CommandInfo; } }

            public XElement CommandSummaryElement { get; private set; }
            public XElement CommandNameElement { get; private set; }
            public XElement CommandDetailsElement { get; private set; }
            public XElement CommandDescriptionElement { get; private set; }
            public XElement CommandElement { get; private set; }
            public XElement CommandCopyrightElement { get; private set; }
            public XElement CommandVerbElement { get; private set; }
            public XElement CommandNounElement { get; private set; }
            public XElement CommandVersionElement { get; private set; }

            protected CommandHelpGenContext(CommandHelpGenContext<T> ctx) : base(ctx)
            {

            }

            protected internal CommandHelpGenContext(ModuleHelpGenContext ctx, T command)
                : base(ctx)
            {
                CommandNameElement = new XElement(Xmlns_command.GetName("name"));
                CommandSummaryElement = new XElement(Xmlns_maml.GetName("description"));
                CommandCopyrightElement = new XElement(Xmlns_maml.GetName("copyright"));
                CommandVerbElement = new XElement(Xmlns_command.GetName("verb"));
                CommandNounElement = new XElement(Xmlns_command.GetName("noun"));
                CommandVersionElement = new XElement(Xmlns_dev.GetName("version"));
                CommandDetailsElement = new XElement(Xmlns_command.GetName("details"),
                    CommandNameElement, CommandSummaryElement, CommandCopyrightElement,
                    CommandVerbElement, CommandNounElement);
                CommandDescriptionElement = new XElement(Xmlns_maml.GetName("description"));
                CommandElement = new XElement(Xmlns_command.GetName("command"),
                    CommandDetailsElement,
                    CommandDescriptionElement);
            }
        }

        class CmdletHelpGenContext : CommandHelpGenContext<CmdletInfo>
        {
            protected CmdletHelpGenContext(CmdletHelpGenContext ctx) : base(ctx)
            {

            }

            protected internal CmdletHelpGenContext(ModuleHelpGenContext ctx, CmdletInfo command)
                : base(ctx, command)
            {

            }
        }

        class FunctionHelpGenContext : CommandHelpGenContext<FunctionInfo>
        {
            protected FunctionHelpGenContext(FunctionHelpGenContext ctx) : base(ctx)
            {

            }

            protected internal FunctionHelpGenContext(ModuleHelpGenContext ctx, FunctionInfo command)
                : base(ctx, command)
            {

            }
        }

        private XDocument _currentAssemblyDoc = null;
        private XElement _currentCommandNode = null;
        private XElement _ = null;
        private string _currentName;
        private CmdletInfo _currentCmdlet;
        private PSModuleInfo _currentModule;
        private FunctionInfo _currentFunction;
        private Type _currentCmdletType;
        private XElement _cmdletHelpElement;
        private CommandInfo _currentCommand;

        public static XDocument GetHelp(PSModuleInfo moduleInfo)
        {
            return GetHelp(new ModuleHelpGenContext(moduleInfo));
        }

        private static XDocument GetHelp(ModuleHelpGenContext context)
        {
            if(context.ModuleInfo.ExportedCmdlets != null) { }
            if (context.ModuleInfo.ExportedCmdlets != null)
            {
                foreach (string name in context.ModuleInfo.ExportedCmdlets.Keys)
                    context.HelpItems.Root.Add(GetCommandHelp(new CmdletHelpGenContext(context, context.ModuleInfo.ExportedCmdlets[name])));
            }
            if (context.ModuleInfo.ExportedFunctions != null)
            {
                foreach (string name in context.ModuleInfo.ExportedFunctions.Keys)
                    context.HelpItems.Root.Add(GetCommandHelp(new FunctionHelpGenContext(context, context.ModuleInfo.ExportedFunctions[name])));
            }
            return context.HelpItems;
        }

        private static XElement GetCommandHelp(ICommandHelpGenContext context)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, XDocument> _assemblyDocCache;

        private void Load(CmdletInfo cmdletInfo)
        {
            _currentCmdlet = cmdletInfo;
            _currentCommand = cmdletInfo;
            lock (_assemblyDocCache)
            {
                _currentCmdletType = cmdletInfo.ImplementingType;
                string key = _currentCmdletType.Assembly.FullName;
                if (_assemblyDocCache.ContainsKey(key))
                    _currentAssemblyDoc = _assemblyDocCache[key];
                else
                {
                    string path = _currentCmdletType.Assembly.Location;
                    path = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".XML");
                    if (File.Exists(path))
                        _currentAssemblyDoc = XDocument.Load(path);
                    else
                    {
                        _currentAssemblyDoc = new XDocument(new XElement("doc"));
                        _assemblyDocCache.Add(key, _currentAssemblyDoc);
                    }
                }
            }
            LoadCurrentCommand();
        }

        private void Load(FunctionInfo functionInfo)
        {
            _currentFunction = functionInfo;
            _currentCommand = functionInfo;

            lock (_assemblyDocCache)
            {
                _currentCmdletType = null;
                if (_currentAssemblyDoc == null || !_currentAssemblyDoc.Root.IsEmpty)
                    _currentAssemblyDoc = new XDocument(new XElement("doc"));
            }
            LoadCurrentCommand();
        }

        private void LoadCurrentCommand()
        {
            //XElement detailsElement = new XElement(_command.GetName("details"));
            //XElement descriptionElement = new XElement(_maml.GetName("description"));
            //XElement syntaxElement = new XElement(_command.GetName("syntax"));
            //XElement parametersElement = new XElement(_command.GetName("parameters"));
            //_cmdletHelpElement = new XElement(_command.GetName("command"), detailsElement,
            //    descriptionElement, syntaxElement, parametersElement);
            //LoadDetails(detailsElement);
            //Load(_currentCommand.Parameters, parametersElement);
            //Load(_currentCommand.ParameterSets, syntaxElement, parametersElement);
            throw new NotImplementedException();
        }

        private void LoadDetails(XElement detailsElement)
        {
            //detailsElement.Add(new XElement(_command.GetName("name"), _currentCommand.Name));
            //string name = "T:"+_currentCmdletType.FullName;
            //XElement typeDocElement = _currentAssemblyDoc.Elements("doc").Elements("members").Elements("member")
            //    .Attributes("name").Where(a => a.Value == name).Select(a => a.Parent).FirstOrDefault();
            throw new NotImplementedException();
        }

        private void Load(Dictionary<string, ParameterMetadata> parameters, XElement parametersElement)
        {
            throw new NotImplementedException();
        }

        private void Load(ReadOnlyCollection<CommandParameterSetInfo> parameterSets, XElement syntaxElement, XElement parametersElement)
        {
            throw new NotImplementedException();
        }
    }
}