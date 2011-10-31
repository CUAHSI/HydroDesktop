using System;
using System.Collections.Generic;
using System.Linq;
using DotSpatial.Data;
using DotSpatial.Topology;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices;
using Search3.Settings;

namespace Search3.Searching
{
    public abstract class SeriesSearcher
    {
        public SearchResult GetSeriesCatalogInRectangle(Box extentBox, string[] keywords, double tileWidth, double tileHeight,
                                                        DateTime startDate, DateTime endDate, WebServiceNode[] serviceIDs, IProgressHandler bgWorker)
        {
            if (extentBox == null) throw new ArgumentNullException("extentBox");
            if (serviceIDs == null) throw new ArgumentNullException("serviceIDs");
            if (bgWorker == null) throw new ArgumentNullException("bgWorker");

            if (keywords == null || keywords.Length == 0)
            {
                keywords = new[] { String.Empty };
            }

            bgWorker.CheckForCancel();
            
            var fullSeriesList = new List<SeriesDataCart>();
            var tiles = SearchHelper.CreateTiles(extentBox, tileWidth, tileHeight);
            for (int i = 0; i < tiles.Count; i++)
            {
                var tile = tiles[i];

                bgWorker.CheckForCancel();

                // Do the web service call
                var tileSeriesList = new List<SeriesDataCart>();
                foreach (var keyword in keywords)
                {
                    bgWorker.ReportMessage(string.Format("Retreiving series from server. Keyword: {0}. Tile: {1} of {2}", keyword, i + 1, tiles.Count));
                    bgWorker.CheckForCancel();
                    tileSeriesList.AddRange(GetSeriesCatalogForBox(tile.XMin, tile.XMax, tile.YMin, tile.YMax, keyword,
                                                                   startDate, endDate, serviceIDs.Select(item => Convert.ToInt32(item.ServiceID)).ToArray()));
                }

                fullSeriesList.AddRange(tileSeriesList);

                // Report progress
                var message = string.Format("{0} Series found", fullSeriesList.Count.ToString());
                var percentProgress = (i * 100) / tiles.Count + 1;
                bgWorker.ReportProgress(percentProgress, message);
            }

            //(4) Create the Feature Set
            SearchResult resultFs = null;
            if (fullSeriesList.Count > 0)
            {
                bgWorker.ReportMessage("Calculating Points...");
                resultFs = SearchHelper.ToFeatureSetsByDataSource(fullSeriesList);
            }

            // (5) Final Background worker updates
            bgWorker.CheckForCancel();

            // Report progress
            bgWorker.ReportProgress(100, "Search Finished.");
            return resultFs;
        }

        public SearchResult GetSeriesCatalogInPolygon(IList<IFeature> polygons, string[] keywords, double tileWidth, double tileHeight,
                                                      DateTime startDate, DateTime endDate, WebServiceNode[] serviceIDs, IProgressHandler bgWorker)
        {
            if (polygons == null) throw new ArgumentNullException("polygons");
            if (bgWorker == null) throw new ArgumentNullException("bgWorker");

            //(1): Get the union of the polygons
            if (polygons.Count == 0)
            {
                throw new ArgumentException("The number of polygons must be greater than zero.");
            }

            if (keywords == null || keywords.Length == 0)
            {
                keywords = new[] { String.Empty };
            }

            // Check for cancel
            bgWorker.CheckForCancel();

            //get the list of series
            var fullSeriesList = new List<SeriesDataCart>();
            for (int index = 0; index < polygons.Count; index++)
            {
                if (polygons.Count > 1)
                {
                    bgWorker.ReportMessage(string.Format("Processing polygons: {0} of {1}", index + 1, polygons.Count));
                }

                var polygon = polygons[index];
                var env = polygon.Envelope; //Split the polygon area bounding box into 1x1 decimal degree tiles
                var extentBox = new Box(env.Left(), env.Right(), env.Bottom(), env.Top());
                var tiles = SearchHelper.CreateTiles(extentBox, tileWidth, tileHeight);
                for (int i = 0; i < tiles.Count; i++)
                {
                    var tile = tiles[i];

                    bgWorker.CheckForCancel();

                    // Do the web service call
                    var tileSeriesList = new List<SeriesDataCart>();
                    foreach (var keyword in keywords)
                    {
                        bgWorker.ReportMessage(
                            string.Format("Retreiving series from server. Keyword: {0}. Tile: {1} of {2}", keyword,
                                          i + 1, tiles.Count));
                        bgWorker.CheckForCancel();
                        tileSeriesList.AddRange(GetSeriesCatalogForBox(tile.XMin, tile.XMax, tile.YMin, tile.YMax,
                                                                       keyword, startDate, endDate, serviceIDs.Select(item => Convert.ToInt32(item.ServiceID)).ToArray()));
                    }

                    // Clip the points by polygon
                    var seriesInPolygon = SearchHelper.ClipByPolygon(tileSeriesList, polygon);
                    fullSeriesList.AddRange(seriesInPolygon);

                    // Report progress
                    var message = string.Format("{0} Series found", fullSeriesList.Count.ToString());
                    var percentProgress = (i * 100) / tiles.Count + 1;
                    bgWorker.ReportProgress(percentProgress, message);
                }
            }

            //(4) Create the Feature Set
            SearchResult resultFs = null;
            if (fullSeriesList.Count > 0)
            {
                bgWorker.ReportMessage("Calculating Points...");
                resultFs = SearchHelper.ToFeatureSetsByDataSource(fullSeriesList);
            }

            // (5) Final Background worker updates
            bgWorker.CheckForCancel();
            bgWorker.ReportProgress(100, "Search Finished.");
            return resultFs;
        }

        /// <summary>
        /// Gets all data series within the geographic bounding box that match the
        /// specified criteria
        /// </summary>
        /// <param name="xMin">minimum x (longitude)</param>
        /// <param name="xMax">maximum x (longitude)</param>
        /// <param name="yMin">minimum y (latitude)</param>
        /// <param name="yMax">maximum y (latitude)</param>
        /// <param name="keyword">the concept keyword. If set to null,
        /// results will not be filtered by concept keyword</param>
        /// <param name="startDate">start date. If set to null, results will not be filtered by start date.</param>
        /// <param name="endDate">end date. If set to null, results will not be filtered by end date.</param>
        /// <param name="networkIDs">array of serviceIDs provided by GetServicesInBox.
        /// If set to null, results will not be filtered by web service.</param>
        /// <returns>A list of data series matching the specified criteria</returns>
        protected abstract IEnumerable<SeriesDataCart> GetSeriesCatalogForBox(double xMin, double xMax, double yMin, double yMax, 
                                                                              string keyword, DateTime startDate, DateTime endDate,
                                                                              int[] networkIDs);
    }
}