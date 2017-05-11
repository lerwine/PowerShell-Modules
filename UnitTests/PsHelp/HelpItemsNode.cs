using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace UnitTests.PsHelp
{
    [XmlRoot(RootElementName, Namespace = ModuleConformance.ExtensionMethods.Xmlns_msh, IsNullable = true)]
    public class HelpItemsNode : Collection<command.CommandNode>, IXmlSerializable
    {
        public const string RootElementName = "helpItems";
        
        public string Schema { get; set; }

        public HelpItemsNode() { Schema = "maml"; }

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
