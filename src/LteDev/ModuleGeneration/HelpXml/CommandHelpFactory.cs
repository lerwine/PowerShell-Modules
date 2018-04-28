using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LteDev.HelpXml
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class CommandHelpFactory : PSHelpFactoryBase
    {
        public const string XmlNsUri_command = "http://schemas.microsoft.com/maml/dev/command/2004/10";
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

        public static readonly XNamespace XmlNs_command = XNamespace.Get(XmlNsUri_command);

        public ParameterFactory ParameterFactoryInstance { get; private set; }

        public SyntaxParameterFactory SyntaxFactory { get; private set; }
        
        internal CommandHelpFactory(PSHelpFactory psHelpFactory) : base(psHelpFactory)
        {
            ParameterFactoryInstance = new ParameterFactory(this);
            SyntaxFactory = new SyntaxParameterFactory(this);
        }

        internal IEnumerable<XElement> GetCommandHelp(PSHelpFactoryContext context)
        {
            CommandFactoryContext[] contextItems = context.ModuleInfo.ExportedCmdlets.Keys.Select(n => new CmdletFactoryContext(context, context.ModuleInfo.ExportedCmdlets[n]))
                .Cast<CommandFactoryContext>().Concat(context.ModuleInfo.ExportedFunctions.Keys.Select(n => new FunctionFactoryContext(context, context.ModuleInfo.ExportedFunctions[n]))
                .Cast<CommandFactoryContext>()).ToArray();
            foreach (string name in context.ModuleInfo.ExportedAliases.Keys)
                AddCommandAlias(context.ModuleInfo.ExportedAliases[name], contextItems);
            return contextItems.Select(i => GetCommandHelp(i));
        }
        
        private XElement GetCommandHelp(CommandFactoryContext context)
        {
            return new XElement(XmlNs_command.GetName(command), GetCommandHelpNodes(context).ToArray());
        }

        private IEnumerable<XElement> GetCommandDetailsNodes(CommandFactoryContext context)
        {
            yield return new XElement(XmlNs_command.GetName(name), context.CommandInfo.Name);
            if (context.SynopsisElement.IsEmpty)
            {
                XElement element = PSHelp.GetSummaryElement(context.AssemblyClassHelp);
                if (element != null)
                {
                    XElement[] elements = PSHelp.Maml.ConvertAssemblyHelp(element).ToArray();
                    if (elements.Length > 0)
                        context.SynopsisElement.Add(elements);
                }
            }
            MamlFactory.EnsureParagraphContent(context.SynopsisElement, () => new XComment("Enter synopsis here."));
            yield return context.SynopsisElement;
            IEnumerable<string> aliases = context.Aliases.Where(a => !String.IsNullOrWhiteSpace(a));
            if (aliases.Any())
                yield return new XElement(XmlNs_command.GetName(synonyms), aliases.Select(a => new XElement(XmlNs_command.GetName(synonym), a)).ToArray());
            if (context.CopyrightElement.IsEmpty)
            {
                if (String.IsNullOrWhiteSpace(context.ModuleInfo.Copyright))
                {
                    if (!String.IsNullOrWhiteSpace(context.AssemblyContext.Copyright))
                        context.CopyrightElement.Add(PSHelp.Maml.CreateParagraph(context.AssemblyContext.Copyright));
                }
                else
                    context.CopyrightElement.Add(PSHelp.Maml.CreateParagraph(context.ModuleInfo.Copyright));
            }
            MamlFactory.EnsureParagraphContent(context.CopyrightElement, () => new XComment("Enter copyright here."));
            yield return context.CopyrightElement;
            yield return new XElement(XmlNs_command.GetName(noun), context.Noun);
            yield return new XElement(XmlNs_command.GetName(verb), context.Verb);
            if (context.VersionElement.IsEmpty)
            {
                Version version = context.ModuleInfo.Version;
                if (version == null)
                    version = context.AssemblyContext.AssemblyVersion;
                if (version != null)
                {
                    if (version.Revision > 0)
                        context.VersionElement.Add(new XText(version.ToString()));
                    else
                        context.VersionElement.Add(new XText(version.ToString((version.Build > 0) ? 3 : 2)));
                }
            }
            EnsureContent(context.VersionElement, () => new XComment("Enter version here."));
            yield return context.VersionElement;
            if (context.VendorElement.IsEmpty)
            {
                if (String.IsNullOrWhiteSpace(context.ModuleInfo.CompanyName))
                {
                    if (!String.IsNullOrWhiteSpace(context.AssemblyContext.Company))
                        context.CopyrightElement.Add(PSHelp.Maml.CreateName(context.AssemblyContext.Company));
                }
                else
                    context.CopyrightElement.Add(PSHelp.Maml.CreateName(context.ModuleInfo.CompanyName));
            }
                yield return context.VendorElement;
        }

        private IEnumerable<XElement> GetCommandHelpNodes(CommandFactoryContext context)
        {
            yield return new XElement(XmlNs_command.GetName(details), GetCommandDetailsNodes(context).ToArray());
            if (context.CommandDescriptionElement.IsEmpty)
            {
                XElement element = PSHelp.GetDescriptionElement(context.AssemblyClassHelp);
                if (element != null)
                {
                    XElement[] elements = PSHelp.Maml.ConvertAssemblyHelp(element).ToArray();
                    if (elements.Length > 0)
                        context.CommandDescriptionElement.Add(elements);
                }
            }
            MamlFactory.EnsureParagraphContent(context.CommandDescriptionElement, () => new XComment("Enter description here."));
            yield return context.CommandDescriptionElement;
            ParameterFactoryContext[] allParameters = ParameterFactoryInstance.CreateParameterContextItems(context);
            yield return SyntaxFactory.GetSyntaxElement(context, allParameters);
            if (allParameters.Length > 0)
            {
                yield return ParameterFactoryInstance.GetParametersElement(allParameters);
                IGrouping<string, CommandParameterInfo>[] inputTypeArr = SyntaxFactory.GetInputTypes(context).ToArray();
                if (inputTypeArr.Length > 0)
                    yield return new XElement(XmlNs_command.GetName(inputTypes), inputTypeArr.Select(g => new XElement(XmlNs_command.GetName(inputType), GetInputTypeContent(context, g.Key, g, allParameters))).ToArray());
            }
        }
        

        private IEnumerable<XElement> GetInputTypeContent(CommandFactoryContext context, string parameterName, IEnumerable<CommandParameterInfo> syntaxParameters, ParameterFactoryContext[] allParameters)
        {
            yield return PSHelp.Dev.CreateTypeElement(syntaxParameters.First().ParameterType);
            string txt = syntaxParameters.Select(p => p.HelpMessage).FirstOrDefault(s => !String.IsNullOrWhiteSpace(s));
            if (txt != null)
                yield return PSHelp.Maml.CreateParagraph(txt);
            else
            {
                ParameterFactoryContext pc = allParameters.FirstOrDefault(p => p.Parameter.Name == parameterName);
                if (pc != null)
                {
                    pc.EnsureDescriptionContent(this);
                    if (pc.CommandDescriptionElement.HasElements)
                    {
                        foreach (XElement e in pc.CommandDescriptionElement.Elements())
                            yield return e.CreateClone();
                    }
                }
            }
        }

        private void AddCommandAlias(AliasInfo aliasInfo, CommandFactoryContext[] contextItems)
        {
            if ((aliasInfo.CommandType != CommandTypes.Cmdlet && aliasInfo.CommandType != CommandTypes.Function) || aliasInfo.ReferencedCommand == null)
                return;

            CommandFactoryContext context = contextItems.FirstOrDefault(i => i.CommandInfo.Name == aliasInfo.ReferencedCommand.Name);
            if (context != null)
                context.Aliases.Add(aliasInfo.Name);
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
