using System;

namespace Search3.Searching
{
    public class CompletedEventArgs : EventArgs
    {
        public CompletedEventArgs(SearchResult result)
        {
            Result = result;
        }

        public SearchResult Result { get; private set; }
    }
}