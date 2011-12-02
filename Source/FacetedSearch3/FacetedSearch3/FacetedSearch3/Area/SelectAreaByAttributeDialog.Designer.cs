namespace FacetedSearch3.Area
{
    partial class SelectAreaByAttributeDialog
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
            this.btnApply = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblActiveLayer = new System.Windows.Forms.Label();
            this.cmbActiveLayer = new System.Windows.Forms.ComboBox();
            this.cmbField = new System.Windows.Forms.ComboBox();
            this.lblField = new System.Windows.Forms.Label();
            this.lblValue = new System.Windows.Forms.Label();
            this.teFirstLetters = new System.Windows.Forms.TextBox();
            this.lbValues = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(146, 333);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(227, 333);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblActiveLayer
            // 
            this.lblActiveLayer.AutoSize = true;
            this.lblActiveLayer.Location = new System.Drawing.Point(14, 19);
            this.lblActiveLayer.Name = "lblActiveLayer";
            this.lblActiveLayer.Size = new System.Drawing.Size(69, 13);
            this.lblActiveLayer.TabIndex = 2;
            this.lblActiveLayer.Text = "Active Layer:";
            // 
            // cmbActiveLayer
            // 
            this.cmbActiveLayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbActiveLayer.FormattingEnabled = true;
            this.cmbActiveLayer.Location = new System.Drawing.Point(17, 36);
            this.cmbActiveLayer.Name = "cmbActiveLayer";
            this.cmbActiveLayer.Size = new System.Drawing.Size(288, 21);
            this.cmbActiveLayer.TabIndex = 0;
            // 
            // cmbField
            // 
            this.cmbField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbField.FormattingEnabled = true;
            this.cmbField.Location = new System.Drawing.Point(17, 81);
            this.cmbField.Name = "cmbField";
            this.cmbField.Size = new System.Drawing.Size(288, 21);
            this.cmbField.TabIndex = 1;
            // 
            // lblField
            // 
            this.lblField.AutoSize = true;
            this.lblField.Location = new System.Drawing.Point(17, 65);
            this.lblField.Name = "lblField";
            this.lblField.Size = new System.Drawing.Size(32, 13);
            this.lblField.TabIndex = 4;
            this.lblField.Text = "Field:";
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(17, 131);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(141, 13);
            this.lblValue.TabIndex = 6;
            this.lblValue.Text = "Value: type-in first few letters";
            // 
            // teFirstLetters
            // 
            this.teFirstLetters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.teFirstLetters.Location = new System.Drawing.Point(17, 147);
            this.teFirstLetters.Name = "teFirstLetters";
            this.teFirstLetters.Size = new System.Drawing.Size(288, 20);
            this.teFirstLetters.TabIndex = 2;
            this.teFirstLetters.TextChanged += new System.EventHandler(this.teFirstLetters_TextChanged);
            // 
            // lbValues
            // 
            this.lbValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbValues.FormattingEnabled = true;
            this.lbValues.Location = new System.Drawing.Point(17, 173);
            this.lbValues.Name = "lbValues";
            this.lbValues.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbValues.Size = new System.Drawing.Size(288, 134);
            this.lbValues.TabIndex = 3;
            // 
            // SelectAreaByAttributeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 368);
            this.Controls.Add(this.lbValues);
            this.Controls.Add(this.teFirstLetters);
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.cmbField);
            this.Controls.Add(this.lblField);
            this.Controls.Add(this.cmbActiveLayer);
            this.Controls.Add(this.lblActiveLayer);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnApply);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectAreaByAttributeDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Area By Attribute";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblActiveLayer;
        private System.Windows.Forms.ComboBox cmbActiveLayer;
        private System.Windows.Forms.ComboBox cmbField;
        private System.Windows.Forms.Label lblField;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.TextBox teFirstLetters;
        private System.Windows.Forms.ListBox lbValues;
    }
}