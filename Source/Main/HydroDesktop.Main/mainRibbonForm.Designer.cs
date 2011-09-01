using HydroDesktop.Configuration;
using System.Threading;
namespace HydroDesktop.Main
{
    partial class mainRibbonForm
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

			// fixed issue 7216
			this.SwapDatabasesOnExit();

            base.Dispose(disposing);

        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainRibbonForm));
            this.mainLegend = new DotSpatial.Controls.Legend();
            this.mainMap = new DotSpatial.Controls.Map();
            this.mwStatusStrip1 = new DotSpatial.Controls.SpatialStatusStrip();
            this.statusLocation = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.applicationManager1 = new DotSpatial.Controls.AppManager();
            this.mwStatusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainLegend
            // 
            this.mainLegend.BackColor = System.Drawing.Color.White;
            this.mainLegend.ControlRectangle = new System.Drawing.Rectangle(0, 0, 201, 160);
            this.mainLegend.DocumentRectangle = new System.Drawing.Rectangle(0, 0, 35, 20);
            this.mainLegend.HorizontalScrollEnabled = true;
            this.mainLegend.Indentation = 30;
            this.mainLegend.IsInitialized = false;
            this.mainLegend.Location = new System.Drawing.Point(48, 204);
            this.mainLegend.MinimumSize = new System.Drawing.Size(5, 5);
            this.mainLegend.Name = "mainLegend";
            this.mainLegend.ProgressHandler = null;
            this.mainLegend.ResetOnResize = false;
            this.mainLegend.SelectionFontColor = System.Drawing.Color.Black;
            this.mainLegend.SelectionHighlight = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(238)))), ((int)(((byte)(252)))));
            this.mainLegend.Size = new System.Drawing.Size(201, 160);
            this.mainLegend.TabIndex = 0;
            this.mainLegend.Text = "legend1";
            this.mainLegend.VerticalScrollEnabled = true;
            // 
            // mainMap
            // 
            this.mainMap.AllowDrop = true;
            this.mainMap.BackColor = System.Drawing.Color.White;
            this.mainMap.CollectAfterDraw = false;
            this.mainMap.CollisionDetection = true;
            this.mainMap.ExtendBuffer = false;
            this.mainMap.FunctionMode = DotSpatial.Controls.FunctionMode.None;
            this.mainMap.IsBusy = false;
            this.mainMap.Legend = this.mainLegend;
            this.mainMap.Location = new System.Drawing.Point(362, 204);
            this.mainMap.Name = "mainMap";
            this.mainMap.ProgressHandler = this.mwStatusStrip1;
            this.mainMap.ProjectionModeDefine = DotSpatial.Controls.ActionMode.Prompt;
            this.mainMap.ProjectionModeReproject = DotSpatial.Controls.ActionMode.Prompt;
            this.mainMap.RedrawLayersWhileResizing = false;
            this.mainMap.SelectionEnabled = true;
            this.mainMap.Size = new System.Drawing.Size(374, 160);
            this.mainMap.TabIndex = 0;
            this.mainMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mainMap_MouseMove);
            // 
            // mwStatusStrip1
            // 
            this.mwStatusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.mwStatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLocation,
            this.lblStatus,
            this.toolStripStatusLabel1});
            this.mwStatusStrip1.Location = new System.Drawing.Point(0, 708);
            this.mwStatusStrip1.Name = "mwStatusStrip1";
            this.mwStatusStrip1.ProgressLabel = this.lblStatus;
            this.mwStatusStrip1.Size = new System.Drawing.Size(1008, 22);
            this.mwStatusStrip1.TabIndex = 3;
            this.mwStatusStrip1.Text = "mwStatusStrip1";
            // 
            // statusLocation
            // 
            this.statusLocation.Name = "statusLocation";
            this.statusLocation.Size = new System.Drawing.Size(165, 17);
            this.statusLocation.Text = "X: 00.00000000, Y: 00.00000000";
            // 
            // lblStatus
            // 
            this.lblStatus.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.lblStatus.Margin = new System.Windows.Forms.Padding(20, 3, 0, 2);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(777, 17);
            this.lblStatus.Spring = true;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BackColor = System.Drawing.Color.White;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // applicationManager1
            // 
            this.applicationManager1.DataManager.DataProviderDirectories = ((System.Collections.Generic.List<string>)(resources.GetObject("applicationManager1.DataManager.DataProviderDirectories")));
            this.applicationManager1.DataManager.LoadInRam = true;
            this.applicationManager1.DataManager.ProgressHandler = null;
            this.applicationManager1.Directories = ((System.Collections.Generic.List<string>)(resources.GetObject("applicationManager1.Directories")));
            this.applicationManager1.DockManager = null;
            this.applicationManager1.HeaderControl = null;
            this.applicationManager1.Legend = this.mainLegend;
            this.applicationManager1.Map = this.mainMap;
            this.applicationManager1.ProgressHandler = this.mwStatusStrip1;
            this.applicationManager1.ShowExtensionsDialog = DotSpatial.Controls.ShowExtensionsDialog.None;
            this.applicationManager1.ToolManager = null;
            // 
            // mainRibbonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.mainLegend);
            this.Controls.Add(this.mainMap);
            this.Controls.Add(this.mwStatusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "mainRibbonForm";
            this.Text = "CUAHSI HydroDesktop";
            this.Load += new System.EventHandler(this.mainRibbonForm_Load);
            this.mwStatusStrip1.ResumeLayout(false);
            this.mwStatusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
        #endregion

        //private DotSpatial.Controls.RibbonControls.Ribbon ribbonControl;
        ////private DotSpatial.Controls.RibbonControls.RibbonTab tabHome;
        //private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem OrbNewProject;
        //private DotSpatial.Controls.RibbonControls.RibbonButton bntTableView;
        private DotSpatial.Controls.SpatialStatusStrip mwStatusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLocation;
        //private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem orbOpenProject;
        //private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem orbSaveProject;
        //private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem orbSaveProjectAs;
        //private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem OrbPrint;
        //private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem OrbExtentions;
        //private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem OrbApplicationSettings;
        //private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem OrbHelp;
        ////private DotSpatial.Controls.RibbonOrbMenuItem OrbExit;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        //private DotSpatial.Controls.RibbonControls.RibbonButton rbHelp;
        //private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem OrbAbout;
        internal DotSpatial.Controls.AppManager applicationManager1;
        //private DotSpatial.Controls.RibbonControls.RibbonSeparator ribbonSeparator1;
        //private DotSpatial.Controls.RibbonControls.RibbonSeparator ribbonSeparator2;
        //private DotSpatial.Controls.RibbonControls.RibbonSeparator ribbonSeparator3;
        //private DotSpatial.Controls.RibbonControls.RibbonOrbOptionButton ribbonOrbOptionButton_Exit;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private DotSpatial.Controls.Legend mainLegend;
        private DotSpatial.Controls.Map mainMap;
        
    }
}