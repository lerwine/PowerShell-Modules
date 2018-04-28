using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Windows.Forms;

namespace Erwine.Leonard.T.WinForms
{
    public partial class ListSelectorForm : Form
    {
        public TerminatingListSelectorAction TerminatingAction { get; set; }

        public bool IsActivated { get; set; }

        public ScriptBlock FormClosingScriptBlock { get; set; }

        public Collection<PSObject> LastScriptBlockResult { get; set; }

        public ListSelectorForm()
        {
            this.InitializeComponent();
        }

        private void ListSelectorForm_Load(object sender, EventArgs e)
        {
            this.TerminatingAction = TerminatingListSelectorAction.User;
            this.IsActivated = false;
        }

        private void ListSelectorForm_Activated(object sender, EventArgs e)
        {
            this.IsActivated = true;
        }

        private void ListSelectorForm_Deactivate(object sender, EventArgs e)
        {
            this.IsActivated = false;
        }

        private void optionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasCommandButton = false;
            if (this.editButton.Visible)
            {
                hasCommandButton = true;
                this.editButton.Enabled = this.optionsListBox.SelectedIndex > -1;
            }

            if (this.deleteButton.Visible)
            {
                hasCommandButton = true;
                this.deleteButton.Enabled = this.optionsListBox.SelectedIndex > -1;
            }

            if (this.selectButton.Visible)
            {
                this.selectButton.Enabled = this.optionsListBox.SelectedIndex > -1;
                return;
            }

            if (hasCommandButton || !this.IsActivated)
                return;

            this.TerminatingAction = TerminatingListSelectorAction.Select;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            this.TerminatingAction = TerminatingListSelectorAction.New;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            this.TerminatingAction = TerminatingListSelectorAction.Select;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            this.TerminatingAction = TerminatingListSelectorAction.Edit;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            this.TerminatingAction = TerminatingListSelectorAction.Delete;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.TerminatingAction = TerminatingListSelectorAction.Cancel;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ListSelectorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.FormClosingScriptBlock != null)
                this.LastScriptBlockResult = this.FormClosingScriptBlock.Invoke(this, e);
        }
    }
}
