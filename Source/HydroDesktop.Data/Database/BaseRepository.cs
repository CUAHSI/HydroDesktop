using System;
using System.Data;
using System.Linq;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Base Repository
    /// </summary>
    public abstract class BaseRepository<T> : IRepository<T>
        where T : BaseEntity
    {
        #region Fields

        private readonly IHydroDbOperations _db;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the manager given a connection string
        /// </summary>
        /// <param name="dbType">The type of the database (SQLite, SQLServer, ...)</param>
        /// <param name="connectionString">The connection string</param>
        protected BaseRepository(DatabaseTypes dbType, string connectionString)
        {
            //if it's a SQLite database - check if DB file exists
            if (dbType == DatabaseTypes.SQLite)
            {
                CheckDbFile(connectionString);
            }
            
            //initialize the DAO objects           
            _db = new DbOperations(connectionString, dbType);
        }

        /// <summary>
        /// Creates a new BaseRepository associated with the specified database
        /// </summary>
        /// <param name="db">The DbOperations object for handling the database</param>
        protected BaseRepository(IHydroDbOperations db)
        {
            //if it's a SQLite database - check if DB file exists
            if (db.DatabaseType == DatabaseTypes.SQLite)
            {
                CheckDbFile(db.ConnectionString);
            }

            _db = db;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Database operations
        /// </summary>
        public IHydroDbOperations DbOperations
        {
            get { return _db; }
        }

        /// <summary>
        /// Table (Entity) name for which this repository used
        /// </summary>
        public abstract string TableName { get; }

        /// <summary>
        /// Name of primary key column
        /// </summary>
        public virtual string PrimaryKeyName
        {
            get
            {
                throw new NotImplementedException("Implement me in child class.");   
            }
        }

        public string LastRowIDSelect
        {
            get { return "; SELECT LAST_INSERT_ROWID();"; }
        }

        #endregion

        #region Public methods

        public DataTable AsDataTable()
        {
            var table = DbOperations.LoadTable(TableName, string.Format("Select * from {0}", TableName));
            return table;
        }

        public long GetNextID()
        {
            return DbOperations.GetNextID(TableName, PrimaryKeyName);
        }

        public T[] GetAll()
        {
            var dt = AsDataTable();
            var res = dt.Rows.Cast<DataRow>().Select(DataRowToEntity).ToArray();
            return res;
        }

        public T GetByKey(object key)
        {
             var table = DbOperations.LoadTable(TableName,
                                    string.Format("select * from {0} where {1}={2}", TableName, PrimaryKeyName, key));
             if (table.Rows.Count == 0)
                 return default(T);
            return DataRowToEntity(table.Rows[0]);
        }

        #endregion

        #region Private Methods

        protected virtual T DataRowToEntity (DataRow row)
        {
            throw new NotImplementedException("Implement me in child class.");
        }

        /// <summary>
        /// Checks if the SQLite db file exists. if it doesn't exist,
        /// re-create it
        /// </summary>
        private void CheckDbFile(string sqLiteConnString)
        {
            string sqlitePath = SQLiteHelper.GetSQLiteFileName(sqLiteConnString);
            if (!SQLiteHelper.DatabaseExists(sqlitePath))
            {
                SQLiteHelper.CreateSQLiteDatabase(sqlitePath);
            }
        }

        #endregion
    }
}