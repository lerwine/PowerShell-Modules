using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace CredentialStorageCLR
{
    /// <summary>
    /// Base class for nodes that can contain ohter credential nodes.
    /// </summary>
    public abstract class CredentialContainer : CredentialComponent
    {
        private System.Collections.Generic.List<ICredentialContentItem> _contents = new List<ICredentialContentItem>();

        /// <summary>
        /// Content nodes
        /// </summary>
        [XmlArrayItem(ElementName = CredentialStorageFolder.ElementName, IsNullable = false, Namespace = CredentialStorageFolder.NamespaceURI)]
        [XmlArrayItem(ElementName = CredentialDomain.ElementName, IsNullable = false, Namespace = CredentialDomain.NamespaceURI)]
        public List<ICredentialContentItem> Contents
        {
            get { return this._contents; }
            set { this._contents = value ?? new List<ICredentialContentItem>(); }
        }

        /// <summary>
        /// Create clone of current node
        /// </summary>
        /// <returns>Cloned node</returns>
        protected override CredentialComponent Clone()
        {
            CredentialContainer container = this.CreateCloneTemplate();
            container.CopyFrom(this);
            return container;
        }

        /// <summary>
        /// Copy/clone properties and child nodes.
        /// </summary>
        /// <param name="other">Node to copy from</param>
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
        /// Create new empty node for cloning.
        /// </summary>
        /// <returns>New node for cloning.</returns>
        protected abstract CredentialContainer CreateCloneTemplate();
    }
}