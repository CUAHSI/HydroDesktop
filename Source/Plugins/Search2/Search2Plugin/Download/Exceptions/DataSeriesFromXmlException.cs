using System;
using System.Runtime.Serialization;

namespace HydroDesktop.Search.Download.Exceptions
{
    class DataSeriesFromXmlException : Exception
    {
         public DataSeriesFromXmlException()
            : this("Data Series From Xml Exception")
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
