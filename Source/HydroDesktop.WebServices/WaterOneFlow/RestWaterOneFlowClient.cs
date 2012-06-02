using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.WebServices.WaterOneFlow
{
    /// <summary>
    /// WaterOneFlow client which can be used to connect to a REST WaterOneFlow
    /// web service
    /// </summary>
    /// <remarks>This implementation is not tested, contains some hard-coded URL's and needs refactoring</remarks>
    public class RestWaterOneFlowClient : IWaterOneFlowClient
    {
        #region Variables

        private string _downloadDirectory;
        
        private WaterOneFlow10Parser _parser;



        #endregion

        /// <summary>
        /// Creates a new instance of a WaterOneFlow client for accessing REST services
        /// </summary>
        public RestWaterOneFlowClient()
        {
            _parser = new WaterOneFlow10Parser();
            DownloadDirectory = Path.Combine(Path.GetTempPath(), "HydroDesktop");

        }

        #region IWaterOneFlowClient Members

        /// <summary>
        /// The directory where WaterML files returned by the web service
        /// are downloaded.
        /// </summary>
        public string DownloadDirectory
        {
            get { return _downloadDirectory; }
            set
            {
                if (Directory.Exists(_downloadDirectory))
                {
                    Directory.CreateDirectory(_downloadDirectory);
                }
                _downloadDirectory = value;
            }
        }


        /// <summary>
        /// Given the site code get the list of Series available at the site
        /// </summary>
        /// <param name="fullSiteCode">The full site code in [NetworkPrefix:SiteCode] format</param>
        /// <returns>The SiteInfo series information</returns>
        public IList<SeriesMetadata> GetSiteInfo(string fullSiteCode)
        {
            string xmlFile = GetSiteInfoXML(fullSiteCode);
            return _parser.ParseGetSiteInfo(xmlFile);
        }


        /// <summary>
        /// Gets the information about all time series supported by the web service as a XML document
        /// in the WaterML format
        /// <param name="fullSiteCode">The full site code in NetworkPrefix:SiteCode format</param>
        /// </summary>
        /// <returns>the downloaded xml file name</returns>
        public string GetSiteInfoXML(string fullSiteCode)
        {
            using (WebClient client = new WebClient())
            {
                string siteCode1 = fullSiteCode.Substring(fullSiteCode.IndexOf(":") + 1);
                string fileName = "Site-" + fullSiteCode + "-" + GenerateTimeStampString() + ".xml";
                fileName = fileName.Replace(":", "-");
                fileName = Path.Combine(DownloadDirectory, fileName);
                client.DownloadFile("http://img.plaveniny.cz/services.ashx?method=getsiteinfo&site=" + siteCode1, fileName);
                return fileName;
            }
        }


        /// <summary>
        /// Gets the information about all sites available at this web service.
        /// </summary>
        /// <returns>The list of all sites supported by this web service.</returns>
        public IList<HydroDesktop.Interfaces.ObjectModel.Site> GetSites()
        {
            string xmlFile = GetSitesXML();
            return _parser.ParseGetSites(xmlFile);
        }


        /// <summary>
        /// Gets the information about all sites available at this web service within a bounding box.
        /// </summary>
        /// <param name="westLongitude">Longitude of western edge of bounding box</param>
        /// <param name="southLatitude">Latitude of southern edge of bounding box</param>
        /// <param name="eastLongitude">Longitude of eastern edge of bounding box</param>
        /// <param name="northLatitude">Latitude of northern edge of bounding box</param>
        /// <returns>The list of all sites supported by this web service within a bounding box.</returns>
        public IList<HydroDesktop.Interfaces.ObjectModel.Site> GetSites(double westLongitude, double southLatitude, double eastLongitude, double northLatitude)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the information about sites within a bounding box, from the web service as a XML document in the WaterML format
        /// </summary>
        /// <param name="westLongitude">Longitude of western edge of bounding box</param>
        /// <param name="southLatitude">Latitude of southern edge of bounding box</param>
        /// <param name="eastLongitude">Longitude of eastern edge of bounding box</param>
        /// <param name="northLatitude">Latitude of northern edge of bounding box</param>
        /// <returns>The downloaded XML file name</returns>
        public string GetSitesXML(double westLongitude, double southLatitude, double eastLongitude, double northLatitude)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the information about all sites in the web service as a XML document in the WaterML format
        /// </summary>
        /// <returns>The downloaded XML file name</returns>
        public string GetSitesXML()
        {
            WebClient client = new WebClient();
            client.BaseAddress = "http://img.plaveniny.cz/services.ashx";
            
            //generate the file name
			string fileName = Path.Combine ( DownloadDirectory, "sites" + GenerateTimeStampString () + ".xml" );
            client.DownloadFile(@"http://img.plaveniny.cz/services.ashx?method=getsites", fileName);

			return fileName;
        }

        /// <summary>
        /// Generates a 'time stamp' string in the yyyyMMddhhmmss-miliseconds format for
        /// the current system dateTime
        /// </summary>
        /// <returns></returns>
        private string GenerateTimeStampString()
        {
            DateTime now = DateTime.Now;
            return now.ToString("yyyyMMddhhmmss") + now.Millisecond.ToString("000");
        }

        /// <summary>
        /// Get the data values for the specific site, variable and time range as a list of Series objects
        /// </summary>
        /// <param name="siteCode">the full site code (networkPrefix:siteCode)</param>
        /// <param name="variableCode">the full variable code (vocabularyPrefix:variableCode)</param>
        /// <param name="startTime">the start date/time</param>
        /// <param name="endTime">the end date/time</param>
        /// <returns>The data series. Each data series object includes a list of data values, 
        /// site, variable, method, source and quality control level information.</returns>
        /// <remarks>Usually the list of Series returned will only contain one series. However in some
        /// cases, it will contain two or more series with the same site code and variable code, but
        /// with a different method or quality control level</remarks>
        public IList<HydroDesktop.Interfaces.ObjectModel.Series> GetValues(string siteCode, string variableCode, DateTime startTime, DateTime endTime)
        {
            
            string xmlFile = GetValuesXML(siteCode, variableCode, startTime, endTime);
            return _parser.ParseGetValues(xmlFile);
        }


        /// <summary>
        /// Get the data values for the specific site, variable and time range as a XML document
        /// in the WaterML format
        /// </summary>
        /// <param name="siteCode">the full site code (networkPrefix:siteCode)</param>
        /// <param name="variableCode">the full variable code (vocabularyPrefix:variableCode)</param>
        /// <param name="startTime">the start date/time</param>
        /// <param name="endTime">the end date/time</param>
        /// <returns>the downloaded xml file name</returns>
        public string GetValuesXML(string siteCode, string variableCode, DateTime startTime, DateTime endTime)
        {
            string siteCode1 = siteCode.Substring(siteCode.IndexOf(":") + 1);
            string variableCode1 = variableCode.Substring(siteCode.IndexOf(":") + 1);

            string timeStamp = GenerateTimeStampString();
            string fileName = siteCode + "-" + variableCode + "-" + timeStamp + ".xml";
            fileName = fileName.Replace(":", "-");
            fileName = fileName.Replace("=", "-");
            fileName = fileName.Replace("/", "-");
            fileName = Path.Combine(DownloadDirectory, fileName);     

            using (WebClient client = new WebClient())
            {
                string uri =
                    String.Format(@"http://img.plaveniny.cz/services.ashx?method=getvalues&site={0}&var={1}&start={2}&end={3}",
                    siteCode1, variableCode1, startTime.ToString("yyyyMMdd"), endTime.ToString("yyyyMMdd"));
                client.DownloadFile(uri, fileName);
                return fileName;
            }
        }


        /// <summary>
        /// Gets or sets the ServiceID (assigned code) corresponding to this web service
        /// </summary>
        public int ServiceID
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// Gets information about the web service used by this web service client
        /// </summary>
        public HydroDesktop.Interfaces.ObjectModel.DataServiceInfo ServiceInfo
        {
            get { throw new NotImplementedException(); }
        }


        /// <summary>
        /// The URL address of the web service being used
        /// </summary>
        public string ServiceURL
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
