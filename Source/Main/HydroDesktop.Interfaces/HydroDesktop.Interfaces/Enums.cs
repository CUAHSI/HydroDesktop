using System;
using System.Collections.Generic;
using System.Text;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// The database types supported by HydroDesktop.
    /// Currently, only SQLite is supported.
    /// </summary>
    public enum DatabaseTypes
    {
        SQLite,
        SQLServer,
        Unknown
    }

    /// <summary>
    /// The overwrite option when saving data to the database
    /// </summary>
    public enum OverwriteOptions
    {
        /// <summary>
        /// All duplicate values in the series are overwritten by new data values
        /// </summary>
        Overwrite,
        /// <summary>
        /// No data values in the original series are replaced by the new data values
        /// </summary>
        Fill,
        /// <summary>
        /// Only data values recorded after the End date of the original series are
        /// added to the series
        /// </summary>
        Append,
        /// <summary>
        /// A completely new series is created in the database. The site code and variable
        /// code of the new series is the same as the original series, but the DateCreated,
        /// SeriesID and Data Values are different. This is like creating a new version of
        /// the series.
        /// </summary>
        Copy
    }
}
