using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;

namespace Erwine.Leonard.T.GDIPlus
{
	public class CrawlComponentCollection<TKey> : IList<ICrawledComponent<TKey>> //, IList, INotifyCollectionChanged
		where TKey : IComparable
    {
        private object _syncRoot = new object();
		private IEqualityComparer<TKey> _keyComparer;
        private IList<ICrawledComponent<TKey>> _innerList = new List<ICrawledComponent<TKey>>();
        private ICrawlComponentContainer<TKey> _owner;

        #region IList<ICrawledComponent<TKey>> Implementation

        public ICrawledComponent<TKey> this[int index]
        {
            get { return _innerList[index]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                ICrawledComponent<TKey> oldItem;
                Monitor.Enter(_syncRoot);
                try
                {
                    int k = _IndexOfKey(value.Key);
                    if (k > -1 && index != k)
                        throw new ArgumentOutOfRangeException("index", value.Key, "Another item with that key already exist.");
                    oldItem = _SetComponent(index, value);
                } finally { Monitor.Exit(_syncRoot); }
                if (oldItem != null)
                    RaiseItemReplaced(oldItem, value, index);
            }
        }
        
        public int IndexOf(ICrawledComponent<TKey> item) { return _innerList.IndexOf(item); }
        
        public void Insert(int index, ICrawledComponent<TKey> item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            Monitor.Enter(_syncRoot);
            try
            {
                if (_IndexOfKey(item.Key) > -1)
                    throw new ArgumentOutOfRangeException("index", item.Key, "Another item with that key already exist.");
                _InsertComponent(index, item);
            } finally { Monitor.Exit(_syncRoot); }
            RaiseItemAdded(item, index);
        }

        public void RemoveAt(int index)
        {
            ICrawledComponent<TKey> item;
            Monitor.Enter(_syncRoot);
            try
            {
                item = _RemoveComponent(index);
            } finally { Monitor.Exit(_syncRoot); }
            RaiseItemRemoved(item, index);
        }
        
        #endregion
        
        #region ICollection<ICrawledComponent<TKey>> Implementation
        
        public int Count { get { return _innerList.Count; } }

        bool ICollection<ICrawledComponent<TKey>>.IsReadOnly { get { return false; } }
        
        public void Add(ICrawledComponent<TKey> item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            int index;
            Monitor.Enter(_syncRoot);
            try
            {
                if (_IndexOfKey(item.Key) > -1)
                    throw new ArgumentOutOfRangeException("index", item.Key, "Another item with that key already exist.");
                index = _AddComponent(item);
            } finally { Monitor.Exit(_syncRoot); }
            RaiseItemAdded(item, index);
        }
        
        public void Clear()
        {
            ICrawledComponent<TKey>[] removedItems;
            Monitor.Enter(_syncRoot);
            try
            {
                if (_innerList.Count == 0)
                    return;
                removedItems = _innerList.ToArray();
                _innerList.Clear();
            } finally { Monitor.Exit(_syncRoot); }
            foreach (ICrawledComponent<TKey> item in removedItems)
                item.Parent = null;
            RaiseItemsRemoved(removedItems, 0);
        }
        
        public bool Contains(ICrawledComponent<TKey> item) { return item != null && _innerList.Contains(item); }
        
        public void CopyTo(ICrawledComponent<TKey>[] array, int arrayIndex) { _innerList.CopyTo(array, arrayIndex); }

        public bool Remove(ICrawledComponent<TKey> item)
        {
            if (item == null)
                return false;
            int index;
            Monitor.Enter(_syncRoot);
            try
            {
                if ((index = _innerList.IndexOf(item)) < 0)
                    return false;
                _RemoveComponent(index);
            } finally { Monitor.Exit(_syncRoot); }
            RaiseItemRemoved(item, index);
            return true;
        }
        
        #endregion
        
        #region IEnumerable Implementations
        
        public IEnumerator<ICrawledComponent<TKey>> GetEnumerator() { return _innerList.GetEnumerator(); }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return (_innerList as System.Collections.IEnumerable).GetEnumerator(); }

        #endregion
        
        /*
        public ICrawledComponent<TKey> this[TKey key]
        {
            get { return _innerList; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                if (!_keyComparer.Equals(value, key))
                    throw new ArgumentOutOfRangeException("key", key, "Key does not match the item being added.");

                Monitor.Enter(_syncRoot);
                try
                {
                    int index = _IndexOfKey(key);
                    if (index < 0)
                        _AddComponent(value);
                    else
                        _SetComponent(index, value);
                } finally { Monitor.Exit(_syncRoot); }
            }
        }
        */
        private void _InsertComponent(int index, ICrawledComponent<TKey> item)
        {
            _innerList.Insert(index, item);
            if (item.Parent != null && !ReferenceEquals(item.Parent, _owner))
                _innerList[index] = item.Clone(_owner);
            else
                _innerList[index].Parent = _owner;
        }
        private int _AddComponent(ICrawledComponent<TKey> item)
        {
            int index = _innerList.Count;
            _innerList.Add(item);
            if (item.Parent != null && !ReferenceEquals(item.Parent, _owner))
                _innerList[index] = item.Clone(_owner);
            else
                _innerList[index].Parent = _owner;
            return index;
        }
        private  ICrawledComponent<TKey> _RemoveComponent(int index)
        {
            ICrawledComponent<TKey> oldItem = _innerList[index];
            _innerList.RemoveAt(index);
            oldItem.Parent = null;
            return oldItem;
        }
        private ICrawledComponent<TKey> _SetComponent(int index, ICrawledComponent<TKey> item)
        {
            ICrawledComponent<TKey> oldItem = _innerList[index];
            if (ReferenceEquals(oldItem, item))
                return null;
            int k = _IndexOfKey(item.Key);
            if (k > -1 && k != index)
                throw new ArgumentOutOfRangeException("item", item.Key, "Another item withthe same key already exists.");
            _innerList[index] = item;
            if (item.Parent != null && !ReferenceEquals(item.Parent, _owner))
                _innerList[index] = item.Clone(_owner);
            else
                _innerList[index].Parent = _owner;
            oldItem.Parent = null;
            return oldItem;
        }
        private int _IndexOfKey(TKey key)
        {
            for (int i = 0; i < _innerList.Count; i++)
            {
                if (_keyComparer.Equals(_innerList[i].Key, key))
                    return i;
            }
            return -1;
        }
        private void RaiseItemAdded(ICrawledComponent<TKey> newItem, int index)
        {
            throw new NotImplementedException();
        }
        private void RaiseItemReplaced(ICrawledComponent<TKey> oldItem, ICrawledComponent<TKey> newItem, int index)
        {
            throw new NotImplementedException();
        }
        private void RaiseItemRemoved(ICrawledComponent<TKey> oldItem, int index)
        {
            throw new NotImplementedException();
        }
        private void RaiseItemsRemoved(ICrawledComponent<TKey>[] oldItems, int index)
        {
            throw new NotImplementedException();
        }
        protected virtual void OnItemsInitialized(ICrawledComponent<TKey>[] existingItems)
        {
            throw new NotImplementedException();
        }
        public CrawlComponentCollection() : this(null, null) { }
        public CrawlComponentCollection(ICrawlComponentContainer<TKey> owner, IEqualityComparer<TKey> keyComparer)
        {
            _owner = owner;
            _keyComparer = (keyComparer == null) ? EqualityComparer<TKey>.Default : keyComparer;
        }
        internal ICrawledComponent<TKey>[] Initialize(ICrawlComponentContainer<TKey> owner, IEqualityComparer<TKey> keyComparer)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            List<ICrawledComponent<TKey>> existingItems = new List<ICrawledComponent<TKey>>();
            Monitor.Enter(_syncRoot);
            try
            {
                if (!ReferenceEquals(owner.ItemCollection, this))
                    throw new InvalidOperationException("Component container does not own this collection.");
                
                if (_owner != null)
                {
                    if (!ReferenceEquals(_owner, owner))
                        throw new InvalidOperationException("Collection owner cannot be changed once it is initialized.");
                    _keyComparer = (keyComparer == null) ? EqualityComparer<TKey>.Default : keyComparer;
                }
                else if (keyComparer != null)
                    _keyComparer = keyComparer;
                _owner = owner;
                existingItems.AddRange(_innerList);
            } finally { Monitor.Exit(_syncRoot); }
            if (existingItems.Count > 0)
            {
                for (int i = 0; i < existingItems.Count; i++)
                {
                    try
                    {
                        int index = _innerList.IndexOf(existingItems[i]);
                        if (index < 0)
                        {
                            existingItems.RemoveAt(i);
                            i--;
                            continue;
                        }
                        if (existingItems[i].Parent != null)
                        {
                            if (ReferenceEquals(existingItems[i].Parent, _owner))
                            {
                                existingItems.RemoveAt(i);
                                i--;
                            }
                            else
                            {
                                existingItems[i] = _innerList[index].Clone(_owner);
                                _innerList[index] = existingItems[i];
                            }
                            continue;
                        }
                    } finally { Monitor.Exit(_syncRoot); }
                    existingItems[i].Parent = _owner;
                }
            }
            OnItemsInitialized(existingItems.ToArray());
            return existingItems.ToArray();
        }
    }
    public interface ICrawlComponentContainer<TKey> : /*IEquatable<ICrawlComponentContainer<TKey>>, IComparable<ICrawlComponentContainer<TKey>>, */INotifyPropertyChanging, INotifyPropertyChanged
    {
        CrawlComponentCollection<TKey> ItemCollection { get; }
    }
    public interface ICrawledComponent<TKey> : /*IEquatable<CrawledComponent<TKey>>, IEquatable<TKey>, IEquatable<IComparable<TKey>>, IComparable<TKey>, */INotifyPropertyChanging, INotifyPropertyChanged, ICloneable
    {
        TKey Key { get; set; }
        ICrawlComponentContainer<TKey> Parent { get; set; }
        ICrawledComponent<TKey> Clone(ICrawlComponentContainer<TKey> parent);
        new ICrawledComponent<TKey> Clone();
    }
    public interface INestedCrawlComponentContainer<TKey> : ICrawlComponentContainer<TKey>, ICrawledComponent<TKey>
    {
        new INestedCrawlComponentContainer<TKey> Clone(ICrawlComponentContainer<TKey> directory);
        new INestedCrawlComponentContainer<TKey> Clone();
    }
    public abstract class CrawledComponent<TKey> : ICrawledComponent<TKey>
    {
        private TKey _key;
		private ICrawlComponentContainer<TKey> _parent;
		private static IEqualityComparer<TKey> _keyComparer;
		
		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected const string PropertyName_Key = "Key";
		protected const string PropertyName_Parent = "Parent";
		
		public virtual TKey Key
		{
			get { return _key; }
			set
			{
				if (_keyComparer.Equals(value, _key))
					return;
				RaisePropertyChanging(PropertyName_Key);
				_key = value;
				RaisePropertyChanged(PropertyName_Key);
			}
		}
		
		public ICrawlComponentContainer<TKey> Parent
		{
			get { return _parent; }
			set
			{
                if (ReferenceEquals(value, _parent))
                    return;
				RaisePropertyChanging(PropertyName_Parent);
				_parent = value;
				RaisePropertyChanged(PropertyName_Parent);
			}
		}
		
		static CrawledComponent()
		{
			_keyComparer = EqualityComparer<TKey>.Default;
		}
		
		protected CrawledComponent()
		{
			
		}
		
		protected CrawledComponent(CrawledComponent<TKey> toClone, ICrawlComponentContainer<TKey> parent) : this((toClone == null) ? default(TKey) : toClone.Key, parent) { }
		
		protected CrawledComponent(TKey key, ICrawlComponentContainer<TKey> parent)
		{
		    _key = key;
		    _parent = parent;
		}
		
		protected void RaisePropertyChanging(string propertyName)
		{
			PropertyChangingEventArgs args = new PropertyChangingEventArgs(propertyName);
			try { OnPropertyChanging(args); }
			finally
			{
				PropertyChangingEventHandler propertyChanging = PropertyChanging;
				if (propertyChanging != null)
					propertyChanging(this, args);
			}
		}
		
		protected virtual void OnPropertyChanging(PropertyChangingEventArgs args) { }
		
		protected void RaisePropertyChanged(string propertyName)
		{
			PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
			try { OnPropertyChanged(args); }
			finally
			{
				PropertyChangedEventHandler propertyChanged = PropertyChanged;
				if (propertyChanged != null)
					propertyChanged(this, args);
			}
		}
		
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) { }
        protected abstract CrawledComponent<TKey> CreateClone(ICrawlComponentContainer<TKey> parent);
        ICrawledComponent<TKey> ICrawledComponent<TKey>.Clone(ICrawlComponentContainer<TKey> parent) { return CreateClone(parent); }
        ICrawledComponent<TKey> ICrawledComponent<TKey>.Clone() { return CreateClone(Parent); }
        object ICloneable.Clone() { return CreateClone(Parent); }
	}
}
