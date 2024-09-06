using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading;

namespace LteDev
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class TypeDetail
    {
        public string Name { get; private set; }

        public string FullName { get; private set; }

        public TypeInfo Type { get; private set; }

        public TypeDetail Owner { get; private set; }

        public string? QualifiedName => (Type is null) ? FullName : Type.AssemblyQualifiedName;

        private readonly Collection<TypeDetail> _types = [];

        private readonly Collection<AssemblyName> _assemblies = [];

        private TypeDetail(string name, string fullName, TypeDetail owner)
        {
            this.Owner = owner;
            this.Name = name;
            this.FullName = fullName;
        }

        private TypeDetail(Type type, TypeDetail? owner)
        {
            ArgumentNullException.ThrowIfNull(type);
            Type = (type is TypeInfo) ? (TypeInfo)(type) : type.GetTypeInfo();
            Name = GetName(type);
            FullName = GetFullName(type);
            Owner = owner;
            _assemblies.Add(type.Assembly.GetName());
            if (type.IsGenericParameter)
                return;
            
            foreach (Type t in type.GetNestedTypes(BindingFlags.NonPublic))
                _types.Add(new TypeDetail(t, this));
        }

        public TypeDetail(Type type) : this(type, null) { }

        public TypeDetail(Assembly assembly, bool byFullNamespace, TypeDetail? owner)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            Type = null;
            AssemblyName assemblyName = assembly.GetName();
            _assemblies.Add(assemblyName);
            Name = assemblyName.Name;
            FullName = assembly.FullName;
            Owner = owner;
        }

        public TypeDetail(Assembly assembly, bool byFullNamespace = false) : this(assembly, byFullNamespace, null) { }

        public TypeDetail(IEnumerable<Assembly> assemblies, bool byAssembly = false, bool byFullNamespace = false)
        {
            Type = null;
            Name = "";
            FullName = "";
            if (assemblies is null)
                return;
            IEnumerable<TypeDetail> aTypes = (assemblies.Select(a => new TypeDetail(a, byFullNamespace)));
            if (byAssembly)
            {
                foreach (TypeDetail d in aTypes)
                {
                    d.Owner = this;
                    _types.Add(d);
                }
            }
            else
            {
                foreach (TypeDetail aT in aTypes)
                {
                    foreach (IGrouping<string, TypeDetail> g in aT._types.GroupBy(t => (t.Type is null) ? t.FullName : ""))
                    {
                        TypeDetail td = _types.FirstOrDefault(d => d.FullName == g.Key);
                        if (td is null)
                            td = new TypeDetail(g.Key, g.Key, this);
                        _types.Add(td);
                        td._assemblies.Add(aT._assemblies[0]);
                        foreach (TypeDetail d in g)
                        {
                            d.Owner = td;
                            td._types.Add(d);
                        }
                    }
                }
            }
        }

        public TypeDetail(bool byAssembly, bool byFullNamespace, params Assembly[] assembly) : this(assembly as IEnumerable<Assembly>, byAssembly, byFullNamespace) { }

        public TypeDetail(bool byAssembly, params Assembly[] assembly) : this(assembly as IEnumerable<Assembly>, byAssembly) { }

        public TypeDetail(params Assembly[] assembly) : this(assembly as IEnumerable<Assembly>) { }

        public static string GetFullName(Type type)
        {
            if (type.IsGenericParameter)
                return type.Name;
            string name = GetName(type);
            if (type.IsNested)
                return GetFullName(type.DeclaringType!) + "+" + name;
            if (string.IsNullOrEmpty(type.Namespace))
                return name;
            return type.Namespace + "." + name;
        }

        public static string GetName(Type type)
        {
            string name = type.Name;
            if (type.IsGenericParameter || !type.IsGenericType)
                return name;
            int i = name.LastIndexOf('~');
            if (i > 0)
                name = name[..i];
            return name + "[" + string.Join(",", type.GetGenericArguments().Select(a => GetName(a))) + "]";
        }

        public static IEnumerable<TypeInfo> AsTypeInfo(IEnumerable<Type> types) { return (types is null) ? [] : types.Select(t => AsTypeInfo(t)); }

        public static TypeInfo? AsTypeInfo(Type type) { return (type is null || type is TypeInfo) ? (TypeInfo?)type : type.GetTypeInfo(); }
    }

    public interface ITypeInfoNodeContainer
    {
        string Name { get; }
        ITypeInfoNodeContainer Parent { get; }
    }
    public interface ITypeInfoTreeNode : ITypeInfoNodeContainer, IList<ITypeInfoTreeNode>, IList
    {
        TypeInfoNsTreeNode GetRoot();
    }

    public class ProxyEnumerator<TItem, TBase> : IEnumerator<TBase>
        where TItem : TBase
    {
        private readonly IEnumerator<TItem> _enumerator;
        public TBase Current => _enumerator.Current;
        object IEnumerator.Current => _enumerator.Current;
        public void Dispose() { _enumerator.Dispose(); }
        public bool MoveNext() { return _enumerator.MoveNext(); }
        public void Reset() { _enumerator.Reset(); }
        public ProxyEnumerator(IEnumerator<TItem> enumerator) { _enumerator = enumerator; }
    }

    public class TypeInfoTreeNode : IList<TypeInfoTreeNode>, ITypeInfoTreeNode
    {
        private readonly TypeInfo _type;
        private readonly List<TypeInfoTreeNode> _innerList = new();
        internal TypeInfoTreeNode(Type type, ITypeInfoTreeNode parent)
        {
            Parent = parent;
            _type = (type is TypeInfo) ? (TypeInfo)type : type.GetTypeInfo();
            foreach (Type t in type.GetNestedTypes())
                _innerList.Add(new TypeInfoTreeNode(t, this));
        }
        public TypeInfoTreeNode this[int index] { get { return _innerList[index]; } }
        TypeInfoTreeNode IList<TypeInfoTreeNode>.this[int index] { get { return _innerList[index]; } set { throw new NotSupportedException(); } }
        ITypeInfoTreeNode IList<ITypeInfoTreeNode>.this[int index] { get { return _innerList[index]; } set { throw new NotSupportedException(); } }
        object IList.this[int index] { get { return _innerList[index]; } set { throw new NotSupportedException(); } }
        public int Count => _innerList.Count;
        bool ICollection<TypeInfoTreeNode>.IsReadOnly => true;
        bool ICollection<ITypeInfoTreeNode>.IsReadOnly => true;
        bool IList.IsReadOnly => false;
        public string Name { get; private set; }
        public ITypeInfoTreeNode Parent { get; private set; }
        ITypeInfoNodeContainer ITypeInfoNodeContainer.Parent => Parent;
        bool IList.IsFixedSize => false;
        object ICollection.SyncRoot => ((IList)_innerList).SyncRoot;
        bool ICollection.IsSynchronized => ((IList)_innerList).IsSynchronized;
        void ICollection<TypeInfoTreeNode>.Add(TypeInfoTreeNode item) { throw new NotSupportedException(); }
        void ICollection<ITypeInfoTreeNode>.Add(ITypeInfoTreeNode item) { throw new NotSupportedException(); }
        int IList.Add(object value) { throw new NotSupportedException(); }
        void ICollection<TypeInfoTreeNode>.Clear() { throw new NotSupportedException(); }
        void ICollection<ITypeInfoTreeNode>.Clear() { throw new NotSupportedException(); }
        void IList.Clear() { throw new NotSupportedException(); }
        bool ICollection<TypeInfoTreeNode>.Contains(TypeInfoTreeNode item) { return _innerList.Contains(item); }
        bool ICollection<ITypeInfoTreeNode>.Contains(ITypeInfoTreeNode item) { return item is not null && item is TypeInfoTreeNode && _innerList.Contains(item); }
        bool IList.Contains(object value) { return value is not null && value is TypeInfoTreeNode && _innerList.Contains(value); }
        public void CopyTo(TypeInfoTreeNode[] array, int arrayIndex) { _innerList.CopyTo(array, arrayIndex); }
        void ICollection<ITypeInfoTreeNode>.CopyTo(ITypeInfoTreeNode[] array, int arrayIndex) { ((IList)_innerList).CopyTo(array, arrayIndex); }
        void ICollection.CopyTo(Array array, int index) { ((IList)_innerList).CopyTo(array, index); }
        public IEnumerator<TypeInfoTreeNode> GetEnumerator() { return _innerList.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IList)_innerList).GetEnumerator(); }
        IEnumerator<ITypeInfoTreeNode> IEnumerable<ITypeInfoTreeNode>.GetEnumerator() { return new ProxyEnumerator<TypeInfoTreeNode, ITypeInfoTreeNode>(_innerList.GetEnumerator()); }
        public TypeInfoNsTreeNode GetRoot()
        {
            TypeInfoTreeNode current = this;
            while (current.Parent is not null && current.Parent is TypeInfoTreeNode)
                current = (TypeInfoTreeNode)(current.Parent);
            return current.Parent.GetRoot();
        }
        public int IndexOf(TypeInfoTreeNode item) { return _innerList.IndexOf(item); }
        int IList<ITypeInfoTreeNode>.IndexOf(ITypeInfoTreeNode item) { return (item is not null && item is TypeInfoTreeNode) ? IndexOf((TypeInfoTreeNode)item) : -1; }
        int IList.IndexOf(object value) { return (value is not null && value is TypeInfoTreeNode) ? IndexOf((TypeInfoTreeNode)value) : -1; }
        void IList<TypeInfoTreeNode>.Insert(int index, TypeInfoTreeNode item) { throw new NotSupportedException(); }
        void IList<ITypeInfoTreeNode>.Insert(int index, ITypeInfoTreeNode item) { throw new NotSupportedException(); }
        void IList.Insert(int index, object value) { throw new NotSupportedException(); }
        bool ICollection<TypeInfoTreeNode>.Remove(TypeInfoTreeNode item) { throw new NotSupportedException(); }
        bool ICollection<ITypeInfoTreeNode>.Remove(ITypeInfoTreeNode item) { throw new NotSupportedException(); }
        void IList.Remove(object value) { throw new NotSupportedException(); }
        void IList<TypeInfoTreeNode>.RemoveAt(int index) { throw new NotSupportedException(); }
        void IList<ITypeInfoTreeNode>.RemoveAt(int index) { throw new NotSupportedException(); }
        void IList.RemoveAt(int index) { throw new NotSupportedException(); }
    }
    public class TypeInfoNsTreeNode : ITypeInfoTreeNode
    {
        private readonly List<ITypeInfoTreeNode> _innerList = new();
        public TypeInfoNsTreeNode(AssemblyTreeNode parent)
        {
            Name = "";
            Parent = parent;
            foreach (Type type in parent.GetAssembly().GetExportedTypes())
            {
                TypeInfoNsTreeNode node = GetNs(type.Namespace);
                node._innerList.Add(new TypeInfoTreeNode(type, node));
            }
        }
        public TypeInfoNsTreeNode(IEnumerable<Assembly> assemblies)
        {
            Name = "";
            Parent = null;
            foreach (Type type in assemblies.SelectMany(a => a.GetExportedTypes()))
            {
                TypeInfoNsTreeNode node = GetNs(type.Namespace);
                node._innerList.Add(new TypeInfoTreeNode(type, node));
            }
        }
        private TypeInfoNsTreeNode(string name, TypeInfoNsTreeNode parent)
        {
            Name = name;
            Parent = parent;
            parent._innerList.Add(this);
        }
        public TypeInfoNsTreeNode GetNs(string ns)
        {
            TypeInfoNsTreeNode currentNode = GetRoot();
            if (string.IsNullOrEmpty(ns))
                return currentNode;
            string[] names = ns.Split('.');
            for (int i = 0; i < names.Length; i++)
            {
                TypeInfoNsTreeNode node = currentNode._innerList.OfType<TypeInfoNsTreeNode>().FirstOrDefault(n => n.Name == names[i]);
                if (node is not null)
                    currentNode = node;
                else
                {
                    for (int n = 0; n < names.Length; n++)
                        currentNode = new TypeInfoNsTreeNode(names[n], currentNode);
                }
            }
            return currentNode;
        }
        public ITypeInfoTreeNode this[int index] { get { return _innerList[index]; } }
        ITypeInfoTreeNode IList<ITypeInfoTreeNode>.this[int index] { get { return _innerList[index]; } set { throw new NotSupportedException(); } }
        object IList.this[int index] { get { return _innerList[index]; } set { throw new NotSupportedException(); } }
        public string Name { get; private set; }
        public ITypeInfoNodeContainer Parent { get; private set; }
        public int Count => _innerList.Count;
        bool ICollection<ITypeInfoTreeNode>.IsReadOnly => true;
        bool IList.IsReadOnly => true;
        bool IList.IsFixedSize => false;
        object ICollection.SyncRoot => ((IList)_innerList).SyncRoot;
        bool ICollection.IsSynchronized => ((IList)_innerList).IsSynchronized;
        void ICollection<ITypeInfoTreeNode>.Add(ITypeInfoTreeNode item) { throw new NotSupportedException(); }
        int IList.Add(object value) { throw new NotSupportedException(); }
        void ICollection<ITypeInfoTreeNode>.Clear() { throw new NotSupportedException(); }
        void IList.Clear() { throw new NotSupportedException(); }
        bool ICollection<ITypeInfoTreeNode>.Contains(ITypeInfoTreeNode item) { return _innerList.Contains(item); }
        bool IList.Contains(object value) { return value is not null && value is ITypeInfoTreeNode && _innerList.Contains((ITypeInfoTreeNode)value); }
        void ICollection<ITypeInfoTreeNode>.CopyTo(ITypeInfoTreeNode[] array, int arrayIndex) { _innerList.CopyTo(array, arrayIndex); }
        void ICollection.CopyTo(Array array, int index) { ((IList)_innerList).CopyTo(array, index); }
        IEnumerator<ITypeInfoTreeNode> IEnumerable<ITypeInfoTreeNode>.GetEnumerator() { return _innerList.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IList)_innerList).GetEnumerator(); }
        public TypeInfoNsTreeNode GetRoot()
        {
            TypeInfoNsTreeNode current = this;
            while (current.Parent is not null && current.Parent is TypeInfoNsTreeNode)
                current = (TypeInfoNsTreeNode)(current.Parent);
            return current;
        }
        int IList<ITypeInfoTreeNode>.IndexOf(ITypeInfoTreeNode item) { return _innerList.IndexOf(item); }
        int IList.IndexOf(object value) { return (value is not null && value is ITypeInfoTreeNode) ? _innerList.IndexOf((ITypeInfoTreeNode)value) : -1; }
        void IList<ITypeInfoTreeNode>.Insert(int index, ITypeInfoTreeNode item) { throw new NotSupportedException(); }
        void IList.Insert(int index, object value) { throw new NotSupportedException(); }
        bool ICollection<ITypeInfoTreeNode>.Remove(ITypeInfoTreeNode item) { throw new NotSupportedException(); }
        void IList.Remove(object value) { throw new NotSupportedException(); }
        void IList<ITypeInfoTreeNode>.RemoveAt(int index) { throw new NotSupportedException(); }
        void IList.RemoveAt(int index) { throw new NotSupportedException(); }
    }

    public class AssemblyTreeNode : ITypeInfoNodeContainer
    {
        private readonly Assembly _assembly;
        public string Name => _assembly.FullName;
        public TypeInfoNsTreeNode NsRoot { get; private set; }
        public ITypeInfoNodeContainer Parent => throw new NotImplementedException();

        public Assembly GetAssembly() { return _assembly; }
        public AssemblyTreeNode(Assembly assembly)
        {
            _assembly = assembly;
            NsRoot = new TypeInfoNsTreeNode(this);
        }
    }

    public class TypeInfoCollections
    {
        private readonly Collection<AssemblyTreeNode> _byAssembly = new();
        public TypeInfoNsTreeNode ByNamespace { get; private set; }
        public ReadOnlyCollection<AssemblyTreeNode> ByAssembly { get; private set; }
        public TypeInfoCollections(IEnumerable<Assembly> assemblies)
        {
            foreach (Assembly a in assemblies)
                _byAssembly.Add(new AssemblyTreeNode(a));
            ByNamespace = new TypeInfoNsTreeNode(assemblies);
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
