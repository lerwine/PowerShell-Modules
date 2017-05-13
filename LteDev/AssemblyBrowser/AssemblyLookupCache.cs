using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDev.AssemblyBrowser
{
    public class AssemblyLookupCache
    {
        private static Dictionary<string, AssemblyLookupCache> _assemblies = new Dictionary<string, AssemblyLookupCache>();
        private static bool _appDomainEnumerated = false;
        public Dictionary<string, TypeDefinitionItem> _types = new Dictionary<string, TypeDefinitionItem>();
        private bool _assemblyEnumerated = false;
        private Assembly _assembly;

        public AssemblyLookupCache(Assembly assembly)
        {
            this._assembly = assembly;
        }

        public static AssemblyLookupCache Get(Assembly assembly)
        {
            lock (AssemblyLookupCache._assemblies)
            {
                if (AssemblyLookupCache._assemblies.ContainsKey(assembly.FullName))
                    return AssemblyLookupCache._assemblies[assembly.FullName];

                AssemblyLookupCache assemblyLookupCache = new AssemblyLookupCache(assembly);
                AssemblyLookupCache._assemblies.Add(assembly.FullName, assemblyLookupCache);
                AssemblyLookupCache._appDomainEnumerated = false;
                return assemblyLookupCache;
            }
        }

        public static TypeDefinitionItem Lookup(Type type)
        {
            if (type == null)
                return null;

            throw new NotImplementedException();
        }

        public TypeDefinitionItem Get(Type type)
        {
            if (type.IsNested)
            {
                if (type.Assembly.FullName == this._assembly.FullName)
                    return this.Get(type.DeclaringType).Get(type);

                return AssemblyLookupCache.Lookup(type.DeclaringType).Get(type);
            }

            lock (this._types)
            {
                if (this._types.ContainsKey(type.AssemblyQualifiedName))
                    return this._types[type.AssemblyQualifiedName];

                TypeDefinitionItem typeLookupCache = new TypeDefinitionItem(type);
                this._types[type.AssemblyQualifiedName] = typeLookupCache;

                return typeLookupCache;
            }
        }
    }
}
