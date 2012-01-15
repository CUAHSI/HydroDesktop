using HydroDesktop.Interfaces;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Base Repository
    /// </summary>
    public abstract class BaseRepository
    {
        #region Fields

        private readonly DbOperations _db;

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
        protected BaseRepository(DbOperations db)
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

        protected IHydroDbOperations HydroDbOperations
        {
            get { return _db; }
        }

        #endregion

        #region Private Methods

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