namespace HydroDesktop.Plugins.DataImport.CommonPages
{
    partial class VariableView
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
            this.lblName = new System.Windows.Forms.Label();
            this.tbCode = new System.Windows.Forms.TextBox();
            this.lblCode = new System.Windows.Forms.Label();
            this.lblVariableUnits = new System.Windows.Forms.Label();
            this.cmbVariableUnits = new System.Windows.Forms.ComboBox();
            this.cmbTimeUnits = new System.Windows.Forms.ComboBox();
            this.lblTimeUnits = new System.Windows.Forms.Label();
            this.lblDataType = new System.Windows.Forms.Label();
            this.lblValueType = new System.Windows.Forms.Label();
            this.lblTimeSupport = new System.Windows.Forms.Label();
            this.lblNoDataValue = new System.Windows.Forms.Label();
            this.cmbName = new System.Windows.Forms.ComboBox();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.cmbValueType = new System.Windows.Forms.ComboBox();
            this.nudNoDataValue = new DataImport.CommonPages.NumericUpDownEx();
            this.nudTimeSupport = new DataImport.CommonPages.NumericUpDownEx();
            this.cmbDataType = new System.Windows.Forms.ComboBox();
            this.cmbSampleMedium = new System.Windows.Forms.ComboBox();
            this.lblSampleMedium = new System.Windows.Forms.Label();
            this.btnCreateNewVariableUnit = new System.Windows.Forms.Button();
            this.btnCreateNewTimeUnit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNoDataValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeSupport)).BeginInit();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(13, 15);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name";
            // 
            // tbCode
            // 
            this.tbCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCode.Location = new System.Drawing.Point(103, 41);
            this.tbCode.Name = "tbCode";
            this.tbCode.Size = new System.Drawing.Size(186, 20);
            this.tbCode.TabIndex = 1;
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.Location = new System.Drawing.Point(13, 44);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(32, 13);
            this.lblCode.TabIndex = 2;
            this.lblCode.Text = "Code";
            // 
            // lblVariableUnits
            // 
            this.lblVariableUnits.AutoSize = true;
            this.lblVariableUnits.Location = new System.Drawing.Point(13, 73);
            this.lblVariableUnits.Name = "lblVariableUnits";
            this.lblVariableUnits.Size = new System.Drawing.Size(70, 13);
            this.lblVariableUnits.TabIndex = 4;
            this.lblVariableUnits.Text = "Variable units";
            // 
            // cmbVariableUnits
            // 
            this.cmbVariableUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbVariableUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVariableUnits.FormattingEnabled = true;
            this.cmbVariableUnits.Location = new System.Drawing.Point(103, 70);
            this.cmbVariableUnits.Name = "cmbVariableUnits";
            this.cmbVariableUnits.Size = new System.Drawing.Size(97, 21);
            this.cmbVariableUnits.TabIndex = 2;
            // 
            // cmbTimeUnits
            // 
            this.cmbTimeUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTimeUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTimeUnits.FormattingEnabled = true;
            this.cmbTimeUnits.Location = new System.Drawing.Point(103, 210);
            this.cmbTimeUnits.Name = "cmbTimeUnits";
            this.cmbTimeUnits.Size = new System.Drawing.Size(97, 21);
            this.cmbTimeUnits.TabIndex = 6;
            // 
            // lblTimeUnits
            // 
            this.lblTimeUnits.AutoSize = true;
            this.lblTimeUnits.Location = new System.Drawing.Point(13, 213);
            this.lblTimeUnits.Name = "lblTimeUnits";
            this.lblTimeUnits.Size = new System.Drawing.Size(55, 13);
            this.lblTimeUnits.TabIndex = 6;
            this.lblTimeUnits.Text = "Time units";
            // 
            // lblDataType
            // 
            this.lblDataType.AutoSize = true;
            this.lblDataType.Location = new System.Drawing.Point(13, 102);
            this.lblDataType.Name = "lblDataType";
            this.lblDataType.Size = new System.Drawing.Size(53, 13);
            this.lblDataType.TabIndex = 8;
            this.lblDataType.Text = "Data type";
            // 
            // lblValueType
            // 
            this.lblValueType.AutoSize = true;
            this.lblValueType.Location = new System.Drawing.Point(13, 131);
            this.lblValueType.Name = "lblValueType";
            this.lblValueType.Size = new System.Drawing.Size(57, 13);
            this.lblValueType.TabIndex = 10;
            this.lblValueType.Text = "Value type";
            // 
            // lblTimeSupport
            // 
            this.lblTimeSupport.AutoSize = true;
            this.lblTimeSupport.Location = new System.Drawing.Point(13, 184);
            this.lblTimeSupport.Name = "lblTimeSupport";
            this.lblTimeSupport.Size = new System.Drawing.Size(68, 13);
            this.lblTimeSupport.TabIndex = 12;
            this.lblTimeSupport.Text = "Time support";
            // 
            // lblNoDataValue
            // 
            this.lblNoDataValue.AutoSize = true;
            this.lblNoDataValue.Location = new System.Drawing.Point(13, 242);
            this.lblNoDataValue.Name = "lblNoDataValue";
            this.lblNoDataValue.Size = new System.Drawing.Size(74, 13);
            this.lblNoDataValue.TabIndex = 14;
            this.lblNoDataValue.Text = "No data value";
            // 
            // cmbName
            // 
            this.cmbName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbName.FormattingEnabled = true;
            this.cmbName.Location = new System.Drawing.Point(103, 12);
            this.cmbName.Name = "cmbName";
            this.cmbName.Size = new System.Drawing.Size(186, 21);
            this.cmbName.TabIndex = 0;
            // 
            // cmbValueType
            // 
            this.cmbValueType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbValueType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbValueType.FormattingEnabled = true;
            this.cmbValueType.Location = new System.Drawing.Point(103, 125);
            this.cmbValueType.Name = "cmbValueType";
            this.cmbValueType.Size = new System.Drawing.Size(186, 21);
            this.cmbValueType.TabIndex = 15;
            // 
            // nudNoDataValue
            // 
            this.nudNoDataValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudNoDataValue.FullReadOnly = false;
            this.nudNoDataValue.Location = new System.Drawing.Point(103, 240);
            this.nudNoDataValue.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.nudNoDataValue.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.nudNoDataValue.Name = "nudNoDataValue";
            this.nudNoDataValue.Size = new System.Drawing.Size(186, 20);
            this.nudNoDataValue.TabIndex = 7;
            // 
            // nudTimeSupport
            // 
            this.nudTimeSupport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudTimeSupport.FullReadOnly = false;
            this.nudTimeSupport.Location = new System.Drawing.Point(103, 182);
            this.nudTimeSupport.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.nudTimeSupport.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.nudTimeSupport.Name = "nudTimeSupport";
            this.nudTimeSupport.Size = new System.Drawing.Size(186, 20);
            this.nudTimeSupport.TabIndex = 5;
            // 
            // cmbDataType
            // 
            this.cmbDataType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDataType.FormattingEnabled = true;
            this.cmbDataType.Location = new System.Drawing.Point(103, 98);
            this.cmbDataType.Name = "cmbDataType";
            this.cmbDataType.Size = new System.Drawing.Size(186, 21);
            this.cmbDataType.TabIndex = 16;
            // 
            // cmbSampleMedium
            // 
            this.cmbSampleMedium.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSampleMedium.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSampleMedium.FormattingEnabled = true;
            this.cmbSampleMedium.Location = new System.Drawing.Point(103, 153);
            this.cmbSampleMedium.Name = "cmbSampleMedium";
            this.cmbSampleMedium.Size = new System.Drawing.Size(186, 21);
            this.cmbSampleMedium.TabIndex = 18;
            // 
            // lblSampleMedium
            // 
            this.lblSampleMedium.AutoSize = true;
            this.lblSampleMedium.Location = new System.Drawing.Point(14, 159);
            this.lblSampleMedium.Name = "lblSampleMedium";
            this.lblSampleMedium.Size = new System.Drawing.Size(82, 13);
            this.lblSampleMedium.TabIndex = 17;
            this.lblSampleMedium.Text = "Sample Medium";
            // 
            // btnCreateNewVariableUnit
            // 
            this.btnCreateNewVariableUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateNewVariableUnit.Location = new System.Drawing.Point(208, 70);
            this.btnCreateNewVariableUnit.Name = "btnCreateNewVariableUnit";
            this.btnCreateNewVariableUnit.Size = new System.Drawing.Size(81, 23);
            this.btnCreateNewVariableUnit.TabIndex = 19;
            this.btnCreateNewVariableUnit.Text = "Create New...";
            this.btnCreateNewVariableUnit.UseVisualStyleBackColor = true;
            this.btnCreateNewVariableUnit.Click += new System.EventHandler(this.btnCreateNewVariableUnit_Click);
            // 
            // btnCreateNewTimeUnit
            // 
            this.btnCreateNewTimeUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateNewTimeUnit.Location = new System.Drawing.Point(208, 208);
            this.btnCreateNewTimeUnit.Name = "btnCreateNewTimeUnit";
            this.btnCreateNewTimeUnit.Size = new System.Drawing.Size(81, 23);
            this.btnCreateNewTimeUnit.TabIndex = 20;
            this.btnCreateNewTimeUnit.Text = "Create New...";
            this.btnCreateNewTimeUnit.UseVisualStyleBackColor = true;
            this.btnCreateNewTimeUnit.Click += new System.EventHandler(this.btnCreateNewTimeUnit_Click);
            // 
            // VariableView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCreateNewTimeUnit);
            this.Controls.Add(this.btnCreateNewVariableUnit);
            this.Controls.Add(this.cmbSampleMedium);
            this.Controls.Add(this.lblSampleMedium);
            this.Controls.Add(this.cmbDataType);
            this.Controls.Add(this.cmbValueType);
            this.Controls.Add(this.cmbName);
            this.Controls.Add(this.nudNoDataValue);
            this.Controls.Add(this.lblNoDataValue);
            this.Controls.Add(this.nudTimeSupport);
            this.Controls.Add(this.lblTimeSupport);
            this.Controls.Add(this.lblValueType);
            this.Controls.Add(this.lblDataType);
            this.Controls.Add(this.cmbTimeUnits);
            this.Controls.Add(this.lblTimeUnits);
            this.Controls.Add(this.cmbVariableUnits);
            this.Controls.Add(this.lblVariableUnits);
            this.Controls.Add(this.tbCode);
            this.Controls.Add(this.lblCode);
            this.Controls.Add(this.lblName);
            this.Name = "VariableView";
            this.Size = new System.Drawing.Size(303, 277);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNoDataValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeSupport)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox tbCode;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.Label lblVariableUnits;
        private System.Windows.Forms.ComboBox cmbVariableUnits;
        private System.Windows.Forms.ComboBox cmbTimeUnits;
        private System.Windows.Forms.Label lblTimeUnits;
        private System.Windows.Forms.Label lblDataType;
        private System.Windows.Forms.Label lblValueType;
        private System.Windows.Forms.Label lblTimeSupport;
        private NumericUpDownEx nudTimeSupport;
        private NumericUpDownEx nudNoDataValue;
        private System.Windows.Forms.Label lblNoDataValue;
        private System.Windows.Forms.ComboBox cmbName;
        private System.Windows.Forms.ComboBox cmbValueType;
        private System.Windows.Forms.ComboBox cmbDataType;
        private System.Windows.Forms.ComboBox cmbSampleMedium;
        private System.Windows.Forms.Label lblSampleMedium;
        private System.Windows.Forms.Button btnCreateNewVariableUnit;
        private System.Windows.Forms.Button btnCreateNewTimeUnit;
    }
}
