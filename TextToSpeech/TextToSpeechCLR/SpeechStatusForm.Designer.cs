namespace TextToSpeechCLR
{
    partial class SpeechStatusForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.previousTextBox = new System.Windows.Forms.TextBox();
            this.currentTextBox = new System.Windows.Forms.TextBox();
            this.nextTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.previousTextBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.currentTextBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.nextTextBox, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.button1, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.button2, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.button3, 4, 3);
            this.tableLayoutPanel1.Controls.Add(this.nextButton, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.backButton, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(597, 464);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // previousTextBox
            // 
            this.previousTextBox.AcceptsReturn = true;
            this.previousTextBox.AcceptsTab = true;
            this.tableLayoutPanel1.SetColumnSpan(this.previousTextBox, 5);
            this.previousTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.previousTextBox.Location = new System.Drawing.Point(3, 3);
            this.previousTextBox.Multiline = true;
            this.previousTextBox.Name = "previousTextBox";
            this.previousTextBox.ReadOnly = true;
            this.previousTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.previousTextBox.Size = new System.Drawing.Size(591, 60);
            this.previousTextBox.TabIndex = 0;
            // 
            // currentTextBox
            // 
            this.currentTextBox.AcceptsReturn = true;
            this.currentTextBox.AcceptsTab = true;
            this.tableLayoutPanel1.SetColumnSpan(this.currentTextBox, 5);
            this.currentTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currentTextBox.Location = new System.Drawing.Point(3, 69);
            this.currentTextBox.Multiline = true;
            this.currentTextBox.Name = "currentTextBox";
            this.currentTextBox.ReadOnly = true;
            this.currentTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.currentTextBox.Size = new System.Drawing.Size(591, 297);
            this.currentTextBox.TabIndex = 1;
            // 
            // nextTextBox
            // 
            this.nextTextBox.AcceptsReturn = true;
            this.nextTextBox.AcceptsTab = true;
            this.tableLayoutPanel1.SetColumnSpan(this.nextTextBox, 5);
            this.nextTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.nextTextBox.Location = new System.Drawing.Point(3, 372);
            this.nextTextBox.Multiline = true;
            this.nextTextBox.Name = "nextTextBox";
            this.nextTextBox.ReadOnly = true;
            this.nextTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.nextTextBox.Size = new System.Drawing.Size(591, 60);
            this.nextTextBox.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(357, 438);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Pause";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(438, 438);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Resume";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(519, 438);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Stop";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // nextButton
            // 
            this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.nextButton.Location = new System.Drawing.Point(276, 438);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(75, 23);
            this.nextButton.TabIndex = 6;
            this.nextButton.Text = "Next";
            this.nextButton.UseVisualStyleBackColor = true;
            // 
            // backButton
            // 
            this.backButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.backButton.Location = new System.Drawing.Point(195, 438);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(75, 23);
            this.backButton.TabIndex = 7;
            this.backButton.Text = "Back";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // SpeechStatusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 464);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SpeechStatusForm";
            this.Text = "Speech Status";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox previousTextBox;
        private System.Windows.Forms.TextBox currentTextBox;
        private System.Windows.Forms.TextBox nextTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button backButton;
    }
}