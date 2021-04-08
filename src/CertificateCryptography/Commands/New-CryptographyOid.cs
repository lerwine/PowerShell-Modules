using System;
using System.Management.Automation;
using System.Security.Cryptography;

namespace LteDev.Commands
{
    [Cmdlet(VerbsCommon.New, "CryptographyOid", DefaultParameterSetName = ParameterSetName_Piped)]
    [OutputType(typeof(Oid))]
    public class New_CryptographyOid : PSCmdlet
    {
        public const string ParameterSetName_Piped = "Piped";
        public const string ParameterSetName_String = "String";
        public const string ParameterSetName_Oid = "Oid";

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_Piped)]
        [ValidateNotNullOrEmpty()]
        public string[] InputString { get; set; }
        
        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_String)]
        [ValidateNotNullOrEmpty()]
        public string Value { get; set; }

        [Parameter(ParameterSetName = ParameterSetName_String)]
        [ValidateNotNullOrEmpty()]
        public string FriendlyName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = ParameterSetName_Oid)]
        [ValidateNotNull()]
        public Oid [] Oid { get; set; }

        protected override void ProcessRecord()
        {
            switch (ParameterSetName)
            {
                case ParameterSetName_String:
                    try
                    {
                        if (MyInvocation.BoundParameters.ContainsKey("FriendlyName"))
                            WriteObject(new Oid(Value, FriendlyName));
                        else
                            WriteObject(new Oid(Value));
                    }
                    catch (Exception exc)
                    {
                        if (MyInvocation.BoundParameters.ContainsKey("FriendlyName"))
                            WriteError(new ErrorRecord(exc, "InvalidValue", ErrorCategory.InvalidArgument, new string[] { Value, FriendlyName }));
                        else
                            WriteError(new ErrorRecord(exc, "InvalidValue", ErrorCategory.InvalidArgument, Value));
                    }
                    break;
                case ParameterSetName_Oid:
                    if (Oid == null)
                        return;
                    foreach (Oid oid in Oid)
                    {
                        if (oid == null)
                            continue;
                        try { WriteObject(new Oid(oid)); }
                        catch (Exception exc)
                        {
                            WriteError(new ErrorRecord(exc, "InvalidOID", ErrorCategory.InvalidArgument, oid));
                        }
                    }
                    break;
                default:
                    if (InputString == null)
                        return;
                    foreach (string s in InputString)
                    {
                        if (s == null)
                            continue;
                        try { WriteObject(new Oid(s)); }
                        catch (Exception exc)
                        {
                            WriteError(new ErrorRecord(exc, "InvalidValue", ErrorCategory.InvalidArgument, s));
                        }
                    }
                    break;
            }
        }
    }
}
