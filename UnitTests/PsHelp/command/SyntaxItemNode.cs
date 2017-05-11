using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace UnitTests.PsHelp.command
{
    [XmlRoot(RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_command, IsNullable = true)]
    public class SyntaxItemNode
    {
        public const string RootElementName = "syntaxItem";

        [XmlElement(ParameterNode.RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_command, IsNullable = true)]
        public Collection<ParameterNode> Parameters { get; set; }

        [XmlElement("name", Namespace = ModuleConformance.ExtensionMethods.Xmlns_maml, IsNullable = true)]
        public string CommandName { get; set; }
    }
}
