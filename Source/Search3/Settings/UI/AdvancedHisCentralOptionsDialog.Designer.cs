namespace Search3.Settings.UI
{
    partial class AdvancedHisCentralOptionsDialog
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
            this.gbHisCentralUrl.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbHisCentralUrl
            // 
            this.gbHisCentralUrl.Controls.Add(this.txtCustomUrl);
            this.gbHisCentralUrl.Controls.Add(this.rbHisCentalCustom);
            this.gbHisCentralUrl.Controls.Add(this.rbHisCentral2);
            this.gbHisCentralUrl.Controls.Add(this.rbHisCentral1);
            this.gbHisCentralUrl.Location = new System.Drawing.Point(12, 16);
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
            this.btnOK.Location = new System.Drawing.Point(176, 95);
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
            this.btnCancel.Location = new System.Drawing.Point(257, 95);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // SearchCatalogSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 129);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gbHisCentralUrl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchCatalogSettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Advanced HIS Central Options";
            this.gbHisCentralUrl.ResumeLayout(false);
            this.gbHisCentralUrl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.GroupBox gbHisCentralUrl;
        private System.Windows.Forms.RadioButton rbHisCentalCustom;
        private System.Windows.Forms.RadioButton rbHisCentral2;
        private System.Windows.Forms.RadioButton rbHisCentral1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.TextBox txtCustomUrl;
    }
}