using System;
using DotSpatial.Data;
using DotSpatial.Symbology;

namespace HydroDesktop.DataDownload.LayerInformation
{
    static class ServiceInfoHelper
    {
        /// <summary>
        /// Converts instance of IFeature into ServiceInfo
        /// </summary>
        /// <param name="feature">Instance of IFeature</param>
        /// <param name="layer">Layer</param>
        /// <returns>Instance of ServiceInfo</returns>
        /// <exception cref="ArgumentNullException"><para>feature</para> must be not null.</exception>
        public static ServiceInfo ToServiceInfo(this IFeature feature, IFeatureLayer layer)
        {
            if (feature == null) throw new ArgumentNullException("feature");

            var getColumnValue = (Func<string, string>)(column => (feature.DataRow[column].ToString()));
            var pInfo = new ServiceInfo{SourceFeature = feature, Layer = layer};
            
            foreach (var fld in feature.ParentFeatureSet.GetColumns())
            {
                var strValue = getColumnValue(fld.ColumnName);

                switch (fld.ColumnName.ToLowerInvariant())
                {
                    case "servicecode":
                        pInfo.DataSource = strValue;
                        break;
                    case "sitename":
                        pInfo.SiteName = strValue;
                        break;
                    case "sitecode":
                        pInfo.SiteCode = strValue;
                        break;
                    case "varcode":
                        pInfo.VarCode = strValue;
                        break;
                    case "datatype":
                        pInfo.DataType = strValue;
                        break;
                    case "valuecount":
                        {
                            int val;
                            pInfo.ValueCount = !Int32.TryParse(strValue, out val) ? (int?)null : val;
                        }
                        break;
                    case "serviceurl":
                        pInfo.ServiceUrl = strValue;
                        break;
                    case "startdate":
                        {
                            DateTime val;
                            pInfo.StartDate = !DateTime.TryParse(strValue, out val) ? DateTime.MinValue : val;
                        }
                        break;
                    case "enddate":
                        {
                            DateTime val;
                            pInfo.EndDate = !DateTime.TryParse(strValue, out val) ? DateTime.MinValue : val;
                        }
                        break;
                    case "varname":
                        pInfo.VarName = strValue;
                        break;
                    case "latitude":
                        {
                            double val;
                            pInfo.Latitude = !Double.TryParse(strValue, out val) ? 0 : val;
                        }
                        break;
                    case "longitude":
                        {
                            double val;
                            pInfo.Longitude = !Double.TryParse(strValue, out val) ? 0 : val;
                        }
                        break;
                    case "watermluri":
                        pInfo.WaterMLUri = strValue;
                        break;

                }
            }

            return pInfo;
        }
    }
}
