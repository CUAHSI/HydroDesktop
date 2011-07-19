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
            this.paDatasourceSelector = new System.Windows.Forms.Panel();
            this.searchDataGridView = new HydroDesktop.Search.SearchDataGridView();
            this.paDatasourceSelector.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDataSeries
            // 
            this.lblDataSeries.AutoSize = true;
            this.lblDataSeries.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDataSeries.Location = new System.Drawing.Point(0, 34);
            this.lblDataSeries.Name = "lblDataSeries";
            this.lblDataSeries.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.lblDataSeries.Size = new System.Drawing.Size(191, 19);
            this.lblDataSeries.TabIndex = 38;
            this.lblDataSeries.Text = "Select specific data series to download";
            // 
            // lcSelectDatSource
            // 
            this.lcSelectDatSource.AutoSize = true;
            this.lcSelectDatSource.Location = new System.Drawing.Point(5, 9);
            this.lcSelectDatSource.Name = "lcSelectDatSource";
            this.lcSelectDatSource.Size = new System.Drawing.Size(103, 13);
            this.lcSelectDatSource.TabIndex = 39;
            this.lcSelectDatSource.Text = "Select Data Source:";
            // 
            // comboDataSource
            // 
            this.comboDataSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboDataSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDataSource.FormattingEnabled = true;
            this.comboDataSource.Location = new System.Drawing.Point(114, 6);
            this.comboDataSource.Name = "comboDataSource";
            this.comboDataSource.Size = new System.Drawing.Size(165, 21);
            this.comboDataSource.TabIndex = 40;
            // 
            // paDatasourceSelector
            // 
            this.paDatasourceSelector.Controls.Add(this.lcSelectDatSource);
            this.paDatasourceSelector.Controls.Add(this.comboDataSource);
            this.paDatasourceSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.paDatasourceSelector.Location = new System.Drawing.Point(0, 0);
            this.paDatasourceSelector.Name = "paDatasourceSelector";
            this.paDatasourceSelector.Size = new System.Drawing.Size(290, 34);
            this.paDatasourceSelector.TabIndex = 0;
            // 
            // searchDataGridView
            // 
            this.searchDataGridView.AllowUserToAddRows = false;
            this.searchDataGridView.BackgroundColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.searchDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.searchDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.searchDataGridView.Location = new System.Drawing.Point(0, 53);
            this.searchDataGridView.Name = "searchDataGridView";
            this.searchDataGridView.RowHeadersWidth = 25;
            this.searchDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.searchDataGridView.Size = new System.Drawing.Size(290, 254);
            this.searchDataGridView.TabIndex = 32;
            this.searchDataGridView.ZoomToSelected = false;
            // 
            // SearchResultsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.searchDataGridView);
            this.Controls.Add(this.lblDataSeries);
            this.Controls.Add(this.paDatasourceSelector);
            this.Name = "SearchResultsControl";
            this.Size = new System.Drawing.Size(290, 307);
            this.paDatasourceSelector.ResumeLayout(false);
            this.paDatasourceSelector.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SearchDataGridView searchDataGridView;
        private System.Windows.Forms.Label lblDataSeries;
        private System.Windows.Forms.Label lcSelectDatSource;
        private System.Windows.Forms.ComboBox comboDataSource;
        private System.Windows.Forms.Panel paDatasourceSelector;
    }
}
