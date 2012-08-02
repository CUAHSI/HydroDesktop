using System.Threading;

namespace HydroDesktop.Common
{
    /// <summary>
    /// Interface for progressing when long operations are executed
    /// </summary>
    public interface IProgressHandler
    {
        /// <summary>
        /// Report progress
        /// </summary>
        /// <param name="percentage">Percentage of progress</param>
        /// <param name="state">State of progress</param>
        void ReportProgress(int percentage, object state);

        /// <summary>
        /// Check for cancel
        /// </summary>
        void CheckForCancel();

        /// <summary>
        /// Report any custom message
        /// </summary>
        /// <param name="message">Message to report</param>
        void ReportMessage(string message);

        /// <summary>
        /// CancellationToken
        /// </summary>
        CancellationToken CancellationToken { get; }
    }
}