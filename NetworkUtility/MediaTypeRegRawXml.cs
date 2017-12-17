using System;
using System.Xml;
using System.Xml.Serialization;

namespace NetworkUtility
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MediaTypeRegRawXml : IXmlSerializable
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract string ElementName { get; }
		
        private XmlDocument _xmlDocument = null;
		
        /// <summary>
        /// 
        /// </summary>
        public XmlElement Content
        {
            get
            {
                if (_xmlDocument == null)
                    _xmlDocument = new XmlDocument();
                if (_xmlDocument.DocumentElement == null)
                    _xmlDocument.AppendChild(_xmlDocument.CreateElement(ElementName, MediaTypeRegistry.XmlNamespace_Registry));
                return _xmlDocument.DocumentElement;
            }
        }
        
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() { return null; }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            _xmlDocument = new XmlDocument();
            _xmlDocument.LoadXml(reader.ReadOuterXml());
        }
		
        void IXmlSerializable.WriteXml(XmlWriter writer) { Content.WriteContentTo(writer); }
    }
}
