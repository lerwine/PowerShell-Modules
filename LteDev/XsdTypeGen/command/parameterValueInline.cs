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
    [XmlRoot("parameterValueInline", Namespace = Constants.Xmlns_command)]
    public class parameterValueInline
    {
        [XmlText]
        public string Value { get; set; }
    }
}