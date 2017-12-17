using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace LteDev.TypeBrowser
{
    public class CustomObjectSchema : TypeKeyDictionary<TypeNameKey, CustomObjectSchema.ObjectPrimarySchemaInfo>
    {
        Collection<PSObject> _allObjects = new Collection<PSObject>();
        public CustomObjectSchema() : base(null) { }

        public void Import(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            PSObject psObj = (obj is PSObject) ? obj as PSObject : PSObject.AsPSObject(obj);
            if (_allObjects.Any(o => o.Equals(psObj)))
                return;
            _allObjects.Add(psObj);
            ObjectPrimarySchemaInfo.Import(this, this, psObj);
        }

        public class ObjectPrimarySchemaInfo : TypeKeyDictionary<TypeNameKey, ObjectMetaSchemaInfo>, ITypeKeyedItem<TypeNameKey>
        {
            public TypeNameKey Key { get; private set; }
            
            private ObjectPrimarySchemaInfo(TypeNameKey key)
                : base(null)
            {
                if (!key.IsPrimaryTypeKey())
                    throw new ArgumentOutOfRangeException("key");
                Key = key;
            }

            internal static void Import(CustomObjectSchema schema, IList<ObjectPrimarySchemaInfo> list, PSObject obj)
            {
                TypeNameKey key = new TypeNameKey(obj);
                if (!key.IsPrimaryTypeKey())
                    key = key.AsPrimaryTypeKey();
                ObjectPrimarySchemaInfo item;
                item = list.FirstOrDefault(s => s.Key.Equals(key));
                if (item == null)
                {
                    item = new ObjectPrimarySchemaInfo(key);
                    list.Add(item);
                }
                ObjectMetaSchemaInfo.Import(schema, item, obj);
            }
        }

        public class ObjectMetaSchemaInfo : TypeKeyDictionary<string, CustomObjectPropertyInfo>, ITypeKeyedItem<TypeNameKey>
        {
            public TypeNameKey Key { get; private set; }

            public int ReferenceCount { get; private set; }

            private ObjectMetaSchemaInfo(TypeNameKey key)
                : base(PSTypeNameComparer.PSNameComparer)
            {
                Key = key;
            }

            internal static void Import(CustomObjectSchema schema, IList<ObjectMetaSchemaInfo> list, PSObject obj)
            {
                if (list == null)
                    throw new ArgumentNullException("list");

                if (obj == null)
                    throw new ArgumentNullException("obj");

                TypeNameKey k = new TypeNameKey(obj);
                ObjectMetaSchemaInfo item = list.FirstOrDefault(s => s.Key.Equals(k));
                if (item == null)
                {
                    item = new ObjectMetaSchemaInfo(k);
                    list.Add(item);
                }
                item.ReferenceCount++;
                foreach (PSPropertyInfo property in obj.Properties)
                    CustomObjectPropertyInfo.Import(schema, item, property);
            }
        }

        public class CustomObjectPropertyInfo : TypeKeyDictionary<TypeNameKey, PropertyPrimarySchemaInfo>, ITypeKeyedItem<string>
        {
            public string PropertyName { get; private set; }

            string ITypeKeyedItem<string>.Key { get { return PropertyName; } }

            public int ReferenceCount { get; private set; }

            public int NullReferenceCount { get; private set; }

            public CustomObjectPropertyInfo(string propertyName)
                : base(null)
            {
                PropertyName = propertyName;
            }

            internal static void Import(CustomObjectSchema schema, IList<CustomObjectPropertyInfo> list, PSPropertyInfo property)
            {
                CustomObjectPropertyInfo item = list.FirstOrDefault(i => PSTypeNameComparer.PSNameComparer.Equals(i.PropertyName, property.Name));
                if (item == null)
                {
                    item = new CustomObjectPropertyInfo(property.Name);
                    list.Add(item);
                }
                item.ReferenceCount++;
                TypeNameKey key;
                bool isNull = property.Value == null;
                if (isNull)
                {
                    key = (String.IsNullOrEmpty(property.TypeNameOfValue)) ? new TypeNameKey(typeof(object)) : new TypeNameKey(property.TypeNameOfValue);
                    item.NullReferenceCount++;
                }
                else
                    key = new TypeNameKey((property.Value is PSObject) ? property.Value as PSObject : PSObject.AsPSObject(property.Value));
                PropertyPrimarySchemaInfo.Import(item, key, isNull);
                if (!isNull)
                    schema.Import(property.Value);
            }
        }

        public class PropertyPrimarySchemaInfo : TypeKeyDictionary<TypeNameKey, PropertyMetaSchemaInfo>, ITypeKeyedItem<TypeNameKey>
        {
            public TypeNameKey Key { get; private set; }

            public int ReferenceCount { get; private set; }

            public int NullReferenceCount { get; private set; }

            private PropertyPrimarySchemaInfo(TypeNameKey key)
                : base(null)
            {
                if (!key.IsPrimaryTypeKey())
                    throw new ArgumentOutOfRangeException("key");
                Key = key;
            }

            internal static void Import(IList<PropertyPrimarySchemaInfo> list, TypeNameKey key, bool isNull)
            {
                TypeNameKey k = (key.IsPrimaryTypeKey()) ? key : key.AsPrimaryTypeKey();
                PropertyPrimarySchemaInfo item = list.FirstOrDefault(i => i.Key.Equals(k));
                if (item == null)
                {
                    item = new PropertyPrimarySchemaInfo(k);
                    list.Add(item);
                }
                item.ReferenceCount++;
                if (isNull)
                    item.NullReferenceCount++;
                PropertyMetaSchemaInfo.Import(item, key, isNull);
            }
        }

        public class PropertyMetaSchemaInfo : ITypeKeyedItem<TypeNameKey>
        {
            public TypeNameKey Key { get; private set; }

            public int ReferenceCount { get; private set; }

            public int NullReferenceCount { get; private set; }

            private PropertyMetaSchemaInfo(TypeNameKey key)
            {
                Key = key;
            }

            internal static void Import(IList<PropertyMetaSchemaInfo> list, TypeNameKey key, bool isNull)
            {
                PropertyMetaSchemaInfo item = list.FirstOrDefault(i => i.Key.Equals(key));
                if (item == null)
                {
                    item = new PropertyMetaSchemaInfo(key);
                    list.Add(item);
                }
                item.ReferenceCount++;
                if (isNull)
                    item.NullReferenceCount++;
            }
        }
    }

    public interface ITypeKeyedItem<T>
    {
        T Key { get; }
    }

    public class TypeKeyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IList<TValue>
        where TValue : ITypeKeyedItem<TKey>
    {
        protected Collection<TValue> _items { get; private set; }

        protected ICollection<TKey> Keys { get { return _keys; } }

        ICollection<TKey> IDictionary<TKey, TValue>.GetKeys()
        { return _keys; }
        ICollection<TValue> IDictionary<TKey, TValue>.Values { get { return Items; } }

        public int Count { get { return _items.Count; } }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly { get { return true; } }

        public TValue this[TKey key]
        {
            get { return _items.FirstOrDefault(i => _keyComparer.Equals(i.Key, key)); }
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get { return this[key]; }
            set
            {
                throw new NotSupportedException();
            }
        }

        protected ReadOnlyCollection<TValue> Items { get; private set; }

        bool ICollection<TValue>.IsReadOnly { get { return true; } }

        TValue IList<TValue>.this[int index]
        {
            get { return _items[index]; }
            set
            {
                throw new NotSupportedException();
            }
        }

        private KeyCollection _keys;
        private IEqualityComparer<TKey> _keyComparer;

        public class KeyCollection : ICollection<TKey>
        {
            Collection<TValue> _items;
            IEqualityComparer<TKey> _keyComparer;
            public KeyCollection(Collection<TValue> items, IEqualityComparer<TKey> keyComparer)
            {
                _items = items;
                _keyComparer = keyComparer;
            }

            public int Count { get { return _items.Count; } }

            bool ICollection<TKey>.IsReadOnly { get { return true; } }

            void ICollection<TKey>.Add(TKey item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TKey>.Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(TKey item)
            {
                return _items.Any(i => i.Key.Equals(item));
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                _items.Select(i => i.Key).ToList().CopyTo(array, arrayIndex);
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return _items.Select(i => i.Key).GetEnumerator();
            }

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new NotSupportedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _items.Select(i => i.Key).ToArray().GetEnumerator();
            }
        }

        public TypeKeyDictionary(IEqualityComparer<TKey> keyComparer)
        {
            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            _items = new Collection<TValue>();
            Items = new ReadOnlyCollection<TValue>(Items);
            _keys = new KeyCollection(_items, _keyComparer);
        }

        public bool ContainsKey(TKey key)
        {
            return _items.Any(i => _keyComparer.Equals(i.Key, key));
        }

        protected void Add(TValue value)
        {
            if (_items.Any(i => _keyComparer.Equals(i.Key, value.Key)))
                throw new InvalidOperationException("Item with same key already exists.");
            _items.Add(value);
        }
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            if (_items.Any(i => _keyComparer.Equals(i.Key, key)))
            {
                value = _items.FirstOrDefault(i => _keyComparer.Equals(i.Key, key));
                return true;
            }
            value = default(TValue);
            return false;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return _keyComparer.Equals(item.Key, item.Value.Key) && _items.Any(i => _keyComparer.Equals(i.Key, item.Key));
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _items.Select(i => new KeyValuePair<TKey, TValue>(i.Key, i)).ToList().CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _items.Select(i => new KeyValuePair<TKey, TValue>(i.Key, i)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.Select(i => new KeyValuePair<TKey, TValue>(i.Key, i)).ToArray().GetEnumerator();
        }

        int IList<TValue>.IndexOf(TValue item)
        {
            return _items.IndexOf(item);
        }

        void IList<TValue>.Insert(int index, TValue item)
        {
            throw new NotSupportedException();
        }

        void IList<TValue>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void ICollection<TValue>.Add(TValue item)
        {
            Add(item);
        }

        void ICollection<TValue>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<TValue>.Contains(TValue item)
        {
            return _items.Contains(item);
        }

        void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        bool ICollection<TValue>.Remove(TValue item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }

    public class PSTypeNameComparer : IEqualityComparer<PSTypeName>, IComparer<PSTypeName>
    {
        private static PSTypeNameComparer _default;
        public static readonly StringComparer PSNameComparer = StringComparer.InvariantCultureIgnoreCase;
        public static readonly StringComparer ClrNameComparer = StringComparer.InvariantCulture;
        public static PSTypeNameComparer Default
        {
            get
            {
                if (_default == null)
                    _default = new PSTypeNameComparer();
                return _default;
            }
        }

        public bool Equals(PSTypeName x, PSTypeName y)
        {
            if (x == null)
                return y == null;

            if (y == null)
                return false;

            if (ReferenceEquals(x, y))
                return true;

            if (!PSNameComparer.Equals(x.Name, y.Name))
                return false;

            if (x.Type == null)
                return y.Type == null;

            return y.Type != null && ClrNameComparer.Equals(x.Type.AssemblyQualifiedName, y.Type.AssemblyQualifiedName);
        }

        public bool Equals(PSTypeName x, Type y)
        {
            if (x == null)
                return y == null;

            if (y == null)
                return false;

            if (x.Type == null)
                return false;

            return ClrNameComparer.Equals(x.Type.AssemblyQualifiedName, y.AssemblyQualifiedName);
        }

        public bool Equals(PSTypeName x, string y)
        {
            if (x == null)
                return String.IsNullOrEmpty(y);

            if (String.IsNullOrEmpty(y))
                return false;

            if (PSNameComparer.Equals(x.Name, y))
                return true;

            return x.Type != null && (ClrNameComparer.Equals(x.Type.FullName, y) || ClrNameComparer.Equals(x.Type.AssemblyQualifiedName, y));
        }

        public int GetHashCode(PSTypeName obj)
        {
            return PSNameComparer.GetHashCode((obj == null) ? "" : obj.Name);
        }

        public int Compare(PSTypeName x, PSTypeName y)
        {
            if (x == null)
                return (y == null) ? 0 : -1;

            if (y == null)
                return 1;

            if (ReferenceEquals(x, y))
                return 0;

            int delta = PSNameComparer.Compare(x.Name, y.Name);
            if (delta != 0 || (delta = ClrNameComparer.Compare(x.Name, y.Name)) != 0)
                return delta;

            if (x.Type == null)
                return (y.Type == null) ? 0 : -1;

            if (y.Type == null)
                return 1;

            if ((delta = ClrNameComparer.Compare(x.Type.FullName, y.Type.FullName)) == 0)
                return ClrNameComparer.Compare(x.Type.AssemblyQualifiedName, y.Type.AssemblyQualifiedName);
            return delta;
        }
    }

    public struct TypeNameKey : IEquatable<TypeNameKey>, IEquatable<PSTypeName>, IEquatable<Type>, IEquatable<string>
    {
        private PSTypeName _metaTypeName;
        private PSTypeName _primaryTypeName;
        private PSTypeName _clrTypeName;
        private ReadOnlyCollection<PSTypeName> _allTypeNames;
        private PSTypeName[] _originalTypeNames;

        public TypeNameKey AsPrimaryTypeKey()
        {
            if (_metaTypeName == null || ReferenceEquals(_primaryTypeName, _metaTypeName))
                return this;

            IEnumerable<PSTypeName> originalTypeNames = _originalTypeNames;
            return new TypeNameKey(_primaryTypeName, _allTypeNames.Skip(2).Where(t => originalTypeNames.Any(o => ReferenceEquals(o, t))).ToArray().ToArray());
        }

        public bool IsPrimaryTypeKey() { return _metaTypeName == null || ReferenceEquals(_metaTypeName, _primaryTypeName); }

        public PSTypeName MetaTypeName { get { return _metaTypeName; } }

        public PSTypeName PrimaryTypeName { get { return _primaryTypeName; } }

        public PSTypeName CLRTypeName { get { return _metaTypeName; } }

        public ReadOnlyCollection<PSTypeName> TypeNames { get { return _allTypeNames; } }

        public TypeNameKey(PSTypeName primaryType, params PSTypeName[] altTypeNames)
        {
            if (primaryType == null)
                throw new ArgumentNullException("primaryType");

            PSTypeName metaTypeName, primaryTypeName, clrTypeName;
            PSTypeName[] originalTypeNames;
            IEnumerable<PSTypeName> typeNames = new PSTypeName[] { primaryType };
            _allTypeNames = GetTypeNames((altTypeNames == null) ? typeNames : typeNames.Concat(altTypeNames), out metaTypeName, out primaryTypeName, out clrTypeName, out originalTypeNames);
            _metaTypeName = metaTypeName;
            _primaryTypeName = primaryTypeName;
            _clrTypeName = clrTypeName;
            _originalTypeNames = originalTypeNames;
        }

        public TypeNameKey(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            PSTypeName metaTypeName, primaryTypeName, clrTypeName;
            PSTypeName[] originalTypeNames;
            _allTypeNames = GetTypeNames(new PSTypeName[] { new PSTypeName(type) }, out metaTypeName, out primaryTypeName, out clrTypeName, out originalTypeNames);
            _metaTypeName = metaTypeName;
            _primaryTypeName = primaryTypeName;
            _clrTypeName = clrTypeName;
            _originalTypeNames = originalTypeNames;
        }

        public TypeNameKey(string primaryType, params string[] altTypeNames)
        {
            if (primaryType == null)
                throw new ArgumentNullException("primaryType");
            if (primaryType.Length == 0)
                throw new ArgumentException("Primary type name cannot be empty.", "primaryType");

            PSTypeName metaTypeName, primaryTypeName, clrTypeName;
            PSTypeName[] originalTypeNames;
            _allTypeNames = GetTypeNames(new PSTypeName[] { new PSTypeName(primaryType) }, out metaTypeName, out primaryTypeName, out clrTypeName, out originalTypeNames);
            _metaTypeName = metaTypeName;
            _primaryTypeName = primaryTypeName;
            _clrTypeName = clrTypeName;
            _originalTypeNames = originalTypeNames;
        }

        public TypeNameKey(PSObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            PSTypeName metaTypeName, primaryTypeName, clrTypeName;
            PSTypeName[] originalTypeNames = ((obj.TypeNames == null) ? new PSTypeName[0] : obj.TypeNames.Where(s => !String.IsNullOrEmpty(s)).Select(s => new PSTypeName(s)).Distinct(PSTypeNameComparer.Default)).ToArray();
            if (originalTypeNames.Length == 0)
                originalTypeNames = new PSTypeName[] { new PSTypeName(obj.BaseObject.GetType()) };
            _allTypeNames = GetTypeNames(originalTypeNames, out metaTypeName, out primaryTypeName, out clrTypeName, out originalTypeNames);
            _metaTypeName = metaTypeName;
            _primaryTypeName = primaryTypeName;
            _clrTypeName = clrTypeName;
            _originalTypeNames = originalTypeNames;
        }

        private static ReadOnlyCollection<PSTypeName> GetTypeNames(IEnumerable<PSTypeName> names, out PSTypeName metaTypeName, out PSTypeName primaryTypeName,
            out PSTypeName clrTypeName, out PSTypeName[] originalTypeNames)
        {
            List<PSTypeName> otn = names.Where(n => n != null).Distinct(PSTypeNameComparer.Default).ToList();
            IEnumerable<Type> allTypes = otn.Select(n => n.Type).Where(t => t != null);
            List<PSTypeName> implementedTypeNames = allTypes.SelectMany(t => GetTypes(t, true)).GroupBy(t => t.AssemblyQualifiedName)
                .Select(g => new { Aqn = g.Key, Type = g.First() }).ToArray()
                .Where(a => !allTypes.Any(t => PSTypeNameComparer.ClrNameComparer.Equals(t.AssemblyQualifiedName, a.Aqn)))
                .Select(a => new PSTypeName(a.Type)).ToList();

            metaTypeName = otn.First();
            clrTypeName = otn.FirstOrDefault(t => t.Type != null);
            if (clrTypeName == null)
            {
                clrTypeName = new PSTypeName(typeof(object));
                implementedTypeNames.Add(clrTypeName);
            }
            int index = metaTypeName.Name.IndexOf('#');
            if (index < 0)
                primaryTypeName = metaTypeName;
            else
            {
                string n = metaTypeName.Name.Substring(0, index);
                primaryTypeName = names.FirstOrDefault(t => PSTypeNameComparer.ClrNameComparer.Equals(t.Name, n));
                if (primaryTypeName == null)
                {
                    primaryTypeName = names.FirstOrDefault(t => PSTypeNameComparer.Default.Equals(t, n));
                    if (primaryTypeName == null)
                    {
                        primaryTypeName = new PSTypeName(n);
                        if (otn.Count == 1)
                            otn.Add(primaryTypeName);
                        else
                            otn.Insert(1, primaryTypeName);
                    }
                }
            }

            originalTypeNames = otn.OrderBy(t => t, PSTypeNameComparer.Default).ToArray();
            return new ReadOnlyCollection<PSTypeName>(otn.Concat(implementedTypeNames).ToArray());
        }

        public static IEnumerable<Type> GetTypes(Type type, bool excludeCurrent = false)
        {
            for (Type t = (excludeCurrent) ? type.BaseType : type; t != null; t = t.BaseType)
                yield return t;
            foreach (Type t in type.GetInterfaces())
                yield return t;
        }

        public bool Equals(TypeNameKey other)
        {
            if (ReferenceEquals(_allTypeNames, other._allTypeNames))
                return true;
            if (_metaTypeName == null)
                return other._metaTypeName == null;
            if (other._metaTypeName == null || _originalTypeNames.Length != other._originalTypeNames.Length)
                return false;
            PSTypeNameComparer comparer = PSTypeNameComparer.Default;
            if (!comparer.Equals(_metaTypeName, other._metaTypeName))
                return false;

            if (ReferenceEquals(_metaTypeName, _primaryTypeName))
            {
                if (!ReferenceEquals(other._metaTypeName, other._primaryTypeName))
                    return false;
            }
            else if (ReferenceEquals(other._metaTypeName, other._primaryTypeName) || !comparer.Equals(other._primaryTypeName, other._primaryTypeName))
                return false;

            if (ReferenceEquals(_primaryTypeName, _clrTypeName))
            {
                if (!ReferenceEquals(other._primaryTypeName, other._clrTypeName))
                    return false;
            }
            else if (ReferenceEquals(other._primaryTypeName, other._clrTypeName) || !comparer.Equals(other._clrTypeName, other._clrTypeName))
                return false;

            for (int i = 0; i < _originalTypeNames.Length; i++)
            {
                if (!comparer.Equals(_originalTypeNames[i], other._originalTypeNames[i]))
                    return false;
            }

            return true;
        }

        public bool Equals(PSTypeName other)
        {
            if (other == null)
                return _metaTypeName == null;
            PSTypeNameComparer comparer = PSTypeNameComparer.Default;
            return _allTypeNames.Any(t => comparer.Equals(t, other));
        }

        public bool Equals(Type other)
        {
            if (other == null)
                return _metaTypeName == null;
            PSTypeNameComparer comparer = PSTypeNameComparer.Default;
            return _allTypeNames.Any(t => comparer.Equals(t, other));
        }

        public bool Equals(string other)
        {
            if (String.IsNullOrEmpty(other))
                return _metaTypeName == null;
            PSTypeNameComparer comparer = PSTypeNameComparer.Default;
            return _allTypeNames.Any(t => comparer.Equals(t, other));
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return _metaTypeName == null;

            if (obj is TypeNameKey)
                return Equals((TypeNameKey)obj);

            if (obj is Type)
                return Equals(obj as Type);

            if (obj is PSTypeName)
                return Equals(obj as PSTypeName);

            return Equals(obj as string);
        }

        public override int GetHashCode()
        {
            return (_metaTypeName == null) ? PSTypeNameComparer.ClrNameComparer.GetHashCode("") : PSTypeNameComparer.Default.GetHashCode(_metaTypeName);
        }

        public override string ToString()
        {
            return (_metaTypeName == null) ? "" : String.Join(",", TypeNames.Select(n => "{" + n.Name.Replace("\\", "\\\\").Replace("{", "\\{").Replace("{", "\\}") + "}").ToArray());
        }
    }
}