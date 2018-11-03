﻿using System;
using System.Runtime.Serialization;

namespace HydroDesktop.Plugins.DataDownload.Downloading.Exceptions
{
    /// <summary>
    /// The exception that is thrown when downloading series from web services.
    /// </summary>
    class DownloadXmlException : Exception
    {
        private const string DEFAULT_MESSAGE = "Download Xml Exception.";

        public DownloadXmlException()
            : this(DEFAULT_MESSAGE)
        {

        }
        public DownloadXmlException(Exception inner)
            : this(DEFAULT_MESSAGE, inner)
        {

        }

        public DownloadXmlException(string message, Exception inner = null)
            : base(message, inner)
        {

        }

        protected DownloadXmlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
