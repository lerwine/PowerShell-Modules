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
    [XmlRoot(ElementName_registry, Namespace = XmlNamespace_Registry)]
    public class MediaTypeRegistryDB : MediaTypeRegistryBase, IDictionary<string, MediaTypeRegistry>
    {
        public const string Registry_DefaultId = "media-types";
		public const string ElementName_category = "category";
		public const string ElementName_updated = "updated";
		public const string ElementName_registration_rule = "registration_rule";
		public const string ElementName_expert = "expert";
		public const string ElementName_people = "people";
		
        private string _category = null;
        private string _updated = null;
        private string _registrationRule = null;
        private string _expert = null;
        private Collection<MediaTypeRegistry> _registries = new Collection<MediaTypeRegistry>();
        private MediaTypeRegPeople _people = null;

        public override string ID
        {
            get
			{
				if (_id.Length == 0)
					_id = Registry_DefaultId;
			}
            set
			{
				string s = (value == null) ? "" : value.Trim();
				_id = (s.Length == 0) ? Registry_DefaultId : s;
			}
        }
		
        [XmlElement(ElementName_category, Namespace = XmlNamespace_Registry, IsNullable = true)]
        public string Category { get { return _category; } set { _category = value; } }
		
        [XmlElement(ElementName_updated, Namespace = XmlNamespace_Registry, IsNullable = true)]
        public string Updated { get { return _updated; } set { _updated = value; } }
		
        [XmlElement(ElementName_registration_rule, Namespace = XmlNamespace_Registry, IsNullable = true)]
        public string RegistrationRule { get { return _registrationRule; } set { _registrationRule = value; } }
		
        [XmlElement(ElementName_expert, Namespace = XmlNamespace_Registry, IsNullable = true)]
        public string Expert { get { return _expert; } set { _expert = value; } }
		
        [XmlElement(ElementName_registry, Namespace = XmlNamespace_Registry, IsNullable = true, Type = typeof(MediaTypeRegistry))]
        public Collection<MediaTypeRegistry> Registries { get { return _registries; } set { _registries = (value == null) ? new Collection<MediaTypeRegistry>() : value; } }
		
        [XmlElement(ElementName_people, Namespace = XmlNamespace_Registry, IsNullable = true, Type = typeof(MediaTypeRegPeople))]
        public MediaTypeRegPeople People { get { return _people; } set { _people = value; } }
    }
}
