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
            this.bsRecentFiles = new System.Windows.Forms.BindingSource(this.components);
            this.groupBoxProject = new System.Windows.Forms.GroupBox();
            this.btnBrowseProject = new System.Windows.Forms.Button();
            this.btnOK = new HydroDesktop.Main.CustomButton();
            this.rbOpenExistingProject = new System.Windows.Forms.RadioButton();
            this.rbEmptyProject = new System.Windows.Forms.RadioButton();
            this.lstProjectTemplates = new HydroDesktop.Main.CustomListBox();
            this.rbNewProjectTemplate = new System.Windows.Forms.RadioButton();
            this.lstRecentProjects = new System.Windows.Forms.ListBox();
            this.lblProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.HelpButton = new System.Windows.Forms.Button();
            this.QuickStartButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnUninstall = new HydroDesktop.Main.CustomButton();
            this.uxFeedSelection = new System.Windows.Forms.ComboBox();
            this.btnInstall = new HydroDesktop.Main.CustomButton();
            this.label2 = new System.Windows.Forms.Label();
            this.uxOnlineProjects = new HydroDesktop.Main.OnlineListBox();
            ((System.ComponentModel.ISupportInitialize)(this.bsRecentFiles)).BeginInit();
            this.groupBoxProject.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
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
            this.lblProductVersion.Location = new System.Drawing.Point(41, 75);
            this.lblProductVersion.Name = "lblProductVersion";
            this.lblProductVersion.Size = new System.Drawing.Size(145, 13);
            this.lblProductVersion.TabIndex = 8;
            this.lblProductVersion.Text = "CUAHSI HydroDesktop 1.5.0";
            // 
            // groupBoxProject
            // 
            this.groupBoxProject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxProject.BackColor = System.Drawing.Color.Snow;
            this.groupBoxProject.Controls.Add(this.btnBrowseProject);
            this.groupBoxProject.Controls.Add(this.btnOK);
            this.groupBoxProject.Controls.Add(this.rbOpenExistingProject);
            this.groupBoxProject.Controls.Add(this.rbEmptyProject);
            this.groupBoxProject.Controls.Add(this.lstProjectTemplates);
            this.groupBoxProject.Controls.Add(this.rbNewProjectTemplate);
            this.groupBoxProject.Controls.Add(this.lstRecentProjects);
            this.groupBoxProject.Location = new System.Drawing.Point(-4, -9);
            this.groupBoxProject.Name = "groupBoxProject";
            this.groupBoxProject.Size = new System.Drawing.Size(308, 238);
            this.groupBoxProject.TabIndex = 13;
            this.groupBoxProject.TabStop = false;
            // 
            // btnBrowseProject
            // 
            this.btnBrowseProject.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowseProject.Image")));
            this.btnBrowseProject.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBrowseProject.Location = new System.Drawing.Point(99, 18);
            this.btnBrowseProject.Name = "btnBrowseProject";
            this.btnBrowseProject.Size = new System.Drawing.Size(78, 22);
            this.btnBrowseProject.TabIndex = 5;
            this.btnBrowseProject.Text = "Browse ...";
            this.btnBrowseProject.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBrowseProject.UseVisualStyleBackColor = true;
            this.btnBrowseProject.Click += new System.EventHandler(this.btnBrowseProject_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnOK.Location = new System.Drawing.Point(215, 187);
            this.btnOK.Name = "btnOK";
            this.btnOK.RoundCorners = ((HydroDesktop.Main.Corners)((((HydroDesktop.Main.Corners.TopLeft | HydroDesktop.Main.Corners.TopRight) 
            | HydroDesktop.Main.Corners.BottomLeft) 
            | HydroDesktop.Main.Corners.BottomRight)));
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // rbOpenExistingProject
            // 
            this.rbOpenExistingProject.AutoSize = true;
            this.rbOpenExistingProject.Location = new System.Drawing.Point(506, 112);
            this.rbOpenExistingProject.Name = "rbOpenExistingProject";
            this.rbOpenExistingProject.Size = new System.Drawing.Size(126, 17);
            this.rbOpenExistingProject.TabIndex = 4;
            this.rbOpenExistingProject.Text = "Open Existing Project";
            this.rbOpenExistingProject.UseVisualStyleBackColor = true;
            this.rbOpenExistingProject.Visible = false;
            // 
            // rbEmptyProject
            // 
            this.rbEmptyProject.AutoSize = true;
            this.rbEmptyProject.Location = new System.Drawing.Point(7, 189);
            this.rbEmptyProject.Name = "rbEmptyProject";
            this.rbEmptyProject.Size = new System.Drawing.Size(149, 17);
            this.rbEmptyProject.TabIndex = 7;
            this.rbEmptyProject.Text = "Create New Empty Project";
            this.rbEmptyProject.UseVisualStyleBackColor = true;
            // 
            // lstProjectTemplates
            // 
            this.lstProjectTemplates.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lstProjectTemplates.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstProjectTemplates.FormattingEnabled = true;
            this.lstProjectTemplates.ItemHeight = 14;
            this.lstProjectTemplates.Location = new System.Drawing.Point(16, 44);
            this.lstProjectTemplates.Name = "lstProjectTemplates";
            this.lstProjectTemplates.Size = new System.Drawing.Size(274, 134);
            this.lstProjectTemplates.TabIndex = 3;
            // 
            // rbNewProjectTemplate
            // 
            this.rbNewProjectTemplate.AutoSize = true;
            this.rbNewProjectTemplate.Checked = true;
            this.rbNewProjectTemplate.Location = new System.Drawing.Point(6, 21);
            this.rbNewProjectTemplate.Name = "rbNewProjectTemplate";
            this.rbNewProjectTemplate.Size = new System.Drawing.Size(87, 17);
            this.rbNewProjectTemplate.TabIndex = 2;
            this.rbNewProjectTemplate.TabStop = true;
            this.rbNewProjectTemplate.Text = "Open Project";
            this.rbNewProjectTemplate.UseVisualStyleBackColor = true;
            // 
            // lstRecentProjects
            // 
            this.lstRecentProjects.FormattingEnabled = true;
            this.lstRecentProjects.Location = new System.Drawing.Point(503, 147);
            this.lstRecentProjects.Name = "lstRecentProjects";
            this.lstRecentProjects.Size = new System.Drawing.Size(245, 56);
            this.lstRecentProjects.TabIndex = 6;
            this.lstRecentProjects.Visible = false;
            this.lstRecentProjects.Click += new System.EventHandler(this.lstRecentProjects_Click);
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
            this.panelStatus.Location = new System.Drawing.Point(8, 221);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(423, 23);
            this.panelStatus.TabIndex = 17;
            // 
            // HelpButton
            // 
            this.HelpButton.Image = ((System.Drawing.Image)(resources.GetObject("HelpButton.Image")));
            this.HelpButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HelpButton.Location = new System.Drawing.Point(20, 164);
            this.HelpButton.Name = "HelpButton";
            this.HelpButton.Size = new System.Drawing.Size(171, 30);
            this.HelpButton.TabIndex = 18;
            this.HelpButton.Text = "View HydroDesktop Help File";
            this.HelpButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.HelpButton.UseVisualStyleBackColor = true;
            this.HelpButton.Click += new System.EventHandler(this.HelpButton_Click);
            // 
            // QuickStartButton
            // 
            this.QuickStartButton.Image = ((System.Drawing.Image)(resources.GetObject("QuickStartButton.Image")));
            this.QuickStartButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.QuickStartButton.Location = new System.Drawing.Point(20, 121);
            this.QuickStartButton.Name = "QuickStartButton";
            this.QuickStartButton.Size = new System.Drawing.Size(171, 30);
            this.QuickStartButton.TabIndex = 9;
            this.QuickStartButton.Text = "View Quick Start Guide";
            this.QuickStartButton.UseVisualStyleBackColor = true;
            this.QuickStartButton.Click += new System.EventHandler(this.QuickStartButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::HydroDesktop.Main.Properties.Resources.welcomeLogo5;
            this.pictureBox1.Location = new System.Drawing.Point(8, 35);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(198, 41);
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(212, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(308, 235);
            this.tabControl1.TabIndex = 20;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Snow;
            this.tabPage1.Controls.Add(this.groupBoxProject);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(300, 209);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Projects";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Snow;
            this.tabPage2.Controls.Add(this.btnUninstall);
            this.tabPage2.Controls.Add(this.uxFeedSelection);
            this.tabPage2.Controls.Add(this.btnInstall);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.uxOnlineProjects);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(300, 209);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Online";
            // 
            // btnUninstall
            // 
            this.btnUninstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUninstall.BackColor = System.Drawing.Color.FromArgb(217, 102, 89);
            this.btnUninstall.Enabled = false;
            this.btnUninstall.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnUninstall.Location = new System.Drawing.Point(211, 178);
            this.btnUninstall.Name = "btnUninstall";
            this.btnUninstall.RoundCorners = ((HydroDesktop.Main.Corners)((((HydroDesktop.Main.Corners.TopLeft | HydroDesktop.Main.Corners.TopRight) 
            | HydroDesktop.Main.Corners.BottomLeft) 
            | HydroDesktop.Main.Corners.BottomRight)));
            this.btnUninstall.Size = new System.Drawing.Size(75, 23);
            this.btnUninstall.TabIndex = 6;
            this.btnUninstall.Text = "Uninstall";
            this.btnUninstall.Visible = false;
            this.btnUninstall.Click += new System.EventHandler(this.btnUninstall_Click);
            // 
            // uxFeedSelection
            // 
            this.uxFeedSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uxFeedSelection.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uxFeedSelection.FormattingEnabled = true;
            this.uxFeedSelection.Items.AddRange(new object[] {
            "Official Sample Projects",
            "User Uploaded Sample Projects"});
            this.uxFeedSelection.Location = new System.Drawing.Point(246, 6);
            this.uxFeedSelection.Name = "uxFeedSelection";
            this.uxFeedSelection.Size = new System.Drawing.Size(179, 23);
            this.uxFeedSelection.TabIndex = 16;
            this.uxFeedSelection.Visible = false;
            // 
            // btnInstall
            // 
            this.btnInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInstall.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btnInstall.Enabled = false;
            this.btnInstall.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnInstall.Location = new System.Drawing.Point(211, 178);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.RoundCorners = ((HydroDesktop.Main.Corners)((((HydroDesktop.Main.Corners.TopLeft | HydroDesktop.Main.Corners.TopRight) 
            | HydroDesktop.Main.Corners.BottomLeft) 
            | HydroDesktop.Main.Corners.BottomRight)));
            this.btnInstall.Size = new System.Drawing.Size(75, 23);
            this.btnInstall.TabIndex = 7;
            this.btnInstall.Text = "Install";
            this.btnInstall.Click += new System.EventHandler(this.btnOKOnline_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(211, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Please select a project to install or uninstall:";
            // 
            // uxOnlineProjects
            // 
            this.uxOnlineProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxOnlineProjects.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.uxOnlineProjects.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uxOnlineProjects.FormattingEnabled = true;
            this.uxOnlineProjects.ItemHeight = 14;
            this.uxOnlineProjects.Location = new System.Drawing.Point(12, 35);
            this.uxOnlineProjects.Name = "uxOnlineProjects";
            this.uxOnlineProjects.Size = new System.Drawing.Size(274, 134);
            this.uxOnlineProjects.TabIndex = 5;
            // 
            // WelcomeScreen
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Snow;
            this.ClientSize = new System.Drawing.Size(524, 252);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.HelpButton);
            this.Controls.Add(this.QuickStartButton);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.lblProductVersion);
            this.Controls.Add(this.checkBox1);
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
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label lblProductVersion;
        private System.Windows.Forms.BindingSource bsRecentFiles;
        private System.Windows.Forms.GroupBox groupBoxProject;
        private System.Windows.Forms.RadioButton rbEmptyProject;
        private System.Windows.Forms.RadioButton rbNewProjectTemplate;
        private CustomButton btnOK;
        private System.Windows.Forms.Button btnBrowseProject;
        private System.Windows.Forms.ToolStripStatusLabel lblProgress;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.Button QuickStartButton;
        private System.Windows.Forms.Button HelpButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox uxFeedSelection;
        private OnlineListBox uxOnlineProjects;
        private CustomButton btnInstall;
        private System.Windows.Forms.RadioButton rbOpenExistingProject;
        private System.Windows.Forms.ListBox lstRecentProjects;
        private CustomButton btnUninstall;
        private CustomListBox lstProjectTemplates;
    }
}