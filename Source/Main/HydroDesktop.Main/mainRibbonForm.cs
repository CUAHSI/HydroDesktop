using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DotSpatial.Controls;
using DotSpatial.Controls.RibbonControls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using HydroDesktop.Configuration;
using HydroDesktop.Controls.Themes;
using HydroDesktop.Database;
using HydroDesktop.Help;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;
using WeifenLuo.WinFormsUI.Docking;

namespace HydroDesktop.Main
{
    public partial class mainRibbonForm : Form
    {
        #region Variable

        private const string _mainHelpFile = "welcome.html";

        //store the map extents
        private List<Extent> _previousExtents = new List<Extent>();
        private List<Extent> _nextExtents = new List<Extent>();
        private int _mCurrentExtents = 0;
        bool _IsManualExtentsChange = false;

        private Extent _defaultMapExtent = new Extent(-170, -50, 170, 50);

        //the default projection of the map - changed to 'Web Mercator Auxiliary Sphere'
        private ProjectionInfo _defaultProjection;
        private ProjectionInfo _wgs84Projection;

        //MapView Ribbon TabPage and its related controls
        private RibbonTab _mapView;
        //private RibbonButton _rbPrint;
        private RibbonButton _rbAdd;
        private RibbonButton _rbPan;
        private RibbonButton _rbSelect;
        private RibbonButton _rbZoomIn;
        private RibbonButton _rbZoomOut;
        private RibbonButton _rbIdentifier;
        private RibbonButton _rbAttribute;
        private RibbonButton _rbMaxExtents;
        private RibbonButton _rbMeasure;
        private RibbonButton _rbZoomToPrevious;
        private RibbonButton _rbZoomToNext;

        //project file related indicators
        private bool _isNewProject = false;
        private WelcomeScreen _welcomeScreen = null;
        private string _projectFileName = null;
        private ProjectChangeTracker _projectManager = null;

        //docking
        private DockPanel dockManager = new WeifenLuo.WinFormsUI.Docking.DockPanel();
        #endregion Variable

        #region Constructor

        /// <summary>
        /// Initialize MapView
        /// </summary>
        /// <param name="args">The file name to open when HydroDesktop starts</param>
        public mainRibbonForm(string[] args)
        {
            InitializeComponent();

            //screen size
            AdjustFormSize();

            this.Shown += new EventHandler(mainRibbonForm_Shown);

            mainMap.MapFrame.ViewExtentsChanged += new EventHandler<ExtentArgs>(MapFrame_ExtentsChanged);
            this.SizeChanged += new EventHandler(mainRibbonForm_SizeChanged);
            this.FormClosing += new FormClosingEventHandler(mainRibbonForm_FormClosing);

            this.ribbonControl.Tabs[0].Tag = DotSpatial.Controls.Header.HeaderControl.HomeRootItemKey;
            this.applicationManager1.HeaderControl = new RibbonHeaderControl(this.ribbonControl);

            // setup docking...
            dockManager.Parent = this;
            this.applicationManager1.DockManager = new DockingManager(dockManager);

            // display panelContainer front and center
            panelContainer.Dock = DockStyle.Fill;

            DockContent content = new DockContent();
            content.ShowHint = DockState.Document;
            content.Controls.Add(panelContainer);
            content.Show(dockManager);

            #region initialize the help menu

            rbHelp.Image = Properties.Resources.help;
            rbHelp.SmallImage = Properties.Resources.help_16x16;

            #endregion initialize the help menu

            #region initialize the MapView Ribbon TabPage and related controls

            _mapView = this.ribbonControl.Tabs[0];
            _mapView.ActiveChanged += new EventHandler(_mapView_ActiveChanged);
            RibbonPanel rpMapTools = new RibbonPanel("Map Tools", RibbonPanelFlowDirection.Bottom);
            rpMapTools.ButtonMoreVisible = false;
            _mapView.Panels.Add(rpMapTools);

            //Pan
            _rbPan = new RibbonButton("Pan");
            rpMapTools.Items.Add(_rbPan);
            _rbPan.Image = Properties.Resources.pan;
            _rbPan.SmallImage = Properties.Resources.pan_16;
            _rbPan.ToolTip = "Pan - Move the Map";
            _rbPan.Click += new EventHandler(rbPan_Click);

            //ZoomIn
            _rbZoomIn = new RibbonButton("Zoom In");
            _rbZoomIn.ToolTip = "Zoom In";
            rpMapTools.Items.Add(_rbZoomIn);
            _rbZoomIn.Image = Properties.Resources.zoom_in;
            _rbZoomIn.SmallImage = Properties.Resources.zoom_in_16;
            _rbZoomIn.Click += new EventHandler(rbZoomIn_Click);

            //ZoomOut
            _rbZoomOut = new RibbonButton("Zoom Out");
            _rbZoomOut.ToolTip = "Zoom Out";
            rpMapTools.Items.Add(_rbZoomOut);
            _rbZoomOut.Image = Properties.Resources.zoom_out;
            _rbZoomOut.SmallImage = Properties.Resources.zoom_out_16;
            _rbZoomOut.Click += new EventHandler(rbZoomOut_Click);

            //ZoomToFullExtent
            _rbMaxExtents = new RibbonButton("MaxExtents");
            _rbMaxExtents.ToolTip = "Maximum Extents";
            rpMapTools.Items.Add(_rbMaxExtents);
            _rbMaxExtents.Image = Properties.Resources.full_extent;
            _rbMaxExtents.SmallImage = Properties.Resources.full_extent_16;
            _rbMaxExtents.Click += new EventHandler(rbMaxExtents_Click);

            //ZoomToPrevious
            _rbZoomToPrevious = new RibbonButton("Previous");
            _rbZoomToPrevious.ToolTip = "Go To Previous Map Extent";
            rpMapTools.Items.Add(_rbZoomToPrevious);
            _rbZoomToPrevious.Image = Properties.Resources.zoom_to_previous;
            _rbZoomToPrevious.SmallImage = Properties.Resources.full_extent_16;

            if (_previousExtents.Count == 0)
                _rbZoomToPrevious.Enabled = false;

            _rbZoomToPrevious.Click += new EventHandler(rbZoomToPrevious_Click);

            //ZoomToNext
            _rbZoomToNext = new RibbonButton("Next");
            _rbZoomToNext.ToolTip = "Go To Next Map Extent";
            rpMapTools.Items.Add(_rbZoomToNext);
            _rbZoomToNext.Image = Properties.Resources.zoom_to_next;
            _rbZoomToNext.SmallImage = Properties.Resources.zoom_to_next_16;

            if ((_mCurrentExtents < _previousExtents.Count - 1) != true)
                _rbZoomToNext.Enabled = false;

            _rbZoomToNext.Click += new EventHandler(rbZoomToNext_Click);

            ////Print
            //_rbPrint = new RibbonButton("Print");
            //rpMapTools.Items.Add(_rbPrint);
            //_rbPrint.Image = Properties.Resources.print;
            //_rbPrint.Click += new EventHandler(rbPrint_Click);
            ////_mapView.Panels[0].Items.Add(new RibbonButton("Print"));

            //Separator
            RibbonSeparator mapTools = new RibbonSeparator();
            rpMapTools.Items.Add(mapTools);

            //Add
            _rbAdd = new RibbonButton("Add");
            _rbAdd.ToolTip = "Add Layer To The Map";
            rpMapTools.Items.Add(_rbAdd);
            _rbAdd.Image = Properties.Resources.add;
            _rbAdd.SmallImage = Properties.Resources.add_16;
            _rbAdd.Click += new EventHandler(rbAdd_Click);

            //Identifier
            _rbIdentifier = new RibbonButton("Identify");
            _rbIdentifier.ToolTip = "Identify";
            _rbIdentifier.SmallImage = Properties.Resources.identifier_16;
            rpMapTools.Items.Add(_rbIdentifier);
            _rbIdentifier.Image = Properties.Resources.identifier;
            _rbIdentifier.Click += new EventHandler(rbIdentifier_Click);

            //Select
            _rbSelect = new RibbonButton("Select");
            _rbSelect.ToolTip = "Select";
            rpMapTools.Items.Add(_rbSelect);
            _rbSelect.Image = Properties.Resources.select;
            _rbSelect.SmallImage = Properties.Resources.select_16;
            _rbSelect.Click += new EventHandler(rbSelect_Click);

            //AttributeTable
            _rbAttribute = new RibbonButton("Attribute");
            _rbAttribute.ToolTip = "Attribute Table";
            rpMapTools.Items.Add(_rbAttribute);
            _rbAttribute.Image = Properties.Resources.attribute_table;
            _rbAttribute.SmallImage = Properties.Resources.attribute_table_16;
            _rbAttribute.Click += new EventHandler(rbAttribute_Click);

            //Measure
            _rbMeasure = new RibbonButton("Measure");
            _rbMeasure.ToolTip = "Measure Distance or Area";
            rpMapTools.Items.Add(_rbMeasure);
            _rbMeasure.Image = Properties.Resources.measure;
            _rbMeasure.SmallImage = Properties.Resources.measure_16;
            _rbMeasure.Click += new EventHandler(rbMeasure_Click);

            #endregion initialize the MapView Ribbon TabPage and related controls
            
            #region initialize the Main menu and related controls

            OrbNewProject.Click += new EventHandler(OrbNewProject_Click);
            orbOpenProject.Click += new EventHandler(OrbOpenProject_Click);
            orbSaveProject.Click += new EventHandler(orbSaveProject_Click);
            orbSaveProjectAs.Click += new EventHandler(orbSaveProjectAs_Click);

            #endregion initialize the Main menu and related controls

            #region initialize the project file management

            if (args != null)
            {
                if (args.Length > 0)
                {
                    string projectFile = args[0];
                    if (File.Exists(projectFile))
                    {
                        _projectFileName = projectFile;
                    }
                }
            }

            #endregion initialize the project file management

            #region initialize the default map projection

            _wgs84Projection = new ProjectionInfo();
            _wgs84Projection.ReadEsriString(Properties.Resources.Wgs84EsriString);

            _defaultProjection = new ProjectionInfo();
            _defaultProjection.CopyProperties(KnownCoordinateSystems.Projected.World.WebMercator);
            mainMap.MapFrame.Projection = new ProjectionInfo();
            mainMap.MapFrame.Projection.CopyProperties(_defaultProjection);

            //handles the default (new) project
            if (String.IsNullOrEmpty(_projectFileName))
            {
                Settings.Instance.CurrentProjectFile = Settings.Instance.DefaultProjectFileName;
            }

            #endregion initialize the default map projection

            #region initialize the main view panel controls

            this.tabContainer.SelectedIndexChanged += new EventHandler(tabContainer_SelectedIndexChanged);

            #endregion initialize the main view panel controls

            #region initialize default database connection strings

            SetupDatabases();

            #endregion initialize default database connection strings

            #region Initialize the Project opening events

            applicationManager1.SerializationManager.Deserializing += new EventHandler<SerializingEventArgs>(SerializationManager_Deserializing);
            applicationManager1.SerializationManager.Serializing += new EventHandler<SerializingEventArgs>(SerializationManager_Serializing);
            _projectManager = new ProjectChangeTracker(applicationManager1);
            _projectManager.ProjectModified += new EventHandler(_projectManager_ProjectModified);

            #endregion Initialize the Project opening events
        }

        #region Method

        /// <summary>
        /// Adjusts the size of the main form, if computer screen
        /// is smaller than the default size.
        /// </summary>
        private void AdjustFormSize()
        {
            int deskHeight = Screen.PrimaryScreen.Bounds.Height - 50;
            int deskWidth = Screen.PrimaryScreen.Bounds.Width;
            if (deskHeight < Height || deskWidth < Width)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        /// <summary>
        /// This method sets up the default databases.
        /// By default these are created in the temporary directory.
        /// </summary>
        public void SetupDatabases()
        {
            // use the 'default' database path is a temporary db file
            // and only should be used when not working with a project.
            string dataRepositoryTempFile = string.Format("NewProject_{0}_{1}{2}.sqlite",
                DateTime.Now.Date.ToString("yyyy-MM-dd"), DateTime.Now.Hour, DateTime.Now.Minute);

            string metadataCacheTempFile = string.Format("NewProject_{0}_{1}{2}_cache.sqlite",
               DateTime.Now.Date.ToString("yyyy-MM-dd"), DateTime.Now.Hour, DateTime.Now.Minute);

            string tempDir = Settings.Instance.TempDirectory;
            string dataRepositoryPath = Path.Combine(tempDir, dataRepositoryTempFile);

            string metadataCachePath = Path.Combine(tempDir, metadataCacheTempFile);

            if (HasWriteAccessToFolder(tempDir))
            {
                //create new dataRepositoryDb
                SQLiteHelper.CreateSQLiteDatabase(dataRepositoryPath);
                string conString1 = SQLiteHelper.GetSQLiteConnectionString(dataRepositoryPath);
                Settings.Instance.DataRepositoryConnectionString = conString1;
                Settings.Instance.CurrentProjectFile = Path.ChangeExtension(dataRepositoryPath, ".dspx");

                //create new metadataCacheDb
                SQLiteHelper.CreateMetadataCacheDb(metadataCachePath);
                string conString2 = SQLiteHelper.GetSQLiteConnectionString(metadataCachePath);
                Settings.Instance.MetadataCacheConnectionString = conString2;

                //allow plugins to access the SeriesView and SeriesSelector
                applicationManager1.SeriesView = this.seriesView1;

                //setup db property of SeriesSelector
                //this code will fail if DataRepositoryConnectionString is not set
                seriesView1.SeriesSelector.SetupDatabase();
            }
        }

        // fix 2011/06/06 :
        // issue 7216 : http://hydrodesktop.codeplex.com/workitem/7216
        // temp files swap
        public void SwapDatabasesOnExit()
        {
            // delete all temp file under this folder
            string tempDir = Settings.Instance.TempDirectory;
            if (!HasWriteAccessToFolder(tempDir)) return;
            if (!Directory.Exists(tempDir)) return;

            // enum and delete
            DirectoryInfo dirInf = new DirectoryInfo(tempDir);
            FileInfo[] fileInfs = dirInf.GetFiles();
            if (fileInfs.Length == 0) return;

            for (int i = 0; i < fileInfs.Length; i++)
            {
                DeleteTempFile(fileInfs[i]);
            }
        }

        // fixed issue 7216
        // delete temp file pointed by fiTemp
        protected bool DeleteTempFile(FileInfo fiTemp)
        {
            if (!fiTemp.Exists)
            { // maybe is a folder
                return false;
            }

            try
            {
                if (!fiTemp.Name.ToLower().StartsWith("theme"))
                {
                    fiTemp.Delete();
                }
                return true;
            }
            catch (System.Exception ex)
            {
                // at a bare minimum we will log the reason the file couldn't be deleted.
                Trace.WriteLine(ex.Message);
                return false;
            }
        }

        private bool HasWriteAccessToFolder(string folderPath)
        {
            try
            {
                // Attempt to get a list of security permissions from the folder.
                // This will raise an exception if the path is read only or do not have access to view the permissions.
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        #endregion Method

        #region Event

        private void MapFrame_LayerAdded(object sender, LayerEventArgs e)
        {
            e.Layer.ItemChanged += new EventHandler(ThemeLayer_ItemChanged);
        }

        private void _projectManager_ProjectModified(object sender, EventArgs e)
        {
            UpdateFormCaption();
        }

        private void UpdateFormCaption()
        {
            if (_projectManager == null)
            {
                this.Text = "CUAHSI HydroDesktop";
            }

            //show a 'star' indicating modification of project
            string projFileText = Path.GetFileNameWithoutExtension(Settings.Instance.CurrentProjectFile);
            if (projFileText.StartsWith("NewProject"))
            {
                projFileText = String.Empty;
            }
            else
            {
                projFileText = "- " + projFileText;
            }

            if (_projectManager.ProjectIsSaved)
            {
                this.Text = "CUAHSI HydroDesktop" + projFileText;
            }
            else
            {
                this.Text = "CUAHSI HydroDesktop" + projFileText + " * ";
            }
        }

        //when project is being saved
        private void SerializationManager_Serializing(object sender, SerializingEventArgs e)
        {
            //throw new NotImplementedException();
            //if (!String.IsNullOrEmpty(e.ProjectFileName))
            //{
            //    Settings.Instance.AddFileToRecentFiles(e.ProjectFileName);
            //}
        }

        //when project is being opened
        private void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            //assign the correct projection
            mainMap.Projection = _defaultProjection;

            //set the 'current project' default property
            Settings.Instance.CurrentProjectFile = applicationManager1.SerializationManager.CurrentProjectFile;

            RefreshTheLayers();

            mainMap.ResetBuffer();
            mainMap.MapFrame.ResetExtents();
            //.ZoomToMaxExtent();

            //Set the correct SQLite databases file path
            //TODO: use customSettings in projectFile, if available
            string dbFile = Path.ChangeExtension(Settings.Instance.CurrentProjectFile, ".sqlite");
            if (SQLiteHelper.DatabaseExists(dbFile))
            {
                Settings.Instance.DataRepositoryConnectionString = SQLiteHelper.GetSQLiteConnectionString(dbFile);
                applicationManager1.SeriesView.SeriesSelector.SetupDatabase();
            }

            //Set the correct SQLite file path for metadata cache DB
            string metadataCacheDbFile = Settings.Instance.CurrentProjectFile.Replace(".dspx", "_cache.sqlite");
            if (SQLiteHelper.DatabaseExists(dbFile))
            {
                Settings.Instance.MetadataCacheConnectionString = SQLiteHelper.GetSQLiteConnectionString(metadataCacheDbFile);
            }
            else
            {
                //if metadata cache db file is not found, create it.
                SQLiteHelper.CreateMetadataCacheDb(metadataCacheDbFile);
            }

            //to refresh all themes
            RefreshAllThemes();

            //event for changing theme name
            foreach (ILayer item in mainMap.MapFrame.GetAllLayers())
            {
                if (item.GetParentItem().LegendText == "Themes")
                {
                    item.ItemChanged += new EventHandler(ThemeLayer_ItemChanged);
                }
            }
            foreach (IMapGroup group in mainMap.MapFrame.GetAllGroups())
            {
                if (group.LegendText == "Themes")
                {
                    group.LayerAdded += new EventHandler<LayerEventArgs>(MapFrame_LayerAdded);
                }
            }

            //set the project change tracker
            _projectManager.ProjectIsSaved = true;

            //update caption of form
            UpdateFormCaption();
        }

        private void RefreshTheLayers()
        {
            //layers with categories need to populate their attribute table to redraw successfully.
            //this function also handles the reprojection.
            List<ILayer> layerList = mainMap.MapFrame.GetAllLayers();
            List<IMapFeatureLayer> emptyLayers = new List<IMapFeatureLayer>();
            int numLayers = layerList.Count;
            int counter = 0;
            foreach (IMapLayer layer in mainMap.MapFrame.GetAllLayers())
            {
                IMapFeatureLayer featureLayer = layer as IMapFeatureLayer;
                if (featureLayer != null)
                {
                    //report progress
                    counter++;
                    int percent = (counter * 100) / numLayers;
                    string message = String.Format("Adding Layer {0} {1}% complete.", featureLayer.LegendText, percent);
                    ReportProgress(percent, message);

                    if (featureLayer.Projection != null && !String.IsNullOrEmpty(featureLayer.DataSet.Filename))
                    {
                        featureLayer.DataSet.Reproject(_defaultProjection);
                        if (!featureLayer.DataSet.AttributesPopulated)
                        {
                            featureLayer.DataSet.FillAttributes();
                        }
                        featureLayer.ApplyScheme(featureLayer.Symbology);
                    }
                    else
                    {
                        emptyLayers.Add(featureLayer);
                    }
                }
            }

            //remove any empty layers of layers with invalid dataset
            //foreach (IMapFeatureLayer layer in emptyLayers)
            //{
            //    mainMap.Layers.Remove(layer);
            //    foreach (IMapGroup group in mainMap.GetAllGroups())
            //    {
            //        group.Remove(layer);
            //    }
            //}
            //emptyLayers.Clear();
        }

        //rename the theme name
        private void ThemeLayer_ItemChanged(object sender, EventArgs e)
        {
            MapPointLayer lay = sender as MapPointLayer;
            if (lay != null)
            {
                if (lay.GetParentItem() == null) return;
                if (lay.GetParentItem().LegendText != "Themes") return;

                string newLegendText = lay.LegendText;
                string name = lay.Name;
                if (!string.IsNullOrEmpty(name))
                {
                    try
                    {
                        int themeID = Convert.ToInt32(name);
                        ThemeManager manager = new ThemeManager(new DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite));
                        string oldName = manager.GetThemeName(themeID);
                        if (newLegendText != oldName)
                        {
                            manager.RenameTheme(themeID, newLegendText);
                            applicationManager1.SeriesView.SeriesSelector.SetupDatabase();
                        }
                    }
                    catch { }
                }
            }
        }

        #endregion Event

        //hide status bar when map panel is not shown
        private void tabContainer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabContainer.SelectedTabName == "MapView")
            {
                statusLocation.Visible = true;
                lblStatus.Visible = false;
            }
            else
            {
                statusLocation.Visible = false;
                lblStatus.Visible = true;
                lblStatus.Text = "Database: " + SQLiteHelper.GetSQLiteFileName(Settings.Instance.DataRepositoryConnectionString);
                //applicationManager1.SerializationManager.GetCustomSetting<string>("DataRepositoryDbPath", "unknown db path");
            }
        }

        //Refresh map when main form is maximized
        private void mainRibbonForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                mainMap.MapFrame.ResetExtents();
            }
        }

        //Load Form
        private void mainRibbonForm_Load(object sender, EventArgs e)
        {
            //to add the 'data' main view tab
            this.tabContainer.Appearance = TabAppearance.FlatButtons;
            this.tabContainer.ItemSize = new Size(0, 1);
            this.tabContainer.SizeMode = TabSizeMode.Fixed;

            //Set Initial Map Projection
            mainMap.Projection = _defaultProjection;
        }

        /// <summary>
        /// When the main form is first shown
        /// </summary>
        private void mainRibbonForm_Shown(object sender, EventArgs e)
        {
            //show the welcome screen, or open a project
            if (String.IsNullOrEmpty(_projectFileName))
            {
                ShowWelcomeScreen();
            }
            else
            {
                //MessageBox.Show("Opening Project as File association:" + _projectFileName);
                Project.OpenProject(_projectFileName, applicationManager1);
            }
        }

        #endregion Constructor

        private void ShowWelcomeScreen()
        {
            _welcomeScreen = new WelcomeScreen(applicationManager1);
            _welcomeScreen.StartPosition = FormStartPosition.Manual;

            int x = this.Location.X + this.Width / 2 - _welcomeScreen.Width / 2;
            int y = this.Location.Y + this.Height / 2 - _welcomeScreen.Height / 2;
            _welcomeScreen.Location = new System.Drawing.Point(x, y);

            if (_welcomeScreen.ShowDialog() == DialogResult.OK)
            {
                _isNewProject = _welcomeScreen.NewProjectCreated;
            }
            //reset the progress handler
            applicationManager1.ProgressHandler = mwStatusStrip1;
        }

        //sets the map extent to continental U.S
        private void SetDefaultMapExtents()
        {
            double[] xy = new double[4];
            xy[0] = _defaultMapExtent.MinX;
            xy[1] = _defaultMapExtent.MinY;
            xy[2] = _defaultMapExtent.MaxX;
            xy[3] = _defaultMapExtent.MaxY;
            double[] z = new double[] { 0, 0 };
            ProjectionInfo wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;
            Reproject.ReprojectPoints(xy, z, wgs84, mainMap.Projection, 0, 2);

            mainMap.ViewExtents = new Extent(xy);
        }

        /// <summary>
        /// Reads all themes from the database and displays them on the map
        /// </summary>
        public void RefreshAllThemes()
        {
            ThemeManager manager = new ThemeManager(Settings.Instance.DataRepositoryConnectionString);
            manager.RefreshAllThemes(mainMap);
        }

        #region Map Tools Click Events

        /// <summary>
        /// Map Tools Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //void rbPrint_Click(object sender, EventArgs e)
        //{
        //    foreach (RibbonButton rb in _mapView.Panels[0].Items)
        //    {
        //        if (rb == _rbPrint)
        //        {
        //            _rbPrint.Checked = true;
        //        }
        //        else
        //        {
        //            rb.Checked = false;
        //        }
        //    }

        //    DotSpatial.Forms.LayoutForm layoutFrm = new DotSpatial.Forms.LayoutForm();
        //    layoutFrm.MapControl = mainMap;
        //    layoutFrm.Show(this);
        //}

        private void rbAdd_Click(object sender, EventArgs e)
        {
            //add a layer to the map
            if (mainMap == null) return;

            mainMap.AddLayers();
        }

        private void rbPan_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _mapView.Panels[0].Items.Count; i++)
            {
                if (_mapView.Panels[0].Items[i] == _rbPan)
                {
                    _rbPan.Checked = true;
                }
                else
                {
                    _mapView.Panels[0].Items[i].Checked = false;
                }
            }

            mainMap.FunctionMode = FunctionMode.Pan;
        }

        private void rbSelect_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _mapView.Panels[0].Items.Count; i++)
            {
                if (_mapView.Panels[0].Items[i] == _rbSelect)
                {
                    _rbSelect.Checked = true;
                }
                else
                {
                    _mapView.Panels[0].Items[i].Checked = false;
                }
            }

            mainMap.FunctionMode = FunctionMode.Select;
        }

        private void rbZoomIn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _mapView.Panels[0].Items.Count; i++)
            {
                if (_mapView.Panels[0].Items[i] == _rbZoomIn)
                {
                    _rbZoomIn.Checked = true;
                }
                else
                {
                    _mapView.Panels[0].Items[i].Checked = false;
                }
            }

            mainMap.FunctionMode = FunctionMode.ZoomIn;
        }

        private void rbZoomOut_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _mapView.Panels[0].Items.Count; i++)
            {
                if (_mapView.Panels[0].Items[i] == _rbZoomOut)
                {
                    _rbZoomOut.Checked = true;
                }
                else
                {
                    _mapView.Panels[0].Items[i].Checked = false;
                }
            }

            mainMap.FunctionMode = FunctionMode.ZoomOut;
        }

        private void MapFrame_ExtentsChanged(object sender, EventArgs e)
        {
            //If m_Extents Is Nothing Then Exit Sub
            if (_previousExtents == null) return;
            //If MapMain.NumLayers = 0 Then Exit Sub
            if (this.mainMap.Layers.Count == 0) return;
            //If m_IsManualExtentsChange = True Then
            //    m_IsManualExtentsChange = False 'reset the flag for the next extents change
            //Else
            //    FlushForwardHistory()
            //    m_Extents.Add(MapMain.Extents)
            //    m_CurrentExtent = m_Extents.Count - 1
            //End If
            if (_IsManualExtentsChange == true)
            {
                _IsManualExtentsChange = false;
            }
            else
            {
                _previousExtents.Add(mainMap.ViewExtents);
                _mCurrentExtents = _previousExtents.Count - 1;
            }

            if (_mCurrentExtents < _previousExtents.Count - 1)
            {
                _rbZoomToNext.Enabled = true;
            }

            if ((_previousExtents.Count > 0) && (_mCurrentExtents > 0))
            {
                _rbZoomToPrevious.Enabled = true;
            }
        }

        private void rbZoomToPrevious_Click(object sender, EventArgs e)
        {
            if ((_previousExtents.Count > 0) && (_mCurrentExtents > 0))
            {
                //for (int i = 0; i < _mapView.Panels[0].Items.Count; i++)
                //{
                //    if (_mapView.Panels[0].Items[i] == _rbZoomToPrevious)
                //    {
                //        _rbZoomToPrevious.Checked = true;
                //    }
                //    else
                //    {
                //        _mapView.Panels[0].Items[i].Checked = false;
                //    }
                //}

                _IsManualExtentsChange = true;
                _mCurrentExtents -= 1;

                mainMap.ViewExtents = _previousExtents[_mCurrentExtents];

                //If m_Extents.Count > 0 And m_CurrentExtent > 0 Then
                //    m_IsManualExtentsChange = True
                //    m_CurrentExtent -= 1
                //    MapWinUtility.Logger.Dbg("In DoZoomPrevious. New CurrentExtent: " + m_CurrentExtent.ToString())
                //    MapMain.Extents = m_Extents(m_CurrentExtent)
                //End If
            }
            else
            {
                _rbZoomToPrevious.Checked = false;
                _rbZoomToPrevious.Enabled = false;
            }
        }

        private void rbZoomToNext_Click(object sender, EventArgs e)
        {
            //If m_CurrentExtent < m_Extents.Count - 1 Then
            //    m_CurrentExtent += 1
            //    m_IsManualExtentsChange = True
            //    MapMain.Extents = m_Extents(m_CurrentExtent)
            //End If
            //UpdateZoomButtons()
            if (_mCurrentExtents < _previousExtents.Count - 1)
            {
                //for (int i = 0; i < _mapView.Panels[0].Items.Count; i++)
                //{
                //    if (_mapView.Panels[0].Items[i] == _rbZoomToNext)
                //    {
                //        _rbZoomToNext.Checked = true;
                //    }
                //    else
                //    {
                //        _mapView.Panels[0].Items[i].Checked = false;
                //    }
                //}

                _mCurrentExtents += 1;
                _IsManualExtentsChange = true;
                mainMap.ViewExtents = _previousExtents[_mCurrentExtents];
            }
            else
            {
                _rbZoomToNext.Checked = false;
                _rbZoomToNext.Enabled = false;
            }
        }

        private void rbIdentifier_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _mapView.Panels[0].Items.Count; i++)
            {
                if (_mapView.Panels[0].Items[i] == _rbIdentifier)
                {
                    _rbIdentifier.Checked = true;
                }
                else
                {
                    _mapView.Panels[0].Items[i].Checked = false;
                }
            }
            //for (int i = 0; i < _mapView.Panels[1].Items.Count; i++)
            //{
            //    //if (_mapView.Panels[0].Items[i] != _rbMaxExtents)
            //    //{
            //    _mapView.Panels[1].Items[i].Checked = false;
            //    //}
            //}

            mainMap.FunctionMode = FunctionMode.Info;
        }

        private void rbAttribute_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < _mapView.Panels[1].Items.Count; i++)
            //{
            //    if (_mapView.Panels[1].Items[i] == _rbAttribute)
            //    {
            //        _rbAttribute.Checked = true;
            //    }
            //    else
            //    {
            //        _mapView.Panels[1].Items[i].Checked = false;
            //    }
            //}
            //for (int i = 0; i < _mapView.Panels[1].Items.Count; i++)
            //{
            //    //if (_mapView.Panels[0].Items[i] != _rbMaxExtents)
            //    //{
            //    _mapView.Panels[1].Items[i].Checked = false;
            //    //}
            //}

            bool featureLayerIsSelected = false;

            foreach (ILayer lay in mainMap.GetAllLayers())
            {
                IMapFeatureLayer ori_fl = lay as IMapFeatureLayer;
                if (ori_fl != null)
                {
                    if (ori_fl.IsSelected == true)
                    {
                        featureLayerIsSelected = true;
                        ori_fl.ShowAttributes();
                    }
                }
            }
            //if no layer is selected, inform the user
            if (featureLayerIsSelected == false)
            {
                MessageBox.Show("Please select a layer in the legend.");
            }
        }

        private void rbMaxExtents_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < _mapView.Panels[0].Items.Count; i++)
            //{
            //    if (_mapView.Panels[0].Items[i] == _rbMaxExtents)
            //    {
            //        _rbMaxExtents.Checked = true;
            //    }
            //    else
            //    {
            //        _mapView.Panels[0].Items[i].Checked = false;
            //    }
            //}
            for (int i = 0; i < _mapView.Panels[0].Items.Count; i++)
            {
                //if (_mapView.Panels[0].Items[i] != _rbMaxExtents)
                //{
                _mapView.Panels[0].Items[i].Checked = false;
                //}
            }
            mainMap.ZoomToMaxExtent();
            mainMap.FunctionMode = FunctionMode.None;

            if (_mCurrentExtents < _previousExtents.Count - 1)
            {
                _rbZoomToNext.Enabled = true;
            }

            if ((_previousExtents.Count > 0) && (_mCurrentExtents > 0))
            {
                _rbZoomToPrevious.Enabled = true;
            }
        }

        private void rbMeasure_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < _mapView.Panels[1].Items.Count; i++)
            //{
            //    if (_mapView.Panels[1].Items[i] == _rbMeasure)
            //    {
            //        _rbMeasure.Checked = true;
            //    }
            //    else
            //    {
            //        _mapView.Panels[1].Items[i].Checked = false;
            //    }
            //}
            for (int i = 0; i < _mapView.Panels[0].Items.Count; i++)
            {
                _mapView.Panels[0].Items[i].Checked = false;
            }

            mainMap.FunctionMode = FunctionMode.Measure;
        }

        #endregion Map Tools Click Events

        private void RemoveTab(string name)
        {
            if (ribbonControl.Tabs.Contains(_mapView))
            {
                ribbonControl.Tabs.Remove(_mapView);
            }
        }

        private void bntMapView_Click(object sender, EventArgs e)
        {
            tabContainer.SelectedTabName = tabContainer.TabPages[0].Text;
        }

        private RibbonTab FindText(string name)
        {
            foreach (RibbonTab tb in this.ribbonControl.Tabs)
            {
                if (tb.Text == name) return tb;
            }
            return null;
        }

        private bool TabContainsText(string name)
        {
            foreach (RibbonTab tb in this.ribbonControl.Tabs)
            {
                if (tb.Text == name) return true;
            }
            return false;
        }

        private void OrbExtentions_MouseMove(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("MouseMove");
            if (OrbExtentions.DropDownItems.Count < 1)
            {
                List<PluginToken> pluginTokens = applicationManager1.PluginTokens;
                foreach (PluginToken token in pluginTokens)
                {
                    RibbonButton item = new RibbonButton();

                    //RibbonDescriptionMenuItem item = new RibbonDescriptionMenuItem();
                    item.Text = token.Name;

                    //added by JK
                    item.MaxSizeMode = RibbonElementSizeMode.DropDown;
                    item.Style = RibbonButtonStyle.DropDownListItem;

                    item.Checked = applicationManager1.IsActive(token);
                    if (item.Text == "Table View")
                    {
                        //item.Image = Properties.Resources.tableView;
                        //item.Description = "Author:" + token.Author + "; " + "PluginType:" + token.PluginType + "; " + "Version:" + token.Version;
                    }
                    OrbExtentions.DropDownItems.Add(item);
                    item.CheckOnClick = true;
                    item.MouseDown += new MouseEventHandler(item_Click);
                }
            }
        }

        private void item_Click(object sender, EventArgs e)
        {
            //to close the 'orb' dropdown
            OrbExtentions.CloseDropDown();
            ribbonControl.OrbDropDown.Close();

            string selectedTokenName = "";
            for (int i = 0; i < this.OrbExtentions.DropDownItems.Count; i++)
            {
                if (this.OrbExtentions.DropDownItems[i].Selected == true)
                {
                    if (this.OrbExtentions.DropDownItems[i].Checked == false)
                    {
                        OrbExtentions.DropDownItems[i].Checked = true;
                    }
                    else
                    {
                        OrbExtentions.DropDownItems[i].Checked = false;
                    }
                    selectedTokenName = this.OrbExtentions.DropDownItems[i].Text;
                }
            }

            List<PluginToken> pluginTokens = applicationManager1.PluginTokens;

            //we need to refresh the ribbon
            RibbonTab newTab = new RibbonTab();

            foreach (PluginToken token in pluginTokens)
            {
                if (selectedTokenName == token.Name)
                {
                    if (applicationManager1.IsActive(token))
                    {
                        applicationManager1.DeactivateToken(token);
                        ribbonControl.Tabs.Remove(newTab);
                        ribbonControl.ActiveTab = tabHome;
                    }
                    else
                    {
                        applicationManager1.ActivateToken(token);
                        ribbonControl.Tabs.Add(newTab);
                    }
                }
            }
        }

        private void EnsureHelpTabLast()
        {
            RibbonTab helpMenuTab = null;
            foreach (RibbonTab tab in ribbonControl.Tabs)
            {
                if (tab.Text == "Help")
                {
                    helpMenuTab = tab;
                }
            }
            if (helpMenuTab != null)
            {
                ribbonControl.Tabs.Remove(helpMenuTab);
                ribbonControl.Tabs.Add(helpMenuTab);
            }
        }

        private void tabHome_ActiveChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        #region Coordinate Display

        private void mainMap_MouseMove(object sender, MouseEventArgs e)
        {
            Coordinate projCor = new Coordinate();
            System.Drawing.Point _mouseLocation = new System.Drawing.Point();
            _mouseLocation.X = e.X;
            _mouseLocation.Y = e.Y;
            projCor = mainMap.PixelToProj(_mouseLocation);

            double[] xy = new double[2];
            xy[0] = projCor.X;
            xy[1] = projCor.Y;

            double[] z = new double[1];
            Reproject.ReprojectPoints(xy, z, _defaultProjection, _wgs84Projection, 0, 1);

            //Convert to Degrees Minutes Seconds
            double[] coord = new double[2];
            coord[0] = Math.Abs(xy[0]);
            coord[1] = Math.Abs(xy[1]);

            double[] d = new double[2];
            double[] m = new double[2];
            double[] s = new double[2];

            d[0] = Math.Floor(coord[0]);
            coord[0] -= d[0];
            coord[0] *= 60;

            m[0] = Math.Floor(coord[0]);
            coord[0] -= m[0];
            coord[0] *= 60;

            s[0] = Math.Floor(coord[0]);

            d[1] = Math.Floor(coord[1]);
            coord[1] -= d[1];
            coord[1] *= 60;

            m[1] = Math.Floor(coord[1]);
            coord[1] -= m[1];
            coord[1] *= 60;

            s[1] = Math.Floor(coord[1]);

            string Long;
            string Lat;

            if (projCor.X > 0) Long = "E";
            else if (projCor.X < 0) Long = "W";
            else Long = " ";

            if (projCor.Y > 0) Lat = "N";
            else if (projCor.Y < 0) Lat = "S";
            else Lat = " ";

            this.mwStatusStrip1.Items[0].Text = "Longitude: " + d[0].ToString() + "°" + m[0].ToString("00") + "'" + s[0].ToString("00") + "\"" + Long + ", Latitude: " + d[1].ToString() + "°" + m[1].ToString("00") + "'" + s[1].ToString("00") + "\"" + Lat;
        }

        #endregion Coordinate Display

        #region Open and Save Project

        private void OrbOpenProject_Click(object sender, EventArgs e)
        {
            this.ribbonControl.OrbDropDown.Close();
            // Ask to save current project
            if (!_projectManager.ProjectIsSaved)
            {
                DialogResult saveProject = MessageBox.Show("Save Changes to current project ?",
                    "Save Changes", MessageBoxButtons.YesNo);
                if (saveProject == DialogResult.Yes)
                {
                    SaveCurrentProject();
                }
            }

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "HydroDesktop Project File|*.dspx";
            fileDialog.Title = "Select the Project File to Open";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                Project.OpenProject(fileDialog.FileName, applicationManager1);
                _projectManager.ProjectIsSaved = true;
                UpdateFormCaption();
            }
            _isNewProject = false;
        }

        private void orbSaveProject_Click(object sender, EventArgs e)
        {
            this.ribbonControl.OrbDropDown.Close();
            SaveCurrentProject();
        }

        private void OrbNewProject_Click(object sender, EventArgs e)
        {
            this.ribbonControl.OrbDropDown.Close();
            //Ask user to save current project
            if (!_projectManager.ProjectIsSaved)
            {
                DialogResult saveProject = MessageBox.Show("Save Changes to current project ?", "Save Changes", MessageBoxButtons.YesNo);
                if (saveProject == DialogResult.Yes)
                {
                    SaveCurrentProject();
                }
            }

            applicationManager1.Map.Layers.Clear();
            //setup new project names
            SetupDatabases();

            Project.CreateNewProject("North America", applicationManager1, (Map)applicationManager1.Map);
            UpdateFormCaption();
        }

        private void orbSaveProjectAs_Click(object sender, EventArgs e)
        {
            this.ribbonControl.OrbDropDown.Close();

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "HydroDesktop Projects|*.dspx";
            fileDialog.Title = "Save Project As";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveProjectAs(fileDialog.FileName);
            }
        }

        private void SaveCurrentProject()
        {
            if (!_isNewProject)
            {
                applicationManager1.SerializationManager.SaveProject(Settings.Instance.CurrentProjectFile);
            }
            else
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Filter = "HydroDesktop Projects|*.dspx";
                fileDialog.Title = "Save Project As";

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    SaveProjectAs(fileDialog.FileName);
                }
            }
        }

        //saves the current HydroDesktop *.hdprj project file to the user specified location with a new database create
        private void SaveProjectAs(string projectFileName)
        {
            Project.SaveProjectAs(projectFileName, applicationManager1);

            UpdateFormCaption();
            _isNewProject = false;
        }

        private void ReportProgress(int percent, string message)
        {
            applicationManager1.ProgressHandler.Progress(message, percent, message);
        }

        #endregion Open and Save Project

        private void _mapView_ActiveChanged(object sender, EventArgs e)
        {
            if (_mapView.Active == true)
            {
                tabContainer.SelectedTab = tabContainer.TabPages[0];
                //mwStatusStrip1.Visible = true;
            }

            string dir = HydroDesktop.Configuration.Settings.Instance.ApplicationDataDirectory;
        }

        //private void rbMapPan_Click(object sender, EventArgs e)
        //{
        //    foreach (RibbonButton rb in this.ribbonControl.Tabs[0].Panels[0].Items)
        //    {
        //        if (rb == rbMapPan)
        //        {
        //            rb.Checked = true;
        //        }
        //        else
        //        {
        //            rb.Checked = false;
        //        }
        //    }

        //    mainMap.FunctionMode = FunctionModes.Pan;
        //}

        //private void rbMapZoomIn_Click(object sender, EventArgs e)
        //{
        //    foreach (RibbonButton rb in this.ribbonControl.Tabs[0].Panels[0].Items)
        //    {
        //        if (rb == rbMapZoomIn)
        //        {
        //            rb.Checked = true;
        //        }
        //        else
        //        {
        //            rb.Checked = false;
        //        }
        //    }

        //    mainMap.FunctionMode = FunctionModes.ZoomIn;
        //}

        //private void rbMapZoomOut_Click(object sender, EventArgs e)
        //{
        //    foreach (RibbonButton rb in this.ribbonControl.Tabs[0].Panels[0].Items)
        //    {
        //        if (rb == rbMapZoomOut)
        //        {
        //            rb.Checked = true;
        //        }
        //        else
        //        {
        //            rb.Checked = false;
        //        }
        //    }

        //    mainMap.FunctionMode = FunctionModes.ZoomOut;
        //}

        //private void rbMapAdd_Click(object sender, EventArgs e)
        //{
        //    foreach (RibbonButton rb in this.ribbonControl.Tabs[0].Panels[0].Items)
        //    {
        //        if (rb == rbMapAdd)
        //        {
        //            rb.Checked = true;
        //        }
        //        else
        //        {
        //            rb.Checked = false;
        //        }
        //    }

        //    if (mainMap == null) return;
        //    mainMap.AddLayer();
        //}

        private void tabSearch_ActiveChanged(object sender, EventArgs e)
        {
            if (tabHome.Active == true && tabContainer.TabPages.Count > 0)
            {
                tabContainer.SelectedTab = tabContainer.TabPages[0];
                //mwStatusStrip1.Visible = true;
            }
        }

        //exit
        private void OrbExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Help
        private void rbHelp_Click(object sender, EventArgs e)
        {
            LocalHelp.OpenHelpFile(_mainHelpFile);
        }

        private void OrbPrint_Click(object sender, EventArgs e)
        {
            //DotSpatial.Controls.LayoutForm layoutFrm = new DotSpatial.Controls.LayoutForm();
            //layoutFrm.MapControl = mainMap;
            //layoutFrm.Show(this);
            //this error ( 8058) can not be traced/reproduced
            //mogikanin code committed by dg on 6/7/2011
            //further/related investigation pending (Dotspatial)
            try
            {
                DotSpatial.Controls.LayoutForm layoutFrm = new DotSpatial.Controls.LayoutForm();
                layoutFrm.MapControl = mainMap;
                layoutFrm.Show(this);
            }
            catch (System.Drawing.Printing.InvalidPrinterException ex)
            {
                MessageBox.Show(ex.Message, "Print error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //mogikanin code ends
        }

        private void OrbAbout_Click(object sender, EventArgs e)
        {
            int x = this.Location.X + this.Width / 2;
            int y = this.Location.Y + this.Height / 2;
            AboutBox frm = new AboutBox(x, y);
            frm.Show();
        }

        private void OrbHelp_Click(object sender, EventArgs e)
        {
            LocalHelp.OpenHelpFile(_mainHelpFile);
        }

        private void OrbApplicationSettings_Click(object sender, EventArgs e)
        {
            int x = this.Location.X + this.Width / 2;
            int y = this.Location.Y + this.Height / 2;
            ConfigurationForm frm = new ConfigurationForm(x, y, applicationManager1);

            this.ribbonControl.OrbDropDown.Close();

            frm.ShowDialog();
        }

        private void mainRibbonForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ribbonControl == null)
            {
                return;
            }

            ribbonControl.OrbDropDown.Close();

            if (_projectManager.ProjectIsSaved)
            {
                ribbonControl.Dispose();
                ribbonControl = null;
                this.Close();
                Application.Exit();
            }

            DialogResult result = MessageBox.Show("Save changes to current project?",
                "Exit",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1);
            // Test the result
            if (result == DialogResult.Yes)
            {
                //save current project if required
                SaveCurrentProject();
            }
            try
            {
                ribbonControl.Dispose();
                ribbonControl = null;
            }
            catch { }
            this.Close();
            Application.Exit();
        }
    }
}