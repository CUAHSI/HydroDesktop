using System.Reflection;
using System.IO;
using log4net;
using HydroDesktop.Interfaces;
using HydroDesktop.Database;


namespace HydroDesktop.Data.Tests
{
    public static class TestConfig
    {
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
       
        private static MetadataCacheManagerSQL _metadataCacheManager = null;
        private static IRepositoryManager _sqlRepositoryManager = null;
        private static DbOperations _dbOperations;
          
        /// <summary>
        /// Get the default 'local cache' database connection string
        /// </summary>
        /// <returns></returns>
        public static string DefaultLocalCacheConnection
        {
            get
            {
                string dbPath = Path.Combine(DefaultDatabaseDirectory, "MetadataCacheTest.sqlite");
                SQLiteHelper.CreateMetadataCacheDb(dbPath);
                log.Info("DefaultMetadataCacheDbFile  " + dbPath);
                return SQLiteHelper.GetSQLiteConnectionString(dbPath);
            }
        }

        /// <summary>
        /// Get the default 'local cache' database connection string
        /// </summary>
        /// <returns></returns>
        static string DefaultActualDataConnection
        {
            get
            {
                string dbPath = Path.Combine(DefaultDatabaseDirectory, "DataRepositoryTest.sqlite");
                SQLiteHelper.CreateSQLiteDatabase(dbPath);
                log.Info("DefaultActualDataFile  " + dbPath);
                return SQLiteHelper.GetSQLiteConnectionString(dbPath);
            }
        }
        /// <summary>
        /// Gets the directory where the default databases used by the system
        /// (DataRepository.sqlite and MetadataCache.sqlite) are located.
        /// </summary>
        /// <returns></returns>
        public static string DefaultDatabaseDirectory
        {
            get 
            {
                return Configuration.Settings.Instance.TempDirectory;
            }
        }

        /// <summary>
        /// Gets the default 'DbOperations' object for SQL-manipulation with the DB
        /// </summary>
        public static DbOperations DbOperations
        {
            get
            {
                return _dbOperations ??
                       (_dbOperations = new DbOperations(DefaultActualDataConnection, DatabaseTypes.SQLite));
            }
        }

        /// <summary>
        /// Gets the data manager for the default 'MetadataCache' database
        /// </summary>
        /// <returns></returns>
        public static MetadataCacheManagerSQL MetadataCacheManager
        {
            get
            {
                if (_metadataCacheManager == null)
                {
                    log.Debug("Call to create MetadataCacheManager"); 
                    _metadataCacheManager = new MetadataCacheManagerSQL(DatabaseTypes.SQLite, DefaultLocalCacheConnection);
                }
                return _metadataCacheManager;
            }
        }
    }
}
