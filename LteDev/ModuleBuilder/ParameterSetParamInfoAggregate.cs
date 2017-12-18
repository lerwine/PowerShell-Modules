using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LteDev.ModuleBuilder
{
    public class ParameterSetParamInfoAggregate : ParamInfoAggregate<CommandParameterInfo>
    {
        private object _syncRoot = new object();
        private CLRTypeInfo _parameterType = null;

        public ParameterSetAggregate ParameterSet { get; private set; }

        public virtual XElement GetParameterTypeHelp()
        {
            // TODO: ParameterSet.Command.MamlCommandHelpInfo#inputTypes inputTypes=@{inputType=@{type=@{name=None}; description=System.Management.Automation.PSObject[]}}
            throw new NotImplementedException();
#warning Not implemented
        }

        public virtual IEnumerable<XElement> GetParameterHelpSummary()
        {
            return new XElement[0];
        }

        public CLRTypeInfo ParameterType
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_parameterType == null)
                        _parameterType = ParameterSet.Command.Module.AggregateFactory.GetAssemblyTypeContext(Parameter.ParameterType);
                }
                finally { Monitor.Exit(_syncRoot); }
                return _parameterType;
            }
        }

        public ParameterSetParamInfoAggregate(CommandParameterInfo parameter, ParameterSetAggregate parmeterSetContext)
            : base(parameter)
        {
            if (parmeterSetContext == null)
                throw new ArgumentNullException("parmeterSetContext");

            if (!parmeterSetContext.ParameterSet.Parameters.Any(p => ReferenceEquals(p, parameter)))
                throw new ArgumentException("Parameter belongs to another parameter set.");

            ParameterSet = parmeterSetContext;
        }
    }
}