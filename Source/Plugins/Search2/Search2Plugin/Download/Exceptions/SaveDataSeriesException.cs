using System;
using System.Runtime.Serialization;

namespace HydroDesktop.Search.Download.Exceptions
{
    class SaveDataSeriesException : Exception
    {
         public SaveDataSeriesException()
            : this("Save Data Series Exception")
        {

        }
        public SaveDataSeriesException(string message)
            : this(message, null)
        {

        }
        public SaveDataSeriesException(string message, Exception inner)
            : base(message, inner)
        {

        }

        protected SaveDataSeriesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
