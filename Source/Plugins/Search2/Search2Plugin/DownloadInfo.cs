using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


    /// <summary>
    /// This class is used to pass information required to download
    /// data values using the WaterML GetValues() call
    /// </summary>
    public class DownloadInfo
    {
        
        public string Wsdl;

        public string FullSiteCode;

        public string FullVariableCode;

        public string SiteName;

        public string VariableName;

        /// <summary>
        /// The 'start date'
        /// </summary>
        public DateTime StartDate;

        /// <summary>
        /// The 'end date'
        /// </summary>
        public DateTime EndDate;

        public double Latitude;

        public double Longitude;
    }

