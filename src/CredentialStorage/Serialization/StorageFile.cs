using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CredentialStorage.Serialization
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    [DataContract(Name = "CredentialStorage")]
    public class StorageFile
    {
        [DataMember()]
        public Realm[] realms { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

}