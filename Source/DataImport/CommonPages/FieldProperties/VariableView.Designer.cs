namespace DataImport.CommonPages
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
            this.tbDataType = new System.Windows.Forms.TextBox();
            this.lblDataType = new System.Windows.Forms.Label();
            this.lblValueType = new System.Windows.Forms.Label();
            this.lblTimeSupport = new System.Windows.Forms.Label();
            this.nudTimeSupport = new DataImport.CommonPages.NumericUpDownEx();
            this.nudNoDataValue = new DataImport.CommonPages.NumericUpDownEx();
            this.lblNoDataValue = new System.Windows.Forms.Label();
            this.cmbName = new System.Windows.Forms.ComboBox();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.cmbValueType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeSupport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNoDataValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
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
            this.tbCode.Size = new System.Drawing.Size(143, 20);
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
            this.cmbVariableUnits.Size = new System.Drawing.Size(143, 21);
            this.cmbVariableUnits.TabIndex = 2;
            // 
            // cmbTimeUnits
            // 
            this.cmbTimeUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTimeUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTimeUnits.FormattingEnabled = true;
            this.cmbTimeUnits.Location = new System.Drawing.Point(103, 186);
            this.cmbTimeUnits.Name = "cmbTimeUnits";
            this.cmbTimeUnits.Size = new System.Drawing.Size(143, 21);
            this.cmbTimeUnits.TabIndex = 6;
            // 
            // lblTimeUnits
            // 
            this.lblTimeUnits.AutoSize = true;
            this.lblTimeUnits.Location = new System.Drawing.Point(13, 189);
            this.lblTimeUnits.Name = "lblTimeUnits";
            this.lblTimeUnits.Size = new System.Drawing.Size(55, 13);
            this.lblTimeUnits.TabIndex = 6;
            this.lblTimeUnits.Text = "Time units";
            // 
            // tbDataType
            // 
            this.tbDataType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDataType.Location = new System.Drawing.Point(103, 99);
            this.tbDataType.Name = "tbDataType";
            this.tbDataType.Size = new System.Drawing.Size(143, 20);
            this.tbDataType.TabIndex = 3;
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
            this.lblTimeSupport.Location = new System.Drawing.Point(13, 160);
            this.lblTimeSupport.Name = "lblTimeSupport";
            this.lblTimeSupport.Size = new System.Drawing.Size(68, 13);
            this.lblTimeSupport.TabIndex = 12;
            this.lblTimeSupport.Text = "Time support";
            // 
            // nudTimeSupport
            // 
            this.nudTimeSupport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudTimeSupport.FullReadOnly = false;
            this.nudTimeSupport.Location = new System.Drawing.Point(103, 158);
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
            this.nudTimeSupport.Size = new System.Drawing.Size(143, 20);
            this.nudTimeSupport.TabIndex = 5;
            // 
            // nudNoDataValue
            // 
            this.nudNoDataValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudNoDataValue.FullReadOnly = false;
            this.nudNoDataValue.Location = new System.Drawing.Point(103, 216);
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
            this.nudNoDataValue.Size = new System.Drawing.Size(143, 20);
            this.nudNoDataValue.TabIndex = 7;
            // 
            // lblNoDataValue
            // 
            this.lblNoDataValue.AutoSize = true;
            this.lblNoDataValue.Location = new System.Drawing.Point(13, 218);
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
            this.cmbName.Size = new System.Drawing.Size(143, 21);
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
            this.cmbValueType.Size = new System.Drawing.Size(143, 21);
            this.cmbValueType.TabIndex = 15;
            // 
            // VariableView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbValueType);
            this.Controls.Add(this.cmbName);
            this.Controls.Add(this.nudNoDataValue);
            this.Controls.Add(this.lblNoDataValue);
            this.Controls.Add(this.nudTimeSupport);
            this.Controls.Add(this.lblTimeSupport);
            this.Controls.Add(this.lblValueType);
            this.Controls.Add(this.tbDataType);
            this.Controls.Add(this.lblDataType);
            this.Controls.Add(this.cmbTimeUnits);
            this.Controls.Add(this.lblTimeUnits);
            this.Controls.Add(this.cmbVariableUnits);
            this.Controls.Add(this.lblVariableUnits);
            this.Controls.Add(this.tbCode);
            this.Controls.Add(this.lblCode);
            this.Controls.Add(this.lblName);
            this.Name = "VariableView";
            this.Size = new System.Drawing.Size(260, 251);
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeSupport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNoDataValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
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
        private System.Windows.Forms.TextBox tbDataType;
        private System.Windows.Forms.Label lblDataType;
        private System.Windows.Forms.Label lblValueType;
        private System.Windows.Forms.Label lblTimeSupport;
        private NumericUpDownEx nudTimeSupport;
        private NumericUpDownEx nudNoDataValue;
        private System.Windows.Forms.Label lblNoDataValue;
        private System.Windows.Forms.ComboBox cmbName;
        private System.Windows.Forms.ComboBox cmbValueType;
    }
}
