using System;
using System.Xml.Serialization;

namespace NetworkUtilityCLR
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlRoot(MediaTypeRegistry.ElementName_hide, Namespace = MediaTypeRegistry.XmlNamespace_Registry, IsNullable = true)]
    public class MediaTypeRegHide : MediaTypeRegRawXml
    {
        /// <summary>
        /// 
        /// </summary>
        public override string ElementName { get { return MediaTypeRegistry.ElementName_hide; } }
    }
}
