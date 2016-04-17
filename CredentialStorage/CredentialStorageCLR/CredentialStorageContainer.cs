using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CredentialStorageCLR
{
    public abstract class CredentialStorageContainer<T> : CredentialStorageComponent<T>, ICredentialContainer<T>
        where T : CredentialStorageContainer<T>
    {
        private CredentialComponentCollection _components;
        private IDictionary<string, ICredentialContent> _namedComponents;
        private IDictionary<Guid, ICredentialContent> _keyedComponents;

        protected CredentialStorageContainer() : base() { }

        protected CredentialStorageContainer(T container)
        {
            this._components = new CredentialComponentCollection(this);
            foreach (ICredentialContent item in container.Components)
                this.Add(item.Clone());
        }

        protected CredentialStorageContainer(IEnumerable<ICredentialContent> collection)
        {
            this._components = new CredentialComponentCollection(this, collection);
        }

        public ICredentialContent this[string key]
        {
            get { return this.NamedComponents[key]; }
            set { this.NamedComponents[key] = value; }
        }

        public ICredentialContent this[Guid key]
        {
            get { return this.KeyedComponents[key]; }
            set { this.KeyedComponents[key] = value; }
        }

        public ICredentialContent this[int index]
        {
            get { return this.Components[index]; }
            set { this.Components[index] = value; }
        }

        [XmlArrayItem(ElementName = CredentialItem.RootElementName, Namespace = CredentialItem.NamespaceURI, Type = typeof(CredentialItem), IsNullable = false)]
        [XmlArrayItem(ElementName = CredentialFolder.RootElementName, Namespace = CredentialFolder.NamespaceURI, Type = typeof(CredentialFolder), IsNullable = false)]
        public virtual CredentialComponentCollection Components { get { return this._components; } }

        ComponentCollection IContainer.Components { get { return this.Components; } }

        CredentialComponentCollection ICredentialContainer.Components { get { return this.Components; } }

        protected virtual IDictionary<string, ICredentialContent> NamedComponents
        {
            get
            {
                if (this._namedComponents == null)
                    this._namedComponents = this.Components as IDictionary<string, ICredentialContent>;

                return this._namedComponents;
            }
        }

        protected virtual IDictionary<Guid, ICredentialContent> KeyedComponents
        {
            get
            {
                if (this._keyedComponents == null)
                    this._keyedComponents = this.Components as IDictionary<Guid, ICredentialContent>;

                return this._keyedComponents;
            }
        }

        [XmlIgnore]
        public int Count { get { return this.Components.Count; } }

        protected virtual bool IsReadOnly { get { return false; } }

        bool ICollection<KeyValuePair<Guid, ICredentialContent>>.IsReadOnly { get { return this.IsReadOnly; } }

        bool ICollection<KeyValuePair<string, ICredentialContent>>.IsReadOnly { get { return this.IsReadOnly; } }

        bool ICollection<ICredentialContent>.IsReadOnly { get { return this.IsReadOnly; } }

        ICollection<string> IDictionary<string, ICredentialContent>.Keys { get { return this.NamedComponents.Keys; } }

        [XmlIgnore]
        public ICollection<Guid> Keys { get { return this.KeyedComponents.Keys; } }

        ICollection<ICredentialContent> IDictionary<string, ICredentialContent>.Values { get { return this.Components; } }

        ICollection<ICredentialContent> IDictionary<Guid, ICredentialContent>.Values { get { return this.Components; } }

        void ICollection<KeyValuePair<string, ICredentialContent>>.Add(KeyValuePair<string, ICredentialContent> item) { this.NamedComponents.Add(item); }

        void IContainer.Add(IComponent component) { this.Add((ICredentialContent)component); }

        void ICollection<KeyValuePair<Guid, ICredentialContent>>.Add(KeyValuePair<Guid, ICredentialContent> item) { this.KeyedComponents.Add(item); }

        public void Add(ICredentialContent item) { this.Components.Add(item); }

        void IContainer.Add(IComponent component, string name) { this.NamedComponents.Add(name, (ICredentialContent)component); }

        void IDictionary<string, ICredentialContent>.Add(string key, ICredentialContent value) { this.NamedComponents.Add(key, value); }

        void IDictionary<Guid, ICredentialContent>.Add(Guid key, ICredentialContent value) { this.KeyedComponents.Add(key, value); }

        void ICredentialContainer.Add(ICredentialContent component, Guid id) { this.KeyedComponents.Add(id, component); }
        
        public void Clear() { this.Components.Clear(); }

        protected abstract override T Clone();

        bool ICollection<KeyValuePair<string, ICredentialContent>>.Contains(KeyValuePair<string, ICredentialContent> item)
        {
            return this.NamedComponents.Contains(item);
        }

        bool ICollection<KeyValuePair<Guid, ICredentialContent>>.Contains(KeyValuePair<Guid, ICredentialContent> item)
        {
            return this.KeyedComponents.Contains(item);
        }

        public bool Contains(ICredentialContent item) { return this.Components.Contains(item); }

        public bool ContainsKey(string key) { return this.Components.ContainsKey(key); }

        public bool ContainsKey(Guid key) { return this.Components.ContainsKey(key); }

        void ICollection<KeyValuePair<Guid, ICredentialContent>>.CopyTo(KeyValuePair<Guid, ICredentialContent>[] array, int arrayIndex)
        {
            this.KeyedComponents.CopyTo(array, arrayIndex);
        }

        void ICollection<KeyValuePair<string, ICredentialContent>>.CopyTo(KeyValuePair<string, ICredentialContent>[] array, int arrayIndex)
        {
            this.NamedComponents.CopyTo(array, arrayIndex);
        }

        public void CopyTo(ICredentialContent[] array, int arrayIndex)
        {
            this.Components.CopyTo(array, arrayIndex);
        }

        public abstract bool Equals(ICredentialContainer<T> other);

        public override bool Equals(ICredentialComponent<T> other) { return this.Equals(other as ICredentialContainer<T>); }

        bool IEquatable<ICredentialContainer>.Equals(ICredentialContainer other) { return this.Equals(other as ICredentialContainer<T>); }

        IEnumerator IEnumerable.GetEnumerator() { return (this.Components as IEnumerable).GetEnumerator(); }

        IEnumerator<KeyValuePair<string, ICredentialContent>> IEnumerable<KeyValuePair<string, ICredentialContent>>.GetEnumerator()
        {
            return this.NamedComponents.GetEnumerator();
        }

        IEnumerator<KeyValuePair<Guid, ICredentialContent>> IEnumerable<KeyValuePair<Guid, ICredentialContent>>.GetEnumerator()
        {
            return this.KeyedComponents.GetEnumerator();
        }

        public IEnumerator<ICredentialContent> GetEnumerator() { return this.Components.GetEnumerator(); }

        IEnumerable<ICredentialContainer> ICredentialContainer.GetParents()
        {
            yield return this;
            if (this.Site != null)
            {
                foreach (ICredentialContainer c in this.Site.Container.GetParents())
                    yield return c;
            }   
        }

        public int IndexOf(ICredentialContent item) { return this.Components.IndexOf(item); }

        public void Insert(int index, ICredentialContent item) { this.Components.Insert(index, item); }

        public bool Remove(string key) { return this.NamedComponents.Remove(key); }

        void IContainer.Remove(IComponent component) { this.Components.Remove((ICredentialContent)component); }

        bool ICollection<KeyValuePair<string, ICredentialContent>>.Remove(KeyValuePair<string, ICredentialContent> item)
        {
            return this.NamedComponents.Remove(item);
        }

        bool ICollection<KeyValuePair<Guid, ICredentialContent>>.Remove(KeyValuePair<Guid, ICredentialContent> item)
        {
            return this.KeyedComponents.Remove(item);
        }

        bool IDictionary<Guid, ICredentialContent>.Remove(Guid key)
        {
            return this.KeyedComponents.Remove(key);
        }

        public bool Remove(ICredentialContent item) { return this.Components.Remove(item); }

        public void RemoveAt(int index) { this.Components.RemoveAt(index); }

        public bool TryGetValue(string key, out ICredentialContent value)
        {
            return this.NamedComponents.TryGetValue(key, out value);
        }

        public bool TryGetValue(Guid key, out ICredentialContent value)
        {
            return this.Components.TryGetValue(key, out value);
        }
    }
}