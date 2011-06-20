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
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyPropertyChanged("ErrorMessage");
            }
        }

        #endregion

        #region Events

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
        Pending,
        Downloading,
        Downloaded,
        Error,
        Ok,
        OkWithWarnings
    }
}

