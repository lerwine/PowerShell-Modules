namespace Erwine.Leonard.T.WinForms
{
    partial class PSForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PSForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "PSForm";
            this.Text = "PSForm";
            this.Activated += new System.EventHandler(this.PSForm_Activated);
            this.Deactivate += new System.EventHandler(this.PSForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PSForm_FormClosing);
            this.Load += new System.EventHandler(this.PSForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
    }
}