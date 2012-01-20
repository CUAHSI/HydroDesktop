using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.Interfaces.ObjectModel;

namespace DroughtAnalysis
{
    /// <summary>
    /// Parameter settings for the drought analysis script
    /// </summary>
    public class DroughtSettings
    {
        /// <summary>
        /// Type of drought (meteo, hydro, agro)
        /// </summary>
        public TypeOfDrought DroughtType { get; set; }

        /// <summary>
        /// Current path to R
        /// </summary>
        public string PathToR { get; set; }

        /// <summary>
        /// Output Directory
        /// </summary>
        public string OutputDirectory { get; set; }

        /// <summary>
        /// Gets or sets the temperature time series
        /// </summary>
        public Series TemperatureSeries { get; set; }

        /// <summary>
        /// Gets or sets the precipitation time series
        /// </summary>
        public Series PrecipitationSeries { get; set; }

        /// <summary>
        /// Gets or sets the list of suitable sites
        /// </summary>
        public IList<Site> SuitableSites { get; set; }

        /// <summary>
        /// Gets or sets the selected site
        /// </summary>
        public Site SelectedSite { get; set; }
    }

    /// <summary>
    /// Type of drought
    /// </summary>
    public enum TypeOfDrought
    {
        /// <summary>
        /// the type of drought Meteorological drought
        /// </summary>
        Meteorological,
        /// <summary>
        /// The type of drought is Hydrological drought
        /// </summary>
        Hydrological
    }
}
