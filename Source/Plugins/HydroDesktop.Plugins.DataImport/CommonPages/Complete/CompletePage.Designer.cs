namespace HydroDesktop.Plugins.DataImport.CommonPages.Complete
{
    partial class CompletePage
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
            this.lblInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Sidebar
            // 
            this.Sidebar.Size = new System.Drawing.Size(165, 304);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(183, 134);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(84, 13);
            this.lblInfo.TabIndex = 1;
            this.lblInfo.Text = "Import finished...";
            // 
            // CompletePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Caption = "";
            this.Controls.Add(this.lblInfo);
            this.Name = "CompletePage";
            this.Size = new System.Drawing.Size(424, 304);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.CompletePage_SetActive);
            this.Controls.SetChildIndex(this.lblInfo, 0);
            this.Controls.SetChildIndex(this.Sidebar, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInfo;
    }
}
