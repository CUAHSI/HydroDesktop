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
            this.tabHome = new DotSpatial.Controls.RibbonControls.RibbonTab();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.tabContainer = new DotSpatial.Controls.SpatialTabControl();
            this.tabMapView = new System.Windows.Forms.TabPage();
            this.splitConMap = new System.Windows.Forms.SplitContainer();
            this.mainLegend = new DotSpatial.Controls.Legend();
            this.mainMap = new DotSpatial.Controls.Map();
            this.mwStatusStrip1 = new DotSpatial.Controls.SpatialStatusStrip();
            this.statusLocation = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.tabSeriesView = new System.Windows.Forms.TabPage();
            this.seriesView1 = new HydroDesktop.Controls.SeriesView();
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
            this.applicationManager1 = new HydroDesktop.Controls.HydroAppManager();
            this.panelContainer.SuspendLayout();
            this.tabContainer.SuspendLayout();
            this.tabMapView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitConMap)).BeginInit();
            this.splitConMap.Panel1.SuspendLayout();
            this.splitConMap.Panel2.SuspendLayout();
            this.splitConMap.SuspendLayout();
            this.mwStatusStrip1.SuspendLayout();
            this.tabSeriesView.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabHome
            // 
            this.tabHome.Tag = null;
            this.tabHome.Text = "Home";
            this.tabHome.ActiveChanged += new System.EventHandler(this.tabSearch_ActiveChanged);
            // 
            // panelContainer
            // 
            this.panelContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelContainer.Controls.Add(this.tabContainer);
            this.panelContainer.Location = new System.Drawing.Point(3, 139);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(999, 566);
            this.panelContainer.TabIndex = 2;
            // 
            // tabContainer
            // 
            this.tabContainer.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabContainer.Controls.Add(this.tabMapView);
            this.tabContainer.Controls.Add(this.tabSeriesView);
            this.tabContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabContainer.ItemSize = new System.Drawing.Size(0, 1);
            this.tabContainer.Location = new System.Drawing.Point(0, 0);
            this.tabContainer.Name = "tabContainer";
            this.tabContainer.SelectedIndex = 0;
            this.tabContainer.Size = new System.Drawing.Size(999, 566);
            this.tabContainer.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabContainer.TabIndex = 2;
            // 
            // tabMapView
            // 
            this.tabMapView.Controls.Add(this.splitConMap);
            this.tabMapView.Location = new System.Drawing.Point(4, 5);
            this.tabMapView.Name = "tabMapView";
            this.tabMapView.Padding = new System.Windows.Forms.Padding(3);
            this.tabMapView.Size = new System.Drawing.Size(991, 557);
            this.tabMapView.TabIndex = 0;
            this.tabMapView.Text = "MapView";
            this.tabMapView.UseVisualStyleBackColor = true;
            // 
            // splitConMap
            // 
            this.splitConMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitConMap.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitConMap.Location = new System.Drawing.Point(3, 3);
            this.splitConMap.Name = "splitConMap";
            // 
            // splitConMap.Panel1
            // 
            this.splitConMap.Panel1.Controls.Add(this.mainLegend);
            // 
            // splitConMap.Panel2
            // 
            this.splitConMap.Panel2.Controls.Add(this.mainMap);
            this.splitConMap.Size = new System.Drawing.Size(985, 551);
            this.splitConMap.SplitterDistance = 201;
            this.splitConMap.TabIndex = 0;
            // 
            // mainLegend
            // 
            this.mainLegend.BackColor = System.Drawing.Color.White;
            this.mainLegend.ControlRectangle = new System.Drawing.Rectangle(0, 0, 201, 551);
            this.mainLegend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLegend.DocumentRectangle = new System.Drawing.Rectangle(0, 0, 35, 20);
            this.mainLegend.HorizontalScrollEnabled = true;
            this.mainLegend.Indentation = 30;
            this.mainLegend.IsInitialized = false;
            this.mainLegend.Location = new System.Drawing.Point(0, 0);
            this.mainLegend.MinimumSize = new System.Drawing.Size(5, 5);
            this.mainLegend.Name = "mainLegend";
            this.mainLegend.ProgressHandler = null;
            this.mainLegend.ResetOnResize = false;
            this.mainLegend.SelectionFontColor = System.Drawing.Color.Black;
            this.mainLegend.SelectionHighlight = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(238)))), ((int)(((byte)(252)))));
            this.mainLegend.Size = new System.Drawing.Size(201, 551);
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
            this.mainMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainMap.ExtendBuffer = false;
            this.mainMap.FunctionMode = DotSpatial.Controls.FunctionMode.None;
            this.mainMap.IsBusy = false;
            this.mainMap.Legend = this.mainLegend;
            this.mainMap.Location = new System.Drawing.Point(0, 0);
            this.mainMap.Name = "mainMap";
            this.mainMap.ProgressHandler = this.mwStatusStrip1;
            this.mainMap.ProjectionEsriString = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.25722356" +
                "2997]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.0174532925199433]]";
            this.mainMap.ProjectionModeDefine = DotSpatial.Controls.ActionMode.Prompt;
            this.mainMap.ProjectionModeReproject = DotSpatial.Controls.ActionMode.Prompt;
            this.mainMap.RedrawLayersWhileResizing = false;
            this.mainMap.SelectionEnabled = true;
            this.mainMap.Size = new System.Drawing.Size(780, 551);
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
            this.lblStatus.Size = new System.Drawing.Size(675, 17);
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "loading...";
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
            // tabSeriesView
            // 
            this.tabSeriesView.Controls.Add(this.seriesView1);
            this.tabSeriesView.Location = new System.Drawing.Point(4, 5);
            this.tabSeriesView.Name = "tabSeriesView";
            this.tabSeriesView.Size = new System.Drawing.Size(991, 557);
            this.tabSeriesView.TabIndex = 1;
            this.tabSeriesView.Text = "Series View";
            this.tabSeriesView.UseVisualStyleBackColor = true;
            // 
            // seriesView1
            // 
            this.seriesView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.seriesView1.Location = new System.Drawing.Point(0, 0);
            this.seriesView1.Name = "seriesView1";
            this.seriesView1.Size = new System.Drawing.Size(991, 557);
            this.seriesView1.TabIndex = 0;
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
            this.applicationManager1.AppEnableMethod = DotSpatial.Controls.AppEnableMethod.None;
            this.applicationManager1.DataManager.DataProviderDirectories = ((System.Collections.Generic.List<string>)(resources.GetObject("applicationManager1.DataManager.DataProviderDirectories")));
            this.applicationManager1.DataManager.LoadInRam = true;
            this.applicationManager1.DataManager.ProgressHandler = null;
            this.applicationManager1.Directories = ((System.Collections.Generic.List<string>)(resources.GetObject("applicationManager1.Directories")));
            this.applicationManager1.LayoutControl = null;
            this.applicationManager1.Legend = this.mainLegend;
            this.applicationManager1.MainMenu = null;
            this.applicationManager1.MainToolStrip = null;
            this.applicationManager1.Map = this.mainMap;
            this.applicationManager1.ProgressHandler = this.mwStatusStrip1;
            this.applicationManager1.Ribbon = this.ribbonControl;
            this.applicationManager1.SeriesView = null;
            this.applicationManager1.TabManager = this.tabContainer;
            this.applicationManager1.ToolManager = null;
            this.applicationManager1.ToolStripContainer = null;
            // 
            // mainRibbonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.mwStatusStrip1);
            this.Controls.Add(this.ribbonControl);
            this.Controls.Add(this.panelContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "mainRibbonForm";
            this.Text = "CUAHSI HydroDesktop";
            this.Load += new System.EventHandler(this.mainRibbonForm_Load);
            this.panelContainer.ResumeLayout(false);
            this.tabContainer.ResumeLayout(false);
            this.tabMapView.ResumeLayout(false);
            this.splitConMap.Panel1.ResumeLayout(false);
            this.splitConMap.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitConMap)).EndInit();
            this.splitConMap.ResumeLayout(false);
            this.mwStatusStrip1.ResumeLayout(false);
            this.mwStatusStrip1.PerformLayout();
            this.tabSeriesView.ResumeLayout(false);
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
        private System.Windows.Forms.Panel panelContainer;
        private DotSpatial.Controls.SpatialTabControl tabContainer;
        private System.Windows.Forms.TabPage tabMapView;
        private System.Windows.Forms.SplitContainer splitConMap;
        private DotSpatial.Controls.Legend mainLegend;
        private DotSpatial.Controls.Map mainMap;
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
        private System.Windows.Forms.TabPage tabSeriesView;
        private HydroDesktop.Controls.SeriesView seriesView1;
        internal HydroDesktop.Controls.HydroAppManager applicationManager1;
        private DotSpatial.Controls.RibbonControls.RibbonSeparator ribbonSeparator1;
        private DotSpatial.Controls.RibbonControls.RibbonSeparator ribbonSeparator2;
        private DotSpatial.Controls.RibbonControls.RibbonSeparator ribbonSeparator3;
        private DotSpatial.Controls.RibbonControls.RibbonOrbOptionButton ribbonOrbOptionButton_Exit;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        
    }
}