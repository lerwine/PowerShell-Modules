using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace CredentialStorageCLR
{
    /// <summary>
    /// Base class for all credential component elements.
    /// </summary>
    public abstract class CredentialComponent : ICloneable
    {
        /// <summary>
        /// Namespace URI for all credential component elements.
        /// </summary>
        public const string NamespaceURI = "urn:Erwine.Leonard.T:PowerShell:CredentialStorage.psm1:CredentialStorage.xsd";
        
        /// <summary>
        /// Clone current node.
        /// </summary>
        /// <returns>Cloned node.</returns>
        protected abstract CredentialComponent Clone();

        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }
    }
}