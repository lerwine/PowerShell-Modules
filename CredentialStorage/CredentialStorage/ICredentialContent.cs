using System;

namespace CredentialStorageCLR
{
    public interface ICredentialContent : IEquatable<ICredentialContent>, ICredentialComponent
    {
        Guid Id { get; }
        string Name { get; set; }
        string NormalizedName { get; }
        ICredentialContent EnsureNestable(ICredentialContainer parent);
        void SetContainer(ICredentialContainer parent);
        new ICredentialContent Clone();
    }

    public interface ICredentialContent<T> : IEquatable<ICredentialContent<T>>, ICredentialComponent<T>, ICredentialContent
        where T : ICredentialContent<T>
    {
        new ICredentialContent<T> EnsureNestable(ICredentialContainer parent);
        new T Clone();
    }
}