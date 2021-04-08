using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    [System.AttributeUsage(System.AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class ValidateTypeOrRefAttribute : ValidateEnumeratedArgumentsAttribute
    {
        public bool AllowNull { get; set; }

        public bool AllowEmpty { get; set; }

        public ValidateTypeOrRefAttribute() { }

        protected override void ValidateElement(object element)
        {
            if (element == null)
            {
                if (AllowNull)
                    return;
                throw new ValidationMetadataException("Value cannot be null");
            }

            if (element is PSObject)
                element = ((PSObject)element).BaseObject;
            
            if (element is Type || element is CodeTypeReference)
                return;

            throw new ValidationMetadataException("Value must be a Type or CodeTypeReference");
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
