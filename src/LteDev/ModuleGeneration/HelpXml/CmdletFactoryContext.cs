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
    public class CmdletFactoryContext : CommandFactoryContext<CmdletInfo>
    {
        public CmdletFactoryContext(PSHelpFactoryContext context, CmdletInfo cmdletInfo) : base(context, cmdletInfo) { }

        public override string Noun { get { return CommandInfo.Noun; } }

        public override string Verb { get { return CommandInfo.Verb; } }

        private AssemblyContext _assemblyContext = null;

        public override XElement AssemblyClassHelp
        {
            get { return AssemblyContext.GetClassElement(CommandInfo.ImplementingType); }
        }

        public override XElement GetAssemblyPropertyHelp(string propertyName)
        {
            return AssemblyContext.GetPropertyElement(CommandInfo.ImplementingType, propertyName);
        }

        public override AssemblyContext AssemblyContext
        {
            get { return GetAssemblyContext(CommandInfo.ImplementingType.Assembly); }
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
