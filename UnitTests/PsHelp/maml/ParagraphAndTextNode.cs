using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace UnitTests.PsHelp.maml
{
    public abstract class ParagraphAndTextNode : Collection<ParagraphAndTextContent>, IXmlSerializable
    {
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