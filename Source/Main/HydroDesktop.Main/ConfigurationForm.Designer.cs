namespace HydroDesktop.Main
{
    partial class ConfigurationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtDataRepository = new System.Windows.Forms.TextBox();
            this.txtProjectFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblProjectFileName = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtMetadataCache = new System.Windows.Forms.TextBox();
            this.lblMetadataCache = new System.Windows.Forms.Label();
            this.txtProjection = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(384, 342);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtProjection);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.txtMetadataCache);
            this.tabPage1.Controls.Add(this.lblMetadataCache);
            this.tabPage1.Controls.Add(this.btnClose);
            this.tabPage1.Controls.Add(this.txtDataRepository);
            this.tabPage1.Controls.Add(this.txtProjectFileName);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.lblProjectFileName);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(376, 316);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Current Project";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(293, 285);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // txtDataRepository
            // 
            this.txtDataRepository.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDataRepository.Location = new System.Drawing.Point(12, 85);
            this.txtDataRepository.Multiline = true;
            this.txtDataRepository.Name = "txtDataRepository";
            this.txtDataRepository.Size = new System.Drawing.Size(356, 34);
            this.txtDataRepository.TabIndex = 3;
            // 
            // txtProjectFileName
            // 
            this.txtProjectFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProjectFileName.Location = new System.Drawing.Point(12, 26);
            this.txtProjectFileName.Multiline = true;
            this.txtProjectFileName.Name = "txtProjectFileName";
            this.txtProjectFileName.Size = new System.Drawing.Size(356, 34);
            this.txtProjectFileName.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(178, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Data Repository Database:";
            // 
            // lblProjectFileName
            // 
            this.lblProjectFileName.AutoSize = true;
            this.lblProjectFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProjectFileName.Location = new System.Drawing.Point(9, 7);
            this.lblProjectFileName.Name = "lblProjectFileName";
            this.lblProjectFileName.Size = new System.Drawing.Size(126, 15);
            this.lblProjectFileName.TabIndex = 0;
            this.lblProjectFileName.Text = "Project File Name:";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(376, 238);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Application Settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtMetadataCache
            // 
            this.txtMetadataCache.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMetadataCache.Location = new System.Drawing.Point(12, 144);
            this.txtMetadataCache.Multiline = true;
            this.txtMetadataCache.Name = "txtMetadataCache";
            this.txtMetadataCache.Size = new System.Drawing.Size(356, 35);
            this.txtMetadataCache.TabIndex = 6;
            // 
            // lblMetadataCache
            // 
            this.lblMetadataCache.AutoSize = true;
            this.lblMetadataCache.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMetadataCache.Location = new System.Drawing.Point(9, 126);
            this.lblMetadataCache.Name = "lblMetadataCache";
            this.lblMetadataCache.Size = new System.Drawing.Size(180, 15);
            this.lblMetadataCache.TabIndex = 5;
            this.lblMetadataCache.Text = "Metadata Cache Database:";
            // 
            // txtProjection
            // 
            this.txtProjection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProjection.Location = new System.Drawing.Point(12, 203);
            this.txtProjection.Multiline = true;
            this.txtProjection.Name = "txtProjection";
            this.txtProjection.Size = new System.Drawing.Size(356, 76);
            this.txtProjection.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 185);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "Map Projection:";
            // 
            // ConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 342);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigurationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Application Settings";
            this.Load += new System.EventHandler(this.ConfigurationForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtDataRepository;
        private System.Windows.Forms.TextBox txtProjectFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblProjectFileName;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtProjection;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMetadataCache;
        private System.Windows.Forms.Label lblMetadataCache;


    }
}