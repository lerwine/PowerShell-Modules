using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Windows.Forms;

namespace Erwine.Leonard.T.WinForms
{
    public partial class PSForm : Form
    {
        public string Title
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        public ScriptBlock LoadScriptBlock { get; set; }

        public ScriptBlock ActivatedScriptBlock { get; set; }

        public ScriptBlock DeactivateScriptBlock { get; set; }

        public ScriptBlock FormClosingScriptBlock { get; set; }

        public Collection<PSObject> LastScriptBlockResult { get; set; }

        public PSForm()
        {
            this.InitializeComponent();
        }

        private void PSForm_Load(object sender, EventArgs e)
        {
            if (this.LoadScriptBlock != null)
                this.LastScriptBlockResult = this.LoadScriptBlock.Invoke(this, e);
        }

        private void PSForm_Activated(object sender, EventArgs e)
        {
            if (this.ActivatedScriptBlock != null)
                this.LastScriptBlockResult = this.ActivatedScriptBlock.Invoke(this, e);
        }

        private void PSForm_Deactivate(object sender, EventArgs e)
        {
            if (this.DeactivateScriptBlock != null)
                this.LastScriptBlockResult = this.DeactivateScriptBlock.Invoke(this, e);
        }

        private void PSForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.FormClosingScriptBlock != null)
                this.LastScriptBlockResult = this.FormClosingScriptBlock.Invoke(this, e);
        }
    }
}
