using System;
using System.Collections.Generic;
using HydroDesktop.DataDownload.Downloading.Exceptions;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.Configuration;
using HydroDesktop.WebServices.WaterOneFlow;

namespace HydroDesktop.DataDownload.Downloading
{
    /// <summary>
    /// Methods to download observation data from WaterML web services and save them to 
    /// the ActualData database.
    /// </summary>
    public class Downloader
    {
        #region Variables

        //to store the proxy class for each WaterOneFlow web service for re-use
        private readonly Dictionary<string, WaterOneFlowClient> _services = new Dictionary<string, WaterOneFlowClient>();
        private static readonly object _syncObject = new object();
        private readonly RepositoryManagerSQL _repositoryManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor with default connection string (from settings).
        /// </summary>
        public Downloader()
        {
            _repositoryManager = new RepositoryManagerSQL(DatabaseManager.Instance.GetDbOperationsForCurrentProject());
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
        private WaterOneFlowClient GetWsClientInstance(string wsdl)
        {
            WaterOneFlowClient wsClient;

            lock (_syncObject)
            {
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
            }
            return wsClient;
        }

        /// <summary>
        /// This function is used to get the Values in XML Format based on the selected sites in the layer
        /// </summary>
        /// <param name="info">DownloadInfo</param>
        /// <returns>The name of the xml file</returns>
        /// <exception cref="DownloadXmlException">Some exception during get values from web service</exception>
        public string DownloadXmlDataValues(OneSeriesDownloadInfo info)
        {
            try
            {
                return GetWsClientInstance(info.Wsdl).GetValuesXML(info.FullSiteCode, info.FullVariableCode, info.StartDate, info.EndDate);
            }
            catch(Exception ex)
            {
                throw new DownloadXmlException(ex.Message, ex);
            }
        }

        #endregion

        #region Database Methods

        /// <summary>
        /// Converts the xml file to a data series object.
        /// </summary>
        /// <param name="dInfo">Download info</param>
        /// <returns>The data series object</returns>
        /// <exception cref="DataSeriesFromXmlException">Exception during parsing</exception>
        /// <exception cref="NoSeriesFromXmlException">Throws when no series in xml file</exception>
        /// <exception cref="TooMuchSeriesFromXmlException">Throws when too much series in xml file.</exception>
        public Series DataSeriesFromXml(OneSeriesDownloadInfo dInfo)
        {
            IList<Series> seriesList;

            try
            {
                //get the service version
                WaterOneFlowClient client = GetWsClientInstance(dInfo.Wsdl);
                if (client.ServiceInfo.Version == 1.0)
                {
                    var parser = new WaterOneFlow10Parser();
                    seriesList = parser.ParseGetValues(dInfo.FileName);
                }
                else
                {
                    var parser = new WaterOneFlow11Parser();
                    seriesList = parser.ParseGetValues(dInfo.FileName);
                }
            }
            catch(Exception ex)
            {
                throw new DataSeriesFromXmlException(ex.Message, ex);
            }

            if (seriesList == null || seriesList.Count == 0)
                throw new NoSeriesFromXmlException();
            if (seriesList.Count > 1)
                throw new TooMuchSeriesFromXmlException();

            return seriesList[0];
        }

        /// <summary>
        /// Creates a new DataSeries from a xml file and saves it to database.
        /// This function uses the underlying NHibernate framework to
        /// communicate with the database
        /// </summary>
        /// <param name="series">The data series to be saved</param>
        /// <param name="theme">The theme associated with this data series</param>
        /// <param name="overwriteOption">Option to how save series</param>
        /// <returns>The number of saved data values</returns>
        /// <exception cref="SaveDataSeriesException">Something wrong during SaveDataSeries</exception>
        public int SaveDataSeries(Series series, Theme theme, OverwriteOptions overwriteOption)
        {
            if (series.GetValueCount() == 0) return 0;

            try
            {
                return _repositoryManager.SaveSeries(series, theme, overwriteOption);
            }
            catch(Exception ex)
            {
                throw new SaveDataSeriesException(ex.Message, ex);
            }
        }

        #endregion
    }
}

