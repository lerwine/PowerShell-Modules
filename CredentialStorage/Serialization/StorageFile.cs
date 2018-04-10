using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CredentialStorage.Serialization
{
    [DataContract(Name = "CredentialStorage")]
    public class StorageFile
    {
        [DataMember()]
        public Realm[] realms { get; set; }
    }
}