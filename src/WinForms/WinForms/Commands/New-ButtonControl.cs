using System.Management.Automation;

namespace Erwine.Leonard.T.WinForms.Commands
{
    [Cmdlet(VerbsCommon.New, "ButtonControl")]
    [OutputType(typeof(PSButton))]
    public class New_ButtonControl : New_Control<PSButton>
    {
        [Parameter(Mandatory = false, Position = 0)]
        public string Text { get; set; }

        [Parameter(Mandatory = false, Position = 1)]
        public ScriptBlock OnClick { get; set; }

        [Parameter(Mandatory = false)]
        public object State { get; set; }

        protected override bool FinalizeControl()
        {
            if (this.Text != null)
                this.Control.Text = this.Text;

            if (this.OnClick != null)
                this.Control.PSOnClick = this.OnClick;

            this.Control.State = this.State;

            return true;
        }
    }
}
