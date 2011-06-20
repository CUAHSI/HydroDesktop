namespace HydroDesktop.ImportExport
{
    partial class ExportDataTableToTextFileDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportDataTableToTextFileDialog));
            this.gbxFields = new System.Windows.Forms.GroupBox();
            this.btnSelectNone = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.clbFieldsToExport = new System.Windows.Forms.CheckedListBox();
            this.gbxDelimiter = new System.Windows.Forms.GroupBox();
            this.rdoComma = new System.Windows.Forms.RadioButton();
            this.rdoTab = new System.Windows.Forms.RadioButton();
            this.rdoSpace = new System.Windows.Forms.RadioButton();
            this.rdoPipe = new System.Windows.Forms.RadioButton();
            this.rdoSemicolon = new System.Windows.Forms.RadioButton();
            this.rdoOthers = new System.Windows.Forms.RadioButton();
            this.tbOther = new System.Windows.Forms.TextBox();
            this.gbxExport = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.tbOutputFilename = new System.Windows.Forms.TextBox();
            this.gbxExportdata = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbxProgresses = new System.Windows.Forms.GroupBox();
            this.btnPgsCancel = new System.Windows.Forms.Button();
            this.pgsBar = new System.Windows.Forms.ProgressBar();
            this.bgwMain = new System.ComponentModel.BackgroundWorker();
            this.gbxFields.SuspendLayout();
            this.gbxDelimiter.SuspendLayout();
            this.gbxExport.SuspendLayout();
            this.gbxExportdata.SuspendLayout();
            this.gbxProgresses.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxFields
            // 
            this.gbxFields.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxFields.Controls.Add(this.btnSelectNone);
            this.gbxFields.Controls.Add(this.btnSelectAll);
            this.gbxFields.Controls.Add(this.clbFieldsToExport);
            this.gbxFields.Location = new System.Drawing.Point(12, 12);
            this.gbxFields.Name = "gbxFields";
            this.gbxFields.Size = new System.Drawing.Size(320, 157);
            this.gbxFields.TabIndex = 6;
            this.gbxFields.TabStop = false;
            this.gbxFields.Text = "Select Fields to Export";
            // 
            // btnSelectNone
            // 
            this.btnSelectNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectNone.Location = new System.Drawing.Point(87, 122);
            this.btnSelectNone.Name = "btnSelectNone";
            this.btnSelectNone.Size = new System.Drawing.Size(75, 23);
            this.btnSelectNone.TabIndex = 8;
            this.btnSelectNone.Text = "Select &None";
            this.btnSelectNone.UseVisualStyleBackColor = true;
            this.btnSelectNone.Click += new System.EventHandler(this.SelectNone_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectAll.Location = new System.Drawing.Point(6, 122);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 7;
            this.btnSelectAll.Text = "Select &All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.SelectAll_Click);
            // 
            // clbFieldsToExport
            // 
            this.clbFieldsToExport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.clbFieldsToExport.CheckOnClick = true;
            this.clbFieldsToExport.FormattingEnabled = true;
            this.clbFieldsToExport.Location = new System.Drawing.Point(6, 19);
            this.clbFieldsToExport.Name = "clbFieldsToExport";
            this.clbFieldsToExport.Size = new System.Drawing.Size(308, 94);
            this.clbFieldsToExport.TabIndex = 6;
            this.clbFieldsToExport.ThreeDCheckBoxes = true;
            // 
            // gbxDelimiter
            // 
            this.gbxDelimiter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxDelimiter.Controls.Add(this.rdoComma);
            this.gbxDelimiter.Controls.Add(this.rdoTab);
            this.gbxDelimiter.Controls.Add(this.rdoSpace);
            this.gbxDelimiter.Controls.Add(this.rdoPipe);
            this.gbxDelimiter.Controls.Add(this.rdoSemicolon);
            this.gbxDelimiter.Controls.Add(this.rdoOthers);
            this.gbxDelimiter.Controls.Add(this.tbOther);
            this.gbxDelimiter.Location = new System.Drawing.Point(12, 175);
            this.gbxDelimiter.Name = "gbxDelimiter";
            this.gbxDelimiter.Size = new System.Drawing.Size(320, 74);
            this.gbxDelimiter.TabIndex = 10;
            this.gbxDelimiter.TabStop = false;
            this.gbxDelimiter.Text = "Select a Delimiter";
            // 
            // rdoComma
            // 
            this.rdoComma.AutoSize = true;
            this.rdoComma.Checked = true;
            this.rdoComma.Location = new System.Drawing.Point(11, 23);
            this.rdoComma.Name = "rdoComma";
            this.rdoComma.Size = new System.Drawing.Size(90, 17);
            this.rdoComma.TabIndex = 10;
            this.rdoComma.TabStop = true;
            this.rdoComma.Text = "&Comma (CSV)";
            this.rdoComma.UseVisualStyleBackColor = true;
            // 
            // rdoTab
            // 
            this.rdoTab.AutoSize = true;
            this.rdoTab.Location = new System.Drawing.Point(120, 23);
            this.rdoTab.Name = "rdoTab";
            this.rdoTab.Size = new System.Drawing.Size(44, 17);
            this.rdoTab.TabIndex = 11;
            this.rdoTab.TabStop = true;
            this.rdoTab.Text = "&Tab";
            this.rdoTab.UseVisualStyleBackColor = true;
            // 
            // rdoSpace
            // 
            this.rdoSpace.AutoSize = true;
            this.rdoSpace.Location = new System.Drawing.Point(213, 23);
            this.rdoSpace.Name = "rdoSpace";
            this.rdoSpace.Size = new System.Drawing.Size(56, 17);
            this.rdoSpace.TabIndex = 12;
            this.rdoSpace.TabStop = true;
            this.rdoSpace.Text = "&Space";
            this.rdoSpace.UseVisualStyleBackColor = true;
            // 
            // rdoPipe
            // 
            this.rdoPipe.AutoSize = true;
            this.rdoPipe.Location = new System.Drawing.Point(11, 41);
            this.rdoPipe.Name = "rdoPipe";
            this.rdoPipe.Size = new System.Drawing.Size(46, 17);
            this.rdoPipe.TabIndex = 13;
            this.rdoPipe.TabStop = true;
            this.rdoPipe.Text = "&Pipe";
            this.rdoPipe.UseVisualStyleBackColor = true;
            // 
            // rdoSemicolon
            // 
            this.rdoSemicolon.AutoSize = true;
            this.rdoSemicolon.Location = new System.Drawing.Point(120, 41);
            this.rdoSemicolon.Name = "rdoSemicolon";
            this.rdoSemicolon.Size = new System.Drawing.Size(74, 17);
            this.rdoSemicolon.TabIndex = 14;
            this.rdoSemicolon.TabStop = true;
            this.rdoSemicolon.Text = "Se&micolon";
            this.rdoSemicolon.UseVisualStyleBackColor = true;
            // 
            // rdoOthers
            // 
            this.rdoOthers.AutoSize = true;
            this.rdoOthers.Location = new System.Drawing.Point(213, 41);
            this.rdoOthers.Name = "rdoOthers";
            this.rdoOthers.Size = new System.Drawing.Size(54, 17);
            this.rdoOthers.TabIndex = 15;
            this.rdoOthers.TabStop = true;
            this.rdoOthers.Text = "&Other:";
            this.rdoOthers.UseVisualStyleBackColor = true;
            // 
            // tbOther
            // 
            this.tbOther.Location = new System.Drawing.Point(273, 38);
            this.tbOther.MaxLength = 1;
            this.tbOther.Name = "tbOther";
            this.tbOther.Size = new System.Drawing.Size(27, 20);
            this.tbOther.TabIndex = 16;
            this.tbOther.TextChanged += new System.EventHandler(this.other_TextChanged);
            // 
            // gbxExport
            // 
            this.gbxExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxExport.Controls.Add(this.btnBrowse);
            this.gbxExport.Controls.Add(this.tbOutputFilename);
            this.gbxExport.Location = new System.Drawing.Point(12, 255);
            this.gbxExport.Name = "gbxExport";
            this.gbxExport.Size = new System.Drawing.Size(320, 52);
            this.gbxExport.TabIndex = 18;
            this.gbxExport.TabStop = false;
            this.gbxExport.Text = "Specify Output File";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(251, 17);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(63, 23);
            this.btnBrowse.TabIndex = 19;
            this.btnBrowse.Text = "&Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // tbOutputFilename
            // 
            this.tbOutputFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbOutputFilename.Location = new System.Drawing.Point(6, 19);
            this.tbOutputFilename.Name = "tbOutputFilename";
            this.tbOutputFilename.Size = new System.Drawing.Size(235, 20);
            this.tbOutputFilename.TabIndex = 18;
            // 
            // gbxExportdata
            // 
            this.gbxExportdata.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxExportdata.Controls.Add(this.btnOK);
            this.gbxExportdata.Controls.Add(this.btnCancel);
            this.gbxExportdata.Location = new System.Drawing.Point(12, 317);
            this.gbxExportdata.Name = "gbxExportdata";
            this.gbxExportdata.Size = new System.Drawing.Size(320, 46);
            this.gbxExportdata.TabIndex = 21;
            this.gbxExportdata.TabStop = false;
            this.gbxExportdata.Text = "Export Data";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(182, 16);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(63, 23);
            this.btnOK.TabIndex = 21;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(251, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(63, 23);
            this.btnCancel.TabIndex = 22;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbxProgresses
            // 
            this.gbxProgresses.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxProgresses.Controls.Add(this.btnPgsCancel);
            this.gbxProgresses.Controls.Add(this.pgsBar);
            this.gbxProgresses.Location = new System.Drawing.Point(12, 317);
            this.gbxProgresses.Name = "gbxProgresses";
            this.gbxProgresses.Size = new System.Drawing.Size(320, 46);
            this.gbxProgresses.TabIndex = 23;
            this.gbxProgresses.TabStop = false;
            this.gbxProgresses.Text = "Processing...";
            this.gbxProgresses.Visible = false;
            // 
            // btnPgsCancel
            // 
            this.btnPgsCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPgsCancel.Location = new System.Drawing.Point(243, 19);
            this.btnPgsCancel.Name = "btnPgsCancel";
            this.btnPgsCancel.Size = new System.Drawing.Size(65, 21);
            this.btnPgsCancel.TabIndex = 0;
            this.btnPgsCancel.Text = "Cancel";
            this.btnPgsCancel.UseVisualStyleBackColor = true;
            this.btnPgsCancel.Click += new System.EventHandler(this.pgsCancel_Click);
            // 
            // pgsBar
            // 
            this.pgsBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pgsBar.Location = new System.Drawing.Point(9, 18);
            this.pgsBar.Name = "pgsBar";
            this.pgsBar.Size = new System.Drawing.Size(230, 21);
            this.pgsBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pgsBar.TabIndex = 1;
            // 
            // bgwMain
            // 
            this.bgwMain.WorkerReportsProgress = true;
            this.bgwMain.WorkerSupportsCancellation = true;
            this.bgwMain.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMain_DoWork);
            this.bgwMain.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMain_RunWorkerCompleted);
            this.bgwMain.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgwMain_ProgressChanged);
            // 
            // ExportDataTableToTextFileDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 375);
            this.Controls.Add(this.gbxExport);
            this.Controls.Add(this.gbxDelimiter);
            this.Controls.Add(this.gbxFields);
            this.Controls.Add(this.gbxExportdata);
            this.Controls.Add(this.gbxProgresses);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(352, 409);
            this.Name = "ExportDataTableToTextFileDialog";
            this.Text = "Export To Text File";
            this.Load += new System.EventHandler(this.ListBox_load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExportDialog_FormClosing);
            this.gbxFields.ResumeLayout(false);
            this.gbxDelimiter.ResumeLayout(false);
            this.gbxDelimiter.PerformLayout();
            this.gbxExport.ResumeLayout(false);
            this.gbxExport.PerformLayout();
            this.gbxExportdata.ResumeLayout(false);
            this.gbxProgresses.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxFields;
        private System.Windows.Forms.Button btnSelectNone;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.CheckedListBox clbFieldsToExport;
        private System.Windows.Forms.GroupBox gbxDelimiter;
        private System.Windows.Forms.RadioButton rdoComma;
        private System.Windows.Forms.RadioButton rdoTab;
        private System.Windows.Forms.RadioButton rdoSpace;
        private System.Windows.Forms.RadioButton rdoPipe;
        private System.Windows.Forms.RadioButton rdoSemicolon;
        private System.Windows.Forms.RadioButton rdoOthers;
        private System.Windows.Forms.TextBox tbOther;
        private System.Windows.Forms.GroupBox gbxExport;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox tbOutputFilename;
        private System.Windows.Forms.GroupBox gbxExportdata;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbxProgresses;
        private System.Windows.Forms.Button btnPgsCancel;
        private System.Windows.Forms.ProgressBar pgsBar;
        private System.ComponentModel.BackgroundWorker bgwMain;
    }
}