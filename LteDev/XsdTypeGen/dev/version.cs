using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LteDev.XsdTypeGen.dev
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member

    [Serializable()]
    [XmlRoot("version", Namespace = Constants.Xmlns_dev)]
    public class version
    {
        // TODO: Reference from command/details.cs: dev:version
    }
    
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}