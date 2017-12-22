using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LteDev.XsdTypeGen.command
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member

    [XmlRoot("command", Namespace = Constants.Xmlns_command)]
    public class command
    {

        [XmlAttribute("contentType")]
        public string contentType { get; set; }

        [XmlElement("details", Namespace = Constants.Xmlns_command)]
        public details details { get; set; }

        [XmlElement("description", Namespace = Constants.Xmlns_maml)]
        public maml.description description { get; set; }

        [XmlElement("syntax", Namespace = Constants.Xmlns_command)]
        public syntax syntax { get; set; }

        [XmlElement("parameters", Namespace = Constants.Xmlns_command)]
        public parameters parameters { get; set; }

        [XmlElement("inputTypes", Namespace = Constants.Xmlns_command)]
        public inputTypes inputTypes { get; set; }

        [XmlElement("returnValues", Namespace = Constants.Xmlns_command)]
        public returnValues returnValues { get; set; }

        [XmlElement("terminatingErrors", Namespace = Constants.Xmlns_command)]
        public terminatingErrors terminatingErrors { get; set; }

        [XmlElement("nonTerminatingErrors", Namespace = Constants.Xmlns_command)]
        public nonTerminatingErrors nonTerminatingErrors { get; set; }

        [XmlElement("alertSet", Namespace = Constants.Xmlns_maml)]
        public maml.alertSet alertSet { get; set; }

        [XmlElement("examples", Namespace = Constants.Xmlns_command)]
        public examples examples { get; set; }

        [XmlElement("relatedLinks", Namespace = Constants.Xmlns_maml)]
        public maml.relatedLinks relatedLinks { get; set; }
    }
    
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
