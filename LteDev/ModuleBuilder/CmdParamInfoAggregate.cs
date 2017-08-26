using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LteDev.ModuleBuilder
{
    public class CmdParamInfoAggregate : ParamInfoAggregate<ParameterMetadata>
    {
        public CmdInfoAggregate Command { get; private set; }

        public CmdParamInfoAggregate(ParameterMetadata parameter, CmdInfoAggregate commandContext)
            : base(parameter)
        {
            if (commandContext == null)
                throw new ArgumentNullException("commandContext");

            if (!commandContext.Command.Parameters.ContainsKey(parameter.Name) || !ReferenceEquals(commandContext.Command.Parameters[parameter.Name], parameter))
                throw new ArgumentException("Parameter belongs to another command.");
            
            Command = commandContext;
        }

        public XElement GetParameterHelp()
        {
            // TODO: Command.MamlCommandHelpInfo#parameters parameters=@{parameter=System.Management.Automation.PSObject[]}
            throw new NotImplementedException();
#warning Not implemented
        }
    }
}