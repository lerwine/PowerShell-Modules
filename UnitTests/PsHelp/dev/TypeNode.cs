using System;
using System.Xml;
using System.Xml.Serialization;

namespace UnitTests.PsHelp.dev
{
    [XmlRoot(RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_dev, IsNullable = true)]
    public class TypeNode
    {
        public const string RootElementName = "type";

        [XmlElement("name", Namespace = ModuleConformance.ExtensionMethods.Xmlns_maml, IsNullable = true)]
        public string Name { get; set; }

        [XmlElement("uri", Namespace = ModuleConformance.ExtensionMethods.Xmlns_maml, IsNullable = true)]
        public string Uri { get; set; }
    }
}
