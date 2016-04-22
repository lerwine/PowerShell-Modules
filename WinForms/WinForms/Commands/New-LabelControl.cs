using System.Management.Automation;
using System.Windows.Forms;

namespace Erwine.Leonard.T.WinForms.Commands
{
    [Cmdlet(VerbsCommon.New, "LabelControl")]
    [OutputType(typeof(Label))]
    public class New_LabelControl : New_Control<Label>
    {
        [Parameter(Mandatory = false, Position = 0)]
        public string Text { get; set; }

        protected override bool FinalizeControl()
        {
            if (this.Text != null)
                this.Control.Text = this.Text;

            return true;
        }
    }
}
