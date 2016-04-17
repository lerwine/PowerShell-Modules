using System;
using System.ComponentModel;

namespace CredentialStorageCLR
{
    public interface ICredentialSite : ISite
    {
        new ICredentialContent Component { get; }
        new ICredentialContainer Container { get; }
    }

    public interface ICredentialSite<T> : ICredentialSite
        where T : ICredentialComponent<T>
    {
        new ICredentialComponent<T> Component { get; }
    }
}
