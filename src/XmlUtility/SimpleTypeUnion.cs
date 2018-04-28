using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlUtility
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public abstract class SimpleTypeUnion<TypeA, TypeB>  : SimpleType
    {
        private TypeA _unionA;
        private TypeB _unionB;

        protected SimpleTypeUnion(TypeA value) { }

        protected SimpleTypeUnion(TypeB value) { }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
