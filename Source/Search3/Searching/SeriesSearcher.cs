using System;
using System.Collections.Generic;
using System.Linq;
using DotSpatial.Data;
using DotSpatial.Topology;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices;
using Search3.Settings;
using IProgressHandler = HydroDesktop.Common.IProgressHandler;

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
            var extent = new Extent(extentBox.XMin, extentBox.YMin, extentBox.XMax, extentBox.YMax);
            var fullSeriesList = GetSeriesListForExtent(extent, keywords, tileWidth, tileHeight, startDate, endDate,
                                                        serviceIDs, bgWorker, series => true);
            SearchResult resultFs = null;
            if (fullSeriesList.Count > 0)
            {
                bgWorker.ReportMessage("Calculating Points...");
                resultFs = SearchHelper.ToFeatureSetsByDataSource(fullSeriesList);
            }
            
            bgWorker.CheckForCancel();
            bgWorker.ReportProgress(100, "Search Finished.");
            return resultFs;
        }
        
        public SearchResult GetSeriesCatalogInPolygon(IList<IFeature> polygons, string[] keywords, double tileWidth, double tileHeight,
                                                      DateTime startDate, DateTime endDate, WebServiceNode[] serviceIDs, IProgressHandler bgWorker)
        {
            if (polygons == null) throw new ArgumentNullException("polygons");
            if (bgWorker == null) throw new ArgumentNullException("bgWorker");
            if (polygons.Count == 0)
            {
                throw new ArgumentException("The number of polygons must be greater than zero.");
            }

            if (keywords == null || keywords.Length == 0)
            {
                keywords = new[] { String.Empty };
            }
            
            var fullSeriesList = new List<SeriesDataCart>();
            for (int index = 0; index < polygons.Count; index++)
            {
                if (polygons.Count > 1)
                {
                    bgWorker.ReportMessage(string.Format("Processing polygons: {0} of {1}", index + 1, polygons.Count));
                }

                bgWorker.CheckForCancel();
                var polygon = polygons[index];
                var extentBox = new Extent(polygon.Envelope);
                var seriesForPolygon = GetSeriesListForExtent(extentBox, keywords, tileWidth, tileHeight, startDate,
                                                              endDate,
                                                              serviceIDs, bgWorker,
                                                              item => polygon.Intersects(new Coordinate(item.Longitude, item.Latitude)));
                fullSeriesList.AddRange(seriesForPolygon);
            }

            SearchResult resultFs = null;
            if (fullSeriesList.Count > 0)
            {
                bgWorker.ReportMessage("Calculating Points...");
                resultFs = SearchHelper.ToFeatureSetsByDataSource(fullSeriesList);
            }
            
            bgWorker.CheckForCancel();
            bgWorker.ReportProgress(100, "Search Finished.");
            return resultFs;
        }

        private List<SeriesDataCart> GetSeriesListForExtent(Extent extent, string[] keywords, double tileWidth, double tileHeight,
                                                      DateTime startDate, DateTime endDate, WebServiceNode[] serviceIDs, 
                                                      IProgressHandler bgWorker, Func<SeriesDataCart, bool> seriesFilter)
        {
            var servicesToSearch = new List<Tuple<WebServiceNode[], Extent>>();
            if (serviceIDs.Length > 0)
            {
                foreach (var webService in serviceIDs)
                {
                    if (webService.ServiceBoundingBox == null)
                    {
                        servicesToSearch.Add(new Tuple<WebServiceNode[], Extent>(new[] { webService }, extent));
                        continue;
                    }
                    var eps = 0.05; //tolerance (0.05 deg) used for services whose bounding box is one point
                    var wsBox = webService.ServiceBoundingBox;
                    var wsExtent = new Extent(wsBox.XMin - eps, wsBox.YMin - eps, wsBox.XMax + eps, wsBox.YMax + eps);
                    if (wsExtent.Intersects(extent))
                    {
                        servicesToSearch.Add(new Tuple<WebServiceNode[], Extent>(new[] { webService }, wsExtent.Intersection(extent)));
                    }
                }
            }
            else
            {
                servicesToSearch.Add(new Tuple<WebServiceNode[], Extent>(new WebServiceNode[] { }, extent));
            }

            var servicesWithExtents = new List<Tuple<WebServiceNode[], List<Extent>>>(servicesToSearch.Count);
            int totalTilesCount = 0;
            foreach (var wsInfo in servicesToSearch)
            {
                var tiles = SearchHelper.CreateTiles(wsInfo.Item2, tileWidth, tileHeight);
                servicesWithExtents.Add(new Tuple<WebServiceNode[], List<Extent>>(wsInfo.Item1, tiles));
                totalTilesCount += tiles.Count;
            }

            var fullSeriesList = new List<SeriesDataCart>();
            int currentTileIndex = 0;
            foreach (var wsInfo in servicesWithExtents)
            {
                var webServices = wsInfo.Item1;
                var tiles = wsInfo.Item2;

                for (int i = 0; i < tiles.Count; i++, currentTileIndex++)
                {
                    var tile = tiles[i];

                    bgWorker.CheckForCancel();

                    // Do the web service call
                    var tileSeriesList = new List<SeriesDataCart>();
                    foreach (var keyword in keywords)
                    {
                        bgWorker.ReportMessage(
                            string.Format("Retrieving series from server. Keyword: {0}. Tile: {1} of {2}", keyword,
                                          currentTileIndex + 1, totalTilesCount));
                        bgWorker.CheckForCancel();
                        tileSeriesList.AddRange(GetSeriesCatalogForBox(tile.MinX, tile.MaxX, tile.MinY, tile.MaxY,
                                                                       keyword, startDate, endDate,
                                                                       webServices.Select(item => item.ServiceID).ToArray()));
                    }

                    fullSeriesList.AddRange(tileSeriesList.Where(seriesFilter));

                    // Report progress
                    var message = string.Format("{0} Series found", fullSeriesList.Count);
                    var percentProgress = (currentTileIndex * 100) / totalTilesCount + 1;
                    bgWorker.ReportProgress(percentProgress, message);
                }
            }

            return fullSeriesList;
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