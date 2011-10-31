using System;
using System.Collections.Generic;
using HydroDesktop.Database;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.Interfaces;

namespace Search3.Searching
{
    /// <summary>
    /// Searches in the 'MetadataCache' database
    /// use this class when the "Metadata Cache" search
    /// option is selected
    /// </summary>
    public class MetadataCacheSearcher : SeriesSearcher
    {
        #region Fields

        private readonly MetadataCacheManagerSQL _db;

        #endregion

        #region Constructors

        public MetadataCacheSearcher()
        {
            _db = new MetadataCacheManagerSQL(DatabaseTypes.SQLite, HydroDesktop.Configuration.Settings.Instance.MetadataCacheConnectionString);
        }

        #endregion

        /// <summary>
        /// Get a list of all keywords from all web services in the metadata cache db
        /// </summary>
        public IList<string> GetKeywords()
        {
            return _db.GetVariableNames();
        }

        /// <summary>
        /// Get a list of all web services registered in the metadata cache database
        /// </summary>
        public IList<DataServiceInfo> GetWebServices()
        {
            return _db.GetAllServices();
        }

        protected override IEnumerable<SeriesDataCart> GetSeriesCatalogForBox(double xMin, double xMax, double yMin, double yMax, string keyword,
            DateTime startDate, DateTime endDate, int[] networkIDs)
        {
            return _db.GetSeriesListInBox(xMin, xMax, yMin, yMax, new []{keyword}, startDate, endDate, networkIDs);
        }
    }
}
