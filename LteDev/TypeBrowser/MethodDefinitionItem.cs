using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDev.TypeBrowser
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class MethodDefinitionItem : MethodInfoDataItem
    {
        public MethodDefinitionItem(MethodInfo representedMember) : base(representedMember) { }

        public override string FullName
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
