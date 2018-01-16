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
    public class SyntaxParameterFactory : PSHelpFactoryBase
    {
        public SyntaxParameterFactory(CommandHelpFactory commandHelpFactory)
            : base(commandHelpFactory.PSHelp)
        {
        }

        internal XElement GetSyntaxElement(CommandFactoryContext context, ParameterFactoryContext[] parameterContextItems)
        {
            return new XElement(CommandHelpFactory.XmlNs_command.GetName(CommandHelpFactory.syntax), context.CommandInfo.ParameterSets.Select(s => GetParameterSetElement(context, s, parameterContextItems)).ToArray());
        }

        private XElement GetParameterSetElement(CommandFactoryContext context, CommandParameterSetInfo parameterSet, ParameterFactoryContext[] parameterContextItems)
        {
            return new XElement(CommandHelpFactory.XmlNs_command.GetName(CommandHelpFactory.syntaxItem), GetParameterSetNodes(context, parameterSet, parameterContextItems).ToArray());
        }

        private IEnumerable<XElement> GetParameterSetNodes(CommandFactoryContext context, CommandParameterSetInfo parameterSet, ParameterFactoryContext[] parameterContextItems)
        {
            yield return new XElement(CommandHelpFactory.XmlNs_command.GetName(CommandHelpFactory.name), context.CommandInfo.Name);
            parameterSet.Parameters.Select(p => new SyntaxParameterFactoryContext(context, p));
#warning Not implemented
            throw new NotImplementedException();
        }

        public IEnumerable<IGrouping<string, CommandParameterInfo>> GetInputTypes(CommandFactoryContext context)
        {
            return context.CommandInfo.ParameterSets.SelectMany(s => s.Parameters.Where(p => p.ValueFromPipeline || p.ValueFromPipelineByPropertyName)).GroupBy(p => p.Name);
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
