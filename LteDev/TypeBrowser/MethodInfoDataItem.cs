using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDev.TypeBrowser
{
    public abstract class MethodInfoDataItem : MemberInfoDataItem<MethodInfo>
    {
        protected MethodInfoDataItem(MethodInfo representedMember) : base(representedMember) { }

    }
}
