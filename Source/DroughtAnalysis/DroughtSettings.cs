using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.Interfaces.ObjectModel;

namespace DroughtAnalysis
{
    public class DroughtSettings
    {
        public TypeOfDrought DroughtType { get; set; }

        public string PathToR { get; set; }

        public string OutputDirectory { get; set; }

        public Series TemperatureSeries { get; set; }

        public Series PrecipitationSeries { get; set; }

        public IList<Site> SuitableSites { get; set; }

        public Site SelectedSite { get; set; }
    }

    public enum TypeOfDrought
    {
        Meteorological,
        Hydrological
    }
}
