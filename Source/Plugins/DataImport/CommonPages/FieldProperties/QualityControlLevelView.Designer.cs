namespace DataImport.CommonPages
{
    partial class QualityControlLevelView
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
            this.lblCode = new System.Windows.Forms.Label();
            this.tbCode = new System.Windows.Forms.TextBox();
            this.tbDefinition = new System.Windows.Forms.TextBox();
            this.lblDefinition = new System.Windows.Forms.Label();
            this.tbExplanation = new System.Windows.Forms.TextBox();
            this.lblExplanation = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.Location = new System.Drawing.Point(14, 20);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(32, 13);
            this.lblCode.TabIndex = 0;
            this.lblCode.Text = "Code";
            // 
            // tbCode
            // 
            this.tbCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCode.Location = new System.Drawing.Point(86, 17);
            this.tbCode.Name = "tbCode";
            this.tbCode.Size = new System.Drawing.Size(118, 20);
            this.tbCode.TabIndex = 0;
            // 
            // tbDefinition
            // 
            this.tbDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDefinition.Location = new System.Drawing.Point(86, 43);
            this.tbDefinition.Name = "tbDefinition";
            this.tbDefinition.Size = new System.Drawing.Size(118, 20);
            this.tbDefinition.TabIndex = 1;
            // 
            // lblDefinition
            // 
            this.lblDefinition.AutoSize = true;
            this.lblDefinition.Location = new System.Drawing.Point(14, 46);
            this.lblDefinition.Name = "lblDefinition";
            this.lblDefinition.Size = new System.Drawing.Size(51, 13);
            this.lblDefinition.TabIndex = 2;
            this.lblDefinition.Text = "Definition";
            // 
            // tbExplanation
            // 
            this.tbExplanation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbExplanation.Location = new System.Drawing.Point(86, 69);
            this.tbExplanation.Name = "tbExplanation";
            this.tbExplanation.Size = new System.Drawing.Size(118, 20);
            this.tbExplanation.TabIndex = 2;
            // 
            // lblExplanation
            // 
            this.lblExplanation.AutoSize = true;
            this.lblExplanation.Location = new System.Drawing.Point(14, 72);
            this.lblExplanation.Name = "lblExplanation";
            this.lblExplanation.Size = new System.Drawing.Size(62, 13);
            this.lblExplanation.TabIndex = 4;
            this.lblExplanation.Text = "Explanation";
            // 
            // QualityControlLevelView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbExplanation);
            this.Controls.Add(this.lblExplanation);
            this.Controls.Add(this.tbDefinition);
            this.Controls.Add(this.lblDefinition);
            this.Controls.Add(this.tbCode);
            this.Controls.Add(this.lblCode);
            this.Name = "QualityControlLevelView";
            this.Size = new System.Drawing.Size(217, 112);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.TextBox tbCode;
        private System.Windows.Forms.TextBox tbDefinition;
        private System.Windows.Forms.Label lblDefinition;
        private System.Windows.Forms.TextBox tbExplanation;
        private System.Windows.Forms.Label lblExplanation;
        private System.Windows.Forms.BindingSource bindingSource1;
    }
}
