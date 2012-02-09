using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroDesktop.Interfaces.ObjectModel
{
    /// <summary>
    /// Basic information about the series as returned by HIS Central
    /// </summary>
    public class SeriesDataCart
    {
        /// <summary>
        /// code of the web service
        /// </summary>
        public string ServCode { get; set; }
        /// <summary>
        /// URL of the web service (WSDL)
        /// </summary>
        public string ServURL { get; set; }
        /// <summary>
        /// The full site code (NetworkCode:SiteCode)
        /// </summary>
        public string SiteCode { get; set; }
        /// <summary>
        /// The full variable code (VocabularyPrefix:Variable)
        /// </summary>
        public string VariableCode { get; set; }
        /// <summary>
        /// Name of the variable
        /// </summary>
        public string VariableName { get; set; }
        /// <summary>
        /// Begin date (date of first available observation in the series)
        /// </summary>
        public DateTime BeginDate { get; set; }
        /// <summary>
        /// End date (date of last available observation in the series)
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// Total number of DataValues provided by the service
        /// </summary>
        public int ValueCount { get; set; }
        /// <summary>
        /// Then name of the site
        /// </summary>
        public string SiteName { get; set; }
        /// <summary>
        /// Latitude of the site
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Longitude of the site
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// Data type of the values in the series (average, minimum, maximum..)
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// Value Type
        /// </summary>
        public string ValueType { get; set; }
        /// <summary>
        /// Sample Medium (water, air, other, not applicable)
        /// </summary>
        public string SampleMedium { get; set; }
        /// <summary>
        /// The time unit of the time support period
        /// </summary>
        public string TimeUnit { get; set; }
        /// <summary>
        /// The general category
        /// </summary>
        public string GeneralCategory { get; set; }
        /// <summary>
        /// The time support. This is the length
        /// of the period following the value DateTime
        /// for which the value is valid
        /// </summary>
        public double TimeSupport { get; set; }
        /// <summary>
        /// This is the concept keyword returened by HIS Central
        /// If the variable is not registered, then an empty
        /// keyword is returned.
        /// </summary>
        public string ConceptKeyword { get; set; }
    }
}
