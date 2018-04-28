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
    public class ParameterFactoryContext : CommandFactoryContext
    {
        public ParameterMetadata Parameter { get; private set; }

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

        public XElement AssemblyPropertyHelp
        {
            get { return CommandContext.GetAssemblyPropertyHelp(Parameter.Name); }
        }

        public void EnsureDescriptionContent(CommandHelpFactory factory)
        {
            if (!ParameterDescriptionElement.IsEmpty)
                return;

            string message = Parameter.ParameterSets.Keys.Select(k => Parameter.ParameterSets[k].HelpMessage).FirstOrDefault(s => !String.IsNullOrWhiteSpace(s));

            if (message != null)
                ParameterDescriptionElement.Add(factory.PSHelp.Maml.CreateParagraph(message));
            else
            {
                XElement element = factory.PSHelp.GetSummaryElement(AssemblyPropertyHelp);
                if (element != null)
                    ParameterDescriptionElement.Add(factory.PSHelp.Maml.ConvertAssemblyHelp(element).ToArray());
            }

            if (ParameterDescriptionElement.IsEmpty)
                MamlFactory.EnsureParagraphContent(ParameterDescriptionElement, () => new XComment("Enter description here."));
        }

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

        public ParameterFactoryContext(CommandFactoryContext context, ParameterMetadata parameterMetadata)
            : base(context, context.CommandInfo)
        {
            CommandContext = context;
            Parameter = parameterMetadata;
        }

        public override XElement GetAssemblyPropertyHelp(string propertyName)
        {
            return CommandContext.GetAssemblyPropertyHelp(propertyName);
        }

        /*
<complexType name="parameterType">
<sequence>
<element ref="maml:name"/>
<element ref="maml:description"/>
<choice minOccurs="0">
<element ref="command:parameterValue"/>
<element ref="command:parameterValueGroup"/>
</choice>
<element ref="dev:type" minOccurs="0"/>
<element ref="dev:defaultValue" minOccurs="0"/>
<element ref="dev:possibleValues" minOccurs="0"/>
<element ref="command:validation" minOccurs="0"/>
</sequence>
<attribute name="required" type="boolean" use="required"/>
<attribute name="variableLength" type="boolean" use="required"/>
<attribute name="globbing" type="boolean" use="required"/>
<attribute name="pipelineInput" type="string" use="required"/>
<attribute name="position" type="string" use="required"/>
<attribute ref="command:requiresTrustedData" use="optional"/>
</complexType>
         */
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
