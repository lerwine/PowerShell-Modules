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
    public class MediaTypeRegistryDB : MediaTypeRegistryBase/*, IDictionary<string, MediaTypeRegistry>*/
    {
        /// <summary>
        /// 
        /// </summary>
        public const string Registry_DefaultId = "media-types";

        /// <summary>
        /// 
        /// </summary>
        public const string ElementName_category = "category";

        /// <summary>
        /// 
        /// </summary>
        public const string ElementName_updated = "updated";

        /// <summary>
        /// 
        /// </summary>
        public const string ElementName_registration_rule = "registration_rule";

        /// <summary>
        /// 
        /// </summary>
        public const string ElementName_expert = "expert";

        /// <summary>
        /// 
        /// </summary>
        public const string ElementName_people = "people";
        
        private string _category = null;
        private string _updated = null;
        private string _registrationRule = null;
        private string _expert = null;
        private Collection<MediaTypeRegistry> _registries = new Collection<MediaTypeRegistry>();
        private MediaTypeRegPeople _people = null;

        /// <summary>
        /// 
        /// </summary>
        public override string ID
        {
            get
            {
                if (base.ID.Length == 0)
                    base.ID = Registry_DefaultId;
                return base.ID;
            }
            set
            {
                string s = (value == null) ? "" : value.Trim();
                base.ID = (s.Length == 0) ? Registry_DefaultId : s;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(ElementName_category, Namespace = XmlNamespace_Registry, IsNullable = true)]
        public string Category { get { return _category; } set { _category = value; } }
        
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(ElementName_updated, Namespace = XmlNamespace_Registry, IsNullable = true)]
        public string Updated { get { return _updated; } set { _updated = value; } }
        
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(ElementName_registration_rule, Namespace = XmlNamespace_Registry, IsNullable = true)]
        public string RegistrationRule { get { return _registrationRule; } set { _registrationRule = value; } }
        
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(ElementName_expert, Namespace = XmlNamespace_Registry, IsNullable = true)]
        public string Expert { get { return _expert; } set { _expert = value; } }
        
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(ElementName_registry, Namespace = XmlNamespace_Registry, IsNullable = true, Type = typeof(MediaTypeRegistry))]
        public Collection<MediaTypeRegistry> Registries { get { return _registries; } set { _registries = (value == null) ? new Collection<MediaTypeRegistry>() : value; } }
        
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(ElementName_people, Namespace = XmlNamespace_Registry, IsNullable = true, Type = typeof(MediaTypeRegPeople))]
        public MediaTypeRegPeople People { get { return _people; } set { _people = value; } }
    }
}
