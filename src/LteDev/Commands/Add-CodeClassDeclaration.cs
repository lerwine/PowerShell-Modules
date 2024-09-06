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
    [Cmdlet(VerbsCommon.Add, "CodeClassDeclaration", DefaultParameterSetName = ParameterSetName_NonAbstract)]
    [OutputType(typeof(CodeDomProxy.INamespaceAndType))]
    public class Add_CodeClassDeclaration : PSCmdlet
    {
        public const string ParameterSetName_Abstract = "Abstract";
        public const string ParameterSetName_NonAbstract = "NonAbstract";
        public const string ParameterSetName_Sealed = "Sealed";
        
        #region Properties

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNull()]
        public CodeNamespace Namespace { get; set; } = null!;

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty()]
        [ValidateCsName()]
        public string[] Name { get; set; } = null!;

        [Parameter]
        [ValidateNotNullOrEmpty()]
        [ValidateCsName()]
        public string[]? GenericArg { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty()]
        [ValidateTypeOrRef()]
        public object[]? Base { get; set; }

        [Parameter(ParameterSetName = ParameterSetName_NonAbstract)]
        public SwitchParameter IsPartial { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_Abstract)]
        public SwitchParameter IsAbstract { get; set; }

        [Parameter(ParameterSetName = ParameterSetName_NonAbstract)]
        [Parameter(ParameterSetName = ParameterSetName_Sealed)]
        public SwitchParameter IsNonPublic { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_Sealed)]
        public SwitchParameter IsSealed { get; set; }

        [Parameter(ParameterSetName = ParameterSetName_Sealed)]
        [Parameter(ParameterSetName = ParameterSetName_NonAbstract)]
        public SwitchParameter IsSerializable { get; set; }

        #endregion

        #region Overrides

        protected override void ProcessRecord()
        {
            foreach (string name in Name)
            {
                CodeTypeDeclaration item = new CodeTypeDeclaration(name);
                item.IsClass = true;
                item.IsPartial = IsPartial.IsPresent;
                item.TypeAttributes = (item.TypeAttributes & ~(TypeAttributes.VisibilityMask | TypeAttributes.LayoutMask | TypeAttributes.ClassSemanticsMask)) |
                    TypeAttributes.Class | ((IsNonPublic.IsPresent) ? TypeAttributes.NotPublic : TypeAttributes.Public);
                if (IsAbstract.IsPresent)
                    item.TypeAttributes |= TypeAttributes.Abstract;
                else if (IsSealed.IsPresent)
                    item.TypeAttributes |= TypeAttributes.Sealed;
                if (IsSerializable.IsPresent)
                    item.TypeAttributes = TypeAttributes.Serializable;

                if (GenericArg is not null && GenericArg.Length > 0)
                {
                    foreach (string a in GenericArg)
                        item.TypeParameters.Add(a);
                }

                if (Base is not null && Base.Length > 0)
                {
                    foreach (object obj in Base)
                    {
                        if (obj is Type)
                            item.BaseTypes.Add((Type)obj);
                        else
                            item.BaseTypes.Add((CodeTypeReference)obj);
                    }
                }

                Namespace.Types.Add(item);
                WriteObject(new CodeDomProxy.NamespaceAndType(Namespace, item));
            }
        }

        #endregion
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
