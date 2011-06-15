using System;
using System.Runtime.Serialization;

namespace HydroDesktop.Search.Download.Exceptions
{
    public class DownloadXmlException : Exception
    {
        public DownloadXmlException()
            : this("Download Xml Exception")
        {

        }
        public DownloadXmlException(string message)
            : this(message, null)
        {

        }
        public DownloadXmlException(string message, Exception inner)
            : base(message, inner)
        {

        }

        protected DownloadXmlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
