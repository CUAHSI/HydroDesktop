using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.Database;
using HydroDesktop.Configuration;
using System.IO;
using HydroDesktop.Interfaces;

namespace HydroDesktop.Controls
{
    /// <summary>
    /// The HydroDatabase class provides a direct access point for
    /// plugins to the DataRepository database
    /// </summary>
    public class HydroDatabase :  IHydroDatabase
    {
        //private bool _connected = false;

        private DbOperations _db;
        private IRepositoryManager _repositoryManager;

        public HydroDatabase()
        {
            //the database path must be known..
            
            //Settings.Instance.Load();
            Initialize(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);
        }

        /// <summary>
        /// Creates a new HydroDatabase component using the specific connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public HydroDatabase(string connectionString)
        {
            Initialize(connectionString, DatabaseTypes.SQLite);
        }

        /// <summary>
        /// Creates a new HydroDatabase manager component for the specific database
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbTypes"></param>
        public HydroDatabase(string connectionString, DatabaseTypes dbTypes)
        {        
            Initialize(connectionString, DatabaseTypes.SQLite);
        }

        /// <summary>
        /// Core initialization rotines
        /// </summary>
        /// <param name="connString">the connection string</param>
        /// <param name="dbTypes">the type of database</param>
        private void Initialize(string connString, DatabaseTypes dbTypes)
        {

            //if the connectionString is invalid - try to create a new empty db
            string dbPath = SQLiteHelper.GetSQLiteFileName(connString);
            
            //try to create the SQLite database in the location specified by conn.string

            if (!SQLiteHelper.DatabaseExists(dbPath))
            {
                SQLiteHelper.CreateSQLiteDatabase(dbPath);

                //if creation fails - try to create the SQLite database in a temporary file
                if (!SQLiteHelper.DatabaseExists(dbPath))
                {
                    dbPath = Path.Combine(Path.GetTempPath(), "default.sqlite");
                    SQLiteHelper.CreateSQLiteDatabase(dbPath);

                    //if creation of temporary db file fails - throw exception
                    if (!SQLiteHelper.DatabaseExists(dbPath))
                    {
                        throw new UnauthorizedAccessException
                            ("Error opening or creating the default SQLite data repository database.");
                    }

                    connString = SQLiteHelper.GetSQLiteConnectionString(dbPath);
                    Settings.Instance.DataRepositoryConnectionString = connString;
                }
            }
            

            _db = new DbOperations(connString, dbTypes);
            _repositoryManager = new RepositoryManagerSQL(_db);
        }

        #region IHydroDatabase Members

        /// <summary>
        /// Gets or sets the database connection string. Setting the connection
        /// string fires the DatabaseChanged event
        /// </summary>
        public string ConnectionString
        {
            get 
            {
                if (_db == null) return null;
                return _db.ConnectionString; 
   
            }
            set
            {
                if (_db.ConnectionString.ToLower() != value.ToLower())
                {
                    
                    /* missing? 
                     * if open close old database connection and 
                     * dispose of the resources, aka in memory tables and objects
                      */
                    _db = null;
                    _repositoryManager = null;
                    _db = new DbOperations(value, DatabaseType);
                    _repositoryManager = new RepositoryManagerSQL(_db);
                    //_connected = true;
                    OnDatabaseChanged();
                }
            }
        }

        public DatabaseTypes DatabaseType
        {
            get { return DatabaseTypes.SQLite; }
        }

        public event EventHandler DatabaseChanged;

        public System.Data.DataTable LoadTable(string sqlQuery)
        {
            return _db.LoadTable(sqlQuery);
        }

        /// <summary>
        /// The repository manager used to connect to the database
        /// </summary>
        public IRepositoryManager RepositoryManager
        {
            get { return _repositoryManager; }
        }

        /// <summary>
        /// The database operations for manipulating the database
        /// </summary>
        public IHydroDbOperations DbOperations
        {
            get { return _db; }
        }

        protected void OnDatabaseChanged()
        {
            if (DatabaseChanged != null) DatabaseChanged(this, null);
        }

        #endregion
    }
}
