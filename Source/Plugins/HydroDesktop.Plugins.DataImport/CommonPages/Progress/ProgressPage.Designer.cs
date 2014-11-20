namespace HydroDesktop.Plugins.DataImport.CommonPages.Progress
{
    partial class ProgressPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.Size = new System.Drawing.Size(516, 64);
            this.Banner.Subtitle = "Importing data...";
            this.Banner.Title = "Progress Page";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(30, 120);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(457, 23);
            this.progressBar.TabIndex = 2;
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(30, 101);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(88, 13);
            this.lblInfo.TabIndex = 3;
            this.lblInfo.Text = "Current operation";
            // 
            // ProgressPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.progressBar);
            this.Name = "ProgressPage";
            this.Size = new System.Drawing.Size(516, 213);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.ProgressPage_SetActive);
            this.Controls.SetChildIndex(this.Banner, 0);
            this.Controls.SetChildIndex(this.progressBar, 0);
            this.Controls.SetChildIndex(this.lblInfo, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblInfo;
    }
}
