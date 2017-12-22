using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev.HelpXml
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class ParameterFactory : PSHelpFactoryBase
    {
        public ParameterFactory(CommandHelpFactory commandHelpFactory)
            : base(commandHelpFactory.PSHelp)
        {
        }

        internal ParameterFactoryContext[] CreateParameterContextItems(CommandFactoryContext context)
        {
            return context.CommandInfo.Parameters.Keys.Select(n => new ParameterFactoryContext(context, context.CommandInfo.Parameters[n])).ToArray();
        }

        internal XElement GetParametersElement(ParameterFactoryContext[] parameters)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
