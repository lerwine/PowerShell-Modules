using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDevClr.AssemblyBrowser
{
    public class TypeDefinitionItem
    {
        private Type _type;
        private string _baseName = null;
        private string _fullName = null;
        private string _name = null;
        public Dictionary<string, TypeDefinitionItem> _nestedTypes = new Dictionary<string, TypeDefinitionItem>();
        public Dictionary<string, TypeDefinitionItem> _genericTypes = null;
        public Dictionary<string, MethodDefinitionItem> _methods = null;

        public Type Type { get { return _type; } }

        public string BaseName { get { return _baseName; } }

        public string FullName
        {
            get
            {
                if (_fullName != null)
                    return _fullName;

                if (_type.IsGenericParameter)
                    _fullName = Name;
                else
                {
                    if (_type.IsNested)
                    {
                        TypeDefinitionItem declaringType = AssemblyLookupCache.Lookup(_type.DeclaringType);
                        _fullName = String.Format("{0}+{1}", declaringType.FullName, Name);
                    }
                }
                throw new NotImplementedException();
            }
        }

        public string Name { get { return _name; } }

        public TypeDefinitionItem(Type type)
        {
            _type = type;
        }

        public TypeDefinitionItem Get(Type type)
        {
            if (!(type.IsNested && type.DeclaringType.Equals(Type)))
                return AssemblyLookupCache.Lookup(type);

            lock (_nestedTypes)
                if (_nestedTypes.ContainsKey(type.AssemblyQualifiedName))
                    return _nestedTypes[type.AssemblyQualifiedName];

            TypeDefinitionItem typeLookupCache = new TypeDefinitionItem(type);
            _nestedTypes[type.AssemblyQualifiedName] = typeLookupCache;

            return typeLookupCache;
        }
    }
}