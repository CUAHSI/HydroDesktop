using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using log4net;
using HydroDesktop.Interfaces;
using HydroDesktop.Controls.Themes;
using HydroDesktop.Database;


namespace HydroDesktop.Data.Tests
{
    public static class TestConfig
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       
        private static MetadataCacheManagerSQL _metadataCacheManager = null;
        private static RepositoryManagerSQL _sqlRepositoryManager = null;
        private static DbOperations _dbOperations = null;

          const string sqlliteExistingDBFormat = "Data Source={0};New=False;Compress=True;Version=3";
        /// <summary>
        /// Get the default 'local cache' database connection string
        /// </summary>
        /// <returns></returns>
        public static string DefaultLocalCacheConnection()
        {
            string dbPath = Path.Combine(DefaultDatabaseDirectory, "MetadataCacheTest.sqlite");
            SQLiteHelper.CreateMetadataCacheDb(dbPath);
            log.Info("DefaultMetadataCacheDbFile  " + dbPath);
            return SQLiteHelper.GetSQLiteConnectionString(dbPath);
        }

        

        /// <summary>
        /// Get the default 'local cache' database connection string
        /// </summary>
        /// <returns></returns>
        public static string DefaultActualDataConnection()
        {
            string dbPath = Path.Combine(DefaultDatabaseDirectory, "DataRepositoryTest.sqlite");
             SQLiteHelper.CreateSQLiteDatabase(dbPath);
            log.Info("DefaultActualDataFile  " +  dbPath);
            return SQLiteHelper.GetSQLiteConnectionString(dbPath);
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
                return HydroDesktop.Configuration.Settings.Instance.TempDirectory;
            }
        }

        public static RepositoryManagerSQL SQLRepositoryManager
        {
            get
            {
                if (_sqlRepositoryManager == null)
                {
                    log.Debug("Call to create SQL repository Manager");
                    _sqlRepositoryManager =
                        new RepositoryManagerSQL(DatabaseTypes.SQLite, DefaultActualDataConnection());
                }
               
                return _sqlRepositoryManager;
            }
        }

        
        /// <summary>
        /// Gets the default 'DbOperations' object for SQL-manipulation with the DB
        /// </summary>
        public static DbOperations DbOperations
        {
            get
            {
                if (_dbOperations == null)
                {
                    log.Debug("Call to create DbOperations"); 
                    _dbOperations = new DbOperations(DefaultActualDataConnection(), DatabaseTypes.SQLite);
                }
                return _dbOperations;
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
                    _metadataCacheManager = new MetadataCacheManagerSQL(DatabaseTypes.SQLite, DefaultLocalCacheConnection());
                }
                return _metadataCacheManager;
            }
        }

        public static ThemeManager ThemeManager
        {
            get
            {
                DbOperations db = new DbOperations(DefaultActualDataConnection(), DatabaseTypes.SQLite);
                return new ThemeManager(db);
            }
        }
    }
}
