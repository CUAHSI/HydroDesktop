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

namespace HydroDesktop.Plugins.MainPlugin
{
    /// <summary>
    /// The welcome screen form shown on program startup
    /// </summary>
    public partial class WelcomeScreen : Form
    {
        /// <summary>
        /// Gets the list tools available.
        /// </summary>
        public List<object> SampleProjects { get; set; }
        
        #region Private Variables

        private List<object> onlineProjects = new List<object>();
        private List<ProjectFileInfo> _recentProjectFiles = new List<ProjectFileInfo>();
        private AppManager _app;
        private bool _newProjectCreated = false;
        private readonly string _newProject = "New Project";
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

            lstProjectTemplates.Click += lstProjectTemplates_Click;
            lstProjectTemplates.KeyDown += lstProjectTemplates_KeyDown;
            FormClosing += WelcomeScreen_FormClosing;

            if (lstProjectTemplates.Items.Count > 0)
            {
                lstProjectTemplates.SelectedIndex = 0;
            }

            packages.SetNewSource("https://www.myget.org/F/cuahsi/");
            UpdatePackageList();
            downloadDialog.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            downloadDialog.Icon = HydroDesktop.Plugins.MainPlugin.Properties.Resources.download_icon;
        }

        private void lstProjectTemplates_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                lstProjectTemplates_Click(sender, e);
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
                this.Cursor = Cursors.WaitCursor;
                if (lstProjectTemplates.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select a project.");
                    DialogResult = DialogResult.None;
                    return;
                }
                if (lstProjectTemplates.SelectedItem as string != null)
                {
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

                this.Cursor = Cursors.Default;
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
            this.Cursor = Cursors.WaitCursor;
            panelStatus.Visible = true;
            myProjectManager.CreateEmptyProject();
            _newProjectCreated = true;
            this.Cursor = Cursors.Default;
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

        private void WelcomeScreen_Load(object sender, EventArgs e)
        {
            UpdateInstalledProjectsList();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            FeatureSet fs = new FeatureSet();
        }

        private void btnBrowseProject_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "HydroDesktop Project File|*.dspx";
            fileDialog.Title = "Select the Project File to Open";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                OpenExistingProject(fileDialog.FileName);
                this.Cursor = Cursors.Default;
            }
        }

        private void lstProjectTemplates_Click(object sender, EventArgs e)
        {
            var item = lstProjectTemplates.SelectedItem;

            if (!(item is string) || item as string == _newProject)
            {
                //if the map is empty or if the current project is already saved, start a new project
                if (!_app.SerializationManager.IsDirty || _app.Map.Layers == null || _app.Map.Layers.Count == 0)
                {
                }
                else if (String.IsNullOrEmpty(_app.SerializationManager.CurrentProjectFile))
                {
                    //if the current project is not specified - just ask to discard changes
                    if (MessageBox.Show("Start a new Project?", "Discard Changes?", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;
                }
                else
                {
                    //the current project is specified - ask the users if they want to save changes to current project
                    string saveProjectMessage = String.Format("Save changes to current project [{0}] ?", Path.GetFileName(_app.SerializationManager.CurrentProjectFile));
                    DialogResult msgBoxResult = MessageBox.Show(saveProjectMessage, "Discard Changes?", MessageBoxButtons.YesNoCancel);

                    if (msgBoxResult == DialogResult.Yes)
                        _app.SerializationManager.SaveProject(_app.SerializationManager.CurrentProjectFile);

                    if (msgBoxResult == DialogResult.Cancel)
                        return;
                }

                if (item as string == _newProject)
                    CreateEmptyProject();
                else if (item is ISampleProject && item is ProjectFileInfo)
                    OpenProject();
                else if (item is ISampleProject)
                    CreateProjectFromTemplate();
                else if (item is IPackage)
                    InstallSampleProject();
            }
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
            ProjectFileInfo selected = lstProjectTemplates.SelectedValue as ProjectFileInfo;
            if (selected != null)
            {
                panelStatus.Visible = true;
                this.Cursor = Cursors.WaitCursor;
                OpenExistingProject(selected.FullPath);
                this.Cursor = Cursors.Default;
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

            bsRecentFiles.ResetBindings(false);
        }

        private void UpdateInstalledProjectsList()
        {
            int index = lstProjectTemplates.SelectedIndex;
            SampleProjectInstaller spi = new SampleProjectInstaller();
            SampleProjects = new List<object>();

            FindRecentProjectFiles();
            SampleProjects.Add("Recent Projects:");
            SampleProjects.Add(_newProject);
            SampleProjects.AddRange(RecentProjectFiles);

            List<ISampleProject> templates = spi.SetupInstalledSampleProjects(spi.FindSampleProjectFiles()).ToList();
            if (templates.Count > 0)
            {
                SampleProjects.Add("Templates:");
                SampleProjects.AddRange(templates);
            }
            if (onlineProjects.Count > 0)
            {
                SampleProjects.Add("Online:");
                foreach (var item in onlineProjects)
                {
                    if (item is IPackage && !HydroDesktop.Plugins.MainPlugin.WelcomeScreen.IsPackageInstalled(item as IPackage))
                        SampleProjects.Add(item);
                    else if (item is string)
                        SampleProjects.Add(item);
                }
            }
            if (SampleProjects.IsEmpty())
                SampleProjects.Add("Could not find any project files.");

            this.lstProjectTemplates.DataSource = this.SampleProjects;
            this.lstProjectTemplates.DisplayMember = "Name";
            lstProjectTemplates.SelectedIndex = index;
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
            onlineProjects.Clear();
            onlineProjects.Add("Loading...");
            Task<IPackage[]> task = Task.Factory.StartNew<IPackage[]>(delegate
            {
                return (
                    from p in this.packages.Repo.GetPackages()
                    where p.IsLatestVersion && (p.Tags.Contains("DotSpatialSampleProject") || p.Tags.Contains("SampleProject"))
                    select p).ToArray<IPackage>();
            });
            task.ContinueWith(delegate(Task<IPackage[]> t)
            {
                onlineProjects.Clear();
                if (t.Exception == null)
                    onlineProjects.AddRange(t.Result);
                UpdateInstalledProjectsList();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion

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

        private void InstallSampleProject()
        {
            if (lstProjectTemplates.SelectedItem != null)
            {
                this.Cursor = Cursors.WaitCursor;
                downloadDialog.Show();
                
                IPackage pack = lstProjectTemplates.SelectedItem as IPackage;

                var inactiveExtensions = _app.Extensions.Where(a => a.IsActive == false).ToArray();

                IEnumerable<PackageDependency> dependency = pack.Dependencies;
                if (dependency.Count() > 0)
                {
                    foreach (PackageDependency dependentPackage in dependency)
                    {
                        _app.ProgressHandler.Progress(null, 0, "Downloading Dependency " + dependentPackage.Id);
                        downloadDialog.ShowDownloadStatus(dependentPackage);
                        downloadDialog.SetProgressBarPercent(0);
                        downloadDialog.Refresh();

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
                downloadDialog.Refresh();

                this.packages.Install(pack.Id);

                _app.ProgressHandler.Progress(null, 0, "Installing " + pack.Title);
                // Load the extension.
                _app.RefreshExtensions();
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

                UpdateInstalledProjectsList();
                selectDownloadedPackage(pack);
                this.Cursor = Cursors.Default;
            }
        }

        private void selectDownloadedPackage(IPackage pack)
        {
            string packagePath = GetPackagePath(pack);
            IEnumerable<string> files = Directory.EnumerateFiles(packagePath, "*.dspx", SearchOption.AllDirectories);
            if (NuGet.EnumerableExtensions.Any<string>(files))
            {
                foreach (var item in SampleProjects)
                {
                    if (item is ISampleProject && Path.GetFileNameWithoutExtension(files.First()) == (item as ISampleProject).Name)
                    {
                        lstProjectTemplates.SelectedIndex = SampleProjects.IndexOf(item);
                        CreateProjectFromTemplate();
                        break;
                    }
                }
            }
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
        int mouseIndex = -1;

        public CustomListBox()
        {
            DoubleBuffered = true;
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            this.DrawMode = DrawMode.OwnerDrawFixed; // We're using custom drawing.
            this.ItemHeight = 16; // Set the item height to 14.
            this.MouseMove += ListBox_MouseMove;
        }

        private void ListBox_MouseMove(object sender, MouseEventArgs e)
        {
            int index = IndexFromPoint(e.Location);
            if(index != mouseIndex)
            {
                if(mouseIndex > -1)
                {
                    int oldIndex = mouseIndex;
                    mouseIndex = -1;
                    if (oldIndex <= Items.Count - 1)
                        Invalidate(GetItemRectangle(oldIndex));
                }
                mouseIndex = index;
                if (mouseIndex > -1)
                    Invalidate(GetItemRectangle(mouseIndex));
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // Make sure we're not trying to draw something that isn't there.
            if (e.Index >= this.Items.Count || e.Index <= -1)
                return;

            object item = Items[e.Index];
            string text = "";
            SizeF stringSize = SizeF.Empty;

            if (item == null)
                return;

            Color backgroundColor = Color.White;
            Color fontColor = Color.Black;

            // Get the item object.
            if (item is string && (item as string).Contains(':'))
            {
                text = item as string;
                Font font = new Font(this.Font, FontStyle.Bold);
                e.Graphics.FillRectangle(new SolidBrush(backgroundColor), e.Bounds);
                stringSize = e.Graphics.MeasureString(text, font);
                e.Graphics.DrawString(text, font, new SolidBrush(fontColor),
                new PointF(0, e.Bounds.Y + (e.Bounds.Height - stringSize.Height)/2));
                base.OnDrawItem(e);
                return;
            } else if (item is string)
                text = item as string;
            else if (item is ISampleProject)
                text = ((ISampleProject)item).Name;
            else if (item is IPackage)
                text = ((IPackage)item).ToString();

            // Draw the background color depending on 
            // if the item is selected or not.
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                backgroundColor = Color.FromArgb(51, 153, 255);
                fontColor = Color.White;
            }
            else if (mouseIndex > -1 && mouseIndex == e.Index)
                backgroundColor = Color.LightBlue;

            // The item is NOT selected.
            // We want a white background color.
            e.Graphics.FillRectangle(new SolidBrush(backgroundColor), e.Bounds);

            // Draw the item.
            stringSize = e.Graphics.MeasureString(text, this.Font);
            e.Graphics.DrawString(text, this.Font, new SolidBrush(fontColor),
                new PointF(15, e.Bounds.Y + (e.Bounds.Height - stringSize.Height) / 2));
            if (item is ISampleProject && !(item is ProjectFileInfo))
            {
                Image image = HydroDesktop.Plugins.MainPlugin.Properties.Resources.Template;
                e.Graphics.DrawImage(image, 0, (e.Bounds.Y + (e.Bounds.Height - image.Height) / 2));
            }
            else if (item is ISampleProject && item is ProjectFileInfo)
            {
                Image image = HydroDesktop.Plugins.MainPlugin.Properties.Resources.recent_project;
                e.Graphics.DrawImage(image, 0, (e.Bounds.Y + (e.Bounds.Height - image.Height) / 2));
            }
            else if (item is IPackage && !HydroDesktop.Plugins.MainPlugin.WelcomeScreen.IsPackageInstalled(item as IPackage))
            {
                Image image = HydroDesktop.Plugins.MainPlugin.Properties.Resources.download;
                e.Graphics.DrawImage(image, 0, (e.Bounds.Y + (e.Bounds.Height - image.Height) / 2));
            }
            base.OnDrawItem(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Region iRegion = new Region(e.ClipRectangle);
            e.Graphics.FillRegion(new SolidBrush(this.BackColor), iRegion);
            if (this.Items.Count > 0)
            {
                for (int i = 0; i < this.Items.Count; ++i)
                {
                    System.Drawing.Rectangle irect = this.GetItemRectangle(i);
                    if (e.ClipRectangle.IntersectsWith(irect))
                    {
                        if ((this.SelectionMode == SelectionMode.One && this.SelectedIndex == i)
                        || (this.SelectionMode == SelectionMode.MultiSimple && this.SelectedIndices.Contains(i))
                        || (this.SelectionMode == SelectionMode.MultiExtended && this.SelectedIndices.Contains(i)))
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font,
                                irect, i,
                                DrawItemState.Selected, this.ForeColor,
                                this.BackColor));
                        }
                        else
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font,
                                irect, i,
                                DrawItemState.Default, this.ForeColor,
                                this.BackColor));
                        }
                        iRegion.Complement(irect);
                    }
                }
            }
            base.OnPaint(e);
        }
    }
}
