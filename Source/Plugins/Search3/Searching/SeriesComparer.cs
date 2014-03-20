using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.Interfaces.ObjectModel;
using System.Diagnostics;

namespace Search3.Searching
{
    class SeriesComparer : IEqualityComparer<SeriesDataCart>
    {

        public bool Equals(SeriesDataCart x, SeriesDataCart y)
        {
            if (x.SiteName.Equals(y.SiteName))
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(SeriesDataCart obj)
        {
            return obj.SiteName.GetHashCode();
        }
    }
}
