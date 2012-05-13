using System;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// information about the changed database
    /// </summary>
    public class DatabaseChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Connection string of the new database
        /// </summary>
        public string ConnectionString;

        /// <summary>
        /// Database type of the new database
        /// </summary>
        public DatabaseTypes DatabaseType;
    }
}
