using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroDesktop.Search
{
    /// <summary>
    /// Basic information about the series as returned by HIS Central
    /// </summary>
    public class SeriesDataCart
    {
        public string ServCode { get; set; }
        public string ServURL { get; set; }
        public string SiteCode { get; set; }
        public string VariableCode { get; set; }
        public string VariableName { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ValueCount { get; set; }
        public string SiteName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string DataType { get; set; }
        public string ValueType { get; set; }
        public string SampleMedium { get; set; }
        public string TimeUnit { get; set; }
        public string GeneralCategory { get; set; }
        public double TimeSupport { get; set; }
    }
}
