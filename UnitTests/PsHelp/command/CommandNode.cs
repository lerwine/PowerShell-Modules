using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace UnitTests.PsHelp.command
{
    [XmlRoot(RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_command, IsNullable = true)]
    public class CommandNode
    {
        public const string RootElementName = "command";

        [XmlArray("parameters", Namespace = ModuleConformance.ExtensionMethods.Xmlns_command, IsNullable = true)]
        public Collection<ParameterNode> Parameters { get; set; }

        [XmlArray("returnValues", Namespace = ModuleConformance.ExtensionMethods.Xmlns_command, IsNullable = true)]
        public Collection<ReturnValueNode> ReturnValues { get; set; }

        [XmlElement(maml.DescriptionNode.RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_maml, IsNullable = true)]
        public maml.DescriptionNode Description { get; set; }

        [XmlArray("syntax", Namespace = ModuleConformance.ExtensionMethods.Xmlns_command, IsNullable = true)]
        public Collection<SyntaxItemNode> Syntax { get; set; }

        [XmlElement(DetailsNode.RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_command, IsNullable = true)]
        public DetailsNode Details { get; set; }

        [XmlArray("inputTypes", Namespace = ModuleConformance.ExtensionMethods.Xmlns_command, IsNullable = true)]
        public Collection<InputTypeNode> InputTypes { get; set; }
    }
}
