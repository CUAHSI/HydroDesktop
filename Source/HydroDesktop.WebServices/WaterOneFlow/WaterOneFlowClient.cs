using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Net;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.WebServices.WaterOneFlow
{
	/// <summary>
	/// Special class for communicating with WaterML / WaterOneFlow web services
	/// </summary>
	public class WaterOneFlowClient : WebServiceClientBase, IWaterOneFlowClient
	{
		#region Variables

		private string _asmxURL;
		private IWaterOneFlowParser _parser;

		//the directory where downloaded files are stored
		private string _downloadDirectory;

		//the object containing additional metadata information
		//about the web service including service version
		private DataServiceInfo _serviceInfo = null;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of a WaterOneFlow web service client
		/// which communicates with the specified web service.
		/// </summary>
		/// <param name="serviceInfo">The object with web service information</param>
		/// <remarks>Throws an exception if the web service is not a valid
		/// WaterOneFlow service</remarks>
		public WaterOneFlowClient ( DataServiceInfo serviceInfo )
		{
			//check if the web service is a valid web service.
			//this will throw an exception when the web service URL is invalid.
			CheckWebService ( serviceInfo.EndpointURL );
			_asmxURL = serviceInfo.EndpointURL;

			DownloadDirectory = Path.Combine ( Path.GetTempPath (), "HydroDesktop" );

			//check if the web service is a valid WaterOneFlow service and determine the
			//service name and service version from the WSDL
			//this will throw an exception if the service doesn't have the valid WaterOneFlow
			//methods.
			_serviceInfo = serviceInfo;
			CheckWaterOneFlowService ( _serviceInfo );

            //assign the waterOneFlow parser
            //the parser is automatically set depending on service version information
            //in the WaterML file
            if (ServiceInfo.Version == 1.0)
            {
                _parser = new WaterOneFlow10Parser();
            }
            else
            {
                _parser = new WaterOneFlow11Parser();
            }
		}

		/// <summary>
		/// Creates a new instance of a WaterOneFlow web service client.
		/// The constructor will throw an exception if the url is an invalid
		/// WaterOneFlow web service url.
		/// </summary>
		/// <param name="asmxURL">The url of the .asmx web page</param>
		/// <remarks>Throws an exception if the web service is not a valid
		/// WaterOneFlow service</remarks>
		public WaterOneFlowClient ( string asmxURL ) :
			this ( new DataServiceInfo ( asmxURL, asmxURL.Replace ( @"http://", "" ) ) ) { }

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
		/// Gets or sets the ServiceID (assigned code) corresponding to this web service
		/// </summary>
		public int ServiceID { get; set; }

		/// <summary>
		/// Gets or sets the name of the directory where 
		/// downloaded xml files are stored
		/// </summary>
		public string DownloadDirectory
		{
			get { return _downloadDirectory; }
			set
			{
				if ( Directory.Exists ( _downloadDirectory ) )
				{
					Directory.CreateDirectory ( _downloadDirectory );
				}
				_downloadDirectory = value;
			}
		}

		/// <summary>
		/// The URL address of the web service being used
		/// </summary>
		public string ServiceURL
		{
			get { return _asmxURL; }
		}

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
		public IList<Series> GetValues ( string siteCode, string variableCode, DateTime startTime, DateTime endTime )
		{
			string xmlFile = GetValuesXML ( siteCode, variableCode, startTime, endTime );
			return _parser.ParseGetValues ( xmlFile );
		}

		/// <summary>
		/// Gets information about all series available for the specific site
		/// </summary>
		/// <param name="siteCode">the full site code (networkPrefix:siteCode)</param>
		/// <returns>A list of all series. The series don't contain any actual data values
		/// but include all series metadata including the site, variable, source, method\
		/// and quality control level.</returns>
		public IList<SeriesMetadata> GetSiteInfo ( string siteCode )
		{
			string xmlFile = GetSiteInfoXML ( siteCode );
			return _parser.ParseGetSiteInfo ( xmlFile );
		}

		/// <summary>
		/// Gets the information about all sites available at this web service.
		/// </summary>
		/// <returns>The list of all sites supported by this web service.</returns>
		public IList<Site> GetSites ()
		{
			string xmlFile = GetSitesXML ();
			return _parser.ParseGetSitesXml ( xmlFile );
		}

		/// <summary>
		/// Gets the information about all sites available at this web service within a bounding box.
		/// </summary>
		/// <param name="westLongitude">Longitude of western edge of bounding box</param>
		/// <param name="southLatitude">Latitude of southern edge of bounding box</param>
		/// <param name="eastLongitude">Longitude of eastern edge of bounding box</param>
		/// <param name="northLatitude">Latitude of northern edge of bounding box</param>
		/// <returns>The list of all sites supported by this web service within a bounding box.</returns>
		public IList<Site> GetSites ( double westLongitude, double southLatitude, double eastLongitude, double northLatitude )
		{
			string xmlFile = GetSitesXML ( westLongitude, southLatitude, eastLongitude, northLatitude );
			return _parser.ParseGetSitesXml ( xmlFile );
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
		public string GetValuesXML ( string siteCode, string variableCode, DateTime startTime, DateTime endTime )
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
	        const int valuesPerReq = 5000;

	        int intervalsCount;
            if (estimatedValuesCount <= 0 || estimatedValuesCount <= valuesPerReq)
                intervalsCount = 1;
            else
                intervalsCount = estimatedValuesCount%valuesPerReq == 0
                                     ? estimatedValuesCount/valuesPerReq
                                     : estimatedValuesCount/valuesPerReq + 1;
                
	        var datesDiff = endTime.Subtract(startTime);
	        var daysPerInteval = datesDiff.Days/intervalsCount;

	        var loopStartDate = startTime;
            var loopEndDate = loopStartDate.AddDays(daysPerInteval);

	        var savedFiles = new List<string>(intervalsCount);
            for(int i = 0; i< intervalsCount; i++)
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
                }finally
                {
                    var endGetTime = DateTime.Now;
                    var timeTaken = endGetTime.Subtract(startGetTime).TotalSeconds;
                    if (progressHandler != null)
                    {
                        progressHandler.Progress(i, intervalsCount, timeTaken);
                    }
                }

                // Set loop dates to next interval
                loopStartDate = loopEndDate.AddDays(1);
                loopEndDate = loopStartDate.AddDays(daysPerInteval);
            }

            return savedFiles.AsEnumerable();
	    }

        private string GetWebResponseString(HttpWebResponse resp)
        {
            // we will read data via the response stream
            Stream receiveStream = resp.GetResponseStream();
            using (StreamReader r = new StreamReader(receiveStream))
            {
                return r.ReadToEnd();
            }     
        }

        private void SaveWebResponseToFile(HttpWebRequest req, string filename)
        {
            using (var resp = (HttpWebResponse)req.GetResponse())
            {
                // we will read data via the response stream
                using (Stream ReceiveStream = resp.GetResponseStream())
                {
                    byte[] buffer = new byte[1024];
                    using (FileStream outFile = new FileStream(filename, FileMode.Create))
                    {
                        int bytesRead;
                        while ((bytesRead = ReceiveStream.Read(buffer, 0, buffer.Length)) != 0)
                            outFile.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

        private string GetAndSavesValuesXML(string siteCode, string variableCode, DateTime startTime, DateTime endTime)
        {
            HttpWebRequest req = WebServiceHelper.CreateGetValuesRequest(_asmxURL, siteCode, variableCode, startTime, endTime);

            string filename = GenerateGetValuesFileName(siteCode, variableCode);

            SaveWebResponseToFile(req, filename);

            return filename;
        }
        private string SaveWebMethodResut(object result, string siteCode, string variableCode)
        {
            //generate the file name
            string timeStamp = GenerateTimeStampString();
            string fileName = siteCode + "-" + variableCode + "-" + timeStamp + ".xml";
            fileName = fileName.Replace(":", "-");
            fileName = fileName.Replace("=", "-");
            fileName = fileName.Replace("/", "-");

            fileName = Path.Combine(DownloadDirectory, fileName);
            WriteLinesToFile(fileName, result.ToString());
            return fileName;
        }
        private string GenerateGetValuesFileName(string siteCode, string variableCode)
        {
            //generate the file name
            string timeStamp = GenerateTimeStampString();
            string fileName = siteCode + "-" + variableCode + "-" + timeStamp + ".xml";
            fileName = fileName.Replace(":", "-");
            fileName = fileName.Replace("=", "-");
            fileName = fileName.Replace("/", "-");

            fileName = Path.Combine(DownloadDirectory, fileName);
            return fileName;
        }

	    /// <summary>
		/// Gets the information about all sites in the web service as a XML document in the WaterML format
		/// </summary>
		/// <returns>The downloaded XML file name</returns>
		public string GetSitesXML ()
		{
            //generate the file name
            string fileName = Path.Combine(DownloadDirectory, "sites" + GenerateTimeStampString() + ".xml");
            
            HttpWebRequest req = WebServiceHelper.CreateGetSitesRequest(_asmxURL);

            SaveWebResponseToFile(req, fileName);

            return fileName; 
		}

		/// <summary>
		/// Gets the information about sites within a bounding box, from the web service as a XML document in the WaterML format
		/// </summary>
		/// <param name="westLongitude">Longitude of western edge of bounding box</param>
		/// <param name="southLatitude">Latitude of southern edge of bounding box</param>
		/// <param name="eastLongitude">Longitude of eastern edge of bounding box</param>
		/// <param name="northLatitude">Latitude of northern edge of bounding box</param>
		/// <returns>The downloaded XML file name</returns>
		public string GetSitesXML ( double westLongitude, double southLatitude, double eastLongitude, double northLatitude )
		{
            throw new NotImplementedException();
            
            //object[] param = new object[2];
            //string argument = "GEOM:BOX(" + westLongitude.ToString () + " " +
            //                                southLatitude.ToString () + "," +
            //                                eastLongitude.ToString () + " " +
            //                                northLatitude.ToString () + ")";
            //param[0] = new string[] { argument };
            //param[1] = "";

            //Object result = null;

            ////for WaterOneFlow 1.0 services we need to call GetSitesXml().
            ////for WaterOneFlow 1.1 services we need to call the GetSites() method instead.
            //if ( ServiceInfo.Version == 1.0 )
            //{
            //    result = CallWebMethod ( "GetSitesXml", param );
            //}
            //else
            //{
            //    result = CallWebMethod ( "GetSites", param );
            //}

            ////generate the file name
            //string fileName = Path.Combine ( DownloadDirectory, "sites" + GenerateTimeStampString () + ".xml" );
            //WriteLinesToFile ( fileName, result.ToString () );
            //return fileName;
		}

		/// <summary>
		/// Gets the information about all time series supported by the web service as a XML document
		/// in the WaterML format
		/// <param name="fullSiteCode">The full site code in NetworkPrefix:SiteCode format</param>
		/// </summary>
		/// <returns>the downloaded xml file name</returns>
		public string GetSiteInfoXML ( string fullSiteCode )
		{
            //generate the file name
            string fileName = "Site-" + fullSiteCode + "-" + GenerateTimeStampString() + ".xml";
            fileName = fileName.Replace(":", "-");
            fileName = Path.Combine(DownloadDirectory, fileName);

            HttpWebRequest req = WebServiceHelper.CreateGetSiteInfoRequest(_asmxURL, fullSiteCode);

            SaveWebResponseToFile(req, fileName);
            return fileName;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Generates a 'time stamp' string in the yyyyMMddhhmmss-miliseconds format for
		/// the current system dateTime
		/// </summary>
		/// <returns></returns>
		private string GenerateTimeStampString ()
		{
			DateTime now = DateTime.Now;
			return now.ToString ( "yyyyMMddhhmmss" ) + now.Millisecond.ToString ( "000" );
		}

		/// <summary>
		/// This method checks whether the service used by this client is a valid WaterOneFlow service
		/// It also checks if it contains all valid web service methods.
		/// This method throws an exception if the service is not a valid WaterOneFlow service.
		/// </summary>
		/// <param name="serviceProxy"></param>
		/// <param name="serviceInfo"></param>
		private void CheckWaterOneFlowService ( DataServiceInfo serviceInfo )
		{
			string errorMessage = "The service " + _asmxURL + " is not a valid WaterOneFlow service. ";

            _serviceInfo.Version = WebServiceHelper.GetWaterOneFlowVersion(_asmxURL);
            
            //MethodInfo getValuesMethod = null;
            //MethodInfo getSiteInfoMethod = null;
            //MethodInfo getSitesMethod = null;
            ////object serviceProxy = null;
            //string getSitesMethodName = "GetSitesXml";

            ////get the names of all service classes in the assembly
            //IList<string> serviceNames = GetServiceNames ( _assembly );

            //foreach ( string serviceName in serviceNames )
            //{
            //    //create the 'service proxy class' object
            //    if ( _webService == null )
            //    {
            //        _webService = _assembly.CreateInstance ( serviceName );
            //    }

            //    if ( _webService != null )
            //    {
            //        //try to find the GetValues method
            //        getValuesMethod = _webService.GetType ().GetMethod ( "GetValues" );

            //        //try to find the GetSiteInfo() method
            //        getSiteInfoMethod = _webService.GetType ().GetMethod ( "GetSiteInfo" );

            //        //look for the GetSitesXml() method. If not found, try to check GetSites() instead.

            //        getSitesMethod = _webService.GetType ().GetMethod ( "GetSitesXml" );

            //        if ( getSitesMethod != null )
            //        {
            //            getSitesMethodName = "GetSitesXml";
            //        }
            //        else
            //        {
            //            getSitesMethod = _webService.GetType ().GetMethod ( "GetSites" );
            //            getSitesMethodName = "GetSites";
            //        }

            //        //if the service class contains all supported methods, then there is no need to check
            //        //the other classes.
            //        if ( getValuesMethod != null && getSitesMethod != null && getSiteInfoMethod != null )
            //        {
            //            break;
            //        }
            //    }
            //}

            ////throw an exception if the web service class or some of its methods don't exist
            //if ( _webService == null )
            //    throw new WebException ( errorMessage );

            //if ( getValuesMethod == null )
            //    throw new WebException ( errorMessage + "GetValues method not found." );
            //if ( getSiteInfoMethod == null )
            //    throw new WebException ( errorMessage + "GetSiteInfo method not found." );

            //if ( getSitesMethod == null )
            //    throw new WebException ( errorMessage + "GetSites method not found." );

            //if ( getSitesMethod.ReturnType != typeof ( string ) )
            //    throw new WebException ( errorMessage + "GetSites method with return type 'string' not found." );

            ////set properties of the ServiceInfo which can be determined from the WSDL
            //_serviceInfo.ServiceName = _webService.GetType ().FullName;

            //if ( getSitesMethodName == "GetSitesXml" )
            //{
            //    _serviceInfo.Version = 1.0;
            //}
            //else
            //{
            //    _serviceInfo.Version = 1.1;
            //}
		}

		/// <summary>
		/// This method is used to write the file to the Hard Disk
		/// </summary>
		/// <param name="filepath">Path of the file</param>
		/// <param name="lines">File contents</param>
		/// 
		private void WriteLinesToFile ( String filepath, String lines )
		{
			if ( filepath == null || filepath.Length == 0 )
			{
				return;
			}
			if ( lines == null || lines.Length == 0 )
			{
				return;
			}

			StreamWriter fileWriter = null;
			try
			{
				if ( File.Exists ( filepath ) )
				{
					File.Delete ( filepath );
					fileWriter = File.CreateText ( filepath );
					fileWriter.Write ( lines );
				}
				else
				{
					fileWriter = File.CreateText ( filepath );
					fileWriter.Write ( lines );
				}
			}
			finally
			{
				if ( fileWriter != null )
				{
					fileWriter.Close ();
				}
			}
		}

		#endregion
	}

    public interface IGetValuesProgressHandler
    {
        /// <summary>
        /// Report progress 
        /// </summary>
        /// <param name="intervalNumber">Number of downloaded interval</param>
        /// <param name="totalIntervalsCount">Total intervals count</param>
        /// <param name="timeTaken">Time taken to download current interval (in seconds)</param>
        void Progress(int intervalNumber, int totalIntervalsCount, double timeTaken);
        /// <summary>
        /// Shows that current opeation should be cancelled
        /// </summary>
        bool CancellationPending { get;}
    }
    
}

