using System;
using System.Collections.Generic;
using System.Data;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Non-generic interface for repositories
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Get all data from current repository as DataTable
        /// </summary>
        /// <returns>DataTable with all data.</returns>
        DataTable AsDataTable();

        /// <summary>
        /// Get the next auto-incremented (primary key) ID
        /// </summary>
        /// <returns>ID</returns>
        [Obsolete("Don't use this method. It added only for backward compatibility with DbOperations.")]
        long GetNextID();

        /// <summary>
        /// Check that entity exists by given key
        /// </summary>
        /// <param name="key">Entity ID (key)</param>
        /// <returns>True - entity exists, otherwise - false.</returns>
        bool Exists(object key);
    }

    /// <summary>
    /// Generic interface for repositories
    /// </summary>
    /// <typeparam name="T">T of entity</typeparam>
    public interface IRepository<T> : IRepository where T : BaseEntity
    {
        /// <summary>
        /// Get all data from current repository as array of entities.
        /// </summary>
        /// <returns>List of entities.</returns>
        IList<T> GetAll();

        /// <summary>
        /// Get entity by key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Entity or null, if it not found.</returns>
        T GetByKey(object key);
    }
}