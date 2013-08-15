using System;

namespace HydroDesktop.ErrorReporting
{
    /// <summary>
    /// Init params for <see cref="ErrorReportingForm"/>
    /// </summary>
    public class ErrorReportingFormArgs
    {
        /// <summary>
        /// Gets or sets Exception to show.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets path to log file.
        /// </summary>
        public string LogFile { get; set; }

        /// <summary>
        /// Gets or sets boolean, that indicates that application will be closed.
        /// </summary>
        public bool IsFatal { get; set; }
    }
}