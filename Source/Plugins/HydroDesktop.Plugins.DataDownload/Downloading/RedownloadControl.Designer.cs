namespace HydroDesktop.Plugins.DataDownload.Downloading
{
    partial class RedownloadControl
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
            this.cmbOptions = new System.Windows.Forms.ComboBox();
            this.btnRedownload = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbOptions
            // 
            this.cmbOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbOptions.FormattingEnabled = true;
            this.cmbOptions.Location = new System.Drawing.Point(90, 3);
            this.cmbOptions.Name = "cmbOptions";
            this.cmbOptions.Size = new System.Drawing.Size(114, 21);
            this.cmbOptions.TabIndex = 0;
            // 
            // btnRedownload
            // 
            this.btnRedownload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRedownload.Location = new System.Drawing.Point(3, 3);
            this.btnRedownload.Name = "btnRedownload";
            this.btnRedownload.Size = new System.Drawing.Size(81, 24);
            this.btnRedownload.TabIndex = 1;
            this.btnRedownload.Text = "Re-download:";
            this.btnRedownload.UseVisualStyleBackColor = true;
            this.btnRedownload.Click += new System.EventHandler(this.btnRedownload_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.btnRedownload, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cmbOptions, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(207, 30);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // RedownloadControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RedownloadControl";
            this.Size = new System.Drawing.Size(207, 30);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbOptions;
        private System.Windows.Forms.Button btnRedownload;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
