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
    public class CrededentialStore
    {
        private Collection<CredentialDomain> _domains = null;

        [XmlElement()]
        public Collection<CredentialDomain> Domains
        {
            get
            {
                if (_domains == null)
                    _domains = new Collection<CredentialDomain>();
                return _domains;
            }
            set { _domains = value; }
        }
    }
}
