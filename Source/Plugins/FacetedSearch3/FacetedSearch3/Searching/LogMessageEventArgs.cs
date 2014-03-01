using System;

namespace FacetedSearch3.Searching
{
    public class LogMessageEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public Exception Exception { get; private set; }

        public LogMessageEventArgs(string message, Exception exception = null)
        {
            Message = message;
            Exception = exception;
        }
    }
}