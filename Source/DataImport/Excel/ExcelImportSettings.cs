using System.Collections.Generic;
using System.Data;
using DotSpatial.Controls;
using HydroDesktop.Interfaces;

namespace DataImport.Excel
{
    /// <summary>
    /// Settings for Excel
    /// </summary>
    public class ExcelImportSettings : IWizardImporterSettings
    {
        /// <summary>
        /// Selected sheet name
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// DataSet that contains all data from excel file
        /// </summary>
        public DataSet DataSet { get; set; }

        #region Implementation of IWizardImporterSettings

        public string PathToFile {get;set;}
        public IList<ColumnInfo> ColumnDatas { get; set; }
        public string DateTimeColumn { get; set; }
        public DataTable Preview { get; set; }
        public DataTable Data { get; set; }
        public string ValuesNumberDecimalSeparator { get; set; }
        public int MaxProgressPercentWhenImport { get; set; }
        public ISeriesSelector SeriesSelector { get; set; }
        public IMap Map { get; set; }

        #endregion
    }
}