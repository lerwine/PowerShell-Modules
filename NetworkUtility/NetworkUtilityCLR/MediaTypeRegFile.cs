using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;

namespace NetworkUtilityCLR
{
    [Serializable]
    [XmlRoot(MediaTypeRegRecord.ElementName_file, Namespace = MediaTypeRegistry.XmlNamespace_Registry, IsNullable = true)]
    public class MediaTypeRegFile
    {
		public const string AttributeName_type = "type";
		
        private string _type = null;
        private string _value = "";
		
        [XmlAttribute(AttributeName_type)]
        public string Type { get { return _type; } set { _type = value; } }
		
        [XmlText()]
        public string Data { get { return _value; } set { _value = (value == null) ? "" : value; } }
    }
}
