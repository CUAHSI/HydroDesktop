using System;
using DotSpatial.Data;
using DotSpatial.Symbology;
using HydroDesktop.Plugins.DataDownload.Downloading;
using HydroDesktop.Plugins.DataDownload.LayerInformation;

namespace HydroDesktop.Plugins.DataDownload
{
    /// <summary>
    /// Converts classes from one type to another
    /// </summary>
    static class ClassConvertor
    {
        /// <summary>
        /// Converts instance of ServiceInfo into OneSeriesDownloadInfo
        /// </summary>
        /// <param name="serviceInfo">Instance of ServiceInfo</param>
        /// <returns>Instance of OneSeriesDownloadInfo</returns>
        /// <exception cref="ArgumentNullException"><para>serviceInfo</para> must be not null.</exception>
        public static OneSeriesDownloadInfo ServiceInfoToOneSeriesDownloadInfo(ServiceInfo serviceInfo)
        {
            if (serviceInfo == null) throw new ArgumentNullException("serviceInfo");

            var oneSeries = new OneSeriesDownloadInfo
            {
                SiteName = serviceInfo.SiteName,
                FullSiteCode = serviceInfo.SiteCode,
                FullVariableCode = serviceInfo.VarCode,
                Wsdl = serviceInfo.ServiceUrl,
                StartDate = !serviceInfo.IsDownloaded? serviceInfo.StartDate : serviceInfo.EndDate,
                EndDate = !serviceInfo.IsDownloaded? serviceInfo.EndDate : DateTime.Now,
                VariableName = serviceInfo.VarName,
                Latitude = serviceInfo.Latitude,
                Longitude = serviceInfo.Longitude,
                SourceFeature = serviceInfo.SourceFeature,
            };
            if (serviceInfo.IsDownloaded)
                oneSeries.OverwriteOption = Interfaces.OverwriteOptions.Overwrite;
            if (serviceInfo.ValueCount.HasValue)
                oneSeries.EstimatedValuesCount = serviceInfo.ValueCount.Value;

            return oneSeries;
        }

        /// <summary>
        /// Converts instance of IFeature into ServiceInfo
        /// </summary>
        /// <param name="feature">Instance of IFeature</param>
        /// <param name="layer">Layer</param>
        /// <returns>Instance of ServiceInfo</returns>
        /// <exception cref="ArgumentNullException"><para>feature</para> must be not null.</exception>
        public static ServiceInfo IFeatureToServiceInfo(IFeature feature, IFeatureLayer layer)
        {
            if (feature == null) throw new ArgumentNullException("feature");

            var getColumnValue = (Func<string, string>)(column => (feature.DataRow[column].ToString()));
            var pInfo = new ServiceInfo{SourceFeature = feature, Layer = layer};
           
            foreach (var fld in feature.ParentFeatureSet.GetColumns())
            {
                var strValue = getColumnValue(fld.ColumnName);

                switch (fld.ColumnName)
                {
                    case "ServiceCode":
                        pInfo.DataSource = strValue;
                        break;
                    case "SiteName":
                        pInfo.SiteName = strValue;
                        break;
                    case "SiteCode":
                        pInfo.SiteCode = strValue;
                        break;
                    case "VarCode":
                        pInfo.VarCode = strValue;
                        break;
                    case "DataType":
                        pInfo.DataType = strValue;
                        break;
                    case "ValueCount":
                        {
                            int val;
                            pInfo.ValueCount = !Int32.TryParse(strValue, out val) ? (int?)null : val;
                        }
                        break;
                    case "ServiceURL":
                        pInfo.ServiceUrl = strValue;
                        break;
                    case "StartDate":
                        {
                            DateTime val;
                            pInfo.StartDate = !DateTime.TryParse(strValue, out val) ? DateTime.MinValue : val;
                        }
                        break;
                    case "EndDate":
                        {
                            DateTime val;
                            pInfo.EndDate = !DateTime.TryParse(strValue, out val) ? DateTime.MinValue : val;
                        }
                        break;
                    case "VarName":
                        pInfo.VarName = strValue;
                        break;
                    case "Latitude":
                        {
                            double val;
                            pInfo.Latitude = !Double.TryParse(strValue, out val) ? 0 : val;
                        }
                        break;
                    case "Longitude":
                        {
                            double val;
                            pInfo.Longitude = !Double.TryParse(strValue, out val) ? 0 : val;
                        }
                        break;

                }
            }

            return pInfo;
        }

        /// <summary>
        /// Converts instance of IFeature into OneSeriesDownloadInfo
        /// </summary>
        /// <param name="feature">Instance of IFeature</param>
        /// <param name="layer">Layer</param>
        /// <returns>Instance of OneSeriesDownloadInfo</returns>
        /// <exception cref="ArgumentNullException"><para>feature</para> must be not null.</exception>
        public static OneSeriesDownloadInfo IFeatureToOneSeriesDownloadInfo(IFeature feature, IFeatureLayer layer)
        {
            if (feature == null) throw new ArgumentNullException("feature");

            return ServiceInfoToOneSeriesDownloadInfo(IFeatureToServiceInfo(feature, layer));
        }
    }
}
