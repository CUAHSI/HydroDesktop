using System;
using System.Runtime.Serialization;

namespace HydroDesktop.DataDownload.Download.Exceptions
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
        public NoSeriesFromXmlException(string message)
            : this(message, null)
        {

        }
        public NoSeriesFromXmlException(string message, Exception inner)
            : base(message, inner)
        {

        }

        protected NoSeriesFromXmlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
