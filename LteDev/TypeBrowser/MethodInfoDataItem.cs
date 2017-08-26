using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDev.TypeBrowser
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public abstract class MethodInfoDataItem : MemberInfoDataItem<MethodInfo>
    {
        protected MethodInfoDataItem(MethodInfo representedMember) : base(representedMember) { }

    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
