using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using log4net;
using HydroDesktop.Configuration;


namespace HydroDesktop.Database.Tests
{
    public static class TestConfig
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       
        private static RepositoryManager _dataRepositoryManager = null;

          const string sqlliteExistingDBFormat = "Data Source={0};New=False;Compress=True;Version=3";
        /// <summary>
        /// Get the default 'local cache' database connection string
        /// </summary>
        /// <returns></returns>
        public static string DefaultLocalCacheConnection()
        {
            string dbDir = AppDomain.CurrentDomain.BaseDirectory;
           dbDir = Path.Combine(dbDir, "TestDatabases");
            string dbPath = System.IO.Path.Combine(dbDir, "MetadataCacheTest.sqlite");
            log.Info("DefaultDB ConncetionPath "+String.Format(sqlliteExistingDBFormat, dbPath));
            return String.Format(sqlliteExistingDBFormat,dbPath );
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
            return dbPath;
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
                string config = Settings.Instance.TempDirectory;

                return config;
            }
        }

        /// <summary>
        /// Gets the data manager for the default 'ActualData' database
        /// </summary>
        public static RepositoryManager RepositoryManager
        {
            get
            {
                if (_dataRepositoryManager == null)
                {
                    log.Debug("Call to create RepositoryManager");
                    _dataRepositoryManager = 
                        new RepositoryManager(DatabaseTypes.SQLite, DefaultActualDataConnection());
                }
                return _dataRepositoryManager;
            }
        }
    }
}
