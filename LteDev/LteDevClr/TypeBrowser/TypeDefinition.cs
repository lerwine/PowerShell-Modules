using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDevClr.TypeBrowser
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
    }
}
