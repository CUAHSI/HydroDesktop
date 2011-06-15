namespace SeriesSelectorTest
{
    partial class Form1
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
            this.btnTestChange = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.bntSelect = new System.Windows.Forms.Button();
            this.bntUnSelect = new System.Windows.Forms.Button();
            this.bntCheckState = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCheckState = new System.Windows.Forms.Label();
            this.seriesSelector1 = new HydroDesktop.Controls.SeriesSelector3();
            this.SuspendLayout();
            // 
            // btnTestChange
            // 
            this.btnTestChange.Location = new System.Drawing.Point(294, 26);
            this.btnTestChange.Name = "btnTestChange";
            this.btnTestChange.Size = new System.Drawing.Size(107, 39);
            this.btnTestChange.TabIndex = 3;
            this.btnTestChange.Text = "Delete Checked Series";
            this.btnTestChange.UseVisualStyleBackColor = true;
            this.btnTestChange.Click += new System.EventHandler(this.btnTestChange_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(294, 86);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(107, 50);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // bntSelect
            // 
            this.bntSelect.Location = new System.Drawing.Point(294, 157);
            this.bntSelect.Name = "bntSelect";
            this.bntSelect.Size = new System.Drawing.Size(106, 51);
            this.bntSelect.TabIndex = 5;
            this.bntSelect.Text = "Select";
            this.bntSelect.UseVisualStyleBackColor = true;
            this.bntSelect.Click += new System.EventHandler(this.bntSelect_Click);
            // 
            // bntUnSelect
            // 
            this.bntUnSelect.Location = new System.Drawing.Point(295, 225);
            this.bntUnSelect.Name = "bntUnSelect";
            this.bntUnSelect.Size = new System.Drawing.Size(106, 51);
            this.bntUnSelect.TabIndex = 6;
            this.bntUnSelect.Text = "UnSelect";
            this.bntUnSelect.UseVisualStyleBackColor = true;
            this.bntUnSelect.Click += new System.EventHandler(this.bntUnSelect_Click);
            // 
            // bntCheckState
            // 
            this.bntCheckState.Location = new System.Drawing.Point(295, 300);
            this.bntCheckState.Name = "bntCheckState";
            this.bntCheckState.Size = new System.Drawing.Size(106, 51);
            this.bntCheckState.TabIndex = 7;
            this.bntCheckState.Text = "Checked State";
            this.bntCheckState.UseVisualStyleBackColor = true;
            this.bntCheckState.Click += new System.EventHandler(this.bntCheckState_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(295, 358);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Checked State:";
            // 
            // lblCheckState
            // 
            this.lblCheckState.BackColor = System.Drawing.Color.White;
            this.lblCheckState.Location = new System.Drawing.Point(298, 375);
            this.lblCheckState.Name = "lblCheckState";
            this.lblCheckState.Size = new System.Drawing.Size(100, 18);
            this.lblCheckState.TabIndex = 9;
            // 
            // seriesSelector1
            // 
            this.seriesSelector1.CheckOnClick = false;
            this.seriesSelector1.Location = new System.Drawing.Point(13, 26);
            this.seriesSelector1.Name = "seriesSelector1";
            this.seriesSelector1.Size = new System.Drawing.Size(250, 425);
            this.seriesSelector1.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 480);
            
            this.Controls.Add(this.lblCheckState);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bntCheckState);
            this.Controls.Add(this.bntUnSelect);
            this.Controls.Add(this.bntSelect);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnTestChange);
            this.Name = "Form1";
            this.Text = "TableView";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnTestChange;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button bntSelect;
        private System.Windows.Forms.Button bntUnSelect;
        private System.Windows.Forms.Button bntCheckState;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCheckState;
        private HydroDesktop.Controls.SeriesSelector3 seriesSelector1;
        
    }
}

