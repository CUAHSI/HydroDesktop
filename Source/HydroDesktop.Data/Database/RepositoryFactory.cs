using System;
using HydroDesktop.Interfaces;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Factory to get repositories
    /// </summary>
    public class RepositoryFactory
    {
        #region Singletone implementation
        
        private RepositoryFactory()
        {
            
        }

        private static readonly Lazy<RepositoryFactory> _instance = new Lazy<RepositoryFactory>(() => new RepositoryFactory(), true);
        /// <summary>
        /// Instance of RepositoryFactory
        /// </summary>
        public static RepositoryFactory Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Create instance of <see cref="IRepositoryManager"/> using connection string
        /// </summary>
        /// <param name="dbType">The type of the database (SQLite, SQLServer, ...)</param>
        /// <param name="connectionString">The connection string</param>
        /// <returns>Instance of <see cref="IRepositoryManager"/></returns>
        public IRepositoryManager CreateRepositoryManager(DatabaseTypes dbType, string connectionString)
        {
            return new RepositoryManagerSQL(dbType, connectionString);
        }

        /// <summary>
        /// Create instance of <see cref="IRepositoryManager"/> using DbOperations
        /// </summary>
        /// <param name="dbOperations">The DbOperations object for handling the database</param>
        /// <returns>Instance of <see cref="IRepositoryManager"/></returns>
        public IRepositoryManager CreateRepositoryManager(DbOperations dbOperations)
        {
            return new RepositoryManagerSQL(dbOperations);
        }

        /// <summary>
        /// Create instance of <see cref="IDataSeriesRepository"/> using connection string
        /// </summary>
        /// <param name="dbType">The type of the database (SQLite, SQLServer, ...)</param>
        /// <param name="connectionString">The connection string</param>
        /// <returns>Instance of <see cref="IDataSeriesRepository"/></returns>
        public IDataSeriesRepository CreateDataSeriesRepository(DatabaseTypes dbType, string connectionString)
        {
            return new DataSeriesRepository(dbType, connectionString);
        }

        /// <summary>
        /// Create instance of <see cref="IDataSeriesRepository"/> using DbOperations
        /// </summary>
        /// <param name="dbOperations">The DbOperations object for handling the database</param>
        /// <returns>Instance of <see cref="IDataSeriesRepository"/></returns>
        public IDataSeriesRepository CreateDataSeriesRepository(DbOperations dbOperations)
        {
            return new DataSeriesRepository(dbOperations);
        }

        /// <summary>
        /// Create instance of <see cref="IDataThemesRepository"/> using connection string
        /// </summary>
        /// <param name="dbType">The type of the database (SQLite, SQLServer, ...)</param>
        /// <param name="connectionString">The connection string</param>
        /// <returns>Instance of <see cref="IDataThemesRepository"/></returns>
        public IDataThemesRepository CreateDataThemesRepository(DatabaseTypes dbType, string connectionString)
        {
            return new DataThemesRepository(dbType, connectionString);
        }

        /// <summary>
        /// Create instance of <see cref="IDataThemesRepository"/> using DbOperations
        /// </summary>
        /// <param name="dbOperations">The DbOperations object for handling the database</param>
        /// <returns>Instance of <see cref="IDataThemesRepository"/></returns>
        public IDataThemesRepository CreateDataThemesRepository(DbOperations dbOperations)
        {
            return new DataThemesRepository(dbOperations);
        }

        #endregion
    }
}