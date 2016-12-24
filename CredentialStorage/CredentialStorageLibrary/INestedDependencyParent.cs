using System.Windows;

namespace CredentialStorageLibrary
{
    public interface INestedDependencyParent<TParent, TItem>
        where TParent : DependencyObject, INestedDependencyParent<TParent, TItem>
        where TItem : DependencyObject, INestedDependencyObject<TItem, TParent>
    {
        NestedDependencyObjectCollection<TParent, TItem> Items { get; }
    }
}