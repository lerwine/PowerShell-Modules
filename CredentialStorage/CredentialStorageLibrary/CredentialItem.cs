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
    public abstract class CredentialItem : CredentialObject
    {
        private Collection<Guid> _groups = null;
        private Collection<Guid> _links = null;

        [XmlElement()]
        public Collection<Guid> Groups
        {
            get
            {
                if (_groups == null)
                    _groups = new Collection<Guid>();
                return _groups;
            }
            set { _groups = value; }
        }

        [XmlElement()]
        public Collection<Guid> Links
        {
            get
            {
                if (_links == null)
                    _links = new Collection<Guid>();
                return _links;
            }
            set { _links = value; }
        }
    }
}