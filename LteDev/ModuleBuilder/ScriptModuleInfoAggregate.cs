using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LteDev.ModuleBuilder
{
    public class ScriptModuleInfoAggregate : ModuleInfoAggregate
    {
        public ScriptModuleInfoAggregate(PSModuleInfo moduleInfo, AggregateInfoFactory factory) : base(moduleInfo, factory)
        {
            if (moduleInfo.ModuleType != ModuleType.Script)
                throw new ArgumentException("Module is not a script module.", "moduleInfo");
        }
    }
}
