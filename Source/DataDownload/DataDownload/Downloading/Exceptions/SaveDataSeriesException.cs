using System;
using System.Runtime.Serialization;

namespace HydroDesktop.DataDownload.Downloading.Exceptions
{
    /// <summary>
    /// The exception that is thrown when saving data series into database.
    /// </summary>
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

        public SaveDataSeriesException(string message, Exception inner = null)
            : base(message, inner)
        {

        }

        protected SaveDataSeriesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
