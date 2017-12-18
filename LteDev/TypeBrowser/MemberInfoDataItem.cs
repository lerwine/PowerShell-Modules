using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDev.TypeBrowser
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public abstract class MemberInfoDataItem
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
    {
        private MemberInfo _representedMember;

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
        public MemberInfo RepresentedMember { get { return this._representedMember; } }

        public abstract string FullName { get; }

        public virtual string BaseName { get { return this._representedMember.Name; } }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
        protected MemberInfoDataItem(MemberInfo representedMember)
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
        {
            if (representedMember == null)
                throw new ArgumentNullException("representedMember");
        }
    }

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public abstract class MemberInfoDataItem<TMemberInfo> : MemberInfoDataItem
        where TMemberInfo : MemberInfo
    {
        public new TMemberInfo RepresentedMember { get { return base.RepresentedMember as TMemberInfo; } }

        public abstract override string FullName { get; }

        protected MemberInfoDataItem(TMemberInfo representedMember) : base(representedMember) { }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}