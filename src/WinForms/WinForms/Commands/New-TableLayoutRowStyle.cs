using System;
using System.Management.Automation;
using System.Windows.Forms;

namespace Erwine.Leonard.T.WinForms.Commands
{
    [Cmdlet(VerbsCommon.New, "TableLayoutRowStyle", DefaultParameterSetName = "DefaultWidth")]
    [OutputType(typeof(RowStyle))]
    public class New_TableLayoutRowStyle : PSCmdlet
    {
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "DefaultWidth")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "ExplicittWidth")]
        public SizeType SizeType { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "ExplicittWidth")]
        public float Width { get; set; }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName == "DefaultWidth")
            {
                if (this.MyInvocation.BoundParameters.ContainsKey("SizeType"))
                {
                    try
                    {
                        this.WriteObject(new RowStyle(this.SizeType));
                    }
                    catch (Exception exc)
                    {
                        this.WriteError(new ErrorRecord(exc, "InitializeRowStyle", ErrorCategory.InvalidArgument, this.SizeType));
                    }
                }
                else
                    this.WriteObject(new RowStyle());
            }
            else
            {
                try
                {
                    this.WriteObject(new RowStyle(this.SizeType, this.Width));
                }
                catch (Exception exc)
                {
                    this.WriteError(new ErrorRecord(exc, "InitializeRowStyle", ErrorCategory.InvalidArgument, this.Width));
                }
            }
        }
    }
}

