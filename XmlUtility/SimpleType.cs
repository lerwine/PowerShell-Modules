using System;

namespace XmlUtility
{
    public abstract class SimpleType : ISimpleType
    {
        protected abstract object BaseValue { get; }

        object ISimpleType.BaseValue { get { return BaseValue; } }

        public abstract string ToXmlText();
    }
}