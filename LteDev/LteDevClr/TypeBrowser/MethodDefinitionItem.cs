using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LteDevClr.TypeBrowser
{
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
}
