using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LteDev.XsdTypeGen.maml
{
    [Serializable()]
    [XmlRoot("description", Namespace = Constants.Xmlns_maml)]
    public class description : textBlockType
    {
        // TODO: Reference from command/command.cs: maml:description
    }
}