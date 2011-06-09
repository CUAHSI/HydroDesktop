namespace HydroDesktop.Main
{
    partial class WelcomeScreen
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WelcomeScreen));
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblProductVersion = new System.Windows.Forms.Label();
            this.lstRecentProjects = new System.Windows.Forms.ListBox();
            this.bsRecentFiles = new System.Windows.Forms.BindingSource(this.components);
            this.groupBoxProject = new System.Windows.Forms.GroupBox();
            this.btnBrowseProject = new System.Windows.Forms.Button();
            this.rbOpenExistingProject = new System.Windows.Forms.RadioButton();
            this.rbEmptyProject = new System.Windows.Forms.RadioButton();
            this.lstProjectTemplates = new System.Windows.Forms.ListBox();
            this.rbNewProjectTemplate = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.spatialStatusStrip1 = new DotSpatial.Controls.SpatialStatusStrip();
            this.lblProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.panelStatus = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsRecentFiles)).BeginInit();
            this.groupBoxProject.SuspendLayout();
            this.spatialStatusStrip1.SuspendLayout();
            this.panelStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(8, 214);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(150, 17);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "Show this dialog at startup";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(12, 98);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(171, 13);
            this.linkLabel1.TabIndex = 4;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Getting Started with HydroDesktop";
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(22, 130);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(140, 13);
            this.linkLabel2.TabIndex = 5;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "View HydroDesktop help file";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::HydroDesktop.Main.Properties.Resources.CuahsiLogo38;
            this.pictureBox1.Location = new System.Drawing.Point(8, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(194, 155);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(51, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 24);
            this.label1.TabIndex = 7;
            this.label1.Text = "HydroDesktop";
            // 
            // lblProductVersion
            // 
            this.lblProductVersion.AutoSize = true;
            this.lblProductVersion.Location = new System.Drawing.Point(22, 67);
            this.lblProductVersion.Name = "lblProductVersion";
            this.lblProductVersion.Size = new System.Drawing.Size(145, 13);
            this.lblProductVersion.TabIndex = 8;
            this.lblProductVersion.Text = "CUAHSI HydroDesktop 1.2.0";
            // 
            // lstRecentProjects
            // 
            this.lstRecentProjects.FormattingEnabled = true;
            this.lstRecentProjects.Location = new System.Drawing.Point(23, 125);
            this.lstRecentProjects.Name = "lstRecentProjects";
            this.lstRecentProjects.Size = new System.Drawing.Size(245, 82);
            this.lstRecentProjects.TabIndex = 9;
            // 
            // bsRecentFiles
            // 
            this.bsRecentFiles.DataSource = this.lstRecentProjects.CustomTabOffsets;
            // 
            // groupBoxProject
            // 
            this.groupBoxProject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxProject.Controls.Add(this.btnBrowseProject);
            this.groupBoxProject.Controls.Add(this.rbOpenExistingProject);
            this.groupBoxProject.Controls.Add(this.rbEmptyProject);
            this.groupBoxProject.Controls.Add(this.lstProjectTemplates);
            this.groupBoxProject.Controls.Add(this.rbNewProjectTemplate);
            this.groupBoxProject.Controls.Add(this.lstRecentProjects);
            this.groupBoxProject.Location = new System.Drawing.Point(208, 1);
            this.groupBoxProject.Name = "groupBoxProject";
            this.groupBoxProject.Size = new System.Drawing.Size(274, 236);
            this.groupBoxProject.TabIndex = 13;
            this.groupBoxProject.TabStop = false;
            // 
            // btnBrowseProject
            // 
            this.btnBrowseProject.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowseProject.Image")));
            this.btnBrowseProject.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBrowseProject.Location = new System.Drawing.Point(138, 99);
            this.btnBrowseProject.Name = "btnBrowseProject";
            this.btnBrowseProject.Size = new System.Drawing.Size(78, 22);
            this.btnBrowseProject.TabIndex = 16;
            this.btnBrowseProject.Text = "Browse ...";
            this.btnBrowseProject.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBrowseProject.UseVisualStyleBackColor = true;
            this.btnBrowseProject.Click += new System.EventHandler(this.btnBrowseProject_Click);
            // 
            // rbOpenExistingProject
            // 
            this.rbOpenExistingProject.AutoSize = true;
            this.rbOpenExistingProject.Location = new System.Drawing.Point(6, 102);
            this.rbOpenExistingProject.Name = "rbOpenExistingProject";
            this.rbOpenExistingProject.Size = new System.Drawing.Size(126, 17);
            this.rbOpenExistingProject.TabIndex = 15;
            this.rbOpenExistingProject.Text = "Open Existing Project";
            this.rbOpenExistingProject.UseVisualStyleBackColor = true;
            // 
            // rbEmptyProject
            // 
            this.rbEmptyProject.AutoSize = true;
            this.rbEmptyProject.Location = new System.Drawing.Point(6, 213);
            this.rbEmptyProject.Name = "rbEmptyProject";
            this.rbEmptyProject.Size = new System.Drawing.Size(149, 17);
            this.rbEmptyProject.TabIndex = 14;
            this.rbEmptyProject.Text = "Create New Empty Project";
            this.rbEmptyProject.UseVisualStyleBackColor = true;
            // 
            // lstProjectTemplates
            // 
            this.lstProjectTemplates.FormattingEnabled = true;
            this.lstProjectTemplates.Items.AddRange(new object[] {
            "North America",
            "World"});
            this.lstProjectTemplates.Location = new System.Drawing.Point(23, 42);
            this.lstProjectTemplates.Name = "lstProjectTemplates";
            this.lstProjectTemplates.Size = new System.Drawing.Size(245, 43);
            this.lstProjectTemplates.TabIndex = 13;
            // 
            // rbNewProjectTemplate
            // 
            this.rbNewProjectTemplate.AutoSize = true;
            this.rbNewProjectTemplate.Checked = true;
            this.rbNewProjectTemplate.Location = new System.Drawing.Point(6, 19);
            this.rbNewProjectTemplate.Name = "rbNewProjectTemplate";
            this.rbNewProjectTemplate.Size = new System.Drawing.Size(190, 17);
            this.rbNewProjectTemplate.TabIndex = 12;
            this.rbNewProjectTemplate.TabStop = true;
            this.rbNewProjectTemplate.Text = "Create New Project From Template";
            this.rbNewProjectTemplate.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(407, 243);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 14;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // spatialStatusStrip1
            // 
            this.spatialStatusStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spatialStatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblProgress,
            this.toolStripProgressBar1});
            this.spatialStatusStrip1.Location = new System.Drawing.Point(0, 0);
            this.spatialStatusStrip1.Name = "spatialStatusStrip1";
            this.spatialStatusStrip1.ProgressBar = this.toolStripProgressBar1;
            this.spatialStatusStrip1.ProgressLabel = this.lblProgress;
            this.spatialStatusStrip1.Size = new System.Drawing.Size(393, 23);
            this.spatialStatusStrip1.SizingGrip = false;
            this.spatialStatusStrip1.TabIndex = 16;
            this.spatialStatusStrip1.Text = "spatialStatusStrip1";
            // 
            // lblProgress
            // 
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(226, 18);
            this.lblProgress.Spring = true;
            this.lblProgress.Text = "...";
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(150, 17);
            // 
            // panelStatus
            // 
            this.panelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelStatus.Controls.Add(this.spatialStatusStrip1);
            this.panelStatus.Location = new System.Drawing.Point(8, 243);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(393, 23);
            this.panelStatus.TabIndex = 17;
            // 
            // WelcomeScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(494, 274);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBoxProject);
            this.Controls.Add(this.lblProductVersion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WelcomeScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " Welcome to HydroDesktop";
            this.Load += new System.EventHandler(this.WelcomeScreen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsRecentFiles)).EndInit();
            this.groupBoxProject.ResumeLayout(false);
            this.groupBoxProject.PerformLayout();
            this.spatialStatusStrip1.ResumeLayout(false);
            this.spatialStatusStrip1.PerformLayout();
            this.panelStatus.ResumeLayout(false);
            this.panelStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblProductVersion;
        private System.Windows.Forms.ListBox lstRecentProjects;
        private System.Windows.Forms.BindingSource bsRecentFiles;
        private System.Windows.Forms.GroupBox groupBoxProject;
        private System.Windows.Forms.RadioButton rbOpenExistingProject;
        private System.Windows.Forms.RadioButton rbEmptyProject;
        private System.Windows.Forms.ListBox lstProjectTemplates;
        private System.Windows.Forms.RadioButton rbNewProjectTemplate;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnBrowseProject;
        private DotSpatial.Controls.SpatialStatusStrip spatialStatusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblProgress;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.Panel panelStatus;
    }
}