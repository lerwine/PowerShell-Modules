using System;
using System.Linq;
using System.Windows.Forms;

namespace PSModuleInstallUtil
{
    public partial class NotificationForm : Form
    {
        public string WindowTitle
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        public string HeadingText
        {
            get { return this.headingTextBox.Text; }
            set { this.headingTextBox.Text = value; }
        }

        public string DetailsText
        {
            get { return this.detailsTextBox.Text; }
            set { this.detailsTextBox.Text = value; }
        }

        public DialogResult Button1DialogResult
        {
            get { return this.button1.DialogResult; }
            set { this.button1.DialogResult = value; }
        }

        public string Button1Text
        {
            get { return this.button1.Text; }
            set { this.button1.Text = value; }
        }

        public bool Button1Visible
        {
            get { return this.button1.Visible; }
            set { this.button1.Visible = value; }
        }

        public DialogResult Button2DialogResult
        {
            get { return this.button2.DialogResult; }
            set { this.button2.DialogResult = value; }
        }

        public string Button2Text
        {
            get { return this.button2.Text; }
            set { this.button2.Text = value; }
        }

        public bool Button2Visible
        {
            get { return this.button2.Visible; }
            set { this.button2.Visible = value; }
        }

        public DialogResult Button3DialogResult
        {
            get { return this.button3.DialogResult; }
            set { this.button3.DialogResult = value; }
        }

        public string Button3Text
        {
            get { return this.button3.Text; }
            set { this.button3.Text = value; }
        }

        public bool Button3Visible
        {
            get { return this.button3.Visible; }
            set { this.button3.Visible = value; }
        }

        public NotificationForm()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, EventArgs e)
        {
            this.DialogResult = (sender as Button).DialogResult;
            this.Close();
        }

        private void NotificationForm_Shown(object sender, EventArgs e)
        {
            this.BringToFront();
        }
    }
}
