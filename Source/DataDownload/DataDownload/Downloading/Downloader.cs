using System;
using System.Collections.Generic;
using System.Reflection;
using HydroDesktop.DataDownload.Downloading.Exceptions;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;
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
        private static readonly Dictionary<string, WaterOneFlowClient> _services = new Dictionary<string, WaterOneFlowClient>();
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
        private static WaterOneFlowClient GetWsClientInstance(string wsdl)
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
        /// <param name="getValueProgressHandler">Progress handler</param>
        /// <returns>Collection of of the xml files with values</returns>
        /// <exception cref="DownloadXmlException">Some exception during get values from web service</exception>
        public IEnumerable<string> DownloadXmlDataValues(OneSeriesDownloadInfo info, IGetValuesProgressHandler getValueProgressHandler)
        {
            try
            {
                var client = GetWsClientInstance(info.Wsdl);
                info.XmlParser = client.Parser;
                return client.GetValuesXML(info.FullSiteCode, info.FullVariableCode,
                                           info.StartDate, info.EndDate,
                                           info.EstimatedValuesCount,
                                           getValueProgressHandler);
            }
            catch (Exception ex)
            {
                Exception exToWrap;
                if (ex is TargetInvocationException &&
                    ex.InnerException != null)
                    exToWrap = ex.InnerException;
                else
                    exToWrap = ex;

                throw new DownloadXmlException(exToWrap.Message, exToWrap);
            }
        }

        #endregion

        #region Database Methods

        /// <summary>
        /// Converts the xml file to a data series object.
        /// </summary>
        /// <param name="dInfo">Download info</param>
        /// <returns>Collection of the data series objects</returns>
        /// <exception cref="DataSeriesFromXmlException">Exception during parsing</exception>
        /// <exception cref="NoSeriesFromXmlException">Throws when no series in xml file</exception>
        public IEnumerable<Series> DataSeriesFromXml(OneSeriesDownloadInfo dInfo)
        {
            var parser = dInfo.XmlParser;
            var result = new List<Series>();
            foreach (var xmlFile in dInfo.FilesWithData)
            {
                IList<Series> seriesList;
                try
                {
                    seriesList = parser.ParseGetValues(xmlFile);
                }
                catch (Exception ex)
                {
                    throw new DataSeriesFromXmlException(ex.Message, ex);
                }
                if (seriesList == null || seriesList.Count == 0)
                    throw new NoSeriesFromXmlException();

                result.AddRange(seriesList);
            }
            return result;
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

        public int SaveDataSeries(IEnumerable<Series> series, Theme theme, OverwriteOptions overwriteOption)
        {
            var result = 0;
            var enumerator = series.GetEnumerator();
            while (enumerator.MoveNext())
            {
                result += SaveDataSeries(enumerator.Current, theme, overwriteOption);
            }
            return result;
        }


        #endregion
    }
}

