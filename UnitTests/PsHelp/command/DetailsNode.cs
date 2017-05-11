using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace UnitTests.PsHelp.command
{
    [XmlRoot(RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_command, IsNullable = true)]
    public class DetailsNode
    {
        public const string RootElementName = "details";

        [XmlElement("name", Namespace = ModuleConformance.ExtensionMethods.Xmlns_command, IsNullable = true)]
        public string CommandName { get; set; }

        [XmlElement("verb", Namespace = ModuleConformance.ExtensionMethods.Xmlns_command, IsNullable = true)]
        public string Verb { get; set; }

        [XmlElement("noun", Namespace = ModuleConformance.ExtensionMethods.Xmlns_command, IsNullable = true)]
        public string Noun { get; set; }

        [XmlElement("version", Namespace = ModuleConformance.ExtensionMethods.Xmlns_dev, IsNullable = true)]
        public string Version { get; set; }

        [XmlElement(maml.DescriptionNode.RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_maml, IsNullable = true)]
        public maml.DescriptionNode Description { get; set; }

        [XmlElement(maml.CopyrightNode.RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_maml, IsNullable = true)]
        public maml.CopyrightNode Copyright { get; set; }
    }
}