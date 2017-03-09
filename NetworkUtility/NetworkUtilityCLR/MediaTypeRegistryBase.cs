using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;

namespace NetworkUtilityCLR
{
    public abstract class MediaTypeRegistryBase
    {
        public const string XmlNamespace_Registry = "http://www.iana.org/assignments";
		public const string AttributeName_id = "id";
		public const string ElementName_registry = "registry";
		public const string ElementName_title = "title";
		public const string ElementName_xref = "xref";
		public const string ElementName_note = "note";
		
        public const string TopLevelType_Text = "text";
        public const string TopLevelType_Image = "image";
        public const string TopLevelType_Audio = "audio";
        public const string TopLevelType_Video = "video";
        public const string TopLevelType_Application = "application";
        public const string TopLevelType_Multipart = "multipart";
        public const string TopLevelType_Message = "message";
		
        private string _id = "";
        private string _title = null;
        private Collection<MediaTypeRegXRef> _xRefs = new Collection<MediaTypeRegXRef>();
        private Collection<MediaTypeRegNote> _notes = new Collection<MediaTypeRegNote>();
        
        [XmlAttribute(AttributeName_id)]
        public virtual string ID
        {
            get { return _id; }
            set { _id = (value == null) ? "" : value.Trim(); }
        }

        [XmlElement(ElementName_title, Namespace = XmlNamespace_Registry, IsNullable = true)]
        public string Title { get { return _title; } set { _title = value; } }
		
        [XmlElement(ElementName_xref, Namespace = XmlNamespace_Registry, IsNullable = true, Type = typeof(MediaTypeRegXRef))]
        public Collection<MediaTypeRegXRef> XRefs { get { return _xRefs; } set { _xRefs = (value == null) ? new Collection<MediaTypeRegXRef>() : value; } }
		
        [XmlElement(ElementName_note, Namespace = XmlNamespace_Registry, IsNullable = true, Type = typeof(MediaTypeRegNote))]
        public Collection<MediaTypeRegNote> Notes { get { return _notes; } set { _notes = (value == null) ? new Collection<MediaTypeRegNote>() : value; } }
    }
}
