using HydroDesktop.Interfaces;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Repository for DataSeries
    /// </summary>
    class DataSeriesRepository : BaseRepository, IDataSeriesRepository
    {
        #region Constructors
        
        /// <summary>
        /// Create new instance of <see cref="DataSeriesRepository"/>
        /// </summary>
        /// <param name="dbType">The type of the database (SQLite, SQLServer, ...)</param>
        /// <param name="connectionString">The connection string</param>
        public DataSeriesRepository(DatabaseTypes dbType, string connectionString) : base(dbType, connectionString)
        {
        }

        /// <summary>
        /// Create new instance of <see cref="DataSeriesRepository"/>
        /// </summary>
        /// <param name="db">The DbOperations object for handling the database</param>
        public DataSeriesRepository(DbOperations db) : base(db)
        {
        }

        #endregion
    }
}