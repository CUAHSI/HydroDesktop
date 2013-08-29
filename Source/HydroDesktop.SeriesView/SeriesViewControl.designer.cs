namespace SeriesView
{
    partial class SeriesViewControl
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
            this.seriesSelector1 = new SeriesView.SeriesSelector();
            this.SuspendLayout();
            // 
            // seriesSelector1
            // 
            this.seriesSelector1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.seriesSelector1.Location = new System.Drawing.Point(0, 0);
            this.seriesSelector1.Name = "seriesSelector1";
            this.seriesSelector1.Size = new System.Drawing.Size(275, 500);
            this.seriesSelector1.TabIndex = 0;
            // 
            // SeriesViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.seriesSelector1);
            this.Name = "SeriesViewControl";
            this.Size = new System.Drawing.Size(275, 500);
            this.ResumeLayout(false);

        }

        #endregion

        private SeriesView.SeriesSelector seriesSelector1;
    }
}
