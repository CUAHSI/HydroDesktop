using System;

namespace HydroDesktop.Common.UserMessage
{
    /// <summary>
    /// Shows custom message to user.
    /// </summary>
    public interface IUserMessage
    {
        /// <summary>
        /// Show info message.
        /// </summary>
        /// <param name="message">Message to show.</param>
        void Info(string message);

        /// <summary>
        /// Show warning message.
        /// </summary>
        /// <param name="message">Message to show.</param>
        void Warn(string message);

        /// <summary>
        /// Show error message.
        /// </summary>
        /// <param name="message">Message to show.</param>
        /// <param name="exception">Exception.</param>
        void Error(string message, Exception exception = null);
    }
}