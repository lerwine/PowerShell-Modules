using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace CredentialStorageCLR
{
    [XmlRoot(ElementName = CredentialStorageFolder.ElementName, Namespace = CredentialStorageFolder.NamespaceURI)]
    [Serializable]
    public class CredentialStorageFolder : CredentialContainer, ICredentialContentItem
    {
        public const string ElementName = "Folder";

        private string _displayText = "";
        private Guid? _id = null;

        [XmlAttribute]
        public string DisplayText
        {
            get { return this._displayText; }
            set { this._displayText = value ?? ""; }
        }

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

        bool ICredentialContentItem.IsFolder { get { return true; } }

        protected override CredentialContainer CreateCloneTemplate()
        {
            CredentialStorageFolder clone = new CredentialStorageFolder();
            clone._id = this.Id;
            clone._displayText = this._displayText;
            return clone;
        }

        ICredentialContentItem ICredentialContentItem.Clone() { return base.Clone() as ICredentialContentItem; }
    }
}