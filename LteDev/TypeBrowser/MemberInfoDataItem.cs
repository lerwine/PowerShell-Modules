using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDev.TypeBrowser
{
    public abstract class MemberInfoDataItem
    {
        private MemberInfo _representedMember;

        public MemberInfo RepresentedMember { get { return this._representedMember; } }

        public abstract string FullName { get; }

        public virtual string BaseName { get { return this._representedMember.Name; } }

        protected MemberInfoDataItem(MemberInfo representedMember)
        {
            if (representedMember == null)
                throw new ArgumentNullException("representedMember");
        }
    }

    public abstract class MemberInfoDataItem<TMemberInfo> : MemberInfoDataItem
        where TMemberInfo : MemberInfo
    {
        public new TMemberInfo RepresentedMember { get { return base.RepresentedMember as TMemberInfo; } }

        public abstract override string FullName { get; }

        protected MemberInfoDataItem(TMemberInfo representedMember) : base(representedMember) { }
    }
}