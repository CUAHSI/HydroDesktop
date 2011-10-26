namespace EPADelineation
{
    partial class FmProgress
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
            this.lblmessage1 = new System.Windows.Forms.Label();
            this.lblmessage2 = new System.Windows.Forms.Label();
            this.lblmessage3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblmessage1
            // 
            this.lblmessage1.AutoSize = true;
            this.lblmessage1.Location = new System.Drawing.Point(12, 34);
            this.lblmessage1.Name = "lblmessage1";
            this.lblmessage1.Size = new System.Drawing.Size(141, 13);
            this.lblmessage1.TabIndex = 2;
            this.lblmessage1.Text = "Calling EPA Web Services...";
            // 
            // lblmessage2
            // 
            this.lblmessage2.AutoSize = true;
            this.lblmessage2.Location = new System.Drawing.Point(12, 20);
            this.lblmessage2.Name = "lblmessage2";
            this.lblmessage2.Size = new System.Drawing.Size(98, 13);
            this.lblmessage2.TabIndex = 3;
            this.lblmessage2.Text = "Sending Request...";
            this.lblmessage2.Visible = false;
            // 
            // lblmessage3
            // 
            this.lblmessage3.AutoSize = true;
            this.lblmessage3.Location = new System.Drawing.Point(12, 34);
            this.lblmessage3.Name = "lblmessage3";
            this.lblmessage3.Size = new System.Drawing.Size(156, 13);
            this.lblmessage3.TabIndex = 4;
            this.lblmessage3.Text = "Drawing Features on the Map...";
            this.lblmessage3.Visible = false;
            // 
            // FmProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(279, 82);
            this.Controls.Add(this.lblmessage3);
            this.Controls.Add(this.lblmessage2);
            this.Controls.Add(this.lblmessage1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FmProgress";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ShowIcon = false;
            this.Text = "Progressing...";
            this.TransparencyKey = System.Drawing.Color.Transparent;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblmessage1;
        private System.Windows.Forms.Label lblmessage2;
        private System.Windows.Forms.Label lblmessage3;

    }
}