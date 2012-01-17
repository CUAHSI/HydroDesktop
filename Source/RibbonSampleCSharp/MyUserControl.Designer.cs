namespace RibbonSamplePlugin
{
    /// <summary>
    /// The user control added by the plug-in
    /// </summary>
    partial class MyUserControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.lstCheckedSeriesIDs = new System.Windows.Forms.ListBox();
            this.lblCheckedSeriesIDs = new System.Windows.Forms.Label();
            this.listBoxEvents = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.lblSelectedID = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label1.Location = new System.Drawing.Point(15, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(520, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "This control has been added by RibbonSamplePlugin";
            // 
            // lstCheckedSeriesIDs
            // 
            this.lstCheckedSeriesIDs.FormattingEnabled = true;
            this.lstCheckedSeriesIDs.Location = new System.Drawing.Point(20, 108);
            this.lstCheckedSeriesIDs.Name = "lstCheckedSeriesIDs";
            this.lstCheckedSeriesIDs.Size = new System.Drawing.Size(95, 368);
            this.lstCheckedSeriesIDs.TabIndex = 2;
            // 
            // lblCheckedSeriesIDs
            // 
            this.lblCheckedSeriesIDs.AutoSize = true;
            this.lblCheckedSeriesIDs.Location = new System.Drawing.Point(17, 92);
            this.lblCheckedSeriesIDs.Name = "lblCheckedSeriesIDs";
            this.lblCheckedSeriesIDs.Size = new System.Drawing.Size(98, 13);
            this.lblCheckedSeriesIDs.TabIndex = 3;
            this.lblCheckedSeriesIDs.Text = "CheckedSeriesIDs:";
            // 
            // listBoxEvents
            // 
            this.listBoxEvents.FormattingEnabled = true;
            this.listBoxEvents.Location = new System.Drawing.Point(185, 108);
            this.listBoxEvents.Name = "listBoxEvents";
            this.listBoxEvents.Size = new System.Drawing.Size(223, 368);
            this.listBoxEvents.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(182, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Events:";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(420, 88);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(147, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "Show Series Checkboxes";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // lblSelectedID
            // 
            this.lblSelectedID.AutoSize = true;
            this.lblSelectedID.Location = new System.Drawing.Point(417, 133);
            this.lblSelectedID.Name = "lblSelectedID";
            this.lblSelectedID.Size = new System.Drawing.Size(95, 13);
            this.lblSelectedID.TabIndex = 7;
            this.lblSelectedID.Text = "SelectedSeriesID: ";
            // 
            // MyUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.lblSelectedID);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listBoxEvents);
            this.Controls.Add(this.lblCheckedSeriesIDs);
            this.Controls.Add(this.lstCheckedSeriesIDs);
            this.Controls.Add(this.label1);
            this.Name = "MyUserControl";
            this.Size = new System.Drawing.Size(581, 504);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstCheckedSeriesIDs;
        private System.Windows.Forms.Label lblCheckedSeriesIDs;
        private System.Windows.Forms.ListBox listBoxEvents;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label lblSelectedID;
    }
}
