using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev.HelpXml
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class SyntaxParameterFactoryContext : CommandFactoryContext
    {
        public CommandParameterInfo Parameter { get; private set; }

        public CommandFactoryContext CommandContext { get; private set; }

        public override string Noun { get { return CommandContext.Noun; } }

        public override string Verb { get { return CommandContext.Verb; } }

        public bool? Required { get; set; }

        public bool? VariableLength { get; set; }

        public bool? Globbing { get; set; }

        public bool? DontShow { get; set; }

        public int? Position { get; set; }

        public bool? ValueFromPipeline { get; set; }

        public bool? ValueFromPipelineByPropertyName { get; set; }

        public bool? ValueFromRemainingArguments { get; set; }

        private XElement _parameterDescriptionElement = null;

        public XElement ParameterDescriptionElement
        {
            get
            {
                if (_parameterDescriptionElement == null)
                    _parameterDescriptionElement = new XElement(MamlFactory.XmlNs_maml.GetName(MamlFactory.description));
                return _parameterDescriptionElement;
            }
        }

        public override AssemblyContext AssemblyContext { get { return CommandContext.AssemblyContext; } }

        public override XElement AssemblyClassHelp { get { return CommandContext.AssemblyClassHelp; } }

        public SyntaxParameterFactoryContext(CommandFactoryContext context, CommandParameterInfo parameterInfo)
            : base(context, context.CommandInfo)
        {
            CommandContext = context;
            Parameter = parameterInfo;
        }

        public override XElement GetAssemblyPropertyHelp(string propertyName) { return CommandContext.GetAssemblyPropertyHelp(propertyName); }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
