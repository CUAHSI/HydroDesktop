namespace TableView
{
    partial class cTableView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataViewSeries = new System.Windows.Forms.DataGridView();
            this.dataGridViewNavigator1 = new TableView.DataGridViewNavigator();
            ((System.ComponentModel.ISupportInitialize)(this.dataViewSeries)).BeginInit();
            this.SuspendLayout();
            // 
            // dataViewSeries
            // 
            this.dataViewSeries.AllowDrop = true;
            this.dataViewSeries.AllowUserToAddRows = false;
            this.dataViewSeries.AllowUserToDeleteRows = false;
            this.dataViewSeries.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.AliceBlue;
            this.dataViewSeries.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataViewSeries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataViewSeries.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataViewSeries.CausesValidation = false;
            this.dataViewSeries.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.ScrollBar;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataViewSeries.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataViewSeries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataViewSeries.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataViewSeries.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataViewSeries.EnableHeadersVisualStyles = false;
            this.dataViewSeries.Location = new System.Drawing.Point(0, 40);
            this.dataViewSeries.Name = "dataViewSeries";
            this.dataViewSeries.ReadOnly = true;
            this.dataViewSeries.RowHeadersVisible = false;
            this.dataViewSeries.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataViewSeries.ShowCellErrors = false;
            this.dataViewSeries.ShowCellToolTips = false;
            this.dataViewSeries.ShowEditingIcon = false;
            this.dataViewSeries.ShowRowErrors = false;
            this.dataViewSeries.Size = new System.Drawing.Size(628, 512);
            this.dataViewSeries.TabIndex = 7;
            // 
            // dataGridViewNavigator1
            // 
            this.dataGridViewNavigator1.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewNavigator1.Name = "dataGridViewNavigator1";
            this.dataGridViewNavigator1.Size = new System.Drawing.Size(352, 31);
            this.dataGridViewNavigator1.TabIndex = 10;
            this.dataGridViewNavigator1.ValuesPerPage = 1000;
            // 
            // cTableView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.dataGridViewNavigator1);
            this.Controls.Add(this.dataViewSeries);
            this.Name = "cTableView";
            this.Size = new System.Drawing.Size(628, 552);
            this.Load += new System.EventHandler(this.cTableView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataViewSeries)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataViewSeries;
        private DataGridViewNavigator dataGridViewNavigator1;


    }
}
