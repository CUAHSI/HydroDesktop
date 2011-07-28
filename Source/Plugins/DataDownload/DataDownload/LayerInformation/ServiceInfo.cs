using System;
using System.ComponentModel;

namespace HydroDesktop.DataDownload.LayerInformation
{
    /// <summary>
    /// Class with information about service on the map
    /// </summary>
    public class ServiceInfo : INotifyPropertyChanged
    {
        #region Fields

        private const string unknown = "Unknown";
        private static readonly ServiceInfo Unknown = new ServiceInfo();

        #endregion

        #region Constructors

        public ServiceInfo()
        {
            DataSource = unknown;
            SiteName = unknown;
            ValueCount = null;
            ServiceDesciptionUrl = unknown;
        }

        #endregion

        private string _dataSource;

        /// <summary>
        /// DataSource
        /// </summary>
        public string DataSource
        {
            get { return _dataSource; }
            set
            {
                _dataSource = value;
                NotifyPropertyChanged("DataSource");
            }
        }

        private string _siteName;

        /// <summary>
        /// SiteName
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

        private int? _valueCount;

        /// <summary>
        /// ValueCount
        /// </summary>
        public int? ValueCount
        {
            get { return _valueCount; }
            set
            {
                _valueCount = value;
                NotifyPropertyChanged("ValueCount");
                NotifyPropertyChanged("ValueCountAsString");
            }
        }

        private string _serviceDesciptionUrl;

        /// <summary>
        /// ServiceDesciptionUrl
        /// </summary>
        public string ServiceDesciptionUrl
        {
            get { return _serviceDesciptionUrl; }
            set
            {
                _serviceDesciptionUrl = value;
                NotifyPropertyChanged("ServiceDesciptionUrl");
            }
        }

        public string ValueCountAsString
        {
            get { return ValueCount.HasValue ? string.Format("{0} values", ValueCount) : unknown; }
        }

        /// <summary>
        /// Shows that at least one property has not default value
        /// </summary>
        public bool IsEmpty
        {
            get { return Equals(Unknown); }
        }

        public string SiteCode { get; set; }

        public string VarCode { get; set; }

        public string ServiceUrl { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string VarName { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public override bool Equals(object obj)
        {
            var pi = obj as ServiceInfo;
            if (pi == null) return false;

            return pi.DataSource == DataSource &&
                   pi.SiteName == SiteName &&
                   pi.ValueCount == ValueCount &&
                   pi.ServiceDesciptionUrl == ServiceDesciptionUrl;
        }

        public override int GetHashCode()
        {
            return DataSource.GetHashCode() ^ SiteName.GetHashCode() ^
                   ValueCount.GetHashCode() ^ ServiceDesciptionUrl.GetHashCode();
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}