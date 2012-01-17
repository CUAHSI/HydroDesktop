using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HydroDesktop.Configuration
{
    /// <summary>
    /// Application - level settings including web service URLs and database
    /// locations
    /// </summary>
    public class Settings
    {
        readonly string _defaultHISCentralURL = //Properties.Settings.Default.DefaultHISCentralUrl;
        "http://hiscentral.cuahsi.org/webservices/hiscentral.asmx";

        private List<string> _hisCentralURLList = Properties.Settings.Default.HISCentralUrlList.Cast<string>().ToList();
        private string _downloadOption = "Append";

        private string _dataRepositoryConnectionString;

        private string _selectedHISCentralUrl = Properties.Settings.Default.SelectedHISCentralUrl;

        private string _metadataCacheConnectionString;

        /// <summary>
        /// Allocate ourselves. We have a private constructor, so no one else can.
        /// </summary>
        static readonly Settings _instance = new Settings();

        /// <summary>
        /// Access SiteStructure.Instance to get the singleton object.
        /// Then call methods on that instance.
        /// </summary>
        public static Settings Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Creates a new settings object with default values.
        /// This is a private constructor, meaning no outsiders have access.
        /// </summary>

        private Settings()
        {
            //_hisCentralURLList.Add(DefaultHISCentralURL);
            //SelectedHISCentralURL = DefaultHISCentralURL;
            //_dataRepositoryConnectionString = SQLiteHelper.GetSQLiteConnectionString(GetDefaultDatabasePath());
            //_downloadOption = OverwriteOptions.Append;
            //_recentProject = GetDefaultRecentProjectFile();
        }

        /// <summary>
        /// Gets the collection of available HIS Central URLs
        /// </summary>
        public IList<string> HISCentralURLList
        {
            get
            {
                return _hisCentralURLList;
            }
        }

        /// <summary>
        /// Get The default HIS Central URL
        /// </summary>
        public string DefaultHISCentralURL
        {
            get
            {
                return _defaultHISCentralURL;
            }
        }

        /// <summary>
        /// Gets or sets currently user-selected HIS Central URL
        /// This is a user-level setting
        /// </summary>
        public string SelectedHISCentralURL
        {
            get
            {
                if (!_hisCentralURLList.Contains(_selectedHISCentralUrl))
                {
                    _selectedHISCentralUrl = _defaultHISCentralURL;
                }
                return _selectedHISCentralUrl;
            }
            set
            {
                if (_hisCentralURLList.Contains(value))
                {
                    _selectedHISCentralUrl = value;
                }
                else
                {
                    _selectedHISCentralUrl = _defaultHISCentralURL;
                }
            }
        }

        /// <summary>
        /// The user-defined data repository connection string
        /// </summary>
        public string DataRepositoryConnectionString
        {
            get { return _dataRepositoryConnectionString; }
            set
            {
                string oldValue = _dataRepositoryConnectionString;
                _dataRepositoryConnectionString = value;
                if (oldValue != value)
                    OnDatabaseChanged();
            }
        }

        /// <summary>
        /// Gets or sets the currently used metadata cache database connection string
        /// </summary>
        public string MetadataCacheConnectionString
        {
            get
            {
                return _metadataCacheConnectionString;
            }
            set
            {
                string oldValue = _metadataCacheConnectionString;
                _metadataCacheConnectionString = value;
                if (oldValue != value)
                    OnMetadataCacheChanged();
            }
        }

        /// <summary>
        /// The option how to handle duplicate data values during
        /// data download and saving to database
        /// </summary>
        public string DownloadOption
        {
            get { return _downloadOption; }
            set
            {
                string[] overwriteOptions = Properties.Settings.Default.OVERWRITE_OPTIONS.Cast<string>().ToArray();

                if (overwriteOptions.Contains(value))
                {
                    _downloadOption = value;
                }
                else
                {
                    throw new ArgumentException(String.Format("The DownloadOption '{0}' is not a valid option.", value));
                }
                _downloadOption = value;
            }
        }

        /// <summary>
        /// Gets the default project file name
        /// </summary>
        [Obsolete()]
        public string DefaultProjectFileName
        {
            get
            {
                return Path.Combine(ApplicationDataDirectory, Properties.Settings.Default.DefaultProjectFileName);
            }
        }

        /// <summary>
        /// Gets the default sqlite path
        /// </summary>
        public string DefaultDatabasePath
        {
            get
            {
                return Path.Combine(ApplicationDataDirectory, Properties.Settings.Default.DefaultDatabaseFileName);
            }
        }

        /// <summary>
        /// Gets the default 'metadata cache' sqlite db path
        /// </summary>
        public string DefaultMetadataCacheDbPath
        {
            get
            {
                return Path.Combine(ApplicationDataDirectory,
                    Properties.Settings.Default.DefaultMetadataCacheFileName);
            }
        }

        /// <summary>
        /// Gets the Current project file name
        /// (the full path is returned)
        /// </summary>
        public string CurrentProjectFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current project directory path
        /// </summary>
        public string CurrentProjectDirectory
        {
            get
            {
                return Path.GetDirectoryName(CurrentProjectFile);
            }
        }

        /// <summary>
        /// Gets or the directory where 'Sample Projects' are located
        /// </summary>
        public string SampleProjectsDirectory
        {
            get
            {
                string programPath = AppDomain.CurrentDomain.BaseDirectory;
                return Path.Combine(programPath, Properties.Settings.Default.SampleProjectsDirectory);
            }
        }

        /// <summary>
        /// Gets the directory with the default 'base map' data
        /// </summary>
        public string DefaultBaseMapDirectory
        {
            get
            {
                string programPath = AppDomain.CurrentDomain.BaseDirectory;
                return Path.Combine(programPath, Properties.Settings.Default.DefaultBaseMapDirectory);
            }
        }

        /// <summary>
        /// Gets the application data directory where the
        /// 'settings.xml' file is saved
        /// </summary>
        public string ApplicationDataDirectory
        {
            get
            {
                return ConfigurationHelper.FindOrCreateAppDataDirectory();
            }
        }

        /// <summary>
        /// Gets the directory for saving temporary HydroDesktop related
        /// files
        /// </summary>
        public string TempDirectory
        {
            get
            {
                return ConfigurationHelper.FindOrCreateTempDirectory();
            }
        }

        /// <summary>
        /// Gets the list of recent project files
        /// </summary>
        public System.Collections.Specialized.StringCollection RecentProjectFiles
        {
            get
            {
                return Properties.Settings.Default.RecentProjectFiles;
            }
        }

        /// <summary>
        /// Add a project file name to the list of recent project files
        /// </summary>
        /// <param name="fileName"></param>
        public void AddFileToRecentFiles(string fileName)
        {
            int maximumNumberOfRecentFiles = 10;

            if (Properties.Settings.Default.RecentProjectFiles.Contains(fileName))
            {
                Properties.Settings.Default.RecentProjectFiles.Remove(fileName);
            }

            if (Properties.Settings.Default.RecentProjectFiles.Count >= maximumNumberOfRecentFiles)
                Properties.Settings.Default.RecentProjectFiles.RemoveAt(Properties.Settings.Default.RecentProjectFiles.Count - 1);

            // insert value at the top of the list
            Properties.Settings.Default.RecentProjectFiles.Insert(0, fileName);

            //save settings
            Properties.Settings.Default.Save();
        }

        #region Events

        /// <summary>
        /// This event occurs when the main database connection string has changed
        /// </summary>
        public event EventHandler DatabaseChanged;

        private void OnDatabaseChanged()
        {
            var handler = DatabaseChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// This event occurs when the metadata cache database connection string
        /// has changed
        /// </summary>
        public event EventHandler MetadataCacheChanged;

        private void OnMetadataCacheChanged()
        {
            var handler = MetadataCacheChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion Events
    }
}