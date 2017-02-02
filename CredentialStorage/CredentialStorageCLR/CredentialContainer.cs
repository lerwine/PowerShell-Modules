using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
namespace CredentialStorageCLR
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CredentialContainer : CredentialComponent
    {
        private System.Collections.Generic.List<ICredentialContentItem> _contents = new List<ICredentialContentItem>();

        /// <summary>
        /// 
        /// </summary>
        [XmlArrayItem(ElementName = CredentialStorageFolder.ElementName, IsNullable = false, Namespace = CredentialStorageFolder.NamespaceURI)]
        [XmlArrayItem(ElementName = CredentialDomain.ElementName, IsNullable = false, Namespace = CredentialDomain.NamespaceURI)]
        public List<ICredentialContentItem> Contents
        {
            get { return this._contents; }
            set { this._contents = value ?? new List<ICredentialContentItem>(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override CredentialComponent Clone()
        {
            CredentialContainer container = this.CreateCloneTemplate();
            container.CopyFrom(this);
            return container;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        protected virtual void CopyFrom(CredentialContainer other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            if (Object.ReferenceEquals(this, other))
                return;

            lock (this._contents)
            {
                if (this._contents.Count > 0)
                    this._contents.Clear();

                lock (other._contents)
                {
                    foreach (ICredentialContentItem item in other._contents)
                        this._contents.Add((item == null) ? null : item.Clone());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract CredentialContainer CreateCloneTemplate();
    }
}