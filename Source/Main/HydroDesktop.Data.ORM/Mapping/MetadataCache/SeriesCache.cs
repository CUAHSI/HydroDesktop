using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.ObjectModel;

namespace HydroDesktop.Database.Map.MetadataCache
{
    /// <summary>
    /// Extends the series with DataService information
    /// </summary>
    public class SeriesCache : Series
    {
        public virtual DataServiceInfo DataService { get; set; }
    }
}
