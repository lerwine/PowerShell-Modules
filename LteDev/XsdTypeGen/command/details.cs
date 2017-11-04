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
    [XmlRoot("details", Namespace = Constants.Xmlns_command)]
    public class details
    {

        [XmlElement("name", Namespace = Constants.Xmlns_command)]
        public maml.textType name { get; set; }

        [XmlElement("description", Namespace = Constants.Xmlns_maml)]
        public maml.description description { get; set; }

        [XmlElement("synonyms", Namespace = Constants.Xmlns_command)]
        public synonyms synonyms { get; set; }

        [XmlElement("copyright", Namespace = Constants.Xmlns_maml)]
        public maml.copyright copyright { get; set; }

        [XmlElement("verb", Namespace = Constants.Xmlns_command)]
        public maml.textType verb { get; set; }

        [XmlElement("noun", Namespace = Constants.Xmlns_command)]
        public maml.textType noun { get; set; }

        [XmlElement("version", Namespace = Constants.Xmlns_dev)]
        public dev.version version { get; set; }

        [XmlElement("vendor", Namespace = Constants.Xmlns_command)]
        public vendor vendor { get; set; }
    }
}