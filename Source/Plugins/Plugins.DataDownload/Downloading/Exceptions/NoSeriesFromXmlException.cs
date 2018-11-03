using System;
using System.Runtime.Serialization;

namespace HydroDesktop.Plugins.DataDownload.Downloading.Exceptions
{
    /// <summary>
    /// The exception that is thrown when no series found in downloaded xml file.
    /// </summary>
    class NoSeriesFromXmlException : Exception
    {
        private const string DEFAULT_MESSAGE = "No series found in xml file.";

        public NoSeriesFromXmlException()
            : this(DEFAULT_MESSAGE)
        {

        }
        public NoSeriesFromXmlException(Exception inner)
            : this(DEFAULT_MESSAGE, inner)
        {

        }

        public NoSeriesFromXmlException(string message, Exception inner = null)
            : base(message, inner)
        {

        }

        protected NoSeriesFromXmlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
