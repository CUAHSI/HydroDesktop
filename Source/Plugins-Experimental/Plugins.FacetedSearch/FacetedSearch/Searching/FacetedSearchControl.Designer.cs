namespace FacetedSearch3
{
    /// <summary>
    /// The user control added by the plug-in
    /// </summary>
    partial class FacetedSearchControl
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
            this.FacetFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // FacetFlowPanel
            // 
            this.FacetFlowPanel.AutoScroll = true;
            this.FacetFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FacetFlowPanel.Location = new System.Drawing.Point(0, 0);
            this.FacetFlowPanel.Name = "FacetFlowPanel";
            this.FacetFlowPanel.Size = new System.Drawing.Size(730, 319);
            this.FacetFlowPanel.TabIndex = 1;
            this.FacetFlowPanel.WrapContents = false;
            this.FacetFlowPanel.Resize += new System.EventHandler(this.FacetedSearchForm_Resize);
            // 
            // FacetedSearchControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.FacetFlowPanel);
            this.Name = "FacetedSearchControl";
            this.Size = new System.Drawing.Size(730, 319);
            this.Resize += new System.EventHandler(this.FacetedSearchForm_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FacetFlowPanel;

    }
}
