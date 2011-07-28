namespace HydroDesktop.DataDownload.LayerInformation
{
    partial class CustomToolTipControl
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
            this.lblServiceDesciptionUrl = new System.Windows.Forms.LinkLabel();
            this.lblDataSource = new System.Windows.Forms.Label();
            this.lblSiteName = new System.Windows.Forms.Label();
            this.lblValueCount = new System.Windows.Forms.Label();
            this.lblDownloadData = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // lblServiceDesciptionUrl
            // 
            this.lblServiceDesciptionUrl.AutoSize = true;
            this.lblServiceDesciptionUrl.Location = new System.Drawing.Point(13, 71);
            this.lblServiceDesciptionUrl.Name = "lblServiceDesciptionUrl";
            this.lblServiceDesciptionUrl.Size = new System.Drawing.Size(116, 13);
            this.lblServiceDesciptionUrl.TabIndex = 0;
            this.lblServiceDesciptionUrl.TabStop = true;
            this.lblServiceDesciptionUrl.Text = "lblServiceDesciptionUrl";
            // 
            // lblDataSource
            // 
            this.lblDataSource.AutoSize = true;
            this.lblDataSource.Location = new System.Drawing.Point(13, 11);
            this.lblDataSource.Name = "lblDataSource";
            this.lblDataSource.Size = new System.Drawing.Size(74, 13);
            this.lblDataSource.TabIndex = 1;
            this.lblDataSource.Text = "lblDataSource";
            // 
            // lblSiteName
            // 
            this.lblSiteName.AutoSize = true;
            this.lblSiteName.Location = new System.Drawing.Point(13, 31);
            this.lblSiteName.Name = "lblSiteName";
            this.lblSiteName.Size = new System.Drawing.Size(63, 13);
            this.lblSiteName.TabIndex = 2;
            this.lblSiteName.Text = "lblSiteName";
            // 
            // lblValueCount
            // 
            this.lblValueCount.AutoSize = true;
            this.lblValueCount.Location = new System.Drawing.Point(13, 51);
            this.lblValueCount.Name = "lblValueCount";
            this.lblValueCount.Size = new System.Drawing.Size(72, 13);
            this.lblValueCount.TabIndex = 3;
            this.lblValueCount.Text = "lblValueCount";
            // 
            // lblDownloadData
            // 
            this.lblDownloadData.AutoSize = true;
            this.lblDownloadData.Location = new System.Drawing.Point(13, 89);
            this.lblDownloadData.Name = "lblDownloadData";
            this.lblDownloadData.Size = new System.Drawing.Size(81, 13);
            this.lblDownloadData.TabIndex = 4;
            this.lblDownloadData.TabStop = true;
            this.lblDownloadData.Text = "Download Data";
            // 
            // CustomToolTipControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblDownloadData);
            this.Controls.Add(this.lblValueCount);
            this.Controls.Add(this.lblSiteName);
            this.Controls.Add(this.lblDataSource);
            this.Controls.Add(this.lblServiceDesciptionUrl);
            this.Name = "CustomToolTipControl";
            this.Size = new System.Drawing.Size(132, 113);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel lblServiceDesciptionUrl;
        private System.Windows.Forms.Label lblDataSource;
        private System.Windows.Forms.Label lblSiteName;
        private System.Windows.Forms.Label lblValueCount;
        private System.Windows.Forms.LinkLabel lblDownloadData;
    }
}
