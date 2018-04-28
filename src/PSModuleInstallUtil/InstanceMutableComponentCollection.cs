using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace PSModuleInstallUtil
{
    public class InstanceMutableComponentCollection : ComponentCollection
    {
        public static InstanceMutableComponentCollection Create(out IList<IComponent> mutator)
        {
            InstanceMutableComponentCollection result = new InstanceMutableComponentCollection();
            mutator = new Mutator(result.InnerList);
            return result;
        }
        
        private InstanceMutableComponentCollection() : base(new IComponent[0]) { }

        public static IEnumerable<IComponent> AsIEnumerable(ComponentCollection collection)
        {
            foreach (IComponent item in collection)
            {
                if (item != null)
                    yield return item;
            }
        }

        public static bool Contains(ComponentCollection collection, IComponent component, bool recursive)
        {
            if (component == null || collection == null)
                return false;

            foreach (IComponent item in AsIEnumerable(collection))
            {
                if (ReferenceEquals(item, component))
                    return true;
            }

            if (!recursive)
                return false;

            foreach (IComponent item in AsIEnumerable(collection))
            {
                if (item is IContainer)
                {
                    IContainer container = item as IContainer;
                    if (Contains(container.Components, component, true))
                        return true;
                }
                
            }

            return false;
        }

        public class Mutator : IList<IComponent>, IList
        {
            private ArrayList _innerList;
            private object _syncRoot = new object();

            public Mutator(ArrayList innerList) { _innerList = innerList; }

            public IComponent this[int index]
            {
                get { return _innerList[index] as FileDirectoryComponent; }
                set { _innerList[index] = value; }
            }

            public int Count { get { return _innerList.Count; } }

            bool ICollection<IComponent>.IsReadOnly { get { return false; } }

            bool IList.IsReadOnly { get { return false; } }

            bool IList.IsFixedSize { get { return false; } }

            public object SyncRoot { get { return _syncRoot; } }

            bool ICollection.IsSynchronized { get { return true; } }

            object IList.this[int index]
            {
                get { return _innerList[index]; }
                set { _innerList[index] = value; }
            }

            public void Add(IComponent item) { _innerList.Add(item); }

            public void Clear() { _innerList.Clear(); }

            public bool Contains(IComponent item) { return _innerList.Contains(item); }

            public void CopyTo(IComponent[] array, int arrayIndex) { _innerList.CopyTo(array, arrayIndex); }

            public IEnumerable<IComponent> AsIEnumerable()
            {
                foreach (object item in _innerList)
                    yield return item as IComponent;
            }

            IEnumerator<IComponent> IEnumerable<IComponent>.GetEnumerator() { return AsIEnumerable().GetEnumerator(); }

            public IEnumerator GetEnumerator() { return _innerList.GetEnumerator(); }

            public int IndexOf(IComponent item) { return _innerList.IndexOf(item); }

            public void Insert(int index, IComponent item) { _innerList.Insert(index, item); }

            public bool Remove(IComponent item)
            {
                if (!_innerList.Contains(item))
                    return false;
                _innerList.Remove(item);
                return true;
            }

            public void RemoveAt(int index) { _innerList.RemoveAt(index); }

            int IList.Add(object value) { return _innerList.Add(value); }

            bool IList.Contains(object value) { return _innerList.Contains(value); }

            int IList.IndexOf(object value) { return _innerList.IndexOf(value); }

            void IList.Insert(int index, object value) { _innerList.Insert(index, value); }

            void IList.Remove(object value) { _innerList.Remove(value); }

            public void CopyTo(Array array, int index) { _innerList.CopyTo(array, index); }
        }
    }
}