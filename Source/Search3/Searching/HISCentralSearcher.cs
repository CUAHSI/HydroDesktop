using System;
using System.Collections.Generic;
using System.Text;
using HydroDesktop.Interfaces.ObjectModel;
using System.Net;
using System.IO;
using System.Globalization;
using System.Web;
using System.Xml;

namespace Search3.Searching
{
    /// <summary>
    /// Search for data series using HIS Central 
    /// </summary>
    public class HISCentralSearcher : SeriesSearcher
    {
        #region Fields

        private readonly string _hisCentralUrl;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new HIS Central Searcher which connects to the HIS Central web
        /// services
        /// </summary>
        /// <param name="hisCentralURL">The URL of HIS Central</param>
        public HISCentralSearcher(string hisCentralURL)
        {
            _hisCentralUrl = hisCentralURL;
        }

        #endregion

        #region Public methods

        public void GetWebServicesXml(string xmlFileName)
        {
            HttpWebResponse response = null;
            try
            {
                string url = _hisCentralUrl + "/GetWaterOneFlowServiceInfo";

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
                                                                              int[] networkIDs)
        {
            //call the web service dynamically, using WebClient
            var usaFormat = new CultureInfo("en-US");

            var url = new StringBuilder();
            url.Append(_hisCentralUrl);
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
            using (var reader = XmlReader.Create(finalURL))
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
        /// <param name="reader">the xml reader</param>
        /// <returns>the list of intermediate 'SeriesDataCart' objects</returns>
        private SeriesDataCart ReadSeriesFromHISCentral(XmlReader reader)
        {
            var usaCulture = new CultureInfo("en-US");

            var series = new SeriesDataCart();
            while (reader.Read())
            {
                string nodeName = reader.Name.ToLower();

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
                            series.BeginDate = Convert.ToDateTime(reader.Value, usaCulture);
                            break;
                        case "enddate":
                            reader.Read();
                            series.EndDate = Convert.ToDateTime(reader.Value, usaCulture);
                            break;
                        case "valuecount":
                            reader.Read();
                            series.ValueCount = Convert.ToInt32(reader.Value);
                            break;
                        case "sitename":
                            reader.Read();
                            series.SiteName = reader.Value;
                            break;
                        case "latitude":
                            reader.Read();
                            series.Latitude = Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);
                            break;
                        case "longitude":
                            reader.Read();
                            series.Longitude = Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);
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
                            series.TimeSupport = Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);
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
