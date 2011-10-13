using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Search3.Searchers;
using Search3.Settings;
using log4net;

namespace Search3.WebServices
{
    class HisCentralWebServicesList : IWebServicesList
    {
        #region Fields

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string WebServicesFilename { get; set; }

        #endregion

        #region Constructors

        public HisCentralWebServicesList()
        {
            WebServicesFilename = Path.Combine(ServicesXmlDirectoryPath, PluginSettings.Instance.WebServicesXmlFileName);
        }

        #endregion

        #region Public methods

        public IEnumerable<WebServiceNode> GetWebServices()
        {
            var document = GetWebServicesFromHISCentral(PluginSettings.Instance.CatalogSettings.HISCentralUrl);
            if (document.DocumentElement == null)
                return new WebServiceNode[] {};

            var result = new List<WebServiceNode>(document.DocumentElement.ChildNodes.Count);
            foreach (XmlNode childNode1 in document.DocumentElement.ChildNodes)
            {
                if (childNode1.Name == "ServiceInfo")
                {
                    string desciptionUrl = null;
                    string serviceUrl = null;
                    string title = null;
                    string serviceID = null;
                    string serviceCode = null;
                    foreach (XmlNode childNode2 in childNode1.ChildNodes)
                    {
                        switch (childNode2.Name)
                        {
                            case "Title":
                                title = childNode2.InnerText;
                                break;
                            case "ServiceID":
                                serviceID = childNode2.InnerText;
                                break;
                            case "ServiceDescriptionURL":
                                desciptionUrl = childNode2.InnerText;
                                break;
                            case "servURL":
                                serviceUrl = childNode2.InnerText;
                                break;
                            case "NetworkName":
                                serviceCode = childNode2.InnerText;
                                break;
                        }
                    }
                    var node = new WebServiceNode(title, serviceCode, serviceID, desciptionUrl, serviceUrl, true);
                    result.Add(node);
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

        private XmlDocument GetWebServicesFromHISCentral(string hisCentralUrl)
        {
            var searcher = new HISCentralSearcher(hisCentralUrl);
            RefreshListFromHisCentral(searcher);
            var document = new XmlDocument();
            try
            {
                document.Load(WebServicesFilename);
            }
            catch (Exception ex)
            {
                log.Error("Error in  GetWebServicesFromHISCentral", ex);
            }
            if (File.Exists(WebServicesFilename))
            {
                return document;
            }

            throw new Exception();
        }

        private void RefreshListFromHisCentral(IHISCentralSearcher searcher)
        {
            searcher.GetWebServicesXml(WebServicesFilename);
        }

        #endregion
    }
}