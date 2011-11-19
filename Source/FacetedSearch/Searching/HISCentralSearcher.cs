using System;
using System.Collections.Generic;
using System.Text;
using DotSpatial.Topology;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices;
using System.ComponentModel;
using DotSpatial.Data;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Web;
using System.Xml;
using FacetedSearch.Extensions;
using log4net;

namespace FacetedSearch.Searching
{
    //todo: Copied from Search2. Need to be refactored.

    public interface IHISCentralSearcher
    {
        /// <summary>
        /// Defined to make the class testable using MOQ
        /// Gets all data series that match the
        /// specified criteria and are within the geographic polygon
        /// </summary>
        /// <param name="polygons">one or multiple polygons</param>
        /// <param name="keywords">array of keywords. If set to null,
        /// results will not be filtered by keyword.</param>
        /// <param name="startDate">start date. If set to null, results will not be filtered by start date.</param>
        /// <param name="endDate">end date. If set to null, results will not be filtered by end date.</param>
        /// <param name="e">The results of the search (convert to DataTable)</param>
        /// <returns>A list of data series matching the specified criteria</returns>
        void GetSeriesCatalogInPolygon(IList<IFeature> polygons, string[] keywords, DateTime startDate,DateTime endDate, int[] serviceIDs, IProgressHandler bgWorker, DoWorkEventArgs e);
        void GetSeriesCatalogInRectangle(double xMin, double xMax, double yMin, double yMax, string[] keywords, DateTime startDate, DateTime endDate, int[] serviceIDs, IProgressHandler bgWorker, DoWorkEventArgs e);
        void GetWebServicesXml(string xmlFileName);
    }


  

    /// <summary>
    /// Search for data series using HIS Central
    /// 
    /// </summary>
    public class HISCentralSearcher :  IHISCentralSearcher
    {
        private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /* I (valentine) honestly have no idea why this cals exsits. Jiri added it
               * it did not inherit from HydroDesktop.Data.Search.HISCentralSearcher
               * even though it had many of the same methods.
               * I added HydroDesktop.Data.Search.HISCentralSearcher
               * */
        #region Constructor
       
        /// <summary>
        /// Create a new HIS Central Searcher which connects to the HIS Central web
        /// services
        /// </summary>
        /// <param name="hisCentralURL">The URL of HIS Central</param>
        public HISCentralSearcher(string hisCentralURL)
        {
            HISCentralUrl = hisCentralURL;
        }

        #endregion

        #region Fields

        private bool _usePagedQuery = false;
        
        #endregion

        #region ISearcher Members

        /// <summary>
        /// Gets or sets the HIS Central URL
        /// </summary>
        public string HISCentralUrl { get; set; }

        /// <summary>
        /// The query method for HIS Central. If use PagedQuery is true,
        /// then the new method 'getSeriesCatalogForBoxPaged()' is used.
        /// </summary>
        public bool UsePagedQuery
        {
            get
            {
                return _usePagedQuery;
            }
            set
            {
                _usePagedQuery = value;
            }
        }

        /// <summary>
        /// Get the 'Ontology Tree' and save it to the xml file
        /// </summary>
        public void GetOntologyTreeXml(string xmlFileName)
        {
            WebClient client = new WebClient();
            string url = HISCentralUrl + "/getOntologyTree?conceptKeyword=Hydrosphere";

            try
            {
                client.DownloadFile(url, xmlFileName);
            }
            catch (Exception ex)
            {
                String error ="Error refreshing Ontology keywords from HIS Central. Using the existing list of keywords.";
                log.Error(error, ex);
                MessageBox.Show(error);
            }
        }

        public void GetWebServicesXml(string xmlFileName)
        {
            HttpWebResponse response = null;
            int bytesRead = 0;
            try
            {
                string url = HISCentralUrl + "/GetWaterOneFlowServiceInfo";

                var request = (HttpWebRequest) WebRequest.Create(url);
                //Endpoint is the URL to which u are making the request.
                request.Method = "GET";
                request.Credentials = CredentialCache.DefaultCredentials;
                request.ContentType = "text/xml";

                request.Timeout = 5000;

                // send the request and get the response
                response = (HttpWebResponse) request.GetResponse();

                using (var responseStream = response.GetResponseStream())
                {
                    using (var localFileStream = new FileStream(xmlFileName, FileMode.Create))
                    {
                        byte[] buffer = new byte[255];
                        double totalBytesRead = 0;

                        while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            totalBytesRead += bytesRead;
                            localFileStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
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
        private IEnumerable<SeriesDataCart> GetSeriesCatalogForBox(double xMin, double xMax, double yMin, double yMax, string[] keywords,
            DateTime startDate, DateTime endDate, int[] networkIDs)
        {
            if (keywords == null)
            {
                return GetSeriesCatalogForBox(xMin, xMax, yMin, yMax, String.Empty, startDate, endDate, networkIDs);
            }
            if (keywords.Length == 0)
            {
                return GetSeriesCatalogForBox(xMin, xMax, yMin, yMax, String.Empty, startDate, endDate, networkIDs);
            }
            if (keywords.Length == 1)
            {
                return GetSeriesCatalogForBox(xMin, xMax, yMin, yMax, keywords[0], startDate, endDate, networkIDs);
            }
            var lst = new List<SeriesDataCart>();
            foreach (string keyword in keywords)
            {
                lst.AddRange(GetSeriesCatalogForBox(xMin, xMax, yMin, yMax, keyword, startDate, endDate, networkIDs));
            }
            return lst;
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
        private IEnumerable<SeriesDataCart> GetSeriesCatalogForBox(double xMin, double xMax, double yMin, double yMax, string keyword,
            DateTime startDate, DateTime endDate, int[] networkIDs)
        {
            //call the web service dynamically, using WebClient
            var usaFormat = new CultureInfo("en-US");

            var url = new StringBuilder();
            url.Append(HISCentralUrl);
            url.Append("/GetSeriesCatalogForBox2");
            url.Append("?xmin=");
            url.Append(HttpUtility.UrlEncode(xMin.ToString(usaFormat)));
            url.Append("&xmax=");
            url.Append(HttpUtility.UrlEncode(xMax.ToString(usaFormat)));
            url.Append("&ymin=");
            url.Append(HttpUtility.UrlEncode(yMin.ToString(usaFormat)));
            url.Append("&ymax=");
            url.Append(HttpUtility.UrlEncode(yMax.ToString(usaFormat)));
            
            //to append the keyword
            url.Append("&conceptKeyword=");
            if (!String.IsNullOrEmpty(keyword))
            {
                url.Append(HttpUtility.UrlEncode(keyword));    
            }
            
            //to append the list of networkIDs separated by comma
            url.Append("&networkIDs=");
            if (networkIDs != null)
            {
                var serviceParam = new StringBuilder();
                for (int i = 0; i < networkIDs.Length - 1; i++)
                {
                    serviceParam.Append(networkIDs[i]);
                    serviceParam.Append(",");
                }
                if (networkIDs.Length > 0)
                {
                    serviceParam.Append(networkIDs[networkIDs.Length - 1]);
                }
                url.Append(HttpUtility.UrlEncode(serviceParam.ToString()));
            }
            
            //to append the start and end date
            url.Append("&beginDate=");
            url.Append(HttpUtility.UrlEncode(startDate.ToString("MM/dd/yyyy")));
            url.Append("&endDate=");
            url.Append(HttpUtility.UrlEncode(endDate.ToString("MM/dd/yyyy")));
            
            //to encode the URL
            string finalURL = url.ToString();

            //to read the xml stream
            var seriesList = new List<SeriesDataCart>();
            using(var reader = XmlReader.Create(finalURL))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "SeriesRecord")
                        {
                            //Read the site information
                            var series = ReadSeriesFromHISCentral(reader);
                            if (series != null)
                            {
                                // Update BeginDate/EndDate/ValueCount to the user-specified range
                                var seriesStartDate = series.BeginDate < startDate ? startDate : series.BeginDate;
                                var seriesEndDate = series.EndDate > endDate ? endDate : series.EndDate;

                                var serverDateRange = series.EndDate.Subtract(series.BeginDate);
                                var userDateRange = seriesEndDate.Subtract(seriesStartDate);

                                var userFromServerPercentage = serverDateRange.TotalDays > 0
                                                                   ? userDateRange.TotalDays/serverDateRange.TotalDays
                                                                   : 1.0;
                                if (userFromServerPercentage > 1.0) 
                                    userFromServerPercentage = 1.0;
                                var esimatedValueCount = (int) (series.ValueCount*userFromServerPercentage);

                                series.ValueCount = esimatedValueCount;
                                series.BeginDate = seriesStartDate;
                                series.EndDate = seriesEndDate;
                                //---

                                seriesList.Add(series);
                            }
                        }
                    }
                }
            }

            return seriesList;
        }

        /// <summary>
        /// Read the list of series from the XML that is returned by HIS Central
        /// </summary>
        /// <param name="r">the xml reader</param>
        /// <returns>the list of intermediate 'SeriesDataCart' objects</returns>
        private SeriesDataCart ReadSeriesFromHISCentral(XmlReader r)
        {
            var usaCulture = new CultureInfo("en-US");
            
            var series = new SeriesDataCart();
            while (r.Read())
            {
                string nodeName = r.Name.ToLower();

                if (r.NodeType == XmlNodeType.Element)
                {

                    switch (nodeName)
                    {
                        case "servcode":
                            r.Read();
                            series.ServCode = r.Value;
                            break;
                        case "servurl":
                            r.Read();
                            series.ServURL = r.Value;
                            break;
                        case "location":
                            r.Read();
                            series.SiteCode = r.Value;
                            break;
                        case "varcode":
                            r.Read();
                            series.VariableCode = r.Value;
                            break;
                        case "varname":
                            r.Read();
                            series.VariableName = r.Value;
                            break;
                        case "begindate":
                            r.Read();
                            series.BeginDate = Convert.ToDateTime(r.Value, usaCulture);
                            break;
                        case "enddate":
                            r.Read();
                            series.EndDate = Convert.ToDateTime(r.Value, usaCulture);
                            break;
                        case "valuecount":
                            r.Read();
                            series.ValueCount = Convert.ToInt32(r.Value);
                            break;
                        case "sitename":
                            r.Read();
                            series.SiteName = r.Value;
                            break;
                        case "latitude":
                            r.Read();
                            series.Latitude = Convert.ToDouble(r.Value, CultureInfo.InvariantCulture);
                            break;
                        case "longitude":
                            r.Read();
                            series.Longitude = Convert.ToDouble(r.Value, CultureInfo.InvariantCulture);
                            break;
                        case "datatype":
                            r.Read();
                            series.DataType = r.Value;
                            break;
                        case "valuetype":
                            r.Read();
                            series.ValueType = r.Value;
                            break;
                        case "samplemedium":
                            r.Read();
                            series.SampleMedium = r.Value;
                            break;
                        case "timeunits":
                            r.Read();
                            series.TimeUnit = r.Value;
                            break;
                        case "conceptkeyword":
                            r.Read();
                            series.ConceptKeyword = r.Value;
                            break;
                        case "gencategory":
                            r.Read();
                            series.GeneralCategory = r.Value;
                            break;
                        case "timesupport":
                            r.Read();
                            series.TimeSupport = Convert.ToDouble(r.Value, CultureInfo.InvariantCulture);
                            break;     
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && nodeName == "seriesrecord")
                {
                    return series;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all search result that match the
        /// specified criteria and are within the specific rectangle
        /// </summary>
        /// <param name="keywords">array of keywords. If set to null,
        /// results will not be filtered by keyword.</param>
        /// <param name="startDate">start date. If set to null, results will not be filtered by start date.</param>
        /// <param name="endDate">end date. If set to null, results will not be filtered by end date.</param>
        /// <param name="e">The results of the search (convert to DataTable)</param>
        /// <returns>A list of data series matching the specified criteria</returns>
        public void GetSeriesCatalogInRectangle(double xMin, double xMax, double yMin, double yMax, string[] keywords,
            DateTime startDate, DateTime endDate, int[] serviceIDs, IProgressHandler bgWorker, DoWorkEventArgs e)
        {
            if (bgWorker == null) throw new ArgumentNullException("bgWorker");

            double tileWidth = 1.0; //the initial tile width is set to 1 degree
            double tileHeight = 1.0; //the initial tile height is set to 1 degree
            
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
                IEnumerable<SeriesDataCart> tileSeriesList = GetSeriesCatalogForBox(tile.XMin, tile.XMax, tile.YMin, tile.YMax, keywords, startDate, endDate, serviceIDs);

                fullSeriesList.AddRange(tileSeriesList);

                // Report progress
                {
                    string message = fullSeriesList.Count.ToString();
                    int percentProgress = (i * 100) / numTiles + 1;
                    bgWorker.ReportProgress(percentProgress, message);
                }
            }
            

            //(4) Create the Feature Set
            SearchResult resultFs = null;
            if (fullSeriesList.Count > 0)
            {
                bgWorker.ReportProgress(0, "Calculating Points");
                resultFs = SearchHelper.ToFeatureSetsByDataSource(fullSeriesList);
            }

            // (5) Final Background worker updates
            if (e != null)
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
        /// <param name="bgWorker">The background worker (may be null) for reporting progress</param>
        /// <param name="e">The results of the search (convert to DataTable)</param>
        /// If set to null, results will not be filtered by web service.</param>
        /// <returns>A list of data series matching the specified criteria</returns>
        public void GetSeriesCatalogInPolygon(IList<IFeature> polygons, string[] keywords, DateTime startDate,
            DateTime endDate, int[] serviceIDs, IProgressHandler bgWorker, DoWorkEventArgs e)
        {
            double tileWidth = 1.0; //the initial tile width is set to 1 degree
            double tileHeight = 1.0; //the initial tile height is set to 1 degree

            //(1): Get the union of the polygons
            if (polygons.Count == 0)
            {
                throw new ArgumentException("The number of polygons must be greater than zero.");
            }

            // Check for cancel
            bgWorker.CheckForCancel();

            if (polygons.Count > 1)
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
                    IEnumerable<SeriesDataCart> tileSeriesList = GetSeriesCatalogForBox(tile.XMin, tile.XMax, tile.YMin, tile.YMax, keywords, startDate, endDate, serviceIDs);

                    // Clip the points by polygon
                    IEnumerable<SeriesDataCart> seriesInPolygon = SearchHelper.ClipByPolygon(tileSeriesList, polygon);
                    
                    fullSeriesList.AddRange(seriesInPolygon);

                    // Report progress
                    {
                        string message = fullSeriesList.Count.ToString();
                        int percentProgress = (i * 100) / numTiles + 1;
                        bgWorker.ReportProgress(percentProgress, message);
                    }
                }
            }

            //(4) Create the Feature Set
            SearchResult resultFs = null;
            if (fullSeriesList.Count > 0)
            {
                bgWorker.ReportProgress(0, "Calculating Points");
                resultFs = SearchHelper.ToFeatureSetsByDataSource(fullSeriesList);
            }

            // (5) Final Background worker updates
            if (e != null)
            {
                bgWorker.CheckForCancel();

                // Report progress
                bgWorker.ReportProgress(100, "Search Finished");
                e.Result = resultFs;
            }  
        }

        #endregion
    }
}
