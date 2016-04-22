using System;
using System.Drawing;
using System.Management.Automation;
using System.Windows.Forms;

namespace Erwine.Leonard.T.WinForms.Commands
{
    public abstract class New_Control<TControl> : PSCmdlet
        where TControl : Control, new()
    {
        protected virtual bool ShouldSuspendLayout { get { return false; } }

        protected virtual bool ShouldPerformLayout { get { return false; } }

        [Parameter(Mandatory = false)]
        public string Name { get; set; }

        [Parameter(Mandatory = false)]
        public DockStyle Dock { get; set; }

        public int TabIndex { get; set; }

        public Size Size { get; set; }

        public Point Location { get; set; }

        SwitchParameter AnchorTop { get; set; }

        SwitchParameter AnchorRight { get; set; }

        SwitchParameter AnchorBottom { get; set; }

        SwitchParameter AnchorLeft { get; set; }

        protected TControl Control { get; private set; }

        protected override void BeginProcessing()
        {
            this.Control = new TControl();
            if (this.ShouldSuspendLayout)
                this.Control.SuspendLayout();
        }

        protected override void EndProcessing()
        {
            try
            {
                if (this.MyInvocation.BoundParameters.ContainsKey("Name"))
                    this.Control.Name = this.Name;
            }
            catch (Exception exc)
            {
                this.WriteError(new ErrorRecord(exc, "SetName", ErrorCategory.WriteError, this.Name));
                this.Control.Dispose();
                this.Control = null;
            }

            if (this.Control == null)
                return;

            try
            {
                if (this.MyInvocation.BoundParameters.ContainsKey("Dock"))
                    this.Control.Dock = this.Dock;
            }
            catch (Exception exc)
            {
                this.WriteError(new ErrorRecord(exc, "SetDock", ErrorCategory.WriteError, this.Dock));
                this.Control.Dispose();
                this.Control = null;
            }

            if (this.Control == null)
                return;

            if (!this.FinalizeControl())
            {
                this.Control.Dispose();
                this.Control = null;
                return;
            }

            try
            {
                if (this.ShouldSuspendLayout)
                    this.Control.ResumeLayout(false);
                if (this.ShouldPerformLayout)
                    this.Control.PerformLayout();
            }
            catch (Exception exc)
            {
                this.WriteError(new ErrorRecord(exc, "ResumeLayout", ErrorCategory.WriteError, this.Control));
                this.Control.Dispose();
                this.Control = null;
            }

            if (this.Control != null)
                this.WriteObject(this.Control);
        }

        protected abstract bool FinalizeControl();
    }
}
