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
            this.spcMain = new System.Windows.Forms.SplitContainer();
            this.seriesSelector1 = new SeriesView.SeriesSelector();
            this.spcMain.Panel1.SuspendLayout();
            this.spcMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // spcMain
            // 
            this.spcMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.spcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.spcMain.Location = new System.Drawing.Point(0, 0);
            this.spcMain.Name = "spcMain";
            // 
            // spcMain.Panel1
            // 
            this.spcMain.Panel1.Controls.Add(this.seriesSelector1);
            this.spcMain.Panel1MinSize = 260;
            this.spcMain.Size = new System.Drawing.Size(800, 500);
            this.spcMain.SplitterDistance = 270;
            this.spcMain.SplitterWidth = 6;
            this.spcMain.TabIndex = 0;
            // 
            // seriesSelector1
            // 
            this.seriesSelector1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.seriesSelector1.Location = new System.Drawing.Point(0, 0);
            this.seriesSelector1.Name = "seriesSelector1";
            this.seriesSelector1.Size = new System.Drawing.Size(270, 500);
            this.seriesSelector1.TabIndex = 0;
            // 
            // SeriesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.spcMain);
            this.Name = "SeriesView";
            this.Size = new System.Drawing.Size(800, 500);
            this.spcMain.Panel1.ResumeLayout(false);
            this.spcMain.ResumeLayout(false);
            this.ResumeLayout(false);

            
        }

        #endregion

        private SeriesView.SeriesSelector seriesSelector1;
        public System.Windows.Forms.SplitContainer spcMain;
    }
}
