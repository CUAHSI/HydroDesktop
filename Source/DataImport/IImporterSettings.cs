using System.Collections.Generic;
using System.Data;

namespace DataImport
{
    /// <summary>
    /// Settings for data importer
    /// </summary>
    public interface IImporterSettings
    {
        /// <summary>
        /// Path to File
        /// </summary>
        string PathToFile { get; set; }

        /// <summary>
        /// List of columns data
        /// </summary>
        IList<ColumnInfo> ColumnDatas { get; set; }

        /// <summary>
        /// Column with date time
        /// </summary>
        string DateTimeColumn { get; set; }

        /// <summary>
        /// Data table with data to import
        /// </summary>
        DataTable Data { get; set; }

        /// <summary>
        /// Get or sets the string to use as a decimal separator when parsing values
        /// </summary>
        string ValuesNumberDecimalSeparator { get; set; }
    }
}