namespace HydroDesktop.Plugins.DataExport
{
    partial class ExportDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportDialog));
            this.btnExport = new System.Windows.Forms.Button();
            this.btncancel = new System.Windows.Forms.Button();
            this.gbxProgress = new System.Windows.Forms.GroupBox();
            this.pgsBar = new System.Windows.Forms.ProgressBar();
            this.bgwMain = new System.ComponentModel.BackgroundWorker();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpMainOptions = new System.Windows.Forms.TabPage();
            this.delimiterSelector1 = new HydroDesktop.Common.Controls.DelimiterSelector();
            this.gbxThemes = new System.Windows.Forms.GroupBox();
            this.btnSelectNoneThemes = new System.Windows.Forms.Button();
            this.btnSelectAllThemes = new System.Windows.Forms.Button();
            this.clbThemes = new System.Windows.Forms.CheckedListBox();
            this.chkNodata = new System.Windows.Forms.CheckBox();
            this.gbxFields = new System.Windows.Forms.GroupBox();
            this.btnSelectNoneFields = new System.Windows.Forms.Button();
            this.btnSelectAllFields = new System.Windows.Forms.Button();
            this.clbExportItems = new System.Windows.Forms.CheckedListBox();
            this.gbxExport = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.tbOutPutFileName = new System.Windows.Forms.TextBox();
            this.tpAdvancedOptions = new System.Windows.Forms.TabPage();
            this.gbxDatesRange = new System.Windows.Forms.GroupBox();
            this.dtpEndDateRange = new System.Windows.Forms.DateTimePicker();
            this.lblAndRange = new System.Windows.Forms.Label();
            this.chbUseDateRange = new System.Windows.Forms.CheckBox();
            this.cmbDateTimeColumns = new System.Windows.Forms.ComboBox();
            this.dtpStartDateRange = new System.Windows.Forms.DateTimePicker();
            this.gbxProgress.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tpMainOptions.SuspendLayout();
            this.gbxThemes.SuspendLayout();
            this.gbxFields.SuspendLayout();
            this.gbxExport.SuspendLayout();
            this.tpAdvancedOptions.SuspendLayout();
            this.gbxDatesRange.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(255, 524);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 28);
            this.btnExport.TabIndex = 21;
            this.btnExport.Text = "Export Data";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btncancel
            // 
            this.btncancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btncancel.Location = new System.Drawing.Point(336, 524);
            this.btncancel.Name = "btncancel";
            this.btncancel.Size = new System.Drawing.Size(75, 28);
            this.btncancel.TabIndex = 22;
            this.btncancel.Text = "Cancel";
            this.btncancel.UseVisualStyleBackColor = true;
            this.btncancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbxProgress
            // 
            this.gbxProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxProgress.Controls.Add(this.pgsBar);
            this.gbxProgress.Location = new System.Drawing.Point(12, 508);
            this.gbxProgress.Name = "gbxProgress";
            this.gbxProgress.Size = new System.Drawing.Size(237, 46);
            this.gbxProgress.TabIndex = 21;
            this.gbxProgress.TabStop = false;
            this.gbxProgress.Text = "Processing...";
            this.gbxProgress.Visible = false;
            // 
            // pgsBar
            // 
            this.pgsBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pgsBar.Location = new System.Drawing.Point(9, 18);
            this.pgsBar.Name = "pgsBar";
            this.pgsBar.Size = new System.Drawing.Size(222, 21);
            this.pgsBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pgsBar.TabIndex = 1;
            // 
            // bgwMain
            // 
            this.bgwMain.WorkerReportsProgress = true;
            this.bgwMain.WorkerSupportsCancellation = true;
            this.bgwMain.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMain_DoWork);
            this.bgwMain.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgwMain_ProgressChanged);
            this.bgwMain.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMain_RunWorkerCompleted);
            // 
            // tcMain
            // 
            this.tcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcMain.Controls.Add(this.tpMainOptions);
            this.tcMain.Controls.Add(this.tpAdvancedOptions);
            this.tcMain.Location = new System.Drawing.Point(12, 12);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(399, 488);
            this.tcMain.TabIndex = 11;
            // 
            // tpMainOptions
            // 
            this.tpMainOptions.Controls.Add(this.delimiterSelector1);
            this.tpMainOptions.Controls.Add(this.gbxThemes);
            this.tpMainOptions.Controls.Add(this.gbxFields);
            this.tpMainOptions.Controls.Add(this.gbxExport);
            this.tpMainOptions.Location = new System.Drawing.Point(4, 22);
            this.tpMainOptions.Name = "tpMainOptions";
            this.tpMainOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tpMainOptions.Size = new System.Drawing.Size(391, 462);
            this.tpMainOptions.TabIndex = 0;
            this.tpMainOptions.Text = "Main";
            this.tpMainOptions.UseVisualStyleBackColor = true;
            // 
            // delimiterSelector1
            // 
            this.delimiterSelector1.CurrentDelimiter = ",";
            this.delimiterSelector1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.delimiterSelector1.Location = new System.Drawing.Point(3, 329);
            this.delimiterSelector1.Name = "delimiterSelector1";
            this.delimiterSelector1.Size = new System.Drawing.Size(385, 78);
            this.delimiterSelector1.TabIndex = 18;
            // 
            // gbxThemes
            // 
            this.gbxThemes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxThemes.Controls.Add(this.btnSelectNoneThemes);
            this.gbxThemes.Controls.Add(this.btnSelectAllThemes);
            this.gbxThemes.Controls.Add(this.clbThemes);
            this.gbxThemes.Controls.Add(this.chkNodata);
            this.gbxThemes.Location = new System.Drawing.Point(3, 6);
            this.gbxThemes.Name = "gbxThemes";
            this.gbxThemes.Size = new System.Drawing.Size(379, 143);
            this.gbxThemes.TabIndex = 1;
            this.gbxThemes.TabStop = false;
            this.gbxThemes.Text = "Select Data Sites Layers";
            // 
            // btnSelectNoneThemes
            // 
            this.btnSelectNoneThemes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectNoneThemes.Location = new System.Drawing.Point(87, 104);
            this.btnSelectNoneThemes.Name = "btnSelectNoneThemes";
            this.btnSelectNoneThemes.Size = new System.Drawing.Size(75, 23);
            this.btnSelectNoneThemes.TabIndex = 10;
            this.btnSelectNoneThemes.Text = "Select &None";
            this.btnSelectNoneThemes.UseVisualStyleBackColor = true;
            this.btnSelectNoneThemes.Click += new System.EventHandler(this.btnSelectNoneThemes_Click);
            // 
            // btnSelectAllThemes
            // 
            this.btnSelectAllThemes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectAllThemes.Location = new System.Drawing.Point(6, 104);
            this.btnSelectAllThemes.Name = "btnSelectAllThemes";
            this.btnSelectAllThemes.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAllThemes.TabIndex = 9;
            this.btnSelectAllThemes.Text = "Select &All";
            this.btnSelectAllThemes.UseVisualStyleBackColor = true;
            this.btnSelectAllThemes.Click += new System.EventHandler(this.btnSelectAllThemes_Click);
            // 
            // clbThemes
            // 
            this.clbThemes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.clbThemes.FormattingEnabled = true;
            this.clbThemes.Location = new System.Drawing.Point(6, 19);
            this.clbThemes.Name = "clbThemes";
            this.clbThemes.Size = new System.Drawing.Size(367, 79);
            this.clbThemes.TabIndex = 5;
            // 
            // chkNodata
            // 
            this.chkNodata.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkNodata.AutoSize = true;
            this.chkNodata.Location = new System.Drawing.Point(230, 110);
            this.chkNodata.Name = "chkNodata";
            this.chkNodata.Size = new System.Drawing.Size(143, 17);
            this.chkNodata.TabIndex = 4;
            this.chkNodata.Text = "&Include \'No Data\' Values";
            this.chkNodata.UseVisualStyleBackColor = true;
            // 
            // gbxFields
            // 
            this.gbxFields.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxFields.Controls.Add(this.btnSelectNoneFields);
            this.gbxFields.Controls.Add(this.btnSelectAllFields);
            this.gbxFields.Controls.Add(this.clbExportItems);
            this.gbxFields.Location = new System.Drawing.Point(3, 152);
            this.gbxFields.Name = "gbxFields";
            this.gbxFields.Size = new System.Drawing.Size(379, 171);
            this.gbxFields.TabIndex = 5;
            this.gbxFields.TabStop = false;
            this.gbxFields.Text = "Select Fields to Export";
            // 
            // btnSelectNoneFields
            // 
            this.btnSelectNoneFields.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectNoneFields.Location = new System.Drawing.Point(87, 131);
            this.btnSelectNoneFields.Name = "btnSelectNoneFields";
            this.btnSelectNoneFields.Size = new System.Drawing.Size(75, 23);
            this.btnSelectNoneFields.TabIndex = 8;
            this.btnSelectNoneFields.Text = "Select &None";
            this.btnSelectNoneFields.UseVisualStyleBackColor = true;
            this.btnSelectNoneFields.Click += new System.EventHandler(this.SelectNone_Click);
            // 
            // btnSelectAllFields
            // 
            this.btnSelectAllFields.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectAllFields.Location = new System.Drawing.Point(6, 131);
            this.btnSelectAllFields.Name = "btnSelectAllFields";
            this.btnSelectAllFields.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAllFields.TabIndex = 7;
            this.btnSelectAllFields.Text = "Select &All";
            this.btnSelectAllFields.UseVisualStyleBackColor = true;
            this.btnSelectAllFields.Click += new System.EventHandler(this.SelectAll_Click);
            // 
            // clbExportItems
            // 
            this.clbExportItems.CheckOnClick = true;
            this.clbExportItems.FormattingEnabled = true;
            this.clbExportItems.Location = new System.Drawing.Point(3, 16);
            this.clbExportItems.Name = "clbExportItems";
            this.clbExportItems.Size = new System.Drawing.Size(373, 109);
            this.clbExportItems.TabIndex = 6;
            this.clbExportItems.ThreeDCheckBoxes = true;
            // 
            // gbxExport
            // 
            this.gbxExport.Controls.Add(this.btnBrowse);
            this.gbxExport.Controls.Add(this.tbOutPutFileName);
            this.gbxExport.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbxExport.Location = new System.Drawing.Point(3, 407);
            this.gbxExport.Name = "gbxExport";
            this.gbxExport.Size = new System.Drawing.Size(385, 52);
            this.gbxExport.TabIndex = 17;
            this.gbxExport.TabStop = false;
            this.gbxExport.Text = "Specify Output File";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(316, 17);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(63, 23);
            this.btnBrowse.TabIndex = 19;
            this.btnBrowse.Text = "&Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // tbOutPutFileName
            // 
            this.tbOutPutFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbOutPutFileName.Location = new System.Drawing.Point(6, 19);
            this.tbOutPutFileName.Name = "tbOutPutFileName";
            this.tbOutPutFileName.Size = new System.Drawing.Size(300, 20);
            this.tbOutPutFileName.TabIndex = 18;
            // 
            // tpAdvancedOptions
            // 
            this.tpAdvancedOptions.Controls.Add(this.gbxDatesRange);
            this.tpAdvancedOptions.Location = new System.Drawing.Point(4, 22);
            this.tpAdvancedOptions.Name = "tpAdvancedOptions";
            this.tpAdvancedOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tpAdvancedOptions.Size = new System.Drawing.Size(391, 417);
            this.tpAdvancedOptions.TabIndex = 1;
            this.tpAdvancedOptions.Text = "Advanced options";
            this.tpAdvancedOptions.UseVisualStyleBackColor = true;
            // 
            // gbxDatesRange
            // 
            this.gbxDatesRange.Controls.Add(this.dtpEndDateRange);
            this.gbxDatesRange.Controls.Add(this.lblAndRange);
            this.gbxDatesRange.Controls.Add(this.chbUseDateRange);
            this.gbxDatesRange.Controls.Add(this.cmbDateTimeColumns);
            this.gbxDatesRange.Controls.Add(this.dtpStartDateRange);
            this.gbxDatesRange.Location = new System.Drawing.Point(6, 6);
            this.gbxDatesRange.Name = "gbxDatesRange";
            this.gbxDatesRange.Size = new System.Drawing.Size(388, 102);
            this.gbxDatesRange.TabIndex = 6;
            this.gbxDatesRange.TabStop = false;
            this.gbxDatesRange.Text = "Dates range";
            // 
            // dtpEndDateRange
            // 
            this.dtpEndDateRange.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEndDateRange.Location = new System.Drawing.Point(226, 58);
            this.dtpEndDateRange.Name = "dtpEndDateRange";
            this.dtpEndDateRange.Size = new System.Drawing.Size(147, 20);
            this.dtpEndDateRange.TabIndex = 4;
            // 
            // lblAndRange
            // 
            this.lblAndRange.AutoSize = true;
            this.lblAndRange.Location = new System.Drawing.Point(160, 62);
            this.lblAndRange.Name = "lblAndRange";
            this.lblAndRange.Size = new System.Drawing.Size(60, 13);
            this.lblAndRange.TabIndex = 5;
            this.lblAndRange.Text = "<= AND <=";
            // 
            // chbUseDateRange
            // 
            this.chbUseDateRange.AutoSize = true;
            this.chbUseDateRange.Location = new System.Drawing.Point(7, 26);
            this.chbUseDateRange.Name = "chbUseDateRange";
            this.chbUseDateRange.Size = new System.Drawing.Size(124, 17);
            this.chbUseDateRange.TabIndex = 0;
            this.chbUseDateRange.Text = "Export data in range:";
            this.chbUseDateRange.UseVisualStyleBackColor = true;
            this.chbUseDateRange.CheckedChanged += new System.EventHandler(this.chbUseDateRange_CheckedChanged);
            // 
            // cmbDateTimeColumns
            // 
            this.cmbDateTimeColumns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDateTimeColumns.FormattingEnabled = true;
            this.cmbDateTimeColumns.Location = new System.Drawing.Point(137, 26);
            this.cmbDateTimeColumns.Name = "cmbDateTimeColumns";
            this.cmbDateTimeColumns.Size = new System.Drawing.Size(153, 21);
            this.cmbDateTimeColumns.TabIndex = 2;
            // 
            // dtpStartDateRange
            // 
            this.dtpStartDateRange.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStartDateRange.Location = new System.Drawing.Point(7, 58);
            this.dtpStartDateRange.Name = "dtpStartDateRange";
            this.dtpStartDateRange.Size = new System.Drawing.Size(147, 20);
            this.dtpStartDateRange.TabIndex = 3;
            // 
            // ExportDialog
            // 
            this.AcceptButton = this.btnExport;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 566);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.btncancel);
            this.Controls.Add(this.gbxProgress);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 479);
            this.Name = "ExportDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Export To Text File";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.ThemeExportDialog_HelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExportDialog_FormClosing);
            this.Load += new System.EventHandler(this.ExportDialog_load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ThemeExportDialog_HelpRequested);
            this.gbxProgress.ResumeLayout(false);
            this.tcMain.ResumeLayout(false);
            this.tpMainOptions.ResumeLayout(false);
            this.gbxThemes.ResumeLayout(false);
            this.gbxThemes.PerformLayout();
            this.gbxFields.ResumeLayout(false);
            this.gbxExport.ResumeLayout(false);
            this.gbxExport.PerformLayout();
            this.tpAdvancedOptions.ResumeLayout(false);
            this.gbxDatesRange.ResumeLayout(false);
            this.gbxDatesRange.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btncancel;
        private System.Windows.Forms.GroupBox gbxProgress;
        private System.Windows.Forms.ProgressBar pgsBar;
        private System.ComponentModel.BackgroundWorker bgwMain;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpAdvancedOptions;
        private System.Windows.Forms.TabPage tpMainOptions;
        private System.Windows.Forms.GroupBox gbxThemes;
        private System.Windows.Forms.Button btnSelectNoneThemes;
        private System.Windows.Forms.Button btnSelectAllThemes;
        private System.Windows.Forms.CheckedListBox clbThemes;
        private System.Windows.Forms.CheckBox chkNodata;
        private System.Windows.Forms.GroupBox gbxFields;
        private System.Windows.Forms.Button btnSelectNoneFields;
        private System.Windows.Forms.Button btnSelectAllFields;
        private System.Windows.Forms.CheckedListBox clbExportItems;
        private System.Windows.Forms.GroupBox gbxExport;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox tbOutPutFileName;
        private System.Windows.Forms.DateTimePicker dtpEndDateRange;
        private System.Windows.Forms.DateTimePicker dtpStartDateRange;
        private System.Windows.Forms.ComboBox cmbDateTimeColumns;
        private System.Windows.Forms.CheckBox chbUseDateRange;
        private System.Windows.Forms.Label lblAndRange;
        private System.Windows.Forms.GroupBox gbxDatesRange;
        private Common.Controls.DelimiterSelector delimiterSelector1;
    }
}