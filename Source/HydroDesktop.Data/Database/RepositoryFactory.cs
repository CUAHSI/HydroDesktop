using System;
using HydroDesktop.Interfaces;
using System.Collections.Generic;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Factory to get repositories
    /// </summary>
    public class RepositoryFactory
    {
        #region Fields

        private readonly Dictionary<Type, RepositoryCreator> _repositoryCreators;

        #endregion

        #region Singletone implementation

        private RepositoryFactory()
        {
            _repositoryCreators = new Dictionary<Type, RepositoryCreator>();

            AddRepoCreator<IRepositoryManager>(
                new RepositoryCreator
                    {
                        CreatorByConnectionString = (dbType, connStr) => new RepositoryManagerSQL(dbType, connStr),
                        CreatorByDbOperations = dbOp => new RepositoryManagerSQL(dbOp)
                    });
            AddRepoCreator<IDataSeriesRepository>(
                new RepositoryCreator
                    {
                        CreatorByConnectionString = (dbType, connStr) => new DataSeriesRepository(dbType, connStr),
                        CreatorByDbOperations = dbOp => new DataSeriesRepository(dbOp)
                    });
            AddRepoCreator<IDataThemesRepository>(
                new RepositoryCreator
                {
                    CreatorByConnectionString = (dbType, connStr) => new DataThemesRepository(dbType, connStr),
                    CreatorByDbOperations = dbOp => new DataThemesRepository(dbOp)
                });
            AddRepoCreator<IMethodsRepository>(
               new RepositoryCreator
               {
                   CreatorByConnectionString = (dbType, connStr) => new MethodsRepository(dbType, connStr),
                   CreatorByDbOperations = dbOp => new MethodsRepository(dbOp)
               });
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
        /// Get instance of <see cref="T"/> using connection string
        /// </summary>
        /// <param name="dbType">The type of the database (SQLite, SQLServer, ...)</param>
        /// <param name="connectionString">The connection string</param>
        /// <returns>Instance of <see cref="T"/></returns>
        public T Get<T>(DatabaseTypes dbType, string connectionString)
        {
            RepositoryCreator repoCreator;
            if (!_repositoryCreators.TryGetValue(typeof(T), out repoCreator))
            {
                throw new Exception("Interface not registered.");
            }

            if (repoCreator != null && repoCreator.CreatorByConnectionString != null)
            {
                return (T)repoCreator.CreatorByConnectionString(dbType, connectionString);
            }

            return default(T);
        }

        /// <summary>
        /// Get instance of <see cref="T"/> using DbOperations
        /// </summary>
        /// <param name="dbOperations">The DbOperations object for handling the database</param>
        /// <returns>Instance of <see cref="T"/></returns>
        public T Get<T>(DbOperations dbOperations)
        {
            RepositoryCreator repoCreator;
            if (!_repositoryCreators.TryGetValue(typeof(T), out repoCreator))
            {
                throw new Exception("Interface not registered.");
            }

            if (repoCreator != null && repoCreator.CreatorByDbOperations != null)
            {
                return (T)repoCreator.CreatorByDbOperations(dbOperations);
            }

            return default(T);
        }

        #endregion

        #region Private methods

        private void AddRepoCreator<T>(RepositoryCreator creator)
        {
            _repositoryCreators.Add(typeof(T), creator);
        }

        #endregion

        #region Nested type: RepositoryCreator

        private class RepositoryCreator
        {
            public Func<DatabaseTypes, string, object> CreatorByConnectionString { get; set; }
            public Func<DbOperations, object> CreatorByDbOperations { get; set; }

        }

        #endregion
    }
}