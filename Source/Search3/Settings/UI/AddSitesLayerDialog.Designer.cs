namespace Search3.Settings.UI
{
    partial class AddSitesLayerDialog
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
            this.urlConnectionTextbox = new System.Windows.Forms.TextBox();
            this.labelUrlConnection = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // urlConnectionTextbox
            // 
            this.urlConnectionTextbox.Location = new System.Drawing.Point(12, 35);
            this.urlConnectionTextbox.Name = "urlConnectionTextbox";
            this.urlConnectionTextbox.Size = new System.Drawing.Size(311, 20);
            this.urlConnectionTextbox.TabIndex = 0;
            // 
            // labelUrlConnection
            // 
            this.labelUrlConnection.AutoSize = true;
            this.labelUrlConnection.Location = new System.Drawing.Point(12, 13);
            this.labelUrlConnection.Name = "labelUrlConnection";
            this.labelUrlConnection.Size = new System.Drawing.Size(112, 13);
            this.labelUrlConnection.TabIndex = 1;
            this.labelUrlConnection.Text = "Please enter the URL:";
            // 
            // AddSitesLayerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 130);
            this.Controls.Add(this.labelUrlConnection);
            this.Controls.Add(this.urlConnectionTextbox);
            this.Name = "AddSitesLayerDialog";
            this.Text = "AddSitesLayerDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox urlConnectionTextbox;
        private System.Windows.Forms.Label labelUrlConnection;
    }
}