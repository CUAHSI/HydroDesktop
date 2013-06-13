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
            this.lblProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.QuickStartButton = new System.Windows.Forms.Button();
            this.HelpButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.bsRecentFiles)).BeginInit();
            this.groupBoxProject.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            this.checkBox1.Visible = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // lblProductVersion
            // 
            this.lblProductVersion.AutoSize = true;
            this.lblProductVersion.Location = new System.Drawing.Point(35, 63);
            this.lblProductVersion.Name = "lblProductVersion";
            this.lblProductVersion.Size = new System.Drawing.Size(145, 13);
            this.lblProductVersion.TabIndex = 8;
            this.lblProductVersion.Text = "CUAHSI HydroDesktop 1.5.0";
            // 
            // lstRecentProjects
            // 
            this.lstRecentProjects.FormattingEnabled = true;
            this.lstRecentProjects.Location = new System.Drawing.Point(23, 145);
            this.lstRecentProjects.Name = "lstRecentProjects";
            this.lstRecentProjects.Size = new System.Drawing.Size(245, 56);
            this.lstRecentProjects.TabIndex = 6;
            this.lstRecentProjects.Click += new System.EventHandler(this.lstRecentProjects_Click);
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
            this.btnBrowseProject.Location = new System.Drawing.Point(138, 117);
            this.btnBrowseProject.Name = "btnBrowseProject";
            this.btnBrowseProject.Size = new System.Drawing.Size(78, 22);
            this.btnBrowseProject.TabIndex = 5;
            this.btnBrowseProject.Text = "Browse ...";
            this.btnBrowseProject.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBrowseProject.UseVisualStyleBackColor = true;
            this.btnBrowseProject.Click += new System.EventHandler(this.btnBrowseProject_Click);
            // 
            // rbOpenExistingProject
            // 
            this.rbOpenExistingProject.AutoSize = true;
            this.rbOpenExistingProject.Location = new System.Drawing.Point(6, 120);
            this.rbOpenExistingProject.Name = "rbOpenExistingProject";
            this.rbOpenExistingProject.Size = new System.Drawing.Size(126, 17);
            this.rbOpenExistingProject.TabIndex = 4;
            this.rbOpenExistingProject.Text = "Open Existing Project";
            this.rbOpenExistingProject.UseVisualStyleBackColor = true;
            // 
            // rbEmptyProject
            // 
            this.rbEmptyProject.AutoSize = true;
            this.rbEmptyProject.Location = new System.Drawing.Point(6, 213);
            this.rbEmptyProject.Name = "rbEmptyProject";
            this.rbEmptyProject.Size = new System.Drawing.Size(149, 17);
            this.rbEmptyProject.TabIndex = 7;
            this.rbEmptyProject.Text = "Create New Empty Project";
            this.rbEmptyProject.UseVisualStyleBackColor = true;
            // 
            // lstProjectTemplates
            // 
            this.lstProjectTemplates.FormattingEnabled = true;
            this.lstProjectTemplates.Location = new System.Drawing.Point(23, 42);
            this.lstProjectTemplates.Name = "lstProjectTemplates";
            this.lstProjectTemplates.Size = new System.Drawing.Size(245, 69);
            this.lstProjectTemplates.TabIndex = 3;
            // 
            // rbNewProjectTemplate
            // 
            this.rbNewProjectTemplate.AutoSize = true;
            this.rbNewProjectTemplate.Checked = true;
            this.rbNewProjectTemplate.Location = new System.Drawing.Point(6, 19);
            this.rbNewProjectTemplate.Name = "rbNewProjectTemplate";
            this.rbNewProjectTemplate.Size = new System.Drawing.Size(190, 17);
            this.rbNewProjectTemplate.TabIndex = 2;
            this.rbNewProjectTemplate.TabStop = true;
            this.rbNewProjectTemplate.Text = "Create New Project From Template";
            this.rbNewProjectTemplate.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(407, 243);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
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
            this.panelStatus.Location = new System.Drawing.Point(8, 243);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(393, 23);
            this.panelStatus.TabIndex = 17;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::HydroDesktop.Main.Properties.Resources.Logo_Small1;
            this.pictureBox1.Location = new System.Drawing.Point(8, 20);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(200, 148);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // QuickStartButton
            // 
            this.QuickStartButton.Image = ((System.Drawing.Image)(resources.GetObject("QuickStartButton.Image")));
            this.QuickStartButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.QuickStartButton.Location = new System.Drawing.Point(22, 121);
            this.QuickStartButton.Name = "QuickStartButton";
            this.QuickStartButton.Size = new System.Drawing.Size(171, 30);
            this.QuickStartButton.TabIndex = 9;
            this.QuickStartButton.Text = "View Quick Start Guide";
            this.QuickStartButton.UseVisualStyleBackColor = true;
            this.QuickStartButton.Click += new System.EventHandler(this.QuickStartButton_Click);
            // 
            // HelpButton
            // 
            this.HelpButton.Image = ((System.Drawing.Image)(resources.GetObject("HelpButton.Image")));
            this.HelpButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HelpButton.Location = new System.Drawing.Point(22, 165);
            this.HelpButton.Name = "HelpButton";
            this.HelpButton.Size = new System.Drawing.Size(171, 30);
            this.HelpButton.TabIndex = 18;
            this.HelpButton.Text = "View HydroDesktop Help File";
            this.HelpButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.HelpButton.UseVisualStyleBackColor = true;
            this.HelpButton.Click += new System.EventHandler(this.HelpButton_Click);
            // 
            // WelcomeScreen
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(494, 274);
            this.Controls.Add(this.HelpButton);
            this.Controls.Add(this.QuickStartButton);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBoxProject);
            this.Controls.Add(this.lblProductVersion);
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
            ((System.ComponentModel.ISupportInitialize)(this.bsRecentFiles)).EndInit();
            this.groupBoxProject.ResumeLayout(false);
            this.groupBoxProject.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1;
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
        private System.Windows.Forms.ToolStripStatusLabel lblProgress;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button QuickStartButton;
        private System.Windows.Forms.Button HelpButton;
    }
}