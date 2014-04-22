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
            this.button1 = new System.Windows.Forms.Button();
            this.variablesListBox = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.WebServiceTitle = new System.Windows.Forms.TextBox();
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
            this.labelUrlConnection.Size = new System.Drawing.Size(205, 13);
            this.labelUrlConnection.TabIndex = 1;
            this.labelUrlConnection.Text = "Please enter the URL for the web service:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(249, 61);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Next";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // variablesListBox
            // 
            this.variablesListBox.CheckOnClick = true;
            this.variablesListBox.FormattingEnabled = true;
            this.variablesListBox.Location = new System.Drawing.Point(12, 115);
            this.variablesListBox.Name = "variablesListBox";
            this.variablesListBox.Size = new System.Drawing.Size(311, 199);
            this.variablesListBox.Sorted = true;
            this.variablesListBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(202, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Please check variables for new site layer:";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(249, 331);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Get Sites";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // WebServiceTitle
            // 
            this.WebServiceTitle.Location = new System.Drawing.Point(12, 334);
            this.WebServiceTitle.Name = "WebServiceTitle";
            this.WebServiceTitle.Size = new System.Drawing.Size(231, 20);
            this.WebServiceTitle.TabIndex = 6;
            // 
            // AddSitesLayerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 364);
            this.Controls.Add(this.WebServiceTitle);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.variablesListBox);
            this.Controls.Add(this.button1);
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckedListBox variablesListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox WebServiceTitle;
    }
}