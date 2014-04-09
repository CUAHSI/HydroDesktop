namespace FacetedSearch3.Settings.UI
{
    partial class SearchCatalogSettingsDialog
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
            this.gbHisCentralUrl = new System.Windows.Forms.GroupBox();
            this.txtCustomUrl = new System.Windows.Forms.TextBox();
            this.rbHisCentalCustom = new System.Windows.Forms.RadioButton();
            this.rbHisCentral2 = new System.Windows.Forms.RadioButton();
            this.rbHisCentral1 = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblHisCentralURL = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnRefreshServices = new System.Windows.Forms.Button();
            this.btnRefreshKeywords = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.rbHisCentral = new System.Windows.Forms.RadioButton();
            this.rbLocalMetadataCache = new System.Windows.Forms.RadioButton();
            this.gbHisCentralUrl.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbHisCentralUrl
            // 
            this.gbHisCentralUrl.Controls.Add(this.txtCustomUrl);
            this.gbHisCentralUrl.Controls.Add(this.rbHisCentalCustom);
            this.gbHisCentralUrl.Controls.Add(this.rbHisCentral2);
            this.gbHisCentralUrl.Controls.Add(this.rbHisCentral1);
            this.gbHisCentralUrl.Location = new System.Drawing.Point(12, 78);
            this.gbHisCentralUrl.Name = "gbHisCentralUrl";
            this.gbHisCentralUrl.Size = new System.Drawing.Size(321, 59);
            this.gbHisCentralUrl.TabIndex = 29;
            this.gbHisCentralUrl.TabStop = false;
            this.gbHisCentralUrl.Text = "Specify the HIS Central URL:";
            // 
            // txtCustomUrl
            // 
            this.txtCustomUrl.BackColor = System.Drawing.Color.Gainsboro;
            this.txtCustomUrl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCustomUrl.ForeColor = System.Drawing.Color.Gray;
            this.txtCustomUrl.Location = new System.Drawing.Point(3, 38);
            this.txtCustomUrl.Name = "txtCustomUrl";
            this.txtCustomUrl.Size = new System.Drawing.Size(314, 13);
            this.txtCustomUrl.TabIndex = 31;
            this.txtCustomUrl.Text = "Type-in the Custom URL here...";
            // 
            // rbHisCentalCustom
            // 
            this.rbHisCentalCustom.AutoSize = true;
            this.rbHisCentalCustom.Location = new System.Drawing.Point(187, 16);
            this.rbHisCentalCustom.Name = "rbHisCentalCustom";
            this.rbHisCentalCustom.Size = new System.Drawing.Size(60, 17);
            this.rbHisCentalCustom.TabIndex = 2;
            this.rbHisCentalCustom.Text = "Custom";
            this.rbHisCentalCustom.UseVisualStyleBackColor = true;
            // 
            // rbHisCentral2
            // 
            this.rbHisCentral2.AutoSize = true;
            this.rbHisCentral2.Location = new System.Drawing.Point(96, 16);
            this.rbHisCentral2.Name = "rbHisCentral2";
            this.rbHisCentral2.Size = new System.Drawing.Size(84, 17);
            this.rbHisCentral2.TabIndex = 1;
            this.rbHisCentral2.Text = "HIS central2";
            this.rbHisCentral2.UseVisualStyleBackColor = true;
            // 
            // rbHisCentral1
            // 
            this.rbHisCentral1.AutoSize = true;
            this.rbHisCentral1.Location = new System.Drawing.Point(5, 16);
            this.rbHisCentral1.Name = "rbHisCentral1";
            this.rbHisCentral1.Size = new System.Drawing.Size(85, 17);
            this.rbHisCentral1.TabIndex = 0;
            this.rbHisCentral1.Text = "HIS Central1";
            this.rbHisCentral1.UseVisualStyleBackColor = true;
            this.rbHisCentral1.CheckedChanged += new System.EventHandler(this.rbHisCentral_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(176, 211);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(257, 211);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblHisCentralURL
            // 
            this.lblHisCentralURL.AutoSize = true;
            this.lblHisCentralURL.Location = new System.Drawing.Point(14, 78);
            this.lblHisCentralURL.Name = "lblHisCentralURL";
            this.lblHisCentralURL.Size = new System.Drawing.Size(0, 13);
            this.lblHisCentralURL.TabIndex = 33;
            this.lblHisCentralURL.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnRefreshServices);
            this.groupBox2.Controls.Add(this.btnRefreshKeywords);
            this.groupBox2.Location = new System.Drawing.Point(11, 143);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(321, 62);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            // 
            // btnRefreshServices
            // 
            this.btnRefreshServices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshServices.Location = new System.Drawing.Point(165, 21);
            this.btnRefreshServices.Name = "btnRefreshServices";
            this.btnRefreshServices.Size = new System.Drawing.Size(142, 23);
            this.btnRefreshServices.TabIndex = 3;
            this.btnRefreshServices.Text = "Refresh Web Services";
            this.btnRefreshServices.UseVisualStyleBackColor = true;
            this.btnRefreshServices.Click += new System.EventHandler(this.btnRefreshServices_Click);
            // 
            // btnRefreshKeywords
            // 
            this.btnRefreshKeywords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshKeywords.Location = new System.Drawing.Point(15, 21);
            this.btnRefreshKeywords.Name = "btnRefreshKeywords";
            this.btnRefreshKeywords.Size = new System.Drawing.Size(120, 23);
            this.btnRefreshKeywords.TabIndex = 2;
            this.btnRefreshKeywords.Text = "Refresh Keywords";
            this.btnRefreshKeywords.UseVisualStyleBackColor = true;
            this.btnRefreshKeywords.Click += new System.EventHandler(this.btnRefreshKeywords_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 13);
            this.label2.TabIndex = 36;
            this.label2.Text = "Select type of catalog:";
            // 
            // rbHisCentral
            // 
            this.rbHisCentral.AutoSize = true;
            this.rbHisCentral.Location = new System.Drawing.Point(17, 41);
            this.rbHisCentral.Name = "rbHisCentral";
            this.rbHisCentral.Size = new System.Drawing.Size(79, 17);
            this.rbHisCentral.TabIndex = 0;
            this.rbHisCentral.TabStop = true;
            this.rbHisCentral.Text = "HIS Central";
            this.rbHisCentral.UseVisualStyleBackColor = true;
            // 
            // rbLocalMetadataCache
            // 
            this.rbLocalMetadataCache.AutoSize = true;
            this.rbLocalMetadataCache.Location = new System.Drawing.Point(121, 41);
            this.rbLocalMetadataCache.Name = "rbLocalMetadataCache";
            this.rbLocalMetadataCache.Size = new System.Drawing.Size(133, 17);
            this.rbLocalMetadataCache.TabIndex = 1;
            this.rbLocalMetadataCache.TabStop = true;
            this.rbLocalMetadataCache.Text = "Local Metadata Cache";
            this.rbLocalMetadataCache.UseVisualStyleBackColor = true;
            // 
            // SearchCatalogSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 245);
            this.Controls.Add(this.rbLocalMetadataCache);
            this.Controls.Add(this.rbHisCentral);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.lblHisCentralURL);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gbHisCentralUrl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchCatalogSettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Search Catalog";
            this.gbHisCentralUrl.ResumeLayout(false);
            this.gbHisCentralUrl.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.GroupBox gbHisCentralUrl;
        private System.Windows.Forms.RadioButton rbHisCentalCustom;
        private System.Windows.Forms.RadioButton rbHisCentral2;
        private System.Windows.Forms.RadioButton rbHisCentral1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Label lblHisCentralURL;
        public System.Windows.Forms.TextBox txtCustomUrl;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnRefreshServices;
        private System.Windows.Forms.Button btnRefreshKeywords;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rbHisCentral;
        private System.Windows.Forms.RadioButton rbLocalMetadataCache;
    }
}