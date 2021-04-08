using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CredentialStorage.Model
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IBaseItem
    {
        string Title { get; set; }
        
        bool Deleted { get; }

        string Description { get; set; }

        Hashtable MetaData { get; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}