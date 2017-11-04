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
    [XmlRoot("parameters", Namespace = Constants.Xmlns_command)]
    public class parameters
    {
        // TODO: Reference from command/command.cs: command:parameters
    }
}