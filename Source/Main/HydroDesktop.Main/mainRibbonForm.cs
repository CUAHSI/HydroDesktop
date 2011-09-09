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
using DotSpatial.Controls.Header;
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
using System.ComponentModel.Composition;

namespace HydroDesktop.Main
{
    public partial class mainRibbonForm : Form
    {
        #region Variable

        private const string _mainHelpFile = "welcome.html";
        private string kHomeRoot = HeaderControl.HomeRootItemKey;

        //the map tools toggle group key
        private const string kHydroMapTools = "kHydroMapTools"; 

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
        //private RootItem _mapView;
        private SimpleActionItem _rbAdd;
        private SimpleActionItem _rbPan;
        private SimpleActionItem _rbSelect;
        private SimpleActionItem _rbZoomIn;
        private SimpleActionItem _rbZoomOut;
        private SimpleActionItem _rbIdentifier;
        private SimpleActionItem _rbAttribute;
        private SimpleActionItem _rbMaxExtents;
        private SimpleActionItem _rbMeasure;
        private SimpleActionItem _rbZoomToPrevious;
        private SimpleActionItem _rbZoomToNext;

        //project file related indicators
        private bool _isNewProject = false;
        private WelcomeScreen _welcomeScreen = null;
        private string _projectFileName = null;
        private ProjectChangeTracker _projectManager = null;

        //parent container for docking
        [Export("Shell", typeof(ContainerControl))]
        public static ContainerControl Shell;

        #endregion Variable

        #region Constructor

        /// <summary>
        /// Initialize MapView
        /// </summary>
        /// <param name="args">The file name to open when HydroDesktop starts</param>
        public mainRibbonForm(string[] args)
        {
            InitializeComponent();

            //set the Shell Container control static variable
            Shell = this;

            //screen size
            AdjustFormSize();

            //when the form is shown (occurs after closing welcome screen)
            this.Shown += new EventHandler(mainRibbonForm_Shown);

            mainMap.MapFrame.ViewExtentsChanged += new EventHandler<ExtentArgs>(MapFrame_ExtentsChanged);
            this.SizeChanged += new EventHandler(mainRibbonForm_SizeChanged);
            this.FormClosing += new FormClosingEventHandler(mainRibbonForm_FormClosing);

            // setup the header control
            //this.applicationManager1.HeaderControl = new DotSpatial.Controls.RibbonControls.RibbonHeaderControl(this.ribbonControl);

            #region initialize the help menu

            ////setup the help quick access button
            //rbHelp.Image = Properties.Resources.help;
            //rbHelp.SmallImage = Properties.Resources.help_16x16;

            #endregion initialize the help menu
            
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

            #region initialize default database connection strings

            SetupDatabases();

            #endregion initialize default database connection strings

            #region Initialize the Project opening events

            applicationManager1.SerializationManager.Deserializing += new EventHandler<SerializingEventArgs>(SerializationManager_Deserializing);
            applicationManager1.SerializationManager.Serializing += new EventHandler<SerializingEventArgs>(SerializationManager_Serializing);

            #endregion Initialize the Project opening events

            #region Load Plugins

            //initialize the home ribbon tab
            applicationManager1.ShowExtensionsDialog = ShowExtensionsDialog.Default;
            applicationManager1.Initialize();

             

            //initialize  the menu bar
            AddRibbonButtons();
            AddApplicationMenu();

            //add the legend and map dock panels
            mainMap.Dock = DockStyle.Fill;
            mainLegend.Dock = DockStyle.Fill;
            applicationManager1.DockManager.Add(new DotSpatial.Controls.Docking.DockablePanel(kHomeRoot, "Map", mainMap, DockStyle.Fill));
            applicationManager1.DockManager.Add(new DotSpatial.Controls.Docking.DockablePanel("kHydroLegend", "Legend", mainLegend, DockStyle.Left));
            
            //activate remaining extensions
            applicationManager1.LoadExtensions(); 

            //to reset the original dock layout
            applicationManager1.DockManager.ResetLayout();
            

            //map activated event
            applicationManager1.DockManager.ActivePanelChanged += new EventHandler<DotSpatial.Controls.Docking.ActivePanelChangedEventArgs>(DockManager_ActivePanelChanged);

            applicationManager1.DockManager.SelectPanel(kHomeRoot);

            #endregion

            //project change tracking
            _projectManager = new ProjectChangeTracker(applicationManager1);
            _projectManager.ProjectModified += new EventHandler(_projectManager_ProjectModified);
        }

        #region Method

        #region initialize the MapView Ribbon TabPage and related controls

        private void AddApplicationMenu()
        {
            var header = applicationManager1.HeaderControl;

            string rootCaption = "BackStage";
            string FileMenuKey = "kFile";
            header.Add(new RootItem(FileMenuKey, rootCaption));

            // NewProject
            header.Add(new SimpleActionItem(FileMenuKey, "New Project", OrbNewProject_Click) 
            { 
                GroupCaption = rootCaption, 
                SmallImage = Properties.Resources.new_file, 
                LargeImage = Properties.Resources.new_file 
            });
            // Open Project
            header.Add(new SimpleActionItem(FileMenuKey, "Open Project", OrbOpenProject_Click) 
            { 
                GroupCaption = rootCaption, 
                SmallImage = Properties.Resources.new_file, 
                LargeImage = Properties.Resources.new_file 
            });
            // Save Project
            header.Add(new SimpleActionItem(FileMenuKey, "Save Project", orbSaveProject_Click) 
            { 
                GroupCaption = rootCaption, 
                SmallImage = Properties.Resources.save_project_16, 
                LargeImage = Properties.Resources.save_project 
            });
            // Save Project As
            header.Add(new SimpleActionItem(FileMenuKey, "Save Project As", orbSaveProjectAs_Click) 
            { 
                GroupCaption = rootCaption, 
                SmallImage = Properties.Resources.save_project_as_16, 
                LargeImage = Properties.Resources.save_project_as 
            });
            // Print
            header.Add(new SimpleActionItem(FileMenuKey, "Print", OrbPrint_Click)
            {
                GroupCaption = rootCaption,
                SmallImage = Properties.Resources.print_16,
                LargeImage = Properties.Resources.print_32
            });
            // Separator 1
            header.Add(new SeparatorItem(FileMenuKey, rootCaption));

            // Application Settings
            header.Add(new SimpleActionItem(FileMenuKey, "Application Settings", OrbApplicationSettings_Click)
            {
                GroupCaption = rootCaption,
                SmallImage = Properties.Resources.settings_16,
                LargeImage = Properties.Resources.settings
            });
            // Separator 2
            header.Add(new SeparatorItem(FileMenuKey, rootCaption));
            // Help
            header.Add(new SimpleActionItem(FileMenuKey, "Help", OrbHelp_Click)
            {
                GroupCaption = rootCaption,
                SmallImage = Properties.Resources.help_16,
                LargeImage = Properties.Resources.help_32
            });
            // About
            header.Add(new SimpleActionItem(FileMenuKey, "About", OrbAbout_Click)
            {
                GroupCaption = rootCaption,
                SmallImage = Properties.Resources.about_16,
                LargeImage = Properties.Resources.about
            });
            // Separator 3
            header.Add(new SeparatorItem(FileMenuKey, rootCaption));
            //Exit
            header.Add(new SimpleActionItem(FileMenuKey, "Exit", OrbExit_Click)
            {
                GroupCaption = rootCaption,
                SmallImage = Properties.Resources.exit_16,
                LargeImage = Properties.Resources.exit
            });
        }
        
        private void AddRibbonButtons()
        {
            //TODO make this localizable
            
            var homeRoot = new RootItem(kHomeRoot, "Home") { SortOrder = -200 };
            applicationManager1.HeaderControl.Add(homeRoot);

            //add the empty "table" tab
            applicationManager1.HeaderControl.Add(new RootItem("kHydroTable", "Table"));

            string rpMapTools = "Map Tools";

            //Pan
            _rbPan = new SimpleActionItem(kHomeRoot, "Pan", rbPan_Click);
            _rbPan.GroupCaption = rpMapTools;
            _rbPan.LargeImage = Properties.Resources.pan;
            _rbPan.SmallImage = Properties.Resources.pan_16;
            _rbPan.ToolTipText = "Pan - Move the Map";
            _rbPan.ToggleGroupKey = kHydroMapTools;
            applicationManager1.HeaderControl.Add(_rbPan);

            //ZoomIn
            _rbZoomIn = new SimpleActionItem(kHomeRoot, "Zoom In", rbZoomIn_Click);
            _rbZoomIn.ToolTipText = "Zoom In";
            _rbZoomIn.GroupCaption = rpMapTools;
            _rbZoomIn.LargeImage = Properties.Resources.zoom_in;
            _rbZoomIn.SmallImage = Properties.Resources.zoom_in_16;
            _rbZoomIn.ToggleGroupKey = kHydroMapTools;
            applicationManager1.HeaderControl.Add(_rbZoomIn);

            //ZoomOut
            _rbZoomOut = new SimpleActionItem(kHomeRoot, "Zoom Out", rbZoomOut_Click);
            _rbZoomOut.ToolTipText = "Zoom Out";
            _rbZoomOut.GroupCaption = rpMapTools;
            _rbZoomOut.LargeImage = Properties.Resources.zoom_out;
            _rbZoomOut.SmallImage = Properties.Resources.zoom_out_16;
            _rbZoomOut.ToggleGroupKey = kHydroMapTools;
            applicationManager1.HeaderControl.Add(_rbZoomOut);

            //ZoomToFullExtent
            _rbMaxExtents = new SimpleActionItem(kHomeRoot, "MaxExtents", rbMaxExtents_Click);
            _rbMaxExtents.ToolTipText = "Maximum Extents";
            _rbMaxExtents.GroupCaption = rpMapTools;
            _rbMaxExtents.LargeImage = Properties.Resources.full_extent;
            _rbMaxExtents.SmallImage = Properties.Resources.full_extent_16;
            applicationManager1.HeaderControl.Add(_rbMaxExtents);

            //ZoomToPrevious
            _rbZoomToPrevious = new SimpleActionItem(kHomeRoot, "Previous", rbZoomToPrevious_Click);
            _rbZoomToPrevious.ToolTipText = "Go To Previous Map Extent";
            _rbZoomToPrevious.GroupCaption = rpMapTools;
            _rbZoomToPrevious.LargeImage = Properties.Resources.zoom_to_previous;
            _rbZoomToPrevious.SmallImage = Properties.Resources.full_extent_16;
            applicationManager1.HeaderControl.Add(_rbZoomToPrevious);

            if (_previousExtents.Count == 0)
                _rbZoomToPrevious.Enabled = false;

            //ZoomToNext
            _rbZoomToNext = new SimpleActionItem(kHomeRoot, "Next", rbZoomToNext_Click);
            _rbZoomToNext.ToolTipText = "Go To Next Map Extent";
            _rbZoomToNext.GroupCaption = rpMapTools;
            _rbZoomToNext.LargeImage = Properties.Resources.zoom_to_next;
            _rbZoomToNext.SmallImage = Properties.Resources.zoom_to_next_16;
            applicationManager1.HeaderControl.Add(_rbZoomToNext);

            if ((_mCurrentExtents < _previousExtents.Count - 1) != true)
                _rbZoomToNext.Enabled = false;

            _rbZoomToNext.Click += new EventHandler(rbZoomToNext_Click);

            //Separator
            var mapTools = new SeparatorItem();
            mapTools.GroupCaption = rpMapTools;
            mapTools.RootKey = kHomeRoot;
            applicationManager1.HeaderControl.Add(mapTools);

            //Add
            _rbAdd = new SimpleActionItem(kHomeRoot, "Add", rbAdd_Click);
            _rbAdd.ToolTipText = "Add Layer To The Map";
            _rbAdd.GroupCaption = rpMapTools;
            _rbAdd.LargeImage = Properties.Resources.add;
            _rbAdd.SmallImage = Properties.Resources.add_16;
            applicationManager1.HeaderControl.Add(_rbAdd);

            //Identifier
            _rbIdentifier = new SimpleActionItem(kHomeRoot, "Identify", rbIdentifier_Click);
            _rbIdentifier.ToolTipText = "Identify";
            _rbIdentifier.SmallImage = Properties.Resources.identifier_16;
            _rbIdentifier.GroupCaption = rpMapTools;
            _rbIdentifier.LargeImage = Properties.Resources.identifier;
            _rbIdentifier.ToggleGroupKey = kHydroMapTools;
            applicationManager1.HeaderControl.Add(_rbIdentifier);

            //Select
            _rbSelect = new SimpleActionItem(kHomeRoot, "Select", rbSelect_Click);
            _rbSelect.ToolTipText = "Select";
            _rbSelect.GroupCaption = rpMapTools;
            _rbSelect.LargeImage = Properties.Resources.select;
            _rbSelect.SmallImage = Properties.Resources.select_16;
            _rbSelect.ToggleGroupKey = kHydroMapTools;
            applicationManager1.HeaderControl.Add(_rbSelect);

            //AttributeTable
            _rbAttribute = new SimpleActionItem(kHomeRoot, "Attribute", rbAttribute_Click);
            _rbAttribute.ToolTipText = "Attribute Table";
            _rbAttribute.GroupCaption = rpMapTools;
            _rbAttribute.LargeImage = Properties.Resources.attribute_table;
            _rbAttribute.SmallImage = Properties.Resources.attribute_table_16;
            applicationManager1.HeaderControl.Add(_rbAttribute);

            //Measure
            _rbMeasure = new SimpleActionItem(kHomeRoot, "Measure", rbMeasure_Click);
            _rbMeasure.ToolTipText = "Measure Distance or Area";
            _rbMeasure.GroupCaption = rpMapTools;
            _rbMeasure.LargeImage = Properties.Resources.measure;
            _rbMeasure.SmallImage = Properties.Resources.measure_16;
            _rbMeasure.ToggleGroupKey = kHydroMapTools;
            applicationManager1.HeaderControl.Add(_rbMeasure);

            //View
            var rbRestore = new SimpleActionItem(kHomeRoot, "Restore Layout", this.rbRestore_Click);
            rbRestore.ToolTipText = "Restore Layout";
            rbRestore.GroupCaption = "View";
            var separat = new SeparatorItem();
            applicationManager1.HeaderControl.Add(rbRestore);
        }
        #endregion initialize the MapView Ribbon TabPage and related controls


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

            //resetting of the map (necessary with reprojection)
            RefreshTheLayers();
            mainMap.ResetBuffer();
            mainMap.MapFrame.ResetExtents();

            //Set the correct SQLite databases file path
            string dbFile = Path.ChangeExtension(Settings.Instance.CurrentProjectFile, ".sqlite");
            if (SQLiteHelper.DatabaseExists(dbFile))
            {
                Settings.Instance.DataRepositoryConnectionString = SQLiteHelper.GetSQLiteConnectionString(dbFile);
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
            //Set Initial Map Projection
            mainMap.Projection = _defaultProjection;
        }

        /// <summary>
        /// When the main form is first shown, display welcome screen
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
                Project.OpenProject(_projectFileName, applicationManager1);
            }

            //applicationManager1.DockManager.SelectPanel(kHomeRoot);
        }

        // when the map dock panel is activated:
        // activate legend and select home ribbon tab
        void DockManager_ActivePanelChanged(object sender, DotSpatial.Controls.Docking.ActivePanelChangedEventArgs e)
        {
            if (e.ActivePanelKey == kHomeRoot)
            {
                applicationManager1.HeaderControl.SelectRoot(kHomeRoot);
                applicationManager1.DockManager.SelectPanel("kHydroLegend");
                
                applicationManager1.DockManager.ActivePanelChanged -= DockManager_ActivePanelChanged;
                applicationManager1.DockManager.SelectPanel(e.ActivePanelKey);
                applicationManager1.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
            }
        }

        #endregion Event

        #endregion Constructor

        //show the welcome screen
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
            //TODO: refresh the 'search results' shapefile instead (only if DB themes are different)
            //ThemeManager manager = new ThemeManager(Settings.Instance.DataRepositoryConnectionString);
            //manager.RefreshAllThemes(mainMap);
        }

        #region Map Tools Click Events

        private void rbAdd_Click(object sender, EventArgs e)
        {
            //add a layer to the map
            if (mainMap == null) return;

            mainMap.AddLayers();
        }

        private void rbPan_Click(object sender, EventArgs e)
        {
            mainMap.FunctionMode = FunctionMode.Pan;
        }

        private void rbSelect_Click(object sender, EventArgs e)
        {
            mainMap.FunctionMode = FunctionMode.Select;
        }

        private void rbZoomIn_Click(object sender, EventArgs e)
        {
            mainMap.FunctionMode = FunctionMode.ZoomIn;
        }

        private void rbZoomOut_Click(object sender, EventArgs e)
        {
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
                _rbZoomToNext.Enabled = false;
            }
        }

        private void rbIdentifier_Click(object sender, EventArgs e)
        {
            mainMap.FunctionMode = FunctionMode.Info;
        }

        private void rbAttribute_Click(object sender, EventArgs e)
        {
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
            mainMap.FunctionMode = FunctionMode.Measure;
        }

        private void rbRestore_Click(object sender, EventArgs e)
        {
            applicationManager1.DockManager.ResetLayout();
        }


        #endregion Map Tools Click Events


        #region Extensions Menu
        private void OrbExtensions_Click(object sender, EventArgs e)
        {
            //TODO fix AppDialog
            //var dialog = new AppDialog(applicationManager1);
            //dialog.Show();
        }
        
        #endregion

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
            SaveCurrentProject();
        }

        private void OrbNewProject_Click(object sender, EventArgs e)
        {
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

            SetupDatabases();

            Project.CreateNewProject("North America", applicationManager1, (Map)applicationManager1.Map);
            UpdateFormCaption();
        }

        private void orbSaveProjectAs_Click(object sender, EventArgs e)
        {
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

        //saves the current HydroDesktop *.dspx project file to the user specified location with a new database create
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

            frm.ShowDialog();
        }

        private void mainRibbonForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //TODO re-implement save project warning
            
            //if (_projectManager.ProjectIsSaved)
            //{
            //    this.Close();
            //    Application.Exit();
            //}

            //DialogResult result = MessageBox.Show("Save changes to current project?",
            //    "Exit",
            //    MessageBoxButtons.YesNo,
            //    MessageBoxIcon.Question,
            //    MessageBoxDefaultButton.Button1);
            //// Test the result
            //if (result == DialogResult.Yes)
            //{
            //    //save current project if required
            //    SaveCurrentProject();
            //}

            //this.Close();
            //Application.Exit();
        }
    }
}