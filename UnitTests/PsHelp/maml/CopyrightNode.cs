using System;
using System.Xml;
using System.Xml.Serialization;

namespace UnitTests.PsHelp.maml
{
    [XmlRoot(RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_maml, IsNullable = true)]
    public class CopyrightNode : ParagraphAndTextNode
    {
        public const string RootElementName = "copyright";
    }
}