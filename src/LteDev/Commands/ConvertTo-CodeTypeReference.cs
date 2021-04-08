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
    [Cmdlet(VerbsData.ConvertTo, "CodeTypeReference", DefaultParameterSetName = ParameterSetName_NsType)]
    [OutputType(typeof(CodeDomProxy.INamespaceAndType))]
    public class ConvertTo_CodeTypeReference : PSCmdlet
    {
        public const string ParameterSetName_Type = "Type";
        public const string ParameterSetName_NsType = "NsType";
        
        #region Properties

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_Type)]
        [ValidateNotNull()]
        public CodeNamespace Namespace { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_Type)]
        [ValidateNotNullOrEmpty()]
        public CodeTypeDeclaration[] Type { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_NsType)]
        [ValidateNotNullOrEmpty()]
        public CodeDomProxy.INamespaceAndType[] NsType { get; set; }

        #endregion

        #region Overrides

        protected override void ProcessRecord()
        {
            if (NsType != null && NsType.Length > 0)
            {
                foreach (CodeDomProxy.INamespaceAndType t in NsType)
                    WriteObject(t.ToTypeReference());
            }
            else if (Namespace != null && Type != null && Type.Length > 0)
            {
                foreach (CodeTypeDeclaration t in Type)
                    (new CodeDomProxy.NamespaceAndType(Namespace, t)).ToTypeReference();
            }
        }

        #endregion
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
