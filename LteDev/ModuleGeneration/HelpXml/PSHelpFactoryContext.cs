using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev.HelpXml
{
    public class PSHelpFactoryContext
    {
        private List<AssemblyContext> _assemblies = new List<AssemblyContext>();

        public PSModuleInfo ModuleInfo { get; private set; }

        public PSHelpFactoryContext(PSModuleInfo moduleInfo)
        {
            ModuleInfo = moduleInfo;
        }

        public XElement GetHelpItems(IEnumerable<XElement> commandElements)
        {
            return new XElement(PSHelpFactory.XmlNs_msh.GetName(PSHelpFactory.helpItems), (new object[] {new XAttribute(XNamespace.Xmlns.GetName("command"), CommandHelpFactory.XmlNsUri_command),
                    new XAttribute(XNamespace.Xmlns.GetName("maml"), MamlFactory.XmlNsUri_maml),
                    new XAttribute(XNamespace.Xmlns.GetName("managed"), PSHelpFactory.XmlNsUri_managed),
                    new XAttribute(XNamespace.Xmlns.GetName("MSHelp"), PSHelpFactory.XmlNs_MSHelp)}).Concat(commandElements).ToArray());

        }

        public AssemblyContext GetAssemblyContext(Assembly assembly)
        {
            AssemblyContext context;

            lock (_assemblies)
            {
                if ((context = _assemblies.FirstOrDefault(a => a.Assembly.FullName == assembly.FullName)) == null)
                {
                    context = new AssemblyContext(assembly);
                    _assemblies.Add(context);
                }
            }

            return context;
        }

        protected PSHelpFactoryContext(PSHelpFactoryContext context)
        {
            ModuleInfo = context.ModuleInfo;
        }
    }
}
