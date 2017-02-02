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
    /// 
    /// </summary>
    [XmlRoot(ElementName = CredentialStorageDocument.ElementName, Namespace = CredentialStorageDocument.NamespaceURI)]
    [Serializable]
    public sealed class CredentialStorageDocument : CredentialContainer
    {
        /// <summary>
        /// 
        /// </summary>
        public const string ElementName = "CredentialStorage";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override CredentialContainer CreateCloneTemplate()
        {
            throw new NotImplementedException();
        }
    }
}