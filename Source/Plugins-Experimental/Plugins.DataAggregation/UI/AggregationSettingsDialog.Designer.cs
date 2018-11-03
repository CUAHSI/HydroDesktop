namespace HydroDesktop.Plugins.DataAggregation.UI
{
    partial class AggregationSettingsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AggregationSettingsDialog));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblType = new System.Windows.Forms.Label();
            this.cmbMode = new System.Windows.Forms.ComboBox();
            this.lblStartTime = new System.Windows.Forms.Label();
            this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
            this.lblEndTime = new System.Windows.Forms.Label();
            this.dtpEndTime = new System.Windows.Forms.DateTimePicker();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.paSettings = new System.Windows.Forms.Panel();
            this.lblDecimalPlaces = new System.Windows.Forms.Label();
            this.nudDecimalPlaces = new System.Windows.Forms.NumericUpDown();
            this.chbCreateCategories = new System.Windows.Forms.CheckBox();
            this.chbCreateNewLayer = new System.Windows.Forms.CheckBox();
            this.lblVariable = new System.Windows.Forms.Label();
            this.cmbVariable = new System.Windows.Forms.ComboBox();
            this.lblProgress = new System.Windows.Forms.Label();
            this.paSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDecimalPlaces)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(153, 315);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(234, 315);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(14, 24);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(103, 13);
            this.lblType.TabIndex = 2;
            this.lblType.Text = "Type of Aggregation";
            // 
            // cmbMode
            // 
            this.cmbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMode.FormattingEnabled = true;
            this.cmbMode.Location = new System.Drawing.Point(119, 21);
            this.cmbMode.Name = "cmbMode";
            this.cmbMode.Size = new System.Drawing.Size(168, 21);
            this.cmbMode.TabIndex = 3;
            // 
            // lblStartTime
            // 
            this.lblStartTime.AutoSize = true;
            this.lblStartTime.Location = new System.Drawing.Point(14, 95);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(55, 13);
            this.lblStartTime.TabIndex = 4;
            this.lblStartTime.Text = "Start Time";
            // 
            // dtpStartTime
            // 
            this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStartTime.Location = new System.Drawing.Point(119, 89);
            this.dtpStartTime.Name = "dtpStartTime";
            this.dtpStartTime.Size = new System.Drawing.Size(168, 20);
            this.dtpStartTime.TabIndex = 5;
            // 
            // lblEndTime
            // 
            this.lblEndTime.AutoSize = true;
            this.lblEndTime.Location = new System.Drawing.Point(14, 128);
            this.lblEndTime.Name = "lblEndTime";
            this.lblEndTime.Size = new System.Drawing.Size(52, 13);
            this.lblEndTime.TabIndex = 6;
            this.lblEndTime.Text = "End Time";
            // 
            // dtpEndTime
            // 
            this.dtpEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEndTime.Location = new System.Drawing.Point(119, 122);
            this.dtpEndTime.Name = "dtpEndTime";
            this.dtpEndTime.Size = new System.Drawing.Size(168, 20);
            this.dtpEndTime.TabIndex = 7;
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgress.Location = new System.Drawing.Point(12, 258);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(297, 23);
            this.pbProgress.TabIndex = 8;
            this.pbProgress.Visible = false;
            // 
            // paSettings
            // 
            this.paSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.paSettings.Controls.Add(this.lblDecimalPlaces);
            this.paSettings.Controls.Add(this.nudDecimalPlaces);
            this.paSettings.Controls.Add(this.chbCreateCategories);
            this.paSettings.Controls.Add(this.chbCreateNewLayer);
            this.paSettings.Controls.Add(this.lblVariable);
            this.paSettings.Controls.Add(this.cmbVariable);
            this.paSettings.Controls.Add(this.lblType);
            this.paSettings.Controls.Add(this.cmbMode);
            this.paSettings.Controls.Add(this.dtpEndTime);
            this.paSettings.Controls.Add(this.lblStartTime);
            this.paSettings.Controls.Add(this.lblEndTime);
            this.paSettings.Controls.Add(this.dtpStartTime);
            this.paSettings.Location = new System.Drawing.Point(12, 12);
            this.paSettings.Name = "paSettings";
            this.paSettings.Size = new System.Drawing.Size(297, 235);
            this.paSettings.TabIndex = 9;
            // 
            // lblDecimalPlaces
            // 
            this.lblDecimalPlaces.AutoSize = true;
            this.lblDecimalPlaces.Location = new System.Drawing.Point(14, 157);
            this.lblDecimalPlaces.Name = "lblDecimalPlaces";
            this.lblDecimalPlaces.Size = new System.Drawing.Size(80, 13);
            this.lblDecimalPlaces.TabIndex = 13;
            this.lblDecimalPlaces.Text = "Decimal Places";
            // 
            // nudDecimalPlaces
            // 
            this.nudDecimalPlaces.Location = new System.Drawing.Point(119, 155);
            this.nudDecimalPlaces.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudDecimalPlaces.Name = "nudDecimalPlaces";
            this.nudDecimalPlaces.Size = new System.Drawing.Size(168, 20);
            this.nudDecimalPlaces.TabIndex = 12;
            // 
            // chbCreateCategories
            // 
            this.chbCreateCategories.AutoSize = true;
            this.chbCreateCategories.Location = new System.Drawing.Point(17, 208);
            this.chbCreateCategories.Name = "chbCreateCategories";
            this.chbCreateCategories.Size = new System.Drawing.Size(109, 17);
            this.chbCreateCategories.TabIndex = 11;
            this.chbCreateCategories.Text = "Create categories";
            this.chbCreateCategories.UseVisualStyleBackColor = true;
            // 
            // chbCreateNewLayer
            // 
            this.chbCreateNewLayer.AutoSize = true;
            this.chbCreateNewLayer.Location = new System.Drawing.Point(17, 185);
            this.chbCreateNewLayer.Name = "chbCreateNewLayer";
            this.chbCreateNewLayer.Size = new System.Drawing.Size(105, 17);
            this.chbCreateNewLayer.TabIndex = 10;
            this.chbCreateNewLayer.Text = "Create new layer";
            this.chbCreateNewLayer.UseVisualStyleBackColor = true;
            // 
            // lblVariable
            // 
            this.lblVariable.AutoSize = true;
            this.lblVariable.Location = new System.Drawing.Point(14, 58);
            this.lblVariable.Name = "lblVariable";
            this.lblVariable.Size = new System.Drawing.Size(45, 13);
            this.lblVariable.TabIndex = 8;
            this.lblVariable.Text = "Variable";
            // 
            // cmbVariable
            // 
            this.cmbVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVariable.FormattingEnabled = true;
            this.cmbVariable.Location = new System.Drawing.Point(119, 55);
            this.cmbVariable.Name = "cmbVariable";
            this.cmbVariable.Size = new System.Drawing.Size(168, 21);
            this.cmbVariable.TabIndex = 9;
            // 
            // lblProgress
            // 
            this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(9, 291);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(93, 13);
            this.lblProgress.TabIndex = 10;
            this.lblProgress.Text = "Current Operation:";
            this.lblProgress.Visible = false;
            // 
            // AggregationSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 360);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.paSettings);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AggregationSettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Aggregation Settings";
            this.paSettings.ResumeLayout(false);
            this.paSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDecimalPlaces)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cmbMode;
        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.Label lblEndTime;
        private System.Windows.Forms.DateTimePicker dtpEndTime;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Panel paSettings;
        private System.Windows.Forms.Label lblVariable;
        private System.Windows.Forms.ComboBox cmbVariable;
        private System.Windows.Forms.CheckBox chbCreateNewLayer;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.CheckBox chbCreateCategories;
        private System.Windows.Forms.Label lblDecimalPlaces;
        private System.Windows.Forms.NumericUpDown nudDecimalPlaces;
    }
}