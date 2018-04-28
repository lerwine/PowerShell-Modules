using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LteDev.XsdTypeGen.maml
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    [Serializable()]
    public class textType
    {
        [XmlElement("notLocalizable", Namespace = Constants.Xmlns_maml, Type = typeof(notLocalizable))]
        [XmlText(Type = typeof(string))]
        public List<object> Contents { get; set; }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
