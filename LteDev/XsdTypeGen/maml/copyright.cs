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
    [XmlRoot("copyright", Namespace = Constants.Xmlns_maml)]
    public class copyright
    {
        // TODO: Reference from command/details.cs: maml:copyright
    }
}