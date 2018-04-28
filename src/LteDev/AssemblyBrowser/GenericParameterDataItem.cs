using System;

namespace LteDev.AssemblyBrowser
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public abstract class GenericParameterDataItem : TypeInfoDataItem
    {
        public override GenericTypeParameterDataItem FindGenericParameter(Type type) { return null; }

        public override TypeInfoDataItem FindNested(Type type) { return null; }

        protected GenericParameterDataItem(Type type) : base(type)
        {
            if (!type.IsGenericParameter)
                throw new ArgumentException("Type must be a Generic Parameter.", "type");
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}