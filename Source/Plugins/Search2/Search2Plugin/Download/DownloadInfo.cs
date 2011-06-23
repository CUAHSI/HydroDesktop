using System;
using System.ComponentModel;

namespace HydroDesktop.Search.Download
{
    /// <summary>
    /// This class is used to pass information required to download
    /// data values using the WaterML GetValues() call
    /// </summary>
    public class DownloadInfo : INotifyPropertyChanged
    {
        #region Constructors

        public DownloadInfo()
        {
            Status = DownloadInfoStatus.Pending;
        }

        #endregion

        #region Properties

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
                NotifyPropertyChanged("Wsdl");
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
                NotifyPropertyChanged("FullSiteCode");
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
                NotifyPropertyChanged("FullVariableCode");
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
                NotifyPropertyChanged("SiteName");
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
                NotifyPropertyChanged("VariableName");
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
                NotifyPropertyChanged("StartDate");
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
                NotifyPropertyChanged("EndDate");
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
                NotifyPropertyChanged("Latitude");
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
                NotifyPropertyChanged("Longitude");
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
                NotifyPropertyChanged("Status");
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
                NotifyPropertyChanged("DownloadTimeTaken");
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
                NotifyPropertyChanged("ErrorMessage");
            }
        }

        /// <summary>
        /// File name containg downloaded data series.
        /// </summary>
        public string FileName { get; set; }

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

