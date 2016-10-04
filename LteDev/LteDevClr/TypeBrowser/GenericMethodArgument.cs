using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDevClr.TypeBrowser
{
    public class GenericMethodArgument : GenericParameterDataItem<MethodBase>
    {
        public GenericMethodArgument(Type representedMember, MethodBase declaringMember)
            : base(representedMember, declaringMember)
        {
            if (representedMember.DeclaringMethod == null)
                throw new ArgumentException("Type must be a generic argument for a method, and not for a type.", "declaringMember");

            if (!representedMember.DeclaringMethod.Equals(declaringMember))
                throw new ArgumentException("Declaring method and type definition do not match.", "declaringMember");
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
