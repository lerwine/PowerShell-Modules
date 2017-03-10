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
    [XmlRoot(MediaTypeRegistry.ElementName_xref, Namespace = MediaTypeRegistry.XmlNamespace_Registry, IsNullable = true)]
    public class MediaTypeRegXRef
    {
		public const string AttributeName_type = "type";
		public const string AttributeName_data = "data";
		
        private string _type = null;
        private string _data = null;
        private string _content = null;
        [XmlAttribute(AttributeName_type)]
        public string Type { get { return _type; } set { _type = value; } }
		
        [XmlAttribute(AttributeName_data)]
        public string Data { get { return _data; } set { _data = value; } }
		
        [XmlText()]
        public string Content { get { return _content; } set { _content = value; } }
    }
}
