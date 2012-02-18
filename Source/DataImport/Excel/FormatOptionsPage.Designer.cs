namespace DataImport.Excel
{
    partial class FormatOptionsPage
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
            this.lblExcelSheet = new System.Windows.Forms.Label();
            this.cmbExcelSheet = new System.Windows.Forms.ComboBox();
            this.dgvPreview = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.Size = new System.Drawing.Size(540, 64);
            this.Banner.Subtitle = "Specify file format options.";
            this.Banner.Title = "Format options";
            // 
            // lblExcelSheet
            // 
            this.lblExcelSheet.AutoSize = true;
            this.lblExcelSheet.Location = new System.Drawing.Point(16, 88);
            this.lblExcelSheet.Name = "lblExcelSheet";
            this.lblExcelSheet.Size = new System.Drawing.Size(62, 13);
            this.lblExcelSheet.TabIndex = 1;
            this.lblExcelSheet.Text = "Excel sheet";
            // 
            // cmbExcelSheet
            // 
            this.cmbExcelSheet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExcelSheet.FormattingEnabled = true;
            this.cmbExcelSheet.Location = new System.Drawing.Point(86, 85);
            this.cmbExcelSheet.Name = "cmbExcelSheet";
            this.cmbExcelSheet.Size = new System.Drawing.Size(121, 21);
            this.cmbExcelSheet.TabIndex = 2;
            // 
            // dgvPreview
            // 
            this.dgvPreview.AllowUserToAddRows = false;
            this.dgvPreview.AllowUserToDeleteRows = false;
            this.dgvPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPreview.Location = new System.Drawing.Point(19, 112);
            this.dgvPreview.Name = "dgvPreview";
            this.dgvPreview.ReadOnly = true;
            this.dgvPreview.Size = new System.Drawing.Size(505, 216);
            this.dgvPreview.TabIndex = 11;
            // 
            // FormatOptionsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvPreview);
            this.Controls.Add(this.lblExcelSheet);
            this.Controls.Add(this.cmbExcelSheet);
            this.Name = "FormatOptionsPage";
            this.Size = new System.Drawing.Size(540, 342);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.FormatOptionsPage_SetActive);
            this.Controls.SetChildIndex(this.cmbExcelSheet, 0);
            this.Controls.SetChildIndex(this.lblExcelSheet, 0);
            this.Controls.SetChildIndex(this.Banner, 0);
            this.Controls.SetChildIndex(this.dgvPreview, 0);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblExcelSheet;
        private System.Windows.Forms.ComboBox cmbExcelSheet;
        private System.Windows.Forms.DataGridView dgvPreview;
    }
}
