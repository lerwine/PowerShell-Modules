using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;

namespace NetworkUtilityCLR
{
    [Serializable]
    [XmlRoot(ElementName_registry, Namespace = XmlNamespace_Registry)]
    public class MediaTypeRegistry : MediaTypeRegistryBase
    {
		public const string ElementName_hide = "hide";
		public const string ElementName_record = "record";
        
        private MediaTypeRegHide _hide = null;
        private Collection<MediaTypeRegRecord> _records = new Collection<MediaTypeRegRecord>();

        [XmlElement(ElementName_hide, Namespace = XmlNamespace_Registry, IsNullable = true, Type = typeof(MediaTypeRegHide))]
        public MediaTypeRegHide Hide { get { return _hide; } set { _hide = value; } }
		
        [XmlElement(ElementName_record, Namespace = XmlNamespace_Registry, IsNullable = true, Type = typeof(MediaTypeRegRecord))]
        public Collection<MediaTypeRegRecord> Records { get { return _records; } set { _records = (value == null) ? new Collection<MediaTypeRegRecord>() : value; } }
    }
}
