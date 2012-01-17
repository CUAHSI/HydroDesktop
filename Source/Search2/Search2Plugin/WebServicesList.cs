using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace HydroDesktop.Search
{
    public class WebServicesList
    {
        //private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string SERVICES_XML_NAME = Properties.Settings.Default.WebServicesFileName;
        private IHISCentralSearcher _searcher;
        private static TimeSpan _UpdatePeriod = new TimeSpan(0, HydroDesktop.Search.Properties.Settings.Default.WebServiceListUpdateInMinutes, 0);

        public WebServicesList()
        {
            WebServicesFilename = Path.Combine(ServicesXmlDirectoryPath, SERVICES_XML_NAME);
        }

        /// <summary>
        /// Time Period between updates
        /// </summary>
        public static TimeSpan UpdateTime { get { return _UpdatePeriod; } set { _UpdatePeriod = value; } }

        public static DateTime LastUpdated { get; set; }

        public static string WebServicesFilename { get; set; }

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

        /// <summary>
        /// This gets the web services list from a local .resx file
        /// </summary>
        /// <returns></returns>
        private XmlDocument GetDefaultWebServiceList()
        {
            Assembly asm = Assembly.GetAssembly(typeof(HydroDesktop.Search.WebServicesList));

            using (System.IO.Stream stream = (asm.GetManifestResourceStream("HydroDesktop.Search.default_web_services.xml")))
            {
                XmlDocument xmldoc = new XmlDocument();

                xmldoc.Load(stream);

                try
                {
                    //this method also tries to save the default list
                    xmldoc.Save(WebServicesFilename);
                }
                catch { }

                return xmldoc;
            }
        }

        /// <summary>
        /// Updates the service icons from HIS central
        /// </summary>
        public void UpdateServiceIcons()
        {
            //get the icons directory
            string iconDirectory = Path.Combine(HydroDesktop.Configuration.Settings.Instance.TempDirectory, "ServiceIcons");
            if (!Directory.Exists(iconDirectory)) Directory.CreateDirectory(iconDirectory);

            string url = HydroDesktop.Configuration.Settings.Instance.SelectedHISCentralURL;
            string baseUrl = url.ToLower().Replace(@"webservices/hiscentral.asmx", String.Empty);

            XmlDocument doc = GetWebServicesList(false, false);

            List<string> serviceCodes = new List<string>();

            //to get the list of service codes
            XmlElement root = doc.DocumentElement;
            foreach (XmlNode child1 in root.ChildNodes)
            {
                foreach (XmlNode child2 in child1.ChildNodes)
                {
                    if (child2.Name == "NetworkName")
                    {
                        serviceCodes.Add(child2.InnerText);
                    }
                }
            }

            using (WebClient client = new WebClient())
            {
                foreach (string code in serviceCodes)
                {
                    string iconUrl = String.Format("{0}getIcon.aspx?name={1}", baseUrl, code);
                    string iconFileName = Path.Combine(iconDirectory, code + ".gif");
                    try
                    {
                        client.DownloadFile(iconUrl, iconFileName);
                    }
                    catch { }
                }
            }
        }

        //gets the 'WebServices' xml file
        //if ForceRefresh is true, then always try to connect to HIS Central
        //otherwise, try to use the web services file first
        public XmlDocument GetWebServicesList(bool forceRefresh, bool showErrorMessage)
        {
            if (!forceRefresh && IsUpToDate)
            {
                //using local file..
                try
                {
                    //the local file exists
                    return ReadFile(WebServicesFilename);
                }
                catch
                {
                    //the local file does not exist
                    return GetDefaultWebServiceList();
                }
            }
            else
            {
                try
                {
                    var centralList = HydroDesktop.Configuration.Settings.Instance.HISCentralURLList;
                    return GetWebServicesFromHISCentral(centralList);
                }
                catch (Exception ex)
                {
                    if (showErrorMessage)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    return GetDefaultWebServiceList();
                }
            }
        }

        public bool IsUpToDate
        {
            get
            {
                return (LastUpdated.Add(UpdateTime) > DateTime.Now);
            }
        }

        ////gets the 'WebServices' xml file from HIS Central
        //public XmlDocument GetWebServicesList()
        //{
        //    var centralList = HydroDesktop.Configuration.Settings.Instance.HISCentralURLList;

        //    if (File.Exists(WebServicesFilename))
        //    {
        //        //Should be longer rather than shorter that is not necessary to update the xml file
        //        if (IsUpToDate)
        //        {
        //            try
        //            {
        //                return ReadFile(WebServicesFilename);
        //            }

        //            catch
        //            {
        //                  // we cant read it. it exists
        //                 XmlDocument servList = GetWebServicesFromHISCentral(centralList);

        //                 try
        //                 {
        //                     SaveFile(WebServicesFilename, servList);
        //                 }

        //                 catch
        //                 {
        //                     log.Error("Could not write WebServices.xml");
        //                 }

        //                 return servList;
        //            }
        //        }
        //    }

        //    {
        //        return GetWebServicesFromHISCentral(centralList);
        //    }

        //}
        public void SaveFile(string webServicesFilename, XmlDocument servList)
        {
            if (String.IsNullOrEmpty(webServicesFilename))
            {
                throw new FileNotFoundException("Webservices file not found");
            }
            var outputFile = File.Create(webServicesFilename);
            var writer = XmlWriter.Create(outputFile);
            servList.WriteContentTo(writer);
        }

        public XmlDocument ReadFile(string webServicesFilename)
        {
            if (String.IsNullOrEmpty(webServicesFilename))
            {
                throw new FileNotFoundException("Webservices file not found");
            }
            XmlDocument getWebServ = new XmlDocument();
            getWebServ.Load(webServicesFilename);
            return getWebServ;
        }

        //gets the 'WebServices' xml file from HIS Central
        private XmlDocument GetWebServicesFromHISCentral(IList<string> hisCentralUrl)
        {
            WebException myWebException = null;

            foreach (var url in hisCentralUrl)
            {
                try
                {
                    _searcher = _searcher ?? new HISCentralSearcher(url);
                    RefreshListFromHisCentral(_searcher);
                    var document = new XmlDocument();
                    try
                    {
                        document.Load(WebServicesFilename);
                    }
                    catch (WebException ex1)
                    {
                        myWebException = ex1;
                    }
                    catch (FileNotFoundException ex)
                    {
                        //log.Error("HIS central GetWebServices Fail to write a file" + url + " " + ex.Message);
                    }
                    if (File.Exists(WebServicesFilename))
                    {
                        return document;
                    }
                }
                catch
                {
                    //log.Error("HIS central GetWebServices Failed " + url);
                    continue;
                }
            }

            //if connecting all servers failed: throw exception
            String error = "Error refreshing web services from HIS Central. Using the existing list of web services.";
            if (myWebException != null)
                throw new WebException(error, myWebException);
            else
                throw new WebException(error);
            //return null;
            //return GetDefaultWebServiceList();
        }

        public void RefreshListFromHisCentral(IHISCentralSearcher searcher)
        {
            searcher.GetWebServicesXml(WebServicesFilename);
        }

        #region mock

        public WebServicesList(String fileName, IHISCentralSearcher searcher)
        {
            WebServicesFilename = fileName;
            _searcher = searcher;
        }

        #endregion mock
    }
}