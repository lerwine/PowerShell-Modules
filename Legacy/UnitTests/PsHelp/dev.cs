using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UnitTests.PsHelp
{
    public static class dev
    {
        public const string Xmlns_Prefix = "dev";
        public const string Xmlns_URI = "http://schemas.microsoft.com/maml/dev/2004/10";
        public const string ElementName_version = "version";

        public static XmlElement AddDevElement(this XmlNode node, string name, XmlNode insertBefore = null)
        {
            return node.AddElement(Xmlns_Prefix, name, Xmlns_URI, insertBefore);
        }

    }
}
