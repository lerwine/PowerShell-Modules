using System;
using System.Xml.Serialization;

namespace NetworkUtilityCLR
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlRoot(MediaTypeRegistryDB.ElementName_note, Namespace = MediaTypeRegistry.XmlNamespace_Registry, IsNullable = true)]
    public class MediaTypeRegNote : MediaTypeRegRawXml
    {
        /// <summary>
        /// 
        /// </summary>
        public override string ElementName { get { return MediaTypeRegistryDB.ElementName_note; } }
    }
}
