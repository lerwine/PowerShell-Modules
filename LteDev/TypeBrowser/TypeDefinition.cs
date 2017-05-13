using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDev.TypeBrowser
{
    public class TypeDefinition : TypeInfoDataItem
    {
        public TypeDefinition(Type representedMember)
            : base(representedMember)
        {
            if (representedMember.HasElementType || representedMember.IsGenericParameter || representedMember.IsGenericType && !representedMember.IsGenericTypeDefinition)
                throw new ArgumentException("Type must be a Type Definition, and not an Instantiated Type Definition", "representedMember");

            if (representedMember.HasElementType || representedMember.IsGenericParameter || representedMember.IsGenericType && !representedMember.IsGenericTypeDefinition)
                throw new ArgumentException("Type must be a Type Definition, and not Generic Parameter", "representedMember");
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
