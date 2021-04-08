using System;
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
    public sealed class ValidateCsNamespaceAttribute : ValidateEnumeratedArgumentsAttribute
    {
        public static readonly Regex NameRegex = new Regex(@"^[a-z_][a-z_\d]*(\.[a-z_][a-z_\d]*)*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool AllowNull { get; set; }

        public bool AllowEmpty { get; set; }

        public bool DoNotIgnoreNonStrings { get; set; }
        
        public ValidateCsNamespaceAttribute() { }

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
            
            if (element is string)
            {
                string s = (string)element;
                if (s.Length == 0)
                {
                    if (AllowEmpty)
                        return;
                    throw new ValidationMetadataException("Value cannot be empty");
                }
                if (NameRegex.IsMatch(s))
                    return;
                throw new ValidationMetadataException("Invalid namespace");
            }

            if (!DoNotIgnoreNonStrings)
                throw new ValidationMetadataException("Value must be a string");
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
