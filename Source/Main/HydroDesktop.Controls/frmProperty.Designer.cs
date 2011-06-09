using System.Windows.Forms;
using System.Drawing;


namespace HydroDesktop.Controls
{
    partial class frmProperty
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private PropertyGrid proGridSeries1;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProperty));
            this.proGridSeries1 = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // proGridSeries1
            // 
            this.proGridSeries1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.proGridSeries1.Location = new System.Drawing.Point(0, 0);
            this.proGridSeries1.Name = "proGridSeries1";
            this.proGridSeries1.Size = new System.Drawing.Size(406, 503);
            this.proGridSeries1.TabIndex = 0;
            // 
            // frmProperty
            // 
            this.ClientSize = new System.Drawing.Size(406, 503);
            this.Controls.Add(this.proGridSeries1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmProperty";
            this.Text = "Detailed Property Information";
            this.ResumeLayout(false);

        }

        #endregion

    }
}