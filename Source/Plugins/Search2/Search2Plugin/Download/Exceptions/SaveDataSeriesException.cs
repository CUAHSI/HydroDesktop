using System;
using System.Runtime.Serialization;

namespace HydroDesktop.Search.Download.Exceptions
{
    class SaveDataSeriesException : Exception
    {
        private const string DEFAULT_MESSAGE = "Save Data Series Exception.";

        public SaveDataSeriesException()
            : this(DEFAULT_MESSAGE)
        {

        }
        public SaveDataSeriesException(Exception inner)
            : this(DEFAULT_MESSAGE, inner)
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
