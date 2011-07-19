namespace HydroDesktop.Search
{
    partial class SearchResultsControl
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
            this.lblDataSeries = new System.Windows.Forms.Label();
            this.lcSelectDatSource = new System.Windows.Forms.Label();
            this.comboDataSource = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.paDatasourceSelector = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.searchDataGridView = new HydroDesktop.Search.SearchDataGridView();
            this.tableLayoutPanel.SuspendLayout();
            this.paDatasourceSelector.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDataSeries
            // 
            this.lblDataSeries.AutoSize = true;
            this.lblDataSeries.Location = new System.Drawing.Point(3, 37);
            this.lblDataSeries.Name = "lblDataSeries";
            this.lblDataSeries.Size = new System.Drawing.Size(191, 13);
            this.lblDataSeries.TabIndex = 38;
            this.lblDataSeries.Text = "Select specific data series to download";
            // 
            // lcSelectDatSource
            // 
            this.lcSelectDatSource.AutoSize = true;
            this.lcSelectDatSource.Location = new System.Drawing.Point(5, 6);
            this.lcSelectDatSource.Name = "lcSelectDatSource";
            this.lcSelectDatSource.Size = new System.Drawing.Size(103, 13);
            this.lcSelectDatSource.TabIndex = 39;
            this.lcSelectDatSource.Text = "Select Data Source:";
            // 
            // comboDataSource
            // 
            this.comboDataSource.FormattingEnabled = true;
            this.comboDataSource.Location = new System.Drawing.Point(114, 3);
            this.comboDataSource.Name = "comboDataSource";
            this.comboDataSource.Size = new System.Drawing.Size(121, 21);
            this.comboDataSource.TabIndex = 40;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.paDatasourceSelector, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.lblDataSeries, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 17F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 157F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(306, 291);
            this.tableLayoutPanel.TabIndex = 41;
            // 
            // paDatasourceSelector
            // 
            this.paDatasourceSelector.Controls.Add(this.lcSelectDatSource);
            this.paDatasourceSelector.Controls.Add(this.comboDataSource);
            this.paDatasourceSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paDatasourceSelector.Location = new System.Drawing.Point(3, 3);
            this.paDatasourceSelector.Name = "paDatasourceSelector";
            this.paDatasourceSelector.Size = new System.Drawing.Size(300, 31);
            this.paDatasourceSelector.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.searchDataGridView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 57);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(300, 231);
            this.panel1.TabIndex = 39;
            // 
            // searchDataGridView
            // 
            this.searchDataGridView.AllowUserToAddRows = false;
            this.searchDataGridView.BackgroundColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.searchDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.searchDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.searchDataGridView.Location = new System.Drawing.Point(0, 0);
            this.searchDataGridView.Name = "searchDataGridView";
            this.searchDataGridView.RowHeadersWidth = 25;
            this.searchDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.searchDataGridView.Size = new System.Drawing.Size(300, 231);
            this.searchDataGridView.TabIndex = 32;
            this.searchDataGridView.ZoomToSelected = false;
            // 
            // SearchResultsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "SearchResultsControl";
            this.Size = new System.Drawing.Size(306, 291);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.paDatasourceSelector.ResumeLayout(false);
            this.paDatasourceSelector.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.searchDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SearchDataGridView searchDataGridView;
        private System.Windows.Forms.Label lblDataSeries;
        private System.Windows.Forms.Label lcSelectDatSource;
        private System.Windows.Forms.ComboBox comboDataSource;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Panel paDatasourceSelector;
        private System.Windows.Forms.Panel panel1;
    }
}
