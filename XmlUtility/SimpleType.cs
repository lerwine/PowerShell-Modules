using System;

namespace XmlUtility
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public abstract class SimpleType : ISimpleType
    {
        protected abstract object BaseValue { get; }

        object ISimpleType.BaseValue { get { return BaseValue; } }

        public abstract string ToXmlText();
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}