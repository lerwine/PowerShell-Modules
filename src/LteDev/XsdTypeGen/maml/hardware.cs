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
    [XmlRoot("hardware", Namespace = Constants.Xmlns_maml)]
    public class hardware : textType
    {
    }
}