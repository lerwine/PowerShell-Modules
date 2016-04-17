using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CredentialStorageCLR
{
    public interface ICredentialContainer : IEquatable<ICredentialContainer>, IList<ICredentialContent>, IDictionary<Guid, ICredentialContent>, IDictionary<string, ICredentialContent>, ICredentialComponent, IContainer
    {
        new CredentialComponentCollection Components { get; }
        void Add(ICredentialContent component, Guid id);
        IEnumerable<ICredentialContainer> GetParents();
    }

    public interface ICredentialContainer<T> : IEquatable<ICredentialContainer<T>>, ICredentialComponent<T>, ICredentialContainer
        where T : ICredentialContainer<T>
    {
    }
}