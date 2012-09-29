using System;

namespace HydroDesktop.Common.Logging
{
    /// <summary>
    /// Logger
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Log info message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        void Info(string message);

        /// <summary>
        /// Log warning message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        void Warn(string message);

        /// <summary>
        /// Log error message.
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exception">Exception.</param>
        void Error(string message, Exception exception = null);

        /// <summary>
        /// Log Destination.
        /// </summary>
        string Destination { get; }
    }
}
