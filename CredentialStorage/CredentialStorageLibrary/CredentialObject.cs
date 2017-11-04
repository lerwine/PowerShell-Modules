using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CredentialStorageLibrary
{
    [Serializable]
    public abstract class CredentialObject
    {
        private Guid? _id = null;
        private string _title = "";
        private string _description = null;
        private string _notes = null;
        private MetaDataDictionary _metaData = null;

        [XmlIgnore]
        public Guid ID
        {
            get
            {
                if (!_id.HasValue)
                    _id = Guid.NewGuid();
                return _id.Value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlAttribute("ID")]
        public string __ID
        {
            get { return ID.ToString("d"); }
            set
            {
                if (_id.HasValue)
                    throw new InvalidOperationException("ID cannot be modified after it has been set.");
                string s;
                Guid g;
                if (value != null && (s = value.Trim()).Length > 0 && Guid.TryParse(s, out g))
                    _id = g;
            }
        }

        [XmlAttribute()]
        public string Title
        {
            get { return _title; }
            set { _title = (value == null) ? "" : value.Trim(); }
        }

        [XmlElement(IsNullable = true)]
        public string Description
        {
            get { return _description; }
            set { _description = (value == null || value.Length > 0) ? value : null; }
        }

        [XmlElement(IsNullable = true)]
        public string Notes
        {
            get { return _notes; }
            set { _notes = (value == null || value.Length > 0) ? value : null; }
        }

        [XmlElement(IsNullable = true)]
        public MetaDataDictionary MetaData
        {
            get
            {
                if (_metaData == null)
                    _metaData = new MetaDataDictionary();
                return _metaData;
            }
            set { _metaData = value; }
        }
    }
}