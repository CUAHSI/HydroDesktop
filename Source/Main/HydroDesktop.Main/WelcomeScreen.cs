using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using HydroDesktop.Configuration;
using HydroDesktop.Help;

namespace HydroDesktop.Main
{
    /// <summary>
    /// The welcome screen form shown on program startup
    /// </summary>
    public partial class WelcomeScreen : Form
    {
        #region Private Variables
        
        private List<ProjectFileInfo> _recentProjectFiles;
        private AppManager _app;
        private bool _newProjectCreated = false;

        private string[] _corePlugins = new string[]
        {
            "Search V2",
            "Table View",
            "Graph",
            "Edit",
            "EPA Delineation",
            "Fetch Basemap",
            "Help Tab",
            "Metadata Fetcher",
            "Data Export"
        };

        private Extent _defaultMapExtent = new Extent(-170, -50, 170, 50);
        
        #endregion

        #region Constructor

        public WelcomeScreen(AppManager app)
        {
            InitializeComponent();

            lblProductVersion.Text = "CUAHSI HydroDesktop " + Application.ProductVersion;
            
            _app = app;
            _recentProjectFiles = new List<ProjectFileInfo>();
            bsRecentFiles = new BindingSource(RecentProjectFiles, null);
            lstRecentProjects.DataSource = bsRecentFiles;
            lstRecentProjects.DoubleClick += new EventHandler(lstRecentProjects_DoubleClick);

            lstProjectTemplates.DoubleClick += new EventHandler(lstProjectTemplates_DoubleClick);

            //set the place for progress reporting ...
            //_app.ProgressHandler = spatialStatusStrip1;
            
            //lblCreateNewProject.Click += new EventHandler(lblCreateNewProject_Click);
            //rbNewProject.DoubleClick += new EventHandler(rbNewProject_DoubleClick);
            //lstRecentProjects.DisplayMember = "Name";

            if (lstProjectTemplates.Items.Count > 0)
            {
                lstProjectTemplates.SelectedIndex = 0;
            }
        }

        
       
        #endregion

        #region Properties
        /// <summary>
        /// The list of recent project files
        /// </summary>
        public List<ProjectFileInfo> RecentProjectFiles
        {
            get { return _recentProjectFiles; }
        }

        /// <summary>
        /// Returns true, if a new project was created
        /// using template or using empty project
        /// </summary>
        public bool NewProjectCreated 
        {
            get { return _newProjectCreated; }
        }
        #endregion

        #region Methods

        private void CreateProjectFromTemplate()
        {
            Map mainMap = _app.Map as Map;
            if (mainMap != null)
            {
                if (lstProjectTemplates.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select a project template.");
                    this.DialogResult = DialogResult.None;
                    return;
                }
                else
                {
                    //lblProgress.Text = "Creating new Project.. ";
                    this.Cursor = Cursors.WaitCursor;
                    
                    //panelStatus.Visible = true;
                    Project.CreateNewProject(lstProjectTemplates.SelectedItem.ToString(), _app, mainMap);

                    //lblProgress.Text = "Loading Plugins...";

                    this.Cursor = Cursors.Default;

                    _newProjectCreated = true;

                    this.DialogResult = DialogResult.OK;
                    
                    this.Close();
                }
            }
        }

        /// <summary>
        /// Creates a new empty project
        /// </summary>
        private void CreateEmptyProject()
        {
            //panelStatus.Visible = true;
            Project.CreateEmptyProject(_app.Map);
            //lblProgress.Text = "Loading Plugins...";
            Project.ActivatePlugins(_app, _corePlugins);

            SetDefaultMapExtents();

            this.Cursor = Cursors.Default;

            _newProjectCreated = true;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public void ReportProgress(int percent, string message)
        {
            //if (!progressBar1.Visible) progressBar1.Visible = true;
            //progressBar1.Value = percent;
            //lblProgress.Text = message;
        }

        #endregion

        #region Event Handlers

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rbNewProjectTemplate.Checked)
            {
                CreateProjectFromTemplate();
            }
            else if (rbEmptyProject.Checked)
            {
                CreateEmptyProject();
            }
            else
            {
                OpenProject();
            }
            //panelStatus.Visible = true;
            //_app.ProgressHandler.
        }

        private void WelcomeScreen_Load(object sender, EventArgs e)
        {
            FindRecentProjectFiles();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            FeatureSet fs = new FeatureSet();
            //fs.Features[0].Coordinates[0].X
        }

        private void btnBrowseProject_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "HydroDesktop Project File|*.dspx";
            fileDialog.Title = "Select the Project File to Open";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenExistingProject(fileDialog.FileName);
            }
        }

        void lstRecentProjects_DoubleClick(object sender, EventArgs e)
        {
            OpenProject();
        }

        void lstProjectTemplates_DoubleClick(object sender, EventArgs e)
        {
            CreateProjectFromTemplate();
        }

        #endregion

        #region Methods

        private void OpenProject()
        {
            ProjectFileInfo selected = lstRecentProjects.SelectedValue as ProjectFileInfo;
            if (selected != null)
            {
                //panelStatus.Visible = true;
                OpenExistingProject(selected.FullPath);
            }
        }

        private void OpenExistingProject(string projectFileName)
        {
            //lblProgress.Text = "Opening Project " + Path.GetFileNameWithoutExtension(projectFileName).ToString() + "...";
            this.Cursor = Cursors.WaitCursor;
            //Project.ActivatePlugins(_app, _corePlugins);
            Project.OpenProject(projectFileName, _app);

            this.Cursor = Cursors.Default;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SetDefaultMapExtents()
        {
            double[] xy = new double[4];
            xy[0] = _defaultMapExtent.MinX;
            xy[1] = _defaultMapExtent.MinY;
            xy[2] = _defaultMapExtent.MaxX;
            xy[3] = _defaultMapExtent.MaxY;
            double[] z = new double[] { 0, 0 };
            ProjectionInfo wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;
            Reproject.ReprojectPoints(xy, z, wgs84, _app.Map.Projection, 0, 2);

            _app.Map.ViewExtents = new Extent(xy);
        }
        
        private void FindRecentProjectFiles()
        {
            this.RecentProjectFiles.Clear();
            
            foreach(string recentFile in Settings.Instance.RecentProjectFiles)
            {
                if (File.Exists(recentFile))
                {
                    this.RecentProjectFiles.Add(new ProjectFileInfo(recentFile));
                }

            }
            bsRecentFiles.ResetBindings(false);
            lstRecentProjects.SelectedIndex = -1;
        }
        #endregion

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void linkLabelHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LocalHelp.OpenHelpFile("welcome.html");
        }

        
    } 

    public class ProjectFileInfo
    {
        public ProjectFileInfo(string fullPath)
        {
            FullPath = fullPath;
        }

        public string FullPath { get; private set; }
        public string Name 
        {
            get
            {
                return Path.GetFileNameWithoutExtension(FullPath);

            }
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
