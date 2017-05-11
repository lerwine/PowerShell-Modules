using System;
using System.Xml;
using System.Xml.Serialization;

namespace UnitTests.PsHelp.command
{
    [XmlRoot(RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_command, IsNullable = true)]
    public class ParameterValueNode
    {
        public const string RootElementName = "parameterValue";

        [XmlAttribute("variableLength")]
        public bool VariableLength { get; set; }

        [XmlText()]
        public string TypeName { get; set; }

        [XmlAttribute("required")]
        public bool Required { get; set; }
    }
}
