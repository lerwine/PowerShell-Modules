using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDev.TypeBrowser
{
    public abstract class GenericArgumentDataItem : TypeInfoDataItem
    {
        private MemberInfo _declaringMember;

        public MemberInfo DeclaringMember { get { return this._declaringMember; } }
        
        protected GenericArgumentDataItem(Type type, MemberInfo declaringMember) : base(type)
        {
            if (!type.IsGenericParameter)
                throw new ArgumentException("Type must be a Generic Parameter.", "representedMember");

            if (declaringMember == null)
                throw new ArgumentNullException("declaringMember");

            this._declaringMember = declaringMember;
        }
        
        public static GenericArgumentDataItem Create(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!type.IsGenericParameter)
                throw new ArgumentException("Type must be a Generic Parameter.", "representedMember");

            if (type.DeclaringMethod != null)
                return new GenericMethodArgument(type, type.DeclaringMethod);

            return new GenericTypeArgument(type, type.DeclaringType);
        }
    }

    public abstract class GenericParameterDataItem<TDeclaringMemberInfo> : GenericArgumentDataItem
        where TDeclaringMemberInfo : MemberInfo
    {
        public new TDeclaringMemberInfo DeclaringMember { get { return base.DeclaringMember as TDeclaringMemberInfo; } }

        protected GenericParameterDataItem(Type representedMember, TDeclaringMemberInfo declaringMember) : base(representedMember, declaringMember) { }
    }
}
