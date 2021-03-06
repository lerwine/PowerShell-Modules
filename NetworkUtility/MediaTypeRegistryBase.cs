﻿using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace NetworkUtility
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MediaTypeRegistryBase
    {
        /// <summary>
        /// 
        /// </summary>
        public const string XmlNamespace_Registry = "http://www.iana.org/assignments";

        /// <summary>
        /// 
        /// </summary>
		public const string AttributeName_id = "id";

        /// <summary>
        /// 
        /// </summary>
		public const string ElementName_registry = "registry";

        /// <summary>
        /// 
        /// </summary>
		public const string ElementName_title = "title";

        /// <summary>
        /// 
        /// </summary>
		public const string ElementName_xref = "xref";

        /// <summary>
        /// 
        /// </summary>
		public const string ElementName_note = "note";
		
        /// <summary>
        /// 
        /// </summary>
        public const string TopLevelType_Text = "text";

        /// <summary>
        /// 
        /// </summary>
        public const string TopLevelType_Image = "image";

        /// <summary>
        /// 
        /// </summary>
        public const string TopLevelType_Audio = "audio";

        /// <summary>
        /// 
        /// </summary>
        public const string TopLevelType_Video = "video";

        /// <summary>
        /// 
        /// </summary>
        public const string TopLevelType_Application = "application";

        /// <summary>
        /// 
        /// </summary>
        public const string TopLevelType_Multipart = "multipart";

        /// <summary>
        /// 
        /// </summary>
        public const string TopLevelType_Message = "message";
		
        private string _id = "";
        private string _title = null;
        private Collection<MediaTypeRegXRef> _xRefs = new Collection<MediaTypeRegXRef>();
        private Collection<MediaTypeRegNote> _notes = new Collection<MediaTypeRegNote>();
        
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName_id)]
        public virtual string ID
        {
            get { return _id; }
            set { _id = (value == null) ? "" : value.Trim(); }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(ElementName_title, Namespace = XmlNamespace_Registry, IsNullable = true)]
        public string Title { get { return _title; } set { _title = value; } }
		
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(ElementName_xref, Namespace = XmlNamespace_Registry, IsNullable = true, Type = typeof(MediaTypeRegXRef))]
        public Collection<MediaTypeRegXRef> XRefs { get { return _xRefs; } set { _xRefs = (value == null) ? new Collection<MediaTypeRegXRef>() : value; } }
		
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(ElementName_note, Namespace = XmlNamespace_Registry, IsNullable = true, Type = typeof(MediaTypeRegNote))]
        public Collection<MediaTypeRegNote> Notes { get { return _notes; } set { _notes = (value == null) ? new Collection<MediaTypeRegNote>() : value; } }
    }
}
