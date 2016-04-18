using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CredentialStorageCLR
{
    [XmlRoot(ElementName = CredentialStorageDocument.ElementName, Namespace = CredentialStorageDocument.NamespaceURI)]
    [Serializable]
    public sealed class CredentialStorageDocument : CredentialContainer
    {
        public const string ElementName = "CredentialStorage";
        
        protected override CredentialContainer CreateCloneTemplate()
        {
            throw new NotImplementedException();
        }
    }
}