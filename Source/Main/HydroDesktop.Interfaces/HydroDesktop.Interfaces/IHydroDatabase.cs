using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// Provides access to the Cuahsi HydroDesktop DataRepository database
    /// </summary>
    public interface IHydroDatabase
    {
        /// <summary>
        /// Gets or sets the connection string for accessing the current
        /// HydroDesktop Data Repository database
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// The database type of the currently used data repository
        /// database (only SQLite is supported)
        /// </summary>
        DatabaseTypes DatabaseType { get; }

        /// <summary>
        /// When the database connection is changed (user chooses a new database
        /// or user opens a different project)
        /// </summary>
        event EventHandler DatabaseChanged;       

        /// <summary>
        /// The data repository manager. This class provides methods for insert and update
        /// operations and conversion between the object model classes and the data repository
        /// database tables
        /// </summary>
        IRepositoryManager RepositoryManager { get; }

        /// <summary>
        /// The database SQL based operations. This class provides methods for querying the database
        /// using standard SQL queries
        /// </summary>
        IHydroDbOperations DbOperations { get; }

        //event EventHandler SeriesChecked;

        //event EventHandler SeriesUnchecked;

        //DataTable LoadTable(string sqlQuery);
    }
}
