using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Search3.Settings;
using Search3.Settings.UI;
using log4net;

namespace Search3
{
    class WebServicesList
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public WebServicesList()
        {
            WebServicesFilename = Path.Combine(ServicesXmlDirectoryPath, PluginSettings.Instance.WebServicesXmlFileName);
        }

        public string WebServicesFilename { get; set; }

        /// <summary>
        ///  Expect that this will be a method in the main class someday
        /// </summary>
        private static string ServicesXmlDirectoryPath
        {
            get
            {
                var servicesXMLPath = HydroDesktop.Configuration.Settings.Instance.ApplicationDataDirectory;
                return servicesXMLPath;
            }
        }

        private XmlDocument GetWebServicesList()
        {
            var catalogSettings = PluginSettings.Instance.CatalogSettings;
            if (catalogSettings.TypeOfCatalog == TypeOfCatalog.LocalMetadataCache)
            {
                //todo: Which source for webservices we should use in this case?
                return null;
            }
            return GetWebServicesFromHISCentral(catalogSettings.HISCentralUrl);
        }

        public IEnumerable<WebServiceNode> GetWebServicesCollection()
        {
           var document = GetWebServicesList();
            if (document.DocumentElement == null)
                yield return null;
            else
                foreach (XmlNode childNode1 in document.DocumentElement.ChildNodes)
                {
                    if (childNode1.Name == "ServiceInfo")
                    {
                        string desciptionUrl = null;
                        string serviceUrl = null;
                        string title = null;
                        string serviceID = null;
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
                            }
                        }
                        yield return new WebServiceNode(title, serviceID, desciptionUrl, serviceUrl, true);
                    }
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
    }
}
