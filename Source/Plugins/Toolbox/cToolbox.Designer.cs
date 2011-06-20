namespace Toolbox
{
    partial class cToolbox
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(cToolbox));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.toolManager1 = new DotSpatial.Controls.ToolManager();
            this.modelerToolStrip1 = new DotSpatial.Controls.ModelerToolStrip();
            this.modeler1 = new DotSpatial.Controls.Modeler();
            this.toolManagerToolStrip1 = new DotSpatial.Controls.ToolManagerToolStrip();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.toolManagerToolStrip1);
            this.splitContainer1.Panel1.Controls.Add(this.toolManager1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.modelerToolStrip1);
            this.splitContainer1.Panel2.Controls.Add(this.modeler1);
            this.splitContainer1.Size = new System.Drawing.Size(779, 514);
            this.splitContainer1.SplitterDistance = 227;
            this.splitContainer1.TabIndex = 0;
            // 
            // toolManager1
            // 
            this.toolManager1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolManager1.ImageIndex = 0;
            this.toolManager1.Legend = null;
            this.toolManager1.Location = new System.Drawing.Point(0, 0);
            this.toolManager1.Name = "toolManager1";
            this.toolManager1.SelectedImageIndex = 0;
            this.toolManager1.Size = new System.Drawing.Size(227, 514);
            this.toolManager1.TabIndex = 0;
            this.toolManager1.TempPath = "c:\\temp";
            this.toolManager1.ToolDirectories = ((System.Collections.Generic.List<string>)(resources.GetObject("toolManager1.ToolDirectories")));
            // 
            // modelerToolStrip1
            // 
            this.modelerToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.modelerToolStrip1.Modeler = this.modeler1;
            this.modelerToolStrip1.Name = "modelerToolStrip1";
            this.modelerToolStrip1.Size = new System.Drawing.Size(548, 25);
            this.modelerToolStrip1.TabIndex = 1;
            this.modelerToolStrip1.Text = "modelerToolStrip1";
            // 
            // modeler1
            // 
            this.modeler1.AllowDrop = true;
            this.modeler1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.modeler1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.modeler1.Cursor = System.Windows.Forms.Cursors.Default;
            this.modeler1.DataColor = System.Drawing.Color.LightGreen;
            this.modeler1.DataFont = new System.Drawing.Font("Tahoma", 8F);
            this.modeler1.DataShape = DotSpatial.Modeling.Forms.ModelShape.Ellipse;
            this.modeler1.DefaultFileExtension = "mwm";
            this.modeler1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modeler1.DrawingQuality = System.Drawing.Drawing2D.SmoothingMode.Default;
            this.modeler1.EnableLinking = false;
            this.modeler1.IsInitialized = true;
            this.modeler1.Location = new System.Drawing.Point(0, 0);
            this.modeler1.MaxExecutionThreads = 2;
            this.modeler1.ModelFilename = null;
            this.modeler1.Name = "modeler1";
            this.modeler1.ShowWaterMark = true;
            this.modeler1.Size = new System.Drawing.Size(548, 514);
            this.modeler1.TabIndex = 0;
            this.modeler1.ToolColor = System.Drawing.Color.Khaki;
            this.modeler1.ToolFont = new System.Drawing.Font("Tahoma", 8F);
            this.modeler1.ToolManager = this.toolManager1;
            this.modeler1.ToolShape = DotSpatial.Modeling.Forms.ModelShape.Rectangle;
            this.modeler1.WorkingPath = null;
            this.modeler1.ZoomFactor = 1F;
            // 
            // toolManagerToolStrip1
            // 
            this.toolManagerToolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolManagerToolStrip1.Location = new System.Drawing.Point(0, 489);
            this.toolManagerToolStrip1.Name = "toolManagerToolStrip1";
            this.toolManagerToolStrip1.Size = new System.Drawing.Size(227, 25);
            this.toolManagerToolStrip1.TabIndex = 1;
            this.toolManagerToolStrip1.Text = "toolManagerToolStrip1";
            this.toolManagerToolStrip1.ToolManager = this.toolManager1;
            // 
            // cToolbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "cToolbox";
            this.Size = new System.Drawing.Size(779, 514);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private DotSpatial.Controls.ToolManager toolManager1;
        private DotSpatial.Controls.ModelerToolStrip modelerToolStrip1;
        private DotSpatial.Controls.Modeler modeler1;
        private DotSpatial.Controls.ToolManagerToolStrip toolManagerToolStrip1;

    }
}
