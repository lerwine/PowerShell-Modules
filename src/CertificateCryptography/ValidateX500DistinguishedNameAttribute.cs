using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.CertificateCryptography
{
    public sealed class ValidateX500DistinguishedNameAttribute : ValidateEnumeratedArgumentsAttribute
    {
        public bool AllowNull { get; set; }

        public bool AllowEmpty { get; set; }

        protected override void ValidateElement(object element)
        {
            if (element == null)
            {
                if (!AllowNull)
                    throw new ValidationMetadataException("Null name not allowed.");
                return;
            }

            string n = (element is PSObject) ? ((((PSObject)element).BaseObject is string) ? (string)(((PSObject)element).BaseObject) : element.ToString()) : ((element is string) ? (string)element : element.ToString());
            if (String.IsNullOrEmpty(n))
            {
                if (!AllowEmpty)
                    throw new ValidationMetadataException("Empty name not allowed.");
                return;
            }

            try { new X500DistinguishedName(n); }
            catch (Exception e) { throw new ValidationMetadataException(e.Message, e); }
        }
    }
}
