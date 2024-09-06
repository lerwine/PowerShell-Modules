using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev.Commands
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    [Cmdlet(VerbsData.ConvertTo, "CodeTypeString")]
    [OutputType(typeof(CodeDomProxy.INamespaceAndType))]
    public class ConvertTo_CodeTypeString : PSCmdlet
    {   
        #region Properties

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public CodeTypeReference[] Type { get; set; } = null!;

        #endregion

        #region Overrides

        public static string ToCodeTypeString(CodeTypeReference type)
        {
            if (type is null)
                return "null";
            if (type.ArrayRank == 1)
                return ToCodeTypeString(type.ArrayElementType) + "[]";
            if (type.ArrayRank == 2)
                return ToCodeTypeString(type.ArrayElementType) + "[,]";
            if (type.ArrayRank > 2)
                return ToCodeTypeString(type.ArrayElementType) + "[" + new string(',', type.ArrayRank - 1) + "]";
            string[] ga = type.TypeArguments.OfType<CodeTypeReference>().Select(t => ToCodeTypeString(t))
                .Where(s => s.Length > 0).ToArray();
            if (ga.Length == 0)
                return type.BaseType;
            return type.BaseType + "." + '[' + string.Join(",", ga) + "]";
        }
        
        protected override void ProcessRecord()
        {
            foreach (string t in Type.Select(s => ToCodeTypeString(s)))
                WriteObject(t);
        }

        #endregion
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
