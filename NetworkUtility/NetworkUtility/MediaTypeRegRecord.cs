using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace NetworkUtilityCLR
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlRoot(MediaTypeRegistry.ElementName_record, Namespace = MediaTypeRegistry.XmlNamespace_Registry)]
    public class MediaTypeRegRecord
    {
        /// <summary>
        /// 
        /// </summary>
		public const string AttributeName_date = "date";

        /// <summary>
        /// 
        /// </summary>
		public const string ElementName_name = "name";

        /// <summary>
        /// 
        /// </summary>
		public const string ElementName_file = "file";
		
        private string _date = "";
        private MediaTypeRegName _name = null;
        private Collection<MediaTypeRegXRef> _xRefs = new Collection<MediaTypeRegXRef>();
        private MediaTypeRegFile _file = null;
        

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName_date)]
        public string Date
        {
            get { return _date; }
            set { _date = (value == null) ? "" : value.Trim(); }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(ElementName_name, Namespace = MediaTypeRegistry.XmlNamespace_Registry, IsNullable = true, Type = typeof(MediaTypeRegName))]
        public MediaTypeRegName Name { get { return _name; } set { _name = value; } }
		
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(MediaTypeRegistry.ElementName_xref, Namespace = MediaTypeRegistry.XmlNamespace_Registry, IsNullable = true, Type = typeof(MediaTypeRegXRef))]
        public Collection<MediaTypeRegXRef> XRefs { get { return _xRefs; } set { _xRefs = (value == null) ? new Collection<MediaTypeRegXRef>() : value; } }
		
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(ElementName_file, Namespace = MediaTypeRegistry.XmlNamespace_Registry, IsNullable = true, Type = typeof(MediaTypeRegFile))]
        public MediaTypeRegFile File { get { return _file; } set { _file = value; } }
    }
}
