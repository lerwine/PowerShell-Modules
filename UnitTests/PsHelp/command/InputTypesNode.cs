using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace UnitTests.PsHelp.command
{
    [XmlRoot(RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_command, IsNullable = true)]
    public class InputTypesNode
    {
        public const string RootElementName = "inputTypes";

    }
}
