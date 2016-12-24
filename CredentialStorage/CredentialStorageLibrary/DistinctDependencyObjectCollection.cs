using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;

namespace CredentialStorageLibrary
{
    public class DistinctDependencyObjectCollection<TItem> : ReadOnlyObservableCollection<TItem>
        where TItem : DependencyObject
    {
        public event EventHandler<NestedDependencyItemEventArgs<TItem>> ItemAdded;
        public event EventHandler<NestedDependencyItemEventArgs<TItem>> ItemRemoved;

        private object _syncRoot = new object();
        private ObservableCollection<TItem> _innerCollection;

        protected ObservableCollection<TItem> InnerCollection { get { return _innerCollection; } }

        private DistinctDependencyObjectCollection(ObservableCollection<TItem> collection) : base(collection)
        {
            _innerCollection = collection;
        }

        public DistinctDependencyObjectCollection(IList<TItem> list) : this(new ObservableCollection<TItem>(list.Where(i => i != null).Distinct())) { }

        public DistinctDependencyObjectCollection() : this(new ObservableCollection<TItem>()) { }

        public void AddItem(TItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Monitor.Enter(_syncRoot);
            try
            {
                if (_innerCollection.Any(i => ReferenceEquals(item, i)))
                    throw new ArgumentException("Item has already been added to this collection", "item");
                int index = _innerCollection.Count;
                OnAddingItem(item, index);
                _innerCollection.Add(item);
                OnItemAdded(item, index);
            }
            catch { throw; }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool AssertItemAdded(TItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Monitor.Enter(_syncRoot);
            try
            {
                if (_innerCollection.Any(i => ReferenceEquals(item, i)))
                    return false;
                int index = _innerCollection.Count;
                OnAddingItem(item, index);
                _innerCollection.Add(item);
                OnItemAdded(item, index);
            }
            catch { throw; }
            finally { Monitor.Exit(_syncRoot); }
            return true;
        }

        public void ClearItems()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                for (int index = 0; index < _innerCollection.Count; index++)
                    OnRemovingItem(_innerCollection[index], index);
                TItem[] removed = _innerCollection.ToArray();
                _innerCollection.Clear();
                for (int index = 0; index < removed.Length; index++)
                    OnItemRemoved(removed[index], index);
            }
            catch { throw; }
            finally { Monitor.Exit(_syncRoot); }
        }

        public void MoveItem(int oldIndex, int newIndex)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                TItem item = _innerCollection[oldIndex];
                _innerCollection.Move(oldIndex, newIndex);
                OnItemMoved(item, oldIndex, newIndex);
            }
            catch { throw; }
            finally { Monitor.Exit(_syncRoot); }
        }

        public void InsertItem(int index, TItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Monitor.Enter(_syncRoot);
            try
            {
                if (index < 0 || index > _innerCollection.Count)
                    throw new ArgumentOutOfRangeException("item");

                if (_innerCollection.Any(i => ReferenceEquals(item, i)))
                    throw new ArgumentException("Item has already been added to this collection", "item");

                OnAddingItem(item, index);
                if (index < _innerCollection.Count)
                    _innerCollection.Insert(index, item);
                else
                {
                    index = _innerCollection.Count;
                    _innerCollection.Add(item);
                }
                OnItemAdded(item, index);
            }
            catch { throw; }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool RemoveItem(TItem item)
        {
            if (item == null)
                return false;

            Monitor.Enter(_syncRoot);
            try
            {
                int index = _innerCollection.IndexOf(item);
                if (index < 0)
                    return false;

                OnRemovingItem(item, index);
                _innerCollection.RemoveAt(index);
                OnItemRemoved(item, index);
            }
            catch { throw; }
            finally { Monitor.Exit(_syncRoot); }

            return true;
        }

        public void RemoveItemAt(int index)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                TItem item = _innerCollection[index];
                OnRemovingItem(item, index);
                _innerCollection.RemoveAt(index);
                OnItemRemoved(item, index);
            }
            catch { throw; }
            finally { Monitor.Exit(_syncRoot); }
        }

        protected virtual void OnAddingItem(TItem item, int index) { }

        protected virtual void OnItemAdded(TItem item, int index)
        {
            ItemAdded?.Invoke(this, new NestedDependencyItemEventArgs<TItem>(item, index));
        }

        protected virtual void OnItemMoved(TItem item, int oldIndex, int newIndex) { }

        protected virtual void OnRemovingItem(TItem item, int index) { }

        protected virtual void OnItemRemoved(TItem item, int index)
        {
            ItemRemoved.Invoke(this, new NestedDependencyItemEventArgs<TItem>(item, index));
        }
    }
}
