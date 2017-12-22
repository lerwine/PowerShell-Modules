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
    public class FunctionFactoryContext : CommandFactoryContext<FunctionInfo>
    {
        public FunctionFactoryContext(PSHelpFactoryContext context, FunctionInfo functionInfo) : base(context, functionInfo) { }

        public override string Noun { get { return CommandInfo.Noun; } }

        public override string Verb { get { return CommandInfo.Verb; } }

        public override AssemblyContext AssemblyContext { get { return null; } }

        public override XElement AssemblyClassHelp { get { return null; } }

        public override XElement GetAssemblyPropertyHelp(string propertyName) { return null; }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
