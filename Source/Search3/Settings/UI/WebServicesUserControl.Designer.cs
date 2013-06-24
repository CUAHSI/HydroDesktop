namespace Search3.Settings.UI
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
            this.gridViewWebServices = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewWebServices)).BeginInit();
            this.SuspendLayout();
            // 
            // gridViewWebServices
            // 
            this.gridViewWebServices.AllowUserToAddRows = false;
            this.gridViewWebServices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridViewWebServices.Location = new System.Drawing.Point(0, 0);
            this.gridViewWebServices.Name = "gridViewWebServices";
            this.gridViewWebServices.Size = new System.Drawing.Size(376, 436);
            this.gridViewWebServices.TabIndex = 40;
            this.gridViewWebServices.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridViewWebServices_CellContentClick);
            // 
            // WebServicesUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridViewWebServices);
            this.Name = "WebServicesUserControl";
            this.Size = new System.Drawing.Size(376, 436);
            ((System.ComponentModel.ISupportInitialize)(this.gridViewWebServices)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridViewWebServices;
    }
}
