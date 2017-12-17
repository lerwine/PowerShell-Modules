using System;
using System.Xml.Serialization;

namespace NetworkUtility
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlRoot(MediaTypeRegistryDB.ElementName_people, Namespace = MediaTypeRegistry.XmlNamespace_Registry, IsNullable = true)]
    public class MediaTypeRegPeople : MediaTypeRegRawXml
    {
        /// <summary>
        /// 
        /// </summary>
        public override string ElementName { get { return MediaTypeRegistryDB.ElementName_people; } }
    }
}
