using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CredentialStorageLibrary
{
    public class NestedDependencyObjectCollection<TParent, TItem> : DistinctDependencyObjectCollection<TItem>
        where TItem : DependencyObject, INestedDependencyObject<TItem, TParent>
        where TParent : DependencyObject, INestedDependencyParent<TParent, TItem>
    {
        private TParent _parent;

        public TParent Parent { get { return _parent; } }

        public NestedDependencyObjectCollection(TParent parent, IList<TItem> list) : base(list)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            TItem[] items = InnerCollection.ToArray();
            foreach (TItem item in items)
            {
                if (!InnerCollection.Any(i => ReferenceEquals(i, items)))
                    continue;
                if (item.Parent != null)
                {
                    if (ReferenceEquals(item.Parent, Parent))
                        continue;

                    item.Parent.Items.RemoveItem(item);
                }

                item.Parent = Parent;
            }
        }

        public NestedDependencyObjectCollection(TParent parent) : this(parent, new TItem[0]) { }

        protected override void OnItemAdded(TItem item, int index)
        {
            if (item.Parent != null)
            {
                if (ReferenceEquals(item.Parent, Parent))
                    return;

                item.Parent.Items.RemoveItem(item);
            }

            item.Parent = Parent;
        }

        protected override void OnItemRemoved(TItem item, int index)
        {
            if (item.Parent != null && ReferenceEquals(item.Parent, Parent))
                item.Parent = null;
        }
    }
}
