using System;

namespace Search3.Searching
{
    public class CompletedEventArgs : EventArgs
    {
        public CompletedEventArgs(SearchResult result)
        {
            Result = result;
        }

        /// <summary>
        /// Search result. May be null.
        /// </summary>
        public SearchResult Result { get; private set; }
    }
}