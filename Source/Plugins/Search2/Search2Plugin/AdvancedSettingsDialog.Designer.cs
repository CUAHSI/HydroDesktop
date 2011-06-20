namespace HydroDesktop.Search
{
    partial class AdvancedSettingsDialog
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtCustomUrl = new System.Windows.Forms.TextBox();
            this.radioButton7 = new System.Windows.Forms.RadioButton();
            this.rbHisCentral2 = new System.Windows.Forms.RadioButton();
            this.rbHisCentral1 = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblHisCentralURL = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbOverwrite = new System.Windows.Forms.RadioButton();
            this.rbCopy = new System.Windows.Forms.RadioButton();
            this.rbAppend = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnRefreshServices = new System.Windows.Forms.Button();
            this.btnRefreshKeywords = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtCustomUrl);
            this.groupBox3.Controls.Add(this.radioButton7);
            this.groupBox3.Controls.Add(this.rbHisCentral2);
            this.groupBox3.Controls.Add(this.rbHisCentral1);
            this.groupBox3.Location = new System.Drawing.Point(12, 21);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(321, 46);
            this.groupBox3.TabIndex = 29;
            this.groupBox3.TabStop = false;
            // 
            // txtCustomUrl
            // 
            this.txtCustomUrl.BackColor = System.Drawing.Color.Gainsboro;
            this.txtCustomUrl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCustomUrl.ForeColor = System.Drawing.Color.Gray;
            this.txtCustomUrl.Location = new System.Drawing.Point(3, 28);
            this.txtCustomUrl.Name = "txtCustomUrl";
            this.txtCustomUrl.Size = new System.Drawing.Size(314, 13);
            this.txtCustomUrl.TabIndex = 31;
            this.txtCustomUrl.Text = "Type-in the Custom URL here...";
            // 
            // radioButton7
            // 
            this.radioButton7.AutoSize = true;
            this.radioButton7.Location = new System.Drawing.Point(187, 7);
            this.radioButton7.Name = "radioButton7";
            this.radioButton7.Size = new System.Drawing.Size(60, 17);
            this.radioButton7.TabIndex = 30;
            this.radioButton7.Text = "Custom";
            this.radioButton7.UseVisualStyleBackColor = true;
            this.radioButton7.CheckedChanged += new System.EventHandler(this.radioButton7_CheckedChanged);
            // 
            // rbHisCentral2
            // 
            this.rbHisCentral2.AutoSize = true;
            this.rbHisCentral2.Location = new System.Drawing.Point(96, 7);
            this.rbHisCentral2.Name = "rbHisCentral2";
            this.rbHisCentral2.Size = new System.Drawing.Size(84, 17);
            this.rbHisCentral2.TabIndex = 29;
            this.rbHisCentral2.Text = "HIS central2";
            this.rbHisCentral2.UseVisualStyleBackColor = true;
            this.rbHisCentral2.CheckedChanged += new System.EventHandler(this.rbHisCentral2_CheckedChanged);
            // 
            // rbHisCentral1
            // 
            this.rbHisCentral1.AutoSize = true;
            this.rbHisCentral1.Checked = true;
            this.rbHisCentral1.Location = new System.Drawing.Point(5, 7);
            this.rbHisCentral1.Name = "rbHisCentral1";
            this.rbHisCentral1.Size = new System.Drawing.Size(85, 17);
            this.rbHisCentral1.TabIndex = 28;
            this.rbHisCentral1.TabStop = true;
            this.rbHisCentral1.Text = "HIS Central1";
            this.rbHisCentral1.UseVisualStyleBackColor = true;
            this.rbHisCentral1.CheckedChanged += new System.EventHandler(this.rbHisCentral1_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(176, 246);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 30;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(257, 246);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 31;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 13);
            this.label1.TabIndex = 32;
            this.label1.Text = "Specify the HIS Central URL:";
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbOverwrite);
            this.groupBox1.Controls.Add(this.rbCopy);
            this.groupBox1.Controls.Add(this.rbAppend);
            this.groupBox1.Location = new System.Drawing.Point(12, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(321, 95);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data Download Options";
            // 
            // rbOverwrite
            // 
            this.rbOverwrite.AutoSize = true;
            this.rbOverwrite.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbOverwrite.Location = new System.Drawing.Point(8, 68);
            this.rbOverwrite.Name = "rbOverwrite";
            this.rbOverwrite.Size = new System.Drawing.Size(79, 17);
            this.rbOverwrite.TabIndex = 38;
            this.rbOverwrite.Text = "Overwrite";
            this.rbOverwrite.UseVisualStyleBackColor = true;
            // 
            // rbCopy
            // 
            this.rbCopy.AutoSize = true;
            this.rbCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbCopy.Location = new System.Drawing.Point(8, 45);
            this.rbCopy.Name = "rbCopy";
            this.rbCopy.Size = new System.Drawing.Size(53, 17);
            this.rbCopy.TabIndex = 37;
            this.rbCopy.Text = "Copy";
            this.rbCopy.UseVisualStyleBackColor = true;
            // 
            // rbAppend
            // 
            this.rbAppend.AutoSize = true;
            this.rbAppend.Checked = true;
            this.rbAppend.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbAppend.Location = new System.Drawing.Point(8, 22);
            this.rbAppend.Name = "rbAppend";
            this.rbAppend.Size = new System.Drawing.Size(68, 17);
            this.rbAppend.TabIndex = 36;
            this.rbAppend.TabStop = true;
            this.rbAppend.Text = "Append";
            this.rbAppend.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnRefreshServices);
            this.groupBox2.Controls.Add(this.btnRefreshKeywords);
            this.groupBox2.Location = new System.Drawing.Point(12, 190);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(321, 44);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            // 
            // btnRefreshServices
            // 
            this.btnRefreshServices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshServices.Location = new System.Drawing.Point(165, 15);
            this.btnRefreshServices.Name = "btnRefreshServices";
            this.btnRefreshServices.Size = new System.Drawing.Size(142, 20);
            this.btnRefreshServices.TabIndex = 37;
            this.btnRefreshServices.Text = "Refresh Web Services";
            this.btnRefreshServices.UseVisualStyleBackColor = true;
            this.btnRefreshServices.Click += new System.EventHandler(this.btnRefreshServices_Click);
            // 
            // btnRefreshKeywords
            // 
            this.btnRefreshKeywords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshKeywords.Location = new System.Drawing.Point(15, 15);
            this.btnRefreshKeywords.Name = "btnRefreshKeywords";
            this.btnRefreshKeywords.Size = new System.Drawing.Size(120, 20);
            this.btnRefreshKeywords.TabIndex = 36;
            this.btnRefreshKeywords.Text = "Refresh Keywords";
            this.btnRefreshKeywords.UseVisualStyleBackColor = true;
            this.btnRefreshKeywords.Click += new System.EventHandler(this.btnRefreshKeywords_Click);
            // 
            // AdvancedSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 274);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblHisCentralURL);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AdvancedSettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Advanced Settings";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButton7;
        private System.Windows.Forms.RadioButton rbHisCentral2;
        private System.Windows.Forms.RadioButton rbHisCentral1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label lblHisCentralURL;
        public System.Windows.Forms.TextBox txtCustomUrl;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbCopy;
        private System.Windows.Forms.RadioButton rbAppend;
        private System.Windows.Forms.RadioButton rbOverwrite;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnRefreshServices;
        private System.Windows.Forms.Button btnRefreshKeywords;
    }
}