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
            this.gbCredentials = new System.Windows.Forms.GroupBox();
            this.linkRegister = new System.Windows.Forms.LinkLabel();
            this.tbLogin = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbDescribe = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbError = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCopyError = new System.Windows.Forms.Button();
            this.btnZipLog = new System.Windows.Forms.Button();
            this.btnSendError = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.gbCredentials.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInfo.Location = new System.Drawing.Point(12, 9);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(410, 29);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "HydroDesktop has encountered an error. We are sorry for the inconvenience.";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.gbCredentials);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.tbDescribe);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.tbError);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(15, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(404, 381);
            this.panel1.TabIndex = 1;
            // 
            // gbCredentials
            // 
            this.gbCredentials.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbCredentials.Controls.Add(this.linkRegister);
            this.gbCredentials.Controls.Add(this.tbLogin);
            this.gbCredentials.Controls.Add(this.label3);
            this.gbCredentials.Controls.Add(this.label1);
            this.gbCredentials.Controls.Add(this.tbPassword);
            this.gbCredentials.Location = new System.Drawing.Point(15, 278);
            this.gbCredentials.Name = "gbCredentials";
            this.gbCredentials.Size = new System.Drawing.Size(370, 89);
            this.gbCredentials.TabIndex = 13;
            this.gbCredentials.TabStop = false;
            this.gbCredentials.Text = "Codeplex";
            // 
            // linkRegister
            // 
            this.linkRegister.AutoSize = true;
            this.linkRegister.Location = new System.Drawing.Point(271, 22);
            this.linkRegister.Name = "linkRegister";
            this.linkRegister.Size = new System.Drawing.Size(79, 13);
            this.linkRegister.TabIndex = 13;
            this.linkRegister.TabStop = true;
            this.linkRegister.Text = "Not registered?";
            // 
            // tbLogin
            // 
            this.tbLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbLogin.Location = new System.Drawing.Point(69, 19);
            this.tbLogin.Name = "tbLogin";
            this.tbLogin.Size = new System.Drawing.Size(178, 20);
            this.tbLogin.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Password:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Login:";
            // 
            // tbPassword
            // 
            this.tbPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbPassword.Location = new System.Drawing.Point(69, 45);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(178, 20);
            this.tbPassword.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 169);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(300, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Describe what you where doing when error occured (optional):";
            // 
            // tbDescribe
            // 
            this.tbDescribe.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDescribe.Location = new System.Drawing.Point(15, 188);
            this.tbDescribe.Multiline = true;
            this.tbDescribe.Name = "tbDescribe";
            this.tbDescribe.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbDescribe.Size = new System.Drawing.Size(372, 84);
            this.tbDescribe.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Error:";
            // 
            // tbError
            // 
            this.tbError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbError.Location = new System.Drawing.Point(15, 60);
            this.tbError.Multiline = true;
            this.tbError.Name = "tbError";
            this.tbError.ReadOnly = true;
            this.tbError.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbError.Size = new System.Drawing.Size(370, 94);
            this.tbError.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(373, 39);
            this.label2.TabIndex = 1;
            this.label2.Text = "We have created an error report that you can send to us by clicking the “Send Err" +
    "or” button below.";
            // 
            // btnCopyError
            // 
            this.btnCopyError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopyError.Location = new System.Drawing.Point(16, 428);
            this.btnCopyError.Name = "btnCopyError";
            this.btnCopyError.Size = new System.Drawing.Size(78, 37);
            this.btnCopyError.TabIndex = 2;
            this.btnCopyError.Text = "Copy Error to clipboard";
            this.btnCopyError.UseVisualStyleBackColor = true;
            this.btnCopyError.Click += new System.EventHandler(this.btnCopyError_Click);
            // 
            // btnZipLog
            // 
            this.btnZipLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnZipLog.Location = new System.Drawing.Point(100, 428);
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
            this.btnSendError.Location = new System.Drawing.Point(313, 428);
            this.btnSendError.Name = "btnSendError";
            this.btnSendError.Size = new System.Drawing.Size(106, 37);
            this.btnSendError.TabIndex = 4;
            this.btnSendError.Text = "Send Error";
            this.btnSendError.UseVisualStyleBackColor = true;
            this.btnSendError.Click += new System.EventHandler(this.btnSendError_Click);
            // 
            // ErrorReportingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 475);
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
            this.gbCredentials.ResumeLayout(false);
            this.gbCredentials.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCopyError;
        private System.Windows.Forms.Button btnZipLog;
        private System.Windows.Forms.Button btnSendError;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbDescribe;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbError;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbLogin;
        private System.Windows.Forms.GroupBox gbCredentials;
        private System.Windows.Forms.LinkLabel linkRegister;
    }
}