using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CredentialStorageCLR
{
    /// <summary>
    /// Represents a stored set of credentials
    /// </summary>
    [XmlRoot(ElementName = CredentialStorageDocument.ElementName, Namespace = CredentialStorageDocument.NamespaceURI)]
    [Serializable]
    public sealed class CredentialStorageDocument : CredentialContainer
    {
        /// <summary>
        /// Local element name
        /// </summary>
        public const string ElementName = "CredentialStorage";

        /// <summary>
        /// Create clone of current node.
        /// </summary>
        /// <returns>Clone of current node.</returns>
        protected override CredentialContainer CreateCloneTemplate()
        {
            throw new NotImplementedException();
        }
    }
}