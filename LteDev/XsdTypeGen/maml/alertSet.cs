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
    [XmlRoot("alertSet", Namespace = Constants.Xmlns_maml)]
    public class alertSet : inline
    {
        [XmlAttribute("class")]
        public alertTypes alertClass { get; set; }
    }
    
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}