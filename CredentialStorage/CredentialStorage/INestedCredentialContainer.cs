using System;
using System.ComponentModel;

namespace CredentialStorageCLR
{
    interface INestedCredentialContainer : IEquatable<ICredentialContainer>, ICredentialContainer, INestedContainer
    {
        new ICredentialContainer Owner { get; }
    }

    interface INestedCredentialContainer<T> : IEquatable<ICredentialContainer<T>>, ICredentialContainer<T>, INestedCredentialContainer
        where T : INestedCredentialContainer<T>
    {
    }
}
