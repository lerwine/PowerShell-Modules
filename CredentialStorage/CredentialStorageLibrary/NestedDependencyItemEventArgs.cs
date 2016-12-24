using System;
using System.Windows;

namespace CredentialStorageLibrary
{
    public class NestedDependencyItemEventArgs<TItem> : EventArgs
        where TItem : DependencyObject
    {
        public TItem Item { get; private set; }

        public int Index { get; private set; }

        public NestedDependencyItemEventArgs(TItem item, int index)
        {
            Item = item;
            Index = index;
        }
    }
}