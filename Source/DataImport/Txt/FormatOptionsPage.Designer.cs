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
            this.gbxDelimiters = new System.Windows.Forms.GroupBox();
            this.rdoComma = new System.Windows.Forms.RadioButton();
            this.rdoTab = new System.Windows.Forms.RadioButton();
            this.rdoSpace = new System.Windows.Forms.RadioButton();
            this.rdoPipe = new System.Windows.Forms.RadioButton();
            this.rdoSemicolon = new System.Windows.Forms.RadioButton();
            this.rdoOthers = new System.Windows.Forms.RadioButton();
            this.tbOther = new System.Windows.Forms.TextBox();
            this.dgvPreview = new System.Windows.Forms.DataGridView();
            this.gbxDelimiters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.Size = new System.Drawing.Size(540, 64);
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
            // gbxDelimiters
            // 
            this.gbxDelimiters.Controls.Add(this.rdoComma);
            this.gbxDelimiters.Controls.Add(this.rdoTab);
            this.gbxDelimiters.Controls.Add(this.rdoSpace);
            this.gbxDelimiters.Controls.Add(this.rdoPipe);
            this.gbxDelimiters.Controls.Add(this.rdoSemicolon);
            this.gbxDelimiters.Controls.Add(this.rdoOthers);
            this.gbxDelimiters.Controls.Add(this.tbOther);
            this.gbxDelimiters.Location = new System.Drawing.Point(19, 117);
            this.gbxDelimiters.Name = "gbxDelimiters";
            this.gbxDelimiters.Size = new System.Drawing.Size(313, 74);
            this.gbxDelimiters.TabIndex = 10;
            this.gbxDelimiters.TabStop = false;
            this.gbxDelimiters.Text = "Select a Delimiter";
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
            this.rdoTab.Location = new System.Drawing.Point(116, 23);
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
            this.rdoSpace.Location = new System.Drawing.Point(203, 23);
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
            this.rdoSemicolon.Location = new System.Drawing.Point(116, 41);
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
            this.rdoOthers.Location = new System.Drawing.Point(203, 41);
            this.rdoOthers.Name = "rdoOthers";
            this.rdoOthers.Size = new System.Drawing.Size(54, 17);
            this.rdoOthers.TabIndex = 15;
            this.rdoOthers.TabStop = true;
            this.rdoOthers.Text = "&Other:";
            this.rdoOthers.UseVisualStyleBackColor = true;
            // 
            // tbOther
            // 
            this.tbOther.Location = new System.Drawing.Point(269, 38);
            this.tbOther.MaxLength = 1;
            this.tbOther.Name = "tbOther";
            this.tbOther.Size = new System.Drawing.Size(27, 20);
            this.tbOther.TabIndex = 16;
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
            // FormatOptionsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvPreview);
            this.Controls.Add(this.gbxDelimiters);
            this.Controls.Add(this.lblFileType);
            this.Controls.Add(this.cmbFileType);
            this.Name = "FormatOptionsPage";
            this.Size = new System.Drawing.Size(540, 342);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.FormatOptionsPage_SetActive);
            this.Controls.SetChildIndex(this.cmbFileType, 0);
            this.Controls.SetChildIndex(this.lblFileType, 0);
            this.Controls.SetChildIndex(this.Banner, 0);
            this.Controls.SetChildIndex(this.gbxDelimiters, 0);
            this.Controls.SetChildIndex(this.dgvPreview, 0);
            this.gbxDelimiters.ResumeLayout(false);
            this.gbxDelimiters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFileType;
        private System.Windows.Forms.ComboBox cmbFileType;
        private System.Windows.Forms.GroupBox gbxDelimiters;
        private System.Windows.Forms.RadioButton rdoComma;
        private System.Windows.Forms.RadioButton rdoTab;
        private System.Windows.Forms.RadioButton rdoSpace;
        private System.Windows.Forms.RadioButton rdoPipe;
        private System.Windows.Forms.RadioButton rdoSemicolon;
        private System.Windows.Forms.RadioButton rdoOthers;
        private System.Windows.Forms.TextBox tbOther;
        private System.Windows.Forms.DataGridView dgvPreview;
    }
}
