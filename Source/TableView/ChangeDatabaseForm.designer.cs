namespace TableView
{
    partial class ChangeDatabaseForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeDatabaseForm));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDataRepositoryRestoreDefault = new System.Windows.Forms.Button();
            this.btnDataRepositoryFileDialog = new System.Windows.Forms.Button();
            this.txtDataRepository = new System.Windows.Forms.TextBox();
            this.lblDataRepository = new System.Windows.Forms.Label();
            this.groupBoxMetadataCache = new System.Windows.Forms.GroupBox();
            this.btnMetadataCacheRestoreDefault = new System.Windows.Forms.Button();
            this.lblMetadataCache = new System.Windows.Forms.Label();
            this.txtMetadataCache = new System.Windows.Forms.TextBox();
            this.btnMetadataCacheFileDialog = new System.Windows.Forms.Button();
            this.groupBoxDataRepository = new System.Windows.Forms.GroupBox();
            this.groupBoxMetadataCache.SuspendLayout();
            this.groupBoxDataRepository.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(324, 239);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(410, 239);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDataRepositoryRestoreDefault
            // 
            this.btnDataRepositoryRestoreDefault.Location = new System.Drawing.Point(10, 64);
            this.btnDataRepositoryRestoreDefault.Name = "btnDataRepositoryRestoreDefault";
            this.btnDataRepositoryRestoreDefault.Size = new System.Drawing.Size(161, 23);
            this.btnDataRepositoryRestoreDefault.TabIndex = 5;
            this.btnDataRepositoryRestoreDefault.Text = "Use Default Database";
            this.btnDataRepositoryRestoreDefault.UseVisualStyleBackColor = true;
            this.btnDataRepositoryRestoreDefault.Click += new System.EventHandler(this.btnRestoreDefault_Click);
            // 
            // btnDataRepositoryFileDialog
            // 
            this.btnDataRepositoryFileDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDataRepositoryFileDialog.Image = ((System.Drawing.Image)(resources.GetObject("btnDataRepositoryFileDialog.Image")));
            this.btnDataRepositoryFileDialog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDataRepositoryFileDialog.Location = new System.Drawing.Point(426, 40);
            this.btnDataRepositoryFileDialog.Name = "btnDataRepositoryFileDialog";
            this.btnDataRepositoryFileDialog.Size = new System.Drawing.Size(38, 20);
            this.btnDataRepositoryFileDialog.TabIndex = 8;
            this.btnDataRepositoryFileDialog.Text = "...";
            this.btnDataRepositoryFileDialog.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDataRepositoryFileDialog.UseVisualStyleBackColor = true;
            this.btnDataRepositoryFileDialog.Click += new System.EventHandler(this.btnDataRepositoryFileDialog_Click);
            // 
            // txtDataRepository
            // 
            this.txtDataRepository.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDataRepository.Location = new System.Drawing.Point(8, 41);
            this.txtDataRepository.Name = "txtDataRepository";
            this.txtDataRepository.Size = new System.Drawing.Size(412, 20);
            this.txtDataRepository.TabIndex = 7;
            // 
            // lblDataRepository
            // 
            this.lblDataRepository.AutoSize = true;
            this.lblDataRepository.Location = new System.Drawing.Point(6, 25);
            this.lblDataRepository.Name = "lblDataRepository";
            this.lblDataRepository.Size = new System.Drawing.Size(235, 13);
            this.lblDataRepository.TabIndex = 6;
            this.lblDataRepository.Text = "Select the Data Repository SQLite database file:";
            // 
            // groupBoxMetadataCache
            // 
            this.groupBoxMetadataCache.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMetadataCache.Controls.Add(this.btnMetadataCacheRestoreDefault);
            this.groupBoxMetadataCache.Controls.Add(this.lblMetadataCache);
            this.groupBoxMetadataCache.Controls.Add(this.txtMetadataCache);
            this.groupBoxMetadataCache.Controls.Add(this.btnMetadataCacheFileDialog);
            this.groupBoxMetadataCache.Location = new System.Drawing.Point(11, 128);
            this.groupBoxMetadataCache.Name = "groupBoxMetadataCache";
            this.groupBoxMetadataCache.Size = new System.Drawing.Size(474, 103);
            this.groupBoxMetadataCache.TabIndex = 9;
            this.groupBoxMetadataCache.TabStop = false;
            this.groupBoxMetadataCache.Text = "Metadata Cache";
            // 
            // btnMetadataCacheRestoreDefault
            // 
            this.btnMetadataCacheRestoreDefault.Location = new System.Drawing.Point(9, 74);
            this.btnMetadataCacheRestoreDefault.Name = "btnMetadataCacheRestoreDefault";
            this.btnMetadataCacheRestoreDefault.Size = new System.Drawing.Size(163, 23);
            this.btnMetadataCacheRestoreDefault.TabIndex = 9;
            this.btnMetadataCacheRestoreDefault.Text = "Use Default Metadata Cache";
            this.btnMetadataCacheRestoreDefault.UseVisualStyleBackColor = true;
            this.btnMetadataCacheRestoreDefault.Click += new System.EventHandler(this.btnMetadataCacheRestoreDefault_Click);
            // 
            // lblMetadataCache
            // 
            this.lblMetadataCache.AutoSize = true;
            this.lblMetadataCache.Location = new System.Drawing.Point(6, 27);
            this.lblMetadataCache.Name = "lblMetadataCache";
            this.lblMetadataCache.Size = new System.Drawing.Size(238, 13);
            this.lblMetadataCache.TabIndex = 9;
            this.lblMetadataCache.Text = "Select the Metadata Cache SQLite database file:";
            // 
            // txtMetadataCache
            // 
            this.txtMetadataCache.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMetadataCache.Location = new System.Drawing.Point(8, 43);
            this.txtMetadataCache.Name = "txtMetadataCache";
            this.txtMetadataCache.Size = new System.Drawing.Size(412, 20);
            this.txtMetadataCache.TabIndex = 10;
            // 
            // btnMetadataCacheFileDialog
            // 
            this.btnMetadataCacheFileDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMetadataCacheFileDialog.Image = ((System.Drawing.Image)(resources.GetObject("btnMetadataCacheFileDialog.Image")));
            this.btnMetadataCacheFileDialog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMetadataCacheFileDialog.Location = new System.Drawing.Point(426, 42);
            this.btnMetadataCacheFileDialog.Name = "btnMetadataCacheFileDialog";
            this.btnMetadataCacheFileDialog.Size = new System.Drawing.Size(39, 20);
            this.btnMetadataCacheFileDialog.TabIndex = 11;
            this.btnMetadataCacheFileDialog.Text = "...";
            this.btnMetadataCacheFileDialog.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnMetadataCacheFileDialog.UseVisualStyleBackColor = true;
            this.btnMetadataCacheFileDialog.Click += new System.EventHandler(this.btnMetadataCacheFileDialog_Click);
            // 
            // groupBoxDataRepository
            // 
            this.groupBoxDataRepository.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxDataRepository.Controls.Add(this.lblDataRepository);
            this.groupBoxDataRepository.Controls.Add(this.txtDataRepository);
            this.groupBoxDataRepository.Controls.Add(this.btnDataRepositoryRestoreDefault);
            this.groupBoxDataRepository.Controls.Add(this.btnDataRepositoryFileDialog);
            this.groupBoxDataRepository.Location = new System.Drawing.Point(12, 12);
            this.groupBoxDataRepository.Name = "groupBoxDataRepository";
            this.groupBoxDataRepository.Size = new System.Drawing.Size(473, 93);
            this.groupBoxDataRepository.TabIndex = 10;
            this.groupBoxDataRepository.TabStop = false;
            this.groupBoxDataRepository.Text = "Data Repository";
            // 
            // ChangeDatabaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 274);
            this.Controls.Add(this.groupBoxDataRepository);
            this.Controls.Add(this.groupBoxMetadataCache);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChangeDatabaseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Change Database";
            this.groupBoxMetadataCache.ResumeLayout(false);
            this.groupBoxMetadataCache.PerformLayout();
            this.groupBoxDataRepository.ResumeLayout(false);
            this.groupBoxDataRepository.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDataRepositoryRestoreDefault;
        private System.Windows.Forms.Button btnDataRepositoryFileDialog;
        private System.Windows.Forms.TextBox txtDataRepository;
        private System.Windows.Forms.Label lblDataRepository;
        private System.Windows.Forms.GroupBox groupBoxMetadataCache;
        private System.Windows.Forms.Button btnMetadataCacheRestoreDefault;
        private System.Windows.Forms.Label lblMetadataCache;
        private System.Windows.Forms.TextBox txtMetadataCache;
        private System.Windows.Forms.Button btnMetadataCacheFileDialog;
        private System.Windows.Forms.GroupBox groupBoxDataRepository;
    }
}