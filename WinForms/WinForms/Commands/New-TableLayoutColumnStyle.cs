using System;
using System.Management.Automation;
using System.Windows.Forms;

namespace Erwine.Leonard.T.WinForms.Commands
{
    [Cmdlet(VerbsCommon.New, "TableLayoutColumnStyle", DefaultParameterSetName = "DefaultHeight")]
    [OutputType(typeof(ColumnStyle))]
    public class New_TableLayoutColumnStyle : PSCmdlet
    {
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "DefaultHeight")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "ExplicittWidth")]
        public SizeType SizeType { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "ExplicittWidth")]
        public float Height { get; set; }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName == "DefaultHeight")
            {
                if (this.MyInvocation.BoundParameters.ContainsKey("SizeType"))
                {
                    try
                    {
                        this.WriteObject(new ColumnStyle(this.SizeType));
                    }
                    catch (Exception exc)
                    {
                        this.WriteError(new ErrorRecord(exc, "InitializeColumnStyle", ErrorCategory.InvalidArgument, this.SizeType));
                    }
                }
                else
                    this.WriteObject(new ColumnStyle());
            }
            else
            {
                try
                {
                    this.WriteObject(new ColumnStyle(this.SizeType, this.Height));
                }
                catch (Exception exc)
                {
                    this.WriteError(new ErrorRecord(exc, "InitializeColumnStyle", ErrorCategory.InvalidArgument, this.Height));
                }
            }
        }
    }
}
