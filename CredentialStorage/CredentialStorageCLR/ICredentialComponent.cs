using System;
using System.ComponentModel;

namespace CredentialStorageCLR
{
    public interface ICredentialComponent : IEquatable<ICredentialComponent>, IComponent, ICloneable
    {
        new ICredentialSite Site { get; set; }
        new ICredentialComponent Clone();
    }

    public interface ICredentialComponent<T> : IEquatable<ICredentialComponent<T>>, ICredentialComponent
        where T : ICredentialComponent<T>
    {
        new ICredentialSite<T> Site { get; set; }
        new T Clone();
    }
}
