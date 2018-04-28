using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Windows.Forms;

namespace Erwine.Leonard.T.WinForms
{
    [Serializable]
    public class GUISelectionResult
    {
        public GUISelectionResult() { }

        public GUISelectionResult(DialogResult dialogResult, TerminatingListSelectorAction terminatingAction, ListBox listBox, Collection<PSObject> lastScriptBlockResult)
        {
            this.DialogResult = dialogResult;
            this.TerminatingAction = terminatingAction;
            this.AllItems = listBox.Items.OfType<ListItem>().Select(i => i.Value).ToArray();
            this.SelectedIndices = listBox.SelectedIndices.OfType<int>().ToArray();
            this.SelectedValues = listBox.SelectedItems.OfType<ListItem>().Select(i => i.Value).ToArray();
            this.SelectedIndex = listBox.SelectedIndex;
            ListItem item = listBox.SelectedValue as ListItem;
            this.SelectedValue = (item == null) ? null : item.Value;
        }

        public DialogResult DialogResult { get; set; }

        public TerminatingListSelectorAction TerminatingAction { get; set; }

        public int[] SelectedIndices { get; set; }

        public object[] SelectedValues { get; set; }

        public int SelectedIndex { get; set; }

        public object SelectedValue { get; set; }

        public object[] AllItems { get; set; }
    }
}
