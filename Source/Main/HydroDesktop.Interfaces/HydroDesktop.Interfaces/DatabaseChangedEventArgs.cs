using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroDesktop.Interfaces
{
    /// <summary>
    /// information about the changed database
    /// </summary>
    public class DatabaseChangedEventArgs : EventArgs
    {
        public string ConnectionString;
        public DatabaseTypes DatabaseType;
    }
}
