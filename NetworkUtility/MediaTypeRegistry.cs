using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace NetworkUtility
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName_registry, Namespace = XmlNamespace_Registry)]
    public class MediaTypeRegistry : MediaTypeRegistryBase
    {
        /// <summary>
        /// 
        /// </summary>
        public const string ElementName_hide = "hide";

        /// <summary>
        /// 
        /// </summary>
        public const string ElementName_record = "record";
        
        private MediaTypeRegHide _hide = null;
        private Collection<MediaTypeRegRecord> _records = new Collection<MediaTypeRegRecord>();

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(ElementName_hide, Namespace = XmlNamespace_Registry, IsNullable = true, Type = typeof(MediaTypeRegHide))]
        public MediaTypeRegHide Hide { get { return _hide; } set { _hide = value; } }
        
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(ElementName_record, Namespace = XmlNamespace_Registry, IsNullable = true, Type = typeof(MediaTypeRegRecord))]
        public Collection<MediaTypeRegRecord> Records { get { return _records; } set { _records = (value == null) ? new Collection<MediaTypeRegRecord>() : value; } }
    }
}
