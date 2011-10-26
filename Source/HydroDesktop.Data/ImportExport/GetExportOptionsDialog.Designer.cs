namespace HydroDesktop.ImportExport
{
    partial class GetExportOptionsDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager ( typeof ( GetExportOptionsDialog ) );
			this.gbxExport = new System.Windows.Forms.GroupBox ();
			this.btnBrowse = new System.Windows.Forms.Button ();
			this.tbOutputFilename = new System.Windows.Forms.TextBox ();
			this.clbFieldsToExport = new System.Windows.Forms.CheckedListBox ();
			this.rdoTab = new System.Windows.Forms.RadioButton ();
			this.rdoComma = new System.Windows.Forms.RadioButton ();
			this.rdoSpace = new System.Windows.Forms.RadioButton ();
			this.rdoPipe = new System.Windows.Forms.RadioButton ();
			this.rdoSemicolon = new System.Windows.Forms.RadioButton ();
			this.rdoOthers = new System.Windows.Forms.RadioButton ();
			this.tbOther = new System.Windows.Forms.TextBox ();
			this.btnOk = new System.Windows.Forms.Button ();
			this.btnCancel = new System.Windows.Forms.Button ();
			this.gbxFields = new System.Windows.Forms.GroupBox ();
			this.btnSelectNone = new System.Windows.Forms.Button ();
			this.btnSelectAll = new System.Windows.Forms.Button ();
			this.gbxDelimiters = new System.Windows.Forms.GroupBox ();
			this.gbxExport.SuspendLayout ();
			this.gbxFields.SuspendLayout ();
			this.gbxDelimiters.SuspendLayout ();
			this.SuspendLayout ();
			// 
			// gbxExport
			// 
			this.gbxExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbxExport.Controls.Add ( this.btnBrowse );
			this.gbxExport.Controls.Add ( this.tbOutputFilename );
			this.gbxExport.Location = new System.Drawing.Point ( 13, 255 );
			this.gbxExport.Name = "gbxExport";
			this.gbxExport.Size = new System.Drawing.Size ( 320, 52 );
			this.gbxExport.TabIndex = 0;
			this.gbxExport.TabStop = false;
			this.gbxExport.Text = "Specify Output File";
			// 
			// btnBrowse
			// 
			this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowse.Location = new System.Drawing.Point ( 251, 17 );
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size ( 63, 23 );
			this.btnBrowse.TabIndex = 10;
			this.btnBrowse.Text = "&Browse...";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler ( this.btnBrowse_Click );
			// 
			// tbOutputFilename
			// 
			this.tbOutputFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tbOutputFilename.Location = new System.Drawing.Point ( 6, 19 );
			this.tbOutputFilename.Name = "tbOutputFilename";
			this.tbOutputFilename.Size = new System.Drawing.Size ( 235, 20 );
			this.tbOutputFilename.TabIndex = 9;
			// 
			// clbFieldsToExport
			// 
			this.clbFieldsToExport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.clbFieldsToExport.CheckOnClick = true;
			this.clbFieldsToExport.FormattingEnabled = true;
			this.clbFieldsToExport.Location = new System.Drawing.Point ( 6, 19 );
			this.clbFieldsToExport.Name = "clbFieldsToExport";
			this.clbFieldsToExport.Size = new System.Drawing.Size ( 308, 94 );
			this.clbFieldsToExport.TabIndex = 1;
			this.clbFieldsToExport.ThreeDCheckBoxes = true;
			// 
			// rdoTab
			// 
			this.rdoTab.AutoSize = true;
			this.rdoTab.Location = new System.Drawing.Point ( 120, 23 );
			this.rdoTab.Name = "rdoTab";
			this.rdoTab.Size = new System.Drawing.Size ( 44, 17 );
			this.rdoTab.TabIndex = 3;
			this.rdoTab.TabStop = true;
			this.rdoTab.Text = "&Tab";
			this.rdoTab.UseVisualStyleBackColor = true;
			// 
			// rdoComma
			// 
			this.rdoComma.AutoSize = true;
			this.rdoComma.Checked = true;
			this.rdoComma.Location = new System.Drawing.Point ( 11, 23 );
			this.rdoComma.Name = "rdoComma";
			this.rdoComma.Size = new System.Drawing.Size ( 90, 17 );
			this.rdoComma.TabIndex = 2;
			this.rdoComma.TabStop = true;
			this.rdoComma.Text = "&Comma (CSV)";
			this.rdoComma.UseVisualStyleBackColor = true;
			// 
			// rdoSpace
			// 
			this.rdoSpace.AutoSize = true;
			this.rdoSpace.Location = new System.Drawing.Point ( 213, 23 );
			this.rdoSpace.Name = "rdoSpace";
			this.rdoSpace.Size = new System.Drawing.Size ( 56, 17 );
			this.rdoSpace.TabIndex = 4;
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
			this.rdoPipe.TabIndex = 5;
			this.rdoPipe.TabStop = true;
			this.rdoPipe.Text = "&Pipe";
			this.rdoPipe.UseVisualStyleBackColor = true;
			// 
			// rdoSemicolon
			// 
			this.rdoSemicolon.AutoSize = true;
			this.rdoSemicolon.Location = new System.Drawing.Point ( 120, 41 );
			this.rdoSemicolon.Name = "rdoSemicolon";
			this.rdoSemicolon.Size = new System.Drawing.Size ( 74, 17 );
			this.rdoSemicolon.TabIndex = 6;
			this.rdoSemicolon.TabStop = true;
			this.rdoSemicolon.Text = "Se&micolon";
			this.rdoSemicolon.UseVisualStyleBackColor = true;
			// 
			// rdoOthers
			// 
			this.rdoOthers.AutoSize = true;
			this.rdoOthers.Location = new System.Drawing.Point ( 213, 41 );
			this.rdoOthers.Name = "rdoOthers";
			this.rdoOthers.Size = new System.Drawing.Size ( 54, 17 );
			this.rdoOthers.TabIndex = 7;
			this.rdoOthers.TabStop = true;
			this.rdoOthers.Text = "&Other:";
			this.rdoOthers.UseVisualStyleBackColor = true;
			// 
			// tbOther
			// 
			this.tbOther.Location = new System.Drawing.Point ( 270, 38 );
			this.tbOther.MaxLength = 1;
			this.tbOther.Name = "tbOther";
			this.tbOther.Size = new System.Drawing.Size ( 27, 20 );
			this.tbOther.TabIndex = 8;
			this.tbOther.TextChanged += new System.EventHandler ( this.other_TextChanged );
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.Location = new System.Drawing.Point ( 195, 313 );
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size ( 63, 23 );
			this.btnOk.TabIndex = 11;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler ( this.btnOk_Click );
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point ( 264, 313 );
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size ( 63, 23 );
			this.btnCancel.TabIndex = 12;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler ( this.btncancel_Click );
			// 
			// gbxFields
			// 
			this.gbxFields.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbxFields.Controls.Add ( this.btnSelectNone );
			this.gbxFields.Controls.Add ( this.btnSelectAll );
			this.gbxFields.Controls.Add ( this.clbFieldsToExport );
			this.gbxFields.Location = new System.Drawing.Point ( 13, 12 );
			this.gbxFields.Name = "gbxFields";
			this.gbxFields.Size = new System.Drawing.Size ( 320, 157 );
			this.gbxFields.TabIndex = 16;
			this.gbxFields.TabStop = false;
			this.gbxFields.Text = "Select Fields to Export";
			// 
			// btnSelectNone
			// 
			this.btnSelectNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnSelectNone.Location = new System.Drawing.Point ( 87, 122 );
			this.btnSelectNone.Name = "btnSelectNone";
			this.btnSelectNone.Size = new System.Drawing.Size ( 75, 23 );
			this.btnSelectNone.TabIndex = 3;
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
			this.btnSelectAll.TabIndex = 2;
			this.btnSelectAll.Text = "Select &All";
			this.btnSelectAll.UseVisualStyleBackColor = true;
			this.btnSelectAll.Click += new System.EventHandler ( this.SelectAll_Click );
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
			this.gbxDelimiters.Location = new System.Drawing.Point ( 13, 175 );
			this.gbxDelimiters.Name = "gbxDelimiters";
			this.gbxDelimiters.Size = new System.Drawing.Size ( 320, 74 );
			this.gbxDelimiters.TabIndex = 17;
			this.gbxDelimiters.TabStop = false;
			this.gbxDelimiters.Text = "Select a Delimiter";
			// 
			// GetExportOptionsDialog
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF ( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size ( 344, 345 );
			this.Controls.Add ( this.gbxDelimiters );
			this.Controls.Add ( this.gbxFields );
			this.Controls.Add ( this.btnOk );
			this.Controls.Add ( this.btnCancel );
			this.Controls.Add ( this.gbxExport );
			this.Icon = ((System.Drawing.Icon)(resources.GetObject ( "$this.Icon" )));
			this.MinimumSize = new System.Drawing.Size ( 352, 379 );
			this.Name = "GetExportOptionsDialog";
			this.Text = "Choose Export Options";
			this.Load += new System.EventHandler ( this.ListBox_load );
			this.gbxExport.ResumeLayout ( false );
			this.gbxExport.PerformLayout ();
			this.gbxFields.ResumeLayout ( false );
			this.gbxDelimiters.ResumeLayout ( false );
			this.gbxDelimiters.PerformLayout ();
			this.ResumeLayout ( false );

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxExport;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox tbOutputFilename;
        private System.Windows.Forms.TextBox tbOther;
        private System.Windows.Forms.RadioButton rdoComma;
        private System.Windows.Forms.RadioButton rdoTab;
        private System.Windows.Forms.RadioButton rdoSpace;
        private System.Windows.Forms.RadioButton rdoPipe;
        private System.Windows.Forms.RadioButton rdoSemicolon;
        private System.Windows.Forms.RadioButton rdoOthers;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckedListBox clbFieldsToExport;
        private System.Windows.Forms.GroupBox gbxFields;
        private System.Windows.Forms.GroupBox gbxDelimiters;
        private System.Windows.Forms.Button btnSelectNone;
        private System.Windows.Forms.Button btnSelectAll;
    }
}