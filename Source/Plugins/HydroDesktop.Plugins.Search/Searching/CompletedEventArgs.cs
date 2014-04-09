using System;
using HydroDesktop.Common;

namespace HydroDesktop.Plugins.Search.Searching
{
    public class CompletedEventArgs : EventArgs
    {
        public CompletedEventArgs(SearchResult result, CompletedReasones reason, IProgressHandler progressHandler)
        {
            Result = result;
            Reason = reason;
            ProgressHandler = progressHandler;
        }

        /// <summary>
        /// Search result. May be null.
        /// </summary>
        public SearchResult Result { get; private set; }

        /// <summary>
        /// Reason of completed event
        /// </summary>
        public CompletedReasones Reason { get; private set; }

        /// <summary>
        /// Progress handler
        /// </summary>
        public IProgressHandler ProgressHandler { get; private set; }
    }

    public enum CompletedReasones
    {
        /// <summary>
        /// Search has been comleted normally
        /// </summary>
        NormalCompleted,
        /// <summary>
        /// Search has been cancelled
        /// </summary>
        Cancelled,
        /// <summary>
        /// Search has been comleted due to an unhandled exception
        /// </summary>
        Faulted,
    }
}