using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace LteDev.TypeBrowser
{
    public abstract class TypeInfoDataItem : MemberInfoDataItem<Type>
    {
        public static readonly Regex GenericParamCountRegex = new Regex(@"`\d+$", RegexOptions.Compiled);

        private bool _allNestedEnumerated = false;
        private Dictionary<string, TypeInfoDataItem> _allNestedTypes = new Dictionary<string, TypeInfoDataItem>();
        private Dictionary<string, TypeInfoDataItem> _visibleNestedTypes = new Dictionary<string, TypeInfoDataItem>();

        protected TypeInfoDataItem(Type representedMember) : base(representedMember) { }

        protected abstract bool CanHaveNestedTypes { get; }
        
        protected abstract bool CanHaveGenericParameters { get; }

        public Dictionary<string, TypeInfoDataItem> NestedTypes
        {
            get
            {
                if (!this.CanHaveNestedTypes)
                    return this._visibleNestedTypes;

                lock (this._allNestedTypes)
                {
                    if (this._allNestedEnumerated)
                        return this._visibleNestedTypes;
                    this._allNestedEnumerated = true;
                }

                foreach (Type type in this.RepresentedMember.GetNestedTypes())
                {
                    lock (this._allNestedTypes)
                    {
                        if (this._visibleNestedTypes.ContainsKey(type.AssemblyQualifiedName))
                            continue;

                        if (this._allNestedTypes.ContainsKey(type.AssemblyQualifiedName))
                        {
                            this._visibleNestedTypes.Add(type.AssemblyQualifiedName, this._allNestedTypes[type.AssemblyQualifiedName]);
                            continue;
                        }
                    }

                    TypeInfoDataItem dataItem = TypeInfoDataItem.CreateNew(type);
                    this._allNestedTypes.Add(type.AssemblyQualifiedName, dataItem);
                    this._visibleNestedTypes.Add(type.AssemblyQualifiedName, dataItem);
                }

                return this._visibleNestedTypes;
            }
        }
        
        public static TypeInfoDataItem CreateNew(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type.IsGenericParameter)
                return GenericArgumentDataItem.Create(type);

            if (type.HasElementType || (type.IsGenericType && !type.IsGenericTypeDefinition))
                return new ConstructedTypeDefinition(type);

            return new TypeDefinition(type);
        }

        public static TypeInfoDataItem Get(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            
            if (type.IsGenericParameter)
                return GenericArgumentDataItem.Create(type);

            if (type.HasElementType || (type.IsGenericType && !type.IsGenericTypeDefinition))
                return new ConstructedTypeDefinition(type);

            if (type.IsNested)
                return TypeInfoDataItem.Get(type.DeclaringType).FindNested(type);

            return AssemblyDataItem.Lookup(type.Assembly).Find(type);
        }

        public TypeInfoDataItem FindNested(Type type)
        {
            if (!this.CanHaveNestedTypes || type == null || !type.IsNested || !type.ReflectedType.Equals(this.RepresentedMember))
                return null;

            lock (this._allNestedTypes)
            {
                if (this._allNestedTypes.ContainsKey(type.AssemblyQualifiedName))
                    return this._allNestedTypes[type.AssemblyQualifiedName];
            }

            TypeInfoDataItem dataItem = TypeInfoDataItem.CreateNew(type);
            this._allNestedTypes.Add(type.AssemblyQualifiedName, dataItem);
            return dataItem;
        }
    }
}
