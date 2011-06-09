using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Xml;
using DotSpatial.Data;
using DotSpatial.Controls;
using HydroDesktop.Database;
using HydroDesktop.WebServices;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.Configuration;
using HydroDesktop.WebServices.WaterOneFlow;

/// <summary>
/// Methods to download observation data from WaterML web services and save them to 
/// the ActualData database.
/// </summary>
public class Downloader
{
    #region Variables

    //to store the proxy class for each WaterOneFlow web service for re-use
    private Dictionary<string, WaterOneFlowClient> _services = new Dictionary<string, WaterOneFlowClient>();

    private DateTime _startDate;
    private DateTime _endDate;
    private string _themeName;
    private string _themeDescription;
    private int _numDownloadedSeries = 0;
    private string _connectionString;

    private IFeatureSet _featureSet = null;
    
    #endregion

    #region Constructors

    public Downloader()
    {

    }

    #endregion

    #region Properties
    
    /// <summary>
    /// Gets or sets the start date of all downloaded time series
    /// </summary>
    public DateTime StartDate
    {
        get { return _startDate; }
        set { _startDate = value; }
    }

    /// <summary>
    /// Gets or sets the end date of all downloaded  time series
    /// </summary>
    public DateTime EndDate
    {
        get { return _endDate; }
        set { _endDate = value; }
    }

    /// <summary>
    /// Gets or sets the theme name (name of feature set where site locations are stored)
    /// </summary>
    public string ThemeName
    {
        get { return _themeName; }
        set { _themeName = value; }
    }

    /// <summary>
    /// Gets or sets the theme description
    /// </summary>
    public string ThemeDescription
    {
        get { return _themeDescription; }
        set { _themeDescription = value; }
    }
    /// <summary>
    /// Gets the number of data series that were downloaded
    /// </summary>
    public int NumDownloadedSeries
    {
        get { return _numDownloadedSeries; }
    }

    public IFeatureSet ThemeFeatureSet
    {
        get { return _featureSet; }
        set { _featureSet = value; }
    }

    /// <summary>
    /// Gets or sets the database connection string
    /// </summary>
    public string ConnectionString
    {
        get { return _connectionString; }
        set { _connectionString = value; }
    }

    #endregion

    #region File I/O Methods
    /// <summary>
    /// Gets the temporary directory for xml files downloaded
    /// by HydroDesktop
    /// </summary>
    /// <returns>the directory path</returns>
    public string GetXmlTempDirectory()
    {
        //Check if we need to create a temporary folder for storing the xml file
        string tempDirectory = Path.Combine(Path.GetTempPath(), "HydroDesktop");
        if (Directory.Exists(tempDirectory) == false)
        {
            try
            {
                Directory.CreateDirectory(tempDirectory);
            }
            catch
            {
                tempDirectory = Path.GetTempPath();
            }
        }
        return tempDirectory;
    }
    #endregion

    #region Web Service Methods

    /// <summary>
    /// Gets an instance of a WaterOneFlow client to be used with
    /// the URL specified in the Download info. This instance is retrieved 
    /// from the dictionary cache or created if necessary.
    /// </summary>
    /// <param name="wsdl">The URL of the web service main page</param>
    /// <returns>the appropriate WaterOneFlow client</returns>
    public WaterOneFlowClient GetWsClientInstance(string wsdl)
    {
        WaterOneFlowClient wsClient = null;

        //To Access the dynamic WSDLs
        if (_services.ContainsKey(wsdl))
        {
            wsClient = _services[wsdl];
        }
        else
        {
            wsClient = new WaterOneFlowClient(wsdl);
            _services.Add(wsdl, wsClient);
        }
        return wsClient;
    }

    /// <summary>
    /// This function is used to get the Values in XML Format based on the selected sites in the layer
    /// </summary>
    /// <param name="SiteCode">Selected point sitecode</param>
    /// <param name="VariableCode">Selected point VariableCode</param>
    /// <returns>The name of the xml file</returns>
    public string DownloadXmlDataValues(DownloadInfo info)
    {
        //temporary fix - replace water.sdsc.edu by river.sdsc.edu
        //if (info.Wsdl.Contains("water.sdsc.edu"))
        //{
        //    info.Wsdl = info.Wsdl.Replace("water.sdsc.edu", "river.sdsc.edu");
        //}
        
        WaterOneFlowClient wsClient= GetWsClientInstance(info.Wsdl);

        //call the web service
        string xmlFileName = string.Empty;
        try
        {
            xmlFileName = wsClient.GetValuesXML(info.FullSiteCode, info.FullVariableCode, info.StartDate, info.EndDate);
        }
        catch
        {
            //TODO: We should log that there was a problem accessing the web service
        }

        return xmlFileName;
    }

    #endregion

    #region Database Methods

    /// <summary>
    /// Converts the xml file to a list of data series objects.
    /// In most cases the list will only contain one object.
    /// </summary>
    /// <param name="xmlFile">The name of the xml file</param>
    /// <returns>the data series list or null in case of error</returns>
    public IList<Series> DataSeriesFromXml(string xmlFile, DownloadInfo dInfo)
    {
        IList<Series> seriesList = null;
        try
        {
            //get the service version
            WaterOneFlowClient client = GetWsClientInstance(dInfo.Wsdl);
            if (client.ServiceInfo.Version == 1.0)
            {
                WaterOneFlow10Parser parser = new WaterOneFlow10Parser();
                seriesList = parser.ParseGetValues(xmlFile);
            }
            else
            {
                WaterOneFlow11Parser parser = new WaterOneFlow11Parser();
                seriesList = parser.ParseGetValues(xmlFile);
            }
        }
        catch
        {
            //in case of error - return an empty list
        }
        return seriesList;
    }

    /// <summary>
    /// creates a new DataSeries from a xml file and saves it to database.
    /// This function uses the underlying NHibernate framework to
    /// communicate with the database
    /// </summary>
    /// <param name="series">The data series to be saved</param>
    /// <param name="theme">The theme associated with this data series</param>
    /// <param name="overwrite">Determines how to handle duplicate data values.</param>
    /// <returns>The number of saved data values</returns>
    public int SaveDataSeries(Series series, Theme theme, OverwriteOptions overwrite)
    {
        //check if the series has values
        //TODO: we should display error message ('series has no data values')
        // no pass an event
        if (series.GetValueCount() == 0) return 0;

        try
        {
            RepositoryManagerSQL manager = new RepositoryManagerSQL(DatabaseTypes.SQLite, ConnectionString);
            OverwriteOptions overwriteOption = (OverwriteOptions)Enum.Parse(typeof(OverwriteOptions),Settings.Instance.DownloadOption);
            return manager.SaveSeries(series, theme, overwriteOption);
        }
        catch
        {
            return 0;
        }
    }

    public DateTime ConvertDateTime(object timeObj)
    {
        if (timeObj == null) return DateTime.MinValue;

        string timeStr = timeObj.ToString();
        timeStr = timeStr.Replace("T", " ");
        return Convert.ToDateTime(timeStr);
    }

    #endregion
}

