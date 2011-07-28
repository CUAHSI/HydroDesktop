using System;
using System.Runtime.Serialization;

namespace HydroDesktop.DataDownload.Download.Exceptions
{
    /// <summary>
    /// The exception that is thrown when converting data series from xml.
    /// </summary>
    class DataSeriesFromXmlException : Exception
    {
        private const string DEFAULT_MESSAGE = "Data Series From Xml Exception.";

        public DataSeriesFromXmlException()
            : this(DEFAULT_MESSAGE)
        {
            
        }
        public DataSeriesFromXmlException(Exception inner)
            : this(DEFAULT_MESSAGE, inner)
        {

        }
        public DataSeriesFromXmlException(string message)
            : this(message, null)
        {

        }
        public DataSeriesFromXmlException(string message, Exception inner)
            : base(message, inner)
        {

        }

        protected DataSeriesFromXmlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
        
    }
}
