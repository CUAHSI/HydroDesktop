namespace HydroDesktop.Plugins.DataImport.CommonPages.FieldProperties
{
    partial class UnitView
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
            this.tbName = new System.Windows.Forms.TextBox();
            this.tbType = new System.Windows.Forms.TextBox();
            this.lblType = new System.Windows.Forms.Label();
            this.tbAbbreviation = new System.Windows.Forms.TextBox();
            this.lblAbbreviation = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(15, 18);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name";
            // 
            // tbName
            // 
            this.tbName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbName.Location = new System.Drawing.Point(86, 15);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(123, 20);
            this.tbName.TabIndex = 0;
            // 
            // tbType
            // 
            this.tbType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbType.Location = new System.Drawing.Point(86, 41);
            this.tbType.Name = "tbType";
            this.tbType.Size = new System.Drawing.Size(123, 20);
            this.tbType.TabIndex = 1;
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(15, 44);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(31, 13);
            this.lblType.TabIndex = 2;
            this.lblType.Text = "Type";
            // 
            // tbAbbreviation
            // 
            this.tbAbbreviation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAbbreviation.Location = new System.Drawing.Point(86, 67);
            this.tbAbbreviation.Name = "tbAbbreviation";
            this.tbAbbreviation.Size = new System.Drawing.Size(123, 20);
            this.tbAbbreviation.TabIndex = 3;
            // 
            // lblAbbreviation
            // 
            this.lblAbbreviation.AutoSize = true;
            this.lblAbbreviation.Location = new System.Drawing.Point(15, 70);
            this.lblAbbreviation.Name = "lblAbbreviation";
            this.lblAbbreviation.Size = new System.Drawing.Size(66, 13);
            this.lblAbbreviation.TabIndex = 4;
            this.lblAbbreviation.Text = "Abbreviation";
            // 
            // UnitView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbAbbreviation);
            this.Controls.Add(this.lblAbbreviation);
            this.Controls.Add(this.tbType);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.lblName);
            this.Name = "UnitView";
            this.Size = new System.Drawing.Size(219, 100);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbType;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.TextBox tbAbbreviation;
        private System.Windows.Forms.Label lblAbbreviation;
        private System.Windows.Forms.BindingSource bindingSource1;
    }
}
