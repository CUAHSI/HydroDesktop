using System;
using System.Runtime.Serialization;

namespace HydroDesktop.Search.Download.Exceptions
{
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
