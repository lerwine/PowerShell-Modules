using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev.Commands
{
    /// <summary>
    /// Generates PSMaml help
    /// </summary>
    [Cmdlet(VerbsData.Export, "PSHelpXml")]
    public class Export_PSHelpXml : PSCmdlet
    {
        #region Constants

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member

        public const string HelpXml_FileName_Append = "-Help.ps1xml";
        public const string HelpInfo_FileName_Append = "_HelpInfo.xml";
        public const string XmlNsUri_Msh = "http://msh";
        public const string XmlNsUri_Command = "http://schemas.microsoft.com/maml/dev/command/2004/10";
        public const string XmlNsUri_Maml = "http://schemas.microsoft.com/maml/2004/10";
        public const string XmlNsUri_Dev = "http://schemas.microsoft.com/maml/dev/2004/10";
        public const string XmlNsUri_Managed = "http://schemas.microsoft.com/maml/dev/managed/2004/10";
        public const string XmlNsUri_MSHelp = "http://msdn.microsoft.com/mshelp";
        public const string XmlNsUri_HelpInfo = "http://schemas.microsoft.com/powershell/help/2010/05";
        public static readonly XNamespace XmlNs_Msh = XNamespace.Get(XmlNsUri_Msh);
        public static readonly XNamespace XmlNs_Command = XNamespace.Get(XmlNsUri_Command);
        public static readonly XNamespace XmlNs_Maml = XNamespace.Get(XmlNsUri_Maml);
        public static readonly XNamespace XmlNs_Dev = XNamespace.Get(XmlNsUri_Dev);
        public static readonly XNamespace XmlNs_Managed = XNamespace.Get(XmlNsUri_Managed);
        public static readonly XNamespace XmlNs_MSHelp = XNamespace.Get(XmlNsUri_MSHelp);
        public static readonly XNamespace XmlNs_HelpInfo = XNamespace.Get(XmlNsUri_HelpInfo);

        public const string ParameterSetName_PipelineableModule = "InputModule";
        public const string ParameterSetName_PositionedModule = "ModuleInfo";
        public const string ParameterSetName_Command = "CommandInfo";

        public const string VariableName_HelpItems = "HelpItems";

        public static class AttributeNames
        {
            public const string required = "required";
            public const string variableLength = "variableLength";
            public const string globbing = "globbing";
            public const string pipelineInput = "pipelineInput";
            public const string position = "position";
            public const string aliases = "aliases;";
        }

        public static class ElementNames
        {
            public const string helpItems = "helpItems";
            public const string command = "command";
            public const string details = "details";
            public const string syntax = "syntax";
            public const string parameters = "parameters";
            public const string inputTypes = "inputTypes";
            public const string inputType = "inputType";
            public const string returnValues = "returnValues";
            public const string returnValue = "returnValue";
            public const string terminatingErrors = "terminatingErrors";
            public const string nonTerminatingErrors = "nonTerminatingErrors";
            public const string examples = "examples";
            public const string name = "name";
            public const string synonyms = "synonyms";
            public const string synonym = "synonym";
            public const string verb = "verb";
            public const string noun = "noun";
            public const string vendor = "vendor";
            public const string syntaxItem = "syntaxItem";
            public const string version = "version";
            public const string alertSet = "alertSet";
            public const string description = "description";
            public const string RelatedLinks = "relatedLinks";
            public const string copyright = "copyright";
            public const string para = "para";
        }

#pragma warning restore 1591 // Missing XML comment for publicly visible type or member

        #endregion

        #region Properties

        /// <summary>
        /// <seealso cref="PSModuleInfo"/> object(s) from which to create help XML.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_PipelineableModule)]
        public PSModuleInfo[] InputModule { get; set; }

        /// <summary>
        /// <seealso cref="CommandInfo"/> object(s) from which to create help XML.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = ParameterSetName_Command)]
        [Alias("Command")]
        public CommandInfo[] InputCommand { get; set; }

        /// <summary>
        /// <seealso cref="PSModuleInfo"/> object to be used for creating help XML.
        /// </summary>
        /// <remarks>If the <seealso cref="PSCmdlet.ParameterSetName"/> is <see cref="ParameterSetName_Command"/>, then
        /// this optionally provides context for generating help for <see cref="InputCommand"/> objects;
        /// otherwise, help XML is generated from this object.</remarks>
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = ParameterSetName_PositionedModule)]
        [Parameter(Position = 1, ParameterSetName = ParameterSetName_Command)]
        public PSModuleInfo Module { get; set; }

        /// <summary>
        /// Do not encapsulate all exported commands of each <seealso cref="PSModuleInfo"/> into single elements.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSetName_PipelineableModule)]
        [Parameter(ParameterSetName = ParameterSetName_PositionedModule)]
        public SwitchParameter DoNotEncapsulate { get; set; }

        /// <summary>
        /// Encapsulate all commands into a single element.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSetName_Command)]
        public SwitchParameter Encapsulate { get; set; }

        #endregion

        #region Static Methods

        /// <summary>
        /// Creates an <seealso cref="XElement"/> object for creating a PSMaml help document.
        /// </summary>
        /// <returns>An <seealso cref="XElement"/> object for creating a PSMaml help document.</returns>
        public static XElement CreateHelpItemsElement()
        {
            return new XElement(XmlNs_Msh.GetName(ElementNames.helpItems),
                new XAttribute(XNamespace.Xmlns.GetName("command"), XmlNsUri_Command),
                new XAttribute(XNamespace.Xmlns.GetName("maml"), XmlNsUri_Maml),
                new XAttribute(XNamespace.Xmlns.GetName("dev"), XmlNsUri_Dev),
                new XAttribute(XNamespace.Xmlns.GetName("managed"), XmlNsUri_Managed),
                new XAttribute(XNamespace.Xmlns.GetName("MSHelp"), XmlNsUri_MSHelp),
                new XAttribute(XNamespace.Xmlns.GetName("HelpInfo"), XmlNsUri_HelpInfo));
        }

        /// <summary>
        /// Generates <seealso cref="XElement"/>s for each exported module command.
        /// </summary>
        /// <param name="module"><seealso cref="PSModuleInfo"/> containing exported commands.</param>
        /// <param name="containerElement">If this is not null, then all generated <seealso cref="XElement"/>
        /// objects will be added to this element; otherwise, all generated <seealso cref="XElement"/>
        /// objects will be written to output.</param>
        /// <param name="cmdlet"><seealso cref="Cmdlet"/> which is invoking this method.</param>
        /// <exception cref="ArgumentNullException"><paramref name="module"/> or <paramref name="cmdlet"/> is null.</exception>
        public static void CreatePSMaml(PSModuleInfo module, XElement containerElement, Cmdlet cmdlet)
        {
            if (module == null)
                throw new ArgumentNullException("module");
            if (cmdlet == null)
                throw new ArgumentNullException("cmdlet");

            if (module.ExportedCommands == null)
                return;

            foreach (string name in module.ExportedCommands.Keys)
            {
                if (cmdlet.Stopping)
                    break;
                CommandInfo commandInfo = module.ExportedCommands[name];
                XElement commandElement = CreateCommandElement(name, commandInfo, cmdlet);
                if (containerElement == null)
                    cmdlet.WriteObject(commandElement);
                else
                    containerElement.Add(commandElement);
            }
        }

        /// <summary>
        /// Creates PSMaml help XML for a command.
        /// </summary>
        /// <param name="commandInfo"><seealso cref="CommandInfo"/> object from which to generate help XML.</param>
        /// <param name="cmdlet"><seealso cref="Cmdlet"/> which is invoking this method.</param>
        /// <returns>An <seealso cref="XElement"/> object representing the PSMaml help for the <paramref name="commandInfo"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="commandInfo"/> or <paramref name="cmdlet"/> is null.</exception>
        public static XElement CreateCommandElement(CommandInfo commandInfo, Cmdlet cmdlet)
        {
            if (commandInfo == null)
                throw new ArgumentNullException("commandInfo");
            if (cmdlet == null)
                throw new ArgumentNullException("cmdlet");

            return _CreateCommandElement(commandInfo.Name, commandInfo, cmdlet);
        }

        /// <summary>
        /// Creates PSMaml help XML for a command.
        /// </summary>
        /// <param name="name">Name to use for the command.</param>
        /// <param name="commandInfo"><seealso cref="CommandInfo"/> object from which to generate help XML.</param>
        /// <param name="cmdlet"><seealso cref="Cmdlet"/> which is invoking this method.</param>
        /// <returns>An <seealso cref="XElement"/> object representing the PSMaml help for the <paramref name="commandInfo"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="commandInfo"/> or <paramref name="cmdlet"/> is null.</exception>
        public static XElement CreateCommandElement(string name, CommandInfo commandInfo, Cmdlet cmdlet)
        {
            if (commandInfo == null)
                throw new ArgumentNullException("commandInfo");
            if (cmdlet == null)
                throw new ArgumentNullException("cmdlet");

            return _CreateCommandElement((String.IsNullOrWhiteSpace(name)) ? commandInfo.Name : name, commandInfo, cmdlet);
        }

        /*
         * Help Format: C:\Windows\System32\WindowsPowerShell\v1.0\Help.format.ps1xml
         * Help V3 Format: C:\Windows\System32\WindowsPowerShell\v1.0\HelpV3.format.ps1xml
         * Example file paths:
         *  C:\Windows\System32\WindowsPowerShell\v1.0\Modules\BitsTransfer\en-US\about_BITS_Cmdlets.help.txt
         *  C:\Windows\System32\WindowsPowerShell\v1.0\Modules\BitsTransfer\en-US\Microsoft.BackgroundIntelligentTransfer.Management.dll-Help.xml
         *  
         */
        private static XElement _CreateCommandElement(string name, CommandInfo commandInfo, Cmdlet cmdlet)
        {
            if (commandInfo == null)
                throw new ArgumentNullException("commandInfo");
            if (cmdlet == null)
                throw new ArgumentNullException("cmdlet");
            if (String.IsNullOrWhiteSpace(name))
                name = commandInfo.Name;

            XElement commandElement = new XElement(XmlNs_Command.GetName(ElementNames.command),
                new XElement(XmlNs_Command.GetName(ElementNames.details),
                    new XElement(XmlNs_Command.GetName(ElementNames.name), commandInfo.Name),
                    new XElement(XmlNs_Maml.GetName(ElementNames.description)),
                    new XElement(XmlNs_Maml.GetName(ElementNames.copyright)),
                    new XElement(XmlNs_Command.GetName(ElementNames.verb)),
                    new XElement(XmlNs_Command.GetName(ElementNames.noun)),
                    new XElement(XmlNs_Dev.GetName(ElementNames.version))
                ),
                new XElement(XmlNs_Maml.GetName(ElementNames.description)),
                GetSyntaxElement(commandInfo));
            
            // BUG: Not Implemented
            throw new NotImplementedException();
        }

        private static XElement GetSyntaxElement(CommandInfo commandInfo)
        {
            XElement result = new XElement(XmlNs_Command.GetName(ElementNames.syntax));

            foreach (CommandParameterSetInfo parameterSet in commandInfo.ParameterSets)
            {
                XElement item = new XElement(XmlNs_Command.GetName(ElementNames.syntax),
                    new XElement(XmlNs_Maml.GetName(ElementNames.name), commandInfo.Name));
                result.Add(item);
                foreach (CommandParameterInfo parameterInfo in parameterSet.Parameters)
                {
                    item.Add(new XElement(XmlNs_Command.GetName(ElementNames.syntaxItem),
                        new XAttribute(AttributeNames.required, parameterInfo.IsMandatory),
                        new XAttribute(AttributeNames.variableLength, parameterInfo.ValueFromRemainingArguments),
                        new XAttribute(AttributeNames.globbing, parameterInfo.IsMandatory),
                        new XAttribute(AttributeNames.pipelineInput, parameterInfo.IsMandatory),
                        new XAttribute(AttributeNames.position, parameterInfo.IsMandatory),
                        new XAttribute(AttributeNames.aliases, parameterInfo.IsMandatory)));
                }
            }

            return result;
        }

        #endregion

        #region Overrides

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member

        protected override void BeginProcessing()
        {
            if (ParameterSetName == ParameterSetName_Command && Encapsulate.IsPresent)
                SessionState.PSVariable.Set(new PSVariable(VariableName_HelpItems, CreateHelpItemsElement(), ScopedItemOptions.Private));
            base.BeginProcessing();
        }
        protected override void ProcessRecord()
        {
            XElement helpItems;
            if (ParameterSetName == ParameterSetName_Command)
            {
                if (Encapsulate.IsPresent)
                {
                    object obj = SessionState.PSVariable.GetValue(VariableName_HelpItems);
                    helpItems = ((obj != null && obj is PSObject) ? (obj as PSObject).BaseObject : obj) as XElement;
                }
                else
                    helpItems = null;
                if (InputCommand != null)
                {
                    foreach (CommandInfo command in InputCommand)
                    {
                        XElement element = CreateCommandElement(command, this);
                        if (helpItems == null)
                            WriteObject(element);
                        else
                            helpItems.Add(element);
                    }   
                }
                return;
            }

            helpItems = (DoNotEncapsulate.IsPresent) ? null : CreateHelpItemsElement();
            if (ParameterSetName == ParameterSetName_PositionedModule)
            {
                if (Module != null)
                    CreatePSMaml(Module, helpItems, this);
            }
            else if (InputModule != null)
            {
                foreach (PSModuleInfo module in InputModule)
                {
                    if (module != null)
                        CreatePSMaml(module, helpItems, this);
                }
            }
            if (helpItems != null && !Stopping)
                WriteObject(helpItems);
        }

        protected override void EndProcessing()
        {
            if (ParameterSetName == ParameterSetName_Command && !Stopping && Encapsulate.IsPresent)
            {
                object obj = SessionState.PSVariable.GetValue(VariableName_HelpItems);
                XElement helpItems = ((obj != null && obj is PSObject) ? (obj as PSObject).BaseObject : obj) as XElement;
                if (helpItems != null)
                    WriteObject(helpItems);
            }
            base.EndProcessing();
        }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member

        #endregion
    }
}
