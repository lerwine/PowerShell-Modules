using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Windows.Forms;

namespace Erwine.Leonard.T.WinForms
{
    public class PSButton : Button
    {
        public ScriptBlock PSOnClick { get; set; }

        public object State { get; set; }

        public Collection<PSObject> LastOutput { get; private set; }

        public PSButton()
        {
            this.Size = new System.Drawing.Size(75, 23);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (this.PSOnClick != null)
                this.LastOutput = this.PSOnClick.Invoke(this);
        }
    }
}
