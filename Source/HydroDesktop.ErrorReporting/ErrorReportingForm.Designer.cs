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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorReportingForm));
            this.lblInfo = new System.Windows.Forms.Label();
            this.paMain = new System.Windows.Forms.Panel();
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
            this.btnCopyError = new System.Windows.Forms.Button();
            this.btnZipLog = new System.Windows.Forms.Button();
            this.btnSendError = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.paProgress = new System.Windows.Forms.Panel();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.paMain.SuspendLayout();
            this.gbCredentials.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.paProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInfo.Location = new System.Drawing.Point(12, 9);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(399, 46);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "HydroDesktop has encountered an error. We are sorry for the inconvenience.\r\nWe ha" +
    "ve created an error report that you can send to us by clicking the “Send Error” " +
    "button below.";
            // 
            // paMain
            // 
            this.paMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.paMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paMain.Controls.Add(this.paProgress);
            this.paMain.Controls.Add(this.gbCredentials);
            this.paMain.Controls.Add(this.label6);
            this.paMain.Controls.Add(this.tbDescribe);
            this.paMain.Controls.Add(this.label4);
            this.paMain.Controls.Add(this.tbError);
            this.paMain.Location = new System.Drawing.Point(15, 58);
            this.paMain.Name = "paMain";
            this.paMain.Size = new System.Drawing.Size(393, 364);
            this.paMain.TabIndex = 1;
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
            this.gbCredentials.Location = new System.Drawing.Point(15, 261);
            this.gbCredentials.Name = "gbCredentials";
            this.gbCredentials.Size = new System.Drawing.Size(359, 89);
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
            this.tbLogin.Validating += new System.ComponentModel.CancelEventHandler(this.tbLogin_Validating);
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
            this.tbPassword.Validating += new System.ComponentModel.CancelEventHandler(this.tbPassword_Validating);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 130);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(300, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Describe what you were doing when error occurred (optional):";
            // 
            // tbDescribe
            // 
            this.tbDescribe.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDescribe.Location = new System.Drawing.Point(15, 146);
            this.tbDescribe.Multiline = true;
            this.tbDescribe.Name = "tbDescribe";
            this.tbDescribe.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbDescribe.Size = new System.Drawing.Size(361, 109);
            this.tbDescribe.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Error:";
            // 
            // tbError
            // 
            this.tbError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbError.Location = new System.Drawing.Point(15, 24);
            this.tbError.Multiline = true;
            this.tbError.Name = "tbError";
            this.tbError.ReadOnly = true;
            this.tbError.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbError.Size = new System.Drawing.Size(359, 94);
            this.tbError.TabIndex = 3;
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
            this.btnSendError.Location = new System.Drawing.Point(259, 428);
            this.btnSendError.Name = "btnSendError";
            this.btnSendError.Size = new System.Drawing.Size(149, 37);
            this.btnSendError.TabIndex = 4;
            this.btnSendError.Text = "Send Error";
            this.btnSendError.UseVisualStyleBackColor = true;
            this.btnSendError.Click += new System.EventHandler(this.btnSendError_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // paProgress
            // 
            this.paProgress.Controls.Add(this.pbProgress);
            this.paProgress.Location = new System.Drawing.Point(44, 159);
            this.paProgress.Name = "paProgress";
            this.paProgress.Size = new System.Drawing.Size(293, 54);
            this.paProgress.TabIndex = 14;
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(17, 15);
            this.pbProgress.MarqueeAnimationSpeed = 50;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(263, 23);
            this.pbProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbProgress.TabIndex = 0;
            // 
            // ErrorReportingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 475);
            this.Controls.Add(this.btnSendError);
            this.Controls.Add(this.btnZipLog);
            this.Controls.Add(this.btnCopyError);
            this.Controls.Add(this.paMain);
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
            this.paMain.ResumeLayout(false);
            this.paMain.PerformLayout();
            this.gbCredentials.ResumeLayout(false);
            this.gbCredentials.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.paProgress.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Panel paMain;
        private System.Windows.Forms.Button btnCopyError;
        private System.Windows.Forms.Button btnZipLog;
        private System.Windows.Forms.Button btnSendError;
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
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Panel paProgress;
        private System.Windows.Forms.ProgressBar pbProgress;
    }
}