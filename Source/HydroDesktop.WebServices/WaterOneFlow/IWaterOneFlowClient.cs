using System;
using System.Collections.Generic;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.WebServices.WaterOneFlow
{
    /// <summary>
    /// Interface for WaterOneFlow SOAP Service Client
    /// </summary>
    public interface IWaterOneFlowClient
    {
        /// <summary>
        /// Gets or sets the directory where temporary WaterML files downloaded by the
        /// client are saved
        /// </summary>
        string DownloadDirectory { get; set; }


        /// <summary>
        /// Gets information about all series available for the specific site
        /// </summary>
        /// <param name="siteCode">the full site code (networkPrefix:siteCode)</param>
        /// <returns>A list of all series. The series don't contain any actual data values
        /// but include all series metadata including the site, variable, source, method\
        /// and quality control level.</returns>
        IList<SeriesMetadata> GetSiteInfo(string siteCode);


        /// <summary>
        /// Gets the information about all time series supported by the web service as a XML document
        /// in the WaterML format
        /// <param name="fullSiteCode">The full site code in NetworkPrefix:SiteCode format</param>
        /// </summary>
        /// <returns>the downloaded xml file name</returns>
        string GetSiteInfoXML(string fullSiteCode);


        /// <summary>
        /// Gets the information about all sites available at this web service.
        /// </summary>
        /// <returns>The list of all sites supported by this web service.</returns>
        IList<Site> GetSites();


        /// <summary>
        /// Gets the information about all sites available at this web service within a bounding box.
        /// </summary>
        /// <param name="westLongitude">Longitude of western edge of bounding box</param>
        /// <param name="southLatitude">Latitude of southern edge of bounding box</param>
        /// <param name="eastLongitude">Longitude of eastern edge of bounding box</param>
        /// <param name="northLatitude">Latitude of northern edge of bounding box</param>
        /// <returns>The list of all sites supported by this web service within a bounding box.</returns>
        IList<Site> GetSites(double westLongitude, double southLatitude, double eastLongitude, double northLatitude);


        /// <summary>
        /// Gets the information about sites within a bounding box, from the web service as a XML document in the WaterML format
        /// </summary>
        /// <param name="westLongitude">Longitude of western edge of bounding box</param>
        /// <param name="southLatitude">Latitude of southern edge of bounding box</param>
        /// <param name="eastLongitude">Longitude of eastern edge of bounding box</param>
        /// <param name="northLatitude">Latitude of northern edge of bounding box</param>
        /// <returns>The downloaded XML file name</returns>
        string GetSitesXML(double westLongitude, double southLatitude, double eastLongitude, double northLatitude);


        /// <summary>
        /// Gets the information about all sites in the web service as a XML document in the WaterML format
        /// </summary>
        /// <returns>The downloaded XML file name</returns>
        string GetSitesXML();


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
        IList<Series> GetValues(string siteCode, string variableCode, DateTime startTime, DateTime endTime);


        /// <summary>
        /// Get the data values for the specific site, variable and time range as a XML document
        /// in the WaterML format
        /// </summary>
        /// <param name="siteCode">the full site code (networkPrefix:siteCode)</param>
        /// <param name="variableCode">the full variable code (vocabularyPrefix:variableCode)</param>
        /// <param name="startTime">the start date/time</param>
        /// <param name="endTime">the end date/time</param>
        /// <returns>the downloaded xml file name</returns>
        string GetValuesXML(string siteCode, string variableCode, DateTime startTime, DateTime endTime);


        /// <summary>
        /// Gets or sets the ServiceID (assigned code) corresponding to this web service
        /// </summary>
        int ServiceID { get; set; }


        /// <summary>
        /// Gets information about the web service used by this web service client
        /// </summary>
        DataServiceInfo ServiceInfo { get; }


        /// <summary>
        /// The URL address of the web service being used
        /// </summary>
        string ServiceURL { get; }
    }
}
