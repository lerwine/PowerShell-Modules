using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteDev
{
    public class PSMamlGenerator
    {
        public const string XmlNs_doc = "http://schemas.microsoft.com/maml/internal";

        public class maml
        {
            public const string NamespaceURI = "http://schemas.microsoft.com/maml/2004/10";
        }

        public class dev
        {
            public const string NamespaceURI = "http://schemas.microsoft.com/maml/dev/2004/10";
        }

        public class command
        {
            public const string NamespaceURI = "http://schemas.microsoft.com/maml/dev/command/2004/10";
        }

        public class CodeXmlDoc
        {

        }
    }
}
