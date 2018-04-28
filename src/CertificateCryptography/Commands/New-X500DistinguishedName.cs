using System;
using System.Management.Automation;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Erwine.Leonard.T.CertificateCryptography.Commands
{

    [Cmdlet(VerbsCommon.New, "X500DistinguishedName", DefaultParameterSetName = ParameterSetName_String)]
    [OutputType(typeof(X500DistinguishedName))]
    public class New_X500DistinguishedName : PSCmdlet
    {
        public const string ParameterSetName_AsnEncoded = "AsnEncoded";
        public const string ParameterSetName_String = "String";
        public const string ParameterSetName_Bytes = "Bytes";

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_String)]
        [ValidateNotNullOrEmpty()]
        public string[] InputString { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_AsnEncoded)]
        [ValidateNotNullOrEmpty()]
        public AsnEncodedData[] Encoded { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_Bytes)]
        [ValidateNotNullOrEmpty()]
        public byte[] Data { get; set; }

        [Parameter(ParameterSetName = ParameterSetName_String)]
        [ValidateNotNullOrEmpty()]
        public X500DistinguishedNameFlags[] Flags { get; set; }

        private X500DistinguishedNameFlags? _flags = null;

        protected override void BeginProcessing()
        {
            if (Flags == null || Flags.Length == 0)
                _flags = null;
            else if (Flags.Length == 1)
                _flags = Flags[0];
            else
                _flags = Flags.Skip(1).Aggregate(Flags[0], (f, a) => f | a);
        }
        protected override void ProcessRecord()
        {
            switch (ParameterSetName)
            {
                case ParameterSetName_String:
                    if (_flags.HasValue)
                    {
                        foreach (string n in InputString)
                        {
                            try { WriteObject(new X500DistinguishedName(n, _flags.Value)); }
                            catch (Exception exc) { WriteError(new ErrorRecord(exc, "InvalidValue", ErrorCategory.InvalidArgument, new object[] { n, _flags })); }
                        }
                    }
                    else
                    {
                        foreach (string n in InputString)
                        {
                            try { WriteObject(new X500DistinguishedName(n)); }
                            catch (Exception exc) { WriteError(new ErrorRecord(exc, "InvalidValue", ErrorCategory.InvalidArgument, n)); }
                        }
                    }
                    break;
                case ParameterSetName_AsnEncoded:
                    foreach (AsnEncodedData n in Encoded)
                    {
                        try { WriteObject(new X500DistinguishedName(n)); }
                        catch (Exception exc) { WriteError(new ErrorRecord(exc, "InvalidValue", ErrorCategory.InvalidArgument, n)); }
                    }
                    break;
                default:
                    try { WriteObject(new X500DistinguishedName(Data)); }
                    catch (Exception exc) { WriteError(new ErrorRecord(exc, "InvalidValue", ErrorCategory.InvalidArgument, Data)); }
                    break;
            }
        }
    }
}
