using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace CredentialStorageCLR
{
    [XmlRoot(ElementName = StoredCredential.ElementName, Namespace = StoredCredential.NamespaceURI)]
    [Serializable]
    public class StoredCredential : CredentialComponent
    {
        public const string ElementName = "Credential";

        private Guid? _id = null;
        private string _displayText = "";
        private string _userName = "";
        private string _password = "";
        private string _location = "";

        [XmlAttribute]
        public Guid Id
        {
            get
            {
                if (!this._id.HasValue)
                    this._id = Guid.NewGuid();

                return this._id.Value;
            }
            set { this._id = value; }
        }

        [XmlAttribute]
        public string DisplayText
        {
            get { return this._displayText; }
            set { this._displayText = value ?? ""; }
        }

        [XmlElement("UserName", Namespace = StoredCredential.NamespaceURI)]
        public string UserName
        {
            get { return this._userName; }
            set { this._userName = value ?? ""; }
        }

        [XmlElement("Password", Namespace = StoredCredential.NamespaceURI)]
        public string Password
        {
            get { return this._password; }
            set { this._password = value ?? ""; }
        }

        [XmlElement("Location", Namespace = StoredCredential.NamespaceURI)]
        public string Location
        {
            get { return this._location; }
            set { this._location = value ?? ""; }
        }

        protected override CredentialComponent Clone()
        {
            StoredCredential clone = new StoredCredential();
            clone._id = this.Id;
            clone._displayText = this._displayText;
            clone._userName = this._userName;
            clone._password = this._password;
            clone._location = this._location;
            return clone;
        }
    }
}