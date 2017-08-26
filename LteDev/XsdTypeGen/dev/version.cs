using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LteDev.XsdTypeGen.dev
{
    [Serializable()]
    [XmlRoot("version", Namespace = Constants.Xmlns_dev)]
    public class version
    {
        // TODO: Reference from command/details.cs: dev:version
    }
}