using System;

namespace FacetedSearch.Searching
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