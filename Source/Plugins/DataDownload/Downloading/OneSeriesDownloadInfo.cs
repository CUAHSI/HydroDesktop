using System;
using System.Collections.Generic;
using DotSpatial.Data;
using HydroDesktop.Common;
using HydroDesktop.Configuration;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.DataDownload.Downloading
{
    /// <summary>
    /// This class is used to pass information required to download
    /// data values using the WaterML GetValues() call
    /// </summary>
    public class OneSeriesDownloadInfo : ObservableObject<OneSeriesDownloadInfo>
    {
        #region Consts

        private const int INITIAL_TIME_TO_DOWNLOAD = 15;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new instance of <see cref="OneSeriesDownloadInfo"/>
        /// </summary>
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
                NotifyPropertyChanged(() => Wsdl);
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
                NotifyPropertyChanged(() => FullSiteCode);
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
                NotifyPropertyChanged(() => FullVariableCode);
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
                NotifyPropertyChanged(() => SiteName);
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
                NotifyPropertyChanged(() => VariableName);
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
                NotifyPropertyChanged(() => StartDate);
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
                NotifyPropertyChanged(() => EndDate);
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
                NotifyPropertyChanged(() => Latitude);
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
                NotifyPropertyChanged(() => Longitude);
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
                NotifyPropertyChanged(() => Status);
                NotifyPropertyChanged(() => StatusAsString);

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
        /// Status as string
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
                NotifyPropertyChanged(() => DownloadTimeTaken);
            }
        }


        private string _errorMessage;
        /// <summary>
        /// Error message. May be not null, if Status == DownloadInfoStatus.Error
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyPropertyChanged(() => ErrorMessage);
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
                NotifyPropertyChanged(() => OverwriteOption);
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
                    case DownloadInfoStatus.OkWithWarning:
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

                NotifyPropertyChanged(() => DownloadedChunksPercent);
            }
        }

        #endregion
    }
}

