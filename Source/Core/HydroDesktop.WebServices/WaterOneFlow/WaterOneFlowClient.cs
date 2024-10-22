﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices.WaterML;

namespace HydroDesktop.WebServices.WaterOneFlow
{
    /// <summary>
    /// Special class for communicating WaterOneFlow web services
    /// </summary>
    public class WaterOneFlowClient
    {
        #region Fields

        private readonly string _serviceURL;
        private readonly IWaterMLParser _parser;
        private string _downloadDirectory;
        private readonly DataServiceInfo _serviceInfo;
        private readonly int _reqTimeOut;
        private readonly int _valuesPerReq;
        private readonly bool _allInOneRequest;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of a WaterOneFlow web service client
        /// which communicates with the specified web service.
        /// </summary>
        /// <param name="serviceInfo">The object with web service information</param>
        /// <param name="reqTimeOut">Request timeout, in seconds</param>
        /// <param name="valuesPerReq">Number of values per request</param>
        /// <param name="allInOneRequest">All values in one request</param>
        /// <remarks>Throws an exception if the web service is not a valid
        /// WaterOneFlow service</remarks>
        public WaterOneFlowClient(DataServiceInfo serviceInfo, int reqTimeOut = 100, int valuesPerReq = 10000,
            bool allInOneRequest = false)
        {
            _serviceURL = serviceInfo.EndpointURL;

            _valuesPerReq = valuesPerReq;
            _allInOneRequest = allInOneRequest;
            _serviceInfo = serviceInfo;
            _reqTimeOut = reqTimeOut;
            _serviceInfo.Version = WebServiceHelper.GetWaterOneFlowVersion(_serviceURL);
            _parser = ParserFactory.GetParser(ServiceInfo);

            SaveXmlFiles = true; // for backward-compatibility
        }

        /// <summary>
        /// Creates a new instance of a WaterOneFlow web service client.
        /// The constructor will throw an exception if the url is an invalid
        /// WaterOneFlow web service url.
        /// </summary>
        /// <param name="asmxURL">The url of the .asmx web page</param>
        /// <param name="reqTimeOut">Request timeout, in seconds</param>
        /// <param name="valuesPerReq">Number of values per request</param>
        /// <param name="allInOneRequest">All values in one request</param>
        /// <remarks>Throws an exception if the web service is not a valid
        /// WaterOneFlow service</remarks>
        public WaterOneFlowClient(string asmxURL, int reqTimeOut = 100, int valuesPerReq = 10000,
            bool allInOneRequest = false) :
            this(new DataServiceInfo(asmxURL, asmxURL.Replace(@"http://", "")), reqTimeOut, valuesPerReq, allInOneRequest)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets information about the web service used by this web service client
        /// </summary>
        public DataServiceInfo ServiceInfo
        {
            get { return _serviceInfo; }
        }

        /// <summary>
        /// Gets or sets the name of the directory where 
        /// downloaded xml files are stored
        /// </summary>
        public string DownloadDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_downloadDirectory))
                {
                    DownloadDirectory = Path.Combine(Path.GetTempPath(), "HydroDesktop");
                }

                return _downloadDirectory;
            }
            set
            {
                if (!Directory.Exists(value))
                {
                    try
                    {
                        Directory.CreateDirectory(value);
                    }
                    catch
                    {
                        value = Path.GetTempPath();
                    }
                }
                _downloadDirectory = value;
            }
        }

        /// <summary>
        /// Gets or sets boolean indicated that WaterOneFlowClient should save temporary xml files.
        /// </summary>
        /// <seealso cref="DownloadDirectory"/>
        public bool SaveXmlFiles { get; set; }

        #endregion

        #region Public Methods

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
        public IList<Series> GetValues(string siteCode, string variableCode, DateTime startTime, DateTime endTime)
        {
            IList<Series> result;
            if (SaveXmlFiles)
            {
                var xmlFile = GetValuesXML(siteCode, variableCode, startTime, endTime);
                using (var fileStream = new FileStream(xmlFile, FileMode.Open))
                {
                    result = _parser.ParseGetValues(fileStream);
                }
            }
            else
            {
                var req = WebServiceHelper.CreateGetValuesRequest(_serviceURL, siteCode, variableCode, startTime, endTime);
                req.Timeout = _reqTimeOut*1000;
                using (var resp = (HttpWebResponse) req.GetResponse())
                {
                    using (var stream = resp.GetResponseStream())
                    {
                        result = _parser.ParseGetValues(stream);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets information about all series available for the specific site
        /// </summary>
        /// <param name="siteCode">the full site code (networkPrefix:siteCode)</param>
        /// <returns>A list of all series. The series don't contain any actual data values
        /// but include all series metadata including the site, variable, source, method\
        /// and quality control level.</returns>
        public IList<SeriesMetadata> GetSiteInfo(string siteCode)
        {
            IList<SeriesMetadata> result;
            if (SaveXmlFiles)
            {
                var xmlFile = GetSiteInfoXML(siteCode);
                using (var fileStream = new FileStream(xmlFile, FileMode.Open))
                {
                    result = _parser.ParseGetSiteInfo(fileStream);
                }
            }
            else
            {
                var req = WebServiceHelper.CreateGetSiteInfoRequest(_serviceURL, siteCode);
                req.Timeout = _reqTimeOut * 1000;
                using (var resp = (HttpWebResponse) req.GetResponse())
                {
                    using (var stream = resp.GetResponseStream())
                    {
                        result = _parser.ParseGetSiteInfo(stream);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the information about all sites available at this web service.
        /// </summary>
        /// <returns>The list of all sites supported by this web service.</returns>
        public IList<Site> GetSites()
        {
            IList<Site> result;
            if (SaveXmlFiles)
            {
                var xmlFile = GetSitesXML();
                using (var fileStream = new FileStream(xmlFile, FileMode.Open))
                {
                    result = _parser.ParseGetSites(fileStream);
                }
            }
            else
            {
                var req = WebServiceHelper.CreateGetSitesRequest(_serviceURL);
                req.Timeout = _reqTimeOut * 1000;
                using (var resp = (HttpWebResponse) req.GetResponse())
                {
                    using (var stream = resp.GetResponseStream())
                    {
                        result = _parser.ParseGetSites(stream);
                    }
                }
            }

            return result;
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
            return GetValuesXML(siteCode, variableCode, startTime, endTime, -1).First();
        }

        /// <summary>
        /// Get the data values for the specific site, variable and time range as a XML document
        /// in the WaterML format
        /// </summary>
        /// <param name="siteCode">the full site code (networkPrefix:siteCode)</param>
        /// <param name="variableCode">the full variable code (vocabularyPrefix:variableCode)</param>
        /// <param name="startTime">the start date/time</param>
        /// <param name="endTime">the end date/time</param>
        /// <param name="estimatedValuesCount">Estimated values count. 
        /// If this value less then zero, the result collection will necessarily contains only 1 file,
        /// otherwise number of file depends from this value.</param>
        /// <param name="progressHandler">Progress handler, may be null.</param>
        /// <returns>Collection of the downloaded xml file names</returns>
        public IEnumerable<string> GetValuesXML(string siteCode, string variableCode,
            DateTime startTime, DateTime endTime,
            int estimatedValuesCount, IGetValuesProgressHandler progressHandler = null)
        {
            var vr = _allInOneRequest ? estimatedValuesCount : _valuesPerReq;
            int intervalsCount;

            if (estimatedValuesCount <= 0 || estimatedValuesCount <= vr)
                intervalsCount = 1;
            else
                intervalsCount = estimatedValuesCount % vr == 0
                    ? estimatedValuesCount / vr
                    : estimatedValuesCount / vr + 1;

            var datesDiff = endTime.Subtract(startTime);
            var daysPerInteval = datesDiff.Days/intervalsCount;

            var loopStartDate = startTime;
            var loopEndDate = loopStartDate.AddDays(daysPerInteval);

            var savedFiles = new List<string>(intervalsCount);
            for (int i = 0; i < intervalsCount; i++)
            {
                if (progressHandler != null &&
                    progressHandler.CancellationPending) break;

                if (i == intervalsCount - 1)
                {
                    loopEndDate = endTime;
                }

                var startGetTime = DateTime.Now;
                try
                {
                    var xmlFile = GetAndSavesValuesXML(siteCode, variableCode, loopStartDate, loopEndDate);
                    savedFiles.Add(xmlFile);
                }
                finally
                {
                    var endGetTime = DateTime.Now;
                    var timeTaken = endGetTime.Subtract(startGetTime).TotalSeconds;
                    if (progressHandler != null)
                    {
                        progressHandler.Progress(i, intervalsCount, timeTaken);
                    }
                }

                // Set loop dates to next interval
                loopStartDate = loopEndDate.AddMinutes(1); //AddDays(1);
                loopEndDate = loopStartDate.AddDays(daysPerInteval);
            }

            return savedFiles.AsEnumerable();
        }

        /// <summary>
        /// Gets the information about all sites in the web service as a XML document in the WaterML format
        /// </summary>
        /// <returns>The downloaded XML file name</returns>
        public string GetSitesXML()
        {
            //generate the file name
            var fileName = Path.Combine(DownloadDirectory, "sites" + GenerateTimeStampString() + ".xml");
            var req = WebServiceHelper.CreateGetSitesRequest(_serviceURL);
            req.Timeout = _reqTimeOut * 1000;
            SaveWebResponseToFile(req, fileName);
            return fileName;
        }

        /// <summary>
        /// Gets the information about all time series supported by the web service as a XML document
        /// in the WaterML format
        /// <param name="fullSiteCode">The full site code in NetworkPrefix:SiteCode format</param>
        /// </summary>
        /// <returns>the downloaded xml file name</returns>
        public string GetSiteInfoXML(string fullSiteCode)
        {
            //generate the file name
            string fileName = "Site-" + fullSiteCode + "-" + GenerateTimeStampString() + ".xml";
            fileName = fileName.Replace(":", "-");
            fileName = Path.Combine(DownloadDirectory, fileName);

            var req = WebServiceHelper.CreateGetSiteInfoRequest(_serviceURL, fullSiteCode);
            req.Timeout = _reqTimeOut * 1000;
            SaveWebResponseToFile(req, fileName);
            return fileName;
        }

        #endregion

        #region Private Methods

        private static void SaveWebResponseToFile(WebRequest req, string filename)
        {
            using (var resp = (HttpWebResponse) req.GetResponse())
            {
                // we will read data via the response stream
                using (var stream = resp.GetResponseStream())
                {
                    var buffer = new byte[1024];
                    using (var outFile = new FileStream(filename, FileMode.Create))
                    {
                        int bytesRead;
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            outFile.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }

        private string GetAndSavesValuesXML(string siteCode, string variableCode, DateTime startTime, DateTime endTime)
        {
            var req = WebServiceHelper.CreateGetValuesRequest(_serviceURL, siteCode, variableCode, startTime, endTime);
            req.Timeout = _reqTimeOut * 1000;
            var filename = GenerateGetValuesFileName(siteCode, variableCode);
            SaveWebResponseToFile(req, filename);
            return filename;
        }

        private string GenerateGetValuesFileName(string siteCode, string variableCode)
        {
            //generate the file name
            var timeStamp = GenerateTimeStampString();
            var fileName = siteCode + "-" + variableCode + "-" + timeStamp + ".xml";
            fileName = fileName.Replace(":", "-");
            fileName = fileName.Replace("=", "-");
            fileName = fileName.Replace("/", "-");

            fileName = Path.Combine(DownloadDirectory, fileName);
            return fileName;
        }

        /// <summary>
        /// Generates a 'time stamp' string in the yyyyMMddhhmmss-miliseconds format for
        /// the current system dateTime
        /// </summary>s
        private static string GenerateTimeStampString()
        {
            var now = DateTime.Now;
            return now.ToString("yyyyMMddhhmmss") + now.Millisecond.ToString("000");
        }

        #endregion
    }
}

