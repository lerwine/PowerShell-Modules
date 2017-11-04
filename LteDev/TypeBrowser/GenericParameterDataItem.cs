using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDev.TypeBrowser
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public abstract class GenericArgumentDataItem : TypeInfoDataItem
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        private MemberInfo _declaringMember;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public abstract class GenericParameterDataItem<TDeclaringMemberInfo> : GenericArgumentDataItem
        where TDeclaringMemberInfo : MemberInfo
    {
        public new TDeclaringMemberInfo DeclaringMember { get { return base.DeclaringMember as TDeclaringMemberInfo; } }

        protected GenericParameterDataItem(Type representedMember, TDeclaringMemberInfo declaringMember) : base(representedMember, declaringMember) { }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
