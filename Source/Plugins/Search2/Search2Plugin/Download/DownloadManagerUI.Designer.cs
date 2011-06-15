namespace HydroDesktop.Search.Download
{
    partial class DownloadManagerUI
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.pbCurrent = new System.Windows.Forms.ProgressBar();
            this.pbTotal = new System.Windows.Forms.ProgressBar();
            this.lbOutput = new System.Windows.Forms.ListBox();
            this.lblCurrent = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblTotalInfo = new System.Windows.Forms.Label();
            this.lblCurrentInfo = new System.Windows.Forms.Label();
            this.btnHide = new System.Windows.Forms.Button();
            this.btnDetails = new System.Windows.Forms.Button();
            this.dgvDownloadData = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.paProgress = new System.Windows.Forms.Panel();
            this.paCommonButtons = new System.Windows.Forms.Panel();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.paWorkButtons = new System.Windows.Forms.Panel();
            this.btnSendError = new System.Windows.Forms.Button();
            this.btnShowFullLog = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDownloadData)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.paProgress.SuspendLayout();
            this.paCommonButtons.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.paWorkButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(5, 43);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pbCurrent
            // 
            this.pbCurrent.Location = new System.Drawing.Point(57, 14);
            this.pbCurrent.Name = "pbCurrent";
            this.pbCurrent.Size = new System.Drawing.Size(346, 23);
            this.pbCurrent.TabIndex = 1;
            // 
            // pbTotal
            // 
            this.pbTotal.Location = new System.Drawing.Point(57, 66);
            this.pbTotal.Name = "pbTotal";
            this.pbTotal.Size = new System.Drawing.Size(346, 23);
            this.pbTotal.TabIndex = 2;
            // 
            // lbOutput
            // 
            this.lbOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOutput.FormattingEnabled = true;
            this.lbOutput.Location = new System.Drawing.Point(3, 290);
            this.lbOutput.Name = "lbOutput";
            this.lbOutput.Size = new System.Drawing.Size(566, 134);
            this.lbOutput.TabIndex = 4;
            // 
            // lblCurrent
            // 
            this.lblCurrent.AutoSize = true;
            this.lblCurrent.Location = new System.Drawing.Point(8, 19);
            this.lblCurrent.Name = "lblCurrent";
            this.lblCurrent.Size = new System.Drawing.Size(44, 13);
            this.lblCurrent.TabIndex = 5;
            this.lblCurrent.Text = "Current:";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(8, 70);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(34, 13);
            this.lblTotal.TabIndex = 6;
            this.lblTotal.Text = "Total:";
            // 
            // lblTotalInfo
            // 
            this.lblTotalInfo.AutoSize = true;
            this.lblTotalInfo.Location = new System.Drawing.Point(54, 92);
            this.lblTotalInfo.Name = "lblTotalInfo";
            this.lblTotalInfo.Size = new System.Drawing.Size(59, 13);
            this.lblTotalInfo.TabIndex = 7;
            this.lblTotalInfo.Text = "lblTotalInfo";
            // 
            // lblCurrentInfo
            // 
            this.lblCurrentInfo.AutoSize = true;
            this.lblCurrentInfo.Location = new System.Drawing.Point(58, 40);
            this.lblCurrentInfo.Name = "lblCurrentInfo";
            this.lblCurrentInfo.Size = new System.Drawing.Size(69, 13);
            this.lblCurrentInfo.TabIndex = 8;
            this.lblCurrentInfo.Text = "lblCurrentInfo";
            // 
            // btnHide
            // 
            this.btnHide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHide.Location = new System.Drawing.Point(5, 14);
            this.btnHide.Name = "btnHide";
            this.btnHide.Size = new System.Drawing.Size(75, 23);
            this.btnHide.TabIndex = 9;
            this.btnHide.Text = "Hide";
            this.btnHide.UseVisualStyleBackColor = true;
            this.btnHide.Click += new System.EventHandler(this.btnHide_Click);
            // 
            // btnDetails
            // 
            this.btnDetails.Location = new System.Drawing.Point(9, 3);
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.Size = new System.Drawing.Size(75, 23);
            this.btnDetails.TabIndex = 10;
            this.btnDetails.Text = "Details...";
            this.btnDetails.UseVisualStyleBackColor = true;
            this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
            // 
            // dgvDownloadData
            // 
            this.dgvDownloadData.AllowUserToAddRows = false;
            this.dgvDownloadData.AllowUserToDeleteRows = false;
            this.dgvDownloadData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDownloadData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDownloadData.Location = new System.Drawing.Point(3, 137);
            this.dgvDownloadData.Name = "dgvDownloadData";
            this.dgvDownloadData.ReadOnly = true;
            this.dgvDownloadData.Size = new System.Drawing.Size(566, 109);
            this.dgvDownloadData.TabIndex = 11;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 89F));
            this.tableLayoutPanel1.Controls.Add(this.paProgress, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.paCommonButtons, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(566, 128);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // paProgress
            // 
            this.paProgress.Controls.Add(this.pbCurrent);
            this.paProgress.Controls.Add(this.lblCurrent);
            this.paProgress.Controls.Add(this.lblCurrentInfo);
            this.paProgress.Controls.Add(this.lblTotalInfo);
            this.paProgress.Controls.Add(this.pbTotal);
            this.paProgress.Controls.Add(this.lblTotal);
            this.paProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paProgress.Location = new System.Drawing.Point(3, 3);
            this.paProgress.Name = "paProgress";
            this.paProgress.Size = new System.Drawing.Size(471, 122);
            this.paProgress.TabIndex = 13;
            // 
            // paCommonButtons
            // 
            this.paCommonButtons.Controls.Add(this.btnCancel);
            this.paCommonButtons.Controls.Add(this.btnHide);
            this.paCommonButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paCommonButtons.Location = new System.Drawing.Point(480, 3);
            this.paCommonButtons.Name = "paCommonButtons";
            this.paCommonButtons.Size = new System.Drawing.Size(83, 122);
            this.paCommonButtons.TabIndex = 13;
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tlpMain.Controls.Add(this.lbOutput, 0, 3);
            this.tlpMain.Controls.Add(this.dgvDownloadData, 0, 1);
            this.tlpMain.Controls.Add(this.paWorkButtons, 0, 2);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 4;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 134F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45.45454F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 54.54546F));
            this.tlpMain.Size = new System.Drawing.Size(572, 427);
            this.tlpMain.TabIndex = 13;
            // 
            // paWorkButtons
            // 
            this.paWorkButtons.Controls.Add(this.btnSendError);
            this.paWorkButtons.Controls.Add(this.btnShowFullLog);
            this.paWorkButtons.Controls.Add(this.btnDetails);
            this.paWorkButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paWorkButtons.Location = new System.Drawing.Point(3, 252);
            this.paWorkButtons.Name = "paWorkButtons";
            this.paWorkButtons.Size = new System.Drawing.Size(566, 32);
            this.paWorkButtons.TabIndex = 13;
            // 
            // btnSendError
            // 
            this.btnSendError.Location = new System.Drawing.Point(173, 3);
            this.btnSendError.Name = "btnSendError";
            this.btnSendError.Size = new System.Drawing.Size(133, 23);
            this.btnSendError.TabIndex = 12;
            this.btnSendError.Text = "Send error to CUAHSI";
            this.btnSendError.UseVisualStyleBackColor = true;
            // 
            // btnShowFullLog
            // 
            this.btnShowFullLog.Location = new System.Drawing.Point(90, 3);
            this.btnShowFullLog.Name = "btnShowFullLog";
            this.btnShowFullLog.Size = new System.Drawing.Size(75, 23);
            this.btnShowFullLog.TabIndex = 11;
            this.btnShowFullLog.Text = "Full log...";
            this.btnShowFullLog.UseVisualStyleBackColor = true;
            // 
            // DownloadManagerUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 427);
            this.Controls.Add(this.tlpMain);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DownloadManagerUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Download Manager";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDownloadData)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.paProgress.ResumeLayout(false);
            this.paProgress.PerformLayout();
            this.paCommonButtons.ResumeLayout(false);
            this.tlpMain.ResumeLayout(false);
            this.paWorkButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar pbCurrent;
        private System.Windows.Forms.ProgressBar pbTotal;
        private System.Windows.Forms.ListBox lbOutput;
        private System.Windows.Forms.Label lblCurrent;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblTotalInfo;
        private System.Windows.Forms.Label lblCurrentInfo;
        private System.Windows.Forms.Button btnHide;
        private System.Windows.Forms.Button btnDetails;
        private System.Windows.Forms.DataGridView dgvDownloadData;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel paProgress;
        private System.Windows.Forms.Panel paCommonButtons;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Panel paWorkButtons;
        private System.Windows.Forms.Button btnShowFullLog;
        private System.Windows.Forms.Button btnSendError;
    }
}