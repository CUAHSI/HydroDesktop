namespace DataImport.CommonPages
{
    partial class CreateSiteForm
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblSiteName = new System.Windows.Forms.Label();
            this.tbSiteName = new System.Windows.Forms.TextBox();
            this.lblLatitude = new System.Windows.Forms.Label();
            this.lblLongitude = new System.Windows.Forms.Label();
            this.nudLat = new System.Windows.Forms.NumericUpDown();
            this.nudLng = new System.Windows.Forms.NumericUpDown();
            this.tbSiteCode = new System.Windows.Forms.TextBox();
            this.lblSiteCode = new System.Windows.Forms.Label();
            this.nudElevation = new System.Windows.Forms.NumericUpDown();
            this.lblElevation = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudLat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLng)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudElevation)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(112, 217);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(203, 217);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblSiteName
            // 
            this.lblSiteName.AutoSize = true;
            this.lblSiteName.Location = new System.Drawing.Point(16, 34);
            this.lblSiteName.Name = "lblSiteName";
            this.lblSiteName.Size = new System.Drawing.Size(54, 13);
            this.lblSiteName.TabIndex = 2;
            this.lblSiteName.Text = "Site name";
            // 
            // tbSiteName
            // 
            this.tbSiteName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSiteName.Location = new System.Drawing.Point(76, 31);
            this.tbSiteName.Name = "tbSiteName";
            this.tbSiteName.Size = new System.Drawing.Size(201, 20);
            this.tbSiteName.TabIndex = 0;
            // 
            // lblLatitude
            // 
            this.lblLatitude.AutoSize = true;
            this.lblLatitude.Location = new System.Drawing.Point(16, 101);
            this.lblLatitude.Name = "lblLatitude";
            this.lblLatitude.Size = new System.Drawing.Size(45, 13);
            this.lblLatitude.TabIndex = 4;
            this.lblLatitude.Text = "Latitude";
            // 
            // lblLongitude
            // 
            this.lblLongitude.AutoSize = true;
            this.lblLongitude.Location = new System.Drawing.Point(16, 133);
            this.lblLongitude.Name = "lblLongitude";
            this.lblLongitude.Size = new System.Drawing.Size(54, 13);
            this.lblLongitude.TabIndex = 5;
            this.lblLongitude.Text = "Longitude";
            // 
            // nudLat
            // 
            this.nudLat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudLat.DecimalPlaces = 4;
            this.nudLat.Location = new System.Drawing.Point(76, 99);
            this.nudLat.Name = "nudLat";
            this.nudLat.Size = new System.Drawing.Size(201, 20);
            this.nudLat.TabIndex = 2;
            // 
            // nudLng
            // 
            this.nudLng.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudLng.DecimalPlaces = 4;
            this.nudLng.Location = new System.Drawing.Point(76, 131);
            this.nudLng.Name = "nudLng";
            this.nudLng.Size = new System.Drawing.Size(201, 20);
            this.nudLng.TabIndex = 3;
            // 
            // tbSiteCode
            // 
            this.tbSiteCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSiteCode.Location = new System.Drawing.Point(76, 63);
            this.tbSiteCode.Name = "tbSiteCode";
            this.tbSiteCode.Size = new System.Drawing.Size(201, 20);
            this.tbSiteCode.TabIndex = 1;
            // 
            // lblSiteCode
            // 
            this.lblSiteCode.AutoSize = true;
            this.lblSiteCode.Location = new System.Drawing.Point(16, 66);
            this.lblSiteCode.Name = "lblSiteCode";
            this.lblSiteCode.Size = new System.Drawing.Size(52, 13);
            this.lblSiteCode.TabIndex = 8;
            this.lblSiteCode.Text = "Site code";
            // 
            // nudElevation
            // 
            this.nudElevation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudElevation.DecimalPlaces = 4;
            this.nudElevation.Location = new System.Drawing.Point(76, 166);
            this.nudElevation.Name = "nudElevation";
            this.nudElevation.Size = new System.Drawing.Size(202, 20);
            this.nudElevation.TabIndex = 4;
            // 
            // lblElevation
            // 
            this.lblElevation.AutoSize = true;
            this.lblElevation.Location = new System.Drawing.Point(16, 168);
            this.lblElevation.Name = "lblElevation";
            this.lblElevation.Size = new System.Drawing.Size(51, 13);
            this.lblElevation.TabIndex = 10;
            this.lblElevation.Text = "Elevation";
            // 
            // CreateSiteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 259);
            this.Controls.Add(this.nudElevation);
            this.Controls.Add(this.lblElevation);
            this.Controls.Add(this.tbSiteCode);
            this.Controls.Add(this.lblSiteCode);
            this.Controls.Add(this.nudLng);
            this.Controls.Add(this.nudLat);
            this.Controls.Add(this.lblLongitude);
            this.Controls.Add(this.lblLatitude);
            this.Controls.Add(this.tbSiteName);
            this.Controls.Add(this.lblSiteName);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateSiteForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create new site";
            ((System.ComponentModel.ISupportInitialize)(this.nudLat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLng)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudElevation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblSiteName;
        private System.Windows.Forms.TextBox tbSiteName;
        private System.Windows.Forms.Label lblLatitude;
        private System.Windows.Forms.Label lblLongitude;
        private System.Windows.Forms.NumericUpDown nudLat;
        private System.Windows.Forms.NumericUpDown nudLng;
        private System.Windows.Forms.TextBox tbSiteCode;
        private System.Windows.Forms.Label lblSiteCode;
        private System.Windows.Forms.NumericUpDown nudElevation;
        private System.Windows.Forms.Label lblElevation;
    }
}