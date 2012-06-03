namespace FacetedSearch3.Settings.UI
{
    partial class WebServicesUserControl
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
            this.treeViewWebServices = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeViewWebServices
            // 
            this.treeViewWebServices.CheckBoxes = true;
            this.treeViewWebServices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewWebServices.Location = new System.Drawing.Point(0, 0);
            this.treeViewWebServices.Name = "treeViewWebServices";
            this.treeViewWebServices.Size = new System.Drawing.Size(376, 436);
            this.treeViewWebServices.TabIndex = 40;
            // 
            // WebServicesUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeViewWebServices);
            this.Name = "WebServicesUserControl";
            this.Size = new System.Drawing.Size(376, 436);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewWebServices;
    }
}
