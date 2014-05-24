namespace HydroDesktop.Plugins.HydroShare
{
    partial class chooseUploadType
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
            this.geoRB = new System.Windows.Forms.RadioButton();
            this.timeRB = new System.Windows.Forms.RadioButton();
            this.otherRB = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.nextButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // geoRB
            // 
            this.geoRB.AutoSize = true;
            this.geoRB.Location = new System.Drawing.Point(10, 12);
            this.geoRB.Name = "geoRB";
            this.geoRB.Size = new System.Drawing.Size(483, 17);
            this.geoRB.TabIndex = 0;
            this.geoRB.TabStop = true;
            this.geoRB.Text = "Geoananalytics (RENCI Geoanalytics content type. Connects HydroShare with GIS dat" +
                "a sources)";
            this.geoRB.UseVisualStyleBackColor = true;
            // 
            // timeRB
            // 
            this.timeRB.AutoSize = true;
            this.timeRB.Location = new System.Drawing.Point(10, 80);
            this.timeRB.Name = "timeRB";
            this.timeRB.Size = new System.Drawing.Size(466, 17);
            this.timeRB.TabIndex = 1;
            this.timeRB.TabStop = true;
            this.timeRB.Text = "Time Series (Support for Time Series CSV file format following the HydroDesktop s" +
                "pecification)\r\n";
            this.timeRB.UseVisualStyleBackColor = true;
            // 
            // otherRB
            // 
            this.otherRB.AutoSize = true;
            this.otherRB.Location = new System.Drawing.Point(10, 47);
            this.otherRB.Name = "otherRB";
            this.otherRB.Size = new System.Drawing.Size(543, 17);
            this.otherRB.TabIndex = 2;
            this.otherRB.TabStop = true;
            this.otherRB.Text = "Other (This is a catch all for other resources which do not correspond to existin" +
                "g HydroShare Resource Types)\r\n";
            this.otherRB.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Select upload type:";
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(497, 185);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(77, 31);
            this.nextButton.TabIndex = 4;
            this.nextButton.Text = "Next";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(414, 185);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(77, 31);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.otherRB);
            this.panel1.Controls.Add(this.timeRB);
            this.panel1.Controls.Add(this.geoRB);
            this.panel1.Location = new System.Drawing.Point(13, 50);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(560, 114);
            this.panel1.TabIndex = 6;
            // 
            // chooseUploadType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 228);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.label1);
            this.Name = "chooseUploadType";
            this.Text = "Publish New Content";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton geoRB;
        private System.Windows.Forms.RadioButton timeRB;
        private System.Windows.Forms.RadioButton otherRB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Panel panel1;
    }
}