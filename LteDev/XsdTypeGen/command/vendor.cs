using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LteDev.XsdTypeGen.command
{
    [Serializable()]
    [XmlRoot("vendor", Namespace = Constants.Xmlns_command)]
    public class vendor
    {
        // TODO: Reference from command/details.cs: command:vendor
    }
}