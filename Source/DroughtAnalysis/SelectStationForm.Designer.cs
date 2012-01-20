namespace DroughtAnalysis
{
    /// <summary>
    /// Main form for selecting stations and drought parameters
    /// </summary>
    partial class SelectStationForm
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
            this.components = new System.ComponentModel.Container();
            this.listBoxStations = new System.Windows.Forms.ListBox();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPathToR = new System.Windows.Forms.TextBox();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnPathToR = new System.Windows.Forms.Button();
            this.btnOutputFolder = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.checkBoxNewDirectory = new System.Windows.Forms.CheckBox();
            this.btnViewResults = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxStations
            // 
            this.listBoxStations.DataSource = this.bindingSource1;
            this.listBoxStations.FormattingEnabled = true;
            this.listBoxStations.Location = new System.Drawing.Point(12, 35);
            this.listBoxStations.Name = "listBoxStations";
            this.listBoxStations.Size = new System.Drawing.Size(399, 82);
            this.listBoxStations.TabIndex = 0;
            this.listBoxStations.SelectedIndexChanged += new System.EventHandler(this.Stations_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select the Station:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Set path to R:";
            // 
            // txtPathToR
            // 
            this.txtPathToR.Location = new System.Drawing.Point(15, 157);
            this.txtPathToR.Name = "txtPathToR";
            this.txtPathToR.Size = new System.Drawing.Size(328, 20);
            this.txtPathToR.TabIndex = 3;
            this.txtPathToR.TextChanged += new System.EventHandler(this.TextBoxR_TextChanged);
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(15, 211);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(328, 20);
            this.txtOutputFolder.TabIndex = 5;
            this.txtOutputFolder.TextChanged += new System.EventHandler(this.TextBoxOutputFolder_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 194);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Output Directory:";
            // 
            // btnPathToR
            // 
            this.btnPathToR.Location = new System.Drawing.Point(350, 157);
            this.btnPathToR.Name = "btnPathToR";
            this.btnPathToR.Size = new System.Drawing.Size(61, 23);
            this.btnPathToR.TabIndex = 6;
            this.btnPathToR.Text = "Browse..";
            this.btnPathToR.UseVisualStyleBackColor = true;
            this.btnPathToR.Click += new System.EventHandler(this.btnPathToR_Click);
            // 
            // btnOutputFolder
            // 
            this.btnOutputFolder.Location = new System.Drawing.Point(350, 208);
            this.btnOutputFolder.Name = "btnOutputFolder";
            this.btnOutputFolder.Size = new System.Drawing.Size(61, 23);
            this.btnOutputFolder.TabIndex = 7;
            this.btnOutputFolder.Text = "Browse..";
            this.btnOutputFolder.UseVisualStyleBackColor = true;
            this.btnOutputFolder.Click += new System.EventHandler(this.btnOutputFolder_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(15, 275);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(184, 30);
            this.button3.TabIndex = 8;
            this.button3.Text = "Analyze Meteorological Drought";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(21, 259);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 13);
            this.lblProgress.TabIndex = 9;
            // 
            // checkBoxNewDirectory
            // 
            this.checkBoxNewDirectory.AutoSize = true;
            this.checkBoxNewDirectory.Checked = true;
            this.checkBoxNewDirectory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxNewDirectory.Location = new System.Drawing.Point(15, 239);
            this.checkBoxNewDirectory.Name = "checkBoxNewDirectory";
            this.checkBoxNewDirectory.Size = new System.Drawing.Size(172, 17);
            this.checkBoxNewDirectory.TabIndex = 10;
            this.checkBoxNewDirectory.Text = "Create new directory for station";
            this.checkBoxNewDirectory.UseVisualStyleBackColor = true;
            // 
            // btnViewResults
            // 
            this.btnViewResults.Enabled = false;
            this.btnViewResults.Location = new System.Drawing.Point(239, 273);
            this.btnViewResults.Name = "btnViewResults";
            this.btnViewResults.Size = new System.Drawing.Size(172, 30);
            this.btnViewResults.TabIndex = 11;
            this.btnViewResults.Text = "View Result Files";
            this.btnViewResults.UseVisualStyleBackColor = true;
            this.btnViewResults.Click += new System.EventHandler(this.btnViewResults_Click);
            // 
            // SelectStationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 315);
            this.Controls.Add(this.btnViewResults);
            this.Controls.Add(this.checkBoxNewDirectory);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnOutputFolder);
            this.Controls.Add(this.btnPathToR);
            this.Controls.Add(this.txtOutputFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtPathToR);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBoxStations);
            this.Name = "SelectStationForm";
            this.Text = "Drought Analysis - Select Station";
            this.Load += new System.EventHandler(this.SelectStationForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxStations;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPathToR;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnPathToR;
        private System.Windows.Forms.Button btnOutputFolder;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.CheckBox checkBoxNewDirectory;
        private System.Windows.Forms.Button btnViewResults;
    }
}