namespace DataAggregation.MergeTool
{
    partial class MergeLayersForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeLayersForm));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lbNewLayerName = new System.Windows.Forms.Label();
            this.tbLayerName = new System.Windows.Forms.TextBox();
            this.dgvLayers = new System.Windows.Forms.DataGridView();
            this.paProgress = new System.Windows.Forms.Panel();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLayers)).BeginInit();
            this.paProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(319, 368);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(400, 368);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lbNewLayerName
            // 
            this.lbNewLayerName.AutoSize = true;
            this.lbNewLayerName.Location = new System.Drawing.Point(13, 17);
            this.lbNewLayerName.Name = "lbNewLayerName";
            this.lbNewLayerName.Size = new System.Drawing.Size(67, 13);
            this.lbNewLayerName.TabIndex = 2;
            this.lbNewLayerName.Text = "Layer Name:";
            // 
            // tbLayerName
            // 
            this.tbLayerName.Location = new System.Drawing.Point(86, 14);
            this.tbLayerName.Name = "tbLayerName";
            this.tbLayerName.Size = new System.Drawing.Size(183, 20);
            this.tbLayerName.TabIndex = 3;
            // 
            // dgvLayers
            // 
            this.dgvLayers.AllowUserToAddRows = false;
            this.dgvLayers.AllowUserToDeleteRows = false;
            this.dgvLayers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLayers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLayers.Location = new System.Drawing.Point(16, 50);
            this.dgvLayers.Name = "dgvLayers";
            this.dgvLayers.RowHeadersVisible = false;
            this.dgvLayers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLayers.Size = new System.Drawing.Size(459, 299);
            this.dgvLayers.TabIndex = 4;
            // 
            // paProgress
            // 
            this.paProgress.Controls.Add(this.pbProgress);
            this.paProgress.Location = new System.Drawing.Point(97, 174);
            this.paProgress.Name = "paProgress";
            this.paProgress.Size = new System.Drawing.Size(293, 54);
            this.paProgress.TabIndex = 15;
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(17, 15);
            this.pbProgress.MarqueeAnimationSpeed = 50;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(263, 23);
            this.pbProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbProgress.TabIndex = 0;
            // 
            // MergeLayersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 403);
            this.Controls.Add(this.paProgress);
            this.Controls.Add(this.dgvLayers);
            this.Controls.Add(this.tbLayerName);
            this.Controls.Add(this.lbNewLayerName);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MergeLayersForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Merge Layers";
            this.Load += new System.EventHandler(this.MergeLayersForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLayers)).EndInit();
            this.paProgress.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lbNewLayerName;
        private System.Windows.Forms.TextBox tbLayerName;
        private System.Windows.Forms.DataGridView dgvLayers;
        private System.Windows.Forms.Panel paProgress;
        private System.Windows.Forms.ProgressBar pbProgress;
    }
}