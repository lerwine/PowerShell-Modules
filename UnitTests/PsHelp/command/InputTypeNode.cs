using System;
using System.Xml;
using System.Xml.Serialization;

namespace UnitTests.PsHelp.command
{
    [XmlRoot(RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_command, IsNullable = true)]
    public class InputTypeNode
    {
        public const string RootElementName = "inputType";

        [XmlElement(dev.TypeNode.RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_dev, IsNullable = true)]
        public dev.TypeNode Type { get; set; }

        [XmlElement(maml.DescriptionNode.RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_maml, IsNullable = true)]
        public maml.DescriptionNode Description { get; set; }
    }
}
