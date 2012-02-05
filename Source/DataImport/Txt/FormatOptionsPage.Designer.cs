namespace DataImport.Txt
{
    partial class FormatOptionsPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblFileType = new System.Windows.Forms.Label();
            this.cmbFileType = new System.Windows.Forms.ComboBox();
            this.dgvPreview = new System.Windows.Forms.DataGridView();
            this.delimiterSelector = new HydroDesktop.Common.Controls.DelimiterSelector();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.Size = new System.Drawing.Size(540, 64);
            this.Banner.Subtitle = "Specify file format options.";
            this.Banner.Title = "Format options";
            // 
            // lblFileType
            // 
            this.lblFileType.AutoSize = true;
            this.lblFileType.Location = new System.Drawing.Point(16, 88);
            this.lblFileType.Name = "lblFileType";
            this.lblFileType.Size = new System.Drawing.Size(46, 13);
            this.lblFileType.TabIndex = 1;
            this.lblFileType.Text = "File type";
            // 
            // cmbFileType
            // 
            this.cmbFileType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFileType.FormattingEnabled = true;
            this.cmbFileType.Location = new System.Drawing.Point(86, 85);
            this.cmbFileType.Name = "cmbFileType";
            this.cmbFileType.Size = new System.Drawing.Size(121, 21);
            this.cmbFileType.TabIndex = 2;
            // 
            // dgvPreview
            // 
            this.dgvPreview.AllowUserToAddRows = false;
            this.dgvPreview.AllowUserToDeleteRows = false;
            this.dgvPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPreview.Location = new System.Drawing.Point(19, 198);
            this.dgvPreview.Name = "dgvPreview";
            this.dgvPreview.ReadOnly = true;
            this.dgvPreview.Size = new System.Drawing.Size(505, 130);
            this.dgvPreview.TabIndex = 11;
            // 
            // delimiterSelector
            // 
            this.delimiterSelector.CurrentDelimiter = ",";
            this.delimiterSelector.Location = new System.Drawing.Point(19, 114);
            this.delimiterSelector.Name = "delimiterSelector";
            this.delimiterSelector.Size = new System.Drawing.Size(313, 78);
            this.delimiterSelector.TabIndex = 12;
            // 
            // FormatOptionsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.delimiterSelector);
            this.Controls.Add(this.dgvPreview);
            this.Controls.Add(this.lblFileType);
            this.Controls.Add(this.cmbFileType);
            this.Name = "FormatOptionsPage";
            this.Size = new System.Drawing.Size(540, 342);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.FormatOptionsPage_SetActive);
            this.Controls.SetChildIndex(this.cmbFileType, 0);
            this.Controls.SetChildIndex(this.lblFileType, 0);
            this.Controls.SetChildIndex(this.Banner, 0);
            this.Controls.SetChildIndex(this.dgvPreview, 0);
            this.Controls.SetChildIndex(this.delimiterSelector, 0);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFileType;
        private System.Windows.Forms.ComboBox cmbFileType;
        private System.Windows.Forms.DataGridView dgvPreview;
        private HydroDesktop.Common.Controls.DelimiterSelector delimiterSelector;
    }
}
