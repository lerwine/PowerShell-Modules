using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CredentialStorage.Model
{
    public interface IBaseItem
    {
        string Title { get; set; }
        
        bool Deleted { get; }

        string Description { get; set; }

        Hashtable MetaData { get; }
    }
}