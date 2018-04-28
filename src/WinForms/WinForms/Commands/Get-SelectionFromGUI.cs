using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Windows.Forms;

namespace Erwine.Leonard.T.WinForms.Commands
{
    [Cmdlet(VerbsCommon.Get, "SelectionFromGUI", DefaultParameterSetName = Get_SelectionFromGUI.ParameterSetName_SingleByObject)]
    [OutputType(typeof(GUISelectionResult))]
    public class Get_SelectionFromGUI : PSCmdlet
    {
        public const string ParameterSetName_SingleByIndex = "SingleByIndex";
        public const string ParameterSetName_MultiByIndex = "MultiByIndex";
        public const string ParameterSetName_SingleByObject = "SingleByObject";
        public const string ParameterSetName_MultiByObject = "MultiByObject";

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Alias("Items", "Item", "Options")]
        public object[] InputObject { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = Get_SelectionFromGUI.ParameterSetName_SingleByIndex)]
        [Parameter(Mandatory = false, ParameterSetName = Get_SelectionFromGUI.ParameterSetName_SingleByObject)]
        [Alias("Single")]
        public SwitchParameter SingleSelect { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = Get_SelectionFromGUI.ParameterSetName_MultiByIndex)]
        [Parameter(Mandatory = true, ParameterSetName = Get_SelectionFromGUI.ParameterSetName_MultiByObject)]
        [Alias("Multi")]
        public SwitchParameter MultiSelect { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = Get_SelectionFromGUI.ParameterSetName_SingleByIndex)]
        public int SelectedIndex { get; set; }

        [Parameter(Mandatory = false, Position = 1, ParameterSetName = Get_SelectionFromGUI.ParameterSetName_SingleByObject)]
        [AllowNull]
        public object SelectedItem { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = Get_SelectionFromGUI.ParameterSetName_MultiByIndex)]
        [AllowEmptyCollection]
        public int[] SelectedIndices { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = Get_SelectionFromGUI.ParameterSetName_MultiByObject)]
        [AllowEmptyCollection]
        [AllowNull]
        public object[] SelectedItems { get; set; }

        [Parameter(Mandatory = false, Position = 2)]
        public ScriptBlock OnFormClosing { get; set; }

        [Parameter(Mandatory = false)]
        public string Title { get; set; }

        [Parameter(Mandatory = false)]
        [Alias("Heading")]
        public string HeadingText { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter AllowNew { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter AllowEdit { get; set; }

        [Parameter(Mandatory = false)]
        [Alias("ShowSelect")]
        public SwitchParameter ShowSelectButton { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter AllowDelete { get; set; }

        [Parameter(Mandatory = false)]
        [Alias("HideCancel")]
        public SwitchParameter HideCancelButton { get; set; }

        private List<ListItem> _items = null;

        protected override void BeginProcessing()
        {
            this._items = new List<ListItem>();
        }

        protected override void ProcessRecord()
        {
            if (this.InputObject != null)
                this._items.AddRange(this.InputObject.Select(o => (o != null && o is ListItem) ? o as ListItem : new ListItem(o)));
        }

        protected override void EndProcessing()
        {
            using (ListSelectorForm form = new ListSelectorForm())
            {
                if (!String.IsNullOrEmpty(this.Title))
                    form.Text = this.Title;
                form.headingLabel.Visible = !String.IsNullOrEmpty(this.HeadingText);
                if (form.headingLabel.Visible)
                    form.headingLabel.Text = this.HeadingText;
                foreach (ListItem item in this._items)
                    form.optionsListBox.Items.Add(item);
                switch (this.ParameterSetName)
                {
                    case Get_SelectionFromGUI.ParameterSetName_MultiByIndex:
                        form.optionsListBox.SelectionMode = SelectionMode.MultiExtended;
                        foreach (int index in this.SelectedIndices.Distinct().Where(i => i > -1 && i < this._items.Count))
                            form.optionsListBox.SelectedIndices.Add(index);
                        break;
                    case Get_SelectionFromGUI.ParameterSetName_MultiByObject:
                        form.optionsListBox.SelectionMode = SelectionMode.MultiExtended;
                        Collection<ListItem> alreadySelected = new Collection<ListItem>();
                        foreach (ListItem item in this.SelectedItems.Select(o => (o != null && o is ListItem) ? o as ListItem : new ListItem(o)))
                        {
                            if (alreadySelected.Any(i => i.Equals(item)))
                                continue;

                            alreadySelected.Add(item);
                            if (!form.optionsListBox.Items.Contains(item))
                                form.optionsListBox.Items.Add(item);
                            form.optionsListBox.SelectedItems.Add(item);
                        }
                        break;
                    case Get_SelectionFromGUI.ParameterSetName_SingleByIndex:
                        form.optionsListBox.SelectionMode = SelectionMode.One;
                        form.optionsListBox.SelectedIndex = (this.SelectedIndex > -1 && this.SelectedIndex < this._items.Count) ? this.SelectedIndex : -1;
                        break;
                    default:
                        form.optionsListBox.SelectionMode = SelectionMode.One;
                        if (this.SelectedItem == null)
                            form.optionsListBox.SelectedItem = this._items.FirstOrDefault(i => i.Value == null);
                        else
                        {
                            ListItem li = (this.SelectedItem is ListItem) ? this.SelectedItem as ListItem : new ListItem(this.SelectedItem);
                            if (!form.optionsListBox.Items.Contains(li))
                                form.optionsListBox.Items.Add(li);
                            form.optionsListBox.SelectedItem = li;
                        }
                        break;
                }

                form.newButton.Visible = this.AllowNew.ToBool();
                form.selectButton.Visible = this.ShowSelectButton.ToBool();
                form.editButton.Visible = this.AllowEdit.ToBool();
                form.deleteButton.Visible = this.AllowDelete.ToBool();
                form.cancelButton.Visible = !this.HideCancelButton.ToBool();
                form.FormClosingScriptBlock = this.OnFormClosing;

                this.WriteObject(new GUISelectionResult(form.ShowDialog(), form.TerminatingAction, form.optionsListBox, form.LastScriptBlockResult));
            }
        }
    }
}
