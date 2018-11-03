using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HydroDesktop.Plugins.MetadataFetcher.Forms
{
    partial class MainForm
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
            var dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            var dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.gbxServices = new System.Windows.Forms.GroupBox();
            this.dgvServices = new System.Windows.Forms.DataGridView();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.gbxProgress = new System.Windows.Forms.GroupBox();
            this.btnCancelDownload = new System.Windows.Forms.Button();
            this.prgMain = new System.Windows.Forms.ProgressBar();
            this.bgwMain = new System.ComponentModel.BackgroundWorker();
            this.lblServicesSelected = new System.Windows.Forms.Label();
            this.lblSelectionCount = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuServiceManagement = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddServices = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuRemoveSelectedServices = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuRefreshServiceList = new System.Windows.Forms.ToolStripMenuItem();
            this.gbxServices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).BeginInit();
            this.gbxProgress.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxServices
            // 
            this.gbxServices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxServices.Controls.Add(this.dgvServices);
            this.gbxServices.Controls.Add(this.chkAll);
            this.gbxServices.Controls.Add(this.btnUpdate);
            this.gbxServices.Location = new System.Drawing.Point(12, 27);
            this.gbxServices.Name = "gbxServices";
            this.gbxServices.Size = new System.Drawing.Size(718, 302);
            this.gbxServices.TabIndex = 1;
            this.gbxServices.TabStop = false;
            this.gbxServices.Text = "Select services and then download metadata to update the metadata cache";
            // 
            // dgvServices
            // 
            this.dgvServices.AllowUserToAddRows = false;
            this.dgvServices.AllowUserToDeleteRows = false;
            this.dgvServices.AllowUserToOrderColumns = true;
            this.dgvServices.AllowUserToResizeRows = false;
            this.dgvServices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvServices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvServices.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvServices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvServices.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvServices.Location = new System.Drawing.Point(6, 19);
            this.dgvServices.MultiSelect = false;
            this.dgvServices.Name = "dgvServices";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvServices.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvServices.RowHeadersVisible = false;
            this.dgvServices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvServices.Size = new System.Drawing.Size(706, 250);
            this.dgvServices.TabIndex = 5;
            this.dgvServices.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvServices_CellValueChanged);
            this.dgvServices.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvServices_CurrentCellDirtyStateChanged);
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Location = new System.Drawing.Point(9, 275);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(70, 17);
            this.chkAll.TabIndex = 1;
            this.chkAll.Text = "Select &All";
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(599, 273);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(113, 23);
            this.btnUpdate.TabIndex = 4;
            this.btnUpdate.Text = "&Download Metadata";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // gbxProgress
            // 
            this.gbxProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxProgress.Controls.Add(this.btnCancelDownload);
            this.gbxProgress.Controls.Add(this.prgMain);
            this.gbxProgress.Location = new System.Drawing.Point(12, 335);
            this.gbxProgress.Name = "gbxProgress";
            this.gbxProgress.Size = new System.Drawing.Size(718, 46);
            this.gbxProgress.TabIndex = 5;
            this.gbxProgress.TabStop = false;
            this.gbxProgress.Text = "Ready";
            this.gbxProgress.Visible = false;
            // 
            // btnCancelDownload
            // 
            this.btnCancelDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelDownload.Enabled = false;
            this.btnCancelDownload.Location = new System.Drawing.Point(627, 16);
            this.btnCancelDownload.Name = "btnCancelDownload";
            this.btnCancelDownload.Size = new System.Drawing.Size(85, 22);
            this.btnCancelDownload.TabIndex = 28;
            this.btnCancelDownload.Text = "Cancel";
            this.btnCancelDownload.UseVisualStyleBackColor = true;
            this.btnCancelDownload.Click += new System.EventHandler(this.btnCancelDownload_Click);
            // 
            // prgMain
            // 
            this.prgMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.prgMain.Location = new System.Drawing.Point(9, 16);
            this.prgMain.Name = "prgMain";
            this.prgMain.Size = new System.Drawing.Size(612, 22);
            this.prgMain.TabIndex = 0;
            // 
            // bgwMain
            // 
            this.bgwMain.WorkerReportsProgress = true;
            this.bgwMain.WorkerSupportsCancellation = true;
            // 
            // lblServicesSelected
            // 
            this.lblServicesSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblServicesSelected.AutoSize = true;
            this.lblServicesSelected.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServicesSelected.Location = new System.Drawing.Point(26, 354);
            this.lblServicesSelected.Name = "lblServicesSelected";
            this.lblServicesSelected.Size = new System.Drawing.Size(125, 15);
            this.lblServicesSelected.TabIndex = 6;
            this.lblServicesSelected.Text = "Services Selected:";
            // 
            // lblSelectionCount
            // 
            this.lblSelectionCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSelectionCount.AutoSize = true;
            this.lblSelectionCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectionCount.Location = new System.Drawing.Point(149, 354);
            this.lblSelectionCount.Name = "lblSelectionCount";
            this.lblSelectionCount.Size = new System.Drawing.Size(15, 15);
            this.lblSelectionCount.TabIndex = 7;
            this.lblSelectionCount.Text = "0";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuServiceManagement});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(742, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuServiceManagement
            // 
            this.mnuServiceManagement.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAddServices,
            this.toolStripSeparator1,
            this.mnuRemoveSelectedServices,
            this.toolStripSeparator2,
            this.mnuRefreshServiceList});
            this.mnuServiceManagement.Name = "mnuServiceManagement";
            this.mnuServiceManagement.Size = new System.Drawing.Size(119, 20);
            this.mnuServiceManagement.Text = "Service Management";
            // 
            // mnuAddServices
            // 
            this.mnuAddServices.Name = "mnuAddServices";
            this.mnuAddServices.Size = new System.Drawing.Size(230, 22);
            this.mnuAddServices.Text = "&Add WaterOneFlow Service(s)";
            this.mnuAddServices.Click += new System.EventHandler(this.mnuAddServices_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(227, 6);
            // 
            // mnuRemoveSelectedServices
            // 
            this.mnuRemoveSelectedServices.Name = "mnuRemoveSelectedServices";
            this.mnuRemoveSelectedServices.Size = new System.Drawing.Size(230, 22);
            this.mnuRemoveSelectedServices.Text = "&Remove Selected Services";
            this.mnuRemoveSelectedServices.Click += new System.EventHandler(this.mnuRemoveSelectedServices_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(227, 6);
            // 
            // mnuRefreshServiceList
            // 
            this.mnuRefreshServiceList.Name = "mnuRefreshServiceList";
            this.mnuRefreshServiceList.Size = new System.Drawing.Size(230, 22);
            this.mnuRefreshServiceList.Text = "Refresh Service List";
            this.mnuRefreshServiceList.Click += new System.EventHandler(this.mnuRefreshServiceList_Click);
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnUpdate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 391);
            this.Controls.Add(this.lblSelectionCount);
            this.Controls.Add(this.lblServicesSelected);
            this.Controls.Add(this.gbxProgress);
            this.Controls.Add(this.gbxServices);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(480, 247);
            this.Name = "MainForm";
            this.Text = "Metadata Fetcher";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.gbxServices.ResumeLayout(false);
            this.gbxServices.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).EndInit();
            this.gbxProgress.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox gbxServices;
        private System.Windows.Forms.Button btnUpdate;
        private CheckBox chkAll;
        private GroupBox gbxProgress;
        private ProgressBar prgMain;
        private BackgroundWorker bgwMain;
        private Button btnCancelDownload;
        private DataGridView dgvServices;
        private Label lblServicesSelected;
        private Label lblSelectionCount;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem mnuServiceManagement;
        private ToolStripMenuItem mnuAddServices;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem mnuRemoveSelectedServices;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem mnuRefreshServiceList;
    }
}
