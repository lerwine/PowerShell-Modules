using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LteDev.ModuleBuilder
{
    public class BinaryModuleInfoAggregate : ModuleInfoAggregate, IHasAssemblyInfo
    {
        public AssemblyInfo AssemblyInfo
        {
            get { throw new NotImplementedException(); }
        }

        public BinaryModuleInfoAggregate(PSModuleInfo moduleInfo, AggregateInfoFactory factory) : base(moduleInfo, factory)
        {
            if (moduleInfo.ModuleType != ModuleType.Binary)
                throw new ArgumentException("Module is not a binary module.", "moduleInfo");
        }
    }
}
