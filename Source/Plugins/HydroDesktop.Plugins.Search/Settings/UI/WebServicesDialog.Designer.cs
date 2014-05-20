namespace HydroDesktop.Plugins.Search.Settings.UI
{
    partial class WebServicesDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebServicesDialog));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.paButtons = new System.Windows.Forms.Panel();
            this.gbTypeOfCatalog = new System.Windows.Forms.GroupBox();
            this.rbLocalMetadataCache = new System.Windows.Forms.RadioButton();
            this.rbHisCentral = new System.Windows.Forms.RadioButton();
            this.btnManageDataSources = new System.Windows.Forms.Button();
            this.bntAddLocalDataSource = new System.Windows.Forms.Button();
            this.btnSelectNone = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.webServicesUserControl1 = new HydroDesktop.Plugins.Search.Settings.UI.WebServicesUserControl(_app, _rectangleDrawing);
            this.paButtons.SuspendLayout();
            this.gbTypeOfCatalog.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(598, 304);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(517, 304);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // paButtons
            // 
            this.paButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.paButtons.Controls.Add(this.gbTypeOfCatalog);
            this.paButtons.Controls.Add(this.btnManageDataSources);
            this.paButtons.Controls.Add(this.bntAddLocalDataSource);
            this.paButtons.Location = new System.Drawing.Point(0, 1);
            this.paButtons.Name = "paButtons";
            this.paButtons.Size = new System.Drawing.Size(681, 68);
            this.paButtons.TabIndex = 45;
            // 
            // gbTypeOfCatalog
            // 
            this.gbTypeOfCatalog.Controls.Add(this.rbLocalMetadataCache);
            this.gbTypeOfCatalog.Controls.Add(this.rbHisCentral);
            this.gbTypeOfCatalog.Location = new System.Drawing.Point(12, 8);
            this.gbTypeOfCatalog.Name = "gbTypeOfCatalog";
            this.gbTypeOfCatalog.Size = new System.Drawing.Size(237, 53);
            this.gbTypeOfCatalog.TabIndex = 49;
            this.gbTypeOfCatalog.TabStop = false;
            this.gbTypeOfCatalog.Text = "Select type of catalog";
            // 
            // rbLocalMetadataCache
            // 
            this.rbLocalMetadataCache.AutoSize = true;
            this.rbLocalMetadataCache.Location = new System.Drawing.Point(113, 19);
            this.rbLocalMetadataCache.Name = "rbLocalMetadataCache";
            this.rbLocalMetadataCache.Size = new System.Drawing.Size(114, 17);
            this.rbLocalMetadataCache.TabIndex = 47;
            this.rbLocalMetadataCache.TabStop = true;
            this.rbLocalMetadataCache.Text = "Local Data Source";
            this.rbLocalMetadataCache.UseVisualStyleBackColor = true;
            // 
            // rbHisCentral
            // 
            this.rbHisCentral.AutoSize = true;
            this.rbHisCentral.Location = new System.Drawing.Point(9, 19);
            this.rbHisCentral.Name = "rbHisCentral";
            this.rbHisCentral.Size = new System.Drawing.Size(79, 17);
            this.rbHisCentral.TabIndex = 46;
            this.rbHisCentral.TabStop = true;
            this.rbHisCentral.Text = "HIS Central";
            this.rbHisCentral.UseVisualStyleBackColor = true;
            // 
            // btnManageDataSources
            // 
            this.btnManageDataSources.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnManageDataSources.Location = new System.Drawing.Point(517, 37);
            this.btnManageDataSources.Name = "btnManageDataSources";
            this.btnManageDataSources.Size = new System.Drawing.Size(156, 23);
            this.btnManageDataSources.TabIndex = 45;
            this.btnManageDataSources.Text = "Manage Data Sources...";
            this.btnManageDataSources.UseVisualStyleBackColor = true;
            this.btnManageDataSources.Click += new System.EventHandler(this.btnManageDataSources_Click);
            // 
            // bntAddLocalDataSource
            // 
            this.bntAddLocalDataSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bntAddLocalDataSource.Location = new System.Drawing.Point(517, 8);
            this.bntAddLocalDataSource.Name = "bntAddLocalDataSource";
            this.bntAddLocalDataSource.Size = new System.Drawing.Size(156, 23);
            this.bntAddLocalDataSource.TabIndex = 44;
            this.bntAddLocalDataSource.Text = "Add Data Source...";
            this.bntAddLocalDataSource.UseVisualStyleBackColor = true;
            this.bntAddLocalDataSource.Click += new System.EventHandler(this.bntAddLocalDataSource_Click);
            // 
            // btnSelectNone
            // 
            this.btnSelectNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectNone.Location = new System.Drawing.Point(174, 304);
            this.btnSelectNone.Name = "btnSelectNone";
            this.btnSelectNone.Size = new System.Drawing.Size(75, 23);
            this.btnSelectNone.TabIndex = 43;
            this.btnSelectNone.Text = "Select None";
            this.btnSelectNone.UseVisualStyleBackColor = true;
            this.btnSelectNone.Click += new System.EventHandler(this.btnSelectNone_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectAll.Location = new System.Drawing.Point(93, 304);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 42;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRefresh.Location = new System.Drawing.Point(12, 304);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 41;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // webServicesUserControl1
            // 
            this.webServicesUserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webServicesUserControl1.Location = new System.Drawing.Point(0, 75);
            this.webServicesUserControl1.Name = "webServicesUserControl1";
            this.webServicesUserControl1.Size = new System.Drawing.Size(673, 221);
            this.webServicesUserControl1.TabIndex = 0;
            // 
            // WebServicesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(681, 339);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.paButtons);
            this.Controls.Add(this.btnSelectNone);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.webServicesUserControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WebServicesDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Data Sources";
            this.MouseEnter += new System.EventHandler(this.WebServicesDialog_MouseEnter);
            this.paButtons.ResumeLayout(false);
            this.gbTypeOfCatalog.ResumeLayout(false);
            this.gbTypeOfCatalog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private WebServicesUserControl webServicesUserControl1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel paButtons;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnSelectNone;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rbLocalMetadataCache;
        private System.Windows.Forms.RadioButton rbHisCentral;
        private System.Windows.Forms.GroupBox gbTypeOfCatalog;
        private System.Windows.Forms.Button bntAddLocalDataSource;
        private System.Windows.Forms.Button btnManageDataSources;
    }
}