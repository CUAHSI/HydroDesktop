namespace HydroDesktop.Plugins.WaterML2Client
{
    partial class WMLDownloadForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbTimeSeriesUrl = new System.Windows.Forms.TextBox();
            this.lblVariable = new System.Windows.Forms.Label();
            this.cbTheme = new System.Windows.Forms.ComboBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.lblDownloading = new System.Windows.Forms.Label();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(203, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enter time series url or path to existing file:";
            // 
            // tbTimeSeriesUrl
            // 
            this.tbTimeSeriesUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTimeSeriesUrl.Location = new System.Drawing.Point(15, 43);
            this.tbTimeSeriesUrl.Name = "tbTimeSeriesUrl";
            this.tbTimeSeriesUrl.Size = new System.Drawing.Size(431, 20);
            this.tbTimeSeriesUrl.TabIndex = 2;
            // 
            // lblVariable
            // 
            this.lblVariable.AutoSize = true;
            this.lblVariable.Location = new System.Drawing.Point(12, 77);
            this.lblVariable.Name = "lblVariable";
            this.lblVariable.Size = new System.Drawing.Size(97, 13);
            this.lblVariable.TabIndex = 5;
            this.lblVariable.Text = "Add data to theme:";
            // 
            // cbTheme
            // 
            this.cbTheme.FormattingEnabled = true;
            this.cbTheme.Location = new System.Drawing.Point(15, 96);
            this.cbTheme.Name = "cbTheme";
            this.cbTheme.Size = new System.Drawing.Size(217, 21);
            this.cbTheme.TabIndex = 4;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(464, 179);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.Location = new System.Drawing.Point(355, 179);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(103, 23);
            this.btnImport.TabIndex = 7;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lblDownloading
            // 
            this.lblDownloading.AutoSize = true;
            this.lblDownloading.Location = new System.Drawing.Point(248, 23);
            this.lblDownloading.Name = "lblDownloading";
            this.lblDownloading.Size = new System.Drawing.Size(78, 13);
            this.lblDownloading.TabIndex = 1;
            this.lblDownloading.Text = "Downloading...";
            this.lblDownloading.Visible = false;
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(464, 43);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(75, 23);
            this.btnOpenFile.TabIndex = 8;
            this.btnOpenFile.Text = "Open....";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // WMLDownloadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(551, 214);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.lblDownloading);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblVariable);
            this.Controls.Add(this.cbTheme);
            this.Controls.Add(this.tbTimeSeriesUrl);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WMLDownloadForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Import from WaterML2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbTimeSeriesUrl;
        private System.Windows.Forms.Label lblVariable;
        private System.Windows.Forms.ComboBox cbTheme;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Label lblDownloading;
        private System.Windows.Forms.Button btnOpenFile;
    }
}