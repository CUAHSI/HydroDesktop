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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.rbParallel = new System.Windows.Forms.RadioButton();
            this.rbSequence = new System.Windows.Forms.RadioButton();
            this.dataViewSeries = new System.Windows.Forms.DataGridView();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataViewSeries)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // rbParallel
            // 
            this.rbParallel.AutoSize = true;
            this.rbParallel.Location = new System.Drawing.Point(168, 2);
            this.rbParallel.Name = "rbParallel";
            this.rbParallel.Size = new System.Drawing.Size(157, 17);
            this.rbParallel.TabIndex = 6;
            this.rbParallel.TabStop = true;
            this.rbParallel.Text = "Show Just Values in Parallel";
            this.rbParallel.UseVisualStyleBackColor = true;
            this.rbParallel.Click += new System.EventHandler(this.rbParallel_Click);
            // 
            // rbSequence
            // 
            this.rbSequence.AutoSize = true;
            this.rbSequence.Location = new System.Drawing.Point(3, 2);
            this.rbSequence.Name = "rbSequence";
            this.rbSequence.Size = new System.Drawing.Size(159, 17);
            this.rbSequence.TabIndex = 5;
            this.rbSequence.TabStop = true;
            this.rbSequence.Text = "Show All Fields in Sequence";
            this.rbSequence.UseVisualStyleBackColor = true;
            this.rbSequence.Click += new System.EventHandler(this.rbSequence_Click);
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
            this.dataViewSeries.Location = new System.Drawing.Point(3, 21);
            this.dataViewSeries.Name = "dataViewSeries";
            this.dataViewSeries.ReadOnly = true;
            this.dataViewSeries.RowHeadersVisible = false;
            this.dataViewSeries.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataViewSeries.ShowCellErrors = false;
            this.dataViewSeries.ShowCellToolTips = false;
            this.dataViewSeries.ShowEditingIcon = false;
            this.dataViewSeries.ShowRowErrors = false;
            this.dataViewSeries.Size = new System.Drawing.Size(630, 528);
            this.dataViewSeries.TabIndex = 7;
            // 
            // lblDatabase
            // 
            this.lblDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDatabase.BackColor = System.Drawing.Color.LightGray;
            this.lblDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDatabase.Location = new System.Drawing.Point(332, 2);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(301, 17);
            this.lblDatabase.TabIndex = 8;
            this.lblDatabase.Text = "Database: ";
            this.lblDatabase.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cTableView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.lblDatabase);
            this.Controls.Add(this.rbParallel);
            this.Controls.Add(this.rbSequence);
            this.Controls.Add(this.dataViewSeries);
            this.Name = "cTableView";
            this.Size = new System.Drawing.Size(636, 552);
            this.Load += new System.EventHandler(this.cTableView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataViewSeries)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbParallel;
        private System.Windows.Forms.RadioButton rbSequence;
        private System.Windows.Forms.DataGridView dataViewSeries;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.BindingSource bindingSource1;


    }
}
