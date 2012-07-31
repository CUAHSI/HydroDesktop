using System.Collections.Generic;
using System.Data;
using DotSpatial.Controls;
using HydroDesktop.Interfaces;

namespace DataImport.WaterML
{
    /// <summary>
    /// WaterML Import Settings
    /// </summary>
    public class WaterMLImportSettings : IWizardImporterSettings
    {
        public string PathToFile { get; set; }

        /// <summary>
        /// Theme name
        /// </summary>
        public string ThemeName { get; set; }

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

        /// <summary>
        /// List of columns data
        /// </summary>
        public IList<ColumnInfo> ColumnDatas
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        /// <summary>
        /// Column with date time
        /// </summary>
        public string DateTimeColumn
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        /// <summary>
        /// Data table with data to import
        /// </summary>
        public DataTable Data
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        /// <summary>
        /// Data table with preview data
        /// </summary>
        public DataTable Preview
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the string to use as a decimal separator when parsing values
        /// </summary>
        public string ValuesNumberDecimalSeparator
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}