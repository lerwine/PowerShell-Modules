using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDev.TypeBrowser
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class GenericTypeArgument : GenericParameterDataItem<Type>
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
    {
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
        public GenericTypeArgument(Type representedMember, Type declaringMember)
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
            : base(representedMember, declaringMember)
        {
            if (representedMember.DeclaringMethod != null)
                throw new ArgumentException("Type must be a generic argument for a type, and not for a method.", "declaringMember");
            
            if (!representedMember.DeclaringType.Equals(declaringMember))
                throw new ArgumentException("Declaring type and type definition do not match.", "declaringMember");
        }

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
        public override string FullName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override bool CanHaveGenericParameters
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override bool CanHaveNestedTypes
        {
            get
            {
                throw new NotImplementedException();
            }
        }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
    }
}
