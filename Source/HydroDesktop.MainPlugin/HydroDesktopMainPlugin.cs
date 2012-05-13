namespace HydroDesktop.Main
{
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;
    using DotSpatial.Controls;
    using DotSpatial.Controls.Header;
    using HydroDesktop.Configuration;
    using HydroDesktop.Database;
    using HydroDesktop.Interfaces;

    public class HydroDesktopMainPlugin : Extension, IPartImportsSatisfiedNotification
    {
        private const string HYDRODESKTOP_NAME = "CUAHSI HydroDesktop";

        //the seriesView component is the shared HydroDesktop component
        //for database management
        [Import("SeriesControl", typeof(ISeriesSelector))]
        internal ISeriesSelector SeriesControl { get; set; }
       
        [Import("Shell")]
        public ContainerControl Shell { get; set; }

        private ProjectManager myProjectManager;

        private WelcomeScreen welcomeScreenForm;

        private CoordinateDisplay latLongDisplay;

        private SelectionStatusDisplay selectionDisplay;

        public override void Activate()
        {
            //startup logging
            var logger = new TraceLogger();
            logger.CreateTraceFile();

            
            App.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
            myProjectManager = new ProjectManager(App);

            App.HeaderControl.RootItemSelected += HeaderControl_RootItemSelected;
            App.SerializationManager.Serializing += SerializationManager_Serializing;
            App.SerializationManager.Deserializing += SerializationManager_Deserializing;
            App.SerializationManager.NewProjectCreated += SerializationManager_NewProjectCreated;
            App.SerializationManager.IsDirtyChanged += SerializationManager_IsDirtyChanged;

            // todo: export Shell in MapWindow as Form to avoid type casting
            if (Shell is Form)
            {
                ((Form)Shell).FormClosing += HydroDesktopMainPlugin_FormClosing;
            }

            //show latitude, longitude coordinate display
            latLongDisplay = new CoordinateDisplay(App);
            //show selection status display
            //selectionDisplay = new SelectionStatusDisplay(App);

            base.Activate();
        }

        void HydroDesktopMainPlugin_FormClosing(object sender, FormClosingEventArgs e)
        {
            //write the log file from trace
            Trace.Flush();
            
            var projectExists = (!String.IsNullOrEmpty(App.SerializationManager.CurrentProjectFile));
            if (!projectExists)
            {
                var res = MessageBox.Show(string.Format("Save changes to new project?"),
                                     HYDRODESKTOP_NAME,
                                     MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
                                     MessageBoxDefaultButton.Button3);
                switch (res)
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                    case DialogResult.No:
                        // Do nothing, exit without saving
                        return;
                    case DialogResult.Yes:
                        // Save and exit
                        e.Cancel = true;
                        ShowSaveProjectDialog();
                        return;
                }
            }

            var hasProjectChanges = App.SerializationManager.IsDirty;
            if (hasProjectChanges)
            {
                var res = MessageBox.Show(string.Format("Save changes to current project [{0}]?", GetProjectShortName()),
                                    HYDRODESKTOP_NAME,
                                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
                                    MessageBoxDefaultButton.Button3);
                switch (res)
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                    case DialogResult.No:
                        // Do nothing, exit without saving
                        return;
                    case DialogResult.Yes:
                        // Save and exit
                        App.SerializationManager.SaveProject(App.SerializationManager.CurrentProjectFile);
                        return;
                }
            }
            else
            {
                var res = MessageBox.Show(string.Format("Exit Hydrodesktop?"),
                                    HYDRODESKTOP_NAME,
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question,
                                    MessageBoxDefaultButton.Button2);
                switch (res)
                {
                    case DialogResult.Cancel:
                        // Not exit
                        e.Cancel = true;
                        return;
                    case DialogResult.OK:
                        // Exit
                        return;
                }
            }
        }

        private void ShowSaveProjectDialog()
        {
            using (var dlg = new SaveFileDialog { Filter = App.SerializationManager.SaveDialogFilterText, SupportMultiDottedExtensions = true })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    App.SerializationManager.SaveProject(dlg.FileName);
                }
            }
        }

        public override void  Deactivate()
        {
            if (Shell is Form)
            {
                ((Form)Shell).FormClosing -= HydroDesktopMainPlugin_FormClosing;
            }

            base.Deactivate();
        }

        #region IPartImportsSatisfiedNotification Members

        /// <summary>
        /// setup the parent form. This 
        /// occurs when the main form becomes available
        /// </summary>
        public void OnImportsSatisfied()
        {
            Form mainForm = Shell as Form;
            if (mainForm != null)
            {
                mainForm.Shown += new EventHandler(mainForm_Shown);
            }
        }

        void mainForm_Shown(object sender, EventArgs e)
        {
            //displays the initial welcome screen
            if (string.IsNullOrEmpty(App.SerializationManager.CurrentProjectFile))
            {
                ShowWelcomeScreen();
            }
            else
            {
                //do not show the welcome screen if a project is being opened
                SerializationManager_Deserializing(null, null);
            }
        }

        #endregion

        //Saving a project (save or save as..)
        void SerializationManager_Serializing(object sender, SerializingEventArgs e)
        {
            myProjectManager.SavingProject();
            Shell.Text = string.Format("{0} - {1}", HYDRODESKTOP_NAME, GetProjectShortName());
        }

        private string GetProjectShortName()
        {
            return Path.GetFileName(App.SerializationManager.CurrentProjectFile);
        }

        //show information about current project state
        void SerializationManager_IsDirtyChanged(object sender, EventArgs e)
        {
            if (App.SerializationManager.IsDirty && !(Shell.Text.EndsWith(" *")))
            {
                Shell.Text += " *";
            }
            else if (!App.SerializationManager.IsDirty && Shell.Text.EndsWith(" *"))
            {
                Shell.Text = Shell.Text.Substring(0, Shell.Text.LastIndexOf("*"));
            }
        }

        void SerializationManager_NewProjectCreated(object sender, SerializingEventArgs e)
        {
            SetupDatabases();
            Shell.Text = HYDRODESKTOP_NAME;
            //setup new db information
            //SeriesControl.SetupDatabase();
            if (App.Map.Projection != null)
            {
                latLongDisplay.MapProjectionString = App.Map.Projection.ToEsriString();
            }
        }

        void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            //try reset projection!
            if (App.Map.MapFrame.Projection != DotSpatial.Projections.KnownCoordinateSystems.Projected.World.WebMercator)
            {
                //App.Map.MapFrame.Reproject(DotSpatial.Projections.KnownCoordinateSystems.Projected.World.WebMercator);
                MapFrameProjectionHelper.ReprojectMapFrame(App.Map.MapFrame, DotSpatial.Projections.KnownCoordinateSystems.Projected.World.WebMercator.ToEsriString());
            }
            
            myProjectManager.OpeningProject();
            Shell.Text = string.Format("{0} - {1}", HYDRODESKTOP_NAME, GetProjectShortName());
            //setup new db information
            SeriesControl.SetupDatabase();
            if (App.Map.Projection != null)
            {
                latLongDisplay.MapProjectionString = App.Map.Projection.ToEsriString();
            }

            //disable progress reporting for the layers
        }


        ////runs when all extensions have been loaded
        //void App_Loaded(object sender, EventArgs e)
        //{
        //    //displays the initial welcome screen
        //    ShowWelcomeScreen();

        //    //sets-up the database connection and creates a
        //    //new empty database if required
        //    SetupDatabases();
        //}

        /// <summary>
        /// In the welcome screen, user chooses to create a project from template,
        /// create a new project, or open an existing project
        /// </summary>
        private void ShowWelcomeScreen()
        {
            //activate the map panel
            App.DockManager.SelectPanel("kMap");
            App.DockManager.SelectPanel("kLegend");

            welcomeScreenForm = new WelcomeScreen(myProjectManager);
            welcomeScreenForm.StartPosition = FormStartPosition.CenterScreen;
            welcomeScreenForm.TopMost = true;
            welcomeScreenForm.FormClosing += new FormClosingEventHandler(welcomeScreen_FormClosing);

            int x = Shell.Location.X + Shell.Width / 2 - welcomeScreenForm.Width / 2;
            int y = Shell.Location.Y + Shell.Height / 2 - welcomeScreenForm.Height / 2;
            welcomeScreenForm.Location = new System.Drawing.Point(x, y);


            App.CompositionContainer.ComposeParts(welcomeScreenForm);

            welcomeScreenForm.Show();
            welcomeScreenForm.Focus();
        }

        void welcomeScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if creating new project - setup the databases
            if (welcomeScreenForm.NewProjectCreated)
            {
                SetupDatabases();
            }

            //setup the lat, long coordinate display
            latLongDisplay.ShowCoordinates = true;
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
            //TODO: find a smart solution when Write access to temp folder is denied
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

        void DockManager_ActivePanelChanged(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == "kMap")
            {
                App.DockManager.SelectPanel("kLegend");

                //if the clicked root item was 'search', then don't select the map root item
                //(the user intended to show search tab and map panel)
                if (!App.SerializationManager.GetCustomSetting("SearchRootClicked", false) &&
                    !App.SerializationManager.GetCustomSetting("MetadataRootClicked", false))
                {
                    App.HeaderControl.SelectRoot(HeaderControl.HomeRootItemKey);
                }
                else
                {
                    App.SerializationManager.SetCustomSetting("SearchRootClicked", false);
                    App.SerializationManager.SetCustomSetting("MetadataRootClicked", false);
                }
            }
        }

        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            if (e.SelectedRootKey == HeaderControl.HomeRootItemKey)
            {
                App.DockManager.SelectPanel("kMap");
                if (latLongDisplay != null)
                    this.latLongDisplay.ShowCoordinates = true;
            }
            else
            {
                if (latLongDisplay != null)
                    latLongDisplay.ShowCoordinates = false;
            }
        }
    }
}
