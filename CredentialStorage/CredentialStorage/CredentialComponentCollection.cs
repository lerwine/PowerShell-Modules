using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace CredentialStorageCLR
{
    [Serializable]
    public class CredentialComponentCollection : ComponentCollection, IList<ICredentialContent>, IDictionary<Guid, ICredentialContent>, IDictionary<string, ICredentialContent>, IEquatable<CredentialComponentCollection>
    {
        private ICredentialContainer _parent;
        private ContentCollection _values;
        private GuidCollection _ids;
        private NameCollection _names;

        public static readonly Regex WhiteSpaceRegex = new Regex(@"\s+", RegexOptions.Compiled);

        bool ICollection<ICredentialContent>.IsReadOnly { get { return false; } }

        bool ICollection<KeyValuePair<Guid, ICredentialContent>>.IsReadOnly { get { return false; } }

        bool ICollection<KeyValuePair<string, ICredentialContent>>.IsReadOnly { get { return false; } }

        [XmlIgnore]
        public ICollection<Guid> Keys { get { return this._ids; } }

        [XmlIgnore]
        public ICollection<ICredentialContent> Values { get { return this._values; } }

        ICollection<string> IDictionary<string, ICredentialContent>.Keys { get { return this._names; } }

        public override IComponent this[string name] { get { return this._names[name]; } }

        ICredentialContent IDictionary<string, ICredentialContent>.this[string key]
        {
            get { return this._names[key]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                lock (this._values)
                {
                    int index = this._names.BaseIndexOf(key);
                    if (index < 0)
                        this._SetAt(this.InnerList.Count, value);
                    else
                        this._SetAt(index, value);
                }
            }
        }

        internal void EnsureNestable(ICredentialContainer parent)
        {
            lock (this._values)
            {
                for (int i = 0; i<this.InnerList.Count; i++)
                {
                    ICredentialContent item = this._values[i].EnsureNestable(parent);
                    if (!Object.ReferenceEquals(item, this._values[i]))
                    {
                        ICredentialContent oldItem = this._values[i];
                        this.InnerList[i] = item;
                        oldItem.SetContainer(null);
                        item.SetContainer(this._parent);
                    }
                }
            }
        }

        public bool Equals(CredentialComponentCollection other)
        {
            if (other == null)
                return false;

            if (Object.ReferenceEquals(other, this))
                return true;

            lock (this._values)
            {
                lock (other._values)
                {
                    if (this._values.Count != other._values.Count)
                        return false;

                    for (int i = 0; i < this._values.Count; i++)
                    {
                        if (!this._values[i].Equals(other._values[i]))
                            return false;
                    }
                }
            }

            return true;
        }

        public ICredentialContent this[Guid key]
        {
            get { return this._ids[key]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                lock (this._values)
                {
                    int index = this._ids.BaseIndexOf(key);
                    if (index < 0)
                        this._SetAt(this.InnerList.Count, value);
                    else
                        this._SetAt(index, value);
                }
            }
        }

        public new ICredentialContent this[int index]
        {
            get { return (ICredentialContent)(base[index]); }
            set
            {
                if (index < 0)
                    throw new IndexOutOfRangeException();

                if (value == null)
                    throw new ArgumentNullException();

                lock (this._values)
                {
                    if (index > this.InnerList.Count)
                        throw new IndexOutOfRangeException();

                    this._SetAt(index, value);
                }
            }
        }

        public CredentialComponentCollection(ICredentialContainer parent) : this(parent, new ICredentialContent[0]) { }

        public CredentialComponentCollection(ICredentialContainer parent, IEnumerable<ICredentialContent> collection)
            : base(new IComponent[0])
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (parent.Components != null)
                throw new InvalidOperationException("Parent component container cannot be changed.");

            this._values = new ContentCollection(this.InnerList);
            this._ids = new GuidCollection(this._values);
            this._names = new NameCollection(this._values);

            if (collection == null)
                return;

            foreach (ICredentialContent item in collection)
            {
                if (item != null)
                    this.Add(item);
            }
        }

        private void _SetAt(int index, ICredentialContent value)
        {
            ICredentialContent movedItem, replacedItem;
            int oldIndex = this._values.IndexOf(value);
            replacedItem = (index < this.InnerList.Count) ? this._values[index] : null;
            if (oldIndex < 0)
                movedItem = null;
            else
            {
                movedItem = this._values[oldIndex];
                this.InnerList.RemoveAt(oldIndex);
                if (oldIndex < index)
                    index--;
            }
            if (index < this.InnerList.Count)
                this.InnerList[index] = value;
            else
                this.InnerList.Add(value);
            if (replacedItem != null && !Object.ReferenceEquals(replacedItem, value) && replacedItem.Site != null && Object.ReferenceEquals(replacedItem.Site.Container, this._parent))
                replacedItem.SetContainer(null);
            if (value.Site == null || !Object.ReferenceEquals(value.Site.Container, this._parent))
                value.SetContainer(this._parent);
        }

        private void _RemoveAt(int index)
        {
            if (index < 0 || index >= this.InnerList.Count)
                throw new IndexOutOfRangeException();

            ICredentialContent item = this._values[index];
            this.InnerList.RemoveAt(index);
        }

        private void _Clear()
        {
            List<ICredentialContent> items = new List<ICredentialContent>(this._values);
            this.InnerList.Clear();
            foreach (ICredentialContent item in items)
            {
                if (item.Site != null && Object.ReferenceEquals(item.Site.Container, this._parent))
                    item.SetContainer(null);
            }
        }

        public static string ConvertToNormalizedName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return name;

            return CredentialComponentCollection.WhiteSpaceRegex.Replace(name.Trim(), "");
        }

        public int IndexOf(ICredentialContent item) { return this.InnerList.IndexOf(item); }

        public void Insert(int index, ICredentialContent item)
        {
            lock (this._values)
            {
                ICredentialContent movedItem;
                int oldIndex = this._values.IndexOf(item);
                if (index == oldIndex)
                {
                    if (Object.ReferenceEquals(item, this._values[index]))
                        return;
                    movedItem = this._values[index];
                    item = item.EnsureNestable(this._parent);
                    this.InnerList[index] = item;
                    if (movedItem.Site != null && Object.ReferenceEquals(movedItem.Site.Container, this._parent))
                        movedItem.SetContainer(null);
                    if (item.Site == null || !Object.ReferenceEquals(item.Site.Container, this._parent))
                        item.SetContainer(this._parent);
                    return;
                }

                item = item.EnsureNestable(this._parent);
                movedItem = (oldIndex < 0) ? null : this._values[oldIndex];
                if (movedItem != null)
                {
                    movedItem = this._values[oldIndex];
                    this.InnerList.RemoveAt(oldIndex);
                    if (oldIndex < index)
                        index--;
                    if (movedItem.Site != null && Object.ReferenceEquals(movedItem.Site.Container, this._parent))
                        movedItem.SetContainer(null);
                }

                if (index < this.InnerList.Count)
                    this.InnerList.Add(item);
                else
                    this.InnerList.Insert(index, item);

                item.SetContainer(this._parent);
            }
        }

        public void RemoveAt(int index)
        {
            lock (this._values)
                this._RemoveAt(index);
        }

        public void Add(ICredentialContent item)
        {
            lock (this._values)
                this._SetAt(this.InnerList.Count, item);
        }

        public void Clear() { this._Clear(); }

        public bool Contains(ICredentialContent item) { return this._values.Contains(item); }

        public void CopyTo(ICredentialContent[] array, int arrayIndex) { ((IList<ICredentialContent>)(this._values)).CopyTo(array, arrayIndex); }

        public bool Remove(ICredentialContent item)
        {
            lock (this._values)
            {
                int index = this._values.IndexOf(item);
                if (index < 0)
                    return false;

                this._RemoveAt(index);
            }

            return true;
        }

        public IEnumerator<ICredentialContent> GetEnumerator() { return this._values.GetEnumerator(); }

        public bool ContainsKey(Guid key) { return this._ids.Contains(key); }

        void IDictionary<Guid, ICredentialContent>.Add(Guid key, ICredentialContent value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (!value.Id.Equals(key))
                throw new ArgumentException("Key does not match Id.", "key");

            this.Add(value);
        }

        public bool Remove(Guid key)
        {
            lock (this._values)
            {
                int index = this._ids.BaseIndexOf(key);
                if (index < 0)
                    return false;
                this._RemoveAt(index);
            }

            return true;
        }

        public bool TryGetValue(Guid key, out ICredentialContent value) { return _ids.TryGetValue(key, out value); }

        void ICollection<KeyValuePair<Guid, ICredentialContent>>.Add(KeyValuePair<Guid, ICredentialContent> item)
        {
            if (item.Value == null)
                throw new ArgumentException("Value cannot be null.", "item");

            if (!item.Value.Id.Equals(item.Key))
                throw new ArgumentException("Key does not match Value Id.", "item");

            this.Add(item.Value);
        }

        bool ICollection<KeyValuePair<Guid, ICredentialContent>>.Contains(KeyValuePair<Guid, ICredentialContent> item)
        {
            return ((IDictionary<Guid, ICredentialContent>)_ids).Contains(item);
        }

        void ICollection<KeyValuePair<Guid, ICredentialContent>>.CopyTo(KeyValuePair<Guid, ICredentialContent>[] array, int arrayIndex)
        {
            ((IDictionary<Guid, ICredentialContent>)_ids).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<Guid, ICredentialContent>>.Remove(KeyValuePair<Guid, ICredentialContent> item)
        {
            if (item.Value == null || !item.Value.Id.Equals(item.Key))
                return false;

            return this.Remove(item.Value);
        }

        IEnumerator<KeyValuePair<Guid, ICredentialContent>> IEnumerable<KeyValuePair<Guid, ICredentialContent>>.GetEnumerator()
        {
            return ((IDictionary<Guid, ICredentialContent>)_ids).GetEnumerator();
        }

        public bool ContainsKey(string key) { return this._names.Contains(key); }

        public void Add(string key, ICredentialContent value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (!value.Id.Equals(key))
                throw new ArgumentException("Key does not match Id.", "key");

            this.Add(value);
        }

        public bool Remove(string key)
        {
            return ((IDictionary<string, ICredentialContent>)_names).Remove(key);
        }

        bool IDictionary<string, ICredentialContent>.TryGetValue(string key, out ICredentialContent value)
        {
            return _names.TryGetValue(key, out value);
        }

        void ICollection<KeyValuePair<string, ICredentialContent>>.Add(KeyValuePair<string, ICredentialContent> item)
        {
            ((IDictionary<string, ICredentialContent>)_names).Add(item);
        }

        bool ICollection<KeyValuePair<string, ICredentialContent>>.Contains(KeyValuePair<string, ICredentialContent> item)
        {
            return ((IDictionary<string, ICredentialContent>)_names).Contains(item);
        }

        void ICollection<KeyValuePair<string, ICredentialContent>>.CopyTo(KeyValuePair<string, ICredentialContent>[] array, int arrayIndex)
        {
            ((IDictionary<string, ICredentialContent>)_names).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, ICredentialContent>>.Remove(KeyValuePair<string, ICredentialContent> item)
        {
            if (item.Value == null || item.Key == null || String.Compare(CredentialComponentCollection.ConvertToNormalizedName(item.Key), item.Value.NormalizedName) != 0)
                return false;


            lock (this._values)
            {
                int index = this._names.BaseIndexOf(item.Key);
                if (index < 0)
                    return false;
                this._RemoveAt(index);
            }

            return true;
        }

        IEnumerator<KeyValuePair<string, ICredentialContent>> IEnumerable<KeyValuePair<string, ICredentialContent>>.GetEnumerator()
        {
            return ((IDictionary<string, ICredentialContent>)_names).GetEnumerator();
        }

        private sealed class ContentCollection : IList<ICredentialContent>
        {
            private ArrayList _innerList;

            public ContentCollection(ArrayList innerList) { this._innerList = innerList; }

            public ICredentialContent this[int index] { get { return (ICredentialContent)(this._innerList[index]); } }

            ICredentialContent IList<ICredentialContent>.this[int index]
            {
                get { return this[index]; }
                set { throw new NotSupportedException(); }
            }

            public int Count { get { return this._innerList.Count; } }

            bool ICollection<ICredentialContent>.IsReadOnly { get { return true; } }

            void ICollection<ICredentialContent>.Add(ICredentialContent item) { throw new NotSupportedException(); }

            void ICollection<ICredentialContent>.Clear() { throw new NotSupportedException(); }

            public bool Contains(ICredentialContent item) { return this._innerList.Contains(item); }

            void ICollection<ICredentialContent>.CopyTo(ICredentialContent[] array, int arrayIndex) { this._innerList.CopyTo(array, arrayIndex); }

            IEnumerator IEnumerable.GetEnumerator() { return this._innerList.GetEnumerator(); }

            public IEnumerator<ICredentialContent> GetEnumerator() { return this.AsEnumerable().GetEnumerator(); }

            public IEnumerable<ICredentialContent> AsEnumerable()
            {
                foreach (ICredentialContent item in this._innerList)
                    yield return item;
            }

            public int IndexOf(ICredentialContent item)
            {
                if (item == null)
                    return -1;

                int i = 0, index = -1;
                foreach (ICredentialContent c in this._innerList)
                {
                    if (Object.ReferenceEquals(item, c))
                        return i;
                    if (c.Id.Equals(item.Id) && index < 0)
                        index = i;
                    i++;
                }

                return index;
            }

            void IList<ICredentialContent>.Insert(int index, ICredentialContent item) { throw new NotSupportedException(); }

            bool ICollection<ICredentialContent>.Remove(ICredentialContent item) { throw new NotSupportedException(); }

            void IList<ICredentialContent>.RemoveAt(int index) { throw new NotSupportedException(); }
        }

        private sealed class GuidCollection : IList<Guid>, IDictionary<Guid, ICredentialContent>
        {
            private IList<ICredentialContent> _innerList;

            public GuidCollection(IList<ICredentialContent> innerList)
            {
                this._innerList = innerList;
            }

            public Guid this[int index]
            {
                get
                {
                    if (index < 0)
                        throw new IndexOutOfRangeException("Index cannot be less than zero.");

                    int i = 0;
                    foreach (ICredentialContent item in this.DistinctItems())
                    {
                        if (i == index)
                            return item.Id;
                    }

                    throw new IndexOutOfRangeException("Index cannot be greater than the number of items.");
                }
            }

            public ICredentialContent this[Guid key]
            {
                get
                {
                    foreach (ICredentialContent item in this._innerList)
                    {
                        if (item.Id.Equals(key))
                            return item;
                    }

                    throw new KeyNotFoundException();
                }
            }

            ICredentialContent IDictionary<Guid, ICredentialContent>.this[Guid key]
            {
                get { return this[key]; }
                set { throw new NotSupportedException(); }
            }

            Guid IList<Guid>.this[int index]
            {
                get { return this[index]; }
                set { throw new NotSupportedException(); }
            }
            
            public int Count
            {
                get
                {
                    int count = 0;
                    foreach (ICredentialContent item in this.DistinctItems())
                        count++;
                    return count;
                }
            }

            public bool IsReadOnly { get { return true; } }
            
            ICollection<Guid> IDictionary<Guid, ICredentialContent>.Keys { get { return this; } }

            ICollection<ICredentialContent> IDictionary<Guid, ICredentialContent>.Values { get { return this._innerList; } }

            void ICollection<KeyValuePair<Guid, ICredentialContent>>.Add(KeyValuePair<Guid, ICredentialContent> item) { throw new NotSupportedException(); }

            void ICollection<Guid>.Add(Guid item) { throw new NotSupportedException(); }

            void IDictionary<Guid, ICredentialContent>.Add(Guid key, ICredentialContent value) { throw new NotSupportedException(); }

            void ICollection<KeyValuePair<Guid, ICredentialContent>>.Clear() { throw new NotSupportedException(); }

            void ICollection<Guid>.Clear() { throw new NotSupportedException(); }

            bool ICollection<KeyValuePair<Guid, ICredentialContent>>.Contains(KeyValuePair<Guid, ICredentialContent> item)
            {
                return item.Value != null && item.Key.Equals(item.Value.Id) && this.Contains(item.Key);
            }

            public bool Contains(Guid id)
            {
                foreach (ICredentialContent item in this.DistinctItems())
                {
                    if (id.Equals(item.Id))
                        return true;
                }

                return false;
            }

            bool IDictionary<Guid, ICredentialContent>.ContainsKey(Guid key) { return this.Contains(key); }

            void ICollection<KeyValuePair<Guid, ICredentialContent>>.CopyTo(KeyValuePair<Guid, ICredentialContent>[] array, int arrayIndex)
            {
                (new List<KeyValuePair<Guid, ICredentialContent>>(this.DistinctPairs())).CopyTo(array, arrayIndex);
            }

            void ICollection<Guid>.CopyTo(Guid[] array, int arrayIndex)
            {
                (new List<Guid>(this.DistinctKeys())).CopyTo(array, arrayIndex);
            }
            
            public IEnumerable<ICredentialContent> DistinctItems()
            {
                List<Guid> ids = new List<Guid>();
                foreach (ICredentialContent item in this._innerList)
                {
                    if (ids.Contains(item.Id))
                        continue;
                    yield return item;
                    ids.Add(item.Id);
                }
            }

            public IEnumerable<Guid> DistinctKeys()
            {
                foreach (ICredentialContent item in this.DistinctItems())
                    yield return item.Id;
            }

            public IEnumerable<KeyValuePair<Guid, ICredentialContent>> DistinctPairs()
            {
                foreach (ICredentialContent item in this.DistinctItems())
                    yield return new KeyValuePair<Guid, ICredentialContent>(item.Id, item);
            }

            IEnumerator IEnumerable.GetEnumerator() { return (this.DistinctKeys() as IEnumerable).GetEnumerator(); }

            IEnumerator<KeyValuePair<Guid, ICredentialContent>> IEnumerable<KeyValuePair<Guid, ICredentialContent>>.GetEnumerator()
            {
                return this.DistinctPairs().GetEnumerator();
            }

            public IEnumerator<Guid> GetEnumerator() { return this.DistinctKeys().GetEnumerator(); }

            public int IndexOf(Guid id)
            {
                int index = 0;

                foreach (ICredentialContent item in this.DistinctItems())
                {
                    if (item.Id.Equals(id))
                        return index;
                    index++;
                }

                return -1;
            }


            public int BaseIndexOf(Guid key)
            {
                int index = 0;

                foreach (ICredentialContent item in this._innerList)
                {
                    if (item.Id.Equals(key))
                        return index;
                    index++;
                }

                return -1;
            }

            void IList<Guid>.Insert(int index, Guid item) { throw new NotSupportedException(); }

            bool ICollection<KeyValuePair<Guid, ICredentialContent>>.Remove(KeyValuePair<Guid, ICredentialContent> item) { throw new NotSupportedException(); }

            bool IDictionary<Guid, ICredentialContent>.Remove(Guid key) { throw new NotSupportedException(); }

            bool ICollection<Guid>.Remove(Guid item) { throw new NotSupportedException(); }

            void IList<Guid>.RemoveAt(int index) { throw new NotSupportedException(); }

            public bool TryGetValue(Guid key, out ICredentialContent value)
            {
                foreach (ICredentialContent item in this._innerList)
                {
                    if (item.Id.Equals(key))
                    {
                        value = item;
                        return true;
                    }
                }

                value = null;
                return false;
            }
        }

        private sealed class NameCollection : IList<string>, IDictionary<string, ICredentialContent>
        {
            private IList<ICredentialContent> _innerList;

            public NameCollection(IList<ICredentialContent> innerList)
            {
                this._innerList = innerList;
            }

            public string this[int index]
            {
                get
                {
                    if (index < 0)
                        throw new IndexOutOfRangeException("Index cannot be less than zero.");

                    int i = 0;
                    foreach (ICredentialContent item in this.DistinctItems())
                    {
                        if (i == index)
                            return item.NormalizedName;
                    }

                    throw new IndexOutOfRangeException("Index cannot be greater than the number of items.");
                }
            }

            public ICredentialContent this[string key]
            {
                get
                {
                    if (key == null)
                        throw new ArgumentNullException("key");

                    string n = CredentialComponentCollection.ConvertToNormalizedName(key);

                    foreach (ICredentialContent item in this._innerList)
                    {
                        if (String.Compare(item.NormalizedName, n, true) == 0)
                            return item;
                    }

                    throw new KeyNotFoundException();
                }
            }

            ICredentialContent IDictionary<string, ICredentialContent>.this[string key]
            {
                get { return this[key]; }
                set { throw new NotSupportedException(); }
            }

            string IList<string>.this[int index]
            {
                get { return this[index]; }
                set { throw new NotSupportedException(); }
            }

            public int Count
            {
                get
                {
                    int count = 0;
                    foreach (ICredentialContent item in this.DistinctItems())
                        count++;
                    return count;
                }
            }

            public bool IsReadOnly { get { return true; } }
            
            ICollection<string> IDictionary<string, ICredentialContent>.Keys { get { return this; } }

            ICollection<ICredentialContent> IDictionary<string, ICredentialContent>.Values { get { return this._innerList; } }

            void ICollection<KeyValuePair<string, ICredentialContent>>.Add(KeyValuePair<string, ICredentialContent> item) { throw new NotSupportedException(); }

            void ICollection<string>.Add(string item) { throw new NotSupportedException(); }

            void IDictionary<string, ICredentialContent>.Add(string key, ICredentialContent value) { throw new NotSupportedException(); }

            void ICollection<KeyValuePair<string, ICredentialContent>>.Clear() { throw new NotSupportedException(); }

            void ICollection<string>.Clear() { throw new NotSupportedException(); }

            bool ICollection<KeyValuePair<string, ICredentialContent>>.Contains(KeyValuePair<string, ICredentialContent> item)
            {
                if (item.Key == null || item.Value == null)
                    return false;

                string n = CredentialComponentCollection.ConvertToNormalizedName(item.Key);

                if (String.Compare(n, item.Value.NormalizedName, true) != 0)
                    return false;

                foreach (ICredentialContent i in this.DistinctItems())
                {
                    if (String.Compare(n, i.NormalizedName, true) == 0 && item.Value.Id.Equals(i.Id))
                        return true;
                }

                return false;
            }

            public bool Contains(string name)
            {
                if (name == null)
                    return false;

                string n = CredentialComponentCollection.ConvertToNormalizedName(name);
                foreach (ICredentialContent item in this.DistinctItems())
                {
                    if (String.Compare(n, item.NormalizedName, true) == 0)
                        return true;
                }

                return false;
            }

            bool IDictionary<string, ICredentialContent>.ContainsKey(string key) { return this.Contains(key); }

            void ICollection<KeyValuePair<string, ICredentialContent>>.CopyTo(KeyValuePair<string, ICredentialContent>[] array, int arrayIndex)
            {
                (new List<KeyValuePair<string, ICredentialContent>>(this.DistinctPairs())).CopyTo(array, arrayIndex);
            }

            void ICollection<string>.CopyTo(string[] array, int arrayIndex)
            {
                (new List<string>(this.DistinctKeys())).CopyTo(array, arrayIndex);
            }

            public IEnumerable<ICredentialContent> DistinctItems()
            {
                List<string> names = new List<string>();
                foreach (ICredentialContent item in this._innerList)
                {
                    string n = item.NormalizedName.ToLower();
                    if (names.Contains(n))
                        continue;
                    yield return item;
                    names.Add(n);
                }
            }

            public IEnumerable<string> DistinctKeys()
            {
                foreach (ICredentialContent item in this.DistinctItems())
                    yield return item.NormalizedName;
            }

            public IEnumerable<KeyValuePair<string, ICredentialContent>> DistinctPairs()
            {
                foreach (ICredentialContent item in this.DistinctItems())
                    yield return new KeyValuePair<string, ICredentialContent>(item.NormalizedName, item);
            }

            IEnumerator IEnumerable.GetEnumerator() { return (this.DistinctKeys() as IEnumerable).GetEnumerator(); }

            IEnumerator<KeyValuePair<string, ICredentialContent>> IEnumerable<KeyValuePair<string, ICredentialContent>>.GetEnumerator()
            {
                return this.DistinctPairs().GetEnumerator();
            }

            public IEnumerator<string> GetEnumerator() { return this.DistinctKeys().GetEnumerator(); }

            public int IndexOf(string name)
            {
                if (name == null)
                    return -1;

                int index = 0;
                string n = CredentialComponentCollection.ConvertToNormalizedName(name);
                foreach (ICredentialContent item in this.DistinctItems())
                {
                    if (String.Compare(n, item.NormalizedName, true) == 0)
                        return index;
                    index++;
                }

                return -1;
            }

            public int BaseIndexOf(string name)
            {
                if (name == null)
                    return -1;

                int index = 0;
                string n = CredentialComponentCollection.ConvertToNormalizedName(name);
                foreach (ICredentialContent item in this._innerList)
                {
                    if (String.Compare(n, item.NormalizedName, true) == 0)
                        return index;
                    index++;
                }

                return -1;
            }

            void IList<string>.Insert(int index, string item) { throw new NotSupportedException(); }

            bool ICollection<KeyValuePair<string, ICredentialContent>>.Remove(KeyValuePair<string, ICredentialContent> item) { throw new NotSupportedException(); }

            bool IDictionary<string, ICredentialContent>.Remove(string key) { throw new NotSupportedException(); }

            bool ICollection<string>.Remove(string item) { throw new NotSupportedException(); }

            void IList<string>.RemoveAt(int index) { throw new NotSupportedException(); }

            public bool TryGetValue(string key, out ICredentialContent value)
            {
                if (key == null)
                {
                    value = null;
                    return false;
                }

                string n = CredentialComponentCollection.ConvertToNormalizedName(key);
                foreach (ICredentialContent item in this._innerList)
                {
                    if (String.Compare(n, item.NormalizedName, true) == 0)
                    {
                        value = item;
                        return true;
                    }
                }

                value = null;
                return false;
            }
        }
    }
}