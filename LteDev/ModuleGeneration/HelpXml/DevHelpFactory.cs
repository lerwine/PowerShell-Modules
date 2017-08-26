using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev.HelpXml
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class DevHelpFactory : PSHelpFactoryBase
    {
        public const string XmlNsUri_dev = "http://schemas.microsoft.com/maml/dev/2004/10";
        public const string version = "version";

        public static readonly XNamespace XmlNs_dev = XNamespace.Get(XmlNsUri_dev);

        internal DevHelpFactory(PSHelpFactory psHelpFactory) : base(psHelpFactory) { }

        internal XElement CreateTypeElement(Type parameterType)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
