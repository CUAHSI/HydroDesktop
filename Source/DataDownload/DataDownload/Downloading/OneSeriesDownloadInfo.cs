using System;
using System.Collections.Generic;
using System.ComponentModel;
using DotSpatial.Data;
using HydroDesktop.Configuration;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.DataDownload.Downloading
{
    /// <summary>
    /// This class is used to pass information required to download
    /// data values using the WaterML GetValues() call
    /// </summary>
    public class OneSeriesDownloadInfo : INotifyPropertyChanged
    {
        #region Consts

        internal const string PROPERTY_Wsdl = "Wsdl";
        internal const string PROPERTY_FullSiteCode = "FullSiteCode";
        internal const string PROPERTY_FullVariableCode = "FullVariableCode";
        internal const string PROPERTY_SiteName = "SiteName";
        internal const string PROPERTY_VariableName = "VariableName";
        internal const string PROPERTY_StartDate = "StartDate";
        internal const string PROPERTY_EndDate = "EndDate";
        internal const string PROPERTY_Latitude = "Latitude";
        internal const string PROPERTY_Longitude = "Longitude";
        internal const string PROPERTY_Status = "Status";
        internal const string PROPERTY_StatusAsString = "StatusAsString";
        internal const string PROPERTY_DownloadTimeTaken = "DownloadTimeTaken";
        internal const string PROPERTY_ErrorMessage = "ErrorMessage";
        internal const string PROPERTY_OverwriteOption = "OverwriteOption";

        private const int INITIAL_TIME_TO_DOWNLOAD = 15;

        #endregion

        #region Constructors

        public OneSeriesDownloadInfo()
        {
            Status = DownloadInfoStatus.Pending;
            EstimatedValuesCount = -1;
        }

        #endregion

        #region Properties

        public IFeature SourceFeature { get; set; }
        public IEnumerable<Series> ResultSeries { get; set; }

        private string _wsdl;
        /// <summary>
        /// Service url
        /// </summary>
        public string Wsdl
        {
            get { return _wsdl; }
            set
            {
                _wsdl = value;
                NotifyPropertyChanged(PROPERTY_Wsdl);
            }
        }

        private string _fullSiteCode;
        /// <summary>
        /// Site code
        /// </summary>
        public string FullSiteCode
        {
            get { return _fullSiteCode; }
            set
            {
                _fullSiteCode = value;
                NotifyPropertyChanged(PROPERTY_FullSiteCode);
            }
        }

        private string _fullVariableCode;
        /// <summary>
        /// Variable code
        /// </summary>
        public string FullVariableCode
        {
            get { return _fullVariableCode; }
            set
            {
                _fullVariableCode = value;
                NotifyPropertyChanged(PROPERTY_FullVariableCode);
            }
        }

        private string _siteName;
        /// <summary>
        /// Site name
        /// </summary>
        public string SiteName
        {
            get { return _siteName; }
            set
            {
                _siteName = value;
                NotifyPropertyChanged(PROPERTY_SiteName);
            }
        }

        private string _variableName;
        /// <summary>
        /// Variable name
        /// </summary>
        public string VariableName
        {
            get { return _variableName; }
            set
            {
                _variableName = value;
                NotifyPropertyChanged(PROPERTY_VariableName);
            }
        }

        private DateTime _startDate;
        /// <summary>
        /// Start date
        /// </summary>
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                NotifyPropertyChanged(PROPERTY_StartDate);
            }
        }

        private DateTime _endDate;
        /// <summary>
        /// End date
        /// </summary>
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                NotifyPropertyChanged(PROPERTY_EndDate);
            }
        }

        private double _latitude;
        /// <summary>
        /// Latitude
        /// </summary>
        public double Latitude
        {
            get { return _latitude; }
            set
            {
                _latitude = value;
                NotifyPropertyChanged(PROPERTY_Latitude);
            }
        }

        private double _longitude;
        /// <summary>
        /// Longitude
        /// </summary>
        public double Longitude
        {
            get { return _longitude; }
            set
            {
                _longitude = value;
                NotifyPropertyChanged(PROPERTY_Longitude);
            }
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
                NotifyPropertyChanged(PROPERTY_Status);
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
        
        public string StatusAsString
        {
            get
            {
                return Status != DownloadInfoStatus.Downloading
                           ? Status.ToString()
                           : string.Format("{0} ({1}%)", Status, (int)DownloadedChunksPercent);
            }
        }

        private TimeSpan _downloadTimeTaken;
        /// <summary>
        /// Time interval, taken to downloading
        /// </summary>
        public TimeSpan DownloadTimeTaken
        {
            get { return _downloadTimeTaken; }
            set
            {
                _downloadTimeTaken = value;
                NotifyPropertyChanged(PROPERTY_DownloadTimeTaken);
            }
        }


        private string _errorMessage;
        /// <summary>
        /// Error mesage. May be not null, if Status == DownloadInfoStatus.Error
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyPropertyChanged(PROPERTY_ErrorMessage);
            }
        }

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

        private OverwriteOptions _overwriteOption = (OverwriteOptions)Enum.Parse(typeof(OverwriteOptions), Settings.Instance.DownloadOption);

        public OverwriteOptions OverwriteOption
        {
            get { return _overwriteOption; }
            set
            {
                _overwriteOption = value;
                NotifyPropertyChanged(PROPERTY_OverwriteOption);
            }
        }

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

        #endregion

        #region Events

        /// <summary>
        /// Raises when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private methods

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
        /// Pending (awaitng to downloading)
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
        /// Some error occured during downloading or saving
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

