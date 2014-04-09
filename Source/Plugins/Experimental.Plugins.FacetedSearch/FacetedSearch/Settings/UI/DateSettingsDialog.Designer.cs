namespace FacetedSearch3.Settings.UI
{
    partial class DateSettingsDialog
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
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.rbLastDay = new System.Windows.Forms.RadioButton();
            this.rbLastMonth = new System.Windows.Forms.RadioButton();
            this.rbLastYear = new System.Windows.Forms.RadioButton();
            this.rbLastWeek = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStartDate.Location = new System.Drawing.Point(111, 12);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(138, 20);
            this.dtpStartDate.TabIndex = 0;
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEndDate.Location = new System.Drawing.Point(111, 38);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(138, 20);
            this.dtpEndDate.TabIndex = 1;
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(8, 18);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(56, 13);
            this.lblStartDate.TabIndex = 2;
            this.lblStartDate.Text = "Start date:";
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(11, 44);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(53, 13);
            this.lblEndDate.TabIndex = 3;
            this.lblEndDate.Text = "End date:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(174, 143);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(93, 143);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // rbLastDay
            // 
            this.rbLastDay.AutoSize = true;
            this.rbLastDay.Location = new System.Drawing.Point(14, 76);
            this.rbLastDay.Name = "rbLastDay";
            this.rbLastDay.Size = new System.Drawing.Size(67, 17);
            this.rbLastDay.TabIndex = 2;
            this.rbLastDay.TabStop = true;
            this.rbLastDay.Text = "Last Day";
            this.rbLastDay.UseVisualStyleBackColor = true;
            // 
            // rbLastMonth
            // 
            this.rbLastMonth.AutoSize = true;
            this.rbLastMonth.Location = new System.Drawing.Point(111, 76);
            this.rbLastMonth.Name = "rbLastMonth";
            this.rbLastMonth.Size = new System.Drawing.Size(78, 17);
            this.rbLastMonth.TabIndex = 3;
            this.rbLastMonth.TabStop = true;
            this.rbLastMonth.Text = "Last Month";
            this.rbLastMonth.UseVisualStyleBackColor = true;
            // 
            // rbLastYear
            // 
            this.rbLastYear.AutoSize = true;
            this.rbLastYear.Location = new System.Drawing.Point(111, 97);
            this.rbLastYear.Name = "rbLastYear";
            this.rbLastYear.Size = new System.Drawing.Size(70, 17);
            this.rbLastYear.TabIndex = 5;
            this.rbLastYear.TabStop = true;
            this.rbLastYear.Text = "Last Year";
            this.rbLastYear.UseVisualStyleBackColor = true;
            // 
            // rbLastWeek
            // 
            this.rbLastWeek.AutoSize = true;
            this.rbLastWeek.Location = new System.Drawing.Point(14, 97);
            this.rbLastWeek.Name = "rbLastWeek";
            this.rbLastWeek.Size = new System.Drawing.Size(77, 17);
            this.rbLastWeek.TabIndex = 4;
            this.rbLastWeek.TabStop = true;
            this.rbLastWeek.Text = "Last Week";
            this.rbLastWeek.UseVisualStyleBackColor = true;
            // 
            // DateSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(265, 178);
            this.Controls.Add(this.rbLastWeek);
            this.Controls.Add(this.rbLastYear);
            this.Controls.Add(this.rbLastMonth);
            this.Controls.Add(this.rbLastDay);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblEndDate);
            this.Controls.Add(this.lblStartDate);
            this.Controls.Add(this.dtpEndDate);
            this.Controls.Add(this.dtpStartDate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DateSettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select dates";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.Label lblEndDate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RadioButton rbLastDay;
        private System.Windows.Forms.RadioButton rbLastMonth;
        private System.Windows.Forms.RadioButton rbLastYear;
        private System.Windows.Forms.RadioButton rbLastWeek;
    }
}