using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using log4net;

namespace HydroDesktop.DataDownload.WebServices
{
    public class WebServicesList
    {
        #region Fields

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string SERVICES_XML_NAME = Properties.Settings.Default.WebServicesFileName;
        private IHISCentralSearcher _searcher;
        private static TimeSpan _UpdatePeriod = new TimeSpan(0, Properties.Settings.Default.WebServiceListUpdateInMinutes, 0);
        /// <summary>
        /// Time Period between updates
        /// </summary>
        private static TimeSpan UpdateTime { get { return _UpdatePeriod; } set { _UpdatePeriod = value; } }
        private static DateTime LastUpdated { get; set; }
        private static string WebServicesFilename { get; set; }
        private bool IsUpToDate
        {
            get
            {
                return (LastUpdated.Add(UpdateTime) > DateTime.Now);
            }
        }

        #endregion

        #region Constructors

        public WebServicesList()
        {
            WebServicesFilename = Path.Combine(ServicesXmlDirectoryPath, SERVICES_XML_NAME);
        }

        #endregion

        #region Public methods
        
        /// <summary>
        /// Gets the 'WebServices' xml file.
        /// </summary>
        /// <param name="forceRefresh">if ForceRefresh is true, then always try to connect to HIS Central
        ///otherwise, try to use the web services file first</param>
        /// <param name="showErrorMessage">Show error message</param>
        /// <returns>XmlDocument with webservices</returns>
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
            try
            {
                var centralList = Configuration.Settings.Instance.HISCentralURLList;
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

        #endregion

        #region Private methods

        /// <summary>
        ///  Expect that this will be a method in the main class someday
        /// </summary>
        private static string ServicesXmlDirectoryPath
        {
            get
            {
                var servicesXMLPath = Configuration.Settings.Instance.ApplicationDataDirectory;
                return servicesXMLPath;
            }
        }

        /// <summary>
        /// This gets the web services list from a local .resx file
        /// </summary>
        /// <returns></returns>
        private XmlDocument GetDefaultWebServiceList()
        {
            var asm = Assembly.GetAssembly(typeof(WebServicesList));

            using (var stream = (asm.GetManifestResourceStream("HydroDesktop.DataDownload.default_web_services.xml")))
            {
                var xmldoc = new XmlDocument();
                if (stream != null) xmldoc.Load(stream);

                try
                {
                    //this method also tries to save the default list
                    xmldoc.Save(WebServicesFilename);
                }
                catch { }

                return xmldoc;
            }
        }

        private XmlDocument ReadFile(string webServicesFilename)
        {
            if (String.IsNullOrEmpty(webServicesFilename))
            {
                throw new FileNotFoundException("Webservices file not found");
            }
            var getWebServ = new XmlDocument();
            getWebServ.Load(webServicesFilename);
            return getWebServ;
        }

        //gets the 'WebServices' xml file from HIS Central
        private XmlDocument GetWebServicesFromHISCentral(IEnumerable<string> hisCentralUrl)
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
                        log.Error("HIS central GetWebServices Fail to write a file" + url + " " + ex.Message);
                    }
                    if (File.Exists(WebServicesFilename))
                    {
                        return document;
                    }
                }
                catch
                {
                    log.Error("HIS central GetWebServices Failed " + url);
                    continue;
                }
            }

            //if connecting all servers failed: throw exception
            String error = "Error refreshing web services from HIS Central. Using the existing list of web services.";
            if (myWebException != null)
                throw new WebException(error, myWebException);
            throw new WebException(error);
        }

        private void RefreshListFromHisCentral(IHISCentralSearcher searcher)
        {
            searcher.GetWebServicesXml(WebServicesFilename);
        }

        #endregion
    }
}
