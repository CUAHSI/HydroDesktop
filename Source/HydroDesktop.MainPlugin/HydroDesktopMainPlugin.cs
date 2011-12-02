namespace HydroDesktop.Main
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using DotSpatial.Controls;
    using DotSpatial.Controls.Header;
    using System.ComponentModel.Composition;
    using DotSpatial.Controls.Docking;
    using HydroDesktop.Configuration;
    using System.IO;
    using HydroDesktop.Interfaces;
    using HydroDesktop.Database;

    public class HydroDesktopMainPlugin : Extension, IPartImportsSatisfiedNotification
    {
        //the seriesView component is the shared HydroDesktop component
        //for database management
        [Import("SeriesControl", typeof(ISeriesSelector))]
        internal ISeriesSelector SeriesControl { get; set; }
       
        [Import("Shell")]
        public ContainerControl Shell { get; set; }

        private ProjectManager myProjectManager;

        private WelcomeScreen welcomeScreenForm;

        public override void Activate()
        {
            App.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
            myProjectManager = new ProjectManager(App);

            App.HeaderControl.RootItemSelected += new EventHandler<RootItemEventArgs>(HeaderControl_RootItemSelected);
            App.SerializationManager.Serializing += new EventHandler<SerializingEventArgs>(SerializationManager_Serializing);
            App.SerializationManager.Deserializing += new EventHandler<SerializingEventArgs>(SerializationManager_Deserializing);
            App.SerializationManager.NewProjectCreated += new EventHandler<SerializingEventArgs>(SerializationManager_NewProjectCreated);
            App.SerializationManager.IsDirtyChanged += new EventHandler(SerializationManager_IsDirtyChanged);
            
            base.Activate();
        }

        public override void  Deactivate()
        {       
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
            ShowWelcomeScreen();   
        }

        #endregion

        //Saving a project (save or save as..)
        void SerializationManager_Serializing(object sender, SerializingEventArgs e)
        {
            myProjectManager.SavingProject();

            Shell.Text = "CUAHSI HydroDesktop - " + Path.GetFileName(App.SerializationManager.CurrentProjectFile);
            
            //string projFile = App.SerializationManager.CurrentProjectFile;
            //App.ProgressHandler.Progress("Saving Project " + projFile, 0, "Saving Project " + projFile);
            
            
            //throw new NotImplementedException();
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
            //throw new NotImplementedException();
        }

        void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            myProjectManager.OpeningProject();
            Shell.Text = "CUAHSI HydroDesktop - " + Path.GetFileName(App.SerializationManager.CurrentProjectFile);
            //setup new db information
            SeriesControl.SetupDatabase();
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

            //int x = this.Location.X + this.Width / 2 - _welcomeScreen.Width / 2;
            //int y = this.Location.Y + this.Height / 2 - _welcomeScreen.Height / 2;
            //_welcomeScreen.Location = new System.Drawing.Point(x, y);

            welcomeScreenForm.Show();
        }

        void welcomeScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if creating new project - setup the databases
            if (welcomeScreenForm.NewProjectCreated)
            {
                SetupDatabases();
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

                //initialize the Series control (populates its columns)
                //this code will fail if DataRepositoryConnectionString is not set
                this.SeriesControl.SetupDatabase();
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
                App.HeaderControl.SelectRoot(HeaderControl.HomeRootItemKey);
            }
        }

        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            if (e.SelectedRootKey == HeaderControl.HomeRootItemKey)
            {
                App.DockManager.SelectPanel("kMap");
            }
        }
    }
}
