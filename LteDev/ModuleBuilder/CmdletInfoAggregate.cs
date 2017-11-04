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
    public class CmdletInfoAggregate : CmdInfoAggregate<CmdletInfo>
    {
        public CLRTypeInfo ImplementingType { get; private set; }

        public override string Verb { get { return Command.Verb; } }

        public override string Noun { get { return Command.Noun; } }

        protected CmdletInfoAggregate(CmdletInfo command, ModuleInfoAggregate module) : base(command, module)
        {
            ImplementingType = module.AggregateFactory.GetAssemblyTypeContext(command.ImplementingType);
        }

        protected override CmdParamInfoAggregate CreateCmdParamInfoAggregate(ParameterMetadata parameterMetadata)
        {
            return base.CreateCmdParamInfoAggregate(parameterMetadata);
        }

        protected override XElement CreateCommandHelpDetails()
        {
            return base.CreateCommandHelpDetails();
        }

        protected override ParameterSetAggregate CreateParameterSetAggregate(CommandParameterSetInfo parameterSet)
        {
            return base.CreateParameterSetAggregate(parameterSet);
        }

        protected internal override ParameterSetParamInfoAggregate CreateParameterSetParamInfoAggregate(CommandParameterInfo parameter, ParameterSetAggregate parameterSet)
        {
            return base.CreateParameterSetParamInfoAggregate(parameter, parameterSet);
        }

        protected override IEnumerable<XElement> GetCommandHelpSynopsis()
        {
            // TODO: Get Synopsis from <summary> of ImplementingType
            return base.GetCommandHelpSynopsis();
        }

        protected override IEnumerable<XElement> GetCopyright()
        {
            // TODO: Get copyright from Module.ModuleInfo.Copyright or ImplementingType.AssemblyInfo.Copyright
            return base.GetCopyright();
        }

        protected override IEnumerable<XElement> GetVersion()
        {
            // TODO: Get version from Module.ModuleInfo.Version or ImplementingType.AssemblyInfo.Version;
            return base.GetVersion();
        }

        protected override IEnumerable<XElement> GetCommandHelpDescription()
        {
            // TODO: Get help description from <help> of ImplementingType.
            return base.GetCommandHelpDescription();
        }

        protected override IEnumerable<XElement> GetReturnValues()
        {
            // TODO: Get return values according to ImplementingType;
            return base.GetReturnValues();
        }

        protected override IEnumerable<XElement> GetTerminatingErrors()
        {
            // TODO: Get terminating errors according to ImplementingType;
            return base.GetTerminatingErrors();
        }

        protected override IEnumerable<XElement> GetNonTerminatingErrors()
        {
            // TODO: Get nonterminating errors according to ImplementingType;
            return base.GetNonTerminatingErrors();
        }

        protected override IEnumerable<XElement> GetNotes()
        {
            // TODO: Get notes from <remarks> of ImplementingType;
            return base.GetNotes();
        }

        protected override IEnumerable<XElement> GetExamples()
        {
            // TODO: Get examples from <remarks> of ImplementingType;
            return base.GetExamples();
        }

        protected override IEnumerable<XElement> GetRelatedNavigationLinks()
        {
            // TODO: Get return related links according to ImplementingType;
            return base.GetRelatedNavigationLinks();
        }
    }
}