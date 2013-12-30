using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Extensions;
using DotSpatial.Projections;
using HydroDesktop.Common;
using HydroDesktop.Configuration;
using HydroDesktop.Help;
using System.Threading.Tasks;
using NuGet;
using System.Drawing;
using System.Linq;
using DotSpatial.Plugins.ExtensionManager;

namespace HydroDesktop.Main
{
    /// <summary>
    /// The welcome screen form shown on program startup
    /// </summary>
    public partial class WelcomeScreen : Form
    {
        /// <summary>
        /// Gets the list tools available.
        /// </summary>
        public IEnumerable<ISampleProject> SampleProjects { get; set; }
        
        #region Private Variables
        
        private List<ProjectFileInfo> _recentProjectFiles;
        private AppManager _app;
        private bool _newProjectCreated = false;
        private readonly string _localHelpUri = Properties.Settings.Default.localHelpUri;
        private readonly string _remoteHelpUri = Properties.Settings.Default.remoteHelpUri;
        private readonly string _quickStartUri = Properties.Settings.Default.quickStartUri;

        private readonly Packages packages = new Packages();
        private readonly DownloadForm downloadDialog = new DownloadForm();

        private Extent _defaultMapExtent = new Extent(-170, -50, 170, 50);

        private ProjectManager myProjectManager;
        

        
        #endregion

        #region Constructor

        public WelcomeScreen(ProjectManager projManager)
        {
            InitializeComponent();
            myProjectManager = projManager;
            lblProductVersion.Text = "CUAHSI HydroDesktop " + AppContext.Instance.ProductVersion;
            
            _app = projManager.App;
            _recentProjectFiles = new List<ProjectFileInfo>();
            bsRecentFiles = new BindingSource(RecentProjectFiles, null);
            lstRecentProjects.DataSource = bsRecentFiles;

            lstRecentProjects.DoubleClick += lstRecentProjects_DoubleClick;
            lstProjectTemplates.DoubleClick += lstProjectTemplates_DoubleClick;
            FormClosing += WelcomeScreen_FormClosing;

            if (lstProjectTemplates.Items.Count > 0)
            {
                lstProjectTemplates.SelectedIndex = 0;
            }
            uxFeedSelection.SelectedIndexChanged += uxFeedSelection_SelectedIndexChanged;
            this.uxOnlineProjects.SelectedIndexChanged += new EventHandler(this.uxOnlineProjects_SelectedIndexChanged);
            this.uxFeedSelection.SelectedIndex = 0;
            downloadDialog.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
                    DialogResult = DialogResult.None;
                    return;
                }
                var selectedTemplate = lstProjectTemplates.SelectedItem as ISampleProject;
                string projectFile = selectedTemplate.AbsolutePathToProjectFile;

                try
                {
                    string newProjectFile = CopyToDocumentsFolder(projectFile);
                    _app.SerializationManager.OpenProject(newProjectFile);
                   if((newProjectFile.Contains("North America Map")
                      || newProjectFile.Contains("World Map")) 
                      && WebUtilities.IsInternetAvailable() == true)
                    {
                        myProjectManager.ProjectToGeoLocation();
                    } 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + @" File: " + projectFile);
                }
                    
                    
                lblProgress.Text = "Creating new Project.. ";
                this.Cursor = Cursors.WaitCursor;
                    
                panelStatus.Visible = true;

                _newProjectCreated = true;

                this.DialogResult = DialogResult.OK;
                    
               
                this.Close();
            }
        }

        private string CopyToDocumentsFolder(string projectFile)
        {
            string projDir = Path.GetDirectoryName(projectFile);
            string docsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string dotSpatialDir = Path.Combine(docsDir, "DotSpatial");
            if (!Directory.Exists(dotSpatialDir))
            {
                //todo check if the directory can be created
                Directory.CreateDirectory(dotSpatialDir);
            }

            string projName = Path.GetFileNameWithoutExtension(projectFile);
            string newProjDir = Path.Combine(dotSpatialDir, projName);
            if (!Directory.Exists(newProjDir))
            {
                Directory.CreateDirectory(newProjDir);
            }

            foreach (string file in Directory.GetFiles(projDir))
            {
                File.Copy(file, Path.Combine(newProjDir, Path.GetFileName(file)), true);
            }
            string newProjFile = Path.Combine(newProjDir, Path.GetFileName(projectFile));
            return newProjFile;
        }

        /// <summary>
        /// Creates a new empty project
        /// </summary>
        private void CreateEmptyProject()
        {
            panelStatus.Visible = true;
            myProjectManager.CreateEmptyProject();
            

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

        private void OpenUri(string uriString)
        {
            if (WebUtilities.IsInternetAvailable() == false)
            {
                MessageBox.Show("Internet connection not available.", "Could not open URI", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                WebUtilities.OpenUri(uriString);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("No URI provided.", "Could not open URI", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UriFormatException ex)
            {
                MessageBox.Show("Invalid URI format for '" + uriString + "'.\n(" + ex.Message + ")", "Could not open URI", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                if (ex.Message == "The system cannot find the path specified")
                {
                    MessageBox.Show("Could not find the target at '" + uriString + "'.", "Could not open URI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Could not open target at '" + uriString + "'.\n(" + ex.Message + ")", "Could not open URI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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
            panelStatus.Visible = true;
            //_app.ProgressHandler.
        }

        private void WelcomeScreen_Load(object sender, EventArgs e)
        {
            SampleProjectInstaller spi = new SampleProjectInstaller();
            List<SampleProjectInfo> sampleProjects1 = spi.FindSampleProjectFiles();
       
            IEnumerable<ISampleProject> sampleProjects2 = spi.SetupInstalledSampleProjects(sampleProjects1);

            
            SampleProjects = sampleProjects2;
            FindRecentProjectFiles();

            IEnumerable<ISampleProject> sampleProjects3 = new List<ISampleProject>();
            ((List<ISampleProject>)sampleProjects3).AddRange(SampleProjects);
            ((List<ISampleProject>)sampleProjects3).AddRange(RecentProjectFiles);
            SampleProjects = sampleProjects3;

            lstProjectTemplates.DataSource = SampleProjects;
            lstProjectTemplates.DisplayMember = "Name";

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

        private void WelcomeScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Create an empty project if the x button is clicked
            if (this.DialogResult != DialogResult.OK)
            {
                if (String.IsNullOrEmpty(_app.SerializationManager.CurrentProjectFile))
                {
                    e.Cancel = true;
                    CreateEmptyProject();
                }
            }
        }

        #endregion

        #region Methods

        private void OpenProject()
        {
            ProjectFileInfo selected = lstRecentProjects.SelectedValue as ProjectFileInfo;
            if (selected != null)
            {
                panelStatus.Visible = true;
                OpenExistingProject(selected.FullPath);
            }
        }

        private void OpenExistingProject(string projectFileName)
        {
            lblProgress.Text = "Opening Project " + Path.GetFileNameWithoutExtension(projectFileName) + "...";
            this.Cursor = Cursors.WaitCursor;

            myProjectManager.OpenProject(projectFileName);

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

            List<string> existingRecentFiles = new List<string>();
                
            foreach (string recentFile in HydroDesktop.Configuration.Settings.Instance.RecentProjectFiles)
            {              
                if (File.Exists(recentFile))
                {
                    if (!existingRecentFiles.Contains(recentFile)) //add to list only if not exists
                    {
                        existingRecentFiles.Add(recentFile);
                    }
                }
            }

            HydroDesktop.Configuration.Settings.Instance.RecentProjectFiles.Clear();
            foreach (string recentFile in existingRecentFiles)
            {
                HydroDesktop.Configuration.Settings.Instance.RecentProjectFiles.Add(recentFile);
                RecentProjectFiles.Add(new ProjectFileInfo(recentFile));
            }

            //also adds the installed 'sample projects' to the directory
            //SetupSampleProjects();

            bsRecentFiles.ResetBindings(false);
            lstRecentProjects.SelectedIndex = -1;
        }

        private void UpdateInstalledProjectsList()
        {

            this.SampleProjects = this.FindSampleProjectFiles();
            if (!NuGet.EnumerableExtensions.Any<SampleProjectInfo>(((List<SampleProjectInfo>)this.SampleProjects)))
            {
                if (RecentProjectFiles.Count == 0)
                {
                    this.lstProjectTemplates.DataSource = null;
                    this.lstProjectTemplates.Items.Add("No projects were found. Please install the online templates.");
                    return;
                }
            }
            SampleProjectInstaller spi = new SampleProjectInstaller();
            List<SampleProjectInfo> sampleProjects1 = spi.FindSampleProjectFiles();

            IEnumerable<ISampleProject> sampleProjects2 = spi.SetupInstalledSampleProjects(sampleProjects1);
            SampleProjects = sampleProjects2;

            FindRecentProjectFiles();
            IEnumerable<ISampleProject> sampleProjects3 = new List<ISampleProject>();
            ((List<ISampleProject>)sampleProjects3).AddRange(SampleProjects);
            ((List<ISampleProject>)sampleProjects3).AddRange(RecentProjectFiles);
            SampleProjects = sampleProjects3;

            this.lstProjectTemplates.DataSource = this.SampleProjects;
            this.lstProjectTemplates.DisplayMember = "Name";
            this.uxOnlineProjects.SelectedIndex = 0;
            this.btnInstall.Enabled = true;
        }

        private IEnumerable<SampleProjectInfo> FindSampleProjectFiles()
        {
            List<SampleProjectInfo> list = new List<SampleProjectInfo>();
            if (Directory.Exists(AppManager.AbsolutePathToExtensions))
            {
                foreach (string current in Directory.EnumerateFiles(AppManager.AbsolutePathToExtensions, "*.dspx", SearchOption.AllDirectories))
                {
                    list.Add(new SampleProjectInfo
                    {
                        AbsolutePathToProjectFile = current,
                        Name = Path.GetFileNameWithoutExtension(current),
                        Description = "description",
                        Version = "1.0"
                    });
                }
            }
            return list;
        }

        private void UpdatePackageList()
        {
            this.uxOnlineProjects.Items.Add("Loading...");
            Task<IPackage[]> task = Task.Factory.StartNew<IPackage[]>(delegate
            {
                return (
                    from p in this.packages.Repo.GetPackages()
                    where p.IsLatestVersion && (p.Tags.Contains("DotSpatialSampleProject") || p.Tags.Contains("SampleProject"))
                    select p).ToArray<IPackage>();
            });
            task.ContinueWith(delegate(Task<IPackage[]> t)
            {
                this.uxOnlineProjects.Items.Clear();
                if (t.Exception == null)
                {
                    this.uxOnlineProjects.Items.AddRange(t.Result);
                    return;
                }
                this.uxOnlineProjects.Items.Add(t.Exception.Message);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void UninstallSampleProject(SampleProjectInfo sample)
        {
            if (this._app.SerializationManager.CurrentProjectFile == sample.AbsolutePathToProjectFile)
            {
                MessageBox.Show("Cannot uninstall " + sample.Name + ". The project is currently open. Please close current project and try again.");
                return;
            }
            string directoryName = Path.GetDirectoryName(sample.AbsolutePathToProjectFile);
            DirectoryInfo parent = Directory.GetParent(directoryName);
            try
            {
                foreach (string current in Directory.EnumerateFiles(directoryName))
                {
                    File.Delete(current);
                }
                Directory.Delete(directoryName);
                FileInfo[] files = parent.GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo fileInfo = files[i];
                    fileInfo.Delete();
                }
                if (!NuGet.EnumerableExtensions.Any<DirectoryInfo>(parent.GetDirectories()) && !NuGet.EnumerableExtensions.Any<FileInfo>(parent.GetFiles()))
                {
                    parent.Delete();
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("Some files could not be uninstalled. " + ex.Message);
            }
            MessageBox.Show("The project was successfully uninstalled.");
        }
        #endregion

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            SampleProjectInfo sample = findTemplate(this.uxOnlineProjects.SelectedItem as IPackage);
           
            this.UninstallSampleProject(sample);
            this.UpdateInstalledProjectsList();

            // This is used to refresh the view
            int temp = uxOnlineProjects.SelectedIndex;
            this.uxOnlineProjects.SelectedIndex += 1;
            this.uxOnlineProjects.SelectedIndex = temp;
            
        }

        private SampleProjectInfo findTemplate(IPackage package)
        {
            string packagePath = GetPackagePath(package);
            List<SampleProjectInfo> projectTemplates = this.FindSampleProjectFiles() as List<SampleProjectInfo>; 
            foreach (SampleProjectInfo p in projectTemplates)
            {
                if(p.AbsolutePathToProjectFile.Contains(packagePath)) 
                {
                    return p;
                }
            }
            
            return null;
        }

        private void uxFeedSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            string feedUrl;
            if (uxFeedSelection.SelectedIndex == 1)
                feedUrl = "https://nuget.org/api/v2/";
            else
                feedUrl = "https://www.myget.org/F/cuahsi/";

            packages.SetNewSource(feedUrl);
            this.UpdatePackageList();
        }

        private void lstRecentProjects_Click(object sender, EventArgs e)
        {
            rbOpenExistingProject.Checked = true;
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            try
            {

                if (WebUtilities.IsInternetAvailable() == false)
                {
                    LocalHelp.OpenHelpFile(_localHelpUri);
                }
                else
                {
                    OpenUri(_remoteHelpUri);    
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open help file at " + _localHelpUri + "\n" + ex.Message, "Could not open help", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        
        }

        private void QuickStartButton_Click(object sender, EventArgs e)
        {
            try
            {

                if (WebUtilities.IsInternetAvailable() == false)
                {
                    string quickStartGuideFile = Properties.Settings.Default.QuickStartGuideName;
                    LocalHelp.OpenHelpFile(quickStartGuideFile);
                }
                else
                {
                    OpenUri(_quickStartUri);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open help file at " + _localHelpUri + "\n" + ex.Message, "Could not open help", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }         
        }
        private void btnOKOnline_Click(object sender, EventArgs e)
        {
            if (this.uxOnlineProjects.SelectedItem != null)
            {
                this.btnInstall.Enabled = false;
                downloadDialog.Show();
                IPackage pack = this.uxOnlineProjects.SelectedItem as IPackage;
                
                var inactiveExtensions = _app.Extensions.Where(a => a.IsActive == false).ToArray();

                Task task = Task.Factory.StartNew(delegate
                {
                    IEnumerable<PackageDependency> dependency = pack.Dependencies;
                    if (dependency.Count() > 0)
                    {
                        foreach (PackageDependency dependentPackage in dependency)
                        {
                            _app.ProgressHandler.Progress(null, 0, "Downloading Dependency " + dependentPackage.Id);
                            downloadDialog.ShowDownloadStatus(dependentPackage);
                            downloadDialog.SetProgressBarPercent(0);

                            var dependentpack = packages.Install(dependentPackage.Id);
                            if (dependentpack == null)
                            {
                                string message = "We cannot download " + dependentPackage.Id + " Please make sure you are connected to the Internet.";
                                MessageBox.Show(message);
                                return;
                            }
                        }
                    }

                    this._app.ProgressHandler.Progress(null, 0, "Downloading " + pack.Title);
                    downloadDialog.ShowDownloadStatus(pack);
                    downloadDialog.SetProgressBarPercent(0);

                    this.packages.Install(pack.Id);
                });
                task.ContinueWith(delegate(Task t)
                {
                    this._app.ProgressHandler.Progress(null, 0, "Installing " + pack.Title);
                    this.UpdateInstalledProjectsList();
                    // Load the extension.
                    _app.RefreshExtensions();
                    IEnumerable<PackageDependency> dependency = pack.Dependencies;
                    _app.ProgressHandler.Progress(null, 50, "Installing " + pack.Title);

                    // Activate the extension(s) that was installed.
                    var extensions = _app.Extensions.Where(a => !inactiveExtensions.Contains(a) && a.IsActive == false);

                    if (extensions.Count() > 0 && !_app.EnsureRequiredImportsAreAvailable())
                        return;

                    foreach (var item in extensions)
                    {
                        item.TryActivate();
                    }
                    this._app.ProgressHandler.Progress(null, 0, "Ready.");
                    downloadDialog.Visible = false;

                    // This is used to refresh the view
                    int temp = uxOnlineProjects.SelectedIndex;
                    this.uxOnlineProjects.SelectedIndex = temp + 1;
                    this.uxOnlineProjects.SelectedIndex = temp;

                }, TaskScheduler.FromCurrentSynchronizationContext());
             
            }
        }
        private void uxOnlineProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.uxOnlineProjects.Items.Count == 0)
            {
                this.btnInstall.Enabled = false;
                this.btnInstall.Visible = true;
                this.btnUninstall.Enabled = false;
                this.btnUninstall.Visible = false;
                return;
            }
            IPackage package = this.uxOnlineProjects.SelectedItem as IPackage;
            if (package == null)
            {
       
                this.btnInstall.Enabled = false;
                this.btnInstall.Visible = true;
                this.btnUninstall.Enabled = false;
                this.btnUninstall.Visible = false;
                return;
            }
            if (IsPackageInstalled(package))
            {
                this.btnInstall.Visible = false;
                this.btnInstall.Enabled = false;
                this.btnUninstall.Enabled = true;
                this.btnUninstall.Visible = true;
                return;
            }
            this.btnInstall.Visible = true;
            this.btnInstall.Enabled = true;
            this.btnUninstall.Enabled = false;
            this.btnUninstall.Visible = false;
        }
        public static bool IsPackageInstalled(IPackage pack)
        {
            string packagePath = GetPackagePath(pack);

            if (Directory.Exists(packagePath))
            {
                return NuGet.EnumerableExtensions.Any<string>(Directory.EnumerateFiles(packagePath, "*.dspx", SearchOption.AllDirectories));
            }
            else
            {
                return false;
            }
        }
        private static string GetPackagePath(IPackage pack)
        {
            return Path.Combine(AppManager.AbsolutePathToExtensions, "Packages", GetPackageFolderName(pack));
        }
        private static string GetPackageFolderName(IPackage pack)
        {
            return string.Format("{0}.{1}", pack.Id, pack.Version);
        }
    } 

    public class ProjectFileInfo : ISampleProject
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

        public override bool Equals(object obj)
        {
            return Equals(obj as ProjectFileInfo);
        }

        public bool Equals(ProjectFileInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        string ISampleProject.AbsolutePathToProjectFile
        {
            get { return FullPath; }
        }

        string ISampleProject.Description
        {
            get { throw new NotImplementedException(); }
        }

        string ISampleProject.Name
        {
            get { return Name; }
        }
    }

    public class CustomListBox : ListBox
    {
        public CustomListBox()
        {
            this.DrawMode = DrawMode.OwnerDrawVariable; // We're using custom drawing.
            this.ItemHeight = 14; // Set the item height to 40.
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // Make sure we're not trying to draw something that isn't there.
            if (e.Index >= this.Items.Count || e.Index <= -1)
                return;

            // Get the item object.
            ISampleProject item = (ISampleProject)this.Items[e.Index];
            if (item == null)
                return;

            // Draw the background color depending on 
            // if the item is selected or not.
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                // The item is selected.
                // We want a blue background color.
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(51, 153, 255)), e.Bounds);
                // Draw the item.
                string text = item.Name;
                SizeF stringSize = e.Graphics.MeasureString(text, this.Font);
                e.Graphics.DrawString(text, this.Font, new SolidBrush(Color.White),
                    new PointF(15, e.Bounds.Y + (e.Bounds.Height - stringSize.Height) / 2));
                if (!(item is ProjectFileInfo))
                {
                    e.Graphics.DrawImage(HydroDesktop.Main.Properties.Resources.Template, 0, (e.Bounds.Y + (e.Bounds.Height - stringSize.Height) / 2) + 2);
                }
                else 
                {
                    e.Graphics.DrawImage(HydroDesktop.Main.Properties.Resources.recent_project, 0, (e.Bounds.Y + (e.Bounds.Height - stringSize.Height) / 2) + 2);
                }
            }
            else
            {
                // The item is NOT selected.
                // We want a white background color.
                e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);
                // Draw the item.
                string text = item.Name;
                SizeF stringSize = e.Graphics.MeasureString(text, this.Font);
                e.Graphics.DrawString(text, this.Font, new SolidBrush(Color.Black),
                    new PointF(15, e.Bounds.Y + (e.Bounds.Height - stringSize.Height) / 2));
                if (!(item is ProjectFileInfo))
                {
                    e.Graphics.DrawImage(HydroDesktop.Main.Properties.Resources.Template, 0, (e.Bounds.Y + (e.Bounds.Height - stringSize.Height) / 2) + 2);
                }
                else
                {
                    e.Graphics.DrawImage(HydroDesktop.Main.Properties.Resources.recent_project, 0, (e.Bounds.Y + (e.Bounds.Height - stringSize.Height) / 2) + 2);
                }
            }

           
        }
    }

    public class OnlineListBox : ListBox
    {
        public OnlineListBox()
        {
            this.DrawMode = DrawMode.OwnerDrawVariable; // We're using custom drawing.
            this.ItemHeight = 14; // Set the item height to 40.
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // Make sure we're not trying to draw something that isn't there.
            if (e.Index >= this.Items.Count || e.Index <= -1)
                return;

            // Get the item object.
            string loading = this.Items[e.Index] as string;
            if(loading != null) 
            {
                SizeF stringSize = e.Graphics.MeasureString(loading, this.Font);
                e.Graphics.DrawString(loading, this.Font, new SolidBrush(Color.Black),
                    new PointF(0, e.Bounds.Y + (e.Bounds.Height - stringSize.Height) / 2));
            }
            else 
            {
            IPackage item = (IPackage)this.Items[e.Index];
            if (item == null)
                return;

            // Draw the background color depending on 
            // if the item is selected or not.
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                // The item is selected.
                // We want a blue background color.
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(51, 153, 255)), e.Bounds);
                // Draw the item.
                string text = item.ToString();
                SizeF stringSize = e.Graphics.MeasureString(text, this.Font);
                e.Graphics.DrawString(text, this.Font, new SolidBrush(Color.White),
                    new PointF(15, e.Bounds.Y + (e.Bounds.Height - stringSize.Height) / 2));
                if (HydroDesktop.Main.WelcomeScreen.IsPackageInstalled(item))
                {
                    e.Graphics.DrawImage(HydroDesktop.Main.Properties.Resources.check_mark, 2, (e.Bounds.Y + (e.Bounds.Height - stringSize.Height) / 2) + 1);
                }
                else
                {
                    e.Graphics.DrawImage(HydroDesktop.Main.Properties.Resources.download, 2, (e.Bounds.Y + (e.Bounds.Height - stringSize.Height) / 2) + 1);
                }
            }
            else
            {
                // The item is NOT selected.
                // We want a white background color.
                e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);
                // Draw the item.
                string text = item.ToString();
                SizeF stringSize = e.Graphics.MeasureString(text, this.Font);
                e.Graphics.DrawString(text, this.Font, new SolidBrush(Color.Black),
                    new PointF(15, e.Bounds.Y + (e.Bounds.Height - stringSize.Height) / 2));
                if (HydroDesktop.Main.WelcomeScreen.IsPackageInstalled(item))
                {
                    e.Graphics.DrawImage(HydroDesktop.Main.Properties.Resources.check_mark, 2, (e.Bounds.Y + (e.Bounds.Height - stringSize.Height) / 2) + 1);
                }
                else
                {
                    e.Graphics.DrawImage(HydroDesktop.Main.Properties.Resources.download, 2, (e.Bounds.Y + (e.Bounds.Height - stringSize.Height) / 2) + 1);
                }
            }


        }
        }
    }

}
