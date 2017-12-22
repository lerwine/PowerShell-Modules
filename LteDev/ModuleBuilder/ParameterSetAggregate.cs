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
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class ParameterSetAggregate : InformationAggregator
    {
        private object _syncRoot = new object();
        private Collection<ParameterSetParamInfoAggregate> _innerParameters = null;
        private ReadOnlyCollection<ParameterSetParamInfoAggregate> _parameters = null;

        public CommandParameterSetInfo ParameterSet { get; private set; }

        public CmdInfoAggregate Command { get; private set; }

        public ReadOnlyCollection<ParameterSetParamInfoAggregate> Parameters
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_parameters == null)
                    {
                        _innerParameters = new Collection<ParameterSetParamInfoAggregate>();
                        foreach (CommandParameterInfo parameter in ParameterSet.Parameters)
                            _innerParameters.Add(Command.CreateParameterSetParamInfoAggregate(parameter, this));
                        _parameters = new ReadOnlyCollection<ParameterSetParamInfoAggregate>(_innerParameters);
                    }
                }
                finally { Monitor.Exit(_syncRoot); }
                return _parameters;
            }
        }

        public ParameterSetAggregate(CommandParameterSetInfo parameterSet, CmdInfoAggregate commandContext)
        {
            if (parameterSet == null)
                throw new ArgumentNullException("parameterSet");

            if (commandContext == null)
                throw new ArgumentNullException("commandContext");

            if (!commandContext.Command.ParameterSets.Any(c => ReferenceEquals(c, parameterSet)))
                throw new ArgumentException("Parameter set belongs to another command.");

            ParameterSet = parameterSet;
            Command = commandContext;
        }

        public XElement GetSyntaxItemHelp()
        {
            // TODO: Command.MamlCommandHelpInfo#syntax syntax=@{syntaxItem=System.Management.Automation.PSObject[]}
            throw new NotImplementedException();
#warning Not implemented
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}