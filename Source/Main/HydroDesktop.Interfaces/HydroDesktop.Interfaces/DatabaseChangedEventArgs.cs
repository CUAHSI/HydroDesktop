using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroDesktop.Interfaces
{
    public class DatabaseChangedEventArgs : EventArgs
    {
        public string ConnectionString;
        public DatabaseTypes DatabaseType;
    }
}
