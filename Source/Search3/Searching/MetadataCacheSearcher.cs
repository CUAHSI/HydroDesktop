using System;
using System.Collections.Generic;
using HydroDesktop.Database;
using HydroDesktop.Interfaces.ObjectModel;
using System.ComponentModel;
using DotSpatial.Data;
using HydroDesktop.WebServices;
using HydroDesktop.Interfaces;
using DotSpatial.Topology;
using Search3.Extensions;

namespace Search3.Searching
{
    /// <summary>
    /// Searches in the 'MetadataCache' database
    /// use this class when the "Metadata Cache" search
    /// option is selected
    /// </summary>
    public class MetadataCacheSearcher
    {
        //todo: Copied from Search2. Need to be refactored.

        public MetadataCacheSearcher()
        {
            string connString = HydroDesktop.Configuration.Settings.Instance.MetadataCacheConnectionString;
            _db = new MetadataCacheManagerSQL(DatabaseTypes.SQLite, connString);
        }
        
        private MetadataCacheManagerSQL _db;
        /// <summary>
        /// Setup the database connection
        /// </summary>
        public void SetupDB(string connString)
        {
            _db = new MetadataCacheManagerSQL(DatabaseTypes.SQLite, connString);
        }
        /// <summary>
        /// Get a list of all keywords available for a web service
        /// </summary>
        /// <param name="serviceID"></param>
        /// <returns></returns>
        public List<string> GetKeywords(int serviceID)
        {
            return _db.GetVariableNamesByService(serviceID) as List<string>;
        }
        /// <summary>
        /// Get a list of all keywords from all web services in the metadata cache db
        /// </summary>
        public List<string> GetKeywords()
        {
            return _db.GetVariableNames() as List<string>;
        }

        /// <summary>
        /// Get a list of all web services registered in the metadata cache database
        /// </summary>
        public List<DataServiceInfo> GetWebServices()
        {
            return _db.GetAllServices() as List<DataServiceInfo>;
        }

        /// <summary>
        /// Get web service registered in the metadata cache database
        /// </summary>
        /// <param name="serviceURL">Service Url</param>
        /// <returns>Web service</returns>
        public DataServiceInfo GetWebServiceByServiceURL(string serviceURL)
        {
            return _db.GetServiceByServiceUrl(serviceURL);
        }

        /// <summary>
        /// Gets all data series within the geographic bounding box that match the
        /// specified criteria
        /// </summary>
        /// <param name="xMin">minimum x (longitude)</param>
        /// <param name="xMax">maximum x (longitude)</param>
        /// <param name="yMin">minimum y (latitude)</param>
        /// <param name="yMax">maximum y (latitude)</param>
        /// <param name="keywords">the array of concept keywords. If set to null,
        /// results will not be filtered by concept keyword</param>
        /// <param name="startDate">start date. If set to null, results will not be filtered by start date.</param>
        /// <param name="endDate">end date. If set to null, results will not be filtered by end date.</param>
        /// <param name="networkIDs">array of serviceIDs provided by GetServicesInBox.
        /// If set to null, results will not be filtered by web service.</param>
        /// <returns>A list of data series matching the specified criteria</returns>
        public IList<SeriesDataCart> GetSeriesCatalogForBox(double xMin, double xMax, double yMin, double yMax, string[] keywords,
            DateTime startDate, DateTime endDate, int[] networkIDs)
        {
            return _db.GetSeriesListInBox(xMin, xMax, yMin, yMax, keywords, startDate, endDate, networkIDs);
        }
      

        


        /// <summary>
        /// Gets all search result that match the
        /// specified criteria and are within the specific rectangle
        /// </summary>
        /// <param name="polygons">one or multiple polygons</param>
        /// <param name="keywords">array of keywords. If set to null,
        /// results will not be filtered by keyword.</param>
        /// <param name="startDate">start date. If set to null, results will not be filtered by start date.</param>
        /// <param name="endDate">end date. If set to null, results will not be filtered by end date.</param>
        /// <param name="serviceIDs">array of serviceIDs provided by GetServicesInBox.
        /// <param name="worker">The background worker for reporting progress</param>
        /// <param name="e">The results of the search (convert to DataTable)</param>
        /// If set to null, results will not be filtered by web service.</param>
        /// <returns>A list of data series matching the specified criteria</returns>
        public void GetSeriesCatalogInRectangle(double xMin, double xMax, double yMin, double yMax, string[] keywords,
            DateTime startDate, DateTime endDate, int[] serviceIDs, IProgressHandler bgWorker, DoWorkEventArgs e)
        {
            double tileWidth = 2.0; //the initial tile width is set to 2 degree
            double tileHeight = 1.5; //the initial tile height is set to 1.5 degree

            //determine if to use the background worker for progress reporting
            //and for cancellation
            bool useWorker = true;
            bgWorker.CheckForCancel();


            //get the list of series
            var fullSeriesList = new List<SeriesDataCart>();

            //Split the polygon area bounding box into 1x1 decimal degree tiles

            Box extentBox = new Box(xMin, xMax, yMin, yMax);
            IList<Box> tiles = SearchHelper.CreateTiles(extentBox, tileWidth, tileHeight);
            int numTiles = tiles.Count;

            for (int i = 0; i < numTiles; i++)
            {
                Box tile = tiles[i];

                bgWorker.CheckForCancel();

                // Do the web service call
                //IList<SeriesDataCart> tileSeriesList = new List<SeriesMetadata>();
                IList<SeriesDataCart> tileSeriesList = GetSeriesCatalogForBox(tile.xmin, tile.xmax, tile.ymin, tile.ymax, keywords, startDate, endDate, serviceIDs);

                fullSeriesList.AddRange(tileSeriesList);

                // Report progress
                if (useWorker == true)
                {
                    var message = string.Format("{0} Series found", fullSeriesList.Count.ToString());
                    int percentProgress = (i * 100) / numTiles + 1;
                    bgWorker.ReportProgress(percentProgress, message);
                }
            }

            //(4) Create the Feature Set
            SearchResult resultFs = null;
            if (fullSeriesList.Count > 0 & useWorker)
            {
                bgWorker.ReportProgress(0, "Calculating Points");
                resultFs = SearchHelper.ToFeatureSetsByDataSource(fullSeriesList);
            }

            // (5) Final Background worker updates
            if (useWorker && e != null)
            {
                bgWorker.CheckForCancel();

                // Report progress
                bgWorker.ReportProgress(100, "Search Finished");
                e.Result = resultFs;
            }  
        }

        /// <summary>
        /// Gets all data series that match the
        /// specified criteria and are within the geographic polygon
        /// </summary>
        /// <param name="polygons">one or multiple polygons</param>
        /// <param name="keywords">array of keywords. If set to null,
        /// results will not be filtered by keyword.</param>
        /// <param name="startDate">start date. If set to null, results will not be filtered by start date.</param>
        /// <param name="endDate">end date. If set to null, results will not be filtered by end date.</param>
        /// <param name="serviceIDs">array of serviceIDs provided by GetServicesInBox.
        /// <param name="worker">The background worker for reporting progress</param>
        /// <param name="e">The results of the search (convert to DataTable)</param>
        /// If set to null, results will not be filtered by web service.</param>
        /// <returns>A list of data series matching the specified criteria</returns>
        public void GetSeriesCatalogInPolygon(IList<IFeature> polygons, string[] keywords, DateTime startDate,
            DateTime endDate, int[] serviceIDs, IProgressHandler bgWorker, DoWorkEventArgs e)
        {
            if (bgWorker == null) throw new ArgumentNullException("bgWorker");


            double tileWidth = 2.0; //the initial tile width is set to 1 degree
            double tileHeight = 2.0; //the initial tile height is set to 1 degree

            //determine if to use the background worker for progress reporting
            //and for cancellation
            bool useWorker = true;

            //(1): Get the union of the polygons
            if (polygons.Count == 0)
            {
                throw new ArgumentException("The number of polygons must be greater than zero.");
            }

            bgWorker.CheckForCancel();

            if (polygons.Count > 1 & useWorker == true)
            {
                bgWorker.ReportProgress(0, "Processing Polygons");
            }

            //get the list of series
            var fullSeriesList = new List<SeriesDataCart>();

            foreach (IFeature polygon in polygons)
            {
                //Split the polygon area bounding box into 1x1 decimal degree tiles
                IEnvelope env = polygon.Envelope;
                Box extentBox = new Box(env.Left(), env.Right(), env.Bottom(), env.Top());
                IList<Box> tiles = SearchHelper.CreateTiles(extentBox, tileWidth, tileHeight);
                int numTiles = tiles.Count;


                for (int i = 0; i < numTiles; i++)
                {
                    Box tile = tiles[i];

                    bgWorker.CheckForCancel();

                    // Do the web service call
                    IList<SeriesDataCart> tileSeriesList = GetSeriesCatalogForBox(tile.xmin, tile.xmax, tile.ymin, tile.ymax, keywords, startDate, endDate, serviceIDs);

                    // Clip the points by polygon
                    IEnumerable<SeriesDataCart> seriesInPolygon = SearchHelper.ClipByPolygon(tileSeriesList, polygon);

                    fullSeriesList.AddRange(seriesInPolygon);

                    // Report progress
                    if (useWorker == true)
                    {
                        var message = string.Format("{0} Series found", fullSeriesList.Count.ToString());
                        int percentProgress = (i * 100) / numTiles + 1;
                        bgWorker.ReportProgress(percentProgress, message);
                    }
                }
            }

            //(4) Create the Feature Set
            SearchResult resultFs = null;
            if (fullSeriesList.Count > 0 & useWorker)
            {
                bgWorker.ReportProgress(0, "Calculating Points");
                resultFs = SearchHelper.ToFeatureSetsByDataSource(fullSeriesList);
            }

            // (5) Final Background worker updates
            if (useWorker && e != null)
            {
                bgWorker.CheckForCancel();

                // Report progress
                bgWorker.ReportProgress(100, "Search Finished");
                e.Result = resultFs;
            }
        }
    }
}
