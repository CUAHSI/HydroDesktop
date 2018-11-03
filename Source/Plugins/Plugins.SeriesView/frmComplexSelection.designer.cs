using DotSpatial.Symbology.Forms;

namespace HydroDesktop.Plugins.SeriesView
{
    partial class frmComplexSelection
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
            this.sqlQueryControl2 = new DotSpatial.Symbology.Forms.SQLQueryControl();
            this.btnCommit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // sqlQueryControl2
            // 
            this.sqlQueryControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlQueryControl2.AttributeSource = null;
            this.sqlQueryControl2.ExpressionText = "";
            this.sqlQueryControl2.Location = new System.Drawing.Point(12, 12);
            this.sqlQueryControl2.Name = "sqlQueryControl2";
            this.sqlQueryControl2.Size = new System.Drawing.Size(325, 338);
            this.sqlQueryControl2.TabIndex = 0;
            this.sqlQueryControl2.Table = null;
            // 
            // btnCommit
            // 
            this.btnCommit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCommit.Location = new System.Drawing.Point(130, 356);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(75, 23);
            this.btnCommit.TabIndex = 1;
            this.btnCommit.Text = "Commit";
            this.btnCommit.UseVisualStyleBackColor = true;
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click_1);
            // 
            // frmComplexSelection
            // 
            this.ClientSize = new System.Drawing.Size(345, 385);
            this.Controls.Add(this.btnCommit);
            this.Controls.Add(this.sqlQueryControl2);
            this.Name = "frmComplexSelection";
            this.Text = "Query Builder";
            this.ResumeLayout(false);

        }

        #endregion

        private SQLQueryControl sqlQueryControl2;
        
    }
}