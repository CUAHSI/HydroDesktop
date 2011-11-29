namespace EPADelineation
{
    partial class SaveWatershed
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
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.gbxSave = new System.Windows.Forms.GroupBox();
            this.cbxOverwrite = new System.Windows.Forms.CheckBox();
            this.tbstreamline = new System.Windows.Forms.TextBox();
            this.tbwshedpoint = new System.Windows.Forms.TextBox();
            this.lblStreamline = new System.Windows.Forms.Label();
            this.lblWshed = new System.Windows.Forms.Label();
            this.lblWshPoint = new System.Windows.Forms.Label();
            this.tbwshed = new System.Windows.Forms.TextBox();
            this.gbxSave.SuspendLayout();
            this.SuspendLayout();
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(37, 171);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(83, 24);
            this.OK.TabIndex = 0;
            this.OK.Text = "&OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(144, 171);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(83, 24);
            this.Cancel.TabIndex = 1;
            this.Cancel.Text = "&Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // gbxSave
            // 
            this.gbxSave.Controls.Add(this.cbxOverwrite);
            this.gbxSave.Controls.Add(this.tbstreamline);
            this.gbxSave.Controls.Add(this.tbwshedpoint);
            this.gbxSave.Controls.Add(this.lblStreamline);
            this.gbxSave.Controls.Add(this.lblWshed);
            this.gbxSave.Controls.Add(this.lblWshPoint);
            this.gbxSave.Controls.Add(this.tbwshed);
            this.gbxSave.Location = new System.Drawing.Point(12, 12);
            this.gbxSave.Name = "gbxSave";
            this.gbxSave.Size = new System.Drawing.Size(238, 146);
            this.gbxSave.TabIndex = 2;
            this.gbxSave.TabStop = false;
            this.gbxSave.Text = "Save Shapefiles";
            // 
            // cbxOverwrite
            // 
            this.cbxOverwrite.AutoSize = true;
            this.cbxOverwrite.Checked = true;
            this.cbxOverwrite.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxOverwrite.Location = new System.Drawing.Point(9, 123);
            this.cbxOverwrite.Name = "cbxOverwrite";
            this.cbxOverwrite.Size = new System.Drawing.Size(134, 17);
            this.cbxOverwrite.TabIndex = 6;
            this.cbxOverwrite.Text = "Over&write Existing Files";
            this.cbxOverwrite.UseVisualStyleBackColor = true;
            // 
            // tbstreamline
            // 
            this.tbstreamline.Location = new System.Drawing.Point(98, 94);
            this.tbstreamline.Name = "tbstreamline";
            this.tbstreamline.Size = new System.Drawing.Size(117, 20);
            this.tbstreamline.TabIndex = 5;
            this.tbstreamline.Text = "Reaches";
            // 
            // tbwshedpoint
            // 
            this.tbwshedpoint.Location = new System.Drawing.Point(98, 28);
            this.tbwshedpoint.Name = "tbwshedpoint";
            this.tbwshedpoint.Size = new System.Drawing.Size(117, 20);
            this.tbwshedpoint.TabIndex = 1;
            this.tbwshedpoint.Text = "Watershed Point";
            // 
            // lblStreamline
            // 
            this.lblStreamline.AutoSize = true;
            this.lblStreamline.Location = new System.Drawing.Point(6, 97);
            this.lblStreamline.Name = "lblStreamline";
            this.lblStreamline.Size = new System.Drawing.Size(56, 13);
            this.lblStreamline.TabIndex = 4;
            this.lblStreamline.Text = "&Streamline";
            // 
            // lblWshed
            // 
            this.lblWshed.AutoSize = true;
            this.lblWshed.Location = new System.Drawing.Point(6, 64);
            this.lblWshed.Name = "lblWshed";
            this.lblWshed.Size = new System.Drawing.Size(59, 13);
            this.lblWshed.TabIndex = 2;
            this.lblWshed.Text = "&Watershed";
            // 
            // lblWshPoint
            // 
            this.lblWshPoint.AutoSize = true;
            this.lblWshPoint.Location = new System.Drawing.Point(6, 31);
            this.lblWshPoint.Name = "lblWshPoint";
            this.lblWshPoint.Size = new System.Drawing.Size(86, 13);
            this.lblWshPoint.TabIndex = 0;
            this.lblWshPoint.Text = "Watershed &Point";
            // 
            // tbwshed
            // 
            this.tbwshed.Location = new System.Drawing.Point(98, 61);
            this.tbwshed.Name = "tbwshed";
            this.tbwshed.Size = new System.Drawing.Size(117, 20);
            this.tbwshed.TabIndex = 3;
            this.tbwshed.Text = "Watershed";
            // 
            // SaveWatershed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 207);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.gbxSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SaveWatershed";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Save Watershed";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.SaveDialog_HelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SaveDialog_FormClosing);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.SaveDialog_HelpRequested);
            this.gbxSave.ResumeLayout(false);
            this.gbxSave.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.GroupBox gbxSave;
        private System.Windows.Forms.TextBox tbstreamline;
        private System.Windows.Forms.Label lblStreamline;
        private System.Windows.Forms.TextBox tbwshed;
        private System.Windows.Forms.Label lblWshed;
        private System.Windows.Forms.TextBox tbwshedpoint;
        private System.Windows.Forms.Label lblWshPoint;
        private System.Windows.Forms.CheckBox cbxOverwrite;
    }
}