namespace HydroDesktop.ObjectModel.Controls
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
            this.lblTimeUnits = new System.Windows.Forms.Label();
            this.lblDataType = new System.Windows.Forms.Label();
            this.lblValueType = new System.Windows.Forms.Label();
            this.lblTimeSupport = new System.Windows.Forms.Label();
            this.lblNoDataValue = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.nudNoDataValue = new HydroDesktop.ObjectModel.Controls.NumericUpDownEx();
            this.nudTimeSupport = new HydroDesktop.ObjectModel.Controls.NumericUpDownEx();
            this.lblSampleMedium = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.tbVariableUnits = new System.Windows.Forms.TextBox();
            this.tbDataType = new System.Windows.Forms.TextBox();
            this.tbValueType = new System.Windows.Forms.TextBox();
            this.tbSampleMedium = new System.Windows.Forms.TextBox();
            this.tbTimeUnits = new System.Windows.Forms.TextBox();
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
            // lblSampleMedium
            // 
            this.lblSampleMedium.AutoSize = true;
            this.lblSampleMedium.Location = new System.Drawing.Point(14, 159);
            this.lblSampleMedium.Name = "lblSampleMedium";
            this.lblSampleMedium.Size = new System.Drawing.Size(82, 13);
            this.lblSampleMedium.TabIndex = 17;
            this.lblSampleMedium.Text = "Sample Medium";
            // 
            // tbName
            // 
            this.tbName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbName.Location = new System.Drawing.Point(103, 12);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(186, 20);
            this.tbName.TabIndex = 19;
            // 
            // tbVariableUnits
            // 
            this.tbVariableUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbVariableUnits.Location = new System.Drawing.Point(103, 70);
            this.tbVariableUnits.Name = "tbVariableUnits";
            this.tbVariableUnits.Size = new System.Drawing.Size(186, 20);
            this.tbVariableUnits.TabIndex = 20;
            // 
            // tbDataType
            // 
            this.tbDataType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDataType.Location = new System.Drawing.Point(103, 99);
            this.tbDataType.Name = "tbDataType";
            this.tbDataType.Size = new System.Drawing.Size(186, 20);
            this.tbDataType.TabIndex = 21;
            // 
            // tbValueType
            // 
            this.tbValueType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbValueType.Location = new System.Drawing.Point(103, 128);
            this.tbValueType.Name = "tbValueType";
            this.tbValueType.Size = new System.Drawing.Size(186, 20);
            this.tbValueType.TabIndex = 22;
            // 
            // tbSampleMedium
            // 
            this.tbSampleMedium.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSampleMedium.Location = new System.Drawing.Point(103, 156);
            this.tbSampleMedium.Name = "tbSampleMedium";
            this.tbSampleMedium.Size = new System.Drawing.Size(186, 20);
            this.tbSampleMedium.TabIndex = 23;
            // 
            // tbTimeUnits
            // 
            this.tbTimeUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTimeUnits.Location = new System.Drawing.Point(103, 210);
            this.tbTimeUnits.Name = "tbTimeUnits";
            this.tbTimeUnits.Size = new System.Drawing.Size(186, 20);
            this.tbTimeUnits.TabIndex = 24;
            // 
            // VariableView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbTimeUnits);
            this.Controls.Add(this.tbSampleMedium);
            this.Controls.Add(this.tbValueType);
            this.Controls.Add(this.tbDataType);
            this.Controls.Add(this.tbVariableUnits);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.lblSampleMedium);
            this.Controls.Add(this.nudNoDataValue);
            this.Controls.Add(this.lblNoDataValue);
            this.Controls.Add(this.nudTimeSupport);
            this.Controls.Add(this.lblTimeSupport);
            this.Controls.Add(this.lblValueType);
            this.Controls.Add(this.lblDataType);
            this.Controls.Add(this.lblTimeUnits);
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
        private System.Windows.Forms.Label lblTimeUnits;
        private System.Windows.Forms.Label lblDataType;
        private System.Windows.Forms.Label lblValueType;
        private System.Windows.Forms.Label lblTimeSupport;
        private NumericUpDownEx nudTimeSupport;
        private NumericUpDownEx nudNoDataValue;
        private System.Windows.Forms.Label lblNoDataValue;
        private System.Windows.Forms.Label lblSampleMedium;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbVariableUnits;
        private System.Windows.Forms.TextBox tbDataType;
        private System.Windows.Forms.TextBox tbValueType;
        private System.Windows.Forms.TextBox tbSampleMedium;
        private System.Windows.Forms.TextBox tbTimeUnits;
    }
}
