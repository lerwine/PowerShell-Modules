using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CredentialStorage.Model
{
    public interface IModelItem : IBaseItem
    {
        Guid ID { get; }
    }
}