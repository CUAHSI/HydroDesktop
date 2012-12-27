namespace Search3.Searching
{
    partial class SearchProgressForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchProgressForm));
            this.btnCloseCancel = new System.Windows.Forms.Button();
            this.lbCurrentOperation = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lbOutput = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnCloseCancel
            // 
            this.btnCloseCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseCancel.Location = new System.Drawing.Point(346, 217);
            this.btnCloseCancel.Name = "btnCloseCancel";
            this.btnCloseCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCloseCancel.TabIndex = 0;
            this.btnCloseCancel.Text = "Cancel";
            this.btnCloseCancel.UseVisualStyleBackColor = true;
            this.btnCloseCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lbCurrentOperation
            // 
            this.lbCurrentOperation.AutoSize = true;
            this.lbCurrentOperation.Location = new System.Drawing.Point(12, 29);
            this.lbCurrentOperation.Name = "lbCurrentOperation";
            this.lbCurrentOperation.Size = new System.Drawing.Size(0, 13);
            this.lbCurrentOperation.TabIndex = 1;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(13, 49);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(408, 26);
            this.progressBar1.TabIndex = 2;
            // 
            // lbOutput
            // 
            this.lbOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbOutput.FormattingEnabled = true;
            this.lbOutput.Location = new System.Drawing.Point(13, 81);
            this.lbOutput.Name = "lbOutput";
            this.lbOutput.Size = new System.Drawing.Size(408, 121);
            this.lbOutput.TabIndex = 5;
            // 
            // SearchProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 252);
            this.Controls.Add(this.lbOutput);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lbCurrentOperation);
            this.Controls.Add(this.btnCloseCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SearchProgressForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Searching...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCloseCancel;
        private System.Windows.Forms.Label lbCurrentOperation;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ListBox lbOutput;
    }
}