using System.Collections.Generic;
using System.Data;
using DotSpatial.Controls;
using HydroDesktop.Interfaces;

namespace DataImport.Csv
{
    /// <summary>
    /// Setting for csv
    /// </summary>
    public class CsvImportSettings : IWizardImporterSettings
    {
        #region Implementation of IWizardImporterSettings

        public string PathToFile{get;set;}
        public DataTable Preview { get; set; }
        public DataTable Data { get; set; }
        public string ValuesNumberDecimalSeparator { get; set; }
        public IList<ColumnInfo> ColumnDatas { get; set; }
        public string DateTimeColumn { get; set; }
        public int MaxProgressPercentWhenImport { get; set; }
        public ISeriesSelector SeriesSelector { get; set; }
        public Map Map { get; set; }
        public string LayerName { get; set; }
    
        #endregion
    }
}