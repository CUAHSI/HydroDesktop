using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using DotSpatial.Data;
using HydroDesktop.Configuration;
using HydroDesktop.DataDownload.LayerInformation;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices.WaterOneFlow;

namespace HydroDesktop.DataDownload.Downloading
{
    /// <summary>
    /// This class is used to pass information required to download
    /// data values using the WaterML GetValues() call
    /// </summary>
    public class OneSeriesDownloadInfo : INotifyPropertyChanged
    {
        private readonly ServiceInfo _serviceInfo;

        #region Consts

        internal const string PROPERTY_Wsdl = "Wsdl";
        internal const string PROPERTY_FullSiteCode = "FullSiteCode";
        internal const string PROPERTY_FullVariableCode = "FullVariableCode";
        internal const string PROPERTY_SiteName = "SiteName";
        internal const string PROPERTY_VariableName = "VariableName";
        internal const string PROPERTY_StatusAsString = "StatusAsString";

        private const int INITIAL_TIME_TO_DOWNLOAD = 15;

        #endregion

        #region Constructors

        /// <summary>
        /// Create instance of <see cref="OneSeriesDownloadInfo"/>
        /// </summary>
        /// <param name="serviceInfo">Service info</param>
        /// <exception cref="ArgumentNullException">Raises if <see cref="serviceInfo"/> is null.</exception>
        public OneSeriesDownloadInfo(ServiceInfo serviceInfo)
        {
            if (serviceInfo == null) throw new ArgumentNullException("serviceInfo");
            Contract.EndContractBlock();

            _serviceInfo = serviceInfo;
            Status = DownloadInfoStatus.Pending;

            OverwriteOption = serviceInfo.IsDownloaded? OverwriteOptions.Overwrite: 
                               (OverwriteOptions)Enum.Parse(typeof (OverwriteOptions), Settings.Instance.DownloadOption);
            EstimatedValuesCount = serviceInfo.ValueCount.HasValue ? serviceInfo.ValueCount.Value : -1;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Uri of WaterML, may be null.
        /// </summary>
        public string WaterMLUri
        {
            get { return _serviceInfo.WaterMLUri; }
        }

        public IFeature SourceFeature
        {
            get { return _serviceInfo.SourceFeature; }
        }

        public IEnumerable<Series> ResultSeries { get; set; }
        
        /// <summary>
        /// Service url
        /// </summary>
        public string Wsdl
        {
            get { return _serviceInfo.ServiceUrl; }
        }
        
        /// <summary>
        /// Site code
        /// </summary>
        public string FullSiteCode
        {
            get { return _serviceInfo.SiteCode; }
        }
        
        /// <summary>
        /// Variable code
        /// </summary>
        public string FullVariableCode
        {
            get { return _serviceInfo.VarCode; }
        }
        
        /// <summary>
        /// Site name
        /// </summary>
        public string SiteName
        {
            get { return _serviceInfo.SiteName; }
        }
        
        /// <summary>
        /// Variable name
        /// </summary>
        public string VariableName
        {
            get { return _serviceInfo.VarName; }
        }
        
        /// <summary>
        /// Start date
        /// </summary>
        public DateTime StartDate
        {
            get {return _serviceInfo.StartDate; }
        }
        
        /// <summary>
        /// End date
        /// </summary>
        public DateTime EndDate
        {
            get { return !_serviceInfo.IsDownloaded? _serviceInfo.EndDate : DateTime.Now; }
        }

        private DownloadInfoStatus _status;
        /// <summary>
        /// Status of current item
        /// </summary>
        public DownloadInfoStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyPropertyChanged(PROPERTY_StatusAsString);

                if (_status == DownloadInfoStatus.Pending)
                {
                    DownloadTimeTaken = new TimeSpan();
                    ErrorMessage = null;
                    FilesWithData = null;
                    EstimatedTimeToDownload = INITIAL_TIME_TO_DOWNLOAD;
                    DownloadedChunksPercent = 0;
                }
            }
        }
        
        /// <summary>
        /// Status of current item (text presentation)
        /// </summary>
        public string StatusAsString
        {
            get
            {
                return Status != DownloadInfoStatus.Downloading
                           ? Status.ToString()
                           : string.Format("{0} ({1}%)", Status, (int)DownloadedChunksPercent);
            }
        }

        /// <summary>
        /// Time interval, taken to downloading
        /// </summary>
        public TimeSpan DownloadTimeTaken { get; set; }


        /// <summary>
        /// Error mesage. May be not null, if Status == DownloadInfoStatus.Error
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Collection of files with downloaded data series.
        /// </summary>
        public IEnumerable<string> FilesWithData { get; set; }

        /// <summary>
        /// Estimated count of values
        /// </summary>
        public int EstimatedValuesCount { get; set; }


        /// <summary>
        /// Description of downloaded series
        /// </summary>
        public string SeriesDescription
        {
            get
            {
                return SiteName + "|" + VariableName;
            }
        }

        public OverwriteOptions OverwriteOption { get; private set; }

        public double EstimatedTimeToDownload { get; set; }

        public double Progress
        {
            get
            {
                const int savePart = 5;

                switch (Status)
                {
                   case DownloadInfoStatus.Pending:
                        return 0;
                    case DownloadInfoStatus.Error:
                    case DownloadInfoStatus.OkWithWarnings:
                    case DownloadInfoStatus.Ok:
                        return 100;
                    case DownloadInfoStatus.Downloaded:
                        return 100 - savePart;
                    case DownloadInfoStatus.Downloading:
                        return DownloadedChunksPercent *((100.0 - savePart)/100.0);
                    default:
                        return 0;
                }
            }
        }

        private double _downloadedChunksPercent;
        public double DownloadedChunksPercent
        {
            get { return _downloadedChunksPercent; }
            set
            {
                _downloadedChunksPercent = value;

                NotifyPropertyChanged(PROPERTY_StatusAsString);
            }
        }

        public IWaterOneFlowParser XmlParser { get; set; }
    
        #endregion

        
        #region INotifyPropertyChanged implementation

        /// <summary>
        /// Raises when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    /// <summary>
    /// Statuses of DownloadInfo
    /// </summary>
    public enum DownloadInfoStatus
    {
        /// <summary>
        /// Pending (awaiting to downloading)
        /// </summary>
        Pending,
        /// <summary>
        /// Downloading
        /// </summary>
        Downloading,
        /// <summary>
        /// Downloaded
        /// </summary>
        Downloaded,
        /// <summary>
        /// Some error occurred during downloading or saving
        /// </summary>
        Error,
        /// <summary>
        /// Downloaded and saved without errors/warnings.
        /// </summary>
        Ok,
        /// <summary>
        /// Downloaded and saved with warnings.
        /// </summary>
        OkWithWarnings
    }
}

