using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CredentialStorage.Model
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IModelItem : IBaseItem
    {
        Guid ID { get; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}