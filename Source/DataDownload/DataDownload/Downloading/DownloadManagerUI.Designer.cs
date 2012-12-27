namespace HydroDesktop.DataDownload.Downloading
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadManagerUI));
            this.pbTotal = new System.Windows.Forms.ProgressBar();
            this.lbOutput = new System.Windows.Forms.ListBox();
            this.lblTotalInfo = new System.Windows.Forms.Label();
            this.btnCancelClose = new System.Windows.Forms.Button();
            this.btnDetails = new System.Windows.Forms.Button();
            this.dgvDownloadData = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.paProgress = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.paProgressInformation = new System.Windows.Forms.Panel();
            this.lcEstimatedTimeInfo = new System.Windows.Forms.Label();
            this.lcEstimatedTime = new System.Windows.Forms.Label();
            this.lcTotalSeriesInfo = new System.Windows.Forms.Label();
            this.lcWithErrorInfo = new System.Windows.Forms.Label();
            this.lcDownloadedAndSavedInfo = new System.Windows.Forms.Label();
            this.lcRemainingSeriesInfo = new System.Windows.Forms.Label();
            this.lcRemaingSeries = new System.Windows.Forms.Label();
            this.lcTotalSeriesCount = new System.Windows.Forms.Label();
            this.lcSeriesWithError = new System.Windows.Forms.Label();
            this.lcDownloadedAndSaved = new System.Windows.Forms.Label();
            this.paCommonButtons = new System.Windows.Forms.Panel();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.paWorkButtons = new System.Windows.Forms.Panel();
            this.redownloadControl1 = new HydroDesktop.DataDownload.Downloading.RedownloadControl();
            this.chbAutoScroll = new System.Windows.Forms.CheckBox();
            this.btnCopyLog = new System.Windows.Forms.Button();
            this.btnSendError = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDownloadData)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.paProgress.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.paProgressInformation.SuspendLayout();
            this.paCommonButtons.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.paWorkButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbTotal
            // 
            this.pbTotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbTotal.Location = new System.Drawing.Point(3, 3);
            this.pbTotal.Name = "pbTotal";
            this.pbTotal.Size = new System.Drawing.Size(537, 24);
            this.pbTotal.TabIndex = 2;
            // 
            // lbOutput
            // 
            this.lbOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOutput.FormattingEnabled = true;
            this.lbOutput.Location = new System.Drawing.Point(3, 312);
            this.lbOutput.Name = "lbOutput";
            this.lbOutput.Size = new System.Drawing.Size(638, 140);
            this.lbOutput.TabIndex = 4;
            // 
            // lblTotalInfo
            // 
            this.lblTotalInfo.AutoSize = true;
            this.lblTotalInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTotalInfo.Location = new System.Drawing.Point(3, 30);
            this.lblTotalInfo.Name = "lblTotalInfo";
            this.lblTotalInfo.Size = new System.Drawing.Size(537, 19);
            this.lblTotalInfo.TabIndex = 7;
            this.lblTotalInfo.Text = "lblTotalInfo";
            // 
            // btnCancelClose
            // 
            this.btnCancelClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelClose.Location = new System.Drawing.Point(5, 10);
            this.btnCancelClose.Name = "btnCancelClose";
            this.btnCancelClose.Size = new System.Drawing.Size(75, 23);
            this.btnCancelClose.TabIndex = 9;
            this.btnCancelClose.Text = "Cancel";
            this.btnCancelClose.UseVisualStyleBackColor = true;
            this.btnCancelClose.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDetails
            // 
            this.btnDetails.Location = new System.Drawing.Point(9, 7);
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
            this.dgvDownloadData.Location = new System.Drawing.Point(3, 145);
            this.dgvDownloadData.Name = "dgvDownloadData";
            this.dgvDownloadData.ReadOnly = true;
            this.dgvDownloadData.Size = new System.Drawing.Size(638, 115);
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(638, 136);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // paProgress
            // 
            this.paProgress.Controls.Add(this.tableLayoutPanel2);
            this.paProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paProgress.Location = new System.Drawing.Point(3, 3);
            this.paProgress.Name = "paProgress";
            this.paProgress.Size = new System.Drawing.Size(543, 130);
            this.paProgress.TabIndex = 13;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.pbTotal, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblTotalInfo, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.paProgressInformation, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(543, 130);
            this.tableLayoutPanel2.TabIndex = 8;
            // 
            // paProgressInformation
            // 
            this.paProgressInformation.Controls.Add(this.lcEstimatedTimeInfo);
            this.paProgressInformation.Controls.Add(this.lcEstimatedTime);
            this.paProgressInformation.Controls.Add(this.lcTotalSeriesInfo);
            this.paProgressInformation.Controls.Add(this.lcWithErrorInfo);
            this.paProgressInformation.Controls.Add(this.lcDownloadedAndSavedInfo);
            this.paProgressInformation.Controls.Add(this.lcRemainingSeriesInfo);
            this.paProgressInformation.Controls.Add(this.lcRemaingSeries);
            this.paProgressInformation.Controls.Add(this.lcTotalSeriesCount);
            this.paProgressInformation.Controls.Add(this.lcSeriesWithError);
            this.paProgressInformation.Controls.Add(this.lcDownloadedAndSaved);
            this.paProgressInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paProgressInformation.Location = new System.Drawing.Point(3, 52);
            this.paProgressInformation.Name = "paProgressInformation";
            this.paProgressInformation.Size = new System.Drawing.Size(537, 75);
            this.paProgressInformation.TabIndex = 8;
            // 
            // lcEstimatedTimeInfo
            // 
            this.lcEstimatedTimeInfo.AutoSize = true;
            this.lcEstimatedTimeInfo.Location = new System.Drawing.Point(104, 52);
            this.lcEstimatedTimeInfo.Name = "lcEstimatedTimeInfo";
            this.lcEstimatedTimeInfo.Size = new System.Drawing.Size(35, 13);
            this.lcEstimatedTimeInfo.TabIndex = 12;
            this.lcEstimatedTimeInfo.Text = "label4";
            // 
            // lcEstimatedTime
            // 
            this.lcEstimatedTime.AutoSize = true;
            this.lcEstimatedTime.Location = new System.Drawing.Point(6, 52);
            this.lcEstimatedTime.Name = "lcEstimatedTime";
            this.lcEstimatedTime.Size = new System.Drawing.Size(78, 13);
            this.lcEstimatedTime.TabIndex = 11;
            this.lcEstimatedTime.Text = "Estimated time:";
            // 
            // lcTotalSeriesInfo
            // 
            this.lcTotalSeriesInfo.AutoSize = true;
            this.lcTotalSeriesInfo.Location = new System.Drawing.Point(104, 6);
            this.lcTotalSeriesInfo.Name = "lcTotalSeriesInfo";
            this.lcTotalSeriesInfo.Size = new System.Drawing.Size(35, 13);
            this.lcTotalSeriesInfo.TabIndex = 10;
            this.lcTotalSeriesInfo.Text = "label3";
            // 
            // lcWithErrorInfo
            // 
            this.lcWithErrorInfo.AutoSize = true;
            this.lcWithErrorInfo.Location = new System.Drawing.Point(283, 28);
            this.lcWithErrorInfo.Name = "lcWithErrorInfo";
            this.lcWithErrorInfo.Size = new System.Drawing.Size(35, 13);
            this.lcWithErrorInfo.TabIndex = 9;
            this.lcWithErrorInfo.Text = "label2";
            // 
            // lcDownloadedAndSavedInfo
            // 
            this.lcDownloadedAndSavedInfo.AutoSize = true;
            this.lcDownloadedAndSavedInfo.Location = new System.Drawing.Point(283, 6);
            this.lcDownloadedAndSavedInfo.Name = "lcDownloadedAndSavedInfo";
            this.lcDownloadedAndSavedInfo.Size = new System.Drawing.Size(35, 13);
            this.lcDownloadedAndSavedInfo.TabIndex = 8;
            this.lcDownloadedAndSavedInfo.Text = "label1";
            // 
            // lcRemainingSeriesInfo
            // 
            this.lcRemainingSeriesInfo.AutoSize = true;
            this.lcRemainingSeriesInfo.Location = new System.Drawing.Point(104, 29);
            this.lcRemainingSeriesInfo.Name = "lcRemainingSeriesInfo";
            this.lcRemainingSeriesInfo.Size = new System.Drawing.Size(35, 13);
            this.lcRemainingSeriesInfo.TabIndex = 7;
            this.lcRemainingSeriesInfo.Text = "label7";
            // 
            // lcRemaingSeries
            // 
            this.lcRemaingSeries.AutoSize = true;
            this.lcRemaingSeries.Location = new System.Drawing.Point(6, 29);
            this.lcRemaingSeries.Name = "lcRemaingSeries";
            this.lcRemaingSeries.Size = new System.Drawing.Size(90, 13);
            this.lcRemaingSeries.TabIndex = 6;
            this.lcRemaingSeries.Text = "Remaining series:";
            // 
            // lcTotalSeriesCount
            // 
            this.lcTotalSeriesCount.AutoSize = true;
            this.lcTotalSeriesCount.Location = new System.Drawing.Point(6, 6);
            this.lcTotalSeriesCount.Name = "lcTotalSeriesCount";
            this.lcTotalSeriesCount.Size = new System.Drawing.Size(64, 13);
            this.lcTotalSeriesCount.TabIndex = 4;
            this.lcTotalSeriesCount.Text = "Total series:";
            // 
            // lcSeriesWithError
            // 
            this.lcSeriesWithError.AutoSize = true;
            this.lcSeriesWithError.Location = new System.Drawing.Point(154, 28);
            this.lcSeriesWithError.Name = "lcSeriesWithError";
            this.lcSeriesWithError.Size = new System.Drawing.Size(56, 13);
            this.lcSeriesWithError.TabIndex = 2;
            this.lcSeriesWithError.Text = "With error:";
            // 
            // lcDownloadedAndSaved
            // 
            this.lcDownloadedAndSaved.AutoSize = true;
            this.lcDownloadedAndSaved.Location = new System.Drawing.Point(154, 6);
            this.lcDownloadedAndSaved.Name = "lcDownloadedAndSaved";
            this.lcDownloadedAndSaved.Size = new System.Drawing.Size(123, 13);
            this.lcDownloadedAndSaved.TabIndex = 0;
            this.lcDownloadedAndSaved.Text = "Downloaded and saved:";
            // 
            // paCommonButtons
            // 
            this.paCommonButtons.Controls.Add(this.btnCancelClose);
            this.paCommonButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paCommonButtons.Location = new System.Drawing.Point(552, 3);
            this.paCommonButtons.Name = "paCommonButtons";
            this.paCommonButtons.Size = new System.Drawing.Size(83, 130);
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
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 142F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45.45454F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 54.54546F));
            this.tlpMain.Size = new System.Drawing.Size(644, 455);
            this.tlpMain.TabIndex = 13;
            // 
            // paWorkButtons
            // 
            this.paWorkButtons.Controls.Add(this.redownloadControl1);
            this.paWorkButtons.Controls.Add(this.chbAutoScroll);
            this.paWorkButtons.Controls.Add(this.btnCopyLog);
            this.paWorkButtons.Controls.Add(this.btnSendError);
            this.paWorkButtons.Controls.Add(this.btnDetails);
            this.paWorkButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paWorkButtons.Location = new System.Drawing.Point(3, 266);
            this.paWorkButtons.Name = "paWorkButtons";
            this.paWorkButtons.Size = new System.Drawing.Size(638, 40);
            this.paWorkButtons.TabIndex = 13;
            // 
            // redownloadControl1
            // 
            this.redownloadControl1.Location = new System.Drawing.Point(254, 4);
            this.redownloadControl1.Name = "redownloadControl1";
            this.redownloadControl1.Size = new System.Drawing.Size(234, 30);
            this.redownloadControl1.TabIndex = 16;
            this.redownloadControl1.DoRedownload += new System.EventHandler<HydroDesktop.DataDownload.Downloading.RedownloadArgs>(this.redownloadControl1_DoRedownload);
            // 
            // chbAutoScroll
            // 
            this.chbAutoScroll.AutoSize = true;
            this.chbAutoScroll.Checked = true;
            this.chbAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbAutoScroll.Location = new System.Drawing.Point(494, 7);
            this.chbAutoScroll.Name = "chbAutoScroll";
            this.chbAutoScroll.Size = new System.Drawing.Size(75, 17);
            this.chbAutoScroll.TabIndex = 15;
            this.chbAutoScroll.Text = "Auto scroll";
            this.chbAutoScroll.UseVisualStyleBackColor = true;
            // 
            // btnCopyLog
            // 
            this.btnCopyLog.Location = new System.Drawing.Point(170, 7);
            this.btnCopyLog.Name = "btnCopyLog";
            this.btnCopyLog.Size = new System.Drawing.Size(78, 23);
            this.btnCopyLog.TabIndex = 13;
            this.btnCopyLog.Text = "Copy log";
            this.btnCopyLog.UseVisualStyleBackColor = true;
            this.btnCopyLog.Click += new System.EventHandler(this.btnCopyLog_Click);
            // 
            // btnSendError
            // 
            this.btnSendError.Location = new System.Drawing.Point(90, 7);
            this.btnSendError.Name = "btnSendError";
            this.btnSendError.Size = new System.Drawing.Size(74, 23);
            this.btnSendError.TabIndex = 12;
            this.btnSendError.Text = "Send error";
            this.btnSendError.UseVisualStyleBackColor = true;
            // 
            // DownloadManagerUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 455);
            this.Controls.Add(this.tlpMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(530, 270);
            this.Name = "DownloadManagerUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Download Manager";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDownloadData)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.paProgress.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.paProgressInformation.ResumeLayout(false);
            this.paProgressInformation.PerformLayout();
            this.paCommonButtons.ResumeLayout(false);
            this.tlpMain.ResumeLayout(false);
            this.paWorkButtons.ResumeLayout(false);
            this.paWorkButtons.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbTotal;
        private System.Windows.Forms.ListBox lbOutput;
        private System.Windows.Forms.Label lblTotalInfo;
        private System.Windows.Forms.Button btnCancelClose;
        private System.Windows.Forms.Button btnDetails;
        private System.Windows.Forms.DataGridView dgvDownloadData;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel paProgress;
        private System.Windows.Forms.Panel paCommonButtons;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.Panel paWorkButtons;
        private System.Windows.Forms.Button btnSendError;
        private System.Windows.Forms.Button btnCopyLog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel paProgressInformation;
        private System.Windows.Forms.Label lcRemainingSeriesInfo;
        private System.Windows.Forms.Label lcRemaingSeries;
        private System.Windows.Forms.Label lcTotalSeriesCount;
        private System.Windows.Forms.Label lcSeriesWithError;
        private System.Windows.Forms.Label lcDownloadedAndSaved;
        private System.Windows.Forms.Label lcTotalSeriesInfo;
        private System.Windows.Forms.Label lcWithErrorInfo;
        private System.Windows.Forms.Label lcDownloadedAndSavedInfo;
        private System.Windows.Forms.Label lcEstimatedTimeInfo;
        private System.Windows.Forms.Label lcEstimatedTime;
        private System.Windows.Forms.CheckBox chbAutoScroll;
        private RedownloadControl redownloadControl1;
    }
}