using System.IO;
using HydroDesktop.Interfaces;
using HydroDesktop.Database;

namespace HydroDesktop.Data.Tests
{
    public static class TestConfig
    {
        private static MetadataCacheManagerSQL _metadataCacheManager;
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
            get {
                return _metadataCacheManager ??
                       (_metadataCacheManager =
                        new MetadataCacheManagerSQL(DatabaseTypes.SQLite, DefaultLocalCacheConnection));
            }
        }
    }
}
