using System;
using DotSpatial.Data;
using DotSpatial.Symbology;
using HydroDesktop.Common;

namespace HydroDesktop.DataDownload.LayerInformation
{
    /// <summary>
    /// Class with information about service on the map
    /// </summary>
    public class ServiceInfo : ObservableObject<ServiceInfo>
    {
        #region Fields

        private const string unknown = "Unknown";
        private static readonly ServiceInfo Unknown = new ServiceInfo();

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor of <see cref="ServiceInfo"/>
        /// </summary>
        public ServiceInfo()
        {
            DataSource = unknown;
            SiteName = unknown;
            ValueCount = null;
            ServiceDesciptionUrl = unknown;
            VarCode = unknown;
        }

        #endregion

        #region Properties

        private string _dataSource;

        /// <summary>
        /// DataSource, not null.
        /// </summary>
        public string DataSource
        {
            get { return _dataSource; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                _dataSource = value;
                NotifyPropertyChanged(() => DataSource);
            }
        }

        private string _siteName;

        /// <summary>
        /// SiteName, not null.
        /// </summary>
        public string SiteName
        {
            get { return _siteName; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                _siteName = value;
                NotifyPropertyChanged(() => SiteName);
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
                NotifyPropertyChanged(() => ValueCount);
                NotifyPropertyChanged(() => ValueCountAsString);
            }
        }

        private string _serviceDesciptionUrl;

        /// <summary>
        /// ServiceDesciptionUrl, not null.
        /// </summary>
        public string ServiceDesciptionUrl
        {
            get { return _serviceDesciptionUrl; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                _serviceDesciptionUrl = value;
                NotifyPropertyChanged(() => ServiceDesciptionUrl);
            }
        }

        /// <summary>
        /// ValueCount as string
        /// </summary>
        public string ValueCountAsString
        {
            get { return ValueCount.HasValue ? string.Format("{0} Values", ValueCount) : unknown; }
        }

        /// <summary>
        /// Shows that at least one property has not default value
        /// </summary>
        public bool IsEmpty
        {
            get { return Equals(Unknown); }
        }

        /// <summary>
        /// Site code.
        /// </summary>
        public string SiteCode { get; set; }

        private string _varCode;
        /// <summary>
        /// Variable code, not null.
        /// </summary>
        public string VarCode
        {
            get { return _varCode; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _varCode = value;
            }
        }

        public string ServiceUrl { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string VarName { get; set; }
        public string DataType { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        private IFeatureLayer _layer;
        public IFeatureLayer Layer
        {
            get { return _layer; }
            set
            {
                _layer = value;
                UodateIsDownloaded();
            }
        }

        private IFeature _sourceFeature;
        public IFeature SourceFeature
        {
            get { return _sourceFeature; }
            set
            {
                _sourceFeature = value;
                UodateIsDownloaded();
            }
        }

        private bool _isDownloaded;
        public bool IsDownloaded
        {
            get { return _isDownloaded; }
            set
            {
                _isDownloaded = value;
                NotifyPropertyChanged(() => IsDownloaded);
            }
        }

        private void UodateIsDownloaded()
        {
            if (Layer == null || SourceFeature == null)
            {
                IsDownloaded = false;
                return;
            }

            IsDownloaded = Layer.DataSet.DataTable.Columns.Contains("SeriesID") &&
                           SourceFeature.DataRow["SeriesID"] != DBNull.Value;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Create deep copy into current instance from source
        /// </summary>
        /// <param name="source">Source to copy</param>
        public void Copy(ServiceInfo source)
        {
            Layer = source.Layer;
            SourceFeature = source.SourceFeature;

            DataSource = source.DataSource;
            SiteName = source.SiteName;
            EndDate = source.EndDate;
            Latitude = source.Latitude;
            Longitude = source.Longitude;
            ServiceUrl = source.ServiceUrl;
            SiteCode = source.SiteCode;
            StartDate = source.StartDate;
            VarCode = source.VarCode;
            VarName = source.VarName;
            DataType = source.DataType;
            ValueCount = source.ValueCount;
            ServiceDesciptionUrl = source.ServiceDesciptionUrl;
        }

        public override bool Equals(object obj)
        {
            var pi = obj as ServiceInfo;
            if (pi == null) return false;

            return pi.DataSource == DataSource &&
                   pi.SiteName == SiteName &&
                   pi.ValueCount == ValueCount &&
                   pi.ServiceDesciptionUrl == ServiceDesciptionUrl &&
                   pi.VarCode == VarCode;
        }

        public override int GetHashCode()
        {
            return DataSource.GetHashCode() ^ SiteName.GetHashCode() ^
                   (ValueCount != null ? ValueCount.GetHashCode() : 0) ^ ServiceDesciptionUrl.GetHashCode() ^
                   VarCode.GetHashCode();
        }

        #endregion
    }
}