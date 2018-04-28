using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LteDev.XsdTypeGen.maml
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    [Serializable()]
    [XmlRoot("definitionList", Namespace = Constants.Xmlns_maml)]
    public class definitionList
    {
        // TODO: Reference from maml/textBlockType.cs: maml:definitionList
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}