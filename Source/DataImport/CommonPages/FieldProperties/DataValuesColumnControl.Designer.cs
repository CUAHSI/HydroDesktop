namespace DataImport.CommonPages.FieldProperties
{
    partial class DataValuesColumnControl
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
            this.lblColumnCount = new System.Windows.Forms.Label();
            this.cmbColumns = new System.Windows.Forms.ComboBox();
            this.btnDetails = new System.Windows.Forms.Button();
            this.btnMore = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblColumnCount
            // 
            this.lblColumnCount.AutoSize = true;
            this.lblColumnCount.Location = new System.Drawing.Point(8, 12);
            this.lblColumnCount.Name = "lblColumnCount";
            this.lblColumnCount.Size = new System.Drawing.Size(139, 13);
            this.lblColumnCount.TabIndex = 0;
            this.lblColumnCount.Text = "Specify Data values column";
            // 
            // cmbColumns
            // 
            this.cmbColumns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbColumns.FormattingEnabled = true;
            this.cmbColumns.Location = new System.Drawing.Point(165, 9);
            this.cmbColumns.Name = "cmbColumns";
            this.cmbColumns.Size = new System.Drawing.Size(127, 21);
            this.cmbColumns.TabIndex = 1;
            // 
            // btnDetails
            // 
            this.btnDetails.Location = new System.Drawing.Point(301, 7);
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.Size = new System.Drawing.Size(75, 23);
            this.btnDetails.TabIndex = 2;
            this.btnDetails.Text = "Details...";
            this.btnDetails.UseVisualStyleBackColor = true;
            this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
            // 
            // btnMore
            // 
            this.btnMore.Location = new System.Drawing.Point(382, 7);
            this.btnMore.Name = "btnMore";
            this.btnMore.Size = new System.Drawing.Size(110, 23);
            this.btnMore.TabIndex = 3;
            this.btnMore.Text = "More Columns...";
            this.btnMore.UseVisualStyleBackColor = true;
            this.btnMore.Click += new System.EventHandler(this.btnMore_Click);
            // 
            // DataValuesColumnControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnMore);
            this.Controls.Add(this.btnDetails);
            this.Controls.Add(this.cmbColumns);
            this.Controls.Add(this.lblColumnCount);
            this.Name = "DataValuesColumnControl";
            this.Size = new System.Drawing.Size(501, 39);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblColumnCount;
        private System.Windows.Forms.ComboBox cmbColumns;
        private System.Windows.Forms.Button btnDetails;
        private System.Windows.Forms.Button btnMore;
    }
}
