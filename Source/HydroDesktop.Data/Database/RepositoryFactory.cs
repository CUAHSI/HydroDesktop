using System;
using HydroDesktop.Configuration;
using HydroDesktop.Interfaces;
using System.Collections.Generic;

namespace HydroDesktop.Database
{
    /// <summary>
    /// Factory to get repositories
    /// </summary>
    public partial class RepositoryFactory
    {
        #region Fields

        private readonly Dictionary<Type, RepositoryCreator> _repositoryCreators;

        #endregion

        #region Constructors

        private RepositoryFactory()
        {
            _repositoryCreators = new Dictionary<Type, RepositoryCreator>();
            Register();
        }

        partial void Register();

        #endregion

        #region Singletone implementation

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
        /// Get instance of <see cref="T"/> using default settings
        /// </summary>
        /// <returns>Instance of <see cref="T"/></returns>
        public T Get<T>()
        {
            return Get<T>(DatabaseTypes.SQLite, Settings.Instance.DataRepositoryConnectionString);
        }

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
        public T Get<T>(IHydroDbOperations dbOperations)
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

        private void Add<T>(RepositoryCreator creator)
        {
            _repositoryCreators.Add(typeof(T), creator);
        }

        #endregion

        #region Nested type: RepositoryCreator

        private class RepositoryCreator
        {
            public Func<DatabaseTypes, string, object> CreatorByConnectionString { get; set; }
            public Func<IHydroDbOperations, object> CreatorByDbOperations { get; set; }

        }

        #endregion
    }
}