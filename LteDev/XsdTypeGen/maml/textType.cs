using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LteDev.XsdTypeGen.maml
{
    [Serializable()]
    public class textType
    {
        [XmlElement("notLocalizable", Namespace = Constants.Xmlns_maml, Type = typeof(notLocalizable))]
        [XmlText(Type = typeof(string))]
        public List<object> Contents { get; set; }
    }
}
