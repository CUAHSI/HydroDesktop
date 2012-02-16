using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;

namespace HydroDesktop.ImportExport
{
    /// <summary>
    /// Specifies the format options for DelimitedTextWriter
    /// </summary>
    public class DelimitedFormatOptions
    {
        /// <summary>
        /// Creates a new instance of delimited format options with default settings
        /// </summary>
        public DelimitedFormatOptions()
        {
            Delimiter = ",";
            IncludeHeaders = true;
            DateTimeFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            UseShortDateFormat = false;
        }

        /// <summary>
        /// The delimiter to be used to separate data items in a given data row.
        /// Typically this is comma (","), semicolon (";"), space or tab
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// True if the column names from the data table should be included as headers in the output streamTrue if column headers should be included, false otherwise
        /// </summary>
        public bool IncludeHeaders { get; set; }

        /// <summary>
        /// The culture specific date/time format
        /// </summary>
        public DateTimeFormatInfo DateTimeFormat { get; set; }

        /// <summary>
        /// If true, then the short date format is used
        /// </summary>
        public bool UseShortDateFormat { get; set; }

        /// <summary>
        /// Determines whether data is to be appended to the file. If the file exists and append is false, the file is overwritten. If the file exists and append is true, the data is appended to the file. Otherwise, a new file is created.
        /// </summary>
        public bool Append { get; set; }

        /// <summary>
        /// True if the culture should be set to InvariantCulture (decimal points will always be saved as ".")
        /// </summary>
        public bool UseInvariantCulture { get; set; }
    }
}
