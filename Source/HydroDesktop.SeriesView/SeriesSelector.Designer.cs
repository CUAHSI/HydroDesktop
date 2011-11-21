using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeriesView
{
    public partial class SeriesSelector
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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.btnUncheckAll = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panelComplexFilter = new System.Windows.Forms.Panel();
            this.btnApplyFilter = new System.Windows.Forms.Button();
            this.btnEditFilter = new System.Windows.Forms.Button();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.dgvSeries = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.radAll = new System.Windows.Forms.RadioButton();
            this.cbBoxCriterion = new System.Windows.Forms.ComboBox();
            this.cbBoxContent = new System.Windows.Forms.ComboBox();
            this.radComplex = new System.Windows.Forms.RadioButton();
            this.radSimple = new System.Windows.Forms.RadioButton();
            this.bgwTable2Txt = new System.ComponentModel.BackgroundWorker();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panelComplexFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSeries)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Controls.Add(this.btnCheckAll);
            this.panel1.Controls.Add(this.btnUncheckAll);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Location = new System.Drawing.Point(3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(245, 420);
            this.panel1.TabIndex = 20;
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Location = new System.Drawing.Point(66, 4);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(80, 20);
            this.btnCheckAll.TabIndex = 19;
            this.btnCheckAll.Text = "Check All";
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Visible = false;
            // 
            // btnUncheckAll
            // 
            this.btnUncheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUncheckAll.Location = new System.Drawing.Point(149, 4);
            this.btnUncheckAll.Name = "btnUncheckAll";
            this.btnUncheckAll.Size = new System.Drawing.Size(80, 20);
            this.btnUncheckAll.TabIndex = 18;
            this.btnUncheckAll.Text = "Uncheck All";
            this.btnUncheckAll.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.panelComplexFilter);
            this.groupBox1.Controls.Add(this.dgvSeries);
            this.groupBox1.Controls.Add(this.radAll);
            this.groupBox1.Controls.Add(this.cbBoxCriterion);
            this.groupBox1.Controls.Add(this.cbBoxContent);
            this.groupBox1.Controls.Add(this.radComplex);
            this.groupBox1.Controls.Add(this.radSimple);
            this.groupBox1.Location = new System.Drawing.Point(5, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(237, 395);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Selection Tool";
            // 
            // panelComplexFilter
            // 
            this.panelComplexFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelComplexFilter.Controls.Add(this.btnApplyFilter);
            this.panelComplexFilter.Controls.Add(this.btnEditFilter);
            this.panelComplexFilter.Controls.Add(this.txtFilter);
            this.panelComplexFilter.Location = new System.Drawing.Point(6, 34);
            this.panelComplexFilter.Name = "panelComplexFilter";
            this.panelComplexFilter.Size = new System.Drawing.Size(226, 49);
            this.panelComplexFilter.TabIndex = 19;
            this.panelComplexFilter.Visible = false;
            // 
            // btnApplyFilter
            // 
            this.btnApplyFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplyFilter.Location = new System.Drawing.Point(158, 26);
            this.btnApplyFilter.Name = "btnApplyFilter";
            this.btnApplyFilter.Size = new System.Drawing.Size(60, 20);
            this.btnApplyFilter.TabIndex = 2;
            this.btnApplyFilter.Text = "Apply";
            this.btnApplyFilter.UseVisualStyleBackColor = true;
            this.btnApplyFilter.Click += new System.EventHandler(this.btnApplyFilter_Click);
            // 
            // btnEditFilter
            // 
            this.btnEditFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditFilter.Location = new System.Drawing.Point(159, 3);
            this.btnEditFilter.Name = "btnEditFilter";
            this.btnEditFilter.Size = new System.Drawing.Size(60, 20);
            this.btnEditFilter.TabIndex = 1;
            this.btnEditFilter.Text = "Edit..";
            this.btnEditFilter.UseVisualStyleBackColor = true;
            this.btnEditFilter.Click += new System.EventHandler(this.btnEditFilter_Click);
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilter.Location = new System.Drawing.Point(3, 3);
            this.txtFilter.Multiline = true;
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(150, 43);
            this.txtFilter.TabIndex = 0;
            // 
            // dgvSeries
            // 
            this.dgvSeries.AllowUserToAddRows = false;
            this.dgvSeries.AllowUserToDeleteRows = false;
            this.dgvSeries.AllowUserToResizeColumns = false;
            this.dgvSeries.AllowUserToResizeRows = false;
            this.dgvSeries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSeries.BackgroundColor = System.Drawing.Color.White;
            this.dgvSeries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSeries.ColumnHeadersVisible = false;
            this.dgvSeries.ContextMenuStrip = this.contextMenuStrip1;
            this.dgvSeries.Location = new System.Drawing.Point(7, 89);
            this.dgvSeries.MultiSelect = false;
            this.dgvSeries.Name = "dgvSeries";
            this.dgvSeries.RowHeadersVisible = false;
            this.dgvSeries.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSeries.Size = new System.Drawing.Size(224, 300);
            this.dgvSeries.TabIndex = 18;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.propertiesToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 92);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteToolStripMenuItem.Text = "Delete Series";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportToolStripMenuItem.Text = "Export Series";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // radAll
            // 
            this.radAll.AutoSize = true;
            this.radAll.Checked = true;
            this.radAll.Location = new System.Drawing.Point(15, 13);
            this.radAll.Name = "radAll";
            this.radAll.Size = new System.Drawing.Size(44, 17);
            this.radAll.TabIndex = 17;
            this.radAll.TabStop = true;
            this.radAll.Text = "ALL";
            this.radAll.UseVisualStyleBackColor = true;
            // 
            // cbBoxCriterion
            // 
            this.cbBoxCriterion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbBoxCriterion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBoxCriterion.FormattingEnabled = true;
            this.cbBoxCriterion.Location = new System.Drawing.Point(6, 34);
            this.cbBoxCriterion.Name = "cbBoxCriterion";
            this.cbBoxCriterion.Size = new System.Drawing.Size(225, 21);
            this.cbBoxCriterion.TabIndex = 13;
            // 
            // cbBoxContent
            // 
            this.cbBoxContent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbBoxContent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBoxContent.FormattingEnabled = true;
            this.cbBoxContent.Location = new System.Drawing.Point(6, 61);
            this.cbBoxContent.Name = "cbBoxContent";
            this.cbBoxContent.Size = new System.Drawing.Size(225, 21);
            this.cbBoxContent.TabIndex = 14;
            // 
            // radComplex
            // 
            this.radComplex.AutoSize = true;
            this.radComplex.Location = new System.Drawing.Point(142, 13);
            this.radComplex.Name = "radComplex";
            this.radComplex.Size = new System.Drawing.Size(90, 17);
            this.radComplex.TabIndex = 12;
            this.radComplex.Text = "Complex Filter";
            this.radComplex.UseVisualStyleBackColor = true;
            // 
            // radSimple
            // 
            this.radSimple.AutoSize = true;
            this.radSimple.Location = new System.Drawing.Point(61, 13);
            this.radSimple.Name = "radSimple";
            this.radSimple.Size = new System.Drawing.Size(81, 17);
            this.radSimple.TabIndex = 11;
            this.radSimple.Text = "Simple Filter";
            this.radSimple.UseVisualStyleBackColor = true;
            // 
            // bgwTable2Txt
            // 
            this.bgwTable2Txt.WorkerReportsProgress = true;
            this.bgwTable2Txt.WorkerSupportsCancellation = true;
            this.bgwTable2Txt.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwTable2Txt_DoWork);
            this.bgwTable2Txt.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwTable2Txt_RunWorkerCompleted);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(5, 4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(59, 20);
            this.btnRefresh.TabIndex = 20;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // SeriesSelector
            // 
            this.Controls.Add(this.panel1);
            this.Name = "SeriesSelector";
            this.Size = new System.Drawing.Size(250, 425);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panelComplexFilter.ResumeLayout(false);
            this.panelComplexFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSeries)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCheckAll;
        private System.Windows.Forms.Button btnUncheckAll;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radAll;
        private System.Windows.Forms.ComboBox cbBoxCriterion;
        private System.Windows.Forms.ComboBox cbBoxContent;
        private System.Windows.Forms.RadioButton radComplex;
        private System.Windows.Forms.DataGridView dgvSeries;
        private System.Windows.Forms.RadioButton radSimple;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Panel panelComplexFilter;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Button btnApplyFilter;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.Button btnEditFilter;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.Button btnRefresh;
        private System.ComponentModel.BackgroundWorker bgwTable2Txt;
    }
}
