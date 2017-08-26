using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDev.TypeBrowser
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class TypeDefinition : TypeInfoDataItem
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public TypeDefinition(Type representedMember)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            : base(representedMember)
        {
            if (representedMember.HasElementType || representedMember.IsGenericParameter || representedMember.IsGenericType && !representedMember.IsGenericTypeDefinition)
                throw new ArgumentException("Type must be a Type Definition, and not an Instantiated Type Definition", "representedMember");

            if (representedMember.HasElementType || representedMember.IsGenericParameter || representedMember.IsGenericType && !representedMember.IsGenericTypeDefinition)
                throw new ArgumentException("Type must be a Type Definition, and not Generic Parameter", "representedMember");
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
