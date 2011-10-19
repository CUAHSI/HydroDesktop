#region Namespace
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Web.Services.Description;
using System.Data;
using HydroDesktop.Interfaces.ObjectModel;
using System.Diagnostics;
using System.Globalization;
#endregion


namespace HydroDesktop.WebServices
{
    /// <summary>
    /// Calls to the HIS central web services
    /// </summary>
    public class HISCentralClient : WebServiceClientBase
    {
        Stopwatch watch1 = new Stopwatch();
        Stopwatch watch2 = new Stopwatch();
        
        /// <summary>
        /// Creates a new instance of HIS central web service client
        /// </summary>
        /// <param name="_assemblyxURL">The url of the server with HIS Central web services</param>
        public HISCentralClient(string asmxURL) :
            base(asmxURL) 
            {
                if (_webService != null)
                {
                    if (_webService.GetType().GetMethod("GetSearchableConcepts") == null)
                    {
                        throw new System.Net.WebException("The Service doesn't have the required method GetSearchableConcepts");
                    }
                }

            }

        #region Private Variables

        //the list of unique site / variable combinations
        private List<string> _uniqueCodeList = new List<string>();

        #endregion

        /// <summary>
        /// Returns a list of mapped variables
        /// </summary>
        /// <returns></returns>
        public DataTable GetMappedVariables()
        {
            if (_assembly == null) return null;

            PropertyInfo[] properties = GetPropertyInfo("MappedVariable");
            DataTable table = CreateResultTable(properties, "MappedVariable");

            object result = null;

            object[] param = new object[2];
            param[0] = new string[] { "" };
            param[1] = new string[] { "" };

            result = CallWebMethod("GetMappedVariables", param);

            WriteResults(result, properties, table);
            return table;
        }

        /// <summary>
        /// Returns a string array of searchable concept names
        /// </summary>
        /// <returns></returns>
        public string[] GetSearchableConcepts()
        {
            if (_assembly == null) return null;
            
            object result = null;

            result = CallWebMethod("GetSearchableConcepts", null);

            Array resultArr = result as Array;
            if (resultArr != null)
            {
                string[] result2 = new string[resultArr.Length];
                for (int i = 0; i < resultArr.Length; i++)
                {
                    result2[i] = (string)resultArr.GetValue(i);
                }
                return result2;
            }

            return null;
        }

        /// <summary>
        /// Gets the ontology tree, in the form of a DataTable
        /// </summary>
        /// <returns>A data table with following three columns:
        /// ID
        /// ParentID
        /// Keyword
        /// </returns>
        public DataTable GetOntologyTreeTable()
        {
            if (_assembly == null) return null;

            object result = null;
            
            DataTable tbl = new DataTable();
            tbl.Columns.Add(new DataColumn("ID", typeof(int)));
            tbl.Columns.Add(new DataColumn("ParentID", typeof(int)));
            tbl.Columns.Add(new DataColumn("Keyword", typeof(string)));

            PropertyInfo[] properties = GetPropertyInfo("OntologyNode");

            //the top-level keyword is always 'hydrosphere'
            object[] param = new object[1] { "hydrosphere" };
            result = CallWebMethod("getOntologyTree", param);

            ReadTree2(result, properties, 1, tbl);
            
            return tbl;
        }

        /// <summary>
        /// Returns all valid pairs of conceptCode, conceptKeyword
        /// from the ontology concept tree
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetOntologyTree()
        {
            if (_assembly == null) return null;

            object result = null;

            PropertyInfo[] properties = GetPropertyInfo("OntologyNode");
            object[] param = new object[1] { "hydrosphere" };
            // Finally, Invoke the web service method
            result = CallWebMethod("getOntologyTree", param);

            Dictionary<string, string> keywordLookup = new Dictionary<string, string>();
            ReadTree(result, properties, keywordLookup);

            return keywordLookup;
        }

        private void ReadTree(object node, PropertyInfo[] properties, Dictionary<string, string> resultList)
        {
            if (resultList == null) return;
            
            string keyword = properties[0].GetValue(node, null).ToString();
            keyword = keyword.Replace("\\c", ",");
            string code = properties[1].GetValue(node, null).ToString();

            Array childNodes = properties[2].GetValue(node, null) as Array;

            //if there are no child nodes, we'll add the keyword
            if (resultList.ContainsKey(keyword) == false && keyword.IndexOf("other") < 0)
            {
                if (childNodes == null)
                {
                    resultList.Add(keyword.ToString(), code.ToString());
                }
                else if (childNodes.Length == 0)
                {
                    resultList.Add(keyword.ToString(), code.ToString());
                }
            }
            
            if (childNodes == null) return;
            if (childNodes.Length == 0) return;
            foreach (object obj in childNodes)
            {
                ReadTree(obj, properties, resultList);
            }
        }

        private void ReadTree2(object node, PropertyInfo[] properties, int parentID, DataTable resultTable)
        {
            string keyword = properties[0].GetValue(node, null).ToString();
            keyword = keyword.Replace("\\c", ",");
            int code = Convert.ToInt32(properties[1].GetValue(node, null));

            //add the keyword to table
            DataRow newRow = resultTable.NewRow();
            newRow["ID"] = code;
            newRow["ParentID"] = parentID;
            newRow["Keyword"] = keyword;
            resultTable.Rows.Add(newRow);

            Array childNodes = properties[2].GetValue(node, null) as Array;

            if (childNodes == null) return;
            if (childNodes.Length == 0) return; //exit if there are no child nodes

            //otherwise, add the child keywords
            foreach (object obj in childNodes)
            {
                ReadTree2(obj, properties, code, resultTable);
            }
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
            if (_assembly == null) return null;

            _uniqueCodeList.Clear();

            //if no keywords are specified - set them to an array with one empty string
            //if no keywords are specified, set the keywords[] parameter to an array with one empty string
            if (keywords == null)
            {
                keywords = new string[]{ String.Empty };
            }
            if (keywords.Length == 0)
            {
                keywords = new string[]{ String.Empty };
            }

            //if no service IDs are specified - set them to all available services within region
            //if (networkIDs == null)
            //{
            //    IList<DataServiceInfo> networks = GetServicesInBox(latLongBox.xmin, latLongBox.ymin, latLongBox.xmax, latLongBox.ymax);
            //    networkIDs = new int[networks.Count];

            //    for (int i = 0; i < networks.Count; i++)
            //    {
            //        networkIDs[i] = networks[i].HISCentralID;
            //    }
            //}
            
			// TODO: why not use MinValue and MaxValue
            //if no begin date specified - set it to a very early date
            if (beginDate == null) beginDate = new DateTime(1900, 1, 1);

            //if no end date specified - set it to current date
            if (endDate == null) endDate = DateTime.Now.Date;
            
            PropertyInfo[] resultProperties = GetPropertyInfo("SeriesRecord");
            DataTable table = CreateResultTable(resultProperties, "SeriesRecord");
            IList<SeriesMetadata> lst = new List<SeriesMetadata>();

            object boxObj = InitializeBox(latLongBox);
            object[] param = new object[5];

            MethodInfo mi = _webService.GetType().GetMethod("GetSeriesCatalogForBox");

            for (int i = 0; i < keywords.Length; i++)
            {
                try
                {
                    param[0] = boxObj;
                    param[1] = keywords[i];
                    param[2] = networkIDs;
                    param[3] = DateToString(beginDate);
                    param[4] = DateToString(endDate);

                    // Invoke the web service method
                    watch1.Start();
                    object result = mi.Invoke(_webService, param);
                    watch1.Stop();
                    // Process the result
                    watch2.Start();
                    WriteDataSeriesResults(result, resultProperties, lst);
                    watch2.Stop();
                }
                finally
                {
                    //System.Windows.Forms.MessageBox.Show(ex.Message + " " + ex.InnerException.Message);
                }
            }
            return lst;
        }

        /// <summary>
        /// Returns the series catalog for the specified region (latitude | longitude bounding box)
        /// </summary>
        /// <param name="latLongBox">the latitude / </param>
        /// <param name="keyword"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns>a list of all matching data series entries</returns>
        public IList<SeriesMetadata> GetSeriesCatalogForBox(Box latLongBox, string[] keywords, DateTime beginDate, DateTime endDate)
        {          
            if (_assembly == null) return null;

            _uniqueCodeList.Clear();

            PropertyInfo[] resultProperties = GetPropertyInfo("SeriesRecord");
            List<SeriesMetadata> lst = new List<SeriesMetadata>();

            object[] param = new object[8];
            object result = null;

            MethodInfo mi = _webService.GetType().GetMethod("GetSeriesCatalogForBox2");

            //if no keywords are specified - set them to an array with one empty string
            if (keywords == null)
            {
                keywords = new string[] { string.Empty };
            }

            //if no begin date specified - set it to a very early date
            if (beginDate == null) beginDate = new DateTime(1900, 1, 1);

            //if no end date specified - set it to current date
            if (endDate == null) endDate = DateTime.Now.Date;

            //if no keywords are specified, set the keywords[] parameter to an array with one empty string
            if (keywords == null)
            {
                keywords = new string[]{ String.Empty };
            }
            if (keywords.Length == 0)
            {
                keywords = new string[]{ String.Empty };
            }
            
            for (int i = 0; i < keywords.Length; i++)
            {
                try
                {
                    param[0] = latLongBox.xmin;
                    param[1] = latLongBox.xmax;
                    param[2] = latLongBox.ymin;
                    param[3] = latLongBox.ymax;
                    param[4] = keywords[i];
                    param[5] = "";
                    param[6] = DateToString(beginDate);
                    param[7] = DateToString(endDate);

                    // Invoke the web service method  
                    watch1.Start();
                    result = mi.Invoke(_webService, param);
                    watch1.Stop();

                    // Process the result
                    watch2.Start();
                    WriteDataSeriesResults(result, resultProperties, lst);
                    watch2.Stop();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error accessing the HIS Central Web service", ex);
                }
            }
            
            return lst;
        }

        /// <summary>
        /// Retrieves information about CUAHSI WaterOneFlow web services available in the specified region
        /// (latitude / longitude bounding box).
        /// </summary>
        /// <param name="latLongBox"></param>
        /// <returns></returns>
        public IList<DataServiceInfo> GetServicesInBox(double xmin, double ymin, double xmax, double ymax)
        {
            object[] param = new object[4];

            param[0] = xmin;
            param[1] = ymin;
            param[2] = xmax;
            param[3] = ymax;

            object result = null;
            result = CallWebMethod("GetServicesInBox2", param);
            
            // Process the result
            PropertyInfo[] properties = GetPropertyInfo("ServiceInfo");
            DataTable table = CreateResultTable(properties, "ServiceInfo");
            WriteResults(result, properties, table);
            return TableToWebServiceInfo(table);
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
            PropertyInfo[] properties = GetPropertyInfo("Site");
            DataTable table = CreateResultTable(properties, "Site");

            object boxObj = InitializeBox(latLongBox);

            object[] param = new object[3];

            param[0] = boxObj;
            param[1] = keyword;
            param[2] = networks;

            object result = null;
            result = CallWebMethod("GetSitesInBox", param);
            //MethodInfo mi = _webService.GetType().GetMethod("GetSitesInBox");
            //result = mi.Invoke(_webService, param);

            // Process the result
            WriteResults(result, properties, table);
            return table;
        }

        /// <summary>
        /// Gets all sites within the specified latitude / longitude bounding box.
        /// </summary>
        /// <param name="latLongBox"></param>
        /// <returns></returns>
        public DataTable GetAllSitesInBox(Box latLongBox)
        {
            //DataTable networkTable = GetServicesInBox(latLongBox);
            //int[] networkIDs = new int[networkTable.Rows.Count];

            PropertyInfo[] properties = GetPropertyInfo("Site");
            DataTable table = CreateResultTable(properties, "Site");

            object boxObj = InitializeBox(latLongBox);

            object[] param = new object[3];

            param[0] = boxObj;
            param[1] = "";
            param[2] = null;

            object result = null;
            result = CallWebMethod("GetSitesInBox", param);
            //MethodInfo mi = _webService.GetType().GetMethod("GetSitesInBox");
            //result = mi.Invoke(_webService, param);

            // Process the result
            WriteResults(result, properties, table);
            return table;
        }

        /// <summary>
        /// Returns all sites within the region, regardless of the web service or variable
        /// </summary>
        /// <param name="latLongBox"></param>
        /// <returns></returns>
        public DataTable GetAllSitesInBox2(Box latLongBox)
        {
            //first we get all available concepts
            string[] keywords = GetSearchableConcepts();

            DataTable resultTable = null;

            foreach (string keyword in keywords)
            {
                DataTable tmpTable = GetAllSitesInBox(latLongBox);

                if (resultTable == null)
                {
                    resultTable = tmpTable.Clone();
                }
                else
                {
                    foreach (DataRow row in tmpTable.Rows)
                    {
                        DataRow newRow = resultTable.NewRow();
                        newRow.ItemArray = row.ItemArray;
                        resultTable.Rows.Add(newRow);
                    }
                }
            }

            return resultTable;
        }

        /// <summary>
        /// Retrieves information about CUAHSI WaterOneFlow web services available at the HIS Central
        /// </summary>
        /// <returns></returns>
        public IList<DataServiceInfo> GetWaterOneFlowServiceInfo()
        {
            // check the dynamic assembly
            if (_assembly == null) return null;

            PropertyInfo[] properties = GetPropertyInfo("ServiceInfo");
            DataTable table = CreateResultTable(properties, "ServiceInfo");

            object result = null;
            result = CallWebMethod("GetWaterOneFlowServiceInfo", null);

            // Process the result
            WriteResults(result, properties, table);
            return TableToWebServiceInfo(table);
        }

        #region Private Methods

        private string DateToString(DateTime dat)
        {
            return dat.ToString("MM/dd/yyyy");
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
                string url = servInfo.EndpointURL.ToLower().Trim();
                if (!servicesLookup.ContainsKey(url))
                {
                    servicesLookup.Add(url, servInfo);
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
                                    sm.BeginDateTime = Convert.ToDateTime(val, CultureInfo.GetCultureInfo("en-US"));
                                    break;
                                case "enddate":
                                    sm.EndDateTime = Convert.ToDateTime(val, CultureInfo.GetCultureInfo("en-US"));
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
                        if (servicesLookup.ContainsKey(sm.DataService.EndpointURL))
                        {
                            sm.Source.Organization = ((DataServiceInfo)servicesLookup[sm.DataService.EndpointURL]).ServiceName;
                            sm.Source.Description = ((DataServiceInfo)servicesLookup[sm.DataService.EndpointURL]).ServiceTitle;
                        }
                        lst.Add(sm);
                        _uniqueCodeList.Add(compoundCode);
                    }
                }
            }
        }

        //initializes the 'box' object that is used by the web services
        private object InitializeBox(Box box)
        {
            // Create the 'Box' object using the dynamic assembly and set its properties
            object boxObj = _assembly.CreateInstance("Box");
            Type boxType = boxObj.GetType();
            PropertyInfo[] boxProperties = boxType.GetProperties();
            boxProperties[0].SetValue(boxObj, box.xmin, null);
            boxProperties[1].SetValue(boxObj, box.xmax, null);
            boxProperties[2].SetValue(boxObj, box.ymin, null);
            boxProperties[3].SetValue(boxObj, box.ymax, null);

            return boxObj;
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
