using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDev.AssemblyBrowser
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class InstantiatedTypeDefinition : TypeInfoDataItem
    {
        public InstantiatedTypeDefinition(Type type) : base(type) { }

        internal static TypeInfoDataItem Create(Type type)
        {
            throw new NotImplementedException();
        }

        public override GenericTypeParameterDataItem FindGenericParameter(Type type)
        {
            throw new NotImplementedException();
        }

        public override TypeInfoDataItem FindNested(Type type)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
