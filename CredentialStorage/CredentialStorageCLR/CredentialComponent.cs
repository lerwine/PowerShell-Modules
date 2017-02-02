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
    public abstract class CredentialComponent : ICloneable
    {
        /// <summary>
        /// 
        /// </summary>
        public const string NamespaceURI = "urn:Erwine.Leonard.T:PowerShell:CredentialStorage.psm1:CredentialStorage.xsd";
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract CredentialComponent Clone();

        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }
    }
}