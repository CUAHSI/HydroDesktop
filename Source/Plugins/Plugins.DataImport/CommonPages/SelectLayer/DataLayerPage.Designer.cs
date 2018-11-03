namespace HydroDesktop.Plugins.DataImport.CommonPages.SelectLayer
{
    partial class DataLayerPage
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
            this.lblExistingLayer = new System.Windows.Forms.Label();
            this.cmbLayers = new System.Windows.Forms.ComboBox();
            this.chbCreateNewLayer = new System.Windows.Forms.CheckBox();
            this.tbNewLayer = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.Subtitle = "Select Data Layer to import";
            this.Banner.Title = "Select Layer";
            // 
            // lblExistingLayer
            // 
            this.lblExistingLayer.AutoSize = true;
            this.lblExistingLayer.Location = new System.Drawing.Point(15, 149);
            this.lblExistingLayer.Name = "lblExistingLayer";
            this.lblExistingLayer.Size = new System.Drawing.Size(100, 13);
            this.lblExistingLayer.TabIndex = 1;
            this.lblExistingLayer.Text = "Select existing layer";
            // 
            // cmbLayers
            // 
            this.cmbLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLayers.FormattingEnabled = true;
            this.cmbLayers.Location = new System.Drawing.Point(18, 167);
            this.cmbLayers.Name = "cmbLayers";
            this.cmbLayers.Size = new System.Drawing.Size(181, 21);
            this.cmbLayers.TabIndex = 2;
            // 
            // chbCreateNewLayer
            // 
            this.chbCreateNewLayer.AutoSize = true;
            this.chbCreateNewLayer.Location = new System.Drawing.Point(18, 88);
            this.chbCreateNewLayer.Name = "chbCreateNewLayer";
            this.chbCreateNewLayer.Size = new System.Drawing.Size(105, 17);
            this.chbCreateNewLayer.TabIndex = 3;
            this.chbCreateNewLayer.Text = "Create new layer";
            this.chbCreateNewLayer.UseVisualStyleBackColor = true;
            this.chbCreateNewLayer.CheckedChanged += new System.EventHandler(this.chbCreateNewLayer_CheckedChanged);
            // 
            // tbNewLayer
            // 
            this.tbNewLayer.Location = new System.Drawing.Point(18, 111);
            this.tbNewLayer.Name = "tbNewLayer";
            this.tbNewLayer.Size = new System.Drawing.Size(181, 20);
            this.tbNewLayer.TabIndex = 4;
            // 
            // DataLayerPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbNewLayer);
            this.Controls.Add(this.chbCreateNewLayer);
            this.Controls.Add(this.lblExistingLayer);
            this.Controls.Add(this.cmbLayers);
            this.Name = "DataLayerPage";
            this.Size = new System.Drawing.Size(432, 254);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.DataLayerPage_SetActive);
            this.WizardNext += new Wizard.UI.WizardPageEventHandler(this.DataLayerPage_WizardNext);
            this.Controls.SetChildIndex(this.cmbLayers, 0);
            this.Controls.SetChildIndex(this.lblExistingLayer, 0);
            this.Controls.SetChildIndex(this.chbCreateNewLayer, 0);
            this.Controls.SetChildIndex(this.tbNewLayer, 0);
            this.Controls.SetChildIndex(this.Banner, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblExistingLayer;
        private System.Windows.Forms.ComboBox cmbLayers;
        private System.Windows.Forms.CheckBox chbCreateNewLayer;
        private System.Windows.Forms.TextBox tbNewLayer;
    }
}
