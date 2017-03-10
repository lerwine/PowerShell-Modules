using System;
using System.Xml;
using System.Xml.Serialization;

namespace NetworkUtilityCLR
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlRoot(MediaTypeRegRecord.ElementName_file, Namespace = MediaTypeRegistry.XmlNamespace_Registry, IsNullable = true)]
    public class MediaTypeRegFile
    {
        /// <summary>
        /// 
        /// </summary>
		public const string AttributeName_type = "type";
		
        private string _type = null;
        private string _value = "";
		
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName_type)]
        public string Type { get { return _type; } set { _type = value; } }
		
        /// <summary>
        /// 
        /// </summary>
        [XmlText()]
        public string Data { get { return _value; } set { _value = (value == null) ? "" : value; } }
    }
}
