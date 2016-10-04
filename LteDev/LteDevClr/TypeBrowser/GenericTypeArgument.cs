using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDevClr.TypeBrowser
{
    public class GenericTypeArgument : GenericParameterDataItem<Type>
    {
        public GenericTypeArgument(Type representedMember, Type declaringMember)
            : base(representedMember, declaringMember)
        {
            if (representedMember.DeclaringMethod != null)
                throw new ArgumentException("Type must be a generic argument for a type, and not for a method.", "declaringMember");
            
            if (!representedMember.DeclaringType.Equals(declaringMember))
                throw new ArgumentException("Declaring type and type definition do not match.", "declaringMember");
        }

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
    }
}
