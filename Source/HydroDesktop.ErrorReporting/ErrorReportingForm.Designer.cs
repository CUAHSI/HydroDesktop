namespace HydroDesktop.ErrorReporting
{
    partial class ErrorReportingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorReportingForm));
            this.lblInfo = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.tbDescribe = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbEmail = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbError = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.hdLink = new System.Windows.Forms.LinkLabel();
            this.btnCopyError = new System.Windows.Forms.Button();
            this.btnZipLog = new System.Windows.Forms.Button();
            this.btnSendError = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInfo.Location = new System.Drawing.Point(12, 9);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(372, 29);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "HydroDesktop has encountered an error. We are sorry for the inconvenience.";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.tbDescribe);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.tbEmail);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.tbError);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.hdLink);
            this.panel1.Location = new System.Drawing.Point(15, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(366, 381);
            this.panel1.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 269);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(300, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Describe what you where doing when error occured (optional):";
            // 
            // tbDescribe
            // 
            this.tbDescribe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDescribe.Location = new System.Drawing.Point(15, 288);
            this.tbDescribe.Multiline = true;
            this.tbDescribe.Name = "tbDescribe";
            this.tbDescribe.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbDescribe.Size = new System.Drawing.Size(334, 80);
            this.tbDescribe.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 221);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(145, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Your email address (optional):";
            // 
            // tbEmail
            // 
            this.tbEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbEmail.Location = new System.Drawing.Point(17, 237);
            this.tbEmail.Name = "tbEmail";
            this.tbEmail.Size = new System.Drawing.Size(332, 20);
            this.tbEmail.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Error:";
            // 
            // tbError
            // 
            this.tbError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbError.Location = new System.Drawing.Point(17, 119);
            this.tbError.Multiline = true;
            this.tbError.Name = "tbError";
            this.tbError.ReadOnly = true;
            this.tbError.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbError.Size = new System.Drawing.Size(332, 94);
            this.tbError.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(335, 69);
            this.label2.TabIndex = 1;
            this.label2.Text = "We have created an error report that you can send to us by clicking the “Send Err" +
    "or” button below.\r\n\r\nAlternatively you may use the online issue tracker to enter" +
    " your error here:";
            // 
            // hdLink
            // 
            this.hdLink.AutoSize = true;
            this.hdLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.hdLink.Location = new System.Drawing.Point(18, 78);
            this.hdLink.Name = "hdLink";
            this.hdLink.Size = new System.Drawing.Size(329, 17);
            this.hdLink.TabIndex = 0;
            this.hdLink.TabStop = true;
            this.hdLink.Text = "http://hydrodesktop.codeplex.com/Wo​rkItem/Create";
            // 
            // btnCopyError
            // 
            this.btnCopyError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyError.Location = new System.Drawing.Point(100, 428);
            this.btnCopyError.Name = "btnCopyError";
            this.btnCopyError.Size = new System.Drawing.Size(78, 37);
            this.btnCopyError.TabIndex = 2;
            this.btnCopyError.Text = "Copy Error to clipboard";
            this.btnCopyError.UseVisualStyleBackColor = true;
            this.btnCopyError.Click += new System.EventHandler(this.btnCopyError_Click);
            // 
            // btnZipLog
            // 
            this.btnZipLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZipLog.Location = new System.Drawing.Point(184, 428);
            this.btnZipLog.Name = "btnZipLog";
            this.btnZipLog.Size = new System.Drawing.Size(75, 37);
            this.btnZipLog.TabIndex = 3;
            this.btnZipLog.Text = "Zip log file";
            this.btnZipLog.UseVisualStyleBackColor = true;
            this.btnZipLog.Click += new System.EventHandler(this.btnZipLog_Click);
            // 
            // btnSendError
            // 
            this.btnSendError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendError.Enabled = false;
            this.btnSendError.Location = new System.Drawing.Point(276, 428);
            this.btnSendError.Name = "btnSendError";
            this.btnSendError.Size = new System.Drawing.Size(106, 37);
            this.btnSendError.TabIndex = 4;
            this.btnSendError.Text = "Send Error";
            this.btnSendError.UseVisualStyleBackColor = true;
            // 
            // ErrorReportingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 475);
            this.Controls.Add(this.btnSendError);
            this.Controls.Add(this.btnZipLog);
            this.Controls.Add(this.btnCopyError);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 500);
            this.Name = "ErrorReportingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HydroDesktop Error";
            this.Shown += new System.EventHandler(this.ErrorReportingForm_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCopyError;
        private System.Windows.Forms.Button btnZipLog;
        private System.Windows.Forms.Button btnSendError;
        private System.Windows.Forms.LinkLabel hdLink;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbDescribe;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbEmail;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbError;
    }
}