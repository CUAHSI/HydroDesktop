#region Namespace
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using HydroDesktop.Interfaces.ObjectModel;
#endregion


namespace HydroDesktop.WebServices
{
    /// <summary>
    /// This is a new version of the HIS Central Client class.
    /// This makes calls to web methods directly over HTTP Protocol.
    /// </summary>
    // todo: Not Used
    public class HISCentralClient2
    {
        #region Stopwatch
        Stopwatch watch1 = new Stopwatch();
        Stopwatch watch2 = new Stopwatch();
        #endregion

        #region Private Variables

        //the list of unique site / variable combinations
        private List<string> _uniqueCodeList = new List<string>();
        private string _asmxURL;

        const string soapEnvelopeHeader =
            @"<?xml version=""1.0"" encoding=""utf-8""?>" +
            @"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" " +
            @"xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" " +
            @"xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" +
            @"<soap:Body>";

        const string soapEnvelopeClosing =
            @"</soap:Body></soap:Envelope>";

        const string cuahsiXmlns = @"xmlns=""http://hiscentral.cuahsi.org/20100205/""";

        const string cuahsiSoapAction = @"http://hiscentral.cuahsi.org/20100205/";

        #endregion

        #region Properties
        /// <summary>
        /// Response time of the web service call
        /// </summary>
        public long ResponseTime
        {
            get
            {
                long elapsedTime =  watch1.ElapsedMilliseconds;
                watch1.Reset();
                return elapsedTime;
            }
        }
        #endregion

        /// <summary>
        /// Creates a new instance of HIS central web service client
        /// </summary>
        /// <param name="asmxURL">The url of the server with HIS Central web services</param>
        public HISCentralClient2(string asmxURL)
        {
            _asmxURL = asmxURL;
        }

        /// <summary>
        /// Returns a list of mapped variables
        /// </summary>
        /// <returns></returns>
        public DataTable GetMappedVariables()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the list of mapped variables as a XML string
        /// </summary>
        /// <returns></returns>
        public string GetMappedVariablesXml()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string array of searchable concept names
        /// </summary>
        /// <returns>the xml file with all concept keywords</returns>
        public void GetSearchableConcepts(string xmlFileName)
        {
            string soapAction = cuahsiSoapAction + "GetSearchableConcepts";

            StringBuilder soapEnvelope = new StringBuilder();
            soapEnvelope.Append(soapEnvelopeHeader);
            soapEnvelope.Append(@"<GetSearchableConcepts ");
            soapEnvelope.Append(cuahsiXmlns);
            soapEnvelope.Append(@"/>");
            soapEnvelope.Append(soapEnvelopeClosing);

            watch1.Start();
            SoapHelper.HttpSOAPToFile(_asmxURL, soapEnvelope.ToString(), soapAction, xmlFileName);
            watch1.Stop();
            Debug.WriteLine("GetSearchableConcepts: " + watch1.ElapsedMilliseconds);
        }

        /// <summary>
        /// Gets the ontology tree as a xml file
        /// </summary>
        /// <returns>the tree xml
        /// </returns>
        public string GetOntologyTree()
        {      
            string soapAction = @"http://hiscentral.cuahsi.org/20100205/getOntologyTree";
            
            StringBuilder soapEnvelope = new StringBuilder();
            soapEnvelope.Append(soapEnvelopeHeader);
            soapEnvelope.Append(@"<getOntologyTree ");
            soapEnvelope.Append(cuahsiXmlns);
            soapEnvelope.Append(@"><conceptKeyword>Hydrosphere</conceptKeyword></getOntologyTree>");
            soapEnvelope.Append(soapEnvelopeClosing);

            watch1.Reset();
            WebResponse resp = SoapHelper.HttpSOAPRequest(soapEnvelope.ToString(), soapAction);
            watch1.Stop();
            Debug.WriteLine("getOntologyTree: " + watch1.ElapsedMilliseconds);

            Stream stm = resp.GetResponseStream();
            StreamReader r = new StreamReader(stm);
            // process SOAP return doc here.
            string result = r.ReadToEnd();
            r.Close();
            resp.Close();
            return result;
        }

        /// <summary>
        /// Download the keyword xml file from server to
        /// and save it on local disk
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <returns></returns>
        public void DownloadOntologyTree(string xmlFileName)
        {
            string soapAction = @"http://hiscentral.cuahsi.org/20100205/getOntologyTree";

            StringBuilder soapEnvelope = new StringBuilder();
            soapEnvelope.Append(soapEnvelopeHeader);
            soapEnvelope.Append(@"<getOntologyTree ");
            soapEnvelope.Append(cuahsiXmlns);
            soapEnvelope.Append(@"><conceptKeyword>Hydrosphere</conceptKeyword></getOntologyTree>");
            soapEnvelope.Append(soapEnvelopeClosing);

            watch1.Reset();
            watch1.Start();
            SoapHelper.HttpSOAPToFile(_asmxURL, soapEnvelope.ToString(), soapAction, xmlFileName);
            watch1.Stop();
            Debug.WriteLine("DownloadOntologyTree: " + watch1.ElapsedMilliseconds);
        }

        

        /// <summary>
        /// Returns the series catalog for the specified region (latitude | longitude bounding box)
        /// </summary>
        /// <param name="latLongBox">the latitude/longitude bounding box</param>
        /// <param name="keywords">the array of search keywords (not included in search criteria if null)</param>
        /// <param name="beginDate">the begin date</param>
        /// <param name="endDate">the end date</param>
        /// <param name="networkIDs">the ServiceIDs. These are obtained by calling GetServicesInBox() function</param>
        /// <returns>a list o all matching data series entries</returns>
        public IList<SeriesMetadata> GetSeriesCatalogForBox(Box latLongBox, string[] keywords, DateTime beginDate, DateTime endDate, int[] networkIDs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the series catalog for the specified region (latitude | longitude bounding box)
        /// </summary>
        /// <param name="latLongBox">the latitude / </param>
        /// <param name="keywords">The array of keywords</param>
        /// <param name="beginDate">The begin start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>a list of all matching data series entries</returns>
        public IList<SeriesMetadata> GetSeriesCatalogForBox(Box latLongBox, string[] keywords, DateTime beginDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the series catalog for the specified region (latitude | longitude bounding box)
        /// </summary>
        /// <param name="latLongBox">the latitude/longitude bounding box</param>
        /// <param name="keyword">search keyword (not included in search criteria if null)</param>
        /// <param name="beginDate">the begin date</param>
        /// <param name="endDate">the end date</param>
        /// <param name="networkIDs">the ServiceIDs. These are obtained by calling GetServicesInBox() function</param>
        /// <param name="xmlFileName">the xml file name where to save results</param>
        public void GetSeriesCatalogForBoxXml(Box latLongBox, string keyword, DateTime beginDate, DateTime endDate, int[] networkIDs, string xmlFileName)
        {
            string soapAction = cuahsiSoapAction + "GetSeriesCatalogForBox2";

            StringBuilder soap = new StringBuilder();
            soap.Append(soapEnvelopeHeader);
            soap.Append(@"<GetSeriesCatalogForBox2 ");
            soap.Append(cuahsiXmlns);
            soap.Append(@">");
            soap.Append(@"<xmin>");
            soap.Append(latLongBox.XMin.ToString());
            soap.Append(@"</xmin>");
            soap.Append(@"<xmax>");
            soap.Append(latLongBox.XMax.ToString());
            soap.Append(@"</xmax>");
            soap.Append(@"<ymin>");
            soap.Append(latLongBox.YMin.ToString());
            soap.Append(@"</ymin>");
            soap.Append(@"<ymax>");
            soap.Append(latLongBox.YMax.ToString());
            soap.Append(@"</ymax>");
            if (string.IsNullOrEmpty(keyword))
            {
                soap.Append(@"<conceptKeyword/>");
            }
            else
            {
                soap.Append(@"<conceptKeyword>");
                soap.Append(keyword);
                soap.Append(@"</conceptKeyword>");
            }

            if (networkIDs == null)
            {
                soap.Append(@"<networkIDs/>");
            }
            else if (networkIDs.Length == 0)
            {
                soap.Append(@"<networkIDs/>");
            }
            else
            {
                soap.Append(@"<networkIDs>");
                for (int i = 0; i < networkIDs.Length; i++)
                {
                    soap.Append(networkIDs[i].ToString());
                    if (i < networkIDs.Length - 1)
                    {
                        soap.Append(",");
                    }
                }
                soap.Append(@"</networkIDs>");
            }
            soap.Append(@"<beginDate>");
            soap.Append(beginDate.Month.ToString());
            soap.Append("/");
            soap.Append(beginDate.Day.ToString());
            soap.Append("/");
            soap.Append(beginDate.Year.ToString());
            soap.Append(@"</beginDate>");

            soap.Append(@"<endDate>");
            soap.Append(endDate.Month.ToString());
            soap.Append("/");
            soap.Append(endDate.Day.ToString());
            soap.Append("/");
            soap.Append(endDate.Year.ToString());
            soap.Append(@"</endDate>");

            soap.Append(@"</GetSeriesCatalogForBox2>");
            soap.Append(soapEnvelopeClosing);

            watch1.Start();
            SoapHelper.HttpSOAPToFile(_asmxURL, soap.ToString(), soapAction, xmlFileName);
            watch1.Stop();
            Debug.WriteLine("GetSeriesCatalogForBox: " + watch1.ElapsedMilliseconds);
        }

        /// <summary>
        /// Retrieves information about CUAHSI WaterOneFlow web services available in the specified region
        /// (latitude / longitude bounding box).
        /// </summary>
        /// <param name="xmin">Minimum x (longitude)</param>
        /// <param name="xmax">Maximum x (longitude)</param>
        /// <param name="ymax">Maximum y (latitude)</param>
        /// <param name="ymin">Minimum y (latitude)</param>
        /// <returns></returns>
        public IList<DataServiceInfo> GetServicesInBox(double xmin, double ymin, double xmax, double ymax)
        {
            List<DataServiceInfo> serviceList = new List<DataServiceInfo>();
            string soapAction = cuahsiSoapAction + "GetServicesInBox2";

            StringBuilder soap = new StringBuilder();
            soap.Append(soapEnvelopeHeader);
            soap.Append(@"<GetServicesInBox2 ");
            soap.Append(cuahsiXmlns);
            soap.Append(@">");
            soap.Append(@"<xmin>");
            soap.Append(xmin.ToString(CultureInfo.InvariantCulture));
            soap.Append(@"</xmin>");
            soap.Append(@"<ymin>");
            soap.Append(ymin.ToString(CultureInfo.InvariantCulture));
            soap.Append(@"</ymin>");
            soap.Append(@"<xmax>");
            soap.Append(xmax.ToString(CultureInfo.InvariantCulture));
            soap.Append(@"</xmax>");
            soap.Append(@"<ymax>");
            soap.Append(ymax.ToString(CultureInfo.InvariantCulture));
            soap.Append(@"</ymax>");
            soap.Append(@"</GetServicesInBox2>");
            soap.Append(soapEnvelopeClosing);

            WebResponse response = SoapHelper.HttpSOAPRequest(soap.ToString(), soapAction);
            Stream responseStream = response.GetResponseStream();
            XmlTextReader reader = new XmlTextReader(responseStream);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string readerName = reader.Name.ToLower();

                    if (readerName == "serviceinfo")
                    {
                        //Read the site information
                        DataServiceInfo service = ReadDataServiceInfo(reader);
                        if (service != null)
                        {
                            serviceList.Add(service);
                        }
                    }
                }
            }
            reader.Close();
            responseStream.Close();
            response.Close();

            return serviceList;
        }

        private DataServiceInfo ReadDataServiceInfo(XmlTextReader r)
        {
            DataServiceInfo serviceInfo = new DataServiceInfo();
            while (r.Read())
            {
                string nodeName = r.Name.ToLower();
                
                if (r.NodeType == XmlNodeType.Element)
                {
                    if (nodeName == "servurl")
                    {
                        r.Read();
                        serviceInfo.EndpointURL = r.Value;
                    }
                    else if (nodeName == "title")
                    {
                        r.Read();
                        serviceInfo.ServiceTitle = r.Value;
                    }
                    else if (nodeName == "servicedescriptionurl")
                    {
                        r.Read();
                        serviceInfo.DescriptionURL = r.Value;
                    }
                    else if (nodeName == "email")
                    {
                        r.Read();
                        serviceInfo.ContactEmail = r.Value;
                    }
                    else if (nodeName == "organization")
                    {
                        r.Read(); 
                        serviceInfo.Citation = r.Value;
                    }
                    else if (nodeName == "citation")
                    {
                        r.Read();
                        serviceInfo.Citation = r.Value;
                    }
                    else if (nodeName == "aabstract")
                    {
                        r.Read();
                        serviceInfo.Abstract = r.Value;
                    }
                    else if (nodeName == "valuecount")
                    {
                        r.Read();
                        if (!String.IsNullOrEmpty(r.Value))
                        {
                            serviceInfo.ValueCount = Convert.ToInt32(r.Value);
                        }
                    }
                    else if (nodeName == "sitecount")
                    {
                        r.Read();
                        if (!String.IsNullOrEmpty(r.Value))
                        {
                            serviceInfo.SiteCount = Convert.ToInt32(r.Value);
                        }
                    }
                    else if (nodeName == "serviceid")
                    {
                        r.Read();
                        serviceInfo.HISCentralID = Convert.ToInt32(r.Value);
                    }
                    else if (nodeName == "minx")
                    {

                    }
                    else if (nodeName == "miny")
                    {

                    }
                    else if (nodeName == "maxx")
                    {

                    }
                    else if (nodeName == "maxy")
                    {

                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement && nodeName == "serviceinfo")
                {
                    return serviceInfo;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets all web services on HIS Central
        /// (not yet implemented)
        /// </summary>
        /// <returns>The list of web services from HIS Central</returns>
        public IList<DataServiceInfo> GetWebServices()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Downloads the list of all available web services to a xml file
        /// </summary>
        /// <param name="xmlFileName">the xml file name</param>
        public void DownloadWebServiceInfo(string xmlFileName)
        {
            double xmin = -180;
            double xmax = 180;
            double ymin = -90;
            double ymax = 90;
            
            string soapAction = cuahsiSoapAction + "GetServicesInBox2";

            StringBuilder soap = new StringBuilder();
            soap.Append(soapEnvelopeHeader);
            soap.Append(@"<GetServicesInBox2 ");
            soap.Append(cuahsiXmlns);
            soap.Append(@">");
            soap.Append(@"<xmin>");
            soap.Append(xmin.ToString(CultureInfo.InvariantCulture));
            soap.Append(@"</xmin>");
            soap.Append(@"<ymin>");
            soap.Append(ymin.ToString(CultureInfo.InvariantCulture));
            soap.Append(@"</ymin>");
            soap.Append(@"<xmax>");
            soap.Append(xmax.ToString(CultureInfo.InvariantCulture));
            soap.Append(@"</xmax>");
            soap.Append(@"<ymax>");
            soap.Append(ymax.ToString(CultureInfo.InvariantCulture));
            soap.Append(@"</ymax>");
            soap.Append(@"</GetServicesInBox2>");
            soap.Append(soapEnvelopeClosing);

            watch1.Start();
            SoapHelper.HttpSOAPToFile(_asmxURL, soap.ToString(), soapAction, xmlFileName);
            watch1.Stop();
            Debug.WriteLine("GetServicesInBox2: " + watch1.ElapsedMilliseconds);
        }

        /// <summary>
        /// Retrieves all sites within a geographic region that belong to the specified network
        /// (web service) and measure the specified variable
        /// </summary>
        /// <param name="latLongBox"></param>
        /// <param name="keyword"></param>
        /// <param name="networks"></param>
        /// <returns></returns>
        public DataTable GetSitesInBox(Box latLongBox, string keyword, int[] networks)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all sites within the specified latitude / longitude bounding box.
        /// </summary>
        /// <param name="latLongBox"></param>
        /// <returns></returns>
        public DataTable GetAllSitesInBox(Box latLongBox)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all sites within the region, regardless of the web service or variable
        /// </summary>
        /// <param name="latLongBox"></param>
        /// <returns></returns>
        public DataTable GetAllSitesInBox2(Box latLongBox)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves information about CUAHSI WaterOneFlow web services available at the HIS Central
        /// </summary>
        /// <returns></returns>
        public IList<DataServiceInfo> GetWaterOneFlowServiceInfo()
        {
            throw new NotImplementedException();
        }

        #region Private Methods

        private string DateToString(DateTime dat)
        {
            throw new NotImplementedException();
        }

        

        // writes the results to a list of data series objects (which is a theme)
        private void WriteDataSeriesResults(Object resultObj, PropertyInfo[] properties, IList<SeriesMetadata> lst)
        {
            if (resultObj == null) return;
            
            //we need to get the data service information as well.
            IList<DataServiceInfo> servicesList = GetServicesInBox(-179, -89, 179, 89);
            Dictionary<string, DataServiceInfo> servicesLookup = new Dictionary<string, DataServiceInfo>();
            
            foreach (DataServiceInfo servInfo in servicesList)
            {
                if (!servicesLookup.ContainsKey(servInfo.EndpointURL))
                {
                    servicesLookup.Add(servInfo.EndpointURL, servInfo);
                }
            }

            //to prevent duplicate search results - each site code / variable code combination
            //is added to the 'uniqueCode' list
            
            Array arr = resultObj as Array;
            if (arr != null)
            {
                foreach (object obj in arr)
                {
                    SeriesMetadata sm = new SeriesMetadata();
                    sm.Site = new Site();
                    sm.Variable = new Variable();
                    sm.DataService = new DataServiceInfo();

                    foreach (PropertyInfo pi in properties)
                    {
                        object val = pi.GetValue(obj, null);
                        if (val != null)
                        {
                            string propertyName = pi.Name.ToLower();

                            switch(propertyName)
                            {
                                case "location":
                                    sm.Site.Code = val.ToString();
                                    break;
                                case "varcode":
                                    sm.Variable.Code = val.ToString();
                                    break;
                                case "servcode":
                                    string servCode = val.ToString();
                                    sm.Site.NetworkPrefix = servCode;
                                    sm.Variable.VocabularyPrefix = servCode;
                                    sm.DataService.ServiceCode = servCode;
                                    break;
                                case "latitude":
                                    sm.Site.Latitude = Convert.ToDouble(val);
                                    break;
                                case "longitude":
                                    sm.Site.Longitude = Convert.ToDouble(val);
                                    break;
                                case "sitename":
                                    sm.Site.Name = Convert.ToString(val);
                                    break;
                                case "varname":
                                    sm.Variable.Name = Convert.ToString(val);
                                    break;
                                case "servurl":
                                    string url = Convert.ToString(val).ToLower().Trim();
                                    if (url.EndsWith("?wsdl"))
                                    {
                                        url = url.Replace("?wsdl", "");
                                    }
                                    sm.DataService.EndpointURL = url;
                                    break;
                                case "valuecount":
                                    sm.ValueCount = Convert.ToInt32(val);
                                    break;
                                case "begindate":
                                    sm.BeginDateTime = Convert.ToDateTime(val);
                                    break;
                                case "enddate":
                                    sm.EndDateTime = Convert.ToDateTime(val);
                                    break;
                                case "datatype":
                                    sm.Variable.DataType = val.ToString();
                                    break;
                                case "valuetype":
                                    sm.Variable.ValueType = val.ToString();
                                    break;
                                case "samplemedium":
                                    sm.Variable.SampleMedium = val.ToString();
                                    break;
                                case "timeunits":
                                    sm.Variable.TimeUnit = new Unit(val.ToString(), "time", val.ToString());
                                    break;
                                case "gencategory":
                                    sm.Variable.GeneralCategory = val.ToString();
                                    break;
                                case "timesupport":
                                    sm.Variable.TimeSupport = Convert.ToDouble(val);
                                    break;
                            }
						}
                    }
                    if (String.IsNullOrEmpty(sm.Variable.Code))
                    {
                        continue;
                    }

                    string compoundCode = sm.Site.Code + "|" + sm.Variable.Code;
                    if (_uniqueCodeList.Contains(compoundCode) == false)
                    {
                        sm.Source.Organization = ((DataServiceInfo)servicesLookup[sm.DataService.EndpointURL]).ServiceName;
                        sm.Source.Description = ((DataServiceInfo)servicesLookup[sm.DataService.EndpointURL]).ServiceTitle;
                        
                        lst.Add(sm);
                        _uniqueCodeList.Add(compoundCode);
                    }
                }
            }
        }

        

        //converts a data table to a list of WaterOneFlowServiceInfo objects
        private IList<DataServiceInfo> TableToWebServiceInfo(DataTable table)
        {
            List<DataServiceInfo> resultList = new List<DataServiceInfo>();
            foreach (DataRow row in table.Rows)
            {
                string endpointURL = row["servURL"].ToString();

                endpointURL = endpointURL.ToLower();
                if (endpointURL.EndsWith("?wsdl"))
                    endpointURL = endpointURL.Replace("?wsdl", "");

                string title = row["Title"].ToString();
                DataServiceInfo servInfo = new DataServiceInfo(endpointURL, title);
                servInfo.DescriptionURL = row["ServiceDescriptionURL"].ToString();
                servInfo.ContactName = row["organization"].ToString();
                servInfo.ContactEmail = row["orgwebsite"].ToString();
                servInfo.Citation = row["citation"].ToString();
                servInfo.Abstract = (string)row["aabstract"];
                servInfo.ValueCount = Convert.ToInt32(row["valuecount"]);
                servInfo.SiteCount = Convert.ToInt32(row["sitecount"]);
                servInfo.HISCentralID = Convert.ToInt32(row["ServiceID"]);
                servInfo.EastLongitude = Convert.ToDouble(row["minx"]);
                servInfo.SouthLatitude = Convert.ToDouble(row["miny"]);
                servInfo.WestLongitude = Convert.ToDouble(row["maxx"]);
                servInfo.NorthLatitude = Convert.ToDouble(row["maxy"]);
                servInfo.ServiceCode = Convert.ToString(row["NetworkName"]);
                servInfo.ServiceName = Convert.ToString(row["organization"]);
                resultList.Add(servInfo);
            }
            return resultList;
        }

        //creates the empty data table for retrieving a list of web services
        private DataTable CreateWebServiceInfoTable()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ServiceID", typeof(int));
            tbl.Columns.Add("ServiceTitle", typeof(string));
            tbl.Columns.Add("ServiceName", typeof(string));
            tbl.Columns.Add("ServiceCode", typeof(string));
            tbl.Columns.Add("ServiceVersion", typeof(string));
            tbl.Columns.Add("ServiceProtocol", typeof(string));
            tbl.Columns.Add("ServiceEndpointURL", typeof(string));
            tbl.Columns.Add("ServiceDescriptionURL", typeof(string));
            tbl.Columns.Add("NorthLatitude", typeof(string));
            tbl.Columns.Add("SouthLatitude", typeof(string));
            tbl.Columns.Add("EastLongitude", typeof(string));
            tbl.Columns.Add("WestLongitude", typeof(string));
            tbl.Columns.Add("Abstract", typeof(string));
            tbl.Columns.Add("ContactName", typeof(string));
            tbl.Columns.Add("Citation", typeof(string));
            return tbl;
        }

        #endregion
    }
}
