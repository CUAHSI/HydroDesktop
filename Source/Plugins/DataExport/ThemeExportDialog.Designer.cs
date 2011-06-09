namespace HydroDesktop.ExportToCSV
{
    partial class ThemeExportDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager ( typeof ( ThemeExportDialog ) );
			this.cbVariable = new System.Windows.Forms.ComboBox ();
			this.lblVariable = new System.Windows.Forms.Label ();
			this.chkNodata = new System.Windows.Forms.CheckBox ();
			this.btnExport = new System.Windows.Forms.Button ();
			this.btncancel = new System.Windows.Forms.Button ();
			this.gbxProgress = new System.Windows.Forms.GroupBox ();
			this.btnPgsCancel = new System.Windows.Forms.Button ();
			this.pgsBar = new System.Windows.Forms.ProgressBar ();
			this.gbxExportData = new System.Windows.Forms.GroupBox ();
			this.bgwMain = new System.ComponentModel.BackgroundWorker ();
			this.gbxDelimiters = new System.Windows.Forms.GroupBox ();
			this.rdoComma = new System.Windows.Forms.RadioButton ();
			this.rdoTab = new System.Windows.Forms.RadioButton ();
			this.rdoSpace = new System.Windows.Forms.RadioButton ();
			this.rdoPipe = new System.Windows.Forms.RadioButton ();
			this.rdoSemicolon = new System.Windows.Forms.RadioButton ();
			this.rdoOthers = new System.Windows.Forms.RadioButton ();
			this.tbOther = new System.Windows.Forms.TextBox ();
			this.gbxExport = new System.Windows.Forms.GroupBox ();
			this.btnBrowse = new System.Windows.Forms.Button ();
			this.tbOutPutFileName = new System.Windows.Forms.TextBox ();
			this.gbxFields = new System.Windows.Forms.GroupBox ();
			this.btnSelectNone = new System.Windows.Forms.Button ();
			this.btnSelectAll = new System.Windows.Forms.Button ();
			this.clbExportItems = new System.Windows.Forms.CheckedListBox ();
			this.gbxThemes = new System.Windows.Forms.GroupBox ();
			this.gbxProgress.SuspendLayout ();
			this.gbxExportData.SuspendLayout ();
			this.gbxDelimiters.SuspendLayout ();
			this.gbxExport.SuspendLayout ();
			this.gbxFields.SuspendLayout ();
			this.gbxThemes.SuspendLayout ();
			this.SuspendLayout ();
			// 
			// cbVariable
			// 
			this.cbVariable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cbVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbVariable.FormattingEnabled = true;
			this.cbVariable.Location = new System.Drawing.Point ( 88, 22 );
			this.cbVariable.Name = "cbVariable";
			this.cbVariable.Size = new System.Drawing.Size ( 226, 21 );
			this.cbVariable.TabIndex = 2;
			// 
			// lblVariable
			// 
			this.lblVariable.AutoSize = true;
			this.lblVariable.Location = new System.Drawing.Point ( 8, 25 );
			this.lblVariable.Name = "lblVariable";
			this.lblVariable.Size = new System.Drawing.Size ( 74, 13 );
			this.lblVariable.TabIndex = 3;
			this.lblVariable.Text = "T&heme Name:";
			// 
			// chkNodata
			// 
			this.chkNodata.AutoSize = true;
			this.chkNodata.Location = new System.Drawing.Point ( 11, 49 );
			this.chkNodata.Name = "chkNodata";
			this.chkNodata.Size = new System.Drawing.Size ( 143, 17 );
			this.chkNodata.TabIndex = 4;
			this.chkNodata.Text = "&Include \'No Data\' Values";
			this.chkNodata.UseVisualStyleBackColor = true;
			// 
			// btnExport
			// 
			this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnExport.Location = new System.Drawing.Point ( 158, 17 );
			this.btnExport.Name = "btnExport";
			this.btnExport.Size = new System.Drawing.Size ( 75, 23 );
			this.btnExport.TabIndex = 21;
			this.btnExport.Text = "Export Data";
			this.btnExport.UseVisualStyleBackColor = true;
			this.btnExport.Click += new System.EventHandler ( this.btnExport_Click );
			// 
			// btncancel
			// 
			this.btncancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btncancel.Location = new System.Drawing.Point ( 239, 17 );
			this.btncancel.Name = "btncancel";
			this.btncancel.Size = new System.Drawing.Size ( 75, 23 );
			this.btncancel.TabIndex = 22;
			this.btncancel.Text = "Cancel";
			this.btncancel.UseVisualStyleBackColor = true;
			this.btncancel.Click += new System.EventHandler ( this.btnCancel_Click );
			// 
			// gbxProgress
			// 
			this.gbxProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbxProgress.Controls.Add ( this.btnPgsCancel );
			this.gbxProgress.Controls.Add ( this.pgsBar );
			this.gbxProgress.Location = new System.Drawing.Point ( 12, 390 );
			this.gbxProgress.Name = "gbxProgress";
			this.gbxProgress.Size = new System.Drawing.Size ( 320, 46 );
			this.gbxProgress.TabIndex = 21;
			this.gbxProgress.TabStop = false;
			this.gbxProgress.Text = "Processing...";
			this.gbxProgress.Visible = false;
			// 
			// btnPgsCancel
			// 
			this.btnPgsCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnPgsCancel.Location = new System.Drawing.Point ( 243, 19 );
			this.btnPgsCancel.Name = "btnPgsCancel";
			this.btnPgsCancel.Size = new System.Drawing.Size ( 65, 21 );
			this.btnPgsCancel.TabIndex = 0;
			this.btnPgsCancel.Text = "Cancel";
			this.btnPgsCancel.UseVisualStyleBackColor = true;
			this.btnPgsCancel.Click += new System.EventHandler ( this.pgsCancel_Click );
			// 
			// pgsBar
			// 
			this.pgsBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pgsBar.Location = new System.Drawing.Point ( 9, 18 );
			this.pgsBar.Name = "pgsBar";
			this.pgsBar.Size = new System.Drawing.Size ( 230, 21 );
			this.pgsBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.pgsBar.TabIndex = 1;
			// 
			// gbxExportData
			// 
			this.gbxExportData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbxExportData.Controls.Add ( this.btnExport );
			this.gbxExportData.Controls.Add ( this.btncancel );
			this.gbxExportData.Location = new System.Drawing.Point ( 12, 390 );
			this.gbxExportData.Name = "gbxExportData";
			this.gbxExportData.Size = new System.Drawing.Size ( 320, 46 );
			this.gbxExportData.TabIndex = 20;
			this.gbxExportData.TabStop = false;
			this.gbxExportData.Text = "Export Data";
			// 
			// bgwMain
			// 
			this.bgwMain.WorkerReportsProgress = true;
			this.bgwMain.WorkerSupportsCancellation = true;
			this.bgwMain.DoWork += new System.ComponentModel.DoWorkEventHandler ( this.bgwMain_DoWork );
			this.bgwMain.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler ( this.bgwMain_RunWorkerCompleted );
			this.bgwMain.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler ( this.bgwMain_ProgressChanged );
			// 
			// gbxDelimiters
			// 
			this.gbxDelimiters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbxDelimiters.Controls.Add ( this.rdoComma );
			this.gbxDelimiters.Controls.Add ( this.rdoTab );
			this.gbxDelimiters.Controls.Add ( this.rdoSpace );
			this.gbxDelimiters.Controls.Add ( this.rdoPipe );
			this.gbxDelimiters.Controls.Add ( this.rdoSemicolon );
			this.gbxDelimiters.Controls.Add ( this.rdoOthers );
			this.gbxDelimiters.Controls.Add ( this.tbOther );
			this.gbxDelimiters.Location = new System.Drawing.Point ( 12, 252 );
			this.gbxDelimiters.Name = "gbxDelimiters";
			this.gbxDelimiters.Size = new System.Drawing.Size ( 320, 74 );
			this.gbxDelimiters.TabIndex = 9;
			this.gbxDelimiters.TabStop = false;
			this.gbxDelimiters.Text = "Select a Delimiter";
			// 
			// rdoComma
			// 
			this.rdoComma.AutoSize = true;
			this.rdoComma.Checked = true;
			this.rdoComma.Location = new System.Drawing.Point ( 11, 23 );
			this.rdoComma.Name = "rdoComma";
			this.rdoComma.Size = new System.Drawing.Size ( 90, 17 );
			this.rdoComma.TabIndex = 10;
			this.rdoComma.TabStop = true;
			this.rdoComma.Text = "&Comma (CSV)";
			this.rdoComma.UseVisualStyleBackColor = true;
			// 
			// rdoTab
			// 
			this.rdoTab.AutoSize = true;
			this.rdoTab.Location = new System.Drawing.Point ( 116, 23 );
			this.rdoTab.Name = "rdoTab";
			this.rdoTab.Size = new System.Drawing.Size ( 44, 17 );
			this.rdoTab.TabIndex = 11;
			this.rdoTab.TabStop = true;
			this.rdoTab.Text = "&Tab";
			this.rdoTab.UseVisualStyleBackColor = true;
			// 
			// rdoSpace
			// 
			this.rdoSpace.AutoSize = true;
			this.rdoSpace.Location = new System.Drawing.Point ( 203, 23 );
			this.rdoSpace.Name = "rdoSpace";
			this.rdoSpace.Size = new System.Drawing.Size ( 56, 17 );
			this.rdoSpace.TabIndex = 12;
			this.rdoSpace.TabStop = true;
			this.rdoSpace.Text = "&Space";
			this.rdoSpace.UseVisualStyleBackColor = true;
			// 
			// rdoPipe
			// 
			this.rdoPipe.AutoSize = true;
			this.rdoPipe.Location = new System.Drawing.Point ( 11, 41 );
			this.rdoPipe.Name = "rdoPipe";
			this.rdoPipe.Size = new System.Drawing.Size ( 46, 17 );
			this.rdoPipe.TabIndex = 13;
			this.rdoPipe.TabStop = true;
			this.rdoPipe.Text = "&Pipe";
			this.rdoPipe.UseVisualStyleBackColor = true;
			// 
			// rdoSemicolon
			// 
			this.rdoSemicolon.AutoSize = true;
			this.rdoSemicolon.Location = new System.Drawing.Point ( 116, 41 );
			this.rdoSemicolon.Name = "rdoSemicolon";
			this.rdoSemicolon.Size = new System.Drawing.Size ( 74, 17 );
			this.rdoSemicolon.TabIndex = 14;
			this.rdoSemicolon.TabStop = true;
			this.rdoSemicolon.Text = "Se&micolon";
			this.rdoSemicolon.UseVisualStyleBackColor = true;
			// 
			// rdoOthers
			// 
			this.rdoOthers.AutoSize = true;
			this.rdoOthers.Location = new System.Drawing.Point ( 203, 41 );
			this.rdoOthers.Name = "rdoOthers";
			this.rdoOthers.Size = new System.Drawing.Size ( 54, 17 );
			this.rdoOthers.TabIndex = 15;
			this.rdoOthers.TabStop = true;
			this.rdoOthers.Text = "&Other:";
			this.rdoOthers.UseVisualStyleBackColor = true;
			// 
			// tbOther
			// 
			this.tbOther.Location = new System.Drawing.Point ( 269, 38 );
			this.tbOther.MaxLength = 1;
			this.tbOther.Name = "tbOther";
			this.tbOther.Size = new System.Drawing.Size ( 27, 20 );
			this.tbOther.TabIndex = 16;
			this.tbOther.TextChanged += new System.EventHandler ( this.other_TextChanged );
			// 
			// gbxExport
			// 
			this.gbxExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbxExport.Controls.Add ( this.btnBrowse );
			this.gbxExport.Controls.Add ( this.tbOutPutFileName );
			this.gbxExport.Location = new System.Drawing.Point ( 12, 332 );
			this.gbxExport.Name = "gbxExport";
			this.gbxExport.Size = new System.Drawing.Size ( 320, 52 );
			this.gbxExport.TabIndex = 17;
			this.gbxExport.TabStop = false;
			this.gbxExport.Text = "Specify Output File";
			// 
			// btnBrowse
			// 
			this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowse.Location = new System.Drawing.Point ( 251, 17 );
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size ( 63, 23 );
			this.btnBrowse.TabIndex = 19;
			this.btnBrowse.Text = "&Browse...";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler ( this.btnBrowse_Click );
			// 
			// tbOutPutFileName
			// 
			this.tbOutPutFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tbOutPutFileName.Location = new System.Drawing.Point ( 6, 19 );
			this.tbOutPutFileName.Name = "tbOutPutFileName";
			this.tbOutPutFileName.Size = new System.Drawing.Size ( 235, 20 );
			this.tbOutPutFileName.TabIndex = 18;
			// 
			// gbxFields
			// 
			this.gbxFields.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbxFields.Controls.Add ( this.btnSelectNone );
			this.gbxFields.Controls.Add ( this.btnSelectAll );
			this.gbxFields.Controls.Add ( this.clbExportItems );
			this.gbxFields.Location = new System.Drawing.Point ( 12, 89 );
			this.gbxFields.Name = "gbxFields";
			this.gbxFields.Size = new System.Drawing.Size ( 320, 157 );
			this.gbxFields.TabIndex = 5;
			this.gbxFields.TabStop = false;
			this.gbxFields.Text = "Select Fields to Export";
			// 
			// btnSelectNone
			// 
			this.btnSelectNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnSelectNone.Location = new System.Drawing.Point ( 87, 122 );
			this.btnSelectNone.Name = "btnSelectNone";
			this.btnSelectNone.Size = new System.Drawing.Size ( 75, 23 );
			this.btnSelectNone.TabIndex = 8;
			this.btnSelectNone.Text = "Select &None";
			this.btnSelectNone.UseVisualStyleBackColor = true;
			this.btnSelectNone.Click += new System.EventHandler ( this.SelectNone_Click );
			// 
			// btnSelectAll
			// 
			this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnSelectAll.Location = new System.Drawing.Point ( 6, 122 );
			this.btnSelectAll.Name = "btnSelectAll";
			this.btnSelectAll.Size = new System.Drawing.Size ( 75, 23 );
			this.btnSelectAll.TabIndex = 7;
			this.btnSelectAll.Text = "Select &All";
			this.btnSelectAll.UseVisualStyleBackColor = true;
			this.btnSelectAll.Click += new System.EventHandler ( this.SelectAll_Click );
			// 
			// clbExportItems
			// 
			this.clbExportItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.clbExportItems.CheckOnClick = true;
			this.clbExportItems.FormattingEnabled = true;
			this.clbExportItems.Location = new System.Drawing.Point ( 6, 19 );
			this.clbExportItems.Name = "clbExportItems";
			this.clbExportItems.Size = new System.Drawing.Size ( 308, 94 );
			this.clbExportItems.TabIndex = 6;
			this.clbExportItems.ThreeDCheckBoxes = true;
			// 
			// gbxThemes
			// 
			this.gbxThemes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbxThemes.Controls.Add ( this.cbVariable );
			this.gbxThemes.Controls.Add ( this.chkNodata );
			this.gbxThemes.Controls.Add ( this.lblVariable );
			this.gbxThemes.Location = new System.Drawing.Point ( 12, 9 );
			this.gbxThemes.Name = "gbxThemes";
			this.gbxThemes.Size = new System.Drawing.Size ( 320, 74 );
			this.gbxThemes.TabIndex = 1;
			this.gbxThemes.TabStop = false;
			this.gbxThemes.Text = "Select a Theme";
			// 
			// ThemeExportDialog
			// 
			this.AcceptButton = this.btnExport;
			this.AutoScaleDimensions = new System.Drawing.SizeF ( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size ( 344, 447 );
			this.Controls.Add ( this.gbxThemes );
			this.Controls.Add ( this.gbxFields );
			this.Controls.Add ( this.gbxExportData );
			this.Controls.Add ( this.gbxExport );
			this.Controls.Add ( this.gbxDelimiters );
			this.Controls.Add ( this.gbxProgress );
			this.HelpButton = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject ( "$this.Icon" )));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size ( 350, 479 );
			this.Name = "ThemeExportDialog";
			this.Text = "Export To Text File";
			this.Load += new System.EventHandler ( this.ExportDialog_load );
			this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler ( this.ThemeExportDialog_HelpButtonClicked );
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler ( this.ExportDialog_FormClosing );
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler ( this.ThemeExportDialog_HelpRequested );
			this.gbxProgress.ResumeLayout ( false );
			this.gbxExportData.ResumeLayout ( false );
			this.gbxDelimiters.ResumeLayout ( false );
			this.gbxDelimiters.PerformLayout ();
			this.gbxExport.ResumeLayout ( false );
			this.gbxExport.PerformLayout ();
			this.gbxFields.ResumeLayout ( false );
			this.gbxThemes.ResumeLayout ( false );
			this.gbxThemes.PerformLayout ();
			this.ResumeLayout ( false );

        }

        #endregion

        private System.Windows.Forms.ComboBox cbVariable;
        private System.Windows.Forms.Label lblVariable;
        private System.Windows.Forms.CheckBox chkNodata;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btncancel;
        private System.Windows.Forms.GroupBox gbxExportData;
        private System.Windows.Forms.GroupBox gbxProgress;
        private System.Windows.Forms.Button btnPgsCancel;
        private System.Windows.Forms.ProgressBar pgsBar;
        private System.ComponentModel.BackgroundWorker bgwMain;
        private System.Windows.Forms.GroupBox gbxDelimiters;
        private System.Windows.Forms.RadioButton rdoComma;
        private System.Windows.Forms.RadioButton rdoTab;
        private System.Windows.Forms.RadioButton rdoSpace;
        private System.Windows.Forms.RadioButton rdoPipe;
        private System.Windows.Forms.RadioButton rdoSemicolon;
        private System.Windows.Forms.RadioButton rdoOthers;
        private System.Windows.Forms.TextBox tbOther;
        private System.Windows.Forms.GroupBox gbxExport;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox tbOutPutFileName;
        private System.Windows.Forms.GroupBox gbxFields;
        private System.Windows.Forms.Button btnSelectNone;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.CheckedListBox clbExportItems;
        private System.Windows.Forms.GroupBox gbxThemes;
    }
}