using System;
using System.Windows;

namespace CredentialStorageLibrary
{
    public interface INestedDependencyObject<TItem, TParent>
        where TItem : DependencyObject, INestedDependencyObject<TItem, TParent>
        where TParent : DependencyObject, INestedDependencyParent<TParent, TItem>
    {
        Guid Id { get; }
        TParent Parent { get; set; }
    }
}