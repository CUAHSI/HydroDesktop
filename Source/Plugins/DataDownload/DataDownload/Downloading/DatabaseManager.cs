using System;
using HydroDesktop.Configuration;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;

namespace HydroDesktop.DataDownload.Downloading
{
    class DatabaseManager
    {
        private DatabaseManager()
        {
        }

        private static readonly Lazy<DatabaseManager> _instance = new Lazy<DatabaseManager>(() => new DatabaseManager(), true);
        public static DatabaseManager Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private DbOperations _cachedDbOperations;
        public DbOperations GetDbOperationsForCurrentProject()
        {
            if (_cachedDbOperations == null ||
                _cachedDbOperations.ConnectionString != Settings.Instance.DataRepositoryConnectionString)
            {
                _cachedDbOperations = new DbOperations(Settings.Instance.DataRepositoryConnectionString,
                                                       DatabaseTypes.SQLite);
            }

            return _cachedDbOperations;
        }
    }
}