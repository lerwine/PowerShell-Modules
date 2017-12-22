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
    public class FunctionInfoAggregate : CmdInfoAggregate<FunctionInfo>
    {
        public override string Verb { get { return Command.Verb; } }

        public override string Noun { get { return Command.Noun; } }

        protected FunctionInfoAggregate(FunctionInfo command, ModuleInfoAggregate module) : base(command, module) { }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}