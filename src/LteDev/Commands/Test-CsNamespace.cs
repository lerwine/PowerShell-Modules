using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev.Commands
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    [Cmdlet(VerbsDiagnostic.Test, "CsNamespace")]
    [OutputType(typeof(bool))]
    public class Test_CsNamespace : PSCmdlet
    {
        #region Properties

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [AllowNull]
        [AllowEmptyString]
        public string[] Name { get; set; } = null!;

        #endregion

        private bool _success = true;

        #region Overrides

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _success = true;
        }
        protected override void ProcessRecord()
        {
            if (!_success)
                return;
            if (Name is null)
            {
                _success = false;
                return;
            }

            foreach(string n in Name)
            {
                if (string.IsNullOrEmpty(n) || !ValidateCsNamespaceAttribute.NameRegex.IsMatch(n))
                {
                    _success = false;
                    break;
                }
            }
        }

        protected override void EndProcessing()
        {
            WriteObject(_success);
        }

        #endregion
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
