using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDevClr.TypeBrowser
{
    public abstract class MethodInfoDataItem : MemberInfoDataItem<MethodInfo>
    {
        protected MethodInfoDataItem(MethodInfo representedMember) : base(representedMember) { }

    }
}
