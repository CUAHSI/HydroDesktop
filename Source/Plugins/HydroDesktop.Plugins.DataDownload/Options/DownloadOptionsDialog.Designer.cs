using HydroDesktop.Plugins.DataDownload.Properties;
namespace HydroDesktop.Plugins.DataDownload.Options
{
    partial class DownloadOptionsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadOptionsDialog));
            this.btnOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chbGetAllValuesInOneRequest = new System.Windows.Forms.CheckBox();
            this.chbUseSingleThread = new System.Windows.Forms.CheckBox();
            this.nudNumberOfValues = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberOfValues)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(179, 60);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 99;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Number of values per request:";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(263, 60);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 100;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chbGetAllValuesInOneRequest
            // 
            this.chbGetAllValuesInOneRequest.AutoSize = true;
            this.chbGetAllValuesInOneRequest.Location = new System.Drawing.Point(12, 66);
            this.chbGetAllValuesInOneRequest.Name = "chbGetAllValuesInOneRequest";
            this.chbGetAllValuesInOneRequest.Size = new System.Drawing.Size(160, 17);
            this.chbGetAllValuesInOneRequest.TabIndex = 2;
            this.chbGetAllValuesInOneRequest.Text = "Get all values in one request";
            this.chbGetAllValuesInOneRequest.UseVisualStyleBackColor = true;
            this.chbGetAllValuesInOneRequest.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // chbUseSingleThread
            // 
            this.chbUseSingleThread.AutoSize = true;
            this.chbUseSingleThread.Location = new System.Drawing.Point(12, 43);
            this.chbUseSingleThread.Name = "chbUseSingleThread";
            this.chbUseSingleThread.Size = new System.Drawing.Size(130, 17);
            this.chbUseSingleThread.TabIndex = 1;
            this.chbUseSingleThread.Text = "Only use single thread";
            this.chbUseSingleThread.UseVisualStyleBackColor = true;
            // 
            // nudNumberOfValues
            // 
            this.nudNumberOfValues.Location = new System.Drawing.Point(179, 10);
            this.nudNumberOfValues.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudNumberOfValues.Name = "nudNumberOfValues";
            this.nudNumberOfValues.Size = new System.Drawing.Size(159, 20);
            this.nudNumberOfValues.TabIndex = 0;
            // 
            // DownloadOptionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.ClientSize = new System.Drawing.Size(350, 94);
            this.Controls.Add(this.nudNumberOfValues);
            this.Controls.Add(this.chbUseSingleThread);
            this.Controls.Add(this.chbGetAllValuesInOneRequest);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DownloadOptionsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Download Settings";
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberOfValues)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chbGetAllValuesInOneRequest;
        private System.Windows.Forms.CheckBox chbUseSingleThread;
        private System.Windows.Forms.NumericUpDown nudNumberOfValues;
    }
}