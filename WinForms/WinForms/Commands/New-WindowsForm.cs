using System;
using System.Linq;
using System.Management.Automation;
using System.Windows.Forms;

namespace Erwine.Leonard.T.WinForms.Commands
{
    [Cmdlet(VerbsCommon.New, "WindowsForm")]
    [OutputType(typeof(PSForm))]
    public class New_WindowsForm : PSCmdlet
    {
        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true)]
        [AllowEmptyCollection]
        [AllowNull]
        public Control[] Control { get; set; }

        [Parameter(Mandatory = false)]
        public System.Windows.Forms.AutoScaleMode AutoScaleMode { get; set; }

        [Parameter(Mandatory = false)]
        public string Name { get; set; }

        [Parameter(Mandatory = false)]
        [Alias("Title")]
        public string Text { get; set; }

        private object _syncRoot = new object();
        private PSForm _form = null;

        protected override void BeginProcessing()
        {
            lock (this._syncRoot)
            {
                if (this._form != null)
                    this._form.Dispose();
                this._form = new PSForm();
                this._form.SuspendLayout();

                if (!String.IsNullOrWhiteSpace(this.Name))
                    this._form.Name = this.Name.Trim();

                if (!String.IsNullOrWhiteSpace(this.Text))
                    this._form.Text = this.Text.Trim();

                if (this.MyInvocation.BoundParameters.ContainsKey("AutoScaleMode"))
                    this._form.AutoScaleMode = this.AutoScaleMode;
            }
        }

        protected override void ProcessRecord()
        {
            if (this.Control == null)
                return;

            foreach (Control control in this.Control.Where(i => i != null))
            {
                try
                {
                    lock (this._syncRoot)
                        this._form.Controls.Add(control);
                }
                catch (Exception exc)
                {
                    this.WriteError(new ErrorRecord(exc, "AddControl", ErrorCategory.DeviceError, control));
                }
            }
        }

        protected override void StopProcessing()
        {
            lock (this._syncRoot)
                this._form.ResumeLayout();
        }

        protected override void EndProcessing()
        {
            lock (this._syncRoot)
                this._form.ResumeLayout();
        }
    }
}