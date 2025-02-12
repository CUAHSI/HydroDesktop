using System.Collections.Generic;
using System.Data;
using DotSpatial.Controls;
using HydroDesktop.Interfaces;

namespace HydroDesktop.Plugins.DataImport.Excel
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

        /// <summary>
        /// Path to File
        /// </summary>
        public string PathToFile {get;set;}

        /// <summary>
        /// List of columns data
        /// </summary>
        public IList<ColumnInfo> ColumnDatas { get; set; }

        /// <summary>
        /// Column with date time
        /// </summary>
        public string DateTimeColumn { get; set; }

        /// <summary>
        /// Data table with preview data
        /// </summary>
        public DataTable Preview { get; set; }

        /// <summary>
        /// Data table with data to import
        /// </summary>
        public DataTable Data { get; set; }

        /// <summary>
        /// Gets or sets the string to use as a decimal separator when parsing values
        /// </summary>
        public string ValuesNumberDecimalSeparator { get; set; }

        /// <summary>
        /// Gets or sets max progress percent when importing
        /// </summary>
        public int MaxProgressPercentWhenImport { get; set; }

        /// <summary>
        /// Gets or sets SeriesSelector
        /// </summary>
        public ISeriesSelector SeriesSelector { get; set; }

        /// <summary>
        /// Gets or sets Map
        /// </summary>
        public Map Map { get; set; }

        /// <summary>
        /// Gets or sets name of layer to import data
        /// </summary>
        public string LayerName { get; set; }

        #endregion
    }
}