using System;
using System.Management.Automation;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CredentialStorageLibrary
{
    public class MetaDataValue : IXmlSerializable
    {
        private object _syncRoot = new object();

        public object SyncRoot { get { return _syncRoot; } }

        public PSObject Value { get; set; }

        internal static MetaDataValue Create(object obj)
        {
            throw new NotImplementedException();
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}