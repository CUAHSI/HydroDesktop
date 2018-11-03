using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using HydroDesktop.Common;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.Plugins.Search.Searching
{
    /// <summary>
    /// Search for data series using HIS Central 
    /// </summary>
    public class HISCentralSearcher : SeriesSearcher
    {
        #region Fields

        private readonly string _hisCentralUrl;
        private static readonly CultureInfo _invariantCulture = CultureInfo.InvariantCulture;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new HIS Central Searcher which connects to the HIS Central web
        /// services
        /// </summary>
        /// <param name="hisCentralUrl">The URL of HIS Central</param>
        public HISCentralSearcher(string hisCentralUrl)
        {
            hisCentralUrl = hisCentralUrl.Trim();
            if (hisCentralUrl.EndsWith("?WSDL", StringComparison.OrdinalIgnoreCase))
            {
                hisCentralUrl = hisCentralUrl.ToUpperInvariant().Replace("?WSDL", "");
            }
            _hisCentralUrl = hisCentralUrl;
        }

        #endregion

        #region Public methods

        public void GetWebServicesXml(string xmlFileName)
        {
            HttpWebResponse response = null;
            try
            {
                var url = _hisCentralUrl + "/GetWaterOneFlowServiceInfo";

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
                        var buffer = new byte[255];
                        int bytesRead;
                        while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            localFileStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        #endregion

        #region Private methods

        protected override IEnumerable<SeriesDataCart> GetSeriesCatalogForBox(double xMin, double xMax, double yMin,
                                                                              double yMax, string keyword,
                                                                              DateTime startDate, DateTime endDate,
                                                                              int[] networkIDs,
                                                                              IProgressHandler bgWorker, long currentTile, long totalTilesCount)
        {
            var url = new StringBuilder();
            url.Append(_hisCentralUrl);
            url.Append("/GetSeriesCatalogForBox2");
            url.Append("?xmin=");
            url.Append(Uri.EscapeDataString(xMin.ToString(_invariantCulture)));
            url.Append("&xmax=");
            url.Append(Uri.EscapeDataString(xMax.ToString(_invariantCulture)));
            url.Append("&ymin=");
            url.Append(Uri.EscapeDataString(yMin.ToString(_invariantCulture)));
            url.Append("&ymax=");
            url.Append(Uri.EscapeDataString(yMax.ToString(_invariantCulture)));

            //to append the keyword
            url.Append("&conceptKeyword=");
            if (!String.IsNullOrEmpty(keyword))
            {
                url.Append(Uri.EscapeDataString(keyword));
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
                url.Append(Uri.EscapeDataString(serviceParam.ToString()));
            }

            //to append the start and end date
            url.Append("&beginDate=");
            url.Append(Uri.EscapeDataString(startDate.ToString("MM/dd/yyyy")));
            url.Append("&endDate=");
            url.Append(Uri.EscapeDataString(endDate.ToString("MM/dd/yyyy")));

            var keywordDesc = string.Format("[{0}. Tile {1}/{2}]",
                                            String.IsNullOrEmpty(keyword) ? "All" : keyword, currentTile,
                                            totalTilesCount);

            // Try to send request several times (in case, when server returns timeout)
            const int tryCount = 5;
            for (int i = 0; i < tryCount; i++)
            {
                try
                {
                    bgWorker.CheckForCancel();
                    bgWorker.ReportMessage(i == 0
                                               ? string.Format("Sent request: {0}", keywordDesc)
                                               : string.Format("Timeout has occurred for {0}. New Attempt ({1} of {2})...",
                                                   keywordDesc, i + 1, tryCount));

                    var request = WebRequest.Create(url.ToString());
                    request.Timeout = 30 * 1000;
                    using (var response = request.GetResponse())
                    using (var reader = XmlReader.Create(response.GetResponseStream()))
                    {
                        bgWorker.ReportMessage(string.Format("Data received for {0}", keywordDesc));
                        return ParseSeries(reader, startDate, endDate);
                    }    
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.Timeout)
                    {
                        continue;
                    }
                    throw;
                }
            }
            throw new WebException("Timeout. Try to decrease Search Area, or Select another Keywords.", WebExceptionStatus.Timeout);
        }

        private IEnumerable<SeriesDataCart> ParseSeries(XmlReader reader, DateTime startDate, DateTime endDate)
        {
            var seriesList = new List<SeriesDataCart>();
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
                            SearchHelper.UpdateDataCartToDateInterval(series, startDate, endDate);
                            seriesList.Add(series);
                        }
                    }
                }
            }
            return seriesList;
        }

        /// <summary>
        /// Read the list of series from the XML that is returned by HIS Central
        /// </summary>
        /// <param name="reader">the xml reader</param>
        /// <returns>the list of intermediate 'SeriesDataCart' objects</returns>
        private SeriesDataCart ReadSeriesFromHISCentral(XmlReader reader)
        {
            var series = new SeriesDataCart();
            while (reader.Read())
            {
                var nodeName = reader.Name.ToLower();
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (nodeName)
                    {
                        case "servcode":
                            reader.Read();
                            series.ServCode = reader.Value;
                            break;
                        case "servurl":
                            reader.Read();
                            series.ServURL = reader.Value;
                            break;
                        case "location":
                            reader.Read();
                            series.SiteCode = reader.Value;
                            break;
                        case "varcode":
                            reader.Read();
                            series.VariableCode = reader.Value;
                            break;
                        case "varname":
                            reader.Read();
                            series.VariableName = reader.Value;
                            break;
                        case "begindate":
                            reader.Read();
                            if (!String.IsNullOrWhiteSpace(reader.Value))
                                series.BeginDate = Convert.ToDateTime(reader.Value, _invariantCulture);
                            else
                                return null;
                            break;
                        case "enddate":
                            reader.Read();
                            if (!String.IsNullOrWhiteSpace(reader.Value))
                                series.EndDate = Convert.ToDateTime(reader.Value, _invariantCulture);
                            else
                                return null;
                            break;
                        case "valuecount":
                            reader.Read();
                            if (!String.IsNullOrWhiteSpace(reader.Value))
                                series.ValueCount = Convert.ToInt32(reader.Value);
                            else
                                return null;
                            break;
                        case "sitename":
                            reader.Read();
                            series.SiteName = reader.Value;
                            break;
                        case "latitude":
                            reader.Read();
                            if (!String.IsNullOrWhiteSpace(reader.Value))
                                series.Latitude = Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);
                            else
                                return null;
                            break;
                        case "longitude":
                            reader.Read();
                            if (!String.IsNullOrWhiteSpace(reader.Value))
                                series.Longitude = Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);
                            else
                                return null;
                            break;
                        case "datatype":
                            reader.Read();
                            series.DataType = reader.Value;
                            break;
                        case "valuetype":
                            reader.Read();
                            series.ValueType = reader.Value;
                            break;
                        case "samplemedium":
                            reader.Read();
                            series.SampleMedium = reader.Value;
                            break;
                        case "timeunits":
                            reader.Read();
                            series.TimeUnit = reader.Value;
                            break;
                        case "conceptkeyword":
                            reader.Read();
                            series.ConceptKeyword = reader.Value;
                            break;
                        case "gencategory":
                            reader.Read();
                            series.GeneralCategory = reader.Value;
                            break;
                        case "timesupport":
                            reader.Read();
                            if (!String.IsNullOrWhiteSpace(reader.Value))
                                series.TimeSupport = Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);
                            break;
                        case "isregular":
                            reader.Read();
                            if (!String.IsNullOrWhiteSpace(reader.Value))
                                series.IsRegular = Convert.ToBoolean(reader.Value);
                            break;
                        case "variableunits":
                            reader.Read();
                            series.VariableUnits = reader.Value;
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && nodeName == "seriesrecord")
                {
                    return series;
                }
            }

            return null;
        }

        #endregion
    }
}
