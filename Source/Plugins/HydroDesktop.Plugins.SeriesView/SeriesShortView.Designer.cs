namespace HydroDesktop.Plugins.SeriesView
{
    partial class SeriesShortView
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
            this.lblBeginDateTime = new System.Windows.Forms.Label();
            this.lblBeginDateTimeUTC = new System.Windows.Forms.Label();
            this.lblEndDateTimeUTC = new System.Windows.Forms.Label();
            this.lblEndDateTime = new System.Windows.Forms.Label();
            this.lblValueCount = new System.Windows.Forms.Label();
            this.tbValueCount = new System.Windows.Forms.TextBox();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.tbBeginDateTime = new System.Windows.Forms.TextBox();
            this.tbBeginDateTimeUTC = new System.Windows.Forms.TextBox();
            this.tbEndDateTime = new System.Windows.Forms.TextBox();
            this.tbEndDateTimeUTC = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblBeginDateTime
            // 
            this.lblBeginDateTime.AutoSize = true;
            this.lblBeginDateTime.Location = new System.Drawing.Point(4, 14);
            this.lblBeginDateTime.Name = "lblBeginDateTime";
            this.lblBeginDateTime.Size = new System.Drawing.Size(80, 13);
            this.lblBeginDateTime.TabIndex = 6;
            this.lblBeginDateTime.Text = "BeginDateTime";
            // 
            // lblBeginDateTimeUTC
            // 
            this.lblBeginDateTimeUTC.AutoSize = true;
            this.lblBeginDateTimeUTC.Location = new System.Drawing.Point(4, 39);
            this.lblBeginDateTimeUTC.Name = "lblBeginDateTimeUTC";
            this.lblBeginDateTimeUTC.Size = new System.Drawing.Size(102, 13);
            this.lblBeginDateTimeUTC.TabIndex = 12;
            this.lblBeginDateTimeUTC.Text = "BeginDateTimeUTC";
            // 
            // lblEndDateTimeUTC
            // 
            this.lblEndDateTimeUTC.AutoSize = true;
            this.lblEndDateTimeUTC.Location = new System.Drawing.Point(5, 89);
            this.lblEndDateTimeUTC.Name = "lblEndDateTimeUTC";
            this.lblEndDateTimeUTC.Size = new System.Drawing.Size(94, 13);
            this.lblEndDateTimeUTC.TabIndex = 16;
            this.lblEndDateTimeUTC.Text = "EndDateTimeUTC";
            // 
            // lblEndDateTime
            // 
            this.lblEndDateTime.AutoSize = true;
            this.lblEndDateTime.Location = new System.Drawing.Point(5, 64);
            this.lblEndDateTime.Name = "lblEndDateTime";
            this.lblEndDateTime.Size = new System.Drawing.Size(72, 13);
            this.lblEndDateTime.TabIndex = 14;
            this.lblEndDateTime.Text = "EndDateTime";
            // 
            // lblValueCount
            // 
            this.lblValueCount.AutoSize = true;
            this.lblValueCount.Location = new System.Drawing.Point(5, 114);
            this.lblValueCount.Name = "lblValueCount";
            this.lblValueCount.Size = new System.Drawing.Size(62, 13);
            this.lblValueCount.TabIndex = 18;
            this.lblValueCount.Text = "ValueCount";
            // 
            // tbValueCount
            // 
            this.tbValueCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbValueCount.Location = new System.Drawing.Point(114, 111);
            this.tbValueCount.Name = "tbValueCount";
            this.tbValueCount.Size = new System.Drawing.Size(188, 20);
            this.tbValueCount.TabIndex = 4;
            // 
            // tbBeginDateTime
            // 
            this.tbBeginDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBeginDateTime.Location = new System.Drawing.Point(114, 11);
            this.tbBeginDateTime.Name = "tbBeginDateTime";
            this.tbBeginDateTime.Size = new System.Drawing.Size(188, 20);
            this.tbBeginDateTime.TabIndex = 0;
            // 
            // tbBeginDateTimeUTC
            // 
            this.tbBeginDateTimeUTC.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBeginDateTimeUTC.Location = new System.Drawing.Point(114, 36);
            this.tbBeginDateTimeUTC.Name = "tbBeginDateTimeUTC";
            this.tbBeginDateTimeUTC.Size = new System.Drawing.Size(188, 20);
            this.tbBeginDateTimeUTC.TabIndex = 1;
            // 
            // tbEndDateTime
            // 
            this.tbEndDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbEndDateTime.Location = new System.Drawing.Point(114, 61);
            this.tbEndDateTime.Name = "tbEndDateTime";
            this.tbEndDateTime.Size = new System.Drawing.Size(188, 20);
            this.tbEndDateTime.TabIndex = 2;
            // 
            // tbEndDateTimeUTC
            // 
            this.tbEndDateTimeUTC.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbEndDateTimeUTC.Location = new System.Drawing.Point(114, 86);
            this.tbEndDateTimeUTC.Name = "tbEndDateTimeUTC";
            this.tbEndDateTimeUTC.Size = new System.Drawing.Size(188, 20);
            this.tbEndDateTimeUTC.TabIndex = 3;
            // 
            // SeriesShortView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbEndDateTimeUTC);
            this.Controls.Add(this.tbEndDateTime);
            this.Controls.Add(this.tbBeginDateTimeUTC);
            this.Controls.Add(this.tbBeginDateTime);
            this.Controls.Add(this.tbValueCount);
            this.Controls.Add(this.lblValueCount);
            this.Controls.Add(this.lblEndDateTimeUTC);
            this.Controls.Add(this.lblEndDateTime);
            this.Controls.Add(this.lblBeginDateTimeUTC);
            this.Controls.Add(this.lblBeginDateTime);
            this.Name = "SeriesShortView";
            this.Size = new System.Drawing.Size(309, 146);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Label lblBeginDateTime;
        private System.Windows.Forms.Label lblBeginDateTimeUTC;
        private System.Windows.Forms.Label lblEndDateTimeUTC;
        private System.Windows.Forms.Label lblEndDateTime;
        private System.Windows.Forms.Label lblValueCount;
        private System.Windows.Forms.TextBox tbValueCount;
        private System.Windows.Forms.TextBox tbBeginDateTime;
        private System.Windows.Forms.TextBox tbBeginDateTimeUTC;
        private System.Windows.Forms.TextBox tbEndDateTime;
        private System.Windows.Forms.TextBox tbEndDateTimeUTC;
    }
}
