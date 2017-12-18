using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDev.AssemblyBrowser
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public abstract class TypeInfoDataItem
    {

        private Type _type;

        public Type Type { get { return this._type; } }

        public static TypeInfoDataItem Get(Type type)
        {
            if (type == null)
                return null;

            if (type.IsNested)
                return TypeInfoDataItem.Get(type.DeclaringType).FindNested(type);

            if (type.HasElementType)
                return InstantiatedTypeDefinition.Create(type);

            throw new NotImplementedException();
        }

        public abstract GenericTypeParameterDataItem FindGenericParameter(Type type);

        protected TypeInfoDataItem(Type type) { this._type = type; }

        public abstract TypeInfoDataItem FindNested(Type type);
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}