using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using HydroDesktop.WebServices;
using HydroDesktop.Plugins.Search.Searching;
using HydroDesktop.Plugins.Search.Settings;

namespace HydroDesktop.Plugins.Search.WebServices
{
    class HisCentralWebServicesList : IWebServicesList
    {
        #region Fields
        
        private string WebServicesFilename { get; set; }
        private readonly string _hisCentralUrl;

        #endregion

        #region Constructors

        public HisCentralWebServicesList(string hisCentralUrl)
        {
            _hisCentralUrl = hisCentralUrl;
            WebServicesFilename = Path.Combine(ServicesXmlDirectoryPath, Properties.Settings.Default.WebServicesFileName);
        }

        #endregion

        #region Public methods

        public IEnumerable<WebServiceNode> GetWebServices()
        {
            var searcher = new HISCentralSearcher(_hisCentralUrl);
            RefreshListFromHisCentral(searcher);
            var xmlReaderSettings = new XmlReaderSettings
            {
                CloseInput = true,
                IgnoreComments = true,
                IgnoreWhitespace = true,
            };

            var result = new List<WebServiceNode>();
            using (var reader = XmlReader.Create(WebServicesFilename, xmlReaderSettings))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "ServiceInfo")
                        {
                            string desciptionUrl = null;
                            string serviceUrl = null;
                            string title = null;
                            int serviceID = -1;
                            string serviceCode = null;
                            string organization = null;

                            int variables = -1, values = -1, sites = -1;
                            double xmin = double.NaN, xmax = double.NaN, ymin = double.NaN, ymax = double.NaN;

                            while (reader.Read())
                            {
                                if  (reader.NodeType == XmlNodeType.EndElement && reader.Name == "ServiceInfo")
                                {
                                    break;
                                }

                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    switch (reader.Name)
                                    {
                                        case "Title":
                                            if (!reader.Read()) continue;
                                            title = reader.Value.Trim();
                                            break;
                                        case "ServiceID":
                                            if (!reader.Read()) continue;
                                            serviceID = Convert.ToInt32(reader.Value.Trim());
                                            break;
                                        case "ServiceDescriptionURL":
                                            if (!reader.Read()) continue;
                                            desciptionUrl = reader.Value.Trim();
                                            break;
                                        case "organization":
                                            if (!reader.Read()) continue;
                                            organization = reader.Value.Trim();
                                            break;
                                        case "servURL":
                                            if (!reader.Read()) continue;
                                            serviceUrl = reader.Value.Trim();
                                            break;
                                        case "valuecount":
                                            if (!reader.Read()) continue;
                                            values = Convert.ToInt32(reader.Value.Trim());
                                            break;
                                        case "variablecount":
                                            if (!reader.Read()) continue;
                                            variables = Convert.ToInt32(reader.Value.Trim());
                                            break;
                                        case "sitecount":
                                            if (!reader.Read()) continue;
                                            sites = Convert.ToInt32(reader.Value.Trim());
                                            break;
                                        case "NetworkName":
                                            if (!reader.Read()) continue;
                                            serviceCode = reader.Value.Trim();
                                            break;
                                        case "minx":
                                            if (!reader.Read()) continue;
                                            double.TryParse(reader.Value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture,
                                                            out xmin);
                                            break;
                                        case "maxx":
                                            if (!reader.Read()) continue;
                                            double.TryParse(reader.Value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture,
                                                            out xmax);
                                            break;
                                        case "miny":
                                            if (!reader.Read()) continue;
                                            double.TryParse(reader.Value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture,
                                                            out ymin);
                                            break;
                                        case "maxy":
                                            if (!reader.Read()) continue;
                                            double.TryParse(reader.Value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture,
                                                            out ymax);
                                            break;
                                    }
                                }
                            }

                            var boundingBox = (Box)null;
                            if (!double.IsNaN(xmin) && !double.IsNaN(xmax) && !double.IsNaN(ymin) && !double.IsNaN(ymax))
                                boundingBox = new Box(xmin, xmax, ymin, ymax);

                            var node = new WebServiceNode(title, serviceCode, serviceID, desciptionUrl, serviceUrl, boundingBox, organization, sites, variables, values);
                            result.Add(node);
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///  Expect that this will be a method in the main class someday
        /// </summary>
        private string ServicesXmlDirectoryPath
        {
            get
            {
                var servicesXMLPath = HydroDesktop.Configuration.Settings.Instance.ApplicationDataDirectory;
                return servicesXMLPath;
            }
        }

        private void RefreshListFromHisCentral(HISCentralSearcher searcher)
        {
            searcher.GetWebServicesXml(WebServicesFilename);
        }

        #endregion
    }
}