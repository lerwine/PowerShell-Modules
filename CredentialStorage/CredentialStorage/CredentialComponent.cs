using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace CredentialStorageCLR
{
    public abstract class CredentialComponent : ICloneable
    {
        public const string NamespaceURI = "urn:Erwine.Leonard.T:PowerShell:CredentialStorage.psm1:CredentialStorage.xsd";
        
        protected abstract CredentialComponent Clone();

        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }
    }
}