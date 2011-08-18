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
            WeifenLuo.WinFormsUI.Docking.DockPanelSkin dockPanelSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPanelSkin();
            WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin autoHideStripSkin1 = new WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient1 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin dockPaneStripSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient dockPaneStripGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient2 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient2 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient3 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient dockPaneStripToolWindowGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient4 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient5 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient3 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient6 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient7 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            this.tabHome = new DotSpatial.Controls.RibbonControls.RibbonTab();
            this.mainLegend = new DotSpatial.Controls.Legend();
            this.mainMap = new DotSpatial.Controls.Map();
            this.mwStatusStrip1 = new DotSpatial.Controls.SpatialStatusStrip();
            this.statusLocation = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.ribbonControl = new DotSpatial.Controls.RibbonControls.Ribbon();
            this.OrbNewProject = new DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem();
            this.orbOpenProject = new DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem();
            this.orbSaveProject = new DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem();
            this.orbSaveProjectAs = new DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem();
            this.OrbPrint = new DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem();
            this.ribbonSeparator1 = new DotSpatial.Controls.RibbonControls.RibbonSeparator();
            this.OrbExtentions = new DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem();
            this.OrbApplicationSettings = new DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem();
            this.ribbonSeparator2 = new DotSpatial.Controls.RibbonControls.RibbonSeparator();
            this.OrbHelp = new DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem();
            this.OrbAbout = new DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem();
            this.ribbonSeparator3 = new DotSpatial.Controls.RibbonControls.RibbonSeparator();
            this.ribbonOrbOptionButton_Exit = new DotSpatial.Controls.RibbonControls.RibbonOrbOptionButton();
            this.rbHelp = new DotSpatial.Controls.RibbonControls.RibbonButton();
            this.bntTableView = new DotSpatial.Controls.RibbonControls.RibbonButton();
            this.applicationManager1 = new DotSpatial.Controls.AppManager();
            this.dockPanel1 = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.mwStatusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabHome
            // 
            this.tabHome.Tag = null;
            this.tabHome.Text = "Home";
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
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.mwStatusStrip1.Location = new System.Drawing.Point(0, 708);
            this.mwStatusStrip1.Name = "mwStatusStrip1";
            this.mwStatusStrip1.ProgressBar = this.toolStripProgressBar1;
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
            this.lblStatus.Size = new System.Drawing.Size(706, 17);
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BackColor = System.Drawing.Color.White;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // ribbonControl
            // 
            this.ribbonControl.BorderMode = DotSpatial.Controls.RibbonControls.RibbonWindowMode.NonClientAreaGlass;
            this.ribbonControl.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ribbonControl.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl.Minimized = false;
            this.ribbonControl.Name = "ribbonControl";
            // 
            // 
            // 
            this.ribbonControl.OrbDropDown.BorderRoundness = 8;
            this.ribbonControl.OrbDropDown.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl.OrbDropDown.MenuItems.Add(this.OrbNewProject);
            this.ribbonControl.OrbDropDown.MenuItems.Add(this.orbOpenProject);
            this.ribbonControl.OrbDropDown.MenuItems.Add(this.orbSaveProject);
            this.ribbonControl.OrbDropDown.MenuItems.Add(this.orbSaveProjectAs);
            this.ribbonControl.OrbDropDown.MenuItems.Add(this.OrbPrint);
            this.ribbonControl.OrbDropDown.MenuItems.Add(this.ribbonSeparator1);
            this.ribbonControl.OrbDropDown.MenuItems.Add(this.OrbExtentions);
            this.ribbonControl.OrbDropDown.MenuItems.Add(this.OrbApplicationSettings);
            this.ribbonControl.OrbDropDown.MenuItems.Add(this.ribbonSeparator2);
            this.ribbonControl.OrbDropDown.MenuItems.Add(this.OrbHelp);
            this.ribbonControl.OrbDropDown.MenuItems.Add(this.OrbAbout);
            this.ribbonControl.OrbDropDown.MenuItems.Add(this.ribbonSeparator3);
            this.ribbonControl.OrbDropDown.Name = "";
            this.ribbonControl.OrbDropDown.OptionItems.Add(this.ribbonOrbOptionButton_Exit);
            this.ribbonControl.OrbDropDown.Size = new System.Drawing.Size(527, 477);
            this.ribbonControl.OrbDropDown.TabIndex = 0;
            this.ribbonControl.OrbImage = ((System.Drawing.Image)(resources.GetObject("ribbonControl.OrbImage")));
            // 
            // 
            // 
            this.ribbonControl.QuickAcessToolbar.AltKey = null;
            this.ribbonControl.QuickAcessToolbar.DropDownButtonVisible = false;
            this.ribbonControl.QuickAcessToolbar.Image = null;
            this.ribbonControl.QuickAcessToolbar.Items.Add(this.rbHelp);
            this.ribbonControl.QuickAcessToolbar.Tag = null;
            this.ribbonControl.QuickAcessToolbar.Text = null;
            this.ribbonControl.QuickAcessToolbar.ToolTip = null;
            this.ribbonControl.QuickAcessToolbar.ToolTipImage = null;
            this.ribbonControl.QuickAcessToolbar.ToolTipTitle = null;
            this.ribbonControl.QuickAcessToolbar.Value = null;
            this.ribbonControl.Size = new System.Drawing.Size(1008, 141);
            this.ribbonControl.TabIndex = 0;
            this.ribbonControl.Tabs.Add(this.tabHome);
            this.ribbonControl.TabsMargin = new System.Windows.Forms.Padding(12, 26, 20, 0);
            this.ribbonControl.TabSpacing = 6;
            this.ribbonControl.Text = "ribbonControl";
            // 
            // OrbNewProject
            // 
            this.OrbNewProject.AltKey = null;
            this.OrbNewProject.DropDownArrowDirection = DotSpatial.Controls.RibbonControls.RibbonArrowDirection.Left;
            this.OrbNewProject.DropDownArrowSize = new System.Drawing.Size(5, 3);
            this.OrbNewProject.Image = ((System.Drawing.Image)(resources.GetObject("OrbNewProject.Image")));
            this.OrbNewProject.SmallImage = ((System.Drawing.Image)(resources.GetObject("OrbNewProject.SmallImage")));
            this.OrbNewProject.Style = DotSpatial.Controls.RibbonControls.RibbonButtonStyle.Normal;
            this.OrbNewProject.Tag = null;
            this.OrbNewProject.Text = "New Project";
            this.OrbNewProject.ToolTip = null;
            this.OrbNewProject.ToolTipImage = null;
            this.OrbNewProject.ToolTipTitle = null;
            this.OrbNewProject.Value = null;
            // 
            // orbOpenProject
            // 
            this.orbOpenProject.AltKey = null;
            this.orbOpenProject.DropDownArrowDirection = DotSpatial.Controls.RibbonControls.RibbonArrowDirection.Left;
            this.orbOpenProject.DropDownArrowSize = new System.Drawing.Size(5, 3);
            this.orbOpenProject.Image = ((System.Drawing.Image)(resources.GetObject("orbOpenProject.Image")));
            this.orbOpenProject.SmallImage = ((System.Drawing.Image)(resources.GetObject("orbOpenProject.SmallImage")));
            this.orbOpenProject.Style = DotSpatial.Controls.RibbonControls.RibbonButtonStyle.Normal;
            this.orbOpenProject.Tag = null;
            this.orbOpenProject.Text = "Open Project";
            this.orbOpenProject.ToolTip = null;
            this.orbOpenProject.ToolTipImage = null;
            this.orbOpenProject.ToolTipTitle = null;
            this.orbOpenProject.Value = null;
            // 
            // orbSaveProject
            // 
            this.orbSaveProject.AltKey = null;
            this.orbSaveProject.DropDownArrowDirection = DotSpatial.Controls.RibbonControls.RibbonArrowDirection.Left;
            this.orbSaveProject.DropDownArrowSize = new System.Drawing.Size(5, 3);
            this.orbSaveProject.Image = ((System.Drawing.Image)(resources.GetObject("orbSaveProject.Image")));
            this.orbSaveProject.SmallImage = ((System.Drawing.Image)(resources.GetObject("orbSaveProject.SmallImage")));
            this.orbSaveProject.Style = DotSpatial.Controls.RibbonControls.RibbonButtonStyle.Normal;
            this.orbSaveProject.Tag = null;
            this.orbSaveProject.Text = "Save Project";
            this.orbSaveProject.ToolTip = null;
            this.orbSaveProject.ToolTipImage = null;
            this.orbSaveProject.ToolTipTitle = null;
            this.orbSaveProject.Value = null;
            // 
            // orbSaveProjectAs
            // 
            this.orbSaveProjectAs.AltKey = null;
            this.orbSaveProjectAs.DropDownArrowDirection = DotSpatial.Controls.RibbonControls.RibbonArrowDirection.Left;
            this.orbSaveProjectAs.DropDownArrowSize = new System.Drawing.Size(5, 3);
            this.orbSaveProjectAs.Image = ((System.Drawing.Image)(resources.GetObject("orbSaveProjectAs.Image")));
            this.orbSaveProjectAs.SmallImage = ((System.Drawing.Image)(resources.GetObject("orbSaveProjectAs.SmallImage")));
            this.orbSaveProjectAs.Style = DotSpatial.Controls.RibbonControls.RibbonButtonStyle.Normal;
            this.orbSaveProjectAs.Tag = null;
            this.orbSaveProjectAs.Text = "Save Project As";
            this.orbSaveProjectAs.ToolTip = null;
            this.orbSaveProjectAs.ToolTipImage = null;
            this.orbSaveProjectAs.ToolTipTitle = null;
            this.orbSaveProjectAs.Value = null;
            // 
            // OrbPrint
            // 
            this.OrbPrint.AltKey = null;
            this.OrbPrint.DropDownArrowDirection = DotSpatial.Controls.RibbonControls.RibbonArrowDirection.Left;
            this.OrbPrint.DropDownArrowSize = new System.Drawing.Size(5, 3);
            this.OrbPrint.Image = ((System.Drawing.Image)(resources.GetObject("OrbPrint.Image")));
            this.OrbPrint.SmallImage = ((System.Drawing.Image)(resources.GetObject("OrbPrint.SmallImage")));
            this.OrbPrint.Style = DotSpatial.Controls.RibbonControls.RibbonButtonStyle.Normal;
            this.OrbPrint.Tag = null;
            this.OrbPrint.Text = "Print";
            this.OrbPrint.ToolTip = null;
            this.OrbPrint.ToolTipImage = null;
            this.OrbPrint.ToolTipTitle = null;
            this.OrbPrint.Value = null;
            this.OrbPrint.Click += new System.EventHandler(this.OrbPrint_Click);
            // 
            // ribbonSeparator1
            // 
            this.ribbonSeparator1.AltKey = null;
            this.ribbonSeparator1.Image = null;
            this.ribbonSeparator1.Tag = null;
            this.ribbonSeparator1.Text = null;
            this.ribbonSeparator1.ToolTip = null;
            this.ribbonSeparator1.ToolTipImage = null;
            this.ribbonSeparator1.ToolTipTitle = null;
            this.ribbonSeparator1.Value = null;
            // 
            // OrbExtentions
            // 
            this.OrbExtentions.AltKey = null;
            this.OrbExtentions.DropDownArrowDirection = DotSpatial.Controls.RibbonControls.RibbonArrowDirection.Left;
            this.OrbExtentions.DropDownArrowSize = new System.Drawing.Size(5, 3);
            this.OrbExtentions.Image = ((System.Drawing.Image)(resources.GetObject("OrbExtentions.Image")));
            this.OrbExtentions.SmallImage = ((System.Drawing.Image)(resources.GetObject("OrbExtentions.SmallImage")));
            this.OrbExtentions.Style = DotSpatial.Controls.RibbonControls.RibbonButtonStyle.SplitDropDown;
            this.OrbExtentions.Tag = null;
            this.OrbExtentions.Text = "Extensions";
            this.OrbExtentions.ToolTip = null;
            this.OrbExtentions.ToolTipImage = null;
            this.OrbExtentions.ToolTipTitle = null;
            this.OrbExtentions.Value = null;
            this.OrbExtentions.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OrbExtentions_MouseMove);
            // 
            // OrbApplicationSettings
            // 
            this.OrbApplicationSettings.AltKey = null;
            this.OrbApplicationSettings.DropDownArrowDirection = DotSpatial.Controls.RibbonControls.RibbonArrowDirection.Left;
            this.OrbApplicationSettings.DropDownArrowSize = new System.Drawing.Size(5, 3);
            this.OrbApplicationSettings.Image = ((System.Drawing.Image)(resources.GetObject("OrbApplicationSettings.Image")));
            this.OrbApplicationSettings.SmallImage = ((System.Drawing.Image)(resources.GetObject("OrbApplicationSettings.SmallImage")));
            this.OrbApplicationSettings.Style = DotSpatial.Controls.RibbonControls.RibbonButtonStyle.Normal;
            this.OrbApplicationSettings.Tag = null;
            this.OrbApplicationSettings.Text = "Application Settings";
            this.OrbApplicationSettings.ToolTip = null;
            this.OrbApplicationSettings.ToolTipImage = null;
            this.OrbApplicationSettings.ToolTipTitle = null;
            this.OrbApplicationSettings.Value = null;
            this.OrbApplicationSettings.Click += new System.EventHandler(this.OrbApplicationSettings_Click);
            // 
            // ribbonSeparator2
            // 
            this.ribbonSeparator2.AltKey = null;
            this.ribbonSeparator2.Image = null;
            this.ribbonSeparator2.Tag = null;
            this.ribbonSeparator2.Text = null;
            this.ribbonSeparator2.ToolTip = null;
            this.ribbonSeparator2.ToolTipImage = null;
            this.ribbonSeparator2.ToolTipTitle = null;
            this.ribbonSeparator2.Value = null;
            // 
            // OrbHelp
            // 
            this.OrbHelp.AltKey = null;
            this.OrbHelp.DropDownArrowDirection = DotSpatial.Controls.RibbonControls.RibbonArrowDirection.Left;
            this.OrbHelp.DropDownArrowSize = new System.Drawing.Size(5, 3);
            this.OrbHelp.Image = ((System.Drawing.Image)(resources.GetObject("OrbHelp.Image")));
            this.OrbHelp.SmallImage = ((System.Drawing.Image)(resources.GetObject("OrbHelp.SmallImage")));
            this.OrbHelp.Style = DotSpatial.Controls.RibbonControls.RibbonButtonStyle.Normal;
            this.OrbHelp.Tag = null;
            this.OrbHelp.Text = "Help";
            this.OrbHelp.ToolTip = null;
            this.OrbHelp.ToolTipImage = null;
            this.OrbHelp.ToolTipTitle = null;
            this.OrbHelp.Value = null;
            this.OrbHelp.Click += new System.EventHandler(this.OrbHelp_Click);
            // 
            // OrbAbout
            // 
            this.OrbAbout.AltKey = null;
            this.OrbAbout.DropDownArrowDirection = DotSpatial.Controls.RibbonControls.RibbonArrowDirection.Left;
            this.OrbAbout.DropDownArrowSize = new System.Drawing.Size(5, 3);
            this.OrbAbout.Image = ((System.Drawing.Image)(resources.GetObject("OrbAbout.Image")));
            this.OrbAbout.SmallImage = ((System.Drawing.Image)(resources.GetObject("OrbAbout.SmallImage")));
            this.OrbAbout.Style = DotSpatial.Controls.RibbonControls.RibbonButtonStyle.Normal;
            this.OrbAbout.Tag = null;
            this.OrbAbout.Text = "About";
            this.OrbAbout.ToolTip = null;
            this.OrbAbout.ToolTipImage = null;
            this.OrbAbout.ToolTipTitle = null;
            this.OrbAbout.Value = null;
            this.OrbAbout.Click += new System.EventHandler(this.OrbAbout_Click);
            // 
            // ribbonSeparator3
            // 
            this.ribbonSeparator3.AltKey = null;
            this.ribbonSeparator3.Image = null;
            this.ribbonSeparator3.Tag = null;
            this.ribbonSeparator3.Text = null;
            this.ribbonSeparator3.ToolTip = null;
            this.ribbonSeparator3.ToolTipImage = null;
            this.ribbonSeparator3.ToolTipTitle = null;
            this.ribbonSeparator3.Value = null;
            // 
            // ribbonOrbOptionButton_Exit
            // 
            this.ribbonOrbOptionButton_Exit.AltKey = null;
            this.ribbonOrbOptionButton_Exit.DropDownArrowDirection = DotSpatial.Controls.RibbonControls.RibbonArrowDirection.Down;
            this.ribbonOrbOptionButton_Exit.DropDownArrowSize = new System.Drawing.Size(5, 3);
            this.ribbonOrbOptionButton_Exit.Image = ((System.Drawing.Image)(resources.GetObject("ribbonOrbOptionButton_Exit.Image")));
            this.ribbonOrbOptionButton_Exit.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonOrbOptionButton_Exit.SmallImage")));
            this.ribbonOrbOptionButton_Exit.Style = DotSpatial.Controls.RibbonControls.RibbonButtonStyle.Normal;
            this.ribbonOrbOptionButton_Exit.Tag = null;
            this.ribbonOrbOptionButton_Exit.Text = "Exit HydroDesktop";
            this.ribbonOrbOptionButton_Exit.ToolTip = null;
            this.ribbonOrbOptionButton_Exit.ToolTipImage = null;
            this.ribbonOrbOptionButton_Exit.ToolTipTitle = null;
            this.ribbonOrbOptionButton_Exit.Value = null;
            this.ribbonOrbOptionButton_Exit.Click += new System.EventHandler(this.OrbExit_Click);
            // 
            // rbHelp
            // 
            this.rbHelp.AltKey = null;
            this.rbHelp.DropDownArrowDirection = DotSpatial.Controls.RibbonControls.RibbonArrowDirection.Down;
            this.rbHelp.DropDownArrowSize = new System.Drawing.Size(5, 3);
            this.rbHelp.Image = null;
            this.rbHelp.MaxSizeMode = DotSpatial.Controls.RibbonControls.RibbonElementSizeMode.Compact;
            this.rbHelp.SmallImage = ((System.Drawing.Image)(resources.GetObject("rbHelp.SmallImage")));
            this.rbHelp.Style = DotSpatial.Controls.RibbonControls.RibbonButtonStyle.Normal;
            this.rbHelp.Tag = null;
            this.rbHelp.Text = "Help";
            this.rbHelp.ToolTip = "HydroDesktop Online Help";
            this.rbHelp.ToolTipImage = null;
            this.rbHelp.ToolTipTitle = null;
            this.rbHelp.Value = null;
            this.rbHelp.Click += new System.EventHandler(this.rbHelp_Click);
            // 
            // bntTableView
            // 
            this.bntTableView.AltKey = null;
            this.bntTableView.DropDownArrowDirection = DotSpatial.Controls.RibbonControls.RibbonArrowDirection.Down;
            this.bntTableView.DropDownArrowSize = new System.Drawing.Size(5, 3);
            this.bntTableView.Image = ((System.Drawing.Image)(resources.GetObject("bntTableView.Image")));
            this.bntTableView.SmallImage = ((System.Drawing.Image)(resources.GetObject("bntTableView.SmallImage")));
            this.bntTableView.Style = DotSpatial.Controls.RibbonControls.RibbonButtonStyle.Normal;
            this.bntTableView.Tag = null;
            this.bntTableView.Text = null;
            this.bntTableView.ToolTip = null;
            this.bntTableView.ToolTipImage = null;
            this.bntTableView.ToolTipTitle = null;
            this.bntTableView.Value = null;
            // 
            // applicationManager1
            // 
            this.applicationManager1.DataManager.DataProviderDirectories = ((System.Collections.Generic.List<string>)(resources.GetObject("applicationManager1.DataManager.DataProviderDirectories")));
            this.applicationManager1.DataManager.LoadInRam = true;
            this.applicationManager1.DataManager.ProgressHandler = null;
            this.applicationManager1.Directories = ((System.Collections.Generic.List<string>)(resources.GetObject("applicationManager1.Directories")));
            this.applicationManager1.DockManager = null;
            this.applicationManager1.HeaderControl = null;
            this.applicationManager1.LayoutControl = null;
            this.applicationManager1.Legend = this.mainLegend;
            this.applicationManager1.Map = this.mainMap;
            this.applicationManager1.ProgressHandler = this.mwStatusStrip1;
            this.applicationManager1.ShowExtensionsDialog = DotSpatial.Controls.ShowExtensionsDialog.None;
            this.applicationManager1.ToolManager = null;
            // 
            // dockPanel1
            // 
            this.dockPanel1.ActiveAutoHideContent = null;
            this.dockPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel1.DockBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.dockPanel1.Location = new System.Drawing.Point(0, 141);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.Size = new System.Drawing.Size(1008, 567);
            dockPanelGradient1.EndColor = System.Drawing.SystemColors.ControlLight;
            dockPanelGradient1.StartColor = System.Drawing.SystemColors.ControlLight;
            autoHideStripSkin1.DockStripGradient = dockPanelGradient1;
            tabGradient1.EndColor = System.Drawing.SystemColors.Control;
            tabGradient1.StartColor = System.Drawing.SystemColors.Control;
            tabGradient1.TextColor = System.Drawing.SystemColors.ControlDarkDark;
            autoHideStripSkin1.TabGradient = tabGradient1;
            autoHideStripSkin1.TextFont = new System.Drawing.Font("Segoe UI", 9F);
            dockPanelSkin1.AutoHideStripSkin = autoHideStripSkin1;
            tabGradient2.EndColor = System.Drawing.SystemColors.ControlLightLight;
            tabGradient2.StartColor = System.Drawing.SystemColors.ControlLightLight;
            tabGradient2.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripGradient1.ActiveTabGradient = tabGradient2;
            dockPanelGradient2.EndColor = System.Drawing.SystemColors.Control;
            dockPanelGradient2.StartColor = System.Drawing.SystemColors.Control;
            dockPaneStripGradient1.DockStripGradient = dockPanelGradient2;
            tabGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
            tabGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
            tabGradient3.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripGradient1.InactiveTabGradient = tabGradient3;
            dockPaneStripSkin1.DocumentGradient = dockPaneStripGradient1;
            dockPaneStripSkin1.TextFont = new System.Drawing.Font("Segoe UI", 9F);
            tabGradient4.EndColor = System.Drawing.SystemColors.ActiveCaption;
            tabGradient4.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            tabGradient4.StartColor = System.Drawing.SystemColors.GradientActiveCaption;
            tabGradient4.TextColor = System.Drawing.SystemColors.ActiveCaptionText;
            dockPaneStripToolWindowGradient1.ActiveCaptionGradient = tabGradient4;
            tabGradient5.EndColor = System.Drawing.SystemColors.Control;
            tabGradient5.StartColor = System.Drawing.SystemColors.Control;
            tabGradient5.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripToolWindowGradient1.ActiveTabGradient = tabGradient5;
            dockPanelGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
            dockPanelGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
            dockPaneStripToolWindowGradient1.DockStripGradient = dockPanelGradient3;
            tabGradient6.EndColor = System.Drawing.SystemColors.InactiveCaption;
            tabGradient6.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            tabGradient6.StartColor = System.Drawing.SystemColors.GradientInactiveCaption;
            tabGradient6.TextColor = System.Drawing.SystemColors.InactiveCaptionText;
            dockPaneStripToolWindowGradient1.InactiveCaptionGradient = tabGradient6;
            tabGradient7.EndColor = System.Drawing.Color.Transparent;
            tabGradient7.StartColor = System.Drawing.Color.Transparent;
            tabGradient7.TextColor = System.Drawing.SystemColors.ControlDarkDark;
            dockPaneStripToolWindowGradient1.InactiveTabGradient = tabGradient7;
            dockPaneStripSkin1.ToolWindowGradient = dockPaneStripToolWindowGradient1;
            dockPanelSkin1.DockPaneStripSkin = dockPaneStripSkin1;
            this.dockPanel1.Skin = dockPanelSkin1;
            this.dockPanel1.TabIndex = 1;
            // 
            // mainRibbonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.mainLegend);
            this.Controls.Add(this.mainMap);
            this.Controls.Add(this.dockPanel1);
            this.Controls.Add(this.mwStatusStrip1);
            this.Controls.Add(this.ribbonControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "mainRibbonForm";
            this.Text = "CUAHSI HydroDesktop";
            this.Load += new System.EventHandler(this.mainRibbonForm_Load);
            this.mwStatusStrip1.ResumeLayout(false);
            this.mwStatusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        void DoSplash()
        {
            hdSplash din = new hdSplash();
            din.ShowDialog();
        }
        
        #endregion

        private DotSpatial.Controls.RibbonControls.Ribbon ribbonControl;
        private DotSpatial.Controls.RibbonControls.RibbonTab tabHome;
        private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem OrbNewProject;
        private DotSpatial.Controls.RibbonControls.RibbonButton bntTableView;
        private DotSpatial.Controls.SpatialStatusStrip mwStatusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLocation;
        private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem orbOpenProject;
        private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem orbSaveProject;
        private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem orbSaveProjectAs;
        private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem OrbPrint;
        private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem OrbExtentions;
        private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem OrbApplicationSettings;
        private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem OrbHelp;
        //private DotSpatial.Controls.RibbonOrbMenuItem OrbExit;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private DotSpatial.Controls.RibbonControls.RibbonButton rbHelp;
        private DotSpatial.Controls.RibbonControls.RibbonOrbMenuItem OrbAbout;
        internal DotSpatial.Controls.AppManager applicationManager1;
        private DotSpatial.Controls.RibbonControls.RibbonSeparator ribbonSeparator1;
        private DotSpatial.Controls.RibbonControls.RibbonSeparator ribbonSeparator2;
        private DotSpatial.Controls.RibbonControls.RibbonSeparator ribbonSeparator3;
        private DotSpatial.Controls.RibbonControls.RibbonOrbOptionButton ribbonOrbOptionButton_Exit;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel1;
        private DotSpatial.Controls.Legend mainLegend;
        private DotSpatial.Controls.Map mainMap;
        
    }
}