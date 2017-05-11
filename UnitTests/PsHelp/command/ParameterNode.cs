using System;
using System.Xml;
using System.Xml.Serialization;

namespace UnitTests.PsHelp.command
{
    [XmlRoot(RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_command, IsNullable = true)]
    public class ParameterNode
    {
        public const string RootElementName = "parameter";

        [XmlElement("name", Namespace = ModuleConformance.ExtensionMethods.Xmlns_maml, IsNullable = true)]
        public string Name { get; set; }
        
        [XmlAttribute("position")]
        public string Position { get; set; }

        [XmlElement(dev.TypeNode.RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_dev, IsNullable = true)]
        public dev.TypeNode Type { get; set; }

        [XmlAttribute("globbing")]
        public bool Globbing { get; set; }

        [XmlElement(maml.DescriptionNode.RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_maml, IsNullable = true)]
        public maml.DescriptionNode Description { get; set; }

        [XmlElement(ParameterValueNode.RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_command, IsNullable = true)]
        public ParameterValueNode ParameterValue { get; set; }

        [XmlAttribute("pipelineInput")]
        public string PipelineInput { get; set; }

        [XmlAttribute("variableLength")]
        public bool VariableLength { get; set; }

        [XmlAttribute("required")]
        public bool Required { get; set; }
    }
}
