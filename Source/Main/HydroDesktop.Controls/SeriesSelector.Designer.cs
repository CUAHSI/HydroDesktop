namespace HydroDesktopControls
{
    partial class SeriesSelector
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
        private void  InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.checkedSeriesList = new System.Windows.Forms.CheckedListBox();
            this.cMenuSeries = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.propertyGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToThemeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radAll = new System.Windows.Forms.RadioButton();
            this.cbBoxCriterion = new System.Windows.Forms.ComboBox();
            this.cbBoxContent = new System.Windows.Forms.ComboBox();
            this.radComplex = new System.Windows.Forms.RadioButton();
            this.radSimple = new System.Windows.Forms.RadioButton();
            this.bgwTable2Txt = new System.ComponentModel.BackgroundWorker();
            this.btnUncheckAll = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.cMenuSeries.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkedSeriesList
            // 
            this.checkedSeriesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedSeriesList.ContextMenuStrip = this.cMenuSeries;
            this.checkedSeriesList.FormattingEnabled = true;
            this.checkedSeriesList.HorizontalScrollbar = true;
            this.checkedSeriesList.Location = new System.Drawing.Point(6, 88);
            this.checkedSeriesList.Name = "checkedSeriesList";
            this.checkedSeriesList.ScrollAlwaysVisible = true;
            this.checkedSeriesList.Size = new System.Drawing.Size(225, 289);
            this.checkedSeriesList.TabIndex = 15;
            
            
            // 
            // cMenuSeries
            // 
            this.cMenuSeries.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.propertyGridToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveToThemeToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.cMenuSeries.Name = "cMenuSeries";
            this.cMenuSeries.Size = new System.Drawing.Size(156, 76);
            // 
            // propertyGridToolStripMenuItem
            // 
            this.propertyGridToolStripMenuItem.Name = "propertyGridToolStripMenuItem";
            this.propertyGridToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.propertyGridToolStripMenuItem.Text = "Properties";
            //this.propertyGridToolStripMenuItem.Click += new System.EventHandler(this.propertyGridToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(152, 6);
            // 
            // saveToThemeToolStripMenuItem
            // 
            this.saveToThemeToolStripMenuItem.Name = "saveToThemeToolStripMenuItem";
            this.saveToThemeToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.saveToThemeToolStripMenuItem.Text = "Save to Theme ";
            //this.saveToThemeToolStripMenuItem.Click += new System.EventHandler(this.saveToThemeToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exportToolStripMenuItem.Text = "Export";
            //this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.radAll);
            this.groupBox1.Controls.Add(this.checkedSeriesList);
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
            // radAll
            // 
            this.radAll.AutoSize = true;
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
            this.radComplex.TabStop = true;
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
            this.radSimple.TabStop = true;
            this.radSimple.Text = "Simple Filter";
            this.radSimple.UseVisualStyleBackColor = true;
            // 
            // bgwTable2Txt
            // 
            //this.bgwTable2Txt.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwTable2Txt_DoWork);
            //this.bgwTable2Txt.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwTable2Txt_RunWorkerCompleted);
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
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.btnCheckAll);
            this.panel1.Controls.Add(this.btnUncheckAll);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Location = new System.Drawing.Point(4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(245, 420);
            this.panel1.TabIndex = 19;
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
            // SeriesSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "SeriesSelector";
            this.Size = new System.Drawing.Size(250, 425);
            
            this.cMenuSeries.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedSeriesList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbBoxCriterion;
        private System.Windows.Forms.ComboBox cbBoxContent;
        private System.Windows.Forms.RadioButton radComplex;
        private System.Windows.Forms.RadioButton radSimple;
        private System.Windows.Forms.RadioButton radAll;
        private System.Windows.Forms.ContextMenuStrip cMenuSeries;
        private System.Windows.Forms.ToolStripMenuItem propertyGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveToThemeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker bgwTable2Txt;
        private System.Windows.Forms.Button btnUncheckAll;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCheckAll;
    }
}
