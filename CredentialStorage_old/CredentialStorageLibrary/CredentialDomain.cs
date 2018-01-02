using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CredentialStorageLibrary
{
    [Serializable]
    public class CredentialDomain : CredentialObject
    {
        private Collection<CredentialGroup> _groups = null;
        private Collection<CredentialItem> _credentials = null;
        private Collection<AccessLink> _links = null;

        [XmlElement()]
        public Collection<CredentialGroup> Groups
        {
            get
            {
                if (_groups == null)
                    _groups = new Collection<CredentialGroup>();
                return _groups;
            }
            set { _groups = value; }
        }

        [XmlElement()]
        public Collection<CredentialItem> Credentials
        {
            get
            {
                if (_credentials == null)
                    _credentials = new Collection<CredentialItem>();
                return _credentials;
            }
            set { _credentials = value; }
        }

        [XmlElement()]
        public Collection<AccessLink> Links
        {
            get
            {
                if (_links == null)
                    _links = new Collection<AccessLink>();
                return _links;
            }
            set { _links = value; }
        }
    }
}