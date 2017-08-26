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
    [XmlRoot("quote", Namespace = Constants.Xmlns_maml)]
    public class quote
    {
        // TODO: Reference from maml/textBlockType.cs: maml:quote
    }
}