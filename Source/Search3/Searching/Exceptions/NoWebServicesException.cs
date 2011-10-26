using System;
using System.Runtime.Serialization;

namespace Search3.Searching.Exceptions
{
    public class NoWebServicesException : SearchSettingsException
    {
        public NoWebServicesException()
        {
            
        }
        public NoWebServicesException(Exception inner)
            : base(inner)
        {
        }

        public NoWebServicesException(string message)
            : base(message)
        {
        }

        public NoWebServicesException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected NoWebServicesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}