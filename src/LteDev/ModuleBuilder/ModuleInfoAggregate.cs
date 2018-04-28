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
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class ModuleInfoAggregate : InformationAggregator
    {
        public PSModuleInfo ModuleInfo { get; private set; }

        public AggregateInfoFactory AggregateFactory { get; private set; }

        public ReadOnlyCollection<CmdletInfoAggregate> ExportedCmdlets { get; private set; }

        public ReadOnlyCollection<FunctionInfoAggregate> ExportedFunctions { get; private set; }

        public XElement CreateModuleCommandHelp()
        {
            return new XElement(NS_msh.GetName("helpItems"), ExportedCmdlets.Select(c => c.CreateCommandHelp())
                .Concat(ExportedFunctions.Select(c => c.CreateCommandHelp())).ToArray());
        }

        public ModuleInfoAggregate(PSModuleInfo moduleInfo, AggregateInfoFactory factory)
        {
            if (moduleInfo == null)
                throw new ArgumentNullException("moduleInfo");

            if (factory == null)
                throw new ArgumentNullException("factory");

            ModuleInfo = moduleInfo;
            AggregateFactory = factory;
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
